using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Data;
using BP.GPM;
using BP.Sys;

namespace BP.Web.GPM
{
    /// <summary>
    /// Dev2Interface 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class Dev2Interface : System.Web.Services.WebService
    {
        /// <summary>
        /// 检查是否连接到GPM.
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public bool CheckIsConn()
        {
            string sql = "SELECT * FROM Port_Emp WHERE 1=2";
            DataTable dt = DA.DBAccess.RunSQLReturnTable(sql);
            return true;
        }
        
        #region 菜单的API.
        /// <summary>
        /// 按datatable的方式,返回用户菜单
        /// 根据这个菜单结构，形成自己的菜单树。
        /// </summary>
        /// <param name="AppNo">系统编号</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>菜单:它的列与GPM数据库中的表GPM_EmpMenu一致.</returns>
        [WebMethod]
        public DataTable GetUserMenuOfDatatable(string AppNo, string userNo)
        {
            DA.Paras ps = new DA.Paras();
            ps.SQL = "SELECT * FROM V_GPM_EmpMenu_GPM WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr
                + "FK_Emp AND FK_App=" + SystemConfig.AppCenterDBVarStr + "FK_App ORDER BY Idx";
            ps.Add("FK_Emp", userNo);
            ps.Add("FK_App", AppNo);
            return DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 按datatable的方式,返回用户菜单
        /// 根据这个菜单结构，形成自己的菜单树。
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns>菜单:它的列与GPM数据库中的表GPM_EmpMenu一致.</returns>
        [WebMethod]
        public DataTable GetUserMenuOfDatatableByPNo(string userNo, string parentMenuNO)
        {
            DA.Paras ps = new DA.Paras();
            ps.SQL = "SELECT * FROM V_GPM_EmpMenu_GPM WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND ParentNo=" + SystemConfig.AppCenterDBVarStr
                + "ParentNo ORDER BY Idx";
            ps.Add("FK_Emp", userNo);
            ps.Add("ParentNo", parentMenuNO);
            return DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 根据用户编号、菜单父编号和是否显示功能点获取菜单项
        /// </summary>
        /// <param name="userNo">用户账号</param>
        /// <param name="ParentNo">菜单编号</param>
        /// <param name="isVisibleFunPoint">是否显示功能点；true 显示，false 不显示</param>
        /// <returns></returns>
        [WebMethod]
        public static DataTable GetMenu_ChildNode_Datatable_ByMenuNo(string userNo, string ParentNo, bool isVisibleFunPoint)
        {
            DA.Paras ps = new DA.Paras();
            ps.SQL = "SELECT * FROM V_GPM_EmpMenu_GPM WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND ParentNo=" + SystemConfig.AppCenterDBVarStr
                + "ParentNo  ORDER BY Idx";
            //不显示功能点
            if (!isVisibleFunPoint)
            {
                ps.SQL = "SELECT * FROM V_GPM_EmpMenu_GPM WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND ParentNo=" + SystemConfig.AppCenterDBVarStr
                               + "ParentNo AND MenuType <> 5 ORDER BY Idx";
            }
            ps.Add("FK_Emp", userNo);
            ps.Add("ParentNo", ParentNo);
            return DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 检查是否可以使用该权限
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="funcNo">功能编号</param>
        /// <returns>返回是否可以使用该权限</returns>
        [WebMethod]
        public bool IsCanUseFunc(string userNo, string FK_Menu)
        {
            DA.Paras ps = new DA.Paras();
            ps.SQL = "SELECT * FROM V_GPM_EmpMenu_GPM WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND FK_Menu=" + SystemConfig.AppCenterDBVarStr + "FK_Menu ";
            ps.Add("FK_Emp", userNo);
            ps.Add("FK_Menu", FK_Menu);
            DataTable dt = DA.DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
                return false;
            return true;
        }
        #endregion 菜单的API.
    }
}
