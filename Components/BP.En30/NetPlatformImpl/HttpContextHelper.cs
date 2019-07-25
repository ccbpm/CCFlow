using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Linq;

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


        public static string RequestParams(string key)
        {
            return Request.Params[key];
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

    }
}
