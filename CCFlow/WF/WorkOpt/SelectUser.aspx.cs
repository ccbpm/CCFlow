using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;

using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;

namespace CCFlow.WF.WorkOpt
{
    public partial class SelectUser : System.Web.UI.Page
    {
        #region  属性
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 传入的值
        /// </summary>
        private string SelUsers
        {
            get
            {
                return Request["CtrlVal"];
            }
        }
        /// <summary>
        /// 绑定已选人员信息
        /// </summary>
        private void LoadSelectedEmployees()
        {
            string selUsers = this.SelUsers;
            if (!String.IsNullOrEmpty(selUsers))
            {
                DataTable dt = DBAccess.RunSQLReturnTable("select * from Port_Emp");
                if (dt != null)
                {
                    for (int i = dt.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = dt.Rows[i];
                        if (!String.Format(",{0},", selUsers).Contains(String.Format(",{0},", dr["No"])))
                        {
                            dt.Rows.RemoveAt(i);
                            dr.Delete();
                        }
                    }
                }
                lbRight.DataTextField = "NAME";
                lbRight.DataValueField = "NO";
                lbRight.DataSource = dt;
                lbRight.DataBind();
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
            {
                LoadSelectedEmployees();
                return;
            }

            method = Request["method"].ToString();
            switch (method)
            {
                case "getdepts":
                    string sql = "";
                    if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                        sql = "select NO,NAME,ParentNo from port_dept ORDER BY Idx";
                    else
                        sql = "select NO,NAME,ParentNo from port_dept  ";

                    DataTable dt_dept = DBAccess.RunSQLReturnTable(sql);
                    s_responsetext = DataTableConvertJson.TransDataTable2TreeJson(dt_dept, "NO", "NAME", "ParentNo", "0");
                    break;
                case "getusers"://查询人员信息
                    s_responsetext = GetSearchedEmps();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }

        /// <summary>
        /// 获取查询人员
        /// </summary>
        private string GetSearchedEmps()
        {
            String curUserNo = BP.Web.WebUser.No;
            String deptId = getUTF8ToString("DeptId");
            bool searchChildDept = Convert.ToBoolean(getUTF8ToString("SearchChild"));
            String name = getUTF8ToString("KeyWord");

            string sql = "";
            if (searchChildDept)
                deptId = GetDeptAndChild(deptId);
            else
                deptId = "'" + deptId + "'";

            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                string filter_dept = deptId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select FK_Emp from Port_DeptEmp where FK_Dept in ({0}))", deptId);
                string filter_name = String.IsNullOrEmpty(name) ? String.Empty : String.Format(" and Port_Emp.Name+','+Port_Emp.NO like '%{0}%'", name);
                if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                {
                    filter_name = String.IsNullOrEmpty(name) ? String.Empty : String.Format(" and CONCAT(Port_Emp.Name,',',Port_Emp.NO) like '%{0}%'", name);
                }
                sql = String.Format("select Port_Emp.*,Port_Dept.Name as DeptName from Port_Emp,Port_Dept where Port_Emp.FK_Dept = Port_Dept.No {0}{1}", filter_dept, filter_name);
            }
            else
            {
                string filter_dept = deptId == "0" ? String.Empty : String.Format(" and Port_Emp.No in (Select No as FK_Emp from Port_Emp where FK_Dept in ({0}))", deptId);
                string filter_name = String.IsNullOrEmpty(name) ? String.Empty : String.Format(" and Port_Emp.Name+','+Port_Emp.NO like '%{0}%'", name);
                if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
                {
                    filter_name = String.IsNullOrEmpty(name) ? String.Empty : String.Format(" and CONCAT(Port_Emp.Name,',',Port_Emp.NO) like '%{0}%'", name);
                }
                sql = String.Format("select Port_Emp.*,Port_Dept.Name as DeptName from Port_Emp,Port_Dept where Port_Emp.FK_Dept = Port_Dept.No {0}{1}", filter_dept, filter_name);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt); 
        }

        /// <summary>
        /// 将datatable中的列名均改为大写样式，为兼容oracle做的此设置，因为oracle查询出的所有列均为大写样式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable ColumnName2UpperCase(DataTable dt)
        {
            foreach(DataColumn col in dt.Columns)
            {
                col.ColumnName = col.ColumnName.ToUpper();
            }

            return dt;
        }

        /// <summary>
        /// 获取本部门与子级部门
        /// </summary>
        /// <returns></returns>
        private string GetDeptAndChild(string deptId)
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
        private void GetChildDept(string parentNo, ref string strDepts)
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
    }
}