using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;
using SquaredInfinity.Foundation.Composition;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges.SourceControl.Integration.Git
{
    [Export(typeof(ISourceControlInfoProvider))]
    public class GitSourceControlInfoProvider : SourceControlInfoProvider
    {
        FileSystemWatcher GitHeadWatcher { get; set; }
        
        readonly Regex GitBranchRegex = new Regex("refs/heads/(?<branchName>.*)$", RegexOptions.IgnoreCase);

        [ImportingConstructor]
        public GitSourceControlInfoProvider()
        {}

        bool EnsureInternalSetup(string solutionPath, ref IDictionary<string, object> properties)
        {
            if (solutionPath.IsNullOrEmpty())
            {
                if (GitHeadWatcher != null)
                {
                    DisposeFileWatcher(GitHeadWatcher);
                    GitHeadWatcher = null;
                }

                return false;
            }

            var solution_directory = Path.GetDirectoryName(solutionPath);

            //# Try to Find Git HEAD file in folders above solution file

            var git_parent_directory = solution_directory;
            var git_directory = Path.Combine(git_parent_directory, ".git");
            var git_head = Path.Combine(git_directory, "HEAD");

            while (!git_parent_directory.IsNullOrEmpty() && !File.Exists(git_head))
            {
                git_parent_directory = Path.GetDirectoryName(git_parent_directory);

                if (git_parent_directory.IsNullOrEmpty())
                    break;

                git_directory = Path.Combine(git_parent_directory, ".git");
                git_head = Path.Combine(git_directory, "HEAD");
            }

            if (!File.Exists(git_head))
                return false;

            properties.Add("git:headPath", git_head);

            //# update file watcher

            // dispose old file watcher if it's been used to watch different location
            if (GitHeadWatcher != null && GitHeadWatcher.Path != git_directory)
            {
                DisposeFileWatcher(GitHeadWatcher);
                GitHeadWatcher = null;
            }

            // create new file watcher if needed
            if (GitHeadWatcher == null)
            {
                GitHeadWatcher = new FileSystemWatcher(git_directory, "HEAD");
                GitHeadWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                GitHeadWatcher.Changed += GitHeadWatcher_Changed;
                GitHeadWatcher.EnableRaisingEvents = true;
            }


            return true;
        }

        void DisposeFileWatcher(FileSystemWatcher fileWatcher)
        {
            fileWatcher.EnableRaisingEvents = false;
            fileWatcher.Changed -= GitHeadWatcher_Changed;
            fileWatcher.Dispose();
        }

        void GitHeadWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            RequestCurrentBadgeRefresh();
        }

        protected override bool DoTryGetSourceControlInfo(string solutionFullPath, out IDictionary<string, object> properties)
        {
            properties = new Dictionary<string, object>();

            if (!EnsureInternalSetup(solutionFullPath, ref properties))
                return false;

            var head_path = (string)properties.GetValueOrDefault("git:headPath", () => "");

            if (!File.Exists(head_path))
                return false;

            var head_content = File.ReadAllText(head_path);
            var branch_match = GitBranchRegex.Match(head_content);

            if (!branch_match.Success)
                return false;

            var branch_group = branch_match.Groups["branchName"];

            if (!branch_group.Success)
                return false;

            var branch_name = branch_group.Value;
            properties.Add("git:head", branch_name);

            properties.AddOrUpdate(KnownProperties.BranchName, branch_name);

            return true;
        }
    }
}
