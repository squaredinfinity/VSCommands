using SquaredInfinity.Foundation.Presentation;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Presentation.Views;
using SquaredInfinity.Foundation.Presentation.Windows;
using Microsoft.VisualStudio.Shell;

namespace SquaredInfinity.VSCommands.Foundation
{
    public class VscUIService : UIService, IVscUIService
    {
        public void Run(Action action)
        {
            ThreadHelper.Generic.Invoke(action);
        }

        public T Run<T>(Func<T> action)
        {
            return ThreadHelper.Generic.Invoke<T>(action);
        }

        public override IHostAwareViewModel GetDefaultAlertViewModel(string alertMessage, string alertDialogTitle)
        {
            throw new NotImplementedException();
        }

        public override IHostAwareViewModel GetDefaultConfirmationDialogViewModel(string message, string dialogTitle)
        {
            throw new NotImplementedException();
        }

        public override void ShowAlert(View view, DialogScope dialogScope, DialogMode dialogMode)
        {
            throw new NotImplementedException();
        }

        public override IHostAwareViewModel ShowConfirmationDialog(string message, string dialogTitle)
        {
            throw new NotImplementedException();
        }

        public override void ShowDialog(View view, DialogScope dialogScope, DialogMode dialogMode, bool showActivated = true, Func<ViewHostWindow> getWindow = null)
        {
            throw new NotImplementedException();
        }

        public override void ShowToolWindow(View view, Func<ViewHostWindow> getWindow = null)
        {
            throw new NotImplementedException();
        }
    }
}
