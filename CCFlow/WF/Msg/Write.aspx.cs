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
using BP.Sys;
namespace CCFlow.WF.Msg
{
    public partial class WF_Msg_Write : BP.Web.WebPage
    {
        public Int64 WorkID
        {
            get
            {
                try
                {

                    return Int64.Parse(this.Request.QueryString["WorkID"]);

                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FK_Node
        {
            get
            {
                try
                {

                    return int.Parse(this.Request.QueryString["FK_Node"]);

                }
                catch
                {
                    return 0;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.SMS msg = new BP.WF.SMS();
            switch (this.DoType)
            {
                case "Re":
                    //msg.MyPK = this.RefOID;
                    //if (msg.Accepter != WebUser.No)
                    //{
                    //    //return;
                    //}
                    //msg.Title = "RE:" + msg.Title;
                    //msg.Doc = " ----------------- " + msg.Doc;
                    //msg.Accepter = msg.Sender;
                    break;
                default:
                    if (this.WorkID != 0 && this.FK_Node != 0)
                    {
                        Node nd = new Node(this.FK_Node);
                        Work wk = nd.HisWork;
                        wk.OID = this.WorkID;
                        wk.Retrieve();

                        string msgInfo = "\t\n ************** 工作信息 **************";
                        Attrs attrs = wk.EnMap.Attrs;
                        foreach (Attr attr in attrs)
                        {
                            if (attr.UIVisible == false)
                                continue;

                            if (attr.IsFKorEnum)
                                continue;

                            msgInfo += "\t\n" + attr.Desc + ": " + wk.GetValStrByKey(attr.Key);
                        }
                        msg.DocOfEmail = msgInfo;
                    }
                    break;
            }
            this.Pub1.AddTable("width='95%'");
            if (WebUser.IsWap)
                this.Pub1.AddCaptionLeft("<a href='./../Home.aspx' ><img src='./../Img/Home.gif' border=0>Home</a> - <a href='./../../WAP/Msg.aspx' >列表</a>");
            //else
            //    this.Pub1.AddCaption("&nbsp;&nbsp;&nbsp;信息发送");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("width=10%", "<b>接受人</b>");
            TextBox tb = new TextBox();
            tb.ID = "TB_Emps";
            tb.Columns = 80;
            tb.Text = msg.SendToEmpNo;
            tb.Attributes["Width"] = "100%";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("width=10%", "<b>标题</b>");
            tb = new TextBox();
            tb.ID = "TB_Title";
            tb.Columns = 80;
            tb.Text = msg.Title;
            tb.Attributes["Width"] = "100%";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Rows = 15;
            tb.Columns = 70;
            tb.Text = msg.DocOfEmail;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Attributes["Width"] = "100%";
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            Button btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_Send";
            btn.Text = "  发 送  ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.AddTD("colspan=2", btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        void btn_Click(object sender, EventArgs e)
        {
            BP.WF.SMS msg = new BP.WF.SMS();
            msg.Title = this.Pub1.GetTextBoxByID("TB_Title").Text;
            msg.DocOfEmail = this.Pub1.GetTextBoxByID("TB_Doc").Text;
            msg.Sender = WebUser.No;
            msg.RDT = DataType.CurrentDataTime;

            string acces = this.Pub1.GetTextBoxByID("TB_Emps").Text.Trim();
            if (acces.Length == 0)
                throw new Exception("请输入或者选择接受人.");

            if (msg.Title.Length == 0)
                throw new Exception("请输入标题.");

            acces = acces.Replace(";", ",");
            acces = acces.Replace(";;", ",");
            acces = acces.Replace(" ", ",");
            acces = acces.Replace(",,", ",");

            string[] strs = acces.Split(',');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                msg.SendToEmpNo = str;
                msg.MyPK = DBAccess.GenerOID().ToString();
                msg.Insert();
            }

            BP.DA.Paras ps = new BP.DA.Paras();
            ps.Add("Sender", WebUser.No);
            ps.Add("Receivers", msg.SendToEmpNo);
            ps.Add("Title", msg.Title);
            ps.Add("Context", msg.DocOfEmail);
            try
            {
                DBAccess.RunSP("CCstaff", ps);
            }
            catch (Exception ex)
            {
                this.ToWFMsgPage("发送消息出现错误:" + ex.Message);
                return;
            }
            this.ToWFMsgPage("您的信息已经成功的发送到:" + acces);
        }
    }

}