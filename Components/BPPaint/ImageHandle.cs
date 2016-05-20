using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using FluxJpeg.Core;
using FluxJpeg.Core.Encoder;

namespace BP
{
    public class ImageHandle
    {
        public static ImageSource ByteToImage(byte[] Byte)
        {
            using (MemoryStream ms = new MemoryStream(Byte))
            {
                BitmapImage bitImage = new BitmapImage();
                bitImage.SetSource(ms);
                return bitImage;
            }
        }
        public static byte[] ImageToByte(ImageSource Image)
        {
            WriteableBitmap wb = new WriteableBitmap(Image as BitmapSource);//将Image对象转换为WriteableBitmap
            byte[] b = Convert.FromBase64String(GetBase64Image(wb));
            return b;
        }

        private static string GetBase64Image(WriteableBitmap bitmap)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;
            byte[][,] raster = new byte[bands][,];

            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);
            using (MemoryStream stream = new MemoryStream())
            {
                JpegEncoder encoder = new JpegEncoder(img, 100, stream);
                encoder.Encode();
                stream.Seek(0, SeekOrigin.Begin);
                byte[] binaryData = new Byte[stream.Length];
                long bytesRead = stream.Read(binaryData, 0, (int)stream.Length);
                string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
                return base64String;
            }
        }
    }
}
