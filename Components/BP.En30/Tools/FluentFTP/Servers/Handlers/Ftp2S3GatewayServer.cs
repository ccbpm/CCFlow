namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class Ftp2S3GatewayServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("FTP2S3 gateway");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.FTP2S3Gateway;
        }
    }
}

