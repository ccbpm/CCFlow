using System;
using System.Windows;
using System.Windows.Media;
using BP.En;

namespace CCForm
{
    public class BPDatePicker : System.Windows.Controls.DatePicker,IElement
    {
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
        public bool ViewDeleted
        {
            get;
            set;
        }
        public bool TrackingMouseMove { get; set; }  
      
       
        #endregion 
        public string KeyName = null;
        public string HisDateType = DataType.AppDate;
        /// <summary>
        /// BPDatePicker
        /// </summary>
        public BPDatePicker()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);

            this.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.DataContext = DateTime.Now.ToString("yyyy-MM-dd");
            this.Width = this.Width - 10;
        }

        #region 焦点事件
        //protected override void OnGotFocus(RoutedEventArgs e)
        //{
        //    this.BorderBrush.Opacity = 4;
        //    base.OnGotFocus(e);
        //}
        //protected override void OnLostFocus(RoutedEventArgs e)
        //{
        //    this.BorderBrush.Opacity = 0.5;
        //    base.OnLostFocus(e);
        //}
        #endregion 焦点事件
      
      
       
    }
}
