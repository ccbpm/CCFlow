using System;
using System.Data;
using BP.En;
using BP.DA;
using BP.Port;
using BP.WF.Template;
using BP.Difference;

namespace BP.WF
{
    /// <summary>
    /// 抄送工作逻辑
    /// </summary>
    public class WorkCC
    {
        #region 身份.
        private WebUserCopy _webUserCopy = null;
        public WebUserCopy WebUser
        {
            get
            {
                if (_webUserCopy == null)
                {
                    _webUserCopy = new WebUserCopy();
                    _webUserCopy.LoadWebUser();
                }
                return _webUserCopy;
            }
            set
            {
                _webUserCopy = value;
            }
        }
        #endregion 身份.

        public WorkNode currWN = null;
        public Int64 WorkID = 0;
        /// <summary>
        /// 构造WorkCC
        /// </summary>
        /// <param name="wn"></param>
        /// <param name="ndFrom"></param>
        /// <param name="webUser"></param>
        public WorkCC(WorkNode userWN, WebUserCopy webUser)
        {
            this.currWN = userWN;
            this.WorkID = this.currWN.WorkID;
            this.WebUser = webUser;
        }
        /// <summary>
        /// 执行抄送.
        /// </summary>
        public void DoCC(string type)
        {
            //查询出来到达的抄送节点.
            Directions dirs = new Directions();
            dirs.Retrieve(DirectionAttr.Node, currWN.HisNode.NodeID, DirectionAttr.NodeType, (int)NodeType.CCNode, DirectionAttr.Idx);
            if (dirs.Count == 0)
                return; //如果没有抄送节点，就不处理.

            string empNames = "";
            //定义容器集合,获得可以抄送的节点.
            Nodes ccNodes = new Nodes();
            foreach (Direction dir in dirs)
            {
                Conds conds = new Conds();
                int i = conds.Retrieve(CondAttr.FK_Node, currWN.HisNode.NodeID,
                    CondAttr.ToNodeID, dir.ToNode, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);

                //判断是否通过.
                if (i == 0 || conds.GenerResult(currWN.rptGe, this.WebUser) == true)
                {
                    Node ccNode = new Node();
                    ccNode.NodeID = dir.ToNode;
                    ccNode.Retrieve();
                    ccNodes.AddEntity(ccNode); //加入到集合.
                    continue;
                }
            }
            if (ccNodes.Count == 0)
                return;

            //定义容器:需要多个规则把他们合并.
            DataTable dtCCers = new DataTable();
            dtCCers.Columns.Add("EmpNo");
            dtCCers.Columns.Add("EmpName");
            dtCCers.Columns.Add("CCNodeID");
            dtCCers.Columns.Add("CCNodeName");
            dtCCers.Columns.Add("NodeID"); //抄送节点的ID.
            dtCCers.Columns.Add("InEmpWorks");
            //执行cc
            foreach (Node myCCNode in ccNodes)
            {
                //获得抄送规则集合.
                CCRoles rols = new CCRoles();
                rols.Retrieve(CCRoleAttr.NodeID, myCCNode.NodeID, "Idx");

                string emps = "";
                string cctoEmps = "";
                //遍历岗位规则集合.
                foreach (CCRole rol in rols)
                {
                    //获得抄送人的集合.
                    DataTable dt = GenerCCers(rol, this.currWN.rptGe, this.WorkID);

                    //把数据放进去.
                    foreach (DataRow dr in dt.Rows)
                    {
                        string empNo = dr[0].ToString();
                        if (DataType.IsNullOrEmpty(empNo) == true)
                            continue;

                        if (emps.Contains(empNo + ",") == false)
                        {
                            emps += empNo + ",";
                            //把他加入到集合里.
                            DataRow mydr = dtCCers.NewRow();
                            mydr[0] = empNo; //EmpNo
                            if (dt.Columns.Count == 2)
                            {
                                mydr[1] = dr[1].ToString(); //EmpName
                                empNames += mydr[1] + ",";
                                cctoEmps += mydr[1] + ",";
                            }
                               

                            //节点ID.
                            mydr[2] = rol.NodeID; //抄送的节点.
                            mydr[3] = myCCNode.Name; //抄送节点的ID.
                            mydr[4] = myCCNode.NodeID; //抄送节点的ID.
                            mydr[5] = myCCNode.CCWriteTo == CCWriteTo.CCList ? 0 : 1;
                            dtCCers.Rows.Add(mydr);
                        }
                    }
                }
                if (type.Equals("WorkNode"))
                {
                    Glo.AddToTrack(ActionType.CC, currWN.HisNode.FlowNo, this.WorkID, 0, currWN.HisNode.NodeID, currWN.HisNode.Name, WebUser.No, WebUser.Name, myCCNode.NodeID, myCCNode.Name, emps, cctoEmps, "来自" + WebUser.Name + "的抄送", null);
                }
            }

            //写入数据：dtCCers
            Int64 fid = this.currWN.HisGenerWorkFlow.FID;
            string flowNo = this.currWN.HisGenerWorkFlow.FlowNo;
            string flowName = this.currWN.HisGenerWorkFlow.FlowName;

            string title = this.currWN.HisGenerWorkFlow.Title;
            Int64 workID = this.currWN.WorkID;
            int nodeIDMain = this.currWN.HisNode.NodeID; //节点ID.
            string nodeMainName = this.currWN.HisNode.Name; //节点ID.
            string sqls = "";

            DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workID + " AND NodeIDWork="+ nodeIDMain);

