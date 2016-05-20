using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Messaging;
using System.Windows.Browser;
using System.IO;
using System.Windows.Media.Imaging;

namespace BP.Controls
{
    public partial class HoverImageLarge : UserControl
    {
        double newLeft, newTop;

        public HoverImageLarge()
        {
            InitializeComponent();
            //HtmlPage.RegisterScriptableObject("HoverImageLarge", this);
        }

        void messageReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string msg;
            if (e.Message.StartsWith("Position:"))
            {
                msg = e.Message.Replace("Position:", "");
                string[] arr = msg.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                newLeft = double.Parse(arr[0]);
                newTop = double.Parse(arr[1]);
                Canvas.SetLeft(img2, -newLeft);
                Canvas.SetTop(img2, -newTop);
            }

            if (e.Message.StartsWith("NoHover:"))
            {
                Canvas.SetLeft(img2, 0);
                Canvas.SetTop(img2, 0);
            }

            if (e.Message.StartsWith("Visibility:"))
            {
                msg = e.Message.Replace("Visibility:", "");
                if (Int32.Parse(msg) == 1)
                {
                    img2.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    img2.Visibility = System.Windows.Visibility.Collapsed;
                }
            }

        }

        #region script method
        [ScriptableMember()]
        public void SetImage(string imageBase64)
        {
            if (imageBase64 == null) return;
            MemoryStream pFileStream = new MemoryStream();
            byte[] imgByte = Convert.FromBase64String(imageBase64);
            pFileStream.Write(imgByte, 0, imgByte.Length);
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(pFileStream);
            img2.Source = bitmap;
        }


        public void SetImage(MemoryStream pFileStream)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(pFileStream);
            img2.Source = bitmap;
        }
        [ScriptableMember()]
        public void SetPosition(string msg)
        {
            if (msg == null) return;
            if (msg.StartsWith("Position:"))
            {
                msg = msg.Replace("Position:", "");
                string[] arr = msg.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                newLeft = double.Parse(arr[0]);
                newTop = double.Parse(arr[1]);
                Canvas.SetLeft(img2, -newLeft);
                Canvas.SetTop(img2, -newTop);
            }

            if (msg.StartsWith("NoHover:"))
            {
                Canvas.SetLeft(img2, 0);
                Canvas.SetTop(img2, 0);
            }

            if (msg.StartsWith("Visibility:"))
            {
                msg = msg.Replace("Visibility:", "");
                if (Int32.Parse(msg) == 1)
                {
                    img2.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    img2.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
