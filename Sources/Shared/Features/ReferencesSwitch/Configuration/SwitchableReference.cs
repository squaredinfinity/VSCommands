using System;
using System.Collections.Generic;
using System.Text;
using VSLangProj;

namespace SquaredInfinity.VSCommands.Features.ReferencesSwitch.Configuration
{
    public class SwitchableReferenceInfo
    {
        public string ProjectUniqueId { get; set; }
        public string OriginalReferenceName { get; set; }
        public Reference OriginalReference { get; set; }
        public Reference SwitchedReference { get; set; }
    }
}
