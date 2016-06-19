using SquaredInfinity.VSCommands.Presentation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Features.NugetReferenceRedirection
{
    public class SolutionPackagesToolWindowPane : VSCToolWindowPane
    {
        public SolutionPackagesToolWindowPane()
        {
            Caption = "Solution Packages";

            var v = new SolutionPackagesView();

            Content = v;
        }
    }
}
