﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Concurrent;
using System.IO;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;
using BP.Port;
using ThoughtWorks.QRCode.Codec;

namespace CCFlow
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //BP.DA.Log.DefaultLogWriteLine(LogType.Info, "----------------------------------  start ------------------ ");
            //BP.DA.Log.DefaultLogWriteLine(LogType.Info, "----------------------------------  end ------------------ ");

            if (DBAccess.IsExitsObject("WF_Flow") == false)
                this.Response.Redirect("./WF/Admin/DBInstall.htm", true);
            else
                this.Response.Redirect("./Portal/Standard/Login.htm", true);
            return;
        }
    }
}