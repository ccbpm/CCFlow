namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class WindowsCeServer : FtpBaseServer
    {
        public override bool DetectBySyst(string message)
        {
            return message.Contains("Windows_CE");
        }

        public virtual FtpParser GetParser()
        {
            return FtpParser.Windows;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.WindowsCE;
        }
    }
}

