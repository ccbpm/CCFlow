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
    public class FrmDictAttr : FrmAttr
    {

    }
    /// <summary>
    /// 实体表单
    /// </summary>
    public class FrmDict : EntityNoName
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
                return (EntityType)this.GetValIntByKey(FrmDictAttr.EntityType);
            }
            set
            {
                this.SetValByKey(FrmDictAttr.EntityType, (int)value);
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
                return this.GetValIntByKey(FrmDictAttr.BtnNewModel);
            }
            set
            {
                this.SetValByKey(FrmDictAttr.BtnNewModel, value);
            }
        }
        /// <summary>
        /// 单据格式(流水号4)
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                string str = this.GetValStrByKey(FrmDictAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "{LSH4}";
                return str;
            }
            set
            {
                this.SetValByKey(FrmDictAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                string str = this.GetValStrByKey(FrmDictAttr.TitleRole);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "@WebUser.FK_DeptName @WebUser.Name @RDT";
                return str;
            }
            set
            {
                this.SetValByKey(FrmDictAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 新建标签
        /// </summary>
        public string BtnNewLable
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnNewLable);
            }
        }
        /// <summary>
        /// 删除标签
        /// </summary>
        public string BtnDelLable
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnDelLable);
            }
        }
        /// <summary>
        /// 保存标签
        /// </summary>
        public string BtnSaveLable
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnSaveLable);
            }
        }
        /// <summary>
        /// 提交标签
        /// </summary>
        public string BtnSubmitLable
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnSubmitLable);
            }
        }
        /// <summary>
        /// 查询标签
        /// </summary>
        public string BtnSearchLabel
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnSearchLabel);
            }
        }
        /// <summary>
        /// 数据快照
        /// </summary>
        public string BtnDataVer
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnDataVer);
            }
        }
        /// <summary>
        /// 分组按钮
        /// </summary>
        public bool BtnGroupEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmDictAttr.BtnGroupEnable);
            }
        }
        public string BtnGroupLabel
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnGroupLabel);
            }
        }
        /// <summary>
        /// 打印HTML按钮
        /// </summary>
        public bool BtnPrintHtmlEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmDictAttr.BtnPrintHtmlEnable);
            }
        }
        public string BtnPrintHtml
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnPrintHtml);
            }
        }
        /// <summary>
        /// 打印PDF按钮
        /// </summary>
        public bool BtnPrintPDFEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmDictAttr.BtnPrintPDFEnable);
            }
        }
        public string BtnPrintPDF
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnPrintPDF);
            }
        }
        /// <summary>
        /// 打印RTF按钮
        /// </summary>
        public bool BtnPrintRTFEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmDictAttr.BtnPrintRTFEnable);
            }
        }
        public string BtnPrintRTF
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnPrintRTF);
            }
        }
        /// <summary>
        /// 打印CCWord按钮
        /// </summary>
        public bool BtnPrintCCWordEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmDictAttr.BtnPrintCCWordEnable);
            }
        }
        public string BtnPrintCCWord
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnPrintCCWord);
            }
        }
        /// <summary>
        /// 打印ZIP按钮
        /// </summary>
        public bool BtnExpZipEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmDictAttr.BtnExpZipEnable);
            }
        }
        public string BtnExpZip
        {
            get
            {
                return this.GetValStrByKey(FrmDictAttr.BtnExpZip);
            }
        }

        #endregion

        #region 构造方法
        /// <summary>
        /// 实体表单
        /// </summary>
        public FrmDict()
        {
        }
        /// <summary>
        /// 实体表单
        /// </summary>
        /// <param name="no">映射编号</param>
        public FrmDict(string no)
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
                Map map = new Map("Sys_MapData", "实体表单");
                map.CodeStruct = "4";

                #region 基本属性.
                map.AddGroupAttr("基本信息");
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.SetHelperAlert(MapDataAttr.No, "也叫表单ID,系统唯一.");

                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单@8=开发者表单");
                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapDataAttr.PTable, "存储的表名,如果您修改一个不存在的系统将会自动创建一个表.");

                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 200, 20, true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), false);
                #endregion 基本属性.

                #region 外观.
                map.AddGroupAttr("外观");
                map.AddDDLSysEnum(FrmAttr.RowOpenModel, 2, "行记录打开模式", true, true,
                  "RowOpenMode", "@0=新窗口打开@1=在本窗口打开@2=弹出窗口打开,关闭后不刷新列表@3=弹出窗口打开,关闭后刷新列表");
                string cfg = "@0=MyDictFrameWork.htm 实体与实体相关功能编辑器";
                cfg += "@1=MyDict.htm 实体编辑器";
                cfg += "@2=MyBill.htm 单据编辑器";
                cfg += "@9=自定义URL";
                map.AddDDLSysEnum("SearchDictOpenType", 0, "双击行打开内容", true, true, "SearchDictOpenType", cfg);
                map.AddBoolean(EnCfgAttr.IsSelectMore, true, "是否下拉查询条件多选?", true, true);
                map.AddTBString(EnCfgAttr.UrlExt, null, "要打开的Url", true, false, 0, 500, 60, true);
                map.AddTBInt(FrmAttr.PopHeight, 500, "弹窗高度", true, false);
                map.AddTBInt(FrmAttr.PopWidth, 760, "弹窗宽度", true, false);

                map.AddDDLSysEnum(MapDataAttr.TableCol, 0, "表单显示列数", true, true, "傻瓜表单显示方式",
                  "@0=4列@1=6列@2=上下模式3列");

                map.AddDDLSysEnum(FrmAttr.EntityEditModel, 0, "编辑模式", true, true, FrmAttr.EntityEditModel, "@0=表格@1=行编辑");
                map.SetHelperAlert(FrmAttr.EntityEditModel, "用什么方式打开实体列表进行编辑0=只读查询模式SearchDict.htm,1=行编辑模式SearchEditer.htm");
                #endregion 外观.

                #region 实体表单.
                map.AddGroupAttr("实体表单");

                map.AddDDLSysEnum(FrmDictAttr.EntityType, 0, "业务类型", true, false, FrmDictAttr.EntityType,
                   "@0=独立表单@1=单据@2=编号名称实体@3=树结构实体");
                map.SetHelperAlert(FrmDictAttr.EntityType, "该实体的类型,@0=单据@1=编号名称实体@2=树结构实体.");

                map.AddTBString(FrmDictAttr.BillNoFormat, null, "实体编号规则", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmDictAttr.BillNoFormat, "\t\n实体编号规则: \t\n 2标识:01,02,03等, 3标识:001,002,003,等..");
                map.AddTBString(FrmBillAttr.SortColumns, null, "排序字段", true, false, 0, 100, 20, true);
                map.AddTBString(FrmBillAttr.ColorSet, null, "表格列颜色设置", true, false, 0, 100, 20, true);

                string msg = "对字段的颜色处理";
                msg += "\t\n @Age:From=0,To=18,Color=green;From=19,To=30,Color=red";
                map.SetHelperAlert(FrmBillAttr.ColorSet, msg);

                map.AddTBString(FrmBillAttr.RowColorSet, null, "表格行颜色设置", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmBillAttr.RowColorSet, "按照指定字段存储的颜色设置表格行的背景色");

                map.AddTBString(FrmBillAttr.FieldSet, null, "字段求和求平均设置", true, false, 0, 100, 20, true);
                //字段格式化函数.
                map.AddTBString("ForamtFunc", null, "字段格式化函数", true, false, 0, 200, 60, true);
                  msg = "对字段的显示使用函数进行处理";
                msg += "\t\n 1. 对于字段内容需要处理后在输出出来.";
                msg += "\t\n 2. 比如：原字段内容 @zhangsa,张三@lisi,李四 显示的内容为 张三,李四";
                msg += "\t\n 3. 配置格式: 字段名@函数名; 比如:  FlowEmps@DealFlowEmps; ";
                msg += "\t\n 4. 函数写入到 /DataUser/JSLibData/SearchSelf.js";
                map.SetHelperAlert("ForamtFunc", msg);
                #endregion 实体表单.

                //#region MyBill - 按钮权限.
                //map.AddTBString(FrmDictAttr.BtnNewLable, "新建", "新建", true, false, 0, 50, 20);
                //map.AddDDLSysEnum(FrmDictAttr.BtnNewModel, 0, "新建模式", true, true, FrmDictAttr.BtnNewModel,
                //  "@0=表格模式@1=卡片模式@2=不可用", true);

                //map.AddTBString(FrmDictAttr.BtnSaveLable, "保存", "保存", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnSaveEnable, true, "是否可用？", true, true);

                //  map.AddTBString(FrmDictAttr.BtnSubmitLable, "提交", "提交", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnSubmitEnable, true, "是否可用？", true, true);

                //删除.
                //map.AddTBString(FrmDictAttr.BtnDelLable, "删除", "删除", true, false, 0, 50, 20);

                //数据版本.
                // map.AddTBString(FrmDictAttr.BtnDataVer, "数据快照", "数据快照", true, false, 0, 50, 20);

                //map.AddBoolean(FrmDictAttr.BtnDelEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnSearchLabel, "列表", "列表", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnSearchEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnGroupEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnPrintHtml, "打印Html", "打印Html", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnPrintHtmlEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnPrintPDF, "打印PDF", "打印PDF", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnPrintPDFEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnPrintRTF, "打印RTF", "打印RTF", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnPrintRTFEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnPrintCCWord, "打印CCWord", "打印CCWord", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnPrintCCWordEnable, false, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnExpZip, "导出zip文件", "导出zip文件", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnExpZipEnable, false, "是否可用？", true, true);
                //#endregion 按钮权限.

                // #region 查询按钮权限.
                // map.AddTBString(FrmDictAttr.BtnImpExcel, "导入", "导入Excel文件", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnImpExcelEnable, true, "是否可用？", true, true);

                //map.AddTBString(FrmDictAttr.BtnExpExcel, "导出", "导出Excel文件", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnExpExcelEnable, true, "是否可用？", true, true);

                // map.AddTBString(FrmDictAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                //map.AddBoolean(FrmDictAttr.BtnGroupEnable, true, "是否可用？", true, true);
                //#endregion 查询按钮权限.

                #region 设计者信息.
                map.AddGroupAttr("设计者信息");

                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapDataAttr.Note, null, "备注", true, false, true);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                #region 扩展参数.
                map.AddTBAtParas(3000); //参数属性.
                map.AddTBString(FrmDictAttr.Tag0, null, "Tag0", false, false, 0, 500, 20);
                map.AddTBString(FrmDictAttr.Tag1, null, "Tag1", false, false, 0, 4000, 20);
                map.AddTBString(FrmDictAttr.Tag2, null, "Tag2", false, false, 0, 500, 20);
                #endregion 扩展参数.

                #region 基本功能.

                map.AddGroupMethod("基本功能");

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
                rm.Title = "表单事件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoEvent";
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
                //  map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "单据url的API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAPI";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
               // map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "打开数据(表格)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoOpenBillDict";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "打开数据(行编辑)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoOpenBillEditer";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "绑定到菜单目录"; // "设计表单";
                //rm.HisAttrs.AddDDLSQL("MENUNo", null, "选择菜单目录", "SELECT No,Name FROM GPM_Menu WHERE MenuType=3");
                //rm.HisAttrs.AddTBString("Name", "@Name", "菜单名称", true, false, 0, 100, 100);
                //rm.ClassMethodName = this.ToString() + ".DoBindMenu";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.Func;
                //rm.Target = "_blank";
                ////rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);


                #endregion 基本功能.

                #region 查询定义.
                map.AddGroupMethod("查询定义");
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
                #endregion 报表定义.

                #region 权限规则.
                map.AddGroupMethod("权限规则");
                rm = new RefMethod();
                rm.Title = "创建规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCreateRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnNewLable;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "保存规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSaveRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnSaveLable;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "提交规则"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoSubmitRole";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.LinkModel;
                //rm.RefAttrKey = FrmDictAttr.BtnSubmitLable;
                //rm.GroupName = "权限规则";
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "删除规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDeleteRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnDelLable;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据版本权限规则"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoBtnDataVer";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnDataVer;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "查询权限"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoSearchRole";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = FrmDictAttr.BtnSearchLabel;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "数据查询权限规则";
                rm.ClassMethodName = this.ToString() + ".DoSearchDataRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 执行模板复制
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="frmName"></param>
        /// <param name="ptable"></param>
        /// <returns></returns>
        public string DoCopyTemplate(string newFrmID, string frmName, string ptable)
        {
            bool isSetReadonly = true;
            bool isCopyMethod = false;

            MapData md = new MapData(this.No);
            md.DoCopy(newFrmID, frmName);

            MapData mdTo = new MapData(newFrmID);
            if (ptable == null)
                mdTo.PTable = ptable;
            mdTo.Name = frmName;
            mdTo.Update();

            #region 如果要设置为只读的.
            if (isSetReadonly == true)
            {
                MapAttrs mattrs = new MapAttrs();
                mattrs.Retrieve(MapAttrAttr.FK_MapData, newFrmID);

                foreach (MapAttr item in mattrs)
                {
                    if (item.UIIsEnable == false)
                        continue;

                    item.setUIIsEnable(false);
                    item.Update();
                }
            }
            #endregion 如果要设置为只读的.


            return "执行成功.";
        }

        public void InsertToolbarBtns()
        {
            //表单的工具栏权限
            ToolbarBtn btn = new ToolbarBtn();
            if(this.EntityType == EntityType.FrmDict)
            {
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


                btn = new ToolbarBtn();
                btn.FrmID = this.No;
                btn.BtnID = "Delete";
                btn.BtnLab = "删除";
                btn.MyPK = btn.FrmID + "_" + btn.BtnID;
                btn.SetValByKey("Idx", 2);
                btn.Insert();
            }
            

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintHtml";
            btn.BtnLab = "打印Html";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.IsEnable = false;
            btn.SetValByKey("Idx", 3);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintPDF";
            btn.BtnLab = "打印PDF";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.IsEnable = false;
            btn.SetValByKey("Idx", 4);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintRTF";
            btn.BtnLab = "打印RTF";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.IsEnable = false;
            btn.SetValByKey("Idx", 5);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "PrintCCWord";
            btn.BtnLab = "打印CCWord";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.IsEnable = false;
            btn.SetValByKey("Idx", 6);
            btn.Insert();

            btn = new ToolbarBtn();
            btn.FrmID = this.No;
            btn.BtnID = "ExpZip";
            btn.BtnLab = "导出Zip包";
            btn.MyPK = btn.FrmID + "_" + btn.BtnID;
            btn.IsEnable = false;
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

            if (this.EntityType == EntityType.FrmDict)
            {
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

            

        }

        
        protected override void afterInsert()
        {
            InsertToolbarBtns();
            CheckEnityTypeAttrsFor_EntityNoName();

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
                attr.setFK_MapData(this.No);
                attr.setKeyOfEn("OID");
                attr.setName("主键ID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.DefVal = "0";
                attr.setEditType(EditType.Readonly);
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.BillNo);
                attr.setName("编号"); //  单据编号
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 名称 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn(GERptAttr.Title); // "FlowEmps";
                attr.setName("名称"); //   单据模式， ccform的模式.
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(true);
                attr.setUIIsEnable(true);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(400);
                attr.Idx = -90;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_BillState") == false)
            {
                /* 单据状态 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("BillState"); // "FlowEmps";
                attr.setName("单据状态"); //  
                attr.setMyDataType(DataType.AppInt);
                attr.setUIContralType(UIContralType.TB);
                attr.setUIContralType(UIContralType.TB);

                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = true;
                attr.setMinLen(0);
                attr.setMaxLen(10);
                attr.Idx = -98;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_Starter") == false)
            {
                /* 发起人 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("Starter");
                attr.setName("创建人"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_StarterName") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("StarterName");
                attr.setName("创建人名称"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(32);
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 参数 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
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
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_RDT") == false)
            {
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("RDT");
                attr.setName("创建时间");
                attr.setMyDataType(DataType.AppDateTime);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);
                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.UIIsLine = false;
                attr.Idx = -97;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_FK_Dept") == false)
            {
                /* 创建人部门 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("FK_Dept");
                attr.setName("创建人部门"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_OrgNo") == false)
            {
                /* 创建人名称 */
                MapAttr attr = new MapAttr();
                attr.setFK_MapData(this.No);
                attr.HisEditType = EditType.UnDel;
                attr.setKeyOfEn("OrgNo");
                attr.setName("创建人所在的组织"); //  
                attr.setMyDataType(DataType.AppString);
                attr.setUIContralType(UIContralType.TB);
                attr.setLGType(FieldTypeS.Normal);

                attr.setUIVisible(false);
                attr.setUIIsEnable(false);
                attr.setMinLen(0);
                attr.setMaxLen(100);
                attr.Idx = -1;
                attr.Insert();
            }
            #endregion 补充上流程字段。

            #region 注册到外键表.
            SFTable sf = new SFTable();
            sf.No = this.No;
            if (sf.RetrieveFromDBSources() == 0)
            {
                sf.Name = this.Name;
                sf.SrcType = DictSrcType.SQL;
                sf.SrcTable = this.PTable;
                sf.ColumnValue = "BillNo";
                sf.ColumnText = "Title";
                sf.SelectStatement = "SELECT BillNo AS No, Title as Name FROM " + this.PTable;
                sf.Insert();
            }
            #endregion 注册到外键表
        }

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
            //  http://localhost:2296/WF/CCBill/Admin/Collection/SearchCond.htm?FrmID=Dict_XueShengTaiZhang&ModuleNo=0d5b6b0b-9051-44cf-b989-7e9d802f01af
            return "../../CCBill/Admin/Collection/SearchCond.htm?FrmID=" + this.No;
        }
        #endregion 报表定义.

        #region 权限控制.
        /// <summary>
        /// 保存权限规则
        /// </summary>
        /// <returns></returns>
        public string DoSaveRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSave";
        }
        /// <summary>
        /// 数据版本权限规则.
        /// </summary>
        /// <returns></returns>
        public string DoBtnDataVer()
        {
            return "../../CCBill/Admin/DataVerRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnSave";
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
        /// 新增权限规则
        /// </summary>
        /// <returns></returns>
        public string DoCreateRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnNew";
        }
        /// <summary>
        /// 删除权限规则
        /// </summary>
        /// <returns></returns>
        public string DoDeleteRole()
        {
            return "../../CCBill/Admin/BillRole.htm?s=34&FrmID=" + this.No + "&CtrlObj=BtnDelete";
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
        /// 数据查询权限规则
        /// </summary>
        /// <returns></returns>
        public string DoSearchDataRole()
        {
            return "../../CCBill/Admin/SearchDataRole.htm?s=34&FrmID=" + this.No;
        }
        #endregion 权限控制.

        public string ToolbarSetting()
        {
            return "../../CCBill/Admin/ToolbarSetting.htm?s=34&FrmID=" + this.No ;
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
            return BP.CCBill.Dev2Interface.CreateBlankDictID(this.No, null, null).ToString();
        }
        #endregion 业务逻辑.

        #region 方法操作.
        /// <summary>
        /// 打开单据
        /// </summary>
        /// <returns></returns>
        public string DoOpenBillDict()
        {
            return "../../CCBill/SearchDict.htm?FrmID=" +
              this.No + "&t=" + BP.DA.DataType.CurrentDateTime;
        }
        public string DoOpenBillEditer()
        {
            return "../../CCBill/SearchEditer.htm?FrmID=" +
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
    /// 实体表单s
    /// </summary>
    public class FrmDicts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 实体表单s
        /// </summary>
        public FrmDicts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmDict();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmDict> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmDict>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmDict> Tolist()
        {
            System.Collections.Generic.List<FrmDict> list = new System.Collections.Generic.List<FrmDict>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmDict)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
