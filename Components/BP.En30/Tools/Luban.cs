using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace BP.Tools
{
    /// <summary>
    /// 文件压缩类
    /// </summary>
    public class Luban
    {
        /// <summary>
        /// 文件大小
        /// </summary>
        public long ImgFileLength { get; set; }
        /// <summary>
        /// 图片文件
        /// </summary>
        public Image Img { get; set; }
        /// <summary>
        /// 忽略压缩
        /// </summary>
        public int IgnoreBy { get; set; } = 102400;
        //质量
        public int Quality { get; set; } = 60;

        public Luban(HttpPostedFile hpf)
        {
            this.Img = Image.FromStream(hpf.InputStream, true, true);
            this.ImgFileLength = hpf.ContentLength;
        }

        public Luban(HttpPostedFileBase hpf)
        {
            this.Img = Image.FromStream(hpf.InputStream, true, true);
            this.ImgFileLength = hpf.ContentLength;
        }

        public Luban(string path)
        {
            this.Img = Image.FromFile(path);
            this.ImgFileLength = new FileInfo(path).Length;
        }
        public void Compress(string outputPath)
        {
            // 先调整大小，再调整品质
            if (ImgFileLength <= this.IgnoreBy)
            {
                Resize(Img, outputPath, Img.Size.Width, Img.Size.Height, Quality);
            }
            else
            {
                var scale = ComputeScale();
                var _tup_1 = Img.Size;
                var srcWidth = _tup_1.Width;
                var srcHeight = _tup_1.Height;
                Resize(Img, outputPath, srcWidth / scale, srcHeight / scale, Quality);
            }
        }

        private int ComputeScale()
        {
            // 计算缩小的倍数
            var _tup_1 = Img.Size;
            var srcWidth = _tup_1.Width;
            var srcHeight = _tup_1.Height;
            srcWidth = srcWidth % 2 == 1 ? srcWidth + 1 : srcWidth;
            srcHeight = srcHeight % 2 == 1 ? srcHeight + 1 : srcHeight;
            var longSide = Math.Max(srcWidth, srcHeight);
            var shortSide = Math.Min(srcWidth, srcHeight);
            double scale = Convert.ToDouble(shortSide) / longSide;
            if (scale <= 1 && scale > 0.5625)
            {
                if (longSide < 1664)
                {
                    return 1;
                }
                else if (longSide < 4990)
                {
                    return 2;
                }
                else if (longSide > 4990 && longSide < 10240)
                {
                    return 4;
                }
                else
                {
                    return Math.Max(1, longSide / 1280);
                }
            }
            else if (scale <= 0.5625 && scale > 0.5)
            {
                return Math.Max(1, longSide / 1280);
            }
            else
            {
                return (int)Math.Ceiling(longSide / (1280.0 / scale));
            }
        }
        private void Resize(Image iSource, string outputPath, int newWidth, int newHeight, int quality)
        {
            ImageFormat tFormat = iSource.RawFormat;
            Bitmap ob = new Bitmap(newWidth, newHeight);
            Graphics graph = Graphics.FromImage(ob);
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.SmoothingMode = SmoothingMode.AntiAlias;
            graph.DrawImage(iSource, new Rectangle(0, 0, newWidth, newHeight));
            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = quality;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(outputPath, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else
                {
                    ob.Save(outputPath, tFormat);
                }
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
    }
}
