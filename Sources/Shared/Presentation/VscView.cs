using SquaredInfinity.Foundation.Presentation.Views;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.VSCommands.Foundation;

namespace SquaredInfinity.VSCommands.Presentation
{
    public class VscView<TViewModel> : View<TViewModel> where TViewModel : IHostAwareViewModel
    {
        public VscView()
        {
            // for performance reasons never refresh view model when data context changes, unless specified otherwise
            this.RefreshViewModelOnDataContextChange = false;
        }

        protected override IHostAwareViewModel ResolveViewModel(Type viewType, object newDatacontext)
        {
            var vmTypeName = viewType.FullName + "Model";

            var vmType = viewType.Assembly.GetType(vmTypeName);

            var vm = VscServices.Instance.Container.Resolve(vmType);

            return vm as IHostAwareViewModel;
        }
    }
}
