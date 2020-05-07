namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class BFtpdServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("bftpd ");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.BFTPd;
        }
    }
}

