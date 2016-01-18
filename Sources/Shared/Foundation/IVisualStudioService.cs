using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
{
    public interface IVisualStudioService
    {
        void RestartVisualStudio(bool forceAsAdmin, bool saveAllChanges, bool asDifferentUser);

        bool TrySetAlwaysRunAsAdmin(bool enable = true);
        bool IsAlwaysRunAsAdminSet();

        void SaveAllChanges();
    }
}
