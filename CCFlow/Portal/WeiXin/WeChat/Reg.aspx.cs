﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.Admin.WeChat
{
    public partial class Reg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BP.Cloud.WeXinAPI.Glo.testFetchDept();
        }
    }
}