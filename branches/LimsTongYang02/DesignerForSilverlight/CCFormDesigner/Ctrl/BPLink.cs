using System;
using System.Windows;
using System.Windows.Media;
namespace CCForm
{
    public class BPLink : LabelExt
    {
        /// <summary>
        /// Url
        /// </summary>
        public string URL = null;
        /// <summary>
        /// 窗口目标
        /// </summary>
        public string WinTarget = "_blank";
        public string FK_MapData = null;

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
        /// BPLink
        /// </summary>
        public BPLink()
        {
            this.BindDrag();
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);

            this.Content = "link...";
            //  this.NavigateUri = new Uri("http://ccflow.org");
            this.Tag = "_blank";
            this.FontSize = 12;
            this.IsSelected = false;
        }
    }
}
