using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.ElevatedPermissions
{
    public interface IElevatedPermissionsService
    {
        bool EnsureElevatedPermissions();
    }
}
