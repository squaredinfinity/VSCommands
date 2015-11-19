using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Win32Api;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public partial class VisualStudioEventsService : IVisualStudioEventsService
    {
        public event EventHandler<EventArgs> AfterVisualStudioThemeChanged;

        public event EventHandler<DocumentEventArgs> AfterActiveDocumentChanged;
        public event EventHandler<EventArgs> AfterSolutionClosed;
        public event EventHandler<EventArgs> AfterSolutionOpened;

        public event EventHandler<EventArgs> AfterDebuggerEnterDesignMode;
        public event EventHandler<EventArgs> AfterDebuggerEnterRunMode;
        public event EventHandler<EventArgs> AfterDebuggerEnterBreakMode;

        public event EventHandler<EventArgs> AfterActiveSolutionConfigChanged;

        public event _dispBuildEvents_OnBuildBeginEventHandler OnBuildBegin;
        public event _dispBuildEvents_OnBuildDoneEventHandler OnBuildDone;
        public event _dispBuildEvents_OnBuildProjConfigBeginEventHandler OnBuildProjConfigBegin;
        public event _dispBuildEvents_OnBuildProjConfigDoneEventHandler OnBuildProjConfigDone;

        readonly IVscUIService UIService;
        readonly IServiceProvider ServiceProvider;

        readonly object UILoadedSync = new object();

        Events2 Events;
        DTEEvents DTEEvents;
        BuildEvents BuildEvents;
        SolutionEvents SolutionEvents;
        DocumentEvents DocumentEvents;
        WindowEvents WindowEvents;
        DebuggerEvents DebuggerEvents;
        CommandEvents CommandEvents;
        SelectionEvents SelectionEvents;

        public bool HasShutdownStarted { get; private set; }

        readonly VsShellPropertyEventsHandler VsShellPropertyEvents;
        readonly BroadcastMessagesEventsHandler BroadsMessagesEvents;

        /// <summary>
        /// Actions to be executed when Visual Studio UI is loaded
        /// </summary>
        ConcurrentQueue<Action> UILoadedActions = new ConcurrentQueue<Action>();
        
        public VisualStudioEventsService(IVscUIService uiService, IServiceProvider serviceProvider)
        {
            this.UIService = uiService;
            this.ServiceProvider = serviceProvider;

            VsShellPropertyEvents = new VsShellPropertyEventsHandler(serviceProvider);
            VsShellPropertyEvents.AfterShellPropertyChanged += VsShellPropertyEvents_AfterShellPropertyChanged;

            BroadsMessagesEvents = new BroadcastMessagesEventsHandler(serviceProvider);
            BroadsMessagesEvents.AfterMessageBroadcast += BroadsMessagesEvents_AfterMessageBroadcast;
        }

        void BroadsMessagesEvents_AfterMessageBroadcast(object sender, BroadcastMessagesEventsHandler.MessageBroadcastEventArgs e)
        {
            SafeExecute(() =>
            {
                if (e.Msg == WM.THEMECHANGED || e.Msg == WM.SYSCOLORCHANGE)
                {
                    if (AfterVisualStudioThemeChanged != null)
                        AfterVisualStudioThemeChanged(this, EventArgs.Empty);
                }
            });
        }

        void VsShellPropertyEvents_AfterShellPropertyChanged(object sender, VsShellPropertyEventsHandler.ShellPropertyChangeEventArgs e)
        {
            SafeExecute(() =>
            {
                // when zombie state changes to false, finish package initialization
                //! DO NOT USE CODE WHICH MAY EXECUTE FOR LONG TIME HERE

                if ((int)__VSSPROPID.VSSPROPID_Zombie == e.PropId)
                {
                    if ((bool)e.Var == false)
                    {

                        var dte2 = ServiceProvider.GetDte2();

                        Events = dte2.Events as Events2;
                        DTEEvents = Events.DTEEvents;
                        SolutionEvents = Events.SolutionEvents;
                        DocumentEvents = Events.DocumentEvents;
                        WindowEvents = Events.WindowEvents;
                        DebuggerEvents = Events.DebuggerEvents;
                        CommandEvents = Events.CommandEvents;
                        SelectionEvents = Events.SelectionEvents;

                        DelayedInitialise();
                    }
                }
            });
        }

        public void RegisterVisualStudioUILoadedAction(Action action)
        {
            //! this methid must run on UI thread
            //  because it accesses Main Window
            if (!UIService.IsUIThread)
            {
                UIService.Run(() => RegisterVisualStudioUILoadedAction(action));
                return;
            }

            if (Application.Current.MainWindow == null)
            {
                UILoadedActions.Enqueue(action);

                // Main Window not yet created, listen to Activated event and perform actions when it is raised

                // make sure the handler is added only once (it is safe to try to remove handler even if it's not there)
                Application.Current.Activated -= UILOADED__CurrentApplication_Activated;
                Application.Current.Activated += UILOADED__CurrentApplication_Activated;
            }
            else
            {
                HwndSource hwndSource = HwndSource.FromDependencyObject(Application.Current.MainWindow) as HwndSource;

                if (hwndSource == null)
                {
                    // Main Window created but not yet ready

                    UILoadedActions.Enqueue(action);


                    // make sure the handler is added only once (it is safe to try to remove handler even if it's not there)
                    Application.Current.MainWindow.LayoutUpdated -= UILOADED__MainWindow_LayoutUpdated;
                    Application.Current.MainWindow.LayoutUpdated += UILOADED__MainWindow_LayoutUpdated;
                }
                else
                {
                    action();
                }
            }
        }

        private void UILOADED__CurrentApplication_Activated(object sender, EventArgs e)
        {
            var w = sender as System.Windows.Window;
            w.Activated -= UILOADED__CurrentApplication_Activated;
            RunUILoadedActions();
        }

        private void UILOADED__MainWindow_LayoutUpdated(object sender, EventArgs e)
        {
            HwndSource hwndSource = HwndSource.FromDependencyObject(Application.Current.MainWindow) as HwndSource;
            if (hwndSource == null)
                return;

            // sender comes as null so can't use it here
            Application.Current.MainWindow.LayoutUpdated -= UILOADED__MainWindow_LayoutUpdated;
            RunUILoadedActions();
        }

        void RunUILoadedActions()
        {
            SafeExecute(() =>
            {
                lock (UILoadedSync)
                {
                    for (int i = UILoadedActions.Count; i >= 0; i--)
                    {
                        try
                        {
                            Action action = null;

                            if (!UILoadedActions.TryDequeue(out action))
                                return;

                            action();
                        }
                        catch (Exception ex)
                        {
                            // todo: log exception
                            Trace.WriteLine(ex.ToString());
                        }
                    }
                }
            });
        }

        void DelayedInitialise()
        {
            //+ DTE EVENTS
            //DTEEvents.OnBeginShutdown += () =>
            //{
            //    HasShutdownStarted = true;

            //    if (OnBeginShutdown != null)
            //        OnBeginShutdown(this, EventArgs.Empty);
            //};

            //+ SOLUTION EVENTS
            SolutionEvents.Opened += () =>
            {
                SafeExecute(() =>
                {
                    TryRegisterBuildEventsIfNeeded();

                    if (AfterSolutionOpened != null)
                        AfterSolutionOpened(this, EventArgs.Empty);
                });
            };

            SolutionEvents.AfterClosing += () =>
            {
                SafeExecute(() =>
                {
                    if (AfterSolutionClosed != null)
                        AfterSolutionClosed(this, EventArgs.Empty);
                });
            };

            //+ DOCUMENT EVENTS
            //DocumentEvents.DocumentOpened += (document) =>
            //{
            //    SafeExecute(() =>
            //    {
            //        if (AfterDocumentOpened != null)
            //        {
            //            var documentInfo = new DocumentInfo(document.FullName, document.ProjectItem);

            //            AfterDocumentOpened(this, new EventArgs<DocumentInfo>(documentInfo));
            //        }
            //    });
            //};

            //DocumentEvents.DocumentOpening += (path, readOnly) =>
            //{
            //    SafeExecute(() =>
            //    {
            //    });
            //};

            //+ WINDOW EVENTS
            WindowEvents.WindowActivated += (gotFocus, lostFocus) =>
            {
                SafeExecute(() =>
                {
                    if (AfterActiveDocumentChanged != null)
                    {
                        if (gotFocus != null
                            && gotFocus.Document != null)
                        {
                            var document = gotFocus.Document;

                            var args = new DocumentEventArgs(document.FullName, document.ProjectItem);

                            AfterActiveDocumentChanged(this, args);
                        }
                    }
                });
            };

            //+ DEBUGGER EVENTS
            DebuggerEvents.OnEnterDesignMode += (reason) =>
            {
                SafeExecute(() =>
                {
                    if (AfterDebuggerEnterDesignMode != null)
                        AfterDebuggerEnterDesignMode(this, EventArgs.Empty);
                });
            };

            DebuggerEvents.OnEnterRunMode += (reason) =>
            {
                SafeExecute(() =>
                {
                    if (AfterDebuggerEnterRunMode != null)
                        AfterDebuggerEnterRunMode(this, EventArgs.Empty);
                });
            };

            DebuggerEvents.OnEnterBreakMode += DebuggerEvents_OnEnterBreakMode;

            //+ COMMAND EVENTS
            //CommandEvents.BeforeExecute += CommandEvents_BeforeExecute;
        }

        void TryRegisterBuildEventsIfNeeded()
        {
            //! Accessing BuildEvents before solution is open will cause 'Publish Now' button in project properties to fail (true for VS10SP1, not tested with newer)
            //! It is safe to access it after solution has been opened

            var dte2 = ServiceProvider.GetDte2();
            var solution = dte2.Solution;

            if (solution == null || !solution.IsOpen)
                return;

            if (BuildEvents != null)
                return; // already registered, nothing to do here

            var events = dte2.Events as Events2;

            BuildEvents = (events).BuildEvents;
            BuildEvents.OnBuildBegin += (scope, action) =>
            {
                if (OnBuildBegin != null)
                    OnBuildBegin(scope, action);
            };

            BuildEvents.OnBuildDone += (scope, action) =>
            {
                if (OnBuildDone != null)
                    OnBuildDone(scope, action);
            };

            BuildEvents.OnBuildProjConfigBegin += (project, projectConfig, platform, solutionConfig) =>
            {
                if (OnBuildProjConfigBegin != null)
                    OnBuildProjConfigBegin(project, projectConfig, platform, solutionConfig);
            };

            BuildEvents.OnBuildProjConfigDone += (project, projectConfig, platform, solutionConfig, success) =>
            {
                if (OnBuildProjConfigDone != null)
                    OnBuildProjConfigDone(project, projectConfig, platform, solutionConfig, success);
            };
        }

        void DebuggerEvents_OnEnterBreakMode(dbgEventReason Reason, ref dbgExecutionAction ExecutionAction)
        {
            SafeExecute(() =>
            {
                if (AfterDebuggerEnterBreakMode != null)
                    AfterDebuggerEnterBreakMode(this, EventArgs.Empty);
            });
        }

        void CommandEvents_BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            //SafeExecute(() =>
            //{
            //    if (BeforeCommandExecuted != null)
            //    {
            //        var args = new BeforeCommandExecutedArgs(Guid, ID, CustomIn, CustomOut);
            //        BeforeCommandExecuted(this, args);
            //    }
            //});
        }


        void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                // todo: log the exception
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
