using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.GPM;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Sys;
using System.Collections.Generic;

namespace BP.Frm
{
    /// <summary>
    /// 单据模版 - Attr
    /// </summary>
    public class FrmTemplateAttr : FrmAttr
    {
    }
    /// <summary>
    /// 单据模版
    /// </summary>
    public class FrmTemplate : EntityNoName
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
                return (EntityType)this.GetValIntByKey(FrmTemplateAttr.EntityType);
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.EntityType, (int)value);
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
                return this.GetValIntByKey(FrmTemplateAttr.BtnNewModel);
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.BtnNewModel, value);
            }
        }
        
        /// <summary>
        /// 单据格式
        /// </summary>
        public string BillNoFormat
        {
            get
            {
                string str = this.GetValStrByKey(FrmTemplateAttr.BillNoFormat);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "{LSH4}";
                return str;
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.BillNoFormat, value);
            }
        }
        /// <summary>
        /// 单据编号生成规则
        /// </summary>
        public string TitleRole
        {
            get
            {
                string str = this.GetValStrByKey(FrmTemplateAttr.TitleRole);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "@WebUser.FK_DeptName @WebUser.Name @RDT";
                return str;
            }
            set
            {
                this.SetValByKey(FrmTemplateAttr.BillNoFormat, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 单据模版
        /// </summary>
        public FrmTemplate()
        {
        }
        /// <summary>
        /// 单据模版
        /// </summary>
        /// <param name="no">映射编号</param>
        public FrmTemplate(string no)
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
                Map map = new Map("Sys_MapData", "单据模版");
                map.Java_SetEnType(EnType.Sys);
                map.Java_SetCodeStruct("4");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                map.SetHelperAlert(MapDataAttr.No, "也叫表单ID,系统唯一.");

                map.AddDDLSysEnum(MapDataAttr.FrmType, 0, "表单类型", true, true, "BillFrmType", "@0=傻瓜表单@1=自由表单");
                map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 500, 20, true);
                map.SetHelperAlert(MapDataAttr.PTable, "存储的表名,如果您修改一个不存在的系统将会自动创建一个表.");

                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 200, 20, true);
                map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), false);

                map.AddDDLSysEnum(FrmAttr.RowOpenMode, 0, "行记录打开模式", true, true,
                    "RowOpenMode", "@0=新窗口打开@1=弹出窗口打开,关闭后刷新列表@2=弹出窗口打开,关闭后不刷新列表");
                #endregion 基本属性.

                #region 单据模版.
                map.AddDDLSysEnum(FrmTemplateAttr.EntityType, 0, "业务类型", true, false, FrmTemplateAttr.EntityType,
                   "@0=独立表单@1=单据@2=编号名称实体@3=树结构实体");
                map.SetHelperAlert(FrmTemplateAttr.EntityType, "该实体的类型,@0=单据@1=编号名称实体@2=树结构实体.");

                map.AddTBString(FrmTemplateAttr.BillNoFormat, null, "实体编号规则", true, false, 0, 100, 20, true);
                map.SetHelperAlert(FrmTemplateAttr.BillNoFormat, "\t\n实体编号规则: \t\n 2标识:01,02,03等, 3标识:001,002,003,等..");
                #endregion 单据模版.


                #region 可以创建的权限.
                //平铺模式.
                map.AttrsOfOneVSM.AddGroupPanelModel(new StationCreates(), new BP.WF.Port.Stations(),
                    StationCreateAttr.FrmID,
                    StationCreateAttr.FK_Station, "可以创建的岗位", StationAttr.FK_StationType);

                map.AttrsOfOneVSM.AddGroupListModel(new StationCreates(), new BP.WF.Port.Stations(),
                  StationCreateAttr.FrmID,
                  StationCreateAttr.FK_Station, "可以创建的岗位AddGroupListModel", StationAttr.FK_StationType);

                //节点绑定部门. 节点绑定部门.
                map.AttrsOfOneVSM.AddBranches(new FrmDeptCreates(), new BP.Port.Depts(),
                   FrmDeptCreateAttr.FrmID,
                   FrmDeptCreateAttr.FK_Dept, "可以创建的部门AddBranches", EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new EmpCreates(), new BP.Port.Emps(),
                   EmpCreateAttr.FrmID,
                   EmpCreateAttr.FK_Emp, "可以创建的人员", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");
                #endregion 可以创建的权限

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
            
    }
    /// <summary>
    /// 单据模版s
    /// </summary>
    public class FrmTemplates : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 单据模版s
        /// </summary>
        public FrmTemplates()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmTemplate();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmTemplate> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmTemplate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmTemplate> Tolist()
        {
            System.Collections.Generic.List<FrmTemplate> list = new System.Collections.Generic.List<FrmTemplate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmTemplate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
