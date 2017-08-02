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
    /// 方向条件控制规则
    /// </summary>
    public enum CondModel
    {
        /// <summary>
        /// 按照用户设置的方向条件计算
        /// </summary>
        ByLineCond,
        /// <summary>
        /// 按照用户选择计算
        /// </summary>
        ByUserSelected,
        /// <summary>
        /// 发送按钮旁下拉框选择
        /// </summary>
        SendButtonSileSelect
    }
    /// <summary>
    /// 关系类型
    /// </summary>
    public enum CondOrAnd
    {
        /// <summary>
        /// 关系集合里面的所有条件都成立.
        /// </summary>
        ByAnd,
        /// <summary>
        /// 关系集合里的只有一个条件成立.
        /// </summary>
        ByOr
    }
    /// <summary>
    /// 待办工作超时处理方式
    /// </summary>
    public enum OutTimeDeal
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None = 0,
        /// <summary>
        /// 自动的转向下一步骤
        /// </summary>
        AutoTurntoNextStep=1,
        /// <summary>
        /// 自动跳转到指定的点
        /// </summary>
        AutoJumpToSpecNode=2,
        /// <summary>
        /// 自动移交到指定的人员
        /// </summary>
        AutoShiftToSpecUser=3,
        /// <summary>
        /// 向指定的人员发送消息
        /// </summary>
        SendMsgToSpecUser=4,
        /// <summary>
        /// 删除流程
        /// </summary>
        DeleteFlow=5,
        /// <summary>
        /// 执行SQL
        /// </summary>
        RunSQL=6
        ///// <summary>
        ///// 到达指定的日期，仍未处理，自动向下发送.
        ///// </summary>
        //WhenToSpecDataAutoSend
    }
    /// <summary>
    /// 选择的数据类别
    /// </summary>
    public enum AccepterDBSort
    {
        /// <summary>
        /// 人员
        /// </summary>
        Emp,
        /// <summary>
        /// 部门
        /// </summary>
        Dept,
        /// <summary>
        /// 岗位
        /// </summary>
        Station,
        /// <summary>
        /// 权限组
        /// </summary>
        Group
    }
    /// <summary>
    /// 显示方式
    /// </summary>
    public enum SelectorDBShowWay
    {
        /// <summary>
        /// 表格
        /// </summary>
        Table,
        /// <summary>
        /// 树
        /// </summary>
        Tree
    }
    public enum SelectorModel
    {
        /// <summary>
        /// 表格
        /// </summary>
        Station,
        /// <summary>
        /// 树
        /// </summary>
        Dept,
        /// <summary>
        /// 操作员
        /// </summary>
        Emp,
        /// <summary>
        /// SQL
        /// </summary>
        SQL,
        /// <summary>
        /// 自定义链接
        /// </summary>
        Url,
        /// <summary>
        /// 通用的人员选择器.
        /// </summary>
        GenerUserSelecter,
        /// <summary>
        /// 按部门与岗位的交集
        /// </summary>
        DeptAndStation
    }
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
        public const string SelectorDBShowWay = "SelectorDBShowWay";
        /// <summary>
        /// 选择的数据类别
        /// </summary>
        public const string AccepterDBSort = "AccepterDBSort";
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
        /// 显示方式
        /// </summary>
        public SelectorDBShowWay SelectorDBShowWay
        {
            get
            {
                return (SelectorDBShowWay)this.GetValIntByKey(SelectorAttr.SelectorDBShowWay);
            }
            set
            {
                this.SetValByKey(SelectorAttr.SelectorDBShowWay, (int)value);
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
        /// 选择的数据类型
        /// </summary>
        public AccepterDBSort AccepterDBSort
        {
            get
            {
                return (AccepterDBSort)this.GetValIntByKey(SelectorAttr.AccepterDBSort);
            }
            set
            {
                this.SetValByKey(SelectorAttr.AccepterDBSort, (int)value);
            }
        }
        /// <summary>
        /// 分组数据源
        /// </summary>
        public string SelectorP1
        {
            get
            {
                string s= this.GetValStringByKey(SelectorAttr.SelectorP1);
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
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// Accpter
        /// </summary>
        public Selector() { }
        /// <summary>
        /// 
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

                Map map = new Map("WF_Node", "选择器");

                map.Java_SetDepositaryOfEntity(Depositary.Application);

                map.AddTBIntPK(SelectorAttr.NodeID, 0, "NodeID", true, true);
                map.AddTBString(SelectorAttr.Name, null, "节点名称", true, true, 0, 100, 100);

                map.AddDDLSysEnum(SelectorAttr.SelectorDBShowWay, 0, "数据显示方式", true, true,
                SelectorAttr.SelectorDBShowWay, "@0=表格显示@1=树形显示");

                map.AddDDLSysEnum(SelectorAttr.SelectorModel, 5, "窗口模式", true, true, SelectorAttr.SelectorModel,
                    "@0=按岗位@1=按部门@2=按人员@3=按SQL@4=自定义Url@5=使用通用人员选择器@6=部门与岗位的交集");

                map.AddDDLSysEnum(SelectorAttr.AccepterDBSort, 0, "选择的数据类别", true, true,
              SelectorAttr.AccepterDBSort, "@0=人员@1=部门@2=岗位@3=权限组");


                map.AddTBStringDoc(SelectorAttr.SelectorP1, null, "分组参数:可以为空,比如:SELECT No,Name,ParentNo FROM  Port_Dept", true, false, true);
                map.AddTBStringDoc(SelectorAttr.SelectorP2, null, "操作员数据源:比如:SELECT No,Name,FK_Dept FROM  Port_Emp", true, false, true);

                map.AddTBStringDoc(SelectorAttr.SelectorP3, null, "默认选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID", true, false, true);
                map.AddTBStringDoc(SelectorAttr.SelectorP4, null, "强制选择的数据源:比如:SELECT FK_Emp FROM  WF_GenerWorkerList WHERE FK_Node=102 AND WorkID=@WorkID", true, false, true);


                //map.AddTBStringDoc(SelectorAttr.SelectorP1, null, "分组参数,可以为空", true, false, true);
                //map.AddTBStringDoc(SelectorAttr.SelectorP2, null, "操作员数据源", true, false, true);


                // 相关功能。
                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeStations(), new BP.WF.Port.Stations(),
                    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                    DeptAttr.Name, DeptAttr.No, "节点岗位");

                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                DeptAttr.No, "节点部门", Dot2DotModel.Default);

                map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeEmps(), new BP.WF.Port.Emps(), NodeEmpAttr.FK_Node, NodeEmpAttr.FK_Emp, DeptAttr.Name,
                    DeptAttr.No, "接受人员", Dot2DotModel.Default);


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
            switch (this.SelectorModel)
            {
                case Template.SelectorModel.Dept:
                    return ByDept(nodeid);
                case Template.SelectorModel.Emp:
                    return ByEmp(nodeid);
                case Template.SelectorModel.Station:
                    return ByStation(nodeid);
                case Template.SelectorModel.DeptAndStation:
                    return ByStation(nodeid);
                case Template.SelectorModel.SQL:
                    return BySQL(nodeid, en);
                default:
                    DataSet ds= new DataSet();
                    return ds;
                    break;
            }
            return null;
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
            if (string.IsNullOrEmpty(sqlGroup) == false)
            {
                sqlGroup = sqlGroup.Replace("@WebUser.No", WebUser.No);
                sqlGroup = sqlGroup.Replace("@WebUser.Name", WebUser.Name);
                sqlGroup = sqlGroup.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sqlGroup);
                dt.TableName = "Depts";
                ds.Tables.Add(dt);
            }


            //求人员范围.
            string sqlDB = this.SelectorP2;
            sqlDB = sqlDB.Replace("@WebUser.No", WebUser.No);
            sqlDB = sqlDB.Replace("@WebUser.Name", WebUser.Name);
            sqlDB = sqlDB.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sqlDB);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
             
            //求默认选择的数据.
            if (this.SelectorP3 != "")
            {
                sqlDB = this.SelectorP3;

                sqlDB = sqlDB.Replace("@WebUser.No", WebUser.No);
                sqlDB = sqlDB.Replace("@WebUser.Name", WebUser.Name);
                sqlDB = sqlDB.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

                sqlDB = sqlDB.Replace("@WorkID", en.GetValStringByKey("OID"));
                sqlDB = sqlDB.Replace("@OID", en.GetValStringByKey("OID"));

                if (sqlDB.Contains("@"))
                    sqlDB = BP.WF.Glo.DealExp(sqlDB, en, null);

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
        private DataSet ByDept(int nodeID)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            //部门.
            string sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a,  WF_NodeDept b WHERE a.No=b.FK_Dept AND B.FK_Node="+nodeID;
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }
            ds.Tables.Add(dt);


            //人员.
            sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeDept b WHERE a.FK_Dept=b.FK_Dept AND B.FK_Node=" + nodeID;
             DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
             ds.Tables.Add(dtEmp);

             if (SystemConfig.AppCenterDBType == DBType.Oracle)
             {
                 dt.Columns["NO"].ColumnName = "No";
                 dt.Columns["NAME"].ColumnName = "Name";
                 dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
             }

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
            string sql = "SELECT distinct a.No,a.Name, a.ParentNo FROM Port_Dept a, WF_NodeEmp b, Port_Emp c WHERE b.FK_Emp=c.No AND a.No=c.FK_Dept AND B.FK_Node=" + nodeID;
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeEmp b WHERE a.No=b.FK_Emp AND b.FK_Node=" + nodeID;
            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);
            return ds;
        }
        private DataSet DeptAndStation(int nodeID)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            //部门.
            string sql = "SELECT distinct a.No, a.Name, a.ParentNo FROM Port_Dept a, WF_NodeStation b, Port_EmpStation c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID;
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);

            //人员.
            sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_EmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID;
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
        private DataSet ByStation(int nodeID)
        {
            // 定义数据容器.
            DataSet ds = new DataSet();

            //部门.
            string sql = "SELECT distinct a.No, a.Name, a.ParentNo FROM Port_Dept a, WF_NodeStation b, Port_EmpStation c, Port_Emp d WHERE a.No=d.FK_Dept AND b.FK_Station=c.FK_Station AND C.FK_Emp=D.No AND B.FK_Node=" + nodeID;
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Depts";
            ds.Tables.Add(dt);


            //人员.
            sql = "SELECT distinct a.No,a.Name, a.FK_Dept FROM Port_Emp a,  WF_NodeStation b, Port_EmpStation c WHERE a.No=c.FK_Emp AND B.FK_Station=C.FK_Station AND b.FK_Node=" + nodeID;
            DataTable dtEmp = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
