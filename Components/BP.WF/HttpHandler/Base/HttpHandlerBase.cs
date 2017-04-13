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
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
        }
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
        private HttpContext context = null;
        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;

            //创建 ctrl 对象, 获得业务实体类.
            WebContralBase ctrl = Activator.CreateInstance(this.CtrlType, context) as WebContralBase;
            ctrl.context = mycontext;

            try
            {
                //执行方法返回json.
                string data = ctrl.DoMethod(ctrl, ctrl.DoType);

                //返回执行的结果.
                ctrl.context.Response.Write(data);
            }
            catch (Exception ex)
            {
                //返回执行错误的结果.
                if (ex.InnerException != null)
                    ctrl.context.Response.Write("err@在执行类[" + this.CtrlType.ToString() + "]，方法[" + ctrl.DoType + "]错误 \t\n @" + ex.InnerException.Message + " \t\n @技术信息:" + ex.StackTrace);
                else
                    ctrl.context.Response.Write("err@在执行类[" + this.CtrlType.ToString() + "]，方法[" + ctrl.DoType + "]错误 \t\n @" + ex.Message + " \t\n @技术信息:" + ex.StackTrace);
            }
        }

    }
}
