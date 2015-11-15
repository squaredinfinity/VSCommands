using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE;

namespace SquaredInfinity.VSCommands.Foundation.VisualStudioEvents
{
    public class DocumentEventArgs : EventArgs
    {
        public string DocumentFileName { get; private set; }
        public string DocumentFullPath { get; private set; }
        public ProjectItem ProjectItem { get; private set; }

        public DocumentEventArgs(string documentFullPath, ProjectItem projectItem)
        {
            this.DocumentFullPath = documentFullPath;
            this.DocumentFileName = Path.GetFileName(documentFullPath);

            this.ProjectItem = projectItem;
        }
    }
}
