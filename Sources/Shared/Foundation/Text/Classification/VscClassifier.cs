using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Text;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using SquaredInfinity.VSCommands.Foundation.Settings;

namespace SquaredInfinity.VSCommands.Foundation.Text.Classification
{
    public abstract partial class VscClassifier : IClassifier
    {
        protected IVscSettingsService SettingsService { get; private set; }
        protected IVisualStudioEventsService VisualStudioEventsService { get; private set; }
        protected IClassificationTypeRegistryService ClassificationTypeRegistryService { get; private set; }
        protected IClassificationFormatMapService ClassificationFormatMapService { get; private set; }
        protected IVsFontAndColorStorage FontAndColorStorageService { get; private set; }
        protected string ClassificationMapName { get; private set; }

        readonly List<ClassificationTypeMatchPattern> ClassificationTypesMatchPatterns = new List<ClassificationTypeMatchPattern>();

        protected bool IsEnabled { get; set; }

        public VscClassifier(
            IVscSettingsService settingsService,
            IVisualStudioEventsService vsEventsService,
            IClassificationTypeRegistryService typeRegistryService,
            IClassificationFormatMapService formatMapService,
            IVsFontAndColorStorage fontAndColorStorageService,
            string mapName)
        {
            this.SettingsService = settingsService;
            this.VisualStudioEventsService = vsEventsService;
            this.ClassificationTypeRegistryService = typeRegistryService;
            this.ClassificationFormatMapService = formatMapService;
            this.FontAndColorStorageService = fontAndColorStorageService;
            this.ClassificationMapName = mapName;

            vsEventsService.AfterVisualStudioThemeChanged += (s, e) => RefreshClassifications();
            vsEventsService.AfterTextColorSettingsChanged += (s, e) => RefreshClassifications();
            vsEventsService.AfterVSCommandsColorSettingsChanged += (s, e) => RefreshClassifications();
        }

        protected void AddClassificationType(string typeName)
        {
            AddClassificationType(typeName, new ClassificationMatchPattern());
        }

        protected void AddClassificationType(string typeName, ClassificationMatchPattern matchPattern)
        {
            var typeMatchPattern = new ClassificationTypeMatchPattern
            {
                ClassificationTypeName = typeName,
                ClassificationMatchPattern = matchPattern
            };

            ClassificationTypesMatchPatterns.Add(typeMatchPattern);
        }

        protected void RefreshClassifications()
        {
            // read color settings from VSCommands category and apply them to classification types

            const uint flags = (uint)(
                __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS | __FCSTORAGEFLAGS.FCSF_PROPAGATECHANGES);

            try
            {
                var x = FontAndColorStorageService.OpenCategory(new Guid("{b0e6a221-92fd-4d72-be80-04a36b591fcb}"), flags);
                foreach (var item in ClassificationTypesMatchPatterns)
                {
                    try
                    {
                        if (FontAndColorStorageService.GetItem(item.ClassificationTypeName, item.ColorableItemInfos) < 0)
                        {
                           // Logger.TraceDiagnosticWarning(() => "Unable to load classification data for {0}".FormatWith(item.ClassificationTypeName));
                            continue;
                        }
                        
                        var ct = ClassificationTypeRegistryService.GetClassificationType(item.ClassificationTypeName);

                        var fm = ClassificationFormatMapService.GetClassificationFormatMap(ClassificationMapName);

                        var currentTp = fm.GetTextProperties(ct);

                        var tp = currentTp.SetForeground(item.ColorableItemInfos[0].GetForeground());

                        tp = tp.SetBackground(item.ColorableItemInfos[0].GetBackground());

                        var fontFlags = (FONTFLAGS)item.ColorableItemInfos[0].dwFontFlags;

                        if (fontFlags.IsFlagSet(FONTFLAGS.FF_BOLD))
                            tp = tp.SetBold(true);
                        else
                            tp = tp.SetBold(false);

                        fm.SetTextProperties(ct, tp);
                    }
                    catch (Exception ex)
                    {
                        // TODO: logging
                       // Logger.LogException(ex);
                    }
                }

                FontAndColorStorageService.CloseCategory();
            }
            catch (Exception ex)
            {
                // TODO: logging
            }
            finally
            {
                FontAndColorStorageService.CloseCategory();
            }
        }
    }
}
