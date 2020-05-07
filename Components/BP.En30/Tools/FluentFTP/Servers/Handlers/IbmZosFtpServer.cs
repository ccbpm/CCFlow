namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class IbmZosFtpServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("IBM FTP CS");
        }

        public virtual FtpParser GetParser()
        {
            return FtpParser.IBM;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.IBMzOSFTP;
        }
    }
}

