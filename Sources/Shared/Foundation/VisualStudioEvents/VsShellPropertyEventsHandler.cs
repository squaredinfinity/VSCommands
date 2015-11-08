using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public partial class VsShellPropertyEventsHandler : IVsShellPropertyEvents
    {
        uint Cookie;

        public event EventHandler<ShellPropertyChangeEventArgs> AfterShellPropertyChanged;

        public VsShellPropertyEventsHandler(IServiceProvider serviceProvider)
        {
            var shellService = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;

            if (shellService != null)
            {
                shellService.AdviseShellPropertyChanges(this, out Cookie);
            }
        }

        public int OnShellPropertyChange(int propid, object var)
        {
            if(AfterShellPropertyChanged != null)
            {
                AfterShellPropertyChanged(this, new ShellPropertyChangeEventArgs(propid, var));
            }

            return VSConstants.S_OK;
        }
    }
}
