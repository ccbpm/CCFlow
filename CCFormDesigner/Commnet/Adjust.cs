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

namespace CCForm
{
    /// <summary>
    /// 鼠标拖拽调整尺寸
    /// </summary>
    public class Adjust
    {
        FrameworkElement bp;
        Point _PrePoint;
        double _ButtonWidth = 0.0;
        bool isLeftDown ;
        public void Bind(FrameworkElement _FrameworkElement)
        {
            bp = _FrameworkElement;
            bp.SizeChanged += new SizeChangedEventHandler(_Control_SizeChanged);
            bp.MouseMove += new MouseEventHandler(_Control_MouseMove);
            bp.MouseLeftButtonDown += new MouseButtonEventHandler(_Control_MouseLeftButtonDown);
            bp.MouseLeftButtonUp += new MouseButtonEventHandler(_Control_MouseLeftButtonUp);
        }

      

        void _Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //allow min Height
            if (bp.Height < 6)
            {
                bp.Height = 6;
                bp.ReleaseMouseCapture();
            }

            //allow min Height
            if (bp.Width < 6)
            {
                bp.Width = 6;
                bp.ReleaseMouseCapture();
            }
            Glo.ViewNeedSave = true;
        }

        private void _Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (isLeftDown)
            {
                e.Handled = true;
                isLeftDown = false;
                Adjust.MPosition = MousePosition.None;
                bp.ReleaseMouseCapture();
            }
        }

        private void _Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
            //    return;

            if (MPosition != MousePosition.None && MPosition != MousePosition.Drag && bp.CaptureMouse())
            {
                isLeftDown = true;
                _ButtonWidth = bp.ActualWidth;
                _PrePoint = e.GetPosition(null);
            }
        }

        public static MousePosition MPosition = CCForm.MousePosition.None;
        private void _Control_MouseMove(object sender, MouseEventArgs e)
        {
           
            if (isLeftDown && MPosition != MousePosition.None && MPosition != MousePosition.Drag)
            {
                Point pFrom = e.GetPosition(null);
                double height = pFrom.Y - _PrePoint.Y;
                double width = pFrom.X - _PrePoint.X;
                move(height, width);
                _PrePoint = pFrom;
                
            }
            else
            {
                MPosition = MouseEventHandlers.MousePointPosition(bp, e, true);
            }
        }

        private void move(double yOffset, double xOffset)
        {
            switch (MPosition)
            {
                case MousePosition.SizeTop: // 向上拉伸，Y轴上移
                    Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);
                    bp.Height = bp.ActualHeight - yOffset;

                    break;
                case MousePosition.SizeBottom:
                    bp.Height = bp.ActualHeight + yOffset;
                    break;

                case MousePosition.SizeLeft: //向左拉伸，X轴左移
                    Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);
                    bp.Width = bp.ActualWidth - xOffset;

                    break;

                case MousePosition.SizeRight:
                    bp.Width = bp.ActualWidth + xOffset;
                    break;

                case MousePosition.SizeTopLeft:
                    bp.Width = bp.ActualWidth - xOffset;
                    Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);

                    bp.Height = bp.ActualHeight - yOffset;
                    Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);

                    break;

                case MousePosition.SizeBottomRight:
                    bp.Width = bp.ActualWidth + xOffset;
                    bp.Height = bp.ActualHeight + yOffset;

                    break;
                case MousePosition.SizeBottomLeft:
                    Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);
                    bp.Width = bp.ActualWidth - xOffset;
                    bp.Height = bp.ActualHeight + yOffset;

                    break;

                case MousePosition.SizeTopRight:
                    bp.Width = bp.ActualWidth + xOffset;
                    Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);
                    bp.Height = bp.ActualHeight - yOffset;

                    break;
                            
            }
        }
    }
   
}
