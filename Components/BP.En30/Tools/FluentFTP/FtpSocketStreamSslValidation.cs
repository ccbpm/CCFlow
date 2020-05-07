namespace FluentFTP
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void FtpSocketStreamSslValidation(FtpSocketStream stream, FtpSslValidationEventArgs e);
}

