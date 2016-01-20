using NuGet.Packaging.Core;
using SquaredInfinity.Foundation.Extensions;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;

namespace SquaredInfinity.VSCommands.Foundation.Nuget
{
    public class NugetService : INugetService
    {
        public NugetService()
        { }

        /// <summary>
        /// Returns the identity of a package which contains specified file.
        /// The file path must be under specific package in "packages" directory.
        /// </summary>
        /// <returns></returns>
        public PackageIdentity GetPackageIdentityForFile(string fullPath)
        {
            return GetPackageIdentityForFile(new FileInfo(fullPath));
        }

        /// <summary>
        /// Returns the identity of a package which contains specified file.
        /// The file path must be under specific package in "packages" directory.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public PackageIdentity GetPackageIdentityForFile(FileInfo file)
        {
            // navigate up the directory hierarchy until .nupkg is found, but terminate at 'packages'

            var package_search_pattern = "*.nupkg";

            var current_directory = file.Directory;

            var package_file = (FileInfo)null;

            while(current_directory != null)
            {
                package_file = 
                    current_directory
                    .GetFiles(package_search_pattern, SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();

                if (package_file != null)
                    break;

                if (string.Equals(current_directory.Name, "packages", StringComparison.CurrentCultureIgnoreCase))
                {
                    // reached root packages directory, nothing else to do here
                    break;
                }

                current_directory = current_directory.Parent;
            }

            if (package_file == null)
                return null;

            using (var fs = File.OpenRead(package_file.FullName))
            {
                using (var r = new NuGet.Packaging.PackageReader(fs))
                {
                    return r.GetIdentity();
                }
            }
        }
    }
}
