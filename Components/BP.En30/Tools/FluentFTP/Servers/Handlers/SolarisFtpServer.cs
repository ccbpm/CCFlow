namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class SolarisFtpServer : FtpBaseServer
    {
        public override bool DetectBySyst(string message)
        {
            return message.Contains("SUNOS");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.SolarisFTP;
        }
    }
}

