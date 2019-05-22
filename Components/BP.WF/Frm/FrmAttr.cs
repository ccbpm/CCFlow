using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.Frm
{
    /// <summary>
    /// 实体表单 - Attr
    /// </summary>
    public class FrmAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作模式
        /// </summary>
        public const string FrmDictWorkModel = "FrmDictWorkModel";
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
        public const string BtnNewModel = "BtnNewModel";
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

        public const string FrmBillWorkModel = "FrmBillWorkModel";
        public const string BtnNewEnable = "BtnNewEnable";
        /// <summary>
        /// 行打开模式
        /// </summary>
        public const string RowOpenMode = "RowOpenMode";

        public const string Tag0 = "Tag0";
        public const string Tag1 = "Tag1";
        public const string Tag2 = "Tag2";

    }
}
