using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace CCForm
{
    public class BPMapPin : TextBoxExt
    {
        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public string IsEdit = "1";
        
        /// <summary>
        /// 附件ID
        /// </summary>
        public string  CtrlID = null;
        public string MyPK = null;

        public override bool IsCanReSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// BPMapPin
        /// </summary>
        public BPMapPin()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
            this.IsReadOnly = true;

            this.Width = 28;
            this.Height = 28;

            ImageBrush ib = new ImageBrush();
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/ic_pin.png", UriKind.Relative));
            
            ib.ImageSource = png;
            this.Background = ib;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
            this.IsSelected = false;
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Text = "";
        }
    }
}
