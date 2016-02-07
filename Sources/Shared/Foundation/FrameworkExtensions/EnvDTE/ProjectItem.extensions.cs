using EnvDTE;
using SquaredInfinity.VSCommands.Foundation.Solution;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using SquaredInfinity.VSCommands.Foundation;

namespace SquaredInfinity.VSCommands
{
    public static class ProjectItemExtensions
    {
        public static SolutionNodeType GetSolutionNodeType(this ProjectItem projectItem)
        {
            switch (projectItem.Kind.ToLower())
            {
                // Folder
                case SolutionItemKindGuids.Folder_AsString:
                    return SolutionNodeType.Folder;
                // File
                case SolutionItemKindGuids.File_AsString:
                    return SolutionNodeType.File;
                // Solution Folder
                case SolutionItemKindGuids.ProjectItemAsSolutionFolder_AsString:
                    return SolutionNodeType.SolutionFolder;
            }

            
            // todo: log
            //Logger.TraceDiagnosticWarning(() => "Unknown project item type: " + projectItem.Kind);

            return SolutionNodeType.Unknown;
        }

        public static object GetPropertyValue<T>(this ProjectItem projectItem, string propertyName)
        {
            var properties = projectItem.Properties;

            var property = properties.Item(propertyName);

            var result = property.Value;

            return (T) result;
        }

        public static bool HasChildItems(this ProjectItem projectItem)
        {
            var childItems = projectItem.ProjectItems;

            return childItems.Count > 0;
        }

        /// <summary>
        /// Return full path of a specified ProjectItem
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static string GetFullPath(this ProjectItem pi)
        {
            //! I seriously do not understand the rules for getting file names
            //! For example, sometimes when pi.FileCount == 1 calling pi.Get_FileNames(0) will throw exception but .Get_FileNames(1) will work
            //! Another time pi.FileCount == 1 but bot (0) and (1) as parameters work.
            //! Generally it seems that .fet_FileNames(1) should be safe to call, but if it fails then try (0) just in case

            var result = string.Empty;

            bool triedZeroIndex = false;

            try
            {
                result = pi.get_FileNames(1);

                if (result.IsNullOrEmpty())
                {
                    triedZeroIndex = true;
                    result = pi.get_FileNames(0);
                }
            }
            catch
            {
                if (!triedZeroIndex)
                {
                    SafeBlock.Run(() =>
                    {
                        result = pi.get_FileNames(0);
                    }, swallowExceptions: true);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns an IVsTextView for the given file path, if the given file is open in Visual Studio.
        /// </summary>
        /// <returns>The IVsTextView for this file, if it is open, null otherwise.</returns>
        public static IVsTextView GetIVsTextView(this ProjectItem projectItem)
        {
            var filePath = projectItem.GetFullPath();

            IVsUIHierarchy uiHierarchy;
            uint itemID;
            IVsWindowFrame windowFrame;

            if (!VsShellUtilities.IsDocumentOpen(
                ServiceProvider.GlobalProvider,
                filePath,
                Guid.Empty,
                out uiHierarchy,
                out itemID,
                out windowFrame))
            {
                return null;
            }

            // Get the IVsTextView from the windowFrame.
            return VsShellUtilities.GetTextView(windowFrame);
        }

        public static bool IsLink(this ProjectItem projectItem)
        {
            bool result = false;

            try
            {
                result = (bool) projectItem.Properties.Item("IsLink").Value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                // todo: log
                //Logger.LogException(ex);
            }

            return result;
        }

        /// <summary>
        /// Will try to find UIHierarchyItem item for specified projectItem.
        /// If ProjectItem is not visible (i.e. its parent is collapsed) then it may not have UIHierarchyItem
        /// If UIHierarchyItem does not exist, it will try to expand Project Item parents to make force its creation.
        /// </summary>
        public static UIHierarchyItem FindUIHierarchyItem(this ProjectItem projectItem)
        {
            var dte2 = ServiceProvider.GlobalProvider.GetDte2();

            var toolWindows = dte2.ToolWindows;
            var solutionExplorer = toolWindows.SolutionExplorer;

            var solutionItems = solutionExplorer.UIHierarchyItems;
            var solutionNode = solutionItems.Item(1);

            return FindUIHierarchyItem(solutionNode.UIHierarchyItems, projectItem);
        }

        public static UIHierarchyItem FindUIHierarchyItem(UIHierarchyItems hItems, object item)
        {
            var dte2 = ServiceProvider.GlobalProvider.GetDte2();
            var toolWindows = dte2.ToolWindows;
            var solutionExplorer = toolWindows.SolutionExplorer;

            Stack<object> hierarchyStack = CreateItemHierarchyStack(item);

            UIHierarchyItems items = hItems;
            UIHierarchyItem lastHItem = null;

            while (hierarchyStack.Count > 0)
            {
                lastHItem = null;

                if (!items.Expanded)
                    items.Expanded = true;

                //! there seems to be a bug in VS when setting Expanded will not always work so try again with different approach if needed
                if (!items.Expanded)
                {
                    UIHierarchyItem parent = items.Parent as UIHierarchyItem;
                    parent.Select(vsUISelectionType.vsUISelectionTypeSelect);
                    solutionExplorer.DoDefaultAction();
                }

                object obj = hierarchyStack.Pop();

                var op = obj as Project;
                var opi = obj as ProjectItem;

                for (int i = 0; i < items.Count; i++)
                {
                    var hi = items.Item(i + 1);

                    var p1 = hi.Object as Project;
                    var pi1 = hi.Object as ProjectItem;

                    bool isMatch = false;

                    if (p1 != null && op != null)
                    {
                        isMatch = p1.Object == op.Object;
                    }
                    if (pi1 != null && opi != null)
                    {
                        isMatch = pi1.Object == opi.Object;
                    }

                    if (isMatch)
                    {
                        lastHItem = hi;
                        items = hi.UIHierarchyItems;
                        break;
                    }
                }
            }

            return lastHItem;
        }

        static Stack<object> CreateItemHierarchyStack(object item)
        {
            return CreateItemHierarchyStack(new Stack<object>(), item);
        }

        static Stack<object> CreateItemHierarchyStack(Stack<object> stack, object item)
        {
            if (item is ProjectItem)
            {
                var pi = item as ProjectItem;

                stack.Push(pi);

                var collection = pi.Collection;
                CreateItemHierarchyStack(stack, collection.Parent);
            }
            else if (item is Project)
            {
                var p = item as Project;

                stack.Push(p);

                if (p.ParentProjectItem != null) // this may be null if a solution node is above
                {
                    CreateItemHierarchyStack(stack, p.ParentProjectItem);
                }
            }
            else
            {
                // todo: log
                Debug.Fail("Unknown item in hierarchy");
                //Logger.TraceDiagnostic(() => "Unknown item in hierarchy");
            }

            return stack;
        }
    }
}
