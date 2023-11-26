using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 流程操作列表
    /// </summary>
    public class FlowOpList
    {
        /// <summary>
        /// 催办
        /// </summary>
        public const string PressTimes = "PressTimes";
        /// <summary>
        /// 强制删除
        /// </summary>
        public const string FlowOverByCoercion = "FlowOverByCoercion.";
        /// <summary>
        /// 撤销发送
        /// </summary>
        public const string UnSend = "UnSend";
        /// <summary>
        /// 强制移交
        /// </summary>
        public const string ShiftByCoercion = "ShiftByCoercion";
        /// <summary>
        /// 撤销挂起
        /// </summary>
        public const string UnHungUp = "UnHungUp";
        /// <summary>
        /// 挂起
        /// </summary>
        public const string HungUp = "HungUp";
    }
}
