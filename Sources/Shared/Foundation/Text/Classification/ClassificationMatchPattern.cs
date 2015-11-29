using SquaredInfinity.Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.VSCommands.Foundation.Text.Classification
{
    public class ClassificationMatchPattern : NotifyPropertyChangedObject
    {
        string _classificationTypeName;
        public string ClassificationTypeName
        {
            get { return _classificationTypeName; }
            set { TrySetThisPropertyValue(ref _classificationTypeName, value); }
        }

        string _pattern;
        public string Pattern
        {
            get { return _pattern; }
            set { TrySetThisPropertyValue(ref _pattern, value); }
        }

        RegexOptions _regexOptions = RegexOptions.None;
        public RegexOptions RegexOptions
        {
            get { return _regexOptions; }
            set { TrySetThisPropertyValue(ref _regexOptions, value); }
        }

        public ClassificationMatchPattern() { }

        public ClassificationMatchPattern(string classificationTypeName, string pattern, RegexOptions regexOptions)
        {
            this.ClassificationTypeName = classificationTypeName;
            this.Pattern = pattern;
            this.RegexOptions = regexOptions;
        }

        public Regex ToRegex()
        {
            var regex = new Regex(Pattern, RegexOptions);

            return regex;
        }
    }
}
