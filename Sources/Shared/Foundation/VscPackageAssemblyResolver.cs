using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
{
    public class VscPackageAssemblyResolver : IDisposable
    {
        DirectoryInfo AssembliesDirectory;

        public void Initialize(DirectoryInfo assembliesDirectory)
        {
            this.AssembliesDirectory = assembliesDirectory;

            //AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            //AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            //AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
        }

        private System.Reflection.Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        private System.Reflection.Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {

        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        #region IDisposable

        bool IsDisposed = false;

        public void Dispose()
        {
            Dispose(disposing: true);

            GC.SuppressFinalize(this);
        }

        ~VscPackageAssemblyResolver()
        {
            Dispose(disposing: false);
        }

        protected void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                // Dispose Managed Resources
            }

            // Dispose Unmanaged Resources
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;

            IsDisposed = true;
        }

        #endregion
    }
}
