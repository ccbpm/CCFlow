namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class FileZillaServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("FileZilla Server");
        }

        public override bool RecursiveList()
        {
            return false;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.FileZilla;
        }
    }
}

