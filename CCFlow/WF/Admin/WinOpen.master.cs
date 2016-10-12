using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class WF_Admin_WinOpen : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (BP.Web.WebUser.No != "admin")
        {
            string url = this.Request.RawUrl;
            this.Session.Add("LastUrl", url);
            this.Response.Redirect("/WF/Admin/ReLogin.htm?LastUrl=[" + url + "]", true);

            //throw new Exception("@非法的用户必须由admin才能操作，现在登录用户是：" + BP.Web.WebUser.No);
        }
    }
}
