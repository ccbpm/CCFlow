namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class WindowsIisServer : FtpBaseServer
    {
        public override bool DetectByWelcome(string message)
        {
            return message.Contains("Microsoft FTP Service");
        }

        public virtual FtpParser GetParser()
        {
            return FtpParser.Windows;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.WindowsServerIIS;
        }
    }
}

