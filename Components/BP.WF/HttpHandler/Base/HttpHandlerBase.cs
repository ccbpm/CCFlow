using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BP.WF.HttpHandler
{
    abstract public class HttpHandlerBase : IHttpHandler
    {
        /// <summary>
        /// 获取 “Handler业务处理类”的Type
        /// <para></para>
        /// <para>注意： “Handler业务处理类”必须继承自BP.WF.WebContral.WebContralBase</para>
        /// </summary>
        public abstract Type CtrlType { get; }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            //创建 ctrl 对象.
            WebContralBase ctrl = Activator.CreateInstance(CtrlType, context) as WebContralBase;
            //组织返回结果字符串
            string re = "{{\"success\":{0},\"msg\":\"{1}\",\"data\":{2}}}";
            string success = "true";
            string msg = string.Empty;
            string data = "\"\"";

            try
            {
                //获得执行的方法.
                string doType = context.Request.QueryString["DoType"];
                if (doType==null)
                    doType = context.Request.QueryString["Action"];
                if (doType == null)
                    doType = context.Request.QueryString["Method"];


                //执行方法返回json.
                data = ctrl.DoMethod(ctrl, doType);

                if (!(data.StartsWith("{") && data.EndsWith("}")) && !(data.StartsWith("[") && data.EndsWith("]")))
                    data = "\"" + data.Replace("\"", "'") + "\"";
            }
            catch (Exception ex)
            {
                success = "false";
                msg = "错误：" + ex.Message.Replace("\"", "'");
            }

            //返回执行的结果.
            context.Response.Write(string.Format(re, success, msg, data));
        }

        

    }
}
