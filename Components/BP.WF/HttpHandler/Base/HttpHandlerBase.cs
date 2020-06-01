using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using BP.Web;
using BP.DA;
using BP.Port;

namespace BP.WF.HttpHandler
{
    abstract public class HttpHandlerBase : IHttpHandler, IRequiresSessionState, IReadOnlySessionState
    {
        /// <summary>
        /// token for guangxi jisuanzhongxin.
        /// 1. 手机端连接服务需要,身份验证,需要token.
        /// 2. 在全局中配置 TokenHost 地址, 每次调用服务都需要传入Token 参数.
        /// 3. 如果不配置 TokenHost 就提示错误.
        /// 4. 仅仅在会话信息丢失后，在调用该方法.
        /// </summary>
        /// <param name="token">获得token.</param>
        /// <returns></returns>
        public string DealToken(string token)
        {
            if (DataType.IsNullOrEmpty(token) == true)
                throw new Exception("err@登录信息丢失，或者没有传递过来token.");

            string host = BP.Sys.SystemConfig.GetValByKey("TokenHost", null);
            if (DataType.IsNullOrEmpty(host) == true)
                throw new Exception("err@全局变量:TokenHost，没有获取到.");

            string url = host + token;
            string data = BP.DA.DataType.ReadURLContext(url, 5000);

            if (DataType.IsNullOrEmpty(data) == true)
                throw new Exception("err@token失效，请重新登录。" + url + "");

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = data;
            if (emp.RetrieveFromDBSources() == 0)
                throw new Exception("err@根据token获取用户名错误:" + token + ",获取数据为:" + data);

            //执行登录.
            BP.WF.Dev2Interface.Port_Login(data);
            //DBAccess.RunSQL("UPDATE WF_Emp SET Token='" + token + "'  WHERE No='" + emp.No + "'");
            return "info@登录成功.";
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
            DirectoryPageBase ctrl = Activator.CreateInstance(this.CtrlType) as DirectoryPageBase;

            //让其支持跨域访问.
            string origin = HttpContextHelper.Request.Headers["Origin"];
            if (!string.IsNullOrEmpty(origin))
            {
                var allAccess_Control_Allow_Origin = System.Web.Configuration.WebConfigurationManager.AppSettings["Access-Control-Allow-Origin"];
                HttpContextHelper.Response.Headers["Access-Control-Allow-Origin"] = origin;
                HttpContextHelper.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                HttpContextHelper.Response.Headers["Access-Control-Allow-Headers"] = "x-requested-with,content-type";

                //if (!string.IsNullOrEmpty(allAccess_Control_Allow_Origin))
                //{
                //    var origin = HttpContextHelper.Request.Headers["Origin"];
                //    if (System.Web.Configuration.WebConfigurationManager.AppSettings["Access-Control-Allow-Origin"].Contains(origin))
                //    {
                //        HttpContextHelper.Response.Headers["Access-Control-Allow-Origin"] = origin;
                //        HttpContextHelper.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                //        HttpContextHelper.Response.Headers["Access-Control-Allow-Headers"] = "x-requested-with,content-type";
                //    }
                //}
            }

            try
            {
                //deal token
                if (WebUser.No == null)
                {
                    bool isCanDealToken = true;
                    if (ctrl.DoType.Contains("Login") == false)
                        isCanDealToken = false;
                    if (ctrl.ToString().Contains("Admin") == false)
                        isCanDealToken = false;

                    if (isCanDealToken == true)
                        this.DealToken(ctrl.GetRequestVal("Token"));
                }

                //执行方法返回json.
                string data = ctrl.DoMethod(ctrl, ctrl.DoType);

                //返回执行的结果.
                HttpContextHelper.Response.Write(data);
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

                HttpContextHelper.Response.Write(err);
            }
        }

    }
}
