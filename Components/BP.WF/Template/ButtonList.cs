using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// Btn属性
    /// </summary>
    public class BtnAttr : BP.Sys.ToolbarExcelAttr
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 发送标签
        /// </summary>
        public const string SendLab = "SendLab";
        /// <summary>
        /// 子线程按钮是否启用
        /// </summary>
        public const string ThreadEnable = "ThreadEnable";
        /// <summary>
        /// 是否可以删除（已经发出去的）子线程.
        /// </summary>
        public const string ThreadIsCanDel = "ThreadIsCanDel";
        /// <summary>
        /// 是否可以移交
        /// </summary>
        public const string ThreadIsCanShift = "ThreadIsCanShift";
        /// <summary>
        /// 子线程按钮标签
        /// </summary>
        public const string ThreadLab = "ThreadLab";
        /// <summary>
        /// 子流程标签
        /// </summary>
        public const string SubFlowLab = "SubFlowLab";
        /// <summary>
        /// 子流程删除规则.
        /// </summary>
        public const string SubFlowCtrlRole = "SubFlowCtrlRole";
        /// <summary>
        /// 可否启用
        /// </summary>
        public const string SubFlowEnable = "SubFlowEnable";
        /// <summary>
        /// 保存是否启用
        /// </summary>
        public const string SaveEnable = "SaveEnable";
        /// <summary>
        /// 跳转规则
        /// </summary>
        public const string JumpWayLab = "JumpWayLab";
        /// <summary>
        /// 保存标签
        /// </summary>
        public const string SaveLab = "SaveLab";
        /// <summary>
        /// 退回是否启用
        /// </summary>
        public const string ReturnRole = "ReturnRole";
        /// <summary>
        /// 退回标签
        /// </summary>
        public const string ReturnLab = "ReturnLab";
        /// <summary>
        /// 退回的信息填写字段
        /// </summary>
        public const string ReturnField = "ReturnField";
        /// <summary>
        /// 打印单据标签
        /// </summary>
        public const string PrintDocLab = "PrintDocLab";
        /// <summary>
        /// 打印单据是否启用
        /// </summary>
        public const string PrintDocEnable = "PrintDocEnable";
        /// <summary>
        /// 移交是否启用
        /// </summary>
        public const string ShiftEnable = "ShiftEnable";
        /// <summary>
        /// 移交标签
        /// </summary>
        public const string ShiftLab = "ShiftLab";
        /// <summary>
        /// 查询标签
        /// </summary>
        public const string SearchLab = "SearchLab";
        /// <summary>
        /// 查询是否可用
        /// </summary>
        public const string SearchEnable = "SearchEnable";
        /// <summary>
        /// 轨迹
        /// </summary>
        public const string TrackLab = "TrackLab";
        /// <summary>
        /// 轨迹是否启用
        /// </summary>
        public const string TrackEnable = "TrackEnable";
        /// <summary>
        /// 抄送
        /// </summary>
        public const string CCLab = "CCLab";
        /// <summary>
        /// 抄送规则
        /// </summary>
        public const string CCRole = "CCRole";
        /// <summary>
        /// 删除
        /// </summary>
        public const string DelLab = "DelLab";
        /// <summary>
        /// 删除是否启用
        /// </summary>
        public const string DelEnable = "DelEnable";
        /// <summary>
        /// 结束流程
        /// </summary>
        public const string EndFlowLab = "EndFlowLab";
        /// <summary>
        /// 结束流程
        /// </summary>
        public const string EndFlowEnable = "EndFlowEnable";
        /// <summary>
        /// 发送按钮
        /// </summary>
        public const string SendJS = "SendJS";
        /// <summary>
        /// 挂起
        /// </summary>
        public const string HungLab = "HungLab";
        /// <summary>
        /// 是否启用挂起
        /// </summary>
        public const string HungEnable = "HungEnable";
        /// <summary>
        /// 查看父流程
        /// </summary>
        public const string ShowParentFormLab = "ShowParentFormLab";
        /// <summary>
        /// 是否启用查看父流程
        /// </summary>
        public const string ShowParentFormEnable = "ShowParentFormEnable";
        /// <summary>
        /// 审核
        /// </summary>
        public const string WorkCheckLab = "WorkCheckLab";
        /// <summary>
        /// 审核是否可用
        /// </summary>
        public const string WorkCheckEnable = "WorkCheckEnable";
        /// <summary>
        /// 批处理
        /// </summary>
        public const string BatchLab = "BatchLab";
        /// <summary>
        /// 批处理是否可用
        /// </summary>
        public const string BatchEnable = "BatchEnable";
        /// <summary>
        /// 加签
        /// </summary>
        public const string AskforLab = "AskforLab";
        /// <summary>
        /// 加签标签
        /// </summary>
        public const string AskforEnable = "AskforEnable";

        /// <summary>
        /// 会签标签
        /// </summary>
        public const string HuiQianLab = "HuiQianLab";
        /// <summary>
        /// 会签规则
        /// </summary>
        public const string HuiQianRole = "HuiQianRole";

        /// <summary>
        /// 流转自定义 TransferCustomLab
        /// </summary>
        public const string TCLab = "TCLab";
        /// <summary>
        /// 是否启用-流转自定义
        /// </summary>
        public const string TCEnable = "TCEnable";

        /// <summary>
        /// 公文
        /// </summary>
        public const string WebOfficeLab = "WebOffice";
        /// <summary>
        /// 公文按钮标签
        /// </summary>
        public const string WebOfficeEnable = "WebOfficeEnable";
        /// <summary>
        /// 节点时限规则
        /// </summary>
        public const string CHRole = "CHRole";
        /// <summary>
        /// 节点时限lab
        /// </summary>
        public const string CHLab = "CHLab";
        /// <summary>
        /// 重要性 
        /// </summary>
        public const string PRILab = "PRILab";
        /// <summary>
        /// 是否启用-重要性
        /// </summary>
        public const string PRIEnable = "PRIEnable";

        /// <summary>
        /// 关注 
        /// </summary>
        public const string FocusLab = "FocusLab";
        /// <summary>
        /// 是否启用-关注
        /// </summary>
        public const string FocusEnable = "FocusEnable";
        /// <summary>
        /// 确认
        /// </summary>
        public const string ConfirmLab = "ConfirmLab";
        /// <summary>
        /// 是否启用确认
        /// </summary>
        public const string ConfirmEnable = "ConfirmEnable";
        /// <summary>
        /// 打印html
        /// </summary>
        public const string PrintHtmlLab = "PrintHtmlLab";
        /// <summary>
        /// 打印html
        /// </summary>
        public const string PrintHtmlEnable = "PrintHtmlEnable";
        /// <summary>
        /// 打印pdf
        /// </summary>
        public const string PrintPDFLab = "PrintPDFLab";
        /// <summary>
        /// 打印pdf
        /// </summary>
        public const string PrintPDFEnable = "PrintPDFEnable";
        /// <summary>
        /// 打印pdf规则
        /// </summary>
        public const string PrintPDFModle = "PrintPDFModle";
        /// <summary>
        /// 水印设置规则
        /// </summary>
        public const string ShuiYinModle = "ShuiYinModle";
        /// <summary>
        /// 打包下载
        /// </summary>
        public const string PrintZipLab = "PrintZipLab";
        /// <summary>
        /// 是否启用打包下载
        /// </summary>
        public const string PrintZipEnable = "PrintZipEnable";

        /// <summary>
        /// 分配
        /// </summary>
        public const string AllotLab = "AllotLab";
        /// <summary>
        /// 分配启用
        /// </summary>
        public const string AllotEnable = "AllotEnable";
        /// <summary>
        /// 选择接受人
        /// </summary>
        public const string SelectAccepterLab = "SelectAccepterLab";
        /// <summary>
        /// 是否启用选择接受人
        /// </summary>
        public const string SelectAccepterEnable = "SelectAccepterEnable";


       


        #region 公文2019
        /// <summary>
        /// 公文标签
        /// </summary>
        public const string OfficeBtnLab = "OfficeBtnLab";
        /// <summary>
        /// 公文标签接受人
        /// </summary>
        public const string OfficeBtnEnable = "OfficeBtnEnable";
        #endregion 公文2019


        #region 公文属性
        public const string DocLeftWord = "DocLeftWord";
        public const string DocRightWord = "DocRightWord";
        /// <summary>
        /// 工作方式
        /// </summary>
        public const string WebOfficeFrmModel = "WebOfficeFrmModel";
        #endregion 公文属性

        /// <summary>
        /// 列表
        /// </summary>
        public const string ListLab = "ListLab";
        /// <summary>
        /// 是否启用-列表
        /// </summary>
        public const string ListEnable = "ListEnable";
    }
    /// <summary>
    /// 按钮列表
    /// </summary>
    public class ButtonList
    {
        /// <summary>
        /// 新建流程
        /// </summary>
        public const string Btn_NewFlow = "Btn_NewFlow";
        /// <summary>
        /// 发送流程
        /// </summary>
        public const string Btn_Send = "Btn_Send";
        /// <summary>
        /// 保存流程
        /// </summary>
        public const string Btn_Save = "Btn_Save";
        /// <summary>
        /// 退回
        /// </summary>
        public const string Btn_Return = "Btn_Return";
        /// <summary>
        /// 转发
        /// </summary>
        public const string Btn_Forward = "Btn_Forward";
        /// <summary>
        /// 撤销发送
        /// </summary>
        public const string Btn_UnSend = "Btn_UnSend";
        /// <summary>
        /// 删除流程
        /// </summary>
        public const string Btn_DelFlow = "Btn_DelFlow";
        /// <summary>
        /// 流程轨迹
        /// </summary>
        public const string Btn_Track = "Btn_Track";
        /// <summary>
        /// Btn_Search
        /// </summary>
        public const string Btn_Search = "Btn_Search";
    }
}
