using System;
using System.Net;
using System.Windows;
using System.IO;

namespace BP.CY
{
    public class StringHandler
    {
        /// <summary>
        /// 将流转换字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ToString(Stream stream)
        {
            byte[] b = (stream as MemoryStream).GetBuffer();
            return Convert.ToBase64String(b);
        }

        /// <summary>
        /// 将字符串转换成流
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Stream ToStream(string str)
        {
            byte[] b = Convert.FromBase64String(str);

            return (new MemoryStream(b));
        }
    }
}
