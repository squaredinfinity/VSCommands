using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Classification;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane.Output
{
    public class OutputClassifier : OutputWindowPaneClassifier
    {
        public OutputClassifier(
            IVscSettingsService settingsService,
            IVisualStudioEventsService vsEventsService,
            IClassificationTypeRegistryService typeRegistryService,
            IClassificationFormatMapService formatMapService,
            IVsFontAndColorStorage fontAndColorStorageService)
            : base(
                  settingsService,
                  vsEventsService,
                  typeRegistryService,
                  formatMapService,
                  fontAndColorStorageService,
                  "output")
        {
            IsEnabled = true;

            // TODO: get from actual configuration and react to any changes
            var Config = new Configuration();

            AddClassificationType(ClassificationNames.OutputError, Config.OutputErrorClassificationPattern);
            AddClassificationType(ClassificationNames.OutputWarning, Config.OutputWarningClassificationPattern);
            AddClassificationType(ClassificationNames.OutputInformation, Config.OutputInformationClassificationPattern);

            AddClassificationType(ClassificationNames.BuildOutputBuildSummarySuccess, Config.BuildOutputBuildSummarySuccessClassificationPattern);
            AddClassificationType(ClassificationNames.BuildOutputBuildSummaryFailed, Config.BuildOutputBuildSummaryFailedClassificationPattern);
            AddClassificationType(ClassificationNames.BuildOutputCodeContractsInformation, Config.BuildOutputCodeContractsInformationClassificationPattern);
            AddClassificationType(ClassificationNames.BuildOutputProjectBuildStart, Config.BuildOutputProjectBuildStartClassificationPattern);
            AddClassificationType(ClassificationNames.BuildOutputBuildSummary, Config.BuildOutputBuildSummaryClassificationPattern);
            AddClassificationType(ClassificationNames.BuildOutputBuildSummaryTotal, Config.BuildOutputBuildSummaryTotalClassificationPattern);
            AddClassificationType(ClassificationNames.BuildOutputProjectBuildSkipped, Config.BuildOutputProjectBuildSkippedClassificationPattern);

            AddClassificationType(ClassificationNames.TfsOutputError, Config.TfsOutputErrorClassificationPattern);
            AddClassificationType(ClassificationNames.TfsOutputWarning, Config.TfsOutputWarningClassificationPattern);
            AddClassificationType(ClassificationNames.TfsOutputSuccess, Config.TfsOutputSuccessClassificationPattern);

            OutputWindowTextClassificationOverride = ClassificationNames.OutputText;
            ShouldOverrideOutputWindowTextClassification = true;

            RefreshClassifications();
        }
    }
}