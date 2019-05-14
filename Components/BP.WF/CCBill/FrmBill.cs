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

namespace BP.WF.CCBill
{
    /// <summary>
    /// 实体类型
    /// </summary>
    public enum EntityType
    {
        CCFrom = 0,
        Bill = 1,
        EnityNoName = 2,
        EntityTree = 3
    }

    /// <summary>
    /// 单据属性 - Attr
    /// </summary>
    public class FrmBillAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string FrmBillWorkModel = "FrmBillWorkModel";
        /// <summary>
        /// 实体类型
        /// </summary>
        public const string EntityType = "EntityType";
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public const string BillNoFormat = "BillNoFormat";
        /// <summary>
        /// 关联的流程号
        /// </summary>
        public const string RefFlowNo = "RefFlowNo";
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public const string TitleRole = "TitleRole";
        #endregion

        #region 隐藏属性.
        /// <summary>
        /// 要显示的列
        /// </summary>
        public const string ShowCols = "ShowCols";
        #endregion 隐藏属性

        #region 按钮信息.
        /// <summary>
        /// 按钮New标签
        /// </summary>
        public const string BtnNewLable = "BtnNewLable";
        /// <summary>
        /// 按钮New启用规则
        /// </summary>
        public const string BtnNewEnable = "BtnNewEnable";
        /// <summary>
        /// 按钮Save标签
        /// </summary>
        public const string BtnSaveLable = "BtnSaveLable";
        /// <summary>
        /// 按钮save启用规则
        /// </summary>
        public const string BtnSaveEnable = "BtnSaveEnable";
        /// <summary>
        /// 按钮del标签
        /// </summary>
        public const string BtnDelLable = "BtnDelLable";
        /// <summary>
        /// 按钮del启用规则
        /// </summary>
        public const string BtnDelEnable = "BtnDelEnable";
        /// <summary>
        /// 按钮del标签
        /// </summary>
        public const string BtnStartFlowLable = "BtnStartFlowLable";
        /// <summary>
        /// 按钮del启用规则
        /// </summary>
        public const string BtnStartFlowEnable = "BtnStartFlowEnable";
        /// <summary>
        /// 查询
        /// </summary>
        public const string BtnSearchLabel = "BtnSearchLabel";
        /// <summary>
        /// 查询
        /// </summary>
        public const string BtnSearchEnable = "BtnSearchEnable";
        /// <summary>
        /// 分析
        /// </summary>
        public const string BtnGroupLabel = "BtnGroupLabel";
        /// <summary>
        /// 分析
        /// </summary>
        public const string BtnGroupEnable = "BtnGroupEnable";
        #endregion

        #region 打印
        public const string BtnPrintHtml = "BtnPrintHtml";
        public const string BtnPrintHtmlEnable = "BtnPrintHtmlEnable";

        public const string BtnPrintPDF = "BtnPrintPDF";
        public const string BtnPrintPDFEnable = "BtnPrintPDFEnable";

        public const string BtnPrintRTF = "BtnPrintRTF";
        public const string BtnPrintRTFEnable = "BtnPrintRTFEnable";

        public const string BtnPrintCCWord = "BtnPrintCCWord";
        public const string BtnPrintCCWordEnable = "BtnPrintCCWordEnable";
        #endregion

        /// <summary>
        /// 导出zip文件
        /// </summary>
        public const string BtnExpZip = "BtnExpZip";
        /// <summary>
        /// 是否可以启用?
        /// </summary>
        public const string BtnExpZipEnable = "BtnExpZipEnable";

        #region 集合的操作.
        /// <summary>
        /// 导入Excel
        /// </summary>
        public const string BtnImpExcel = "BtnImpExcel";
        /// <summary>
        /// 是否启用导入
        /// </summary>
        public const string BtnImpExcelEnable = "BtnImpExcelEnable";

