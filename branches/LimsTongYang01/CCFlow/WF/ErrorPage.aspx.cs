using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;

namespace CCFlow.WF
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string s = Request["Message"];

            var message = Application["gloError"];
            if (message != null)
            {
                s = message.ToString();
            }

            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(s))
                {
                    s = Application["info" + WebUser.No] as string;
                }
                if (string.IsNullOrEmpty(s))
                    s = this.Session["info"] as string;

                if (string.IsNullOrEmpty(s))
                    s = "提示信息丢失了.";
            }
            string sUrl = Request["MessageUrl"];
            if (string.IsNullOrEmpty(sUrl))
            {
                sUrl = Application["url"].ToString();
            }

            errorUrl.InnerText = sUrl;

            errorMessage.InnerText = s;

        }
    }
}