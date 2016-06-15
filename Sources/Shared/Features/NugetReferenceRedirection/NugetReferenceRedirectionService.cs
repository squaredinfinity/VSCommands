using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    public class NugetReferenceRedirectionService
    {
        /// <summary>
        /// Returns information about all packages used by specified solution.
        /// </summary>
        /// <param name="solutionDirectory">solution directory</param>
        /// <returns></returns>
        public IReadOnlyList<NugetPackage> GetAllUsedPackages(
            DirectoryInfo solutionDirectory)
        {
            return GetAllUsedPackages(solutionDirectory, new DirectoryInfo(Path.Combine(solutionDirectory.FullName, "packages")));
        }

        /// <summary>
        /// Returns information about all packages used by specified solution.
        /// </summary>
        /// <param name="solutionDirectory">solution directory</param>
        /// <param name="packagesDirectory">directory where packages are located</param>
        /// <returns></returns>
        public IReadOnlyList<NugetPackage> GetAllUsedPackages(
            DirectoryInfo solutionDirectory,
            DirectoryInfo packagesDirectory)
        {
            // Assumption:  all referenced projects are under solution directory
            //              it would be better to iterate all loaded projects using DTE, but it also would be slower
            //              this may need to be changed in the future though

            //# Find all packages.config files
            var all_packages_configs =
                solutionDirectory.GetFiles("packages.config", SearchOption.AllDirectories);

            //# Get distinct combinations of package ID, Version and TargetFramework
            //  This is going to form a list of all known used packages

            #region Read Individual Packages

            var distinct_packages_keys = new ConcurrentBag<Package__Id_Version_Target>();

            // todo: this can be done in parallel
            foreach (var pc in all_packages_configs)
            {
                try
                {
                    if (!pc.Exists)
                        continue;

                    var xml_sting = File.ReadAllText(pc.FullName);

                    var xDoc = XDocument.Parse(xml_sting);

                    foreach (var package in xDoc.Descendants(XName.Get("package")))
                    {
                        var key = new Package__Id_Version_Target
                        {
                            Id = package.Attribute("id").Value,
                            Version = package.Attribute("version").Value,
                            Target = package.Attribute("targetFramework").Value
                        };

                        distinct_packages_keys.Add(key);
                    }
                }
                catch (Exception ex)
                {
                    // todo: logging
                }
            }

            #endregion

            //# Process results

            var all_distinct_keys = new List<Package__Id_Version_Target>(capacity: distinct_packages_keys.Count);

            //! empty concurrent bag while iterating so it doesn't stay pinned to current thread
            var package_key = default(Package__Id_Version_Target);
            while (distinct_packages_keys.TryTake(out package_key))
                all_distinct_keys.Add(package_key);

            #region Process each individual package

            var result = new List<NugetPackage>();

            var packages_by_id =
                all_distinct_keys.GroupBy(x => x.Id);

            // process each package id
            foreach (var id_group in packages_by_id)
            {
                var package = new NugetPackage();
                package.Id = id_group.Key;

                var targets_by_version =
                    id_group.GroupBy(x => x.Version);

                var all_versions = new List<NugetPackageVersion>();

                // process each version / target combination in use
                foreach (var pack_ver in targets_by_version)
                {
                    var ver = new NugetPackageVersion();
                    ver.Version = pack_ver.Key;

                    var package_dir_name = $"{package.Id}.{ver.Version}";

                    var package_dir_full_path = Path.Combine(packagesDirectory.FullName, package_dir_name);

                    if (Directory.Exists(package_dir_full_path))
                    {
                        // todo: logging
                        // skip this version
                        continue;
                    }
                    else
                    {
                        ver.DirectoryFullPath = package_dir_full_path;
                    }

                    ver.Targets = pack_ver.Select(x => x.Target).ToArray();

                    all_versions.Add(ver);
                }

                package.Versions = all_versions;

                result.Add(package);
            }

            #endregion

            return result;
        }
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


//}
