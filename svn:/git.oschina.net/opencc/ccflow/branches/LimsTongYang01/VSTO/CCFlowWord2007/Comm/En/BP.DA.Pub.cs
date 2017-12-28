using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.DA
{
    public class DataType
    {
       
        /// <summary>
        /// 是否音频文集
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsMp3(string ext)
        {
            ext = ext.Replace(".", "");
            switch (ext.ToLower())
            {
                case "mp3":
                case "rm":
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 是否图片文件
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsImg(string ext)
        {
            ext = ext.Replace(".", "");
            switch (ext.ToLower())
            {
                case "jpg":
                case "gif":
                case "png":
                case "ico":
                case "bmp":
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 是否是视频文件
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static bool IsVoide(string ext)
        {
            if (IsSwf(ext))
                return true;

            ext = ext.Replace(".", "");
            switch (ext.ToLower())
            {
                case "asf":
                case "asx":
                case "wpl":
                case "wm":
                case "wmx":
                case "wmd":
                case "wmz":
                case "avi":
                case "mepg":
                case "mpg":
                case "mpe":
                case "m1v":
                case "mp2":
                case "wmv":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSwf(string ext)
        {
             ext = ext.Replace(".", "");
             switch (ext.ToLower())
             {
                 case "swf":
                     return true;
                 default:
                     return false;
             }
        }
    }
}
