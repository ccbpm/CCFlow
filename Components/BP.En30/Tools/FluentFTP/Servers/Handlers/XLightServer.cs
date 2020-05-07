namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class XLightServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("Xlight FTP Server");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.XLight;
        }
    }
}

