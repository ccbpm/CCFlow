namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class CrushFtpServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("CrushFTP Server");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.CrushFTP;
        }
    }
}

