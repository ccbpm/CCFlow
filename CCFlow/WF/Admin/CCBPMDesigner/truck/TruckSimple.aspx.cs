using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.CCBPMDesigner.truck
{
    public partial class TruckSimple : System.Web.UI.Page
    {
        public string FK_Flow
        {
            get { return Request.QueryString["FK_Flow"]; }
        }

        public string WorkID
        {
            get { return Request.QueryString["WorkID"]; }
        }

        public string FID
        {
            get { return Request.QueryString["FID"]; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}