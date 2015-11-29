using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using SquaredInfinity.VSCommands.Foundation.Settings;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane.Output
{
    [ContentType("output")]
    [ContentType("text")] // text needed to support of build log files opened in main editor
    [Export(typeof(IClassifierProvider))]
    [Name("VSC - Build Output Classifier Provider")]
    public class OutputWindowColoringClasifierProvider : IClassifierProvider
    {
        readonly OutputClassifier Classifier;
        readonly IVscSettingsService SettingsService;
        readonly IVisualStudioEventsService VisualStudioEventsService; 
        readonly IClassificationFormatMapService ClassificationFormatMapService;
        readonly IClassificationTypeRegistryService ClassificationTypeRegistryService;

        [ImportingConstructor]
        public OutputWindowColoringClasifierProvider(
            IServiceProvider serviceProvider,
            IClassificationFormatMapService classificationFormatMapService,
            IClassificationTypeRegistryService classificationTypeRegistryService
            )
        {
            this.SettingsService = VscServices.Instance.Container.Resolve<IVscSettingsService>();
            this.VisualStudioEventsService = VscServices.Instance.Container.Resolve<IVisualStudioEventsService>();

            this.ClassificationFormatMapService = classificationFormatMapService;
            this.ClassificationTypeRegistryService = classificationTypeRegistryService;

            var fontAndColorStorageService = serviceProvider.GetService(typeof(SVsFontAndColorStorage)) as IVsFontAndColorStorage;

            Classifier = new OutputClassifier(
                SettingsService,
                VisualStudioEventsService,
                ClassificationTypeRegistryService,
                ClassificationFormatMapService,
                fontAndColorStorageService);
        }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            try
            {
                if (textBuffer.ContentType.IsOfType("output")
                    || (textBuffer.ContentType.IsOfType("text") && textBuffer.CurrentSnapshot.GetText().StartsWith("Build started ")))
                {
                    return 
                        textBuffer.Properties.GetOrCreateSingletonProperty<IClassifier>(
                            "VSC.BuildOutputColoringClassifier", 
                            () => Classifier);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO
                //ex.Log();
                return null;
            }
        }
    }
}
