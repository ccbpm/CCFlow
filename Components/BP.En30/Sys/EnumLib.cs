using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.Sys
{
    /// <summary>
    /// 从表显示模式
    /// </summary>
    public enum ListShowModel
    {
        /// <summary>
        /// 表格模式
        /// </summary>
        Table,
        /// <summary>
        /// 傻瓜表单模式
        /// </summary>
        Card
    }
    /// <summary>
    /// 从表显示方式
    /// </summary>
    public enum EditModel
    {
        /// <summary>
        /// 表格模式
        /// </summary>
        TableModel,
        /// <summary>
        /// 傻瓜表单模式
        /// </summary>
        FoolModel,
        /// <summary>
        /// 自由表单模式
        /// </summary>
        FreeModel
    }
    /// <summary>
    /// 明细表存盘方式
    /// </summary>
    public enum DtlAddRecModel
    {
        /// <summary>
        /// 自动初始化空白行
        /// </summary>
        ByBlank,
        /// <summary>
        /// 用按钮增加行
        /// </summary>
        ByButton
    }
    public enum DtlSaveModel
    {
        /// <summary>
        /// 失去焦点自动存盘
        /// </summary>
        AutoSave,
        /// <summary>
        /// 由保存按钮触发存盘
        /// </summary>
        HandSave
    }
    /// <summary>
    /// 棫行处理
    /// </summary>
    public enum WhenOverSize
    {
        /// <summary>
        /// 不处理
        /// </summary>
        None,
        /// <summary>
        /// 增加一行
        /// </summary>
        AddRow,
        /// <summary>
        /// 翻页
        /// </summary>
        TurnPage
    }
    public enum DtlOpenType
    {
        /// <summary>
        /// 对人员开放
        /// </summary>
        ForEmp,
        /// <summary>
        /// 对工作开放
        /// </summary>
        ForWorkID,
        /// <summary>
        /// 对流程开放
        /// </summary>
        ForFID
    }
    /// <summary>
    /// 明细表工作方式
    /// </summary>
    public enum DtlModel
    {
        /// <summary>
        /// 普通的
        /// </summary>
        Ordinary,
        /// <summary>
        /// 固定列
        /// </summary>
        FixRow
    }
}
