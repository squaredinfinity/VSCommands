using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl
{
    public interface ISourceControlInfoProvider
    {
        event EventHandler<EventArgs> CurrentBadgeRefreshRequested;
        bool TryGetSourceControlInfo(string solutionFullPath, out IDictionary<string, object> properties);
    }
}
