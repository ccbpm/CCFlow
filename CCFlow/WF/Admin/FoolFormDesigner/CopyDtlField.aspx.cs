using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.En;
using BP.Sys;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_CopyDtlField : System.Web.UI.Page
    {
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
