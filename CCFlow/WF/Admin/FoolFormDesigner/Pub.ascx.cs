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

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    public partial class WF_MapDef_Pub : BP.Web.UC.UCBase3
    {
        #region 变量
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string ExtType
        {
            get
            {
                string s = this.Request.QueryString["ExtType"];
                if (s == "")
                    s = null;
                return s;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}