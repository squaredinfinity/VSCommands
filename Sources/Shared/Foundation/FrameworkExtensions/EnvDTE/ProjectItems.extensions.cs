using EnvDTE;
using SquaredInfinity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;

namespace SquaredInfinity.VSCommands
{
    public static class ProjectItemsExtensions
    {
        /// <summary>
        /// Returns child Project Items
        /// </summary>
        /// <param name="projectItems"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectItem> TreeTraversal(this ProjectItems projectItems)
        {
            //! COM: don't use iterator in this method

            var result = new List<ProjectItem>();

            try
            {
                if (projectItems == null)
                    return result;

                //! COM: indexing starts at 1
                for (int i = 1; i <= projectItems.Count; i++)
                {
                    var pi = projectItems.Item(i);

                    var piChildren = pi.TreeTraversal(TreeTraversalMode.DepthFirst, GetChildrenFunc).ToList();

                    //! not COM: indexing starts at 0
                    for (int i2 = 0; i2 < piChildren.Count; i2++)
                    {
                        var pi2 = piChildren[i2];

                        result.Add(pi2);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: log
                //ex.Log();
            }

            return result;
        }

        /// <summary>
        /// Returns children of specified ProjectItem
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<ProjectItem> GetChildrenFunc(ProjectItem parent)
        {
            //! COM: don't use iterator in this method

            var result = new List<ProjectItem>();

            if (parent == null)
                return result;

            var projectItems = parent.ProjectItems;

            try
            {
                //# solution folders have subprojects
                //# we want to iterate them too

                if (projectItems == null)
                {
                    Project subProject = null;

                    //! Wrap in try/catch
                    //! SubProject getter is not guaranteed to be implemented, in some cases it may throw NotImplementedException

                    try
                    {
                        subProject = parent.SubProject;

                        if (subProject != null)
                        {
                            var subProjectItems = subProject.ProjectItems;

                            try
                            {
                                var subProjectItemsList = subProjectItems.TreeTraversal().ToList();

                                for (int i = 0; i < subProjectItemsList.Count; i++)
                                {
                                    var spi = subProjectItemsList[i];

                                    result.Add(spi);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                                // todo: log
                                //Logger.DiagnosticOnlyLogException(ex);
                            }
                            finally
                            {
                                subProjectItems.SafeReleaseComObject();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        // todo: log exception
                        //Logger.SwallowException(ex, "ProjectItem doesn't expose any way to check if SubProject property is supported");
                    }
                    finally
                    {
                        subProject.SafeReleaseComObject();
                        subProject = null;
                    }
                }

                if (projectItems == null)
                    return result;

                // return child project items
                //! COM: indexing starts at 1
                for (int i = 1; i <= projectItems.Count; i++)
                {
                    var pi = projectItems.Item(i);

                    result.Add(pi);
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
    }
}
