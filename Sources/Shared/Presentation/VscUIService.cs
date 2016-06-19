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
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Microsoft.Practices.Unity;

namespace SquaredInfinity.VSCommands.Presentation
{
    public class VscUIService : DefaultUIService, IVscUIService
    {
        public VscUIService() 
            : base(Application.Current.Dispatcher, GetNewDialogWindow, GetNewToolWindow)
        { }

        new static ViewHostWindow GetNewDialogWindow()
        {
            return new SquaredInfinity.Foundation.Presentation.Xaml.Styles.Modern.Windows.DefaultDialogWindow();
        }

        new static ViewHostWindow GetNewToolWindow()
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

        #region Run

        public void Run(Action action)
        {
            ThreadHelper.Generic.Invoke(action);
        }

        public T Run<T>(Func<T> action)
        {
            return ThreadHelper.Generic.Invoke<T>(action);
        }

        #endregion

        #region Tool Window Panes

        public void ShowToolWindowPane<TToolWindow>()
        {
            ShowToolWindowPane<TToolWindow>(VSFRAMEMODE.VSFM_Float);
        }

        public void ShowToolWindowPane<TToolWindow>(VSFRAMEMODE frameMode)
        {
            ShowToolWindowPane(typeof(TToolWindow), frameMode);
        }

        void ShowToolWindowPane(Type toolWindowPaneType, VSFRAMEMODE frameMode)
        {
            try
            {
                var container = VscServices.Instance.Container;

                var package = container.Resolve<Package>();

                var toolWindowPane = package.FindToolWindow(toolWindowPaneType, 0, false);

                bool isAlreadyShown = toolWindowPane != null;

                if (!isAlreadyShown)
                {
                    toolWindowPane = package.FindToolWindow(toolWindowPaneType, 0, true);
                }

                IVsWindowFrame wf = toolWindowPane.Frame as IVsWindowFrame;

                if (!isAlreadyShown)
                {
                    wf.SetProperty((int)__VSFPROPID.VSFPROPID_FrameMode, frameMode);
                }

                wf.Show();
            }
            catch (Exception ex)
            {
                // todo: log
                Trace.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}
