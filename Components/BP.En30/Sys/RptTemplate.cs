using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
	/// <summary>
	/// 百分比显示方式
	/// </summary>
	public enum PercentModel
	{
		/// <summary>
		/// 不显示
		/// </summary>
		None,
		/// <summary>
		/// 纵向
		/// </summary>
		Vertical,
		/// <summary>
		/// 横向
		/// </summary>
		Transverse,
	}
	/// <summary>
	/// RptTemplateAttr
	/// </summary>
    public class RptTemplateAttr : EntityOIDAttr
    {
        /// <summary>
        /// 类名称
        /// </summary>
        public const string EnsName = "EnsName";
        /// <summary>
        /// 描述
        /// </summary>
        public const string MyPK = "MyPK";
        /// <summary>
        /// D1
        /// </summary> 
        public const string D1 = "D1";
        /// <summary>
        /// d2
        /// </summary>
        public const string D2 = "D2";
        /// <summary>
        /// d3
        /// </summary>
        public const string D3 = "D3";
        /// <summary>
        /// 要分析的对象s
        /// </summary>
        public const string AlObjs = "AlObjs";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Height = "Height";
        /// <summary>
        /// EnsName
        /// </summary>
        public const string Width = "Width";
        /// <summary>
        /// 是否显示大合计
        /// </summary>
        public const string IsSumBig = "IsSumBig";
        /// <summary>
        /// 是否显示小合计
        /// </summary>
        public const string IsSumLittle = "IsSumLittle";
        /// <summary>
        /// 是否显示右合计
        /// </summary>
        public const string IsSumRight = "IsSumRight";
        /// <summary>
        /// 比率显示方式
        /// </summary>
        public const string PercentModel = "PercentModel";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
    }
	/// <summary>
	/// 报表模板
	/// </summary>
	public class RptTemplate: Entity
	{
		#region 基本属性
		/// <summary>
		/// 集合类名称
		/// </summary>
		public string EnsName
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.EnsName) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.EnsName,value) ; 
			}		
		}
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(RptTemplateAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(RptTemplateAttr.FK_Emp, value);
            }
        }
		/// <summary>
		/// 描述
		/// </summary>
		public string MyPK
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.MyPK ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.MyPK,value) ; 
			}
		}		 
		/// <summary>
		/// D1
		/// </summary>
		public string D1
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.D1) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.D1,value) ; 
			}
		}
		/// <summary>
		/// D2
		/// </summary>
		public string D2
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.D2) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.D2,value) ; 
			}
		}
		/// <summary>
		/// D3
		/// </summary>
		public string D3
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.D3) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.D3,value) ; 
			}
		}
		public string AlObjsText
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.AlObjs ) ; 
			}
		}
		/// <summary>
		/// 分析的对象
		/// 数据格式 @分析对象1@分析对象2@分析对象3@
		/// </summary>
		public string AlObjs
		{
			get
			{
				return this.GetValStringByKey(RptTemplateAttr.AlObjs) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.AlObjs,value) ; 
			}
		}
		public int Height
		{
			get
			{
				return this.GetValIntByKey(RptTemplateAttr.Height ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.Height,value) ; 
			}
		}
		public int Width
		{
			get
			{
				return this.GetValIntByKey(RptTemplateAttr.Width ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.Width,value) ; 
			}
		}
		/// <summary>
		/// 是否显示大合计
		/// </summary>
		public bool IsSumBig
		{
			get
			{
				return this.GetValBooleanByKey(RptTemplateAttr.IsSumBig ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.IsSumBig,value) ; 
			}
		}
		/// <summary>
		/// 小合计
		/// </summary>
		public bool IsSumLittle
		{
			get
			{
				return this.GetValBooleanByKey(RptTemplateAttr.IsSumLittle ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.IsSumLittle,value) ; 
			}
		}
		/// <summary>
		/// 是否现实右合计。
		/// </summary>
		public bool IsSumRight
		{
			get
			{
				return this.GetValBooleanByKey(RptTemplateAttr.IsSumRight ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.IsSumRight,value) ; 
			}
		}
		public PercentModel PercentModel
		{
			get
			{
				return (PercentModel)this.GetValIntByKey(RptTemplateAttr.PercentModel ) ; 
			}
			set
			{
				this.SetValByKey(RptTemplateAttr.PercentModel,(int)value) ; 
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
        /// 
        /// </summary>
		public RptTemplate()
		{
		}
        /// <summary>
        /// 类
        /// </summary>
        /// <param name="EnsName"></param>
        public RptTemplate(string ensName)
        {
            this.EnsName = ensName;
            this.FK_Emp = Web.WebUser.No;
            this.MyPK = Web.WebUser.No + "@" + EnsName;
            try
            {
                this.Retrieve();
            }
            catch
            {
                this.Insert();
            }
        }
		 
		/// <summary>
        /// 报表模板
		/// </summary>
		public override Map EnMap
		{
			get
			{
				if (this._enMap!=null) 
					return this._enMap;
				Map map = new Map("Sys_RptTemplate");
				map.DepositaryOfEntity=Depositary.Application;
				map.EnDesc="报表模板";
				map.EnType=EnType.Sys;

                map.AddMyPK();
				map.AddTBString(RptTemplateAttr.EnsName,null,"类名",false,false,0,500,20);
                map.AddTBString(RptTemplateAttr.FK_Emp, null, "操作员", true, false, 0, 20, 20);

				map.AddTBString(RptTemplateAttr.D1,null,"D1",false,true,0,90,10);
				map.AddTBString(RptTemplateAttr.D2,null,"D2",false,true,0,90,10);
				map.AddTBString(RptTemplateAttr.D3,null,"D3",false,true,0,90,10);

				map.AddTBString(RptTemplateAttr.AlObjs,null,"要分析的对象",false,true,0,90,10);

				map.AddTBInt(RptTemplateAttr.Height,600,"Height",false,true);
				map.AddTBInt(RptTemplateAttr.Width,800,"Width",false,true);

				map.AddBoolean(RptTemplateAttr.IsSumBig,false,"是否显示大合计",false,true);
				map.AddBoolean(RptTemplateAttr.IsSumLittle,false,"是否显示小合计",false,true);
				map.AddBoolean(RptTemplateAttr.IsSumRight,false,"是否显示右合计",false,true);

				map.AddTBInt(RptTemplateAttr.PercentModel,0,"比率显示方式",false,true);
				this._enMap=map;
				return this._enMap;
			}
		}
		#endregion 
	}
	
	/// <summary>
	/// 实体集合
	/// </summary>
	public class RptTemplates : Entities
	{		
		#region 构造
		public RptTemplates()
		{
		}
		
		/// <summary>
		/// 查询
		/// </summary>
		/// <param name="EnsName"></param>
		public RptTemplates(string EnsName)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(RptTemplateAttr.EnsName, EnsName);			 
			qo.DoQuery();
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity 
		{
			get
			{
				return new RptTemplate();
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
        public System.Collections.Generic.IList<RptTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<RptTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<RptTemplate> Tolist()
        {
            System.Collections.Generic.List<RptTemplate> list = new System.Collections.Generic.List<RptTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((RptTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
		
	}
}
