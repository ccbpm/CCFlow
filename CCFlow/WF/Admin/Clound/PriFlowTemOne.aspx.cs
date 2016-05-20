using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.Clound
{
    public partial class PriFlowTemOne : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.CloudWS.WSSoapClient ccflowCloud = BP.WF.Cloud.Glo.GetSoap();
            try
            {
                ccflowCloud.GetNetState();
            }
            catch (Exception)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>netInterruptJs();</script>");
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(BP.WF.Cloud.CCLover.UserNo) ||
                  string.IsNullOrWhiteSpace(BP.WF.Cloud.CCLover.Password) ||
                  string.IsNullOrWhiteSpace(BP.WF.Cloud.CCLover.GUID))
                {
                    this.Response.Redirect("RegUser.aspx");
                }
            }
            catch (Exception)
            {
                this.Response.Redirect("RegUser.aspx");
            }
        }
    }
}