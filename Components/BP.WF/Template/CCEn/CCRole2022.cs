using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template.CCEn
{
    /// <summary>
    /// 抄送属性
    /// </summary>
    public class CCRoleAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string FlowNo = "FlowNo";

        public const string CCRoleExcType = "CCRoleExcType";
        public const string Tag1 = "Tag1";
        public const string Tag2 = "Tag2";
        public const string CCStaWay = "CCStaWay";
        #endregion
    }
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCRole : Entity
    {
        #region 属性
        /// <summary>
        /// 获得抄送人
        /// </summary>
        /// <param name="rpt"></param>
        /// <returns></returns>
        public DataTable GenerCCers(Entity rpt, Int64 workid)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("No", typeof(string)));
            dt.Columns.Add(new DataColumn("Name", typeof(string)));

            DataTable mydt = new DataTable();
            string sql = "";
            if (this.CCRoleExcType == CCRoleExcType.ByDepts)
            {
                /*如果抄送到部门. */
                //  sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A, WF_CCDept B  WHERE  B.FK_Dept=A.FK_Dept AND B.FK_Node=" + this.NodeID;
                sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A  WHERE  A.FK_Dept IN (" + this.Tag1 + ") ";

                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
            }

            if (this.CCRoleExcType == CCRoleExcType.ByEmps)
            {
                /*如果抄送到人员. */
                sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A, WF_CCEmp B WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Node=" + this.NodeID;

                sql = "SELECT A." + BP.Sys.Base.Glo.UserNo + ", A.Name FROM Port_Emp A  WHERE  " + BP.Sys.Base.Glo.UserNo + " IN (" + this.Tag1 + ") ";

                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
            }

            if (this.CCRoleExcType == CCRoleExcType.ByStations)
            {
                if (this.CCStaWay == WF.CCStaWay.StationOnly)
                {

                    //   sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C  WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "= //B.FK_Emp AND B.FK_Station=C.FK_Station AND C.FK_Node=" + this.NodeID;

                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B  WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "= B.FK_Emp AND B.FK_Station IN (" + this.Tag1 + ")";

                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"] = mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                }

                if (this.CCStaWay == WF.CCStaWay.StationSmartCurrNodeWorker || this.CCStaWay == WF.CCStaWay.StationSmartNextNodeWorker)
                {
                    /*按角色智能计算*/
                    string deptNo = "";
                    if (this.CCStaWay == WF.CCStaWay.StationSmartCurrNodeWorker)
                        deptNo = BP.Web.WebUser.FK_Dept;
                    else
                        deptNo = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID=" + workid + " AND IsEnable=1 AND IsPass=0", BP.Web.WebUser.FK_Dept);


                    //sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station=C.FK_Station  AND C.FK_Node=" + this.NodeID + " AND B.FK_Dept='" + deptNo + "'";

                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station IN (" + this.Tag1 + ") AND B.FK_Dept='" + deptNo + "'";

                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"] = mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                }

                if (this.CCStaWay == WF.CCStaWay.StationAndDept)
                {
                    throw new Exception("err@没有解析StationAndDept. ");
                    //sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C, WF_CCDept D WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station=C.FK_Station AND A.FK_Dept=D.FK_Dept AND B.FK_Dept=D.FK_Dept AND C.FK_Node=" + this.NodeID + " AND D.FK_Node=" + this.NodeID;

                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C, WF_CCDept D WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station=C.FK_Station AND A.FK_Dept=D.FK_Dept AND B.FK_Dept=D.FK_Dept AND C.FK_Node=" + this.NodeID + " AND D.FK_Node=" + this.NodeID;

                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"] = mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                }

                if (this.CCStaWay == CCStaWay.StationDeptUpLevelCurrNodeWorker ||
                    this.CCStaWay == CCStaWay.StationDeptUpLevelNextNodeWorker)
                {
                    // 求当事人的部门编号.
                    string deptNo = "";

                    if (this.CCStaWay == CCStaWay.StationDeptUpLevelCurrNodeWorker)
                        deptNo = BP.Web.WebUser.FK_Dept;

                    if (this.CCStaWay == CCStaWay.StationDeptUpLevelNextNodeWorker)
                        deptNo = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID=" + workid + " AND IsEnable=1 AND IsPass=0", BP.Web.WebUser.FK_Dept);

                    while (true)
                    {
                        BP.Port.Dept dept = new Dept(deptNo);

                        //sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station=C.FK_Station  AND C.FK_Node=" + this.NodeID + " AND B.FK_Dept='" + deptNo + "'";

                        sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmpStation B WHERE A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=B.FK_Emp AND B.FK_Station IN ("+this.Tag1+") AND B.FK_Dept='" + deptNo + "'";

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
                }
            }

            if (this.CCRoleExcType == CCRoleExcType.BySQLs)
            {
                sql = this.Tag1.Clone() as string;
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
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
            }
            /**按照表单字段抄送*/
            if (this.CCRoleExcType == CCRoleExcType.ByFrmField)
            {
                if (DataType.IsNullOrEmpty(this.Tag1) == true)
                    throw new Exception("抄送规则自动抄送选择按照表单字段抄送没有设置抄送人员字段");

                string[] attrs = this.Tag1.Split(',');
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
            }
            //将dt中的重复数据过滤掉  
            DataView myDataView = new DataView(dt);
            //此处可加任意数据项组合  
            string[] strComuns = { "No", "Name" };
            dt = myDataView.ToTable(true, strComuns);

            return dt;
        }
        /// <summary>
        ///节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CCRoleExcType CCRoleExcType
        {
            get
            {
                return (CCRoleExcType)this.GetValIntByKey(CCRoleAttr.CCRoleExcType);
            }
            set
            {
                this.SetValByKey(CCRoleAttr.CCRoleExcType, value);
            }
        }
        public CCStaWay CCStaWay
        {
            get
            {
                return (CCStaWay)this.GetValIntByKey(CCRoleAttr.CCStaWay);
            }
        }

        public string Tag1
        {
            get
            {
                string str= this.GetValStringByKey(CCRoleAttr.Tag1);
                str = str.Replace(",","','");
                str = "'" + str;
                str = str.Replace("''","'");
                return str;
            }
        }
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (BP.Web.WebUser.IsAdmin == false)
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 抄送设置
        /// </summary>
        public CCRole()
        {
        }
        /// <summary>
        /// 抄送设置
        /// </summary>
        /// <param name="nodeid"></param>
        public CCRole(int nodeid)
        {
            this.NodeID = nodeid;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_CCRole", "抄送规则");

                map.AddMyPK();
                map.AddTBInt(CCRoleAttr.NodeID, 0, "节点", false, true);
                map.AddTBString(CCRoleAttr.FlowNo, null, "流程编号", false, false, 0, 10, 50, true);
                // 执行类型.
                string val = "@0=按表单字段计算@1=按人员计算@2=按角色计算@3=按部门计算@4=按SQL计算";
                map.AddDDLSysEnum(CCRoleAttr.CCRoleExcType, 0, "执行类型", true, true, CCRoleAttr.CCRoleExcType, val);
                map.AddTBStringDoc(CCRoleAttr.Tag1, null, "执行内容1", true, false, true);
                map.AddTBStringDoc(CCRoleAttr.Tag2, null, "执行内容2", true, false, true);
                map.AddTBInt(CCRoleAttr.CCStaWay, 0, "CCStaWay", false, true);

                map.AddTBAtParas(300);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 执行类型
    /// </summary>
    public enum CCRoleExcType
    {
        ByFrmField,
        ByEmps,
        ByStations,
        ByDepts,
        BySQLs
    }
    /// <summary>
    /// 抄送s
    /// </summary>
    public class CCRoles : Entities
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCRole();
            }
        }
        /// <summary>
        /// 抄送
        /// </summary>
        public CCRoles() { }
        public CCRoles(int nodeID)
        {
            this.Retrieve(NodeAttr.NodeID, nodeID, NodeAttr.Step);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCRole> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCRole>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCRole> Tolist()
        {
            System.Collections.Generic.List<CCRole> list = new System.Collections.Generic.List<CCRole>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCRole)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
