using SquaredInfinity.Foundation;
using System.Linq;
using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE80;
using SquaredInfinity.VSCommands.Foundation;
using VSLangProj;
using SquaredInfinity.VSCommands.Foundation.Nuget;
using NuGet.Packaging.Core;
using System.Diagnostics;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;

namespace SquaredInfinity.VSCommands.Features.ReferencesSwitch
{
    public class ReferencesSwitchService
    {
        IVscSettingsService SettingsService { get; set; }
        DTE2 Dte2 { get; set; }
        IVisualStudioService VisualStudioService { get; set; }
        INugetService NugetService { get; set; }

        public ReferencesSwitchService(
            IVscSettingsService settingsService, 
            IServiceProvider serviceProvider,
            IVisualStudioEventsService vsEventsService,
            IVisualStudioService visualStudioService,
            INugetService nugetService)
        {
            this.SettingsService = settingsService;
            this.Dte2 = serviceProvider.GetDte2();
            this.VisualStudioService = visualStudioService;
            this.NugetService = nugetService;
        }

        public MultiMap<EnvDTE.Project, SwitchableReference> GetSwitchableReferencesByProjectInSolution()
        {
            var result = new MultiMap<EnvDTE.Project, SwitchableReference>();

            //# save all
            if (Dte2.Solution.IsDirty)
            {
                VisualStudioService.SaveAllChanges();
            }

            // iterate all projects in solution
            foreach (var project in Dte2.Solution.Projects.ProjectsTreeTraversal())
            {
                // iterate all references and see if they are assembly/project/nuget references
                var vsProject = (VSProject)null;

                if (!project.TryGetVSProject(out vsProject))
                    continue;
                
                for(int i = 0; i < vsProject.References.Count; i++)
                {
                    var reference = vsProject.References.Item(i + 1); //! '1' based index
                    
                    if(reference.SourceProject != null)
                    {
                        // this is a project reference

                        var source_project = reference.SourceProject;

                        var project_ref = new SwitchableProjectReference()
                        {
                            Name = reference.Name,
                            ReferencedProjectFullPath = source_project.FullName
                        };

                        result[project].Add(project_ref);
                        continue;
                    }
                    
                    if(reference.Type == prjReferenceType.prjReferenceTypeAssembly)
                    {
                        // this is either assembly or nuget reference

                        var asm_ref = (SwitchableAssemblyReference)null;

                        if(IsNugetReference(reference))
                        {
                            var package_ref = new SwitchableNugetReference();
                            asm_ref = package_ref;

                            var package_id = NugetService.GetPackageIdentityForFile(reference.Path);

                            if(package_id == null)
                            {
                                // todo: log
                                continue;
                            }

                            package_ref.PackageId = package_id.Id;
                            package_ref.PackageVersion = package_id.Version.ToString(valueWhenNull: "");
                        }
                        else
                        {
                            // todo: for now don't handle direct assembly references
                            continue;

                            asm_ref = new SwitchableAssemblyReference();
                        }

                        asm_ref.Name = reference.Name;
                        asm_ref.FullPath = reference.Path;

                        result.Add(project, asm_ref);

                        continue;

                    }
                }
            }

            return result;
        }

        bool IsNugetReference(Reference reference)
        {
            return reference.Path.Contains("packages", StringComparison.CurrentCultureIgnoreCase);
        }

        public void SwitchReferences(IReadOnlyList<ReferenceSwitchRequest> switchRequests)
        {
            // save solution and all files

            foreach(var sr in switchRequests)
            {
                SwitchReference(sr.From, sr.To);
            }

            // save solution and all files
        }

        void SwitchReference(Reference oldReference, Reference newReference)
        {

        }

        public Dictionary<string, FileInfo> DiscoverProjectsPaths(IReadOnlyList<string> projectNames, DirectoryInfo seachRootDirectory)
        {
            var result = new Dictionary<string, FileInfo>(capacity: projectNames.Count);

            // enumerate project files under the root, looking in subdirectories
            foreach(var file in seachRootDirectory.EnumerateFiles("*.csproj", SearchOption.AllDirectories))
            {
                if(projectNames.Contains(file.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    result[file.Name] = file;
                }
            }

            return result;
        }
    }

    public class ReferenceSwitchRequest
    {
        string ProjectUniqueId { get; set; }

        public Reference From { get; set; }
        public Reference To { get; set; }
    }
    
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class SwitchableReference : NotifyPropertyChangedObject
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }

