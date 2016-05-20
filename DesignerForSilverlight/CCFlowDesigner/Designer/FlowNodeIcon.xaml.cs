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
using System.ComponentModel;
using BP.Picture;

namespace BP
{
    public partial class FlowNodeIcon : IFlowNode, INotifyPropertyChanged
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

        public IContainer CurrentContainer { get; set; }

        public FlowNodeIcon()
        {
            InitializeComponent();
        }
        public new TextBlock Name
        {
            get
            {
                return txtNodeName;
            }
            set
            {
                txtNodeName = value ;
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
        public new double Opacity
        {
            set;
            get;
        }
        public  double PictureWidth
        {
            set
            {
                bgBorder.Width = value;
            }
            get
            {
                return bgBorder.Width; 
            }
        }
        public  double PictureHeight
        {
            set
            {
                bgBorder.Height = value;
            }
            get 
            { 
                return bgBorder.Height; 
            }
        }
        public new Brush Background
        {
            set;
            get;
        }
        public  Visibility PictureVisibility
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
     

      
        public void SetBorderColor(Brush brush)
        {
        }

        PointCollection _thisPointCollection;
        public PointCollection ThisPointCollection
        {
            get
            {
                return _thisPointCollection; 
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
    }
}
