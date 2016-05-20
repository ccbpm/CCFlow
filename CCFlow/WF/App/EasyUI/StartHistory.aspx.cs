using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP;
using BP.WF;

namespace CCFlow.AppDemoLigerUI
{
    public partial class StartHistory : System.Web.UI.Page
    {
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
          
        }
    }
}