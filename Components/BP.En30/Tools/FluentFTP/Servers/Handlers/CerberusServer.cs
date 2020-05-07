namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class CerberusServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("Cerberus FTP");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.Cerberus;
        }
    }
}

