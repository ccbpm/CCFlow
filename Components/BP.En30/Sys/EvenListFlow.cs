using System;

namespace BP.Sys
{
    /// <summary>
    /// 事件标记列表
    /// </summary>
    public class EventListFlow
    {
        #region 流程事件
        /// <summary>
        /// 当创建workid的时候.
        /// </summary>
        public const string FlowOnCreateWorkID = "FlowOnCreateWorkID";
        /// <summary>
        /// 流程完成时.
        /// </summary>
        public const string FlowOverBefore = "FlowOverBefore";
        /// <summary>
        /// 结束后.
        /// </summary>
        public const string FlowOverAfter = "FlowOverAfter";
        /// <summary>
        /// 流程删除前
        /// </summary>
        public const string BeforeFlowDel = "BeforeFlowDel";
        /// <summary>
        /// 流程删除后
        /// </summary>
        public const string AfterFlowDel = "AfterFlowDel";
        #endregion 流程事件
    }
}
