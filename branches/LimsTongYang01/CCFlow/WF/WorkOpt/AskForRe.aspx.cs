using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BP.DA;
using BP.WF;
using BP.Sys;
using BP.Web;
namespace CCFlow.WF.WorkOpt
{
    public partial class AskForRe : System.Web.UI.Page
    {
        #region 参数
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Columns = 50;
            tb.Rows = 10;
            tb.TextMode = TextBoxMode.MultiLine;

            //获得加签意见.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            tb.Text = gwf.Paras_AskForReply; 

            this.Pub1.Add(tb);
            this.Pub1.AddBR();

            Button btn = new Button();
            btn.Text = "提交加签回复意见";
            btn.ID = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.Text = "取消/返回";
            btn.ID = "Btn_Cancel";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ID == "Btn_Cancel")
            {
                this.Response.Redirect("../MyFlow.aspx?FK_Flow="+this.FK_Flow+"&FK_Node="+this.FK_Node+"&WorkID="+this.WorkID+"&FID="+this.FID,true);
                return;
            }

            string replay= this.Pub1.GetTextBoxByID("TB_Doc").Text;
            string info = BP.WF.Dev2Interface.Node_AskforReply(this.WorkID, replay);

            this.ToMsg(info, "Info");
        }

        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }

    }
}