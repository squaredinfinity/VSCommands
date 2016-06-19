using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.Shell;
using SquaredInfinity.VSCommands.Foundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Presentation
{
    public abstract class VSCToolWindowPane : ToolWindowPane
    {
        public IUnityContainer Container { get; private set; }
        public IVscUIService UIService { get; private set; }

        public VSCToolWindowPane()
        {
            Container = VscServices.Instance.Container;
            UIService = VscServices.Instance.Container.Resolve<IVscUIService>();
        }
    }
}
