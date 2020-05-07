namespace FluentFTP
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FtpException : Exception
    {
        public FtpException(string message) : base(message)
        {
        }

        protected FtpException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FtpException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

