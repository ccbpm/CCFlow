namespace FluentFTP
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FtpSecurityNotAvailableException : FtpException
    {
        public FtpSecurityNotAvailableException() : base("Security is not available on the server.")
        {
        }

        public FtpSecurityNotAvailableException(string message) : base(message)
        {
        }

        protected FtpSecurityNotAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

