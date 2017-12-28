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
    /// 可退回的节点属性	  
    /// </summary>
    public class NodeReturnAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 退回到
        /// </summary>
        public const string ReturnTo = "ReturnTo";
        /// <summary>
        /// 中间点
        /// </summary>
        public const string Dots = "Dots";
    }
    /// <summary>
    /// 可退回的节点
    /// 节点的退回到有两部分组成.	 
    /// 记录了从一个节点到其他的多个节点.
    /// 也记录了到这个节点的其他的节点.
    /// </summary>
    public class NodeReturn : EntityMM
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
        ///退回到
        /// </summary>
        public int ReturnTo
        {
            get
            {
                return this.GetValIntByKey(NodeReturnAttr.ReturnTo);
            }
            set
            {
                this.SetValByKey(NodeReturnAttr.ReturnTo, value);
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(NodeReturnAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(NodeReturnAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 可退回的节点
        /// </summary>
        public NodeReturn() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeReturn", "可退回的节点");

                map.AddTBIntPK(NodeReturnAttr.FK_Node, 0, "节点", true, true);
                map.AddTBIntPK(NodeReturnAttr.ReturnTo, 0, "退回到", true, true);
                map.AddTBString(NodeReturnAttr.Dots, null, "轨迹信息", true, true,0,300,0,false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 可退回的节点
    /// </summary>
    public class NodeReturns : EntitiesMM
    {
        /// <summary>
        /// 他的退回到
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (NodeReturn ns in this)
                {
                    ens.AddEntity(new Node(ns.ReturnTo));
                }
                return ens;
            }
        }
        /// <summary>
        /// 可退回的节点
        /// </summary>
        public NodeReturns() { }
        /// <summary>
        /// 可退回的节点
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeReturns(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeReturnAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 可退回的节点
        /// </summary>
        /// <param name="NodeNo">NodeNo </param>
        public NodeReturns(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeReturnAttr.ReturnTo, NodeNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeReturn();
            }
        }
        /// <summary>
        /// 可退回的节点s
        /// </summary>
        /// <param name="sts">可退回的节点</param>
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
        /// 可退回的节点
        /// </summary>
        /// <param name="NodeNo">退回到编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeReturnAttr.ReturnTo, NodeNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (NodeReturn en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
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
            qo.AddWhere(NodeReturnAttr.FK_Node, nodeID);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (NodeReturn en in this)
            {
                ens.AddEntity(new Node(en.ReturnTo));
            }
            return ens;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeReturn> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeReturn>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeReturn> Tolist()
        {
            System.Collections.Generic.List<NodeReturn> list = new System.Collections.Generic.List<NodeReturn>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeReturn)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
