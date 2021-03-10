using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.WF
{
    /// <summary>
    /// 流程运行类型
    /// </summary>
    public enum TransferCustomType
    {
        /// <summary>
        /// 按照流程定义的模式执行(自动模式)
        /// </summary>s
        ByCCBPMDefine,
        /// <summary>
        /// 按照工作人员的设置执行(人工干涉模式,人工定义模式.)
        /// </summary>
        ByWorkerSet
    }
    /// <summary>
    /// 时间段
    /// </summary>
    public enum TSpan
    {
        /// <summary>
        /// 本周
        /// </summary>
        ThisWeek,
        /// <summary>
        /// 上周
        /// </summary>
        NextWeek,
        /// <summary>
        /// 上上周
        /// </summary>
        TowWeekAgo,
        /// <summary>
        /// 更早
        /// </summary>
        More
    }
    /// <summary>
    /// 流程状态(简)
    /// </summary>
    public enum WFSta
    {
        /// <summary>
        /// 运行中
        /// </summary>
        Runing = 0,
        /// <summary>
        /// 已完成
        /// </summary>
        Complete,
        /// <summary>
        /// 其他
        /// </summary>
        Etc
    }
    /// <summary>
    /// 会签任务状态
    /// </summary>
    public enum HuiQianTaskSta
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 会签中
        /// </summary>
        HuiQianing,
        /// <summary>
        /// 会签完成
        /// </summary>
        HuiQianOver
    }
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskSta
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 共享
        /// </summary>
        Sharing,
        /// <summary>
        /// 已经取走
        /// </summary>
        Takeback
    }
}
