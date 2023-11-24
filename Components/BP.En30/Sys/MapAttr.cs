using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using System.IO;
using BP.Pub;

namespace BP.Sys
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public class MapAttrAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 实体标识
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 物理表
        /// </summary>
        public const string KeyOfEn = "KeyOfEn";
        /// <summary>
        /// 实体名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 默认值
        /// </summary>
        public const string DefVal = "DefVal";
        /// <summary>
        /// 字段
        /// </summary>
        public const string Field = "Field";
        /// <summary>
        /// 最大长度
        /// </summary>
        public const string MaxLen = "MaxLen";
        /// <summary>
        /// 最小长度
        /// </summary>
        public const string MinLen = "MinLen";
        /// <summary>
        /// 绑定的值
        /// </summary>
        public const string UIBindKey = "UIBindKey";
        /// <summary>
        /// 空件类型
        /// </summary>
        public const string UIContralType = "UIContralType";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string UIWidth = "UIWidth";
        /// <summary>
        /// UIHeight
        /// </summary>
        public const string UIHeight = "UIHeight";
        /// <summary>
        /// 是否只读
        /// </summary>
        public const string UIIsEnable = "UIIsEnable";
        /// <summary>
        /// 关联的表的Key
        /// </summary>
        public const string UIRefKey = "UIRefKey";
        /// <summary>
        /// 关联的表的Lab
        /// </summary>
        public const string UIRefKeyText = "UIRefKeyText";
        /// <summary>
        /// 是否可见的
        /// </summary>
        public const string UIVisible = "UIVisible";
        /// <summary>
        /// 是否单独行显示
        /// </summary>
        public const string UIIsLine = "UIIsLine";
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 标识（存放临时数据）
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// 扩展字段3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// MyDataType
        /// </summary>
        public const string MyDataType = "MyDataType";
        /// <summary>
        /// 逻辑类型
        /// </summary>
        public const string LGType = "LGType";
        /// <summary>
        /// 编辑类型
        /// </summary>
        public const string EditType = "EditType";
        /// <summary>
        /// 自动填写内容
        /// </summary>
        public const string AutoFullDoc = "AutoFullDoc";
        /// <summary>
        /// 自动填写方式
        /// </summary>
        public const string AutoFullWay = "AutoFullWay";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 图标
        /// </summary>
        public const string ICON = "ICON";
        /// <summary>
        /// 是否是签字
        /// </summary>
        public const string IsSigan = "IsSigan";
        /// <summary>
        /// 字体大小
        /// </summary>
        public const string FontSize = "FontSize";



        /// <summary>
        /// TabIdx
        /// </summary>
        public const string TabIdx = "TabIdx";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 合并单元格数
        /// </summary>
        public const string ColSpan = "ColSpan";

        /// <summary>
        /// 文本合并单元格数
        /// </summary>
        public const string LabelColSpan = "LabelColSpan";

        /// <summary>
        /// 合并行数
        /// </summary>
        public const string RowSpan = "RowSpan";

        /// <summary>
        /// 签名字段
        /// </summary>
        public const string SiganField = "SiganField";
        /// <summary>
        /// 操作提示
        /// </summary>
        public const string Tip = "Tip";
        /// <summary>
        /// 是否自动签名
        /// </summary>
        public const string PicType = "PicType";
        /// <summary>
        /// 是否是img字段
        /// </summary>
        public const string IsImgField = "IsImgField";
        /// <summary>
        /// 类型
        /// </summary>
        public const string TBModel = "TBModel";

        public const string CSSCtrl = "CSSCtrl";

        public const string CSSLabel = "CSSLabel";

        #region 参数属性.
        /// <summary>
        /// 是否必填
        /// </summary>
        public const string UIIsInput = "UIIsInput";
        #endregion 参数属性.

        /// <summary>
        /// 数值字段是否合计
        /// </summary>
        public const string IsSum = "IsSum";
        /// <summary>
        /// 列求和
        /// </summary>
        public const string ExtIsSum = "ExtIsSum";
        /// <summary>
        /// 在手机端是否显示
        /// </summary>
        public const string IsEnableInAPP = "IsEnableInAPP";
        public const string IsSupperText = "IsSupperText";
        /// <summary>
        /// 默认值设置方式
        /// </summary>
        public const string DefValType = "DefValType";
        public const string DefaultVal = "";

        public const string TextModel = "TextModel";
    }
    /// <summary>
    /// 实体属性
    /// </summary>
    public class MapAttr : EntityMyPK
    {
        #region 文本字段参数属性.
        /// <summary>
        /// 是否是超大文本？
        /// </summary>
        public int ItIsSupperText
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.IsSupperText, 0);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSupperText, value);
            }
        }

        public int TextModel
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.TextModel);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.TextModel, value);
            }
        }
        /// <summary>
        /// 是否启用二维码？
        /// </summary>
        public bool ItIsEnableQrCode
        {
            get
            {
                return this.GetParaBoolen("IsEnableQrCode", false);
            }
            set
            {
                this.SetPara("IsEnableQrCode", value);
            }
        }
        #endregion

        #region 数值字段参数属性,2017-1-9,liuxc
        /// <summary>
        /// 数值字段是否合计(默认true)
        /// </summary>
        public bool ItIsSum
        {
            get
            {
                return this.GetParaBoolen(MapAttrAttr.IsSum, true);
            }
            set
            {
                this.SetPara(MapAttrAttr.IsSum, value);
            }
        }
        public bool ExtIsSum
        {
            get
            {
                return this.GetParaBoolen(MapAttrAttr.ExtIsSum, true);
            }
            set
            {
                this.SetPara(MapAttrAttr.ExtIsSum, value);
            }
        }

        #endregion

        #region 参数属性.
        /// <summary>
        /// 是否必填字段
        /// </summary>
        public bool UIIsInput
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIIsInput, false);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIIsInput, value);
            }
        }
        /// <summary>
        /// 在手机端中是否显示
        /// </summary>
        public bool ItIsEnableInAPP
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsEnableInAPP, true);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsEnableInAPP, value);
            }
        }
        /// <summary>
        /// 是否启用高级JS设置
        /// </summary>
        public bool ItIsEnableJS
        {
            get
            {
                return this.GetParaBoolen("IsEnableJS", false);
            }
            set
            {
                this.SetPara("IsEnableJS", value);
            }
        }
        #endregion

        #region 属性
        public EntitiesNoName _ens = null;
        /// <summary>
        /// 实体类
        /// </summary>
        public EntitiesNoName HisEntitiesNoName
        {
            get
            {
                if (this.UIBindKey.Contains("."))
                {
                    EntitiesNoName ens = (EntitiesNoName)BP.En.ClassFactory.GetEns(this.UIBindKey);
                    if (ens == null)
                        return null;

                    ens.RetrieveAll();
                    return ens;
                }

                if (_ens == null)
                {
                    SFTable sf = new SFTable(this.UIBindKey);

                    if (sf.SFDBSrcNo.Equals("local"))
                    {
                        GENoNames myens = new GENoNames(this.UIBindKey, this.Name);

                        if (sf.SrcType == DictSrcType.SQL)
                        {
                            //此种类型时，没有物理表或视图，从SQL直接查出数据
                            DataTable dt = sf.GenerHisDataTable();
                            EntityNoName enn = null;
                            foreach (DataRow row in dt.Rows)
                            {
                                enn = myens.GetNewEntity as EntityNoName;
                                enn.No = row["No"] as string;
                                enn.Name = row["Name"] as string;

                                myens.AddEntity(enn);
                            }
                        }
                        else
                        {
                            myens.RetrieveAll();
                        }

                        _ens = myens;
                    }
                    else
                    {
                        GENoNames myens = new GENoNames(this.UIBindKey, this.Name);
                        _ens = myens;
                        //throw new Exception("@非实体类实体不能获取EntitiesNoName。");
                    }
                }
                return _ens;
            }
        }

        private DataTable _dt = null;
        /// <summary>
        /// 外部数据表
        /// </summary>
        public DataTable HisDT
        {
            get
            {
                if (_dt == null)
                {
                    if (DataType.IsNullOrEmpty(this.UIBindKey))
                        throw new Exception("@属性：" + this.MyPK + " 丢失属性 UIBindKey 字段。");

                    SFTable sf = new SFTable(this.UIBindKey);
                    _dt = sf.GenerHisDataTable();
                }
                return _dt;
            }
        }
        /// <summary>
        /// 是否是导入过来的字段
        /// </summary>
        public bool ItIsTableAttr
        {
            get
            {
                return DataType.IsNumStr(this.KeyOfEn.Replace("F", ""));
            }
        }
        /// <summary>
        /// 转换成属性.
        /// </summary>
        public Attr HisAttr
        {
            get
            {
                Attr attr = new Attr();
                attr.Key = this.KeyOfEn;
                attr.Desc = this.Name;


                string s = this.DefValReal;
                if (DataType.IsNullOrEmpty(s))
                    attr.DefaultValOfReal = null;
                else
                    attr.DefaultValOfReal = this.DefValReal;

                attr.DefValType = this.DefValType;

                attr.Field = this.Field;
                attr.MaxLength = this.MaxLen;
                attr.MinLength = this.MinLen;
                attr.UIBindKey = this.UIBindKey;
                attr.UIIsLine = this.UIIsLine;
                attr.UIHeight = 0;
                if (this.UIHeight > 30)
                    attr.UIHeight = (int)this.UIHeight;

                attr.UIWidth = this.UIWidth;
                attr.MyDataType = this.MyDataType;
                attr.UIRefKeyValue = this.UIRefKey;
                attr.UIRefKeyText = this.UIRefKeyText;
                attr.UIVisible = this.UIVisible;
                attr.MyFieldType = FieldType.Normal; //普通类型的字段.
                if (this.ItIsPK)
                    attr.MyFieldType = FieldType.PK;

                attr.ItIsSupperText = this.ItIsSupperText;

                switch (this.LGType)
                {
                    case FieldTypeS.Enum:
                        attr.UIContralType = this.UIContralType;
                        attr.MyFieldType = FieldType.Enum;
                        attr.UIIsReadonly = this.UIIsEnable;
                        break;
                    case FieldTypeS.FK:
                        attr.UIContralType = this.UIContralType;
                        attr.MyFieldType = FieldType.FK;
                        //attr.UIRefKeyValue = "No";
                        //attr.UIRefKeyText = "Name";
                        attr.UIIsReadonly = this.UIIsEnable;
                        break;
                    default:

                        if (this.ItIsPK)
                            attr.MyFieldType = FieldType.PK;

                        attr.UIIsReadonly = !this.UIIsEnable;
                        switch (this.MyDataType)
                        {
                            case DataType.AppBoolean:
                                attr.UIContralType = UIContralType.CheckBok;
                                attr.UIIsReadonly = this.UIIsEnable;
                                break;
                            case DataType.AppDate:
                                if (this.Tag.Equals("1"))
                                    attr.DefaultVal = DataType.CurrentDate;
                                break;
                            case DataType.AppDateTime:
                                if (this.Tag.Equals("1"))
                                    attr.DefaultVal = DataType.CurrentDate;
                                break;
                            default:
                                attr.UIContralType = this.UIContralType;
                                break;
                        }
                        break;
                }
                return attr;
            }
        }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool ItIsPK
        {
            get
            {
                switch (this.KeyOfEn)
                {
                    case "OID":
                    case "No":
                    case "MyPK":
                    case "NodeID":
                    case "WorkID":
                        return true;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// 编辑类型
        /// </summary>
        public EditType HisEditType
        {
            get
            {
                return (EditType)this.GetValIntByKey(MapAttrAttr.EditType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.EditType, (int)value);
            }
        }
        public void setEditType(EditType val)
        {
            this.SetValByKey(MapAttrAttr.EditType, (int)val);
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.FK_MapData);
            }
            set {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段名
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.KeyOfEn);
            }
        }
        public void setKeyOfEn(string val)
        {

            this.SetValByKey(MapAttrAttr.KeyOfEn, val);
        }
        public FieldTypeS LGType
        {
            get
            {
                return (FieldTypeS)this.GetValIntByKey(MapAttrAttr.LGType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.LGType, (int)value);
            }
        }
        public void setLGType(FieldTypeS val)
        {
            this.SetValByKey(MapAttrAttr.LGType, (int)val);
        }
        public string LGTypeT
        {
            get
            {
                return this.GetValRefTextByKey(MapAttrAttr.LGType);
            }
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string Name
        {
            get
            {
                string s = this.GetValStrByKey(MapAttrAttr.Name);
                if (DataType.IsNullOrEmpty(s) == true)
                    return this.KeyOfEn;
                return s;
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Name, value);
            }
        }
        public void setName(string val)
        {
            this.SetValByKey(MapAttrAttr.Name, val);
        }

        public bool ItIsNum
        {
            get
            {
                switch (this.MyDataType)
                {
                    case DataType.AppString:
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                    case DataType.AppBoolean:
                        return false;
                    default:
                        return true;
                }
            }
        }
        public decimal DefValDecimal
        {
            get
            {
                return decimal.Parse(this.DefVal);
            }
        }
        public string DefValReal
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.DefVal);
            }
        }
        public void setDefValReal(string val)
        {
            this.SetValByKey(MapAttrAttr.DefVal, val);
        }
        public int DefValType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.DefValType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefValType, value);
            }
        }
        public void setDefValType(int val)
        {
            this.SetValByKey(MapAttrAttr.DefValType, val);
        }
        /// <summary>
        /// 合并单元格数
        /// </summary>
        public int ColSpan
        {
            get
            {
                int i = this.GetValIntByKey(MapAttrAttr.ColSpan);
                if (this.UIIsLine && i == 1)
                    return 3;
                if (i == 0)
                    return 1;
                return i;
            }
            set
            {
                this.SetValByKey(MapAttrAttr.ColSpan, value);
            }
        }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefVal
        {
            get
            {
                string s = this.GetValStrByKey(MapAttrAttr.DefVal);
                if (this.ItIsNum)
                {
                    if (s.Equals(""))
                        return "0";
                }

                switch (this.MyDataType)
                {
                    case DataType.AppDate:
                        if (this.Tag.Equals("1") || s.Equals("@RDT"))
                            return DataType.CurrentDate;
                        else
                            return "          ";
                    case DataType.AppDateTime:
                        if (this.Tag.Equals("1") || s.Equals("@RDT"))
                            return DataType.CurrentDateTime;
                        else
                            return "               ";
                    default:
                        break;
                }

                if (s.Contains("@") == false)
                    return s;

                switch (s.ToLower())
                {
                    case "@webuser.no":
                        return BP.Web.WebUser.No;
                    case "@webuser.name":
                        return BP.Web.WebUser.Name;
                    case "@webuser.fk_dept":
                        return BP.Web.WebUser.DeptNo;
                    case "@webuser.fk_deptname":
                        return BP.Web.WebUser.DeptName;
                    case "@webuser.fk_deptfullname":
                        return BP.Web.WebUser.DeptNameOfFull;
                    case "@fk_ny":
                        return DataType.CurrentYearMonth;
                    case "@fk_nd":
                        return DataType.CurrentYear;
                    case "@fk_yf":
                        return DataType.CurrentMonth;
                    case "@rdt":
                    case "@rd":
                        if (this.MyDataType == DataType.AppDate)
                            return DataType.CurrentDate;
                        return DataType.CurrentDateTime;
                    case "@yyyy年MM月dd日":
                        return DataType.CurrentDateCNOfLong;
                    case "@yyyy年MM月dd日hh时mm分":
                        return DataType.CurrentDateByFormart("yyyy年MM月dd日HH时mm分");
                    case "@yy年MM月dd日":
                        return DataType.CurrentDateCNOfShort;
                    case "@yy年MM月dd日hh时mm分":
                        return DataType.CurrentDateByFormart("yy年MM月dd日HH时mm分");
                    default:
                        return s;
                        //throw new Exception("没有约定的变量默认值类型" + s);
                }
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefVal, value);
            }
        }
        public void setDefVal(object val)
        {
            this.GetValStrByKey(MapAttrAttr.DefVal, val.ToString());
        }
        public bool DefValOfBool
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.DefVal, false);
            }

        }
        /// <summary>
        /// 字段
        /// </summary>
        public string Field
        {
            get
            {
                return this.KeyOfEn;
            }
        }

        public int MyDataType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MyDataType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.MyDataType, value);
            }
        }
        public void setMyDataType(int val)
        {
            this.SetValByKey(MapAttrAttr.MyDataType, val);
        }
        public string MyDataTypeS
        {
            get
            {
                switch (this.MyDataType)
                {
                    case DataType.AppString:
                        return "String";
                    case DataType.AppInt:
                        return "Int";
                    case DataType.AppFloat:
                        return "Float";
                    case DataType.AppMoney:
                        return "Money";
                    case DataType.AppDate:
                        return "Date";
                    case DataType.AppDateTime:
                        return "DateTime";
                    case DataType.AppBoolean:
                        return "Bool";
                    default:
                        throw new Exception("没有判断。");
                }
            }
            set
            {

                switch (value)
                {
                    case "String":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppString);
                        break;
                    case "Int":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppInt);
                        break;
                    case "Float":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppFloat);
                        break;
                    case "Money":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppMoney);
                        break;
                    case "Date":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDate);
                        break;
                    case "DateTime":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppDateTime);
                        break;
                    case "Bool":
                        this.SetValByKey(MapAttrAttr.MyDataType, DataType.AppBoolean);
                        break;
                    default:
                        throw new Exception("sdsdsd");
                }

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
        /// 最大长度
        /// </summary>
        public int MaxLen
        {
            get
            {
                switch (this.MyDataType)
                {
                    case DataType.AppDate:
                        return 100;
                    case DataType.AppDateTime:
                        return 100;
                    default:
                        break;
                }

                int i = this.GetValIntByKey(MapAttrAttr.MaxLen);
                if (i > 4000)
                    i = 4000;
                if (i == 0)
                    return 50;
                return i;
            }
        }
        public void setMaxLen(int val)
        {
            this.SetValByKey(MapAttrAttr.MaxLen, val);
        }
        /// <summary>
        /// 最小长度
        /// </summary>
        public int MinLen
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MinLen);
            }
        }
        public void setMinLen(int val)
        {
            this.SetValByKey(MapAttrAttr.MinLen, val);
        }
        /// <summary>
        /// 是否可以为空, 对数值类型的数据有效.
        /// </summary>
        public bool ItIsNull
        {
            get
            {
                if (this.MinLen == 0)
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// 所在的分组
        /// </summary>
        public int GroupID
        {
            get
            {
                string str = this.GetValStringByKey(MapAttrAttr.GroupID);
                if (str.Equals("无") || str.Equals(""))
                    return 1;
                return int.Parse(str);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.GroupID, value);
            }
        }
        public void setGroupID(int val)
        {
            this.SetValByKey(MapAttrAttr.GroupID, val);
        }
        /// <summary>
        /// 是否是大块文本？
        /// </summary>
        public bool ItIsBigDoc
        {
            get
            {
                if (this.UIRows > 1 && this.MyDataType == DataType.AppString)
                    return true;
                //if (this.ColSpan == 4 && this.MyDataType == DataType.AppString)
                //    return true;
                return false;
            }
        }
        /// <summary>
        /// textbox控件的行数.
        /// </summary>
        public int UIRows
        {
            get
            {
                if (this.UIHeight < 40)
                    return 1;

                decimal d = decimal.Parse(this.UIHeight.ToString()) / 23;
                return (int)Math.Round(d, 0);
            }
        }
        /// <summary>
        /// 高度
        /// </summary>
        public int UIHeightInt
        {
            get
            {
                return (int)this.UIHeight;
            }
            set
            {
                this.UIHeight = value;
            }
        }

        /// <summary>
        /// 高度
        /// </summary>
        public float UIHeight
        {
            get
            {
                return this.GetValFloatByKey(MapAttrAttr.UIHeight);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIHeight, value);
            }
        }
        public void setUIHeight(float val)
        {
            this.SetValByKey(MapAttrAttr.UIHeight, val);
        }
        /// <summary>
        /// 宽度
        /// </summary>
        public int UIWidthInt
        {
            get
            {
                return (int)this.UIWidth;
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        public float UIWidth
        {
            get
            {
                return this.GetValFloatByKey(MapAttrAttr.UIWidth);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIWidth, value);
            }
        }
        public void setUIWidth(float val)
        {
            this.SetValByKey(MapAttrAttr.UIWidth, val);
        }
        public int UIWidthOfLab
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 是否是否可见？
        /// </summary>
        public bool UIVisible
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIVisible);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIVisible, value);
            }
        }
        public void setUIVisible(bool val)
        {
            this.SetValByKey(MapAttrAttr.UIVisible, val);
        }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool UIIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIIsEnable);
            }
        }
        public void setUIIsEnable(bool val)
        {
            this.SetValByKey(MapAttrAttr.UIIsEnable, val);
        }
        /// <summary>
        /// 是否单独行显示
        /// </summary>
        public bool UIIsLine
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.UIIsLine);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIIsLine, value);
            }
        }
        public void setUIIsLine(bool val)
        {
            this.SetValByKey(MapAttrAttr.UIIsLine, val);
        }
        /// <summary>
        /// 是否数字签名
        /// </summary>
        public bool ItIsSigan
        {
            get
            {
                if (this.UIIsEnable)
                    return false;
                return this.GetValBooleanByKey(MapAttrAttr.IsSigan);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSigan, value);
            }
        }
        /// <summary>
        /// 签名类型
        /// </summary>
        public SignType SignType
        {
            get
            {
                //if (this.UIIsEnable)
                //    return SignType.None;
                return (SignType)this.GetValIntByKey(MapAttrAttr.IsSigan);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSigan, (int)value);
            }
        }
        public int Para_FontSize
        {
            get
            {
                return this.GetParaInt(MapAttrAttr.FontSize);
            }
            set
            {
                this.SetPara(MapAttrAttr.FontSize, value);
            }
        }
        /// <summary>
        /// radiobutton的展现方式
        /// </summary>
        public int RBShowModel
        {
            get
            {
                return this.GetParaInt("RBShowModel");
            }
            set
            {
                this.SetPara("RBShowModel", value);
            }
        }
        public void setRBShowModel(int val)
        {
            this.SetPara("RBShowModel", val);
        }

        /// <summary>
        /// 操作提示
        /// </summary>
        public string Para_Tip
        {
            get
            {
                return this.GetParaString(MapAttrAttr.Tip);
            }
            set
            {
                this.SetPara(MapAttrAttr.Tip, value);
            }
        }
        /// <summary>
        /// 是否数字签名
        /// </summary>
        public string Para_SiganField
        {
            get
            {
                if (this.UIIsEnable)
                    return "";
                return this.GetParaString(MapAttrAttr.SiganField);
            }
            set
            {
                this.SetPara(MapAttrAttr.SiganField, value);
            }
        }
        /// <summary>
        /// 签名类型
        /// </summary>
        public PicType PicType
        {
            get
            {
                if (this.UIIsEnable)
                    return PicType.Auto;
                return (PicType)this.GetParaInt(MapAttrAttr.PicType);
            }
            set
            {
                this.SetPara(MapAttrAttr.PicType, (int)value);
            }
        }
        /// <summary>
        /// TextBox类型
        /// </summary>
        public TBModel TBModel
        {
            get
            {
                return (TBModel)this.GetParaInt(MapAttrAttr.TBModel);
            }
            set
            {
                this.SetPara(MapAttrAttr.TBModel, (int)value);
            }
        }
        /// <summary>
        /// 绑定的值
        /// </summary>
        public string UIBindKey
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.UIBindKey);
            }
            set
            {
                string val = BP.Sys.Base.Glo.DealClassEntityName(value);
                this.SetValByKey(MapAttrAttr.UIBindKey, val);
            }
        }
        public void setUIBindKey(string val)
        {
            string myval = BP.Sys.Base.Glo.DealClassEntityName(val);
            this.SetValByKey(MapAttrAttr.UIBindKey, myval);
        }
        /// <summary>
        /// 关联的表的Key
        /// </summary>
        public string UIRefKey
        {
            get
            {
                string s = this.GetValStrByKey(MapAttrAttr.UIRefKey);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "No";
                return s;
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIRefKey, value);
            }
        }
        /// <summary>
        /// 关联的表的Lab
        /// </summary>
        public string UIRefKeyText
        {
            get
            {
                string s = this.GetValStrByKey(MapAttrAttr.UIRefKeyText);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "Name";
                return s;
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIRefKeyText, value);
            }
        }
        /// <summary>
        /// 标识
        /// </summary>
        public string Tag
        {
            get
            {
                return this.GetValStrByKey(MapAttrAttr.Tag);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Tag, value);
            }
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        public UIContralType UIContralType
        {
            get
            {
                return (UIContralType)this.GetValIntByKey(MapAttrAttr.UIContralType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIContralType, (int)value);

            }
        }
        public void setUIContralType(UIContralType val)
        {
            this.SetValByKey(MapAttrAttr.UIContralType, (int)val);
        }

        /// <summary>
        /// TabIdx
        /// </summary>
        public int TabIdx
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.TabIdx);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.TabIdx, value);
            }
        }
        /// <summary>
        /// 序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.Idx);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.Idx, value);
            }
        }
        public void setIdx(int val)
        {
            this.SetValByKey(MapAttrAttr.Idx, val);
        }
        #endregion

        #region 构造方法b
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttr()
        {
        }
        public MapAttr(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
        }
        public MapAttr(string fk_mapdata, string key)
        {
            this.SetValByKey(MapAttrAttr.FK_MapData, fk_mapdata);
            this.SetValByKey(MapAttrAttr.KeyOfEn, key);
            this.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.KeyOfEn, this.KeyOfEn);
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_MapAttr", "实体属性");

                map.AddMyPK();

                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", true, true, 1, 100, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "属性", true, true, 1, 200, 20);

                map.AddTBString(MapAttrAttr.Name, null, "描述", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.DefVal, null, "默认值", false, false, 0, 400, 20);
                map.AddTBInt(MapAttrAttr.DefValType, 1, "默认值类型", true, false);

                map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", true, false);
                map.AddTBInt(MapAttrAttr.MyDataType, 1, "数据类型", true, false);

                map.AddDDLSysEnum(MapAttrAttr.LGType, 0, "逻辑类型", true, false, MapAttrAttr.LGType,
                    "@0=普通@1=枚举@2=外键@3=打开系统页面");

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, false);

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 300, "最大长度", true, false);

                map.AddTBString(MapAttrAttr.UIBindKey, null, "绑定的信息", true, false, 0, 100, 20);
                map.AddTBString(MapAttrAttr.UIRefKey, null, "绑定的Key", true, false, 0, 30, 20);
                map.AddTBString(MapAttrAttr.UIRefKeyText, null, "绑定的Text", true, false, 0, 30, 20);


                map.AddTBInt(MapAttrAttr.ExtIsSum, 0, "是否显示合计(对从表有效)", true, true);
                map.AddTBInt(MapAttrAttr.UIVisible, 1, "是否可见", true, true);
                map.AddTBInt(MapAttrAttr.UIIsEnable, 1, "是否启用", true, true);
                map.AddTBInt(MapAttrAttr.UIIsLine, 0, "是否单独栏显示", true, true);
                map.AddTBInt(MapAttrAttr.UIIsInput, 0, "是否必填字段", true, true);
                map.AddTBInt(MapAttrAttr.TextModel, 0, "TextModel", true, true);
                map.AddTBInt(MapAttrAttr.IsSupperText, 0, "是否是大文本", true, true);
                map.AddTBInt(MapAttrAttr.FontSize, 0, "字体大小", true, true);

                // 是否是签字，操作员字段有效。2010-09-23 增加。 @0=无@1=图片签名@2=CA签名.
                map.AddTBInt(MapAttrAttr.IsSigan, 0, "签字？", true, false);

                map.AddTBString(MapAttrAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                map.AddTBInt(MapAttrAttr.EditType, 0, "编辑类型", true, false);

                map.AddTBString(MapAttrAttr.Tag, null, "标识", true, false, 0, 100, 20);
                map.AddTBString(MapAttrAttr.Tag1, null, "标识1", true, false, 0, 4000, 20);
                map.AddTBString(MapAttrAttr.Tag2, null, "标识2", true, false, 0, 100, 20);
                map.AddTBString(MapAttrAttr.Tag3, null, "标识3", true, false, 0, 100, 20);

                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", false, true, 0, 200, 20);

                //单元格数量。2013-07-24 增加。
                //  map.AddTBString(MapAttrAttr.ColSpan, "1", "单元格数量", true, false, 0, 3, 3);
                map.AddTBInt(MapAttrAttr.ColSpan, 1, "单元格数量", true, false);

                //文本占单元格数量
                map.AddTBInt(MapAttrAttr.LabelColSpan, 1, "文本单元格数量", true, false);

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddTBInt(MapAttrAttr.GroupID, 0, "显示的分组", false, true);

                map.AddBoolean(MapAttrAttr.IsEnableInAPP, true, "是否在移动端中显示", true, true);

                // xxx 新增的样式.
                map.AddTBString(MapAttrAttr.CSSCtrl, "0", "CSSCtrl自定义样式", true, false, 0, 50, 20);
                map.AddTBString(MapAttrAttr.CSSLabel, "0", "CSSLabel标签样式", true, false, 0, 50, 20);
                map.AddTBInt(MapAttrAttr.Idx, 0, "序号", true, false);
                map.AddTBString(MapAttrAttr.ICON, "0", "ICON", true, false, 0, 50, 20);
                //参数属性.
                map.AddTBAtParas(4000); //


                map.AddLang(); //增加多语言.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 移动插入
        /// </summary>
        /// <param name="insertPK"></param>
        /// <returns></returns>
        public string InsertTo(string insertPK)
        {
            this.DoOrderInsertTo("Idx", insertPK, MapAttrAttr.GroupID);
            return "执行成功.";
        }
        /// <summary>
        /// 保存大块html文本
        /// </summary>
        /// <returns></returns>
        public string SaveBigNoteHtmlText(string text)
        {
            string file = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/BigNoteHtmlText/" + this.FrmID + ".htm";
            //若文件夹不存在，则创建
            string folder = System.IO.Path.GetDirectoryName(file);
            if (System.IO.Directory.Exists(folder) == false)
                System.IO.Directory.CreateDirectory(folder);

            DataType.WriteFile(file, text);
            return "保存成功！";
        }
        //删除大块文本信息
        public string DeleteBigNoteHtmlText()
        {
            string file = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/BigNoteHtmlText/" + this.FrmID + ".htm";

            if (System.IO.File.Exists(file) == true)
                System.IO.File.Delete(file);

            this.Delete();


            return "删除成功！";
        }
        /// <summary>
        /// 读取大块html文本
        /// </summary>
        /// <returns></returns>
        public string ReadBigNoteHtmlText()
        {
            string doc = "";
            string file = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/BigNoteHtmlText/" + this.FrmID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            if (System.IO.Directory.Exists(folder) != false)
            {
                if (File.Exists(file))
                {
                    doc = DataType.ReadTextFile(file);

                }
            }

            return doc;
        }
        public void DoDownTabIdx()
        {
            this.DoOrderDown(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);
        }
        public void DoUpTabIdx()
        {
            this.DoOrderUp(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);
        }
        public string DoUp()
        {
            this.DoOrderUp(MapAttrAttr.GroupID, this.GroupID.ToString(), MapAttrAttr.Idx);

            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_Title");
            if (attr.RetrieveFromDBSources() == 1)
            {
                //  attr.Idx = -1;
                attr.Update("Idx", -1);
            }
            return "执行成功";
        }
        //字段插队
        public string DoInsertTo(string entityPK)
        {
            this.DoOrderInsertTo(MapAttrAttr.Idx, entityPK, MapAttrAttr.GroupID);

            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_Title");
            if (attr.RetrieveFromDBSources() == 1)
            {
                attr.Update("Idx", -1);
            }
            return "执行成功！";
        }
        /// <summary>
        /// 生成他的外键字典数据,转化为json.
        /// </summary>
        /// <returns></returns>
        public string GenerHisFKData()
        {
            SFTable sf = new SFTable(this.UIBindKey);
            return BP.Tools.Json.ToJson(sf.GenerHisDataTable());
        }

        /// <summary>
        /// 下移
        /// </summary>
        public string DoDown()
        {
            this.DoOrderDown(MapAttrAttr.GroupID, this.GroupID.ToString(), MapAttrAttr.Idx);

            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_Title");
            if (attr.RetrieveFromDBSources() == 1)
            {
                attr.Update("Idx", -1);
            }
            return "执行成功";
        }
        /// <summary>
        /// 上移for 明细表.
        /// </summary>
        public string DoUpForMapDtl()
        {
            //规整groupID.
            GroupField gf = new GroupField();
            gf.Retrieve(GroupFieldAttr.FrmID, this.FrmID);
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.FrmID + "'");

            this.DoOrderUp(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.UIVisible, "1", MapAttrAttr.Idx);

            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_Title");
            if (attr.RetrieveFromDBSources() == 1)
            {
                //  attr.Idx = -1;
                attr.Update("Idx", -1);
            }
            return "执行成功";
        }
        /// <summary>
        /// 下移 for 明细表.
        /// </summary>
        public string DoDownForMapDtl()
        {
            //规整groupID.
            GroupField gf = new GroupField();
            gf.Retrieve(GroupFieldAttr.FrmID, this.FrmID);
            DBAccess.RunSQL("UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.FrmID + "'");

            this.DoOrderDown(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.UIVisible, "1", MapAttrAttr.Idx);

            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_Title");
            if (attr.RetrieveFromDBSources() == 1)
            {
                attr.Update("Idx", -1);
            }

            return "执行成功";
        }
        public void DoJump(MapAttr attrTo)
        {
            if (attrTo.Idx <= this.Idx)
                this.DoJumpUp(attrTo);
            else
                this.DoJumpDown(attrTo);
        }
        private string DoJumpUp(MapAttr attrTo)
        {
            string sql = "UPDATE Sys_MapAttr SET Idx=Idx+1 WHERE Idx <=" + attrTo.Idx + " AND FK_MapData='" + this.FrmID + "' AND GroupID=" + this.GroupID;
            DBAccess.RunSQL(sql);
            this.Idx = attrTo.Idx - 1;
            this.SetValByKey(MapAttrAttr.GroupID, attrTo.GroupID);

            this.Update();
            return null;
        }
        private string DoJumpDown(MapAttr attrTo)
        {
            string sql = "UPDATE Sys_MapAttr SET Idx=Idx-1 WHERE Idx <=" + attrTo.Idx + " AND FK_MapData='" + this.FrmID + "' AND GroupID=" + this.GroupID;
            DBAccess.RunSQL(sql);
            this.Idx = attrTo.Idx + 1;
            this.SetValByKey(MapAttrAttr.GroupID, attrTo.GroupID);

            this.Update();
            return null;
        }
        protected override bool beforeUpdateInsertAction()
        {
            //if (this.LGType == FieldTypeS.Normal)
            //    if (this.UIIsEnable == true &&this.DefVal !=null &&  this.DefVal.Contains("@") == true)
            //        throw new Exception("@不能在非只读(不可编辑)的字段设置具有@的默认值. 您设置的默认值为:" + this.DefVal);
            //if (this.UIContralType == En.UIContralType.DDL && this.LGType == FieldTypeS.Normal)

            //added by liuxc,2016-12-2
            //判断当前属性是否有分组，没有分组，则自动创建一个分组，并关联
            if (this.GroupID.ToString().Equals("1"))
            {
                //查找分组，查找到的第一个分组，关联当前属性
                GroupField group = new GroupField();
                if (group.Retrieve(GroupFieldAttr.FrmID, this.FrmID) > 0)
                {
                    this.SetValByKey(MapAttrAttr.GroupID, group.OID);

                }
                else
                {
                    group.FrmID = this.FrmID;
                    group.Lab = "基础信息";
                    group.Idx = 1;
                    group.Insert();

                    this.SetValByKey(MapAttrAttr.GroupID, group.OID);
                }
            }

            if (this.LGType == FieldTypeS.Enum && this.UIContralType == UIContralType.RadioBtn)
            {
                string sql = "UPDATE Sys_FrmRB SET UIIsEnable=" + this.GetValIntByKey(MapAttrAttr.UIIsEnable) + " WHERE FK_MapData='" + this.FrmID + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);
            }

            //为日期类型固定宽度.
            if (this.MyDataType == DataType.AppDate)
                this.UIWidth = 125;
            if (this.MyDataType == DataType.AppDateTime)
                this.UIWidth = 165;
            if(this.MyDataType == DataType.AppString)
            {
                MapAttr attr = new MapAttr();
                attr.setMyPK(this.MyPK);
                attr.RetrieveFromDBSources();
                if (this.TextModel == 2 || this.TextModel == 3)
                {
                    //attr.setMaxLen(4000);
                    this.SetValByKey(MapAttrAttr.MaxLen, 4000);
                }


               //自动扩展字段长度. 需要翻译.
                if (attr.MaxLen < this.MaxLen && DataType.IsNullOrEmpty(this.Field)==false)
                {
                    string sql = "";
                    MapData md = new MapData();
                    md.No = this.FrmID;
                    if (md.RetrieveFromDBSources() == 1)
                    {
                        if (DBAccess.IsExitsTableCol(md.PTable, this.KeyOfEn) == true)
                        {
                            switch (BP.Difference.SystemConfig.AppCenterDBType)
                            {
                                case DBType.MSSQL:
                                    sql = "ALTER TABLE " + md.PTable + " ALTER column " + this.Field + " NVARCHAR(" + this.MaxLen + ")";
                                    break;
                                case DBType.MySQL:
                                    if (this.MaxLen >= 4000)
                                        sql = "ALTER table " + md.PTable + " modify " + this.Field + " text";
                                    else
                                        sql = "ALTER table " + md.PTable + " modify " + this.Field + " NVARCHAR(" + this.MaxLen + ")";
                                    break;
                                case DBType.Oracle:
                                case DBType.DM:
                                    sql = "ALTER table " + md.PTable + " modify " + this.Field + " VARCHAR2(" + this.MaxLen + ")";
                                    break;
                                case DBType.KingBaseR3:
                                case DBType.KingBaseR6:
                                    sql = "ALTER table " + md.PTable + " ALTER COLUMN " + this.Field + " Type NVARCHAR2(" + this.MaxLen + ")";
                                    break;
                                case DBType.PostgreSQL:
                                case DBType.UX:
                                case DBType.HGDB:
                                    sql = "ALTER table " + md.PTable + " alter " + this.Field + " type character varying(" + this.MaxLen + ")";
                                    break;
                                default:
                                    throw new Exception("err@没有判断的数据库类型.");
                            }
                            DBAccess.RunSQL(sql); //如果是oracle如果有nvarchar与varchar类型，就会出错.
                        }
                    }
                }
            }
            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeUpdate()
        {
            switch (this.MyDataType)
            {
                case DataType.AppDateTime:
                    this.SetValByKey(MapAttrAttr.MaxLen, 20);
                    break;
                case DataType.AppDate:
                    this.SetValByKey(MapAttrAttr.MaxLen, 10);
                    break;
                default:
                    break;
            }

            if (DataType.IsNullOrEmpty(this.KeyOfEn))
                this.setMyPK(this.FrmID);
            else
                this.setMyPK(this.FrmID + "_" + this.KeyOfEn);

            return base.beforeUpdate();
        }
        /// <summary>
        /// 插入之间需要做的事情.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.Name))
                throw new Exception("@请输入字段名称。");

            if (this.KeyOfEn == null || this.KeyOfEn.Trim().Equals(""))
            {
                try
                {
                    this.SetValByKey(MapAttrAttr.KeyOfEn, CCFormAPI.ParseStringToPinyinField(this.Name, true, true, 100));

                    if (this.KeyOfEn.Length > 20)
                        this.SetValByKey(MapAttrAttr.KeyOfEn, CCFormAPI.ParseStringToPinyinField(this.Name, false, true, 20));

                    if (this.KeyOfEn == null || this.KeyOfEn.Trim().Equals(""))
                        throw new Exception("@请输入字段描述或者字段名称。");
                }
                catch (Exception ex)
                {
                    throw new Exception("@请输入字段描述或字段名称，异常信息:" + ex.Message);
                }
            }
            else
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, BP.Pub.PubClass.DealToFieldOrTableNames(this.KeyOfEn));
            }

            string keyofenC = this.KeyOfEn.Clone() as string;
            keyofenC = keyofenC.ToLower();
            string keyFields = PubClass.KeyFields;
            if (keyFields != null && keyFields.Contains("," + keyofenC + ",") == true)
                throw new Exception("@错误:[" + this.KeyOfEn + "]是字段关键字，您不能用它做字段。");

            if (this.IsExit(MapAttrAttr.KeyOfEn, this.KeyOfEn,
                MapAttrAttr.FK_MapData, this.FrmID))
            {
                return false;
               // throw new Exception("@在[" + this.MyPK + "]已经存在字段名称[" + this.Name + "]字段[" + this.KeyOfEn + "]");
            }

            if (this.Idx == 0)
                this.Idx = DBAccess.RunSQLReturnValInt("SELECT MAX(Idx) FROM Sys_MapAttr WHERE FK_MapData='" + this.FrmID + "'", 0) + 1;

            //
            if (this.GroupID == 0)
                this.GroupID = DBAccess.RunSQLReturnValInt("SELECT MAX(GroupID) FROM Sys_MapAttr WHERE FK_MapData='" + this.FrmID + "'", 0);

            this.setMyPK(this.FrmID + "_" + this.KeyOfEn);

            return base.beforeInsert();
        }
        protected override void afterInsert()
        {
            if (this.KeyOfEn.Equals("Tel") || this.Name.Contains("电话") || this.Name.Contains("手机"))
            {
                DBAccess.RunSQL("UPDATE Sys_MapAttr SET ICON='icon-phone' WHERE MyPK='" + this.MyPK + "'");
            }
            else if (this.KeyOfEn.Contains("Email") || this.Name.Contains("邮件") || this.Name.Contains("手机"))
            {
                DBAccess.RunSQL("UPDATE Sys_MapAttr SET ICON='icon-envelope-letter' WHERE MyPK='" + this.MyPK + "'");
            }
            else if (this.KeyOfEn.Contains("Addr") || this.Name.Contains("地址"))
            {
                DBAccess.RunSQL("UPDATE Sys_MapAttr SET ICON='icon-location-pin' WHERE MyPK='" + this.MyPK + "'");
            }
            else if (this.MyDataType == DataType.AppMoney)
            {
                DBAccess.RunSQL("UPDATE Sys_MapAttr SET ICON='fa-cny' WHERE MyPK='" + this.MyPK + "'");
            }


            base.afterInsert();
        }
        protected override void afterUpdate()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);

            base.afterUpdate();
        }
        /// <summary>
        /// 删除之前
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            string sqls = "DELETE FROM Sys_MapExt WHERE (AttrOfOper='" + this.KeyOfEn + "' OR AttrsOfActive='" + this.KeyOfEn + "' ) AND (FK_MapData='" + this.FrmID + "')";
            //删除权限管理字段.
            //sqls += "@DELETE FROM Sys_FrmSln WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + this.FrmID + "'";

            //如果外部数据，或者ws数据，就删除其影子字段.
            if (this.UIContralType == UIContralType.DDL && this.LGType == FieldTypeS.Normal)
                sqls += "@DELETE FROM Sys_MapAttr WHERE KeyOfEn='" + this.KeyOfEn + "T' AND FK_MapData='" + this.FrmID + "'";

            DBAccess.RunSQLs(sqls);
            return base.beforeDelete();
        }
        protected override void afterDelete()
        {
            if (this.UIContralType == UIContralType.AthShow)
            {
                //删除附件
                FrmAttachment ath = new FrmAttachment();
                ath.setMyPK(this.MyPK);
                ath.Delete();
            }
            base.afterDelete();
        }
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrs()
        {
        }
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrs(string frmID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MapAttrAttr.FK_MapData, frmID);
            qo.addOrderBy(MapAttrAttr.Idx);
            qo.DoQuery();
        }
        public int SearchMapAttrsYesVisable(string fk_map)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MapAttrAttr.FK_MapData, fk_map);
            qo.addAnd();
            qo.AddWhere(MapAttrAttr.UIVisible, 1);
            qo.addOrderBy(MapAttrAttr.Idx);
            // qo.addOrderBy(MapAttrAttr.Idx);
            return qo.DoQuery();
        }
        /// <summary>
        /// 查询日期+日期时间类型的
        ///  代码移动.
        /// </summary>
        /// <param name="fk_map"></param>
        /// <returns></returns>
        public string SearchMapAttrsDateAndDateTime(string fk_map)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MapAttrAttr.FK_MapData, fk_map);
            qo.addAnd();
            qo.AddWhere(MapAttrAttr.MyDataType, " IN ", "(6,7)");
            qo.addOrderBy(MapAttrAttr.Idx);
            qo.DoQuery();
            return this.ToJson();
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttr();
            }
        }
        public int WithOfCtl
        {
            get
            {
                int i = 0;
                foreach (MapAttr item in this)
                {
                    if (item.UIVisible == false)
                        continue;

                    i += item.UIWidthInt;
                }
                return i;
            }
        }
        #endregion


        #region 业务方法.

        #endregion


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttr> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttr>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttr> Tolist()
        {
            System.Collections.Generic.List<MapAttr> list = new System.Collections.Generic.List<MapAttr>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttr)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
