<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Web.SessionState;
using BP.Sys;
using BP.DA;

namespace CCFlow.WF.MapDef
{
    public class Handler : IHttpHandler, IRequiresSessionState
    {
        #region 属性.
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                return context.Request.QueryString["DoType"];
            }
        }
        public string MyPK
        {
            get
            {
                return context.Request.QueryString["MyPK"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return context.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_MapDtl
        {
            get
            {
                return context.Request.QueryString["FK_MapDtl"];
            }
        }
        public HttpContext context = null;
        #endregion 属性.

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg="";
            switch (this.DoType)
            {
                case "InitDtl": //初始化明细表.
                    msg = this.InitDtl(); 
                    break;
                case "DoUp": //上移
                    MapAttr attrU = new MapAttr(this.MyPK);
                    attrU.DoUp();
                    return;
                case "DoDown": //下移.
                    MapAttr attrD = new MapAttr(this.MyPK);
                    attrD.DoDown();
                    return;
                case "DownTempFrm":
                    this.DownTempFrm();
                    return;
                default:
                    msg = "没有判断的执行类型："+this.DoType;
                    break;
            }
        }
        public string InitDtl()
        {
            MapDtl dtl = new MapDtl(this.FK_MapDtl);
            return null;
        }
        /// <summary>
        /// 下载表单.
        /// </summary>
        public void DownTempFrm()
        {
            string fileFullName = context.Request.PhysicalApplicationPath + "\\Temp\\" + context.Request.QueryString["FK_MapData"] + ".xml";
            FileInfo fileInfo = new FileInfo(fileFullName);
            if (fileInfo.Exists)
            {
                byte[] buffer = new byte[102400];
                context.Response.Clear();
                using (FileStream iStream = File.OpenRead(fileFullName))
                {
                    long dataLengthToRead = iStream.Length; //获取下载的文件总大小.

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