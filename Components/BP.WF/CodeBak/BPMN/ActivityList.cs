using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.WF.Template;
using BP.Sys;

namespace BP.BPMN
{
    /// <summary>
    ///  活动类型列表
    /// </summary>
    public class ActivityList
    {
        #region 任务
        /// <summary>
        /// 人机交互任务
        /// </summary>
        public const string UserTask = "UserTask";
        /// <summary>
        /// 服务任务
        /// </summary>
        public const string ServiceTask = "ServiceTask";
        /// <summary>
        /// 脚本执行任务
        /// </summary>
        public const string ScriptTask = "ScriptTask";
        /// <summary>
        /// 业务规则任务
        /// </summary>
        public const string BusinessRuleTask = "BusinessRuleTask";
        /// <summary>
        /// 状态任务
        /// </summary>
        public const string ReceiveTask = "ReceiveTask";
        /// <summary>
        /// 发送任务
        /// </summary>
        public const string MailTask = "MailTask";
        /// <summary>
        /// 线下手工执行任务
        /// </summary>
        public const string ManualTask = "ManualTask";
        #endregion 任务

        #region 其他元素.
        /// <summary>
        /// 顺序流
        /// </summary>
        public const string SequenceFlow = "SequenceFlow";
        /// <summary>
        /// 子流程
        /// </summary>
        public const string SubProcess = "SubProcess";
        #endregion 其他元素.
    }
   
}
