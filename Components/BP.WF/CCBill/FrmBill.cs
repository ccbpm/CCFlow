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
        /// 单据工作类型
        /// </summary>
        public int FrmBillWorkModel
        {
            get
            {
                return this.GetValIntByKey(FrmBillAttr.FrmBillWorkModel);
            }
            set
            {
                this.SetValByKey(FrmBillAttr.FrmBillWorkModel, value);
            }
        }
        /// <summary>
        /// 表单类型
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
                string str= this.GetValStrByKey(FrmBillAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "ccbpm-{YYYY}-{LSH4}";
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
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, false, 1, 190, 20);
                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单");
                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 500, 20,true);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20,true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
                #endregion 基本属性.

                #region 单据属性.
                map.AddDDLSysEnum(FrmBillAttr.FrmBillWorkModel, 0, "工作模式", true, true, FrmBillAttr.FrmBillWorkModel,
                    "@0=独立表单@1=单据工作模式@2=流程工作模式");

               // map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "", "@0=独立表单@1=单据工作模式@2=流程工作模式");
                
                map.AddTBString(FrmBillAttr.BillNoFormat, null, "单号规则", true, false, 0, 100, 20,false);
                map.AddTBString(FrmBillAttr.RefFlowNo, null, "关联流程号", true, false, 0, 100, 20);

                map.AddTBString(FrmBillAttr.TitleRole, null, "标题生成规则", true, false, 0, 100, 20, true);
                #endregion 单据属性.

                #region 按钮权限.
                map.AddTBString(FrmBillAttr.BtnNewLable, "新建", "新建", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnNewEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnSaveLable, "保存", "保存", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnSaveEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnStartFlowLable, "启动流程", "启动流程", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnStartFlowEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnDelLable, "删除", "删除", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnDelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnSearchLabel, "查询", "查询", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnSearchEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnGroupLabel, "分析", "分析", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnGroupEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintHtml, "打印Html", "打印Html", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintHtmlEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintPDF, "打印PDF", "打印PDF", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintPDFEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintRTF, "打印RTF", "打印RTF", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintRTFEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnPrintCCWord, "打印CCWord", "打印CCWord", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnPrintCCWordEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnExpZip, "导出zip文件", "导出zip文件", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnExpZipEnable, true, "是否可用？", true, true);
                #endregion 按钮权限.

                #region 查询按钮权限.
                map.AddTBString(FrmBillAttr.BtnImpExcel, "导入Excel文件", "导入Excel文件", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnImpExcelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnExpExcel, "导出Excel文件", "导出Excel文件", true, false, 0, 50, 20);
                map.AddBoolean(FrmBillAttr.BtnExpExcelEnable, true, "是否可用？", true, true);
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
            return "../../CCBill/Admin/ColsChose.htm?FrmID="+this.No;
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
        /// 更新
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            //如果是单据,就要检查一下单据状态.
            if (this.FrmBillWorkModel == 1)
            {
                this.CheckBill(); //检查bill的状态.
            }
            return base.beforeUpdate();
        }
        /// <summary>
        /// 检查单据.
        /// </summary>
        public void CheckBill()
        {
            //取出来全部的属性.
            MapAttrs attrs = new MapAttrs(this.No);

            #region 补充上流程字段到NDxxxRpt.
            int groupID = 0;
            foreach (MapAttr attr in attrs)
            {
                switch (attr.KeyOfEn)
                {
                    case StartWorkAttr.FK_Dept:
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = true;
                        attr.GroupID = groupID;// gfs[0].GetValIntByKey("OID");
                        attr.UIIsEnable = false;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.Update();
                        break;
                    case "FK_NY":
                        //  attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = true;
                        attr.UIIsEnable = false;
                        attr.GroupID = groupID;
                        attr.Update();
                        break;
                    case "FK_Emp":
                        break;
                    default:
                        break;
                }
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.Title) == false)
            {
                /* 标题 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.Title; // "FlowEmps";
                attr.Name = "标题"; //  
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
                attr.Name = "WorkID";
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
                /* 父流程 流程编号 */
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
                /* 参数 流程编号 */
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
                attr.Idx = -100;
                attr.Insert();
            }

            return;

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowEmps) == false)
            {
                /* 参与人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEmps; // "FlowEmps";
                attr.Name = "参与人"; //  
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 1000;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowStarter) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStarter;
                attr.Name = "发起人"; //  
                attr.MyDataType = DataType.AppString;

                //attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;

                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowStartRDT) == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowStartRDT; // "FlowStartRDT";
                attr.Name = "发起时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowEnder) == false)
            {
                /* 发起人 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnder;
                attr.Name = "结束人"; //  
                attr.MyDataType = DataType.AppString;
                // attr.UIBindKey = "BP.Port.Emps";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -1;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowEnderRDT) == false)
            {
                /* MyNum */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEnderRDT; // "FlowStartRDT";
                attr.Name = "结束时间";
                attr.MyDataType = DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowEndNode) == false)
            {
                /* 结束节点 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowEndNode;
                attr.Name = "结束节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.PFlowNo) == false)
            {
                /* 父流程 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PFlowNo;
                attr.Name = "父流程编号"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 3;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.PNodeID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PNodeID;
                attr.Name = "父流程启动的节点";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.PWorkID) == false)
            {
                /* 父流程WorkID */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PWorkID;
                attr.Name = "父流程WorkID";
                attr.MyDataType = DataType.AppInt;
                attr.DefVal = "0";
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.HisEditType = EditType.UnDel;
                attr.Idx = -101;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.PEmp) == false)
            {
                /* 调起子流程的人员 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PEmp;
                attr.Name = "调起子流程的人员";
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -100;
                attr.Insert();
            }

        

            if (attrs.Contains(this.No + "_" + GERptAttr.GUID) == false)
            {
                /* GUID 流程编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.GUID;
                attr.Name = "GUID"; // 单据编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.UIIsLine = false;
                attr.MinLen = 0;
                attr.MaxLen = 32;
                attr.Idx = -100;
                attr.Insert();
            }

            if (attrs.Contains(this.No + "_" + GERptAttr.PrjNo) == false)
            {
                /* 项目编号 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PrjNo;
                attr.Name = "项目编号"; //  项目编号
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
            if (attrs.Contains(this.No + "_" + GERptAttr.PrjName) == false)
            {
                /* 项目名称 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.PrjName;
                attr.Name = "项目名称"; //  项目名称
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

            if (attrs.Contains(this.No + "_" + GERptAttr.FlowNote) == false)
            {
                /* 流程信息 */
                MapAttr attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = EditType.UnDel;
                attr.KeyOfEn = GERptAttr.FlowNote;
                attr.Name = "流程信息"; //  父流程流程编号
                attr.MyDataType = DataType.AppString;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = true;
                attr.UIIsEnable = false;
                attr.UIIsLine = true;
                attr.MinLen = 0;
                attr.MaxLen = 500;
                attr.Idx = -100;
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

            string guid=DBAccess.GenerGUID();

            string url = "../WF/CCBill/Search.htm?FrmID="+this.No;
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
