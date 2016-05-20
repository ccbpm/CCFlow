using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 工作提醒规则
    /// </summary>
    public enum CHAlertRole
    {
        /// <summary>
        /// 不提醒
        /// </summary>
        None,
        /// <summary>
        /// 一天一次
        /// </summary>
        OneDayOneTime,
        /// <summary>
        /// 一天两次
        /// </summary>
        OneDayTowTime
    }
    /// <summary>
    /// 工作提醒方式
    /// </summary>
    public enum CHAlertWay
    {
        /// <summary>
        /// 邮件
        /// </summary>
        ByEmail,
        /// <summary>
        /// 短消息
        /// </summary>
        BySMS,
        /// <summary>
        /// 即时通讯
        /// </summary>
        ByCCIM
    }
    /// <summary>
    /// 运行平台
    /// </summary>
    public enum Plant
    {
        CCFlow,
        JFlow
    }
    /// <summary>
    /// 周末休息类型
    /// </summary>
    public enum WeekResetType
    {
        /// <summary>
        /// 双休
        /// </summary>
        Double,
        /// <summary>
        /// 单休
        /// </summary>
        Single,
        /// <summary>
        /// 不
        /// </summary>
        None
    }
    /// <summary>
    /// 用户信息显示格式
    /// </summary>
    public enum UserInfoShowModel
    {
        /// <summary>
        /// 用户ID,用户名
        /// </summary>
        UserIDUserName = 0,
        /// <summary>
        /// 用户ID
        /// </summary>
        UserIDOnly = 1,
        /// <summary>
        /// 用户名
        /// </summary>
        UserNameOnly = 2
    }
}