        /// <summary>
        /// 导出Excel
        /// </summary>
        public const string BtnExpExcel = "BtnExpExcel";
        /// <summary>
        /// 导出excel
        /// </summary>
        public const string BtnExpExcelEnable = "BtnExpExcelEnable";
        #endregion 集合的操作.


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
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsDelete = false;
                    uac.IsUpdate = true;
                    return uac;
                }
                uac.Readonly();
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
                if (s == "" || s == null)
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
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单");
                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20, true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                #endregion 基本属性.

                #region 单据属性.
                //map.AddDDLSysEnum(FrmBillAttr.FrmBillWorkModel, 0, "工作模式", true, false, FrmBillAttr.FrmBillWorkModel,
                //    "@0=独立表单@1=单据工作模式");

                map.AddDDLSysEnum(FrmBillAttr.EntityType, 0, "业务类型", true, false, FrmBillAttr.EntityType,
                   "@0=独立表单@1=单据@2=编号名称实体@3=树结构实体");
                map.SetHelperAlert(FrmBillAttr.EntityType, "该实体的类型,@0=单据@1=编号名称实体@2=树结构实体.");

                //map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "", "@0=独立表单@1=单据工作模式@2=流程工作模式");

                map.AddTBString(FrmBillAttr.BillNoFormat, null, "单号规则", true, false, 0, 100, 20, false);
                map.AddTBString(FrmBillAttr.RefFlowNo, null, "关联流程号", true, false, 0, 100, 20);

                map.AddTBString(FrmBillAttr.TitleRole, null, "标题生成规则", true, false, 0, 100, 20, true);
                #endregion 单据属性.

                #region MyBill - 按钮权限.
                map.AddTBString(FrmBillAttr.BtnNewLable, "新建", "新建", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnNewEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnSaveLable, "保存", "保存", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnSaveEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnStartFlowLable, "启动流程", "启动流程", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnStartFlowEnable, false, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnDelLable, "删除", "删除", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnDelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnSearchLabel, "查询", "查询", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnSearchEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnGroupEnable, false, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintHtml, "打印Html", "打印Html", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintHtmlEnable, false, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintPDF, "打印PDF", "打印PDF", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintPDFEnable, false, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintRTF, "打印RTF", "打印RTF", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintRTFEnable, false, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintCCWord, "打印CCWord", "打印CCWord", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintCCWordEnable, false, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnExpZip, "导出zip文件", "导出zip文件", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnExpZipEnable, false, "是否可用？", true, true);
                #endregion 按钮权限.

                #region 查询按钮权限.
                map.AddTBString(FrmBillAttr.BtnImpExcel, "导入Excel文件", "导入Excel文件", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnImpExcelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnExpExcel, "导出Excel文件", "导出Excel文件", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnExpExcelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnGroupEnable, true, "是否可用？", true, true);

                #endregion 查询按钮权限.



                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBStringDoc(MapDataAttr.Note, null, "备注", true, false, true);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                map.AddTBAtParas(800); //参数属性.


                #region 基本功能.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "设计表单"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoDesigner";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "单据url的API"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoAPI";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //rm.GroupName = "开发接口";
                map.AddRefMethod(rm);

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

                rm = new RefMethod();
                rm.Title = "执行方法"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoMethod";
                rm.Icon = "../../WF/Img/Event.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                #endregion 基本功能.

                #region 报表定义.
                rm = new RefMethod();
                rm.GroupName = "报表定义";
                rm.Title = "设置显示的列"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_ColsChose";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "报表定义";
                rm.Title = "列的顺序"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_ColsIdxAndLabel";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                //   map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "报表定义";
                rm.Title = "查询条件"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoRpt_SearchCond";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 报表定义.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

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
        #endregion 报表定义.

        public string DoMethod()
        {
            return "../../CCBill/Admin/Method.htm?s=34&FrmID=" + this.No + "&ExtType=PageLoadFull&RefNo=";
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
        /// 设计表单
        /// </summary>
        /// <returns></returns>
        public string DoDesigner()
        {
            if (this.FrmType == Sys.FrmType.FreeFrm)
                return "";
            return "";
        }
        /// <summary>
        /// 检查检查实体类型
        /// </summary>
        public void CheckEnityTypeAttrsFor_Bill()
        {
            //取出来全部的属性.
            MapAttrs attrs = new MapAttrs(this.No);

            #region 补充上流程字段到 NDxxxRpt.
            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 标题 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.Title; // "FlowEmps";
                    attr.Name = "标题"; //   单据模式， ccform的模式.
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 400;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.OID) == false)
            {
                /* WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.KeyOfEn = "OID";
                attr.Name = "主键ID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.BillNo;

                attr.Name = "单据编号"; //  单据编号
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

            if (attrs.Contains(this.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 参数 */
                MapAttr attr = new BP.Sys.MapAttr();
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

            if (attrs.Contains(this.No + "_BillState") == false)
            {
                /* 单据状态 */
                MapAttr attr = new BP.Sys.MapAttr();
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
                MapAttr attr = new BP.Sys.MapAttr();
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
                MapAttr attr = new BP.Sys.MapAttr();
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

            if (attrs.Contains(this.No + "_RDT") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "RDT"; // "FlowStartRDT";
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
            #endregion 补充上流程字段。
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
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.KeyOfEn = "OID";
                attr.Name = "主键ID";
                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "0";
                attr.HisEditType = BP.En.EditType.Readonly;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_" + GERptAttr.BillNo) == false)
            {
                /* 单据编号 */
                MapAttr attr = new BP.Sys.MapAttr();
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

            if (attrs.Contains(this.No + "_" + GERptAttr.AtPara) == false)
            {
                /* 参数 */
                MapAttr attr = new BP.Sys.MapAttr();
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
            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 标题 */
                MapAttr attr = new BP.Sys.MapAttr();
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
                attr.Idx = -100;
                attr.Insert();
            }
            if (attrs.Contains(this.No + "_BillState") == false)
            {
                /* 单据状态 */
                MapAttr attr = new BP.Sys.MapAttr();
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
                MapAttr attr = new BP.Sys.MapAttr();
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
                MapAttr attr = new BP.Sys.MapAttr();
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

            if (attrs.Contains(this.No + "_RDT") == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = "RDT"; // "FlowStartRDT";
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
            #endregion 补充上流程字段。

            #region 注册到外键表.
            SFTable sf = new SFTable();
            sf.No = this.No;
            if (sf.RetrieveFromDBSources() == 0)
            {
                sf.Name = this.Name;
                sf.SrcType = SrcType.SQL;
                sf.SrcTable = this.PTable;
                sf.ColumnValue = "BillNo";
                sf.ColumnText = "Title";
                sf.SelectStatement = "SELECT BillNo AS No, Title as Name FROM " + this.PTable;
                sf.Insert();
            }

            #endregion 注册到外键表
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
            return BP.WF.CCBill.Dev2Interface.CreateBlankWork(this.No, BP.Web.WebUser.No, null).ToString();
        }
        #endregion 业务逻辑.

        #region 方法操作.
        /// <summary>
        /// 打开单据
        /// </summary>
        /// <returns></returns>
        public string DoOpenBill()
        {
            return "../../CCBill/Search.htm?FrmID=" +
              this.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        }
        public string DoAPI()
        {
            return "../../Admin/FoolFormDesigner/Bill/API.htm?FrmID=" +
              this.No + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff");
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
