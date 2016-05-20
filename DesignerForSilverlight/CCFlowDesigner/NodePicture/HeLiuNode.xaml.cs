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

namespace BP.Picture
{
    public partial class HeLiuNode : UserControl, INodeShape
    {
        MergePictureRepeatDirection _repeatDirection = MergePictureRepeatDirection.None;
        public MergePictureRepeatDirection RepeatDirection
        {
            get
            {
                if (_repeatDirection == MergePictureRepeatDirection.None)
                {
                    if (picRect.Width > picRect.Height)
                        _repeatDirection = MergePictureRepeatDirection.Horizontal;
                    else
                        _repeatDirection = MergePictureRepeatDirection.Vertical;
                }

                return _repeatDirection;
            }
            set
            {
                _repeatDirection = value;
                if (_repeatDirection == MergePictureRepeatDirection.Vertical)
                {
                    picRect.Height = 120.0;
                    picRect.Width = 40.0;
                }
                else
                {
                    picRect.Height = 40.0;
                    picRect.Width = 120.0;
                }
            }
        } 
        
        public PointCollection ThisPointCollection
        {
            get
            {
                return null; 
                //if (true)//(_thisPointCollection == null)
                //{
                //    _thisPointCollection = new PointCollection();


                //    PathGeometry pg = (PathGeometry)picRect.GetValue(Path.DataProperty);

                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[0]).Point);
                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[1]).Point);
                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[2]).Point);
                //    _thisPointCollection.Add(((LineSegment)pg.Figures[0].Segments[3]).Point);
                //}

                //return _thisPointCollection;
            }
        }

        public HeLiuNode()
        {
            InitializeComponent();
        }
        public new double Opacity
        {
            set { picRect.Opacity = value; }
            get { return picRect.Opacity; }
        }
        public double PictureWidth
        {
            set { picRect.Width = value - 4; }
            get { return picRect.Width + 4; }
        }
        public double PictureHeight
        {
            set { picRect.Height = value - 4; }
            get { return picRect.Height + 4; }
        }
        public new Brush Background
        {
            set
            {
                picRect.Fill = value;
            }
            get { return picRect.Fill; }
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
       
    
        public void SetBorderColor(Brush brush)
        {
            picRect.Stroke = brush;
        }
    }
}
