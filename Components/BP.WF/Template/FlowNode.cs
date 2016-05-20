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
    /// 属性  
    /// </summary>
    public class FlowNodeAttr
    {
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Node = "FK_Node";
    }
    /// <summary>
    /// 流程节点
    /// </summary>
    public class FlowNode : EntityMM
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
        ///节点
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FlowNodeAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FlowNodeAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FlowNodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(FlowNodeAttr.FK_Flow, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 流程岗位属性
        /// </summary>
        public FlowNode() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FlowNode", "流程抄送节点");
                 

                map.AddTBStringPK(FlowNodeAttr.FK_Flow, null, "流程编号", true, true, 1, 20, 20);
                map.AddTBStringPK(FlowNodeAttr.FK_Node, null, "节点", true, true, 1, 20, 20);

                //      map.AddDDLEntitiesPK(FlowNodeAttr.FK_Flow, null, "FK_Flow", new Flows(), true);
                //     map.AddDDLEntitiesPK(FlowNodeAttr.FK_Node, null, "工作节点", new Nodes(), true);
                this._enMap = map;
                

                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 属性
    /// </summary>
    public class FlowNodes : EntitiesMM
    {
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (FlowNode ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;
            }
        }
        /// <summary>
        /// 流程抄送节点
        /// </summary>
        public FlowNodes() { }
        /// <summary>
        /// 流程抄送节点
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public FlowNodes(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowNodeAttr.FK_Flow, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 流程抄送节点
        /// </summary>
        /// <param name="NodeNo">NodeNo </param>
        public FlowNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowNodeAttr.FK_Node, NodeNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowNode();
            }
        }
        /// <summary>
        /// 流程抄送节点s
        /// </summary>
        /// <param name="sts">流程抄送节点</param>
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
        /// 流程抄送节点
        /// </summary>
        /// <param name="NodeNo">工作节点编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string NodeNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowNodeAttr.FK_Node, NodeNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FlowNode en in this)
            {
                ens.AddEntity(new Node(en.FK_Flow));
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
            qo.AddWhere(FlowNodeAttr.FK_Flow, nodeID);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (FlowNode en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowNode> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowNode>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowNode> Tolist()
        {
            System.Collections.Generic.List<FlowNode> list = new System.Collections.Generic.List<FlowNode>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowNode)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
