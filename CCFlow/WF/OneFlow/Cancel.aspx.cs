using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.DA;


namespace CCFlow.WF.OneFlow
{

    public partial class WF_OneFlow_Cancel : System.Web.UI.Page
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
            string sql = "SELECT OID,Title,FlowStarter,FlowStartRDT,FlowEnder FROM ND" + int.Parse(this.FK_Flow) + "Rpt WHERE WFState=0";

        }
    }
}