#region
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Windows.Interop;
using System.Windows.Browser;
using System.Collections.ObjectModel;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;

using System.IO.IsolatedStorage;
using System.Windows.Controls.Primitives;
//using SilverBullet.Util;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;


#endregion

namespace BP
{
    public partial class ToolBox : UserControl
    {
        private double left = 10, top = 35;
        public bool isOpened ;
        public void SetVisiable(bool flag)
        {
            isOpened = flag;
            this.LayoutRoot.IsOpen = flag;
        }
      
        /// <summary>
        /// 在本对象 down时 =true,
        /// 自身、工具箱容器和设计器up时       =false,
        /// 自身、工具箱容器LostFocu时未down则 =false 
        /// </summary>
        public static bool isToolDraging;

        public ToolBox()
        {
            InitializeComponent();
            this.SetVisiable(false);
            this.close.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                this.SetVisiable(false);
            };

            this.label.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.line.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.lineReturn.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.nodeOrdinary.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.nodeHL.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.nodeFL.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.nodeFHL.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);
            this.subThread.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseLeftButtonDown);


            this.label.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.line.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.lineReturn.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.nodeOrdinary.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.nodeHL.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.nodeFL.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.nodeFHL.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);
            this.subThread.MouseLeftButtonUp += new MouseButtonEventHandler(Border_MouseLeftButtonUp);


            //this.label.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.line.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.lineReturn.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.nodeOrdinary.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.nodeHL.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.nodeFL.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.nodeFHL.MouseMove += new MouseEventHandler(Border_MouseMove);
            //this.subThread.MouseMove += new MouseEventHandler(Border_MouseMove);


            this.label.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.line.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.lineReturn.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.nodeOrdinary.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.nodeHL.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.nodeFL.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.nodeFHL.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);
            this.subThread.MouseEnter += new MouseEventHandler(Toolbar_OnMouseEnter);

            this.label.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.line.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.lineReturn.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.nodeOrdinary.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.nodeHL.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.nodeFL.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.nodeFHL.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);
            this.subThread.MouseLeave += new MouseEventHandler(Toolbar_OnMouseLeave);

        }

       
        #region 拖动 缩放
       
        void toolBoxLayer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Point p = e.GetPosition(null);

            //myScaleTransform.CenterX = p.X;
            //myScaleTransform.CenterY = p.Y;

            //if (e.Delta > 0)
            //{
            //    if ( (int)myScaleTransform.ScaleX >= 2)
            //    {
            //        return;
            //    }
            //    myScaleTransform.ScaleX += 0.05 ;
            //    myScaleTransform.ScaleY += 0.05 ;
            //}
            //else
            //{
            //    if ((int)myScaleTransform.ScaleX <= 0.5)
            //    {
            //        return;
            //    }
            //    myScaleTransform.ScaleX -= 0.05 ;
            //    myScaleTransform.ScaleY -= 0.05 ;
            //}

            e.Handled = true;
        }
       
        public bool isToolBoxCanDrag ;
        Point pointFrom;
      
        void gridTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isToolBoxCanDrag)
            {
                FrameworkElement element = this as FrameworkElement;
             
                double
                    x = Canvas.GetLeft(element),
                    y = Canvas.GetTop(element);

                Point pointTo = e.GetPosition(null);
            
                x += pointTo.X - pointFrom.X; //xOffset;
                y += pointTo.Y - pointFrom.Y;//yOffset; 

                Canvas container = this.Parent as Canvas;
                if ((this.left < x && this.top < y)
                   && (x < container.ActualWidth - this.ActualWidth - this.left && y < container.ActualHeight - this.ActualHeight - 110 - this.top)
                )
                {
                    Canvas.SetTop(element, y);
                    Canvas.SetLeft(element, x);
                }
                else
                {
                    element.Cursor = Cursors.IBeam ;
                }
             
                pointFrom = pointTo;

            }
        }

        void gridTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isToolBoxCanDrag = false;
            (sender as FrameworkElement).ReleaseMouseCapture();
        }
        void gridTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            {
                isToolBoxCanDrag = true;
                (sender as FrameworkElement).CaptureMouse();
                pointFrom = e.GetPosition(null);
            }
        }
        void Toolbar_OnMouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            Border b;
            if (typeof(Border).IsInstanceOfType(element))
            {
                b = element as Border;
            }
            else
            {
                b = element.Parent as Border;
            }

            if (b != null)
            {
                b.BorderBrush = new SolidColorBrush(ColorConverter.ToColor("#FFABCEFB"));
                b.Background = new SolidColorBrush(ColorConverter.ToColor("#FFD5E2F2"));//#FFFFE1
            }
        }
        void Toolbar_OnMouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Cursor = null;

            Border b;
            if (typeof(Border).IsInstanceOfType(element))
            {
                b = element as Border;
            }
            else
            {
                b = element.Parent as Border;
            }

            if (b != null)
            {
                b.BorderBrush = new SolidColorBrush(Colors.Transparent);
                b.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        #endregion


      
        // 取消默认右键
        private void toolBoxLayer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
     
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isToolDraging = true;
            Border border = sender as Border;

            if (Glo.IsDbClick)
            {
                SetDefault();
                MainPage.Instance.AddElement( border.Name);
            }
            else
            {// ToolboxItem 拖拽元素
                MainPage.NewElementNameOrIcon = border.Name;
            }
        }
        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetDefault();
        }

        public static void SetDefault()
        {
            ToolBox.isToolDraging = false;
            MainPage.NewElementNameOrIcon = string.Empty;
            MainPage.Instance.SetSelectedTool("Arrow");
        }
 }
}
