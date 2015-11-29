using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using Microsoft.VisualStudio.Text;
using SquaredInfinity.VSCommands.Foundation;
using EnvDTE80;
using System.Text.RegularExpressions;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane.FindResults
{
    public class FindResultsClassifier : OutputWindowPaneClassifier
    {
        readonly DTE2 Dte;

        public FindResultsClassifier(
            IVscSettingsService settingsService,
            IVisualStudioEventsService vsEventsService,
            IClassificationTypeRegistryService typeRegistryService,
            IClassificationFormatMapService formatMapService,
            IVsFontAndColorStorage fontAndColorStorageService,
            DTE2 dte)
            : base(
                  settingsService,
                  vsEventsService,
                  typeRegistryService,
                  formatMapService,
                  fontAndColorStorageService,
                  "find results")
        {
            this.Dte = dte;

            IsEnabled = true;
            
            AddClassificationType(ClassificationNames.FindResultsOutputMatch);

            OutputWindowTextClassificationOverride = ClassificationNames.OutputText;
            ShouldOverrideOutputWindowTextClassification = true;

            RefreshClassifications();
        }

        protected override IList<ClassificationSpan> DoGetClassificationSpans(SnapshotSpan span)
        {
            List<ClassificationSpan> result = new List<ClassificationSpan>();

            try
            {
                if (!IsEnabled)
                    return result;

                if (span == null || span.IsEmpty)
                    return result;

                string spanText = span.GetText();

                if (spanText.IsNullOrEmpty())
                    return result;

                var matchClassification = ClassificationTypeRegistryService.GetClassificationType(ClassificationNames.FindResultsOutputMatch);
                var textClassification = ClassificationTypeRegistryService.GetClassificationType(ClassificationNames.OutputText);
                
                var find = Dte.Find;

                // get searched text
                var searchedText = find.FindWhat;

                // escape regex special characters
                var pattern = Regex.Escape(searchedText);

                var regexOptions = RegexOptions.None;

                // match whole word?
                if (find.MatchWholeWord)
                    pattern = @"\b{0}\b".FormatWith(pattern);

                // match case?
                if (!find.MatchCase)
                    regexOptions = RegexOptions.IgnoreCase;

                Regex regex = new Regex(pattern, regexOptions);
                var matches = regex.Matches(spanText);

                //+ gray out path part if needed
                if (matches.Count > 0 && ShouldOverrideOutputWindowTextClassification)
                {
                    var lineNumberPartIndex = spanText.IndexOf("):");

                    if (lineNumberPartIndex < matches[0].Index)
                    {
                        result.Add(new ClassificationSpan(new SnapshotSpan(span.Start, lineNumberPartIndex + 2), textClassification));
                    }
                }

                //+ highlight matches
                foreach (Match match in matches)
                {
                    result.Add(new ClassificationSpan(new SnapshotSpan(span.Start + match.Index, match.Length), matchClassification));
                }

                //+ gray out lines without results
                if (ShouldOverrideOutputWindowTextClassification)
                {
                    var isResultsHeader = spanText.StartsWith("Find all \"");

                    if (result.Count == 0 || isResultsHeader)
                    {
                        result.Add(new ClassificationSpan(new SnapshotSpan(span.Start, span.End), textClassification));
                    }
                }

            }
            catch (Exception ex)
            {
                //tODO: log
                //ex.Log();
            }

            return result;
        }
    }
}
