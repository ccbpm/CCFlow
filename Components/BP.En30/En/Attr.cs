using System;
using System.Collections;
using BP.Web.Controls ;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 编辑类型
    /// </summary>
    public enum EditType
    {
        /// <summary>
        /// 可编辑
        /// </summary>
        Edit,
        /// <summary>
        /// 不可删除
        /// </summary>
        UnDel,
        /// <summary>
        /// 只读,不可删除。
        /// </summary>
        Readonly
    }
    /// <summary>
    /// 自动填充方式
    /// </summary>
    public enum AutoFullWay
    {
        /// <summary>
        /// 不设置
        /// </summary>
        Way0,
        /// <summary>
        /// 方式1
        /// </summary>
        Way1_JS,
        /// <summary>
        /// sql 方式。
        /// </summary>
        Way2_SQL,
        /// <summary>
        /// 外键
        /// </summary>
        Way3_FK,
        /// <summary>
        /// 明细
        /// </summary>
        Way4_Dtl,
        /// <summary>
        /// 脚本
        /// </summary>
        Way5_JS
    }
	/// <summary>
	///  控件类型
	/// </summary>
    public enum UIContralType
    {
        /// <summary>
        /// 文本框
        /// </summary>
        TB = 0,
        /// <summary>
        /// 下拉框
        /// </summary>
        DDL = 1,
        /// <summary>
        /// CheckBok
        /// </summary>
        CheckBok = 2,
        /// <summary>
        /// 单选择按钮
        /// </summary>
        RadioBtn = 3,
        /// <summary>
        /// 地图定位
        /// </summary>
        MapPin = 4,
        /// <summary>
        /// 录音控件
        /// </summary>
        MicHot = 5,
        /// <summary>
        /// 附件展示控件
        /// </summary>
        AthShow = 6,
        /// <summary>
        /// 手机拍照控件
        /// </summary>
        MobilePhoto = 7,
        /// <summary>
        /// 手写签名版
        /// </summary>
        HandWriting = 8,
        /// <summary>
        /// 超链接
        /// </summary>
        HyperLink = 9,
        /// <summary>
        /// 文本
        /// </summary>
        Lab = 10,
        /// <summary>
        /// 图片
        /// </summary>
        FrmImg = 11,
        /// <summary>
        /// 流程进度图
        /// </summary>
        JobSchedule=50

    }
    /// <summary>
    /// 逻辑类型
    /// </summary>
    public enum FieldTypeS
    {
        /// <summary>
        /// 普通类型
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 枚举类型
        /// </summary>
        Enum=1,
        /// <summary>
        /// 外键
        /// </summary>
        FK=2
    }
	/// <summary>
	/// 字段类型
	/// </summary>
	public enum FieldType
	{
		/// <summary>
		/// 正常的
		/// </summary>
		Normal,
		/// <summary>
		/// 主键
		/// </summary>
		PK,
		/// <summary>
		/// 外键
		/// </summary>
		FK,
		/// <summary>
		/// 枚举
		/// </summary>
	    Enum,		 
		/// <summary>
		/// 既是主键又是外键
		/// </summary>
		PKFK,
		/// <summary>
		/// 既是主键又是枚举
		/// </summary>
		PKEnum,
		/// <summary>
		/// 关连的文本.
		/// </summary>
		RefText,
		/// <summary>
		/// 虚拟的
		/// </summary>
		NormalVirtual,
		/// <summary>
		/// 多值的
		/// </summary>
		MultiValues
	}
	/// <summary>
	/// 属性
	/// </summary>
	public class Attr
	{
        public BP.Sys.MapAttr ToMapAttr
        {
            get
            {

                BP.Sys.MapAttr attr = new BP.Sys.MapAttr();

                attr.KeyOfEn = this.Key;
                attr.Name = this.Desc;
                attr.DefVal  = this.DefaultVal.ToString();
                attr.KeyOfEn = this.Field;

                attr.MaxLen = this.MaxLength;
                attr.MinLen = this.MinLength;
                attr.UIBindKey = this.UIBindKey;
                attr.UIIsLine = this.UIIsLine;
                attr.UIHeight = 0;

                if (this.MaxLength > 3000)
                    attr.UIHeight = 10;

                attr.UIWidth = this.UIWidth;
                attr.MyDataType = this.MyDataType;

                attr.UIRefKey = this.UIRefKeyValue;

                attr.UIRefKeyText = this.UIRefKeyText;
                attr.UIVisible = this.UIVisible;

                //if (this.IsPK)
                //    attr.MyDataType =  = FieldType.PK;
                //    attr.MyFieldType = FieldType.PK;

                switch (this.MyFieldType)
                {
                    case FieldType.Enum:
                    case FieldType.PKEnum:
                        attr.UIContralType = this.UIContralType;
                        attr.LGType = FieldTypeS.Enum;
                        attr.UIIsEnable = this.UIIsReadonly;
                        break;
                    case FieldType.FK:
                    case FieldType.PKFK:
                        attr.UIContralType = this.UIContralType;
                        attr.LGType = FieldTypeS.FK;
                        //attr.MyDataType = (int)FieldType.FK;
                        attr.UIRefKey = "No";
                        attr.UIRefKeyText = "Name";
                        attr.UIIsEnable = this.UIIsReadonly;
                        break;
                    default:
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;

                        attr.UIIsEnable = !this.UIIsReadonly;
                        switch (this.MyDataType)
                        {
                            case DataType.AppBoolean:
                                attr.UIContralType = UIContralType.CheckBok;
                                attr.UIIsEnable = this.UIIsReadonly;
                                break;
                            case DataType.AppDate:
                                //if (this.Tag == "1")
                                //    attr.DefaultVal = DataType.CurrentData;
                                break;
                            case DataType.AppDateTime:
                                //if (this.Tag == "1")
                                //    attr.DefaultVal = DataType.CurrentData;
                                break;
                            default:
                                break;
                        }
                        break;
                }

                //attr.HisAutoFull = this.AutoFullWay;
                //attr.AutoFullDoc = this.AutoFullDoc;
                return attr;
            }
        }
        public BP.Web.Controls.TBType HisTBType
        {
            get
            {
                switch (this.MyDataType)
                {
                    case BP.DA.DataType.AppMoney:
                        return BP.Web.Controls.TBType.Moneny;
                    case BP.DA.DataType.AppInt:
                    case BP.DA.DataType.AppFloat:
                    case BP.DA.DataType.AppDouble:
                        return BP.Web.Controls.TBType.Num;
                    default:
                        return BP.Web.Controls.TBType.TB;
                }
            }
        }
        public bool IsFK
        {
            get
            {
                if (this.MyFieldType == FieldType.FK || this.MyFieldType == FieldType.PKFK)
                    return true;
                else
                    return false;
            }
        }
        public bool IsFKorEnum
        {
            get
            {
                if (
                this.MyFieldType == FieldType.Enum
                            || this.MyFieldType == FieldType.PKEnum
                            || this.MyFieldType == FieldType.FK
                            || this.MyFieldType == FieldType.PKFK)
                    return true;
                else
                    return false;
            }
        }
		/// <summary>
		/// 是不是能使用默认值。
		/// </summary>
		public bool IsCanUseDefaultValues
		{
			get
			{
				if ( this.MyDataType==DataType.AppString && this.UIIsReadonly==false )
					return true;
				return false;
			}
		}
        public bool IsNum
        {
            get
            {
                if (MyDataType == DataType.AppBoolean || MyDataType == DataType.AppDouble
                    || MyDataType == DataType.AppFloat
                    || MyDataType == DataType.AppInt
                    || MyDataType == DataType.AppMoney
                    )
                    return true;
                else
                    return false;
            }
        }
        public bool IsEnum
        {
            get
            {
                if ( MyFieldType == FieldType.Enum || MyFieldType == FieldType.PKEnum)
                    return true;
                else
                    return false;
            }
        }
        public bool IsRefAttr
        {
            get
            {
                if (this.MyFieldType == FieldType.RefText)
                    return true;
                return false;
            }
        }
		/// <summary>
		/// 计算属性是不是PK
		/// </summary>
		public bool IsPK
		{
			get
			{
				if ( MyFieldType==FieldType.PK || MyFieldType==FieldType.PKFK || MyFieldType==FieldType.PKEnum  )
					return true;
				else
					return false;
			}
		}
        private int _IsKeyEqualField = -1;
        public bool IsKeyEqualField
        {
            get
            {
                if (_IsKeyEqualField == -1)
                {
                    if (this.Key == this.Field)
                        _IsKeyEqualField = 1;
                    else
                        _IsKeyEqualField = 0;
                }

                if (_IsKeyEqualField == 1)
                    return true;
                return false;
            }
        }
		/// <summary>
		/// 输入描述
		/// </summary>
		public string EnterDesc
		{
			get
			{
				if (this.UIContralType==UIContralType.TB)
				{
					if (this.UIIsReadonly || this.UIVisible==false)
					{
						return "此字段只读";
					}
					else
					{
						if (this.MyDataType==DataType.AppDate )
						{
							return "输入日期类型"+DataType.SysDataFormat;
						}
						else if (this.MyDataType==DataType.AppDateTime)
						{
							return "输入日期时间类型"+DataType.SysDataTimeFormat;
						}
						else if (this.MyDataType==DataType.AppString)
						{ 
							return "输入要求最小长度"+this.MinLength+"字符，最大长度"+this.MaxLength+"字符";
						}
						else if (this.MyDataType==DataType.AppMoney)
						{
							return "金额类型 0.00";
						}
						else 
						{
							return "输入数值类型";
						}
					}

				}
				else if ( this.UIContralType==UIContralType.DDL || this.UIContralType==UIContralType.CheckBok )
				{
					if (this.UIIsReadonly )
					{
						return "此字段只读";
					}
					else
					{
						if (this.MyDataType==DataType.AppBoolean)
						{
							return "是/否";
						}
						else
						{
							return "列表选择";
						}
					}
				}
				 
				return "";
			}
		}

		#region 构造函数
		public Attr()
		{}
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="key"></param>
		/// <param name="field"></param>
		/// <param name="defaultVal"></param>
		/// <param name="dataType"></param>
		/// <param name="isPK"></param>
		/// <param name="desc"></param>
		public Attr(string key , string field,  object defaultVal, int dataType, bool isPK, string desc, int minLength, int maxlength)
		{			
			this._key=key;
			this._field=field;
			this._desc=desc;
			if (isPK)
				this.MyFieldType = FieldType.PK ;			 
			this._dataType=dataType;
			this._defaultVal=defaultVal;
			this._minLength=minLength;
			this._maxLength=maxlength;
		}
		public Attr(string key , string field,  object defaultVal, int dataType, bool isPK, string desc)
		{			
			this._key=key;
			this._field=field;
			this._desc=desc;
			if (isPK)
				this.MyFieldType = FieldType.PK;
			this._dataType=dataType;
			this._defaultVal=defaultVal;			 
		}
		#endregion

		#region 属性
        public string HelperUrl = null;
        public AutoFullWay AutoFullWay = AutoFullWay.Way0;
        public string  AutoFullDoc =null;
		/// <summary>
		/// 属性名称
		/// </summary>
		private string _key=null;
		/// <summary>
		/// 属性名称
		/// </summary>
		public string Key
		{
			get
			{
				return this._key;
			}
            set
            {
                if (value != null)
                    this._key = value.Trim();
            }
		}		
		/// <summary>
		/// 属性对应的字段
		/// </summary>
		private string _field=null;
		/// <summary>
		/// 属性对应的字段
		/// </summary>
		/// <returns></returns>
		public string Field
		{
			get
			{
				return this._field;
			}
			set
			{
                if (value != null)
                    this._field = value.Trim();
			}
		}		
		/// <summary>
		/// 字段默认值
		/// </summary>
		private object _defaultVal=null;
        public string DefaultValOfReal
        {
            get
            {
                if (_defaultVal == null)
                    return null;
                return _defaultVal.ToString();
            }
            set
            {
                _defaultVal = value;
            }
        }
		/// <summary>
		/// 字段默认值
		/// </summary>
		public object DefaultVal
		{
			get{
				switch (this.MyDataType)
				{
					case  DataType.AppString :
						if (this._defaultVal==null) 
							return "";
						break;
					case   DataType.AppInt :
						if (this._defaultVal==null )
							return 0;
						try
						{
							return int.Parse(this._defaultVal.ToString()) ;
						}
						catch
						{
							return 0;
							//throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 int 转换。");
						}
					case   DataType.AppMoney :
						if (this._defaultVal==null)
							return 0;
						try
						{
							return float.Parse(this._defaultVal.ToString()) ;
						}
						catch
						{
                            return 0;
						//	throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 AppMoney 转换。");
						}						
					case   DataType.AppFloat :
						if (this._defaultVal==null) 
							return 0;						 
						try
						{
							return float.Parse(this._defaultVal.ToString()) ;
						}
						catch
						{
                            return 0;
						//	throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 float 转换。");
						}
						 
					case   DataType.AppBoolean :
						if (this._defaultVal==null || this._defaultVal.ToString()=="" ) 
							return 0;
						try
						{ 
							if ( DataType.StringToBoolean(this._defaultVal.ToString())  )
								return 1;
							else
								return 0;							 
						}
						catch
						{
							throw new Exception("@设置["+this.Key+"]默认值出现错误，["+this._defaultVal.ToString()+"]不能向 bool 转换，请设置0/1。");
						}					 
						 
					case 5: 					 
						if (this._defaultVal==null) 
							return 0;
						try
						{
							return double.Parse(this._defaultVal.ToString()) ;
						}
						catch
						{
							throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 double 转换。");
						}
						 					
					case   DataType.AppDate	 :					
						if (this._defaultVal==null) 
							return "";
						break;
					case   DataType.AppDateTime :
						if (this._defaultVal==null) 
							return "";
						break;
					default:
						throw new Exception("@bulider insert sql error: 没有这个数据类型，字段名称:"+this.Desc+" 英文:"+this.Key);
				}
				return this._defaultVal;
			}
			 
			set
			{
				this._defaultVal=value;
			}
		}
		/// <summary>
		/// 数据类型。
		/// </summary>
		private int _dataType=0;
		/// <summary>
		/// 数据类型。
		/// </summary>
		public int MyDataType 
		{
			get{
				return this._dataType;
			}
			set
			{
				this._dataType=value;
			}
		}
		public string MyDataTypeStr
		{
			get
			{
				return DataType.GetDataTypeDese(this.MyDataType);
			}
		}

		/// <summary>
		/// 是不是主键。
		/// </summary>
		private FieldType _FieldType=FieldType.Normal;
		/// <summary>
		/// 是不是主键
		/// </summary>
		/// <returns> yes / no</returns>
		public FieldType MyFieldType
		{
			get
			{
				return this._FieldType;
			}
			set
			{
				this._FieldType=value;
			}
		}
		/// <summary>
		/// 描述。
		/// </summary>
		private string _desc=null;
		public string Desc
		{
			get
			{
				return this._desc;
			}
			set
			{
				this._desc=value;
			}
		}
        /// <summary>
        /// 在线帮助
        /// </summary>
        public string DescHelper
        {
            get
            {
                if (this.HelperUrl == null)
                    return this._desc;

                if (this.HelperUrl.Contains("script"))
                    return "<a href=\"" + this.HelperUrl + "\"  ><img src='../../Img/Help.png'  height='20px' border=0/>" + this._desc + "</a>";
                else
                    return "<a href=\"" + this.HelperUrl + "\" target=_blank ><img src='../../Img/Help.png'  height='20px' border=0/>" + this._desc + "</a>";
            }
        }
        public string DescHelperIcon
        {
            get
            {
                if (this.HelperUrl == null)
                    return this._desc;
                return "<a href=\"" + this.HelperUrl + "\" ><img src='../../Img/Help.png' height='20px' border=0/></a>";
            }
        }
		/// <summary>
		/// 最大长度。
		/// </summary>
		private int _maxLength=4000;
		/// <summary>
		/// 最大长度。
		/// </summary>
        public int MaxLength
        {
            get
            {
                switch (this.MyDataType)
                {
                    case DataType.AppDate:
                        return 50;
                    case DataType.AppDateTime:
                        return 50;
                    case DataType.AppString:
                        if (this.IsFK)
                        {
                            return 100;
                        }
                        else
                        {
                            if (this._maxLength == 0)
                                return 50;
                            return this._maxLength;
                        }
                    default:
                        if (this.IsFK)
                            return 100;
                        else
                        {
                            return this._maxLength;
                        }
                }
            }
            set
            {
                this._maxLength = value;
            }
        }
		/// <summary>
		/// 最小长度。
		/// </summary>
		private int _minLength=0;
		/// <summary>
		/// 最小长度。
		/// </summary>
		public int MinLength
		{
			get
			{
				return this._minLength;
			}
			set
			{
				this._minLength=value;
			}
		}
        /// <summary>
        /// 是否可以为空, 对数值类型的数据有效.
        /// </summary>
        public bool IsNull
        {
            get
            {
                if (this.MinLength == 0)
                    return false;
                else
                    return true;
            }
        }
		#endregion 

		#region UI 的扩展属性
        public int UIWidthInt
        {
            get
            {
                return (int)this.UIWidth;
            }
        }
        private float _UIWidth = 80;
		/// <summary>
		/// 宽度
		/// </summary>
		public float UIWidth
		{
            get
            {
                if (this._UIWidth <= 10)
                    return 15;
                else
                    return this._UIWidth;
            }
			set
			{
				this._UIWidth=value;
			}
		}

		private int _UIHeight=0;
		/// <summary>
		/// 高度
		/// </summary>
		public int UIHeight
		{
			get
			{
				return this._UIHeight*10;
			}
			set
			{
				this._UIHeight=value;
			}
		}

		private bool _UIVisible=true;
		/// <summary>
		/// 是不是可见
		/// </summary>
        public bool UIVisible
        {
            get
            {
                return this._UIVisible;
            }
            set
            {
                this._UIVisible = value;
            }
        }
      

        /// <summary>
        /// 是否单行显示
        /// </summary>
        public bool UIIsLine = false;
		private bool _UIIsReadonly=false;
		/// <summary>
		/// 是不是只读
		/// </summary>
		public bool UIIsReadonly
		{
			get
			{
				return this._UIIsReadonly;
			}
			set
			{
				this._UIIsReadonly=value;
			}
		}
		private UIContralType _UIContralType=UIContralType.TB;
		/// <summary>
		/// 控件类型。
		/// </summary>
		public UIContralType UIContralType
		{
			get
			{
				return this._UIContralType;
			}
			set
			{
				this._UIContralType=value;
			}
		}
		private string _UIBindKey=null;
		/// <summary>
		/// 要Bind 的Key.
		/// 在TB 里面就是 DataHelpKey
		/// 在DDL 里面是  SelfBindKey.
		/// </summary>
		public string UIBindKey
		{
			get
			{
				return this._UIBindKey ;
			}
			set
			{
				this._UIBindKey=value;
			}
		}
        private string _UIBindKeyOfEn = null;
		public bool UIIsDoc
		{
			get
			{
				if (this.UIHeight!=0 && this.UIContralType==UIContralType.TB)
					return true;
				else
					return false;
			}
		}
        private Entity _HisFKEn = null;
        public Entity HisFKEn
        {
            get
            {
                #warning new a entity.

               return this.HisFKEns.GetNewEntity;

                if (_HisFKEn == null)
                    _HisFKEn = this.HisFKEns.GetNewEntity;

                return _HisFKEn;
            }
        }
		private Entities _HisFKEns=null;
		/// <summary>
		/// 它关联的ens.这个只有在,这个属性是fk, 时有效。
		/// </summary>
		public Entities HisFKEns
		{
			get
			{
				if (_HisFKEns==null)
				{

                    if (this.MyFieldType == FieldType.Enum || this.MyFieldType == FieldType.PKEnum)
                    {
                        return null;
                    }
                    else if (this.MyFieldType == FieldType.FK || this.MyFieldType == FieldType.PKFK)
                    {
                        if (this.UIBindKey.Contains("."))
                            _HisFKEns = ClassFactory.GetEns(this.UIBindKey);
                        else
                            _HisFKEns = new GENoNames(this.UIBindKey, this.Desc);  // ClassFactory.GetEns(this.UIBindKey);
                    }
                    else
                    {
                        return null;
                    }
				}
				return _HisFKEns;
			}
			set
			{
				_HisFKEns=value;
			}
		}
		private  TBType _TBShowType =TBType.TB;
		/// <summary>
		/// 要现实的控件类型。
		/// </summary>
		public TBType UITBShowType
		{
			get
			{
				if (this.MyDataType==DataType.AppDate)
					return TBType.Date ;
				else if (this.MyDataType==DataType.AppFloat)
					return TBType.Float ;
				else if (this.MyDataType==DataType.AppBoolean)
					return TBType.Date ; //throw new Exception("@属性配置错误。");
				else if (this.MyDataType==DataType.AppDouble)
					return TBType.Decimal ;
				else if (this.MyDataType==DataType.AppInt)
					return TBType.Num ;
				else if (this.MyDataType==DataType.AppMoney)
					return TBType.Moneny;
				else
					return _TBShowType; 
			}
			set
			{
				this._TBShowType=value;
			}
		}
		private  DDLShowType _UIDDLShowType =DDLShowType.None;
		/// <summary>
		/// 要现实的控件类型。
		/// </summary>
		public DDLShowType UIDDLShowType
		{
			get
			{
				if (this.MyDataType == DataType.AppBoolean)
					return DDLShowType.Boolean ;
				else 
				    return this._UIDDLShowType ;
			}
			set
			{
				this._UIDDLShowType=value;
			}
		}

		private string _UIRefKey=null;
		/// <summary>
		/// 要Bind 的Key. 在TB 里面就是 DataHelpKey 
		/// 在DDL 里面是SelfBindKey.
		/// </summary>
		public string UIRefKeyValue
		{
			get
			{
				return this._UIRefKey ;
			}
			set
			{
				this._UIRefKey=value;
			}
		}
		private string _UIRefText=null;
		/// <summary>
		/// 关联的实体valkey	 
		/// </summary>
		public string UIRefKeyText
		{
			get
			{
				return this._UIRefText ;
			}
			set
			{
				this._UIRefText=value;
			}
		}
        public string UITag = null;
		#endregion		 
	}
	/// <summary>
	/// 属性集合
	/// </summary>
	[Serializable]
	public class Attrs: CollectionBase
	{
		#region 关于属性的增加 String
		protected void AddTBString(string key , string field,  object defaultVal, 
            FieldType _FieldType, TBType tbType, string desc, bool uiVisable, bool isReadonly ,int minLength, int maxLength, int tbWith )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppString;
			attr.Desc=desc;
			attr.UITBShowType=tbType;
			attr.UIVisible = uiVisable;
			attr.UIWidth=tbWith;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength =maxLength;
			attr.MinLength =minLength;
			attr.MyFieldType=_FieldType;
			this.Add(attr);
		}
		public void AddTBString(string key, string defaultVal,  string desc, bool uiVisable, bool isReadonly ,int minLength, int maxLength, int tbWith )
		{
			AddTBString(  key ,   key,    defaultVal,   FieldType.Normal,  TBType.TB,  desc,   uiVisable,   isReadonly ,  minLength,   maxLength,   tbWith );
		}
		public void AddTBString(string key, string field , object defaultVal,   string desc, bool uiVisable, bool isReadonly ,int minLength, int maxLength, int tbWith )
		{
			AddTBString(  key ,   field,    defaultVal,   FieldType.Normal,  TBType.TB,  desc,   uiVisable,   isReadonly ,  minLength,   maxLength,   tbWith );
		}

		public void AddTBStringDoc(string key, string defaultVal,  string desc, bool uiVisable, bool isReadonly)
		{
			AddTBStringDoc(  key ,   key,    defaultVal,    desc,   uiVisable,   isReadonly ,  0,   2000,   300, 300 );
		}
		public void AddTBStringDoc(string key, string defaultVal,  string desc, bool uiVisable, bool isReadonly ,int minLength, int maxLength, int tbWith , int rows )
		{
			AddTBStringDoc(  key ,   key,    defaultVal,    desc,   uiVisable,   isReadonly ,  minLength,   maxLength,   tbWith, rows );
		}
		public void AddTBStringDoc(string key,string field, string defaultVal,  string desc, bool uiVisable, bool isReadonly ,int minLength, int maxLength, int tbWith, int rows )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppString;
			attr.Desc=desc;
			attr.UITBShowType=TBType.TB;
			attr.UIVisible = uiVisable;
			attr.UIWidth=300;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength =maxLength;
			attr.MinLength =minLength;
			attr.MyFieldType=FieldType.Normal;
			attr.UIHeight = rows;
			this.Add(attr);
		}
        /// <summary>
        /// 增加附件
        /// </summary>
        /// <param name="fileDesc"></param>
        public void AddMyFile(string fileDesc)
        {
            this.AddTBString(EntityNoMyFileAttr.MyFileName, null, fileDesc, false, false, 0, 100, 200);
            this.AddTBString(EntityNoMyFileAttr.MyFilePath, null, "MyFilePath", false, false, 0, 100, 200);
            this.AddTBString(EntityNoMyFileAttr.MyFileExt, null, "MyFileExt", false, false, 0, 10, 10);
            //  this.AddTBInt(EntityNoMyFileAttr.MyFileNum, 0, "MyFileNum", false, false);
            this.AddTBInt(EntityNoMyFileAttr.MyFileH, 0, "MyFileH", false, false);
            this.AddTBInt(EntityNoMyFileAttr.MyFileW, 0, "MyFileW", false, false);
            this.AddTBInt("MyFileSize", 0, "MyFileSize", false, false);

            //this.IsHaveFJ = true;
        }
		#endregion  关于属性的增加 String

		#region 关于属性的增加 Int

		/// <summary>
		/// 增加一个普通的类型。
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="_Field">字段</param>
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBInt(string key, string _Field, int defaultVal, string desc, bool uiVisable, bool isReadonly )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=_Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppInt;
			attr.MyFieldType = FieldType.Normal;
			attr.Desc=desc;
			attr.UITBShowType=TBType.Int;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Add(attr);
		}
		/// <summary>
		/// 增加一个普通的类型。字段值与属性相同。
		/// </summary>
		/// <param name="key">键</param>		 
		/// <param name="defaultVal">默认值</param>
		/// <param name="desc">描述</param>
		/// <param name="uiVisable">是不是可见</param>
		/// <param name="isReadonly">是不是只读</param>
		public void AddTBInt(string key,  int defaultVal, string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBInt(key,key,defaultVal,desc,uiVisable,isReadonly) ;
		}
        public void AddBoolen(string key, bool defaultVal, string desc)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = key;

            if (defaultVal)
                attr.DefaultVal = 1;
            else
                attr.DefaultVal = 0;

            attr.MyDataType = DataType.AppBoolean;
            attr.Desc = desc;
            attr.UIContralType = UIContralType.CheckBok;
            attr.UIIsReadonly = true;
            attr.UIVisible = true;
            this.Add(attr);
        }
		#endregion  关于属性的增加 Int

		#region 关于属性的增加 Float类型
		public void AddTBFloat(string key, string _Field, float defaultVal, string desc, bool uiVisable, bool isReadonly )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=_Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppFloat;
			attr.Desc=desc;
			attr.UITBShowType=TBType.Num;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Add(attr);
		}
		public void AddTBFloat(string key, float defaultVal,  string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBFloat(key,key, defaultVal,desc,uiVisable,isReadonly) ; 
		}
		#endregion  关于属性的增加 Float

		#region Decimal类型
		public void AddTBDecimal(string key, string _Field, decimal defaultVal, string desc, bool uiVisable, bool isReadonly )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=_Field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppDouble;
			attr.Desc=desc;
			attr.UITBShowType=TBType.Decimal;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			this.Add(attr);
		}
		public void AddTBDecimal(string key, decimal defaultVal,  string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBDecimal(key,key, defaultVal,desc,uiVisable,isReadonly) ; 
		}
		#endregion

		#region 日期
		public void AddTBDate(string key, string field, string defaultVal,  string desc, bool uiVisable, bool isReadonly )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppDate;
			attr.Desc=desc;
			attr.UITBShowType=TBType.Date;
			attr.UIVisible = uiVisable;
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength=20;
			this.Add(attr);
		}
		 
		public void AddTBDate(string key, string defaultVal,  string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBDate(key,key,defaultVal,desc,uiVisable,isReadonly) ; 
		}
		/// <summary>
		/// 增加日期类型的控健(默认日期是当前日期)
		/// </summary>
		/// <param name="key">key</param>
		/// <param name="desc">desc</param>
		/// <param name="uiVisable">uiVisable</param>
		/// <param name="isReadonly">isReadonly</param>
		public void AddTBDate(string key, string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBDate(key,key,DateTime.Now.ToString(DataType.SysDataFormat),desc,uiVisable,isReadonly) ; 
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
		public void AddTBDateTime(string key, string field, string defaultVal, 
			string desc, bool uiVisable, bool isReadonly )
		{
			Attr attr = new Attr();
			attr.Key=key;
			attr.Field=field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType=DataType.AppDateTime;
			attr.Desc=desc;
			attr.UITBShowType=TBType.DateTime ;
			attr.UIVisible = uiVisable;			 
			attr.UIIsReadonly = isReadonly;
			attr.MaxLength=30;
			attr.MinLength=0;
			attr.UIWidth=100;
			this.Add(attr);
		}
		public void AddTBDateTime(string key, string defaultVal,  string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBDateTime(key,key,defaultVal,desc,uiVisable,isReadonly);
		}
		public void AddTBDateTime(string key,string desc, bool uiVisable, bool isReadonly )
		{
			this.AddTBDateTime(key,key,DateTime.Now.ToString(DataType.SysDataTimeFormat),desc,uiVisable,isReadonly);
		}
		#endregion 

		#region 于帮定自定义,枚举类型有关系的操作。
        public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
        {
            this.AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, null);
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="defaultVal"></param>
        /// <param name="desc"></param>
        /// <param name="isUIVisable"></param>
        /// <param name="isUIEnable"></param>
        /// <param name="sysEnumKey"></param>
        public void AddDDLSysEnum(string key,string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
        {
            this.AddDDLSysEnum(key, field, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, null);
        }
		/// <summary>
		/// 自定义枚举类型
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="field">字段</param>
		/// <param name="defaultVal">默认</param>
		/// <param name="desc">描述</param>
		/// <param name="sysEnumKey">Key</param>
        public void AddDDLSysEnum(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVal)
        {
            Attr attr = new Attr();
            attr.Key = key;
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
            this.Add(attr);
        }
		/// <summary>
		/// 自定义枚举类型
		/// </summary>
		/// <param name="key">键</param>		
		/// <param name="defaultVal">默认</param>
		/// <param name="desc">描述</param>
		/// <param name="sysEnumKey">Key</param>
		public void AddDDLSysEnum(string key , int defaultVal, string desc,  bool isUIVisable, bool isUIEnable, string sysEnumKey,string cfgVals)
		{
            AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, cfgVals);
		}
        public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable)
        {
            AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, key);
        }
		#endregion 

		#region entities
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
		public void AddDDLEntities(string key , string field,  object defaultVal, int dataType, FieldType _fildType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable )
		{
			Attr attr = new Attr();
			attr.Key=key;  
			attr.Field=field;
			attr.DefaultVal = defaultVal;
			attr.MyDataType= dataType ;
			attr.MyFieldType=_fildType;
			 
			attr.Desc=desc;
			attr.UIContralType=UIContralType.DDL ;
			attr.UIDDLShowType=DDLShowType.Ens;
			attr.UIBindKey=ens.ToString();
            //attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();
            attr.HisFKEns = ens;

			attr.HisFKEns=ens;
			attr.UIRefKeyText=refText;
			attr.UIRefKeyValue=refKey;
			attr.UIIsReadonly=uiIsEnable;
			this.Add(attr,true,false);
		}

        #region DDLSQL
        public void AddDDLSQL(string key, object defaultVal, string desc, string sql, bool uiIsEnable = true)
        {
            //@sly
            if (defaultVal == null)
                defaultVal = "";

            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = key;

            if (typeof(int)== defaultVal.GetType() )
            { 
                attr.DefaultVal = defaultVal;
                attr.MyDataType = DataType.AppInt;
            }
            else
            {
                attr.DefaultVal = defaultVal;
                attr.MyDataType = DataType.AppString;
            }

            attr.MyFieldType = FieldType.Normal;
            attr.MaxLength = 50;

            attr.Desc = desc;
            attr.UIContralType = UIContralType.DDL;

            attr.UIDDLShowType = DDLShowType.BindSQL;

            attr.UIBindKey = sql;
            attr.HisFKEns = null;
            attr.UIIsReadonly = !uiIsEnable;
            this.Add(attr);


            //他的名称列.
            attr = new Attr();
            attr.Key = key + "Text";
            attr.Field = key + "Text";
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppString;
            attr.MyFieldType = FieldType.RefText;
            attr.MaxLength = 200; //最大长度 @李国文
            attr.Desc = desc;
            attr.UIContralType = UIContralType.TB;
            //	attr.UIBindKey = sql;
            attr.UIIsReadonly = true;
            attr.UIVisible = false;
            this.Add(attr);
        }
        #endregion DDLSQL

		public void AddDDLEntities(string key , string field,  object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable )
		{
			AddDDLEntities(key,field,defaultVal, dataType , FieldType.FK , desc, ens,refKey,refText,uiIsEnable);
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
		public void AddDDLEntities(string key ,  object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText , bool uiIsEnable)
		{
			AddDDLEntities(key,key,defaultVal,  dataType ,desc, ens,refKey,refText,uiIsEnable);
		}
        #endregion

		#region entityNoName
		public void AddDDLEntities(string key ,object defaultVal,   string desc, EntitiesNoName ens, bool uiIsEnable )
		{
			this.AddDDLEntities(key,key,defaultVal,DataType.AppString, desc,ens,"No","Name",uiIsEnable);
		}
		public void AddDDLEntities(string key , string field,  object defaultVal, string desc, EntitiesNoName ens, bool uiIsEnable )
		{
			this.AddDDLEntities(key,field,defaultVal,DataType.AppString,desc,ens,"No","Name",uiIsEnable);
		}
		#endregion

        #region EntitiesSimpleTree
        public void AddDDLEntities(string key, object defaultVal, string desc, EntitiesTree ens, bool uiIsEnable)
        {
            this.AddDDLEntities(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
        }
        #endregion

        #region EntitiesOIDName
        public void AddDDLEntities(string key, object defaultVal, string desc, EntitiesOIDName ens, bool uiIsEnable)
        {
            this.AddDDLEntities(key, key, defaultVal, DataType.AppInt, desc, ens, "OID", "Name", uiIsEnable);
        }
        public void AddDDLEntities(string key, string field, object defaultVal, string desc, EntitiesOIDName ens, bool uiIsEnable)
        {
            this.AddDDLEntities(key, field, defaultVal, DataType.AppInt, desc, ens, "OID", "Name", uiIsEnable);
        }
		#endregion

		public Attrs Clone()
		{
			Attrs attrs = new Attrs();
			foreach(Attr attr in this)
			{
				attrs.Add(attr);
			}
			return attrs;
		}
		/// <summary>
		/// 下一个Attr 是否是 Doc 类型.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public Attr NextAttr(string CurrentKey)
		{
			int i =this.GetIndexByKey( CurrentKey) ;

			if (this.Count > i )
				return null;

			return  this[i+1] as Attr;
		}
		public Attr PrvAttr(string CurrentKey)
		{
			int i =this.GetIndexByKey( CurrentKey ) ;

			if (this.Count < i )
				return null;

			return  this[i-1] as Attr;
		}
		/// <summary>
		/// 是否包含属性key。
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(string key)
		{
            foreach (Attr attr in this)
            {
                if (attr.Key  == key )
                    return true;
            }
			return false;
		}
        public bool ContainsUpper(string key)
        {
            foreach (Attr attr in this)
            {
                if (attr.Key.ToUpper() == key.ToUpper() )
                    return true;
            }
            return false;
        }
		/// <summary>
		/// 物理字段Num
		/// </summary>
		public int ConutOfPhysicsFields
		{
			get
			{
				int i = 0 ;
				foreach(Attr attr in this)
				{
					if (attr.MyFieldType!=FieldType.RefText)
						i++;
				}
				return i ;
			}
		}
		
		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete (index, value);
		}
		
		/// <summary>
		/// 通过Key ， 取出他的Index.
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>index</returns>
		public int GetIndexByKey(string key)
		{
			for(int i=0 ; i < this.Count ; i++)
			{
				if (this[i].Key == key)
					return i ;
			}
			return -1;
		}

        
        public Attr GetAttrByKey(string key)
		{
            foreach (Attr  item in this)
            {
                if (item.Key == key)
                    return item;
            }
            return null;
        }
        public Attr GetAttrByKeyOfEn(string Field)
        {
            foreach (Attr item in this)
            {
                if (item.Field == Field)
                    return item;
            }
            return null;
        }
        /// <summary>
        /// 属性集合
        /// </summary>
        /// <param name="cfgKeys">约定的字段格式</param>
        public Attrs(string cfgKeys) 
        {

            /**
             *  根据约定的格式的字符串生成集合.
             *  
 @Key=MyStringField;Name=我的中文字段;Type=String;DefVal=默认值;AppType=Normal;IsLine=1             
 @Key=MyIntField;Name=我的数字字段;Type=Int;DefVal=12;AppType=Normal;IsLine=false
 @Key=MyFloatField;Name=我的浮点字段;Type=Float;DefVal=12.0;AppType=Normal;IsLine=false
 @Key=MyEnumField;Name=我的枚举字段;Type=Int;DefVal=0;AppType=Enum;IsLine=false;BindKey=[0=Yes,1=No,2=Unhnow]
 @Key=MyFKField;Name=我的外键字段;Type=String;DefVal=01;AppType=FK;IsLine=false;BindKey=BP.Port.Depts
             * */

            string[] strs = cfgKeys.Split('@');
            foreach (string str in strs)
            {
                AtPara ap = new AtPara(str.Replace(";", "@"));

                FieldTypeS ft = (FieldTypeS)ap.GetValIntByKey("AppType");
                switch (ft)
                {
                    case FieldTypeS.Enum:
                        this.AddDDLSysEnum(ap.GetValStrByKey("Key"), ap.GetValStrByKey("Key"),
                            ap.GetValIntByKey("DefVal"), ap.GetValStrByKey("Name"), true, true, ap.GetValStrByKey("Key"), "@"+ap.GetValStrByKey("BindKey").Replace(",","@"));
                        break;
                    case FieldTypeS.FK:
                        EntitiesNoName ens = (EntitiesNoName)BP.En.ClassFactory.GetEns(ap.GetValStrByKey("BindKey"));
                        this.AddDDLEntities(ap.GetValStrByKey("Key"), ap.GetValStrByKey("DefVal"), ap.GetValStrByKey("Name"), ens,true);
                        break;
                    default:
                        switch (ap.GetValStrByKey("Type"))
                        {
                            case "String":
                                this.AddTBString(ap.GetValStrByKey("Key"), ap.GetValStrByKey("DefVal"), ap.GetValStrByKey("Name"), true, false, 0, 1000, 500);
                                break;
                            case "Int":
                                this.AddTBInt(ap.GetValStrByKey("Key"), ap.GetValIntByKey("DefVal"), ap.GetValStrByKey("Name"), true, false);
                                break;
                            case "Float":
                                this.AddTBFloat(ap.GetValStrByKey("Key"), ap.GetValFloatByKey("DefVal"), ap.GetValStrByKey("Name"), true, false);
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
        }
		/// <summary>
		/// 属性集合
		/// </summary>
		public Attrs(){}

        /// <summary>
        /// 转换为mapattrs
        /// </summary>
        public BP.Sys.MapAttrs ToMapAttrs
        {
            get
            {
                BP.Sys.MapAttrs mapAttrs = new Sys.MapAttrs();
                foreach (Attr item in this)
                {
                    if (item.MyFieldType == FieldType.RefText)
                        continue;

                    BP.Sys.MapAttr mattr = new Sys.MapAttr();
                    mattr.KeyOfEn = item.Key;
                    mattr.Name = item.Desc;
                    mattr.MyDataType = item.MyDataType;
                    mattr.UIContralType = item.UIContralType;
                    mattr.UIBindKey = item.UIBindKey;

                    //@于庆海，这里需要翻译.
                    mattr.UIWidthInt = item.UIWidthInt;
                    mattr.UIHeightInt = item.UIHeight;

                    mattr.MaxLen = item.MaxLength;
                    mattr.MinLen = item.MinLength;
                    mattr.UIVisible = item.UIVisible;
                    mattr.DefValReal = item.DefaultValOfReal;

                    mattr.UIIsEnable = item.UIIsReadonly;

                    if (item.MyFieldType == FieldType.Normal || item.MyFieldType == FieldType.PK )
                    {
                        if (item.MyDataType == DataType.AppInt ||
                            item.MyDataType == DataType.AppFloat ||
                            item.MyDataType == DataType.AppDouble ||
                            item.MyDataType == DataType.AppMoney ||
                            item.MyDataType == DataType.AppString ||
                            item.MyDataType == DataType.AppDate ||
                            item.MyDataType == DataType.AppDateTime)
                        {
                            mattr.UIIsEnable = !item.UIIsReadonly;
                        }
                    }

                    //if (item.MyFieldType == FieldType.Normal && item.MyDataType == DataType.AppString)
                    //{
                    //    mattr.UIIsEnable = !item.UIIsReadonly;
                    //}


                    if (item.UIIsLine == true)
                        mattr.ColSpan = 3;

                    //帮助url.
                    mattr.SetPara("HelpUrl", item.HelperUrl);
                    mattr.UIRefKeyText = item.UIRefKeyText;
                    mattr.UIRefKey = item.UIRefKeyValue;

                    //if (item.UIIsReadonly == true && item.MyFieldType== FieldType.Normal)
                    //    mattr.UIIsEnable = !item.UIIsReadonly;
                    //else
                    //    mattr.UIIsEnable = item.UIIsReadonly;
                   // else
                     //   mattr.UIIsEnable = !item.UIIsReadonly;



                    if (item.MyFieldType == FieldType.Enum)
                        mattr.LGType = FieldTypeS.Enum;

                    if (item.MyFieldType == FieldType.FK)
                        mattr.LGType = FieldTypeS.FK;

                    mapAttrs.AddEntity(mattr);
                }

                return mapAttrs;
            }
        }
        public void Add(Attr attr)
        {
            if (attr.Field == null || attr.Field == "")
                throw new Exception("@属性设置错误：您不能设置 key='" + attr.Key + "',得字段值为空");

            bool k = attr.IsKeyEqualField;
            this.Add(attr, true, false);
        }
		/// <summary>
		/// 加入一个属性。
		/// </summary>
		/// <param name="attr">attr</param>
		/// <param name="isAddHisRefText">isAddHisRefText</param>
		public void Add(Attr attr, bool isAddHisRefText, bool isAddHisRefName )
		{
            foreach (Attr myattr in this)
            {
                if (myattr.Key == attr.Key)
                    return;
            }

			this.InnerList.Add(attr);

			if (isAddHisRefText)
				this.AddRefAttrText(attr);

            if (isAddHisRefName)
                this.AddRefAttrName(attr);
		}
		private void AddRefAttrText(Attr attr)
		{
			if ( attr.MyFieldType==FieldType.FK 
				||  attr.MyFieldType==FieldType.Enum 
				||  attr.MyFieldType==FieldType.PKEnum 
				||  attr.MyFieldType==FieldType.PKFK )
			{

				Attr myattr= new Attr();
				myattr.MyFieldType=FieldType.RefText ;
				myattr.MyDataType=DataType.AppString ;
				myattr.UIContralType=UIContralType.TB;
				myattr.UIWidth=attr.UIWidth*2;
				myattr.Key=    attr.Key+"Text";

				myattr.UIIsReadonly=true;
				myattr.UIBindKey = attr.UIBindKey ;
               // myattr.UIBindKeyOfEn = attr.UIBindKeyOfEn;
                myattr.HisFKEns = attr.HisFKEns;
                             
              
				//myattr.Desc=attr.Desc+"名称";
				 
				string desc=myattr.Desc="名称";
				if (desc.IndexOf("编号") >=0 )
					myattr.Desc=attr.Desc.Replace("编号","名称");
				else
					myattr.Desc=attr.Desc+"名称";

				if (attr.UIContralType==UIContralType.DDL)
					myattr.UIVisible=false;

				this.InnerList.Add(myattr);


				//this.Add(myattr,true);
			}
		}
        private void AddRefAttrName(Attr attr)
        {
            if (attr.MyFieldType == FieldType.FK
                || attr.MyFieldType == FieldType.Enum
                || attr.MyFieldType == FieldType.PKEnum
                || attr.MyFieldType == FieldType.PKFK)
            {

                Attr myattr = new Attr();
                myattr.MyFieldType = FieldType.Normal;
                myattr.MyDataType = DataType.AppString;
                myattr.UIContralType = UIContralType.TB;
                myattr.UIWidth = attr.UIWidth * 2;

                myattr.Key = attr.Key + "Name";
                myattr.Field = attr.Key + "Name";

                myattr.MaxLength = 200;
                myattr.MinLength = 0;

                myattr.UIVisible = false;
                myattr.UIIsReadonly = true;

                myattr.Desc = myattr.Desc = "Name";
                this.InnerList.Add(myattr);
            }
        }

		/// <summary>
		/// 根据索引访问集合内的元素Attr。
		/// </summary>
		public Attr this[int index]
		{			
			get
			{	
				return (Attr)this.InnerList[index];
			}
		}
	}	
}
