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

namespace CCForm
{
    public delegate void LineMoved(BPLine line);

    public partial class BPLine : IElement
    {
        #region Properties
     
        public bool PositionChanged = false;
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
                if (value)
                {
                    this.MyLine.Stroke = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    this.MyLine.Stroke = new SolidColorBrush(Glo.ToColor(this.Color));
                }
            }
        }

        public bool TrackingMouseMove { get; set; }
        public bool ViewDeleted   {  get; set;   }
        public bool IsCanReSize
        {
            get
            {
                return false;
            }
        }  
        #endregion 

        public event LineMoved Moved;
        public string FK_MapData = null;

        double borderW = 10;
        string color;

        public string Color
        {
            get
            {
                return color;
            }
            set
            {
                if (color != value)
                {
                    color = value;
                    if (string.IsNullOrEmpty(color))
                        color = Glo.PreaseColorToName(Colors.Black.ToString());

                    this.MyLine.Stroke = new SolidColorBrush(Glo.ToColor(color));
                }
            }
        }
      
        public BPLine()
        {
            InitializeComponent();
            this.BindDrag();
            this.MyLine.SizeChanged += MyLine_SizeChanged;
            this.SizeChanged += MyLine_SizeChanged;
            this.Cursor = Cursors.Hand;
        }

        void MyLine_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Width = this.MyLine.ActualWidth;// +borderEdit;
            this.Height = this.MyLine.ActualHeight;// +borderEdit;
        }

        public BPLine( string color, double borderW,
            double x1, double y1, double x2, double y2):this()
        {
            this.borderW = borderW;
            this.Color = color;

         
            this.MyLine.X1 = x1;
            this.MyLine.Y1 = y1;
            this.MyLine.X2 = x2;
            this.MyLine.Y2 = y2;
            this.MyLine.StrokeThickness = borderW;
         
          
        }

        public void UpdatePos(double deltaX, double deltaY)
        {

            this.MyLine.X1 += deltaX;
            this.MyLine.X2 += deltaX;

            this.MyLine.Y1 += deltaY;
            this.MyLine.Y2 += deltaY;
            if (Moved != null)
                Moved(this);

            Glo.ViewNeedSave = true;

            //double newLeft = Canvas.GetLeft(this.MyLine) ;
            //newLeft +=
            //    this.MyLine.ActualWidth / 2 
            //    - this.ActualWidth / 2;
            //double newTop = Canvas.GetTop(this.MyLine);
            //newTop +=
            //    this.MyLine.ActualHeight / 2 
            //    - this.ActualHeight / 2;

            //this.SetValue(Canvas.LeftProperty, newLeft );
            //this.SetValue(Canvas.TopProperty, newTop );
        }

    }
}
