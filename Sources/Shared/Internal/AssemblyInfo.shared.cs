using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Squared Infinity Limited")]
[assembly: AssemblyCopyright("Copyright © 2014")]
[assembly: AssemblyProduct("VSCommands for Visual Studio")]

[assembly: AssemblyVersion("14." + CurrentVersionStrings.CommonCurrentReleaseNumber)]
[assembly: AssemblyFileVersion("14." + CurrentVersionStrings.CommonCurrentReleaseNumber)]

#if SIGN
#pragma warning disable 1699  // warning CS1699: Use command line option '/keyfile' or appropriate project settings instead of 'AssemblyKeyFile'
[assembly: AssemblyKeyFile(@"c:\!\StartSSL Class 2 - Jaroslaw Kardas.snk")]
#pragma warning restore 1699
#endif

static class CurrentVersionStrings
{
    public const string CommonCurrentReleaseNumber = "0.0.2";
}
