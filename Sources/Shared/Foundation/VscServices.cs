using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
{
    // TODO:    this is just a temporary measure
    //          until I find out how to register custom instances with VS MEF
    public class VscServices
    {
        public IUnityContainer Container { get; set; }

        public static VscServices Instance { get; private set; }
        
        public static void Initialise(VscServices instance)
        {
            Instance = instance;

            Instance.Container.RegisterInstance<IClassificationTypeRegistryService>(Instance.ClassificationTypeRegistryService);
            Instance.Container.RegisterInstance<IClassificationFormatMapService>(Instance.ClassificationFormatMapService);
            Instance.Container.RegisterInstance<IEditorFormatMapService>(Instance.FormatMapService);
        }

        [Import]
        public IEditorFormatMapService FormatMapService;

        [Import]
        public IClassificationFormatMapService ClassificationFormatMapService;

        [Import]
        public IClassificationTypeRegistryService ClassificationTypeRegistryService;

    }
}
