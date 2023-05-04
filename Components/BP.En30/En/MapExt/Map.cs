using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Difference;
using BP.Sys;

namespace BP.En
{
    /// <summary>
    /// EnMap 的摘要说明。
    /// </summary>
    public class Map
    {
        #region MapExt 类似的UI 设置.
        //public BP.En.FrmUI.Connections Connections = new FrmUI.Connections();
        #endregion MapExt 类似的UI 设置.

        #region 帮助.
        /// <summary>
        /// 是否是加密字段
        /// </summary>
        public bool IsJM = false;
        /// <summary>
        /// 增加帮助
        /// </summary>
        /// <param name="key">字段</param>
        /// <param name="url"></param>
        public void SetHelperUrl(string key, string url)
        {
            if (SystemConfig.IsDisHelp == true)
                return;
            Attr attr = this.GetAttrByKey(key);
            attr.HelperUrl = url;
        }

        /// <summary>
        /// 增加帮助
        /// </summary>
        /// <param name="key">字段</param>
        public void SetHelperBaidu(string key)
        {
            if (SystemConfig.IsDisHelp == true)
                return;
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
            if (SystemConfig.IsDisHelp == true)
                return;
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
            if (SystemConfig.IsDisHelp == true)
                return;
            context = context.Replace("@", "＠");
            Attr attr = this.GetAttrByKey(key);
            attr.HelperUrl = "javascript:alert('" + context + "')";
        }
        #endregion 帮助.

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
        private SearchNormals _SearchNormals = null;
        /// <summary>
        /// 查找属性
        /// </summary>
        public SearchNormals SearchNormals
        {
            get
            {
                if (this._SearchNormals == null)
                    this._SearchNormals = new SearchNormals();
                return this._SearchNormals;
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
        private SearchFKEnums _SearchAttrs = null;
        /// <summary>
        /// 查找的attrs
        /// </summary>
        public SearchFKEnums SearchFKEnums
        {
            get
            {
                if (this._SearchAttrs == null)
                    this._SearchAttrs = new SearchFKEnums();
                return this._SearchAttrs;
            }
        }
        /// <summary>
        /// 增加查询条件exp
        /// </summary>
        /// <param name="exp">表达式</param>
        public void AddHidden(string exp)
        {
            SearchNormal aos = new SearchNormal("K" + this.SearchNormals.Count, exp, exp, "exp", exp, 0, true);
            this.SearchNormals.Add(aos);
        }
        public void AddHidden(string refKey, string symbol, string val)
        {
            SearchNormal aos = new SearchNormal(refKey, refKey, refKey, symbol, val, 0, true);
            this.SearchNormals.Add(aos);
        }
        /// <summary>
        /// 加入查找属性.必须是外键盘/枚举类型/boolen.
        /// </summary>
        /// <param name="key">key</param>
        /// application/json
        /// requestbody 
        public void AddSearchAttr(string key, int width = 130)
        {
            Attr attr = this.GetAttrByKey(key);
            if (attr.Key == "FK_Dept")
                this.SearchFKEnums.Add(attr, false, null, width);
            else
                this.SearchFKEnums.Add(attr, true, null, width);
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
            this.SearchFKEnums.Add(attr, isShowSelectedAll, relationalDtlKey);
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
                throw new Exception("@[" + this.ToString() + "," + this.EnDesc + "," + this.PhysicsTable + "] 没有找到 key=[" + key + "]的属性，请检查Map文件。此问题出错的原因之一是，在设置系统中的一个实体的属性关联这个实体，你在给实体设置信息时没有按照规则书写reftext, refvalue。请核实。");
            else
            {
                throw new Exception("@[" + this.ToString() + "," + this.EnDesc + "," + this.PhysicsTable + "] 没有找到 key=[" + key + "]的属性，请检查Sys_MapAttr表是否有该数据,用SQL执行: SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.ToString() + "' AND KeyOfEn='" + key + "' 是否可以查询到数据，如果没有可能该字段属性丢失。");
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
                throw new Exception("@[" + this.ToString() + "," + this.EnDesc + "," + this.ToString() + "] 没有找到 key=[" + key + "]的属性，请检查Map文件。此问题出错的原因之一是，在设置系统中的一个实体的属性关联这个实体，你在给实体设置信息时没有按照规则书写reftext, refvalue。请核实。");
            else
                throw new Exception("@[" + this.ToString() + "," + this.EnDesc + "," + this.ToString() + "] 没有找到 key=[" + key + "]的属性，请检查Sys_MapAttr表是否有该数据,用SQL执行: SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + this.ToString() + "' AND KeyOfEn='" + key + "' 是否可以查询到数据，如果没有可能该字段属性丢失。");
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
                throw new Exception("@[" + this.ToString() + "," + this.EnDesc + "] 获取属性 desc  值不能为空.");

            throw new Exception("@[" + this.ToString() + "," + this.EnDesc + "] 没有找到 desc=[" + desc + "]的属性，请检查Map文件。此问题出错的原因之一是，在设置系统中的一个实体的属性关联这个实体，你在给实体设置信息时没有按照规则书写reftext, refvalue。请核实。");
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
        public BPEntityAthType HisBPEntityAthType = BPEntityAthType.None;
        /// <summary>
        /// 附件存储位置
        /// </summary>
        public string FJSavePath = null;
        /// <summary>
        /// 移动到显示方式
        /// </summary>
        public string TitleExt = null;
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
        /// 增加
        /// </summary>
        /// <param name="func"></param>
        public void AddRefMethod(RefMethod rm)
        {
            rm.GroupName = this.currGroupMethodName;
            this.HisRefMethods.Add(rm);
        }
        #endregion

        #region 关于他的明细信息
        /// <summary>
        /// 增加明细
        /// </summary>
        /// <param name="ens">子类</param>
        /// <param name="refKey">关联的键值</param>
        /// <param name="groupName">分组名字</param>
        /// <param name="model">模式</param>
        public void AddDtl(Entities ens, string refKey, string groupName = null, DtlEditerModel model = DtlEditerModel.DtlBatch, string icon = null)
        {
            EnDtl dtl = new EnDtl();
            dtl.Ens = ens;
            dtl.RefKey = refKey;
            dtl.GroupName = this.currGroupMethodName;
            dtl.DtlEditerModel = model;
            dtl.Icon = icon;
            this.Dtls.Add(dtl);
        }
        public void AddDtl(string  url, string refKey, string groupName = null, DtlEditerModel model = DtlEditerModel.DtlBatch, string icon = null,string desc="")
        {
            EnDtl dtl = new EnDtl();
            dtl.UrlExt = url;
            dtl.RefKey = refKey;
            dtl.GroupName = this.currGroupMethodName;
            dtl.Desc = desc;
            dtl.DtlEditerModel = model;
            dtl.Icon = icon;
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
            set
            {
                _RefMethods = value;
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
                this._AttrsOfOneVSM.GroupName = this.currGroupMethodName;
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
        public string MoveTo = null;
        /// <summary>
        /// 
        /// </summary>
        public string IndexField = null;
        /// <summary>
        /// 属性字段
        /// </summary>
        public string ParaFields = null;
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
        public void setEnDesc(string val)
        {
            this._EnDesc = val;
        }
        /// <summary>
        /// 是否版本管理
        /// </summary>
        public bool IsEnableVer = false;
        public bool IsShowSearchKey = true;
        /// <summary>
        /// 如果是null，就按照通用的查询关键字.
        /// 如果按照指定的格式查询按照如下格式配置.
        /// @名称=No@名称=Name@件号=JianHao
        /// </summary>
        public string SearchFields = "";
        /// <summary>
        /// 查询的数值 @年龄=Age@薪水=XinShui
        /// </summary>
        public string SearchFieldsOfNum = "";
        /// <summary>
        /// 数值查询范围.
        /// </summary>
        public BP.Sys.DTSearchWay DTSearchWay = BP.Sys.DTSearchWay.None;
        public string DTSearchLabel = "日期从";
        public string DTSearchKey = "";
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
        public void setEnType(EnType val)
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
                        this.setEnDesc(val);
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
                                this.setEnType(EnType.Admin);
                                break;
                            case "App":
                                this.setEnType(EnType.App);
                                break;
                            case "Dot2Dot":
                                this.setEnType(EnType.Dot2Dot);
                                break;
                            case "Dtl":
                                this.setEnType(EnType.Dtl);
                                break;
                            case "Etc":
                                this.setEnType(EnType.Etc);
                                break;
                            case "PowerAble":
                                this.setEnType(EnType.PowerAble);
                                break;
                            case "Sys":
                                this.setEnType(EnType.Sys);
                                break;
                            case "View":
                                this.setEnType(EnType.View);
                                break;
                            case "XML":
                                this.setEnType(EnType.XML);
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
        private void DealDT_Dot2Dot(DataTable dt)
        {
        }
        #endregion

        #region 与生成No字串有关
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
                this._IsAutoGenerNo = true;

            }
        }
        public void setCodeStruct(string val)
        {
            this._CodeStruct = val;
            this._IsAutoGenerNo = true;
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
        public void setIsAutoGenerNo(bool val)
        {
            _IsAutoGenerNo = val;
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
        /// <summary>
        /// 是否是视图
        /// </summary>
		public bool IsView
        {
            get
            {
                return DBAccess.IsView(this.PhysicsTableExt, this.EnDBUrl.DBType);
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
                _attrs = value;
                return;
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

            attr.UIIsReadonly = !isUIEnable;

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
            attr.UIBindKey = sysEnumKey;
            attr.UITag = cfgVal;
            attr.UIVisible = isUIVisable;
            attr.UIIsReadonly = !isUIEnable;
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
        /// <param name="cfgVals">配置的值,格式:@0=女@1=男</param>
        public void AddRadioBtnSysEnum(string key, string field, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVals = "")
        {
            if (field == null)
                field = key;

            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppInt;
            attr.MyFieldType = FieldType.Enum;
            attr.Desc = desc;
            attr.UIContralType = UIContralType.RadioBtn;
            attr.UIBindKey = sysEnumKey;
            attr.UIVisible = isUIVisable;
            attr.UIIsReadonly = !isUIEnable;
            attr.UITag = cfgVals; //设置的值.
            this.Attrs.Add(attr);
        }
        /// <summary>
        /// 自定义枚举类型
        /// </summary>
        /// <param name="key">键</param>		
        /// <param name="defaultVal">默认</param>
        /// <param name="desc">描述</param>
        /// <param name="sysEnumKey">Key</param>
        /// <param name="cfgVals">配置的值,格式:@0=女@1=男</param>
        public void AddRadioBtnSysEnum(string key, int defaultVal, string desc, bool isUIVisable, bool isUIEnable, string sysEnumKey, string cfgVals)
        {
            AddDDLSysEnum(key, key, defaultVal, desc, isUIVisable, isUIEnable, sysEnumKey, cfgVals, false);
        }
        #endregion


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
                attr.MyDataType = DataType.AppInt; ;
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
            this.Attrs.Add(attr);


            //他的名称列.
            attr = new Attr();
            attr.Key = key + "Text";
            attr.Field = key + "Text";
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppString;
            attr.MyFieldType = FieldType.Normal;
            attr.MaxLength = 200; //最大长度 @李国文
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
            attr.UIBindKey = ens.ToString();
            // attr.UIBindKeyOfEn = ens.GetNewEntity.ToString();

            attr.HisFKEns = ens;


            attr.HisFKEns = ens;
            attr.UIRefKeyText = refText;
            attr.UIRefKeyValue = refKey;
            attr.UIIsReadonly = uiIsEnable == true ? false : true;

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
        #endregion

        #endregion
        #endregion

        #region TB

        #region string 有关系的操作。

        #region 关于
        protected void AddTBString(string key, string field, object defaultVal, FieldType _FieldType, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine)
        {
            AddTBString(key, field, defaultVal, _FieldType, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, isUILine, null);
        }
        protected void AddTBString(string key, string field, object defaultVal, FieldType _FieldType, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine, string helpUrl)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.HelperUrl = helpUrl;

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
            attr.UIIsLine = isUILine;
            this.Attrs.Add(attr);
        }
        #endregion

        #region 公共的。
        /// <summary>
        /// 增加集合
        /// </summary>
        /// <param name="attrs">属性集合</param>
        /// <param name="isClearGroupName">是否清除分组名称</param>
        public void AddAttrs(Attrs attrs, bool isClearGroupName = false)
        {
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr)
                    continue;
                this.Attrs.Add(attr, isClearGroupName);
            }
        }
        public void AddAttr(Attr attr, bool isClearGroupName = false)
        {
            this.Attrs.Add(attr, isClearGroupName);
        }
        public void AddAttr(string key, object defaultVal, int dbtype, bool isPk, string desc)
        {
            if (isPk)
                AddTBStringPK(key, key, desc, true, false, 0, 1000, 100);
            else
                AddTBString(key, key, defaultVal.ToString(), FieldType.Normal, desc, true, false, 0, 1000, 100, false);
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
            AddTBString(key, key, defaultVal, FieldType.Normal, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
        }
        public void AddTBString(string key, string field, object defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
        {
            AddTBString(key, field, defaultVal, FieldType.Normal, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
        }
        public void AddTBString(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine)
        {
            AddTBString(key, key, defaultVal, FieldType.Normal, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, isUILine);
        }
        public void AddTBString(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith, bool isUILine, string helpUrl)
        {
            AddTBString(key, key, defaultVal, FieldType.Normal, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, isUILine, helpUrl);
        }
        /// <summary>
        /// 附件集合
        /// </summary>
        public void AddMyFileS()
        {
            this.AddTBInt(EntityNoNameAttr.MyFileNum, 0, "附件", false, false);
            this.HisBPEntityAthType = BPEntityAthType.Multi;
        }
        /// <summary>
        /// 附件集合
        /// </summary>
        /// <param name="desc"></param>
        public void AddMyFileS(string desc)
        {
            this.AddTBInt(EntityNoNameAttr.MyFileNum, 0, desc, false, false);
            this.HisBPEntityAthType = BPEntityAthType.Multi;
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

            this.AddTBString(EntityNoNameAttr.MyFileName, null, fileDesc, false, false, 0, 300, 200);
            this.AddTBString(EntityNoNameAttr.MyFilePath, null, "MyFilePath", false, false, 0, 300, 200);
            this.AddTBString(EntityNoNameAttr.MyFileExt, null, "MyFileExt", false, false, 0, 20, 10);
            this.AddTBString(EntityNoNameAttr.WebPath, null, "WebPath", false, false, 0, 300, 10);

            this.AddTBInt(EntityNoNameAttr.MyFileH, 0, "MyFileH", false, false);
            this.AddTBInt(EntityNoNameAttr.MyFileW, 0, "MyFileW", false, false);
            this.AddTBFloat("MyFileSize", 0, "MyFileSize", false, false);

            this.HisBPEntityAthType = BPEntityAthType.Single;
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
            this.HisBPEntityAthType = BPEntityAthType.Single;
            AddMyFile(fileDesc, fExt, null);
        }

        #region 字段分组方法.
        public string currGroupAttrName = "基本信息";
        public void AddGroupAttr(string groupName, string icon = "")
        {
            this.currGroupAttrName = groupName;
            this.Attrs.currGroupAttrName = groupName;
        }
        #endregion 字段分组方法.

        #region 方法分组.
        public string currGroupMethodName = "基本信息";
        public void AddGroupMethod(string groupName)
        {
            this.currGroupMethodName = groupName;
        }
        #endregion 方法分组.

        #region 属性.
        public void AddDDLStringEnum(string key, string defaultVal, string name, string cfgString, bool uiIsEnable, string helpDoc = "", bool isUILine = false)
        {
            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = key;

            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppString;

            // 设置外部数据源类型字段.
            attr.MyFieldType = FieldType.Normal;
            attr.UIContralType = UIContralType.DDL;
            attr.MaxLength = 50;
            attr.MinLength = 0;
            attr.Desc = name;
            //转化为sql.
            attr.UIBindKey = Glo.DealSQLStringEnumFormat(cfgString);
            // alert(attr.UIBindKey);

            attr.UIIsReadonly = !uiIsEnable;
            attr.HelperUrl = helpDoc;
            attr.UIIsLine = isUILine;
            this.Attrs.Add(attr);

            //他的名称列.
            Attr attr2 = new Attr();
            attr2.Key = key + 'T';
            attr2.Field = key + 'T';
            attr2.DefaultVal = defaultVal;
            attr2.MyDataType = DataType.AppString;
            attr2.MyFieldType = FieldType.Normal;
            attr2.MaxLength = 200;
            attr2.Desc = name;
            attr2.UIContralType = UIContralType.TB;
            attr2.HelperUrl = helpDoc;
            attr2.UIIsLine = !!isUILine;
            //	attr.UIBindKey = sql;
            attr2.UIIsReadonly = true;
            attr2.UIVisible = false;
            this.Attrs.Add(attr2);
        }
        #endregion 枚举属性.

        #region 增加大块文本输入
        public void AddTBStringDoc()
        {
            AddTBStringDoc("Doc", "Doc", null, "内容", true, false, 0, 4000, 10, true);
        }

        public void AddTBStringDoc(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, bool isUILine, int rows = 10)
        {
            AddTBStringDoc(key, key, defaultVal, desc, uiVisable, isReadonly, 0, 4000, rows, isUILine);
        }

        public void AddTBStringDoc(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            AddTBStringDoc(key, key, defaultVal, desc, uiVisable, isReadonly, 0, 4000, 300, false);
        }
        public void AddTBStringDoc(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int rows)
        {
            AddTBStringDoc(key, key, defaultVal, desc, uiVisable, isReadonly, minLength, maxLength, rows, false);
        }
        public void AddTBStringDoc(string key, string field, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int rows, bool isUILine, bool isRichText = false)
        {
            if (field == null)
                field = key;

            Attr attr = new Attr();
            attr.Key = key;
            attr.Field = field;
            attr.DefaultVal = defaultVal;
            attr.MyDataType = DataType.AppString;
            attr.Desc = desc;
            attr.UIVisible = uiVisable;
            attr.UIWidth = 300;
            attr.UIIsReadonly = isReadonly;
            attr.MaxLength = maxLength;  //决定是否是超级长度字段.
            attr.MinLength = minLength;
            attr.MyFieldType = FieldType.Normal;
            attr.UIHeight = rows;

            if (isRichText == true)
            {
                attr.IsSupperText = 1; //是富文本. 都要解析为上下结构.
                isUILine = true; //必须是上下结构.
            }
            else
            {
                attr.IsSupperText = 0; //不是富文本. 根据 isUILine 解析是否上下结构.
            }

            attr.UIIsLine = isUILine;

            this.Attrs.Add(attr);
        }
        #endregion

        #region  PK
        public void AddTBStringPK(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
        {
            this.PKs = key;
            AddTBString(key, key, defaultVal, FieldType.PK, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
        }
        public void AddTBStringPK(string key, string field, object defaultVal, string desc, bool uiVisable, bool isReadonly, int minLength, int maxLength, int tbWith)
        {
            this.PKs = key;
            AddTBString(key, field, defaultVal, FieldType.PK, desc, uiVisable, isReadonly, minLength, maxLength, tbWith, false);
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
            attr.UIVisible = uiVisable;
            attr.UIIsReadonly = isReadonly;
            attr.MaxLength = 50;
            attr.UIWidth = 100;
            this.Attrs.Add(attr);
        }
        public void AddTBDateTime(string key, string defaultVal, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBDateTime(key, key, defaultVal, desc, uiVisable, isReadonly);
        }
        public void AddTBDateTime(string key, string desc, bool uiVisable, bool isReadonly)
        {
            this.AddTBDateTime(key, key, DateTime.Now.ToString(DataType.SysDateTimeFormat), desc, uiVisable, isReadonly);
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
            attr.MyDataType = DataType.AppInt; ;
            attr.MyFieldType = FieldType.Normal;
            attr.Desc = desc;
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
            attr.MyDataType = DataType.AppInt; ;
            attr.MyFieldType = FieldType.PK;
            attr.Desc = desc;
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
        public void AddTBIntPKOID()
        {
            this.AddTBIntPKOID("OID", "OID");
        }
        /// <summary>
        /// 增加  AtParas字段.
        /// </summary>
        /// <param name="fieldLength"></param>
        public void AddTBAtParas(int fieldLength = 4000)
        {
            this.AddTBString(EntityNoNameAttr.AtPara, null, "AtPara", false, true, 0, fieldLength, 10);
        }
        /// <summary>
        /// 查询关键字：系统字段
        /// </summary>
        /// <param name="fieldLength"></param>
        public void AddTBSKeyWords(int fieldLength = 4000)
        {
            this.AddTBString(EntityNoNameAttr.SKeyWords, null, "查询关键字", false, true, 0, fieldLength, 10);
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
            //attr.MyDataType=DataType.AppString);
            //attr.MyFieldType = FieldType.PK;
            //attr.Desc = "MyPK";
            //attr.UITBShowType = TBType.TB;
            //attr.setUIVisible(false);
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
            attr.MyDataType = DataType.AppInt; ;
            attr.MyFieldType = FieldType.PK;
            attr.Desc = "AID";
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
            attr.MyDataType = DataType.AppMoney;
            attr.Desc = desc;
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
