namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpVerify
    {
        Delete = 2,
        None = 0,
        OnlyChecksum = 8,
        Retry = 1,
        Throw = 4
    }
}

