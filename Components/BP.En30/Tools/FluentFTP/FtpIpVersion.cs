namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpIpVersion
    {
        ANY = 3,
        IPv4 = 1,
        IPv6 = 2
    }
}

