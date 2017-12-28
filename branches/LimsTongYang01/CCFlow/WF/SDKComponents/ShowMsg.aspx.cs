using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ShenZhenGovOA.WF.SDKComponents
{
    public partial class ShowMsg : System.Web.UI.Page
    {
        public string Info
        {
            get
            {
                string msg = this.Request.QueryString["Msg"];
                return msg;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}