using System;
using System.IO;
using System.Web;
using BP.Difference;


namespace BP.WF.Difference
{
    public class WF_File
    {
        public static void GetFileName(int idx)
        {
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="filePath"></param>
        public static void UploadFile(string filePath)
        {
            //2023.8.21,解决文件文件目录不存在报错的异常，不存在时先创建 by oppein
            FileInfo saveFileInfo = new FileInfo(filePath);
            string saveDirectory = saveFileInfo.DirectoryName;
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            try
            {
                HttpFileCollection filelist = HttpContextHelper.Current.Request.Files;
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
        public static HttpPostedFile GetUploadFile(int index = 0)
        {
            try
            {
                HttpFileCollection filelist = HttpContextHelper.Current.Request.Files;
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
                HttpFileCollection filelist = HttpContextHelper.Current.Request.Files;
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
        public static void UploadFile(HttpPostedFile file, string filePath)
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
        public static long GetFileLength(HttpPostedFile file)
        {
            try
            {
                return file.ContentLength;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.Message);
            }
        }
    }
}
