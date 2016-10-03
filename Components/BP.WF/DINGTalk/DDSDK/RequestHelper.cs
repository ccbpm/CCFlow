using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace BP.WF.DINGTalk.DDSDK
{
    /// <summary>  
    /// 请求协助类  
    /// </summary>  
    public class RequestHelper
    {
        #region Get
        /// <summary>  
        /// 执行基本的命令方法,以Get方式  
        /// </summary>  
        /// <param name="apiurl"></param>  
        /// <returns></returns>  
        public static String Get(string apiurl)
        {
            WebRequest request = WebRequest.Create(@apiurl);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encode);
            string resultJson = reader.ReadToEnd();
            return resultJson;
        }
        #endregion

        #region Post
        /// <summary>  
        /// 以Post方式提交命令  
        /// </summary>  
        public static String Post(string apiurl, string jsonString)
        {
            WebRequest request = WebRequest.Create(@apiurl);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] bs = Encoding.UTF8.GetBytes(jsonString);
            request.ContentLength = bs.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(bs, 0, bs.Length);
            newStream.Close();

            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encode);
            string resultJson = reader.ReadToEnd();
            return resultJson;
        }
        #endregion
    } 
}