            foreach (DataRow mydr in dtCCers.Rows)
            {
                string empNo = mydr[0].ToString();
                string nodeID = mydr[4].ToString(); //抄送节点.
                string nodeName = mydr[3].ToString();
                int InEmpWorks = mydr[5] == null ? 0 : int.Parse(mydr[5].ToString());
                //string nodeName = mydr[3].ToString();
                string mypk = workID + "_" + nodeIDMain + "_" + empNo;
                //判断MyPK是否已经存在....
                if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_CCList WHERE MyPK = '" + mypk + "'") > 0)
                    continue;

                sqls += "@ INSERT INTO WF_CCList (MyPK,WorkID,CCTo,NodeIDWork,FlowNo,Title,RDT,NodeIDCC,NodeIDCCName,InEmpWorks) ";
                sqls += " VALUES ('" + mypk + "'," + workID + ",'" + empNo + "'," + nodeIDMain + ",'" + flowNo + "','" + title + "','" + DataType.CurrentDateTime + "',"+ nodeID+",'"+ nodeName + "'," + InEmpWorks + ")"; ;
            }
            sqls += "";
            DBAccess.RunSQLs(sqls);
            if (type.Equals("FullSA"))
            {
                string sql = "UPDATE WF_CCList SET Sta=-1, RecEmpNo='" + WebUser.No + "',RecEmpName='" + WebUser.Name + "',NodeName='" + nodeMainName + "',FlowName='" + flowName + "' WHERE WorkID=" + this.WorkID + " AND NodeIDWork=" + nodeIDMain;
                DBAccess.RunSQL(sql);
            }
            if (type.Equals("WorkNode"))
            {
                string sql = "UPDATE WF_CCList SET Sta=0, RecEmpNo='" + WebUser.No + "',RecEmpName='" + WebUser.Name + "',NodeName='" + nodeMainName + "',FlowName='" + flowName + "' WHERE WorkID=" + this.WorkID + " AND NodeIDWork=" + nodeIDMain;
                DBAccess.RunSQL(sql);
            }
            CCList en = new CCList();
            //en.CheckPhysicsTable();

