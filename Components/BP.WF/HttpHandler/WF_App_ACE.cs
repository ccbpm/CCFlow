using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_App_ACE : WebContralBase
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_App_ACE(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 获得发起流程
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(WebUser.No);
            return BP.Tools.Json.ToJsonUpper(dt);
        }
        /// <summary>
        /// 获得待办
        /// </summary>
        /// <returns></returns>
        public string Todolist_Init()
        {
            string fk_node = this.GetRequestVal("FK_Node");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, this.FK_Node);
            return BP.Tools.Json.ToJsonUpper(dt);
        }
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
            return BP.Tools.Json.ToJsonUpper(dt);
        }

        /// <summary>
        /// 初始化赋值.
        /// </summary>
        /// <returns></returns>
        public string Home_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("UserNo", BP.Web.WebUser.No);
            ht.Add("UserName", BP.Web.WebUser.Name);

            //系统名称.
            ht.Add("SysName", BP.Sys.SystemConfig.SysName);


            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks);
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing);
            ht.Add("Todolist_Sharing", BP.WF.Dev2Interface.Todolist_Sharing);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 转换成菜单.
        /// </summary>
        /// <returns></returns>
        public string Home_Menu()
        {
            DataSet ds = new DataSet();

            BP.WF.XML.ClassicMenus menus = new XML.ClassicMenus();
            menus.RetrieveAll();

           DataTable dtMain=  menus.ToDataTable();
           dtMain.TableName = "ClassicMenus";
           ds.Tables.Add(dtMain);

           BP.WF.XML.ClassicMenuAdvFuncs advMenms = new XML.ClassicMenuAdvFuncs();
           advMenms.RetrieveAll();

           DataTable dtMenuAdv = advMenms.ToDataTable();
           dtMenuAdv.TableName = "ClassicMenusAdv";
           ds.Tables.Add(dtMenuAdv);

           return BP.Tools.Json.ToJson(ds);
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        { 
            switch (this.DoType)
            {

                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记["+this.DoType+"]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        #region 控制台.
        /// <summary>
        /// 控制台信息.
        /// </summary>
        /// <returns></returns>
        public string Index_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("Todolist_Runing", BP.WF.Dev2Interface.Todolist_Runing); //运行中.
            ht.Add("Todolist_EmpWorks", BP.WF.Dev2Interface.Todolist_EmpWorks); //待办
            ht.Add("Todolist_CCWorks", BP.WF.Dev2Interface.Todolist_CCWorks); //抄送.

            //本周.
            ht.Add("TodayNum", BP.WF.Dev2Interface.Todolist_CCWorks); //抄送.

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        #endregion 控制台.

        /// <summary>
        /// 设置
        /// </summary>
        /// <returns></returns>
        public string Setting_Init()
        {
            return "";
        }


        #region 登录界面.
        /// <summary>
        /// 登录.
        /// </summary>
        /// <returns></returns>
        public string Login_Submit()
        {
            string userNo = this.GetRequestVal("TB_UserNo");
            string pass = this.GetRequestVal("TB_Pass");

            BP.Port.Emp emp = new Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() ==0)
                return "err@用户名或者密码错误.";

            if (emp.Pass !=pass )
                return "err@用户名或者密码错误.";

            //调用登录方法.
            BP.WF.Dev2Interface.Port_Login(emp.No, emp.Name, emp.FK_Dept, emp.FK_DeptText);

            return "登录成功.";

        }
        public string Login_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("SysName", SystemConfig.SysName);
            ht.Add("ServiceTel", SystemConfig.ServiceTel);
            ht.Add("UserNo", WebUser.No);
            ht.Add("UserName", WebUser.Name);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        #endregion 登录界面.

        #region 草稿.
        /// <summary>
        /// 草稿.
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(this.FK_Flow);

            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 草稿.
    }
}
