using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;

namespace BP.CY.ExportAsPNG
{
    /// <summary>
    /// Based on:
    /// 
    /// Andy Beaulieu
    /// http://www.andybeaulieu.com/Home/tabid/67/EntryID/161/Default.aspx
    /// 
    /// 
    /// Tom Giam
    /// http://silverlight.net/forums/t/108713.aspx
    /// </summary>
    public class ElementToPNG
    {
        public void ShowSaveDialog(UIElement elementToExport)
        {

            //Canvas canvasToExport = e as Canvas;
            // Instantiate SaveFileDialog
            // and set defautl settings (just PNG export)

            SaveFileDialog sfd = new SaveFileDialog()
            {
                DefaultExt = "png",
                Filter = "Png files (*.png)|*.png|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (sfd.ShowDialog() == true)
            {
                SaveAsPNG(sfd, elementToExport);
            }
        }

        private void SaveAsPNG(SaveFileDialog sfd, UIElement elementToExport)
        {
            // Save it to disk
            Stream pngStream = GetPNGStream(elementToExport);

            byte[] binaryData = new Byte[pngStream.Length];

            long bytesRead = pngStream.Read(binaryData, 0, (int)pngStream.Length);

            Stream stream = sfd.OpenFile();

            stream.Write(binaryData, 0, binaryData.Length);

            stream.Close();


        }

        /// <summary>
        /// 返回流
        /// </summary>
        /// <param name="elementToExport"></param>
        /// <returns></returns>
        public Stream GetPNGStream(UIElement elementToExport)
        {
            WriteableBitmap bitmap = new WriteableBitmap(elementToExport, new TranslateTransform());
            EditableImage imageData = new EditableImage(bitmap.PixelWidth, bitmap.PixelHeight);

            try
            {
                for (int y = 0; y < bitmap.PixelHeight; ++y)
                {
                    for (int x = 0; x < bitmap.PixelWidth; ++x)
                    {
                        int pixel = bitmap.Pixels[bitmap.PixelWidth * y + x];
                        imageData.SetPixel(x, y,

                        (byte)((pixel >> 16) & 0xFF),
                        (byte)((pixel >> 8) & 0xFF),

                        (byte)(pixel & 0xFF), (byte)((pixel >> 24) & 0xFF)
                        );
                    }
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("Cannot print images from other domains");
            }

            return imageData.GetStream(); ;
        }
    }
}
