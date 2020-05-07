namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class HomegateFtpServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("Homegate FTP Server");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.HomegateFTP;
        }
    }
}

