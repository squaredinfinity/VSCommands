using SquaredInfinity.Foundation;
using System.Linq;
using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE80;
using SquaredInfinity.VSCommands.Foundation;
using VSLangProj;
using SquaredInfinity.VSCommands.Foundation.Nuget;
using NuGet.Packaging.Core;
using System.Diagnostics;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;

namespace SquaredInfinity.VSCommands.Features.ReferencesSwitch
{
    public class ReferencesSwitchService
    {
        IVscSettingsService SettingsService { get; set; }
        DTE2 Dte2 { get; set; }
        IVisualStudioService VisualStudioService { get; set; }
        INugetService NugetService { get; set; }

        public ReferencesSwitchService(
            IVscSettingsService settingsService, 
            IServiceProvider serviceProvider,
            IVisualStudioEventsService vsEventsService,
            IVisualStudioService visualStudioService,
            INugetService nugetService)
        {
            this.SettingsService = settingsService;
            this.Dte2 = serviceProvider.GetDte2();
            this.VisualStudioService = visualStudioService;
            this.NugetService = nugetService;
        }

        public MultiMap<EnvDTE.Project, SwitchableReference> GetSwitchableReferencesByProjectInSolution()
        {
            var result = new MultiMap<EnvDTE.Project, SwitchableReference>();

            //# save all
            if (Dte2.Solution.IsDirty)
            {
                VisualStudioService.SaveAllChanges();
            }

            // iterate all projects in solution
            foreach (var project in Dte2.Solution.Projects.ProjectsTreeTraversal())
            {
                // iterate all references and see if they are assembly/project/nuget references
                var vsProject = (VSProject)null;

                if (!project.TryGetVSProject(out vsProject))
                    continue;
                
                for(int i = 0; i < vsProject.References.Count; i++)
                {
                    var reference = vsProject.References.Item(i + 1); //! '1' based index
                    
                    if(reference.SourceProject != null)
                    {
                        // this is a project reference

                        var source_project = reference.SourceProject;

                        var project_ref = new SwitchableProjectReference()
                        {
                            Name = reference.Name,
                            ReferencedProjectFullPath = source_project.FullName
                        };

                        result[project].Add(project_ref);
                        continue;
                    }
                    
                    if(reference.Type == prjReferenceType.prjReferenceTypeAssembly)
                    {
                        // this is either assembly or nuget reference

                        var asm_ref = (SwitchableAssemblyReference)null;

                        if(IsNugetReference(reference))
                        {
                            var package_ref = new SwitchableNugetReference();
                            asm_ref = package_ref;

                            var package_id = NugetService.GetPackageIdentityForFile(reference.Path);

                            if(package_id == null)
                            {
                                // todo: log
                                continue;
                            }

                            package_ref.PackageId = package_id.Id;
                            package_ref.PackageVersion = package_id.Version.ToString(valueWhenNull: "");
                        }
                        else
                        {
                            // todo: for now don't handle direct assembly references
                            continue;

                            asm_ref = new SwitchableAssemblyReference();
                        }

                        asm_ref.Name = reference.Name;
                        asm_ref.FullPath = reference.Path;

                        result.Add(project, asm_ref);

                        continue;

                    }
                }
            }

            return result;
        }

        bool IsNugetReference(Reference reference)
        {
            return reference.Path.Contains("packages", StringComparison.CurrentCultureIgnoreCase);
        }

        public void SwitchReferences(IReadOnlyList<ReferenceSwitchRequest> switchRequests)
        {
            // save solution and all files

            foreach(var sr in switchRequests)
            {
                SwitchReference(sr.From, sr.To);
            }

            // save solution and all files
        }

        void SwitchReference(Reference oldReference, Reference newReference)
        {

        }

        public Dictionary<string, FileInfo> DiscoverProjectsPaths(IReadOnlyList<string> projectNames, DirectoryInfo seachRootDirectory)
        {
            var result = new Dictionary<string, FileInfo>(capacity: projectNames.Count);

            // enumerate project files under the root, looking in subdirectories
            foreach(var file in seachRootDirectory.EnumerateFiles("*.csproj", SearchOption.AllDirectories))
            {
                if(projectNames.Contains(file.Name, StringComparer.CurrentCultureIgnoreCase))
                {
                    result[file.Name] = file;
                }
            }

            return result;
        }
    }

    public class ReferenceSwitchRequest
    {
        string ProjectUniqueId { get; set; }

        public Reference From { get; set; }
        public Reference To { get; set; }
    }
    
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class SwitchableReference : NotifyPropertyChangedObject
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }

        public string DebuggerDisplay
        {
            get { return Name.ToString(valueWhenNull: "[NULL]"); }
        }
    }

    public class SwitchableProjectReference : SwitchableReference
    {
        string _referencedProjectFullPath;
        public string ReferencedProjectFullPath
        {
            get { return _referencedProjectFullPath; }
            set { TrySetThisPropertyValue(ref _referencedProjectFullPath, value); }
        }
    }

    public class SwitchableAssemblyReference : SwitchableReference
    {
        string _fullPath;
        public string FullPath
        {
            get { return _fullPath; }
            set { TrySetThisPropertyValue(ref _fullPath, value); }
        }
    }

    public class SwitchableNugetReference : SwitchableAssemblyReference
    {
        //! This type may be persisted, so it's important that no properties with 3rd party types are exposed (in case the type definitions change)
        public string PackageId { get; set; }
        public string PackageVersion { get; set; }
    }
}
