using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;


namespace CCFlow
{
    public partial class Default : WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Redirect("./WF/Admin/CCBPMDesigner/Login.aspx", true);
            return;
        }
    }
}