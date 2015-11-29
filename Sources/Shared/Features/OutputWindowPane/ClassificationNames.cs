using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane
{
    public static class ClassificationNames
    {
        // NOTE: When modifying members here make sure to update VSCFontAndColorDefaultsProvider as well!

        public const string OutputText = "VSC.Output.Text";
        public const string OutputInformation = "VSC.Output.Information";
        public const string OutputWarning = "VSC.Output.Warning";
        public const string OutputError = "VSC.Output.Error";


        public const string BuildOutputBuildSummary = "VSC.BuildOutput.BuildSummary";
        public const string BuildOutputBuildSummarySuccess = "VSC.BuildOutput.BuildSummary.Success";
        public const string BuildOutputBuildSummaryFailed = "VSC.BuildOutput.BuildSummary.Failed";
        public const string BuildOutputBuildSummaryTotal = "VSC.BuildOutput.BuildSummary.Total";
        public const string BuildOutputCodeContractsInformation = "VSC.BuildOutput.CodeContracts.Information";
        public const string BuildOutputProjectBuildStart = "VSC.BuildOutput.ProjectBuildStart";
        public const string BuildOutputProjectBuildSkipped = "VSC.BuildOutput.ProjectBuildSkipped";


        public const string TfsOutputError = "VSC.TfsOutput.Error";
        public const string TfsOutputWarning = "VSC.TfsOutput.Warning";
        public const string TfsOutputSuccess = "VSC.TfsOutput.Success";

        public const string FindResultsOutputMatch = "VSC.FindResultsOutput.Match";
    }
}
