using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 累加表单方案属性	  
    /// </summary>
    public class FoolTruckNodeFrmAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Frm = "FK_Frm";
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 是否readonly.
        /// </summary>
        public const string IsEdit = "IsEdit";
        /// <summary>
        /// IsPrint
        /// </summary>
        public const string IsPrint = "IsPrint";
        /// <summary>
        /// 是否启用装载填充事件.
        /// </summary>
        public const string IsEnableLoadData = "IsEnableLoadData";
        /// <summary>
        /// 是否1变N(对于分流节点有效)
        /// </summary>
        public const string Is1ToN = "Is1ToN";
        /// <summary>
        /// 是否默认打开
        /// </summary>
        public const string IsDefaultOpen = "IsDefaultOpen";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// FK_Flow
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FrmType = "FrmType";
        /// <summary>
        /// 方案
        /// </summary>
        public const string FrmSln = "FrmSln";
        /// <summary>
        /// 谁是主键？
        /// </summary>
        public const string WhoIsPK = "WhoIsPK";
        /// <summary>
        /// 模版文件
        /// </summary>
        public const string TempleteFile = "TempleteFile";
        /// <summary>
        /// 是否显示
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 关键字段
        /// </summary>
        public const string GuanJianZiDuan = "GuanJianZiDuan";
        /// <summary>
        /// 汇总
        /// </summary>
        public const string HuiZong = "HuiZong";
        /// <summary>
        /// 表单启用规则
        /// </summary>
        public const string FrmEnableRole = "FrmEnableRole";
        /// <summary>
        ///  表单启动表达式.
        /// </summary>
        public const string FrmEnableExp = "FrmEnableExp";
    }
    /// <summary>
    /// 表单
    /// 节点的工作节点有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class FoolTruckNodeFrm : EntityMyPK
    {

        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 是否启用装载填充事件
        /// </summary>
        public bool IsEnableLoadData
        {
            get
            {
                return this.GetValBooleanByKey(FoolTruckNodeFrmAttr.IsEnableLoadData);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.IsEnableLoadData, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FoolTruckNodeFrmAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(FoolTruckNodeFrmAttr.Idx);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.Idx, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_Frm
        {
            get
            {
                return this.GetValStringByKey(FoolTruckNodeFrmAttr.FK_Frm);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.FK_Frm, value);
            }
        }
       
        /// <summary>
        /// 是否显示
        /// </summary>
        public string IsEnable
        {
            get
            {
                return this.GetValStringByKey(FoolTruckNodeFrmAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.IsEnable,value);
            }
        }
       
        /// <summary>
        /// 对应的解决方案
        /// 0=默认方案.节点编号= 自定义方案, 1=不可编辑.
        /// </summary>
        public int FrmSln
        {
            get
            {
                return this.GetValIntByKey(FoolTruckNodeFrmAttr.FrmSln);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.FrmSln, value);
            }
        }
        /// <summary>
        /// 表单启动表达式
        /// </summary>
        public string FrmEnableExp
        {
            get
            {
                return this.GetValStringByKey(FoolTruckNodeFrmAttr.FrmEnableExp);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.FrmEnableExp, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FoolTruckNodeFrmAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 是否可以编辑？
        /// </summary>
        public bool IsEdit
        {
            get
            {
                if (this.FrmSln == 1)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 是否可以编辑？
        /// </summary>
        public int IsEditInt
        {
            get
            {
                if (this.IsEdit)
                    return 1;
                return 0;
            }
        }
        /// <summary>
        /// 是否可以打印
        /// </summary>
        public bool IsPrint
        {
            get
            {
                return this.GetValBooleanByKey(FoolTruckNodeFrmAttr.IsPrint);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.IsPrint, value);
            }
        }
        /// <summary>
        /// 是否可以打印
        /// </summary>
        public int IsPrintInt
        {
            get
            {
                return this.GetValIntByKey(FoolTruckNodeFrmAttr.IsPrint);
            }
        }
        /// <summary>
        /// 汇总
        /// </summary>
        public string HuiZong
        {
            get
            {
                return this.GetValStringByKey(FoolTruckNodeFrmAttr.HuiZong);
            }
            set
            {
                this.SetValByKey(FoolTruckNodeFrmAttr.HuiZong, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 累加表单方案
        /// </summary>
        public FoolTruckNodeFrm() { }
        /// <summary>
        /// 累加表单方案
        /// </summary>
        /// <param name="mypk"></param>
        public FoolTruckNodeFrm(string mypk)
            : base(mypk)
        {
        }
        /// <summary>
        /// 累加表单方案
        /// </summary>
        /// <param name="fk_node">节点</param>
        /// <param name="fk_frm">表单</param>
        public FoolTruckNodeFrm(string fk_flow, int fk_node, string fk_frm)
        {
            int i = this.Retrieve(FoolTruckNodeFrmAttr.FK_Node, fk_node, FoolTruckNodeFrmAttr.FK_Frm, fk_frm);
            if (i == 0)
            {
                this.IsPrint = false;
                //不可以编辑.
                this.FrmSln = 1;
               // this.IsEdit = false;
                return;
                throw new Exception("@表单关联信息已被删除。");
            }
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

                Map map = new Map("WF_NodeFrm", "累加表单方案");

                map.AddMyPK();

                map.AddTBInt(FoolTruckNodeFrmAttr.FK_Node, 0, "要作用的节点ID", true, false);
                map.AddTBString(FoolTruckNodeFrmAttr.FK_Frm, null, "表单ID", true, true, 1, 200, 200);
                map.AddTBInt(FoolTruckNodeFrmAttr.FrmSln, 0, "表单控制方案", true, false);


                //菜单在本节点的权限控制.
                map.AddTBInt(FoolTruckNodeFrmAttr.IsEnableLoadData, 0, "是否启用装载填充事件", true, false);

                //是否显示
                map.AddTBInt(FoolTruckNodeFrmAttr.IsEnable,1,"是否显示",true,false);
                map.AddTBString(FoolTruckNodeFrmAttr.FK_Flow, null, "流程编号", true, true, 1, 20, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法.
        protected override bool beforeUpdateInsertAction()
        {
            if (this.FK_Frm.Length == 0)
                throw new Exception("@表单编号为空");

            if (this.FK_Node == 0)
                throw new Exception("@节点ID为空");

            if (this.FK_Flow.Length == 0)
                throw new Exception("@流程编号为空");

            this.MyPK = this.FK_Frm + "_" + this.FK_Node + "_" + this.FK_Flow;
            return base.beforeUpdateInsertAction();
        }
        #endregion 方法.
    }
    /// <summary>
    /// 累加表单方案s
    /// </summary>
    public class FoolTruckNodeFrms : EntitiesMyPK
    {
        #region 属性.
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (FoolTruckNodeFrm ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        #endregion 属性.

        #region 构造方法..
        /// <summary>
        /// 累加表单方案
        /// </summary>
        public FoolTruckNodeFrms() { }
        /// <summary>
        /// 累加表单方案
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public FoolTruckNodeFrms(string fk_flow, int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FoolTruckNodeFrmAttr.FK_Flow, fk_flow);
            qo.addAnd();
            qo.AddWhere(FoolTruckNodeFrmAttr.FK_Node, nodeID);

            qo.addOrderBy(FoolTruckNodeFrmAttr.Idx);
            qo.DoQuery();
        }
        /// <summary>
        /// 累加表单方案
        /// </summary>
        /// <param name="NodeNo">NodeNo </param>
        public FoolTruckNodeFrms(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FoolTruckNodeFrmAttr.FK_Node, nodeID);
            qo.addOrderBy(FoolTruckNodeFrmAttr.Idx);
            qo.DoQuery();
        }
        #endregion 构造方法..

        #region 公共方法.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FoolTruckNodeFrm();
            }
        }
        /// <summary>
        /// 累加表单方案s
        /// </summary>
        /// <param name="sts">累加表单方案</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Nodes sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Node st in sts)
            {
                tmp = this.GetHisNodes(st.No);
                foreach (Node nd in tmp)
                {
                    if (nds.Contains(nd))
                        continue;
                    nds.AddEntity(nd);
                }
            }
            return nds;
        }
        /// <summary>
        /// 累加表单方案
        /// </summary>
        /// <param name="NodeNo">工作节点编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FoolTruckNodeFrmAttr.FK_Node, NodeNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FoolTruckNodeFrm en in this)
            {
                ens.AddEntity(new Node(en.FK_Frm));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
        public Nodes GetHisNodes(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FoolTruckNodeFrmAttr.FK_Frm, nodeID);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FoolTruckNodeFrm en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FoolTruckNodeFrm> ToJavaList()
        {
            return (System.Collections.Generic.IList<FoolTruckNodeFrm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FoolTruckNodeFrm> Tolist()
        {
            System.Collections.Generic.List<FoolTruckNodeFrm> list = new System.Collections.Generic.List<FoolTruckNodeFrm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FoolTruckNodeFrm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
