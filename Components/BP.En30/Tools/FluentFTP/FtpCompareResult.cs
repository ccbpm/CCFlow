namespace FluentFTP
{
    using System;

    public enum FtpCompareResult
    {
        ChecksumNotSupported = 4,
        Equal = 1,
        FileNotExisting = 3,
        NotEqual = 2
    }
}

