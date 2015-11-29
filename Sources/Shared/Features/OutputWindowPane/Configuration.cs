using SquaredInfinity.VSCommands.Foundation.Text.Classification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane
{
    public class Configuration
    {
        public ClassificationMatchPattern OutputErrorClassificationPattern { get; set; }

        public ClassificationMatchPattern OutputWarningClassificationPattern { get; set; }

        public ClassificationMatchPattern OutputInformationClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputBuildSummarySuccessClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputBuildSummaryFailedClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputBuildSummaryTotalClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputCodeContractsInformationClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputProjectBuildStartClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputBuildSummaryClassificationPattern { get; set; }

        public ClassificationMatchPattern BuildOutputProjectBuildSkippedClassificationPattern { get; set; }

        public ClassificationMatchPattern TfsOutputErrorClassificationPattern { get; set; }

        public ClassificationMatchPattern TfsOutputWarningClassificationPattern { get; set; }

        public ClassificationMatchPattern TfsOutputSuccessClassificationPattern { get; set; }

        public bool ShouldOverrideOutputWindowTextColor { get; set; }

        public bool EnableFindResultsColoring { get; set; }

        public bool UseCustomFindResultsFormat { get; set; }

        public string CustomFindResultsFormat { get; set; }

        public Configuration()
        {
            this.OutputErrorClassificationPattern =
                new ClassificationMatchPattern(
                    ClassificationNames.OutputError,
                    @"(: error)|(\[error\])|(Exception was thrown)|(   at )|(error:)|(A first chance exception of type)|(Unable to create the Web site)",
                    RegexOptions.IgnoreCase);

            this.OutputWarningClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.OutputWarning,
                        @"(: warning)|(warning:)|(warning CS)|(\[warning\])",
                        RegexOptions.IgnoreCase);

            this.OutputInformationClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.OutputWarning,
                        @"(\[thread:)|(Information:)|(\[information\])",
                        RegexOptions.IgnoreCase);

            this.BuildOutputBuildSummarySuccessClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputBuildSummarySuccess,
                        @"(- Success -)|(Build succeeded)|(\[success\])",
                        RegexOptions.IgnoreCase);

            this.BuildOutputBuildSummaryFailedClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputBuildSummaryFailed,
                        "(- Failed  -)|(Build FAILED)",
                        RegexOptions.IgnoreCase);

            this.BuildOutputBuildSummaryTotalClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputBuildSummaryTotal,
                        "Total build time:",
                        RegexOptions.IgnoreCase);

            this.BuildOutputCodeContractsInformationClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputCodeContractsInformation,
                        "(EnsureContractReferenceAssemblies:)|(CodeContracts:)",
                        RegexOptions.IgnoreCase);

            this.BuildOutputProjectBuildStartClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputProjectBuildStart,
                        "------ Build started:",
                        RegexOptions.IgnoreCase);

            this.BuildOutputBuildSummaryClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputBuildSummary,
                        "(==========)|(--------)|(Deployment Summary)|(Build Summary)|(Clean Summary)",
                        RegexOptions.IgnoreCase);

            this.BuildOutputProjectBuildSkippedClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.BuildOutputProjectBuildSkipped,
                        "(Project not selected to build)|(------ Skipped )",
                        RegexOptions.IgnoreCase);




            this.TfsOutputErrorClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.TfsOutputError,
                        "TF[0-9]+:",
                        RegexOptions.IgnoreCase);

            this.TfsOutputWarningClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.TfsOutputWarning,
                        "could not be found in your workspace, or you do not have permission to access it.$",
                        RegexOptions.IgnoreCase);

            this.TfsOutputSuccessClassificationPattern =
                    new ClassificationMatchPattern(
                        ClassificationNames.TfsOutputSuccess,
                        "successfully checked in.$",
                        RegexOptions.IgnoreCase);



            this.ShouldOverrideOutputWindowTextColor = true;

            this.EnableFindResultsColoring = true;

            this.UseCustomFindResultsFormat = false;

            this.CustomFindResultsFormat = @"$p($l):$T\n";
        }
    }
}