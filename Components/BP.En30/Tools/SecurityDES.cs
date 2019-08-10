using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections;
using System.IO;
using System.Data;
using BP.Sys;
using BP;
using BP.En;
using System.Data.Sql;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace BP.Tools
{
    /// <summary>
    /// 字符串加解密
    /// </summary>
    public sealed class SecurityDES
    {
        //默认密钥向量
        private static byte[] IV = { 0x65, 0x88, 0x35, 0x71, 0x60, 0x1B, 0x2D, 0x7F };
        private static string key = "ligy@163";
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string Encrypt(string toEncryptString)
        {
            try
            {
                if (toEncryptString == null || toEncryptString == "" || toEncryptString == string.Empty)
                    return "";
                byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
                byte[] rgbIV = IV;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(toEncryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return toEncryptString;
            }
        }
    }
}
