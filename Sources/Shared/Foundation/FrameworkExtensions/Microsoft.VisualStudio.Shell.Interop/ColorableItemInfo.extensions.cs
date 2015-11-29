using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace SquaredInfinity.VSCommands
{
    public static class ColorableItemInfoExtensions
    {
        public static Color GetForeground(this ColorableItemInfo cii)
        {
            return GetColor(cii.crForeground);
        }

        public static Color GetBackground(this ColorableItemInfo cii)
        {
            return GetColor(cii.crBackground);
        }

        static Color GetColor(uint color)
        {
            if (color == 1)
                return Colors.Transparent;

            var match = Regex.Match(color.ToString("x6"), @"(?<blue>..)(?<green>..)(?<red>..)");

            var r = byte.Parse(match.Groups["red"].Value, NumberStyles.HexNumber);
            var g = byte.Parse(match.Groups["green"].Value, NumberStyles.HexNumber);
            var b = byte.Parse(match.Groups["blue"].Value, NumberStyles.HexNumber);

            return Color.FromRgb(r, g, b);
        }
    }
}
