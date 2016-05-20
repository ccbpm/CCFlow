using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace CCForm
{
    public class BPImg : TextBoxExt
    {
        public string LinkTarget = "_blank";
        public string LinkURL = "";
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgURL = "";
        /// <summary>
        /// 文件路径
        /// </summary>
        public string ImgPath = "";
        /// <summary>
        /// 中文名
        /// </summary>
        public string TB_CN_Name = "";
        /// <summary>
        /// 英文名
        /// </summary>
        public string TB_En_Name = "";
        /// <summary>
        /// 来源类型
        /// </summary>
        public int SrcType = 0;
        public BitmapImage HisPng = null;

      
        /// <summary>
        /// BPImg
        /// </summary>
        public BPImg()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
            this.IsReadOnly = true;

            this.Width = 200;
            this.Height = 120;

            ImageBrush ib = new ImageBrush();
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/Logo/"+Glo.CustomerNo+"/LogoBig.png", UriKind.Relative));
            ib.ImageSource = png;
            this.Background = ib;

            this.HisPng = png;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
         
            this.IsSelected = false;
        }

    }
}
