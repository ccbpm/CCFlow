using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CCForm
{
    public class BPRadioBtn : System.Windows.Controls.RadioButton, IElement, IRouteEvent, IDelete
    {
        public MouseButtonEventHandler LeftDown { get; set; }
        public MouseButtonEventHandler LeftUp { get; set; }
        #region IElement.
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
                    this.BorderThickness = new Thickness(0.5);
                    this.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    this.BorderThickness = new Thickness(0.5);
                    this.BorderBrush = new SolidColorBrush(Glo.BorderBrush);
                }
            }
        }
        public bool IsCanReSize
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

        public string KeyName = null;
        public string UIBindKey = null;
        protected override void OnClick()
        {
            base.OnClick();
        }
       
        public string FK_MapData = null;
        /// <summary>
        /// BPRadioButton
        /// </summary>
        public BPRadioBtn()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
        }

        #region 焦点事件
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            this.BorderBrush.Opacity = 4;
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.BorderBrush.Opacity = 0.5;
            base.OnLostFocus(e);
        }
        #endregion 焦点事件
        #region 移动事件
        Point mousePosition;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            #region 把相同分组的按钮设置成一组颜色
            Canvas c = this.Parent as Canvas;
            foreach (UIElement ctl in c.Children)
            {
                if (ctl is BPRadioBtn)
                {
                    BPRadioBtn rb = ctl as BPRadioBtn;
                    if (rb != null)
                    {
                        if (rb.GroupName == this.GroupName)
                            rb.Foreground = new SolidColorBrush(Colors.Blue);
                        else
                            rb.Foreground = new SolidColorBrush(Colors.Black);
                    }
                }
            }
            #endregion 

            
            if (this.CaptureMouse())
            {
                mousePosition = e.GetPosition(null);
            }
            this.TrackingMouseMove = true;
            base.OnMouseLeftButtonDown(e);

            if (null != LeftDown)
                LeftDown(this, e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.ReleaseMouseCapture();
            this.TrackingMouseMove = false;

            if (null != LeftUp)
                LeftUp(this, e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (TrackingMouseMove)
            {
                double moveH = e.GetPosition(null).Y - mousePosition.Y;
                double moveW = e.GetPosition(null).X - mousePosition.X;
                double newTop = moveH + (double)this.GetValue(Canvas.TopProperty);
                double newLeft = moveW + (double)this.GetValue(Canvas.LeftProperty);
                this.SetValue(Canvas.TopProperty, newTop);
                this.SetValue(Canvas.LeftProperty, newLeft);
                mousePosition = e.GetPosition(null);
            }
            base.OnMouseMove(e);
        }
        public void DeleteIt()
        {
            Canvas c = this.Parent as Canvas;
            string ids = "";
            foreach (UIElement uiCtl in c.Children)
            {
                Control ctl = uiCtl as Control;
                if (ctl == null)
                    continue;

                if (ctl.Name.Contains(this.GroupName))
                    ids += "@" + ctl.Name;
            }

            string[] strs = ids.Split('@');
            foreach (string str in strs)
            {
                foreach (UIElement uiCtl in c.Children)
                {
                    Control ctl = uiCtl as Control;
                    if (ctl == null)
                        continue;

                    if (ctl.Name == str)
                    {
                        c.Children.Remove(ctl);
                        BPRadioBtn eleBtn = ctl as BPRadioBtn;
                        eleBtn.ViewDeleted = true;
                        break;
                    }
                }
            }
            Glo.Remove(this);
            Glo.currEle = null;
         
        }
        #endregion 移动事件
      
    }
}
