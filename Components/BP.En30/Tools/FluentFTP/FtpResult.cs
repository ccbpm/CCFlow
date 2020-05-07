namespace FluentFTP
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class FtpResult
    {
        public System.Exception Exception;
        public bool IsDownload;
        public bool IsFailed;
        public bool IsSkipped;
        public bool IsSkippedByRule;
        public bool IsSuccess;
        public string Name;
        public long Size;
        public FtpFileSystemObjectType Type;

        public FtpListItem ToListItem(bool useLocalPath)
        {
            return new FtpListItem { Type = this.Type, Size = this.Size, Name = this.Name, FullName = useLocalPath ? this.LocalPath : this.RemotePath };
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.IsSkipped)
            {
                builder.Append("Skipped:     ");
            }
            else if (this.IsFailed)
            {
                builder.Append("Failed:      ");
            }
            else if (this.IsDownload)
            {
                builder.Append("Downloaded:  ");
            }
            else
            {
                builder.Append("Uploaded:    ");
            }
            if (this.IsDownload)
            {
                builder.Append(this.RemotePath);
                builder.Append("  -->  ");
                builder.Append(this.LocalPath);
            }
            else
            {
                builder.Append(this.LocalPath);
                builder.Append("  -->  ");
                builder.Append(this.RemotePath);
            }
            if ((this.IsFailed && (this.Exception != null)) && (this.Exception.Message != null))
            {
                builder.Append("  [!]  ");
                builder.Append(this.Exception.Message);
            }
            return builder.ToString();
        }

        public string LocalPath { get; set; }

        public string RemotePath { get; set; }
    }
}

