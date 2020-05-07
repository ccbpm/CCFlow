namespace FluentFTP
{
    using System;

    public enum FtpRemoteExists
    {
        NoCheck,
        Skip,
        Overwrite,
        Append,
        AppendNoCheck
    }
}

