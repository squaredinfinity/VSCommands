using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Win32Api;
using SquaredInfinity.VSCommands.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.ElevatedPermissions
{
    public class ElevatedPermissionsService : IElevatedPermissionsService
    {
        IVscUIService UIService { get; set; }
        IVisualStudioService VSService { get; set; }

        public ElevatedPermissionsService(IVscUIService uiService, IVisualStudioService vsService)
        {
            this.UIService = uiService;
            this.VSService = vsService;
        }

        public bool EnsureElevatedPermissions()
        {
            if (shell32.IsUserAnAdmin())
                return true;

            var v = new ElevationRequiredView();

            UIService.ShowDialog(v);

            var vm = v.ViewModel;

            if (vm.InteractionOutcome != UserInteractionOutcome.OtherSuccess)
                return false;

            if (vm.AlwaysRunAsAdmin)
            {
                VSService.TrySetAlwaysRunAsAdmin();
            }
            else
            {
                VSService.TrySetAlwaysRunAsAdmin(enable: false);
            }

            if (vm.ShouldRestartAsAdmin)
            {
                VSService.RestartVisualStudio(forceAsAdmin: true, saveAllChanges: true, asDifferentUser: false);
                return false;
            }
            else
            {
                VSService.RestartVisualStudio(forceAsAdmin: true, saveAllChanges: true, asDifferentUser: true);
                return false;
            }
        }
    }
}
