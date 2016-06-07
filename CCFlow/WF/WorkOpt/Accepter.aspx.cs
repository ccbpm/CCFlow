using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using System.Text;
using BP.Port;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;
namespace CCFlow.WF
{
    /// <summary>
    /// 接受人
    /// </summary>
    public partial class WF_Accepter : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 打开
        /// </summary>
        public int IsWinOpen
        {
            get
            {
                string str = this.Request.QueryString["IsWinOpen"];
                if (str == "1" || str == null || str == "")
                    return 1;
                return 0;
            }
        }
        /// <summary>
        /// 到达的节点
        /// </summary>
        public int ToNode
        {
            get
            {

                if (string.IsNullOrEmpty(this.Request.QueryString["ToNode"]))
                    return 0;
                return int.Parse(this.Request["ToNode"].ToString());
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request["FK_Node"].ToString());
            }
        }
        public Int64 WorkID
        {
            get
            {
                var v = Int64.Parse(this.Request["WorkID"].ToString());
                if (v == 0)
                {
                    string[] ids = this.WorkIDs.Split(',');
                    foreach (string id in ids)
                    {
                        if (string.IsNullOrEmpty(id) == true)
                            continue;
                        return Int64.Parse(id);
                    }
                    throw new Exception("@没有或得到 WorkIDs");
                }
                return v;
            }
        }
        public Int64 FID
        {
            get
            {
                Int64 fid = 0;
                if (this.Request["FID"] != null)
                    fid = Int64.Parse(this.Request["FID"].ToString());
                if (fid == 0 && this.WorkIDs != null && this.WorkIDs.Length > 3)
                    fid = BP.DA.DBAccess.RunSQLReturnValInt("select fid from wf_generworkflow where workid=" + this.WorkID, 0);
                return fid;
            }
        }
        public string FK_Dept
        {
            get
            {
                string s = this.Request.QueryString["FK_Dept"];
                if (s == null)
                    s = WebUser.FK_Dept;
                return s;
            }
        }
        public string FK_Station
        {
            get
            {
                return this.Request.QueryString["FK_Station"];
            }
        }
        public string WorkIDs
        {
            get
            {
                return this.Request.QueryString["WorkIDs"];
            }
        }
        public string DoFunc
        {
            get
            {
                return this.Request.QueryString["DoFunc"];
            }
        }
        public string CFlowNo
        {
            get
            {
                return this.Request.QueryString["CFlowNo"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }

        //private bool IsMultiple = false;
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(this.Request[param], System.Text.Encoding.UTF8);
        }
        #endregion 属性.

        public DataTable GetTable()
        {
            if (this.ToNode == 0)
                throw new Exception("@流程设计错误，没有转向的节点。举例说明: 当前是A节点。如果您在A点的属性里启用了[接受人]按钮，那么他的转向节点集合中(就是A可以转到的节点集合比如:A到B，A到C, 那么B,C节点就是转向节点集合)，必须有一个节点是的节点属性的[访问规则]设置为[由上一步发送人员选择]");

            NodeStations stas = new NodeStations(this.ToNode);
            if (stas.Count == 0)
            {
                BP.WF.Node toNd = new BP.WF.Node(this.ToNode);
                throw new Exception("@流程设计错误：设计员没有设计节点[" + toNd.Name + "]，接受人的岗位范围。");
            }

            string BindByStationSql = "";
            string DdlEmpSql = "";
            string ParSql = "select No from Port_Dept where ParentNo='0'";
            DataTable ParDt = DBAccess.RunSQLReturnTable(ParSql);

            BindByStationSql = string.Format("select No,Name,ParentNo,'1' IsParent from Port_Dept where ParentNo='0' union" +
                                               " select No,Name,b.FK_Station as ParentNo,'0' IsParent  from Port_Emp a inner" +
                                               " join " + BP.WF.Glo.EmpStation + " b on a.No=b.FK_Emp and b.FK_Station in" +
                                               " (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')  WHERE No in" +
                                               "  (SELECT FK_EMP FROM " + BP.WF.Glo.EmpStation + " " +
                                               " WHERE FK_STATION IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}'))" +
                                               " AND No IN (SELECT FK_Emp FROM Port_EmpDept) " +
                                               " union select No,Name,'{2}' ParentNo,'1' IsParent  from Port_Station where no " +
                                               "in(SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')", ToNode, WebUser.FK_Dept, ParDt.Rows[0][0].ToString());

            DdlEmpSql = string.Format("select No,Name,b.FK_Station as ParentNo,'0' IsParent  from Port_Emp a inner" +
                                               " join " + BP.WF.Glo.EmpStation + " b on a.No=b.FK_Emp and b.FK_Station in" +
                                               " (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}')  WHERE No in" +
                                               "  (SELECT FK_EMP FROM " + BP.WF.Glo.EmpStation + " " +
                                               " WHERE FK_STATION IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node='{0}'))" +
                                               " AND No IN (SELECT FK_Emp FROM Port_EmpDept) ", ToNode, WebUser.FK_Dept, ParDt.Rows[0][0].ToString());


            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                BindByStationSql = BindByStationSql.Replace("Port_EmpDept", "Port_DeptEmp");
                DdlEmpSql = DdlEmpSql.Replace("Port_EmpDept", "Port_DeptEmp");
            }
            else
            {
                BindByStationSql = BindByStationSql.Replace("Port_DeptEmp", "Port_EmpDept");
                DdlEmpSql = DdlEmpSql.Replace("Port_DeptEmp", "Port_EmpDept");

                BindByStationSql = BindByStationSql.Replace("Port_DeptEmpStation", "Port_EmpSTATION");
                DdlEmpSql = DdlEmpSql.Replace("Port_DeptEmpStation", "Port_EmpSTATION");
            }

            DdlEmpDt = DBAccess.RunSQLReturnTable(DdlEmpSql);
            return DBAccess.RunSQLReturnTable(BindByStationSql);
        }
        private BP.WF.Node _HisNode = null;
        /// <summary>
        /// 它的节点
        /// </summary>
        public BP.WF.Node HisNode
        {
            get
            {
                if (_HisNode == null)
                    _HisNode = new BP.WF.Node(this.FK_Node);
                return _HisNode;
            }
        }
        /// <summary>
        /// 是否多分支
        /// </summary>
        public bool IsMFZ
        {
            get
            {
                Nodes nds = this.HisNode.HisToNodes;
                int num = 0;
                foreach (BP.WF.Node mynd in nds)
                {
                    #region 过滤不能到达的节点.
                    Cond cond = new Cond();
                    int i = cond.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                    if (i == 0)
                        continue; // 没有设置方向条件，就让它跳过去。
                    cond.WorkID = this.WorkID;
                    cond.en = geRpt;

                    if (cond.IsPassed == false)
                        continue;
                    #endregion 过滤不能到达的节点.

                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        num++;
                    }
                }
                if (num == 0)
                    return false;
                if (num == 1)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 绑定多分支
        /// </summary>
        public void BindMStations()
        {

            this.BindByStation();

            Nodes mynds = this.HisNode.HisToNodes;
            this.Left.Add("<fieldset><legend>&nbsp;选择方向:列出所选方向设置的人员&nbsp;</legend>");
            string str = "<p>";
            foreach (BP.WF.Node mynd in mynds)
            {
                if (mynd.HisDeliveryWay != DeliveryWay.BySelected)
                    continue;

                #region 过滤不能到达的节点.
                Cond cond = new Cond();
                int i = cond.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                if (i == 0)
                    continue; // 没有设置方向条件，就让它跳过去。

                cond.WorkID = this.WorkID;
                cond.en = geRpt;
                if (cond.IsPassed == false)
                    continue;
                #endregion 过滤不能到达的节点.

                if (this.ToNode == mynd.NodeID)
                    str += "&nbsp;&nbsp;<b class='l-link'><font color='red' >" + mynd.Name + "</font></b>";
                else
                    str += "&nbsp;&nbsp;<b><a class='l-link' href='Accepter.aspx?FK_Node=" + this.FK_Node + "&type=1&ToNode=" + mynd.NodeID + "&WorkID=" + this.WorkID + "&WorkIDs=" + this.WorkIDs + "' >" + mynd.Name + "</a></b>";
            }
            this.Left.Add(str + "</p>");
            this.Left.AddFieldSetEnd();
        }

        public Selector MySelector = null;
        public GERpt _wk = null;
        public GERpt geRpt
        {
            get
            {
                if (_wk == null)
                {
                    _wk = this.HisNode.HisFlow.HisGERpt;
                    _wk.OID = this.WorkID;
                    int i = _wk.RetrieveFromDBSources();
                    if (i == 0)
                    {
                        _wk.OID = this.FID;
                        _wk.RetrieveFromDBSources();
                    }
                    _wk.ResetDefaultVal();
                }
                return _wk;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Pub1.Clear();
            this.Title = "选择下一步骤接受的人员";

            //判断是否需要转向。
            if (this.ToNode == 0)
            {
                int num = 0;
                int tempToNodeID = 0;

                /*如果到达的点为空 */
                /*首先判断当前节点的ID，是否配置到了其他节点里面，
                 * * 如果有则需要转向高级的选择框中去，当前界面不能满足公文类的选择人需求。*/
                string sql = "SELECT COUNT(*) FROM WF_Node WHERE FK_Flow='" + this.HisNode.FK_Flow + "' AND " + NodeAttr.DeliveryWay + "=" + (int)DeliveryWay.BySelected + " AND " + NodeAttr.DeliveryParas + " LIKE '%" + this.HisNode.NodeID + "%' ";

                if (DBAccess.RunSQLReturnValInt(sql, 0) > 0)
                {
                    /*说明以后的几个节点人员处理的选择 */
                    string url = "AccepterAdv.aspx?1=3" + this.RequestParas;
                    this.Response.Redirect(url, true);
                    return;
                }

                Nodes nds = this.HisNode.HisToNodes;
                if (nds.Count == 0)
                {
                    this.Pub1.AddFieldSetRed("提示", "当前点是最后的一个节点，不能使用此功能。");
                    return;
                }
                else if (nds.Count == 1)
                {
                    BP.WF.Node toND = nds[0] as BP.WF.Node;
                    tempToNodeID = toND.NodeID;
                }
                else
                {
                    BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
                    foreach (BP.WF.Node mynd in nds)
                    {
                        if (mynd.HisDeliveryWay != DeliveryWay.BySelected)
                            continue;

                        #region 过滤不能到达的节点.
                        if (nd.CondModel == CondModel.ByLineCond)
                        {
                            Cond cond = new Cond();
                            int i = cond.Retrieve(CondAttr.FK_Node, this.HisNode.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                            if (i == 0)
                                continue; // 没有设置方向条件，就让它跳过去。
                            cond.WorkID = this.WorkID;
                            cond.en = geRpt;
                            if (cond.IsPassed == false)
                                continue;
                        }
                        #endregion 过滤不能到达的节点.
                        tempToNodeID = mynd.NodeID;
                        num++;
                    }
                }

                if (tempToNodeID == 0)
                {
                    this.WinCloseWithMsg("@流程设计错误：\n\n 当前节点的所有分支节点没有一个接受人员规则为按照选择接受。");
                    return;
                }


                this.Response.Redirect("Accepter.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&ToNode=" + tempToNodeID + "&FID=" + this.FID + "&type=1&WorkID=" + this.WorkID + "&WorkIDs=" + this.WorkIDs + "&IsWinOpen=" + this.IsWinOpen, true);
                return;
            }


            try
            {
                /* 首先判断是否有多个分支的情况。*/
                if (ToNode == 0 && this.IsMFZ)
                {
                    //IsMultiple = true;
                    //this.BindMStations();
                    return;
                }
                MySelector = new Selector(this.ToNode);
                switch (MySelector.SelectorModel)
                {
                    case SelectorModel.Station:
                        returnValue("BindByStation");
                        break;
                    case SelectorModel.SQL:
                        returnValue("BindBySQL");
                        break;
                    case SelectorModel.Dept:
                        returnValue("BindByDept");
                        break;
                    case SelectorModel.Emp:
                        returnValue("BindByEmp");
                        break;
                    case SelectorModel.Url:
                        if (MySelector.SelectorP1.Contains("?"))
                            this.Response.Redirect(MySelector.SelectorP1 + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                        else
                            this.Response.Redirect(MySelector.SelectorP1 + "?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node, true);
                        return;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Pub1.Clear();
                this.Pub1.AddMsgOfWarning("错误", ex.Message);
            }
        }
        /// <summary>
        /// 按sql方式
        /// </summary>
        public string BindBySQL()
        {
            string sqlGroup = MySelector.SelectorP1;
            sqlGroup = sqlGroup.Replace("@WebUser.No", WebUser.No);
            sqlGroup = sqlGroup.Replace("@WebUser.Name", WebUser.Name);
            sqlGroup = sqlGroup.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            string sqlDB = MySelector.SelectorP2;
            sqlDB = sqlDB.Replace("@WebUser.No", WebUser.No);
            sqlDB = sqlDB.Replace("@WebUser.Name", WebUser.Name);
            sqlDB = sqlDB.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            return GetTreeJsonByTable(replaceRepeatEmps(sqlGroup, sqlDB), "NO", "NAME", "ParentNo", "0", "IsParent", "");
        }
        /// <summary>
        /// 剔除重复的人员编号
        /// </summary>
        /// <param name="SelectorP_1">参数一</param>
        /// <param name="SelectorP_2">参数二</param>
        /// <returns>DataTable</returns>
        public DataTable replaceRepeatEmps(string SelectorP_1, string SelectorP_2)
        {
            string sql = null;
            DataTable dt = new DataTable();

            if (!string.IsNullOrEmpty(SelectorP_1) && !string.IsNullOrEmpty(SelectorP_2))
            {
                sql = "SELECT NO,NAME,FK_DEPT ParentNo,'0' IsParent  FROM (" + SelectorP_1 + ") A";
                dt = DBAccess.RunSQLReturnTable(sql);

                sql = "SELECT NO,NAME,FK_DEPT ParentNo,'0' IsParent  FROM (" + SelectorP_2 + ") A";
                dt.Merge(DBAccess.RunSQLReturnTable(sql));
            }

            if (string.IsNullOrEmpty(SelectorP_1) || string.IsNullOrEmpty(SelectorP_2))
            {
                if (string.IsNullOrEmpty(SelectorP_1) && string.IsNullOrEmpty(SelectorP_2))
                {
                    throw new Exception("参数1和参数2均为空！");
                }
                if (!string.IsNullOrEmpty(SelectorP_1))
                {
                    sql = "SELECT NO,NAME,FK_DEPT ParentNo,'0' IsParent  FROM (" + SelectorP_1 + ") A";
                    dt = DBAccess.RunSQLReturnTable(sql);
                }
                if (!string.IsNullOrEmpty(SelectorP_2))
                {
                    sql = "SELECT NO,NAME,FK_DEPT ParentNo,'0' IsParent  FROM (" + SelectorP_2 + ") A";
                    dt = DBAccess.RunSQLReturnTable(sql);
                }
            }

            DataView dv = new DataView(dt);
            dt = dv.ToTable(true);//去除重复的emp

            DdlEmpDt = dt;//给下拉框表赋值

            string fk_dept = null;
            foreach (DataRow dr in dt.Rows)
            {
                if (string.IsNullOrEmpty(dr["ParentNo"].ToString()))
                    continue;

                fk_dept += dr["ParentNo"] + ",";
            }

            fk_dept = fk_dept.Substring(0, fk_dept.Length - 1);

            sql = "SELECT NO,NAME, ParentNo,'1' IsParent FROM PORT_DEPT WHERE  ParentNo=0";
            DataTable rootNodeDt = DBAccess.RunSQLReturnTable(sql);
            dt.Merge(rootNodeDt);

            sql = "SELECT NO,NAME,'" + rootNodeDt.Rows[0]["NO"] + "' ParentNo,'1' IsParent FROM PORT_DEPT WHERE NO IN (" +
                fk_dept + ") AND ParentNo!=0";
            dt.Merge(DBAccess.RunSQLReturnTable(sql));

            return dt;
        }
        /// <summary>
        /// 按BindByEmp 方式
        /// </summary>
        public string BindByEmp()
        {

            string BindByEmpSql = string.Format("select No,Name,ParentNo,'1' IsParent  from Port_Dept  WHERE No IN (SELECT FK_Dept FROM " +
                                              "Port_Emp WHERE No in(SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node={0})) or ParentNo=0 union " +
                                              "select No,Name,FK_Dept as ParentNo,'0' IsParent  from Port_Emp  WHERE No in (SELECT FK_EMP " +
                                              "FROM WF_NodeEmp WHERE FK_Node={0})", MySelector.NodeID);
            DdlEmpDt = DBAccess.RunSQLReturnTable(string.Format("select No,Name from Port_Emp  WHERE No in (SELECT FK_EMP " +
                                              "FROM WF_NodeEmp WHERE FK_Node={0})", MySelector.NodeID));

            DataTable BindByEmpDt = DBAccess.RunSQLReturnTable(BindByEmpSql);
            DataTable ParDt = DBAccess.RunSQLReturnTable("select No from Port_Dept where ParentNo='0'");
            foreach (DataRow r in BindByEmpDt.Rows)
            {
                if (r["IsParent"].ToString() == "1" && r["ParentNo"].ToString() != "0")
                {
                    r["ParentNo"] = ParDt.Rows[0][0].ToString();
                }
            }
            return GetTreeJsonByTable(BindByEmpDt, "NO", "NAME", "ParentNo", "0", "IsParent", "");
        }
        public DataTable DdlEmpDt;
        /// <summary>
        /// 返回值
        /// </summary>
        private void returnValue(string whichMet)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;

            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getTreeDateMet"://获取数据
                    s_responsetext = getTreeDateMet(whichMet);
                    break;
                case "saveMet":
                    saveMet();
                    break;
                case "copyToMet":
                    copyToMet();
                    break;
            }

            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            s_responsetext = AppendJson(s_responsetext);
            s_responsetext = DdlValue(s_responsetext);
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        public string AppendJson(string json)
        {
            StringBuilder AppendJson = new StringBuilder();
            AppendJson.Append(json);
            AppendJson.Append(",CheId:");
            string alreadyHadEmps = string.Format("select No, Name from Port_Emp where No in( select FK_Emp from WF_SelectAccper " +
                                                "where FK_Node={0} and WorkID={1})", this.ToNode, this.WorkID);
            DataTable dt = DBAccess.RunSQLReturnTable(alreadyHadEmps);
            AppendJson.Append("[{\"id\":\"CheId\",\"iconCls\":\"icon-groupadd\",\"text\":\"已选接受人员\",\"children\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AppendJson.Append("{\"id\":\"" + dt.Rows[i][0].ToString() + "\",iconCls:\"icon-user\"" + ",\"text\":\"" + dt.Rows[i][1].ToString() + "\"");
                if (i == dt.Rows.Count - 1)
                {
                    AppendJson.Append("}");
                    break;
                }
                AppendJson.Append("},");
            }

            AppendJson.Append("]}]");

            AppendJson.Insert(0, "{tt:");
            return AppendJson.ToString();
        }
        public string DdlValue(string StrJson)
        {
            StringBuilder SBuilder = new StringBuilder();
            SBuilder.Append(StrJson);
            DataTable dt = DdlEmpDt;

            try
            {
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    if (dt.Rows[i]["IsParent"].ToString() == "1")
                        dt.Rows.RemoveAt(i);
                }
            }
            catch (Exception)
            {
            }



            SBuilder.Append(",ddl:[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    SBuilder.Append("{\"id\":\"" + dt.Rows[i]["No"].ToString() + "\",\"text\":\"" + dt.Rows[i]["Name"].ToString() + "\",\"selected\":\"selected\"}");
                }
                else
                {
                    SBuilder.Append("{\"id\":\"" + dt.Rows[i]["No"].ToString() + "\",\"text\":\"" + dt.Rows[i]["Name"].ToString() + "\"}");
                }
                if (i == dt.Rows.Count - 1)
                {
                    SBuilder.Append("");
                    continue;
                }
                SBuilder.Append(",");
            }
            SBuilder.Append("]}");
            return SBuilder.ToString();
        }
        public string getTreeDateMet(string Met)
        {
            switch (Met)
            {
                case "BindByEmp":
                    return BindByEmp();
                case "BindByDept":
                    return BindByDept();
                case "BindByStation":
                    return BindByStation();
                case "BindBySQL":
                    return BindBySQL();
                default:
                    return "";
            }
        }
        public string BindByDept()
        {
            string BindByDeptSql = string.Format("SELECT  No,Name,ParentNo,'1' IsParent  FROM Port_Dept WHERE No IN (SELECT " +
                                                 "FK_Dept FROM WF_NodeDept WHERE FK_Node={0}) or ParentNo=0 union SELECT No,Name,FK_Dept " +
                                                 "as ParentNo,'0' IsParent FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node={0})", MySelector.NodeID);

            DdlEmpDt = DBAccess.RunSQLReturnTable(string.Format("SELECT No,Name FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node={0})", MySelector.NodeID));

            DataTable BindByDeptDt = DBAccess.RunSQLReturnTable(BindByDeptSql);
            DataTable ParDt = DBAccess.RunSQLReturnTable("select No from Port_Dept where ParentNo='0'");
            foreach (DataRow r in BindByDeptDt.Rows)
            {
                if (r["IsParent"].ToString() == "1" && r["ParentNo"].ToString() != "0")
                {
                    r["ParentNo"] = ParDt.Rows[0][0].ToString();
                }
            }
            return GetTreeJsonByTable(BindByDeptDt, "NO", "NAME", "ParentNo", "0", "IsParent", "");
        }
        /// <summary>
        /// 按table方式.
        /// </summary>
        public void BindBySQL_Table(DataTable dtGroup, DataTable dtObj)
        {
            int col = 4;
            this.Pub1.AddTable("style='border:0px;width:100%'");
            foreach (DataRow drGroup in dtGroup.Rows)
            {
                string ctlIDs = "";
                string groupNo = drGroup[0].ToString();

                //增加全部选择.
                this.Pub1.AddTR();
                CheckBox cbx = new CheckBox();
                cbx.ID = "CBs_" + drGroup[0].ToString();
                cbx.Text = drGroup[1].ToString();
                this.Pub1.AddTDTitle("align=left", cbx);
                this.Pub1.AddTREnd();

                this.Pub1.AddTR();
                this.Pub1.AddTDBegin("nowarp=false");

                this.Pub1.AddTable("style='border:0px;width:100%'");
                int colIdx = -1;
                foreach (DataRow drObj in dtObj.Rows)
                {
                    string no = drObj[0].ToString();
                    string name = drObj[1].ToString();
                    string group = drObj[2].ToString();
                    if (group.Trim() != groupNo.Trim())
                        continue;

                    colIdx++;
                    if (colIdx == 0)
                        this.Pub1.AddTR();

                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + no;
                    ctlIDs += cb.ID + ",";
                    cb.Attributes["onclick"] = "isChange=true;";
                    cb.Text = name;
                    cb.Checked = false;
                    if (cb.Checked)
                        cb.Text = "<font color=green>" + cb.Text + "</font>";
                    this.Pub1.AddTD(cb);
                    if (col - 1 == colIdx)
                    {
                        this.Pub1.AddTREnd();
                        colIdx = -1;
                    }
                }
                cbx.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";

                if (colIdx != -1)
                {
                    while (colIdx != col - 1)
                    {
                        colIdx++;
                        this.Pub1.AddTD();
                    }
                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();

            this.BindEnd();
        }

        public string BindByStation()
        {
            return GetTreeJsonByTable(this.GetTable(), "No", "Name", "ParentNo", "0", "IsParent", "");
        }
        /// <summary>
        /// 处理绑定结束
        /// </summary>
        public void BindEnd()
        {
            Button btn = new Button();
            if (this.IsWinOpen == 1)
            {
                btn.Text = "确定并关闭";
                btn.ID = "Btn_Save";
                btn.CssClass = "Btn";
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);
            }
            else
            {
                btn = new Button();
                btn.Text = "确定并发送";
                btn.ID = "Btn_Save";
                btn.CssClass = "Btn";
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);

                btn = new Button();
                btn.Text = "取消并返回";
                btn.ID = "Btn_Cancel";
                btn.CssClass = "Btn";
                btn.Click += new EventHandler(btn_Save_Click);
                this.Pub1.Add(btn);
            }

            CheckBox mycb = new CheckBox();
            mycb.ID = "CB_IsSetNextTime";
            mycb.Text = "以后发送都按照本次设置计算";
            this.Pub1.Add(mycb);

            //CheckBox mycb = new CheckBox();
            //mycb.ID = "CB_IsSetNextTime";
            //mycb.Text = "以后发送都按照本次设置计算";
            //mycb.Checked = accps.IsSetNextTime;
            //this.Pub1.Add(mycb);

        }
        //保存
        public void saveMet()
        {
            string getSaveNo = getUTF8ToString("getSaveNo");

            //此处做判断,删除checked的部门数据
            string[] getSaveNoArray = getSaveNo.Split(',');
            List<string> getSaveNoList = new List<string>();

            for (int i = 0; i < getSaveNoArray.Length; i++)
            {
                getSaveNoList.Add(getSaveNoArray[i]);
            }

            getSaveNo = null;
            string ziFu = ",";
            for (int i = 0; i < getSaveNoList.Count; i++)
            {
                if (i == getSaveNoList.Count - 1)
                {
                    ziFu = null;
                }
                getSaveNo += (getSaveNoList[i] + ziFu);
            }

            //设置人员. --------循环添加WorkIDs的优先级比WorkID高，只可以判断一个

            string WorkIDs = this.WorkIDs;
            if (!string.IsNullOrEmpty(WorkIDs))
            {
                string[] ids = WorkIDs.Split(',');
                foreach (string id in ids)
                {
                    if (string.IsNullOrEmpty(id) == true)
                        continue;
                    BP.WF.Dev2Interface.WorkOpt_SetAccepter(this.ToNode, long.Parse(id), this.FID, getSaveNo, false);
                }
            }
            else
            {
                BP.WF.Dev2Interface.WorkOpt_SetAccepter(this.ToNode, this.WorkID, this.FID, getSaveNo, false);
            }


            if (this.IsWinOpen == 0)
            {
                /*如果是 MyFlow.aspx 调用的, 就要调用发送逻辑. */
                //this.DoSend();
                return;
            }

            if (this.Request.QueryString["IsEUI"] == null)
            {
                this.WinClose();
            }
            else
            {
                PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");

            }

            //#warning 刘文辉 保存收件人后调用发送按钮

            //            BtnLab nd = new BtnLab(this.FK_Node);
            //            if (nd.SelectAccepterEnable == 1)
            //            {
            //                if (this.Request.QueryString["IsEUI"] == null)
            //                {

            //                    /*如果是1不说明直接关闭它.*/
            //                    this.WinClose();
            //                    //ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "this.close();", true);
            //                }
            //                else
            //                {
            //                    PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");

            //                }
            //            }
            //            else
            //            {
            //                ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "send();", true);
            //            }
        }
        /// <summary>
        /// 抄送
        /// </summary>
        public void copyToMet()
        {
            string getSaveNo = getUTF8ToString("getSaveNo");
            string cs_titleV = getUTF8ToString("cs_titleV");
            string cs_messageV = getUTF8ToString("cs_messageV");

            string[] emps = getSaveNo.Split(',');
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            BP.Port.Emp myemp = new BP.Port.Emp();


            foreach (string emp in emps)
            {
                if (string.IsNullOrEmpty(emp))
                    continue;

                myemp.No = emp;
                myemp.Retrieve();
                switch (nd.CCWriteTo)
                {
                    case BP.WF.CCWriteTo.All:
                        BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, this.FK_Node, this.WorkID, emp, myemp.Name, cs_titleV, cs_messageV);
                        BP.WF.Dev2Interface.Node_CC_WriteTo_Todolist(this.FK_Node, this.FK_Node, this.WorkID, emp, myemp.Name);
                        break;
                    case BP.WF.CCWriteTo.CCList:
                        BP.WF.Dev2Interface.Node_CC_WriteTo_CClist(this.FK_Node, this.FK_Node, this.WorkID, emp, myemp.Name, cs_titleV, cs_messageV);
                        break;
                    case BP.WF.CCWriteTo.Todolist:
                        BP.WF.Dev2Interface.Node_CC_WriteTo_Todolist(this.FK_Node, this.FK_Node, this.WorkID, emp, myemp.Name);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Save_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.ID == "Btn_Cancel")
            {
                string url = "../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                this.Response.Redirect(url, true);
                return;
            }

            //DataTable dt = this.GetTable();
            string emps = "";
            foreach (Control ctl in this.Pub1.Controls)
            {
                CheckBox cb = ctl as CheckBox;
                if (cb == null || cb.ID == null || cb.ID.Contains("CBs_") || cb.ID == "CB_IsSetNextTime")
                    continue;

                if (cb.Checked == false)
                    continue;
                emps += cb.ID.Replace("CB_", "") + ",";
            }

            if (emps.Length < 2)
            {
                this.Alert("您没有选择人员。");
                return;
            }

            //获取是否下次自动设置.
            bool isNextTime = this.Pub1.GetCBByID("CB_IsSetNextTime").Checked;

            //设置人员.
            BP.WF.Dev2Interface.WorkOpt_SetAccepter(this.ToNode, this.WorkID, this.FID, emps, isNextTime);

            if (this.IsWinOpen == 0)
            {
                /*如果是 MyFlow.aspx 调用的, 就要调用发送逻辑. */
                this.DoSend();
                return;
            }

            if (this.Request.QueryString["IsEUI"] == null)
            {
                this.WinClose("ok");
            }
            else
            {
                PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");
            }

#warning 刘文辉 保存收件人后调用发送按钮

            BtnLab nd = new BtnLab(this.FK_Node);
            if (nd.SelectAccepterEnable == 1)
            {
                if (this.Request.QueryString["IsEUI"] == null)
                {
                    /*如果是1不说明直接关闭它.*/
                    this.WinClose("ok");
                    //ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "this.close();", true);
                }
                else
                {
                    PubClass.ResponseWriteScript("window.parent.$('windowIfrem').window('close');");
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "", "send();", true);
            }
        }

        public void DoSend()
        {
            // 以下代码是从 MyFlow.aspx Send 方法copy 过来的，需要保持业务逻辑的一致性，所以代码需要保持一致.

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();

            WorkNode firstwn = new WorkNode(wk, nd);
            string msg = "";
            try
            {
                msg = firstwn.NodeSend().ToMsgOfHtml();
            }
            catch (Exception exSend)
            {
                this.Pub1.AddFieldSetGreen("错误");
                this.Pub1.Add(exSend.Message.Replace("@@", "@").Replace("@", "<BR>@"));
                this.Pub1.AddFieldSetEnd();
                return;
            }

            #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
            try
            {
                //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                BP.WF.Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs);
            }
            catch (Exception ex)
            {
                this.ToMsg(msg, ex.Message);
                return;
            }
            #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.


            /*处理转向问题.*/
            switch (firstwn.HisNode.HisTurnToDeal)
            {
                case TurnToDeal.SpecUrl:
                    string myurl = firstwn.HisNode.TurnToDealDoc.Clone().ToString();
                    if (myurl.Contains("?") == false)
                        myurl += "?1=1";
                    Attrs myattrs = firstwn.HisWork.EnMap.Attrs;
                    Work hisWK = firstwn.HisWork;
                    foreach (Attr attr in myattrs)
                    {
                        if (myurl.Contains("@") == false)
                            break;
                        myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                    }
                    if (myurl.Contains("@"))
                        throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);

                    myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                    this.Response.Redirect(myurl, true);
                    return;
                case TurnToDeal.TurnToByCond:
                    TurnTos tts = new TurnTos(this.FK_Flow);
                    if (tts.Count == 0)
                        throw new Exception("@您没有设置节点完成后的转向条件。");
                    foreach (TurnTo tt in tts)
                    {
                        tt.HisWork = firstwn.HisWork;
                        if (tt.IsPassed == true)
                        {
                            string url = tt.TurnToURL.Clone().ToString();
                            if (url.Contains("?s") == false)
                                url += "?1=1";
                            Attrs attrs = firstwn.HisWork.EnMap.Attrs;
                            Work hisWK1 = firstwn.HisWork;
                            foreach (Attr attr in attrs)
                            {
                                if (url.Contains("@") == false)
                                    break;
                                url = url.Replace("@" + attr.Key, hisWK1.GetValStrByKey(attr.Key));
                            }
                            if (url.Contains("@"))
                                throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + url);

                            url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                            this.Response.Redirect(url, true);
                            return;
                        }
                    }
#warning 为上海修改了如果找不到路径就让它按系统的信息提示。
                    this.ToMsg(msg, "info");
                    //throw new Exception("您定义的转向条件不成立，没有出口。");
                    break;
                default:
                    this.ToMsg(msg, "info");
                    break;
            }
            return;
        }

        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;

            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("./../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
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
        public string GetTreeJsonByTable(DataTable tabel, string idCol, string txtCol, string rela, object pId, string IsParent, string CheckedString)
        {
            string treeJson = string.Empty;
            treeResult.Append(treesb.ToString());

            treesb.Clear();
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
                DataRow[] rows = tabel.Select(filer);
                if (rows.Length > 0)//修改
                {
                    foreach (DataRow row in rows)
                    {
                        string deptNo = row[idCol].ToString();

                        if (treeResult.Length == 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                    + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                    + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                    + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                                + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower() + ",\"state\":\"open\"");
                        }
                        else
                        {
                            treesb.Append("{\"id\":\"" + row[idCol]
                                + "\",\"text\":\"" + row[txtCol]
                                    + "\",\"attributes\":{\"IsParent\":\"" + row[IsParent] + "\"}"
                              + ",\"checked\":" + CheckedString.Contains("," + row[idCol] + ",").ToString().ToLower());
                        }


                        if (tabel.Select(string.Format("{0}='{1}'", rela, row[idCol])).Length > 0)
                        {
                            treesb.Append(",\"children\":");
                            if (row[idCol].ToString() == row[rela].ToString())
                                continue;

                            GetTreeJsonByTable(tabel, idCol, txtCol, rela, row[idCol], IsParent, CheckedString);
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
    }
}