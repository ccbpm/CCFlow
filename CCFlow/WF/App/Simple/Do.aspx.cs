using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.App.Simple
{
    public partial class Do : System.Web.UI.Page
    {
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "ViewStartSrc":
                    string path = BP.Sys.SystemConfig.PathOfWebApp + "\\WF\\App\\Simple\\Start.aspx";
                    this.Response.Write(   BP.DA.DataType.ReadTextFile(path));
                    break;
                default:
                    break;
            }

        }
    }
}