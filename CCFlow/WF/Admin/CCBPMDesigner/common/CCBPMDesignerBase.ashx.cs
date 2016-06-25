using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web.SessionState;
using BP.DA;
using BP.Web;
using BP.WF;
using BP.Sys;
using BP.En;
using BP.WF.Template;

namespace CCFlow.WF.Admin.CCBPMDesigner.common
{
    /// <summary>
    /// CCBPMDesignerBase 的摘要说明
    /// </summary>
    public class CCBPMDesignerBase : IHttpHandler, IRequiresSessionState
    {
        #region 全局变量IRequiresSessionState
        /// <summary>
        /// http请求
        /// </summary>
        public HttpContext _Context
        {
            get;
            set;
        }

        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(_Context.Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return getUTF8ToString("FK_Flow");
            }
        }
        #endregion

        public void ProcessRequest(HttpContext context)
        {
            _Context = context;

            if (_Context == null) return;

            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["method"]))
                method = context.Request["method"].ToString();

            switch (method)
            {
                case "WebUserInfo"://获取用户信息
                    s_responsetext = GetWebUserInfo();
                    break;
                case "GetFlowTree"://获取流程树数据
                    s_responsetext = GetFlowTreeTable();// GetFlowTree();
                    break;
                case "GetFormTree"://获取表单库数据
                    s_responsetext = GetFormTreeTable();//GetFormTree();
                    break;
                case "GetSrcTree"://获取数据源数据
                    s_responsetext = GetSrcTreeTable();
                    break;
                case "GetStructureTree"://获取组织结构数据
                    s_responsetext = GetStructureTreeTable();
                    break;
                case "GetBindingForms"://获取流程绑定表单列表
                    s_responsetext = GetBindingFormsTable();
                    break;
                case "GetFlowNodes"://获取流程节点列表
                    s_responsetext = GetFlowNodesTable();
                    break;
                case "Do"://公共方法
                    s_responsetext = Do();
                    break;
                case "LetLogin":    //使管理员登录
                    s_responsetext = WebUser.No == "admin" ? string.Empty : LetAdminLogin("CH", true);
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            context.Response.Charset = "UTF-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            context.Response.Expires = 0;
            context.Response.Write(s_responsetext);
            context.Response.End();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        private string GetWebUserInfo()
        {
            try
            {
                if (WebUser.No == null)
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = "当前用户没有登录，请登录后再试。", data = new { } });
                BP.Port.Emp emp = new BP.Port.Emp(WebUser.No);

                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, msg = string.Empty, data = new { No = emp.No, Name = emp.Name, FK_Dept = emp.FK_Dept, SID = emp.SID } });
            }
            catch (Exception ex)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message, data = new { } });
            }
        }

        StringBuilder sbJson = new StringBuilder();
        /// <summary>
        /// 获取流程树数据
        /// </summary>
        /// <returns>返回结果Json,流程树</returns>
        private string GetFlowTree()
        {
            string sql = @"SELECT 'F'+No No,'F'+ParentNo ParentNo,Name, Idx, 1 IsParent,'FLOWTYPE' TType,-1 DType FROM WF_FlowSort
                           union 
                           SELECT No, 'F'+FK_FlowSort as ParentNo,Name,Idx,0 IsParent,'FLOW' TType,DType FROM WF_Flow";

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = @"SELECT 'F'||No No,'F'||ParentNo ParentNo,Name, Idx, 1 IsParent,'FLOWTYPE' TType,-1 DType FROM WF_FlowSort
                        union 
                        SELECT No, 'F'||FK_FlowSort as ParentNo,Name,Idx,0 IsParent,'FLOW' TType,DType FROM WF_Flow";
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            var dt_Clone = dt.Clone();

            //1.流程类别；2.流程
            foreach (DataRow row in dt.Rows)
                dt_Clone.Rows.Add(row.ItemArray);

            //3.流程云数据；4.共有云；5.私有云
            dt_Clone.Rows.Add("FlowCloud", "-1", "流程云", 0, 1, "FLOWCLOUD", "-1");
            dt_Clone.Rows.Add("ShareFlow", "FlowCloud", "共有流程云", 0, 0, "SHAREFLOW", "-1");
            dt_Clone.Rows.Add("PriFlow", "FlowCloud", "私有流程云", 0, 0, "PRIFLOW", "-1");
            sbJson.Clear();

            string sTmp = "";
            if (dt_Clone != null && dt_Clone.Rows.Count > 0)
            {
                GetTreeJsonByTable(dt_Clone, "", attrFields: new[] { "TType", "DType" });
            }
            sTmp += sbJson.ToString();
            return sTmp;
        }

        private string GetFlowTreeTable()
        {
            string sql = @"SELECT 'F'+No NO,'F'+ParentNo PARENTNO,NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                           union 
                           SELECT NO, 'F'+FK_FlowSort as PARENTNO,NAME,IDX,0 ISPARENT,'FLOW' TTYPE,DTYPE FROM WF_Flow";

            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                sql = @"SELECT 'F'||No NO,'F'||ParentNo PARENTNO,NAME, IDX, 1 ISPARENT,'FLOWTYPE' TTYPE,-1 DTYPE FROM WF_FlowSort
                        union 
                        SELECT NO, 'F'||FK_FlowSort as PARENTNO,NAME,IDX,0 ISPARENT,'FLOW' TTYPE,DTYPE FROM WF_Flow";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //判断是否为空，如果为空，则创建一个流程根结点，added by liuxc,2016-01-24
            if (dt.Rows.Count == 0)
            {
                FlowSort fs = new FlowSort();
                fs.No = "99";
                fs.ParentNo = "0";
                fs.Name = "流程树";
                fs.Insert();

                dt.Rows.Add("F99", "F0", "流程树", 0, 1, "FLOWTYPE", -1);
            }
            else
            {
                DataRow[] drs = dt.Select("NAME='流程树'");
                if (drs.Length > 0 && !Equals(drs[0]["PARENTNO"], "F0"))
                    drs[0]["PARENTNO"] = "F0";
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }

        private string GetFlowNodesTable()
        {
            string fk_flow = getUTF8ToString("fk_flow");
            if (string.IsNullOrWhiteSpace(fk_flow))
                return "[]";

            var sql = new StringBuilder();

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql.AppendLine("SELECT CAST(wn.NodeID AS VARCHAR) AS NO,");
                    sql.AppendLine("       ('(' + CAST(wn.NodeID AS VARCHAR) + ')' + wn.Name) AS NAME,");
                    break;
                case DBType.MySQL:
                    sql.AppendLine("SELECT CAST(wn.NodeID AS CHAR) AS NO,");
                    sql.AppendLine("       CONCAT('(',wn.NodeID,')',wn.Name) AS NAME,");
                    break;
                case DBType.Informix:
                    sql.AppendLine("SELECT CAST(wn.NodeID AS CHAR) AS NO,");   //未证实
                    sql.AppendLine("       ('(' || wn.NodeID || ')' || wn.Name) AS NAME,");    //未证实 
                    break;
                case DBType.Oracle:
                    sql.AppendLine("SELECT to_char(wn.NodeID) AS NO,");
                    sql.AppendLine("       ('(' || wn.NodeID || ')' || wn.Name) AS NAME,");
                    break;
                case DBType.DB2:
                    sql.AppendLine("SELECT CHAR(wn.NodeID) AS NO,");   //未证实
                    sql.AppendLine("       ('(' || wn.NodeID || ')' || wn.Name) AS NAME,");    //未证实，也可用CONCAT函数，但此函数只支持两两连接，且两个必须都是字符串
                    break;
                case DBType.Access:
                    sql.AppendLine("SELECT CStr(wn.NodeID) AS NO,");
                    sql.AppendLine("       ('(' & wn.NodeID & ')' & wn.Name) AS NAME,"); //Access中的别名必须加AS操作符
                    break;
                case DBType.Sybase:
                    sql.AppendLine("SELECT CONVERT(VARCHAR, wn.NodeID) AS NO,");   //未证实
                    sql.AppendLine("       ('(' + CONVERT(VARCHAR, wn.NodeID) + ')' + wn.Name) AS NAME,"); //未证实
                    break;
            }

            sql.AppendLine("       NULL AS PARENTNO,");
            sql.AppendLine("       'NODE' AS TTYPE,");
            sql.AppendLine("       -1 AS DTYPE,");
            sql.AppendLine("       0 AS ISPARENT");
            sql.AppendLine("FROM   WF_Node wn");
            sql.AppendLine("WHERE  wn.FK_Flow = '{0}'");
            sql.AppendLine("ORDER BY");
            sql.AppendLine("       wn.Step ASC");

            DataTable dt = DBAccess.RunSQLReturnTable(string.Format(sql.ToString(), fk_flow));
            return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }

        private string GetBindingFormsTable()
        {
            string fk_flow = getUTF8ToString("fk_flow");
            if (string.IsNullOrWhiteSpace(fk_flow))
                return "[]";

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT wfn.FK_Frm NO,");
            sql.AppendLine("       smd.NAME,");
            sql.AppendLine("       NULL PARENTNO,");
            sql.AppendLine("       'FORM' TTYPE,");
            sql.AppendLine("       -1 DTYPE,");
            sql.AppendLine("       0 ISPARENT");
            sql.AppendLine("FROM   WF_FrmNode wfn");
            sql.AppendLine("       INNER JOIN Sys_MapData smd");
            sql.AppendLine("            ON  smd.No = wfn.FK_Frm");
            sql.AppendLine("WHERE  wfn.FK_Flow = '{0}'");
            sql.AppendLine("       AND wfn.FK_Node = (");
            sql.AppendLine("               SELECT TOP 1 wn.NodeID");
            sql.AppendLine("               FROM   WF_Node wn");
            sql.AppendLine("               WHERE  wn.FK_Flow = '{0}'");
            sql.AppendLine("               ORDER BY");
            sql.AppendLine("                      wn.Step ASC");
            sql.AppendLine("           )");

            DataTable dt = DBAccess.RunSQLReturnTable(string.Format(sql.ToString(), fk_flow));
            return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }

        private string GetFormTreeTable()
        {
            string sqls = "SELECT NO ,PARENTNO,NAME, IDX, 1 ISPARENT, 'FORMTYPE' TTYPE, DBSRC FROM Sys_FormTree;" + Environment.NewLine
                      +
                      " SELECT NO, FK_FrmSort as PARENTNO,NAME,IDX,0 ISPARENT, 'FORM' TTYPE FROM Sys_MapData   where AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree);" + Environment.NewLine
                      +
                      " SELECT ss.NO,'' PARENTNO,ss.NAME,0 IDX, 1 ISPARENT, 'SRC' TTYPE FROM Sys_SFDBSrc ss ORDER BY ss.DBSrcType ASC;";

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            DataTable dt = ds.Tables[1].Clone();

            DataRow[] rows = ds.Tables[0].Select("NAME='表单库'");
            DataRow rootRow;

            if (rows.Length == 0)
            {
                rootRow = dt.Rows.Add("0", null, "表单库", 0, 1, "FORMTYPE");
            }
            else
            {
                rootRow = dt.Rows.Add(rows[0]["NO"], null, rows[0]["NAME"], rows[0]["IDX"], rows[0]["ISPARENT"], rows[0]["TTYPE"]);
            }

            foreach (DataRow row in ds.Tables[2].Rows)
            {
                dt.Rows.Add("SRC_" + row["NO"], rootRow["NO"], row["NAME"], row["IDX"], row["ISPARENT"], row["TTYPE"]);

                rows = ds.Tables[0].Select("DBSRC='" + row["NO"] + "' AND NAME <> '表单库'");

                foreach (DataRow dr in rows)
                {
                    if (Equals(dr["PARENTNO"], rootRow["NO"]))
                    {
                        dr["PARENTNO"] = "SRC_" + row["NO"];
                    }

                    dt.Rows.Add(dr["NO"], dr["PARENTNO"], dr["NAME"], dr["IDX"], dr["ISPARENT"], dr["TTYPE"]);
                }
            }

            foreach (DataRow row in ds.Tables[1].Rows)
            {
                dt.Rows.Add(row.ItemArray);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }

        /// <summary>
        /// 获取表单库数据
        /// </summary>
        /// <returns>返回结果Json,表单库</returns>
        private string GetFormTree()
        {
            var sqls = "SELECT No ,ParentNo,Name, Idx, 1 IsParent, 'FORMTYPE' TType FROM Sys_FormTree"
                      + " union "
                      +
                      " SELECT No, FK_FrmSort as ParentNo,Name,Idx,0 IsParent, 'FORM' TType FROM Sys_MapData   where AppType=0 AND FK_FormTree IN (SELECT No FROM Sys_FormTree);" + Environment.NewLine
                      +
                      " SELECT ss.No,'SrcRoot' AS ParentNo,ss.Name,0 AS Idx, 1 IsParent, 'SRC' TType FROM Sys_SFDBSrc ss ORDER BY ss.DBSrcType ASC;" + Environment.NewLine
                      +
                      " SELECT st.No, st.FK_SFDBSrc AS ParentNo,st.Name,0 AS Idx, 0 IsParent, 'SRCTABLE' TType FROM Sys_SFTable st;";

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            var dt = ds.Tables[0].Clone();

            //1.表单类别；2.表单
            foreach (DataRow row in ds.Tables[0].Rows)
                dt.Rows.Add(row.ItemArray);

            //3.数据源字典表
            dt.Rows.Add("SrcRoot", "-1", "数据源字典表", 0, 1, "SRCROOT");

            //4.数据源
            foreach (DataRow row in ds.Tables[1].Rows)
                dt.Rows.Add(row.ItemArray);

            //5.数据接口表
            foreach (DataRow row in ds.Tables[2].Rows)
                dt.Rows.Add(row["No"], Equals(row["ParentNo"], null) || DBNull.Value == row["ParentNo"] || string.IsNullOrWhiteSpace(row["ParentNo"].ToString()) ? "local" : row["ParentNo"], row["Name"], row["Idx"], row["IsParent"],
                            row["TType"]);

            //6.表单相关；7.枚举列表；8.JS验证库；9.Internet云数据；10.私有表单库；11.共享表单库
            dt.Rows.Add("FormRef", "-1", "表单相关", 0, 1, "FORMREF");
            dt.Rows.Add("Enums", "FormRef", "枚举列表", 0, 0, "ENUMS");
            dt.Rows.Add("JSLib", "FormRef", "JS验证库", 0, 0, "JSLIB");
            dt.Rows.Add("FUNCM", "FormRef", "功能执行", 0, 0, "FUNCM");

            dt.Rows.Add("CloundData", "-1", "ccbpm云服务-表单云", 0, 1, "CLOUNDDATA");
            dt.Rows.Add("PriForm", "CloundData", "私有表单云", 0, 0, "PRIFORM");
            dt.Rows.Add("ShareForm", "CloundData", "共有表单云", 0, 0, "SHAREFORM");

            sbJson.Clear();

            string sTmp = "";

            if (dt.Rows.Count > 0)
            {
                GetTreeJsonByTable(dt, "", attrFields: new[] { "TType" });
            }
            sTmp += sbJson.ToString();

            return sTmp;
        }

        private string GetSrcTreeTable()
        {
            string sqls = "SELECT ss.NO,'SrcRoot' PARENTNO,ss.NAME,0 IDX, 1 ISPARENT, 'SRC' TTYPE FROM Sys_SFDBSrc ss ORDER BY ss.DBSrcType ASC;" + Environment.NewLine
                      +
                      " SELECT st.NO, st.FK_SFDBSrc AS PARENTNO,st.NAME,0 AS IDX, 0 ISPARENT, 'SRCTABLE' TTYPE FROM Sys_SFTable st;";

            DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
            DataTable dt = ds.Tables[0].Clone();

            foreach (DataRow row in ds.Tables[0].Rows)
                dt.Rows.Add(row.ItemArray);

            foreach (DataRow row in ds.Tables[1].Rows)
                dt.Rows.Add(row.ItemArray);

            return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }

        private string GetStructureTreeTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("NO", typeof(string));
            dt.Columns.Add("PARENTNO", typeof(string));
            dt.Columns.Add("NAME", typeof(string));
            dt.Columns.Add("TTYPE", typeof(string));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.WF.Port.Depts depts = new BP.WF.Port.Depts();
                depts.RetrieveAll();
                BP.WF.Port.Stations sts = new BP.WF.Port.Stations();
                sts.RetrieveAll();
                BP.WF.Port.Emps emps = new BP.WF.Port.Emps();
                emps.RetrieveAll(BP.WF.Port.EmpAttr.Name);
                BP.WF.Port.EmpStations empsts = new BP.WF.Port.EmpStations();
                empsts.RetrieveAll();
                BP.GPM.DeptEmps empdetps = new BP.GPM.DeptEmps();
                empdetps.RetrieveAll();

                //部门人员
                Dictionary<string, List<string>> des = new Dictionary<string, List<string>>();
                //岗位人员
                Dictionary<string, List<string>> ses = new Dictionary<string, List<string>>();
                //部门岗位
                Dictionary<string, List<string>> dss = new Dictionary<string, List<string>>();
                BP.WF.Port.Station stt = null;
                BP.WF.Port.Emp empt = null;

                foreach (BP.WF.Port.Dept dept in depts)
                {
                    //增加部门
                    dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");
                    des.Add(dept.No, new List<string>());
                    dss.Add(dept.No, new List<string>());

                    //获取部门下的岗位
                    empdetps.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, dept.No);
                    foreach (BP.GPM.DeptEmp empdept in empdetps)
                    {
                        des[dept.No].Add(empdept.FK_Emp);
                        //判断该人员拥有的岗位
                        empsts.Retrieve(BP.WF.Port.EmpStationAttr.FK_Emp, empdept.FK_Emp);
                        foreach (BP.WF.Port.EmpStation es in empsts)
                        {
                            if (ses.ContainsKey(es.FK_Station))
                            {
                                if (ses[es.FK_Station].Contains(es.FK_Emp) == false)
                                    ses[es.FK_Station].Add(es.FK_Emp);
                            }
                            else
                            {
                                ses.Add(es.FK_Station, new List<string> { es.FK_Emp });
                            }

                            //增加部门的岗位
                            if (dss[dept.No].Contains(es.FK_Station) == false)
                            {
                                stt = sts.GetEntityByKey(es.FK_Station) as BP.WF.Port.Station;

                                if (stt == null) continue;

                                dss[dept.No].Add(es.FK_Station);
                                dt.Rows.Add(dept.No + "|" + es.FK_Station, dept.No, stt.Name, "STATION");
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, List<string>> ds in dss)
                {
                    foreach (string st in ds.Value)
                    {
                        foreach (string emp in ses[st])
                        {
                            empt = emps.GetEntityByKey(emp) as BP.WF.Port.Emp;

                            if (empt == null) continue;

                            dt.Rows.Add(ds.Key + "|" + st + "|" + emp, ds.Key + "|" + st, empt.Name, "EMP");
                        }
                    }
                }
            }
            else
            {
                BP.GPM.Depts depts = new BP.GPM.Depts();
                depts.RetrieveAll();
                BP.GPM.Stations sts = new BP.GPM.Stations();
                sts.RetrieveAll();
                BP.GPM.Emps emps = new BP.GPM.Emps();
                emps.RetrieveAll(BP.WF.Port.EmpAttr.Name);
                BP.GPM.DeptStations dss = new BP.GPM.DeptStations();
                dss.RetrieveAll();
                BP.GPM.DeptEmpStations dess = new BP.GPM.DeptEmpStations();
                dess.RetrieveAll();
                BP.GPM.Station stt = null;
                BP.GPM.Emp empt = null;

                foreach (BP.GPM.Dept dept in depts)
                {
                    //增加部门
                    dt.Rows.Add(dept.No, dept.ParentNo, dept.Name, "DEPT");

                    //增加部门岗位
                    dss.Retrieve(BP.GPM.DeptStationAttr.FK_Dept, dept.No);
                    foreach (BP.GPM.DeptStation ds in dss)
                    {
                        stt = sts.GetEntityByKey(ds.FK_Station) as BP.GPM.Station;

                        if (stt == null) continue;

                        dt.Rows.Add(dept.No + "|" + ds.FK_Station, dept.No, stt.Name, "STATION");

                        //增加部门岗位人员
                        dess.Retrieve(BP.GPM.DeptEmpStationAttr.FK_Dept, dept.No, BP.GPM.DeptEmpStationAttr.FK_Station,
                                      ds.FK_Station);

                        foreach (BP.GPM.DeptEmpStation des in dess)
                        {
                            empt = emps.GetEntityByKey(des.FK_Emp) as BP.GPM.Emp;

                            if (empt == null) continue;

                            dt.Rows.Add(dept.No + "|" + ds.FK_Station + "|" + des.FK_Emp, dept.No + "|" + ds.FK_Station,
                                        empt.Name, "EMP");
                        }
                    }
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
        }

        public string GetStructureDatas(string deptNo, string stationNo, string empNo)
        {
            return null;
        }

        /// <summary>
        /// 根据DataTable生成Json树结构
        /// </summary>
        public string GetTreeJsonByTable(DataTable tabel, object pId, string rela = "ParentNo", string idCol = "No", string txtCol = "Name", string IsParent = "IsParent", string sChecked = "", string[] attrFields = null)
        {
            string treeJson = string.Empty;

            if (tabel.Rows.Count > 0)
            {
                sbJson.Append("[");
                string filer = string.Empty;
                if (pId.ToString() == "")
                {
                    filer = string.Format("{0} is null or {0}='-1' or {0}='0' or {0}='F0'", rela);
                }
                else
                {
                    filer = string.Format("{0}='{1}'", rela, pId);
                }

                DataRow[] rows = tabel.Select(filer, idCol);
                if (rows.Length > 0)
                {
                    for (int i = 0; i < rows.Length; i++)
                    {
                        DataRow row = rows[i];


                        string jNo = row[idCol] as string;
                        string jText = row[txtCol] as string;
                        if (jText.Length > 25)
                            jText = jText.Substring(0, 25) + "<img src='../Scripts/easyUI/themes/icons/add2.png' onclick='moreText(" + jNo + ")'/>";

                        string jIsParent = row[IsParent].ToString();
                        string jState = "1".Equals(jIsParent) ? "open" : "closed";
                        jState = "open".Equals(jState) && i == 0 ? "open" : "closed";

                        DataRow[] rowChild = tabel.Select(string.Format("{0}='{1}'", rela, jNo));
                        string tmp = "{\"id\":\"" + jNo + "\",\"text\":\"" + jText;

                        //增加自定义attributes列，added by liuxc,2015-10-6
                        var attrs = string.Empty;
                        if (attrFields != null && attrFields.Length > 0)
                        {
                            foreach (var field in attrFields)
                            {
                                if (!tabel.Columns.Contains(field)) continue;
                                if (string.IsNullOrEmpty(row[field].ToString()))
                                {
                                    attrs += ",\"" + field + "\":\"\"";
                                    continue;
                                }
                                attrs += ",\"" + field + "\":" +
                                         (tabel.Columns[field].DataType == typeof(string)
                                              ? string.Format("\"{0}\"", row[field])
                                              : row[field]);
                            }
                        }

                        if ("0".Equals(pId.ToString()) || row[rela].ToString() == "F0")
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\",\"IsRoot\":\"1\"" + attrs + "}";
                        }
                        else
                        {
                            tmp += "\",\"attributes\":{\"IsParent\":\"" + jIsParent + "\"" + attrs + "}";
                        }

                        if (rowChild.Length > 0)
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower()
                                + ",\"state\":\"" + jState + "\"";
                        }
                        else
                        {
                            tmp += ",\"checked\":" + sChecked.Contains("," + jNo + ",").ToString().ToLower();
                        }

                        sbJson.Append(tmp);
                        if (rowChild.Length > 0)
                        {
                            sbJson.Append(",\"children\":");
                            GetTreeJsonByTable(tabel, jNo, rela, idCol, txtCol, IsParent, sChecked, attrFields);
                        }

                        sbJson.Append("},");
                    }
                    sbJson = sbJson.Remove(sbJson.Length - 1, 1);
                }
                sbJson.Append("]");
                treeJson = sbJson.ToString();
            }
            return treeJson;
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <returns></returns>
        private string DelFlow()
        {
            string msg = WorkflowDefintionManager.DeleteFlowTemplete(this.FK_Flow);
            if (msg == null)
                return "true";
            return "false";
        }

        /// <summary>
        /// 树节点管理
        /// </summary>
        public string Do()
        {
            string doWhat = getUTF8ToString("doWhat");
            string para1 = getUTF8ToString("para1");
            // 如果admin账户登陆时有错误发生，则返回错误信息
            var result = LetAdminLogin("CH", true);

            if (string.IsNullOrEmpty(result) == false)
                return result;

            switch (doWhat)
            {
                case "GetFlowSorts":    //获取所有流程类型
                    try
                    {
                        FlowSorts flowSorts = new FlowSorts();
                        flowSorts.RetrieveAll(FlowSortAttr.Idx);
                        return BP.Tools.Entitis2Json.ConvertEntitis2GenerTree(flowSorts, "0");
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "NewSameLevelFrmSort": //创建同级别的 表单树 目录.
                    SysFormTree frmSort = null;
                    try
                    {
                        var para = para1.Split(',');
                        frmSort = new SysFormTree(para[0]);
                        string sameNodeNo = frmSort.DoCreateSameLevelNode().No;
                        frmSort = new SysFormTree(sameNodeNo);
                        frmSort.Name = para[1];
                        frmSort.Update();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Do Method NewFormSort Branch has a error , para:\t" + para1 + ex.Message;
                    }
                case "NewSubLevelFrmSort": //创建子级别的 表单树 目录.
                    SysFormTree frmSortSub = null;
                    try
                    {
                        var para = para1.Split(',');
                        frmSortSub = new SysFormTree(para[0]);
                        string sameNodeNo = frmSortSub.DoCreateSubNode().No;
                        frmSortSub = new SysFormTree(sameNodeNo);
                        frmSortSub.Name = para[1];
                        frmSortSub.Update();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return "Do Method NewSubLevelFrmSort Branch has a error , para:\t" + para1 + ex.Message;
                    }
                case "NewSameLevelFlowSort":  //创建同级别的 流程树 目录.
                    FlowSort fs = null;
                    try
                    {
                        var para = para1.Split(',');
                        fs = new FlowSort(para[0].Replace("F", ""));//传入的编号多出F符号，需要替换掉
                        string sameNodeNo = fs.DoCreateSameLevelNode().No;
                        fs = new FlowSort(sameNodeNo);
                        fs.Name = para[1];
                        fs.Update();
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = "F" + fs.No });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewSameLevelFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "NewSubFlowSort": //创建子级别的 流程树 目录.
                    try
                    {
                        var para = para1.Split(',');
                        FlowSort fsSub = new FlowSort(para[0].Replace("F", ""));//传入的编号多出F符号，需要替换掉
                        string subNodeNo = fsSub.DoCreateSubNode().No;
                        FlowSort subFlowSort = new FlowSort(subNodeNo);
                        subFlowSort.Name = para[1];
                        subFlowSort.Update();
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = "F" + subFlowSort.No });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewSubFlowSort Branch has a error , para:\t" + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "EditFlowSort": //编辑表单树.
                    try
                    {
                        var para = para1.Split(',');
                        fs = new FlowSort(para[0].Replace("F", ""));//传入的编号多出F符号，需要替换掉
                        fs.Name = para[1];
                        fs.Save();
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = fs.No });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method EditFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "NewFlow": //创建新流程.
                    try
                    {
                        string[] ps = para1.Split(',');
                        if (ps.Length != 6)
                            throw new Exception("@创建流程参数错误");

                        string fk_floSort = ps[0]; //类别编号.
                        fk_floSort = fk_floSort.Replace("F", "");//传入的编号多出F符号，需要替换掉

                        string flowName = ps[1]; // 流程名称.
                        DataStoreModel dataSaveModel = (DataStoreModel)int.Parse(ps[2]); //数据保存方式。
                        string pTable = ps[3]; // 物理表名。
                        string flowMark = ps[4]; // 流程标记.
                        string flowVer = ps[5]; // 流程版本

                        string FK_Flow = BP.BPMN.Glo.NewFlow(fk_floSort, flowName, dataSaveModel, pTable, flowMark, flowVer);
                        return
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = string.Empty, data = new { no = FK_Flow, name = flowName } });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method NewFlow Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "DelFlow": //删除流程.
                    try
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(
                                new { success = true, msg = WorkflowDefintionManager.DeleteFlowTemplete(para1) });
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "DelFlowSort":
                    try
                    {
                        string FK_FlowSort = para1.Replace("F", "");
                        string forceDel = getUTF8ToString("force");
                        FlowSort delfs = new FlowSort();
                        delfs.No = FK_FlowSort;
                        //强制删除，不需判断是否含有子项。
                        if (forceDel == "true")
                        {
                            delfs.DeleteFlowSortSubNode_Force();
                            delfs.Delete();
                            return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, reason = "" });
                        }

                        //判断是否包含子类别
                        if (delfs.HisSubFlowSorts != null && delfs.HisSubFlowSorts.Count > 0)
                            return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, reason = "havesubsorts", msg = "此类别下包含子类别。" });

                        //判断是否包含工作流程
                        if (delfs.HisFlows != null && delfs.HisFlows.Count > 0)
                            return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, reason = "haveflows", msg = "此类别下包含流程。" });

                        //执行删除
                        delfs.Delete();
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, reason = "" });
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DefaultLogWriteLineError("Do Method DelFlowSort Branch has a error , para:\t" + para1 + ex.Message);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "DelNode":
                    try
                    {
                        if (!string.IsNullOrEmpty(para1))
                        {
                            BP.WF.Node delNode = new BP.WF.Node(int.Parse(para1));
                            delNode.Delete();
                        }
                        else
                        {
                            throw new Exception("@参数错误:" + para1);
                        }
                    }
                    catch (Exception ex)
                    {
                        return "err:" + ex.Message;
                    }
                    return null;
                case "SetBUnit":
                    try
                    {
                        if (!string.IsNullOrEmpty(para1))
                        {
                            BP.WF.Node nd = new BP.WF.Node(int.Parse(para1));
                            nd.IsTask = !nd.IsBUnit;
                            nd.Update();
                        }
                        else
                        {
                            throw new Exception("@参数错误:" + para1);
                        }
                    }
                    catch (Exception ex)
                    {
                        return "err:" + ex.Message;
                    }
                    return null;
                case "GetSettings":
                    return SystemConfig.AppSettings[para1];
                case "SaveFlowFrm":  //保存流程表单.
                    Entity en = null;
                    try
                    {
                        AtPara ap = new AtPara(para1);
                        string enName = ap.GetValStrByKey("EnName");
                        string pk = ap.GetValStrByKey("PKVal");
                        en = ClassFactory.GetEn(enName);
                        en.ResetDefaultVal();
                        if (en == null)
                            throw new Exception("无效的类名:" + enName);

                        if (string.IsNullOrEmpty(pk) == false)
                        {
                            en.PKVal = pk;
                            en.RetrieveFromDBSources();
                        }

                        foreach (string key in ap.HisHT.Keys)
                        {
                            if (key == "PKVal")
                                continue;
                            en.SetValByKey(key, ap.HisHT[key].ToString().Replace('^', '@'));
                        }
                        en.Save();
                        return en.PKVal as string;
                    }
                    catch (Exception ex)
                    {
                        if (en != null)
                            en.CheckPhysicsTable();
                        return "Error:" + ex.Message;
                    }
                case "ChangeNodeType":
                    var p = para1.Split(',');

                    try
                    {
                        if (p.Length != 3)
                            throw new Exception("@修改节点类型参数错误");

                        //var sql = "UPDATE WF_Node SET Icon='{0}' WHERE FK_Flow='{1}' AND NodeID='{2}'";
                        var sql = "UPDATE WF_Node SET RunModel={0} WHERE FK_Flow='{1}' AND NodeID={2}";
                        DBAccess.RunSQL(string.Format(sql, p[0], p[1], p[2]));
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                case "ChangeNodeIcon":
                    p = para1.Split(',');

                    try
                    {
                        if (p.Length != 3)
                            throw new Exception("@修改节点图标参数错误");

                        var sql = "UPDATE WF_Node SET Icon='{0}' WHERE FK_Flow='{1}' AND NodeID={2}";
                        DBAccess.RunSQL(string.Format(sql, p[0], p[1], p[2]));
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message });
                    }
                default:
                    throw new Exception("@没有约定的执行标记:" + doWhat);
            }
        }

        /// <summary>
        /// 让admin 登陆
        /// </summary>
        /// <param name="lang">当前的语言</param>
        /// <returns>成功则为空，有异常时返回异常信息</returns>
        public string LetAdminLogin(string lang, bool islogin)
        {
            try
            {
                if (islogin)
                {
                    BP.Port.Emp emp = new BP.Port.Emp("admin");
                    WebUser.SignInOfGener(emp, lang, "admin", true, false);
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            return string.Empty;
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