using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Sys;
using BP.WF.Rpt;

namespace CCFlow.WF.MapDef.Rpt
{
    public partial class NewOrEdit : BP.Web.WebPage
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];

            }
        }
        public string RptNo
        {
            get
            {
                return this.Request.QueryString["RptNo"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];

            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            MapRpt rpt = new MapRpt();
            if (this.RptNo != null)
            {
                /**/
                rpt.No = this.RptNo; ;
                rpt.RetrieveFromDBSources();
            }

            this.TB_No.Text = rpt.No;
            this.TB_Name.Text = rpt.Name;
            this.TB_Note.Text = rpt.Note;

            if (string.IsNullOrEmpty(rpt.No) == false)
                this.TB_No.Enabled = false;
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Save();

            Response.Redirect(string.Format("S1_Edit.aspx?FK_MapData={0}&FK_Flow={1}&RptNo={2}&s={3}", FK_MapData,
                                            FK_Flow, RptNo, DateTime.Now.ToString("yyyyMMddHHmmssffffff")), true);
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }

        private void Save()
        {
            MapRpt rpt = new MapRpt();
            rpt = BP.Sys.PubClass.CopyFromRequest(rpt) as MapRpt;
            if (this.RptNo != null)
                rpt.No = this.RptNo;

            Flow fl = new Flow(this.FK_Flow);
            rpt.PTable = fl.PTable;
            rpt.Save();

            //if (string.IsNullOrWhiteSpace(this.RptNo))
            //{
            //    if (rpt.IsExits)
            //    {
            //        BP.Sys.PubClass.Alert("@该编号已经存在:" + rpt.No);
            //        return;
            //    }
            //    rpt.Insert();
            //}
            //else
            //{
            //    rpt.Update();
            //}
        }

        protected void Btn_SaveAndNext1_Click(object sender, EventArgs e)
        {
            Save();

            Response.Redirect(string.Format("S2_ColsChose.aspx?FK_MapData={0}&FK_Flow={1}&RptNo={2}&s={3}", FK_MapData,
                                            FK_Flow, RptNo, DateTime.Now.ToString("yyyyMMddHHmmssffffff")), true);
        }
    }
}