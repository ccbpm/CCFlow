using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点部门属性	  
    /// </summary>
    public class NodeDeptAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 工作部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
    }
    /// <summary>
    /// 节点部门
    /// 节点的工作部门有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class NodeDept : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(NodeDeptAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeDeptAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(NodeDeptAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(NodeDeptAttr.FK_Dept, value);
            }
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return base.HisUAC;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点部门
        /// </summary>
        public NodeDept() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeDept", "节点部门");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.IndexField = NodeEmpAttr.FK_Node;

                map.AddMyPK();
                map.AddTBInt(NodeStationAttr.FK_Node, 0, "节点", false, false);
                map.AddDDLEntities(NodeDeptAttr.FK_Dept, null, "部门", new BP.Port.Depts(), true);

                //            map.AddTBIntPK(NodeStationAttr.FK_Node, 0, "节点", false, false);
                //map.AddDDLEntitiesPK( NodeDeptAttr.FK_Dept,null,"部门",new BP.Port.Depts(),true);

                this._enMap = map;

                return this._enMap;
            }
        }
        #endregion


        protected override bool beforeUpdateInsertAction()
        {
            this.setMyPK(this.FK_Node + "_" + this.FK_Dept);
            return base.beforeUpdateInsertAction();
        }

    }
    /// <summary>
    /// 节点部门
    /// </summary>
    public class NodeDepts : EntitiesMyPK
    {
        /// <summary>
        /// 节点部门
        /// </summary>
        public NodeDepts() { }
        /// <summary>
        /// 节点部门
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeDepts(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeDeptAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeDept();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeDept> Tolist()
        {
            System.Collections.Generic.List<NodeDept> list = new System.Collections.Generic.List<NodeDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
