using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    public class SolutionBadgeViewModel : ViewModel
    {
        Color _accentColor = Colors.DeepPink;
        public Color AccentColor
        {
            get { return _accentColor; }
            set { TrySetThisPropertyValue(ref _accentColor, value); }
        }
    }
}
