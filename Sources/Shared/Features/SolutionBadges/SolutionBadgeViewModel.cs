using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using System.Windows.Media;
using SquaredInfinity.Foundation.Media.ColorSpaces;

namespace SquaredInfinity.VSCommands.Features.SolutionBadges
{
    public class SolutionBadgeViewModel : ViewModel
    {
        System.Windows.Media.Color _accentColor = Colors.WhiteSmoke;
        public System.Windows.Media.Color AccentColor
        {
            get { return _accentColor; }
            set { TrySetThisPropertyValue(ref _accentColor, value); }
        }

        System.Windows.Media.Color _textColor;
        public System.Windows.Media.Color TextColor
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
            set { TrySetThisPropertyValue(ref _subtitle, value); }
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

        public void RefreshFrom(IDictionary<string, object> properties)
        {
            var sln_is_open = (bool) properties.GetValueOrDefault("sln:isOpen", () => false);

            if(sln_is_open)
            {
                Title = AsNiceString((string) properties.GetValueOrDefault("sln:fileName", () => ""));
            }

            var branch_name = AsNiceString((string)properties.GetValueOrDefault(KnownProperties.BranchName, () => ""));

            Subtitle = branch_name;

            ActiveDocumentName = (string)properties.GetValueOrDefault("activeDocument:fileName", () => "");

            AccentColor = (System.Windows.Media.Color)properties.GetValueOrDefault(KnownProperties.AccentColor, () => Colors.WhiteSmoke);
            TextColor = GetTextColorFromAccent(AccentColor);
        }

        System.Windows.Media.Color GetTextColorFromAccent(System.Windows.Media.Color accent)
        {
            var scRgb = accent.ToScRGBColor();
            var xyz = KnownColorSpaces.scRGB.ToXYZColor(scRgb);
            var lab = (LabColor)KnownColorSpaces.Lab.FromXYZColor(xyz);

            if (lab.L >= 95)
                return System.Windows.Media.Color.FromRgb(42, 42, 42);
            else if (lab.L >= 50)
                return System.Windows.Media.Colors.Black;
            else
                return System.Windows.Media.Colors.White;
        }

        string AsNiceString(string str)
        {
            return 
                str.Replace('.', ' ')
                .Replace('-', ' ')
                .Replace('_', ' ')
                .SplitCamelCase();
        }
    }

    // TODO: move to foundation
    public static class IDictionaryExtensions
    {
        public static void IfContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Action<TValue> action)
        {
            if (dict.ContainsKey(key))
                action(dict[key]);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getDefaultValue)
        {
            if (dict.ContainsKey(key))
                return dict[key];

            return getDefaultValue();
        }
    }
}
