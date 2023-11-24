using BP.DA;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.Sys;
using BP.CCBill.Template;

namespace BP.CCBill
{

    /// <summary>
    /// 实体表单 - Attr
    /// </summary>
    public class FrmBillAttr : FrmAttr
    {
        /// <summary>
        /// 单据关联的实体
        /// </summary>
        public const string RefDict = "RefDict";
    }
    /// <summary>
    /// 单据属性
    /// </summary>
    public class FrmBill : EntityNoName
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
                return (EntityType)this.GetValIntByKey(FrmBillAttr.EntityType);
            }
            set
            {
                this.SetValByKey(FrmBillAttr.EntityType, (int)value);
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
        public string FormTreeNo
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
        /// 单据格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                string str = this.GetValStrByKey(FrmBillAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "{LSH4}";
                return str;
            }
            set
            {
                this.SetValByKey(FrmBillAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                string str = this.GetValStrByKey(FrmBillAttr.TitleRole);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "@WebUser.FK_DeptName @WebUser.Name @RDT";
                return str;
            }
            set
            {
                this.SetValByKey(FrmBillAttr.BillNoFormat, value);
            }
        }

        public string SortColumns
        {
            get
            {
                return this.GetValStrByKey(FrmBillAttr.SortColumns);
            }
            set
            {
                this.SetValByKey(FrmBillAttr.SortColumns, value);
            }
        }

        public string FieldSet
        {
            get
            {
                return this.GetValStrByKey(FrmBillAttr.FieldSet);
            }
            set
            {
                this.SetValByKey(FrmBillAttr.FieldSet, value);
            }
        }

        public string RefDict
        {
            get
            {
                return this.GetValStrByKey(FrmBillAttr.RefDict);
            }
            set
            {
                this.SetValByKey(FrmBillAttr.RefDict, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 单据属性
        /// </summary>
        public FrmBill()
        {
        }
        /// <summary>
        /// 单据属性
        /// </summary>
        /// <param name="no">映射编号</param>
        public FrmBill(string no)
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

                Map map = new Map("Sys_MapData", "单据属性");


                #region 基本属性.
                map.AddGroupAttr("基本属性");
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单@8=开发者表单");
                //map.AddDDLSysEnum(MapDataAttr.FrmModel, 0, "单据模板", true, true, "BillFrmModel", "@0=系统预置@1=用户新增");
                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20, true);

                if (CCBPMRunModel.SAAS == BP.Difference.SystemConfig.CCBPMRunModel)
                {
                    string sql = "SELECT No,Name FROM WF_FlowSort WHERE OrgNo='" + BP.Web.WebUser.OrgNo + "' AND No!='" + BP.Web.WebUser.OrgNo + "'";
                    map.AddDDLSQL(MapDataAttr.FK_FormTree, null, "表单类别", sql, true);
                    //map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                }
                else
                {
                    map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                }

                map.AddDDLSysEnum(MapDataAttr.TableCol, 0, "表单显示列数", true, true, "傻瓜表单显示方式",
                 "@0=4列@1=6列@2=上下模式3列");

                map.AddDDLSysEnum(FrmAttr.RowOpenModel, 0, "行记录打开模式", true, true,
                "RowOpenMode", "@0=新窗口打开@1=在本窗口打开@2=弹出窗口打开,关闭后不刷新列表@3=弹出窗口打开,关闭后刷新列表");
                string cfg = "@0=MyDictFrameWork.htm 实体与实体相关功能编辑器";
                cfg += "@1=MyDict.htm 实体编辑器";
                cfg += "@2=MyBill.htm 单据编辑器";
                cfg += "@9=自定义URL";
                map.AddDDLSysEnum("SearchDictOpenType", 0, "双击行打开内容", true, true, "SearchDictOpenType", cfg);
                map.AddBoolean(EnCfgAttr.IsSelectMore, true, "是否下拉查询条件多选?", true, true);
                map.AddTBString(EnCfgAttr.UrlExt, null, "要打开的Url", true, false, 0, 500, 60, true);
                #endregion 基本属性.

                #region 单据属性.
                map.AddGroupAttr("单据属性");

                //map.AddDDLSysEnum(FrmBillAttr.FrmBillWorkModel, 0, "工作模式", true, false, FrmBillAttr.FrmBillWorkModel,
                //    "@0=独立表单@1=单据工作模式");

                map.AddDDLSysEnum(FrmBillAttr.EntityType, 0, "业务类型", true, false, FrmBillAttr.EntityType,
                   "@0=独立表单@1=单据@2=编号名称实体@3=树结构实体");
                map.SetHelperAlert(FrmBillAttr.EntityType, "该实体的类型,@0=单据@1=编号名称实体@2=树结构实体.");

                //map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "", "@0=独立表单@1=单据工作模式@2=流程工作模式");

                map.AddTBString(FrmBillAttr.BillNoFormat, null, "单号规则", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.TitleRole, null, "标题生成规则", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.SortColumns, null, "排序字段", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.ColorSet, null, "颜色设置", true, false, 0, 100, 20, true);
                string msg = "对字段的颜色处理";
                msg += "\t\n @Age:From=0,To=18,Color=green;From=19,To=30,Color=red";
                map.SetHelperAlert(FrmBillAttr.ColorSet, msg);

                map.AddTBString(FrmBillAttr.RowColorSet, null, "表格行颜色设置", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmBillAttr.RowColorSet, "按照指定字段存储的颜色设置表格行的背景色");

                map.AddTBString(FrmBillAttr.FieldSet, null, "字段求和求平均设置", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.RefDict, null, "单据关联的实体", false, true, 0, 190, 20, true);
                #endregion 单据属性.

                #region 按钮权限.
                //map.AddTBString(FrmBillAttr.BtnNewLable, "新建", "新建", true, false, 0, 50, 20);
                //map.AddDDLSysEnum(FrmDictAttr.BtnNewModel, 0, "新建模式", true, true, FrmDictAttr.BtnNewModel,
                //  "@0=表格模式@1=卡片模式@2=不可用", true);


                //map.AddTBString(FrmBillAttr.BtnSaveLable, "保存", "保存", true, false, 0, 50, 20);
                ////map.AddBoolean(FrmBillAttr.BtnSaveEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnSubmitLable, "提交", "提交", true, false, 0, 50, 20);

                //map.AddTBString(FrmBillAttr.BtnDelLable, "删除", "删除", true, false, 0, 50, 20);
                ////map.AddBoolean(FrmBillAttr.BtnDelEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnSearchLabel, "列表", "列表", true, false, 0, 50, 20);
                ////map.AddBoolean(FrmBillAttr.BtnSearchEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnGroupEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnPrintHtml, "打印Html", "打印Html", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnPrintHtmlEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnPrintPDF, "打印PDF", "打印PDF", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnPrintPDFEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnPrintRTF, "打印RTF", "打印RTF", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnPrintRTFEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnPrintCCWord, "打印CCWord", "打印CCWord", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnPrintCCWordEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmBillAttr.BtnExpZip, "导出zip文件", "导出zip文件", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnExpZipEnable, false, "是否可用？", true, true);


                map.AddTBString(FrmBillAttr.BtnRefBill, "关联单据", "关联单据", true, false, 0, 50, 20);

                map.AddDDLSysEnum(FrmAttr.RefBillRole, 0, "关联单据工作模式", true, true,
                    "RefBillRole", "@0=不启用@1=非必须选择关联单据@2=必须选择关联单据");

                map.AddTBString(FrmBillAttr.RefBill, null, "关联单据ID", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmBillAttr.RefBill, "请输入单据编号,多个单据编号用逗号分开.\t\n比如:Bill_Sale,Bill_QingJia");
                #endregion 按钮权限.

                #region 查询按钮权限.
                //map.AddTBString(FrmBillAttr.BtnImpExcel, "导入", "导入Excel文件", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnImpExcelEnable, true, "是否可用？", true, true);
                //map.AddTBString(FrmBillAttr.BtnExpExcel, "导出", "导出Excel文件", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnExpExcelEnable, true, "是否可用？", true, true);
                //map.AddTBString(FrmBillAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                //map.AddBoolean(FrmBillAttr.BtnGroupEnable, true, "是否可用？", true, true);
                #endregion 查询按钮权限.

                #region 设计者信息.
                map.AddGroupAttr("设计者信息");

                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapDataAttr.Note, null, "备注", true, false, true);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);

                map.AddTBString("HidenField", null, "隐藏查询条件", false, false, 0, 30, 20);
                #endregion 设计者信息.

                #region 扩展参数.
                map.AddTBString(FrmAttr.Tag0, null, "Tag0", false, false, 0, 500, 20);
                map.AddTBString(FrmAttr.Tag1, null, "Tag1", false, false, 0, 4000, 20);
                map.AddTBString(FrmAttr.Tag2, null, "Tag2", false, false, 0, 500, 20);
                #endregion 扩展参数.

                map.AddTBAtParas(800); //参数属性.

                #region 基本功能.

                map.AddGroupMethod("基本设置");

                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "按钮权限"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".ToolbarSetting";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设计表单"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDesigner";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "单据url的API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAPI";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                // map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "打开单据数据"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoOpenBill";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "绑定到菜单目录"; // "设计表单";
                rm.HisAttrs.AddDDLSQL("MENUNo", null, "选择菜单目录", "SELECT No,Name FROM GPM_Menu WHERE MenuType=3");
                rm.HisAttrs.AddTBString("Name", "@Name", "菜单名称", true, false, 0, 100, 100);
                rm.ClassMethodName = this.ToString() + ".DoBindMenu";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.Func;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "装载填充"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoPageLoadFull";
                rm.Icon = "../../WF/Img/FullData.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "表单事件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoEvent";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "执行方法"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoMethod";
                //rm.Icon = "../../WF/Img/Event.png";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);
                #endregion 基本功能.

                #region 权限规则.
                map.AddGroupMethod("权限规则");

                rm = new RefMethod();
                rm.Title = "创建规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCreateRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmBillAttr.BtnNewLable;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "保存规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSaveRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmBillAttr.BtnSaveLable;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "提交规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSubmitRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmBillAttr.BtnSubmitLable;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "删除规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDeleteRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmBillAttr.BtnDelLable;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查询权限"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearchRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmBillAttr.BtnSearchLabel;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据查询权限规则";
                rm.ClassMethodName = this.ToString() + ".DoSearchDataRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion

                #region 报表定义.
                map.AddGroupMethod("报表定义");
                rm = new RefMethod();
                rm.Title = "设置显示的列"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_ColsChose";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置多表头"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRptMTitle";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "列的顺序"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_ColsIdxAndLabel";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //   map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "查询条件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_SearchCond";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.GroupName = "报表定义";
                //rm.Title = "页面展示设置"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoRpt_Setting";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                //map.AddRefMethod(rm);

                #endregion 报表定义.

                this._enMap = map;
                return this._enMap;
            }
        }
        public void InsertToolbarBtns()
        {
            //表单的工具栏权限
            ToolbarBtn btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "New";
            btn.BtnLab = "新建";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.SetValByKey("Idx", 0);
            btn.Insert();


            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "Save";
            btn.BtnLab = "保存";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.SetValByKey("Idx", 1);
            btn.Insert();


            //单据增加提交的功能
            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "Submit";
            btn.BtnLab = "提交";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.SetValByKey("Idx", 1);
            btn.Insert();


            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "Delete";
            btn.BtnLab = "删除";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.SetValByKey("Idx", 2);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintHtml";
            btn.BtnLab = "打印Html";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.ItIsEnable = false;
            btn.SetValByKey("Idx", 3);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintPDF";
            btn.BtnLab = "打印PDF";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.ItIsEnable = false;
            btn.SetValByKey("Idx", 4);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintRTF";
            btn.BtnLab = "打印RTF";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.ItIsEnable = false;
            btn.SetValByKey("Idx", 5);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintCCWord";
            btn.BtnLab = "打印CCWord";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.ItIsEnable = false;
            btn.SetValByKey("Idx", 6);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "ExpZip";
            btn.BtnLab = "导出Zip包";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.ItIsEnable = false;
            btn.SetValByKey("Idx", 7);
            btn.Insert();

            //列表权限
            //查询
            Collection collection = new Collection();
            collection.FrmID = this.No;
            collection.MethodID = "Search";
            collection.Name = "查询";
            collection.MethodModel = "Search";
            collection.Mark = "Search";
            collection.No = collection.FrmID + "_" + collection.MethodID;
            collection.SetValByKey("Idx", 0);
            collection.Insert();


            //新建
            collection = new Collection();
            collection.FrmID = this.No;
            collection.MethodID = "New";
            collection.Name = "新建";
            collection.MethodModel = "New";
            collection.Mark = "New";
            collection.No = collection.FrmID + "_" + collection.MethodID;
            collection.SetValByKey("Idx", 1);
            collection.Insert();

            //删除
            collection = new Collection();
            collection.FrmID = this.No;
            collection.MethodID = "Delete";
            collection.Name = "删除";
            collection.MethodModel = "Delete";
            collection.Mark = "Delete";
            collection.No = collection.FrmID + "_" + collection.MethodID;
            collection.SetValByKey("Idx", 2);
            collection.Insert();

            collection = new Collection();
            collection.FrmID = this.No;
            collection.MethodID = "Group";
            collection.Name = "分析";
            collection.MethodModel = "Group";
            collection.Mark = "Group";
            collection.No = collection.FrmID + "_" + collection.MethodID;
            collection.SetValByKey("Idx", 3);
            collection.SetValByKey("IsEnable", false);
            collection.Insert();

            //导出
            collection = new Collection();
            collection.FrmID = this.No;
            collection.MethodID = "ExpExcel";
            collection.Name = "导出Excel";
            collection.MethodModel = "ExpExcel";
            collection.Mark = "ExpExcel";
            collection.No = collection.FrmID + "_" + collection.MethodID;
            collection.SetValByKey("Idx", 4);
            collection.Insert();

            //导入
            collection = new Collection();
            collection.FrmID = this.No;
            collection.MethodID = "ImpExcel";
            collection.Name = "导入Excel";
            collection.MethodModel = "ImpExcel";
            collection.Mark = "ImpExcel";
            collection.No = collection.FrmID + "_" + collection.MethodID;
            collection.SetValByKey("Idx", 5);
            collection.Insert();
        }

        protected override void afterInsert()
        {
            InsertToolbarBtns();
            CheckEnityTypeAttrsFor_Bill();

            base.afterInsertUpdateAction();
        }
        #endregion

        #region 权限控制.
        public string DoSaveRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSave";
        }
        /// <summary>
        /// 提交权限规则
        /// </summary>
        /// <returns></returns>
        public string DoSubmitRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSubmit";
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <returns></returns>
        public string DoCreateRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnNew";
        }
        /// <summary>
        /// 查询权限
        /// </summary>
        /// <returns></returns>
        public string DoSearchRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSearch";
        }
        /// <summary>
        /// 删除规则.
        /// </summary>
        /// <returns></returns>
        public string DoDeleteRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnDelete";
        }

        /// <summary>
        /// 数据查询权限规则
        /// </summary>
        /// <returns></returns>
        public string DoSearchDataRole()
        {
            return "../../CCBill/Admin/SearchDataRole.htm?s=34&FrmID=" + this.No;
        }
        #endregion 权限控制.

        #region 报表定义
        /// <summary>
        /// 选择显示的列
        /// </summary>
        /// <returns></returns>
        public string DoRpt_ColsChose()
        {
            return "../../CCBill/Admin/ColsChose.htm?FrmID=" + this.No;
        }
        /// <summary>
        /// 设置多表头
        /// </summary>
        /// <returns></returns>
        public string DoRptMTitle()
        {
            return "../../Comm/Sys/MultiTitle.htm?EnsName=" + this.No + "&DoType=Bill";
        }
        /// <summary>
        /// 列的顺序
        /// </summary>
        /// <returns></returns>
        public string DoRpt_ColsIdxAndLabel()
        {
            return "../../CCBill/Admin/ColsIdxAndLabel.htm?FrmID=" + this.No;
        }
        /// <summary>
        /// 查询条件
        /// </summary>
        /// <returns></returns>
        public string DoRpt_SearchCond()
        {
            return "../../CCBill/Admin/SearchCond.htm?FrmID=" + this.No;
        }

        public string DoRpt_Setting()
        {
            return "../Sys/SearchSetting.htm?EnsName=" + this.No + "&SettingType=1";
        }
        #endregion 报表定义.

        public string ToolbarSetting()
        {
            return "../../CCBill/Admin/ToolbarSetting.htm?s=34&FrmID=" + this.No;
        }
        public string DoPageLoadFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PageLoadFull.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
        }
        /// <summary>
        /// 表单事件
        /// </summary>
        /// <returns></returns>
        public string DoEvent()
        {
            return "../../Admin/CCFormDesigner/Action.htm?FK_MapData=" + this.No + "&T=sd&FK_Node=0";
        }

        /// <summary>
        /// 检查检查实体类型
        /// </summary>
        public void CheckEnityTypeAttrsFor_Bill()
        {
            //取出来全部的属性.
            MapAttrs attrs = new MapAttrs(this.No);

            //@hongyan. 为attr找到gfID.
            GroupFields gfs = new GroupFields();
            gfs.Retrieve("FrmID", this.No);
            if (gfs.Count == 0)
            {
                GroupField gf = new GroupField();
                gf.Lab = "基本信息";
                gf.FrmID = this.No;
                gf.Insert();
                gfs.AddEntity(gf);
            }
            int gfID = gfs[0].GetValIntByKey("OID");


            #region 补充上流程字段到 NDxxxRpt.
            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 标题 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.Title); // "FlowEmps";
                attr.setName("标题"); //   单据模式， ccform的模式.
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(400);
                attr.Idx = -100;
                attr.setGroupID(gfID); //@hongyan. 其他字段都有.
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.setKeyOfEn("OID");
                attr.setName("主键ID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.setEditType(EditType.Readonly);
                attr.setGroupID(gfID);
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.BillNo);

                attr.setName("单据编号"); //  单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.setGroupID(gfID);
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 参数 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.AtPara);
                attr.setName("参数"); // 单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(4000);
                attr.Idx = -99;
                attr.setGroupID(gfID);
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_BillState") == false)
            {
                /* 单据状态 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("BillState"); // "FlowEmps";
                attr.setName("单据状态"); //  
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(10);
                attr.Idx = -98;
                attr.setGroupID(gfID);
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_Starter") == false)
            {
                /* 发起人 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("Starter");
                attr.setName("制单人"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.setGroupID(gfID);
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_StarterName") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("StarterName");
                attr.setName("制单人名称"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(32);
                attr.Idx = -1;
                attr.setGroupID(gfID);
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_RDT") == false)
            {
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("RDT");
                attr.setName("制单日期");
                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.Idx = -97;
                attr.setGroupID(gfID);
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_FK_Dept") == false)
            {
                /* 创建人部门 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("FK_Dept");
                attr.setName("制单人部门"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.setGroupID(gfID);
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_OrgNo") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("OrgNo");
                attr.setName("制单人所在的组织"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.setGroupID(gfID);
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_PWorkID") == false)
            {
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("PWorkID");
                attr.setName("实体发起的单据"); //  
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(50);
                attr.Idx = -1;
                attr.setGroupID(gfID);
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_PFrmID") == false)
            {
                MapAttr attr = new MapAttr();
                attr.FrmID = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("PFrmID");
                attr.setName("实体名称"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(200);
                attr.Idx = -1;
                attr.setGroupID(gfID);
                attr.Insert();
            }
            #endregion 补充上流程字段。
        }
        /// <summary>
        /// 绑定菜单树
        /// </summary>
        /// <returns>返回执行结果.</returns>
        public string DoBindMenu(string menumDirNo, string menuName)
        {
            string sql = "SELECT FK_App FROM GPM_Menu WHERE No='" + menumDirNo + "'";
            string app = DBAccess.RunSQLReturnString(sql);

            string guid = DBAccess.GenerGUID();

            string url = "../WF/CCBill/Search.htm?FrmID=" + this.No;
            sql = "INSERT INTO GPM_Menu (No, Name, ParentNo, Idx, MenuType, FK_App, Url, OpenWay,Icon,MenuCtrlWay) VALUES ('" + guid + "', '" + menuName + "', '" + menumDirNo + "', 1, 4, '" + app + "', '" + url + "',  0,'',1)";
            DBAccess.RunSQL(sql);
            return "加入成功,如何<a href='En.htm?EnName=BP.GPM.Menu&No=" + guid + "'>控制权限请转GPM.</a>";
        }

        #region 业务逻辑.
        public string CreateBlankWorkID()
        {
            return BP.CCBill.Dev2Interface.CreateBlankBillID(this.No, BP.Web.WebUser.No, null).ToString();
        }
        #endregion 业务逻辑.

        #region 方法操作.
        /// <summary>
        /// 打开单据
        /// </summary>
        /// <returns></returns>
        public string DoOpenBill()
        {
            return "../../CCBill/SearchBill.htm?FrmID=" +
              this.No + "&t=" + BP.DA.DataType.CurrentDateTime;
        }
        public string DoAPI()
        {
            return "../../Admin/FoolFormDesigner/Bill/API.htm?FrmID=" +
              this.No + "&t=" + BP.DA.DataType.CurrentDateTime;
        }
        #endregion 方法操作.
    }
    /// <summary>
    /// 单据属性s
    /// </summary>
    public class FrmBills : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 单据属性s
        /// </summary>
        public FrmBills()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmBill();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmBill> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmBill>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmBill> Tolist()
        {
            System.Collections.Generic.List<FrmBill> list = new System.Collections.Generic.List<FrmBill>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmBill)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
