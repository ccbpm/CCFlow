using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.En;
using BP.WF.XML;

namespace CCFlow.WF.Rpt
{
    public partial class WF_Rpt_MasterPage : BP.Web.MasterPage
    {
        #region 属性.
        public string FK_MapRpt
        {
            get
            {
                string s = this.Request.QueryString["FK_MapRpt"];
                if (string.IsNullOrEmpty(s))
                    return "ND" + int.Parse(this.FK_Flow) + "MyRpt";
                return s;
            }
        }
        public string FK_Flow
        {
            get
            {
                string s= this.Request.QueryString["FK_Flow"];
                if (string.IsNullOrEmpty(s))
                    return "068";
                return s;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("sds",
            "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            RptXmls xmls = new RptXmls();
            xmls.RetrieveAll();
        }
    }
}