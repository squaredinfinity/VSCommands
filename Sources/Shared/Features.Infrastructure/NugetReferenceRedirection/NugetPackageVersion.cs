using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NugetPackageVersion
    {
        public string Version { get; set; }
        public IReadOnlyList<string> Targets { get; set; }
        public string DirectoryFullPath { get; set; }

        public string DebuggerDisplay
        {
            get { return $"{Version}"; }
        }
    }
}
