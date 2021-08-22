using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Sys;
using System.Collections.Generic;
using BP.CCBill.Template;


namespace BP.CCBill
{
    /// <summary>
    /// 数据源实体 - Attr
    /// </summary>
    public class DBListAttr : FrmAttr
    {
        public const string MainTable = "MainTable";
        public const string MainTablePK = "MainTablePK";
    }
    /// <summary>
    /// 数据源实体
    /// </summary>
    public class DBList : EntityNoName
    {
        #region 权限控制.
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        #endregion 权限控制.

        #region 属性
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.PTable);
                if (DataType.IsNullOrEmpty(s) == true)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// 实体类型：@0=单据@1=编号名称实体@2=树结构实体
        /// </summary>
        public EntityType EntityType
        {
            get
            {
                return (EntityType)this.GetValIntByKey(DBListAttr.EntityType);
            }
            set
            {
                this.SetValByKey(DBListAttr.EntityType, (int)value);
            }
        }
        /// <summary>
        /// 表单类型 (0=傻瓜，2=自由 ...)
        /// </summary>
        public FrmType FrmType
        {
            get
            {
                return (FrmType)this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, (int)value);
            }
        }
        /// <summary>
        /// 表单树
        /// </summary>
        public string FK_FormTree
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FormTree);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FormTree, value);
            }
        }
        /// <summary>
        /// 新建模式 @0=表格模式@1=卡片模式@2=不可用
        /// </summary>
        public int BtnNewModel
        {
            get
            {
                return this.GetValIntByKey(DBListAttr.BtnNewModel);
            }
            set
            {
                this.SetValByKey(DBListAttr.BtnNewModel, value);
            }
        }
        /// <summary>
        /// 单据格式(流水号4)
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                string str = this.GetValStrByKey(DBListAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "{LSH4}";
                return str;
            }
            set
            {
                this.SetValByKey(DBListAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                string str = this.GetValStrByKey(DBListAttr.TitleRole);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "@WebUser.FK_DeptName @WebUser.Name @RDT";
                return str;
            }
            set
            {
                this.SetValByKey(DBListAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 新建标签
        /// </summary>
        public string BtnNewLable
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnNewLable);
            }
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        public string BtnDelLable
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnDelLable);
            }
        }
        /// <summary>
        /// 保存标签
        /// </summary>
        public string BtnSaveLable
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnSaveLable);
            }
        }
        /// <summary>
        /// 提交标签
        /// </summary>
        public string BtnSubmitLable
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnSubmitLable);
            }
        }
        /// <summary>
        /// 查询标签
        /// </summary>
        public string BtnSearchLabel
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnSearchLabel);
            }
        }
        /// <summary>
        /// 数据快照
        /// </summary>
        public string BtnDataVer
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnDataVer);
            }
        }
        /// <summary>
        /// 分组按钮
        /// </summary>
        public bool BtnGroupEnable
        {
            get
            {
                return this.GetValBooleanByKey(DBListAttr.BtnGroupEnable);
            }
        }
        public string BtnGroupLabel
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnGroupLabel);
            }
        }
        /// <summary>
        /// 打印HTML按钮
        /// </summary>
        public bool BtnPrintHtmlEnable
        {
            get
            {
                return this.GetValBooleanByKey(DBListAttr.BtnPrintHtmlEnable);
            }
        }
        public string BtnPrintHtml
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnPrintHtml);
            }
        }
        /// <summary>
        /// 打印PDF按钮
        /// </summary>
        public bool BtnPrintPDFEnable
        {
            get
            {
                return this.GetValBooleanByKey(DBListAttr.BtnPrintPDFEnable);
            }
        }
        public string BtnPrintPDF
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnPrintPDF);
            }
        }
        /// <summary>
        /// 打印RTF按钮
        /// </summary>
        public bool BtnPrintRTFEnable
        {
            get
            {
                return this.GetValBooleanByKey(DBListAttr.BtnPrintRTFEnable);
            }
        }
        public string BtnPrintRTF
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnPrintRTF);
            }
        }
        /// <summary>
        /// 打印CCWord按钮
        /// </summary>
        public bool BtnPrintCCWordEnable
        {
            get
            {
                return this.GetValBooleanByKey(DBListAttr.BtnPrintCCWordEnable);
            }
        }
        public string BtnPrintCCWord
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.BtnPrintCCWord);
            }
        }
        /// <summary>
        /// 数据源类型
        /// </summary>
        public int DBType
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.DBType);
            }
        }
        public string DBSrc
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.DBSrc);
            }
            set
            {
                this.SetValByKey(MapDataAttr.DBSrc, value);
            }
        }
        public string ExpEn
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.ExpEn);
            }
            set
            {
                this.SetValByKey(MapDataAttr.ExpEn, value);
            }
        }
        public string ExpList
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.ExpList);
            }
            set
            {
                this.SetValByKey(MapDataAttr.ExpList, value);
            }
        }
        public string ExpCount
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.ExpCount);
            }
            set
            {
                this.SetValByKey(MapDataAttr.ExpCount, value);
            }
        }
        public string MainTable
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.MainTable);
            }
            set
            {
                this.SetValByKey(DBListAttr.MainTable, value);
            }
        }
        public string MainTablePK
        {
            get
            {
                return this.GetValStrByKey(DBListAttr.MainTablePK);
            }
            set
            {
                this.SetValByKey(DBListAttr.MainTablePK, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 数据源实体
        /// </summary>
        public DBList()
        {
        }
        /// <summary>
        /// 数据源实体
        /// </summary>
        /// <param name="no">映射编号</param>
        public DBList(string no)
            : base(no)
        {
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
                Map map = new Map("Sys_MapData", "数据源实体");

                map.CodeStruct = "4";

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.SetHelperAlert(MapDataAttr.No, "也叫表单ID,系统唯一.");

                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单@8=开发者表单");
                //  map.AddTBString(MapDataAttr.PTable, null, "存储表", false, false, 0, 500, 20, true);
                // map.SetHelperAlert(MapDataAttr.PTable, "存储的表名,如果您修改一个不存在的系统将会自动创建一个表.");
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 200, 20, true);
                // map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), false);
                #endregion 基本属性.

                #region 数据源.
                map.AddTBInt(MapDataAttr.DBType, 0, "数据源类型", true, true);
                map.AddTBString(MapDataAttr.DBSrc, null, "数据源", false, false, 0, 600, 20);

                map.AddTBString(MapDataAttr.ExpEn, null, "实体数据源", false, false, 0, 600, 20, true);
                map.AddTBString(MapDataAttr.ExpList, null, "列表数据源", false, false, 0, 600, 20, true);

                map.AddTBString(DBListAttr.MainTable, null, "列表数据源主表", false, false, 0, 50, 20, false);
                map.AddTBString(DBListAttr.MainTablePK, null, "列表数据源主表主键", false, false, 0, 50, 20, false);
                map.AddTBString(MapDataAttr.ExpCount, null, "列表总数", false, false, 0, 600, 20, true);

                #endregion 数据源.

                #region 外观.
                map.AddDDLSysEnum(FrmAttr.RowOpenModel, 2, "行记录打开模式", true, true,
                  "RowOpenMode", "@0=新窗口打开@1=在本窗口打开@2=弹出窗口打开,关闭后不刷新列表@3=弹出窗口打开,关闭后刷新列表");
                string cfg = "@0=MyDictFrameWork.htm 实体与实体相关功能编辑器";
                cfg += "@1=MyDict.htm 实体编辑器";
                cfg += "@2=MyBill.htm 单据编辑器";
                cfg += "@9=自定义URL";
                map.AddDDLSysEnum("SearchDictOpenType", 0, "双击行打开内容", true, true, "SearchDictOpenType", cfg);
                map.AddTBString(EnCfgAttr.UrlExt, null, "要打开的Url", true, false, 0, 500, 60, true);
                map.AddTBInt(FrmAttr.PopHeight, 500, "弹窗高度", true, false);
                map.AddTBInt(FrmAttr.PopWidth, 760, "弹窗宽度", true, false);

                map.AddDDLSysEnum(MapDataAttr.TableCol, 0, "表单显示列数", true, true, "傻瓜表单显示方式",
                  "@0=4列@1=6列@2=上下模式3列");

                map.AddDDLSysEnum(FrmAttr.EntityEditModel, 0, "编辑模式", true, true, FrmAttr.EntityEditModel, "@0=表格@1=行编辑");
                map.SetHelperAlert(FrmAttr.EntityEditModel, "用什么方式打开实体列表进行编辑0=只读查询模式SearchDict.htm,1=行编辑模式SearchEditer.htm");
                #endregion 外观.

                #region 数据源实体.
                map.AddDDLSysEnum(DBListAttr.EntityType, 0, "业务类型", true, false, DBListAttr.EntityType,
                   "@0=独立表单@1=单据@2=编号名称实体@3=树结构实体");
                map.SetHelperAlert(DBListAttr.EntityType, "该实体的类型,@0=单据@1=编号名称实体@2=树结构实体.");

                map.AddTBString(DBListAttr.BillNoFormat, null, "实体编号规则", true, false, 0, 100, 20, true);
                map.SetHelperAlert(DBListAttr.BillNoFormat, "\t\n实体编号规则: \t\n 2标识:01,02,03等, 3标识:001,002,003,等..");
                map.AddTBString(FrmBillAttr.SortColumns, null, "排序字段", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.ColorSet, null, "颜色设置", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.FieldSet, null, "字段求和求平均设置", true, false, 0, 100, 20, true);

                //字段格式化函数.
                map.AddTBString("ForamtFunc", null, "字段格式化函数", true, false, 0, 200, 60, true);
                string msg = "对字段的显示使用函数进行处理";
                msg += "\t\n 1. 对于字段内容需要处理后在输出出来.";
                msg += "\t\n 2. 比如：原字段内容 @zhangsa,张三@lisi,李四 显示的内容为 张三,李四";
                msg += "\t\n 3. 配置格式: 字段名@函数名; 比如:  FlowEmps@DealFlowEmps; ";
                msg += "\t\n 4. 函数写入到 \\DataUser\\JSLibData\\SearchSelf.js";
                map.SetHelperAlert("ForamtFunc", msg);
                #endregion 数据源实体.
                //增加参数字段.
                map.AddTBAtParas(4000);

                #region 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "步骤1: 设置数据源."; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDBSrc";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "步骤2: 实体数据"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoExpEn";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "步骤3: 列表数据"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoExpList";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "步骤4: 测试"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDBList";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);



                rm = new RefMethod();
                rm.Title = "查询条件"; // "设计表单";
                                   //   rm.GroupName = "高级选项";
                rm.ClassMethodName = this.ToString() + ".DoSearch";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 基本功能.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoDBSrc()
        {
            return "../../Comm/RefFunc/EnOnly.htm?EnName=BP.CCBill.DBListDBSrc&No=" + this.No;
        }

        public string DoExpEn()
        {
            return "../../CCBill/Admin/DBList/FieldsORM.htm?s=34&FrmID=" + this.No + "&ExtType=PageLoadFull&RefNo=ss3";
        }
        public string DoExpList()
        {
            return "../../CCBill/Admin/DBList/ListDBSrc.htm?s=34&FrmID=" + this.No + "&ExtType=PageLoadFull&RefNo=ss3";
        }
        public string DoDBList()
        {
            return "../../CCBill/SearchDBList.htm?FrmID=" + this.No;
        }
        public string DoSearch()
        {
            return "../../CCBill/Admin/Collection/SearchCond.htm?s=34&FrmID=" + this.No + "&ExtType=PageLoadFull&RefNo=ss3";
        }

        protected override void afterInsertUpdateAction()
        {
            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 检查enittyNoName类型的实体
        /// </summary>
        public void CheckEnityTypeAttrsFor_EntityNoName()
        {
            //取出来全部的属性.
            MapAttrs attrs = new MapAttrs(this.No);

            #region 补充上流程字段到 NDxxxRpt.
            if (attrs.Contains(this.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.KeyOfEn = "OID";
                attr.Name = "主键ID";
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = EditType.Readonly;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.BillNo;
                attr.Name = "编号"; //  单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 名称 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.Title; // "FlowEmps";
                attr.Name = "名称"; //   单据模式， ccform的模式.
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 400;
                attr.Idx = -90;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_BillState") == false)
            {
                /* 单据状态 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "BillState"; // "FlowEmps";
                attr.Name = "单据状态"; //  
                attr.MyDataType = DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 10;
                attr.Idx = -98;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_Starter") == false)
            {
                /* 发起人 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "Starter";
                attr.Name = "创建人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_StarterName") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "StarterName";
                attr.Name = "创建人名称"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 参数 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.AtPara;
                attr.Name = "参数"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 4000;
                attr.Idx = -99;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_RDT") == false)
            {
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "RDT";
                attr.Name = "创建时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -97;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_FK_Dept") == false)
            {
                /* 创建人部门 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "FK_Dept";
                attr.Name = "创建人部门"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 100;
                attr.Idx = -1;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_OrgNo") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "OrgNo";
                attr.Name = "创建人所在的组织"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }
            #endregion 补充上流程字段。

        }
    }
    /// <summary>
    /// 数据源实体s
    /// </summary>
    public class DBLists : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 数据源实体s
        /// </summary>
        public DBLists()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DBList();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DBList> ToJavaList()
        {
            return (System.Collections.Generic.IList<DBList>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DBList> Tolist()
        {
            System.Collections.Generic.List<DBList> list = new System.Collections.Generic.List<DBList>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DBList)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
