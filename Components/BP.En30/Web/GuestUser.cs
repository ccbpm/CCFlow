
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Web;
using System.Data;
using BP.En;
using BP.DA;
using System.Configuration;
using BP.Port;
using BP.Pub;
using BP.Sys;


namespace BP.Web
{
    /// <summary>
    /// 客户的用户信息。
    /// </summary>
    public class GuestUser
    {
        /// <summary>
        /// 通用的登陆
        /// </summary>
        /// <param name="guestNo"></param>
        /// <param name="guestName"></param>
        /// <param name="lang"></param>
        /// <param name="isRememberMe"></param>
        public static void SignInOfGener(string guestNo, string guestName, string lang, bool isRememberMe)
        {
            SignInOfGener(guestNo, guestName, "deptNo", "deptName", lang, isRememberMe);
        }
        /// <summary>
        /// 通用的登陆
        /// </summary>
        /// <param name="guestNo">客户编号</param>
        /// <param name="guestName">客户名称</param>
        /// <param name="deptNo">部门编号</param>
        /// <param name="deptName">部门名称</param>
        /// <param name="lang">语言</param>
        /// <param name="isRememberMe">是否记忆我</param>
        public static void SignInOfGener(string guestNo, string guestName, string deptNo, 
            string deptName,string lang, bool isRememberMe)
        {
            if (System.Web.HttpContext.Current == null)
                SystemConfig.IsBSsystem = false;
            else
                SystemConfig.IsBSsystem = true;

            //记录客人信息.
            GuestUser.No = guestNo;
            GuestUser.Name = guestName;
            GuestUser.DeptNo = deptNo;
            GuestUser.DeptName = deptName;


            //记录内部客户信息.
            BP.Port.Emp em = new Emp();
            em.No = "Guest";
            if (em.RetrieveFromDBSources() == 0)
            {
                em.Name = "客人";
                em.Insert();
            }
            WebUser.No = em.No;
            WebUser.Name = em.Name;
            WebUser.FK_Dept = em.FK_Dept;
            WebUser.FK_DeptName = em.FK_DeptText;
            WebUser.SysLang = lang;
            if (BP.Sys.SystemConfig.IsBSsystem)
            {
                // Guest  信息.
                HttpCookie cookie = new HttpCookie("CCSGuest");
                //cookie.Expires = DateTime.Now.AddMonths(10);
                cookie.Expires = DateTime.Now.AddDays(2);
                cookie.Values.Add("GuestNo", guestNo);
                cookie.Values.Add("GuestName", HttpUtility.UrlEncode(guestName));
                cookie.Values.Add("DeptNo", deptNo);
                cookie.Values.Add("DeptName", HttpUtility.UrlEncode(deptName));
                System.Web.HttpContext.Current.Response.AppendCookie(cookie); //加入到会话。


                HttpCookie cookie2 = new HttpCookie("CCS");
                cookie2.Expires = DateTime.Now.AddDays(2);
                // Guest  信息.
                cookie2.Values.Add("GuestNo", guestNo);
                cookie2.Values.Add("GuestName", HttpUtility.UrlEncode(guestName));

                cookie2.Values.Add("DeptNo", deptNo);
                cookie2.Values.Add("DeptName", HttpUtility.UrlEncode(deptName));

                cookie2.Values.Add("No", "Guest");
                cookie2.Values.Add("Name", HttpUtility.UrlEncode(em.Name));

                if (isRememberMe)
                    cookie2.Values.Add("IsRememberMe", "1");
                else
                    cookie2.Values.Add("IsRememberMe", "0");

                cookie2.Values.Add("FK_Dept", em.FK_Dept);
                cookie2.Values.Add("FK_DeptName", HttpUtility.UrlEncode(em.FK_DeptText));

                cookie2.Values.Add("Token", System.Web.HttpContext.Current.Session.SessionID);
                cookie2.Values.Add("SID", System.Web.HttpContext.Current.Session.SessionID);

                cookie2.Values.Add("Lang", lang);
                cookie2.Values.Add("Style", "0");
                cookie2.Values.Add("Auth", ""); //授权人.
                System.Web.HttpContext.Current.Response.AppendCookie(cookie2);
            }
        }
      
        #region 静态方法
        /// <summary>
        /// 通过key,取出session.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="isNullAsVal">如果是Null, 返回的值.</param>
        /// <returns></returns>
        public static string GetSessionByKey(string key, string isNullAsVal)
        {
            if (IsBSMode)
            {
                string str = System.Web.HttpContext.Current.Session[key] as string;
                if (DataType.IsNullOrEmpty(str))
                    str = isNullAsVal;
                return str;
            }
            else
            {
                if (BP.Port.Current.Session[key] == null || BP.Port.Current.Session[key].ToString() == "")
                {
                    BP.Port.Current.Session[key] = isNullAsVal;
                    return isNullAsVal;
                }
                else
                    return (string)BP.Port.Current.Session[key];
            }
        }
        public static object GetObjByKey(string key)
        {
            if (IsBSMode)
            {
                return System.Web.HttpContext.Current.Session[key];
            }
            else
            {
                return BP.Port.Current.Session[key];
            }
        }
        #endregion

