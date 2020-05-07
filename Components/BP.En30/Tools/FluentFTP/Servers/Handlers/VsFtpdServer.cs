namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class VsFtpdServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("(vsFTPd");
        }

        public override bool RecursiveList()
        {
            return false;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.VsFTPd;
        }
    }
}

