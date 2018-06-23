using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.XML;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.WorkOpt
{
    public partial class WF_WorkOpt_WinOpen : System.Web.UI.MasterPage
    {
        public string WorkID
        {
            get
            {
                return this.Request.QueryString["WorkID"];
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Title = "工作选项";
            this.Page.RegisterClientScriptBlock("s",
        "<link href='../../Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            WorkOptXmls xmls = new WorkOptXmls();
            xmls.RetrieveAll();

            this.Pub1.Add("\t\n<div id='tabsJ'  align='center'>");
            this.Pub1.Add("\t\n<ul>");
            foreach (BP.WF.XML.WorkOptXml item in xmls)
            {
                string url = item.URL;
                url = url.Replace("~", "&");
                url = url.Replace("@WorkID", this.WorkID);
                url = url.Replace("@FK_Node", this.FK_Node);
                this.Pub1.AddLi("<a href=\"" + url + "\" ><span>" + item.Name + "</span></a>");
            }
            this.Pub1.Add("\t\n</ul>");
            this.Pub1.Add("\t\n</div>");
        }
    }

}