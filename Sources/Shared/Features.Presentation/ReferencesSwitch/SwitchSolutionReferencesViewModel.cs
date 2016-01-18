using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.VSCommands.Features.ReferencesSwitch
{
    public class SwitchSolutionReferencesViewModel : ViewModel
    {
        ReferencesSwitchService ReferencesSwitchService;

        ProjectCollection _allProjects;
        public ProjectCollection AllProjects
        {
            get { return _allProjects; }
            set { TrySetThisPropertyValue(ref _allProjects, value); }
        }

        public SwitchSolutionReferencesViewModel(ReferencesSwitchService referencesSwitchService)
        {
            this.ReferencesSwitchService = referencesSwitchService;

            RefreshThottle.Invoke(DoRefresh);
        }

        InvocationThrottle RefreshThottle = new InvocationThrottle(min: TimeSpan.FromMilliseconds(50), max: TimeSpan.FromSeconds(2));

        public void Refresh()
        {
            RefreshThottle.Invoke(DoRefresh);
        }

        void DoRefresh()
        {
            var project_collection = ReferencesSwitchService.GetSwitchableReferencesByProjectInSolution();

            AllProjects = project_collection;
        }

        public void Apply()
        {
            EnsureAllReferencesSwitched();
        }

        void EnsureAllReferencesSwitched()
        {
            // get the list of references switch requests,
            // process all of them
        }
    }
}
