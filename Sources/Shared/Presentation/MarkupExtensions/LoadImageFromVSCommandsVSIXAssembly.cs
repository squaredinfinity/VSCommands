using SquaredInfinity.Foundation.Presentation.Resources;
using SquaredInfinity.VSCommands.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;

namespace SquaredInfinity.VSCommands.Presentation.MarkupExtensions
{
    /// <summary>
    /// ! VSCommands cannot use LoadImageFromMainAssembly markup extension because it uses Assembly.GetEntryAssembly() to resove main assembly.
    /// ! Can't use Assembly.GetEntryAssembly() as it always returns null.
    /// </summary>
    public class LoadImageFromVSCommandsVSIXAssembly : MarkupExtension
    {
        public string RelativeUri { get; set; }

        public LoadImageFromVSCommandsVSIXAssembly()
        { }

        public LoadImageFromVSCommandsVSIXAssembly(string relativeUri)
        {
            this.RelativeUri = relativeUri;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return ResourcesManager.LoadImageFromAssembly(StaticSettings.VSCommandsVSIXAssemblyName, RelativeUri as string);
        }
    }
}
