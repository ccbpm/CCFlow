using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 属性
    /// </summary>
    public class Attr
    {
        public string GroupName = "基本信息";
        /// <summary>
        /// 转成mapattr.
        /// </summary>
        public BP.Sys.MapAttr ToMapAttr
        {
            get
            {

                BP.Sys.MapAttr attr = new BP.Sys.MapAttr();

                attr.setKeyOfEn(this.Key);
                attr.setName(this.Desc);
                attr.setDefVal(this.DefaultVal.ToString());
                attr.setKeyOfEn(this.Field);

                attr.setMaxLen(this.MaxLength);
                attr.setMinLen(this.MinLength);
                attr.UIBindKey = this.UIBindKey;
                attr.setUIIsLine(this.UIIsLine);
                if (this.UIHeight > 10)
                {
                    if (this.UIIsLine == true)
                        attr.ColSpan = 4;
                    else
                        attr.ColSpan = 3;
                }
                else
                {
                    if (this.UIIsLine == true)
                        attr.ColSpan = 3;
                }
                attr.setUIHeight(0);
                attr.setDefValType(this.DefValType);

                if (this.MaxLength > 3000)
                    attr.setUIHeight(10);

                attr.UIWidth = this.UIWidth;
                attr.setMyDataType(this.MyDataType);

                attr.UIRefKey = this.UIRefKeyValue;

                attr.UIRefKeyText = this.UIRefKeyText;
                attr.setUIVisible(this.UIVisible);
                attr.setUIIsEnable(!this.UIIsReadonly);

                //帮助url.
                attr.SetPara("HelpUrl", this.HelperUrl);
                attr.UIRefKeyText = this.UIRefKeyText;
                attr.UIRefKey = this.UIRefKeyValue;

                switch (this.MyFieldType)
                {
                    case FieldType.Enum:
                    case FieldType.PKEnum:
                        attr.setUIContralType(this.UIContralType);
                        attr.setLGType(FieldTypeS.Enum);
                        //attr.setUIIsEnable(this.UIIsReadonly);
                        break;
                    case FieldType.FK:
                    case FieldType.PKFK:
                        attr.setUIContralType(this.UIContralType);
                        attr.setLGType(FieldTypeS.FK);
                        //attr.MyDataType = (int)FieldType.FK;
                        attr.UIRefKey = "No";
                        attr.UIRefKeyText = "Name";
                        //attr.setUIIsEnable(this.UIIsReadonly);
                        break;
                    default:
                        attr.setUIContralType(this.UIContralType);
                        attr.setLGType(FieldTypeS.Normal);

                        if (this.IsSupperText == 1)
                            attr.TextModel = 3;

                        switch (this.MyDataType)
                        {
                            case DataType.AppBoolean:
                                attr.setUIContralType(UIContralType.CheckBok);
                                //attr.setUIIsEnable(this.UIIsReadonly);
                                break;
                            case DataType.AppDate:
                                //if (this.Tag == "1")
                                //    attr.DefaultVal = DataType.CurrentDate;
                                break;
                            case DataType.AppDateTime:
                                //if (this.Tag == "1")
                                //    attr.DefaultVal = DataType.CurrentDate;
                                break;
                            default:
                                break;
                        }
                        break;
                }
                return attr;
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
        public bool IsNum
        {
            get
            {
                if (MyDataType == DataType.AppBoolean
                    || MyDataType == DataType.AppDouble
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
                if (MyFieldType == FieldType.Enum || MyFieldType == FieldType.PKEnum)
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
                if (MyFieldType == FieldType.PK || MyFieldType == FieldType.PKFK || MyFieldType == FieldType.PKEnum)
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

        #region 构造函数
        public Attr()
        {
        }
        public Attr(string key, string field, object defaultVal, int dataType, bool isPK, string desc)
        {
            this.Key = key;
            this.Field = field;
            this.Desc = desc;
            if (isPK)
                this.MyFieldType = FieldType.PK;
            this.MyDataType = dataType;
            this._defaultVal = defaultVal;
        }
        #endregion

        #region 属性
        public string HelperUrl = null;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Key = null;
        /// <summary>
        /// 属性对应的字段
        /// </summary>
        public string Field = null;
        public int DefValType = 0;
        /// <summary>
        /// 字段默认值
        /// </summary>
        private object _defaultVal = null;
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
            get
            {
                switch (this.MyDataType)
                {
                    case DataType.AppString:
                        if (this._defaultVal == null)
                            return "";
                        break;
                    case DataType.AppInt:
                        if (this._defaultVal == null)
                            return 0;
                        try
                        {
                            return int.Parse(this._defaultVal.ToString());
                        }
                        catch
                        {
                            return 0;
                            //throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 int 转换。");
                        }
                    case DataType.AppMoney:
                        if (this._defaultVal == null)
                            return 0;
                        try
                        {
                            return float.Parse(this._defaultVal.ToString());
                        }
                        catch
                        {
                            return 0;
                            //	throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 AppMoney 转换。");
                        }
                    case DataType.AppFloat:
                        if (this._defaultVal == null)
                            return 0;
                        try
                        {
                            return float.Parse(this._defaultVal.ToString());
                        }
                        catch
                        {
                            return 0;
                            //	throw new Exception("@设置["+this.Key+"]默认值出现错误，["+_defaultVal.ToString()+"]不能向 float 转换。");
                        }

                    case DataType.AppBoolean:
                        if (this._defaultVal == null || this._defaultVal.ToString() == "")
                            return 0;
                        try
                        {
                            if (DataType.StringToBoolean(this._defaultVal.ToString()))
                                return 1;
                            else
                                return 0;
                        }
                        catch
                        {
                            throw new Exception("@设置[" + this.Key + "]默认值出现错误，[" + this._defaultVal.ToString() + "]不能向 bool 转换，请设置0/1。");
                        }

                    case 5:
                        if (this._defaultVal == null)
                            return 0;
                        try
                        {
                            return double.Parse(this._defaultVal.ToString());
                        }
                        catch
                        {
                            throw new Exception("@设置[" + this.Key + "]默认值出现错误，[" + _defaultVal.ToString() + "]不能向 double 转换。");
                        }

                    case DataType.AppDate:
                        if (this._defaultVal == null)
                            return "";
                        break;
                    case DataType.AppDateTime:
                        if (this._defaultVal == null)
                            return "";
                        break;
                    default:
                        throw new Exception("@bulider insert sql error: 没有这个数据类型，字段名称:" + this.Desc + " 英文:" + this.Key);
                }
                return this._defaultVal;
            }

            set
            {
                this._defaultVal = value;
            }
        }
        /// <summary>
        /// 数据类型。
        /// </summary>
        public int MyDataType = 0;

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
        public FieldType MyFieldType = FieldType.Normal;
        /// <summary>
        /// 描述。
        /// </summary>
        public string Desc = null;
        /// <summary>
        /// 最大长度。
        /// </summary>
        private int _maxLength = 4000;
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
        public int MinLength = 0;
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
                this._UIWidth = value;
            }
        }

        public int UIHeight = 0;

        /// <summary>
        /// 是不是可见
        /// </summary>
        public bool UIVisible = true;


        /// <summary>
        /// 是否单行显示
        /// </summary>
        public bool UIIsLine = false;
        /// <summary>
        /// 是不是只读
        /// </summary>
        public bool UIIsReadonly = false;
        public UIContralType UIContralType = UIContralType.TB;
        public string UIBindKey = null;
        public int IsSupperText = 0; //是否大文本，还解析了日期格式,这个地方需要修改.

        private string _UIBindKeyOfEn = null;
        public bool UIIsDoc
        {
            get
            {
                if (this.UIHeight != 0 && this.UIContralType == UIContralType.TB)
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
        private Entities _HisFKEns = null;
        /// <summary>
        /// 它关联的ens.这个只有在,这个属性是fk, 时有效。
        /// </summary>
        public Entities HisFKEns
        {
            get
            {
                if (_HisFKEns == null)
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
                _HisFKEns = value;
            }
        }
        public string UIRefKeyValue = null;

        /// <summary>
        /// 关联的实体valkey	 
        /// </summary>
        public string UIRefKeyText = null;

        public string UITag = null;
        #endregion
    }

    /// <summary>
    /// 属性集合
    /// </summary>
    [Serializable]
    public class Attrs : CollectionBase
    {
        #region 关于属性的增加 String
        protected void AddTBString(string key, string field, object defaultVal,
            FieldType _FieldType, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppString;
            attr.Desc = desc;
            attr.UIVisible = uiVisable;
            attr.UIWidth = tbWith;
            attr.UIIsReadonly = isReadonly;
            attr.MaxLength = maxLength;
            attr.MinLength = minLength;
            attr.MyFieldType = _FieldType;
            this.Add(attr);
        }
        public string currGroupAttrName = "基本信息";
        public void AddTBString(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
        {
            AddTBString(key, key, defaultVal, FieldType.Normal, desc, uiVisable, isReadonly, minLength, maxLength, tbWith);
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
        public void AddTBInt(string key, string _Field, int defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = _Field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppInt;
            attr.MyFieldType = FieldType.Normal;
            attr.Desc = desc;
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
        public void AddTBInt(string key, int defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBInt(key, key, defaultVal, desc, uiVisable, isReadonly);
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
        public void AddTBFloat(string key, string _Field, float defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = _Field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppFloat;
            attr.Desc = desc;
            attr.UIVisible = uiVisable;
            attr.UIIsReadonly = isReadonly;
            this.Add(attr);
        }
        public void AddTBFloat(string key, float defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBFloat(key, key, defaultVal, desc, uiVisable, isReadonly);
        }
        #endregion  关于属性的增加 Float

        #region Decimal类型
        public void AddTBDecimal(string key, string _Field, decimal defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = _Field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppDouble;
            attr.Desc = desc;
            attr.UIVisible = uiVisable;
            attr.UIIsReadonly = isReadonly;
            this.Add(attr);
        }
        public void AddTBDecimal(string key, decimal defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBDecimal(key, key, defaultVal, desc, uiVisable, isReadonly);
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
        public void AddTBDate(string key, string field, string defaultVal,
            string desc, bool uiVisable, bool isReadonly)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppDate;
            attr.Desc = desc;
            attr.UIVisible = uiVisable;
            attr.UIIsReadonly = isReadonly;
            attr.MaxLength = 30;
            attr.MinLength = 0;
            attr.UIWidth = 100;
            this.Add(attr);
        }
        public void AddTBDate(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBDate(key, key, defaultVal, desc, uiVisable, isReadonly);
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
            string desc, bool uiVisable, bool isReadonly)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppDateTime;
            attr.Desc = desc;
            attr.UIVisible = uiVisable;
            attr.UIIsReadonly = isReadonly;
            attr.MaxLength = 30;
            attr.MinLength = 0;
            attr.UIWidth = 100;
            this.Add(attr);
        }
        public void AddTBDateTime(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBDateTime(key, key, defaultVal, desc, uiVisable, isReadonly);
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
        public void AddDDLSysEnum(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey)
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
            attr.UIBindKey = sysEnumKey;
            attr.UITag = cfgVal;
            attr.UIVisible = isUIVisable;
            attr.UIIsReadonly = !isUIEnable;
            this.Add(attr);
        }
        /// <summary>
        /// 自定义枚举类型
        /// </summary>
        /// <param name="key">键</param>		
        /// <param name="defaultVal">默认</param>
        /// <param name="desc">描述</param>
        /// <param name="sysEnumKey">Key</param>
        public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVals)
        {
            AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, cfgVals);
        }
        public void AddDDLSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable)
        {
            AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, key);
        }
        #endregion

        #region 集合属性.
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
        public void AddDDLEntities(string key, string field, object defaultVal, int dataType, FieldType _fildType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = dataType;
            attr.MyFieldType = _fildType;

            attr.Desc = desc;
            attr.UIContralType = UIContralType.DDL;
            attr.UIBindKey = ens.ToString();
            //attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();
            attr.HisFKEns = ens;

            attr.HisFKEns = ens;
            attr.UIRefKeyText = refText;
            attr.UIRefKeyValue = refKey;
            attr.UIIsReadonly = uiIsEnable;
            this.Add(attr, true, false);
        }

        #region DDLSQL
        public void AddDDLSQL(string key, object defaultVal, string desc, string sql, bool uiIsEnable = true)
        {
            if (defaultVal == null)
                defaultVal = "";

            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = key;

            if (typeof(int) == defaultVal.GetType())
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

        public void AddDDLEntities(string key, string field, object defaultVal, int dataType, string desc, Entities ens, string refKey, string refText, bool uiIsEnable)
        {
            AddDDLEntities(key, field, defaultVal, dataType, FieldType.FK, desc, ens, refKey, refText, uiIsEnable);
        }

        #endregion

        #region entityNoName
        public void AddDDLEntities(string key, object defaultVal, string desc, EntitiesNoName ens, bool uiIsEnable)
        {
            this.AddDDLEntities(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
        }
        public void AddDDLEntities(string key, object defaultVal, string desc, EntitiesTree ens, bool uiIsEnable)
        {
            this.AddDDLEntities(key, key, defaultVal, DataType.AppString, desc, ens, "No", "Name", uiIsEnable);
        }
        #endregion

        /// <summary>
        /// 是否包含属性key。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            foreach (Attr attr in this)
            {
                if (attr.Key.Equals(key) == true)
                    return true;
            }
            return false;
        }

        public Attr GetAttrByKey(string key)
        {
            foreach (Attr item in this)
            {
                if (item.Key.Equals(key) == true)
                    return item;
            }
            return null;
        }
        public Attr GetAttrByKeyOfEn(string f)
        {
            foreach (Attr item in this)
            {
                if (DataType.IsNullOrEmpty(item.Field) == true)
                    continue;
                if (item.Field.Equals(f) == true)
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
                            ap.GetValIntByKey("DefVal"), ap.GetValStrByKey("Name"), true, true, ap.GetValStrByKey("Key"), "@" + ap.GetValStrByKey("BindKey").Replace(",", "@"));
                        break;
                    case FieldTypeS.FK:
                        EntitiesNoName ens = (EntitiesNoName)ClassFactory.GetEns(ap.GetValStrByKey("BindKey"));
                        this.AddDDLEntities(ap.GetValStrByKey("Key"), ap.GetValStrByKey("DefVal"), ap.GetValStrByKey("Name"), ens, true);
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
        public Attrs()
        {
        }
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
                    mattr.setKeyOfEn(item.Key);
                    mattr.setName(item.Desc);
                    mattr.setMyDataType(item.MyDataType);
                    mattr.setUIContralType(item.UIContralType);
                    mattr.setUIBindKey(item.UIBindKey);

                    mattr.setUIWidth(item.UIWidthInt);
                    mattr.setUIHeight(item.UIHeight);

                    mattr.setMaxLen(item.MaxLength);
                    mattr.setMinLen(item.MinLength);
                    mattr.setUIVisible(item.UIVisible);
                    mattr.setDefValReal(item.DefaultValOfReal);
                    mattr.setDefValType(item.DefValType);

                    mattr.setUIIsEnable(!item.UIIsReadonly);



                    if (item.IsSupperText == 1)
                        mattr.TextModel = 3;
                    if (item.UIHeight > 10)
                    {
                        if (item.UIIsLine == true)
                            mattr.ColSpan = 4;
                        else
                            mattr.ColSpan = 3;
                    }
                    else
                    {
                        if (item.UIIsLine == true)
                            mattr.ColSpan = 3;
                    }

                    //帮助url.
                    mattr.SetPara("HelpUrl", item.HelperUrl);
                    mattr.UIRefKeyText = item.UIRefKeyText;
                    mattr.UIRefKey = item.UIRefKeyValue;

                    if (item.MyFieldType == FieldType.Enum)
                        mattr.LGType = FieldTypeS.Enum;

                    if (item.MyFieldType == FieldType.FK)
                        mattr.LGType = FieldTypeS.FK;

                    mapAttrs.AddEntity(mattr);
                }
                return mapAttrs;
            }
        }
        public void Add(Attr attr, bool isClearGroupName = false)
        {
            if (isClearGroupName == false)
                attr.GroupName = this.currGroupAttrName;

            if (attr.Field == null || attr.Field == "")
            {
                attr.Field = attr.Key; //@wwh.
                // throw new Exception("@属性设置错误：您不能设置 key='" + attr.Key + "', " + attr.Desc + ",得字段值为空");
            }

            bool k = attr.IsKeyEqualField;
            this.Add(attr, true, false, isClearGroupName);
        }
        /// <summary>
        /// 加入一个属性。
        /// </summary>
        /// <param name="attr">attr</param>
        /// <param name="isAddHisRefText">isAddHisRefText</param>
        public void Add(Attr attr, bool isAddHisRefText, bool isAddHisRefName, bool isClearGroupName = false)
        {
            foreach (Attr myattr in this)
            {
                if (myattr.Key == attr.Key)
                    return;
            }

            if (isClearGroupName==false)
            attr.GroupName = this.currGroupAttrName;

            this.InnerList.Add(attr);

            if (isAddHisRefText)
                this.AddRefAttrText(attr);

            if (isAddHisRefName)
                this.AddRefAttrName(attr);
        }
        private void AddRefAttrText(Attr attr)
        {
            if (attr.MyFieldType == FieldType.Enum && attr.MyDataType == DataType.AppString)
                return;
            if (attr.MyFieldType == FieldType.FK
                || attr.MyFieldType == FieldType.Enum
                || attr.MyFieldType == FieldType.PKEnum
                || attr.MyFieldType == FieldType.PKFK)
            {

                Attr myattr = new Attr();
                myattr.GroupName = this.currGroupAttrName;
                myattr.MyFieldType = FieldType.RefText;
                myattr.MyDataType = DataType.AppString;
                myattr.UIContralType = UIContralType.TB;
                myattr.UIWidth = attr.UIWidth * 2;
                myattr.Key = attr.Key + "Text";

                myattr.UIIsReadonly = true;
                myattr.UIBindKey = attr.UIBindKey;
                // myattr.UIBindKeyOfEn = attr.UIBindKeyOfEn;
                myattr.HisFKEns = attr.HisFKEns;

                //myattr.Desc=attr.Desc+"名称";

                string desc = myattr.Desc = "名称";
                if (desc.IndexOf("编号") >= 0)
                    myattr.Desc = attr.Desc.Replace("编号", "名称");
                else
                    myattr.Desc = attr.Desc + "名称";

                if (attr.UIContralType == UIContralType.DDL)
                    myattr.UIVisible = false;

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
                myattr.GroupName = this.currGroupAttrName;
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
