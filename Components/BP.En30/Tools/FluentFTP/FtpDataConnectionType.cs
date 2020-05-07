namespace FluentFTP
{
    using System;

    public enum FtpDataConnectionType
    {
        AutoPassive,
        PASV,
        PASVEX,
        EPSV,
        AutoActive,
        PORT,
        EPRT
    }
}

