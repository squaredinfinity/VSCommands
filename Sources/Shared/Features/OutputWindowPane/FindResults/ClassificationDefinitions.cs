using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane.FindResults
{
    public class ClassificationDefinitions
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.FindResultsOutputMatch)]
        internal static ClassificationTypeDefinition MatchClassifierTypeDefinition = null;

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.FindResultsOutputMatch)]
        [Order(Before = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.FindResultsOutputMatch)]
        public sealed class MatchClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public MatchClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.FindResultsOutputMatch;
                base.BackgroundOpacity = 0;
                base.BackgroundColor = Colors.Yellow;
            }
        }
    }
}
