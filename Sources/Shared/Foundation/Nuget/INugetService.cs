using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.Nuget
{
    public interface INugetService
    {
        PackageIdentity GetPackageIdentityForFile(string fullPath);
        PackageIdentity GetPackageIdentityForFile(FileInfo file);
    }
}
