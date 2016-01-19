using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SquaredInfinity.VSCommands.Foundation.Solution;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
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
                // todo: log
                //ex.TryAddContextData("propertyName", () => propertyName);
                //ex.TryAddContextData("newValue", () => newValue);
                //Logger.LogException(ex);
                return false;
            }
        }
    }
}
