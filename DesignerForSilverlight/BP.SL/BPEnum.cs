using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BP.En
{
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 运行存储过程
        /// </summary>
        RunSP = 1,
        /// <summary>
        /// 运行sql
        /// </summary>
        RunSQL = 2,
        /// <summary>
        /// 执行URL
        /// </summary>
        RunURL = 3,
        /// <summary>
        /// 运行webservices
        /// </summary>
        RunWS = 4,
        /// <summary>
        /// 运行Exe文件.
        /// </summary>
        RunExe =5,
        /// <summary>
        /// 运行JS
        /// </summary>
        RunJS
    }
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum BtnType
    {
        /// <summary>
        /// 保存
        /// </summary>
        Save = 0,
        /// <summary>
        /// 打印
        /// </summary>
        Print = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 增加
        /// </summary>
        Add = 3,
        /// <summary>
        /// 自定义
        /// </summary>
        Self = 100
    }
    /// <summary>
    /// 编辑类型
    /// </summary>
    public enum EditType
    {
        /// <summary>
        /// 可编辑
        /// </summary>
        Edit,
        /// <summary>
        /// 不可删除
        /// </summary>
        UnDelete,
        /// <summary>
        /// 只读,不可删除
        /// </summary>
        Readonly
    }
}
