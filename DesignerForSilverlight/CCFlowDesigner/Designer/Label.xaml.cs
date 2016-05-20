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
using Silverlight;
using BP;

namespace BP
{
    public partial class NodeLabel : IElement
    {
        #region IElement 成员

        public UserStation Station()
        {
            throw new NotImplementedException();
        }



        public void Worklist(DataSet dataSet)
        {
        }

        #endregion
        public CheckResult CheckSave()
        {
            CheckResult cr = new CheckResult();
            cr.IsPass = true;
            return cr;
        }

        public string LableID { get; set; }
        

        public void ResetPosition(double x, double y)
        {
            Point p = new Point();
            p.X = (double) this.GetValue(Canvas.LeftProperty);
            p.Y = (double) this.GetValue(Canvas.TopProperty);

            this.SetValue(Canvas.TopProperty, p.Y + y);
            this.SetValue(Canvas.LeftProperty, p.X + x);
        }

        public NodeLabel Clone()
        {
            NodeLabel l = new NodeLabel(_container);
            l.LabelName = this.LabelName;
            l.Position = this.Position;
            return l;
        }

        public WorkFlowElementType ElementType
        {
            get { return WorkFlowElementType.Label; }
        }

        private PageEditType editType = PageEditType.None;

        public PageEditType EditType
        {
            get { return editType; }
            set { editType = value; }
        }

        public NodeLabel()
        {
        }

        public NodeLabel(IContainer container)
        {
            _container = container;
             System.Windows.Browser.HtmlPage.Document.AttachEvent("oncontextmenu", OnContextMenu);

            InitializeComponent();
        }

        private void OnContextMenu(object sender, System.Windows.Browser.HtmlEventArgs e)
        {
            //if (_container.MouseIsInContainer)
            //{
            //    e.PreventDefault();

            //    if (canShowMenu && !IsDeleted)
            //    {

            //        _container.ShowLabelContentMenu(this, sender, e);
            //    }
            //}
        }

        
        public string LabelName
        {
            get { return txtLabel.Text; }
            set
            {
                txtEdit.Text = value;
                txtLabel.Text = value;
            }
        }

        private bool canShowMenu = false;
        private bool isSelectd = false;

       
        public bool IsSelectd
        {
            get { return isSelectd; }
            set
            {
                isSelectd = value;
                if (isSelectd)
                {
                    this.txtBorder.BorderThickness = new Thickness(1);
                    if (!_container.SelectedWFElements.Contains(this))
                        _container.AddSelectedControl(this);
                }
                else
                {
                    this.txtBorder.BorderThickness = new Thickness(0);
                    txtEdit_LostFocus(this.txtEdit,null);
                }
            }
        }

        public bool IsDeleted
        {
            get { return isDeleted; }
        }

        private bool isDeleted = false;

        public void Delete()
        {
            if (!isDeleted)
            {
                isDeleted = true;
                canShowMenu = false;
                sbClose.Completed += (object sender, EventArgs e)=>
                {
                    if (isDeleted)
                    {
                        this.Visibility = Visibility.Collapsed;
                        _container.RemoveUI(this);
                    }
                };
                sbClose.Begin();
            }
        }


        private Point origPosition;
        private bool positionIsChange = true;

        public void Zoom(double zoomDeep)
        {
            if (positionIsChange)
            {
                origPosition = this.Position;
                positionIsChange = false;
            }
            this.Position = new Point(origPosition.X*zoomDeep, origPosition.Y*zoomDeep);
        }

        public int ZIndex
        {
            get { return (int) this.GetValue(Canvas.ZIndexProperty); }
            set { this.SetValue(Canvas.ZIndexProperty, value); }
        }

        public void UpperZIndex()
        {
            ZIndex = _container.NextMaxIndex;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection != null)
            {
                e.Handled = false;
            }
            else
                e.Handled = true;

            canShowMenu = true;
            if (!hadActualMove && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SelectedWorkFlowElement(this, IsSelectd);
            }
            if (hadActualMove)
                _container.SaveChange(HistoryType.New);
            FrameworkElement element = sender as FrameworkElement;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();

            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;
        }

        private IContainer _container;

        public IContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }

        private bool hadActualMove = false;
        private Point mousePosition;
        private bool trackingMouseMove = false;

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            hadActualMove = false;
            if (Glo.IsDbClick)
            {
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID") 
                    ||System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
                {
                    return;
                }
                txtEdit.Visibility = Visibility.Visible;
                txtLabel.Visibility = Visibility.Collapsed;
                Container.IsSomeChildEditing = true;
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

        public Point Position
        {
            get
            {
                Point position;

                position = new Point();
                position.Y = (double) this.GetValue(Canvas.TopProperty);
                position.X = (double) this.GetValue(Canvas.LeftProperty);
                return position;
            }
            set
            {
                this.SetValue(Canvas.TopProperty, value.Y);
                this.SetValue(Canvas.LeftProperty, value.X);
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;
                element.Cursor = Cursors.Hand;

                if (e.GetPosition(null) == mousePosition)
                    return;
                hadActualMove = true;
                double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                double deltaH = e.GetPosition(null).X - mousePosition.X;
                double newTop = deltaV + Position.Y;
                double newLeft = deltaH + Position.X;


                double containerWidth = (double) this.Parent.GetValue(Canvas.WidthProperty);
                double containerHeight = (double) this.Parent.GetValue(Canvas.HeightProperty);
                if (false)
                {
                    //超过流程容器的范围
                }
                
                else
                {
                    positionIsChange = true;
                    this.SetValue(Canvas.TopProperty, newTop);
                    this.SetValue(Canvas.LeftProperty, newLeft);

                    //Move(this, e);
                    mousePosition = e.GetPosition(null);
                    _container.IsNeedSave = true;
                    _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);
                }
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            canShowMenu = false;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            canShowMenu = true;
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_container.MouseIsInContainer)
            {
                e.GetPosition(this);

                if (canShowMenu && !IsDeleted)
                {
                    _container.ShowContentMenu(this, sender, e);
                }
            }
        }

        private void txtEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            txtLabel.Visibility = Visibility.Visible;
            txtEdit.Visibility = Visibility.Collapsed;
            Container.IsSomeChildEditing = false;
        }
        private void txtEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtLabel.Text = txtEdit.Text;
        }
        public void Edit()
        {
            _container.UpdateSelectedToMemory(this);
        }

       
    }
}