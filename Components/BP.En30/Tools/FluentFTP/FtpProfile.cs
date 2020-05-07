namespace FluentFTP
{
    using System;
    using System.Net;
    using System.Security.Authentication;
    using System.Text;

    [Serializable]
    public class FtpProfile
    {
        public NetworkCredential Credentials;
        public FtpDataConnectionType DataConnection = FtpDataConnectionType.PASV;
        public System.Text.Encoding Encoding;
        public FtpEncryptionMode Encryption;
        public string Host;
        public SslProtocols Protocols;
        public int RetryAttempts;
        public int SocketPollInterval;
        public int Timeout;

        public string ToCode()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("// add this above your namespace declaration");
            builder.AppendLine("using FluentFTP;");
            builder.AppendLine("using System.Text;");
            builder.AppendLine("using System.Net;");
            builder.AppendLine("using System.Security.Authentication;");
            builder.AppendLine();
            builder.AppendLine("// add this to create and configure the FTP client");
            builder.AppendLine("var client = new FtpClient();");
            builder.AppendLine("client.LoadProfile(new FtpProfile {");
            builder.AppendLine("\tHost = " + this.Host.EscapeStringLiteral() + ",");
            builder.AppendLine("\tCredentials = new NetworkCredential(" + this.Credentials.UserName.EscapeStringLiteral() + ", " + this.Credentials.Password.EscapeStringLiteral() + "),");
            builder.AppendLine("\tEncryption = FtpEncryptionMode." + this.Encryption.ToString() + ",");
            builder.AppendLine("\tProtocols = SslProtocols." + this.Protocols.ToString() + ",");
            builder.AppendLine("\tDataConnection = FtpDataConnectionType." + this.DataConnection.ToString() + ",");
            string str = this.Encoding.ToString();
            if (str.Contains("+"))
            {
                builder.AppendLine("\tEncoding = " + str.Substring(0, str.IndexOf('+')) + ",");
            }
            else
            {
                builder.AppendLine("\tEncoding = " + str + ",");
            }
            if (this.Timeout != 0)
            {
                builder.AppendLine("\tTimeout = " + this.Timeout + ",");
            }
            if (this.SocketPollInterval != 0)
            {
                builder.AppendLine("\tSocketPollInterval = " + this.SocketPollInterval + ",");
            }
            if (this.RetryAttempts != 0)
            {
                builder.AppendLine("\tRetryAttempts = " + this.RetryAttempts + ",");
            }
            builder.AppendLine("});");
            if (this.Encryption != FtpEncryptionMode.None)
            {
                builder.AppendLine("// if you want to accept any certificate then set ValidateAnyCertificate=true and delete the following event handler");
                builder.AppendLine("client.ValidateCertificate += new FtpSslValidation(delegate (FtpClient control, FtpSslValidationEventArgs e) {");
                builder.AppendLine("\t// add your logic to test if the SSL certificate is valid (see the FAQ for examples)");
                builder.AppendLine("\te.Accept = true;");
                builder.AppendLine("});");
            }
            builder.AppendLine("client.Connect();");
            return builder.ToString();
        }
    }
}

