using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel.Composition;
using SquaredInfinity.Foundation;
using SquaredInfinity.Foundation.Presentation.Resources;
using SquaredInfinity.Foundation.Serialization.FlexiXml;
using SquaredInfinity.VSCommands.Features.SolutionBadges;
using SquaredInfinity.VSCommands.Foundation;
using SquaredInfinity.VSCommands.Foundation.Settings;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.ComponentModel.Design;
using SquaredInfinity.VSCommands.Foundation.FontsAndColors;
using SquaredInfinity.VSCommands.Features.OutputWindowPane;

namespace SquaredInfinity.VSCommands
{
    public class Bootstrapper
    {
        public void InitializeServices(IVsPackage package)
        {
            // TODO:    eventually perhaps most of it could be done using MEF alone
            //          but for know do what worked so far.
            //          Need to find out how to register custom instance with VS MEF

            //# IoC
            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);

            //# Service Provider from package
            var service_provider = (IServiceProvider)package;
            container.RegisterInstance<IServiceProvider>(service_provider);

            //# Service Container from package
            var service_container = (IServiceContainer)package;
            container.RegisterInstance<IServiceContainer>(service_container);

            // VS MEF
            var componentModel = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel2;
            container.RegisterInstance<IComponentModel>(componentModel);
            container.RegisterInstance<IComponentModel2>(componentModel);

            //# Settings Service
            var settings_serializer = new FlexiXmlSerializer();

            settings_serializer.GetOrCreateTypeSerializationStrategy<VersionInstallationInfo>()
                .SerializeMemberAsAttribute(x => x.Version, x => x.Version != null, (x, _v) => _v.ToString(), _a => new Version(_a.Value));

            var settings_location = StaticSettings.AppDataDirectory;
            IVscSettingsService settings_service = new VscSettingsService(settings_serializer, StaticSettings.AppDataDirectory);

            container.RegisterInstance<IVscSettingsService>(settings_service);

            var vscservices = new VscServices();
            vscservices.Container = container;
            componentModel.DefaultCompositionService.SatisfyImportsOnce(vscservices);
            VscServices.Initialise(vscservices);

            //# UI Service
            var vscUIService = new VscUIService();
            container.RegisterInstance<IVscUIService>(vscUIService);

            //# Visual Studio Events Service
            var vs_events_service = container.Resolve<VisualStudioEventsService>();
            container.RegisterInstance<IVisualStudioEventsService>(vs_events_service);

            //# Fonts And Color
            var fonts_and_colors = container.Resolve<VSCFontAndColorDefaultsProvider>();

#if DEBUG
            //      fonts_and_colors.TryClearFontAndColorCache();
#endif

            fonts_and_colors.RegisterCollorableItem(ClassificationNames.OutputText, "Output Window Text");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.OutputInformation, "Output Window Information");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.OutputWarning, "Output Window Warning");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.OutputError, "Output Window Error");

            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputBuildSummary, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputBuildSummarySuccess, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputBuildSummaryFailed, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputBuildSummaryTotal, "");

            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputCodeContractsInformation, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputProjectBuildStart, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.BuildOutputProjectBuildSkipped, "");

            fonts_and_colors.RegisterCollorableItem(ClassificationNames.TfsOutputError, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.TfsOutputWarning, "");
            fonts_and_colors.RegisterCollorableItem(ClassificationNames.TfsOutputSuccess, "");

            fonts_and_colors.RegisterCollorableItem(ClassificationNames.FindResultsOutputMatch, "Find Results Match", isBold: true);

            fonts_and_colors.Initialise();

            container.RegisterInstance<VSCFontAndColorDefaultsProvider>(fonts_and_colors);


            //# Initialize User Interface
            vs_events_service.RegisterVisualStudioUILoadedAction(InitializeUserInterface);

            //# Solution Badges Service
            var solution_badges_service = container.Resolve<SolutionBadgesService>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(solution_badges_service);
            solution_badges_service.Initialise();
            container.RegisterInstance<ISolutionBadgesService>(solution_badges_service);

            //# Update Settings with local context info
            var deployment_info = (DeploymentInfo)null;
            var has_just_been_upgraded = false;

            if(!settings_service.TryGetSetting<DeploymentInfo>("internal.deployment", VscSettingScope.User, out deployment_info))
            {
                deployment_info = new DeploymentInfo();
                deployment_info.InitialVersion = VersionInstallationInfo.GetCurrent();
                has_just_been_upgraded = true;
            }

            if (deployment_info.CurrentVersion == null)
            {
                deployment_info.CurrentVersion = VersionInstallationInfo.GetCurrent();
                has_just_been_upgraded = true;
            }
            else if (deployment_info.CurrentVersion.Version != StaticSettings.CurrentVersion)
            { 
                deployment_info.PreviousVersion = deployment_info.CurrentVersion;
                deployment_info.CurrentVersion = VersionInstallationInfo.GetCurrent();
                has_just_been_upgraded = true;
            }

            if (has_just_been_upgraded)
            {
                settings_service.SetSetting("internal.deployment", VscSettingScope.User, deployment_info);

                // clear fotn and color cache as new items may have been added.
                // cache rebuilding can be an expensive operation
                fonts_and_colors.TryClearFontAndColorCache();
            }
        }

        void InitializeUserInterface()
        {
            var resources = new SquaredInfinity.Foundation.Presentation.XamlResources();
            resources.LoadAndMergeResources();

            //# Initialize UI using MEF

            var applicationCatalog = new DirectoryCatalog(StaticSettings.VSCommandsAssembliesDirectory.FullName, "SquaredInfinity.VSCommands.*.dll");
            var compositionContainer = new CompositionContainer(applicationCatalog);
            compositionContainer.Compose(new CompositionBatch());

            //# Import Xaml Resources
            ResourcesManager.ImportAndLoadAllResources(compositionContainer);
        }
    }
}
