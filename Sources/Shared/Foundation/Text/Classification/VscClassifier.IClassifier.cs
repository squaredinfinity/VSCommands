using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace SquaredInfinity.VSCommands.Foundation.Text.Classification
{
    public partial class VscClassifier : IClassifier
    {
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        IList<ClassificationSpan> IClassifier.GetClassificationSpans(SnapshotSpan span)
        {
            return DoGetClassificationSpans(span);
        }

        protected virtual IList<ClassificationSpan> DoGetClassificationSpans(SnapshotSpan span)
        {
            List<ClassificationSpan> result = new List<ClassificationSpan>();

            try
            {
                if (!IsEnabled)
                    return result;

                if (span == null || span.IsEmpty)
                    return result;

                string spanText = span.GetText();

                foreach (var ctmp in ClassificationTypesMatchPatterns)
                {
                    if (!ctmp.IsMatch(spanText))
                    {
                        continue;
                    }
                    else
                    {
                        var c = ClassificationTypeRegistryService.GetClassificationType(ctmp.ClassificationTypeName);

                        result.Add(
                            new ClassificationSpan(
                                new SnapshotSpan(span.Start, span.Length), c));
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: logging
                //ex.Log();
            }

            return result;
        }
    }
}
