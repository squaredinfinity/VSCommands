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
        #region IVsFontAndColorEvents

        public int OnApply()
        {
            VisualStudioEventsService.RaiseAfterVSCommandsColorSettingsChanged();
            return VSConstants.S_OK;
        }

        public int OnFontChanged(ref Guid rguidCategory, FontInfo[] pInfo, LOGFONTW[] pLOGFONT, uint HFONT)
        {
            return VSConstants.S_OK;
        }

        public int OnItemChanged(ref Guid rguidCategory, string szItem, int iItem, ColorableItemInfo[] pInfo, uint crLiteralForeground, uint crLiteralBackground)
        {
            return VSConstants.S_OK;
        }

        public int OnReset(ref Guid rguidCategory)
        {
            VisualStudioEventsService.RaiseAfterVSCommandsColorSettingsChanged();
            return VSConstants.S_OK;
        }

        public int OnResetToBaseCategory(ref Guid rguidCategory)
        {
            return VSConstants.S_OK;
        }

        #endregion
    }
}
