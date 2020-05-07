namespace FluentFTP
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class FtpCommandException : FtpException
    {
        private string _code;

        public FtpCommandException(FtpReply reply) : this(reply.Code, reply.ErrorMessage)
        {
        }

        protected FtpCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FtpCommandException(string code, string message) : base(message)
        {
            this.CompletionCode = code;
        }

        public string CompletionCode
        {
            get
            {
                return this._code;
            }
            private set
            {
                this._code = value;
            }
        }

        public FtpResponseType ResponseType
        {
            get
            {
                if (this._code != null)
                {
                    switch (this._code[0])
                    {
                        case '4':
                            return FtpResponseType.TransientNegativeCompletion;

                        case '5':
                            return FtpResponseType.PermanentNegativeCompletion;
                    }
                }
                return FtpResponseType.None;
            }
        }
    }
}

