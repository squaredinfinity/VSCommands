using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    /// <summary>
    /// Represents mapping of a nuget package (all versions)
    /// to a single directory. All dlls within the package lib subdirectory (across all versions)
    /// will be mapepd to dlls inside target directory.
    /// </summary>
    public class NugetPackageMapping
    {
        public NugetPackage Package { get; set; }
        public string Directory { get; set; }
    }
}
