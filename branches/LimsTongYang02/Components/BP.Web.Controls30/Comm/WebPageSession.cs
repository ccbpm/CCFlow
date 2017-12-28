using System;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using BP.Web;
//using BP.Rpt ; 
using BP.DA;
using BP.En;
using System.Reflection;
using System.Text.RegularExpressions;


namespace BP.Web
{
    /// <summary>
    /// PortalPage 的摘要说明。
    /// </summary>
    public class WebPageSession : WebPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this.Request.Browser.Cookies == false)
                throw new Exception("您的浏览器不支持cookies功能，无法使用ccflow。");

            if (WebUser.No==null)
                throw new Exception("您登陆的时间太长，请重新登陆。");
        }
    }
}

