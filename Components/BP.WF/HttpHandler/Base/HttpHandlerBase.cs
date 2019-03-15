using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace BP.WF.HttpHandler
{
    abstract public class HttpHandlerBase : IHttpHandler, IRequiresSessionState, IReadOnlySessionState
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
        private HttpContext context = null;
        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;

            //创建 ctrl 对象, 获得业务实体类.
            DirectoryPageBase ctrl = Activator.CreateInstance(this.CtrlType, context) as DirectoryPageBase;
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
                string paras = "";
                foreach (string key in context.Request.QueryString.Keys)
                {
                    paras += "@" + key + "=" + context.Request.QueryString[key];
                }

                string err = "";
                //返回执行错误的结果.
                if (ex.InnerException != null)
                    err = "err@在执行类[" + this.CtrlType.ToString() + "]，方法[" + ctrl.DoType + "]错误 \t\n @" + ex.InnerException.Message + " \t\n @技术信息:" + ex.StackTrace + " \t\n相关参数:" + paras;
                else
                    err = "err@在执行类[" + this.CtrlType.ToString() + "]，方法[" + ctrl.DoType + "]错误 \t\n @" + ex.Message + " \t\n @技术信息:" + ex.StackTrace + " \t\n相关参数:" + paras;

                if (Web.WebUser.No == null)
                    err = "err@登录时间过长,请重新登录. @其他信息:" + err;

                //记录错误日志以方便分析.
                BP.DA.Log.DebugWriteError(err);

                ctrl.context.Response.Write(err);
            }
        }

    }
}
