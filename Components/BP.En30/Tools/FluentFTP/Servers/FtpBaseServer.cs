namespace FluentFTP.Servers
{
    using FluentFTP;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class FtpBaseServer
    {
        protected FtpBaseServer()
        {
        }

        public virtual bool CreateDirectory(FtpClient client, string path, string ftppath, bool force)
        {
            return false;
        }

        public async virtual Task<bool> CreateDirectoryAsync(FtpClient client, string path, string ftppath, bool force, CancellationToken token)
        {
            return false;
        }

        public virtual string[] DefaultCapabilities()
        {
            return null;
        }

        public virtual bool DeleteDirectory(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options)
        {
            return false;
        }

        public async virtual Task<bool> DeleteDirectoryAsync(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options, CancellationToken token)
        {
            return false;
        }

        public virtual bool DetectBySyst(string message)
        {
            return false;
        }

        public virtual bool DetectByWelcome(string message)
        {
            return false;
        }

        public virtual FtpParser GetParser()
        {
            return FtpParser.Unix;
        }

        public virtual bool IsAbsolutePath(string path)
        {
            return false;
        }

        public virtual bool RecursiveList()
        {
            return false;
        }

        public virtual FtpServer ToEnum()
        {
            return FtpServer.Unknown;
        }


    }
}

