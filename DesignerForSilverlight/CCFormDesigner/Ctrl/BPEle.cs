using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace CCForm
{
    public class BPEle : TextBoxExt
    {
        public string EleType = null;
        public string EleName = null;
        public string EleID = null;
        public string Tag1 = null;
        public string Tag2 = null;
        public string Tag3 = null;
        public string Tag4 = null;
        public BitmapImage HisPng = null;

        /// <summary>
        /// BPEle
        /// </summary>
        public BPEle()
        {

            ImageBrush ib = new ImageBrush();
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/FrmEleDef.png", UriKind.Relative));
            ib.ImageSource = png;
            this.Background = ib;

            this.HisPng = png;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
            this.IsReadOnly = true;
            this.Width = 200;
            this.Height = 150;

            this.IsSelected = false;
        }

      
    }
}
