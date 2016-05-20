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
using Ccflow.Web.Component.Workflow;

namespace BP
{ 

    public partial class DirectionTurnPoint : UserControl
    {
        public delegate void DirectionTurnPointMoveDelegate(object sender, MouseEventArgs e, Point newPoint);

        public  delegate void DoubleClickDelegate(object sender, EventArgs e);

        public event DirectionTurnPointMoveDelegate DirectionTurnPointMove;
        public DirectionTurnPoint()
        {
            InitializeComponent();
           
        }
     
        public  Brush Fill
        {
            get
            {
                return eliTurnPoint.Fill;
            }
            set
            {
                eliTurnPoint.Fill = value;
            }
        }
        //public void ShowDisplayAutomation()
        //{
        //    sbDisplay.Begin();
        //}
        //public void ShowCloseAutomation()
        //{
        //    sbColse.Begin();
        //}

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {

        }
        Point mousePosition;
        bool trackingMouseMove = false;
        public event DoubleClickDelegate OnDoubleClick;
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (Glo.IsDbClick)
            {
                if (OnDoubleClick != null)
                    OnDoubleClick(this, e);
            }
            else
            {
                FrameworkElement element = sender as FrameworkElement;
                mousePosition = e.GetPosition(null);
                trackingMouseMove = true;
                if (null != element)
                {
                    element.CaptureMouse();
                    element.Cursor = Cursors.Hand;
                }
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();
            mousePosition = e.GetPosition(null);
            element.Cursor = null;
             
        }
        public double Radius
        {
            get
            {
                return eliTurnPoint.Width / 2;
            }
        }
        public Point CenterPosition {
            get
            {
                return new Point((double)this.GetValue(Canvas.LeftProperty) + Radius, (double)this.GetValue(Canvas.TopProperty) + Radius);
            }
            set
            {
                this.SetValue(Canvas.LeftProperty, value.X - Radius);
                this.SetValue(Canvas.TopProperty, value.Y - Radius);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (trackingMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;
                element.Cursor = Cursors.Hand;

                if (e.GetPosition(null) == mousePosition)
                    return;
                double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                double deltaH = e.GetPosition(null).X - mousePosition.X;
                double newTop = deltaV + CenterPosition.Y;
                double newLeft = deltaH + CenterPosition.X;

                Point p  =  new Point(newLeft, newTop);
                CenterPosition = p;

                if (DirectionTurnPointMove != null)
                {
                    DirectionTurnPointMove(sender, e,p);
                }
                mousePosition = e.GetPosition(null);

            }
        }
    }
}
