namespace FluentFTP
{
    using FluentFTP.Helpers.Hashing;
    using FluentFTP.Streams;
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class FtpHash
    {
        private FtpHashAlgorithm m_algorithm;
        private string m_value;

        internal FtpHash()
        {
        }

        public bool Verify(Stream istream)
        {
            if (this.IsValid)
            {
                HashAlgorithm algorithm = null;
                switch (this.m_algorithm)
                {
                    case FtpHashAlgorithm.SHA1:
                        algorithm = new SHA1CryptoServiceProvider();
                        break;

                    case FtpHashAlgorithm.SHA256:
                        algorithm = new SHA256CryptoServiceProvider();
                        break;

                    case FtpHashAlgorithm.SHA512:
                        algorithm = new SHA512CryptoServiceProvider();
                        break;

                    case FtpHashAlgorithm.MD5:
                        algorithm = new MD5CryptoServiceProvider();
                        break;

                    case FtpHashAlgorithm.CRC:
                        algorithm = new CRC32();
                        break;

                    default:
                        throw new NotImplementedException("Unknown hash algorithm: " + this.m_algorithm.ToString());
                }
                try
                {
                    byte[] buffer = null;
                    StringBuilder builder = new StringBuilder();
                    buffer = algorithm.ComputeHash(istream);
                    if (buffer != null)
                    {
                        foreach (byte num2 in buffer)
                        {
                            builder.Append(num2.ToString("x2"));
                        }
                        return builder.ToString().Equals(this.m_value, StringComparison.OrdinalIgnoreCase);
                    }
                }
                finally
                {
                    if (algorithm != null)
                    {
                        algorithm.Dispose();
                    }
                }
            }
            return false;
        }

        public bool Verify(string file)
        {
            using (Stream stream = FtpFileStream.GetFileReadStream(null, file, false, 0x100000L, 0L))
            {
                return this.Verify(stream);
            }
        }

        public FtpHashAlgorithm Algorithm
        {
            get
            {
                return this.m_algorithm;
            }
            internal set
            {
                this.m_algorithm = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return ((this.m_algorithm != FtpHashAlgorithm.NONE) && !string.IsNullOrEmpty(this.m_value));
            }
        }

        public string Value
        {
            get
            {
                return this.m_value;
            }
            internal set
            {
                this.m_value = value;
            }
        }
    }
}

