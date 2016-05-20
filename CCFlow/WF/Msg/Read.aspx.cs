using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.Msg
{
    public partial class WF_Msg_Read : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //BP.Sys.SMS msg = new BP.Sys.SMS(this.RefOID);
            //if (msg.Accepter != WebUser.No)
            //    return;

            //if (msg.HisMsgSta == BP.Sys.MsgSta.UnRead)
            //{
            //    msg.HisMsgSta = BP.Sys.MsgSta.Read;
            //    msg.Update();
            //}

            //this.Pub1.AddTable("width='95%'");
            //this.Pub1.AddTR();
            //this.Pub1.AddTD("width=10%", "发送人");
            //TextBox tb = new TextBox();
            //tb.ID = "TB_Emps";
            //tb.Columns = 80;
            //tb.Text = msg.SenderText;
            //this.Pub1.AddTD(tb);
            //this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //this.Pub1.AddTD("width=10%", "标题");
            //tb = new TextBox();
            //tb.ID = "TB_Title";
            //tb.Columns = 80;
            //tb.Text = msg.Title;
            //this.Pub1.AddTD(tb);
            //this.Pub1.AddTREnd();

            //this.Pub1.AddTR();
            //tb = new TextBox();
            //tb.ID = "TB_Doc";
            //tb.Rows = 15;
            //tb.Columns = 70;
            //tb.TextMode = TextBoxMode.MultiLine;
            //tb.Text = msg.Doc;
            //this.Pub1.AddTD("colspan=2", tb);
            //this.Pub1.AddTREnd();


            //this.Pub1.AddTR();
            //tb = new TextBox();
            //tb.ID = "TB_DocRe";
            //tb.Rows = 5;
            //tb.Columns = 70;
            //tb.TextMode = TextBoxMode.MultiLine;
            //tb.Text = msg.Doc;
            //this.Pub1.AddTD("colspan=2", tb);
            //this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();


            this.Pub1.AddTD("colspan=2", "<a href='Write.aspx?DoType=Re&RefOID=" + this.RefOID + "'>回复信息</a>");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
        }
    }

}