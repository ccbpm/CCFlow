using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.Port;
using BP.Web;
using BP.GPM;

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
                if (BP.Web.WebUser.No.Equals("admin") == true)
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

                map.DepositaryOfEntity = Depositary.Application;

                map.AddTBIntPK(SelectorAttr.NodeID, 0, "NodeID", true, true);
                map.AddTBString(SelectorAttr.Name, null, "节点名称", true, true, 0, 100, 100);

                map.AddDDLSysEnum(SelectorAttr.SelectorModel, 5, "显示方式", true, true, SelectorAttr.SelectorModel,
                    "@0=按岗位@1=按部门@2=按人员@3=按SQL@4=按SQL模版计算@5=使用通用人员选择器@6=部门与岗位的交集@7=自定义Url@8=使用通用部门岗位人员选择器@9=按岗位智能计算(操作员所在部门)");

                map.AddDDLSQL(SelectorAttr.FK_SQLTemplate, null, "SQL模版",
                    "SELECT No,Name FROM WF_SQLTemplate WHERE SQLType=5", true);

                map.AddBoolean(SelectorAttr.IsAutoLoadEmps, true, "是否自动加载上一次选择的人员？", true, true);
                map.AddBoolean(SelectorAttr.IsSimpleSelector, false, "是否单项选择(只能选择一个人)？", true, true);
                map.AddBoolean(SelectorAttr.IsEnableDeptRange, false, "是否启用部门搜索范围限定(对使用通用人员选择器有效)？", true, true, true);
                map.AddBoolean(SelectorAttr.IsEnableStaRange, false, "是否启用岗位搜索范围限定(对使用通用人员选择器有效)？", true, true, true);


                // map.AddDDLSysEnum(SelectorAttr.IsMinuesAutoLoadEmps, 5, "接收人选择方式", true, true, SelectorAttr.SelectorModel,
                // "@0=按岗位@1=按部门@2=按人员@3=按SQL@4=按SQL模版计算@5=使用通用人员选择器@6=部门与岗位的交集@7=自定义Url");

                map.AddTBStringDoc(SelectorAttr.SelectorP1, null, "分组参数:可以为空,比如:SELECT No,Name,ParentNo FROM  Port_Dept", true, false, 0, 300, 3);
                map.AddTBStringDoc(SelectorAttr.SelectorP2, null, "操作员数据源:比如:SELECT No,Name,FK_Dept FROM  Port_Emp", true, false, 0, 300, 3);

                map.AddTBStringDoc(SelectorAttr.SelectorP3, null, "默认选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID", true, false, 0, 300, 3);
                map.AddTBStringDoc(SelectorAttr.SelectorP4, null, "强制选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID", true, false, 0, 300, 3);
                map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 300, 10);
                #endregion

                #region 对应关系
                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new BP.WF.Template.NodeStations(), new BP.Port.Stations(),
                    BP.WF.Template.NodeStationAttr.FK_Node,
                    BP.WF.Template.NodeStationAttr.FK_Station, "绑定岗位(平铺)", BP.Port.StationAttr.FK_StationType, "Name", "No");

                map.AttrsOfOneVSM.AddGroupListModel(new BP.WF.Template.NodeStations(), new BP.Port.Stations(),
                  BP.WF.Template.NodeStationAttr.FK_Node,
                  BP.WF.Template.NodeStationAttr.FK_Station, "绑定岗位(树)", BP.Port.StationAttr.FK_StationType, "Name", "No");

                //节点绑定部门. 节点绑定部门.
                map.AttrsOfOneVSM.AddBranches(new BP.WF.Template.NodeDepts(), new BP.Port.Depts(),
                   BP.WF.Template.NodeDeptAttr.FK_Node,
                   BP.WF.Template.NodeDeptAttr.FK_Dept, "绑定部门", BP.Port.EmpAttr.Name, BP.Port.EmpAttr.No, "@WebUser.FK_Dept");

                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.WF.Template.NodeEmps(), new BP.Port.Emps(),
                   BP.WF.Template.NodeEmpAttr.FK_Node,
                   BP.WF.Template.NodeEmpAttr.FK_Emp, "绑定接受人", BP.Port.EmpAttr.FK_Dept, BP.Port.EmpAttr.Name, BP.Port.EmpAttr.No, "@WebUser.FK_Dept");
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
        public DataSet GenerDataSet(int nodeid, Entity en)
        {
            DataSet ds = null;
            switch (this.SelectorModel)
            {
                case SelectorModel.Dept:
                    ds = ByDept(nodeid, en);
                    break;
                case SelectorModel.TeamOrgOnly:
                    ds = ByTeam(nodeid, en, this.SelectorModel);
                    break;
                case SelectorModel.TeamOnly:
                    ds = ByTeam(nodeid, en, this.SelectorModel);
                    break;
                case SelectorModel.TeamDeptOnly:
                    ds = ByTeam(nodeid, en, this.SelectorModel);
                    break;
                case SelectorModel.Emp:
                    ds = ByEmp(nodeid);
                    break;
                case SelectorModel.Station:
                    ds = ByStation(nodeid, en);
                    break;
                case SelectorModel.ByStationAI:
                    ds = ByStationAI(nodeid, en);

                    break;
                case SelectorModel.DeptAndStation:
                    ds = DeptAndStation(nodeid);
                    break;
                case SelectorModel.SQL:
                    ds = BySQL(nodeid, en);
                    break;
                case SelectorModel.SQLTemplate:
                    ds = SQLTemplate(nodeid, en);
                    break;
                case SelectorModel.GenerUserSelecter:
                    ds = ByGenerUserSelecter();
                    break;
                case SelectorModel.AccepterOfDeptStationOfCurrentOper: //按岗位智能计算.
                    ds = AccepterOfDeptStationOfCurrentOper(nodeid, en);
                    break;
                case SelectorModel.ByWebAPI:
                    ds = ByWebAPI(en);
                    break;
                case SelectorModel.ByMyDeptEmps:
                    ds = ByMyDeptEmps();
                    break;
                default:
                    throw new Exception("@错误:没有判断的选择类型:" + this.SelectorModel);
                    break;
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (dt.Columns[i].ColumnName.ToUpper() == "NO")
                            dt.Columns[i].ColumnName = "No";

                        if (dt.Columns[i].ColumnName.ToUpper() == "NAME")
                            dt.Columns[i].ColumnName = "Name";

                        if (dt.Columns[i].ColumnName.ToUpper() == "PARENTNO")
                            dt.Columns[i].ColumnName = "ParentNo";

                        if (dt.Columns[i].ColumnName.ToUpper() == "FK_DEPT")
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
            string sql = "SELECT distinct No,Name, ParentNo FROM Port_Dept  ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT  No, Name, FK_Dept FROM Port_Emp ";
            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);

            return ds;
        }
        /// <summary>
        /// 按照模版
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="en"></param>
        /// <returns></returns>
        private DataSet SQLTemplate(int nodeID, Entity en)
        {
            //设置他的模版.
            //Node nd = new Node(nodeID);

            SQLTemplate sql = new SQLTemplate(this.SelectorP1);
            this.SelectorP2 = sql.Docs;

            return BySQL(nodeID, en);
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
            string sqlGroup = this.SelectorP1; // @sly
            if (DataType.IsNullOrEmpty(sqlGroup) == false && sqlGroup.Length > 6)
            {
                sqlGroup = BP.WF.Glo.DealExp(sqlGroup, en, null);  //@祝梦娟
                DataTable dt = DBAccess.RunSQLReturnTable(sqlGroup);
                dt.TableName = "Depts";
                //转换大小写
                foreach (DataColumn col in dt.Columns)
                {
                    string colName = col.ColumnName.ToLower();
                    switch (colName)
                    {
                        case "no":
                            col.ColumnName = "No";
                            break;
                        case "name":
                            col.ColumnName = "Name";
                            break;
                        default:
                            break;
                    }
                }
                ds.Tables.Add(dt);
            }

            //求人员范围.
            string sqlDB = this.SelectorP2;
            sqlDB = BP.WF.Glo.DealExp(sqlDB, en, null);  //@祝梦娟

            DataTable dtEmp = DBAccess.RunSQLReturnTable(sqlDB);
            dtEmp.TableName = "Emps";
            //转换大小写
            foreach (DataColumn col in dtEmp.Columns)
            {
                string colName = col.ColumnName.ToLower();
                switch (colName)
                {
                    case "no":
                        col.ColumnName = "No";
                        break;
                    case "name":
                        col.ColumnName = "Name";
                        break;
                    case "fk_dept":
                        col.ColumnName = "FK_Dept";
                        break;
                    default:
                        break;
                }
            }
            ds.Tables.Add(dtEmp);

            //求默认选择的数据.
            if (this.SelectorP3 != "")
            {
                sqlDB = this.SelectorP3;
                sqlDB = BP.WF.Glo.DealExp(sqlDB, en, null);  //@祝梦娟

                DataTable dtDef = DBAccess.RunSQLReturnTable(sqlDB);
                dtDef.TableName = "DefaultSelected";
                foreach (DataColumn col in dtDef.Columns)
                {
                    string colName = col.ColumnName.ToLower();
                    switch (colName)
                    {
                        case "no":
                            col.ColumnName = "No";
                            break;
                        case "name":
                            col.ColumnName = "Name";
                            break;
                        default:
                            break;
                    }
                }

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

                DataTable dtForce = DBAccess.RunSQLReturnTable(sqlDB);
                foreach (DataColumn col in dtForce.Columns)
                {
                    string colName = col.ColumnName.ToLower();
                    switch (colName)
                    {
                        case "no":
                            col.ColumnName = "No";
                            break;
                        case "name":
                            col.ColumnName = "Name";
                            break;
                        default:
                            break;
                    }
                }
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
                sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a,  WF_NodeDept b, WF_PrjEmp C,Port_DeptEmp D WHERE A.No=B.FK_Dept AND B.FK_Node=" + nodeID + " AND C.FK_Prj='" + en.GetValStrByKey("PrjNo") + "' ORDER BY a.Idx ";
                sql += "  AND C.FK_Emp=D.FK_Emp ";


                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                //人员.
                sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ", a.Name, a.FK_Dept FROM Port_Emp a, WF_NodeDept b, WF_PrjEmp C WHERE a.FK_Dept=b.FK_Dept  AND A." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=C.FK_Emp  AND B.FK_Node=" + nodeID + " AND C.FK_Prj='" + en.GetValStrByKey("PrjNo") + "'  ORDER BY a.Idx ";
                dtEmp = DBAccess.RunSQLReturnTable(sql);
                ds.Tables.Add(dtEmp);
                dtEmp.TableName = "Emps";
                return ds;
            }


            //部门.
            sql = "SELECT distinct a.No,a.Name, a.ParentNo,a.Idx FROM Port_Dept a,WF_NodeDept b WHERE a.No=b.FK_Dept AND B.FK_Node=" + nodeID + "   ORDER BY a.Idx ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ", a.Name, d.FK_Dept ,a.Idx FROM Port_Emp a, WF_NodeDept b,Port_DeptEmp d WHERE d.FK_Dept=b.FK_Dept AND a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=d.FK_Emp AND B.FK_Node=" + nodeID + "  ORDER BY a.Idx";
            dtEmp = DBAccess.RunSQLReturnTable(sql);
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


            //部门.
            string sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a, WF_NodeEmp b, Port_Emp c WHERE b.FK_Emp=c." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND a.No=c.FK_Dept AND B.FK_Node=" + nodeID + " ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept FROM Port_Emp a, WF_NodeEmp b WHERE a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=b.FK_Emp AND b.FK_Node=" + nodeID;

            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }
        /// <summary>
        /// 按照Emp获取部门人员树.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns>返回数据源dataset</returns>
        private DataSet ByMyDeptEmps()
        {
            // 定义数据容器.
            DataSet ds = new DataSet();


            //部门.
            string sql = "SELECT No,Name FROM Port_Dept  WHERE No='" + WebUser.FK_Dept + "' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = " SELECT No,Name,FK_Dept FROM Port_Emp  WHERE FK_Dept='" + WebUser.FK_Dept + "'";

            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        private DataSet AccepterOfDeptStationOfCurrentOper(int nodeID, Entity en)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            //部门.
            string sql = "";
            sql = "SELECT d.No,d.Name,d.ParentNo  FROM  Port_DeptEmp  de, Port_Dept as d WHERE de.FK_Dept = d.No and de.FK_Emp = '" + BP.Web.WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //人员.
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND C.FK_Dept='" + WebUser.FK_Dept + "' AND b.FK_Node=" + nodeID + ")  ORDER BY A.Idx ";
            else
                sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a." + BP.Sys.Base.Glo.UserNo + "=c.FK_Emp AND C.FK_Dept='" + WebUser.FK_Dept + "' AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + "  ORDER BY A.Idx";

            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
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

                sql = " SELECT No,Name, ParentNo FROM Port_Dept WHERE no  in (  SELECT  ParentNo FROM Port_Dept WHERE No  IN "
                + "( SELECT FK_Dept FROM WF_GenerWorkerlist WHERE WorkID ='" + workID + "' ))";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                // 如果当前的节点不是开始节点， 从轨迹里面查询。
                sql = "SELECT DISTINCT b." + BP.Sys.Base.Glo.UserNo + ",b.Name,b.FK_Dept   FROM Port_DeptEmpStation a,Port_Emp b  WHERE FK_Station IN "
                   + "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeID + ") "
                   + "AND a.FK_Dept IN (SELECT ParentNo FROM Port_Dept WHERE No IN (SELECT FK_DEPT FROM WF_GenerWorkerlist WHERE WorkID=" + workID + "))"
                   + " AND a.FK_Emp = b." + BP.Sys.Base.Glo.UserNo + " ";
                sql += " ORDER BY b.No ";

                dtEmp = DBAccess.RunSQLReturnTable(sql);
                dtEmp.TableName = "Emps";
                ds.Tables.Add(dtEmp);
            }
            return ds;
        }
        /// <summary>
        /// 部门于岗位的交集 @zkr. 
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private DataSet DeptAndStation(int nodeID)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            //部门.
            string sql = "";

            sql = "SELECT B.No,B.Name,B.ParentNo FROM WF_NodeDept A, Port_Dept B WHERE A.FK_Dept=B.No AND FK_Node=" + nodeID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //@zkr.
            sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,a.Idx FROM Port_Emp A,  WF_NodeStation b, Port_DeptEmpStation c,WF_NodeDept D WHERE a." + BP.Sys.Base.Glo.UserNo + "=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + " AND D.FK_Dept=A.FK_Dept AND D.FK_Node=" + nodeID + "  ORDER BY A.Idx ";
            DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }

        /// <summary>
        /// 按用户组计算
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        private DataSet ByTeam(int nodeID, Entity en, SelectorModel sm)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();
            string sql = null;
            DataTable dt = null;
            DataTable dtEmp = null;
            Node nd = new Node(nodeID);
            if (sm == SelectorModel.TeamDeptOnly)
                sql = "SELECT  No,Name FROM Port_Dept WHERE No='" + WebUser.FK_Dept + "'";
            if (sm == SelectorModel.TeamOnly)
                sql = "SELECT DISTINCT a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeTeam b, Port_TeamEmp c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Team=c.FK_Team AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID + "   ORDER BY A.No,A.Idx";
            if (sm == SelectorModel.TeamOrgOnly)
                sql = "SELECT DISTINCT a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeTeam b, Port_TeamEmp c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Team=c.FK_Team AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID + " AND D.OrgNo='" + WebUser.OrgNo + "' ORDER BY A.No,A.Idx";

            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle
                || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || DBAccess.AppCenterDBType == DBType.UX)
            {
                if (sm == SelectorModel.TeamDeptOnly)
                    sql = "SELECT * FROM (SELECT DISTINCT a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeTeam b, Port_TeamEmp c WHERE a.No=c.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nodeID + " AND A.FK_Dept='" + WebUser.FK_Dept + "') ORDER BY FK_Dept,Idx,No";
                if (sm == SelectorModel.TeamOrgOnly)
                    sql = "SELECT * FROM (SELECT DISTINCT a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeTeam b, Port_TeamEmp c WHERE a.No=c.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nodeID + " AND A.OrgNo='" + WebUser.OrgNo + "') ORDER BY FK_Dept,Idx,No";
                if (sm == SelectorModel.TeamOnly)
                    sql = "SELECT * FROM (SELECT DISTINCT a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeTeam b, Port_TeamEmp c WHERE a.No=c.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nodeID + " ) ORDER BY FK_Dept,Idx,No";
            }
            else
            {
                if (sm == SelectorModel.TeamDeptOnly)
                    sql = "SELECT DISTINCT a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,a.Idx FROM Port_Emp A,  WF_NodeTeam B, Port_TeamEmp C WHERE a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=c.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nodeID + " AND A.FK_Dept='" + WebUser.FK_Dept + "'  ORDER BY A.Idx";
                if (sm == SelectorModel.TeamOrgOnly)
                    sql = "SELECT DISTINCT a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,a.Idx FROM Port_Emp A,  WF_NodeTeam B, Port_TeamEmp C WHERE a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=c.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nodeID + " AND A.OrgNo='" + WebUser.OrgNo + "'  ORDER BY A.Idx";
                if (sm == SelectorModel.TeamOnly)
                    sql = "SELECT DISTINCT a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,a.Idx FROM Port_Emp A,  WF_NodeTeam B, Port_TeamEmp C WHERE a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=c.FK_Emp AND B.FK_Team=C.FK_Team AND B.FK_Node=" + nodeID + "  ORDER BY A.Idx";
            }

            dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }
        private DataSet ByGroupOnly(int nodeID, Entity en)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();
            string sql = null;
            DataTable dt = null;
            DataTable dtEmp = null;

            Node nd = new Node(nodeID);

            //部门.
            sql = "SELECT distinct a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeTeam b, Port_TeamEmp c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Group=c.FK_Group AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID + " ORDER BY A.No,A.Idx";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || DBAccess.AppCenterDBType == DBType.UX)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "Idx") == true)
                    sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeTeam b, Port_TeamEmp c WHERE a.No=c.FK_Emp AND B.FK_Group=C.FK_Group AND b.FK_Node=" + nodeID + ") ORDER BY FK_Dept,Idx,No";
                else
                    sql = "SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeTeam b, Port_TeamEmp c WHERE a.No=c.FK_Emp AND B.FK_Group=C.FK_Group AND b.FK_Node=" + nodeID + " ";
            }
            else
            {
                sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeTeam b, Port_TeamEmp c WHERE a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=c.FK_Emp AND B.FK_Group=C.FK_Group AND b.FK_Node=" + nodeID + "  ORDER BY A.Idx";
            }

            dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }

        private DataSet ByStationAI(int nodeID, Entity en)
        {
            Node nd = new Node(nodeID);

            int ShenFenModel = nd.GetParaInt("ShenFenModel");

            //如果按照上一个节点的操作员身份计算.
            if (ShenFenModel == 0)
                return ByStationAI(en, BP.Web.WebUser.FK_Dept, WebUser.No);

            //如果按照指定节点的操作员身份计算.
            if (ShenFenModel == 1)
            {
                int specNodeID = nd.GetParaInt("ShenFenVal");

                int workID = en.GetValIntByKey("OID");

                string sql = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerList WHERE FK_Node=" + specNodeID + " AND WorkID=" + workID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                string empNo = "", deptNo = "";
                if (dt.Rows.Count == 0)
                {
                    BP.WF.Node ndSpec = new Node(specNodeID);
                    if (ndSpec.IsStartNode == false)
                        throw new Exception("err@没有找到上一步节点，参数信息: NodeID=" + specNodeID + ",WorkID=" + workID + ", 不应该出现的异常，请联系管理员, 有可能您配置了没有路过的节点，作为指定节点的身份计算了。");
                    empNo = BP.Web.WebUser.No;
                    deptNo = BP.Web.WebUser.FK_Dept;
                }
                else
                {
                    //获得指定节点的人员编号.
                    empNo = dt.Rows[0][0].ToString();
                    deptNo = dt.Rows[0][1].ToString();
                }

                return ByStationAI(en, deptNo, empNo);
            }

            //如果按指定字段的身份计算.
            if (ShenFenModel == 2)
            {
                string empNo = nd.GetParaString("ShenFenVal");
                BP.Port.Emp emp = new BP.Port.Emp(empNo);
                return ByStationAI(en, emp.FK_Dept, emp.No);
            }

            throw new Exception("err@没有判断的身份模式." + ShenFenModel);
        }

        private DataSet ByStationAI(Entity en, string deptNo, string userID)
        {
            //第一次计算.
            DataSet ds = ByStationAI_Ext(en, deptNo, userID);

            if (ds.Tables[1].Rows.Count == 0)
            {
                //如果在本部门找不到，就到父部门去找.
                BP.Port.Dept mydept = new Dept(deptNo);
                ds = ByStationAI_Ext(en, mydept.ParentNo, userID);

                if (ds.Tables[1].Rows.Count == 0)
                {
                    //如果父部门找不到，就到父父部门去找, 在找不到就不找了。
                    if (mydept.ParentNo.Equals("0") == false)
                    {
                        Dept myParentDept = new Dept(mydept.ParentNo);
                        ds = ByStationAI_Ext(en, myParentDept.ParentNo, userID);
                        if (ds.Tables[1].Rows.Count != 0)
                            return ds;
                    }

                    if (ds.Tables[1].Rows.Count == 0)
                    {
                        //如果爷爷部门也找不到，就到于父亲同一级的部门去找.
                        BP.Port.Depts pDepts = new Depts();
                        pDepts.Retrieve(BP.Port.DeptAttr.ParentNo, mydept.ParentNo);

                        foreach (BP.Port.Dept item in pDepts)
                        {
                            ds = ByStationAI_Ext(en, item.No, userID);
                            if (ds.Tables[1].Rows.Count >= 1)
                                return ds;
                        }
                    }
                }
            }

            //如果实在找不到了，就仅按岗位计算.
            if (ds.Tables[1].Rows.Count == 0)
            {
                ds = ByStation(this.NodeID, en);
            }

            return ds;

            ////如果人员为空.
            //while (ds.Tables[1].Rows.Count == 0)
            //{
            //    GPM.Dept mydept = new GPM.Dept(deptNo);

            //    //首先扫描平级部门.
            //    BP.Port.Depts depts = new GPM.Depts();
            //    depts.Retrieve(BP.Port.DeptAttr.ParentNo, mydept.ParentNo);
            //    BP.Port.Dept dept = new BP.Port.Dept(WebUser.FK_Dept);
            //    ds = ByStationAI_Ext(nodeID, en, dept.ParentNo, BP.Web.WebUser.No);
            //}
            //return ds;
        }
        /// <summary>
        /// 指定部门下的，岗位人员的数据。
        /// </summary>
        /// <param name="nodeID"></param>
        /// <param name="en"></param>
        /// <param name="deptNo"></param>
        /// <param name="userNo"></param>
        /// <returns></returns>
        private DataSet ByStationAI_Ext(Entity en, string deptNo, string userNo)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();
            string sql = null;
            DataTable dt = null;
            DataTable dtEmp = null;

            //部门. @zkr
            sql = "";
            sql += "SELECT No, Name FROM Port_Dept WHERE No = '" + deptNo + "'";
            sql += " UNION ";
            sql += "SELECT  No, Name FROM Port_Dept A, Port_DeptEmp B WHERE A.No = B.FK_Dept AND B.FK_Emp = '" + userNo + "'";

            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //查询人员.
            sql = "SELECT A.No,A.Name, A.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B, WF_NodeStation C WHERE C.FK_Node = " + this.NodeID + " AND B.FK_Dept = '" + deptNo + "' AND A.FK_Dept = B.FK_Dept AND B.FK_Station=C.FK_Station AND A.No=b.FK_Emp  ORDER BY A.Idx";
            dtEmp = DBAccess.RunSQLReturnTable(sql);
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
                sql = "SELECT distinct a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeStation b, Port_DeptEmpStation c, Port_Emp d, WF_PrjEmp E WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND d." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=e.FK_Emp And C.FK_Emp=E.FK_Emp  AND B.FK_Node=" + nodeID + " AND E.FK_Prj='" + en.GetValStrByKey("PrjNo") + "' ORDER BY A.No,A.Idx";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);

                //人员.
                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || DBAccess.AppCenterDBType == DBType.UX)
                {
                    if (DBAccess.IsExitsTableCol("Port_Emp", "Idx") == true)
                        sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c, WF_PrjEmp d  WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station And a.No=d.FK_Emp And C.FK_Emp=d.FK_Emp AND b.FK_Node=" + nodeID + " AND D.FK_Prj='" + en.GetValStrByKey("PrjNo") + "') ORDER BY FK_Dept,Idx,No";
                    else
                        sql = "SELECT distinct a.No,a.Name, a.FK_Dept,A.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c, WF_PrjEmp d  WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station And a.No=d.FK_Emp And C.FK_Emp=d.FK_Emp AND b.FK_Node=" + nodeID + " AND D.FK_Prj='" + en.GetValStrByKey("PrjNo") + "'  ORDER BY A.Idx ";
                }
                else
                {
                    sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,A.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c, WF_PrjEmp d WHERE a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=c.FK_Emp AND B.FK_Station=C.FK_Station And a." + BP.Sys.Base.Glo.UserNoWhitOutAS + "=d.FK_Emp And C.FK_Emp=d.FK_Emp AND b.FK_Node=" + nodeID + " AND D.FK_Prj='" + en.GetValStrByKey("PrjNo") + "'  ORDER BY A.Idx ";
                }

                dtEmp = DBAccess.RunSQLReturnTable(sql);
                ds.Tables.Add(dtEmp);
                dtEmp.TableName = "Emps";
                return ds;
            }


            //部门.
            sql = "SELECT distinct a.No, a.Name, a.ParentNo,a.Idx FROM Port_Dept a, WF_NodeStation b, Port_DeptEmpStation c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND B.FK_Node=" + nodeID + " ORDER BY A.No,A.Idx";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || DBAccess.AppCenterDBType == DBType.UX)
            {
                if (DBAccess.IsExitsTableCol("Port_Emp", "Idx") == true)
                    sql = "SELECT * FROM (SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + ") ORDER BY FK_Dept,Idx,No";
                else
                    sql = "SELECT distinct a.No,a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + "  ORDER BY A.Idx";
            }
            else
            {
                sql = "SELECT distinct a." + BP.Sys.Base.Glo.UserNo + ",a.Name, a.FK_Dept,a.Idx FROM Port_Emp a,  WF_NodeStation b, Port_DeptEmpStation c WHERE a." + BP.Sys.Base.Glo.UserNo + "=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID + "  ORDER BY A.Idx";
            }

            dtEmp = DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);

            return ds;
        }
        private DataSet ByWebAPI(Entity en)
        {
            DataSet ds = new DataSet();
            //返回值
            string postData = "";
            //配置的api地址
            string apiUrl = this.SelectorP1;
            if (apiUrl.Contains("@WebApiHost"))//可以替换配置文件中配置的webapi地址
                apiUrl = apiUrl.Replace("@WebApiHost", BP.Difference.SystemConfig.AppSettings["WebApiHost"].ToString());

            //增加header参数
            Hashtable headerMap = new Hashtable();


            //saas模式，需要传入systemNo
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                //获取系统编号
                //string systemNo = BP.DA.DBAccess.RunSQLReturnStringIsNull("select No from port_domain where No=(select domain from port_org where No=(select orgNo from port_emp where No='" + WebUser.No + "'))", "");
                //headerMap.Add("systemNo", systemNo);
                //headerMap.Add("orgNo", WebUser.OrgNo);
            }
            //集团模式，传入域编号
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                //传入域
                headerMap.Add("OrgNo", WebUser.OrgNo);
            }

            //加入token
            headerMap.Add("Content-Type", "application/json");
            headerMap.Add("Authorization", WebUser.Token);



            apiUrl = BP.WF.Glo.DealExp(apiUrl, en, null);
            //执行POST
            postData = BP.WF.Glo.HttpPostConnect(apiUrl, headerMap, "");

            DataTable dt = BP.Tools.Json.ToDataTable(postData);
            dt.TableName = "Emps";
            ds.Tables.Add(dt);

            //部门
            //string sql = "SELECT distinct No,Name, ParentNo FROM Port_Dept where No='null'";
            //DataTable dtDept = DBAccess.RunSQLReturnTable(sql);
            //dtDept.TableName = "Depts";
            //ds.Tables.Add(dtDept);

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

        public Selectors(string fk_flow)
        {
            string sql = "select NodeId from WF_Node where FK_Flow='" + fk_flow + "'";
            this.RetrieveInSQL(sql);
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
