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

namespace BP.Picture
{
    public partial class VirtualNode : UserControl, INodeShape
    {
        public VirtualNode()
        {
            InitializeComponent();
        }
        public new double Opacity
        {
            set { PicRect.Opacity = value; }
            get { return PicRect.Opacity; }
        }
        public  double PictureWidth
        {
            set { picRect.Width = value; }
            get { return picRect.Width; }
        }
        public  double PictureHeight
        {
            set { PicRect.Height = value; }
            get { return PicRect.Height; }
        }
        public new Brush Background
        {
            set { PicRect.Fill = value; }
            get { return PicRect.Fill; }
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

        public bool IsStartNode { get; set; }

        public Ellipse PicRect
        {
            get { return IsStartNode ? picRect : picRect1; }
        }
       
        public void SetBorderColor(Brush brush)
        {
            PicRect.Stroke = brush;
        }
        public PointCollection ThisPointCollection
        {
            get { return null; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
