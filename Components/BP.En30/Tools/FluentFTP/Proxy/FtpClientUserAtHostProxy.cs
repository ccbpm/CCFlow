namespace FluentFTP.Proxy
{
    using FluentFTP;
    using System;

    public class FtpClientUserAtHostProxy : FtpClientProxy
    {
        public FtpClientUserAtHostProxy(ProxyInfo proxy) : base(proxy)
        {
            base.ConnectionType = "User@Host";
        }

        protected override FtpClient Create()
        {
            return new FtpClientUserAtHostProxy(base.Proxy);
        }

        protected override void Handshake()
        {
            if (base.Proxy.Credentials != null)
            {
                this.Authenticate(base.Proxy.Credentials.UserName, base.Proxy.Credentials.Password);
            }
            object[] objArray1 = new object[] { base.Credentials.UserName, "@", base.Host, ":", base.Port };
            base.Credentials.UserName = string.Concat(objArray1);
        }
    }
}

