using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BP.DA;

namespace BP.GPM
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class Crypto
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="encryptString">传入加密字符串</param>
        /// <returns>加密后字符</returns>
        public static string EncryptString(string encryptString)
        {
            string strEncrypt = encryptString;
            if (DataType.IsNullOrEmpty(encryptString)) return encryptString;

            strEncrypt = Crypto.MD5_Encrypt(strEncrypt);
            strEncrypt = Crypto.MD5_Encrypt(strEncrypt);
            return strEncrypt.ToUpper();
        }
        /// <summary> 
        /// 使用指定的编码将字符串散列 
        /// </summary>
        /// <param name="encryptString">要散列的字符串</param> 
        /// <returns>散列后的字符串</returns> 
        public static string MD5_Encrypt(string encryptString)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] source = md5.ComputeHash(Encoding.Default.GetBytes(encryptString));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                sBuilder.Append(source[i].ToString("X2"));
            }
            return sBuilder.ToString();
        }

        //SHA1 加密 （HASH算法没有解密）
        /// <summary>
        /// 利用SHA1加密一个字符串
        /// </summary>
        public static string SHA1_Encrypt(string encryptString)
        {
            byte[] StrRes = Encoding.Default.GetBytes(encryptString);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();
        }
    }
}
