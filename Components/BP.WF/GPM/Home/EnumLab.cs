using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using System.Text.RegularExpressions;
namespace BP.GPM.Home
{
    /// <summary>
    /// 内容类型
    /// </summary>
    public enum WinDocType
    {
        /// <summary>
        /// Html
        /// </summary>
        Html = 0,
        /// <summary>
        /// 系统内置
        /// </summary>
        System = 1,
        /// <summary>
        /// SQL列表
        /// </summary>
        SQLList = 2,
        /// <summary>
        /// 折线图
        /// </summary>
        ChatZheXian = 3,
        /// <summary>
        /// 柱状图
        /// </summary>
        ChatZhuZhuang = 4,
        /// <summary>
        /// 饼图
        /// </summary>
        ChatPie = 5 

    }

}
