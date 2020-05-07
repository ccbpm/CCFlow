namespace FluentFTP
{
    using System;
    using System.Text;

    public class FtpListItem
    {
        public int Chmod;
        public DateTime Created;
        public string FullName;
        public FtpPermission GroupPermissions;
        public string Input;
        public int LinkCount;
        public FtpListItem LinkObject;
        public string LinkTarget;
        private string m_name;
        public DateTime Modified;
        public FtpPermission OthersPermissions;
        public FtpPermission OwnerPermissions;
        public string RawGroup;
        public string RawOwner;
        public string RawPermissions;
        public long Size;
        public FtpSpecialPermissions SpecialPermissions;
        public FtpFileSystemObjectSubType SubType;
        public FtpFileSystemObjectType Type;

        public FtpListItem()
        {
            this.Modified = DateTime.MinValue;
            this.Created = DateTime.MinValue;
            this.Size = -1L;
        }

        public FtpListItem(string record, string name, long size, bool isDir, ref DateTime lastModifiedTime)
        {
            this.Modified = DateTime.MinValue;
            this.Created = DateTime.MinValue;
            this.Size = -1L;
            this.Input = record;
            this.Name = name;
            this.Size = size;
            this.Type = isDir ? FtpFileSystemObjectType.Directory : FtpFileSystemObjectType.File;
            this.Modified = lastModifiedTime;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.Type == FtpFileSystemObjectType.File)
            {
                builder.Append("FILE");
            }
            else if (this.Type == FtpFileSystemObjectType.Directory)
            {
                builder.Append("DIR ");
            }
            else if (this.Type == FtpFileSystemObjectType.Link)
            {
                builder.Append("LINK");
            }
            builder.Append("   ");
            builder.Append(this.Name);
            if (this.Type == FtpFileSystemObjectType.File)
            {
                builder.Append("      ");
                builder.Append("(");
                builder.Append(this.Size.FileSizeToString());
                builder.Append(")");
            }
            if (this.Created != DateTime.MinValue)
            {
                builder.Append("      ");
                builder.Append("Created : ");
                builder.Append(this.Created.ToString());
            }
            if (this.Modified != DateTime.MinValue)
            {
                builder.Append("      ");
                builder.Append("Modified : ");
                builder.Append(this.Modified.ToString());
            }
            return builder.ToString();
        }

        public string Name
        {
            get
            {
                if ((this.m_name == null) && (this.FullName != null))
                {
                    return this.FullName.GetFtpFileName();
                }
                return this.m_name;
            }
            set
            {
                this.m_name = value;
            }
        }
    }
}

