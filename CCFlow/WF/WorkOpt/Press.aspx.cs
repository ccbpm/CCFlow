using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.WF;

namespace CCFlow.WF.WorkOpt
{
    public partial class WF_WorkOpt_Press : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int workid = int.Parse(this.Request.QueryString["WorkID"]);
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            //this.Pub1.AddFieldSet("请输入催办消息-" + gwf.Title);
            this.Pub1.AddFieldSet("请输入催办消息" );
            TextBox tb = new TextBox();
            tb.Rows = 7;
            tb.Columns = 40;
            tb.ID = "TB_Msg";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Text = "您好:" + gwf.TodoEmps.Replace(";",":") + " \t\n \t\n  此工作需要您请尽快处理.... \t\n \t\n致! \t\n \t\n   " + WebUser.Name + " " + BP.DA.DataType.CurrentDataCNOfShort;

            this.Pub1.Add(tb);
            this.Pub1.AddBR();
            Button btn = new Button();
            btn.ID = "Btn_Send";
            btn.Text = " 催办 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_Cancel";
            btn.Text = " 取消 ";
            btn.Attributes["onclick"] = "window.close();";
            this.Pub1.Add(btn);
            this.Pub1.AddFieldSetEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            string msg = this.Pub1.GetTextBoxByID("TB_Msg").Text;
            Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
            string info = BP.WF.Dev2Interface.Flow_DoPress(workid, msg, true);
            this.WinCloseWithMsg(info);
        }
    }
}