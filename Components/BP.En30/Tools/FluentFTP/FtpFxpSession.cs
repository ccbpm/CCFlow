namespace FluentFTP
{
    using System;
    using System.Runtime.CompilerServices;

    public class FtpFxpSession : IDisposable
    {
        public FtpClient ProgressServer;
        public FtpClient SourceServer;
        public FtpClient TargetServer;

        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                if (this.SourceServer != null)
                {
                    this.SourceServer.AutoDispose();
                    this.SourceServer = null;
                }
                if (this.TargetServer != null)
                {
                    this.TargetServer.AutoDispose();
                    this.TargetServer = null;
                }
                if (this.ProgressServer != null)
                {
                    this.ProgressServer.AutoDispose();
                    this.ProgressServer = null;
                }
                this.IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public bool IsDisposed { get; private set; }
    }
}

