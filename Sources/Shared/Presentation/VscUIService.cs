using SquaredInfinity.Foundation.Presentation;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Presentation.Views;
using SquaredInfinity.Foundation.Presentation.Windows;
using Microsoft.VisualStudio.Shell;
using SquaredInfinity.VSCommands.Foundation;
using System.Windows.Threading;
using System.Windows;
using SquaredInfinity.Foundation.Presentation.Styles.Modern;

namespace SquaredInfinity.VSCommands.Presentation
{
    public class VscUIService : DefaultUIService, IVscUIService
    {
        public VscUIService() 
            : base(Application.Current.Dispatcher, GetNewDialogWindow, GetNewToolWindow)
        { }

        static ViewHostWindow GetNewDialogWindow()
        {
            return new SquaredInfinity.Foundation.Presentation.Xaml.Styles.Modern.Windows.DefaultDialogWindow();
        }

        static ViewHostWindow GetNewToolWindow()
        {
            return new SquaredInfinity.Foundation.Presentation.Xaml.Styles.Modern.Windows.ModernWindow();
        }

        protected override void PrepareViewForDisplay(ViewHostWindow viewHost, View view)
        {
            base.PrepareViewForDisplay(viewHost, view);

            var fe = viewHost as FrameworkElement;
            if(fe != null)
            {
                DefaultXamlResources.ApplyAllStyles(fe.Resources);
            }

            fe = view as FrameworkElement;
            if(fe != null)
            {
                DefaultXamlResources.ApplyAllStyles(fe.Resources);
            }
        }


        public void Run(Action action)
        {
            ThreadHelper.Generic.Invoke(action);
        }

        public T Run<T>(Func<T> action)
        {
            return ThreadHelper.Generic.Invoke<T>(action);
        }
    }
}
