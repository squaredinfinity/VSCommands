using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace SquaredInfinity.VSCommands.Foundation.Text.Classification
{
    public class ClassificationTypeMatchPattern
    {
        public string ClassificationTypeName { get; set; }
        public ClassificationMatchPattern ClassificationMatchPattern { get; set; }
        public ColorableItemInfo[] ColorableItemInfos { get; set; }

        public ClassificationTypeMatchPattern()
        {
            this.ColorableItemInfos = new[] { new ColorableItemInfo() };
        }

        public bool IsMatch(string input)
        {
            return ClassificationMatchPattern.ToRegex().IsMatch(input);
        }
    }
}
