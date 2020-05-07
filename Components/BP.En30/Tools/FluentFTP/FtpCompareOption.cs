namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpCompareOption
    {
        Auto = 0,
        Checksum = 4,
        DateModified = 2,
        Size = 1
    }
}

