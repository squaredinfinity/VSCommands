using EnvDTE;
using System;
using System.Collections.Generic;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using System.Diagnostics;
using SquaredInfinity.VSCommands.Foundation.Solution;

namespace SquaredInfinity.VSCommands
{
    public static class ProjectsExtensions
    {
        /// <summary>
        /// Returns flat list of all Project Items from all Projects in a solution
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectItem> ProjectItemsTreeTraversal(this Projects projects)
        {
            foreach (Project p in projects)
            {
                foreach (var pi in p.ProjectItems.TreeTraversal())
                {
                    yield return pi;
                }
            }
        }

        /// <summary>
        /// Returns all Projects in current solution
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public static IEnumerable<Project> ProjectsTreeTraversal(this Projects projects)
        {
            for (int i = 1; i < projects.Count; i++)
            {
                var p = projects.Item(i);

                if (p.GetSolutionNodeType() == SolutionNodeType.SolutionFolder)
                {
                    foreach (var cp in p.ProjectsTreeTraversal())
                    {
                        yield return cp;
                    }
                }
                yield return p;
            }
        }

        [Conditional("DEBUG")]
        public static void Debug__ListAllProperties(this Project me)
        {
            Debug.WriteLine(string.Empty);
            Debug.WriteLine("Properties of {0} Project:".FormatWith(me.Name));

            foreach (Property p in me.Properties)
            {
                try
                {
                    Debug.WriteLine("{0} : {1}".FormatWith(p.Name, (object)p.Value));
                }
                catch
                {
                    Debug.WriteLine("Unable to get property " + p.Name);
                }
            }
        }
    }
}
