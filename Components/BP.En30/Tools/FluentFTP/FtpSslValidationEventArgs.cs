namespace FluentFTP
{
    using System;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class FtpSslValidationEventArgs : EventArgs
    {
        private bool m_accept;
        private X509Certificate m_certificate;
        private X509Chain m_chain;
        private SslPolicyErrors m_policyErrors;

        public bool Accept
        {
            get
            {
                return this.m_accept;
            }
            set
            {
                this.m_accept = value;
            }
        }

        public X509Certificate Certificate
        {
            get
            {
                return this.m_certificate;
            }
            set
            {
                this.m_certificate = value;
            }
        }

        public X509Chain Chain
        {
            get
            {
                return this.m_chain;
            }
            set
            {
                this.m_chain = value;
            }
        }

        public SslPolicyErrors PolicyErrors
        {
            get
            {
                return this.m_policyErrors;
            }
            set
            {
                this.m_policyErrors = value;
            }
        }
    }
}

