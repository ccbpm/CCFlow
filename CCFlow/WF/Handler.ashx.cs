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
using BP.WF.Port;

namespace CCFlow.WF
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 公共的属性，方法变量。
        public HttpContext context = null;
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                string str = context.Request.QueryString["FK_Node"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string str = context.Request.QueryString["FK_Flow"];
                if (str == null || str == "" || str == "null")
                    return str;
                return str;
            }
        }
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

        #region 入口函数.
        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="mycontext"></param>
        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "Runing_Init":
                        msg = this.Runing_Init(BP.Web.WebUser.No, this.FK_Flow);
                        break;
                    case "HungUpList_Init":
                        msg = this.HungUpList_Init(BP.Web.WebUser.No, this.FK_Flow);
                        break;
                    case "Draft_Init":
                        msg = this.Draft_Init(BP.Web.WebUser.No, this.FK_Flow);
                        break;
                    case "Todolist_Init":
                        msg = this.Todolist_Init(WebUser.No,this.FK_Node);
                        break;
                    case "LoginInit":  //处理login的初始化工作.
                        msg= this.LoginInit();
                        break;
                    case "LoginSubmit": //处理login的初始化工作.
                        msg = this.LoginSubmit();
                        break;
                    case "LoginExit": //退出安全登录.
                        BP.WF.Dev2Interface.Port_SigOut();
                        break;
                    case "IsAuthorize"://是否有授权
                        msg=this.IsHaveAuthor();
                        break;
                    case "Load_Author"://获取授权人
                        msg = LoadAuthor();
                        break;
                    case "Todolist_Author"://获取授权人待办
                        msg = this.Todolist_Init(this.No, this.FK_Node);
                        break;
                    case "LoginAs"://授权登录
                        msg = this.LoginAs();
                        break;
                    case "AuthExit":
                        msg = this.AuthExitAndLogin(this.No,BP.Web.WebUser.Auth);
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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        #endregion 入口函数.

        #region 获得列表. 
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init(string UserNo, string fk_flow)
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerRuning();
            return BP.Tools.FormatToJson.ToJson(dt);
        }
        /// <summary>
        /// 挂起列表
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>挂起列表</returns>
        public string HungUpList_Init(string userNo, string fk_flow)
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <param name="UserNo">用户编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns></returns>
        public string Draft_Init(string UserNo, string fk_flow)
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();

            //转化大写.
            return BP.Tools.Json.ToJsonUpper(dt);
        }
        /// <summary>
        /// 获得待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init(string userNo, int FK_Node)
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(userNo, FK_Node);

            //转化大写的toJson.
            return BP.Tools.Json.ToJsonUpper(dt);
        }
        #endregion 获得列表.

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
            return BP.Tools.FormatToJson.ToJson(ht);
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
           string sid= BP.WF.Dev2Interface.Port_Login(emp.No);
           return sid;
        }
        /// <summary>
        /// 执行授权登录
        /// </summary>
        /// <returns></returns>
        public string LoginAs()
        {
            BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.No);
            if (wfemp.AuthorIsOK == false)
                return "err@授权登录失败！";
            BP.Port.Emp emp1 = new BP.Port.Emp(this.No);
            BP.Web.WebUser.SignInOfGener(emp1,"CH",false,false,BP.Web.WebUser.No,BP.Web.WebUser.Name);
            return "success@授权登录成功！";
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="Author"></param>
        /// <returns></returns>
        public string AuthExitAndLogin(string UserNo,string Author)
        {
            string msg = "suess@退出成功！";
            try
            {
                BP.Port.Emp emp = new BP.Port.Emp(UserNo);
                //首先退出
                BP.Web.WebUser.Exit();
                //再进行登录
                BP.Port.Emp emp1 = new BP.Port.Emp(Author);
                BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, null, null);
            }
            catch (Exception ex)
            {
                msg = "err@退出时发生错误。"+ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 获取授权人列表
        /// </summary>
        /// <returns></returns>
        public string LoadAuthor()
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + BP.Web.WebUser.No + "'");
            return BP.Tools.FormatToJson.ToJson(dt);
        }
        /// <summary>
        /// 当前登陆人是否有授权
        /// </summary>
        /// <returns></returns>
        public string IsHaveAuthor()
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + BP.Web.WebUser.No + "'");
            WFEmp em = new WFEmp();
            em.Retrieve(WFEmpAttr.Author, BP.Web.WebUser.No);

            if (dt.Rows.Count > 0 && BP.Web.WebUser.IsAuthorize == false)
                return "suess@有授权";
            else
                return "err@没有授权";
        }
        #endregion 登录相关.
      
    }
}