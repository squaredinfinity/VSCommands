using EnvDTE80;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.VSCommands.Foundation;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SquaredInfinity.VSCommands.Features.ReferencesSwitch
{
    public class SwitchSolutionReferencesViewModel : ViewModel
    {
        ReferencesSwitchService ReferencesSwitchService;
        IVisualStudioEventsService VisualStudioEventsService { get; set; }

        DTE2  Dte2 { get; set; }

        public SwitchSolutionReferencesViewModel(
            IVisualStudioEventsService vsEventsService, 
            ReferencesSwitchService referencesSwitchService,
            IServiceProvider serviceProvider)
        {
            this.ReferencesSwitchService = referencesSwitchService;
            this.VisualStudioEventsService = vsEventsService;
            Dte2 = serviceProvider.GetDte2();

            RefreshThottle.Invoke(DoRefresh);

            VisualStudioEventsService.AfterSolutionOpened += VisualStudioEventsService_AfterSolutionOpened;
        }

        protected override void DisposeManagedResources()
        {
            VisualStudioEventsService.AfterSolutionOpened -= VisualStudioEventsService_AfterSolutionOpened;

            base.DisposeManagedResources();
        }

        void VisualStudioEventsService_AfterSolutionOpened(object sender, EventArgs e)
        {
            Refresh();
        }

        InvocationThrottle RefreshThottle = new InvocationThrottle(min: TimeSpan.FromMilliseconds(50), max: TimeSpan.FromSeconds(2));

        public void Refresh()
        {
            RefreshThottle.Invoke(DoRefresh);
        }

        void DoRefresh()
        {
            try
            {
                var references_by_project = ReferencesSwitchService.GetSwitchableReferencesByProjectInSolution();

                var projects = Dte2.Solution.Projects.ProjectsTreeTraversal().ToArray();

                Trace.WriteLine("lol");
            }
            catch(Exception ex)
            {
                // todo: log
                Debug.WriteLine(ex.ToString());
            }
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
