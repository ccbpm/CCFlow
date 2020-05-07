namespace FluentFTP.Rules
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public class FtpFolderRegexRule : FtpRule
    {
        public List<string> RegexPatterns;
        public bool Whitelist;

        public FtpFolderRegexRule(bool whitelist, IList<string> regexPatterns)
        {
            this.Whitelist = whitelist;
            this.RegexPatterns = regexPatterns.Where<string>((<>c.<>9__2_0 ?? (<>c.<>9__2_0 = new Func<string, bool>(<>c.<>9.<.ctor>b__2_0)))).ToList<string>();
        }

        public override bool IsAllowed(FtpListItem item)
        {
            string[] pathSegments;
            if (this.RegexPatterns.Count != 0)
            {
                pathSegments = null;
                if (item.Type == FtpFileSystemObjectType.File)
                {
                    pathSegments = item.FullName.GetFtpDirectoryName().GetPathSegments();
                    goto Label_0045;
                }
                if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    pathSegments = item.FullName.GetPathSegments();
                    goto Label_0045;
                }
            }
            return true;
        Label_0045:
            if (this.Whitelist)
            {
                foreach (string str in pathSegments)
                {
                    foreach (string str2 in this.RegexPatterns)
                    {
                        if (Regex.IsMatch(str.Trim(), str2))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            foreach (string str3 in pathSegments)
            {
                foreach (string str4 in this.RegexPatterns)
                {
                    if (Regex.IsMatch(str3.Trim(), str4))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FtpFolderRegexRule.<>c <>9 = new FtpFolderRegexRule.<>c();
            public static Func<string, bool> <>9__2_0;

            internal bool <.ctor>b__2_0(string x)
            {
                return x.IsValidRegEx();
            }
        }
    }
}

