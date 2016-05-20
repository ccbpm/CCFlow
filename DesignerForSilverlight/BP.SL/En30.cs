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
    /// 数据类型
    /// </summary>
    public class DataType
    {
        #region datatype
        /// <summary>
        /// string
        /// </summary>
        public const string AppString = "1";
        /// <summary>
        /// int
        /// </summary>
        public const string AppInt = "2";
        /// <summary>
        /// float
        /// </summary>
        public const string AppFloat = "3";
        /// <summary>
        /// AppBoolean
        /// </summary>
        public const string AppBoolean = "4";
        /// <summary>
        /// AppDouble
        /// </summary>
        public const string AppDouble = "5";
        /// <summary>
        /// AppDate
        /// </summary>
        public const string AppDate = "6";
        /// <summary>
        /// AppDateTime
        /// </summary>
        public const string AppDateTime = "7";
        /// <summary>
        /// AppMoney
        /// </summary>
        public const string AppMoney = "8";
        /// <summary>
        /// 率百分比。
        /// </summary>
        public const string AppRate = "9";
        #endregion
    }
    public class LGType
    {
        public const string Normal = "0";
        public const string Enum = "1";
        public const string FK = "2";
    }
    /// <summary>
    /// 控件类型
    /// </summary>
    public class CtrlType
    {
        public const string TextBox = "0";
        public const string DDL = "1";
        public const string CheckBox = "2";
        public const string RB = "3";
    }
}
