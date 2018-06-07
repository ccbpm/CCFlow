using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace BitmapCutter.Core.API
{
    /// <summary>
    /// Bitmap operations
    /// </summary>
    public class Callback
    {
        /// <summary>
        /// create a new BitmapCutter.Core.API.BitmapOPS instance
        /// </summary>
        public Callback() { }

        /// <summary>
        /// rotate bitmap with any angle
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public string RotateBitmap(string src)
        {
            try
            {
                src = src.Replace("/", "\\");
                src = src.Replace("..\\", string.Empty);
                HttpContext context = HttpContext.Current;
                float angle = float.Parse(context.Request["angle"]);
                Image oldImage = Bitmap.FromFile(src);
                Bitmap newBmp = Helper.RotateImage(oldImage, angle);
                oldImage.Dispose();
                int nw = newBmp.Width;
                int nh = newBmp.Height;
                newBmp.Save(src);
                newBmp.Dispose();
                return "{msg:'success',size:{width:" + nw.ToString() + ",height:" + nh.ToString() + "}}";
            }
            catch (Exception ex)
            {
                return "{msg:'" + ex.Message + "'}";
            }
        }

        public string GenerateBitmap(string src)
        {
            try
            {
                src = src.Replace("/", "\\");
                src = src.Replace("..\\", string.Empty);

                string[] srcImg = src.Split('\\');
                string imgName = srcImg[srcImg.Length - 1];
                string[] str = imgName.Split('B');
                string noName = "";
                if (str.Length<2)
                {
                    string[] strName = imgName.Split('M');
                    noName = strName[0];
                }
                else
                {
                    noName = str[0];
                }
                string newName = DateTime.Now.ToString("yyyyMMhh");
                FileInfo fi = new FileInfo(src);
                string ext = fi.Extension;

                string newfileName = "DataUser/UserIcon/" + noName + "SmallerCon.png";
                if (src.Contains("ImgAth"))//附件图片使用变量，解决页面图片缓存问题
                    newfileName = "DataUser/ImgAth/Temp/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
                //Image.GetThumbnailImageAbort abort = null;
                Bitmap oldBitmap = new Bitmap(src);

                HttpContext context = HttpContext.Current;
                Cutter cut = new Cutter(
                    double.Parse(context.Request["zoom"]),
                    -int.Parse(context.Request["x"]),
                    -int.Parse(context.Request["y"]),
                    int.Parse(context.Request["width"]),
                    int.Parse(context.Request["height"]),
                    oldBitmap.Width,
                    oldBitmap.Height);

                Bitmap bmp = Helper.GenerateBitmap(oldBitmap, cut);
                oldBitmap.Dispose();

                string temp = Path.Combine(context.Server.MapPath("~/"), newfileName);
                bmp.Save(temp, ImageFormat.Png);
                bmp.Dispose();
                System.Drawing.Image myImage = System.Drawing.Image.FromFile(temp);
                int phWidth = myImage.Width;
                int phHeight = myImage.Height;
                GetPicThumbnail(temp, context.Server.MapPath("~/") + "/DataUser/UserIcon/" + noName + "Biger.png", 100, 100);
                GetPicThumbnail(temp, context.Server.MapPath("~/") + "/DataUser/UserIcon/" + noName + ".png",  60, 60);
                GetPicThumbnail(temp, context.Server.MapPath("~/") + "/DataUser/UserIcon/" + noName + "Smaller.png", 40, 40);
                myImage.Dispose();

                System.Drawing.Image myImages = System.Drawing.Image.FromFile(src);
                int w = myImages.Width;
                int h = myImages.Height;
                myImages.Dispose();
                GetPicThumbnail(src, context.Server.MapPath("~/") + "/DataUser/UserIcon/" + noName + "BigerCon.png", h, w);
                if (File.Exists(src))
                {
                    File.SetAttributes(src, FileAttributes.Normal);
                    File.Delete(src);
                }
                return "{msg:'success',src:'../../" + newfileName + "'}";
            }
            catch (Exception ex)
            {
                return "{msg:'" + ex.Message + "'}";
            }
        }
        public static Bitmap MakeThumbnail(System.Drawing.Image fromImg, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            int ow = fromImg.Width;
            int oh = fromImg.Height;

            //新建一个画板
            Graphics g = Graphics.FromImage(bmp);

            //设置高质量插值法
            g.InterpolationMode = InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            g.DrawImage(fromImg, new Rectangle(0, 0, width, height),
                new Rectangle(0, 0, ow, oh),
                GraphicsUnit.Pixel);

            return bmp;

        }
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">图片的原始路径</param>
        /// <param name="dFile">缩放后的保存路径</param>
        /// <param name="dHeight">缩放后图片的高度</param>
        /// <param name="dWidth">缩放后图片的宽带</param>
        /// <returns></returns>
        public static bool GetPicThumbnail(string sFile, string dFile, int dHeight, int dWidth)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);//从指定的文件创建Image
            ImageFormat tFormat = iSource.RawFormat;//指定文件的格式并获取
            int sW = 0, sH = 0;//记录宽度和高度
            Size tem_size = new Size(iSource.Width, iSource.Height);//实例化size。知矩形的高度和宽度
            if (tem_size.Height > dHeight || tem_size.Width > dWidth)//判断原图大小是否大于指定大小
            {
                if ((tem_size.Width * dHeight) > (tem_size.Height * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else//如果原图大小小于指定的大小
            {
                sW = tem_size.Width;//原图宽度等于指定宽度
                sH = tem_size.Height;//原图高度等于指定高度
            }
            Bitmap thumBnail = MakeThumbnail(iSource, sH, sW);

            Bitmap oB = new Bitmap(dWidth, dHeight);//实例化
            Graphics g = Graphics.FromImage(oB);//从指定的Image中创建Graphics
            Rectangle destRect = new Rectangle(new Point(0, 0), new Size(sW, sH));//目标位置
            Rectangle origRect = new Rectangle(new Point(0, 0), new Size(sH, sW));//原图位置（默认从原图中截取的图片大小等于目标图片的大小）
            Graphics G = Graphics.FromImage(oB);

            G.Clear(Color.White);
            // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
            G.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // 指定高质量、低速度呈现。 
            G.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawImage(thumBnail, destRect, origRect, GraphicsUnit.Pixel);
            //G.DrawString("Xuanye", f, b, 0, 0);
            G.Dispose();
            //保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();//用于向图像编码器传递值
            long[] qy = new long[1];
            qy[0] = 100;
            EncoderParameter eParm = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParm;
            try
            {
                oB.Save(dFile, tFormat);// 已指定格式保存到指定文件
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();//释放资源
                oB.Dispose();
            }
        }
    }
}
