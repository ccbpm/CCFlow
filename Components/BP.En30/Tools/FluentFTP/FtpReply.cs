namespace FluentFTP
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    [StructLayout(LayoutKind.Sequential)]
    public struct FtpReply
    {
        private string m_respCode;
        private string m_respMessage;
        private string m_infoMessages;
        public FtpResponseType Type
        {
            get
            {
                if ((this.Code != null) && (this.Code.Length > 0))
                {
                    int num;
                    char ch = this.Code[0];
                    if (int.TryParse(ch.ToString(), out num))
                    {
                        return (FtpResponseType) num;
                    }
                }
                return FtpResponseType.None;
            }
        }
        public string Code
        {
            get
            {
                return this.m_respCode;
            }
            set
            {
                this.m_respCode = value;
            }
        }
        public string Message
        {
            get
            {
                return this.m_respMessage;
            }
            set
            {
                this.m_respMessage = value;
            }
        }
        public string InfoMessages
        {
            get
            {
                return this.m_infoMessages;
            }
            set
            {
                this.m_infoMessages = value;
            }
        }
        public bool Success
        {
            get
            {
                if (((this.Code == null) || (this.Code.Length <= 0)) || (((this.Code[0] != '1') && (this.Code[0] != '2')) && (this.Code[0] != '3')))
                {
                    return false;
                }
                return true;
            }
        }
        public string ErrorMessage
        {
            get
            {
                string str = "";
                if (this.Success)
                {
                    return str;
                }
                if ((this.InfoMessages != null) && (this.InfoMessages.Length > 0))
                {
                    char[] separator = new char[] { '\n' };
                    string[] strArray = this.InfoMessages.Split(separator);
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        str = str + Regex.Replace(strArray[i], "^[0-9]{3}-", "").Trim() + "; ";
                    }
                }
                return (str + this.Message);
            }
        }
    }
}

