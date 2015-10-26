using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    public class SolutionBadgeViewModel : ViewModel
    {
        Color _accentColor = Colors.WhiteSmoke;
        public Color AccentColor
        {
            get { return _accentColor; }
            set { TrySetThisPropertyValue(ref _accentColor, value); }
        }

        Color _textColor;
        public Color TextColor
        {
            get { return _textColor; }
            set { TrySetThisPropertyValue(ref _textColor, value); }
        }

        string _title = "Let's Code!";
        public new string Title
        {
            get { return _title; }
            set { TrySetThisPropertyValue(ref _title, value); }
        }

        string _subtitle;
        public string Subtitle
        {
            get { return _subtitle; }
            set { _subtitle = value; }
        }

        string _activeDocumentName;
        public string ActiveDocumentName
        {
            get { return _activeDocumentName; }
            set { TrySetThisPropertyValue(ref _activeDocumentName, value); }
        }

        bool _isRunMode;
        public bool IsRunMode
        {
            get { return _isRunMode; }
            set { TrySetThisPropertyValue(ref _isRunMode, value); }
        }

        bool _isBreakMode;
        public bool IsBreakMode
        {
            get { return _isBreakMode; }
            set { TrySetThisPropertyValue(ref _isBreakMode, value); }
        }

        bool _isDefaultBranch = true;
        public bool IsDefaultBranch
        {
            get { return _isDefaultBranch; }
            set { TrySetThisPropertyValue(ref _isDefaultBranch, value); }
        }

        bool _isFeatureBranch;
        public bool IsFeatureBranch
        {
            get { return _isFeatureBranch; }
            set { TrySetThisPropertyValue(ref _isFeatureBranch, value); }
        }

        bool _isBugBranch;
        public bool IsBugBranch
        {
            get { return _isBugBranch; }
            set { TrySetThisPropertyValue(ref _isBugBranch, value); }
        }

        bool _isTeamBranch;
        public bool IsTeamBranch
        {
            get { return _isTeamBranch; }
            set { TrySetThisPropertyValue(ref _isTeamBranch, value); }
        }
    }
}
