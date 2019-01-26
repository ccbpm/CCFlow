using System;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// Pop返回值类型
    /// </summary>
    public enum PopValFormat
    {
        /// <summary>
        /// 编号
        /// </summary>
        OnlyNo,
        /// <summary>
        /// 名称
        /// </summary>
        OnlyName,
        /// <summary>
        /// 编号与名称
        /// </summary>
        NoName
    }
    /// <summary>
    /// 选择模式
    /// </summary>
    public enum PopValSelectModel
    {
        /// <summary>
        /// 单选
        /// </summary>
        One,
        /// <summary>
        /// 多选
        /// </summary>
        More
    }
    /// <summary>
    /// PopVal - 工作方式
    /// </summary>
    public enum PopValWorkModel
    {
        /// <summary>
        /// 禁用
        /// </summary>
        None,
        /// <summary>
        /// 自定义URL
        /// </summary>
        SelfUrl,
        /// <summary>
        /// 表格模式
        /// </summary>
        TableOnly,
        /// <summary>
        /// 表格分页模式
        /// </summary>
        TablePage,
        /// <summary>
        /// 分组模式
        /// </summary>
        Group,
        /// <summary>
        /// 树展现模式
        /// </summary>
        Tree,
        /// <summary>
        /// 双实体树
        /// </summary>
        TreeDouble
    }
    /// <summary>
    /// 扩展
    /// </summary>
    public class MapExtAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// ExtType
        /// </summary>
        public const string ExtType = "ExtType";
        /// <summary>
        /// 插入表单的位置
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 高度
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// 是否可以自适应大小
        /// </summary>
        public const string IsAutoSize = "IsAutoSize";
        /// <summary>
        /// 设置的属性
        /// </summary>
        public const string AttrOfOper = "AttrOfOper";
        /// <summary>
        /// 激活的属性
        /// </summary>
        public const string AttrsOfActive = "AttrsOfActive";
        /// <summary>
        /// 执行方式
        /// </summary>
        public const string DoWay = "DoWay";
        /// <summary>
        /// Tag
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// Tag1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// Tag2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// Tag3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// tag4
        /// </summary>
        public const string Tag4 = "Tag4";
        /// <summary>
        /// tag5
        /// </summary>
        public const string Tag5 = "Tag5";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBType = "DBType";
        /// <summary>
        /// Doc
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// 参数
        /// </summary>
        public const string AtPara = "AtPara";
        /// <summary>
        /// 计算的优先级
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string FK_DBSrc = "FK_DBSrc";
        /// <summary>
        /// 排序
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 扩展
    /// </summary>
    public class MapExt : EntityMyPK
    {
        #region 关于 Pop at 参数
        /// <summary>
        /// 转化JSON
        /// </summary>
        /// <returns></returns>
        public string PopValToJson()
        {
            return BP.Tools.Json.ToJsonEntityModel(this.PopValToHashtable());
        }
        public Hashtable PopValToHashtable()
        {

            //创建一个ht, 然后把他转化成json返回出去。
            Hashtable ht = new Hashtable();

            switch (this.PopValWorkModel)
            {
                case PopValWorkModel.SelfUrl:
                    ht.Add("URL", this.PopValUrl);
                    break;
                case PopValWorkModel.TableOnly: 
                    ht.Add("EntitySQL", this.PopValEntitySQL);
                    break;
                case PopValWorkModel.TablePage:
                    ht.Add("PopValTablePageSQL", this.PopValTablePageSQL);
                    ht.Add("PopValTablePageSQLCount", this.PopValTablePageSQLCount);
                    break;
                case PopValWorkModel.Group:
                    ht.Add("GroupSQL", this.Tag1);
                    ht.Add("EntitySQL", this.PopValEntitySQL);
                    break;
                case PopValWorkModel.Tree:
                    ht.Add("TreeSQL", this.PopValTreeSQL);
                    ht.Add("TreeParentNo", this.PopValTreeParentNo);
                    break;
                case PopValWorkModel.TreeDouble:
                    ht.Add("DoubleTreeSQL", this.PopValTreeSQL);
                    ht.Add("DoubleTreeParentNo", this.PopValTreeParentNo);
                    ht.Add("DoubleTreeEntitySQL", this.PopValDoubleTreeEntitySQL);
                    break;
                default:
                    break;
            }

            ht.Add(MapExtAttr.W, this.W);
            ht.Add(MapExtAttr.H, this.H);

            ht.Add("PopValWorkModel", this.PopValWorkModel.ToString()); //工作模式.
            ht.Add("PopValSelectModel", this.PopValSelectModel.ToString()); //单选，多选.

            ht.Add("PopValFormat", this.PopValFormat.ToString()); //返回值格式.
            ht.Add("PopValTitle", this.PopValTitle); //窗口标题.
            ht.Add("PopValColNames", this.PopValColNames); //列名 @No=编号@Name=名称@Addr=地址.
            ht.Add("PopValSearchTip", this.PopValSearchTip); //搜索提示..

            //查询条件.
            ht.Add("PopValSearchCond", this.PopValSearchCond); //查询条件..


            //转化为Json.
            return ht;
        }
        /// <summary>
        /// 连接
        /// </summary>
        public string PopValUrl
        {
            get
            {
                return this.Doc;
            }
            set
            {
                this.Doc = value;
            }
        }
        /// <summary>
        /// 实体SQL
        /// </summary>
        public string PopValEntitySQL
        {
            get
            {
                return this.Tag2;
            }
            set
            {
                this.Tag2 = value;
            }
        }
        /// <summary>
        /// 分组SQL
        /// </summary>
        public string PopValGroupSQL
        {
            get
            {
                return this.Tag1;
            }
            set
            {
                this.Tag1 = value;
            }
        }
        /// <summary>
        /// 分页SQL带有关键字
        /// </summary>
        public string PopValTablePageSQL
        {
            get
            {
                return this.Tag;
            }
            set
            {
                this.Tag = value;
            }
        }
        /// <summary>
        /// 分页SQL获取总行数
        /// </summary>
        public string PopValTablePageSQLCount
        {
            get
            {
                return this.Tag1;
            }
            set
            {
                this.Tag1 = value;
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string PopValTitle
        {
            get
            {
                return this.GetParaString("PopValTitle");
            }
            set
            {
                this.SetPara("PopValTitle", value);
            }
        }

        public string PopValTreeSQL
        {
            get
            {
                return this.PopValEntitySQL;
            }
            set
            {
                  this.PopValEntitySQL=value;
            }
        }
        /// <summary>
        /// 根目录
        /// </summary>
        public string PopValTreeParentNo
        {
            get
            {
                return this.GetParaString("PopValTreeParentNo");
            }
            set
            {
                this.SetPara("PopValTreeParentNo", value);
            }
        }
        /// <summary>
        /// Pop 返回值的格式.
        /// </summary>
        public PopValFormat PopValFormat
        {
            get
            {
                return (PopValFormat)this.GetParaInt("PopValFormat");
            }
            set
            {
                this.SetPara("PopValFormat", (int)value);
            }
        }
        /// <summary>
        /// 双实体树的实体
        /// </summary>
        public string PopValDoubleTreeEntitySQL
        {
            get
            {
                return this.Tag1;
            }
            set
            {
                this.Tag1 = value;
            }
        }
        /// <summary>
        /// pop 选择方式
        /// 0,多选,1=单选.
        /// </summary>
        public PopValSelectModel PopValSelectModel
        {
            get
            {
                return (PopValSelectModel)this.GetParaInt("PopValSelectModel");
            }
            set
            {
                this.SetPara("PopValSelectModel", (int)value);
            }
        }
        /// <summary>
        /// PopVal工作模式
        /// </summary>
        public PopValWorkModel PopValWorkModel
        {
            get
            {
                return (PopValWorkModel)this.GetParaInt("PopValWorkModel");
            }
            set
            {
                this.SetPara("PopValWorkModel", (int)value);
            }
        }
        /// <summary>
        /// 开窗的列中文名称.
        /// </summary>
        public string PopValColNames
        {
            get
            {
              return  this.Tag3;
            }
            set
            {
                this.Tag3 = value;
            }
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        public string PopValSearchCond
        {
            get
            {
              return  this.Tag4;
            }
            set
            {
                this.Tag4 = value;
            }
        }
        /// <summary>
        /// 搜索提示关键字
        /// </summary>
        public string PopValSearchTip
        {
            get
            {
                return this.GetParaString("PopValSearchTip", "请输入关键字");
            }
            set
            {
                this.SetPara("PopValSearchTip", value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string FK_DBSrc
        {
            get
            {
                return this.GetValStrByKey(MapExtAttr.FK_DBSrc);
            }
            set
            {
                this.SetValByKey(MapExtAttr.FK_DBSrc, value);
            }
        }
        #endregion

        #region 属性
        public string ExtDesc
        {
            get
            {
                string dec = "";
                switch (this.ExtType)
                {
                    case MapExtXmlList.ActiveDDL:
                        dec += "字段" + this.AttrOfOper;
                        break;
                    case MapExtXmlList.TBFullCtrl:
                        dec += this.AttrOfOper;
                        break;
                    case MapExtXmlList.DDLFullCtrl:
                        dec += "" + this.AttrOfOper;
                        break;
                    case MapExtXmlList.InputCheck:
                        dec += "字段：" + this.AttrOfOper + " 检查内容：" + this.Tag1;
                        break;
                    case MapExtXmlList.PopVal:
                        dec += "字段：" + this.AttrOfOper + " Url：" + this.Tag;
                        break;
                    default:
                        break;
                }
                return dec;
            }
        }
        /// <summary>
        /// 是否自适应大小
        /// </summary>
        public bool IsAutoSize
        {
            get
            {
                return this.GetValBooleanByKey(MapExtAttr.IsAutoSize);
            }
            set
            {
                this.SetValByKey(MapExtAttr.IsAutoSize, value);
            }
        }
        /// <summary>
        /// 数据格式
        /// </summary>
        public string DBType
        {
            get
            {
                return this.GetValStrByKey(MapExtAttr.DBType);
            }
            set
            {
                this.SetValByKey(MapExtAttr.DBType, value);
            }
        }
        public string AtPara
        {
            get
            {
                return this.GetValStrByKey(MapExtAttr.AtPara);
            }
            set
            {
                this.SetValByKey(MapExtAttr.AtPara, value);
            }
        }
      
        public string ExtType
        {
            get
            {
                return this.GetValStrByKey(MapExtAttr.ExtType);
            }
            set
            {
                this.SetValByKey(MapExtAttr.ExtType, value);
            }
        }
        public int DoWay
        {
            get
            {
                return this.GetValIntByKey(MapExtAttr.DoWay);
            }
            set
            {
                this.SetValByKey(MapExtAttr.DoWay, value);
            }
        }
        /// <summary>
        /// 操作的attrs
        /// </summary>
        public string AttrOfOper
        {
            get
            {
                return this.GetValStrByKey(MapExtAttr.AttrOfOper);
            }
            set
            {
                this.SetValByKey(MapExtAttr.AttrOfOper, value);
            }
        }
        /// <summary>
        /// 激活的attrs
        /// </summary>
        public string AttrsOfActive
        {
            get
            {
              //  return this.GetValStrByKey(MapExtAttr.AttrsOfActive).Replace("~", "'");
                return this.GetValStrByKey(MapExtAttr.AttrsOfActive);
            }
            set
            {
                this.SetValByKey(MapExtAttr.AttrsOfActive, value);
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapExtAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapExtAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Doc
        /// </summary>
        public string Doc
        {
            get
            {
                string str=  this.GetValStrByKey("Doc").Replace("~","'");
                str = str.Replace("~", "'");
                return str;
            }
            set
            {
                string str = value.Replace("'", "~");
                this.SetValByKey("Doc", str);
            }
        }

       /// <summary>
       ///  处理自动填充SQL
       /// </summary>
       /// <param name="ht"></param>
       /// <returns></returns>
        public string AutoFullDLL_SQL_ForDtl(Hashtable htMainEn, Hashtable htDtlEn)
        {
            string fullSQL = this.Doc.Replace("@WebUser.No", WebUser.No);
            fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
            fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            fullSQL = fullSQL.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            if (fullSQL.Contains("@"))
            {
                foreach (string key in htDtlEn.Keys)
                {
                    if (fullSQL.Contains("@") == false)
                        break;
                    if (fullSQL.Contains("@" + key + ";") == true)
                    {
                        fullSQL = fullSQL.Replace("@" + key + ";", htDtlEn[key] as string);
                    }

                    if (fullSQL.Contains("@" + key) == true)
                    {
                        fullSQL = fullSQL.Replace("@" + key, htDtlEn[key] as string);
                    }
                }
            }

            if (fullSQL.Contains("@"))
            {
                foreach (string key in htMainEn.Keys)
                {
                    if (fullSQL.Contains("@") == false)
                        break;

                    if (fullSQL.Contains("@" + key + ";") == true)
                    {
                        fullSQL = fullSQL.Replace("@" + key + ";", htMainEn[key] as string);
                    }

                    if (fullSQL.Contains("@" + key) == true)
                    {
                        fullSQL = fullSQL.Replace("@" + key, htMainEn[key] as string);
                    }
                }
            }
            return fullSQL;
        }

        public string TagOfSQL_autoFullTB
        {
            get
            {
                if (DataType.IsNullOrEmpty(this.Tag))
                    return this.DocOfSQLDeal;

                string sql = this.Tag;
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_DeptNameOfFull", BP.Web.WebUser.FK_DeptNameOfFull);
                sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                return sql;
            }
        }

        public string DocOfSQLDeal
        {
            get
            {
                string sql = this.Doc;
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_DeptNameOfFull", BP.Web.WebUser.FK_DeptNameOfFull);
                sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                return sql;
            }
        }
        public string Tag
        {
            get
            {
                string s= this.GetValStrByKey("Tag").Replace("~", "'");

                s = s.Replace("\\\\", "\\");
                s = s.Replace("\\\\", "\\");

                s = s.Replace(@"CCFlow\Data\", @"CCFlow\WF\Data\");

                return s;
            }
            set
            {
                this.SetValByKey("Tag", value);
            }
        }
        public string Tag1
        {
            get
            {
                return this.GetValStrByKey("Tag1").Replace("~", "'");
            }
            set
            {
                this.SetValByKey("Tag1", value);
            }
        }
        public string Tag2
        {
            get
            {
                return this.GetValStrByKey("Tag2").Replace("~", "'");
            }
            set
            {
                this.SetValByKey("Tag2", value);
            }
        }
        public string Tag3
        {
            get
            {
                return this.GetValStrByKey("Tag3").Replace("~", "'");
            }
            set
            {
                this.SetValByKey("Tag3", value);
            }
        }
        public string Tag4
        {
            get
            {
                return this.GetValStrByKey("Tag4").Replace("~", "'");
            }
            set
            {
                this.SetValByKey("Tag4", value);
            }
        }
        public int H
        {
            get
            {
                return this.GetValIntByKey(MapExtAttr.H);
            }
            set
            {
                this.SetValByKey(MapExtAttr.H, value);
            }
        }
        public int W
        {
            get
            {
                return this.GetValIntByKey(MapExtAttr.W);
            }
            set
            {
                this.SetValByKey(MapExtAttr.W, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 扩展
        /// </summary>
        public MapExt()
        {
        }
        /// <summary>
        /// 扩展
        /// </summary>
        /// <param name="no"></param>
        public MapExt(string mypk)
        {
            this.MyPK = mypk;
            this.Retrieve();
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

                Map map = new Map("Sys_MapExt","业务逻辑");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();

                map.AddTBString(MapExtAttr.FK_MapData, null, "主表", true, false, 0, 100, 20);
                map.AddTBString(MapExtAttr.ExtType, null, "类型", true, false, 0, 30, 20);

                map.AddTBInt(MapExtAttr.DoWay, 0, "执行方式", true, false);

                map.AddTBString(MapExtAttr.AttrOfOper, null, "操作的Attr", true, false, 0, 30, 20);
                map.AddTBString(MapExtAttr.AttrsOfActive, null, "激活的字段", true, false, 0, 900, 20);

                map.AddTBStringDoc();

                map.AddTBString(MapExtAttr.Tag, null, "Tag", true, false, 0, 2000, 20);

                map.AddTBString(MapExtAttr.Tag1, null, "Tag1", true, false, 0, 2000, 20);
                map.AddTBString(MapExtAttr.Tag2, null, "Tag2", true, false, 0, 2000, 20);
                map.AddTBString(MapExtAttr.Tag3, null, "Tag3", true, false, 0, 2000, 20);
                map.AddTBString(MapExtAttr.Tag4, null, "Tag4", true, false, 0, 2000, 20);
                map.AddTBString(MapExtAttr.Tag5, null, "Tag5", true, false, 0, 2000, 20);


                map.AddTBInt(MapExtAttr.H, 500, "高度", false, false);
                map.AddTBInt(MapExtAttr.W, 400, "宽度", false, false);

                // 数据类型 @0=SQL@1=URLJSON@2=FunctionJSON.
                map.AddTBInt(MapExtAttr.DBType, 0, "数据类型", true, false);
                map.AddTBString(MapExtAttr.FK_DBSrc, null, "数据源", true, false, 0, 100, 20);

                // add by stone 2013-12-21 计算的优先级,用于js的计算. 
                // 也可以用于 字段之间的计算 优先级.
                map.AddTBInt(MapExtAttr.PRI, 0, "PRI/顺序号", false, false);
                map.AddTBString(MapExtAttr.AtPara, null, "参数", true, false, 0, 3999, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 其他方法.
        /// <summary>
        /// 统一生成主键的规则.
        /// </summary>
        public void InitPK()
        {
            switch (this.ExtType)
            {
                case MapExtXmlList.ActiveDDL:
                    this.MyPK = MapExtXmlList.ActiveDDL + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.DDLFullCtrl:
                    this.MyPK = MapExtXmlList.DDLFullCtrl + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.PopVal:
                    this.MyPK = MapExtXmlList.PopVal + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.TBFullCtrl:
                    this.MyPK = MapExtXmlList.TBFullCtrl + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.PopFullCtrl:
                    this.MyPK = MapExtXmlList.PopFullCtrl + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.AutoFull:
                    this.MyPK = MapExtXmlList.AutoFull + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.AutoFullDLL:
                    this.MyPK = MapExtXmlList.AutoFullDLL + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.InputCheck:
                    this.MyPK = MapExtXmlList.InputCheck + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                case MapExtXmlList.PageLoadFull:
                    this.MyPK = MapExtXmlList.PageLoadFull + "_" + this.FK_MapData;
                    break;
                case MapExtXmlList.RegularExpression:
                    this.MyPK = MapExtXmlList.RegularExpression + "_" + this.FK_MapData + "_" + this.AttrOfOper + "_" + this.Tag;
                    break;
                case MapExtXmlList.BindFunction:
                    this.MyPK = MapExtXmlList.BindFunction + "_" + this.FK_MapData + "_" + this.AttrOfOper + "_" + this.Tag;
                    break;
                case MapExtXmlList.Link:
                    this.MyPK = MapExtXmlList.Link + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
                default:
                    //这里要去掉，不然组合组主键，会带来错误.
                    if (DataType.IsNullOrEmpty(this.AttrOfOper) == true)
                        this.MyPK = this.ExtType + "_" + this.FK_MapData;
                    else
                        this.MyPK = this.ExtType + "_" + this.FK_MapData + "_" + this.AttrOfOper;
                    break;
            }
        }

        protected override bool beforeInsert()
        {
            if (this.MyPK == "")
                this.MyPK = DBAccess.GenerGUID(); //@李国文

            return base.beforeInsert();
        }

        protected override bool beforeUpdate()
        {
            this.InitPK();

            switch (this.ExtType)
            {
                case MapExtXmlList.ActiveDDL:
                case MapExtXmlList.DDLFullCtrl:
                case MapExtXmlList.TBFullCtrl:
                    if (this.Doc.Contains("@Key") == false)
                        throw new Exception("@SQL表达式错误，您必须包含@Key ,这个关键字. ");
                    break;
                case MapExtXmlList.AutoFullDLL:
                    if (this.Doc.Length <= 13)
                        throw new Exception("@必须填写SQL表达式. ");
                    break;
                case MapExtXmlList.AutoFull:
                    if (this.Doc.Length <= 3)
                        throw new Exception("@必须填写表达式. 比如 @单价;*@数量; ");
                    break;
                case MapExtXmlList.PopVal:
                    break;
                default:
                    break;
            }

            return base.beforeUpdate();
        }
        #endregion 

        /// <summary>
        /// 删除垃圾数据.
        /// </summary>
        public static void DeleteDB()
        {
            MapExts exts = new MapExts();
            exts.RetrieveAll();
            return;
           
            foreach (MapExt ext in exts)
            {
                if (ext.ExtType == MapExtXmlList.ActiveDDL)
                {
                    if (ext.AttrOfOper.Trim().Length == 0)
                    {
                        ext.Delete();
                        continue;
                    }

                    MapAttr attr = new MapAttr();
                    attr.MyPK = ext.AttrOfOper;
                    if (attr.IsExits == true)
                    {
                        ext.AttrOfOper = attr.KeyOfEn;
                        ext.Delete();

                        ext.MyPK = ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive;
                        ext.Save();
                    }

                    if (ext.MyPK == ext.ExtType + "_" + ext.FK_MapData + "_" + ext.FK_MapData + "_" + ext.AttrOfOper)
                    {
                        ext.Delete(); //直接删除.

                        ext.MyPK = ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive;
                        ext.Save();
                        continue;
                    }

                    if (ext.MyPK == ext.ExtType + "_" + ext.FK_MapData + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive)
                    {
                        ext.Delete(); //直接删除.
                        ext.MyPK = ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive;
                        ext.Save();
                        continue;
                    }

                    if (ext.MyPK == ext.ExtType + "_" + ext.FK_MapData + "_" + ext.FK_MapData + "_" + ext.AttrsOfActive + "_" + ext.AttrOfOper)
                    {
                        ext.Delete(); //直接删除.
                        ext.MyPK = ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive;
                        ext.Save();
                        continue;
                    }


                    //三个主键的情况.
                    if (ext.MyPK == ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper )
                    {
                        ext.Delete();
                        ext.MyPK = ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive;
                        ext.Save();
                        continue;
                    }

                    //三个主键的情况.
                    if (ext.MyPK == ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrsOfActive)
                    {
                        ext.Delete();
                        ext.MyPK = ext.ExtType + "_" + ext.FK_MapData + "_" + ext.AttrOfOper + "_" + ext.AttrsOfActive;
                        ext.Save();
                        continue;
                    }

                }
            }
        }
    }
    /// <summary>
    /// 扩展s
    /// </summary>
    public class MapExts : Entities
    {
        #region 构造
        /// <summary>
        /// 扩展s
        /// </summary>
        public MapExts()
        {
        }
        /// <summary>
        /// 扩展s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public MapExts(string fk_mapdata)
        {
            this.Retrieve(MapExtAttr.FK_MapData, fk_mapdata, MapExtAttr.PRI);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapExt> Tolist()
        {
            System.Collections.Generic.List<MapExt> list = new System.Collections.Generic.List<MapExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
