using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web.Controls;

namespace BP.En
{
	/// <summary>
	/// 编辑器类型
	/// </summary>
	public enum EditerType
	{
		/// <summary>
		/// 无编辑器
		/// </summary>
		None,
		/// <summary>
		/// Sina编辑器
		/// </summary>
		Sina,
		/// <summary>
		/// FKEditer
		/// </summary>
		FKEditer,
		/// <summary>
		/// KindEditor
		/// </summary>
		KindEditor,
		/// <summary>
		/// 百度的UEditor
		/// </summary>
		UEditor
	}
	/// <summary>
	/// 附件类型
	/// </summary>
	public enum AdjunctType
	{
		/// <summary>
		/// 不需要附件。
		/// </summary>
		None,
		/// <summary>
		/// 图片
		/// </summary>
		PhotoOnly,
		/// <summary>
		/// word 文档。
		/// </summary>
		WordOnly,
		/// <summary>
		/// 所有的类型
		/// </summary>
		ExcelOnly,
		/// <summary>
		/// 所有的类型。
		/// </summary>
		AllType
	}
	/// <summary>
	/// 实体类型
	/// </summary>
	public enum EnType
	{
		/// <summary>
		/// 系统实体
		/// </summary>
		Sys,
		/// <summary>
		/// 管理员维护的实体
		/// </summary>
		Admin,
		/// <summary>
		/// 应用程序实体
		/// </summary>
		App,
		/// <summary>
		/// 第三方的实体（可以更新）
		/// </summary>
		ThirdPartApp,
		/// <summary>
		/// 视图(更新无效)
		/// </summary>
		View,
		/// <summary>
		/// 可以纳入权限管理
		/// </summary>
		PowerAble,
		/// <summary>
		/// 其他
		/// </summary>
		Etc,
		/// <summary>
		/// 明细或着点对点。
		/// </summary>
		Dtl,
		/// <summary>
		/// 点对点
		/// </summary>
		Dot2Dot,
		/// <summary>
		/// XML　类型
		/// </summary>
		XML,
		/// <summary>
		/// 扩展类型，它用于查询的需要。
		/// </summary>
		Ext
	}
	/// <summary>
	/// 移动到显示方式
	/// </summary>
	public enum MoveToShowWay
	{
		/// <summary>
		/// 不显示
		/// </summary>
		None,
		/// <summary>
		/// 下拉列表
		/// </summary>
		DDL,
		/// <summary>
		/// 平铺
		/// </summary>
		Panel
	}
	/// <summary>
	/// EnMap 的摘要说明。
	/// </summary>
	public class Map
	{
		#region 帮助.
		/// <summary>
		/// 增加帮助
		/// </summary>
		/// <param name="key">字段</param>
		/// <param name="url"></param>
		public void SetHelperUrl(string key, string url)
		{
			Attr attr = this.GetAttrByKey(key);
			attr.HelperUrl = url;
		}
		/// <summary>
		/// 增加帮助
		/// </summary>
		/// <param name="key">字段</param>
		public void SetHelperBaidu(string key)
		{
			Attr attr = this.GetAttrByKey(key);
			attr.HelperUrl = "http://www.baidu.com/s?word=ccflow " + attr.Desc;
		}
		/// <summary>
		/// 增加帮助
		/// </summary>
		/// <param name="key">字段</param>
		/// <param name="keyword">关键字</param>
		public void SetHelperBaidu(string key, string keyword)
		{
			Attr attr = this.GetAttrByKey(key);
			attr.HelperUrl = "http://www.baidu.com/s?word=" + keyword;
		}
		/// <summary>
		/// 增加帮助
		/// </summary>
		/// <param name="key">字段</param>
		/// <param name="context">连接</param>
		public void SetHelperAlert(string key, string context)
		{
			Attr attr = this.GetAttrByKey(key);
			attr.HelperUrl = "javascript:alert('" + context + "')";
		}
		#endregion 帮助.

		#region 与xml 文件操作有关系
		/// <summary>
		/// xml 文件的位置
		/// </summary>
		public string XmlFile = null;
		#endregion 与xml 文件操作有关系

		private Boolean _IsAllowRepeatNo;
		public Boolean IsAllowRepeatNo
		{
			get { return _IsAllowRepeatNo; }
			set { _IsAllowRepeatNo = value; }
		}

       

		#region chuli
		/// <summary>
		/// 查询语句(为了避免过多的资源浪费,一次性生成多次使用)
		/// </summary>
		public string SelectSQL = null;
		/// <summary>
		/// 是否是简单的属性集合
		/// 这里是处理外键的问题，在系统的批量运行过程中太多的外键就会影响效率。
		/// </summary>
		public bool IsSimpleAttrs = false;
		/// <summary>
		/// 设置为简单的
		/// </summary>
		public Attrs SetToSimple()
		{
			Attrs attrs = new Attrs();
			foreach (Attr attr in this._attrs)
			{
				if (attr.MyFieldType == FieldType.PK ||
					attr.MyFieldType == FieldType.PKEnum
					||
					attr.MyFieldType == FieldType.PKFK)
				{
					attrs.Add(new Attr(attr.Key, attr.Field, attr.DefaultVal, attr.MyDataType, true, attr.Desc));
				}
				else
				{
					attrs.Add(new Attr(attr.Key, attr.Field, attr.DefaultVal, attr.MyDataType, false, attr.Desc));
				}
			}
			return attrs;
		}
		#endregion

		#region 关于缓存问题
		public string _FK_MapData = null;
		public string FK_MapData
		{
			get
			{
				if (_FK_MapData == null)
					return this.PhysicsTable;
				return _FK_MapData;
			}
			set
			{
				_FK_MapData = value;
			}
		}
		/// <summary>
		/// 显示方式
		/// </summary>
		private FormShowType _FormShowType = FormShowType.NotSet;
		/// <summary>
		/// 存放位置OfEntity
		/// </summary>
		public FormShowType FormShowType
		{
			get
			{
				return _FormShowType;
			}
			set
			{
				_FormShowType = value;
			}
		}
		/// <summary>
		/// 存放位置
		/// </summary>
		private Depositary _DepositaryOfEntity = Depositary.None;
		/// <summary>
		/// 存放位置OfEntity
		/// </summary>
		public Depositary DepositaryOfEntity
		{
			get
			{
				return _DepositaryOfEntity;
			}
			set
			{
				_DepositaryOfEntity = value;
			}
		}
		/// <summary>
		/// 为java
		/// </summary>
		/// <param name="val"></param>
		public void Java_SetDepositaryOfMap(Depositary val)
		{
			this.DepositaryOfMap = val;
		}
		/// <summary>
		/// 为java
		/// </summary>
		/// <param name="val"></param>
		public void Java_SetDepositaryOfEntity(Depositary val)
		{
			this.DepositaryOfEntity = val;
		}
		/// <summary>
		/// 
		/// </summary>		
		private Depositary _DepositaryOfMap = Depositary.Application;
		/// <summary>
		/// 存放位置
		/// </summary>
		public Depositary DepositaryOfMap
		{
			get
			{
				return _DepositaryOfMap;
			}
			set
			{
				_DepositaryOfMap = value;
			}
		}
		#endregion

		#region 查询属性处理

		#region 非枚举值与外键条件查询
		private AttrsOfSearch _attrsOfSearch = null;
		/// <summary>
		/// 查找属性
		/// </summary>
		public AttrsOfSearch AttrsOfSearch
		{
			get
			{
				if (this._attrsOfSearch == null)
					this._attrsOfSearch = new AttrsOfSearch();
				return this._attrsOfSearch;
			}
		}
		/// <summary>
		/// 得到全部的Attrs
		/// </summary>
		/// <returns></returns>
		public Attrs GetChoseAttrs(Entity en)
		{
			return BP.Sys.CField.GetMyAttrs(en.GetNewEntities, en.EnMap);
		}
		public Attrs GetChoseAttrs(Entities ens)
		{
			return BP.Sys.CField.GetMyAttrs(ens, this);
		}
		#endregion

		#region 关于枚举值与外键查找条件
		/// <summary>
		/// 查找的attrs 
		/// </summary>
		private AttrSearchs _SearchAttrs = null;
		/// <summary>
		/// 查找的attrs
		/// </summary>
		public AttrSearchs SearchAttrs
		{
			get
			{
				if (this._SearchAttrs == null)
					this._SearchAttrs = new AttrSearchs();
				return this._SearchAttrs;
			}
		}
		public void AddHidden(string refKey, string symbol, string val)
		{
			AttrOfSearch aos = new AttrOfSearch("K" + this.AttrsOfSearch.Count, refKey, refKey, symbol, val, 0, true);
			this.AttrsOfSearch.Add(aos);
		}
		/// <summary>
		/// 加入查找属性.必须是外键盘/枚举类型/boolen.
		/// </summary>
		/// <param name="key">key</param>
		public void AddSearchAttr(string key)
		{
			Attr attr = this.GetAttrByKey(key);
			if (attr.Key == "FK_Dept")
				this.SearchAttrs.Add(attr, false, null);
			else
				this.SearchAttrs.Add(attr, true, null);
		}
		/// <summary>
		/// 加入查找属性.必须是外键盘/枚举类型/boolen.
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="isShowSelectedAll">是否显示全部</param>
		/// <param name="relationalDtlKey">级联子菜单字段</param>
		public void AddSearchAttr(string key, bool isShowSelectedAll, string relationalDtlKey)
		{
			Attr attr = this.GetAttrByKey(key);
			this.SearchAttrs.Add(attr, isShowSelectedAll, relationalDtlKey);
		}
		#endregion

