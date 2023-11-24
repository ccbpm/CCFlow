using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
using BP.DA;
using BP.Cloud.WeXinAPI;
using BP.Port;
using BP.Sys;
namespace CCFlow.Admin.WeChat
{
    public partial class CallReg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //授权成功通知回调推送给服务商
            string authCode = BP.Cloud.WeXinAPI.Glo.getValue("auth_code");
            HttpContext.Current.Cache.Insert("authcode", authCode);

            //根据临时授权码，获得永久授权码.
            BP.Cloud.WeXinAPI.Glo.InstallIt(authCode);

            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "text/html";
            HttpContext.Current.Response.Expires = 0;
            HttpContext.Current.Response.Write("安装成功: <a href='" + BP.Cloud.WeXinAPI.Glo.Domain + "Default.htm?QYWXSM=1'>回到主页，扫码登录！</a>");
            HttpContext.Current.Response.End();
        }

    }
}