        public string DebuggerDisplay
        {
            get { return Name.ToString(valueWhenNull: "[NULL]"); }
        }
    }

    public class SwitchableProjectReference : SwitchableReference
    {
        string _referencedProjectFullPath;
        public string ReferencedProjectFullPath
        {
            get { return _referencedProjectFullPath; }
            set { TrySetThisPropertyValue(ref _referencedProjectFullPath, value); }
        }
    }

    public class SwitchableAssemblyReference : SwitchableReference
    {
        string _fullPath;
        public string FullPath
        {
            get { return _fullPath; }
            set { TrySetThisPropertyValue(ref _fullPath, value); }
        }
    }

    public class SwitchableNugetReference : SwitchableAssemblyReference
    {
        //! This type may be persisted, so it's important that no properties with 3rd party types are exposed (in case the type definitions change)
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
    }
}



//public static class kernel32
//{
//    [DllImport("kernel32.dll")]
//    public static extern IntPtr GetCurrentProcess();

//    [DllImport("kernel32.dll", SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.I1)]
//    public static extern bool CreateSymbolicLink(string symlinkFileName, string targetFileName, SymbolicLinkType linkType);

//    [DllImport("kernel32.dll")]
//    public static extern IntPtr OpenProcess(
//        ProcessAccessFlags dwDesiredAccess,
//        [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
//        int dwProcessId);

//    [Flags]
//    public enum ProcessAccessFlags : uint
//    {
//        All = 0x001F0FFF | DELETE | READ_CONTROL | WRITE_OWNER | SYNCHRONIZE | WRITE_DAC,
//        Terminate = 0x00000001,
//        CreateThread = 0x00000002,
//        VMOperation = 0x00000008,
//        VMRead = 0x00000010,
//        VMWrite = 0x00000020,
//        DupHandle = 0x00000040,
//        SetInformation = 0x00000200,
//        QueryInformation = 0x00000400,
//        Synchronize = 0x00100000,

//        // standard access rights used by all objects

//        /// <summary>
//        /// Required to delete the object. 
//        /// </summary>
//        DELETE = 0x00010000,
//        /// <summary>
//        ///  Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right. 
//        /// </summary>
//        READ_CONTROL = 0x00020000,
//        /// <summary>
//        /// The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state. 
//        /// </summary>
//        SYNCHRONIZE = 0x00100000,
//        /// <summary>
//        ///  Required to modify the DACL in the security descriptor for the object. 
//        /// </summary>
//        WRITE_DAC = 0x00040000,

//        WRITE_OWNER = 0x00080000

//    }

//    public enum SymbolicLinkType : UInt32
//    {
//        File = 0,
//        Directory = 1
//    }
//}

//public static class advapi32
//{
//    public const int SE_PRIVILEGE_DISABLED = 0x00000001;
//    public const int SE_PRIVILEGE_ENABLED = 0x00000002;


//    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    public static extern bool LookupPrivilegeValue(
//        string lpSystemName,
//        string lpName,
//        out LUID lpLuid);

//    [DllImport("advapi32.dll", SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    public static extern bool AdjustTokenPrivileges(
//        IntPtr TokenHandle,
//        [MarshalAs(UnmanagedType.Bool)]bool DisableAllPrivileges,
//        ref TOKEN_PRIVILEGE NewState,
//        int Zero,
//        ref TOKEN_PRIVILEGE Null1,
//        ref int Null2);

//    [StructLayout(LayoutKind.Sequential)]
//    public struct LUID
//    {
//        public uint LowPart;
//        public int HighPart;
//    }

//    [StructLayout(LayoutKind.Sequential)]
//    public struct TOKEN_PRIVILEGE
//    {
//        public int PrivilegeCount;
//        public LUID_AND_ATTRIBUTES Privilege;
//    };

//    [StructLayout(LayoutKind.Sequential)]
//    public struct LUID_AND_ATTRIBUTES
//    {
//        public LUID Luid;
//        public int Attributes;
//    };

//    public static LUID GetLuid(string privilegeName)
//    {
//        LUID privilegeLuid = new LUID();

//        if (!advapi32.LookupPrivilegeValue(string.Empty, privilegeName, out privilegeLuid))
//        {
//            throw new Win32Exception(Marshal.GetLastWin32Error());
//        }

//        return privilegeLuid;
//    }

//    public static void AdjustTokenPrivilege(IntPtr hToken, string privilegeName, bool enable)
//    {
//        advapi32.LUID privilegeLuid = advapi32.GetLuid(privilegeName);

