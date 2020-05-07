namespace FluentFTP
{
    using System;
    using System.IO;
    using System.Net.Security;

    internal class FtpSslStream : SslStream
    {
        private bool sentCloseNotify;

        public FtpSslStream(Stream innerStream) : base(innerStream)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen) : base(innerStream, leaveInnerStreamOpen)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
        {
        }

        public FtpSslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, encryptionPolicy)
        {
        }

        public override void Close()
        {
            try
            {
                if (!this.sentCloseNotify)
                {
                    SslDirectCall.CloseNotify(this);
                    this.sentCloseNotify = true;
                }
            }
            finally
            {
                base.Close();
            }
        }
    }
}

