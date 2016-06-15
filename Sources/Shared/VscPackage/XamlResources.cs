using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Windows;

namespace SquaredInfinity.VSCommands.VscPackage
{
    [ExportAttribute(typeof(IXamlResourcesProvider))]
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public class XamlResources : IXamlResourcesProvider
    {
        // Import Order is higher than Features Import Order (on which resources from this assembly may depend)
        public const uint ImportOrder = Features.XamlResources.ImportOrder + 100;

        public IEnumerable<ResourceDictionary> LoadResources()
        {
            yield return ResourcesManager.LoadCompiledResourceDictionaryFromThisAssembly("XamlResources.xaml");
        }
    }
}
