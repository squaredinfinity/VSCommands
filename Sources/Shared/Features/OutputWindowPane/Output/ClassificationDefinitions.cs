using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane.Output
{
    public class ClassificationDefinitions
    {
        #region OUTPUT WINDOW TEXT

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.OutputText)]
        internal static ClassificationTypeDefinition OutputTextClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.OutputInformation)]
        internal static ClassificationTypeDefinition OutputInformationClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.OutputWarning)]
        internal static ClassificationTypeDefinition OutputWarningClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.OutputError)]
        internal static ClassificationTypeDefinition OutputErrorClassifierTypeDefinition = null;



        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.OutputText)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.OutputText)]
        public sealed class OutputTextClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public OutputTextClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.OutputText;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Colors.Gray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.OutputInformation)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.OutputInformation)]
        public sealed class OutputInformationClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public OutputInformationClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.OutputInformation;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Colors.Gray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.OutputWarning)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.OutputWarning)]
        public sealed class OutputWarningClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public OutputWarningClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.OutputWarning;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(180, 165, 00);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.OutputError)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.OutputError)]
        public sealed class OutputErrorClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public OutputErrorClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.OutputError;
                base.ForegroundColor = Colors.DarkRed;
                base.BackgroundOpacity = 0;
            }
        }

        #endregion

        #region BUILD OUTPUT

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputBuildSummarySuccess)]
        internal static ClassificationTypeDefinition BuildSummarySuccessClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputBuildSummaryFailed)]
        internal static ClassificationTypeDefinition BuildSummaryFailedClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputCodeContractsInformation)]
        internal static ClassificationTypeDefinition CodeContractsInformationClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputProjectBuildStart)]
        internal static ClassificationTypeDefinition ProjectBuildStartClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputBuildSummary)]
        internal static ClassificationTypeDefinition BuildOutputBuildSummaryClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputBuildSummaryTotal)]
        internal static ClassificationTypeDefinition BuildOutputBuildSummaryTotalClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.BuildOutputProjectBuildSkipped)]
        internal static ClassificationTypeDefinition BuildOutputProjectBuildClassifierTypeDefinition = null;

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputProjectBuildStart)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputProjectBuildStart)]
        public sealed class ProjectBuildStartClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public ProjectBuildStartClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputProjectBuildStart;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(51, 153, 51);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputBuildSummarySuccess)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputBuildSummarySuccess)]
        public sealed class BuildOutputBuildSummarySuccessClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public BuildOutputBuildSummarySuccessClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputBuildSummarySuccess;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(51, 153, 51);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputBuildSummaryFailed)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputBuildSummaryFailed)]
        public sealed class BuildOutputBuildSummaryFailedClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public BuildOutputBuildSummaryFailedClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputBuildSummaryFailed;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Colors.DarkRed;
                //! Color.FromRgb(229, 20, 00); - is too bright on the dark theme
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputCodeContractsInformation)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputCodeContractsInformation)]
        public sealed class CodeContractsInformationClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public CodeContractsInformationClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputCodeContractsInformation;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Colors.Gray;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputBuildSummary)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputBuildSummary)]
        public sealed class BuildOutputBuildSummaryClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public BuildOutputBuildSummaryClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputBuildSummary;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(51, 153, 51);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputBuildSummaryTotal)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputBuildSummaryTotal)]
        public sealed class BuildOutputBuildSummaryTotalClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public BuildOutputBuildSummaryTotalClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputBuildSummaryTotal;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(51, 153, 51);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.BuildOutputProjectBuildSkipped)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.BuildOutputProjectBuildSkipped)]
        public sealed class BuildOutputProjectBuildSkippedClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public BuildOutputProjectBuildSkippedClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputProjectBuildSkipped;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Colors.DarkGray;
            }
        }

        #endregion

        #region TFS OUTPUT

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.TfsOutputError)]
        internal static ClassificationTypeDefinition TfsErrorClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.TfsOutputWarning)]
        internal static ClassificationTypeDefinition TfsOutputWarningClassifierTypeDefinition = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationNames.TfsOutputSuccess)]
        internal static ClassificationTypeDefinition TfsOutputSuccessClassifierTypeDefinition = null;


        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.TfsOutputError)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.TfsOutputError)]
        public sealed class TfsOutputErrorClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public TfsOutputErrorClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.TfsOutputError;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Colors.DarkRed;
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.TfsOutputWarning)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.TfsOutputWarning)]
        public sealed class TfsOutputWarningClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public TfsOutputWarningClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.TfsOutputWarning;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(180, 165, 00);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(false)]
        [Name(ClassificationNames.TfsOutputSuccess)]
        [Order(Before = Priority.Default)]
        [ClassificationType(ClassificationTypeNames = ClassificationNames.TfsOutputSuccess)]
        public sealed class TfsOutputSuccessClassifierFormatDefinition : ClassificationFormatDefinition
        {
            public TfsOutputSuccessClassifierFormatDefinition()
            {
                base.DisplayName = ClassificationNames.BuildOutputProjectBuildSkipped;
                base.BackgroundOpacity = 0;
                base.ForegroundColor = Color.FromRgb(51, 153, 51);
            }
        }
        #endregion
    }
}
