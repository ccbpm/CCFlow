using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace DecryptAndEncryptionHelper
{
    public class DecryptAndEncryptionHelper
    {
        private readonly SymmetricAlgorithm _symmetricAlgorithm;
        private const String DefKey = "qazwsxedcrfvtgb!@#$%^&*(tgbrfvedcwsxqaz)(*&^%$#@!qaz";
        private String _key = "";
        public String Key
        {
            get { return _key; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _key = value;
                }
                else
                {
                    _key = DefKey;
                }
            }
        }

        private const String DefIV = "tgbrfvedcwsxqaz)(*&^%$#@!qazwsxedcrfvtgb!@#$%^&*(qaz";
        private String _iv = "";
        public String IV
        {
            get { return _iv; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _iv = value;
                }
                else
                {
                    _iv = DefIV;
                }
            }
        }
        public DecryptAndEncryptionHelper()
        {
            _symmetricAlgorithm = new RijndaelManaged();
        }

        public DecryptAndEncryptionHelper(String Key, String IV)
        {
            _symmetricAlgorithm = new RijndaelManaged();
            _key = String.IsNullOrEmpty(Key) ? DefKey : Key;
            _iv = String.IsNullOrEmpty(IV) ? DefIV : IV;
        }
        /// <summary>
        /// Get Key
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            _symmetricAlgorithm.GenerateKey();
            byte[] bytTemp = _symmetricAlgorithm.Key;
            int KeyLength = bytTemp.Length;
            if (_key.Length > KeyLength)
                _key = _key.Substring(0, KeyLength);
            else if (_key.Length < KeyLength)
                _key = _key.PadRight(KeyLength, '#');
            return ASCIIEncoding.ASCII.GetBytes(_key);
        }

        /// <summary>
        /// Get IV
        /// </summary>
        private byte[] GetLegalIV()
        {
            _symmetricAlgorithm.GenerateIV();
            byte[] bytTemp = _symmetricAlgorithm.IV;
            int IVLength = bytTemp.Length;
            if (_iv.Length > IVLength)
                _iv = _iv.Substring(0, IVLength);
            else if (_iv.Length < IVLength)
                _iv = _iv.PadRight(IVLength, '#');
            return ASCIIEncoding.ASCII.GetBytes(_iv);
        }

        /// <summary>
        /// Encrypto 加密
        /// </summary>
        public string Encrypto(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            _symmetricAlgorithm.Key = GetLegalKey();
            _symmetricAlgorithm.IV = GetLegalIV();
            ICryptoTransform encrypto = _symmetricAlgorithm.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }

        /// <summary>
        /// Decrypto 解密
        /// </summary>
        public string Decrypto(string Source)
        {
            byte[] bytIn = Convert.FromBase64String(Source);
            MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
            _symmetricAlgorithm.Key = GetLegalKey();
            _symmetricAlgorithm.IV = GetLegalIV();
            ICryptoTransform encrypto = _symmetricAlgorithm.CreateDecryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

    }
}
