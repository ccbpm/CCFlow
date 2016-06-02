using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.Web.Controls;
using BP.WF;
using BP.Web;
using BP.DA;
using System.Data;

namespace CCFlow.WF
{
    public partial class MessagesReplay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string receiver = this.Request.QueryString["RE"];
            string mypk = this.Request.QueryString["MyPK"];

            BP.WF.SMS sms = null;
           if (mypk != null)
           {
               sms = new SMS();
               sms.MyPK = mypk;
               sms.RetrieveFromDBSources();
           }

            this.Pub1.AddTable(); // (" id='recTable' class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%;margin-left:auto;margin-right:auto;' ");
            this.Pub1.AddCaptionMsg("消息回复"); //("<caption><div class='CaptionMsg'>消息</div></caption>");
            this.Pub1.AddTR();
            this.Pub1.AddTD("接受人：");

            TB tb = new TB();
            tb.ID = "rec";
            tb.Width = 430;
            if (!string.IsNullOrEmpty(receiver) && !string.IsNullOrEmpty(mypk))
            {
                Emp emp = new Emp(receiver);
                tb.Text = emp.Name;
                tb.ReadOnly = true;
                this.Pub1.AddTD(tb);
            }
            else
            {
                this.Pub1.AddTDBegin();
                this.Pub1.Add(tb);

                HiddenField hid = new HiddenField();
                hid.ID = "Hid_FQR";
                Pub1.Add(hid);

                this.Pub1.Add("<a onclick=\"openSelectEmp('" + hid.ClientID + "','" + tb.ClientID + "')\" href='javascript:;'>添加人员</a>");
                this.Pub1.AddTDEnd();
            }
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTD("标题");
            tb = new TB();
            tb.ID = "title";
            if (sms!= null)
              tb.Text = "RE:" + sms.Title;

            tb.Width = 430;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTD("正文");
            tb = new TB();
            tb.ID = "con";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Width = 430;
            tb.Height = 120;

            if (sms != null)
              tb.Text = "\t\n ------------------ \t\n " + sms.DocOfEmail;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDBegin(" colspan=2 ");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "发送";
            btn.Click += new EventHandler(btn_Save_Click);

            Button btnClose = new Button();
            btnClose.ID = "Btn_Close";
            btnClose.Text = "取消";
            btnClose.Click += new EventHandler(btnClose_Click);

            this.Pub1.Add(btn);
            this.Pub1.Add(btnClose);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            this.Pub1.AddTableEnd();

        }

        public void btn_Save_Click(object sender, EventArgs e)
        {

            string receiver = this.Request.QueryString["RE"];
            string MyPK = this.Request.QueryString["MyPK"];

            SMS sms = null;

            string title = this.Pub1.GetTBByID("title").Text;
            string doc = this.Pub1.GetTBByID("con").Text;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(doc))
            {
                BP.Sys.PubClass.Alert("标题和内容不可以为空！");
                return;
            }

            if (!string.IsNullOrEmpty(receiver) && !string.IsNullOrEmpty(MyPK))
            {
                sms = new SMS();
                sms.RetrieveByAttr(SMSAttr.MyPK, MyPK);

                sms.MyPK = DBAccess.GenerGUID();
                sms.RDT = DataType.CurrentDataTime;  
                sms.Sender = WebUser.No;
                sms.SendToEmpNo = sms.Sender;

                sms.Title = title;
                sms.DocOfEmail = doc;
                sms.Insert();

            }
            else
            {
                string emps = (this.Pub1.FindControl("Hid_FQR") as HiddenField).Value;

                if (string.IsNullOrEmpty(emps))
                {
                    BP.Sys.PubClass.Alert("接受人不可以为空！");
                    return;
                }
                string[] empArr = emps.Split(',');

                foreach (string emp in empArr)
                {
                    if (string.IsNullOrEmpty(emp))
                        continue;

                    sms = new SMS();

                    sms.MyPK = DBAccess.GenerGUID();
                    sms.RDT = DataType.CurrentDataTime;  
                    sms.Sender = WebUser.No;
                    sms.SendToEmpNo = emp;
                    sms.Title = title;
                    sms.DocOfEmail = doc;
                    //sms.MsgType = SMSMsgType.ToDo;
                    sms.Insert();
                   // BP.Sys.PubClass.WinClose();
                }
            }

            //发送转向.
            this.Response.Redirect("Messages.aspx", true);
            //BP.Sys.PubClass.WinClose();
        }

        public void btnClose_Click(object sender, EventArgs e)
        {
            Response.Write("<script language=javascript>history.go(-2);</script>");
        }
    }
}