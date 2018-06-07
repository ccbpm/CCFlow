using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace BP.EAI.Plugins.DDSDK
{
    public class SignPackageHelper
    {
        #region Sha1Hex
        public static string Sha1Hex(string value)
        {
            SHA1 algorithm = SHA1.Create();
            byte[] data = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }
        #endregion

        #region CreateNonceStr
        /// <summary>  
        /// 创建随机字符串  
        /// </summary>  
        /// <returns></returns>  
        public static string CreateNonceStr()
        {
            int length = 16;
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "";
            Random rad = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }
        #endregion

        #region ConvertToUnixTimeStamp
        /// <summary>    
        /// 将DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time">时间</param>    
        /// <returns>double</returns>    
        public static string ConvertToUnixTimeStamp(DateTime time)
        {
            //int intResult = 0;
            //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            //intResult = Convert.ToInt32((time - startTime).TotalSeconds);
            //return intResult;

            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion
    }
}
