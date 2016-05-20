using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Data;
using BP.WF;
using BP.DA;

namespace CCFlow.WF.CH1234
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            float tspan = 7.5f;
            float mmsF = tspan * 60f;
            int mms = (int)mmsF;
            //DateTime dt = BP.WF.Glo.AddMinutes(DateTime.Now, mms);
            //  this.Response.Write(dt.ToString("yyyy-MM-dd HH:mm"));

            this.Response.Redirect("FlowCH.aspx", true);
        }
    }
}