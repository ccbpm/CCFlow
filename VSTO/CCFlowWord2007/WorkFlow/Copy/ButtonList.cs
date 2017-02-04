using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// Btn属性
    /// </summary>
    public class BtnAttr
    {
        /// <summary>
        /// 流程
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
        /// 子线程按钮标签
        /// </summary>
        public const string ThreadLab = "ThreadLab";
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
        /// 打印单据标签
        /// </summary>
        public const string PrintDocLab = "PrintDocLab";
        /// <summary>
        /// 打印单据是否起用
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
        /// 选择接受人
        /// </summary>
        public const string SelectAccepterLab = "SelectAccepterLab";
        /// <summary>
        /// SelectAccepterEnable
        /// </summary>
        public const string SelectAccepterEnable = "SelectAccepterEnable";
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
