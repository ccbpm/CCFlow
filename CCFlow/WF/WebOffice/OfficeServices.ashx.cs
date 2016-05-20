using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Word;

namespace CCFlow.WF.WebOffice
{
    /// <summary>
    /// OfficeServices 的摘要说明
    /// </summary>
    public class OfficeServices : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string type = context.Request.QueryString["Type"];
            string result = "";
            switch (type)
            {
                case "savePDF":
                    result = SavePDF(context);
                    break;
                default:
                    break;

            }


            context.Response.Write(result);
        }


        private string SavePDF(HttpContext context)
        {
            string result = "";
            try
            {
                string path = context.Request.QueryString["Path"];

                string fileStart = context.Request.QueryString["Start"];
                string fileExtension = context.Request.QueryString["Extension"];
                Microsoft.Office.Interop.Word.Application appClass = new Microsoft.Office.Interop.Word.Application();
                appClass.Visible = false;

                Object missing = System.Reflection.Missing.Value;

                object fileNameR = path + "\\" + fileStart + fileExtension;
                var wordDoc = appClass.Documents.Open(ref fileNameR, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing,
                    ref missing, ref missing, ref missing);


                object format = WdSaveFormat.wdFormatPDF;
                object savePath = path + "\\" + fileStart + ".pdf";
                wordDoc.SaveAs(ref savePath, ref format, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing,
                    ref missing);
                wordDoc.Close();
                result = "true";
            }
            catch 
            {
                result = "false";
            }
            return result;

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