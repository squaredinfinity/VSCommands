using Microsoft.VisualStudio.Shell.Interop;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using Microsoft.Win32;
using System.Linq;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.VisualStudio;

namespace SquaredInfinity.VSCommands.Foundation.FontsAndColors
{
    [Guid("3448FF72-B072-435E-9059-29D89C0A3CD1")]
    public partial class VSCFontAndColorDefaultsProvider : IVsFontAndColorDefaultsProvider, IVsFontAndColorEvents, IVsFontAndColorDefaults
    {
        public static string VSCFontAndColorCategoryGuid_AsString = "b0e6a221-92fd-4d72-be80-04a36b591fcb";
        public static Guid VSCFontAndColorCategoryGuid = new Guid(VSCFontAndColorCategoryGuid_AsString);

        List<AllColorableItemInfo> ColorableItems = new List<AllColorableItemInfo>();

        readonly IVisualStudioEventsService VisualStudioEventsService;
        readonly IServiceProvider ServiceProvider;
        readonly IServiceContainer ServiceContainer;

        public VSCFontAndColorDefaultsProvider(
            IVisualStudioEventsService visualStudioEventsService,
            IServiceProvider serviceProvider,
            IServiceContainer serviceContainer)
        {
            this.VisualStudioEventsService = visualStudioEventsService;
            this.ServiceProvider = serviceProvider;
            this.ServiceContainer = serviceContainer;
        }

