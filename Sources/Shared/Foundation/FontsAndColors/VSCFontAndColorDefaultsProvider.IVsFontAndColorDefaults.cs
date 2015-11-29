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
        #region IVsFontAndColorDefaults

        public int GetBaseCategory(out Guid pguidBase)
        {
            pguidBase = new Guid("{b0e6a221-92fd-4d72-be80-04a36b591fcb}");

            return VSConstants.S_OK;
        }

        public int GetCategoryName(out string pbstrName)
        {
            pbstrName = "VSCommands";

            return VSConstants.S_OK;
        }

        public int GetFlags(out uint dwFlags)
        {
            dwFlags = (uint)__FONTCOLORFLAGS.FCF_SAVEALL;

            return VSConstants.S_OK;
        }

        public int GetFont(FontInfo[] pInfo)
        {
            return VSConstants.S_OK;
        }

        public int GetItem(int iItem, AllColorableItemInfo[] pInfo)
        {
            pInfo[0] = ColorableItems[iItem];

            return VSConstants.S_OK;
        }

        public int GetItemByName(string szItem, AllColorableItemInfo[] pInfo)
        {
            return VSConstants.S_OK;
        }

        public int GetItemCount(out int pcItems)
        {
            pcItems = ColorableItems.Count;

            return VSConstants.S_OK;
        }

        public int GetPriority(out ushort pPriority)
        {
            pPriority = 0;

            return VSConstants.S_OK;
        }

        #endregion
    }
}
