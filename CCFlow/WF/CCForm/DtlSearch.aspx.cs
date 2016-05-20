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
using BP.DA;
using BP.Sys;
namespace CCFlow.WF.CCForm
{
    public partial class WF_DtlSearch : BP.Web.WebPage
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
            MapDtls dtls = new MapDtls("ND" + int.Parse(this.FK_Flow) + "Rpt");
            if (dtls.Count == 0)
            {
                this.WinCloseWithMsg("该流程下没有从表。");
                return;
            }

            if (dtls.Count == 1)
            {
                this.Response.Redirect("../Comm/Search.aspx?EnsName=" + dtls[0].GetValStrByKey("No"), true);
                return;
            }

            this.Pub1.AddFieldSet("请选择您要查看的从表。");

            this.Pub1.AddUL();
            foreach (MapDtl dtl in dtls)
            {
                this.Pub1.AddLi("../Comm/Search.aspx?EnsName=" + dtls[0].GetValStrByKey("No"), dtl.Name);
            }
            this.Pub1.AddULEnd();
            this.Pub1.AddFieldSetEnd();
        }
    }

}