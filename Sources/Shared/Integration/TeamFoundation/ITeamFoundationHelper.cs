using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.VSCommands.Integration.TeamFoundation
{
    public interface ITeamFoundationHelper
    {
        bool IsConnected();

        bool WaitForConnection();
        bool WaitForConnection(TimeSpan timeout);
        bool WaitForConnection(CancellationToken cancellationToken);

        string GetBranchNameForCurrentSolution();

        string GetBranchNameForItem(string localFullPath);
        string GetBranchNameForItem(string localFullPath, bool waitForConnection, TimeSpan timeout);
        string GetBranchNameForItem(string localFullPath, bool waitForConnection, CancellationToken cancellationToken);
    }
}
