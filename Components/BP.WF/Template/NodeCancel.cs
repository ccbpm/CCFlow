using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.Port;
//using BP.ZHZS.Base;

namespace BP.WF.Template
{
    /// <summary>
    /// 可撤销的节点属性	  
    /// </summary>
    public class NodeCancelAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 撤销到
        /// </summary>
        public const string CancelTo = "CancelTo";
    }
    /// <summary>
    /// 可撤销的节点
    /// 节点的撤销到有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class NodeCancel : EntityMM
    {
        #region 基本属性
        /// <summary>
        ///撤销到
        /// </summary>
        public int CancelTo
        {
            get
            {
                return this.GetValIntByKey(NodeCancelAttr.CancelTo);
            }
            set
            {
                this.SetValByKey(NodeCancelAttr.CancelTo, value);
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(NodeCancelAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeCancelAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 可撤销的节点
        /// </summary>
        public NodeCancel() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeCancel", "可撤销的节点");
                map.IndexField = NodeEmpAttr.FK_Node;

                 

                map.AddTBIntPK(NodeCancelAttr.FK_Node, 0, "节点", true, true);
                map.AddTBIntPK(NodeCancelAttr.CancelTo, 0, "撤销到", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 可撤销的节点
    /// </summary>
    public class NodeCancels : EntitiesMM
    {
        #region 构造与属性.
        /// <summary>
        /// 他的撤销到
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (NodeCancel ns in this)
                {
                    ens.AddEntity(new Node(ns.CancelTo));
                }
                return ens;
            }
        }
        /// <summary>
        /// 可撤销的节点
        /// </summary>
        public NodeCancels() { }
        /// <summary>
        /// 可撤销的节点
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeCancels(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeCancelAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 可撤销的节点
        /// </summary>
        /// <param name="NodeNo">NodeNo </param>
        public NodeCancels(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeCancelAttr.CancelTo, NodeNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeCancel();
            }
        }
        #endregion 构造与属性.

        #region 公共方法.
        /// <summary>
        /// 可撤销的节点s
        /// </summary>
        /// <param name="sts">可撤销的节点</param>
        /// <Cancels></Cancels>
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
        /// 可撤销的节点
        /// </summary>
        /// <param name="NodeNo">撤销到编号</param>
        /// <Cancels>节点s</Cancels>
        public Nodes GetHisNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeCancelAttr.CancelTo, NodeNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (NodeCancel en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <Cancels>转向此节点的集合的Nodes (FromNodes)</Cancels> 
        public Nodes GetHisNodes(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeCancelAttr.FK_Node, nodeID);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (NodeCancel en in this)
            {
                ens.AddEntity(new Node(en.CancelTo));
            }
            return ens;
        }
        #endregion 公共方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeCancel> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeCancel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeCancel> Tolist()
        {
            System.Collections.Generic.List<NodeCancel> list = new System.Collections.Generic.List<NodeCancel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeCancel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
