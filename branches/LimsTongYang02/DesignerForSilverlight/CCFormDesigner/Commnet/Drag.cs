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
    public static class Drag
    {
        // Global variables used to keep track of the 
        // mouse position and whether the object is captured
        // by the mouse.
        static Point pFrom = new Point(0,0);
        static bool isMouseCaptured;
      

        static UIElement _Element;
        public static void BindDrag(this UIElement element)
        {
            _Element = element;
            element.MouseLeftButtonDown += Handle_MouseDown;
            element.MouseLeftButtonUp += Handle_MouseUp;
            element.MouseMove += Handle_MouseMove;

        }
        public static void UnBindDrag(this UIElement element)
        {
            _Element = null;
            element.MouseLeftButtonDown -= Handle_MouseDown;
            element.MouseLeftButtonUp -= Handle_MouseUp; 
            element.MouseMove -= Handle_MouseMove;

        }

        public static void Handle_MouseDown(object sender, MouseEventArgs args)
        {
            if (Glo.IsDbClick)
            {
                isMouseCaptured = false;
                return;
            }

            FrameworkElement e = sender as FrameworkElement;
            _Element = e;
           
            Point p = args.GetPosition(e);
            isMouseCaptured = true;
            if (e is BPLine)
            {
            }
            else if (Adjust.MPosition == MousePosition.Drag)
            {
            }
            else if (p.X > 5 && p.X < e.ActualWidth - 5 && p.Y > 5 && p.Y < e.ActualHeight - 5)
            {
            }
            else
            {
                isMouseCaptured = false;
            }

            if(isMouseCaptured)
            {
                e.CaptureMouse();
                pFrom = args.GetPosition(null);
            }
        }

        public static void Handle_MouseMove(object sender, MouseEventArgs args)
        {
            FrameworkElement _UIElement = sender as FrameworkElement;
            if (_Element != null && _Element != _UIElement)
            {
                _Element.ReleaseMouseCapture();
                isMouseCaptured = false;
                return;
            }

            if (isMouseCaptured && (Adjust.MPosition == MousePosition.Drag|| _UIElement is BPLine))
            {
                Point pTo = args.GetPosition(null);

                // Calculate the current position of the object.
                double deltaV = pTo.Y - pFrom.Y;
                double deltaH = pTo.X - pFrom.X;

                if (_UIElement is BPLine)
                {
                    // Calculate the current position of the object.
                    (_UIElement as BPLine).UpdatePos(deltaH, deltaV);
                }
                else
                {
                    double newTop = deltaV + (double)_UIElement.GetValue(Canvas.TopProperty);
                    double newLeft = deltaH + (double)_UIElement.GetValue(Canvas.LeftProperty);

                    _UIElement.SetValue(Canvas.TopProperty, newTop);
                    _UIElement.SetValue(Canvas.LeftProperty, newLeft);
                }
                pFrom = pTo;
                Glo.ViewNeedSave = true;
            }
            else if (Adjust.MPosition == MousePosition.None)
            {
                Adjust.MPosition = MouseEventHandlers.MousePointPosition(_UIElement, args, false);
            }
        }

        public static void Handle_MouseUp(object sender, MouseEventArgs args)
        {
            if (isMouseCaptured)
            {
                _Element = null;

                UIElement _UIElement = sender as UIElement;
                if (_UIElement != null)
                    _UIElement.ReleaseMouseCapture();
                isMouseCaptured = false;
                pFrom = new Point(0, 0);
                Adjust.MPosition = MousePosition.None;
            }
        }
    }
}
