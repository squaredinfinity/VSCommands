using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace SquaredInfinity.VSCommands.Foundation
{
    public static class StaticSettings
    {
        /// <summary>
        /// e.g. 0.0.1 (full number would be 11.0.0.1 for VS11 and 10.0.0.1 for VS10 etc.)
        /// </summary>
        public const string CommonCurrentReleaseNumber = CurrentVersionStrings.CommonCurrentReleaseNumber;

        /// <summary>
        /// e.g. 11.1.0.0 for VSC11 or 10.1.0.0 for VSC10
        /// </summary>
        public static readonly string CurrentReleaseNumber;

        public static readonly Version CurrentVersion;



        // NOTE: this must be const so it can be used in GuidAttribute on VscPackage
#if VS11
        public const string PackageGuidString = "a0e6a221-92fd-4d72-be80-04a36b591fcb";
#elif VS12
        public const string PackageGuidString = "a99761e2-42ea-40bd-992f-4b84ccaeaf05";
#elif VS14
        public const string PackageGuidString = "3d446624-3e4b-473a-8463-ee52e1f5d7ab";
#else
        something is wrong !
#endif


        public static readonly string CommandsGuidString;

        /// <summary>
        /// Returns name of Visual Studio folder name in My Documents (e.g. Visual Studio 2010, Visual Studio 2012 (was Visual Studio 11 in RC))
        /// </summary>
        public static readonly string VisualStudioMyDocumentsFolderName;

        /// <summary>
        /// Returns name of Visual Studio folder name in My Documents (e.g. Visual Studio 2010, Visual Studio 2012 (was Visual Studio 11 in RC))
        /// </summary>
        public static string VisualStudioRCMyDocumentsFolderName;

        /// <summary>
        /// e.g. 11, 12, 14 etc.
        /// </summary>
        public static readonly string ShortVersionNumber = "12";

        /// <summary>
        /// e.g. 11.0, 12.0, 14.0
        /// </summary>
        public static readonly string VisualStudioVersionNumber = "12.0";

        /// <summary>
        /// e.g. Visual Studio 2012
        /// </summary>
        public static readonly string VisualStudioVersionName = "Visual Studio 2013";
        
        /// <summary>
        /// e.g. 11.0 or 11.0Exp
        /// </summary>
        public static readonly string VisualStudioHiveVersionNumber;

        public static readonly string VisualStudioExeFullPath;


        /// <summary>
        /// e.g C:\Users\jarek.kardas\AppData\Local\VSC 11\
        /// </summary>
        public static readonly DirectoryInfo AppDataDirectory;

        /// <summary>
        /// e.g C:\Users\jarek.kardas\AppData\Local\VSC 11\Shared
        /// </summary>
        public static readonly DirectoryInfo SharedAssembliesDirectory;

        /// <summary>
        /// e.g C:\Users\jarek.kardas\AppData\Local\VSC 11\Cache
        /// </summary>
        public static readonly DirectoryInfo CacheDirectory;

        /// <summary>
        /// e.g C:\Users\jarek.kardas\AppData\Local\VSC 11\Shared\Win32ResourcesManagedWrapper.dll
        /// </summary>
        public static readonly FileInfo Win32ResourcesManagedWrapperFile;

        public static readonly string VSCommandsRegistryKeyName;

        public static readonly DirectoryInfo VSCommandsAssembliesDirectory;

        public static string VSCommandsVSIXAssemblyName;

        static Assembly _vscommandsVsixAssembly;
        public static Assembly VSCommandsVSIXAssembly
        {
            get { return _vscommandsVsixAssembly; }
            set
            {
                _vscommandsVsixAssembly = value;
                VSCommandsVSIXAssemblyName = value.GetName().Name;
            }
        }

        static StaticSettings()
        {
#if VS11
            VisualStudioMyDocumentsFolderName = "Visual Studio 2012";
            VisualStudioRCMyDocumentsFolderName = "Visual Studio 11";
            ShortVersionNumber = "11";
            VisualStudioVersionNumber = "11.0";
            VisualStudioVersionName = "Visual Studio 2012";
#elif VS12
            VisualStudioMyDocumentsFolderName = "Visual Studio 2013";
            VisualStudioRCMyDocumentsFolderName = "Visual Studio 2013";
            ShortVersionNumber = "12";
            VisualStudioVersionNumber = "12.0";
            VisualStudioVersionName = "Visual Studio 2013";
#elif VS14
            VisualStudioMyDocumentsFolderName = "Visual Studio 2015";
            VisualStudioRCMyDocumentsFolderName = "Visual Studio 2015";
            ShortVersionNumber = "14";
            VisualStudioVersionNumber = "14.0";
            VisualStudioVersionName = "Visual Studio 2015";
#else
            throw new NotImplementedException();
#endif
            CurrentReleaseNumber = ShortVersionNumber + "." + CommonCurrentReleaseNumber;
            CurrentVersion = new Version(CurrentReleaseNumber);

            CommandsGuidString = "{00000000-0000-0000-0000-313370000001}";
            VSCommandsRegistryKeyName = @"Software\squaredinfinity\vscommands";
            VisualStudioExeFullPath = System.Environment.GetCommandLineArgs()[0];
            AppDataDirectory = 
                new DirectoryInfo(
                    Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Squared Infinity",
                        "VSCommands " + ShortVersionNumber));

            SharedAssembliesDirectory =
                new DirectoryInfo(
                    Path.Combine(
                        AppDataDirectory.FullName, 
                        "Shared Assemblies"));

            CacheDirectory =
                new DirectoryInfo(
                    Path.Combine(
                        AppDataDirectory.FullName, 
                        "Cache"));

            Win32ResourcesManagedWrapperFile =
                new FileInfo(
                    Path.Combine(
                        SharedAssembliesDirectory.FullName, 
                        "VSCommands.Win32Resources.ManagedWrapper.dll"));
#if DEBUG
            VisualStudioHiveVersionNumber = VisualStudioVersionNumber + "Exp";
#else
            VisualStudioHiveVersionNumber = VisualStudioVersionNumber;
#endif

            var vsc_asm = Assembly.GetExecutingAssembly();
            VSCommandsAssembliesDirectory = new FileInfo(vsc_asm.Location).Directory;
        }

        public static string GetReleaseNotesUrl(Version version)
        {
            return "http://vscommands.squaredinfinity.com/release-notes/{0}.{1}.{2}?src=vsc{3}"
                .FormatWith(version.Minor, version.Build, version.Revision, ShortVersionNumber);
        }
    }
}
