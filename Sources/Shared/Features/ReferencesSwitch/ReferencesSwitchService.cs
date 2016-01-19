using SquaredInfinity.Foundation;
using System.Linq;
using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.VSCommands.Foundation.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE80;
using SquaredInfinity.VSCommands.Foundation;

namespace SquaredInfinity.VSCommands.Features.ReferencesSwitch
{
    public class ReferencesSwitchService
    {
        IVscSettingsService SettingsService { get; set; }
        DTE2 Dte2 { get; set; }
        IVisualStudioService VisualStudioService { get; set; }

        public ReferencesSwitchService(
            IVscSettingsService settingsService, 
            IServiceProvider serviceProvider,
            IVisualStudioService visualStudioService)
        {
            this.SettingsService = settingsService;
            this.Dte2 = serviceProvider.GetDte2();
            this.VisualStudioService = visualStudioService;
        }

        public ProjectCollection GetSwitchableReferencesByProjectInSolution()
        {
            var result = new ProjectCollection();


            //# save all
            if (Dte2.Solution.IsDirty)
            {
                VisualStudioService.SaveAllChanges();
            }
            
            Dte2.Solution.Projects
            // iterate all projects in solution
            // get packages.config
            // iterate all references and see if they are assembly/project/nuget references

            //# save all
            if (Dte2.Solution.IsDirty)
            {
                VisualStudioService.SaveAllChanges();
            }

            return result;
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
        public Reference From { get; set; }
        public Reference To { get; set; }
    }

    public class SwitchableReference
    {
        public string ProjectUniqueId { get; set; }
        public string OriginalReferenceName { get; set; }
        public Reference OriginalReference { get; set; }
        public Reference SwitchedReference { get; set; }
    }

    public class SwitchableReferenceCollection : ObservableCollectionEx<SwitchableReference>
    {

    }
    
    public class ProjectCollection : ObservableCollectionEx<Project>
    {

    }

    public class Project : NotifyPropertyChangedObject
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }

        string _fullPath;
        public string FullPath
        {
            get { return _fullPath; }
            set { TrySetThisPropertyValue(ref _fullPath, value); }
        }
    }

    public class Reference : NotifyPropertyChangedObject
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { TrySetThisPropertyValue(ref _name, value); }
        }
    }

    public class ProjectReference : Reference
    {

    }

    public class NugetReference : Reference
    {

    }
}
