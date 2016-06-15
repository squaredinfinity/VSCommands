using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NugetPackage
    {
        public string Id { get; set; }

        public IReadOnlyList<NugetPackageVersion> Versions { get; set; }

        public string DebuggerDisplay
        {
            get { return $"{Id}"; }
        }
    }
}
