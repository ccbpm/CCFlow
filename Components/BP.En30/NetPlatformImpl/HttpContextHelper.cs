using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

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
        /// 请求的物理路径
        /// </summary>
        public static string PhysicalApplicationPath
        {
            get
            {
                return Request.PhysicalApplicationPath;
            }
        }


        public static string RequestUserAgent
        {
            get
            {
                return Request.UserAgent;
            }
        }

        // regex from http://detectmobilebrowsers.com/
        private static readonly Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static bool RequestIsFromMobile
        {
            get
            {
                var userAgent = RequestUserAgent;
                if ((b.IsMatch(userAgent) || v.IsMatch(userAgent.Substring(0, 4))))
                {
                    return true;
                }

                return false;
            }
           
        }

    }
}
