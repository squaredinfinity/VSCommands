using EnvDTE;
using System;
using System.Collections.Generic;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace SquaredInfinity.VSCommands
{
    public static class UIHierarchyExtensions
    {
        public static UIHierarchyItem FindUIHierarchyItem(this UIHierarchy uiHierarchy, ProjectItem projectItem)
        {
            var hItems = uiHierarchy.UIHierarchyItems;
            var hRoot = hItems.GetHierarchyItem(0);

            var result = from hi in hRoot.TreeTraversal()
                         where hi.Object == projectItem
                         select hi;

            return result.FirstOrDefault();
        }

        public static UIHierarchyItem FindUIHierarchyItem(this UIHierarchy uiHierarchy, Project project)
        {
            var result = from hi in uiHierarchy.UIHierarchyItems.GetHierarchyItem(0).TreeTraversal()
                         where
                         (hi.IsProject() && hi.AsProject().UniqueName == project.UniqueName)
                         || (hi.IsSolutionFolder() && hi.AsSolutionFolder().UniqueName == project.UniqueName)
                         select hi;


            return result.FirstOrDefault();
        }

        /// <summary>
        /// Reloads the project with a given name.
        /// Assumes that the project has been uloaded before.
        /// </summary>
        /// <param name="uiHierarchy"></param>
        public static void ReloadUloadedProject(this UIHierarchy uiHierarchy, string projectName)
        {
            var projects = (from p in uiHierarchy.UIHierarchyItems.TreeTraversal()
                            where !(p.Object is EnvDTE.Solution) // do not include solution
                            && p.TryGetName().Equals(projectName)
                            select p).ToList().Distinct();

            if (projects.Count() > 1)
            {
                // todo: log
                //Trace.TraceWarning("Unable to uniquely locate unloaded project with name '{0}'.".FormatWith(projectName));
            }

            var project = projects.FirstOrDefault();

            if (project == null)
                return;

            try
            {
                project.Select(vsUISelectionType.vsUISelectionTypeSelect);
                uiHierarchy.DTE.ExecuteCommand("Project.ReloadProject");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: log
                //ex.Swallow("No need to show it to users, but will have to investigate what's causing it");
            }
        }
    }
}
