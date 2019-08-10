using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Linq;
using System.IO;

namespace BP.Web
{
    public static class HttpContextHelper
    {
        /// <summary>
        /// 获取当前的HttpContext
        /// </summary>
        public static HttpContext Current
        {
            get
            {
                return HttpContext.Current;
            }
        }

        /// <summary>
        /// 获取当前的 Session
        /// </summary>
        public static HttpSessionState Session
        {
            get
            {
                return Current.Session;
            }
        }

        public static string SessionID
        {
            get
            {
                return Session.SessionID;
            }
        }

        public static void SessionClear()
        {
            Session.Clear();
        }

        /// <summary>
        /// 获取当前的 Request
        /// </summary>
        public static HttpRequest Request
        {
            get
            {
                return Current.Request;
            }
        }

        /// <summary>
        /// 获取当前的 Response
        /// </summary>
        public static HttpResponse Response
        {
            get
            {
                return Current.Response;
            }
        }

        public static void ResponseWrite(string content)
        {
            Response.Write(content);
        }

        public static void ResponseWriteString(string content, Encoding encoding)
        {
            Response.ContentType = "text/html";
            Response.ContentEncoding = encoding;
            Response.Write(content);
            Response.End();
        }

        public static void ResponseWriteJson(string json, Encoding encoding)
        {
            Response.ContentType = "application/json";
            Response.ContentEncoding = encoding;
            Response.Write(json);
            Response.End();
        }

        public static void ResponseWriteScript(string script, Encoding encoding)
        {
            Response.ContentType = "application/javascript";
            Response.ContentEncoding = encoding;
            Response.Write(script);
            Response.End();
        }

        /// <summary>
        /// 向Response中写入文件数据
        /// </summary>
        /// <param name="fileData">文件数据，字节流</param>
        /// <param name="fileName">客户端显示的文件名</param>
        public static void ResponseWriteFile(byte[] fileData, string fileName, string contentType = "application/octet-stream")
        {
            Response.ContentType = String.Format("{0};charset=utf8", contentType);

            // 在Response的Header中设置下载文件的文件名，这样客户端浏览器才能正确显示下载的文件名
            // 注意这里要用HttpUtility.UrlEncode编码文件名，否则有些浏览器可能会显示乱码文件名
            var contentDisposition = "attachment;" + "filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8);
            // Response.Headers.Add("Content-Disposition", contentDisposition); IIS 7之前的版本可能不支持此写法
            Response.AddHeader("Content-Disposition", contentDisposition);

            // 在Response的Header中设置下载文件的大小，这样客户端浏览器才能正确显示下载的进度
            Response.AddHeader("Content-Length", fileData.Length.ToString());

            Response.BinaryWrite(fileData);
            Response.End();
            Response.Close();
        }

        /// <summary>
        /// 向Response中写入文件
        /// </summary>
        /// <param name="filePath">文件的完整路径，含文件名</param>
        /// <param name="clientFileName">客户端显示的文件名。若为空，自动从filePath参数中提取文件名。</param>
        public static void ResponseWriteFile(string filePath, string clientFileName = null, string contentType = "application/octet-stream")
        {
            if (String.IsNullOrEmpty(clientFileName))
                clientFileName = Path.GetFileName(filePath);

            Response.AppendHeader("Content-Disposition", "attachment;filename=" + clientFileName);
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = String.Format("{0};charset=utf8", contentType);

            Response.WriteFile(filePath);
            Response.End();
            Response.Close();
        }

        public static void ResponseWriteHeader(string key, string stringvalues)
        {
            Response.AddHeader(key, stringvalues);
        }

