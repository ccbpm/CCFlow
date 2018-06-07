using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace BP.EAI.Plugins.DDSDK
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

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="downLoadUrl">下载地址</param>
        /// <param name="saveFullName">保存路径</param>
        /// <returns></returns>
        public static bool HttpDownLoadFile(string downLoadUrl, string saveFullName)
        {
            bool flagDown = false;
            HttpWebRequest httpWebRequest = null;
            try
            {
                //根据url获取远程文件流
                httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(downLoadUrl);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream sr = httpWebResponse.GetResponseStream();
                
                //创建本地文件写入流
                FileStream fs = new FileStream(saveFullName, FileMode.Create);
                long totalDownLoadByte = 0;
                byte[] by = new byte[1024];
                int osize = sr.Read(by, 0, by.Length);
                while (osize > 0)
                {
                    fs.Write(by, 0, osize);
                    osize = sr.Read(by, 0, by.Length);
                }
                System.Threading.Thread.Sleep(100);
                flagDown = true;
                fs.Close();
                sr.Close();
                httpWebResponse.Close();
            }
            catch (Exception ex)
            {
                if (httpWebRequest != null)
                    httpWebRequest.Abort();
            }
            return flagDown;
        }
    }
}