		#endregion

		#region 公共方法
		/// <summary>
		/// 取得字段
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>field name </returns>
		public string GetFieldByKey(string key)
		{
			return GetAttrByKey(key).Field;
		}
		/// <summary>
		/// 取得描述
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>val</returns>
		public string GetDescByKey(String key)
		{
			return GetAttrByKey(key).Desc;
		}
		/// <summary>
		/// 通过一个key 得到它的属性值。
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>attr</returns>
		public Attr GetAttrByKey(string key)
		{
			foreach (Attr attr in this.Attrs)
			{
				if (attr.Key.ToUpper() == key.ToUpper())
				{
					return attr;
				}
			}

			if (key == null)
				throw new Exception("@[" + this.EnDesc + "] 获取属性key 值不能为空.");


			if (this.ToString().Contains("."))
				throw new Exception("@[" + this.EnDesc + "," + this.PhysicsTable + "] 没有找到 key=[" + key + "]的属性，请检查Map文件。此问题出错的原因之一是，在设置系统中的一个实体的属性关联这个实体，你在给实体设置信息时没有按照规则书写reftext, refvalue。请核实。");
			else
			{
				throw new Exception("@[" + this.EnDesc + "," + this.PhysicsTable + "] 没有找到 key=[" + key + "]的属性，请检查Sys_MapAttr表是否有该数据,用SQL执行: SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.ToString() + "' AND KeyOfEn='" + key + "' 是否可以查询到数据，如果没有可能该字段属性丢失。");
			}
		}
		/// <summary>
		/// 获得属性.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Attr GetAttrByBindKey(string key)
		{
			foreach (Attr attr in this.Attrs)
			{
				if (attr.UIBindKey == key)
				{
					return attr;
				}
			}
			if (key == null)
				throw new Exception("@[" + this.EnDesc + "] 获取属性key 值不能为空.");

			if (this.ToString().Contains("."))
				throw new Exception("@[" + this.EnDesc + "," + this.ToString() + "] 没有找到 key=[" + key + "]的属性，请检查Map文件。此问题出错的原因之一是，在设置系统中的一个实体的属性关联这个实体，你在给实体设置信息时没有按照规则书写reftext, refvalue。请核实。");
			else
				throw new Exception("@[" + this.EnDesc + "," + this.ToString() + "] 没有找到 key=[" + key + "]的属性，请检查Sys_MapAttr表是否有该数据,用SQL执行: SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.ToString() + "' AND KeyOfEn='" + key + "' 是否可以查询到数据，如果没有可能该字段属性丢失。");
		}
		/// <summary>
		/// 通过一个key 得到它的属性值。
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>attr</returns>
		public Attr GetAttrByDesc(string desc)
		{
			foreach (Attr attr in this.Attrs)
			{
				if (attr.Desc == desc)
				{
					return attr;
				}
			}
			if (desc == null)
				throw new Exception("@[" + this.EnDesc + "] 获取属性 desc  值不能为空.");

			throw new Exception("@[" + this.EnDesc + "] 没有找到 desc=[" + desc + "]的属性，请检查Map文件。此问题出错的原因之一是，在设置系统中的一个实体的属性关联这个实体，你在给实体设置信息时没有按照规则书写reftext, refvalue。请核实。");
		}
		#endregion

		#region 计算属性
		/// <summary>
		/// 取道最大的TB宽度。
		/// </summary>
		private int _MaxTBLength = 0;
		/// <summary>
		/// 最大的TB宽度。
		/// </summary>
		public float MaxTBLength
		{
			get
			{
				if (_MaxTBLength == 0)
				{
					foreach (Attr attr in this.Attrs)
					{
						if (attr.UIWidth > _MaxTBLength)
						{
							_MaxTBLength = (int)attr.UIWidth;
						}
					}
				}
				return _MaxTBLength;
			}
		}
		/// <summary>
		/// 物理键盘集合
		/// </summary>
		private Attrs _HisPhysicsAttrs = null;
		/// <summary>
		/// 物理键盘集合
		/// </summary>
		public Attrs HisPhysicsAttrs
		{
			get
			{
				if (_HisPhysicsAttrs == null)
				{
					_HisPhysicsAttrs = new Attrs();
					foreach (Attr attr in this.Attrs)
					{
						if (attr.MyFieldType == FieldType.NormalVirtual || attr.MyFieldType == FieldType.RefText)
							continue;
						_HisPhysicsAttrs.Add(attr, false, this.IsAddRefName);
					}
				}
				return _HisPhysicsAttrs;
			}
		}
		/// <summary>
		/// 他的外键集合
		/// </summary>
		private Attrs _HisFKAttrs = null;
		/// <summary>
		/// 他的外键集合
		/// </summary>
		public Attrs HisFKAttrs
		{
			get
			{
				if (_HisFKAttrs == null)
				{
					_HisFKAttrs = new Attrs();
					foreach (Attr attr in this.Attrs)
					{
						if (attr.MyFieldType == FieldType.FK
							|| attr.MyFieldType == FieldType.PKFK)
						{
							_HisFKAttrs.Add(attr, false, false);
						}
					}
				}
				return _HisFKAttrs;
			}
		}
		private int _isFull = -1;
		/// <summary>
		/// 是否有自动计算
		/// </summary>
		public bool IsHaveAutoFull
		{
			get
			{
				if (_isFull == -1)
				{
					foreach (Attr attr in _attrs)
					{
						if (attr.AutoFullDoc != null)
							_isFull = 1;
					}
					if (_isFull == -1)
						_isFull = 0;
				}
				if (_isFull == 0)
					return false;
				return true;
			}
		}
		public bool IsHaveFJ = false;
		/// <summary>
		/// 附件存储位置
		/// </summary>
		public string FJSavePath = null;
		/// <summary>
		/// 移动到显示方式
		/// </summary>
		public string TitleExt = null;
		private int _isJs = -1;
		public bool IsHaveJS
		{
			get
			{
				if (_isJs == -1)
				{
					foreach (Attr attr in _attrs)
					{
						if (attr.AutoFullDoc == null)
							continue;
						if (attr.AutoFullWay == AutoFullWay.Way1_JS)
						{
							_isJs = 1;
							break;
						}
					}

					if (_isJs == -1)
						_isJs = 0;
				}

				if (_isJs == 0)
					return false;
				return true;
			}
		}
		/// <summary>
		/// 是否加入相关联的名称
		/// AttrKey -  AttrKeyName 
		/// </summary>
		public bool IsAddRefName = false;
		/// <summary>
		/// 他的外键Enum集合
		/// </summary>
		private Attrs _HisEnumAttrs = null;
		/// <summary>
		/// 他的外键Enum集合
		/// </summary>
		public Attrs HisEnumAttrs
		{
			get
			{
				if (_HisEnumAttrs == null)
				{
					_HisEnumAttrs = new Attrs();
					foreach (Attr attr in this.Attrs)
					{
						if (attr.MyFieldType == FieldType.Enum || attr.MyFieldType == FieldType.PKEnum)
						{
							_HisEnumAttrs.Add(attr, true, false);
						}
					}
				}
				return _HisEnumAttrs;
			}
		}
		/// <summary>
		/// 他的外键EnumandPk集合
		/// </summary>
		private Attrs _HisFKEnumAttrs = null;
		/// <summary>
		/// 他的外键EnumandPk集合
		/// </summary>
		public Attrs HisFKEnumAttrs
		{
			get
			{
				if (_HisFKEnumAttrs == null)
				{
					_HisFKEnumAttrs = new Attrs();
					foreach (Attr attr in this.Attrs)
					{
						if (attr.MyFieldType == FieldType.Enum
							|| attr.MyFieldType == FieldType.PKEnum
							|| attr.MyFieldType == FieldType.FK
							|| attr.MyFieldType == FieldType.PKFK)
						{
							_HisFKEnumAttrs.Add(attr);
						}
					}
				}
				return _HisFKEnumAttrs;
			}
		}
		#endregion

		#region 他的实体配置信息
		private Attrs _HisCfgAttrs = null;
		public Attrs HisCfgAttrs
		{
			get
			{
				if (this._HisCfgAttrs == null)
				{
					this._HisCfgAttrs = new Attrs();
					if (Web.WebUser.No == "admin")
					{

						this._HisCfgAttrs.AddDDLSysEnum("UIRowStyleGlo", 2, "表格数据行风格(应用全局)", true, false, "UIRowStyleGlo",
							"@0=无风格@1=交替风格@2=鼠标移动@3=交替并鼠标移动");

						this._HisCfgAttrs.AddBoolen("IsEnableDouclickGlo", true,
							 "是否启动双击打开(应用全局)");

						this._HisCfgAttrs.AddBoolen("IsEnableFocusField", true, "是否启用焦点字段");
						this._HisCfgAttrs.AddTBString("FocusField", null, "焦点字段(用于显示点击打开的列",
							true, false, 0, 20, 20);
						this._HisCfgAttrs.AddBoolen("IsEnableRefFunc", true, "是否启用相关功能列");
						this._HisCfgAttrs.AddBoolen("IsEnableOpenICON", true, "是否启用打开图标");
						this._HisCfgAttrs.AddDDLSysEnum("MoveToShowWay", 0, "移动到显示方式", true, false,
							"MoveToShowWay", "@0=不显示@1=下拉列表@2=平铺");
						this._HisCfgAttrs.AddTBString("MoveTo", null, "移动到字段", true, false, 0, 20, 20);
						this._HisCfgAttrs.AddTBInt("WinCardW", 820, "弹出窗口宽度", true, false);
						this._HisCfgAttrs.AddTBInt("WinCardH", 480, "弹出窗口高度", true, false);
						this._HisCfgAttrs.AddDDLSysEnum("EditerType", 0, "大块文本编辑器",
							true, false, "EditerType", "@0=无@1=sina编辑器@2=FKCEditer@3=KindEditor@4=UEditor");
						this._HisCfgAttrs.AddTBString("ShowColumns", "", "选择列", false, false, 0, 1000, 100);    //added by liuxc,2015-8-7,增加选择列存储字段
						//  this._HisCfgAttrs.AddDDLSysEnum("UIRowStyleGlo", 2, "表格数据行风格(应用全局)", true, false, "UIRowStyleGlo", "@0=无风格@1=交替风格@2=鼠标移动@3=交替并鼠标移动");
					}
				}
				return _HisCfgAttrs;
			}
		}
		#endregion

		#region 他的关连信息.
		private Attrs _HisRefAttrs = null;
		public Attrs HisRefAttrs
		{
			get
			{
				if (this._HisRefAttrs == null)
				{
					this._HisRefAttrs = new Attrs();

					foreach (Attr attr in this.Attrs)
					{
						if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
						{
							_HisRefAttrs.Add(attr);
						}
					}
				}
				return _HisRefAttrs;
			}
		}
		#endregion

		#region 关于相关功能
		/// <summary>
		/// 增加一个相关功能
		/// </summary>
		/// <param name="title">标题</param>
		/// <param name="classMethodName">连接</param>
		/// <param name="icon">图标</param>
		/// <param name="tooltip">提示信息</param>
		/// <param name="target">连接到</param>
		/// <param name="width">宽度</param>
		/// <param name="height">高度</param>
		public void AddRefMethod(string title, string classMethodName, Attrs attrs, string warning, string icon, string tooltip, string target, int width, int height)
		{
			RefMethod func = new RefMethod();
			func.Title = title;
			func.Warning = warning;
			func.ClassMethodName = classMethodName;
			func.Icon = icon;
			func.ToolTip = tooltip;
			func.Width = width;
			func.Height = height;
			func.HisAttrs = attrs;
			this.HisRefMethods.Add(func);
		}
		public void AddRefMethodOpen()
		{
			RefMethod func = new RefMethod();
			func.Title = "打开";
			func.ClassMethodName = this.ToString() + ".DoOpenCard";
			func.Icon = SystemConfig.CCFlowWebPath + "WF/Img/Btn/Edit.gif";
			this.HisRefMethods.Add(func);
		}
		/// <summary>
		/// 增加
		/// </summary>
		/// <param name="func"></param>
		public void AddRefMethod(RefMethod rm)
		{
			this.HisRefMethods.Add(rm);
		}
		#endregion

		#region 关于他的明细信息
		/// <summary>
		/// 增加明细
		/// </summary>
		/// <param name="ens">集合信息</param>
		/// <param name="refKey">属性</param>
        /// <param name="groupName">显示到分组</param>
		public void AddDtl(Entities ens, string refKey, string groupName=null)
		{
			EnDtl dtl = new EnDtl();
			dtl.Ens = ens;
			dtl.RefKey = refKey;
            dtl.GroupName = groupName;
			this.Dtls.Add(dtl);
		}
		/// <summary>
		/// 相关功能s
		/// </summary> 
		private RefMethods _RefMethods = null;
		/// <summary>
		/// 相关功能
		/// </summary>
		public RefMethods HisRefMethods
		{
			get
			{
				if (this._RefMethods == null)
					_RefMethods = new RefMethods();
				return _RefMethods;
			}
		}
		/// <summary>
		/// 明细s
		/// </summary> 
		private EnDtls _Dtls = null;
		/// <summary>
		/// 他的明细
		/// </summary>
		public EnDtls Dtls
		{
			get
			{
				if (this._Dtls == null)
					_Dtls = new EnDtls();

				return _Dtls;
			}
		}
		/// <summary>
		/// 所有的明细
		/// </summary> 
		private EnDtls _DtlsAll = null;
		/// <summary>
		/// 所有的明细
		/// </summary>
		public EnDtls DtlsAll
		{
			get
			{
				if (this._DtlsAll == null)
				{
					_DtlsAll = this.Dtls;

					// 加入他的多选。
					foreach (AttrOfOneVSM en in this.AttrsOfOneVSM)
					{
						EnDtl dtl = new EnDtl();
						dtl.Ens = en.EnsOfMM;
						dtl.RefKey = en.AttrOfOneInMM;
						//dtl.Desc =en.Desc;
						//dtl.Desc = en.Desc ;
						_DtlsAll.Add(dtl);
					}

				}
				return _DtlsAll;
			}
		}
		#endregion

		#region 构造涵数
		/// <summary>
		/// 构造涵数 
		/// </summary>
		/// <param name="dburl">数据库连接</param>
		/// <param name="physicsTable">物理table.</param>
		public Map(DBUrl dburl, string physicsTable)
		{
			this.EnDBUrl = dburl;
			this.PhysicsTable = physicsTable;
		}
		/// <summary>
		/// 构造涵数
		/// </summary>
		/// <param name="physicsTable">物理table</param>
		public Map(string physicsTable)
		{
			this.PhysicsTable = physicsTable;
		}
		/// <summary>
		/// 构造涵数
		/// </summary>
		/// <param name="physicsTable">表</param>
		/// <param name="enDesc">中文描述</param>
		public Map(string physicsTable, string enDesc)
		{
			this.PhysicsTable = physicsTable;
			this._EnDesc = enDesc;
		}
		/// <summary>
		/// 构造涵数
		/// </summary>
		/// <param name="DBUrlKeyList">连接的Key 你可以用  DBUrlKeyList 得到</param>
		/// <param name="physicsTable">物理表</param>
		public Map(DBUrlType dburltype, string physicsTable)
		{
			this.EnDBUrl = new DBUrl(dburltype);
			this.PhysicsTable = physicsTable;
		}
		/// <summary>
		/// 构造涵数
		/// </summary>
		public Map() { }
		#endregion

		#region 属性
		/// <summary>
		/// 多对多的关联
		/// </summary>
		private AttrsOfOneVSM _AttrsOfOneVSM = new AttrsOfOneVSM();
		/// <summary>
		/// 点对多的关联
		/// </summary>
		public AttrsOfOneVSM AttrsOfOneVSM
		{
			get
			{
				if (this._AttrsOfOneVSM == null)
					this._AttrsOfOneVSM = new AttrsOfOneVSM();
				return this._AttrsOfOneVSM;
			}
			set
			{
				this._AttrsOfOneVSM = value;
			}
		}
		/// <summary>
		/// 通过多实体的类名称取出他的OneVSM属性.
		/// </summary>
		/// <param name="ensOfMMclassName"></param>
		/// <returns></returns>
		public AttrOfOneVSM GetAttrOfOneVSM(string ensOfMMclassName)
		{
			foreach (AttrOfOneVSM attr in this.AttrsOfOneVSM)
			{
				if (attr.EnsOfMM.ToString() == ensOfMMclassName)
				{
					return attr;
				}
			}
			throw new Exception("error param:  " + ensOfMMclassName);
		}
		/// <summary>
		/// 文件类型
		/// </summary>
		private AdjunctType _AdjunctType = AdjunctType.None;
		/// <summary>
		/// 文件类型
		/// </summary>
		public AdjunctType AdjunctType
		{
			get
			{
				return this._AdjunctType;
			}
			set
			{
				this._AdjunctType = value;
			}
		}
		public string MoveTo = null;
		/// <summary>
		/// 实体描述
		/// </summary>
		string _EnDesc = "";
		public string EnDesc
		{
			get
			{
				return this._EnDesc;
			}
			set
			{
				this._EnDesc = value;
			}
		}
		/// <summary>
		/// 是否版本管理
		/// </summary>
		public bool IsEnableVer = false;
		public bool IsShowSearchKey = true;
		public BP.Sys.DTSearchWay DTSearchWay = BP.Sys.DTSearchWay.None;
		public string DTSearchLable = "日期从:";
		public string DTSearchKey = null;
		/// <summary>
		/// 图片DefaultImageUrl
		/// </summary>
		public string Icon = "../Images/En/Default.gif";
		/// <summary>
		/// 实体类型
		/// </summary>
		EnType _EnType = EnType.App;
		/// <summary>
		/// 实体类型 默认为0(用户应用).
		/// </summary>
		public EnType EnType
		{
			get
			{
				return this._EnType;
			}
			set
			{
				this._EnType = value;
			}
		}
		/// <summary>
		/// 为方便java转换设置
		/// </summary>
		/// <param name="val"></param>
		public void Java_SetEnType(EnType val)
		{
			this._EnType = val;
		}

		#region  生成属性根据xml.
		private string PKs = "";
		public void GenerMap(string xml)
		{
			DataSet ds = new DataSet("");
			ds.ReadXml(xml);
			foreach (DataTable dt in ds.Tables)
			{
				switch (dt.TableName)
				{
					case "Base":
						this.DealDT_Base(dt);
						break;
					case "Attr":
						this.DealDT_Attr(dt);
						break;
					case "SearchAttr":
						this.DealDT_SearchAttr(dt);
						break;
					case "Dtl":
						this.DealDT_SearchAttr(dt);
						break;
					case "Dot2Dot":
						this.DealDT_Dot2Dot(dt);
						break;
					default:
						throw new Exception("XML 配置信息错误，没有约定的标记:" + dt.TableName);
				}
			}
			// 检查配置的完整性。

		}

		private void DealDT_Base(DataTable dt)
		{
			if (dt.Rows.Count != 1)
				throw new Exception("基础信息配置错误，不能多于或者少于1行记录。");
			foreach (DataColumn dc in dt.Columns)
			{
				string val = dt.Rows[0][dc.ColumnName].ToString();
				if (val == null)
					continue;
				if (dt.Rows[0][dc.ColumnName] == DBNull.Value)
					continue;

				switch (dc.ColumnName)
				{
					case "EnDesc":
						this.EnDesc = val;
						break;
					case "Table":
						this.PhysicsTable = val;
						break;
					case "DBUrl":
						this.EnDBUrl = new DBUrl(DataType.GetDBUrlByString(val));
						break;
					case "ICON":
						this.Icon = val;
						break;
					case "CodeStruct":
						this.CodeStruct = val;
						break;
					case "AdjunctType":
						//this.PhysicsTable=val;
						break;
					case "EnType":
						switch (val)
						{
							case "Admin":
								this.EnType = BP.En.EnType.Admin;
								break;
							case "App":
								this.EnType = BP.En.EnType.App;
								break;
							case "Dot2Dot":
								this.EnType = BP.En.EnType.Dot2Dot;
								break;
							case "Dtl":
								this.EnType = BP.En.EnType.Dtl;
								break;
							case "Etc":
								this.EnType = BP.En.EnType.Etc;
								break;
							case "PowerAble":
								this.EnType = BP.En.EnType.PowerAble;
								break;
							case "Sys":
								this.EnType = BP.En.EnType.Sys;
								break;
							case "View":
								this.EnType = BP.En.EnType.View;
								break;
							case "XML":
								this.EnType = BP.En.EnType.XML;
								break;
							default:
								throw new Exception("没有约定的标记:EnType =  " + val);
						}
						break;
					case "DepositaryOfEntity":
						switch (val)
						{
							case "Application":
								this.DepositaryOfEntity = Depositary.Application;
								break;
							case "None":
								this.DepositaryOfEntity = Depositary.None;
								break;
							case "Session":
								this.DepositaryOfEntity = Depositary.Application;
								break;
							default:
								throw new Exception("没有约定的标记:DepositaryOfEntity=[" + val + "] 应该选择为,Application, None, Session ");
						}
						break;
					case "DepositaryOfMap":
						switch (val)
						{
							case "Application":
							case "Session":
								this.DepositaryOfMap = Depositary.Application;
								break;
							case "None":
								this.DepositaryOfMap = Depositary.None;
								break;
							default:
								throw new Exception("没有约定的标记:DepositaryOfMap=[" + val + "] 应该选择为,Application, None, Session ");
						}
						break;
					case "PKs":
						this.PKs = val;
						break;
					default:
						throw new Exception("基础信息中没有约定的标记:" + val);
				}
			}
		}
		private void DealDT_Attr(DataTable dt)
		{
			foreach (DataRow dr in dt.Rows)
			{
				Attr attr = new Attr();
				foreach (DataColumn dc in dt.Columns)
				{
					string val = dr[dc.ColumnName].ToString();
					switch (dc.ColumnName)
					{
						case "Key":
							attr.Key = val;
							break;
						case "Field":
							attr.Field = val;
							break;
						case "DefVal":
							attr.DefaultVal = val;
							break;
						case "DT":
							attr.MyDataType = DataType.GetDataTypeByString(val);
							break;
						case "UIBindKey":
							attr.UIBindKey = val;
							break;
						case "UIIsReadonly":
							if (val == "1" || val.ToUpper() == "TRUE")
								attr.UIIsReadonly = true;
							else
								attr.UIIsReadonly = false;
							break;
						case "MinLen":
							attr.MinLength = int.Parse(val);
							break;
						case "MaxLen":
							attr.MaxLength = int.Parse(val);
							break;
						case "TBLen":
							attr.UIWidth = int.Parse(val);
							break;
						default:
							throw new Exception("没有约定的标记:" + val);
					}
				}

				// 判断属性.
				if (attr.UIBindKey == null)
				{
					/* 说明没有设置外键或者枚举类型。*/
					//if (attr.MyDataType
				}
				else
				{
					if (attr.UIBindKey.IndexOf(".") != -1)
					{
						/*说明它是一个类。*/
						Entities ens = attr.HisFKEns;
						EntitiesNoName ensNoName = ens as EntitiesNoName;
						if (ensNoName == null)
						{
							/*没有转换成功的情况。*/
						}
						else
						{
							/*已经转换成功, 说明它是EntityNoName 类型。 */
							if (this.PKs.IndexOf(attr.Key) != -1)
							{
								/* 如果是一个主键  */
								if (attr.Field == "")
									attr.Field = attr.Key;
								this.AddDDLEntitiesPK(attr.Key, attr.Field, attr.DefaultVal.ToString(), attr.Desc, ensNoName, attr.UIIsReadonly);
							}
							else
							{
								this.AddDDLEntities(attr.Key, attr.Field, attr.DefaultVal.ToString(), attr.Desc, ensNoName, attr.UIIsReadonly);
							}
						}

					}
					else
					{
					}

				}


			}
		}
		private void DealDT_SearchAttr(DataTable dt)
		{
		}
		private void DealDT_Dtl(DataTable dt)
		{
		}
		private void DealDT_Dot2Dot(DataTable dt)
		{
		}
		#endregion

		#region 与生成No字串有关
		/// <summary>
		/// 生成字串的字段的长度。
		/// </summary>
		int _GenerNoLength = 0;
		public int GenerNoLength
		{
			get
			{
				if (this._GenerNoLength == 0)
					throw new Exception("@没有指定生成字串的字段长度。");
				return this._GenerNoLength;
			}
			set
			{
				this._GenerNoLength = value;
			}
		}
		/// <summary>
		/// 编码结构
		/// 例如： 0， 2322;
		/// </summary>
		string _CodeStruct = "2";
		/// <summary>
		/// 编码的结构
		/// </summary>
		public string CodeStruct
		{
			get
			{
				return this._CodeStruct;
			}
			set
			{
				this._CodeStruct = value;
				this.IsAutoGenerNo = true;
			}
		}
		/// <summary>
		/// 设置编码规则，为方便java转换使用.
		/// </summary>
		/// <param name="val"></param>
		public void Java_SetCodeStruct(string val)
		{
			CodeStruct = val;
		}
		/// <summary>
		/// 编号的总长度。
		/// </summary>
		public int CodeLength
		{
			get
			{
				int i = 0;
				if (CodeStruct.Length == 0)
				{
					i = int.Parse(this.CodeStruct);
				}
				else
				{
					char[] s = this.CodeStruct.ToCharArray();
					foreach (char c in s)
					{
						i = i + int.Parse(c.ToString());
					}
				}

				return i;
			}
		}
		/// <summary>
		/// 是否允许重复的名称(默认不允许重复。)
		/// </summary>
		private bool _IsAllowRepeatName = true;
		/// <summary>
		/// 是否允许重复的名称.
		/// 在insert，update 前检查。
		/// </summary>
		public bool IsAllowRepeatName
		{
			get
			{
				return _IsAllowRepeatName;
			}
			set
			{
				_IsAllowRepeatName = value;
			}
		}
		/// <summary>
		/// 是否自动编号
		/// </summary>
		private bool _IsAutoGenerNo = false;
		/// <summary>
		/// 是否自动编号.		 
		/// </summary>
		public bool IsAutoGenerNo
		{
			get
			{
				return _IsAutoGenerNo;
			}
			set
			{
				_IsAutoGenerNo = value;
			}
		}
		/// <summary>
		/// 是否检查编号长度。（默认的false）
		/// </summary>
		private bool _IsCheckNoLength = false;
		/// <summary>
		/// 是否检查编号长度.
		/// 在insert 前检查。
		/// </summary>
		public bool IsCheckNoLength
		{
			get
			{
				return _IsCheckNoLength;
			}
			set
			{
				_IsCheckNoLength = value;
			}
		}
		#endregion

		#region 与连接有关系。

		DBUrl _EnDBUrl = null;
		/// <summary>
		/// 数据库连接
		/// </summary>
		public DBUrl EnDBUrl
		{
			get
			{
				if (this._EnDBUrl == null)
				{
					_EnDBUrl = new DBUrl();
				}
				return this._EnDBUrl;
			}
			set
			{
				this._EnDBUrl = value;
			}
		}
		private string _PhysicsTable = null;

		public bool IsView
		{
			get
			{
				string sql = "";
				switch (this.EnDBUrl.DBType)
				{
					case DBType.Oracle:
						sql = "SELECT TABTYPE  FROM TAB WHERE UPPER(TNAME)=:v";
						DataTable oradt = DBAccess.RunSQLReturnTable(sql, "v", this.PhysicsTableExt.ToUpper());
						if (oradt.Rows.Count == 0)
							throw new Exception("@表不存在[" + this.PhysicsTableExt + "]");
						if (oradt.Rows[0][0].ToString().ToUpper().Trim() == "V".ToString())
							return true;
						else
							return false;
						break;
					case DBType.Access:
						sql = "select   Type   from   msysobjects   WHERE   UCASE(name)='" + this.PhysicsTableExt.ToUpper() + "'";
						DataTable dtw = DBAccess.RunSQLReturnTable(sql);
						if (dtw.Rows.Count == 0)
							throw new Exception("@表不存在[" + this.PhysicsTableExt + "]");
						if (dtw.Rows[0][0].ToString().Trim() == "5")
							return true;
						else
							return false;
					case DBType.MSSQL:
						sql = "select xtype from sysobjects WHERE name =" + SystemConfig.AppCenterDBVarStr + "v";
						DataTable dt1 = DBAccess.RunSQLReturnTable(sql, "v", this.PhysicsTableExt);
						if (dt1.Rows.Count == 0)
							throw new Exception("@表不存在[" + this.PhysicsTableExt + "]");

						if (dt1.Rows[0][0].ToString().ToUpper().Trim() == "V".ToString())
							return true;
						else
							return false;
					case DBType.Informix:
						sql = "select tabtype from systables where tabname = '" + this.PhysicsTableExt.ToLower() + "'";
						DataTable dtaa = DBAccess.RunSQLReturnTable(sql);
						if (dtaa.Rows.Count == 0)
							throw new Exception("@表不存在[" + this.PhysicsTableExt + "]");

						if (dtaa.Rows[0][0].ToString().ToUpper().Trim() == "V")
							return true;
						else
							return false;
					case DBType.MySQL:
						sql = "SELECT Table_Type FROM information_schema.TABLES WHERE table_name='" + this.PhysicsTableExt + "' and table_schema='" + SystemConfig.AppCenterDBDatabase + "'";
						DataTable dt2 = DBAccess.RunSQLReturnTable(sql);
						if (dt2.Rows.Count == 0)
							throw new Exception("@表不存在[" + this.PhysicsTableExt + "]");

						if (dt2.Rows[0][0].ToString().ToUpper().Trim() == "VIEW")
							return true;
						else
							return false;
					default:
						throw new Exception("@没有做的判断。");
				}

				DataTable dt = DBAccess.RunSQLReturnTable(sql, "v", this.PhysicsTableExt.ToUpper());
				if (dt.Rows.Count == 0)
					throw new Exception("@表不存在[" + this.PhysicsTableExt + "]");

				if (dt.Rows[0][0].ToString() == "VIEW")
					return true;
				else
					return false;
			}
		}

		public string PhysicsTableExt
		{
			get
			{
				if (this.PhysicsTable.IndexOf(".") != -1)
				{
					string[] str = this.PhysicsTable.Split('.');
					return str[1];
				}
				else
					return this.PhysicsTable;
			}
		}
		/// <summary>
		/// 物理表名称
		/// </summary>
		/// <returns>Table name</returns>
		public string PhysicsTable
		{
			get
			{
				return this._PhysicsTable;
				/*
				if (DBAccess.AppCenterDBType==DBType.Oracle)
				{
					return ""+this._PhysicsTable+"";
				}
				else
				{
					return this._PhysicsTable;
				}
				*/
			}
			set
			{
				// 因为组成的select 语句放入了内存,修改它的时间也要修改内存的数据。
				//DA.Cash.AddObj(this.ToString()+"SQL",Depositary.Application,null);

				DA.Cash.RemoveObj(this.ToString() + "SQL", Depositary.Application);
				Cash.RemoveObj("MapOf" + this.ToString(), this.DepositaryOfMap); // RemoveObj

				//DA.Cash.setObj(en.ToString()+"SQL",en.EnMap.DepositaryOfMap) as string;
				this._PhysicsTable = value;
			}
		}
		#endregion

		private Attrs _attrs = null;
		public Attrs Attrs
		{
			get
			{
				if (this._attrs == null)
					this._attrs = new Attrs();
				return this._attrs;
			}
			set
			{
				if (this._attrs == null)
					this._attrs = new En.Attrs();

				Attrs myattrs = value;
				foreach (Attr item in myattrs)
					this._attrs.Add(item);
			}
		}
		#endregion

		#region 于属性相关的操作

		#region DDL

		#region 于帮定 固定 枚举类型有关系的操作。
		private void AddDDLFixEnum(string key, string field, int defaultVal, bool IsPK, string desc, DDLShowType showtype, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppInt;

			if (IsPK)
				attr.MyFieldType = FieldType.PK;
			else
				attr.MyFieldType = FieldType.Normal;

			attr.Desc = desc;
			attr.UIContralType = UIContralType.DDL;
			attr.UIDDLShowType = showtype;
			attr.UIIsReadonly = isReadonly;
			this.Attrs.Add(attr);
		}
		private void AddDDLFixEnumPK(string key, int defaultVal, string desc, DDLShowType showtype, bool isReadonly)
		{
			this.AddDDLFixEnum(key, key, defaultVal, true, desc, showtype, isReadonly);
		}
		private void AddDDLFixEnumPK(string key, string field, int defaultVal, string desc, DDLShowType showtype, bool isReadonly)
		{
			this.AddDDLFixEnumPK(key, field, defaultVal, desc, showtype, isReadonly);
		}
		private void AddDDLFixEnum(string key, int defaultVal, string desc, DDLShowType showtype, bool isReadonly)
		{
			this.AddDDLFixEnum(key, key, defaultVal, false, desc, showtype, isReadonly);
		}
		#endregion

		#region  与boolen 有关系的操作.
		public void AddBoolean(string key, bool defaultVal, string desc, bool isUIVisable, bool isUIEnable, bool isLine, string helpUrl)
		{
			AddBoolean(key, key, defaultVal, desc, isUIVisable, isUIEnable, isLine, null);
		}
		public void AddBoolean(string key, string field, bool defaultVal, string desc, bool isUIVisable, bool isUIEnable, bool isLine)
		{
			AddBoolean(key, field, defaultVal, desc, isUIVisable, isUIEnable, isLine, null);
		}
		/// <summary>
		/// 增加与boolen 有关系的操作.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="field">field</param>
		/// <param name="defaultVal">defaultVal</param>
		/// <param name="desc">desc</param>
		/// <param name="isUIEnable">isUIEnable</param>
		/// <param name="isUIVisable">isUIVisable</param>
		public void AddBoolean(string key, string field, bool defaultVal, string desc, bool isUIVisable, bool isUIEnable, bool isLine, string helpUrl)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.HelperUrl = helpUrl;

			if (defaultVal)
				attr.DefaultVal = 1;
			else
				attr.DefaultVal = 0;

			attr.MyDataType = DataType.AppBoolean;
			attr.Desc = desc;
			attr.UIContralType = UIContralType.CheckBok;
			
            attr.UIIsReadonly = isUIEnable;

			attr.UIVisible = isUIVisable;
			attr.UIIsLine = isLine;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 增加与boolen 有关系的操作.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="field">field</param>
		/// <param name="defaultVal">defaultVal</param>
		/// <param name="desc">desc</param>
		/// <param name="isUIEnable">isUIEnable</param>
		/// <param name="isUIVisable">isUIVisable</param>
		public void AddBoolean(string key, bool defaultVal, string desc, bool isUIVisable, bool isUIEnable)
		{
			AddBoolean(key, key, defaultVal, desc, isUIVisable, isUIEnable, false);
		}

		/// <summary>
		/// 增加与boolen 有关系的操作.
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="field">field</param>
		/// <param name="defaultVal">defaultVal</param>
		/// <param name="desc">desc</param>
		/// <param name="isUIEnable">isUIEnable</param>
		/// <param name="isUIVisable">isUIVisable</param>
		public void AddBoolean(string key, bool defaultVal, string desc, bool isUIVisable, bool isUIEnable, bool isLine)
		{
			AddBoolean(key, key, defaultVal, desc, isUIVisable, isUIEnable, isLine);
		}


		#endregion

		#region 于帮定自定义,枚举类型有关系的操作。
		public void AddDDLSysEnumPK(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.PKEnum;
			attr.Desc = desc;
			attr.UIContralType = UIContralType.DDL;
			attr.UIDDLShowType = DDLShowType.SysEnum;
			attr.UIBindKey = sysEnumKey;
			attr.UIVisible = isUIVisable;
			attr.UIIsReadonly = !isUIEnable;
			this.Attrs.Add(attr);
		}
		public void AddDDLSysEnum(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVal, bool isLine)
		{
			AddDDLSysEnum(key, field, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, cfgVal, isLine, null);
		}
		/// <summary>
		/// 自定义枚举类型
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认</param>
		/// <param name="desc">描述</param>
		/// <param name="sysEnumKey">Key</param>
		public void AddDDLSysEnum(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVal, bool isLine, string helpUrl)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.HelperUrl = helpUrl;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.Enum;
			attr.Desc = desc;
			attr.UIContralType = UIContralType.DDL;
			attr.UIDDLShowType = DDLShowType.SysEnum;
			attr.UIBindKey = sysEnumKey;
			attr.UITag = cfgVal;
			attr.UIVisible = isUIVisable;
			attr.UIIsReadonly = isUIEnable;
			attr.UIIsLine = isLine;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 自定义枚举类型
		/// </summary>
		/// <param name="key">键</param>		
		/// <param name="defaultVal">默认</param>
		/// <param name="desc">描述</param>
		/// <param name="sysEnumKey">Key</param>
		public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
		{
			AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, null, false);
		}
		public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVal, bool isLine)
		{
			AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, cfgVal, isLine);
		}
		public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVal)
		{
			AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, cfgVal, false);
		}
		public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable)
		{
			AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, key, null, false);
		}
		#endregion


		#region 于帮定自定义,枚举类型有关系的操作。
		/// <summary>
		/// 自定义枚举类型
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认</param>
		/// <param name="desc">描述</param>
		/// <param name="sysEnumKey">Key</param>
		public void AddRadioBtnSysEnum(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.Enum;
			attr.Desc = desc;
			attr.UIContralType = UIContralType.RadioBtn;
			attr.UIDDLShowType = DDLShowType.Self;
			attr.UIBindKey = sysEnumKey;
			attr.UIVisible = isUIVisable;
			attr.UIIsReadonly = !isUIEnable;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 自定义枚举类型
		/// </summary>
		/// <param name="key">键</param>		
		/// <param name="defaultVal">默认</param>
		/// <param name="desc">描述</param>
		/// <param name="sysEnumKey">Key</param>
		public void AddRadioBtnSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
		{
			AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, null, false);
		}
		#endregion


		#region DDLSQL
		public void AddDDLSQL(string key, string defaultVal, string desc, string sql, bool uiIsEnable = true)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = key;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.MyFieldType = FieldType.Normal;
			attr.MaxLength = 50;

			attr.Desc = desc;
			attr.UIContralType = UIContralType.DDL;

			attr.UIDDLShowType = DDLShowType.BindSQL;

			attr.UIBindKey = sql;
			attr.HisFKEns = null;
			attr.UIIsReadonly = !uiIsEnable;
			this.Attrs.Add(attr);


			//他的名称列.
			attr = new Attr();
			attr.Key = key + "Text";
			attr.Field = key + "Text";
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.MyFieldType = FieldType.Normal;
			attr.MaxLength = 50;
			attr.Desc = desc;
			attr.UIContralType = UIContralType.TB;
		//	attr.UIBindKey = sql;
			attr.UIIsReadonly = true;
			attr.UIVisible = false;
			this.Attrs.Add(attr);
		}
		#endregion DDLSQL


		#region 与实体由关系的操作。

		#region entityNoName
		public void AddDDLEntities(string key, string defaultVal, string desc, EntitiesSimpleTree ens, bool uiIsEnable)
		{
			this.AddDDLEntities(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
		}
		public void AddDDLEntities(string key, string defaultVal, string desc, EntitiesTree ens, bool uiIsEnable)
		{
			this.AddDDLEntities(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
		}
		public void AddDDLEntities(string key, string defaultVal, string desc, EntitiesNoName ens, bool uiIsEnable)
		{
			this.AddDDLEntities(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
		}
		public void AddDDLEntities(string key, string field, string defaultVal, string desc, EntitiesNoName ens, bool uiIsEnable)
		{
			this.AddDDLEntities(key, field, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
		}
		#endregion

		#region EntitiesOIDName
		public void AddDDLEntities(string key, int defaultVal, string desc, EntitiesOIDName ens, bool uiIsEnable)
		{
			this.AddDDLEntities(key, key, defaultVal, DataType.AppInt, desc, ens, "OID", "Name", uiIsEnable);
		}
		public void AddDDLEntities(string key, string field, object defaultVal, string desc, EntitiesOIDName ens, bool uiIsEnable)
		{
			this.AddDDLEntities(key, field, defaultVal, DataType.AppInt, desc, ens, "OID", "Name", uiIsEnable);
		}
		#endregion

		/// <summary>
		/// 于实体有关系的操作。
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="dataType">DataType类型</param>
		/// <param name="desc">描述</param>
		/// <param name="ens">实体集合</param>
		/// <param name="refKey">关联的建</param>
		/// <param name="refText">关联的Text</param>
		private void AddDDLEntities(string key, string field, object defaultVal, int dataType, FieldType _fildType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = dataType;
			attr.MyFieldType = _fildType;
			attr.MaxLength = 50;

			attr.Desc = desc;
			attr.UIContralType = UIContralType.DDL;
			attr.UIDDLShowType = DDLShowType.Ens;
			attr.UIBindKey = ens.ToString();
			// attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();

			attr.HisFKEns = ens;


			attr.HisFKEns = ens;
			attr.UIRefKeyText = refText;
			attr.UIRefKeyValue = refKey;
			attr.UIIsReadonly = uiIsEnable;

			this.Attrs.Add(attr, true, this.IsAddRefName);
		}
		public void AddDDLEntities(string key, string field, object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
		{
			AddDDLEntities(key, field, defaultVal, dataType, FieldType.FK, desc, ens, refKey, refText, uiIsEnable);
		}
		/// <summary>
		/// 于实体有关系的操作。字段与属性名称相同。
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="dataType">DataType类型</param>
		/// <param name="desc">描述</param>
		/// <param name="ens">实体集合</param>
		/// <param name="refKey">关联的建</param>
		/// <param name="refText">关联的Text</param>
		public void AddDDLEntities(string key, object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
		{
			AddDDLEntities(key, key, defaultVal, dataType, desc, ens, refKey, refText, uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, object defaultVal, int dataType, string desc, EntitiesTree ens, bool uiIsEnable)
		{
			AddDDLEntities(key, key, defaultVal, dataType, FieldType.PKFK, desc, ens, "No", "Name", uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
		{
			AddDDLEntities(key, key, defaultVal, dataType, FieldType.PKFK, desc, ens, refKey, refText, uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, string field, object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
		{
			AddDDLEntities(key, field, defaultVal, dataType, FieldType.PKFK, desc, ens, refKey, refText, uiIsEnable);
		}

		#region 关于EntitiesNoName 有关系的操作。
		/// <summary>
		/// 关于EntitiesNoName 有关系的操作
		/// </summary>
		/// <param name="key"></param>
		/// <param name="field"></param>
		/// <param name="defaultVal"></param>
		/// <param name="desc"></param>
		/// <param name="ens"></param>
		/// <param name="uiIsEnable"></param>
		public void AddDDLEntitiesPK(string key, string field, string defaultVal, string desc, EntitiesTree ens, bool uiIsEnable)
		{
			AddDDLEntities(key, field, (object)defaultVal, DataType.AppString, FieldType.PKFK, desc, ens, "No", "Name", uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, string field, string defaultVal, string desc, EntitiesNoName ens, bool uiIsEnable)
		{
			AddDDLEntities(key, field, (object)defaultVal, DataType.AppString, FieldType.PKFK, desc, ens, "No", "Name", uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, string defaultVal, string desc, EntitiesNoName ens, bool uiIsEnable)
		{
			AddDDLEntitiesPK(key, key, defaultVal, desc, ens, uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, string defaultVal, string desc, EntitiesTree ens, bool uiIsEnable)
		{
			AddDDLEntitiesPK(key, key, defaultVal, desc, ens, uiIsEnable);
		}
		public void AddDDLEntitiesPK(string key, string defaultVal, string desc, EntitiesSimpleTree ens, bool uiIsEnable)
		{
			AddDDLEntitiesPK(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
		}
		#endregion

		#endregion






		#endregion

		#region TB

		#region string 有关系的操作。

		#region 关于
		protected void AddTBString(string key, string field, object defaultVal, FieldType _FieldType, TBType tbType, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine)
		{
			AddTBString(key, field, defaultVal, _FieldType, tbType, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, isUILine, null);
		}
		protected void AddTBString(string key, string field, object defaultVal, FieldType _FieldType, TBType tbType, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine, string helpUrl)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.HelperUrl = helpUrl;

			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.Desc = desc;
			attr.UITBShowType = tbType;
			attr.UIVisible = uiVisable;
			attr.UIWidth = tbWith;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = maxLength;
			attr.MinLength = minLength;
			attr.MyFieldType = _FieldType;
			attr.UIIsLine = isUILine;
			this.Attrs.Add(attr);
		}
		#endregion

		#region 公共的。
		/// <summary>
		/// 同步两个实体属性.
		/// </summary>
		public void AddAttrsFromMapData()
		{
			if (DataType.IsNullOrEmpty(this.FK_MapData))
				throw new Exception("@您没有为map的 FK_MapData 赋值.");

			MapData md = null;
			md = new MapData();
			md.No = this.FK_MapData;
			if (md.RetrieveFromDBSources() == 0)
			{
				md.Name = this.FK_MapData;
				md.PTable = this.PhysicsTable;
				md.EnPK = this.PKs;
				md.Insert();
				md.RepairMap();
			}
			md.Retrieve();
			BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(this.FK_MapData);

			/*把 手工编写的attr 放入 mapattrs里面去. */
			foreach (Attr attr in this.Attrs)
			{
				if (attrs.Contains(BP.Sys.MapAttrAttr.KeyOfEn, attr.Key))
					continue;

				if (attr.IsRefAttr)
					continue;

				//把文件实体类的属性放入关系实体类中去。
				BP.Sys.MapAttr mapattrN = attr.ToMapAttr;
				mapattrN.FK_MapData = this.FK_MapData;
				if (mapattrN.UIHeight == 0)
					mapattrN.UIHeight = 23;
				mapattrN.Insert();
				attrs.AddEntity(mapattrN);
			}

			//把关系实体类的属性放入文件实体类中去。
			foreach (BP.Sys.MapAttr attr in attrs)
			{
				if (this.Attrs.Contains(attr.KeyOfEn) == true)
					continue;
				this.AddAttr(attr.HisAttr);
			}
		}
		public void AddAttrs(Attrs attrs)
		{
			foreach (Attr attr in attrs)
			{
				if (attr.IsRefAttr)
					continue;
				this.Attrs.Add(attr);
			}
		}
		public void AddAttr(Attr attr)
		{
			this.Attrs.Add(attr);
		}
		public void AddAttr(string key, object defaultVal, int dbtype, bool isPk, string desc)
		{
			if (isPk)
				AddTBStringPK(key, key, desc, true, false, 0, 1000, 100);
			else
				AddTBString(key, key, defaultVal.ToString(), FieldType.Normal, TBType.TB, desc, true, false, 0, 1000, 100, false);
		}
		/// <summary>
		/// 增加一个textbox 类型的属性。
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="field">字段值</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="_FieldType">字段类型</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="uiVisable">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param> 
		public void AddTBString(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			AddTBString(key, key, defaultVal, FieldType.Normal, TBType.TB, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
		}
		public void AddTBString(string key, string field, object defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			AddTBString(key, field, defaultVal, FieldType.Normal, TBType.TB, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
		}
		public void AddTBString(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine)
		{
			AddTBString(key, key, defaultVal, FieldType.Normal, TBType.TB, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, isUILine);
		}
		public void AddTBString(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine, string helpUrl)
		{
			AddTBString(key, key, defaultVal, FieldType.Normal, TBType.TB, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, isUILine, helpUrl);
		}
		/// <summary>
		/// 附件集合
		/// </summary>
		public void AddMyFileS()
		{
			this.AddTBInt(EntityNoMyFileAttr.MyFileNum, 0, "附件", false, false);
			this.IsHaveFJ = true;
		}
		/// <summary>
		/// 附件集合
		/// </summary>
		/// <param name="desc"></param>
		public void AddMyFileS(string desc)
		{
			this.AddTBInt(EntityNoMyFileAttr.MyFileNum, 0, desc, false, false);
			this.IsHaveFJ = true;
		}
		/// <summary>
		/// 增加一个附件
		/// </summary>
		/// <param name="fileDesc">附件描述</param>
		/// <param name="ext">附件ID</param>
		/// <param name="savePath">保存位置(默认为:\datauser\ensName\)</param>
		public void AddMyFile(string fileDesc = null, string ext = null, string savePath = null)
		{
			if (fileDesc == null)
				fileDesc = "附件或图片";

			this.AddTBString(EntityNoMyFileAttr.MyFileName, null, fileDesc, false, false, 0, 100, 200);
			this.AddTBString(EntityNoMyFileAttr.MyFilePath, null, "MyFilePath", false, false, 0, 100, 200);
			this.AddTBString(EntityNoMyFileAttr.MyFileExt, null, "MyFileExt", false, false, 0, 10, 10);
			this.AddTBString(EntityNoMyFileAttr.WebPath, null, "WebPath", false, false, 0, 200, 10);
			this.AddTBInt(EntityNoMyFileAttr.MyFileH, 0, "MyFileH", false, false);
			this.AddTBInt(EntityNoMyFileAttr.MyFileW, 0, "MyFileW", false, false);
			this.AddTBFloat("MyFileSize", 0, "MyFileSize", false, false);
			this.IsHaveFJ = true;
			this.FJSavePath = savePath;
		}
		private AttrFiles _HisAttrFiles = null;
		public AttrFiles HisAttrFiles
		{
			get
			{
				if (_HisAttrFiles == null)
					_HisAttrFiles = new AttrFiles();
				return _HisAttrFiles;
			}
		}
		/// <summary>
		/// 增加一个特定的附件,可以利用它增加多个？
		/// 比如：增加简历，增加论文。
		/// </summary>
		/// <param name="fileDesc"></param>
		/// <param name="fExt"></param>
		public void AddMyFile(string fileDesc, string fExt)
		{
			HisAttrFiles.Add(fExt, fileDesc);
			this.IsHaveFJ = true;
		}

		#region 增加大块文本输入
		public void AddTBStringDoc()
		{
			AddTBStringDoc("Doc", "Doc", null, "内容", true, false, 0, 4000, 300, 300, true);
		}
		public void AddTBStringDoc(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, bool isUILine)
		{
			AddTBStringDoc(key, key, defaultVal, desc, uiVisable, isReadonly, 0, 4000, 300,10, isUILine);
		}
		public void AddTBStringDoc(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
            AddTBStringDoc(key, key, defaultVal, desc, uiVisable, isReadonly, 0, 4000, 300, 10, false);
		}
		public void AddTBStringDoc(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, int rows)
		{
			AddTBStringDoc(key, key, defaultVal, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, rows, false);
		}
		public void AddTBStringDoc(string key, string field, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, int rows, bool isUILine)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.Desc = desc;
			attr.UITBShowType = TBType.TB;
			attr.UIVisible = uiVisable;
			attr.UIWidth = 300;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = 4000;
			attr.MinLength = minLength;
			attr.MyFieldType = FieldType.Normal;
			attr.UIHeight = rows;
			attr.UIIsLine = isUILine;
			this.Attrs.Add(attr);
		}
		#endregion

		#region  PK
		public void AddTBStringPK(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			this.PKs = key;
			AddTBString(key, key, defaultVal, FieldType.PK, TBType.TB, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
		}
		public void AddTBStringPK(string key, string field, object defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			this.PKs = key;
			AddTBString(key, field, defaultVal, FieldType.PK, TBType.TB, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
		}
		#endregion

		#region PKNo

		#endregion

		#region  外键于 Ens 有关系的操作。
		/// <summary>
		/// 外键于 Ens 有关系的操作。
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="ens">实体</param>		 
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringFKEns(string key, string field, string defaultVal, string desc, Entities ens, string refKey, string refText, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			Attr attr = new Attr();
			attr.Key = key;

			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.UIBindKey = ens.ToString();
			attr.HisFKEns = ens;
			// attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();

			attr.Desc = desc;
			attr.UITBShowType = TBType.Ens;
			attr.UIVisible = uiVisable;
			attr.UIWidth = tbWith;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = maxLength;
			attr.MinLength = minLength;
			attr.UIRefKeyValue = refKey;
			attr.UIRefKeyText = refText;
			attr.MyFieldType = FieldType.FK;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 外键于 Ens 有关系的操作。
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="ens">实体</param>		 
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringFKEns(string key, string defaultVal, string desc, Entities ens, string refKey, string refText, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			this.AddTBStringFKEns(key, key, defaultVal, desc, ens, refKey, refText, uiVisable, isReadonly, minLength, maxLength, tbWith);
		}
		#endregion

		#region 于多值有关系的操作
		/// <summary>
		/// 于多值有关系的操作
		/// </summary>
		/// <param name="key"></param>
		/// <param name="field"></param>
		/// <param name="defaultVal"></param>
		/// <param name="desc"></param>
		/// <param name="ens"></param>
		/// <param name="uiVisable"></param>
		/// <param name="isReadonly"></param>
		/// <param name="minLength"></param>
		/// <param name="maxLength"></param>
		/// <param name="tbWith"></param>
		public void AddTBMultiValues(string key, string field, object defaultVal, string desc, Entities ens, string refValue, string refText, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.UIBindKey = ens.ToString();
			attr.HisFKEns = ens;

			// attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();

			attr.Desc = desc;
			attr.UITBShowType = TBType.Ens;
			attr.UIVisible = uiVisable;
			attr.UIWidth = tbWith;
			attr.UIIsReadonly = isReadonly;
			attr.UIRefKeyText = refText;
			attr.UIRefKeyValue = refValue;
			attr.MaxLength = maxLength;
			attr.MinLength = minLength;
			attr.MyFieldType = FieldType.MultiValues;

			this.Attrs.Add(attr);
		}
		#endregion

		#region  主键于 Ens 有关系的操作。
		/// <summary>
		/// 外键于 Ens 有关系的操作。
		/// 主键
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="ens">实体</param>		 
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringPKEns(string key, string field, object defaultVal, string desc, Entities ens, string refVal, string refText, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.UIBindKey = ens.ToString();
			attr.HisFKEns = attr.HisFKEns;
			//attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();
			attr.Desc = desc;
			attr.UITBShowType = TBType.Ens;
			attr.UIVisible = uiVisable;
			attr.UIWidth = tbWith;
			attr.UIIsReadonly = isReadonly;

			attr.UIRefKeyText = refText;
			attr.UIRefKeyValue = refVal;

			attr.MaxLength = maxLength;
			attr.MinLength = minLength;
			attr.MyFieldType = FieldType.PKFK;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 外键于 Ens 有关系的操作。
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="ens">实体</param>		 
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringPKEns(string key, string defaultVal, string desc, Entities ens, string refKey, string refText, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			this.AddTBStringPKEns(key, key, defaultVal, desc, ens, refKey, refText, uiVisable, isReadonly, minLength, maxLength, tbWith);
		}
		#endregion

		#region  主键于 DataHelpKey 有关系的操作。
		/// <summary>
		/// 外键于 DataHelpKey 有关系的操作, 用与自定义的右键帮助系统.
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="DataHelpKey"> 在TB 里定义的右健帮助Key </param></param>		 
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringPKSelf(string key, string field, object defaultVal, string desc, string DataHelpKey, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.UIBindKey = DataHelpKey;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Self;
			attr.UIVisible = uiVisable;
			attr.UIWidth = tbWith;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = maxLength;
			attr.MinLength = minLength;
			attr.MyFieldType = FieldType.PK;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 外键于 Ens 有关系的操作。用与自定义的右键帮助系统.
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="DataHelpKey"> 在TB 里定义的右健帮助Key </param></param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringPKSelf(string key, object defaultVal, string desc, string DataHelpKey, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			this.AddTBStringPKSelf(key, key, defaultVal, desc, DataHelpKey, uiVisable, isReadonly, minLength, maxLength, tbWith);
		}
		#endregion

		#region  外键于 DataHelpKey 有关系的操作。
		/// <summary>
		/// 外键于 DataHelpKey 有关系的操作。用与自定义的右键帮助系统.
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="DataHelpKey"> 在TB 里定义的右健帮助Key </param></param>		 
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringFKSelf(string key, string field, object defaultVal, string desc, string DataHelpKey, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppString;
			attr.UIBindKey = DataHelpKey;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Self;
			attr.UIVisible = uiVisable;
			attr.UIWidth = tbWith;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = maxLength;
			attr.MinLength = minLength;
			attr.MyFieldType = FieldType.Normal;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 外键于 Ens 有关系的操作。用与 Ens 右键帮助系统.
		/// </summary>
		/// <param name="key">属性</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="DataHelpKey"> 在TB 里定义的右健帮助Key </param></param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		/// <param name="minLength">最小长度</param>
		/// <param name="maxLength">最大长度</param>
		/// <param name="tbWith">宽度</param>
		public void AddTBStringFKSelf(string key, object defaultVal, string desc, string DataHelpKey, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
		{
			this.AddTBStringFKSelf(key, key, defaultVal, desc, DataHelpKey, uiVisable, isReadonly, minLength, maxLength, tbWith);
		}
		#endregion

		#region  增加外键植
		public void AddTBStringFKValue(string refKey, string key, string desc, bool IsVisable, int with)
		{

		}

		#endregion

		#endregion

		#endregion

		#region 日期类型
		public void AddTBDate(string key)
		{
			switch (key)
			{
				case "RDT":
					AddTBDate("RDT", "记录日期", true, true);
					break;
				case "UDT":
					AddTBDate("UDT", "更新日期", true, true);
					break;
				default:
					AddTBDate(key, key, true, true);
					break;
			}
		}
		/// <summary>
		/// 增加日期类型的控健
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBDate(string key, string field, string defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppDate;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Date;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = 50;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 增加日期类型的控健
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="defaultVal">defaultVal/如果你想用当天的信息,请选择后面的方法加入</param>
		/// <param name="desc">desc</param>
		/// <param name="uiVisable">uiVisable</param>
		/// <param name="isReadonly">isReadonly</param>
		public void AddTBDate(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			AddTBDate(key, key, defaultVal, desc, uiVisable, isReadonly);
		}
		/// <summary>
		/// 增加日期类型的控健(默认日期是当前日期)
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="desc">desc</param>
		/// <param name="uiVisable">uiVisable</param>
		/// <param name="isReadonly">isReadonly</param>
		public void AddTBDate(string key, string desc, bool uiVisable, bool isReadonly)
		{
			AddTBDate(key, key, DateTime.Now.ToString(DataType.SysDataFormat), desc, uiVisable, isReadonly);
		}
		#endregion

		#region 日期时间类型。
		/// <summary>
		/// 增加日期类型的控健
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBDateTime(string key, string field, string defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppDateTime;
			attr.Desc = desc;
			attr.UITBShowType = TBType.DateTime;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength = 50;
			attr.UIWidth = 55;
			this.Attrs.Add(attr);
		}
		public void AddTBDateTime(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBDateTime(key, key, defaultVal, desc, uiVisable, isReadonly);
		}
		public void AddTBDateTime(string key, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBDateTime(key, key, DateTime.Now.ToString(DataType.SysDataTimeFormat), desc, uiVisable, isReadonly);
		}
		#endregion

		#region 资金类型
		public void AddTBMoney(string key, string field, float defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppMoney;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Moneny;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Attrs.Add(attr);
		}
		public void AddTBMoney(string key, float defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBMoney(key, key, defaultVal, desc, uiVisable, isReadonly);
		}
		#endregion

		#region Int类型
		/// <summary>
		/// 增加一个普通的类型。
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="_Field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBInt(string key, string _Field, int defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = _Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.Normal;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Int;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 增加一个普通的类型。字段值与属性相同。
		/// </summary>
		/// <param name="key">键</param>		 
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBInt(string key, int defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBInt(key, key, defaultVal, desc, uiVisable, isReadonly);
		}
		/// <summary>
		/// 增加一个PK的类型。
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="_Field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBIntPK(string key, string _Field, int defaultVal, string desc, bool uiVisable, bool isReadonly, bool identityKey)
		{
			this.PKs = key;
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = _Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.PK;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Int;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			if (identityKey)
				attr.UIBindKey = "1"; //特殊标记此值，让它可以自动生成自增长的列.
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 增加一个PK的类型。字段值与属性相同。
		/// </summary>
		/// <param name="key">键</param>		 
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBIntPKOID(string _field, string desc)
		{
			this.AddTBIntPK("OID", _field, 0, "OID", false, true, false);
		}
		/// <summary>
		/// 增加一个MID
		/// </summary>
		public void AddTBMID()
		{
			Attr attr = new Attr();
			attr.Key = "MID";
			attr.Field = "MID";
			attr.DefaultVal = 0;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.Normal;
			attr.Desc = "MID";
			attr.UITBShowType = TBType.Int;
			attr.UIVisible = false;
			attr.UIIsReadonly = true;
			this.Attrs.Add(attr);
		}
		public void AddTBIntPKOID()
		{
			this.AddTBIntPKOID("OID", "OID");
		}
		public void AddTBMyNum(string desc)
		{
			this.AddTBInt("MyNum", 1, desc, true, true);
		}
		public void AddTBMyNum()
		{
			this.AddTBInt("MyNum", 1, "个数", true, true);
		}
		/// <summary>
		/// 增加  AtParas字段.
		/// </summary>
		/// <param name="fieldLength"></param>
		public void AddTBAtParas(int fieldLength)
		{
			this.AddTBString("AtPara", null, "AtPara", false, true, 0, fieldLength, 10);
		}
		/// <summary>
		/// 主键
		/// </summary>
		public void AddMyPK(bool uiVisable = true)
		{
			this.PKs = "MyPK";
			this.AddTBStringPK("MyPK", null, "主键MyPK", uiVisable, true, 1, 100, 10);
			//Attr attr = new Attr();
			//attr.Key = "MyPK";
			//attr.Field = "MyPK";
			//attr.DefaultVal = null;
			//attr.MyDataType = DataType.AppString;
			//attr.MyFieldType = FieldType.PK;
			//attr.Desc = "MyPK";
			//attr.UITBShowType = TBType.TB;
			//attr.UIVisible = false;
			//attr.UIIsReadonly = true;
			//attr.MinLength = 1;
			//attr.MaxLength = 100;
			//this.Attrs.Add(attr);
		}
		/// <summary>
		/// 增加自动增长列
		/// </summary>
		public void AddAID()
		{
			Attr attr = new Attr();
			attr.Key = "AID";
			attr.Field = "AID";
			attr.DefaultVal = null;
			attr.MyDataType = DataType.AppInt;
			attr.MyFieldType = FieldType.PK;
			attr.Desc = "AID";
			attr.UITBShowType = TBType.TB;
			attr.UIVisible = false;
			attr.UIIsReadonly = true;
			this.Attrs.Add(attr);
		}
		/// <summary>
		/// 增加一个PK的类型。字段值与属性相同。
		/// </summary>
		/// <param name="key">键</param>		 
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBIntPK(string key, int defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBIntPK(key, key, defaultVal, desc, uiVisable, isReadonly, false);
		}

		public void AddTBIntPK(string key, int defaultVal, string desc, bool uiVisable, bool isReadonly, bool identityKey)
		{
			this.AddTBIntPK(key, key, defaultVal, desc, uiVisable, isReadonly, identityKey);
		}
		public void AddTBIntMyNum()
		{
			this.AddTBInt("MyNum", "MyNum", 1, "个数", true, true);
		}
		#endregion

		#region Float类型
		public void AddTBFloat(string key, string _Field, float defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = _Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppFloat;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Num;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Attrs.Add(attr);
		}
		public void AddTBFloat(string key, float defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBFloat(key, key, defaultVal, desc, uiVisable, isReadonly);
		}
		#endregion

		#region Decimal类型
		public void AddTBDecimal(string key, string _Field, decimal defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			Attr attr = new Attr();
			attr.Key = key;
			attr.Field = _Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType = DataType.AppDouble;
			attr.Desc = desc;
			attr.UITBShowType = TBType.Decimal;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Attrs.Add(attr);
		}
		public void AddTBDecimal(string key, decimal defaultVal, string desc, bool uiVisable, bool isReadonly)
		{
			this.AddTBDecimal(key, key, defaultVal, desc, uiVisable, isReadonly);
		}
		#endregion
		#endregion

		#endregion
	}
}
