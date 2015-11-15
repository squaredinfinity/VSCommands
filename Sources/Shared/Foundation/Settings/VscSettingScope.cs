using SquaredInfinity.Foundation.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.Settings
{
    public class VscSettingScope : SettingScope
    {
        /// <summary>
        /// Solution for all users
        /// </summary>
        public static int Solution = 100;
        /// <summary>
        /// Solution for current user
        /// </summary>
        public static int UserSolution = 101;
        /// <summary>
        /// Project for all users
        /// </summary>
        public static int Project = 102;
        /// <summary>
        /// Project for current user
        /// </summary>
        public static int UserProject = 103;
    }
}
