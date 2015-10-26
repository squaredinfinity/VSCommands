using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands
{
    public static class IServiceProviderExtensions
    {
        public static DTE2 GetDte2(this IServiceProvider service_provider)
        {
            var dte2 = service_provider.GetService(typeof(DTE)) as DTE2;

            return dte2;
        }
    }
}
