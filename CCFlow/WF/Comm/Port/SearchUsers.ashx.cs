using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Data;

using BP.En;
using BP.DA;
using BP.Pub;
using CCFlow.WF.Comm.Port;

namespace CCFlow.Web.Comm.Port
{
    /// <summary>
    /// SearchUsers 的摘要说明
    /// </summary>
    public class SearchUsers : IHttpHandler
    {
        private string CurUserNo
        {
            get { return BP.Web.WebUser.No; }
        }
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            String method = context.Request["method"];
            switch (method)
            {
                case "getdepts":
                    string s_responsetext = string.Empty;
                    DataTable dt_dept = DBAccess.RunSQLReturnTable("select NO,NAME,ParentNo,IDX from port_dept");
                    s_responsetext = GetTreeJsonByTable(dt_dept, "NO", "NAME", "ParentNo", "0","IDX");
                    if (DataType.IsNullOrEmpty(s_responsetext) || s_responsetext == "[]")//如果为空，使用另一种查询
                    {
                        treeResult.Clear();
                        s_responsetext = GetTreeJsonByTable(dt_dept, "NO", "NAME", "ParentNo", "O0", "IDX");
                    }

                    context.Response.Write(s_responsetext);
                    break;
                case "getusers":
                    //0.用户权限
                    //if (!BP.OA.GPM.IsCanUseFun(this.FuncNo)) { BP.OA.GPM.RedirectNoAccess(); return; }
                    //1.获取用户变量
                    String curUserNo = BP.Web.WebUser.No;
                    String deptId = context.Request["DeptId"];
                    bool searchChildDept = Convert.ToBoolean(context.Request["SearchChild"]);
                    String stationId = context.Request["StationId"];
                    String name = Uri.UnescapeDataString(context.Request["KeyWord"]??"");

                    //2.数据验证
                    StringBuilder sbErr = new StringBuilder();

                    //3.数据处理
                    DataTable dt = this.GetSearchedEmps1(deptId, searchChildDept, stationId, name);
                    //4.统计/日志
                    //5.提示处理信息
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataTableLine2Json dj = new DataTableLine2Json(dt);
                        dj.FormatMode = 2;

                        string json = dj.ToJsonStr(null);
                        context.Response.Write(json);
                    }
                    else
                        context.Response.Write("");
                    break;
            }
        }

        private DataTable GetSearchedEmps1(string deptId, bool searchChildDept, string stationId, string name)
        {
            DataTable dt = null;
            if (BP.Sys.SystemConfig.OSModel == BP.Sys.OSModel.OneMore)
            {
                dt = GetSearchedEmps(deptId, searchChildDept, stationId, name);
            }
            else
            {
                dt = GetSearchedEmpOA(deptId, searchChildDept, stationId, name);
            }
            return dt;
        }

        public static DataTable GetSearchedEmps(string deptId, bool searchChildDept, string stationId, string name)
        {
            if (searchChildDept)
                deptId = GetDeptAndChild(deptId);
            else
                deptId = "'" + deptId + "'";

            string filter_dept = deptId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select FK_Emp from Port_DeptEmp where FK_Dept in ({0}))", deptId);
            string filter_station = stationId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select FK_Emp from Port_DeptEmpStation where FK_Station='{0}')", stationId);
            string filter_name = DataType.IsNullOrEmpty(name) ? String.Empty : String.Format(" and Port_Emp.Name+','+Port_Emp.NO like '%{0}%'", name);
            if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
            {
                filter_name = DataType.IsNullOrEmpty(name) ? String.Empty : String.Format(" and CONCAT(Port_Emp.Name,',',Port_Emp.NO) like '%{0}%'", name);
            }
            else if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
            {
                filter_name = DataType.IsNullOrEmpty(name) ? String.Empty : String.Format(" and Port_Emp.Name || ',' || Port_Emp.NO like '%{0}%'", name);
            }
            string sql = String.Format("select Port_Emp.*,Port_Dept.Name as DeptName from Port_Emp,Port_Dept where Port_Emp.FK_Dept = Port_Dept.No {0}{1}{2}", filter_dept, filter_station, filter_name);
            return DBAccess.RunSQLReturnTable(sql);
        }

        public static DataTable GetSearchedEmpOA(string deptId, bool searchChildDept, string stationId, string name)
        {
            if (searchChildDept)
                deptId = GetDeptAndChild(deptId);
            else
                deptId = "'" + deptId + "'";

            string filter_dept = "";

            if (BP.Sys.SystemConfig.OSModel == BP.Sys.OSModel.OneOne)
                filter_dept = deptId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select No from Port_Emp where FK_Dept in ({0}))", deptId);
            else
                filter_dept = deptId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select FK_Emp from Port_DeptEmp where FK_Dept in ({0}))", deptId);


            string filter_station = "";
            if (BP.Sys.SystemConfig.OSModel == BP.Sys.OSModel.OneOne)
                filter_station = stationId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select FK_Emp from  Port_EmpStation  where FK_Station='{0}')", stationId);
            else
                filter_station = stationId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select FK_Emp from Port_DeptEmpStation where FK_Station='{0}')", stationId);

            string filter_name = DataType.IsNullOrEmpty(name) ? String.Empty : String.Format(" and Port_Emp.Name+','+Port_Emp.NO like '%{0}%'", name);
            if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
            {
                filter_name = DataType.IsNullOrEmpty(name) ? String.Empty : String.Format(" and CONCAT(Port_Emp.Name,',',Port_Emp.NO) like '%{0}%'", name);
            }
            else if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
            {
                filter_name = DataType.IsNullOrEmpty(name) ? String.Empty : String.Format(" and Port_Emp.Name || ',' || Port_Emp.NO like '%{0}%'", name);
            }
            string sql = String.Format("select Port_Emp.*,Port_Dept.Name as DeptName from Port_Emp,Port_Dept where Port_Emp.FK_Dept = Port_Dept.No {0}{1}{2}", filter_dept, filter_station, filter_name);
            return DBAccess.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// 获取本部门与子级部门
        /// </summary>
        /// <returns></returns>
        private static string GetDeptAndChild(string deptId)
        {
            string strDepts = "'" + deptId + "'";
            GetChildDept(deptId, ref strDepts);
            return strDepts;
        }

        /// <summary>
        /// 增加子级
        /// </summary>
        /// <param name="parentNo"></param>
        /// <param name="depts"></param>
        private static void GetChildDept(string parentNo, ref string strDepts)
        {
            BP.Port.Depts depts = new BP.Port.Depts(parentNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (BP.Port.Dept item in depts)
                {
                    strDepts += ",'" + item.No + "'";
                    GetChildDept(item.No, ref strDepts);
                }
            }
        }

        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        /// <param name="tabel">数据源</param>
        /// <param name="idCol">ID列</param>
        /// <param name="txtCol">Text列</param>
        /// <param name="rela">关系字段</param>
        /// <param name="pId">父ID</param>
        ///<returns>easyui tree json格式</returns>
        StringBuilder treeResult = new StringBuilder();
        StringBuilder treesb = new StringBuilder();
        public string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId,string order = null)
        {
            string treeJson = string.Empty;
            string treeState = "close";
            treeResult.Append(treesb.ToString());

            treesb.Clear();
            if (treeResult.Length == 0)
            {
                treeState = "open";
            }
            if (tabel.Rows.Count > 0)
            {
                treesb.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }
                DataRow[] rows = null;
                if (order != null)
                {
                    rows = tabel.Select(filer, order);
                }
                else
                {
                    rows = tabel.Select(filer);
                }
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                + "\",\"state\":\"" + treeState + "\"");


                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol]);
                            treeResult.Append(treesb.ToString());
                            treesb.Clear();
                        }
                        treeResult.Append(treesb.ToString());
                        treesb.Clear();
                        treesb.Append("},");
                    }
                    treesb = treesb.Remove(treesb.Length - 1, 1);
                }
                treesb.Append("]");
                treeResult.Append(treesb.ToString());
                treeJson = treeResult.ToString();
                treesb.Clear();
            }
            return treeJson;
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