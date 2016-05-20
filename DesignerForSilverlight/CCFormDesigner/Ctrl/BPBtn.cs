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
using BP.En;

namespace CCForm
{
    public class BPBtn : System.Windows.Controls.Button, IElement, IRouteEvent
    {

        public MouseButtonEventHandler LeftDown { get; set; }
        public MouseButtonEventHandler LeftUp { get; set; }
        #region IElement
     
        public BtnType HisBtnType = BtnType.Self;
        public EventType HisEventType = EventType.Disable;
        private string _EventContext = "";
        public string EventContext
        {
            get
            {
                if (_EventContext == null)
                    _EventContext = "";
                return _EventContext;
            }
            set
            {
                _EventContext = value;
            }
        }
        private string _MsgErr = "";
        public string MsgErr
        {
            get
            {
                if (_MsgErr == null)
                    _MsgErr = "";
                return _MsgErr;
            }
            set
            {
                _MsgErr = value;
            }
        }

        private string _MsgOK = "";
        public string MsgOK
        {
            get
            {
                if (_MsgOK == null)
                    _MsgOK = "";
                return _MsgOK;
            }
            set
            {
                _MsgOK = value;
            }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                if (value )
                {
                    this.BorderThickness = new Thickness(0.5); ;
                    this.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    this.BorderThickness = new Thickness(0); ;
                    this.BorderBrush = new SolidColorBrush(Glo.BorderBrush);
                }
            }
        }
        public  bool IsCanReSize
        {
            get
            {
                return false;
            }
        }      
        public bool TrackingMouseMove { get; set; }
        public bool ViewDeleted
        {
            get;
            set;
        }
        #endregion 
        
        /// <summary>
        /// BPBtn
        /// </summary>
        public BPBtn()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
            this.Content = "Btn...";
            this.Foreground = new SolidColorBrush(Colors.Black);
            this.FontStyle = FontStyles.Normal;
            this.AllowDrop = false;
        }
         
        Point mousePosition;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            mousePosition = e.GetPosition(null);

            if (null != LeftDown)
                LeftDown(this, e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.TrackingMouseMove = false;
            if (null != LeftUp)
                LeftUp(this, e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            //FrameworkElement element = sender as FrameworkElement;
            if (TrackingMouseMove)
            {
                double moveH = e.GetPosition(null).Y - mousePosition.Y;
                double moveW = e.GetPosition(null).X - mousePosition.X;
                double newTop = moveH + (double)this.GetValue(Canvas.TopProperty);
                double newLeft = moveW + (double)this.GetValue(Canvas.LeftProperty);
                this.SetValue(Canvas.TopProperty, newTop);
                this.SetValue(Canvas.LeftProperty, newLeft);
                mousePosition = e.GetPosition(null);
                Glo.ViewNeedSave = true;
            }
            base.OnMouseMove(e);
        }

       
    }
}
