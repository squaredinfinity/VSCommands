using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    public class SolutionBadgeView : View<SolutionBadgeViewModel>
    {
        public SolutionBadgeView()
        {
            base.RefreshViewModelOnDataContextChange = false;
        }
    }
}
