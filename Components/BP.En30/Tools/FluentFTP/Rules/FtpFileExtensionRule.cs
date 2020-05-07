namespace FluentFTP.Rules
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class FtpFileExtensionRule : FtpRule
    {
        public IList<string> Exts;
        public bool Whitelist;

        public FtpFileExtensionRule(bool whitelist, IList<string> exts)
        {
            this.Whitelist = whitelist;
            this.Exts = exts;
        }

        public override bool IsAllowed(FtpListItem item)
        {
            if (item.Type != FtpFileSystemObjectType.File)
            {
                return true;
            }
            string str = Path.GetExtension(item.Name).Replace(".", "").ToLower();
            if (this.Whitelist)
            {
                if (str.IsBlank())
                {
                    return false;
                }
                return this.Exts.Contains(str);
            }
            return (str.IsBlank() || !this.Exts.Contains(str));
        }
    }
}

