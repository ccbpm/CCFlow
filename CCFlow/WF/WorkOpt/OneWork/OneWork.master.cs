using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.XML;
using BP.Web;
using BP.Sys;
namespace CCFlow.WF.OneWork
{
    public partial class WF_WorkOpt_OneWork : BP.Web.MasterPage
    {
        #region Property

        public string WorkID
        {
            get
            {
                return this.Request.QueryString["WorkID"];
            }
        }
        public string FID
        {
            get
            {
                return this.Request.QueryString["FID"];
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            OneWorkXmls xmls = new OneWorkXmls();
            xmls.RetrieveAll();

            string pageId = this.PageID;

            foreach (OneWorkXml item in xmls)
            {
                string url = string.Format("{0}.aspx?FK_Node={1}&WorkID={2}&FK_Flow={3}&FID={4}", item.No, this.FK_Node, this.WorkID, this.FK_Flow, this.FID);

                Pub1.AddLi(string.Format("<div{2}><a href='{0}'><span class='nav'>{1}</span></a></div>{3}", url, item.Name, item.No == pageId ? "  class='selected'" : "", Environment.NewLine));
            }


        }
    }
}