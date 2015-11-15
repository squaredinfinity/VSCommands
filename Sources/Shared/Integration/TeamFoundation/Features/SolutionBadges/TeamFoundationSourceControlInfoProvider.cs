using Microsoft.TeamFoundation.VersionControl.Client;
using SquaredInfinity.VSCommands.Integration.TeamFoundation;
using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl.Integration.TeamFoundation
{
    [Export(typeof(ISourceControlInfoProvider))]
    public class TeamFoundationSourceControlInfoProvider : SourceControlInfoProvider
    {
        readonly ITeamFoundationHelper TeamFoundationHelper;

        [ImportingConstructor]
        public TeamFoundationSourceControlInfoProvider(ITeamFoundationHelper teamFoundationHelper)
        {
            this.TeamFoundationHelper = teamFoundationHelper;
        }

        protected override bool DoTryGetSourceControlInfo(string solutionFullPath, out IDictionary<string, object> properties)
        {
            properties = new Dictionary<string, object>();

            if (!TeamFoundationHelper.IsConnected())
                return false;

            var branchName = TeamFoundationHelper.GetBranchNameForItem(solutionFullPath);

            if (branchName.IsNullOrEmpty())
                return false;

            properties.AddOrUpdate(KnownProperties.BranchName, branchName);

            return true;
        }
    }
}
