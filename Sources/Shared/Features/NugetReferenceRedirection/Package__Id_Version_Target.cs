using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    struct Package__Id_Version_Target
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Target { get; set; }

        public string DebuggerDisplay
        {
            get { return $"{Id}, {Version}, {Target}"; }
        }
    }
}
