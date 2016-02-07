using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SquaredInfinity.VSCommands.Foundation.Solution;
using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;
using VSLangProj;
using System.Diagnostics;

namespace SquaredInfinity.VSCommands
{
    public static class ProjectExtensions
    {
        public static SolutionNodeType GetSolutionNodeType(this Project project)
        {
            if (project.KindAsGuid() == SolutionItemKindGuids.ProjectAsSolutionFolder)
                return SolutionNodeType.SolutionFolder;
            else
                return SolutionNodeType.Project;
        }

        public static Guid KindAsGuid(this Project project)
        {
            return new Guid(project.Kind);
        }

        public static IVsHierarchy ToHierarchy(this Project project)
        {
            var solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution;

            IVsHierarchy hierarchy = null;

            solution.GetProjectOfUniqueName(project.FullName, out hierarchy);

            return hierarchy;
        }

        public static bool HasProperty(this Project project, string propertyName)
        {
            try
            {
                var properties = project.Properties;

                if (properties == null)
                    return false;

                Property property = null;

                for (int i = 0; i < properties.Count; i++)
                {
                    property = properties.Item(i + 1);

                    if (property.Name.ToLower() == propertyName.ToLower())
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool TryGetPropertyValue<TProperty>(this Project project, string propertyName, out TProperty value)
        {
            value = default(TProperty);

            try
            {
                var properties = project.Properties;

                if (properties == null)
                {
                    return false;
                }

                var property = properties.Item(propertyName);

                if (property != null)
                {
                    value = (TProperty)property.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: log
                //ex.TryAddContextData("propertyName", () => propertyName);
                //Logger.LogException(ex);
                //return default(TProperty);
                return false;
            }
        }

        public static bool TrySetPropertyValue(this Project project, string propertyName, object newValue)
        {
            try
            {
                var properties = project.Properties;

                var property = properties.Item(propertyName);

                property.Value = newValue;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: log
                //ex.TryAddContextData("propertyName", () => propertyName);
                //ex.TryAddContextData("newValue", () => newValue);
                //Logger.LogException(ex);
                return false;
            }
        }

        public static IEnumerable<Project> ProjectsTreeTraversal(this Project project)
        {
            var result = new List<Project>();

            var projectItems = project.ProjectItems;

            try
            {
                //! ProjectItems may be null when project is in solution but has been unloaded
                if (projectItems == null)
                    return result;

                for (int i = 1; i <= projectItems.Count; i++)
                {
                    try
                    {
                        var pi = projectItems.Item(i);

                        var piObjectAsProject = pi.Object as Project;

                        if (piObjectAsProject != null)
                        {
                            result.Add(piObjectAsProject);

                            //! this is previous check done here, may still need to revert to it in a future if needed
                            //if (string.Equals(piObjectAsProject.GetSolutionNodeType.Kind, ProjectKinds.vsProjectKindSolutionFolder, StringComparison.InvariantCultureIgnoreCase))
                            if(piObjectAsProject.GetSolutionNodeType() == SolutionNodeType.SolutionFolder)
                            {
                                var childProjects = piObjectAsProject.ProjectsTreeTraversal();

                                result.AddRange(childProjects);
                            }
                        }
                        else
                        {
                            piObjectAsProject.SafeReleaseComObject();
                            piObjectAsProject = null;
                        }

                        pi.SafeReleaseComObject();
                        pi = null;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        // todo: logging
                        //Logger.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: log
                //ex.Log();
            }
            finally
            {
                projectItems.SafeReleaseComObject();
                projectItems = null;
            }

            return result;
        }

        public static bool TryUnloadProject(this Project project)
        {
            try
            {
                var dte2 = (project.DTE as DTE2);

                var toolWindows = dte2.ToolWindows;
                var solutionExplorer = toolWindows.SolutionExplorer;

                var projectHi = solutionExplorer.FindUIHierarchyItem(project);

                if (projectHi == null)
                {
                    
                    // todo: logging
                    //Logger.TraceWarning(() => "Unable to locate UIHI for project {0}".FormatWith(project.Name));
                    return false;
                }

                (solutionExplorer.Parent as Window).Activate();

                projectHi.Select(vsUISelectionType.vsUISelectionTypeSelect);

                dte2.ExecuteCommand("Project.UnloadProject", "");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: logging
                //ex.Log();
                return false;
            }
        }

        /// <summary>
        /// Try to retrieve VSProject for a specified Project.
        /// VSProject is a C# or VB.Net project
        /// </summary>
        /// <param name="project"></param>
        /// <param name="vsProject"></param>
        /// <returns></returns>
        public static bool TryGetVSProject(this Project project, out VSProject vsProject)
        {
            vsProject = project.Object as VSProject;

            return vsProject != null;
        }
    }
}
