namespace FluentFTP.Proxy
{
    using FluentFTP;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class FtpClientProxy : FtpClient
    {
        private ProxyInfo _proxy;

        protected FtpClientProxy(ProxyInfo proxy)
        {
            this._proxy = proxy;
        }

        protected override void Connect(FtpSocketStream stream)
        {
            stream.Connect(this.Proxy.Host, this.Proxy.Port, base.InternetProtocolVersions);
        }

        protected override Task ConnectAsync(FtpSocketStream stream, CancellationToken token)
        {
            return stream.ConnectAsync(this.Proxy.Host, this.Proxy.Port, base.InternetProtocolVersions, token);
        }

        protected ProxyInfo Proxy
        {
            get
            {
                return this._proxy;
            }
        }
    }
}

