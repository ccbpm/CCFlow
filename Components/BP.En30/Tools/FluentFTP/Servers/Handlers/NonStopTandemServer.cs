namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class NonStopTandemServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return (message.Contains("FTP SERVER ") && message.Contains(" TANDEM "));
        }

        public virtual FtpParser GetParser()
        {
            return FtpParser.NonStop;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.NonStopTandem;
        }
    }
}

