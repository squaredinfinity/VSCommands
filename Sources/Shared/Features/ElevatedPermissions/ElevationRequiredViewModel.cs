using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.ElevatedPermissions
{
    public class ElevationRequiredViewModel : ViewModel
    {
        public bool ShouldRestartAsAdmin { get; private set; }
        public bool ShouldRestartAsDifferentUser { get; private set; }

        bool _alwaysRunAsAdmin = false;
        public bool AlwaysRunAsAdmin
        {
            get { return _alwaysRunAsAdmin; }
            set { TrySetThisPropertyValue(ref _alwaysRunAsAdmin, value); }
        }

        public ElevationRequiredViewModel()
        {
            this.Title = "This task requires elevated permissions";
        }

        public void RestartAsAdmin()
        {
            this.ShouldRestartAsAdmin = true;
            CompleteInteraction(UserInteractionOutcome.OtherSuccess);
        }

        public void RestartAsDifferentUser()
        {
            this.ShouldRestartAsDifferentUser = true;
            CompleteInteraction(UserInteractionOutcome.OtherSuccess);
        }
    }
}
