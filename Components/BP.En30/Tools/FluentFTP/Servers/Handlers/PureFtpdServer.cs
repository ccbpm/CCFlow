namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class PureFtpdServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("Pure-FTPd");
        }

        public override bool RecursiveList()
        {
            return true;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.PureFTPd;
        }
    }
}

