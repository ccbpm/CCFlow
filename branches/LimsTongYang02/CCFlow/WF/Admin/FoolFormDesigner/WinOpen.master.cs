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

namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_WinOpen : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (BP.Web.WebUser.No != "admin")
            //    throw new Exception("@非法的用户必须由admin才能操作。");
        }
    }
}
