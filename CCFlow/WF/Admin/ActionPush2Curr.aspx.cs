using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;
using BP.WF;
using BP.WF.XML;

namespace CCFlow.WF.Admin
{
    public partial class ActionPush2Curr : BP.Web.WebPage
    {
        #region 属性

        public string Event
        {
            get
            {
                return this.Request.QueryString["Event"];
            }
        }

        public string NodeID
        {
            get
            {
                return this.Request.QueryString["NodeID"];
            }
        }

        public string FK_MapData
        {
            get
            {
                return "ND" + this.Request.QueryString["NodeID"];
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
            FrmEvents ndevs = new FrmEvents();
            ndevs.Retrieve(FrmEventAttr.FK_MapData, this.FK_MapData);

            FrmEvent mynde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, this.Event) as FrmEvent;

            if (mynde == null)
            {
                mynde = new FrmEvent();
                mynde.FK_Event = this.Event;
                if (mynde.FK_Event == BP.Sys.EventListOfNode.SendSuccess)
                {
                    mynde.MsgCtrl = MsgCtrl.BySet;
                    mynde.FK_Event = EventListOfNode.SendSuccess;
                    mynde.MailEnable = true;
                }
            }

            this.Pub1.AddTable("class='Table' cellspacing='1' cellpadding='1' border='1' style='width:100%'");
            
            this.Pub1.AddTR();
            this.Pub1.AddTD("控制方式");
            var ddl = new DDL();
            ddl.BindSysEnum("MsgCtrl");
            ddl.ID = "DDL_" + FrmEventAttr.MsgCtrl;
            ddl.SetSelectItem((int)mynde.MsgCtrl);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("");
            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + FrmEventAttr.MailEnable;
            cb.Text = "是否启用邮件通知？";
            cb.Checked = mynde.MailEnable;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("邮件标题模版");
            var tb = new TextBox();
            tb.ID = "TB_" + FrmEventAttr.MailTitle;
            tb.Text = mynde.MailTitle;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("邮件内容模版:");
            tb = new TextBox();
            tb.ID = "TB_" + FrmEventAttr.MailDoc;
            tb.Text = mynde.MailDoc;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Columns = 50;
            tb.Rows = 7;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            //手机短信....
            this.Pub1.AddTR();
            this.Pub1.AddTD("默认:不启用");
            cb = new CheckBox();
            cb.ID = "CB_" + FrmEventAttr.SMSEnable;
            cb.Text = "是否启用手机短信通知？";
            cb.Checked = mynde.SMSEnable;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();

            tb = new TextBox();
            tb.ID = "TB_" + FrmEventAttr.SMSDoc;
            tb.Text = mynde.SMSDoc_Real;
            tb.Style.Add("width", "99%");
            tb.Rows = 2;
            this.Pub1.AddTR();
            if (string.IsNullOrEmpty(tb.Text) == true)
                this.Pub1.AddTD("短信模版:");
            else
                this.Pub1.AddTD("短信模版");

            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("默认:启用");
            cb = new CheckBox();
            cb.ID = "CB_" + FrmEventAttr.MobilePushEnable;
            cb.Text = "是否启用手机应用，平板应用信息推送？";
            cb.Checked = mynde.MobilePushEnable;
            this.Pub1.AddTD(cb);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            Pub1.AddBR();
            Pub1.AddSpace(1);

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Click);
            Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            FrmEvent fe = new FrmEvent();
            fe.MyPK = this.FK_MapData + "_" + this.Event;
            fe.RetrieveFromDBSources();

            fe = (FrmEvent)this.Pub1.Copy(fe);
            fe.Save();

            //var pm = new PushMsg();
            //pm.Retrieve(PushMsgAttr.FK_Event, this.Event, PushMsgAttr.FK_Node, this.NodeID);

            this.Response.Redirect("ActionPush2Curr.aspx?NodeID=" + this.NodeID + "&MyPK=" + fe.MyPK + "&Event=" + this.Event + "&tk=" + new Random().NextDouble(), true);
        }
    }
}