        /// <summary>
        /// 添加cookie
        /// </summary>
        /// <param name="cookieValues">Dictionary</param>
        /// <param name="expires"></param>
        /// <param name="cookieName">.net core 中无需传此参数，传了也会被忽略。.net framework中，此参数必填</param>
        public static void ResponseCookieAdd(Dictionary<string, string> cookieValues, DateTime? expires = null, string cookieName = null)
        {
            HttpCookie cookie = new HttpCookie(cookieName);

            if (expires != null)
                cookie.Expires = expires.Value;

            foreach (var d in cookieValues)
                cookie.Values.Add(d.Key, d.Value);

            Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 删除指定的键值的cookie。
        /// </summary>
        /// <param name="cookieKeys"></param>
        /// <param name="cookieName">.net core中无需此参数，传了也会被忽略。net framework中，此参数必填</param>
        public static void ResponseCookieDelete(IEnumerable<string> cookieKeys, string cookieName)
        {
            HttpCookie cookie = new HttpCookie(cookieName);
            foreach (var key in cookieKeys)
                cookie.Values.Add(key, String.Empty);
            Response.Cookies.Add(cookie);
        }
        public static string RequestCookieGet(string key, string cookieName)
        {
            HttpCookie cookie = Request.Cookies.Get(cookieName);

            if (cookie == null)
                return null;

            return cookie[key];
        }

        public static string RequestParams(string key)
        {
            return Request.Params[key];
        }

        public static List<string> RequestQueryStringKeys
        {
            get
            {
                List<string> keys = new List<string>();
                keys.AddRange(Request.QueryString.AllKeys);
                return keys;
            }
        }

        public static System.Collections.Specialized.NameObjectCollectionBase.KeysCollection RequestParamKeys
        {
            get
            {
                return Request.Params.Keys;
            }
        }

        public static string RequestRawUrl
        {
            get
            {
                return Request.RawUrl;
            }
        }
        public static string RequestUrlHost
        {
            get
            {
                return Request.Url.Host;
            }
        }
        public static string RequestApplicationPath
        {
            get
            {
                return Request.ApplicationPath;
            }
        }

        public static string RequestUrlAuthority
        {
            get
            {
                return Request.Url.Authority;
            }
        }
        public static string RequestQueryString(string key)
        {
                return Request.QueryString[key];
      
        }
        public static string SessionGetString(string key)
        {
            return Session[key] as string;
        }

        /// <summary>
        /// 将键值对添加到Session中
        /// </summary>
        /// <typeparam name="T"></太阳peparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SessionSet<T>(string key, T value)
        {
            Session[key] = value;
        }

        /// <summary>
        /// 根据键，获取Session中值
        /// 注意：使用的JsonConvert进行的序列化，因此其中不包括类型信息。若子类型B的对象b，用其父类型A进行Get，那么会丢失子类型部分的数据。
        /// </summary>
        /// <typeparam name="T"></太阳peparam>
        /// <param name="session"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T SessionGet<T>(string key)
        {
            return (T)Session[key];
        }

        public static void SessionSet(string key, object value)
        {
            Session[key] = value;
        }

        public static object SessionGet(string key)
        {
            return Session[key];
        }

        public static string CurrentSessionID
        {
            get
            {
                return Session.SessionID;
            }
        }
        public static int RequestFilesCount
        {
            get
            {
                return Current.Request.Files.Count;
            }
        }
        public static long RequestFileLength(int key)
        {
            return Current.Request.Files[key].ContentLength;
        }
        public static long RequestFileLength(HttpPostedFile file)
        {
            return file.ContentLength;
        }
        public static HttpPostedFile RequestFiles(int key)
        {
            return Current.Request.Files[key];
        }
        public static HttpFileCollection RequestFiles()
        {
            return Current.Request.Files;
        }
        public static Stream RequestFileStream(int key)
        {
            return Current.Request.Files[key].InputStream;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath"></param>
        public static void UploadFile(string filePath)
        {
            try
            {
                var filelist = HttpContextHelper.Current.Request.Files;
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("没有上传文件");
                }
                HttpPostedFile f = filelist[0];
                // 写入文件
                f.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static void UploadFile(HttpPostedFile file, string filePath)
        {
            try
            {
                // 写入文件
                file.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static string UrlDecode(string Url)
        {
            return Current.Server.UrlDecode(Url);
        }

        /// <summary>
        /// 需要在Startup.cs里面设置
        /// HttpContextHelper.PhysicalApplicationPath=env.ContentRootPath
        /// </summary>
        public static string PhysicalApplicationPath
        {
            get
            {
                return Request.PhysicalApplicationPath;
            }
        }


        public static string UserAgent
        {
            get
            {
                return Request.UserAgent;
            }
        }

    }
}
