using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CCForm
{
  
    public interface IElement 
    {
        //bool isCopy = false;
        bool IsCanReSize { get;  }
        //bool ViewAdded { get; set; }
        //bool ViewChanged { get; set; }
        bool ViewDeleted { get; set; }
        bool IsSelected { get; set; }
        bool TrackingMouseMove { get; set; }
    }
    /// <summary>
    /// 事件需要由元素触发容器事件
    /// </summary>
    interface IRouteEvent 
    {
        MouseButtonEventHandler LeftDown { get; set; }
        MouseButtonEventHandler LeftUp { get; set; }
    }
    interface IDelete
    {
        void DeleteIt();
    }
    public abstract class LabelExt : System.Windows.Controls.Label, IElement
    {
        protected bool _IsSelected;
        public virtual bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;

                if (value)
                {
                    this.BorderThickness = new Thickness(2);
                    this.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    this.BorderThickness = new Thickness(2);
                    this.BorderBrush = new SolidColorBrush(Glo.BorderBrush);
                }
            }
        }

        protected bool trackingMouseMove;
        public bool TrackingMouseMove
        {
            get { return trackingMouseMove; }
            set { trackingMouseMove = value; }
        }
        public bool ViewDeleted
        {
            get;
            set;
        }
        public virtual bool IsCanReSize { get; set; }
      
    }


    public abstract class TextBoxExt : System.Windows.Controls.TextBox, IElement, IRouteEvent
    {
        public MouseButtonEventHandler LeftDown { get; set; }
        public MouseButtonEventHandler LeftUp { get; set; }

        protected bool _IsSelected;
        public virtual bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;

                if (value)
                {
                    this.BorderThickness = new Thickness(1);
                    this.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    this.BorderThickness = new Thickness(0.5);
                    this.BorderBrush = new SolidColorBrush(Glo.BorderBrush);
                }
            }
        }
        public bool TrackingMouseMove { get; set; }
        public bool ViewDeleted    {   get; set;  }

        public string KeyName = null;
        public double X = 0;
        public double Y = 0;
        public virtual bool IsCanReSize
        {
            get
            {
                return true;
            }
        }
        public virtual bool IsCanDel
        {
            get
            {
                return true;
            }
        }
        public double MoveStep
        {
            get
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                    return 1;
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    return 2;
                return 0;
            }
        }

        public void DeleteIt()
        {
            if (!IsCanDel)
            {
                MessageBox.Show("该字段[" + this.Name + "]不可删除!", "提示", MessageBoxButton.OK);
                return;
            }

            if (!string.IsNullOrEmpty(this.Name))
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                string sqls = "DELETE FROM Sys_MapAttr WHERE FK_MapData='{0}' AND KeyOfEn='{1}'";
                sqls = string.Format(sqls, Glo.FK_MapData, this.Name.Trim());
                da.RunSQLAsync(sqls, Glo.UserNo, Glo.SID);
            }

            Glo.Remove(this);
        }


        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            this.Text = "按住鼠标左键拖动位置,鼠标拖动边框改变大小.";
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Text = "";
        }

        Point pointFrom;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //base.OnMouseLeftButtonDown(e);

            if (this.CaptureMouse())
            {
                this.TrackingMouseMove = true;
                pointFrom = e.GetPosition(null);
            }

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
            try
            {
                Point pointTo = e.GetPosition(null);

                if (!TrackingMouseMove)
                {
                    MPPosition = MouseEventHandlers.MousePointPosition(this, e, this.IsCanReSize);
                }
                else
                {
                    Glo.ViewNeedSave = true;
                    #region
                    if (MPPosition != MousePosition.None)
                    {
                        double xOffset = pointTo.X - pointFrom.X;
                        double yOffset = pointTo.Y - pointFrom.Y;
                        if (MPPosition == MousePosition.Drag)
                        {
                            Canvas.SetTop(this, Canvas.GetTop(this) + yOffset);
                            Canvas.SetLeft(this, Canvas.GetLeft(this) + xOffset);
                        }
                        if (this.IsCanReSize)
                        {
                            if (this.Height < 20)
                            {
                                this.Height = 20;
                                TrackingMouseMove = false;
                                return;
                            }
                            else if (this.Width < 40)
                            {
                                this.Width = 40;
                                TrackingMouseMove = false;
                                return;
                            }

                            switch (MPPosition)
                            {
                                case MousePosition.SizeTop: // 向上拉伸，Y轴上移
                                    Canvas.SetTop(this, Canvas.GetTop(this) + yOffset);
                                    this.Height -= yOffset;

                                    break;
                                case MousePosition.SizeBottom:
                                    this.Height += yOffset;
                                    break;

                                case MousePosition.SizeLeft: //向左拉伸，X轴左移
                                    Canvas.SetLeft(this, Canvas.GetLeft(this) + xOffset);
                                    this.Width -= xOffset;

                                    break;

                                case MousePosition.SizeRight:
                                    this.Width += xOffset;
                                    break;

                                case MousePosition.SizeTopLeft:
                                    this.Width -= xOffset;
                                    Canvas.SetLeft(this, Canvas.GetLeft(this) + xOffset);

                                    this.Height -= yOffset;
                                    Canvas.SetTop(this, Canvas.GetTop(this) + yOffset);

                                    break;

                                case MousePosition.SizeBottomRight:
                                    this.Width += xOffset;
                                    this.Height += yOffset;

                                    break;
                                case MousePosition.SizeBottomLeft:
                                    Canvas.SetLeft(this, Canvas.GetLeft(this) + xOffset);
                                    this.Width -= xOffset;
                                    this.Height += yOffset;

                                    break;

                                case MousePosition.SizeTopRight:
                                    this.Width += xOffset;
                                    Canvas.SetTop(this, Canvas.GetTop(this) + yOffset);
                                    this.Height -= yOffset;

                                    break;
                            }
                        }
                    }
                    #endregion
                }

                pointFrom = pointTo;
            }
            catch (System.Exception ex)
            {
            }
        }

    }
}