        void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                // todo: log the exception
                Trace.WriteLine(ex.ToString());
            }
        }

        public void TryClearFontAndColorCache()
        {
            SafeExecute(() =>
            {
                IVsFontAndColorCacheManager cacheManager = 
                ServiceProvider.GetService(typeof(SVsFontAndColorCacheManager)) as IVsFontAndColorCacheManager;

                cacheManager.ClearCache(ref VSCFontAndColorCategoryGuid);
            });
        }

        public void RegisterCollorableItem(string name, string description, bool isBold = false)
        {
            var colorableItemInfo = CreateAllColorableItemInfo(name, description, isBold);

            ColorableItems.Add(colorableItemInfo);
        }

        //private void InitialiseColorableItems()
        //{
        //    //! colors will be populated through themes in VS11+

        //    ColorableItems.AddRange(new AllColorableItemInfo[] {
        //        CreateAllColorableItemInfo(ClassificationNames.OutputText, "Output Window Text"),
        //        CreateAllColorableItemInfo(ClassificationNames.OutputInformation, "Output Window Information"),
        //        CreateAllColorableItemInfo(ClassificationNames.OutputWarning, "Output Window Warning"),
        //        CreateAllColorableItemInfo(ClassificationNames.OutputError, "Output Window Error"),

        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputBuildSummary, ""),
        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputBuildSummarySuccess, ""),
        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputBuildSummaryFailed, ""),
        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputBuildSummaryTotal, ""),

        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputCodeContractsInformation, ""),

        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputProjectBuildStart, ""),
        //        CreateAllColorableItemInfo(ClassificationNames.BuildOutputProjectBuildSkipped, ""),

        //        CreateAllColorableItemInfo(ClassificationNames.TfsOutputError, ""),
        //        CreateAllColorableItemInfo(ClassificationNames.TfsOutputWarning, ""),
        //        CreateAllColorableItemInfo(ClassificationNames.TfsOutputSuccess, ""),

        //        CreateAllColorableItemInfo(ClassificationNames.FindResultsOutputMatch, "Find Results Match", isBold:true)
        //    });
        //}

        AllColorableItemInfo CreateAllColorableItemInfo(string name, string description, bool isBold = false)
        {
            var result = new AllColorableItemInfo
            {
                bNameValid = 1,
                bstrName = name,
                bDescriptionValid = 1,
                bstrDescription = description,
                bLocalizedNameValid = 1,
                bstrLocalizedName = name,
                bFlagsValid = 1,
                fFlags = (uint)(__FCITEMFLAGS.FCIF_ALLOWFGCHANGE | __FCITEMFLAGS.FCIF_ALLOWBGCHANGE | __FCITEMFLAGS.FCIF_ALLOWBOLDCHANGE | __FCITEMFLAGS.FCIF_ALLOWCUSTOMCOLORS),
                Info = new ColorableItemInfo
                {
                    bBackgroundValid = 1,
                    crBackground = 1,
                    bForegroundValid = 1,
                    crForeground = 1
                }
            };

            if (isBold)
            {
                result.Info.bFontFlagsValid = 1;
                result.Info.dwFontFlags = 1;
            }

            return result;
        }

        public void Initialise()
        {
            ServiceContainer.AddService(typeof(VSCFontAndColorDefaultsProvider), this, promote: true);

            if (!TryUpdateRegistry())
            {
                // todo: logging
                //Logger.TraceWarning(() => "Unable to register VSCommands Font and Color Settings. Run Visual Studio as Administrator to provide required registry access.");
            }
        }

        public bool TryUpdateRegistry()
        {
            bool resultOne = false;
            bool resultTwo = false;

            resultOne =
                TryUpdateRegistry(
                Registry.LocalMachine,
                @"SOFTWARE\Microsoft\VisualStudio\{0}".FormatWith(StaticSettings.VisualStudioHiveVersionNumber),
                @"SOFTWARE\Microsoft\VisualStudio\{0}\FontAndColors\VSCommands".FormatWith(StaticSettings.VisualStudioHiveVersionNumber));

            resultTwo =
                TryUpdateRegistry(
                Registry.CurrentUser,
                @"SOFTWARE\Microsoft\VisualStudio\{0}_Config".FormatWith(StaticSettings.VisualStudioHiveVersionNumber),
                @"SOFTWARE\Microsoft\VisualStudio\{0}_Config\FontAndColors\VSCommands".FormatWith(StaticSettings.VisualStudioHiveVersionNumber));

            return resultOne && resultTwo;
        }

        public bool TryUpdateRegistry(RegistryKey masterKey, string vsKeyName, string fullKeyName)
        {
            try
            {
                // verify registry key exists, if so then just return
                using (var key = masterKey.OpenSubKey(fullKeyName))
                {
                    if (key != null)
                    {
                        var valueNames = key.GetValueNames();

                        if (valueNames.Contains("Category") && valueNames.Contains("Package"))
                        {
                            if (key.GetValue("Category").ToString() == "{b0e6a221-92fd-4d72-be80-04a36b591fcb}"
                                && key.GetValue("Package").ToString() == "{3448FF72-B072-435E-9059-29D89C0A3CD1}")
                            {
                                return true;
                            }
                        }
                    }
                }

                var registryUserSecurity = new RegistrySecurity();

                var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var registryAccessRule = new RegistryAccessRule(sid, RegistryRights.FullControl, AccessControlType.Allow);
                registryUserSecurity.AddAccessRule(registryAccessRule);


                var packageGuid = "{3448FF72-B072-435E-9059-29D89C0A3CD1}";

                using (var vscReg = masterKey.OpenSubKey(vsKeyName, writable: false))
                {
                    if (vscReg == null)
                        return false;

                    RegistryKey fontsAndColorsKey = null;

                    if (vscReg.GetSubKeyNames().Contains("FontAndColors"))
                    {
                        fontsAndColorsKey = vscReg.OpenSubKey("FontAndColors", writable: true);
                    }
                    else
                    {
                        fontsAndColorsKey = vscReg.CreateSubKey("FontAndColors", RegistryKeyPermissionCheck.Default, registryUserSecurity);
                    }

                    using (fontsAndColorsKey)
                    {
                        RegistryKey vscommandsKey = null;

                        if (fontsAndColorsKey.GetSubKeyNames().Contains("VSCommands"))
                        {
                            vscommandsKey = fontsAndColorsKey.OpenSubKey("VSCommands", writable: true);
                        }
                        else
                        {
                            vscommandsKey = fontsAndColorsKey.CreateSubKey("VSCommands", RegistryKeyPermissionCheck.ReadWriteSubTree, registryUserSecurity);
                        }

                        using (vscommandsKey)
                        {
                            vscommandsKey.SetValue("Category", "{b0e6a221-92fd-4d72-be80-04a36b591fcb}", RegistryValueKind.String);
                            vscommandsKey.SetValue("Package", packageGuid, RegistryValueKind.String);
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //Logger.DiagnosticOnlyLogException(ex);
                return false;
            }
        }
    }
}
