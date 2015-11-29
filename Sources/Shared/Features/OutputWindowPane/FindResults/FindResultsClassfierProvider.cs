using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using SquaredInfinity.VSCommands.Foundation;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;

namespace SquaredInfinity.VSCommands.Features.OutputWindowPane.FindResults
{
    [ContentType("FindResults")]
    [Export(typeof(IClassifierProvider))]
    [Name("VSC - Find Results Classifier Provider")]
    public class FindResultsClassfierProvider : IClassifierProvider
    {
        readonly FindResultsClassifier Classifier;
        readonly IVscSettingsService SettingsService;
        readonly IVisualStudioEventsService VisualStudioEventsService;
        readonly IClassificationFormatMapService ClassificationFormatMapService;
        readonly IClassificationTypeRegistryService ClassificationTypeRegistryService;

        [ImportingConstructor]
        public FindResultsClassfierProvider(
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

            Classifier = new FindResultsClassifier(
                SettingsService,
                VisualStudioEventsService,
                ClassificationTypeRegistryService,
                ClassificationFormatMapService,
                fontAndColorStorageService,
                serviceProvider.GetDte2());
        }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            try
            {
                if (textBuffer.ContentType.IsOfType("FindResults"))
                {
                    return textBuffer.Properties.GetOrCreateSingletonProperty<IClassifier>(
                        "VSC.FindResultsWindowColoringClassifier", 
                        () => Classifier);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO: log
                //Logger.LogException(ex);
                return null;
            }
        }
    }
}
