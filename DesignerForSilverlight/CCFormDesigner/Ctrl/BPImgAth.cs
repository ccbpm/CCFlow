using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace CCForm
{
    public class BPImgAth : TextBoxExt
    {
        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public bool IsEdit = true;
        /// <summary>
        /// 附件ID
        /// </summary>
        public string  CtrlID = null;
     
      
        /// <summary>
        /// BPImgAth
        /// </summary>
        public BPImgAth()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
            this.IsReadOnly = true;

            this.Width = 160;
            this.Height = 200;

            ImageBrush ib = new ImageBrush();
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/Logo/" + Glo.CustomerNo + "/LogoH.png", UriKind.Relative));

            ib.ImageSource = png;
            this.Background = ib;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
          
            this.IsSelected = false;
        }
      
    }
}
