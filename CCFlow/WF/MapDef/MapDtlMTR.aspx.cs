using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_MapDtlMTR : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.MyPK);
            this.Title = "从表头";
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("请书写html标记,以《TR》开头，以《/TR》结尾。");
            this.Pub1.AddTR();
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 15;
            tb.Columns = 60;
            tb.Text = dtl.MTR;
            //  tb.Attributes["onblur"] = "Rep('"+tb.ClientID+"')";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = "保存";
            btn.OnClientClick += "javascript:Rep();";
            // btn.Attributes["onclick"] = "return Rep('" + tb.ClientID + "');";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }
        void btn_Click(object sender, EventArgs e)
        {
            MapDtl dtl = new MapDtl(this.MyPK);
            dtl.MTR = this.Pub1.GetTextBoxByID("TB_Doc").Text;
            dtl.Update();
            this.WinClose();
        }
    }
}