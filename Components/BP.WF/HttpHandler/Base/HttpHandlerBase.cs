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
        /// <para>注意： “Handler业务处理类”必须继承自BP.WF.HttpHandler.WebContralBase</para>
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


            try
            {
                //获得执行的方法.
                string doType = context.Request.QueryString["DoType"];
                if (doType == null)
                    doType = context.Request.QueryString["Action"];
                if (doType == null)
                    doType = context.Request.QueryString["Method"];


                //执行方法返回json.
                string data = ctrl.DoMethod(ctrl, doType);

                //返回执行的结果.
                context.Response.Write(data);

            }
            catch (Exception ex)
            {
                //返回执行错误的结果.
                context.Response.Write("err@" + ex.Message);
            }
        }

    }
}
