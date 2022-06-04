using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 节点人员属性
	/// </summary>
	public class NodeEmpAttr
	{
		/// <summary>
		/// 节点
		/// </summary>
		public const string FK_Node="FK_Node";
		/// <summary>
		/// 人员
		/// </summary>
		public const string FK_Emp="FK_Emp";
	}
	/// <summary>
	/// 节点人员
	/// 节点的到人员有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
	public class NodeEmp :EntityMM
	{
		#region 基本属性
		/// <summary>
		///节点
		/// </summary>
		public int  FK_Node
		{
			get
			{
				return this.GetValIntByKey(NodeEmpAttr.FK_Node);
			}
			set
			{
				this.SetValByKey(NodeEmpAttr.FK_Node,value);
			}
		}
		/// <summary>
		/// 到人员
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(NodeEmpAttr.FK_Emp);
			}
			set
			{
				this.SetValByKey(NodeEmpAttr.FK_Emp,value);
			}
		}
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(NodeEmpAttr.FK_Emp);
            }
        }
		#endregion 

		#region 构造方法
		/// <summary>
		/// 节点人员
		/// </summary>
		public NodeEmp()
        {
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

                Map map = new Map("WF_NodeEmp", "节点人员");
                map.IndexField = NodeEmpAttr.FK_Node;

                map.AddTBIntPK(NodeEmpAttr.FK_Node,0,"Node",true,true);
                map.AddDDLEntitiesPK(NodeEmpAttr.FK_Emp, null, "到人员", new BP.Port.Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
		}
        #endregion
        protected override bool beforeInsert()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == BP.Sys.CCBPMRunModel.SAAS)
                this.FK_Emp = Web.WebUser.OrgNo + "_" + this.FK_Emp;
            return base.beforeInsert();
        }

    }
	/// <summary>
	/// 节点人员
	/// </summary>
    public class NodeEmps : EntitiesMM
    {
        /// <summary>
        /// 节点人员
        /// </summary>
        public NodeEmps() { }
        /// <summary>
        /// 节点人员
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public NodeEmps(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeEmpAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点人员
        /// </summary>
        /// <param name="EmpNo">EmpNo </param>
        public NodeEmps(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NodeEmpAttr.FK_Emp, EmpNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NodeEmp();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<NodeEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<NodeEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<NodeEmp> Tolist()
        {
            System.Collections.Generic.List<NodeEmp> list = new System.Collections.Generic.List<NodeEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((NodeEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
