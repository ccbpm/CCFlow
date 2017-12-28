using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CCForm
{
    public class BPCheckBox : System.Windows.Controls.CheckBox, IElement, IRouteEvent, IDelete
    {
        #region 
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
       
        #endregion 
        public MouseButtonEventHandler LeftDown { get; set; }
        public MouseButtonEventHandler LeftUp { get; set; }
        public string KeyName = null;
        public string UIBindKey = null;
        public string AtPara { get; set; }
        /// <summary>
        /// BPCheckBox
        /// </summary>
        public BPCheckBox()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);

            this.BindDrag();
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

        public bool TrackingMouseMove { get; set; }
        #region 移动事件
        Point pointFrom;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //base.OnMouseLeftButtonDown(e);

            if (this.CaptureMouse())
            {
                pointFrom = e.GetPosition(null);
            }
            this.TrackingMouseMove = true;
            if (null != this.LeftDown)
                this.LeftDown(this, e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //base.OnMouseLeftButtonUp(e);
            this.ReleaseMouseCapture();
            this.TrackingMouseMove = false;

            if (null != this.LeftUp)
                this.LeftUp(this, e);
        }

        MousePosition MPPosition;
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!TrackingMouseMove)
            {
                MPPosition = MouseEventHandlers.MousePointPosition(this, e, this.IsCanReSize);
            }
            else
            {
                double moveH = e.GetPosition(null).Y - pointFrom.Y;
                double moveW = e.GetPosition(null).X - pointFrom.X;
                double newTop = moveH + (double)this.GetValue(Canvas.TopProperty);
                double newLeft = moveW + (double)this.GetValue(Canvas.LeftProperty);
                this.SetValue(Canvas.TopProperty, newTop);
                this.SetValue(Canvas.LeftProperty, newLeft);
                pointFrom = e.GetPosition(null);
                Glo.ViewNeedSave = true;
            }
            base.OnMouseMove(e);
        }
        #endregion 移动事件

        public void DeleteIt()
        {
            if (this.Name != null)
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                string sqls = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + Glo.FK_MapData + "' AND KeyOfEn='" + this.Name + "'";
                da.RunSQLAsync(sqls, Glo.UserNo, Glo.SID);
            }
            Glo.Remove(this);
        }

        public void HidIt()
        {
            string sql = "UPDATE Sys_MapAttr SET UIVisible=0 WHERE KeyOfEn='" + this.Name + "' AND FK_MapData='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
            hidDA.RunSQLCompleted += (object sender, FF.RunSQLCompletedEventArgs e)=>
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            };
        }

        public bool ViewDeleted
        {
            get;
            set;
        }
    }
}
