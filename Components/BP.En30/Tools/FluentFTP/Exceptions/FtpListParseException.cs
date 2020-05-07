namespace FluentFTP.Exceptions
{
    using FluentFTP;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FtpListParseException : FtpException
    {
        public FtpListParseException() : base("Cannot parse file listing!")
        {
        }

        protected FtpListParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

