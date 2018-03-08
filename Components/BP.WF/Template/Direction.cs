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
        /// 方向类型
        /// </summary>
        public const string DirType = "DirType";
        /// <summary>
        /// 是否可以原路返回
        /// </summary>
        public const string IsCanBack = "IsCanBack";
        /// <summary>
        /// 折线信息
        /// </summary>
        public const string Dots = "Dots";
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
        public int DirType
        {
            get
            {
                return this.GetValIntByKey(DirectionAttr.DirType);
            }
            set
            {
                this.SetValByKey(DirectionAttr.DirType, value);
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
		public int  ToNode
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
        public bool IsCanBack
        {
            get
            {
                return this.GetValBooleanByKey(DirectionAttr.IsCanBack);
            }
            set
            {
                this.SetValByKey(DirectionAttr.IsCanBack, value);
            }
        }
        public string Dots
        {
            get
            {
                return this.GetValStringByKey(DirectionAttr.Dots);
            }
            set
            {
                this.SetValByKey(DirectionAttr.Dots, value);
            }
        }
		#endregion 

		#region 构造方法
		/// <summary>
		/// 节点方向
		/// </summary>
		public Direction(){}
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

                /*
                 * MyPK 是一个复合主键 是由 Node+'_'+ToNode+'_'+DirType 组合的. 比如: 101_102_1
                 */
                map.AddMyPK();
                map.AddTBString(DirectionAttr.FK_Flow, null, "流程", true, true, 0, 10, 0, false);
                map.AddTBInt(DirectionAttr.Node, 0, "从节点", false, true);
				map.AddTBInt( DirectionAttr.ToNode,0,"到节点",false,true);
                map.AddTBInt(DirectionAttr.DirType, 0, "类型0前进1返回", false, true);
                map.AddTBInt(DirectionAttr.IsCanBack, 0, "是否可以原路返回(对后退线有效)", false, true);
                /*
                 * Dots 存储格式为: @x1,y1@x2,y2
                 */
                map.AddTBString(NodeReturnAttr.Dots, null, "轨迹信息", true, true, 0, 300, 0, false);

                //相关功能。
                map.AttrsOfOneVSM.Add(new BP.WF.Template.DirectionStations(), new BP.WF.Port.Stations(),
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
            this.MyPK = this.FK_Flow+"_" +this.Node + "_" + this.ToNode + "_" + this.DirType;
            return base.beforeInsert();
        }
        protected override bool beforeDelete()
        {
            this.MyPK = this.FK_Flow + "_" + this.Node + "_" + this.ToNode + "_" + this.DirType;
            return base.beforeDelete();
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
        /// <param name="dirType">类型</param>
        public Directions(int NodeID, int dirType)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(DirectionAttr.Node,NodeID);
            qo.addAnd();
            qo.AddWhere(DirectionAttr.DirType, dirType);
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
