using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.Difference;
using BP.WF.HttpHandler;

namespace BP.GPM.Menu2020
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class PortPage : DirectoryPageBase
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public PortPage()
        {
        }
        #endregion 构造函数

        #region 组织结构维护.
        /// <summary>
        /// 获取该部门的所有人员
        /// </summary>
        /// <returns></returns>        
        public string LoadDatagridDeptEmp_Init()
        {
            string deptNo = this.GetRequestVal("deptNo");
            if (string.IsNullOrEmpty(deptNo))
            {
                return "{ total: 0, rows: [] }";
            }
            string orderBy = this.GetRequestVal("orderBy");


            string searchText = this.GetRequestVal("searchText");
            if (!DataType.IsNullOrEmpty(searchText))
            {
                searchText.Trim();
            }
            string addQue = "";
            if (!string.IsNullOrEmpty(searchText))
            {
                addQue = "  AND (pe.No like '%" + searchText + "%' or pe.Name like '%" + searchText + "%') ";
            }

            string pageNumber = this.GetRequestVal("pageNumber");
            int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
            //每页多少行
            string pageSize = this.GetRequestVal("pageSize");
            int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);

            string sql = "(select pe.*,pd.name FK_DutyText from port_emp pe left join port_duty pd on pd.no = pe.fk_duty where pe.no in (select fk_emp from Port_DeptEmp where fk_dept='" + deptNo + "') "
                + addQue + " ) dbSo ";


            return DBPaging(sql, iPageNumber, iPageSize, "No", orderBy);

        }

        /// <summary>
        /// 以下算法只包含 oracle mysql sqlserver 三种类型的数据库 qin
        /// </summary>
        /// <param name="dataSource">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">当前页数据条数</param>
        /// <param name="key">计算总行数需要</param>
        /// <param name="orderKey">排序字段</param>
        /// <returns></returns>
        public string DBPaging(string dataSource, int pageNumber, int pageSize, string key, string orderKey)
        {
            string sql = "";
            string orderByStr = "";

            if (!string.IsNullOrEmpty(orderKey))
                orderByStr = " ORDER BY " + orderKey;

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    int beginIndex = (pageNumber - 1) * pageSize + 1;
                    int endIndex = pageNumber * pageSize;

                    sql = "SELECT * FROM ( SELECT A.*, ROWNUM RN " +
                        "FROM (SELECT * FROM  " + dataSource + orderByStr + ") A WHERE ROWNUM <= " + endIndex + " ) WHERE RN >=" + beginIndex;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT TOP " + pageSize + " * FROM " + dataSource + " WHERE " + key + " NOT IN  ("
                    + "SELECT TOP (" + pageSize + "*(" + pageNumber + "-1)) " + key + " FROM " + dataSource + " )" + orderByStr;
                    break;
                case DBType.MySQL:
                    pageNumber -= 1;
                    sql = "select * from  " + dataSource + orderByStr + " limit " + pageNumber + "," + pageSize;
                    break;
                default:
                    throw new Exception("暂不支持您的数据库类型.");
            }

            DataTable DTable = DBAccess.RunSQLReturnTable(sql);

            int totalCount = DBAccess.RunSQLReturnCOUNT("select " + key + " from " + dataSource);

            return DataTableConvertJson.DataTable2Json(DTable, totalCount);
        }
        #endregion

        #region 获取菜单权限.
        /// <summary>
        /// 获得菜单数据.
        /// </summary>
        /// <returns></returns>
        public string GPM_DB_Menus()
        {
            var appNo = this.GetRequestVal("AppNo");

            var sql1 = "SELECT No,Name,FK_Menu,ParentNo,UrlExt,Icon,Idx ";
            sql1 += " FROM V_GPM_EmpMenu ";
            sql1 += " WHERE FK_Emp = '" + WebUser.No + "' ";
            sql1 += " AND MenuType = '3' ";
            sql1 += " AND FK_App = '" + appNo + "' ";
            sql1 += " UNION ";  //加入不需要权限控制的菜单.
            sql1 += "SELECT No,Name, No as FK_Menu,ParentNo,UrlExt,Icon,Idx";
            sql1 += " FROM GPM_Menu ";
            sql1 += " WHERE MenuCtrlWay=1 ";
            sql1 += " AND MenuType = '3' ";
            sql1 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";
            var dirs = DBAccess.RunSQLReturnTable(sql1);
            dirs.TableName = "Dirs"; //获得目录.

            var sql2 = "SELECT No,Name,FK_Menu,ParentNo,UrlExt,Icon,Idx ";
            sql2 += " FROM V_GPM_EmpMenu ";
            sql2 += " WHERE FK_Emp = '" + WebUser.No + "'";
            sql2 += " AND MenuType = '4' ";
            sql2 += " AND FK_App = '" + appNo + "' ";
            sql2 += " UNION ";  //加入不需要权限控制的菜单.
            sql2 += "SELECT No,Name, No as FK_Menu,ParentNo,UrlExt,Icon,Idx ";
            sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
            sql2 += " WHERE MenuCtrlWay=1 ";
            sql2 += " AND MenuType = '4' ";
            sql2 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";

            var menus = DBAccess.RunSQLReturnTable(sql2);
            menus.TableName = "Menus"; //获得菜单.

            //组装数据.
            DataSet ds = new DataSet();
            ds.Tables.Add(dirs);
            ds.Tables.Add(menus);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 获得OA菜单数据.
        /// </summary>
        /// <returns></returns>
        public string GPM_OA_Menus()
        {
            var appNo = this.GetRequestVal("AppNo");

            Paras ps = new Paras();
            string dbstr = SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT No FROM GPM_Menu WHERE MenuType=" + dbstr + "MenuType AND FK_App=" + dbstr + "FK_App";
            ps.Add("MenuType", 2);
            ps.Add("FK_App", appNo);

            string ParentNo = DBAccess.RunSQLReturnString(ps);

            if (string.IsNullOrWhiteSpace(ParentNo))
                return "[]";

            var sql1 = "SELECT No,Name,FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
            sql1 += " FROM v_gpm_empmenu ";
            sql1 += " WHERE FK_Emp = '" + WebUser.No + "' ";
            sql1 += " AND ParentNo = '" + ParentNo + "' ";
            sql1 += " AND FK_App = '" + appNo + "' ";
            sql1 += " UNION ";  //加入不需要权限控制的菜单.
            sql1 += "SELECT No,Name, No as FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx";
            sql1 += " FROM GPM_Menu ";
            sql1 += " WHERE MenuCtrlWay=1 ";
            sql1 += " AND ParentNo = '" + ParentNo + "' ";
            sql1 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";
            var dirs = DBAccess.RunSQLReturnTable(sql1);
            dirs.TableName = "Dirs"; //获得目录.

            var sql2 = "SELECT No,Name,FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx,openway ";
            sql2 += " FROM v_gpm_empmenu ";
            sql2 += " WHERE FK_Emp = '" + WebUser.No + "'";
            sql2 += " AND ParentNo != '" + ParentNo + "'  ";
            sql2 += " AND FK_App = '" + appNo + "' ";
            sql2 += " UNION ";  //加入不需要权限控制的菜单.
            sql2 += "SELECT No,Name, No as FK_Menu,MenuType,ParentNo,Url,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx,openway ";
            sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
            sql2 += " WHERE MenuCtrlWay=1 ";
            sql2 += " AND ParentNo != '" + ParentNo + "' ";
            sql2 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";

            var menus = DBAccess.RunSQLReturnTable(sql2);
            menus.TableName = "Menus"; //获得菜单.

            //组装数据.
            DataSet ds = new DataSet();
            ds.Tables.Add(dirs);
            ds.Tables.Add(menus);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 是否可以执行当前工作
        /// </summary>
        /// <returns></returns>
        public string GPM_IsCanExecuteFunction()
        {
            var dt = GPM_GenerFlagDB(); //获得所有的标记.
            var funcNo = this.GetRequestVal("FuncFlag");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString().Equals(funcNo) == true)
                    return "1";
            }
            return "0";
        }
        /// <summary>
        /// 获得所有的权限标记.
        /// </summary>
        /// <returns></returns>
        public DataTable GPM_GenerFlagDB()
        {
            var appNo = this.GetRequestVal("AppNo");
            var sql2 = "SELECT Flag,Idx";
            sql2 += " FROM V_GPM_EmpMenu ";
            sql2 += " WHERE FK_Emp = '" + WebUser.No + "'";
            sql2 += " AND MenuType = '5' ";
            sql2 += " AND FK_App = '" + appNo + "' ";
            sql2 += " UNION ";  //加入不需要权限控制的菜单.
            sql2 += "SELECT Flag,Idx ";
            sql2 += " FROM GPM_Menu "; //加入不需要权限控制的菜单.
            sql2 += " WHERE MenuCtrlWay=1 ";
            sql2 += " AND MenuType = '5' ";
            sql2 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql2);
            return dt;
        }
        /// <summary>
        /// 获得所有权限的标记
        /// </summary>
        /// <returns></returns>
        public string GPM_AutoHidShowPageElement()
        {
            var dt = GPM_GenerFlagDB(); //获得所有的标记.
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 组织结构查询
        /// </summary>
        /// <returns></returns>
        public string GPM_Search()
        {
            var searchKey = this.GetRequestVal("searchKey");
            //var SearchDept = this.GetRequestVal("SearchDept");
            //var SearchEmp = this.GetRequestVal("SearchEmp");
            //var SearchTel = this.GetRequestVal("SearchTel");
            var sql = "SELECT e.No AS No,e.Name AS Name,d.Name AS deptName,e.Email AS Email,e.Tel AS Tel from Port_Dept d,Port_Emp e " +
                "where d.No=e.FK_Dept AND (e.No LIKE '%" + searchKey + "%' or e.NAME LIKE '%" + searchKey + "%' or d.Name LIKE '%" + searchKey + "%' or e.Tel LIKE '%" + searchKey + "%')";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion

    }
}
