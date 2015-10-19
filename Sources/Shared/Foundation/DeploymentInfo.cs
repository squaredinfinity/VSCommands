using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
{
    public class DeploymentInfo
    {
        public VersionInstallationInfo InitialVersion { get; set; }
        public VersionInstallationInfo PreviousVersion { get; set; }
        public VersionInstallationInfo CurrentVersion { get; set; }

    }
}
