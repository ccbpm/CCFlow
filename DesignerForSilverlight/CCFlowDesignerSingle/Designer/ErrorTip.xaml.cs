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
using BP;

namespace BP
{
    public partial class ErrorTip : UserControl
    {

        public ErrorTip()
        {
            InitializeComponent();

        }
        IElement _parentElement;
        public IElement ParentElement
        {
            get
            {
                return _parentElement;
            }
            set
            {
                _parentElement = value;
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        public string ErrorMessage
        {
            set
            {
                tbMessage.Text = value;
            }
        }

        bool trackingMouseMove = false;
        Point mousePosition;
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {


            if (trackingMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;
                element.Cursor = Cursors.Hand;

                if (e.GetPosition(null) == mousePosition)
                    return;
                double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                double deltaH = e.GetPosition(null).X - mousePosition.X;
                double newTop = deltaV + (double)this.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)this.GetValue(Canvas.LeftProperty);


                this.SetValue(Canvas.TopProperty, newTop);
                this.SetValue(Canvas.LeftProperty, newLeft);

                mousePosition = e.GetPosition(null);




            }
        }
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ParentElement.UpperZIndex();
            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(null);
            if (null != element)
            {
                trackingMouseMove = true;
                this.CaptureMouse();
                this.Cursor = Cursors.Hand;
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            this.ReleaseMouseCapture();
            mousePosition.X = mousePosition.Y = 0;
            this.Cursor = null;
            e.Handled = true;


        }

        private void btnClose_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Opacity = 0.2;
        }

        private void btnClose_MouseEnter(object sender, MouseEventArgs e)
        {
            btnClose.Opacity = 1;

        }

    }
}
