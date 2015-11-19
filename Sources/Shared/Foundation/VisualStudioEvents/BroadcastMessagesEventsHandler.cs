using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public partial class BroadcastMessagesEventsHandler : IVsBroadcastMessageEvents
    {
        uint Cookie;

        public event EventHandler<MessageBroadcastEventArgs> AfterMessageBroadcast;

        public BroadcastMessagesEventsHandler(IServiceProvider serviceProvider)
        {
            var shellService = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;

            if (shellService != null)
            {
                shellService.AdviseBroadcastMessages(this, out Cookie);
            }
        }

        public int OnBroadcastMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (AfterMessageBroadcast != null)
            {
                AfterMessageBroadcast(this, new MessageBroadcastEventArgs(msg, wParam, lParam));
            }

            return VSConstants.S_OK;
        }
    }
}
