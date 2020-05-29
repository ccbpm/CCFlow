using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.WF.Template;

namespace BP.WF.Template
{
	/// <summary>
	/// 节点方向属性
	/// </summary>
	public class DirectionAttr
	{
		/// <summary>
		/// 节点
		/// </summary>
		public const string Node="Node";
		/// <summary>
		/// 转向的节点
		/// </summary>
		public const string ToNode="ToNode";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
	}
	/// <summary>
	/// 节点方向
	/// 节点的方向有两部分组成.
	/// 1, Node.
	/// 2, toNode.
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
	public class Direction :EntityMyPK
	{
		#region 基本属性
		/// <summary>
		///节点
		/// </summary>
        public int Node
        {
            get
            {
                return this.GetValIntByKey(DirectionAttr.Node);
            }
            set
            {
                this.SetValByKey(DirectionAttr.Node, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(DirectionAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(DirectionAttr.FK_Flow, value);
            }
        }
		/// <summary>
		/// 转向的节点
		/// </summary>
		public int ToNode
		{
			get
			{
				return this.GetValIntByKey(DirectionAttr.ToNode);
			}
			set
			{
				this.SetValByKey(DirectionAttr.ToNode,value);
			}
		}
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(DirectionAttr.Idx);
            }
            set
            {
                this.SetValByKey(DirectionAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点方向
        /// </summary>
        public Direction(){}
        public Direction(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;

                Map map = new Map("WF_Direction", "节点方向信息");

                map.IndexField = DirectionAttr.FK_Flow;

                /*
                 * MyPK 是一个复合主键 是由 Node+'_'+ToNode+'_'+DirType 组合的. 比如: 101_102_1
                 */
                map.AddMyPK();
                map.AddTBString(DirectionAttr.FK_Flow, null, "流程", true, true, 0, 10, 0, false);
                map.AddTBInt(DirectionAttr.Node, 0, "从节点", false, true);
				map.AddTBInt( DirectionAttr.ToNode,0,"到节点",false,true);

                //map.AddTBInt(DirectionAttr.CondExpModel, 0, "条件计算方式", false, true);
                map.AddTBInt(DirectionAttr.Idx, 0, "计算优先级顺序", true, true);


                //相关功能。
                map.AttrsOfOneVSM.Add(new BP.WF.Template.DirectionStations(), new BP.Port.Stations(),
                    NodeStationAttr.FK_Node, NodeStationAttr.FK_Station,
                    StationAttr.Name, StationAttr.No, "方向条件与岗位");

                //map.AttrsOfOneVSM.Add(new BP.WF.Template.NodeDepts(), new BP.WF.Port.Depts(), NodeDeptAttr.FK_Node, NodeDeptAttr.FK_Dept, DeptAttr.Name,
                //DeptAttr.No, "节点部门", Dot2DotModel.TreeDept);


				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion

        /// <summary>
        /// 处理pk 
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_Flow+"_" +this.Node + "_" + this.ToNode;
            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            this.MyPK = this.FK_Flow + "_" + this.Node + "_" + this.ToNode;
            return base.beforeDelete();
        }
        /// <summary>
        /// 上移
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(DirectionAttr.Node, this.Node.ToString(), DirectionAttr.Idx);
        }
        /// <summary>
        /// 下移
        /// </summary>
        public void DoDown()
        {
            this.DoOrderDown(DirectionAttr.Node,this.Node.ToString(), DirectionAttr.Idx);
        }
    }
	 /// <summary>
	 /// 节点方向
	 /// </summary>
	public class Directions :En.Entities
	{
		/// <summary>
		/// 节点方向
		/// </summary>
		public Directions(){}
        /// <summary>
        /// 方向
        /// </summary>
        /// <param name="flowNo"></param>
        public Directions(string flowNo)
        {
            this.Retrieve(DirectionAttr.FK_Flow, flowNo);
        }
        /// <summary>
        /// 节点方向
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public Directions(int NodeID)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(DirectionAttr.Node,NodeID);    
			qo.addOrderBy(DirectionAttr.Idx);  //方向条件的优先级. @sly
		    qo.DoQuery();			
		}
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new Direction();
			}
		}
		/// <summary>
		/// 此节点的转向方向集合
		/// </summary>
		/// <param name="nodeID">此节点的ID</param>
		/// <param name="isLifecyle">是不是判断在节点的生存期内</param>		 
		/// <returns>转向方向集合(ToNodes)</returns> 
		public Nodes GetHisToNodes(int nodeID, bool isLifecyle)
		{
			Nodes nds = new Nodes();
			QueryObject qo = new QueryObject(nds);
			qo.AddWhereInSQL(NodeAttr.NodeID,"SELECT ToNode FROM WF_Direction WHERE Node="+nodeID );
			qo.DoQuery();
			return nds;
		}
		/// <summary>
		/// 转向此节点的集合的Nodes
		/// </summary>
		/// <param name="nodeID">此节点的ID</param>
		/// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
		public Nodes GetHisFromNodes(int nodeID)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(DirectionAttr.ToNode,nodeID);
			qo.DoQuery();
			Nodes ens = new Nodes();
			foreach(Direction en in this)
			{
				ens.AddEntity( new Node(en.Node) ) ;
			}
			return ens;
		}

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Direction> ToJavaList()
        {
            return (System.Collections.Generic.IList<Direction>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Direction> Tolist()
        {
            System.Collections.Generic.List<Direction> list = new System.Collections.Generic.List<Direction>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Direction)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
		 
	}
}
