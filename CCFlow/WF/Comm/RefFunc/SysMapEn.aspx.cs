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
using BP.Port;
using BP.Sys.XML;
using BP;
using BP.Web;
using BP.Web.Controls;
using BP.En;
using BP.Sys;
using BP.DA;

namespace CCFlow.WF.Comm.RefFunc
{
    public partial class SysMapEnUI : System.Web.UI.Page
    {
        string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        string PK
        {
            get
            {
                return this.Request.QueryString["PK"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            en.PKVal = this.PK;
            en.RetrieveFromDBSources();

           
                this.SysMapEnUC1.BindColumn4(en, this.EnsName);
            this.Title = en.EnDesc;
        }
    }
}
