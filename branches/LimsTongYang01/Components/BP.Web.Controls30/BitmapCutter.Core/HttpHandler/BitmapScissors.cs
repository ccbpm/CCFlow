using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Reflection;
using BitmapCutter.Core.API;
using System.IO;
//51-aspx
namespace BitmapCutter.Core.HttpHandler
{
    /// <summary>
    /// bitmap scissors(http handler)
    /// </summary>
   public  class BitmapScissors:IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string methodName = context.Request["action"];
            Callback ops =new Callback();
            MethodInfo method = typeof(Callback).GetMethod(methodName);
            string msg = method.Invoke(ops,
                                                      new object[] { 
                                                            Path.Combine(context.Server.MapPath("~/"), 
                                                           context.Server.MapPath("~/")+context.Request["src"]) 
                                                       }).ToString();
            context.Response.Write(msg);
        }       
       
        public bool IsReusable
        {
            get { return false; }
        }
    }
}
