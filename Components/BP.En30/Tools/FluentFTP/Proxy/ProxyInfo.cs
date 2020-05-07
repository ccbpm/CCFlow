namespace FluentFTP.Proxy
{
    using System;
    using System.Net;
    using System.Runtime.CompilerServices;

    public class ProxyInfo
    {
        public NetworkCredential Credentials { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }
    }
}

