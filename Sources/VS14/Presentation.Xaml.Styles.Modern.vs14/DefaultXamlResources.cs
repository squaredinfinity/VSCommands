using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace  SquaredInfinity.Foundation.Presentation.Styles.Modern
{

    [ExportAttribute(typeof(IXamlResourcesProvider))]
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public partial class DefaultXamlResources : IXamlResourcesProvider
    {
        // Import Order is higher than Foundation.Presentation Import Order (on which resources from this assembly may depend)
        public const int ImportOrder = SquaredInfinity.Foundation.Presentation.XamlResources.ImportOrder + 100;

        public void LoadAndMergeResources()
        {
            ResourcesManager.LoadAndMergeCompiledResourceDictionaryFromThisAssembly(@"All.xaml");
        }

        public void ApplyAllStyles()
        {
            Application.Current.Resources[typeof(Button)] = Application.Current.Resources["Styles.Button"];
            Application.Current.Resources[typeof(CheckBox)] = Application.Current.Resources["Styles.CheckBox"];
            Application.Current.Resources[typeof(ComboBox)] = Application.Current.Resources["Styles.ComboBox"];
            Application.Current.Resources[typeof(Expander)] = Application.Current.Resources["Styles.Expander"];
            Application.Current.Resources[typeof(Menu)] = Application.Current.Resources["Styles.Menu"];
            Application.Current.Resources[typeof(MenuItem)] = Application.Current.Resources["Styles.MenuItem"];
            Application.Current.Resources[typeof(ListView)] = Application.Current.Resources["Styles.ListView"];
            Application.Current.Resources[typeof(ListViewItem)] = Application.Current.Resources["Styles.ListViewItem"];
            Application.Current.Resources[typeof(RadioButton)] = Application.Current.Resources["Styles.RadioButton"];
            Application.Current.Resources[typeof(ScrollBar)] = Application.Current.Resources["Styles.ScrollBar"];
            Application.Current.Resources[typeof(TabControl)] = Application.Current.Resources["Styles.TabControl"];
            Application.Current.Resources[typeof(TabItem)] = Application.Current.Resources["Styles.TabItem"];
            Application.Current.Resources[typeof(TextBox)] = Application.Current.Resources["Styles.TextBox"];
            Application.Current.Resources[typeof(TextBlock)] = Application.Current.Resources["Styles.TextBlock"];
            Application.Current.Resources[typeof(Label)] = Application.Current.Resources["Styles.Label"];
        }
    }
}
