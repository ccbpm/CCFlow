using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using BP.Web;

namespace BP.NetPlatformImpl
{
    public class WF_File
    {
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath"></param>
        public static void UploadFile(string filePath)
        {
            try
            {
                 var filelist = HttpContextHelper.Current.Request.Files;
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("没有上传文件");
                }
                HttpPostedFile f = filelist[0];
                // 写入文件
                f.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <returns></returns>
        public static HttpPostedFile GetUploadFile(int index=0)
        {
            try
            {
                var filelist = HttpContextHelper.Current.Request.Files;
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("没有上传文件");
                }
                return filelist[index];
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static HttpFileCollection GetUploadFiles()
        {
            try
            {
                var filelist = HttpContextHelper.Current.Request.Files;
                if (filelist == null || filelist.Count == 0)
                {
                    throw new NotImplementedException("没有上传文件");
                }
                return filelist;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
        public static void UploadFile(HttpPostedFile file,string filePath)
        {
            try
            {
                // 写入文件
                file.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }
}
