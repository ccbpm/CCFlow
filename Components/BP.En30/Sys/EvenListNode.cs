using System;

namespace BP.Sys
{
    /// <summary>
    /// 事件标记列表
    /// </summary>
    public class EventListNode
    {
        #region 节点表单事件
        /// <summary>
        /// 保存后
        /// </summary>
        public const string NodeFrmSaveAfter = "NodeFrmSaveAfter";
        /// <summary>
        /// 保存前
        /// </summary>
        public const string NodeFrmSaveBefore = "NodeFrmSaveBefore";
        public const string FrmLoadAfter = "FrmLoadAfter";
        public const string FrmLoadBefore = "FrmLoadBefore";
        #endregion 节点表单事件

        #region 节点事件
        /// <summary>
        /// 节点发送前
        /// </summary>
        public const string SendWhen = "SendWhen";
        /// <summary>
        /// 工作到达
        /// </summary>
        public const string WorkArrive = "WorkArrive";
        /// <summary>
        /// 节点发送成功后
        /// </summary>
        public const string SendSuccess = "SendSuccess";
        /// <summary>
        /// 节点发送失败后
        /// </summary>
        public const string SendError = "SendError";
        /// <summary>
        /// 当节点退回前
        /// </summary>
        public const string ReturnBefore = "ReturnBefore";
        /// <summary>
        /// 当节点退后
        /// </summary>
        public const string ReturnAfter = "ReturnAfter";
        /// <summary>
        /// 当节点撤销发送前
        /// </summary>
        public const string UndoneBefore = "UndoneBefore";
        /// <summary>
        /// 当节点撤销发送后
        /// </summary>
        public const string UndoneAfter = "UndoneAfter";
        /// <summary>
        /// 当前节点移交后
        /// </summary>
        public const string ShitAfter = "ShitAfter";
        /// <summary>
        /// 节点催办后
        /// </summary>
        public const string PressAfter = "PressAfter";
        /// <summary>
        /// 节点抄送后
        /// </summary>
        public const string CCAfter = "CCAfter";
        /// <summary>
        /// 当节点加签后
        /// </summary>
        public const string AskerAfter = "AskerAfter";
        /// <summary>
        /// 当节点加签答复后
        /// </summary>
        public const string AskerReAfter = "AskerReAfter";
        /// <summary>
        /// 队列节点发送后
        /// </summary>
        public const string QueueSendAfter = "QueueSendAfter";
        /// <summary>
        /// 节点打开后.
        /// </summary>
        public const string WhenReadWork = "WhenReadWork";
        /// <summary>
        /// 节点预警
        /// </summary>
        public const string NodeWarning = "NodeWarning";
        /// <summary>
        /// 节点逾期
        /// </summary>
        public const string NodeOverDue = "NodeOverDue";
        /// <summary>
        /// 流程预警
        /// </summary>
        public const string FlowWarning = "FlowWarning";
        /// <summary>
        /// 流程逾期
        /// </summary>
        public const string FlowOverDue = "FlowOverDue";
        #endregion 节点事件
    }
     
}
