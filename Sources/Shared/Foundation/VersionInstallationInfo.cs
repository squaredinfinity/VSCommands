using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
{
    public class VersionInstallationInfo
    {
        public DateTime InstalledOnUtc { get; set; }
        public Version Version { get; set; }
        public string MachineName { get; set; }

        public static VersionInstallationInfo GetCurrent()
        {
            return new VersionInstallationInfo
            {
                InstalledOnUtc = DateTime.UtcNow,
                MachineName = Environment.MachineName,
                Version = StaticSettings.CurrentVersion
            };
        }
    }
}
