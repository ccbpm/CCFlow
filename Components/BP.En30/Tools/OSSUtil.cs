using System;
using System.IO;
using Aliyun.OSS;

namespace BP.Tools
{
    public static class OSSUtil
    {
        /// <summary>
        /// ���OSS����������Ƿ�����
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void CheckOSSConfig()
        {
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSEndpoint))
                throw new Exception("err@��⵽û������OSSEndpoint����������OSSEndpoint�������ϴ��ļ�");
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSAccessKeyId))
                throw new Exception("err@��⵽û������OSSAccessKeyId����������OSSAccessKeyId�������ϴ��ļ�");
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSAccessKeySecret))
                throw new Exception("err@��⵽û������OSSAccessKeySecret����������OSSAccessKeySecret�������ϴ��ļ�");
            if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.OSSBucketName))
                throw new Exception("err@��⵽û������OSSBucketName����������OSSBucketName�������ϴ��ļ�");
        }

        /// <summary>
        /// OSS�ϴ��ļ�
        /// </summary>
        /// <param name="objectName">Object����·��</param>
        /// <param name="temp">�������ʱ�ļ�·��</param>
        /// <exception cref="Exception"></exception>
        public static void doUpload(string objectName, string temp)
        {
            //�������
            CheckOSSConfig();

            // ����OssClientʵ����
            var client = new OssClient(BP.Difference.SystemConfig.OSSEndpoint,
                BP.Difference.SystemConfig.OSSAccessKeyId, BP.Difference.SystemConfig.OSSAccessKeySecret);
            if (objectName.StartsWith("/"))
                objectName = objectName.Substring(1);
            try
            {
                // �ϴ��ļ���
                client.PutObject(BP.Difference.SystemConfig.OSSBucketName, objectName, temp);
            }
            catch (Exception ex)
            {
                throw new Exception("err@ �ϴ�ʧ��, ������ʾ��Ϣ��" + ex.Message);
            }
        }
        /// <summary>
        /// OSS�����ļ�
        /// </summary>
        /// <param name="fileFullNamem">�ļ�ȫ·����</param>
        /// <param name="tempFile">��ʱ�ļ�·��</param>
        /// <exception cref="Exception"></exception>

        public static void doDownload(string fileFullNamem, string tempFile)
        {
            //�������
            CheckOSSConfig();

            // ����OssClientʵ����
            var client = new OssClient(BP.Difference.SystemConfig.OSSEndpoint,
                BP.Difference.SystemConfig.OSSAccessKeyId, BP.Difference.SystemConfig.OSSAccessKeySecret);
            try
            {
                string subDir = "";
                //�ж��Ƿ�����Ŀ¼·��
                if (BP.DA.DataType.IsNullOrEmpty(BP.Difference.SystemConfig.BucketSubPath) == false && "/".Equals(BP.Difference.SystemConfig.BucketSubPath) == false)
                {
                    subDir = BP.Difference.SystemConfig.BucketSubPath + "/";
                }
                //ת��·��
                fileFullNamem = subDir + fileFullNamem.Replace("//", "/");
                if (fileFullNamem.StartsWith("/"))
                    fileFullNamem = fileFullNamem.Substring(1);
                // �����ļ�������OssObject�������ļ��ĸ�����Ϣ�����ļ����ڵĴ洢�ռ䡢�ļ�����Ԫ��Ϣ�Լ�һ����������
                var obj = client.GetObject(BP.Difference.SystemConfig.OSSBucketName, fileFullNamem);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[1024];
                    var fs = File.Open(tempFile, FileMode.OpenOrCreate);
                    var len = 0;
                    // ͨ�����������ļ������ݶ�ȡ���ļ������ڴ��С�
                    while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("err@����ʧ��.������ʾ��Ϣ�� " + ex.Message);
            }
        }
    }
}