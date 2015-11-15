using System;
using System.Collections.Generic;
using System.IO;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.Foundation;

namespace SquaredInfinity.VSCommands.Foundation.Settings
{
    /// <summary>
    /// Manages the storage and retrieval of settings.
    /// </summary>
    public class VscSettingsService : FileSystemSettingsService, IVscSettingsService
    {
        public VscSettingsService(ISerializer defaultSerializer, DirectoryInfo location)
            : base(defaultSerializer, location)
        { }

        protected override IReadOnlyList<int> GetScopesByPriority()
        {
            return new int[]
            {
                VscSettingScope.UserProject,
                VscSettingScope.Project,
                VscSettingScope.UserSolution,
                VscSettingScope.Solution,
                SettingScope.UserMachine,
                SettingScope.User,
                
                // not supported at the moment
                //SettingScope.Machine,
                //SettingScope.Global
            };
        }



        protected override FileInfo GetFile(DirectoryInfo location, string application, string container, string key, int scope, string machineName, string userName)
        {
            // todo: add support for solution and project settings
            // it will come from using different overload of GetSetting / Set setting
            return GetFile(location, application, container, key, scope, machineName, userName, null, null);
        }

        FileInfo GetFile(
            DirectoryInfo location, 
            string application, 
            string container, 
            string key, 
            int scope, 
            string machineName, 
            string userName, 
            FileInfo solution, 
            FileInfo project)
        {
            if (scope == SettingScope.Global || scope == SettingScope.Machine)
                throw new NotSupportedException("todo: global and machine settings are not supported");

            if (scope >= 100)
                throw new NotSupportedException("todo: support for solution and project settings");

            if (scope.IsIn(VscSettingScope.Project, VscSettingScope.UserProject))
            {
                return GetFile(project.Directory, application, container, key, scope, machineName, userName);
            }

            if(scope.IsIn(VscSettingScope.Solution, VscSettingScope.UserSolution))
            {
                return GetFile(solution.Directory, application, container, key, scope, machineName, userName);
            }

            return base.GetFile(Location, application, container, key, scope, machineName, userName);
        }
    }
}
