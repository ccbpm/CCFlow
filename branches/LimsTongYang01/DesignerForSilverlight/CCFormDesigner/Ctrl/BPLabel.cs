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

namespace CCForm
{
    public class BPLabel : LabelExt
    {
        public override bool IsSelected
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
                    this.BorderThickness = new Thickness(0);
                    this.BorderBrush = new SolidColorBrush(Glo.BorderBrush);
                }
            }
        }
     
        /// <summary>
        /// BPLabel
        /// </summary>
        public BPLabel()
        {
            this.BindDrag();
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
            this.Content = "label...";
            this.FontSize = 14;
            this.IsSelected = false;

        }

        #region 转向方法.
        public void ToLeft()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.LeftProperty, x - 1);
        }
        public void ToRight()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.LeftProperty, x + 1);
        }

        public void ToUp()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.TopProperty, y - 1);
        }
        public void ToDown()
        {
            double x = (double)this.GetValue(Canvas.LeftProperty);
            double y = (double)this.GetValue(Canvas.TopProperty);
            this.SetValue(Canvas.TopProperty, y + 1);
        }
        #endregion 转向方法.

    }
}
