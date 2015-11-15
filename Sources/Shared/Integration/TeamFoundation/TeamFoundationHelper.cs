using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Linq;
using System.Threading;
using SquaredInfinity.VSCommands;
using System.ComponentModel.Composition;

namespace SquaredInfinity.VSCommands.Integration.TeamFoundation
{
    // NOTE:    one of the easiest ways to interact with TFS in visual studio is via TeamFoundationServerExt,
    //          but it would need hard reference to Microsoft.TeamFoundation.dll which I have a feeling is not the best way
    //          This class will try to avoid using it and utilise officially available nuget packages instead

    [Export(typeof(ITeamFoundationHelper))]
    public class TeamFoundationHelper : ITeamFoundationHelper
    {
        readonly IServiceProvider ServiceProvider;

        [ImportingConstructor]
        public TeamFoundationHelper(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public bool IsConnected()
        {
            // check if workspace for currently loaded solution exists
            // if it doesn't then it's likely that we're not connected

            var dte2 = ServiceProvider.GetDte2();
            var solution = dte2.Solution;

            if (solution == null)
                return false;

            if (solution.FullName.IsNullOrEmpty())
                return false;

            var info = Workstation.Current.GetLocalWorkspaceInfo(solution.FullName);

            return info != null;
        }

        public bool WaitForConnection()
        {
            return WaitForConnection(TimeSpan.FromSeconds(5));
        }

        public bool WaitForConnection(TimeSpan timeout)
        {
            var cts = new CancellationTokenSource(timeout);
            return WaitForConnection(cts.Token);
        }

        public bool WaitForConnection(CancellationToken cancellationToken)
        {
            if(!IsConnected())
            {
                do
                {
                    Thread.Sleep(50);

                    if (cancellationToken.IsCancellationRequested)
                        return false;

                } while (!IsConnected());
            }

            return true;
        }

        public string GetBranchNameForCurrentSolution()
        {
            var dte2 = ServiceProvider.GetDte2();

            var solution = dte2.Solution;

            if (solution == null)
                return null;

            if (solution.FullName.IsNullOrEmpty())
                return null;

            return GetBranchNameForItem(solution.FullName);
        }

        public string GetBranchNameForItem(string localFullPath)
        {
            var workspace = (Workspace)null;
            var branchObject = (BranchObject)null;

            return GetBranchNameForItem(
                localFullPath, 
                waitForConnection: true, 
                timeout: TimeSpan.FromSeconds(5),
                workspace: out workspace,
                branchObject: out branchObject);
        }

        public string GetBranchNameForItem(string localFullPath, out Workspace workspace, out BranchObject branchObject)
        {
            workspace = null;
            branchObject = null;

            return GetBranchNameForItem(
                localFullPath, 
                waitForConnection: true, 
                timeout: TimeSpan.FromSeconds(5),
                workspace: out workspace,
                branchObject: out branchObject);
        }

        public string GetBranchNameForItem(
            string localFullPath, 
            bool waitForConnection, 
            TimeSpan timeout, 
            out Workspace workspace, 
            out BranchObject branchObject)
        {
            CancellationTokenSource cts = new CancellationTokenSource(timeout);

            return GetBranchNameForItem(
                localFullPath, 
                waitForConnection, 
                cts.Token,
                out workspace,
                out branchObject);
        }

        public string GetBranchNameForItem(
            string localFullPath, 
            bool waitForConnection, 
            CancellationToken cancellationToken,
            out Workspace workspace, 
            out BranchObject branchObject)
        {
            workspace = null;
            branchObject = null;

            try
            {
                if (localFullPath.IsNullOrEmpty())
                    return null;

                WaitForConnection();

                if (cancellationToken.IsCancellationRequested)
                    return null;

                var info = Workstation.Current.GetLocalWorkspaceInfo(localFullPath);
                if (info == null)
                    return null;

                var serverName = info.ServerUri;

                var tfsSrv = Microsoft.TeamFoundation.Client.TeamFoundationServerFactory.GetServer(serverName);

                VersionControlServer vcs = (VersionControlServer)tfsSrv.GetService(typeof(VersionControlServer));

                if (cancellationToken.IsCancellationRequested)
                    return null;

                workspace = vcs.TryGetWorkspace(localFullPath);
                if (workspace == null)
                    return null;

                var serverItem = workspace.GetServerItemForLocalItem(localFullPath);

                if (cancellationToken.IsCancellationRequested)
                    return null;

                var branchObjects =
                    vcs.QueryRootBranchObjects(RecursionType.Full)
                    .OrderBy(bo => bo.Properties.RootItem.Item)
                    .Reverse();

                var itemSpecs = new ItemSpec[]
                {
                    new ItemSpec(serverItem, RecursionType.None)
                };

                // for each itemSpec return BranchHistoryItem[]
                var branchHistory = vcs.GetBranchHistory(itemSpecs, VersionSpec.Latest);

                if (!branchHistory[0].Any())
                    return null;

                if (cancellationToken.IsCancellationRequested)
                    return null;

                branchObject =
                    (from bo in branchObjects
                     where serverItem.StartsWith(bo.Properties.RootItem.Item)
                     select bo).
                     FirstOrDefault();

                if (branchObject == null)
                    return null;

                var branchName = System.IO.Path.GetFileName(branchObject.Properties.RootItem.Item);

                if (cancellationToken.IsCancellationRequested)
                    return null;

                return branchName;

            }
            catch (Exception ex)
            {
                // TODO: logging
                return null;
            }
        }
    }
}
