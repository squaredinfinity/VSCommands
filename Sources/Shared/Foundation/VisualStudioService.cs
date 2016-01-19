using EnvDTE;
using EnvDTE80;
using Microsoft.Win32;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace SquaredInfinity.VSCommands.Foundation
{
    public class VisualStudioService : IVisualStudioService
    {
        DTE2 Dte2 { get; set; }

        public VisualStudioService(IServiceProvider serviceProvider)
        {
            this.Dte2 = serviceProvider.GetDte2();
        }

        public void RestartVisualStudio(bool forceAsAdmin, bool saveAllChanges, bool asDifferentUser)
        {
            throw new NotSupportedException();

            //var thisProcess = System.Diagnostics.Process.GetCurrentProcess();

            //if (saveAllChanges)
            //{
            //    SaveAllChanges();
            //}

            //var visualStudioFullPath = thisProcess.MainModule.FileName;
            //var arguments = "";

            //var solution = Dte2.Solution;

            //if (solution != null && !solution.FullName.IsNullOrEmpty())
            //{
            //    arguments = @"""{0}""".FormatWith(solution.FullName);
            //}

            //var argVSPath = Environment.GetCommandLineArgs().First();

            //// if VS was started by clicking .sln file, vs path may appear twice as an argument (VS bug?)
            //var argsWithoutVSPath =
            //    from arg in Environment.GetCommandLineArgs()
            //    where arg != argVSPath
            //    select arg;

            //var args =
            //    string.Join(" ",
            //    argsWithoutVSPath
            //    .Select(s =>
            //    {
            //        if (s.Contains(" "))
            //            return "\"{0}\"".FormatWith(s);
            //        else return s;
            //    }));

            //arguments = arguments + " " + args;

            //ProcessStartInfo psi = new ProcessStartInfo();
            //psi.FileName = visualStudioFullPath;
            //psi.UseShellExecute = true;
            //psi.ErrorDialog = true;

            //psi.Arguments = arguments;

            ////+ Force As Admin
            //if (forceAsAdmin)
            //{
            //    psi.Verb = "runas";
            //}

            ////+ As Different User
            //if (asDifferentUser)
            //{
            //    string userName = string.Empty;
            //    string domainName = string.Empty;
            //    string password = string.Empty;

            //    var imgSource = Resources.LoadImageFromAssembly(StaticSettings.VSCommandsAssemblyName, @"UI/Resources/Images/run-as-different-user-banner.png");

            //    if (credui.PromptForCredentials(
            //        "Run as Different User",
            //        "Please enter credentials to use for Visual Studio",
            //        "vscommands.restartvisualstudio",
            //        imgSource.ToBitmap(),
            //        out userName,
            //        out domainName,
            //        out password)
            //        || userName.IsNullOrEmpty())
            //    {
            //        psi.UserName = userName;
            //        psi.Domain = domainName;
            //        psi.Password = new System.Security.SecureString();

            //        foreach (var c in password.ToString().ToCharArray())
            //            psi.Password.AppendChar(c);
            //    }
            //    else
            //    {
            //        Logger.TraceInformation(() => "Restart Visual Studio canceled by the user.");
            //        return;
            //    }
            //}

            ////+ With Medium Integrity Level
            //if (withMediumIntegrityLevel)
            //{
            //    var process =
            //        kernel32.OpenProcess(
            //        kernel32.ProcessAccessFlags.All,
            //        bInheritHandle: false,
            //        dwProcessId: System.Diagnostics.Process.GetCurrentProcess().Id);

            //    //+ get process token
            //    IntPtr hProcessToken = IntPtr.Zero;
            //    if (!advapi32.OpenProcessToken(process, TokenAccessLevels.MaximumAllowed, out hProcessToken))
            //    {
            //        throw new Win32Exception(Marshal.GetLastWin32Error());
            //    }

            //    //+ duplicate process token
            //    advapi32.SECURITY_ATTRIBUTES sa = new advapi32.SECURITY_ATTRIBUTES();
            //    IntPtr hDuplicatedToken = IntPtr.Zero;
            //    if (!advapi32.DuplicateTokenEx(hProcessToken, TokenAccessLevels.MaximumAllowed,
            //        ref sa, advapi32.SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, advapi32.TOKEN_TYPE.TokenPrimary, out hDuplicatedToken))
            //    {
            //        throw new Win32Exception(Marshal.GetLastWin32Error());
            //    }

            //    //+ create sid for medium integrity level
            //    IntPtr hMediumIntegritySID = IntPtr.Zero;
            //    if (!advapi32.ConvertStringSidToSid("S-1-16-8192", out hMediumIntegritySID))
            //    {
            //        throw new Win32Exception(Marshal.GetLastWin32Error());
            //    }

            //    //+ prepare new token information
            //    advapi32.TOKEN_MANDATORY_LABEL newTokenInformation = new advapi32.TOKEN_MANDATORY_LABEL();
            //    newTokenInformation.Label.Attributes = advapi32.SE_GROUP_INTEGRITY;
            //    newTokenInformation.Label.Sid = hMediumIntegritySID;

            //    //+ set medium integrity on token
            //    if (!advapi32.SetTokenInformation(hDuplicatedToken,
            //        advapi32.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel,
            //        ref newTokenInformation, (uint)Marshal.SizeOf(typeof(advapi32.TOKEN_MANDATORY_LABEL)) + advapi32.GetLengthSid(hMediumIntegritySID)))
            //    {
            //        throw new Win32Exception(Marshal.GetLastWin32Error());
            //    }

            //    advapi32.STARTUPINFO si = new advapi32.STARTUPINFO();
            //    advapi32.PROCESS_INFORMATION pi = new advapi32.PROCESS_INFORMATION();

            //    if (!advapi32.CreateProcessAsUser(
            //        hDuplicatedToken,
            //        visualStudioFullPath,
            //        arguments,
            //        ref sa,
            //        ref sa,
            //        false,
            //        0,
            //        IntPtr.Zero,
            //        null,
            //        ref si,
            //        ref pi))
            //    {
            //        throw new Win32Exception(Marshal.GetLastWin32Error());
            //    }

            //    if (pi.dwProcessID > 0)
            //    {
            //        thisProcess.Kill();
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        System.Diagnostics.Process.Start(psi);
            //        thisProcess.Kill();
            //    }
            //    catch (Win32Exception ex)
            //    {
            //        // operation canceled by the user
            //        if (ex.NativeErrorCode == 1223)
            //        {
            //            return;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// saves all changes to documents opened in this instance of vs.
        /// </summary>
        /// <param name="dte2"></param>
        public void SaveAllChanges()
        {
            //# Save all opened documents

            var documents = Dte2.Documents;

            for (int i = 0; i < documents.Count; i++)
            {
                SafeBlock.Run(() =>
                {
                    var doc = documents.Item(i + 1);

                    if (!doc.Saved)
                        doc.Save();
                });
            }

            //# Save all projects

            var solution = Dte2.Solution;

            if (solution != null && solution.IsOpen)
            {
                SafeBlock.Run(() =>
                {
                    var projects = solution.Projects;

                    for (int i = 0; i < projects.Count; i++)
                    {
                        var project = projects.Item(i + 1);
                        project.Save();
                    }
                });
            }
        }

        public bool TrySetAlwaysRunAsAdmin(bool enable = true)
        {
            try
            {
                if (enable)
                {
                    // registry key to mark devenv as RUNASADMIN (compatiblity mode setting)
                    var appCompatFlagsKey =
                        RegistryKey
                        .OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                        .OpenOrCreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", true);

                    if (appCompatFlagsKey != null)
                    {
                        appCompatFlagsKey.SetValue(StaticSettings.VisualStudioExeFullPath, "RUNASADMIN", RegistryValueKind.String);
                        appCompatFlagsKey.SetValue(@"C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\VSLauncher.exe", "RUNASADMIN", RegistryValueKind.String);
                    }
                }
                else
                {
                    // registry key to mark devenv as RUNASADMIN (compatiblity mode setting)
                    //! do not create subkey if does not exist
                    var appCompatFlagsKey =
                        RegistryKey
                        .OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                        .OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", true);

                    if (appCompatFlagsKey != null)
                    {
                        appCompatFlagsKey.DeleteValue(StaticSettings.VisualStudioExeFullPath, throwOnMissingValue: false);
                        appCompatFlagsKey.DeleteValue(@"C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\VSLauncher.exe", throwOnMissingValue: false);
                    }
                }
            }
            catch (Exception ex)
            {
                // todo : log
                return false;
            }

            return true;
        }

        public bool IsAlwaysRunAsAdminSet()
        {
            try
            {
                // registry key to mark devenv as RUNASADMIN (compatiblity mode setting)
                var appCompatFlagsKey =
                    RegistryKey
                    .OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default)
                    .OpenOrCreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", true);

                var isVSRunAsAdmin = string.Equals(appCompatFlagsKey.GetValue(StaticSettings.VisualStudioExeFullPath, ""), "RUNASADMIN");
                var isVSLauncherRunAsAdmin = string.Equals(appCompatFlagsKey.GetValue(@"C:\Program Files (x86)\Common Files\Microsoft Shared\MSEnv\VSLauncher.exe", ""), "RUNASADMIN");

                return isVSRunAsAdmin && isVSLauncherRunAsAdmin;

            }
            catch (Exception ex)
            {
                // todo:
                // user may not have permission to access this reg key
                //Logger.DiagnosticOnlyLogException(ex);
                return false;
            }
        }
    }
}
