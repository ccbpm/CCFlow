namespace FluentFTP
{
    using System;
    using System.Runtime.CompilerServices;

    internal class FtpSizeReply
    {
        public FtpReply Reply;

        public long FileSize { get; set; }
    }
}

