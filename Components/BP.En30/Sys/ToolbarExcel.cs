using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// ToolbarExcelAttr 属性
    /// </summary>
    public class ToolbarExcelAttr : EntityNoNameAttr
    {
        #region  公文按钮
        /// <summary>
        /// 打开本地-标签
        /// </summary>
        public const string OfficeOpenLab = "OfficeOpenLab";
        /// <summary>
        /// 打开本地
        /// </summary>
        public const string OfficeOpenEnable = "OfficeOpenEnable";
        /// <summary>
        /// 打开模板
        /// </summary>
        public const string OfficeOpenTemplateLab = "OfficeOpenTemplateLab";
        public const string OfficeOpenTemplateEnable = "OfficeOpenTemplateEnable";
        /// <summary>
        /// 保存
        /// </summary>
        public const string OfficeSaveLab = "OfficeSaveLab";
        public const string OfficeSaveEnable = "OfficeSaveEnable";
        /// <summary>
        /// 接受修订
        /// </summary>
        public const string OfficeAcceptLab = "OfficeAcceptLab";
        public const string OfficeAcceptEnable = "OfficeAcceptEnable";
        /// <summary>
        /// 拒绝修订
        /// </summary>
        public const string OfficeRefuseLab = "OfficeRefuseLab";
        public const string OfficeRefuseEnable = "OfficeRefuseEnable";
        /// <summary>
        /// 套红按钮
        /// </summary>
        public const string OfficeOverLab = "OfficeOverLab";
        public const string OfficeOverEnable = "OfficeOverEnable";

        //public const string OfficeOVerLab = "OfficeOVerLab";

        /// <summary>
        /// 查看用户留痕
        /// </summary>
        public const string OfficeMarksEnable = "OfficeMarksEnable";
        /// <summary>
        /// 打印
        /// </summary>
        public const string OfficePrintLab = "OfficePrintLab";
        public const string OfficePrintEnable = "OfficePrintEnable";
        /// <summary>
        /// 签章
        /// </summary>
        public const string OfficeSealLab = "OfficeSealLab";
        public const string OfficeSealEnable = "OfficeSealEnable";

        /// <summary>
        /// 插入流程
        /// </summary>
        public const string OfficeInsertFlowLab = "OfficeInsertFlowLab";
        public const string OfficeInsertFlowEnable = "OfficeInsertFlowEnable";

        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public const string OfficeNodeInfo = "OfficeNodeInfo";
        /// <summary>
        /// 是否该节点保存为PDF
        /// </summary>
        public const string OfficeReSavePDF = "OfficeReSavePDF";
        /// <summary>
        /// 是否进入留痕模式
        /// </summary>
        public const string OfficeIsMarks = "OfficeIsMarks";
        /// <summary>
        /// 指定文档模板
        /// </summary>
        public const string OfficeTemplate = "OfficeTemplate";
        /// <summary>
        /// 是否使用父流程的文档
        /// </summary>
        public const string OfficeIsParent = "OfficeIsParent";
        /// <summary>
        /// 是否启用下载
        /// </summary>
        public const string OfficeDownLab = "OfficeDownLab";
        public const string OfficeDownEnable = "OfficeDownEnable";
        
        /// <summary>
        /// //是否自动套红
        /// </summary>
        public const string OfficeTHEnable = "OfficeTHEnable";
        public const string OfficeTHTemplate = "OfficeTHTemplate";
        /// <summary>
        /// 风险点模板
        /// </summary>
        public const string OfficeFengXianTemplate = "OfficeFengXianTemplate";
        /// <summary>
        /// 插入风险点标签
        /// </summary>
        public const string OfficeInsertFengXian = "OfficeInsertFengXian";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string OfficeInsertFengXianEnabel = "OfficeInsertFengXianEnabel";

        /// <summary>
        /// OfficeReadOnly
        /// </summary>
        public const string OfficeReadOnly = "OfficeReadOnly";
        /// <summary>
        /// 是否可以下载.
        /// </summary>
        public const string OfficeIsDown = "OfficeIsDown";

        public const string OfficeIsTrueTH = "OfficeIsTrueTH";
        public const string OfficeOpen = "OfficeOpen";
        public const string OfficeOpenTemplate = "OfficeOpenTemplate";

        public const string OfficeSave = "OfficeSave";

        public const string OfficeAccept = "OfficeAccept";
        public const string OfficeRefuse = "OfficeRefuse";

        public const string OfficeOver = "OfficeOver";

        public const string OfficeMarks = "OfficeMarks";
        public const string OfficePrint = "OfficePrint";
        public const string OfficeSeal = "OfficeSeal";

        public const string OfficeInsertFlow = "OfficeInsertFlow";
        #endregion
    }
    /// <summary>
    ///  ToolbarExcel 控制器
    /// </summary>
    public class ToolbarExcel : EntityNoName
    {
        #region 界面上的访问控制
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsView = false;
                if (BP.Web.WebUser.No == "admin")
                {
                    uac.IsUpdate = true;
                    uac.IsView = true;
                }
                return uac;
            }
        }
        #endregion

        #region 功能按钮.
        /// <summary>
        /// 打开本地标签.
        /// </summary>
        public string  OfficeOpenLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeOpenLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenLab, value);
            }
        }
        /// <summary>
        /// 是否打开本地模版文件.
        /// </summary>
        public bool OfficeOpenEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeOpenEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenEnable, value);
            }
        }
        /// <summary>
        /// 打开模板 标签.
        /// </summary>
        public string OfficeOpenTemplateLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeOpenTemplateLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenTemplateLab, value);
            }
        }
        /// <summary>
        /// 打开模板.
        /// </summary>
        public bool OfficeOpenTemplateEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeOpenTemplateEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOpenTemplateEnable, value);
            }
        }
        /// <summary>
        /// 保存 标签.
        /// </summary>
        public string OfficeSaveLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeSaveLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSaveLab, value);
            }
        }
        /// <summary>
        /// 保存.是否启用.
        /// </summary>
        public bool OfficeSaveEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeSaveEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSaveEnable, value);
            }
        }
        /// <summary>
        /// 接受修订 标签.
        /// </summary>
        public string OfficeAcceptLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeAcceptLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeAcceptLab, value);
            }
        }
        /// <summary>
        /// 接受修订.
        /// </summary>
        public bool OfficeAcceptEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeAcceptEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeAcceptEnable, value);
            }
        }
        /// <summary>
        /// 拒绝修订 标签.
        /// </summary>
        public string OfficeRefuseLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeRefuseLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeRefuseLab, value);
            }
        }
        /// <summary>
        /// 拒绝修订.
        /// </summary>
        public bool OfficeRefuseEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeRefuseEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeRefuseEnable, value);
            }
        }
        /// <summary>
        /// 套红按钮 标签.
        /// </summary>
        public string OfficeOverLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeOverLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOverLab, value);
            }
        }
        /// <summary>
        /// 套红按钮.
        /// </summary>
        public bool OfficeOverEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeOverEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeOverEnable, value);
            }
        }
        /// <summary>
        /// 查看用户留痕
        /// </summary>
        public bool OfficeMarksEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeMarksEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeMarksEnable, value);
            }
        }
        /// <summary>
        /// 打印按钮-标签
        /// </summary>
        public string OfficePrintLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficePrintLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficePrintLab, value);
            }
        }
        /// <summary>
        /// 打印
        /// </summary>
        public bool OfficePrintEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficePrintEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficePrintEnable, value);
            }
        }
        /// <summary>
        /// 签章-标签
        /// </summary>
        public string OfficeSealLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeSealLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSealLab, value);
            }
        }
        /// <summary>
        /// 签章
        /// </summary>
        public bool OfficeSealEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeSealEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeSealEnable, value);
            }
        }



        /// <summary>
        /// 插入流程-标签
        /// </summary>
        public string OfficeInsertFlowLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeInsertFlowLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeInsertFlowLab, value);
            }
        }
        /// <summary>
        /// 插入流程
        /// </summary>
        public bool OfficeInsertFlowEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeInsertFlowEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeInsertFlowEnable, value);
            }
        }

        /// <summary>
        /// 是否自动记录节点信息
        /// </summary>
        public bool OfficeNodeInfo
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeNodeInfo);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeNodeInfo, value);
            }
        }
        /// <summary>
        /// 是否该节点保存为PDF
        /// </summary>
        public bool OfficeReSavePDF
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeReSavePDF);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeReSavePDF, value);
            }
        }
        /// <summary>
        /// 是否进入留痕模式
        /// </summary>
        public bool OfficeIsMarks
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeIsMarks);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeIsMarks, value);
            }
        }
        /// <summary>
        /// 指定文档模板
        /// </summary>
        public string OfficeTemplate
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeTemplate);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeTemplate, value);
            }
        }
        /// <summary>
        /// 是否使用父流程的文档
        /// </summary>
        public bool OfficeIsParent
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeIsParent);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeIsParent, value);
            }
        }
        /// <summary>
        /// 是否启用标签
        /// </summary>
        public string OfficeDownLab
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeDownLab);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeDownLab, value);
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        public bool OfficeDownEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeDownEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeDownEnable, value);
            }
        }
        /// <summary>
        /// 是否自动套红
        /// </summary>
        public bool OfficeTHEnable
        {
            get
            {
                return this.GetValBooleanByKey(ToolbarExcelAttr.OfficeTHEnable);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeTHEnable, value);
            }
        }
        /// <summary>
        /// 套红模板
        /// </summary>
        public string OfficeTHTemplate
        {
            get
            {
                return this.GetValStringByKey(ToolbarExcelAttr.OfficeTHTemplate);
            }
            set
            {
                this.SetValByKey(ToolbarExcelAttr.OfficeTHTemplate, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// ToolbarExcel功能控制区域
        /// </summary>
        public ToolbarExcel() { }
        /// <summary>
        /// ToolbarExcel功能控制
        /// </summary>
        /// <param name="no">表单ID</param>
        public ToolbarExcel(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_MapData","ToolbarExcel功能控制");

                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, false, 1, 200, 20);
                map.AddTBString(MapDataAttr.Name, null, "表单名称", true, false, 0, 500, 20);

                #region 公文按钮
                map.AddTBString(ToolbarExcelAttr.OfficeOpenLab, "打开本地", "打开本地标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeOpenEnable, false, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeOpenTemplateLab, "打开模板", "打开模板标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeOpenTemplateEnable, false, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeSaveLab, "保存", "保存标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeSaveEnable, true, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeAcceptLab, "接受修订", "接受修订标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeAcceptEnable, false, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeRefuseLab, "拒绝修订", "拒绝修订标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeRefuseEnable, false, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeOverLab, "套红按钮", "套红按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeOverEnable, false, "是否启用", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeMarksEnable, true, "是否查看用户留痕", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficePrintLab, "打印按钮", "打印按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficePrintEnable, false, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeSealLab, "签章按钮", "签章按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeSealEnable, false, "是否启用", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeInsertFlowLab, "插入流程", "插入流程标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeInsertFlowEnable, false, "是否启用", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeNodeInfo, false, "是否记录节点信息", true, true);
                map.AddBoolean(ToolbarExcelAttr.OfficeReSavePDF, false, "是否该自动保存为PDF", true, true);

                map.AddTBString(ToolbarExcelAttr.OfficeDownLab, "下载", "下载按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeDownEnable, false, "是否启用", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeIsMarks, true, "是否进入留痕模式", true, true);
                map.AddTBString(ToolbarExcelAttr.OfficeTemplate, "", "指定文档模板", true, false, 0, 100, 10);
                map.AddBoolean(ToolbarExcelAttr.OfficeIsParent, true, "是否使用父流程的文档", true, true);

                map.AddBoolean(ToolbarExcelAttr.OfficeTHEnable, false, "是否自动套红", true, true);
                map.AddTBString(ToolbarExcelAttr.OfficeTHTemplate, "", "自动套红模板", true, false, 0, 200, 10);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// ToolbarExcel表单.
    /// </summary>
    public class ToolbarExcels : EntitiesNoName
    {
        /// <summary>
        /// 功能控制
        /// </summary>
        public ToolbarExcels()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ToolbarExcel();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ToolbarExcel> Tolist()
        {
            System.Collections.Generic.List<ToolbarExcel> list = new System.Collections.Generic.List<ToolbarExcel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ToolbarExcel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