        /// <summary>
        /// 是不是b/s 工作模式。
        /// </summary>
        protected static bool IsBSMode
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return false;
                else
                    return true;
            }
        }
        public static object GetSessionByKey(string key, Object defaultObjVal)
        {
            if (IsBSMode)
            {
                if (System.Web.HttpContext.Current.Session[key] == null)
                    return defaultObjVal;
                else
                    return System.Web.HttpContext.Current.Session[key];
            }
            else
            {
                if (BP.Port.Current.Session[key] == null)
                    return defaultObjVal;
                else
                    return BP.Port.Current.Session[key];
            }
        }
        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public static void SetSessionByKey(string key, object val)
        {
            if (val == null)
                return;
            if (IsBSMode)
                System.Web.HttpContext.Current.Session[key] = val;
            else
                BP.Port.Current.SetSession(key, val);
        }
        /// <summary>
        /// 退回
        /// </summary>
        public static void Exit()
        {
            if (IsBSMode == false)
            {
                try
                {
                    string token = WebUser.Token;
                    System.Web.HttpContext.Current.Response.Cookies.Clear();
                    BP.Sys.Glo.Request.Cookies.Clear();


                    HttpCookie cookie = new HttpCookie("CCS", string.Empty);
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Values.Add("No", string.Empty);
                    cookie.Values.Add("Name", string.Empty);
                    // 2013.06.07 H
                    cookie.Values.Add("Pass", string.Empty); 
                    cookie.Values.Add("IsRememberMe", "0");
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    WebUser.Token = token;
                    BP.Port.Current.Session.Clear();


                    // Guest  信息.
                    cookie = new HttpCookie("CCSGuest");
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Values.Add("GuestNo", string.Empty);
                    cookie.Values.Add("GuestName", string.Empty);
                    cookie.Values.Add("DeptNo", string.Empty);
                    cookie.Values.Add("DeptName", string.Empty);
                    System.Web.HttpContext.Current.Response.AppendCookie(cookie); //加入到会话。

                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    string token = WebUser.Token;
                    System.Web.HttpContext.Current.Response.Cookies.Clear();
                    BP.Sys.Glo.Request.Cookies.Clear();
                 
                    System.Web.HttpContext.Current.Session.Clear();

                    HttpCookie cookie = new HttpCookie("CCS", string.Empty);
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Values.Add("No", string.Empty);
                    cookie.Values.Add("Name", string.Empty);
                    // 2013.06.07 H
                    cookie.Values.Add("Pass",string.Empty);
                    cookie.Values.Add("IsRememberMe", "0");
                    System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                    WebUser.Token = token;

                    // Guest  信息.
                    cookie = new HttpCookie("CCSGuest");
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Values.Add("GuestNo", string.Empty);
                    cookie.Values.Add("GuestName", string.Empty);
                    cookie.Values.Add("DeptNo", string.Empty);
                    cookie.Values.Add("DeptName", string.Empty);
                    System.Web.HttpContext.Current.Response.AppendCookie(cookie); //加入到会话。

                }
                catch
                {
                }
            }
        }
        public static string GetValFromCookie(string valKey, string isNullAsVal, bool isChinese)
        {
            if (IsBSMode == false)
                return BP.Port.Current.GetSessionStr(valKey, isNullAsVal);
            string key = "CCSGuest";
            HttpCookie hc = BP.Sys.Glo.Request.Cookies[key];
            if (hc == null)
                return null;
            try
            {
                string val = null;
                if (isChinese)
                    val = HttpUtility.UrlDecode(hc[valKey]);
                else
                    val = hc.Values[valKey];

                if (DataType.IsNullOrEmpty(val))
                    return isNullAsVal;
                return val;
            }
            catch
            {
                return isNullAsVal;
            }
            throw new Exception("@err-001 登陆信息丢失。");
        }
        /// <summary>
        /// 编号
        /// </summary>
        public static string No
        {
            get
            {
                return GetValFromCookie("GuestNo", null, false);
                string no = null; // GetSessionByKey("No", null);
                if (no == null || no == "")
                {
                    if (IsBSMode == false)
                        return "admin";

                    string key = "CCSGuest";
                    HttpCookie hc = BP.Sys.Glo.Request.Cookies[key];
                    if (hc == null)
                        return null;

                    if (hc.Values["GuestNo"] != null)
                    {
                       GuestUser.No = hc["GuestNo"];
                       GuestUser.Name = hc["GuestName"];
                       return hc.Values["GuestNo"];
                    }
                    throw new Exception("@err-002 Guest 登陆信息丢失。");
                }
                return no;
            }
            set
            {
                SetSessionByKey("GuestNo", value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public static string Name
        {
            get
            {
                string val = GetValFromCookie("GuestName", null, true);
                if (val == null)
                    throw new Exception("@err-001 GuestName 登陆信息丢失。");
                return val;
            }
            set
            {
                SetSessionByKey("GuestName", value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public static string DeptNo
        {
            get
            {
                string val = GetValFromCookie("DeptNo", null, true);
                if (val == null)
                    throw new Exception("@err-003 DeptNo 登陆信息丢失。");
                return val;
            }
            set
            {
                SetSessionByKey("DeptNo", value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public static string DeptName
        {
            get
            {
                string val = GetValFromCookie("DeptName", null, true);
                if (val == null)
                    throw new Exception("@err-002 DeptName 登陆信息丢失。");
                return val;
            }
            set
            {
                SetSessionByKey("DeptName", value);
            }
        }
        /// <summary>
        /// 风格
        /// </summary>
        public static string Style
        {
            get
            {
                return GetSessionByKey("Style", "0");
            }
            set
            {
                SetSessionByKey("Style", value);
            }
        }
    }
}
