using System;
using System.IO;
using Aliyun.OSS;

namespace BP.Tools
{
    public static class OSSUtil
    {
        /// <summary>
        /// 检查OSS服务的配置是否完整
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void CheckOSSConfig()
        {
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSEndpoint))
                throw new Exception("err@检测到没有配置OSSEndpoint，请您配置OSSEndpoint后重新上传文件");
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSAccessKeyId))
                throw new Exception("err@检测到没有配置OSSAccessKeyId，请您配置OSSAccessKeyId后重新上传文件");
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSAccessKeySecret))
                throw new Exception("err@检测到没有配置OSSAccessKeySecret，请您配置OSSAccessKeySecret后重新上传文件");
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSBucketName))
                throw new Exception("err@检测到没有配置OSSBucketName，请您配置OSSBucketName后重新上传文件");
        }

        /// <summary>
        /// OSS上传文件
        /// </summary>
        /// <param name="objectName">Object完整路径</param>
        /// <param name="temp">保存的临时文件路径</param>
        /// <exception cref="Exception"></exception>
        public static void doUpload(string objectName, string temp)
        {
            //检查配置
            CheckOSSConfig();

            // 创建OssClient实例。
            var client = new OssClient(BP.Difference.SystemConfig.OSSEndpoint,
                BP.Difference.SystemConfig.OSSAccessKeyId, BP.Difference.SystemConfig.OSSAccessKeySecret);
            if (objectName.StartsWith("/"))
                objectName = objectName.Substring(1);
            try
            {
                // 上传文件。
                client.PutObject(BP.Difference.SystemConfig.OSSBucketName, objectName, temp);
            }
            catch (Exception ex)
            {
                throw new Exception("err@ 上传失败, 错误提示信息：" + ex.Message);
            }
        }
        /// <summary>
        /// OSS下载文件
        /// </summary>
        /// <param name="fileFullNamem">文件全路径名</param>
        /// <param name="tempFile">临时文件路径</param>
        /// <exception cref="Exception"></exception>

        public static void doDownload(string fileFullNamem, string tempFile)
        {
            //检查配置
            CheckOSSConfig();

            // 创建OssClient实例。
            var client = new OssClient(BP.Difference.SystemConfig.OSSEndpoint,
                BP.Difference.SystemConfig.OSSAccessKeyId, BP.Difference.SystemConfig.OSSAccessKeySecret);
            try
            {
                string subDir = "";
                //判断是否有子目录路径
                if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.BucketSubPath) == false && "/".Equals(BP.Difference.SystemConfig.BucketSubPath) == false)
                {
                    subDir = BP.Difference.SystemConfig.BucketSubPath + "/";
                }
                //转换路径
                fileFullNamem = subDir + fileFullNamem.Replace("//", "/");
                if (fileFullNamem.StartsWith("/"))
                    fileFullNamem = fileFullNamem.Substring(1);
                // 下载文件到流。OssObject包含了文件的各种信息，如文件所在的存储空间、文件名、元信息以及一个输入流。
                var obj = client.GetObject(BP.Difference.SystemConfig.OSSBucketName, fileFullNamem);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[1024];
                    var fs = File.Open(tempFile, FileMode.OpenOrCreate);
                    var len = 0;
                    // 通过输入流将文件的内容读取到文件或者内存中。
                    while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("err@下载失败.错误提示信息： " + ex.Message);
            }
        }
    }
}