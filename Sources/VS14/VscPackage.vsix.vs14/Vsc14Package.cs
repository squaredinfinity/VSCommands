//------------------------------------------------------------------------------
// <copyright file="Vsc14Package.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using SquaredInfinity.VSCommands.Foundation;
using System.Reflection;
using SquaredInfinity.VSCommands.Foundation.FontsAndColors;

namespace SquaredInfinity.VSCommands.VscPackage
{

    [ProvideService(typeof(IVsFontAndColorDefaultsProvider))]
    [ProvideService(typeof(VSCFontAndColorDefaultsProvider))]

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(StaticSettings.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
    public sealed class Vsc14Package : Package
    {
        readonly Bootstrapper Bootstrapper;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Vsc14Package"/> class.
        /// </summary>
        public Vsc14Package()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.

            Bootstrapper = new Bootstrapper();
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var vscAssembly = Assembly.GetExecutingAssembly();
            StaticSettings.VSCommandsVSIXAssembly = vscAssembly;

            Bootstrapper.InitializeServices(this);
        }

        #endregion
    }
}
