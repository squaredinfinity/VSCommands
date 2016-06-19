using EnvDTE;
using SquaredInfinity.VSCommands.Foundation.Solution;
using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace SquaredInfinity.VSCommands
{
    public static class UIHierarchyItemExtensions
    {
        public static UIHierarchyItem FindItemByName(this UIHierarchyItem uiHierarchyItem, string itemName)
        {
            var result =
                from ui in uiHierarchyItem.TreeTraversal()
                where ui.Name == itemName
                select ui;

            return result.FirstOrDefault();
        }

        public static IEnumerable<UIHierarchyItem> TreeTraversal(this UIHierarchyItem me)
        {
            foreach (var child in me.TreeTraversal(GetChildrenFunc))
            {
                yield return child;
            }
        }

        public static IEnumerable<UIHierarchyItem> GetChildrenFunc(UIHierarchyItem parent)
        {
            if (parent == null)
            {
                // todo: log
                //Logger.TraceDiagnosticWarning(() => "Unable to find Children for [Null] UIHierarchyItem");
                yield break;
            }

            UIHierarchyItems childHierarchyItems = null;

            try
            {
                // todo: this fails sometime, not sure why
                childHierarchyItems = parent.UIHierarchyItems;
            }
            catch (Exception)
            {
                // todo
                //ex.Swallow("UI Hierarchy out of date?");
            }

            if (childHierarchyItems == null)
            {
                // todo: log
                //Logger.TraceDiagnostic(() => "Child UIHierarchyItems is [Null]");
            }
            else
            {
                foreach (UIHierarchyItem child in childHierarchyItems)
                {
                    yield return child;
                }
            }

            yield break;
        }

        public static bool IsProject(this UIHierarchyItem uiHierarchyItem)
        {
            bool isProject = uiHierarchyItem.Object is Project
                             && (uiHierarchyItem.Object as Project).Kind.ToLower() != SolutionItemKindGuids.ProjectAsSolutionFolder_AsString;

            bool isProjectItemAsProject = uiHierarchyItem.Object is ProjectItem
                                          && ((uiHierarchyItem.Object as ProjectItem).Object is Project)
                                          && ((uiHierarchyItem.Object as ProjectItem).Object as Project).Kind.ToLower() != SolutionItemKindGuids.ProjectItemAsSolutionFolder_AsString;

            return isProject || isProjectItemAsProject;
        }

        public static bool IsSolutionFolder(this UIHierarchyItem uiHierarchyItem)
        {
            bool isSolutionFolder = uiHierarchyItem.Object is Project
                                    && (uiHierarchyItem.Object as Project).Kind.ToLower() == SolutionItemKindGuids.ProjectAsSolutionFolder_AsString;

            return isSolutionFolder;
        }

        //? should it be returned as SolutionFolder instead?
        public static Project AsSolutionFolder(this UIHierarchyItem uiHierarchyItem)
        {
            bool isSolutionFolder = uiHierarchyItem.Object is Project
                                    && (uiHierarchyItem.Object as Project).Kind.ToLower() == SolutionItemKindGuids.ProjectAsSolutionFolder_AsString;

            if (!isSolutionFolder)
            {
                //todo: log
                //Logger.TraceDiagnostic(() => "UIHierarchyItem is not a Solution Folder");
                return null;
            }

            return uiHierarchyItem.Object as Project;
        }

        public static Project AsProject(this UIHierarchyItem uiHierarchyItem)
        {
            bool isProject = uiHierarchyItem.Object is Project
                             && (uiHierarchyItem.Object as Project).Kind.ToLower() != SolutionItemKindGuids.ProjectAsSolutionFolder_AsString;

            bool isProjectItemAsProject = uiHierarchyItem.Object is ProjectItem
                                          && ((uiHierarchyItem.Object as ProjectItem).Object is Project)
                                          && ((uiHierarchyItem.Object as ProjectItem).Object as Project).Kind.ToLower() != SolutionItemKindGuids.ProjectItemAsSolutionFolder_AsString;

            if (isProject)
                return uiHierarchyItem.Object as Project;

            if (isProjectItemAsProject)
                return ((uiHierarchyItem.Object as ProjectItem).Object as Project);

            // todo: log
            //Logger.TraceDiagnostic(() => "UIHierarchy Item is not a Project");

            return null;
        }

        public static string TryGetName(this UIHierarchyItem uiHierarchyItem)
        {
            string result = string.Empty;

            try
            {
                result = uiHierarchyItem.Name;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //todo: log
                //ex.Swallow("Some project items may throw exceptions when accessing Name property");
            }

            return result;
        }
    }
}
