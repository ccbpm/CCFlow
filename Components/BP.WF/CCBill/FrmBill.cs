using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF;
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
        #endregion

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
                    str = "ccbpm-{YYYY}-{MM}-{LSH4}";
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
                #endregion 单据属性.

                #region 按钮权限.
                map.AddTBString(FrmBillAttr.BtnNewLable, null, "新建", true, false, 0, 100, 20);
                map.AddBoolean(FrmBillAttr.BtnNewEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnSaveLable, null, "保存", true, false, 0, 100, 20);
                map.AddBoolean(FrmBillAttr.BtnSaveEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnStartFlowLable, null, "启动流程", true, false, 0, 100, 20);
                map.AddBoolean(FrmBillAttr.BtnStartFlowEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnDelLable, null, "删除", true, false, 0, 100, 20);
                map.AddBoolean(FrmBillAttr.BtnDelEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnSearchLabel, null, "查询", true, false, 0, 100, 20);
                map.AddBoolean(FrmBillAttr.BtnSearchEnable, true, "是否可用？", true, true);

                map.AddTBString(FrmBillAttr.BtnGroupLabel, null, "分析", true, false, 0, 100, 20);
                map.AddBoolean(FrmBillAttr.BtnGroupEnable, true, "是否可用？", true, true);
                #endregion 按钮权限.


                #region 设计者信息.
                //map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                //map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                //map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20,true);
                //map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20,false);
                //map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                //map.AddTBStringDoc(MapDataAttr.Note, null, "备注", true, false,true);

                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                RefMethod rm = new RefMethod();

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
                 
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

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
