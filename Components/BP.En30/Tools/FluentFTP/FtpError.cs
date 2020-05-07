namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpError
    {
        DeleteProcessed = 1,
        None = 0,
        Stop = 2,
        Throw = 4
    }
}

