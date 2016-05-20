using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using BP.WF;
using BP.Web;
using BP.WF.Template;

namespace CCFlow.WF.WorkOpt
{
    /// <summary>
    /// ServAccepter 的摘要说明
    /// 修改者：张庆鹏
    /// 修改说明：1.可以分辨当前所属部门、岗位、角色，选择重名的人员
    /// 2.选择人员时，初次不加载人员列表、以及加载后使用分页方式显示，可以避免因绑定人员、部门或者岗位太多造成反应速度慢的问题
    /// 3.修改后，逻辑更加清晰，方便二次开发人员理解
    /// 4.页面更加清晰
    /// 5.与之前版本相比操作更加方便
    /// </summary>
    public class ServAccepter : IHttpHandler
    {
        #region 参数.
        public void OutHtml(string msg)
        {
            //组装ajax字符串格式,返回调用客户端
            MyContext.Response.Charset = "UTF-8";
            MyContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
            MyContext.Response.ContentType = "text/html";
            MyContext.Response.Expires = 0;
            MyContext.Response.Write(msg);
            MyContext.Response.End();
        }
        /// <summary>
        /// 封装有关个别 HTTP 请求的所有 HTTP 特定的信息
        /// </summary>
        HttpContext MyContext = null;
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(MyContext.Request[param], System.Text.Encoding.UTF8);
        }
        //岗位ID
        public string FK_Station
        {
            get
            {
                return getUTF8ToString("stationID");
            }
        }
        //部门ID
        public string FK_Dept
        {
            get
            {
                return getUTF8ToString("deptId");
            }
        }
        //查询的名称
        public string Name
        {
            get
            {
                return getUTF8ToString("name");
            }
        }
        //要到达的节点
        public string ToNode
        {
            get
            {
                return getUTF8ToString("ToNode");
            }
        }
        //工作ID
        public string WorkID
        {
            get
            {
                return getUTF8ToString("WorkID");
            }
        }
        //当前节点ID
        public string FK_Node
        {
            get
            {
                return getUTF8ToString("FK_Node");
            }
        }
        public Selector MySelector = null;
        #endregion 参数.
        public void ProcessRequest(HttpContext context)
        {
            MyContext = context;
            context.Response.Charset = "UTF-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/plain";
            string strResponse = String.Empty;
            var page = Convert.ToInt32(context.Request["page"]);  //当前页码
            var rows = Convert.ToInt32(context.Request["rows"]);   //传当前页
            var strtype = context.Request["type"];
            switch (strtype)
            {
                case "2":
                    //获取部门树
                    strResponse = GetDeptTree();
                    break;
                case "3":
                    //获取岗位树
                    strResponse = GetStationTree();
                    break;
                default:
                    //获取人员列表
                    strResponse = GetEmp(strtype,page,rows);
                    break;

            }
            context.Response.Write(strResponse);
        }
        public string GetEmp(string type,int page,int rows)
        {
            string val = "";
            //第一次加载将不会加载人员列表，此处进行判断，避免抛出null值异常
            if (!String.IsNullOrEmpty(this.ToNode))
            {
                //查询出要到达的节点的绑定规则
                MySelector = new Selector(int.Parse(this.ToNode));
                
                //此处根据节点属性绑定的规则，绑定数据
                switch (MySelector.SelectorModel)
                {
                    //按岗位查询
                    case SelectorModel.Station:
                        //点击岗位列表时
                        if (type == "4")
                            val = GetEmpByStation("Station", this.FK_Station, page, rows);
                        //点击部门列表时
                        if (type == "5")
                            val = GetEmpByDept("Station", this.FK_Dept, page, rows);
                        //点击人员查询时
                        if (type == "6")
                            val = GetEmpByEmp("Station", this.Name, page, rows);
                        break;
                    //按SQL语句查询
                    case SelectorModel.SQL:
                        if (type == "4")
                            val = BindBySQL("Station", this.FK_Station, page, rows);
                        if (type == "5")
                            val = BindBySQL("Dept", this.FK_Dept, page, rows);
                        if (type == "6")
                            val = BindBySQL("Emp", this.Name, page, rows);
                        break;
                    //按部门查询
                    case SelectorModel.Dept:
                        if (type == "4")
                            val = GetEmpByStation("Dept", this.FK_Station, page, rows);
                        if (type == "5")
                            val = GetEmpByDept("Dept", this.FK_Dept, page, rows);
                        if (type == "6")
                            val = GetEmpByEmp("Dept", this.Name, page, rows);
                        break;
                    //按接受人查询
                    case SelectorModel.Emp:
                        if (type == "4")
                            val = GetEmpByStation("Emp", this.FK_Station, page, rows);
                        if (type == "5")
                            val = GetEmpByDept("Emp", this.FK_Dept, page, rows);
                        if (type == "6")
                            val = GetEmpByEmp("Emp", this.Name, page, rows);
                        break;
                    //按URL查询
                    case SelectorModel.Url:
                        if (MySelector.SelectorP1.Contains("?"))
                            this.MyContext.Response.Redirect(MySelector.SelectorP1 + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                        else
                            this.MyContext.Response.Redirect(MySelector.SelectorP1 + "?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                        return "";
                    default:
                        break;
                }
            }
            return val;
        }
        /// <summary>
        /// 获取部门树
        /// </summary>
        /// <returns></returns>
        public string GetDeptTree()
        {
            string sql = "select No,Name,ParentNo,'1' IsParent  from Port_Dept where Name not in('null','',' ')";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            string treeJson = GetTreeJsonByTable(dt, "No", "Name", "ParentNo", "0");
            return treeJson;
        }
        /// <summary>
        /// 获取岗位树
        /// </summary>
        /// <returns></returns>
        public string GetStationTree()
        {
            string sql = "select No,Name,'0' ParentNo from Port_Station";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            string treeJson = GetTreeJsonByTable(dt, "No", "Name", "ParentNo", "0");
            return treeJson;
        }
        #region 根据节点属性绑定规则查询人员列表
        /// <summary>
        /// 按绑定的部门查询
        /// </summary>
        /// <param name="type">查询条件（岗位、部门、人员）</param>
        /// <param name="val">当前岗位、部门的编号</param>
        /// <param name="page">当前页数</param>
        /// <param name="rows">每页显示行数</param>
        /// <returns></returns>
        public string GetEmpByDept(string type, string val, int page, int rows)
        {
            string sql = "";
            string SqlCount = "";
            if (page == 0)
                page = 1;
            //点击岗位时
            if (type == "Station")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where " + BP.WF.Glo.EmpStation + ".FK_Station "
                    + " in(select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Station in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_Emp.FK_Dept='" + val + "' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where " + BP.WF.Glo.EmpStation + ".FK_Station "
                    + " in(select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Station in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_Emp.FK_Dept='" + val + "'  order by Port_Emp.No) order by Port_Emp.No";
                SqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where " + BP.WF.Glo.EmpStation + ".FK_Station "
                    + " in(select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Station in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_Emp.FK_Dept='" + val + "'";
            }
            //点击部门时
            if (type == "Dept")
            {
                sql = "select distinct top " + rows + " Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_emp.FK_Dept='" + val + "' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_emp.No from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_emp.FK_Dept='" + val + "' order by Port_Emp.No) order by Port_Emp.No";
                SqlCount = "select distinct Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_emp.FK_Dept='" + val + "'";
            }
            //点击人员查询时
            if (type == "Emp")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(SELECT FK_Emp FROM WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and Port_dept.No='" + val + "' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(SELECT FK_Emp FROM WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and Port_dept.No='" + val + "' order by Port_Emp.No) order by Port_Emp.No";
                SqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(SELECT FK_Emp FROM WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and Port_dept.No='" + val + "'";
            }
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            DataTable dte = BP.DA.DBAccess.RunSQLReturnTable(SqlCount);
            string gridJson = GetEmpJson(dt, dte.Rows.Count);
            return gridJson;
        }
        /// <summary>
        /// 按绑定的人员查询
        /// </summary>
        /// <param name="type">查询条件（岗位、部门、人员）</param>
        /// <param name="val">当前岗位、部门的编号</param>
        /// <param name="page">当前页数</param>
        /// <param name="rows">每页显示行数</param>
        /// <returns></returns>
        public string GetEmpByEmp(string type, string val, int page, int rows)
        {
            string sql = "";
            string sqlCount = "";
            if (page == 0)
                page=1;
            if (String.IsNullOrEmpty(val))
            {
                //点击岗位时
                if (type == "Station")
                {
                    sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no where Port_Emp.No!='admin' and Port_Emp.No in"
                    + "(select distinct FK_Emp from " + BP.WF.Glo.EmpStation + " where FK_Station in"
                    + "(select FK_STATION from WF_NodeStation where FK_Node='" + this.ToNode + "')) and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no where Port_Emp.No in"
                    + "(select distinct FK_Emp from " + BP.WF.Glo.EmpStation + " where FK_Station in (select FK_STATION from WF_NodeStation where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                    sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no where Port_Emp.No in"
                    + "(select distinct FK_Emp from " + BP.WF.Glo.EmpStation + " where FK_Station in"
                    + "(select FK_STATION from WF_NodeStation where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin')";
                }
                //点击部门时
                if (type == "Dept")
                {
                    sql = "select distinct top " + rows + " Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select distinct No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_emp.No not in(select distinct top " + (page - 1) * rows + " Port_emp.No from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in(select distinct No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                    sqlCount = "select distinct Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select distinct No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin')";
                }
                //点击人员查询时
                if (type == "Emp")
                {
                    sql = "select distinct top " + rows + " Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                        + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                        + "(select distinct FK_EMP from WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and Port_emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_emp.No from Port_emp left join Port_dept on Port_emp.FK_Dept=Port_dept.no"
                    + " where Port_emp.No in(select distinct FK_EMP from WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                    sqlCount = "select distinct Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                        + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                        + "(select distinct FK_EMP from WF_NodeEmp where FK_Node='" + this.ToNode + "' and Port_Emp.No!='admin')";
                }
            }
            else
            {
                string strSql = "select No from Port_Emp where Name like'%" + Name + "%'";
                DataTable dtl = BP.DA.DBAccess.RunSQLReturnTable(strSql);
                string emps = "";
                foreach (DataRow item in dtl.Rows)
                {
                    emps += item["No"].ToString() + "','";
                }
                //点击岗位时
                if (type == "Station")
                {
                    sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no where Port_Emp.No in"
                    + "(select distinct FK_Emp from " + BP.WF.Glo.EmpStation + " where FK_Station in"
                    + "(select FK_STATION from WF_NodeStation where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_Emp.No in('" + emps + "') and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no where Port_Emp.No in"
                    + "(select distinct FK_Emp from " + BP.WF.Glo.EmpStation + " where FK_Station in (select FK_STATION from WF_NodeStation where FK_Node='" + this.ToNode + "')) and Port_Emp.No in('" + emps + "') and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                    sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no where Port_Emp.No in"
                    + "(select distinct FK_Emp from " + BP.WF.Glo.EmpStation + " where FK_Station in"
                    + "(select FK_STATION from WF_NodeStation where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin')";
                }
                //点击部门时
                if (type == "Dept")
                {
                    sql = "select distinct top " + rows + " Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select distinct No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No in('" + emps + "') and Port_Emp.No!='admin' and Port_emp.No not in(select distinct top " + (page - 1) * rows + " Port_emp.No from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in(select distinct No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_Emp.No in('" + emps + "') order by Port_Emp.No) order by Port_Emp.No";
                    sqlCount = "select distinct Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                    + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                    + "(select distinct No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin')";
                }
                //点击人员查询时
                if (type == "Emp")
                {
                    sql = "select distinct top " + rows + " Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                            + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                            + "(select distinct FK_EMP from WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and Port_Emp.No in('" + emps + "') and Port_emp.No not in("
                        + "select distinct top " + (page - 1) * rows + " Port_emp.No from Port_emp left join Port_dept on Port_emp.FK_Dept=Port_dept.no"
                        + " where Port_emp.No in(select distinct FK_EMP from WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and Port_Emp.No in('" + emps + "') order by Port_Emp.No) order by Port_Emp.No";
                    sqlCount = "select distinct Port_emp.No,Port_emp.Name,port_dept.Name as DeptName from Port_emp left join Port_dept "
                            + "on Port_emp.FK_Dept=Port_dept.no where Port_emp.No in"
                            + "(select distinct FK_EMP from WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin'";
                }
            }   
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            DataTable dte = BP.DA.DBAccess.RunSQLReturnTable(sqlCount);
            string gridJson = GetEmpJson(dt, dte.Rows.Count);
            return gridJson;
        }
        /// <summary>
        /// 按绑定的岗位查询
        /// </summary>
        /// <param name="type">查询条件（岗位、部门、人员）</param>
        /// <param name="val">当前岗位、部门的编号</param>
        /// <param name="page">当前页数</param>
        /// <param name="rows">每页显示行数</param>
        /// <returns></returns>
        public string GetEmpByStation(string type, string val, int page, int rows)
        {
            string sql = "";
            string sqlCount = "";
            if (page == 0)
                page = 1;
            //如果点击岗位时
            if (type == "Station")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where " + BP.WF.Glo.EmpStation + ".FK_Station='" + val + "' and "+BP.WF.Glo.EmpStation+".FK_Station "
                    + " in(select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Station in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where " + BP.WF.Glo.EmpStation + ".FK_Station='" + val + "' and "+BP.WF.Glo.EmpStation+".FK_Station "
                    + " in(select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Station in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where " + BP.WF.Glo.EmpStation + ".FK_Station='" + val + "' and " + BP.WF.Glo.EmpStation + ".FK_Station "
                    + " in(select FK_Station from " + BP.WF.Glo.EmpStation + " where FK_Station in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin'";
            }
            //如果点击部门时
            if (type == "Dept")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join " + BP.WF.Glo.EmpStation + " "
                    + " on " + BP.WF.Glo.EmpStation + ".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(select No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and " + BP.WF.Glo.EmpStation + ".FK_Station='" + val + "' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(select No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and " + BP.WF.Glo.EmpStation + ".FK_Station='" + val + "' order by Port_Emp.No) order by Port_Emp.No";
                sqlCount="select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(select No from Port_emp where FK_Dept in(SELECT FK_Dept FROM WF_NodeDept where FK_Node='" + this.ToNode + "')) and Port_Emp.No!='admin' and " + BP.WF.Glo.EmpStation + ".FK_Station='" + val + "'";
            }
            //如果点击人员查询时
            if (type == "Emp")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(SELECT FK_Emp FROM WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and "+BP.WF.Glo.EmpStation+".FK_Station='" + val + "' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(SELECT FK_Emp FROM WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and "+BP.WF.Glo.EmpStation+".FK_Station='" + val + "' order by Port_Emp.No) order by Port_Emp.No";
                sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No "
                    + " in(SELECT FK_Emp FROM WF_NodeEmp where FK_Node='" + this.ToNode + "') and Port_Emp.No!='admin' and "+BP.WF.Glo.EmpStation+".FK_Station='" + val + "'";
            }
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            DataTable dte = BP.DA.DBAccess.RunSQLReturnTable(sqlCount);
            string gridJson = GetEmpJson(dt, dte.Rows.Count);
            return gridJson;
        }
        /// <summary>
        /// 按绑定的SQL语句查询
        /// </summary>
        /// <param name="type">查询条件（岗位、部门、人员）</param>
        /// <param name="val">当前岗位、部门的编号或者人员的名称</param>
        /// <returns></returns>
        public string BindBySQL(string type, string val, int page, int rows)
        {
            string BindBySQL = MySelector.SelectorP1;
            BindBySQL = BindBySQL.Replace("@WebUser.No", WebUser.No);
            BindBySQL = BindBySQL.Replace("@WebUser.Name", WebUser.Name);
            BindBySQL = BindBySQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            string sql = "";
            string sqlCount = "";
            if (page == 0)
                page = 1;
            //如果点击岗位时
            if (type == "Station")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where "+BP.WF.Glo.EmpStation+".FK_Station='" + val + "' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No!='admin' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where "+BP.WF.Glo.EmpStation+".FK_Station='" + val + "' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where "+BP.WF.Glo.EmpStation+".FK_Station='" + val + "' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No!='admin'";
            }
            //如果点击部门时
            if (type == "Dept")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Dept.No='" + val + "' and Port_Emp.No!='admin' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No!='admin' and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Dept.No='" + val + "' and Port_Emp.No!='admin' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No!='admin' order by Port_Emp.No) order by Port_Emp.No";
                sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Dept.No='" + val + "' and Port_Emp.No!='admin' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No!='admin'";
            }
            //如果点击人员查询时
            if (type == "Emp")
            {
                sql = "select distinct top " + rows + " Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No!='admin' and Port_Emp.No in(" + BindBySQL + ") and Port_Emp.No not in("
                    + "select distinct top " + (page - 1) * rows + " Port_Emp.No from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No!='admin' and Port_Emp.No in(" + BindBySQL + ") order by Port_Emp.No) order by Port_Emp.No";
                sqlCount = "select distinct Port_Emp.No,Port_Emp.Name,port_dept.Name as DeptName from Port_Emp left join "+BP.WF.Glo.EmpStation+" "
                    + " on "+BP.WF.Glo.EmpStation+".FK_Emp=Port_Emp.No left join Port_dept on Port_emp.FK_Dept=Port_dept.no "
                    + "where Port_Emp.No!='admin' and Port_Emp.No in(" + BindBySQL + ")";

            }

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            DataTable dte = BP.DA.DBAccess.RunSQLReturnTable(sqlCount);
            string gridJson = GetEmpJson(dt,dte.Rows.Count);
            return gridJson;
        }
        #endregion
        #region 生成json的通用方法
        /// <summary>
        /// 生成Json的通用方法
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        public string GetEmpJson( DataTable dt,int count)
        {
            int cnt = count;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"total\":" + cnt.ToString() + ",\"rows\":[");
            foreach (DataRow row in dt.Rows)
            {
                sb.Append("{\"Name\":\"" + row["Name"] + "\",\"DepartName\":\"" + row["DeptName"] + "\",\"UserName\":\"" + row["No"] + "\"");
                sb.Append("},");
            }

            sb = (cnt > 0) ? sb.Remove(sb.Length - 1, 1) : sb;

            sb.Append("]}");

            return sb.ToString();
        }
        #endregion
        #region 根据DataTable生成EasyUI Tree Json树结构
        StringBuilder result = new StringBuilder();
        StringBuilder sb = new StringBuilder();
        /// <summary>  
        /// 根据DataTable生成EasyUI Tree Json树结构  
        /// </summary>  
        /// <param name="tabel">数据源</param>  
        /// <param name="idCol">ID列</param>  
        /// <param name="txtCol">Text列</param>  
        /// <param name="url">节点Url</param>  
        /// <param name="rela">关系字段</param>  
        /// <param name="pId">父ID</param>  
        private string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId)
        {
            result.Append(sb.ToString());
            sb.Clear();

            if (tabel.Rows.Count > 0)
            {
                sb.Append("[");
                string filer = string.Format("{0}='{1}'", rela, pId);
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)
                {
                    try
                    {
                        foreach (DataRow row in rows)
                        {
                            sb.Append("{\"id\":\"" + row[idCol] + "\",\"text\":\"" + row[txtCol] + "\"");
                            if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                            {
                                if (Convert.ToString(pId) == "0") { sb.Append(",\"state\":\"open\",\"children\":"); }
                                //点击展开
                                else
                                    sb.Append(",\"state\":\"closed\",\"children\":");
                                GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol]);
                                result.Append(sb.ToString());
                                sb.Clear();
                            }
                            result.Append(sb.ToString());
                            sb.Clear();
                            sb.Append("},");
                        }
                        sb = sb.Remove(sb.Length - 1, 1);
                        sb.Append("]");
                        result.Append(sb.ToString());
                        sb.Clear();
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }

                }
            }
            return result.ToString();
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}