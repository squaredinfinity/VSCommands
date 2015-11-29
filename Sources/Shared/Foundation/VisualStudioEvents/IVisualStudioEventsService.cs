using EnvDTE;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public interface IVisualStudioEventsService
    {
        event EventHandler<EventArgs> AfterVSCommandsColorSettingsChanged;
        event EventHandler<EventArgs> AfterTextColorSettingsChanged;
        event EventHandler<EventArgs> AfterVisualStudioThemeChanged;

        event EventHandler<EventArgs> AfterSolutionOpened;
        event EventHandler<EventArgs> AfterSolutionClosed;

        event EventHandler<DocumentEventArgs> AfterActiveDocumentChanged;

        event EventHandler<EventArgs> AfterDebuggerEnterDesignMode;
        event EventHandler<EventArgs> AfterDebuggerEnterRunMode;
        event EventHandler<EventArgs> AfterDebuggerEnterBreakMode;

        event EventHandler<EventArgs> AfterActiveSolutionConfigChanged;


        event _dispBuildEvents_OnBuildBeginEventHandler OnBuildBegin;
        event _dispBuildEvents_OnBuildDoneEventHandler OnBuildDone;
        event _dispBuildEvents_OnBuildProjConfigBeginEventHandler OnBuildProjConfigBegin;
        event _dispBuildEvents_OnBuildProjConfigDoneEventHandler OnBuildProjConfigDone;

        /// <summary>
        /// Register an action to be executed when VS UI is loaded (i.e. Main Window has been created)
        /// The action will be executed immediately if UI has already been loaded.
        /// </summary>
        /// <param name="action"></param>
        void RegisterVisualStudioUILoadedAction(Action action);
        void RaiseAfterVSCommandsColorSettingsChanged();
    }
}