//        var luidAndAttributes = new advapi32.LUID_AND_ATTRIBUTES();
//        luidAndAttributes.Luid = privilegeLuid;
//        luidAndAttributes.Attributes = enable ? advapi32.SE_PRIVILEGE_ENABLED : advapi32.SE_PRIVILEGE_DISABLED;

//        advapi32.TOKEN_PRIVILEGE tokenPrivileges = new advapi32.TOKEN_PRIVILEGE();
//        tokenPrivileges.PrivilegeCount = 1;
//        tokenPrivileges.Privilege = luidAndAttributes;

//        TOKEN_PRIVILEGE previousState = new TOKEN_PRIVILEGE();
//        int resultLength = 0;

//        if (!advapi32.AdjustTokenPrivileges(
//            hToken,
//            false,
//            ref tokenPrivileges,
//            Marshal.SizeOf(previousState),
//            ref previousState,
//            ref resultLength))
//        {
//            throw new Win32Exception(Marshal.GetLastWin32Error());
//        }
//    }

//    [DllImport("advapi32.dll", SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    public static extern bool OpenProcessToken(
//        IntPtr processHandle,
//        TokenAccessLevels desiredAccess,
//        out IntPtr tokenHandle);
//}

//class Program
//{
//    static void Main(string[] args)
//    {
//        try
//        {
//            var process_handle =
//                kernel32.GetCurrentProcess();

//            //+ get process token
//            IntPtr hProcessToken = IntPtr.Zero;
//            if (!advapi32.OpenProcessToken(process_handle, TokenAccessLevels.AdjustPrivileges | TokenAccessLevels.Query, out hProcessToken))
//            {
//                throw new Win32Exception(Marshal.GetLastWin32Error());
//            }

//            advapi32.AdjustTokenPrivilege(hProcessToken, "SeCreateSymbolicLinkPrivilege", enable: true);

//            // create note.txt

//            // if (!File.Exists("note.txt"))
//            //    File.WriteAllText("note.txt", "ORIGINAL");

//            //// create note.txt.new

//            //if (!File.Exists("note.txt.new"))
//            //    File.WriteAllText("note.txt.new", "NEW");

//            //// rename note.txt to note.txt.original
//            //File.Move("note.txt", "note.txt.original");

//            if (File.Exists(@"C:\Users\jarek\Documents\visual studio 2015\Projects\SymbolicLinkTest\testlib.dll"))
//            {
//                File.Delete(@"C:\Users\jarek\Documents\visual studio 2015\Projects\SymbolicLinkTest\testlib.dll");

//                Thread.Sleep(100);
//            }



//            // create symbolic link note.txt => note.txt.new
//            if (!kernel32.CreateSymbolicLink(
//                @"C:\Users\jarek\Documents\visual studio 2015\Projects\SymbolicLinkTest\testlib.dll",
//                @"C:\Users\jarek\Documents\visual studio 2015\Projects\SymbolicLinkTest\testlib.old.dll",
//                kernel32.SymbolicLinkType.File))
//            {
//                var x = Marshal.GetLastWin32Error();
//                Console.WriteLine(x);
//            }

//            // did it work?

//            testlib.myclass.Shout();

//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("ERROR");
//            Console.WriteLine(ex.ToString());
//        }

//        var targets = GetAvailableTargetFrameworks(new DirectoryInfo(@"C:\Users\jarek\Source\Repos\Foundation\Foundation\packages"));

//        foreach (var t in targets)
//            Console.WriteLine(t);

//        Console.ReadLine();
//    }

//    public class PackageInfo
//    {
//        public string Name { get; set; }
//        public string Version { get; set; }
//        IReadOnlyList<string> AvailableTargets { get; set; }
//    }

//    public static IReadOnlyList<PackageInfo> GetInstalledPackages(DirectoryInfo packages)
//    {
//        foreach (var packageDir in packages.GetDirectories())
//        }

//    public static IReadOnlyList<string> GetAvailableTargetFrameworks(DirectoryInfo packages)
//    {
//        HashSet<string> unique_targets = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

//        // look inside each lib directory inside each individual package driectory to find all distinct available targets
//        foreach (var packageDir in packages.GetDirectories())
//        {
//            // look in lib directory
//            var lib_dir = new DirectoryInfo(Path.Combine(packageDir.FullName, "lib"));
//            if (!lib_dir.Exists)
//                continue;

//            foreach (var dir in lib_dir.GetDirectories())
//            {
//                unique_targets.Add(dir.Name);
//            }
//        }

//        return unique_targets.ToArray();
//    }
//}