﻿using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
	/// <summary>
	/// 抄送属性
	/// </summary>
    public class CCAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string CCTitle = "CCTitle";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string CCDoc = "CCDoc";
        /// <summary>
        /// 按表单字段抄送
        /// </summary>
        public const string CCIsAttr = "CCIsAttr";
        /// <summary>
        /// 表单字段
        /// </summary>
        public const string CCFormAttr = "CCFormAttr";
        /// <summary>
        /// 是否启用抄送到岗位
        /// </summary>
        public const string CCIsStations = "CCIsStations";
        /// <summary>
        /// 按照岗位计算方式
        /// </summary>
        public const string CCStaWay = "CCStaWay";
        /// <summary>
        /// 是否抄送到部门
        /// </summary>
        public const string CCIsDepts = "CCIsDepts";
        /// <summary>
        /// 是否抄送到人员
        /// </summary>
        public const string CCIsEmps = "CCIsEmps";
        /// <summary>
        /// 是否启用按照SQL抄送.
        /// </summary>
        public const string CCIsSQLs = "CCIsSQLs";
        /// <summary>
        /// 要抄送的SQL
        /// </summary>
        public const string CCSQL = "CCSQL";
        #endregion
    }
	/// <summary>
	/// 抄送
	/// </summary>
    public class CC : Entity
    {
        #region 属性
        /// <summary>
        /// 抄送
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
            if (this.CCIsDepts == true)
            {
                /*如果抄送到部门. */
                if (Glo.OSModel == BP.Sys.OSModel.OneOne)
                    sql = "SELECT A.No, A.Name FROM Port_Emp A, WF_CCDept B WHERE  A.FK_Dept=B.FK_Dept AND B.FK_Node=" + this.NodeID;
                else
                    sql = "SELECT A.No, A.Name FROM Port_Emp A, WF_CCDept B  WHERE  B.FK_Dept=A.FK_Dept AND B.FK_Node=" + this.NodeID;

                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
            }

            if (this.CCIsEmps == true)
            {
                /*如果抄送到人员. */
                sql = "SELECT A.No, A.Name FROM Port_Emp A, WF_CCEmp B WHERE A.No=B.FK_Emp AND B.FK_Node=" + this.NodeID;
                mydt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow mydr in mydt.Rows)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = mydr["No"];
                    dr["Name"] = mydr["Name"];
                    dt.Rows.Add(dr);
                }
            }

            if (this.CCIsStations == true)
            {
                if (this.CCStaWay == WF.CCStaWay.StationOnly)
                {
                  
                        sql = "SELECT No,Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C  WHERE A.No=B.FK_Emp AND B.FK_Station=C.FK_Station AND C.FK_Node=" + this.NodeID;

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
                    /*按岗位智能计算*/
                    string deptNo = "";
                    if (this.CCStaWay == WF.CCStaWay.StationSmartCurrNodeWorker)
                        deptNo = BP.Web.WebUser.FK_Dept;
                    else
                        deptNo = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID=" + workid + " AND IsEnable=1 AND IsPass=0", BP.Web.WebUser.FK_Dept);

                 
                        sql = "SELECT No,Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C WHERE A.No=B.FK_Emp AND B.FK_Station=C.FK_Station  AND C.FK_Node=" + this.NodeID + " AND B.FK_Dept='" + deptNo + "'";

                    mydt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow mydr in mydt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        dr["No"] = mydr["No"];
                        dr["Name"] = mydr["Name"];
                        dt.Rows.Add(dr);
                    }
                }

                if (this.CCStaWay == WF.CCStaWay.StationAdndDept)
                {
                  
                        sql = "SELECT No,Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C, WF_CCDept D WHERE A.No=B.FK_Emp AND B.FK_Station=C.FK_Station AND A.FK_Dept=D.FK_Dept AND B.FK_Dept=D.FK_Dept AND C.FK_Node="+this.NodeID+" AND D.FK_Node="+this.NodeID;

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
                    this.CCStaWay == CCStaWay.StationDeptUpLevelNextNodeWorker )
                {
                    // 求当事人的部门编号.
                    string deptNo = "";

                    if (this.CCStaWay == CCStaWay.StationDeptUpLevelCurrNodeWorker)
                        deptNo = BP.Web.WebUser.FK_Dept;

                    if (this.CCStaWay == CCStaWay.StationDeptUpLevelNextNodeWorker)
                        deptNo = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID="+workid+" AND IsEnable=1 AND IsPass=0", BP.Web.WebUser.FK_Dept);

                    while (true)
                    {
                        BP.Port.Dept dept = new Dept(deptNo);

                      
                            sql = "SELECT No,Name FROM Port_Emp A, Port_DeptEmpStation B, WF_CCStation C WHERE A.No=B.FK_Emp AND B.FK_Station=C.FK_Station  AND C.FK_Node="+this.NodeID+" AND B.FK_Dept='"+deptNo+"'";

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

            if (this.CCIsSQLs == true)
            {
                sql = this.CCSQL.Clone() as string;
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
            //将dt中的重复数据过滤掉  
            DataView myDataView = new DataView(dt);
            //此处可加任意数据项组合  
            string[] strComuns = { "No","Name" };
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
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No != "admin")
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
        /// <summary>
        /// 抄送标题
        /// </summary>
        public string CCTitle
        {
            get
            {
                string s= this.GetValStringByKey(CCAttr.CCTitle);
                if (DataType.IsNullOrEmpty(s))
                    s = "来自@Rec的抄送信息.";
                return s;
            }
            set
            {
                this.SetValByKey(CCAttr.CCTitle, value);
            }
        }
        /// <summary>
        /// 抄送内容
        /// </summary>
        public string CCDoc
        {
            get
            {
                string s = this.GetValStringByKey(CCAttr.CCDoc);
                if (DataType.IsNullOrEmpty(s))
                    s = "来自@Rec的抄送信息.";
                return s;
            }
            set
            {
                this.SetValByKey(CCAttr.CCDoc, value);
            }
        }
        /// <summary>
        /// 抄送对象
        /// </summary>
        public string CCSQL
        {
            get
            {
                string sql= this.GetValStringByKey(CCAttr.CCSQL);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("‘", "'");
                sql = sql.Replace("’", "'");
                sql = sql.Replace("''", "'");
                return sql;
            }
            set
            {
                this.SetValByKey(CCAttr.CCSQL, value);
            }
        }
        /// <summary>
        /// 是否启用按照岗位抄送
        /// </summary>
        public bool CCIsStations
        {
            get
            {
                return this.GetValBooleanByKey(CCAttr.CCIsStations);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsStations, value);
            }
        }
        /// <summary>
        /// 抄送到岗位计算方式.
        /// </summary>
        public CCStaWay CCStaWay
        {
            get
            {
                return (CCStaWay)this.GetValIntByKey(CCAttr.CCStaWay);
            }
            set
            {
                this.SetValByKey(CCAttr.CCStaWay, (int)value);
            }
        }
        /// <summary>
        /// 是否启用按照部门抄送
        /// </summary>
        public bool CCIsDepts
        {
            get
            {
                return  this.GetValBooleanByKey(CCAttr.CCIsDepts);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsDepts, value);
            }
        }
        /// <summary>
        /// 是否启用按照人员抄送
        /// </summary>
        public bool CCIsEmps
        {
            get
            {
                return this.GetValBooleanByKey(CCAttr.CCIsEmps);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsEmps, value);
            }
        }
        /// <summary>
        /// 是否启用按照SQL抄送
        /// </summary>
        public bool CCIsSQLs
        {
            get
            {
                return this.GetValBooleanByKey(CCAttr.CCIsSQLs);
            }
            set
            {
                this.SetValByKey(CCAttr.CCIsSQLs, value);
            }
        }
        
        #endregion

        #region 构造函数
        /// <summary>
        /// 抄送设置
        /// </summary>
        public CC()
        {
        }
        /// <summary>
        /// 抄送设置
        /// </summary>
        /// <param name="nodeid"></param>
        public CC(int nodeid)
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

                Map map = new Map("WF_Node", "抄送规则");

                map.Java_SetEnType(EnType.Admin);

                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "节点名称", true, true, 0, 100, 10, false);
                map.AddTBString(NodeAttr.FK_Flow, null, "FK_Flow", false, false, 0, 3, 10);
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "抄送数据写入规则", true, true, NodeAttr.CCWriteTo,"@0=写入抄送列表@1=写入待办@2=写入待办与抄送列表");
                map.AddBoolean(CCAttr.CCIsAttr, false, "按表单字段抄送", true, true, true);
                map.AddTBString(CCAttr.CCFormAttr, null, "抄送人员字段", true, false, 0, 100, 10, true);
                
                map.AddBoolean(CCAttr.CCIsStations, false, "按照岗位抄送", true, true, true);
                map.AddDDLSysEnum(CCAttr.CCStaWay, 0, "抄送岗位计算方式", true, true, CCAttr.CCStaWay,
                    "@0=仅按岗位计算@1=按岗位智能计算(当前节点)@2=按岗位智能计算(发送到节点)@3=按岗位与部门的交集@4=按直线上级部门找岗位下的人员(当前节点)@5=按直线上级部门找岗位下的人员(接受节点)");

                map.AddBoolean(CCAttr.CCIsDepts, false, "按照部门抄送", true, true, true);
                map.AddBoolean(CCAttr.CCIsEmps, false, "按照人员抄送", true, true, true);

                map.AddBoolean(CCAttr.CCIsSQLs, false, "按照SQL抄送", true, true, true);
                map.AddTBString(CCAttr.CCSQL, null, "SQL表达式", true, false, 0, 200, 10, true);

                map.AddTBString(CCAttr.CCTitle, null, "抄送标题", true, false, 0, 100, 10, true);
                map.AddTBStringDoc(CCAttr.CCDoc, null, "抄送内容(标题与内容支持变量)", true, false, true);


                #region 对应关系
                // 相关功能。

                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.CCStations(), new BP.WF.Port.Stations(),
                    BP.WF.Template.NodeStationAttr.FK_Node,
                    BP.WF.Template.NodeStationAttr.FK_Station, "抄送岗位(AddGroupPanelModel)", StationAttr.FK_StationType);

                map.AttrsOfOneVSM.AddGroupListModel(new BP.WF.Template.CCStations(), new BP.WF.Port.Stations(),
                  BP.WF.Template.NodeStationAttr.FK_Node,
                  BP.WF.Template.NodeStationAttr.FK_Station, "抄送岗位(AddGroupListModel)", StationAttr.FK_StationType);


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranches(new BP.WF.Template.CCDepts(), new BP.Port.Depts(),
                   BP.WF.Template.NodeDeptAttr.FK_Node,
                   BP.WF.Template.NodeDeptAttr.FK_Dept, "抄送部门AddBranches", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.WF.Template.CCEmps(), new BP.Port.Emps(),
                   BP.WF.Template.NodeEmpAttr.FK_Node,
                   BP.WF.Template.NodeEmpAttr.FK_Emp, "抄送接受人(AddBranchesAndLeaf)", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                #endregion 对应关系

                //// 相关功能。
                //map.AttrsOfOneVSM.Add(new BP.WF.Template.CCStations(), new BP.WF.Port.Stations(),
                //    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                //    DeptAttr.Name, DeptAttr.No, "抄送岗位");

                //map.AttrsOfOneVSM.Add(new BP.WF.Template.CCDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                //DeptAttr.No,  "抄送部门" );

                //map.AttrsOfOneVSM.Add(new BP.WF.Template.CCEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, NodeEmpAttr.FK_Emp, DeptAttr.Name,
                //    DeptAttr.No, "抄送人员");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
	/// 抄送s
	/// </summary>
	public class CCs: Entities
	{
		#region 方法
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new CC();
			}
		}
		/// <summary>
        /// 抄送
		/// </summary>
		public CCs(){} 		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CC> ToJavaList()
        {
            return (System.Collections.Generic.IList<CC>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CC> Tolist()
        {
            System.Collections.Generic.List<CC> list = new System.Collections.Generic.List<CC>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CC)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
