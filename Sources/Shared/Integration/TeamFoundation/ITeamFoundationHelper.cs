using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SquaredInfinity.VSCommands.Integration.TeamFoundation
{
    public interface ITeamFoundationHelper
    {
        bool IsConnected();

        #region Wait For Connection
        bool WaitForConnection();
        bool WaitForConnection(TimeSpan timeout);
        bool WaitForConnection(CancellationToken cancellationToken);
        #endregion

        string GetBranchNameForCurrentSolution();

        #region Get Branch Name For Item
        string GetBranchNameForItem(string localFullPath);
        string GetBranchNameForItem(
            string localFullPath,
            out Workspace workspace,
            out BranchObject branchObject);
        string GetBranchNameForItem(
            string localFullPath, 
            bool waitForConnection, 
            TimeSpan timeout,
            out Workspace workspace,
            out BranchObject branchObject);
        string GetBranchNameForItem(
            string localFullPath, 
            bool waitForConnection, 
            CancellationToken cancellationToken,
            out Workspace workspace,
            out BranchObject branchObject);
        #endregion
    }
}
