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
using BP.NetPlatformImpl;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class GPMPage : DirectoryPageBase
    {
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public GPMPage()
        {
        }
        #endregion 构造函数

        #region 签名.
        /// <summary>
        /// 图片签名初始化
        /// </summary>
        /// <returns></returns>
        public string Siganture_Init()
        {
            if (BP.Web.WebUser.NoOfRel == null)
                return "err@登录信息丢失";
            Hashtable ht = new Hashtable();
            ht.Add("No", BP.Web.WebUser.No);
            ht.Add("Name",BP.Web.WebUser.Name);
            ht.Add("FK_Dept", BP.Web.WebUser.FK_Dept);
            ht.Add("FK_DeptName", BP.Web.WebUser.FK_DeptName);
            return BP.Tools.Json.ToJson(ht);
        }

        /// <summary>
        /// 签名保存
        /// </summary>
        /// <returns></returns>
        public string Siganture_Save()
        {
            var f = HttpContextHelper.RequestFiles(0);

            //判断文件类型.
            string fileExt = ",bpm,jpg,jpeg,png,gif,";
            string ext = f.FileName.Substring(f.FileName.LastIndexOf('.') + 1).ToLower();
            if (fileExt.IndexOf(ext + ",") == -1)
            {
                return "err@上传的文件必须是以图片格式:" + fileExt + "类型, 现在类型是:" + ext;
            }

            try
            {
                string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg";
                if (System.IO.File.Exists(tempFile) == true)
                    System.IO.File.Delete(tempFile);

                //f.SaveAs(tempFile);
                HttpContextHelper.UploadFile(f, tempFile);

                System.Drawing.Image img = System.Drawing.Image.FromFile(tempFile);
                img.Dispose();
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }

            HttpContextHelper.UploadFile(f, BP.Sys.SystemConfig.PathOfWebApp + "/DataUser/Siganture/" + this.FK_Emp + ".jpg");
            return "上传成功！";
        }
        #endregion
        
        #region 组织结构维护.
        /// <summary>
        /// 初始化组织结构部门表维护.
        /// </summary>
        /// <returns></returns>
        public string Organization_Init()
        {
        
            BP.GPM.Depts depts = new GPM.Depts();
            //QueryObject qo = new QueryObject(depts);
            //qo.addOrderBy(GPM.DeptAttr.Idx);
            //qo.DoQuery();
            if (WebUser.No.Equals("admin") == false)
            {
                QueryObject qo = new QueryObject(depts);
                qo.addOrderBy(GPM.DeptAttr.Idx);
                qo.DoQuery();

                return depts.ToJson();
            }

            depts.RetrieveAll();

            return depts.ToJson();
        }

        /// <summary>
        /// 获取该部门的所有人员
        /// </summary>
        /// <returns></returns>
        /// 
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

            var sql1 = "SELECT No,Name,FK_Menu,ParentNo,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
            sql1 += " FROM V_GPM_EmpMenu ";
            sql1 += " WHERE FK_Emp = '" + WebUser.No+ "' ";
            sql1 += " AND MenuType = '3' ";
            sql1 += " AND FK_App = '" + appNo + "' ";
            sql1 += " UNION ";  //加入不需要权限控制的菜单.
            sql1 += "SELECT No,Name, No as FK_Menu,ParentNo,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx";
            sql1 += " FROM GPM_Menu ";
            sql1 += " WHERE MenuCtrlWay=1 ";
            sql1 += " AND MenuType = '3' ";
            sql1 += " AND FK_App = '" + appNo + "' ORDER BY Idx ";
            var dirs = DBAccess.RunSQLReturnTable(sql1);
            dirs.TableName = "Dirs"; //获得目录.

            var sql2 = "SELECT No,Name,FK_Menu,ParentNo,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
            sql2 += " FROM V_GPM_EmpMenu ";
            sql2 += " WHERE FK_Emp = '" + WebUser.No + "'";
            sql2 += " AND MenuType = '4' ";
            sql2 += " AND FK_App = '" + appNo + "' ";
            sql2 += " UNION ";  //加入不需要权限控制的菜单.
            sql2 += "SELECT No,Name, No as FK_Menu,ParentNo,UrlExt,Tag1,Tag2,Tag3,WebPath,Icon,Idx ";
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
