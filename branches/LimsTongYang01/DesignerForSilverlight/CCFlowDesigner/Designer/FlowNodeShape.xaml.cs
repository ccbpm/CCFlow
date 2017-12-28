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
using BP.Picture;
using Ccflow.Web.Component.Workflow;
using System.ComponentModel;
using BP;

namespace BP
{

    public partial class FlowNodeShape : IFlowNode, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        public event EventHandler<PropertyChangedEventArgs> PropertyChanging;
        protected virtual void RaisePropertyChanging(string prop)
        {
            RaisePropertyChanging(new PropertyChangedEventArgs(prop));
        }
        protected virtual void RaisePropertyChanging(PropertyChangedEventArgs e)
        {
            if (PropertyChanging != null)
                PropertyChanging(this, e);
        }

        #endregion

        public FlowNodeShape()
        {
            InitializeComponent();
        }

        public IContainer CurrentContainer { get; set; }
        public new TextBlock Name
        {
            get
            {
                return txtNodeName;
            }
            set
            {
                txtNodeName = value;
                if (value.Visibility == System.Windows.Visibility.Visible)
                {
                    tbNodeName.Visibility = Visibility.Collapsed;
                }
                else
                {
                    tbNodeName.Visibility = Visibility.Visible;
                    tbNodeName.Text = value.Text;
                    tbNodeName.Focus();
                }
            }
        }
        public string NodeName
        {
            get { return txtNodeName.Text; }
            set
            {
                RaisePropertyChanging(new PropertyChangedEventArgs("NodeName"));
                txtNodeName.Text = value;
                tbNodeName.Text = value;
            }
        }
        public double ContainerWidth
        {
            set
            {
                gridContainer.Width = value;
            }
            get
            {
                return gridContainer.Width;
            }
        }
        public double ContainerHeight
        {
            set
            {

                gridContainer.Height = value;
            }
            get
            {
                return gridContainer.Height;
            }
        }

        public double PictureWidth
        {
            get
            {
                return ((INodeShape)CurrentPic).PictureWidth;
            }
            set
            {
                ((INodeShape)CurrentPic).PictureWidth = value;
            }
        }
        public double PictureHeight
        {
            get
            {
                return ((INodeShape)CurrentPic).PictureHeight;
            }
            set
            {
                ((INodeShape)CurrentPic).PictureHeight = value;
            }
        }

        UserControl currentPic;
        public UserControl CurrentPic
        {
            get
            {
                this.picOrdinaryNode.PictureVisibility = Visibility.Collapsed;
                this.picFenLiuNode.PictureVisibility = Visibility.Collapsed;
                this.picHeLiuNode.PictureVisibility = Visibility.Collapsed;
                this.picFenHeLiuNode.PictureVisibility = Visibility.Collapsed;
                this.picSubThreadNode.PictureVisibility = Visibility.Collapsed;
                this.picVitualNode.PictureVisibility = Visibility.Collapsed;
                switch (this.nodeType)
                {
                    case FlowNodeType.Ordinary:
                        currentPic = picOrdinaryNode;
                        break;
                    case FlowNodeType.FL:
                        currentPic = picFenLiuNode;
                        break;
                    case FlowNodeType.HL:
                        currentPic = picHeLiuNode;
                        break;
                    case FlowNodeType.FHL:
                        currentPic = picFenHeLiuNode;
                        break;
                    case FlowNodeType.SubThread:
                        currentPic = picSubThreadNode;
                        break;
                    case FlowNodeType.VirtualStart:
                        picVitualNode.IsStartNode = true;
                        currentPic = picVitualNode;
                        break;
                    case FlowNodeType.VirtualEnd:
                        picVitualNode.IsStartNode = false;
                        currentPic = picVitualNode;
                        break;
                    default:
                        throw new Exception("errrrss");
                }
                ((INodeShape)currentPic).PictureVisibility = Visibility.Visible;
                return currentPic;
            }
            set { currentPic = value; }
        }
        public new Brush Background
        {
            set
            {
                ((INodeShape)CurrentPic).Background = value;
            }
            get
            {
                return ((INodeShape)CurrentPic).Background;
            }
        }
        FlowNodeType nodeType;
        public FlowNodeType NodeType
        {
            get
            {
                return nodeType;
            }
            set
            {
                nodeType = value;
            }
        }

        public void SetBorderColor(Brush brush)
        {
            ((INodeShape)CurrentPic).SetBorderColor(brush);
        }
     
        public Point GetPointOfIntersection(Point beginPoint, Point endPoint, DirectionMove type)
        {
            return new Point(0, 0);
        }
        public PointCollection ThisPointCollection
        {
            get
            {
                return ((INodeShape)CurrentPic).ThisPointCollection;
            }
        }
        MergePictureRepeatDirection _repeatDirection;
        public MergePictureRepeatDirection RepeatDirection
        {
            get
            {
              
                return _repeatDirection;
            }
            set
            {
                _repeatDirection = value;
              
            }
        }

        private void tbNodeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtNodeName.Text = tbNodeName.Text;
            CurrentContainer.IsNeedSave = true;
        }

        private void tbNodeName_LostFocus(object sender, RoutedEventArgs e)
        {
            tbNodeName.Visibility = Visibility.Collapsed;
            txtNodeName.Visibility = Visibility.Visible;

        }


        public Visibility PictureVisibility
        {
            set
            {
                this.Visibility = value;
            }
            get
            {
                return this.Visibility;
            }
        }
    }
}
