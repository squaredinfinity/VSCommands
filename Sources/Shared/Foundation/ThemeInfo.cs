using Microsoft.VisualStudio.Shell;
using SquaredInfinity.Foundation.Media.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.VSCommands.Foundation
{
    public static class ThemeInfo
    {
        public static System.Windows.Media.Color GetThemeColor()
        {
            if (!ThreadHelper.CheckAccess())
            {
                return ThreadHelper.Generic.Invoke(() => GetThemeColor());
            }

            var windowColorKey = Microsoft.VisualStudio.PlatformUI.EnvironmentColors.SystemWindowColorKey;
            var windowColor = (System.Windows.Media.Color)Application.Current.MainWindow.FindResource(windowColorKey);

            return windowColor;
        }

        public static bool IsDarkTheme()
        {
            var theme_color = GetThemeColor();

            var scRgb = theme_color.ToScRGBColor();
            var xyz = KnownColorSpaces.scRGB.ToXYZColor(scRgb);
            var lab = (LabColor)KnownColorSpaces.Lab.FromXYZColor(xyz);

            return lab.L < 50;
        }
    }
}
