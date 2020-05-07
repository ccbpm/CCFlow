namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;
    using System.Text.RegularExpressions;

    public class OpenVmsServer : FtpBaseServer
    {
        public override string[] DefaultCapabilities()
        {
            return new string[] { "CWD", "DELE", "LIST", "NLST", "MKD", "MDTM", "PASV", "PORT", "PWD", "QUIT", "RNFR", "RNTO", "SITE", "STOR", "STRU", "TYPE" };
        }

        public override bool DetectBySyst(string message)
        {
            return message.Contains("OpenVMS");
        }

        public override bool DetectByWelcome(string message)
        {
            return message.Contains("OpenVMS FTPD");
        }

        public virtual FtpParser GetParser()
        {
            return FtpParser.VMS;
        }

        public override bool IsAbsolutePath(string path)
        {
            return new Regex(@"[A-Za-z$._]*:\[[A-Za-z0-9$_.]*\]").Match(path).Success;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.OpenVMS;
        }
    }
}

