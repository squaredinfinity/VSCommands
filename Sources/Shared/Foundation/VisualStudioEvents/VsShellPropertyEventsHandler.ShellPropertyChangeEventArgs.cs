using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public partial class VsShellPropertyEventsHandler : IVsShellPropertyEvents
    {
        public class ShellPropertyChangeEventArgs : EventArgs
        {
            public int PropId { get; private set; }
            public object Var { get; private set; }

            public ShellPropertyChangeEventArgs(int propId, object var)
            {
                this.PropId = propId;
                this.Var = var;
            }
        }
    }
}
