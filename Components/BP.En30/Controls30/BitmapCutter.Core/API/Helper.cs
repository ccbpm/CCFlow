using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
//5-1-aspx
namespace BitmapCutter.Core.API
{
    public sealed class Helper
    {
        public Helper() { }
        /// <summary>
        /// Image Rotation
        /// </summary>
        /// <param name="oldImage">A <see cref="System.Drawing.Image"/> source</param>
        /// <param name="angle">Rotate angle(only -180, -90, 90, 180,)</param>
        /// <returns></returns>
        public static Bitmap RotateImage(Image oldImage, float angle)
        {
            float na = Math.Abs(angle);
            if (na != 90 && na != 180)
                throw new ArgumentException("angle could only be -180, -90, 90, 180 degrees, but now it is "+angle.ToString()+" degrees!");

            #region -Unused(for any degrees)-
            //double radius = angle * Math.PI / 180.0;
            //double cos = Math.Cos(radius);
            //double sin = Math.Sin(radius);

            //int ow = oldImage.Width;
            //int oh = oldImage.Height;
            //int nw = (int)(Math.Abs(ow * cos) + Math.Abs(oh * sin));
            //int nh = (int)(Math.Abs(ow * sin) + Math.Abs(oh * cos));
            #endregion

            int ow = oldImage.Width;
            int oh = oldImage.Height;
            int nw = ow;
            int nh = oh;
            //90/-90
            if (na == 90)
            {
                nw = oh;
                nh = ow;
            }

            Bitmap bmp = new Bitmap(nw, nh);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
                g.SmoothingMode = SmoothingMode.HighQuality;

                Point offset = new Point((nw - ow) / 2, (nh - oh) / 2);
                Rectangle rect = new Rectangle(offset.X, offset.Y, ow, oh);
                Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                g.TranslateTransform(center.X, center.Y);
                g.RotateTransform(angle);
                g.TranslateTransform(-center.X, -center.Y);
                g.DrawImage(oldImage, rect);
                g.ResetTransform();
                g.Save();
            }
            return bmp;
        }
        /// <summary>
        /// Generate the new bitmap
        /// </summary>
        /// <param name="oldImage">A <see cref="System.Drawing.Image"/> source</param>
        /// <param name="cut">The <see cref="BitmapCutter.Core.API.Cutter"/></param>
        /// <returns></returns>
        public static Bitmap GenerateBitmap(Image oldImage, Cutter cut)
        {
            if (oldImage == null)
                throw new ArgumentNullException("oldImage");

            Image newBitmap = new Bitmap(cut.SaveWidth, cut.SaveHeight);
            //Re-paint the oldImage
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(oldImage, new Rectangle(0, 0, cut.SaveWidth, cut.SaveHeight), new Rectangle(0, 0, cut.Width, cut.Height), GraphicsUnit.Pixel);
                g.Save();
            }

            Bitmap bmp = new Bitmap(cut.CutterWidth, cut.CutterHeight);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(newBitmap, 0, 0, new Rectangle(cut.X, cut.Y, cut.CutterWidth, cut.CutterHeight), GraphicsUnit.Pixel);
                g.Save();
                newBitmap.Dispose();
            }
            return bmp;
        }
    }
}
