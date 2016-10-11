using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.En;
using BP.DA;
using BP.Sys;


namespace CCFlow.WF.Admin.CCBPMDesigner
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
                    case "DefaultInit": //初始化登录界面.
                        msg = this.DefaultInit();
                        break;
                    case "Logout": //获得枚举列表的JSON.
                        BP.WF.Dev2Interface.Port_SigOut();
                        return;
                    case "LoginInit": //登录初始化..
                        if (BP.DA.DBAccess.IsExitsObject("WF_Emp") == false)
                            msg="url@=../DBInstall.aspx";
                        break;
                    case "Login":
                        msg = this.Login();
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
        /// <summary>
        /// 初始化登录界面.
        /// </summary>
        /// <returns></returns>
        public string DefaultInit()
        {
            //让admin登录
            if (string.IsNullOrEmpty(BP.Web.WebUser.No) || BP.Web.WebUser.No != "admin")
                return "url@Login.htm?DoType=Logout";

            // 执行升级
            string str = BP.WF.Glo.UpdataCCFlowVer();

            string osModel = "";
            if (BP.WF.Glo.OSModel == OSModel.OneOne)
                osModel = "0";
            osModel = "1";

            if (str != null)
            {
                if (str == "0")
                    return "{ msg:'系统升级错误，请查看日志文件\\DataUser\\log\\*.*', OSModel:'" + osModel + "'}";
                else
                    return "{ msg:'系统成功升级到:" + str + " ，系统升级不会破坏现有的数据', OSModel:'" + osModel + "'}";
            }

            return "{ msg:'', OSModel:'" + osModel + "'}";
        }

        public string Login()
        {
            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = this.GetValFromFrmByKey("TB_UserNo");

            if (emp.RetrieveFromDBSources()==0)
                return "err@用户名或密码错误.";
            string pass= this.GetValFromFrmByKey("TB_Pass");
            if (emp.Pass.Equals(pass)==false)
                return "err@用户名或密码错误.";
            //让其登录.
            BP.WF.Dev2Interface.Port_Login(emp.No,true);
            return "SID=xxx&UserNo="+emp.No;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}