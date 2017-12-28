<%@ WebHandler Language="C#" Class="WebClientDownloadHandler" %>

using System;
using System.IO;
using System.Web;

public class WebClientDownloadHandler : IHttpHandler {
    
  
        public void ProcessRequest(HttpContext context)
        {         
            String filePath =context.Request.QueryString["filePath"].Replace(@"\\",@"\");
            var fileName = filePath.Split('.')[1].Trim('\\') + ".xml";
            String fileFullName = filePath + fileName;
            FileInfo fileInfo = new FileInfo(fileFullName);
            if (fileInfo.Exists)
            {
                byte[] buffer = new byte[102400];
                context.Response.Clear();
                using (FileStream iStream = File.OpenRead(fileFullName))
                {
                    long dataLengthToRead = iStream.Length; //获取下载的文件总大小

                    context.Response.ContentType = "application/octet-stream";
                    context.Response.AddHeader("Content-Disposition", "attachment;  filename=" +
                                       HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));
                    while (dataLengthToRead > 0 && context.Response.IsClientConnected)
                    {
                        int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(102400));//'读取的大小

                        context.Response.OutputStream.Write(buffer, 0, lengthRead);
                        context.Response.Flush();
                        dataLengthToRead = dataLengthToRead - lengthRead;
                    }
                    context.Response.Close();
                    context.Response.End();
                }
            }
        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
}