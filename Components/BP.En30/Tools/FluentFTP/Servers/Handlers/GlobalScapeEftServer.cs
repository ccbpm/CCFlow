namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class GlobalScapeEftServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("EFT Server");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.GlobalScapeEFT;
        }
    }
}

