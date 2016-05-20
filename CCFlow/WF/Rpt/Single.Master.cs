using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Rpt
{
    public partial class Single : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("sds",
            "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");
        }
    }
}