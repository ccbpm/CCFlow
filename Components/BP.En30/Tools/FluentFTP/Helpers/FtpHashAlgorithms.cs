namespace FluentFTP.Helpers
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;

    public static class FtpHashAlgorithms
    {
        private static readonly Dictionary<FtpHashAlgorithm, string> EnumToName;
        private static readonly Dictionary<string, FtpHashAlgorithm> NameToEnum;

        static FtpHashAlgorithms()
        {
            Dictionary<string, FtpHashAlgorithm> dictionary1 = new Dictionary<string, FtpHashAlgorithm>();
            dictionary1.Add("SHA-1", FtpHashAlgorithm.SHA1);
            dictionary1.Add("SHA-256", FtpHashAlgorithm.SHA256);
            dictionary1.Add("SHA-512", FtpHashAlgorithm.SHA512);
            dictionary1.Add("MD5", FtpHashAlgorithm.MD5);
            dictionary1.Add("CRC", FtpHashAlgorithm.CRC);
            NameToEnum = dictionary1;
            Dictionary<FtpHashAlgorithm, string> dictionary2 = new Dictionary<FtpHashAlgorithm, string>();
            dictionary2.Add(FtpHashAlgorithm.SHA1, "SHA-1");
            dictionary2.Add(FtpHashAlgorithm.SHA256, "SHA-256");
            dictionary2.Add(FtpHashAlgorithm.SHA512, "SHA-512");
            dictionary2.Add(FtpHashAlgorithm.MD5, "MD5");
            dictionary2.Add(FtpHashAlgorithm.CRC, "CRC");
            EnumToName = dictionary2;
        }

        public static FtpHashAlgorithm FromString(string ftpHashAlgorithm)
        {
            if (!NameToEnum.ContainsKey(ftpHashAlgorithm))
            {
                throw new NotImplementedException("Unknown hash algorithm: " + ftpHashAlgorithm);
            }
            return NameToEnum[ftpHashAlgorithm];
        }

        public static string ToString(FtpHashAlgorithm ftpHashAlgorithm)
        {
            if (!EnumToName.ContainsKey(ftpHashAlgorithm))
            {
                return ftpHashAlgorithm.ToString();
            }
            return EnumToName[ftpHashAlgorithm];
        }
    }
}

