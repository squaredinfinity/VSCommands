using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl
{
    public abstract class SourceControlInfoProvider : ISourceControlInfoProvider
    {
        public event EventHandler<EventArgs> CurrentBadgeRefreshRequested;
        protected void RequestCurrentBadgeRefresh()
        {
            if (CurrentBadgeRefreshRequested != null)
                CurrentBadgeRefreshRequested(this, EventArgs.Empty);
        }

        public bool TryGetSourceControlInfo(string solutionFullPath, out IDictionary<string, object> properties)
        {
            return DoTryGetSourceControlInfo(solutionFullPath, out properties);
        }

        protected abstract bool DoTryGetSourceControlInfo(string solutionFullPath, out IDictionary<string, object> properties);
    }
}
