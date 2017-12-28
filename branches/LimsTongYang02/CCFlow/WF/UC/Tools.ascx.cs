using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Web;
using System.IO;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.UC
{
    public partial class Tools : BP.Web.UC.UCBase3
    {
        public string _PageSamll = null;
        public string PageSmall
        {
            get
            {
                if (_PageSamll == null)
                {
                    if (this.PageID.ToLower().Contains("small"))
                        _PageSamll = "Small";
                    else
                        _PageSamll = "";
                }
                return _PageSamll;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
          

         
        }
    }

}