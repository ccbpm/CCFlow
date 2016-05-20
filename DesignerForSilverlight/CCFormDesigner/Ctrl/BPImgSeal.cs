using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BP.En;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
namespace CCForm
{
    public class BPImgSeal : TextBoxExt
    {
        /// <summary>
        /// 存放岗位集合
        /// </summary>
        public string Tag0 = "";
        /// <summary>
        /// 是否允许编辑
        /// </summary>
        public bool IsEdit = true;
        /// <summary>
        /// 中文名
        /// </summary>
        public string TB_CN_Name = "";
        /// <summary>
        /// 英文名
        /// </summary>
        public string TB_En_Name = "";
        public BitmapImage HisPng = null;

        /// <summary>
        /// BPImg
        /// </summary>
        public BPImgSeal()
        {
            this.Name = MainPage.Instance.GenerElementNameFromUI(this);
            this.IsReadOnly = true;

            this.Width = 200;
            this.Height = 120;

            ImageBrush ib = new ImageBrush();
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/sealBig.png", UriKind.Relative));
            ib.ImageSource = png;
            this.Background = ib;

            this.HisPng = png;
            this.TextWrapping = System.Windows.TextWrapping.Wrap;
         
            this.IsSelected = false;
        }
    }
}
