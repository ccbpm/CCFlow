namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpPermission : uint
    {
        Execute = 1,
        None = 0,
        Read = 4,
        Write = 2
    }
}

