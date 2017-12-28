using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using FluxJpeg.Core.Encoder;

namespace BP.Controls
{
    public class SnapshotCapturer
    {
        
        public static string SaveScreenToString()
        {
            var bitmap = new WriteableBitmap(Application.Current.RootVisual, null);

            //Convert the Image to pass into FJCore
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;

            var raster = new byte[bands][,];

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

            var model = new ColorModel { colorspace = ColorSpace.RGB };

            var img = new Image(model, raster);

            //Encode the Image as a JPEG
            var stream = new MemoryStream();
            var encoder = new JpegEncoder(img, 100, stream);

            encoder.Encode();


            //byte[] bs = stream.ToArray(); 
           // return System.Text.Encoding.UTF8.GetString(bs, 0, bs.Length); 

            //Move back to the start of the stream
            stream.Seek(0, SeekOrigin.Begin);

            //Get the Bytes and write them to the stream
            var binaryData = new Byte[stream.Length];
            long bytesRead = stream.Read(binaryData, 0, (int)stream.Length);

            return Convert.ToBase64String(binaryData);

        }
    }
}