            // 更新其他字段 DeptName, 等字段》
            string updateSQL = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.PostgreSQL:
                case DBType.HGDB:
                    updateSQL = " UPDATE WF_CCList cc SET cc.CCToName=emp.Name,cc.DeptNo = emp.FK_Dept,cc.DeptName =dept.Name  FROM Port_Emp emp,Port_Dept dept ";
                    updateSQL += " WHERE emp.No = cc.CCTo AND dept.No = emp.Dept AND cc.WorkID = " + workID;
                    break;
                case DBType.Oracle:
                    updateSQL = "UPDATE WF_CCList E SET (CCToName,DeptNo,DeptName)=(SELECT U.Name,U.FK_Dept,D.Name FROM Port_Emp U,Port_Dept D WHERE E.CCTo=U.No AND U.FK_Dept=D.No) WHERE E.WorkID = " + workID;
                    break;
                default:
                    updateSQL = "UPDATE WF_CCList cc LEFT JOIN Port_Emp emp ON cc.CCTo = emp.No LEFT JOIN Port_Dept dept ON emp.FK_Dept = dept.No SET cc.CCToName = emp.Name, cc.DeptNo = emp.FK_Dept, cc.DeptName = dept.Name WHERE  WorkID =" + workID;
                    break;
            }
            DBAccess.RunSQL(updateSQL);
            if (type.Equals("WorkNode"))
            {
                this.currWN.addMsg("CC", BP.WF.Glo.multilingual("@自动抄送给:{0}.", "WorkNode", "cc", empNames));
            }
        }
        /// <summary>
        /// 获得人员
        /// </summary>
        /// <param name="rol"></param>
        /// <param name="rpt"></param>
        /// <param name="workid"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataTable GenerCCers(CCRole rol, Entity rpt, Int64 workid)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));

            DataTable mydt = new DataTable();

            ///按照接受人规则计算.
            if (rol.CCRoleExcType == CCRoleExcType.ByDeliveryWay)
            {
                Node toNode = new Node(rol.NodeID);
                WorkNode toWn = new WorkNode(this.currWN.HisWork, toNode);

                FindWorker fw = new FindWorker();
                DataTable mydt1 = fw.DoIt(this.currWN.HisFlow, this.currWN, toWn);
                return mydt1;

            }

            string sql = "";
            if (rol.CCRoleExcType == CCRoleExcType.ByDepts)
            {
                /*如果抄送到部门. */
                //  sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A, WF_CCDept B  WHERE  B.FK_Dept=A.FK_Dept AND B.FK_Node=" + this.NodeID;
                sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A  WHERE  A.FK_Dept IN (" + rol.EnIDs + ") ";

                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            if (rol.CCRoleExcType == CCRoleExcType.ByEmps)
            {
                /*如果抄送到人员. */
              //  sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A, WF_CCEmp B WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Node=" + rol.NodeID;
                sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A  WHERE  " + BP.Sys.Base.Glo.UserNo + " IN (" + rol.EnIDs + ") ";

                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            if (rol.CCRoleExcType == CCRoleExcType.ByStations)
            {
                if (rol.CCStaWay == BP.WF.CCStaWay.StationOnly)
                {
                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B  WHERE A.No= B.FK_Emp AND B.FK_Station IN (" + rol.EnIDs + ")";
                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"] = mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }

                if (rol.CCStaWay == BP.WF.CCStaWay.StationSmartCurrNodeWorker || rol.CCStaWay == BP.WF.CCStaWay.StationSmartNextNodeWorker)
                {
                    /*按角色智能计算*/
                    string deptNo = "";
                    if (rol.CCStaWay == BP.WF.CCStaWay.StationSmartCurrNodeWorker)
                        deptNo = BP.Web.WebUser.DeptNo;
                    else
                        deptNo = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID=" + workid + " AND IsEnable=1 AND IsPass=0", BP.Web.WebUser.DeptNo);

                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station IN (" + rol.EnIDs + ") AND B.FK_Dept='" + deptNo + "'";
                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"] = mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }

                if (rol.CCStaWay == BP.WF.CCStaWay.StationAndDept)
                {
                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B  WHERE A.No= B.FK_Emp AND B.FK_Station IN (" + rol.EnIDs + ") AND B.FK_Dept IN(" + rol.EnDeptIDs + ")";
                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"]=mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }

                if (rol.CCStaWay == CCStaWay.StationDeptUpLevelCurrNodeWorker ||
                    rol.CCStaWay == CCStaWay.StationDeptUpLevelNextNodeWorker)
                {
                    // 求当事人的部门编号.
                    string deptNo = "";

                    if (rol.CCStaWay == CCStaWay.StationDeptUpLevelCurrNodeWorker)
                        deptNo = BP.Web.WebUser.DeptNo;

                    if (rol.CCStaWay == CCStaWay.StationDeptUpLevelNextNodeWorker)
                        deptNo = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID=" + workid + " AND IsEnable=1 AND IsPass=0", WebUser.DeptNo);

                    while (true)
                    {
                        BP.Port.Dept dept = new Dept(deptNo);

                        //sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station=C.FK_Station  AND C.FK_Node=" + this.NodeID + " AND B.FK_Dept='" + deptNo + "'";

                        sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND B.FK_Station IN (" + rol.EnIDs + ") AND B.FK_Dept='" + deptNo + "'";

                        mydt = DBAccess.RunSQLReturnTable(sql);
                        foreach (DataRow mydr in mydt.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr["No"] = mydr["No"];
                            dr["Name"] = mydr["Name"];
                            dt.Rows.Add(dr);
                        }

                        if (dept.ParentNo == "0")
                            break;

                        deptNo = dept.ParentNo;
                    }
                    return dt;
                }
            }

            if (rol.CCRoleExcType == CCRoleExcType.BySQLs)
            {
                sql = rol.EnIDs.Clone() as string;
                sql = sql.Replace("@WebUser.No", WebUser.No);
                sql = sql.Replace("@WebUser.Name", WebUser.Name);
                sql = sql.Replace("@WebUser.FK_Dept", WebUser.DeptNo);
                if (sql.Contains("@") == true)
                    sql = BP.WF.Glo.DealExp(sql, rpt, null);

                /*按照SQL抄送. */
                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            /**按照表单字段抄送*/
            if (rol.CCRoleExcType == CCRoleExcType.ByFrmField)
            {
                if (DataType.IsNullOrEmpty(rol.EnIDs) == true)
                    throw new Exception("抄送规则自动抄送选择按照表单字段抄送没有设置抄送人员字段");

                string[] attrs = rol.EnIDs.Split(',');
                foreach (string attr in attrs)
                {
                    string ccers = rpt.GetValStrByKey(attr);
                    if (DataType.IsNullOrEmpty(ccers) == false)
                    {
                        //判断该字段是否启用了pop返回值？
                        sql = "SELECT  Tag1 AS VAL FROM Sys_FrmEleDB WHERE RefPKVal=" + workid + " AND EleID='" + attr + "'";
                        DataTable dtVals = DBAccess.RunSQLReturnTable(sql);
                        string emps = "";
                        //获取接受人并格式化接受人, 
                        if (dtVals.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtVals.Rows)
                                emps += dr[0].ToString() + ",";
                        }
                        else
                        {
                            emps = ccers;
                        }
                        //end判断该字段是否启用了pop返回值

                        emps = emps.Replace(" ", ""); //去掉空格.
                        if (DataType.IsNullOrEmpty(emps) == false)
                        {
                            /*如果包含,; 例如 zhangsan,张三;lisi,李四;*/
                            string[] ccemp = emps.Split(',');
                            foreach (string empNo in ccemp)
                            {
                                if (DataType.IsNullOrEmpty(empNo) == true)
                                    continue;
                                Emp emp = new Emp();
                                emp.UserID = empNo;
                                if (emp.RetrieveFromDBSources() == 1)
                                {
                                    DataRow dr = dt.NewRow();
                                    dr["No"] = empNo;
                                    dr["Name"] = emp.Name;
                                    dt.Rows.Add(dr);
                                }
                            }
                        }
                    }
                }
                return dt;
            }
            throw new Exception("err@没有解析的设置." + rol.CCRoleExcType);
        }
    }
}
