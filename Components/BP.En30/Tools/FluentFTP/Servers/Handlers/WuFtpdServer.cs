namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;

    public class WuFtpdServer : FtpBaseServer
    {
        public override string[] DefaultCapabilities()
        {
            return new string[] { 
                "ABOR", "ACCT", "ALLO", "APPE", "CDUP", "CWD", "DELE", "EPSV", "EPRT", "HELP", "LIST", "LPRT", "LPSV", "MKD", "MDTM", "MODE", 
                "NLST", "NOOP", "PASS", "PASV", "PORT", "PWD", "QUIT", "REST", "RETR", "RMD", "RNFR", "RNTO", "SITE", "SIZE", "STAT", "STOR", 
                "STOU", "STRU", "SYST", "TYPE"
             };
        }

        public override bool DetectByWelcome(string message)
        {
            if (!message.Contains("Version wuftpd") && !message.Contains("Version wu-"))
            {
                return false;
            }
            return true;
        }

        public override bool RecursiveList()
        {
            return false;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.WuFTPd;
        }
    }
}

