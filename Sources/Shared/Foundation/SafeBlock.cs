using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation
{
    public static class SafeBlock
    {
        public static void Run(Action action, bool swallowExceptions = false)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (!swallowExceptions)
                {
                    // todo: log
                    //ex.Log();
                }
            }
        }
    }
}
