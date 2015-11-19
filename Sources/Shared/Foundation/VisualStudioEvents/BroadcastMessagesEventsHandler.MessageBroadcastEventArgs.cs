using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public partial class BroadcastMessagesEventsHandler
    {
        public class MessageBroadcastEventArgs : EventArgs
        {
            public uint Msg { get; private set; }
            public IntPtr wParam { get; private set; }
            public IntPtr lParam { get; private set; }

            public MessageBroadcastEventArgs(uint msg, IntPtr wParam, IntPtr lParam)
            {
                this.Msg = msg;
                this.wParam = wParam;
                this.lParam = lParam;
            }
        }
    }
}
