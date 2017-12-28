using System;
using System.Collections;
using BP.DA;
using BP.En;
//using BP.ZHZS.Base;
using BP;
namespace BP.Sys
{
	/// <summary>
	/// abc_afs
	/// </summary>
    public class GroupEnsTemplateAttr : EntityOIDAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 实体名称
        /// </summary>
        public const string EnName = "EnName";
        /// <summary>
        /// 属性
        /// </summary> 
        public const string Attrs = "Attrs";
        /// <summary>
        /// 操作列
        /// </summary>
        public const string OperateCol = "OperateCol";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// EnsName
        /// </summary>
        public const string EnsName = "EnsName";
    }
	/// <summary>
	/// 报表模板
	/// </summary>
	public class GroupEnsTemplate: EntityOID
	{
		#region 基本属性
		/// <summary>
		/// 集合类名称
		/// </summary>
		public string EnsName
		{
			get
			{
				return this.GetValStringByKey(GroupEnsTemplateAttr.EnsName) ; 
			}
			set
			{
				this.SetValByKey(GroupEnsTemplateAttr.EnsName,value) ; 
			}		
		}
		/// <summary>
		/// 实体名称
		/// </summary>
		public string OperateCol
		{
			get
			{
				return this.GetValStringByKey(GroupEnsTemplateAttr.OperateCol ) ; 
			}
			set
			{
				this.SetValByKey(GroupEnsTemplateAttr.OperateCol,value) ; 
			}
		}
		/// <summary>
		/// 数据源
		/// </summary>
		public string Attrs
		{
			get
			{
				return this.GetValStringByKey(GroupEnsTemplateAttr.Attrs ) ; 
			}
			set
			{
				this.SetValByKey(GroupEnsTemplateAttr.Attrs,value) ; 
			}
		}
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get
			{
				return this.GetValStringByKey(GroupEnsTemplateAttr.Name ) ; 
			}
			set
			{
				this.SetValByKey(GroupEnsTemplateAttr.Name,value) ; 
			}
		}
		public string EnName
		{
			get
			{
				return this.GetValStringByKey(GroupEnsTemplateAttr.EnName ) ; 
			}
			set
			{
				this.SetValByKey(GroupEnsTemplateAttr.EnName,value) ; 
			}
		}
		public string Rec
		{
			get
			{
				return this.GetValStringByKey(GroupEnsTemplateAttr.Rec ) ; 
			}
			set
			{
				this.SetValByKey(GroupEnsTemplateAttr.Rec,value) ; 
			}
		}
		 
		#endregion

		#region 构造方法

		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				uac.IsUpdate=true;
				uac.IsView=true;
				return base.HisUAC;
			}
		}

		/// <summary>
		/// 系统实体
		/// </summary>
		public GroupEnsTemplate()
		{
		}
       
		/// <summary>
		/// EnMap
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
                Map map = new Map("Sys_GroupEnsTemplate", "报表模板");
				map.DepositaryOfEntity=Depositary.None;
				map.EnType=EnType.Sys;

				map.AddTBIntPKOID();
				map.AddTBString(GroupEnsTemplateAttr.EnName,null,"表称",false,false,0,500,20);
				map.AddTBString(GroupEnsTemplateAttr.Name,null,"报表名",true,false,0,500,20);
				map.AddTBString(GroupEnsTemplateAttr.EnsName,null,"报表类名",false,true,0,90,10);
				map.AddTBString(GroupEnsTemplateAttr.OperateCol,null,"操作属性",false,true,0,90,10);
				map.AddTBString(GroupEnsTemplateAttr.Attrs,null,"运算属性",false,true,0,90,10);
				map.AddTBString(GroupEnsTemplateAttr.Rec,null,"记录人",false,true,0,90,10);
				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion 

		#region 查询方法
		/// <summary>
		/// 报表模板
		/// </summary>
		/// <param name="fk_emp">fk_emp</param>
		/// <param name="className">className</param>
		/// <param name="attrs">attrs</param>
		/// <returns>查询返回个数</returns>
		public int Search(string fk_emp, string className, string attrs)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(GroupEnsTemplateAttr.Rec, fk_emp);
			qo.addAnd();
			qo.AddWhere(GroupEnsTemplateAttr.Attrs, className);
			qo.addAnd();
			qo.AddWhere(GroupEnsTemplateAttr.EnsName, className);
			return qo.DoQuery();
		}
		#endregion
	}	
	/// <summary>
	/// 实体集合
	/// </summary>
	public class GroupEnsTemplates : EntitiesOID
	{		
		#region 构造
		public GroupEnsTemplates()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="emp"></param>
		public GroupEnsTemplates(string emp)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(GroupEnsTemplateAttr.Rec, emp);
			qo.addOr();
			qo.AddWhere(GroupEnsTemplateAttr.Rec,"admin");
			qo.DoQuery();

		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity 
		{
			get
			{
				return new GroupEnsTemplate();
			}

		}
		#endregion

		#region 查询方法
		 
		#endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupEnsTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupEnsTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupEnsTemplate> Tolist()
        {
            System.Collections.Generic.List<GroupEnsTemplate> list = new System.Collections.Generic.List<GroupEnsTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupEnsTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}
