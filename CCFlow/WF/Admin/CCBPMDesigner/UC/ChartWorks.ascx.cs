using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.CCBPMDesigner.UC
{
    public partial class ChartWorks : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("style_1", "<link href='" + BP.WF.Glo.CCFlowAppPath +
                "WF/Comm/Style/Table0.css' rel='stylesheet' type='text/css' />");
            this.Page.RegisterClientScriptBlock("style_2", "<link href='" + BP.WF.Glo.CCFlowAppPath +
            "WF/Comm/Style/Tabs.css' rel='stylesheet' type='text/css' />");

        }
    }
}