using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.Admin.CCFlowDesigner.lib.sets
{
    /// <summary>
    /// svg 的摘要说明
    /// </summary>
    public class svg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string set = context.Request["set"];
            string figure = context.Request["figure"];
            string basePath = context.Server.MapPath("");
            string filePath = "CCFlowDesigner/" + set + "/" + figure + ".png";
            context.Response.Write(filePath);
            context.Response.End();
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