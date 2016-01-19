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

namespace SquaredInfinity.VSCommands.Foundation
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
                // todo: log
                //Logger.LogException(ex);
            }

            return result;
        }
    }
}
