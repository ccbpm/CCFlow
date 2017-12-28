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

namespace BP.SL
{
    public delegate void CallBack();
    /// <summary>
    /// 成功执行某个功能后回调
    /// </summary>
    public interface ICallBack
    {
        /// <summary>
        /// 成功执行某个功能后回调
        /// </summary>
        event CallBack DoCompleted;
    }
}
