using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;
using BP.Web;

namespace BP.WF.Template
{
    /// <summary>
    /// Selector属性
    /// </summary>
    public class SelectorAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 流程
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 接受模式
        /// </summary>
        public const string SelectorModel = "SelectorModel";
        /// <summary>
        /// 选择人分组
        /// </summary>
        public const string SelectorP1 = "SelectorP1";
        /// <summary>
        /// 操作员
        /// </summary>
        public const string SelectorP2 = "SelectorP2";
        /// <summary>
        /// 默认选择的数据源
        /// </summary>
        public const string SelectorP3 = "SelectorP3";
        /// <summary>
        /// 强制选择的数据源
        /// </summary>
        public const string SelectorP4 = "SelectorP4";
        /// <summary>
        /// 数据显示方式(表格与树)
        /// </summary>
        public const string FK_SQLTemplate = "FK_SQLTemplate";
        /// <summary>
        /// 是否自动装载上一笔加载的数据
        /// </summary>
        public const string IsAutoLoadEmps = "IsAutoLoadEmps";
        /// <summary>
        /// 是否单项选择？
        /// </summary>
        public const string IsSimpleSelector = "IsSimpleSelector";
        /// <summary>
        /// 是否启用部门搜索范围限定
        /// </summary>
        public const string IsEnableDeptRange = "IsEnableDeptRange";
        /// <summary>
        /// 是否启用岗位搜索范围限定
        /// </summary>
        public const string IsEnableStaRange = "IsEnableStaRange";
    }
    /// <summary>
    /// 选择器
    /// </summary>
    public class Selector : Entity
    {
        #region 基本属性
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        /// <summary>
        /// 选择模式
        /// </summary>
        public SelectorModel SelectorModel
        {
            get
            {
                return (SelectorModel)this.GetValIntByKey(SelectorAttr.SelectorModel);
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorModel, (int)value);
            }
        }
        /// <summary>
        /// 分组数据源
        /// </summary>
        public string SelectorP1
        {
            get
            {
                string s = this.GetValStringByKey(SelectorAttr.SelectorP1);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorP1, value);
            }
        }
        /// <summary>
        /// 实体数据源
        /// </summary>
        public string SelectorP2
        {
            get
            {
                string s = this.GetValStringByKey(SelectorAttr.SelectorP2);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorP2, value);
            }
        }
        /// <summary>
        /// 默认选择数据源
        /// </summary>
        public string SelectorP3
        {
            get
            {
                string s = this.GetValStringByKey(SelectorAttr.SelectorP3);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorP3, value);
            }
        }
        /// <summary>
        /// 强制选择数据源
        /// </summary>
        public string SelectorP4
        {
            get
            {
                string s = this.GetValStringByKey(SelectorAttr.SelectorP4);
                s = s.Replace("~", "'");
                return s;
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorP3, value);
            }
        }
        /// <summary>
        /// 是否自动装载上一笔加载的数据
        /// </summary>
        public bool IsAutoLoadEmps
        {
            get
            {
                return this.GetValBooleanByKey(SelectorAttr.IsAutoLoadEmps);
            }
            set
            {
                this.SetValByKey(SelectorAttr.IsAutoLoadEmps, value);
            }
        }
        /// <summary>
        /// 是否单选？
        /// </summary>
        public bool IsSimpleSelector
        {
            get
            {
                return this.GetValBooleanByKey(SelectorAttr.IsSimpleSelector);
            }
            set
            {
                this.SetValByKey(SelectorAttr.IsSimpleSelector, value);
            }
        }
        /// <summary>
        /// 是否启用部门搜索范围限定
        /// </summary>
        public bool IsEnableDeptRange
        {
            get
            {
                return this.GetValBooleanByKey(SelectorAttr.IsEnableDeptRange);
            }
            set
            {
                this.SetValByKey(SelectorAttr.IsEnableDeptRange, value);
            }
        }
        /// <summary>
        /// 是否启用岗位搜索范围限定
        /// </summary>
        public bool IsEnableStaRange
        {
            get
            {
                return this.GetValBooleanByKey(SelectorAttr.IsEnableStaRange);
            }
            set
            {
                this.SetValByKey(SelectorAttr.IsEnableStaRange, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(SelectorAttr.NodeID);
            }
            set
            {
                this.SetValByKey(SelectorAttr.NodeID, value);
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
                uac.IsDelete = false;
                uac.IsInsert = false;
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsUpdate = true;
                    uac.IsView = true;
                }

                uac.IsUpdate = true;

                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 接受人选择器
        /// </summary>
        public Selector() { }
        /// <summary>
        /// 接受人选择器
        /// </summary>
        /// <param name="nodeid"></param>
        public Selector(int nodeid)
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

                #region 字段.
                Map map = new Map("WF_Node", "选择器");

                map.Java_SetDepositaryOfEntity(Depositary.Application);

                map.AddTBIntPK(SelectorAttr.NodeID, 0, "NodeID", true, true);
                map.AddTBString(SelectorAttr.Name, null, "节点名称", true, true, 0, 100, 100);

                map.AddDDLSysEnum(SelectorAttr.SelectorModel, 5, "显示方式", true, true, SelectorAttr.SelectorModel,
                    "@0=按岗位@1=按部门@2=按人员@3=按SQL@4=按SQL模版计算@5=使用通用人员选择器@6=部门与岗位的交集@7=自定义Url@8=使用通用部门岗位人员选择器@9=按岗位智能计算(操作员所在部门)");

                map.AddDDLSQL(SelectorAttr.FK_SQLTemplate, null, "SQL模版", "SELECT No,Name FROM WF_SQLTemplate WHERE SQLType=5", true);

                map.AddBoolean(SelectorAttr.IsAutoLoadEmps, true, "是否自动加载上一次选择的人员？", true, true);
                map.AddBoolean(SelectorAttr.IsSimpleSelector, false, "是否单项选择(只能选择一个人)？", true, true);

                map.AddBoolean(SelectorAttr.IsEnableDeptRange, false, "是否启用部门搜索范围限定(对使用通用人员选择器有效)？", true, true, true);
                map.AddBoolean(SelectorAttr.IsEnableStaRange, false, "是否启用岗位搜索范围限定(对使用通用人员选择器有效)？", true, true, true);


                //     map.AddDDLSysEnum(SelectorAttr.IsMinuesAutoLoadEmps, 5, "接收人选择方式", true, true, SelectorAttr.SelectorModel,
                //   "@0=按岗位@1=按部门@2=按人员@3=按SQL@4=按SQL模版计算@5=使用通用人员选择器@6=部门与岗位的交集@7=自定义Url");


                map.AddTBStringDoc(SelectorAttr.SelectorP1, null, "分组参数:可以为空,比如:SELECT No,Name,ParentNo FROM  Port_Dept", true, false, true);
                map.AddTBStringDoc(SelectorAttr.SelectorP2, null, "操作员数据源:比如:SELECT No,Name,FK_Dept FROM  Port_Emp", true, false, true);

                map.AddTBStringDoc(SelectorAttr.SelectorP3, null, "默认选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID", true, false, true);
                map.AddTBStringDoc(SelectorAttr.SelectorP4, null, "强制选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID", true, false, true);

                #endregion

                #region 对应关系
                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                    BP.WF.Template.NodeStationAttr.FK_Node,
                    BP.WF.Template.NodeStationAttr.FK_Station, "绑定岗位(平铺)", StationAttr.FK_StationType, "Name", "No");

                map.AttrsOfOneVSM.AddGroupListModel(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                  BP.WF.Template.NodeStationAttr.FK_Node,
                  BP.WF.Template.NodeStationAttr.FK_Station, "绑定岗位(树)", StationAttr.FK_StationType, "Name", "No");

                //节点绑定部门. 节点绑定部门.
                map.AttrsOfOneVSM.AddBranches(new BP.WF.Template.NodeDepts(), new BP.Port.Depts(),
                   BP.WF.Template.NodeDeptAttr.FK_Node,
                   BP.WF.Template.NodeDeptAttr.FK_Dept, "绑定部门", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.WF.Template.NodeEmps(), new BP.Port.Emps(),
                   BP.WF.Template.NodeEmpAttr.FK_Node,
                   BP.WF.Template.NodeEmpAttr.FK_Emp, "绑定接受人", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");
                #endregion



                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 产生数据.
        /// </summary>
        /// <returns></returns>
        public System.Data.DataSet GenerDataSet(int nodeid, Entity en)
        {
            DataSet ds = null;
            switch (this.SelectorModel)
            {
                case SelectorModel.Dept:
                    ds = ByDept(nodeid, en);
                    break;
                case SelectorModel.Emp:
                    ds = ByEmp(nodeid);
                    break;
                case SelectorModel.Station:
                    ds = ByStation(nodeid, en);
                    break;
                case SelectorModel.DeptAndStation:
                    ds = DeptAndStation(nodeid);
                    break;
                case SelectorModel.SQL:
                    ds = BySQL(nodeid, en);
                    break;
                case SelectorModel.GenerUserSelecter:
                    ds = ByGenerUserSelecter();
                    break;
                case SelectorModel.AccepterOfDeptStationOfCurrentOper:
                    ds = AccepterOfDeptStationOfCurrentOper(nodeid, en);
                    break;
                default:
                    throw new Exception("@错误:没有判断的选择类型:" + this.SelectorModel);
                    break;
            }

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i].ColumnName == "NO")
                            dt.Columns[i].ColumnName = "No";

                        if (dt.Columns[i].ColumnName == "NAME")
                            dt.Columns[i].ColumnName = "Name";

                        if (dt.Columns[i].ColumnName == "PARENTNO")
                            dt.Columns[i].ColumnName = "ParentNo";

                        if (dt.Columns[i].ColumnName == "FK_DEPT")
                            dt.Columns[i].ColumnName = "FK_Dept";
                    }
                }
            }

            ds.Tables.Add(this.ToDataTableField("Selector"));

            return ds;
        }
        /// <summary>
        /// 通用
        /// </summary>
        /// <returns></returns>
        private DataSet ByGenerUserSelecter()
        {
            DataSet ds = new DataSet();

            ////排序.
            //string orderByDept = "";
            //if (DBAccess.IsExitsTableCol("Port_Dept", "Idx"))
            //    orderByDept = " ORDER BY Port_Dept.Idx";

            //string orderByEmp = "";
            //if (DBAccess.IsExitsTableCol("Port_Emp", "Idx"))
            //    orderByDept = " ORDER BY Port_Emp.FK_Dept, Port_Emp.Idx";

            //部门
            string sql = "SELECT distinct No,Name, ParentNo FROM Port_Dept ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct No, Name, FK_Dept FROM Port_Emp ";
            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);

            return ds;
        }
        /// <summary>
        /// 按照SQL计算.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns>返回值</returns>
        private DataSet BySQL(int nodeID, Entity en)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            //求部门.
            string sqlGroup = this.SelectorP1;
            if (DataType.IsNullOrEmpty(sqlGroup) == false)
            {
                sqlGroup = BP.WF.Glo.DealExp(sqlGroup, en, null);  //@祝梦娟
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlGroup);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);
            }

            //求人员范围.
            string sqlDB = this.SelectorP2;
            sqlDB = BP.WF.Glo.DealExp(sqlDB, en, null);  //@祝梦娟

            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sqlDB);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);

            //求默认选择的数据.
            if (this.SelectorP3 != "")
            {
                sqlDB = this.SelectorP3;
                sqlDB = BP.WF.Glo.DealExp(sqlDB, en, null);  //@祝梦娟

                DataTable dtDef = BP.DA.DBAccess.RunSQLReturnTable(sqlDB);
                dtDef.TableName = "DefaultSelected";

                ds.Tables.Add(dtDef);
            }


            //求强制选择的数据源.
            if (this.SelectorP4 != "")
            {
                sqlDB = this.SelectorP4;

                sqlDB = sqlDB.Replace("@WebUser.No", WebUser.No);
                sqlDB = sqlDB.Replace("@WebUser.Name", WebUser.Name);
                sqlDB = sqlDB.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

                sqlDB = sqlDB.Replace("@WorkID", en.GetValStringByKey("OID"));
                sqlDB = sqlDB.Replace("@OID", en.GetValStringByKey("OID"));

                if (sqlDB.Contains("@"))
                    sqlDB = BP.WF.Glo.DealExp(sqlDB, en, null);

                DataTable dtForce = BP.DA.DBAccess.RunSQLReturnTable(sqlDB);
                dtForce.TableName = "ForceSelected";
                ds.Tables.Add(dtForce);
            }

            return ds;
        }

        /// <summary>
        /// 按照部门获取部门人员树.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns>返回数据源dataset</returns>
        private DataSet ByDept(int nodeID, Entity en)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();
            string sql = null;
            DataTable dt = null;
            DataTable dtEmp = null;

            Node nd = new Node(nodeID);
            if (nd.HisDeliveryWay == DeliveryWay.BySelectedForPrj)
            {
                //部门.
                sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a,  WF_NodeDept b, WF_PrjEmp C,Port_DeptEmp D WHERE A.No=B.FK_Dept AND B.FK_Node=" + nodeID + " AND C.FK_Prj='" + en.GetValStrByKey("PrjNo") + "' ";
                sql += "  AND C.FK_Emp=D.FK_Emp ";


                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                //人员.
                sql = "SELECT distinct a.No, a.Name, a.FK_Dept FROM Port_Emp a, WF_NodeDept b, WF_PrjEmp C WHERE a.FK_Dept=b.FK_Dept  AND A.No=C.FK_Emp  AND B.FK_Node=" + nodeID + " AND C.FK_Prj='" + en.GetValStrByKey("PrjNo") + "'  ";
                dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
                ds.Tables.Add(dtEmp);
                dtEmp.TableName = "Emps";
                return ds;
            }


            //部门.
            sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a,  WF_NodeDept b WHERE a.No=b.FK_Dept AND B.FK_Node=" + nodeID + " ";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct a.No, a.Name, a.FK_Dept FROM Port_Emp a, WF_NodeDept b WHERE a.FK_Dept=b.FK_Dept AND B.FK_Node=" + nodeID + " ";
            dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            ds.Tables.Add(dtEmp);
            dtEmp.TableName = "Emps";
            return ds;
        }
        /// <summary>
        /// 按照Emp获取部门人员树.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns>返回数据源dataset</returns>
        private DataSet ByEmp(int nodeID)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            ////排序.
            //string orderByDept = "";
            //if (DBAccess.IsExitsTableCol("Port_Dept", "Idx"))
            //    orderByDept = " ORDER BY Port_Dept.Idx";

            //string orderByEmp = "";
            //if (DBAccess.IsExitsTableCol("Port_Emp", "Idx"))
            //    orderByEmp = " ORDER BY Port_Emp.Idx";

            //部门.
            string sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a, WF_NodeEmp b, Port_Emp c WHERE b.FK_Emp=c.No AND a.No=c.FK_Dept AND B.FK_Node=" + nodeID + " ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a, WF_NodeEmp b WHERE a.No=b.FK_Emp AND b.FK_Node=" + nodeID + " ";

            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }

        private DataSet AccepterOfDeptStationOfCurrentOper(int nodeID, Entity en)
        {

            // 定义数据容器.
            DataSet ds = new DataSet();


            //部门.
            string sql = "";
            sql = "SELECT d.No,d.Name,d.ParentNo  FROM  Port_DeptEmp  de,port_dept as d where de.FK_Dept = d.No and de.FK_Emp = '" + BP.Web.WebUser.No + "'";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            //人员.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND C.FK_Dept='" + WebUser.FK_Dept + "' AND b.FK_Node=" + nodeID + ")  ";
            else
                sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND C.FK_Dept='" + WebUser.FK_Dept + "' AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID;

            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dtEmp.Rows.Count > 0)
            {
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                dtEmp.TableName = "Emps";
                ds.Tables.Add(dtEmp);
            }
            else //如果没人，就查询父级
            {
                //查询当前节点的workdID
                long workID = long.Parse(en.GetValStringByKey("OID"));
                BP.WF.WorkNode node = new WorkNode(workID, nodeID);

                sql = " select No,Name, ParentNo from port_dept where no  in (  select  ParentNo from port_dept where no  in"
                + "( SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID ='" + workID + "' ))";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                // 如果当前的节点不是开始节点， 从轨迹里面查询。
                sql = "SELECT DISTINCT b.No,b.Name,b.FK_Dept   FROM " + BP.WF.Glo.EmpStation + " a,Port_Emp b  WHERE FK_Station IN "
                   + "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeID + ") "
                   + "AND a.FK_Dept IN (SELECT ParentNo FROM Port_Dept WHERE No in (SELECT FK_DEPT FROM WF_GenerWorkerlist WHERE WorkID=" + workID + "))"
                   + " AND a.FK_Emp = b.No ";
                sql += " ORDER BY b.No ";

                dtEmp = DBAccess.RunSQLReturnTable(sql);
                dtEmp.TableName = "Emps";
                ds.Tables.Add(dtEmp);
            }
            return ds;
        }
        private DataSet DeptAndStation(int nodeID)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();


            //部门.
            string sql = "";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT * FROM (SELECT distinct a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeStation b, Port_DeptEmpStation c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID + ")  AS Port_Dept  ";
            else
                sql = "SELECT distinct a.No, a.Name, a.ParentNo FROM Port_Dept a, WF_NodeStation b, Port_DeptEmpStation c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID + " ";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);



            //人员.

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + ")  ";
            else
                sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID;


            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }

        /// <summary>
        /// 按照Station获取部门人员树.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns>返回数据源dataset</returns>
        private DataSet ByStation(int nodeID, Entity en)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();
            string sql = null;
            DataTable dt = null;
            DataTable dtEmp = null;

            Node nd = new Node(nodeID);
            if (nd.HisDeliveryWay == DeliveryWay.BySelectedForPrj)
            {
                //部门.
                sql = "SELECT distinct a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeStation b, Port_DeptEmpStation c, Port_Emp d, WF_PrjEmp E WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D.No AND d.No=e.FK_Emp And C.FK_Emp=E.FK_Emp  AND B.FK_Node=" + nodeID + " AND E.FK_Prj='" + en.GetValStrByKey("PrjNo") + "' ORDER BY A.No,A.Idx";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                //人员.
                //人员.
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    if (DBAccess.IsExitsTableCol("Port_Emp", "Idx") == true)
                        sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c, WF_PrjEmp d  WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station And a.No=d.FK_Emp And C.FK_Emp=d.FK_Emp AND b.FK_Node=" + nodeID + " AND D.FK_Prj='" + en.GetValStrByKey("PrjNo") + "') ORDER BY FK_Dept,Idx,No";
                    else
                        sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c, WF_PrjEmp d  WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station And a.No=d.FK_Emp And C.FK_Emp=d.FK_Emp AND b.FK_Node=" + nodeID + " AND D.FK_Prj='" + en.GetValStrByKey("PrjNo") + "'  ";
                }
                else
                {
                    sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c, WF_PrjEmp d WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station And a.No=d.FK_Emp And C.FK_Emp=d.FK_Emp AND b.FK_Node=" + nodeID + " AND D.FK_Prj='" + en.GetValStrByKey("PrjNo") + "'  ";
                }

                dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
                ds.Tables.Add(dtEmp);
                dtEmp.TableName = "Emps";
                return ds;
            }


            //部门.
            sql = "SELECT distinct a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeStation b, Port_DeptEmpStation c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID + " ORDER BY A.No,A.Idx";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "Idx") == true)
                    sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + ") ORDER BY FK_Dept,Idx,No";
                else
                    sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID;
            }
            else
            {
                sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID;
            }

            dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);

            return ds;
        }
    }
    /// <summary>
    /// Accpter
    /// </summary>
    public class Selectors : Entities
    {
        /// <summary>
        /// Accpter
        /// </summary>
        public Selectors()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Selector();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Selector> ToJavaList()
        {
            return (System.Collections.Generic.IList<Selector>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Selector> Tolist()
        {
            System.Collections.Generic.List<Selector> list = new System.Collections.Generic.List<Selector>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Selector)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
