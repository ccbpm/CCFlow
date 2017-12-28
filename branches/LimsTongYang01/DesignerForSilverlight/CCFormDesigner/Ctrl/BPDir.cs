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
    public class BPDir : LabelExt
    {
        public string WinTarget = "_blank";
        public string WinURL = "";
        public string Desc = null;
        public double X = 0;
        public double Y = 0;
        public string KeyName = null;

      
        /// <summary>
        /// BPDir
        /// </summary>
        public BPDir()
        {
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/Dir.png", UriKind.Relative));
            this.Content = png;

            this.IsSelected = false;

            //ImageBrush ib = new ImageBrush();
            //BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/Dir.png", UriKind.Relative));
            //ib.ImageSource = png;
            //this.Background = ib;


            //this.Name = "TB" + DateTime.Now.ToString("yyMMddhhmmss");
            //this.Width = 200;
            //this.Height = 120;

            //this.HisPng = png;
            //this.TextWrapping = System.Windows.TextWrapping.Wrap;
        }

        #region 移动事件
        public override bool IsCanReSize
        {
            get
            {
                return true;
            }
        }

        public double MoveStep
        {
            get
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                    return 1;
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    return 2;
                return 0;
            }
        }
       
        #endregion 移动事件

   
    }
}
