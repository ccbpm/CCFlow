using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
namespace CCFlow.WF.CCForm
{
    public partial class WF_WorkOpt_Draw : BP.Web.WebPage
    {
        public string PKVal
        {
            get
            {
                return this.Request.QueryString["PKVal"];
            }
        }
        public string H
        {
            get
            {
                return this.Request.QueryString["H"];
            }
        }
        public string W
        {
            get
            {
                return this.Request.QueryString["W"];
            }
        }
        public string initParams { set; get; }
        protected void Page_Load(object sender, EventArgs e)
        {
            FrmEle ele = new FrmEle(this.MyPK);
            string path = ele.HandSigantureSavePath;
            string fk_mapdata = ele.FK_MapData;
            this.initParams = "mypk=" + this.MyPK + ",pkval=" + PKVal + ",H=" + ele.HandSiganture_WinOpenH + ",W=" + ele.HandSiganture_WinOpenW;
        }
    }
}