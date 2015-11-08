using EnvDTE;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl
{
    public interface ISourceControlInfoProvider
    {
        bool TryGetSourceControlInfo(Solution solution, out IDictionary<string, object> properties);
    }
}
