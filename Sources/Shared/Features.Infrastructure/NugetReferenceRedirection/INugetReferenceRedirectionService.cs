using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    public interface INugetReferenceRedirectionService
    {
        /// <summary>
        /// Returns information about all packages used by specified solution.
        /// </summary>
        /// <param name="solutionDirectory">solution directory</param>
        /// <returns></returns>
        IReadOnlyList<NugetPackage> GetAllUsedPackages(DirectoryInfo solutionDirectory);

        /// <summary>
        /// Returns information about all packages used by specified solution.
        /// </summary>
        /// <param name="solutionDirectory">solution directory</param>
        /// <param name="packagesDirectory">directory where packages are located</param>
        /// <returns></returns>
        IReadOnlyList<NugetPackage> GetAllUsedPackages(
            DirectoryInfo solutionDirectory,
            DirectoryInfo packagesDirectory);
    }
}
