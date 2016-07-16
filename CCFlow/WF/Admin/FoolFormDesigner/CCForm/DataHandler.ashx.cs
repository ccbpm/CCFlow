using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace CCFlow.WF.MapDef.CCForm
{
    /// <summary>
    /// DataHandler 的摘要说明
    /// </summary>
    public class DataHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string filename = context.Request.QueryString["filename"].ToString();
            using (FileStream fs = File.Create(context.Server.MapPath("~/DataUser/ImgAth/Upload/" + filename)))
            {
                SaveImage(context.Request.InputStream, fs);
            }
        }
        private void SaveImage(Stream stream, FileStream fs)
        {
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, bytesRead);
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
}