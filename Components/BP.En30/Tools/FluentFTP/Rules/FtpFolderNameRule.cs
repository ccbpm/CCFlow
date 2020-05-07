namespace FluentFTP.Rules
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;

    public class FtpFolderNameRule : FtpRule
    {
        public static List<string> CommonBlacklistedFolders;
        public IList<string> Names;
        public bool Whitelist;

        static FtpFolderNameRule()
        {
            List<string> list1 = new List<string> { ".git", ".svn", ".DS_Store", "node_modules" };
            CommonBlacklistedFolders = list1;
        }

        public FtpFolderNameRule(bool whitelist, IList<string> names)
        {
            this.Whitelist = whitelist;
            this.Names = names;
        }

        public override bool IsAllowed(FtpListItem item)
        {
            string[] pathSegments = null;
            if (item.Type == FtpFileSystemObjectType.File)
            {
                pathSegments = item.FullName.GetFtpDirectoryName().GetPathSegments();
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                pathSegments = item.FullName.GetPathSegments();
            }
            else
            {
                return true;
            }
            if (this.Whitelist)
            {
                foreach (string str in pathSegments)
                {
                    if (this.Names.Contains(str.Trim()))
                    {
                        return true;
                    }
                }
                return false;
            }
            foreach (string str2 in pathSegments)
            {
                if (this.Names.Contains(str2.Trim()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

