using BP.WF.HttpHandler;
using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CCFlow.DataUser.API.Controllers
{
    [EnableCors("*", "*", "*")]
    [HiddenApi]
    public class CommController : ApiController
    {
        [HttpGet, HttpPost]
        public HttpResponseMessage ProcessRequest()
        {
            DirectoryPageBase ctrl = Activator.CreateInstance(this.CtrlType) as DirectoryPageBase;
            try
            {

                //执行方法返回json.
                string data = ctrl.DoMethod(ctrl, ctrl.DoType);
                if (data == null)
                    return new HttpResponseMessage();
                return new HttpResponseMessage { Content = new StringContent(data, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };

                //返回执行的结果.
                //HttpContext.Current.Response.Write(data);
            }
            catch (Exception ex)
            {
                string paras = "";
                foreach (string key in HttpContext.Current.Request.QueryString.Keys)
                {
                    paras += "@" + key + "=" + HttpContext.Current.Request.QueryString[key];
                }

                string err = "";
                //返回执行错误的结果.
                if (ex.InnerException != null)
                    err = "err@在执行类[" + this.CtrlType.ToString() + "]，方法[" + ctrl.DoType + "]错误 \t\n @" + ex.InnerException.Message + " \t\n @技术信息:" + ex.StackTrace + " \t\n相关参数:" + paras;
                else
                    err = "err@在执行类[" + this.CtrlType.ToString() + "]，方法[" + ctrl.DoType + "]错误 \t\n @" + ex.Message + " \t\n @技术信息:" + ex.StackTrace + " \t\n相关参数:" + paras;

                if (BP.Web.WebUser.No == null)
                    err = "err@登录时间过长,请重新登录. @其他信息:" + err;

                //记录错误日志以方便分析.
                BP.DA.Log.DebugWriteError(err);
                return new HttpResponseMessage { Content = new StringContent(err, System.Text.Encoding.GetEncoding("UTF-8"), "application/json") };
                //HttpContextHelper.Response.Write(err);
            }
        }
        public  Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_Comm);
            }
        }
    }

}
