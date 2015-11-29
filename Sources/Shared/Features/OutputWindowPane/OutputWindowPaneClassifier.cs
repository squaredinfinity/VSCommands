using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.Text.Classification;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane
{
    public class OutputWindowPaneClassifier : VscClassifier
    {
        protected bool ShouldOverrideOutputWindowTextClassification { get; set; }

        protected string OutputWindowTextClassificationOverride { get; set; }

        public OutputWindowPaneClassifier(
            IVscSettingsService settingsService,
            IVisualStudioEventsService vsEventsService,
            IClassificationTypeRegistryService typeRegistryService,
            IClassificationFormatMapService formatMapService,
            IVsFontAndColorStorage fontAndColorStorageService,
            string classificationMapName)
            : base(
                  settingsService,
                  vsEventsService,
                  typeRegistryService, 
                  formatMapService,
                  fontAndColorStorageService,
                  classificationMapName)
        {

        }

        protected override IList<ClassificationSpan> DoGetClassificationSpans(Microsoft.VisualStudio.Text.SnapshotSpan span)
        {
            var result = base.DoGetClassificationSpans(span);

            if (ShouldOverrideOutputWindowTextClassification
                && (result == null || result.Count == 0))
            {
                var c = ClassificationTypeRegistryService.GetClassificationType(OutputWindowTextClassificationOverride);

                result.Add(new ClassificationSpan(new SnapshotSpan(span.Start, span.Length), c));
            }

            return result;
        }
    }
}
