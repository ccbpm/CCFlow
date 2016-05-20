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

namespace BP
{
    public class BaseSetup
    {
        #region 起点相关
        
        /// <summary>
        /// 起点颜色
        /// </summary>
        public static Brush StartColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 0, 80, 0);
                return brush;
            }
        }
        /// <summary>
        /// 起点直径
        /// </summary>
        public static double StartDia
        {
            get { return 50.0; }
        }
        /// <summary>
        /// 起点不透明度
        /// </summary>
        public static double StartOpacity
        {
            get { return 1.0; }
        }

        #endregion

        #region 规则拖拽点相关

        /// <summary>
        /// 拖拽点颜色
        /// </summary>
        public static Brush RuleDragColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 160, 160, 160);
                return brush;
            }
        }
        /// <summary>
        /// 拖拽点直径
        /// </summary>
        public static double RuleDragDia
        {
            get { return 20.0; }
        }
        /// <summary>
        /// 拖拽点不透明度
        /// </summary>
        public static double RuleDragOpacity
        {
            get { return 0.8; }
        }
        /// <summary>
        /// 拖拽线的颜色
        /// </summary>
        public static Brush RuleDragThicknessColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 160, 160, 160);
                return brush;
            }
        }
        /// <summary>
        /// 拖拽线的宽度
        /// </summary>
        public static double RuleDragThickness
        {
            get { return 5.0; }
        }
        /// <summary>
        /// 拖拽线的不透明度
        /// </summary>
        public static double RuleDragThicknessOpacity
        {
            get { return 0.8; }
        }

        #endregion

        #region 活动节点相关

        /// <summary>
        /// 活动节点背景颜色
        /// </summary>
        public static Brush ActivityBackgroundColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 255, 255, 255);
                return brush;
            }
        }
        /// <summary>
        /// 活动节点边框颜色
        /// </summary>
        public static Brush ActivityStrokeColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 0, 80, 0);
                return brush;
            }
        }
        /// <summary>
        /// 活动节点边框宽度
        /// </summary>
        public static double ActivityStrokeThickness
        {
            get { return 2.0; }
        }
        /// <summary>
        /// 活动节点宽度
        /// </summary>
        public static double ActivityWidth
        {
            get { return 120.0; }
        }
        /// <summary>
        /// 活动节点高度
        /// </summary>
        public static double ActivityHeight
        {
            get { return 60.0; }
        }
        /// <summary>
        /// 活动节点圆角X轴半径
        /// </summary>
        public static double ActivityRadiusX
        {
            get { return 10.0; }
        }
        /// <summary>
        /// 活动节点圆角Y轴半径
        /// </summary>
        public static double ActivityRadiusY
        {
            get { return 10.0; }
        }
        /// <summary>
        /// 活动节点不透明度
        /// </summary>
        public static double ActivityOpacity
        {
            get { return 1.0; }
        }

        /// <summary>
        /// 标题颜色
        /// </summary>
        public static Brush ActivityTitleColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 0, 0, 255);
                return brush;
            }
        }

        /// <summary>
        /// 标题字体大小
        /// </summary>
        public static double ActivityTitleSize
        {
            get { return 11.0; }
        }

        #endregion

        #region 路由线相关

        /// <summary>
        /// 规则线颜色
        /// </summary>
        public static Brush RuleColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 0, 80, 0);
                return brush;
            }
        }

        /// <summary>
        /// 规则线条不透明度
        /// </summary>
        public static double RuleOpacity
        {
            get { return 0.8; }
        }

        /// <summary>
        /// 规则线条宽度
        /// </summary>
        public static double RuleThickness
        {
            get { return 8.0; }
        }

        /// <summary>
        /// 箭头边长
        /// </summary>
        public static double RuleArrowPar
        {
            get { return 20.0; }
        }

        /// <summary>
        /// 标题颜色
        /// </summary>
        public static Brush RuleTitleColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 180, 40, 0);
                return brush;
            }
        }

        /// <summary>
        /// 标题字体大小
        /// </summary>
        public static double RuleTitleSize
        {
            get { return 11.0; }
        }

        /// <summary>
        /// 标题在路由线上的剧中比率，1为居中
        /// </summary>
        public static double RuleTitleRatio
        {
            get { return 0.7; }
        }

        #endregion

        #region 结束点相关

        /// <summary>
        /// 结束点颜色
        /// </summary>
        public static Brush EndColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 255, 0, 0);
                return brush;
            }
        }
        /// <summary>
        /// 结束点直径
        /// </summary>
        public static double EndDia
        {
            get { return 50.0; }
        }
        /// <summary>
        /// 结束点不透明度
        /// </summary>
        public static double EndOpacity
        {
            get { return 1.0; }
        }

        #endregion

        #region 箭头相关

        /// <summary>
        /// 箭头颜色
        /// </summary>
        public static Brush ArrowColor
        {
            get
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Color.FromArgb(255, 160, 160, 160);
                return brush;
            }
        }
        /// <summary>
        /// 箭头边长
        /// </summary>
        public static double ArrowPar
        {
            get { return 20.0; }
        }
        /// <summary>
        /// 箭头不透明度
        /// </summary>
        public static double ArrowOpacity
        {
            get { return 0.8; }
        }

        #endregion

        #region 往返箭头相关

        /// <summary>
        /// 往返箭头之间的半径
        /// </summary>
        public static double BidRad
        {
            get { return 10.0; }
        }

        #endregion

        /// <summary>
        /// 警告颜色
        /// </summary>
        public static Brush WarningColor
        {
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 232, 112, 2));
            }
        }

        public static Brush DefaultSelectColor
        { 
            get
            {
                return new SolidColorBrush(Color.FromArgb(255, 160, 171, 193));
            }
        }
    }
}
