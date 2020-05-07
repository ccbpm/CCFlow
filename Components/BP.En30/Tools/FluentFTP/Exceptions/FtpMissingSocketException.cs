namespace FluentFTP.Exceptions
{
    using FluentFTP;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FtpMissingSocketException : FtpException
    {
        public FtpMissingSocketException(Exception innerException) : base("Socket is missing", innerException)
        {
        }

        protected FtpMissingSocketException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

