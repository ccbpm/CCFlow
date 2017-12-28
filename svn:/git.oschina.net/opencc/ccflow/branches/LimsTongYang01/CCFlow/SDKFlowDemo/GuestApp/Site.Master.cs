using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.Web;

namespace CCFlow.SDKFlowDemo.GuestApp
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.Url.AbsolutePath;
            int index = path.LastIndexOf("/");
            int lastIndex = path.LastIndexOf(".");
            string id = path.Substring(index + 1, lastIndex - index - 1);

            if (string.IsNullOrWhiteSpace(BP.Web.GuestUser.No) && id.ToUpper() != "HOME")
                this.Response.Redirect("Login.aspx");

            try
            {
                DataTable dt = BP.WF.Dev2InterfaceGuest.DB_GenerEmpWorksOfDataTable(GuestUser.No);
                if (dt.Rows.Count == 0)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "hide", "hide();", true);
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "show", "show();", true);
            }
            catch 
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "hide", "hide();", true);
            }
          

            Page.ClientScript.RegisterStartupScript(this.GetType(), "indexhover", "navleftHover('" + id.ToUpper() + "');", true);
        }
    }
}