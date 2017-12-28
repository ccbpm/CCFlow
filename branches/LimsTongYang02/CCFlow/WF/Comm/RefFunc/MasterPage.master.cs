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
using BP.Port;
using BP.DA;
using BP.En;

namespace CCFlow.WF.Comm.RefFunc
{
    public partial class MasterPage : BP.Web.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
         "<link href='./../Style/Table0.css' rel='stylesheet' type='text/css' />");

        }
    }
}
