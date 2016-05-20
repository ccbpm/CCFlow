using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Comm.Sys
{
    public partial class SFDBSrcUI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Redirect("../RefFunc/UIEn.aspx?EnsName=BP.Sys.SFDBSrcs", true);
        }
    }
}