namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpHashAlgorithm
    {
        CRC = 0x10,
        MD5 = 8,
        NONE = 0,
        SHA1 = 1,
        SHA256 = 2,
        SHA512 = 4
    }
}

