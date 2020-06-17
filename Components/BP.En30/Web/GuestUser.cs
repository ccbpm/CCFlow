
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
using System.Collections.Generic;

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
        public static void SignInOfGener(string guestNo, string guestName, string lang="CH", bool isRememberMe=true)
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
            //2019-07-25 zyt改造
            if (HttpContextHelper.Current == null)
                SystemConfig.IsBSsystem = false;
            else
                SystemConfig.IsBSsystem = true;

            //记录客人信息.
            GuestUser.No = guestNo;
            GuestUser.Name = guestName;

            //记录内部客户信息.
            BP.Port.Emp em = new Emp();
            em.No = "Guest";
            if (em.RetrieveFromDBSources() == 0)
            {
                em.Name = "客人";
                em.Insert();
            }
            BP.Web.WebUser.SignInOfGener(em);
            return;
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
                string str = HttpContextHelper.SessionGetString(key);
                if (DataType.IsNullOrEmpty(str))
                    str = isNullAsVal;
                return str;
            }
            else
            {
                if (BP.Pub.Current.Session[key] == null || BP.Pub.Current.Session[key].ToString() == "")
                {
                    BP.Pub.Current.Session[key] = isNullAsVal;
                    return isNullAsVal;
                }
                else
                    return (string)BP.Pub.Current.Session[key];
            }
        }
        /* 2019-7-25 张磊注释，net core中需要知道object的具体类型才行（不能被序列化的对象，无法放入session中）
        public static object GetObjByKey(string key)
        {
            if (IsBSMode)
            {
                return System.Web.HttpContext.Current.Session[key];
            }
            else
            {
                return BP.Pub.Current.Session[key];
            }
        }*/
        #endregion

        /// <summary>
        /// 是不是b/s 工作模式。
        /// </summary>
        protected static bool IsBSMode
        {
            get
            {
                //2019-07-25 zyt改造
                if (HttpContextHelper.Current == null)
                    return false;
                else
                    return true;
            }
        }

        public static object GetSessionByKey(string key, Object defaultObjVal)
        {
            if (IsBSMode)
            {
                object obj = HttpContextHelper.SessionGet(key);
                if (obj == null)
                    return defaultObjVal;
                else
                    return obj;
            }
            else
            {
                if (BP.Pub.Current.Session[key] == null)
                    return defaultObjVal;
                else
                    return BP.Pub.Current.Session[key];
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
                HttpContextHelper.SessionSet(key, val);
            else
                BP.Pub.Current.SetSession(key, val);
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

                    HttpContextHelper.ResponseCookieDelete(new string[] {
                        "GuestNo", "GuestName" },
                    "CCS");

                    BP.Pub.Current.Session.Clear();

                    /* 2019-07-25 张磊 注释掉，CCSGuest 不再使用
                    // Guest  信息.
                    cookie = new HttpCookie("CCSGuest");
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Values.Add("GuestNo", string.Empty);
                    cookie.Values.Add("GuestName", string.Empty);
                    cookie.Values.Add("DeptNo", string.Empty);
                    cookie.Values.Add("DeptName", string.Empty);
                    System.Web.HttpContext.Current.Response.AppendCookie(cookie); //加入到会话。
                    */
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    BP.Pub.Current.Session.Clear();
                    HttpContextHelper.ResponseCookieDelete(new string[] {
                        "GuestNo", "GuestName"},
                       "CCS");

                    HttpContextHelper.SessionClear();
                    /* 2019-07-25 张磊 注释掉，CCSGuest 不再使用 
                    // Guest  信息.
                    cookie = new HttpCookie("CCSGuest");
                    cookie.Expires = DateTime.Now.AddDays(2);
                    cookie.Values.Add("GuestNo", string.Empty);
                    cookie.Values.Add("GuestName", string.Empty);
                    cookie.Values.Add("DeptNo", string.Empty);
                    cookie.Values.Add("DeptName", string.Empty);
                    System.Web.HttpContext.Current.Response.AppendCookie(cookie); //加入到会话。
                    */
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public static string No
        {
            get
            {
                return BP.Web.WebUser.GetValFromCookie("GuestNo", null, true);
            }
            set
            {
                BP.Web.WebUser.SetSessionByKey("GuestNo", value.Trim()); //@祝梦娟.
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public static string Name
        {
            get
            {
                string val = BP.Web.WebUser.GetValFromCookie("GuestName", null, true);
                if (val == null)
                    throw new Exception("@err-001 GuestName 登录信息丢失。");
                return val;
            }
            set
            {
                BP.Web.WebUser.SetSessionByKey("GuestName", value);
            }
        }
    }
}
