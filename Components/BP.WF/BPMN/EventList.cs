using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Template;
using BP.Sys;

namespace BP.BPMN
{
    /// <summary>
    ///  网关类型列表
    /// </summary>
    public class EventList
    {
        /// <summary>
        /// 流程启动事件
        /// </summary>
        public const string StartNone = "StartNone";
        /// <summary>
        /// 定时发起
        /// </summary>
        public const string StartTimer = "StartTimer";
        /// <summary>
        /// 信号发起
        /// </summary>
        public const string StartSignal = "StartSignal";
        /// <summary>
        /// 消息驱动发起
        /// </summary>
        public const string StartMessage = "StartMessage";
        /// <summary>
        /// 发起错误事件
        /// </summary>
        public const string StartError = "StartError";
        /// <summary>
        /// 结束事件
        /// </summary>
        public const string EndNone = "EndNone";
        /// <summary>
        /// 结束错误
        /// </summary>
        public const string EndError = "EndError";
    }
   
}
