namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class GlFtpdServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("glFTPd ");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.glFTPd;
        }
    }
}

