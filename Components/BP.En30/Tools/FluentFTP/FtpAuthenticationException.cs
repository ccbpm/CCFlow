namespace FluentFTP
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FtpAuthenticationException : FtpCommandException
    {
        public FtpAuthenticationException(FtpReply reply) : base(reply)
        {
        }

        protected FtpAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FtpAuthenticationException(string code, string message) : base(code, message)
        {
        }
    }
}

