using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 流程对应组织属性
	/// </summary>
	public class FlowOrgAttr
    {
		/// <summary>
		/// 流程
		/// </summary>
		public const string FlowNo="FlowNo";
		/// <summary>
		/// 组织
		/// </summary>
		public const string OrgNo="OrgNo";
	}
	/// <summary>
	/// 流程对应组织
	/// </summary>
	public class FlowOrg :EntityMM
	{
		#region 基本属性
		/// <summary>
		///流程
		/// </summary>
		public string FlowNo
		{
			get
			{
				return this.GetValStringByKey(FlowOrgAttr.FlowNo);
			}
			set
			{
				this.SetValByKey(FlowOrgAttr.FlowNo,value);
			}
		}
		/// <summary>
		/// 组织
		/// </summary>
		public string OrgNo
		{
			get
			{
				return this.GetValStringByKey(FlowOrgAttr.OrgNo);
			}
			set
			{
				this.SetValByKey(FlowOrgAttr.OrgNo,value);
			}
		}
        public string OrgNoT
        {
            get
            {
                return this.GetValRefTextByKey(FlowOrgAttr.OrgNo);
            }
        }
		#endregion 

		#region 构造方法
		/// <summary>
		/// 流程对应组织
		/// </summary>
		public FlowOrg()
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

                Map map = new Map("WF_FlowOrg", "流程对应组织");
                map.IndexField = FlowOrgAttr.FlowNo;

                map.AddTBStringPK(FlowOrgAttr.FlowNo,null,"流程",true,true,1,100,100);
                map.AddDDLEntitiesPK(FlowOrgAttr.OrgNo, null, "到组织", new BP.WF.Port.Admin2.Orgs(),
                    true);

                this._enMap = map;
                return this._enMap;
            }
		}
		#endregion
	}
	/// <summary>
	/// 流程对应组织
	/// </summary>
    public class FlowOrgs : EntitiesMM
    {
        /// <summary>
        /// 流程对应组织
        /// </summary>
        public FlowOrgs() { }
        /// <summary>
        /// 流程对应组织
        /// </summary>
        /// <param name="orgNo">orgNo </param>
        public FlowOrgs(string orgNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FlowOrgAttr.OrgNo, orgNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowOrg();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowOrg> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowOrg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowOrg> Tolist()
        {
            System.Collections.Generic.List<FlowOrg> list = new System.Collections.Generic.List<FlowOrg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowOrg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
