<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.IO;
using System.Web.SessionState;

namespace CCFlow.WF.MapDef
{
    public class Handler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            switch (context.Request.QueryString["DoType"])
            {
                case "DownTempFrm":
                    string fileFullName = context.Request.PhysicalApplicationPath + "\\Temp\\" + context.Request.QueryString["FK_MapData"] + ".xml";
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
                                               HttpUtility.UrlEncode(fileInfo.Name, System.Text.Encoding.UTF8));
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
                    break;
                default:
                    break;
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