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
    public class WinDocModel
    {
        /// <summary>
        /// Html
        /// </summary>
        public const string Html = "Html";
        /// <summary>
        /// 文本变量
        /// </summary>
        public const string HtmlVar = "HtmlVar";
        /// <summary>
        /// 系统内置
        /// </summary>
        public const string System = "System";
        /// <summary>
        /// SQL列表
        /// </summary>
        public const string Table = "Table";
        /// <summary>
        /// 折线图
        /// </summary>
        public const string ChartZZT = "ChartZZT";
        /// <summary>
        /// 柱状图
        /// </summary>
        public const string ChartLine = "ChartLine";
        /// <summary>
        /// 饼图
        /// </summary>
        public const string ChartPie = "ChartPie";
        /// <summary>
        /// 扇形图
        /// </summary>
        public const string ChartRate = "ChartRate";
        /// <summary>
        /// 环形图
        /// </summary>
        public const string ChartRing = "ChartRing";
        /// <summary>
        /// 标签页
        /// </summary>
        public const string Tab = "Tab";
    }

}
