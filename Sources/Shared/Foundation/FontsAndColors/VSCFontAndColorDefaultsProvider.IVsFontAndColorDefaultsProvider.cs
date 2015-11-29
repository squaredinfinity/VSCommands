using Microsoft.VisualStudio.Shell.Interop;
using SquaredInfinity.VSCommands.Foundation.VisualStudioEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using Microsoft.Win32;
using System.Linq;
using System.Security.Principal;
using System.Security.AccessControl;
using Microsoft.VisualStudio;

namespace SquaredInfinity.VSCommands.Foundation.FontsAndColors
{
    public partial class VSCFontAndColorDefaultsProvider
    {
        #region IVsFontAndColorDefaultsProvider

        public int GetObject(ref Guid rguidCategory, out object ppObj)
        {
            rguidCategory = VSCFontAndColorCategoryGuid;
            ppObj = this;

            return VSConstants.S_OK;
        }

        #endregion
    }
}
