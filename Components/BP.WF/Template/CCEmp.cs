using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;
//using BP.ZHZS.Base;

namespace BP.WF.Template
{
	/// <summary>
	/// 节点到人员属性
	/// </summary>
	public class CCEmpAttr
	{
		/// <summary>
		/// 节点
		/// </summary>
		public const string FK_Node="FK_Node";
		/// <summary>
		/// 到人员
		/// </summary>
		public const string FK_Emp="FK_Emp";
	}
	/// <summary>
	/// 节点到人员
	/// 节点的到人员有两部分组成.	 
	/// 记录了从一个节点到其他的多个节点.
	/// 也记录了到这个节点的其他的节点.
	/// </summary>
	public class CCEmp :EntityMM
	{
		#region 基本属性
		/// <summary>
		///节点
		/// </summary>
		public int  FK_Node
		{
			get
			{
				return this.GetValIntByKey(CCEmpAttr.FK_Node);
			}
			set
			{
				this.SetValByKey(CCEmpAttr.FK_Node,value);
			}
		}
		/// <summary>
		/// 到人员
		/// </summary>
		public string FK_Emp
		{
			get
			{
				return this.GetValStringByKey(CCEmpAttr.FK_Emp);
			}
			set
			{
				this.SetValByKey(CCEmpAttr.FK_Emp,value);
			}
		}
        public string FK_EmpT
        {
            get
            {
                return this.GetValRefTextByKey(CCEmpAttr.FK_Emp);
            }
        }
		#endregion 

		#region 构造方法
		/// <summary>
		/// 节点到人员
		/// </summary>
		public CCEmp(){}
		/// <summary>
		/// 重写基类方法
		/// </summary>
		public override Map EnMap
		{
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_CCEmp", "抄送人员");
                map.IndexField = CCEmpAttr.FK_Node;

                map.AddTBIntPK(CCEmpAttr.FK_Node, 0, "节点", true, true);
                map.AddDDLEntitiesPK(CCEmpAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion
	}
	/// <summary>
	/// 节点到人员
	/// </summary>
    public class CCEmps : EntitiesMM
    {
        /// <summary>
        /// 他的到人员
        /// </summary>
        public Emps HisEmps
        {
            get
            {
                Emps ens = new Emps();
                foreach (CCEmp ns in this)
                {
                    ens.AddEntity(new Emp(ns.FK_Emp));
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
                foreach (CCEmp ns in this)
                {
                    ens.AddEntity(new Node(ns.FK_Node));
                }
                return ens;

            }
        }
        /// <summary>
        /// 节点到人员
        /// </summary>
        public CCEmps() { }
        /// <summary>
        /// 节点到人员
        /// </summary>
        /// <param name="NodeID">节点ID</param>
        public CCEmps(int NodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCEmpAttr.FK_Node, NodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 节点到人员
        /// </summary>
        /// <param name="EmpNo">EmpNo </param>
        public CCEmps(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCEmpAttr.FK_Emp, EmpNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCEmp();
            }
        }
        /// <summary>
        /// 取到一个到人员集合能够访问到的节点s
        /// </summary>
        /// <param name="sts">到人员集合</param>
        /// <returns></returns>
        public Nodes GetHisNodes(Emps sts)
        {
            Nodes nds = new Nodes();
            Nodes tmp = new Nodes();
            foreach (Emp st in sts)
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
        /// 到人员对应的节点
        /// </summary>
        /// <param name="EmpNo">到人员编号</param>
        /// <returns>节点s</returns>
        public Nodes GetHisNodes(string EmpNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCEmpAttr.FK_Emp, EmpNo);
            qo.DoQuery();

            Nodes ens = new Nodes();
            foreach (CCEmp en in this)
            {
                ens.AddEntity(new Node(en.FK_Node));
            }
            return ens;
        }
        /// <summary>
        /// 转向此节点的集合的 Nodes
        /// </summary>
        /// <param name="nodeID">此节点的ID</param>
        /// <returns>转向此节点的集合的Nodes (FromNodes)</returns> 
        public Emps GetHisEmps(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(CCEmpAttr.FK_Node, nodeID);
            qo.DoQuery();

            Emps ens = new Emps();
            foreach (CCEmp en in this)
            {
                ens.AddEntity(new Emp(en.FK_Emp));
            }
            return ens;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCEmp> Tolist()
        {
            System.Collections.Generic.List<CCEmp> list = new System.Collections.Generic.List<CCEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
