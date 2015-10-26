using EnvDTE;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    public class CurrentVSStateInfo
    {
        public string SolutionName;
        public string BranchName;
        public string ActiveDocumentName;
        public dbgDebugMode DebugMode;
    }
}
