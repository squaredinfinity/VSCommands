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

namespace SquaredInfinity.VSCommands
{
    public class Bootstrapper
    {
        public void InitializeServices(IVsPackage package)
        {
            // TODO:    eventually perhaps most of it could be done using MEF alone
            //          but for know do what worked so far

            //# IoC
            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);

            //# Service Provider from package
            container.RegisterInstance<IServiceProvider>((IServiceProvider) package);

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

            //# UI Service
            var vscUIService = new VscUIService();
            container.RegisterInstance<IVscUIService>(vscUIService);

            //# Visual Studio Events Service
            var vs_events_service = container.Resolve<VisualStudioEventsService>();
            container.RegisterInstance<IVisualStudioEventsService>(vs_events_service);

            //# Initialize User Interface
            vs_events_service.RegisterVisualStudioUILoadedAction(InitializeUserInterface);

            //# Solution Badges Service
            var solution_badges_service = container.Resolve<SolutionBadgesService>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(solution_badges_service);
            solution_badges_service.Initialise();
            container.RegisterInstance<ISolutionBadgesService>(solution_badges_service);

            //# Update Settings with local context info
            var deployment_info = (DeploymentInfo)null;
            var update_deployment_info = false;

            if(!settings_service.TryGetSetting<DeploymentInfo>("internal.deployment", VscSettingScope.User, out deployment_info))
            {
                deployment_info = new DeploymentInfo();
                deployment_info.InitialVersion = VersionInstallationInfo.GetCurrent();
                update_deployment_info = true;
            }

            if (deployment_info.CurrentVersion == null)
            {
                deployment_info.CurrentVersion = VersionInstallationInfo.GetCurrent();
                update_deployment_info = true;
            }
            else if (deployment_info.CurrentVersion.Version != StaticSettings.CurrentVersion)
            { 
                deployment_info.PreviousVersion = deployment_info.CurrentVersion;
                deployment_info.CurrentVersion = VersionInstallationInfo.GetCurrent();
                update_deployment_info = true;
            }

            if (update_deployment_info)
                settings_service.SetSetting("internal.deployment", VscSettingScope.User, deployment_info);
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
