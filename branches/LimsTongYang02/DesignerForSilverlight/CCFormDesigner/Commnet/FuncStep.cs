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
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace CCForm
{
    public class FuncStep
    {
        #region attrs
        /// <summary>
        /// DelEle
        /// </summary>
        public const string DelEle = "DelEle";
        /// <summary>
        /// 移动 ele
        /// </summary>
        public const string MoveEle = "MoveEle";
        /// <summary>
        /// add lab 
        /// </summary>
        public const string AddEle = "AddEle";
        #endregion

        #region 字段
        string _DoType;
        UIElement _Ele;
        int _Step;
        double _X1 = 0;
        double _Y1 = 0;
        double _X2 = 0;
        double _Y2 = 0;
        #endregion

        #region 属性
        public double X1
        {
            get { return _X1; }
            set { _X1 = value; }
        }
        public double X2
        {
            get { return _X2; }
            set { _X2 = value; }
        }
        public double Y1
        {
            get { return _Y1; }
            set { _Y1 = value; }
        }
        public double Y2
        {
            get { return _Y2; }
            set { _Y2 = value; }
        }
        /// <summary>
        /// 图标名称
        /// </summary>
        public string DoType
        {
            get { return _DoType; }
            set { _DoType = value; }
        }
        /// <summary>
        /// Step
        /// </summary>
        public int Step
        {
            get { return _Step; }
            set { _Step = value; }
        }
        public UIElement Ele
        {
            get { return _Ele; }
            set { _Ele = value as UIElement; }
        }
        #endregion

        #region 单一实例
        public static readonly FuncStep instance = new FuncStep();
        #endregion

        #region 公共方法
        public List<FuncStep> GetEns()
        {
            List<FuncStep> ToolList = new List<FuncStep>()
            {
            };
            return ToolList;
        }
        #endregion
    }
}
