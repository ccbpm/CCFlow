namespace FluentFTP.Proxy
{
    using FluentFTP;
    using System;

    public class FtpClientBlueCoatProxy : FtpClientProxy
    {
        public FtpClientBlueCoatProxy(ProxyInfo proxy) : base(proxy)
        {
            base.ConnectionType = "User@Host";
        }

        protected override FtpClient Create()
        {
            return new FtpClientBlueCoatProxy(base.Proxy);
        }

        protected override void Handshake()
        {
            if (base.Proxy.Credentials != null)
            {
                this.Authenticate(base.Proxy.Credentials.UserName, base.Proxy.Credentials.Password);
            }
            object[] objArray1 = new object[] { base.Credentials.UserName, "@", base.Host, ":", base.Port };
            base.Credentials.UserName = string.Concat(objArray1);
            FtpReply reply = base.GetReply();
            if (reply.Code == "220")
            {
                base.LogLine(FtpTraceLevel.Info, "Status: Server is ready for the new client");
            }
            base.HandshakeReply = reply;
        }
    }
}

