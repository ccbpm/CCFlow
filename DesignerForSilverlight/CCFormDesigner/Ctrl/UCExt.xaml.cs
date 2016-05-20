using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CCForm
{
    public partial class UCExt : UserControl,IElement
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
                    this.layout.BorderBrush = new SolidColorBrush(Colors.Blue);
                }
                else
                {
                    this.layout.BorderBrush = new SolidColorBrush(Glo.BorderBrush);
                }
            }
        }
        public virtual bool TrackingMouseMove
        {
            get;
            set;
        }
        public bool ViewDeleted
        {
            get;
            set;
        }

        protected bool isCanReSize = false;
        public bool IsCanReSize
        {
            get { return isCanReSize; }
            set { isCanReSize = value; }
        }

        public UCExt()
        {
            InitializeComponent();
            this.layout.BorderThickness = new Thickness(2);
        }
    }
}
