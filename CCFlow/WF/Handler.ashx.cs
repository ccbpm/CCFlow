using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.En;
using BP.WF;
using BP.DA;
using BP.Web;
using BP.Port;

namespace CCFlow.WF
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 执行.
        public HttpContext context = null;
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = context.Request.QueryString["DoType"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
        {
            get
            {
                string str = context.Request.QueryString["MyPK"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 枚举值
        /// </summary>
        public string EnumKey
        {
            get
            {
                string str = context.Request.QueryString["EnumKey"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 实体 EnsName
        /// </summary>
        public string EnsName
        {
            get
            {
                string str = context.Request.QueryString["EnsName"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string SFTable
        {
            get
            {
                string str = context.Request.QueryString["SFTable"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 表单外键
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string str = context.Request.QueryString["FK_MapData"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 获得表单的属性.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValFromFrmByKey(string key)
        {
            string val = context.Request.Form[key];
            if (val == null)
                return null;
            val = val.Replace("'", "~");
            return val;
        }
        public int GetValIntFromFrmByKey(string key)
        {
            return int.Parse(this.GetValFromFrmByKey(key));
        }
        public bool GetValBoolenFromFrmByKey(string key)
        {
            string val = this.GetValFromFrmByKey(key);
            if (val == null || val == "")
                return false;
            return true;
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string GetRequestVal(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
        }
        #endregion 执行.

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "LoginInit":  //处理login的初始化工作.
                        msg= this.LoginInit();
                        break;
                    case "LoginSubmit": //处理login的初始化工作.
                        msg = this.LoginSubmit();
                        break;
                    case "LoginExit": //退出安全登录.
                        BP.WF.Dev2Interface.Port_SigOut();
                        break;
                    default:
                        msg = "err@没有判断的标记:" + this.DoType;
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
        }

        #region 登录相关.
        /// <summary>
        /// 返回当前会话信息.
        /// </summary>
        /// <returns></returns>
        public string LoginInit()
        {
            Hashtable ht = new Hashtable();

            if (BP.Web.WebUser.No == null)
                ht.Add("UserNo", "");
            else
                ht.Add("UserNo", BP.Web.WebUser.No);

            if (BP.Web.WebUser.IsAuthorize)
                ht.Add("Auth", BP.Web.WebUser.Auth);
            else
                ht.Add("Auth", "");

            return BP.Tools.Json.ToJson(ht, false);
        }
        /// <summary>
        /// 执行登录.
        /// </summary>
        /// <returns></returns>
        public string LoginSubmit()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetValFromFrmByKey("TB_UserNo");

            if (emp.RetrieveFromDBSources() == 0)
                return "err@用户名或密码错误.";
            string pass = this.GetValFromFrmByKey("TB_Pass");
            if (emp.Pass.Equals(pass) == false)
                return "err@用户名或密码错误.";
            //让其登录.
           string sid= BP.WF.Dev2Interface.Port_Login(emp.No, true);
           return sid;
        }
        #endregion 登录相关.


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}