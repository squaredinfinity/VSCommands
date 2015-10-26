using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands
{
    public static class DTE2Extensions
    {
        public static EnvDTE.Document GetActiveDocument(this EnvDTE80.DTE2 dte2)
        {
            EnvDTE.Document result = null;

            try
            {
                result = dte2.ActiveDocument;
            }
            catch (Exception ex)
            {
                //todo: investigate why it is sometimes thrown
                //Logger.SwallowException(ex, "Dte.ActiveDocument can throw exceptions when accessed but this does not impact this scenario.");
            }

            return result;
        }
    }
}
