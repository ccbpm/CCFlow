using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace CCFlow.AppDemoLigerUI
{
    public class BasePage : Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (BP.Web.WebUser.No == null)
            {
                this.Response.Redirect("Login.aspx", true);
            }
            base.OnInit(e);
        }
        /// <summary>
        /// 不用page 参数，show message
        /// </summary>
        /// <param name="mess"></param>
        protected void Alert(string mess, bool isClent)
        {
            if (string.IsNullOrEmpty(mess))
                return;

            mess = mess.Replace("@@", "@");
            mess = mess.Replace("@@", "@");

            mess = mess.Replace("'", "＇");

            mess = mess.Replace("\"", "＇");

            mess = mess.Replace("\"", "＂");

            mess = mess.Replace(";", "；");
            mess = mess.Replace(")", "）");
            mess = mess.Replace("(", "（");

            mess = mess.Replace(",", "，");
            mess = mess.Replace(":", "：");

            mess = mess.Replace("<", "［");
            mess = mess.Replace(">", "］");

            mess = mess.Replace("[", "［");
            mess = mess.Replace("]", "］");

            mess = mess.Replace("@", "\\n@");
            string script = "<script language=JavaScript>alert('" + mess + "');</script>";
            if (isClent)
                System.Web.HttpContext.Current.Response.Write(script);
            else
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "kesy", script);
        }
        protected void Alert(Exception ex)
        {
            this.Alert(ex.Message, false);
        }
    }
}