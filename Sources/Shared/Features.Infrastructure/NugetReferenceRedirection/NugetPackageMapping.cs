using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    public class PackagesMappings
    {
        /// <summary>
        /// Provide mappings for each known package
        /// Key: Package Id
        /// Value: Mapping
        /// </summary>
        public MappingsMap PackageIdToMappings { get; set; } = new MappingsMap();
    }

    /// <summary>
    /// Represents mapping of a nuget package (all versions)
    /// to a single directory. All dlls within the package lib subdirectory (across all versions)
    /// will be mapepd to dlls inside target directory.
    /// </summary>
    public class NugetPackageMapping
    {
        /// <summary>
        /// Package referenced by this mapping
        /// </summary>
        public NugetPackage Package { get; set; }

        public string PackageId { get; set; }

        /// <summary>
        /// Mapping for this package
        /// </summary>
        public Mapping PackageMapping { get; set; }

        /// <summary>
        /// Mappings for individual targets specified by this package
        /// </summary>
        public MappingsMap TargetsMappings { get; set; } = new MappingsMap();
    }

    public class Mapping
    {
        public string TargetDirectory { get; set; }
        public StringCollection RecentDirectories { get; set; } = new StringCollection();
    }

    public class MappingsMap : Dictionary<string, Mapping>
    { }
}
