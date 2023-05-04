using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.TA
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public class TaskSta
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public const int Blank = 0;
        /// <summary>
        /// 待办
        /// </summary>
        public const int Todolist = 1;
        /// <summary>
        /// 审核中
        /// </summary>
        public const int SubmitChecking = 2;
        /// <summary>
        /// 完成
        /// </summary>
        public const int WorkOver = 3;
        /// <summary>
        /// 重新工作
        /// </summary>
        public const int ReTodolist = 4;
        /// <summary>
        /// 退回
        /// </summary>
        public const int ReturnWork = 5;
    }
    /// <summary>
    /// 项目状态
    /// </summary>
    public class PrjSta
    {
        /// <summary>
        /// 空白
        /// </summary>
        public const int Blank = 0;
        /// <summary>
        /// 草稿
        /// </summary>
        public const int Draft = 1;
        /// <summary>
        /// 运行中
        /// </summary>
        public const int Runing = 2;
        /// <summary>
        /// 已完成
        /// </summary>
        public const int Complete = 3;
        ///// <summary>
        ///// 挂起
        ///// </summary>
        //public const int Hungup = 4;
        /// <summary>
        /// 删除(逻辑删除状态)
        /// </summary>
        public const int Delete = 7;
        ///// <summary>
        ///// 冻结
        ///// </summary>
        //public const int Fix = 9;
    }
     
}
