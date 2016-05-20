using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.App
{
    public partial class SiteMenu : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.Web.WebUser.No == null && this.Request.RawUrl.Contains("Login")==false)
                this.Response.Redirect("Login.aspx", true);
        }
    }
}