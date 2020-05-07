namespace FluentFTP.Rules
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public class FtpFileNameRegexRule : FtpRule
    {
        public List<string> RegexPatterns;
        public bool Whitelist;

        public FtpFileNameRegexRule(bool whitelist, IList<string> regexPatterns)
        {
            this.Whitelist = whitelist;
            this.RegexPatterns = regexPatterns.Where<string>((<>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<string, bool>(<>c.<>9.<.ctor>b__2_0)))).ToList<string>();
        }

        public override bool IsAllowed(FtpListItem item)
        {
            if (this.RegexPatterns.Count == 0)
            {
                return true;
            }
            if (item.Type != FtpFileSystemObjectType.File)
            {
                return true;
            }
            string fileName = item.Name;
            if (this.Whitelist)
            {
                return this.RegexPatterns.Any<string>(x => Regex.IsMatch(fileName, x));
            }
            return !this.RegexPatterns.Any<string>(x => Regex.IsMatch(fileName, x));
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FtpFileNameRegexRule.<>c <>9 = new FtpFileNameRegexRule.<>c();
            public static Func<string, bool> <>9__2_0;

            internal bool <.ctor>b__2_0(string x)
            {
                return x.IsValidRegEx();
            }
        }
    }
}

