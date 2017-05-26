using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.WF;
using BP.WF.Port;

namespace BP.WF.HttpHandler
{
    public class WF : DirectoryPageBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        protected override string DoDefaultMethod()
        {
            return base.DoDefaultMethod();
        }
      

        #region 获得列表.
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="UserNo">人员编号</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>运行中的流程</returns>
        public string Runing_Init()
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
        public string HungUpList_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 草稿
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();

            //转化大写.
            return BP.Tools.Json.DataTableToJson(dt,true);
        }
        /// <summary>
        /// 获得授权人的待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Author()
        {
            DataTable dt = null;
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(this.No, this.FK_Node);

            //转化大写的toJson.
            return BP.Tools.Json.DataTableToJson(dt,true);
        }
        /// <summary>
        /// 获得待办.
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            DataTable dt = null;

            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(BP.Web.WebUser.No, this.FK_Node);

            //转化大写的toJson.
            return BP.Tools.Json.DataTableToJson(dt,true);
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
            string sid = BP.WF.Dev2Interface.Port_Login(emp.No);
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
            BP.Web.WebUser.SignInOfGener(emp1, "CH", false, false, BP.Web.WebUser.No, BP.Web.WebUser.Name);
            return "success@授权登录成功！";
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="UserNo"></param>
        /// <param name="Author"></param>
        /// <returns></returns>
        public string AuthExitAndLogin(string UserNo, string Author)
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
                msg = "err@退出时发生错误。" + ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// 获取授权人列表
        /// </summary>
        /// <returns></returns>
        public string Load_Author()
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
        /// <summary>
        /// 退出.
        /// </summary>
        /// <returns></returns>
        public string LoginExit()
        {
            BP.WF.Dev2Interface.Port_SigOut();
            return null;
        }
        /// <summary>
        /// 授权退出.
        /// </summary>
        /// <returns></returns>
        public string AuthExit()
        {
            return this.AuthExitAndLogin(this.No, BP.Web.WebUser.Auth);
        }
        #endregion 登录相关.
         
    }
}
