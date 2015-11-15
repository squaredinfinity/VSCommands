using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl.Integration.Mercurial
{
    [Export(typeof(ISourceControlInfoProvider))]
    public class MercurialSourceControlInfoProvider : SourceControlInfoProvider
    {
        FileSystemWatcher BranchWatcher { get; set; }

        bool EnsureInternalSetup(string solutionPath, ref IDictionary<string, object> properties)
        {
            if (solutionPath.IsNullOrEmpty())
            {
                if (BranchWatcher != null)
                {
                    DisposeFileWatcher(BranchWatcher);
                    BranchWatcher = null;
                }

                return false;
            }

            var solution_directory = Path.GetDirectoryName(solutionPath);

            // try to find <solution directory>/.hg/branch if nolders above solution file

            var mercurial_parent_directory = solution_directory;
            var mercurial_directory = Path.Combine(mercurial_parent_directory, ".hg");
            var mercurial_branch = Path.Combine(mercurial_directory, "branch");

            while (!mercurial_parent_directory.IsNullOrEmpty() && !File.Exists(mercurial_branch))
            {
                mercurial_parent_directory = Path.GetDirectoryName(mercurial_parent_directory);

                if (mercurial_parent_directory.IsNullOrEmpty())
                    break;

                mercurial_directory = Path.Combine(mercurial_parent_directory, ".hg");
                mercurial_branch = Path.Combine(mercurial_directory, "branch");
            }

            if (!File.Exists(mercurial_branch))
                return false;

            properties.Add("hg:branchPath", mercurial_branch);

            //# Update File Watcher

            // dispose old file watcher if it's been used to watch different location
            if (BranchWatcher != null && BranchWatcher.Path != mercurial_directory)
            {
                DisposeFileWatcher(BranchWatcher);
                BranchWatcher = null;
            }

            // create new file watcher if needed
            if (BranchWatcher == null)
            {
                BranchWatcher = new FileSystemWatcher(mercurial_directory, "branch");
                BranchWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                BranchWatcher.Changed += BranchWatcher_Changed;
                // VisualHg seems to delete old branch file, create temporary new file (e.g. branch-23123), write to this new file and then rename it to branch
                BranchWatcher.Renamed += BranchWatcher_Renamed;

                BranchWatcher.EnableRaisingEvents = true;
            }

            return true;
        }

        private void BranchWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (!string.Equals(e.Name, "branch", StringComparison.InvariantCultureIgnoreCase))
                return;

            RequestCurrentBadgeRefresh();
        }

        private void BranchWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            RequestCurrentBadgeRefresh();
        }

        void DisposeFileWatcher(FileSystemWatcher fileWatcher)
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Changed -= BranchWatcher_Changed;
            fileWatcher.Renamed -= BranchWatcher_Renamed;
            fileWatcher.Dispose();
        }

        protected override bool DoTryGetSourceControlInfo(string solutionFullPath, out IDictionary<string, object> properties)
        {
            properties = new Dictionary<string, object>();

            if (!EnsureInternalSetup(solutionFullPath, ref properties))
                return false;

            var branch_path = (string)properties.GetValueOrDefault("hg:branchPath", () => "");

            if (!File.Exists(branch_path))
                return false;


            var branch_name = File.ReadAllText(branch_path);

            if (branch_name.IsNullOrEmpty())
                return false;

            properties.Add("hg:head", branch_name);

            properties.AddOrUpdate(KnownProperties.BranchName, branch_name);

            return true;
        }
    }
}
