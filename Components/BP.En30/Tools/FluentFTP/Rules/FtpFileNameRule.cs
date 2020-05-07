namespace FluentFTP.Rules
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;

    public class FtpFileNameRule : FtpRule
    {
        public IList<string> Names;
        public bool Whitelist;

        public FtpFileNameRule(bool whitelist, IList<string> names)
        {
            this.Whitelist = whitelist;
            this.Names = names;
        }

        public override bool IsAllowed(FtpListItem item)
        {
            if (item.Type != FtpFileSystemObjectType.File)
            {
                return true;
            }
            string name = item.Name;
            if (this.Whitelist)
            {
                return this.Names.Contains(name);
            }
            return !this.Names.Contains(name);
        }
    }
}

