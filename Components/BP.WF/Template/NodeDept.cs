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
		public const string FK_Node="FK_Node";
		/// <summary>
		/// 工作部门
		/// </summary>
		public const string FK_Dept="FK_Dept";
	}
	/// <summary>
	/// 节点部门
	/// 节点的工作部门有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
	public class NodeDept :EntityMM
	{
		#region 基本属性
		/// <summary>
		///节点
		/// </summary>
		public int  FK_Node
		{
			get
			{
				return this.GetValIntByKey(NodeDeptAttr.FK_Node);
			}
			set
			{
				this.SetValByKey(NodeDeptAttr.FK_Node,value);
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
				this.SetValByKey(NodeDeptAttr.FK_Dept,value);
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
		public NodeDept(){}
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;

                Map map = new Map("WF_NodeDept", "节点部门");			 

				map.DepositaryOfEntity=Depositary.None;
				map.DepositaryOfMap=Depositary.Application;

                map.IndexField = NodeEmpAttr.FK_Node;


                map.AddTBIntPK(NodeStationAttr.FK_Node, 0, "节点", false, false);

                //map.AddDDLEntitiesPK(NodeDeptAttr.FK_Node,0,DataType.AppInt,"节点",new Nodes(),
                //    NodeAttr.NodeID,NodeAttr.Name,true);

				map.AddDDLEntitiesPK( NodeDeptAttr.FK_Dept,null,"部门",new BP.Port.Depts(),true);

				this._enMap=map;
				 
				return this._enMap;
			}
		}
		#endregion

	}
	/// <summary>
	/// 节点部门
	/// </summary>
    public class NodeDepts : EntitiesMM
    {
        /// <summary>
        /// 他的工作部门
        /// </summary>
        public Stations HisStations
        {
            get
            {
                Stations ens = new Stations();
                foreach (NodeDept ns in this)
                {
                    ens.AddEntity(new Station(ns.FK_Dept));
                }
                return ens;
            }
        }
        /// <summary>
        /// 他的工作节点
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                Nodes ens = new Nodes();
                foreach (NodeDept ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;

            }
        }
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
        /// 节点部门
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public NodeDepts(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeDeptAttr.FK_Dept, StationNo);
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
        /// <summary>
        /// 取到一个工作部门集合能够访问到的节点s
        /// </summary>
        /// <param name="sts">工作部门集合</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Stations sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Station st in sts)
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
        /// 工作部门对应的节点
        /// </summary>
        /// <param name="stationNo">工作部门编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string stationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeDeptAttr.FK_Dept, stationNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (NodeDept en in this)
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
        public Stations GetHisStations(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeDeptAttr.FK_Node, nodeID);
            qo.DoQuery();

            Stations ens = new Stations();
            foreach (NodeDept en in this)
            {
                ens.AddEntity(new Station(en.FK_Dept));
            }
            return ens;
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
