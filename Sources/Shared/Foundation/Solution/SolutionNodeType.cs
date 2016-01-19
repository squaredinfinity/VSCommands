using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands.Foundation.Solution
{
    public enum SolutionNodeType
    {
        /// <summary>
        /// Used to signify unknown (to VSCommands) solution node type
        /// </summary>
        Unknown = 0,

        None,

        MultiSelection,

        /// <summary>
        /// Root solution node
        /// </summary>
        Solution,
        /// <summary>
        /// Project node
        /// </summary>
        Project,
        /// <summary>
        /// Folder inside a project
        /// </summary>
        Folder,
        /// <summary>
        /// File inside a project
        /// </summary>
        File,
        /// <summary>
        /// Reference
        /// </summary>
        Reference,
        /// <summary>
        /// References node under a project (holds individual references)
        /// </summary>
        ReferenceRoot,
        /// <summary>
        /// Solution folder
        /// </summary>
        SolutionFolder
    }
}
