using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;

namespace CCFlow.WF.Admin.AttrNode
{
    public partial class PushMessageEntity : System.Web.UI.Page
    {
        #region 属性.
        public int FK_Flow
        {
            get {
                return int.Parse(this.Request.QueryString["FK_Flow"]);
            }
        }

        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        /// <summary>
        /// 事件类型.
        /// </summary>
        public string FK_Event
        {
            get
            {
                return this.Request.QueryString["FK_Event"];
            }
        }
        #endregion 属性.


        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.Node nd  =new BP.WF.Node(this.FK_Node); 

            //判断节点消息事件类型，
            switch (this.FK_Event)
            {
                case BP.Sys.EventListOfNode.WorkArrive://1.节点到达时
                    this.RB_Email_1.Text = "发送给当前节点的所有处理人";
                    this.RB_SMS_1.Text = "发送给当前节点的所有处理人";
                    break;
                case BP.Sys.EventListOfNode.SendSuccess://2.节点发送成功时
                    this.RB_Email_1.Text = "发送给下一个节点的所有接受人";
                    this.RB_SMS_1.Text = "发送给下一个节点的所有接受人";
                    break;
                case BP.Sys.EventListOfNode.ReturnAfter://3.节点退回后
                    this.RB_Email_1.Text = "发送给被退回的节点处理人.";
                    this.RB_SMS_1.Text = "发送给被退回的节点处理人";
                    break;
                case BP.Sys.EventListOfNode.UndoneAfter://4.工作撤销后
                    this.RB_Email_1.Text = "撤销工作后通知的节点接受人";
                    this.RB_SMS_1.Text = "撤销工作后通知的节点接受人"; ;
                    break;

                case BP.Sys.EventListOfNode.FlowOverAfter://6.流程结束后
                    this.RB_Email_1.Text = "流程结束后通知的节点接受人";
                    this.RB_SMS_1.Text = "流程结束通知的节点接受人";
                    break;
                case BP.Sys.EventListOfNode.AfterFlowDel://7.流程删除后
                    this.RB_Email_1.Text = "流程删除后通知的节点接受人";
                    this.RB_SMS_1.Text = "流程删除通知的节点接受人";
                    break;
            }

            if (this.IsPostBack == true)
                return;

            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.FK_Event = this.FK_Event;
            msg.FK_Node = this.FK_Node;

            if (msg.RetrieveFromDBSources() == 0 && 
                (this.FK_Event ==BP.Sys.EventListOfNode.SendSuccess ||this.FK_Event ==BP.Sys.EventListOfNode.ReturnAfter)  )
            {
                /*如果是发送成功的消息没有被查询到. */
                msg.MailPushWay = 1;
                msg.SMSPushWay = 0;
            }

            #region 设置字段下拉框.
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs();
            int i = attrs.Retrieve(BP.Sys.MapAttrAttr.FK_MapData, "ND" + FK_Node);
            foreach (BP.Sys.MapAttr item in attrs)
            {
                if (item.LGType != BP.En.FieldTypeS.Normal)
                    continue;
                if (item.MyDataType != BP.DA.DataType.AppString)
                    continue;
                switch (item.KeyOfEn)
                {
                    case BP.WF.GEWorkAttr.Emps:
                    case BP.WF.GEWorkAttr.MD5:
                    case BP.WF.GEWorkAttr.Rec:
                    case BP.WF.GEWorkAttr.RecText:
                        break;
                    default:
                        break;
                }
                this.DDL_SMS_Fields.Items.Add(new ListItem(item.KeyOfEn + "  ; " + item.Name, item.KeyOfEn));
                this.DDL_Email.Items.Add(new ListItem(item.KeyOfEn + "  ; " + item.Name, item.KeyOfEn));
            }
            #endregion 设置字段下拉框.

            #region 短信内容设置。
            if (msg.SMSPushWay == 0)
                this.RB_SMS_0.Checked = true;
            if (msg.SMSPushWay == 1)
                this.RB_SMS_1.Checked = true;
            if (msg.SMSPushWay == 2)
                this.RB_SMS_2.Checked = true;
            if (msg.SMSPushWay == 3)
                this.RB_SMS_3.Checked = true;

            this.TB_SMS.Text = msg.SMSDoc;
            this.DDL_SMS_Fields.SelectedValue = msg.SMSField;
            #endregion

            #region 邮件内容设置。
            if (msg.MailPushWay == 0)
                this.RB_Email_0.Checked = true;
            if (msg.MailPushWay == 1)
                this.RB_Email_1.Checked = true;
            if (msg.MailPushWay == 2)
                this.RB_Email_2.Checked = true;
            if (msg.MailPushWay == 3)
                this.RB_Email_3.Checked = true;

            this.TB_Email_Title.Text = msg.MailTitle;
            this.TB_Email_Doc.Text = msg.MailDoc;
            this.DDL_Email.SelectedValue = msg.MailAddress;
            #endregion 邮件内容设置

            #region 绑定节点.
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);
            foreach (BP.WF.Node mynd in nds)
            {
                CheckBox cb = new CheckBox();
                cb.ID = "CB_SMS_" + mynd.NodeID;
                cb.Text = mynd.Name;
                cb.Checked = msg.SMSNodes.Contains(mynd.NodeID+"");
                this.Pub1.Add(cb);

                CheckBox cb2 = new CheckBox();
                cb2.ID = "CB_Email_" + mynd.NodeID;
                cb2.Text = mynd.Name;
                cb2.Checked = msg.MailNodes.Contains(mynd.NodeID + "");
                this.Pub2.Add(cb2);
            }
            #endregion 绑定节点

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            BP.WF.Template.PushMsg msg = new BP.WF.Template.PushMsg();
            msg.MyPK = this.MyPK;
            msg.RetrieveFromDBSources();

            msg.FK_Event = this.FK_Event;
            msg.FK_Node = this.FK_Node;

            BP.WF.Node nd=new BP.WF.Node(this.FK_Node);
            BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);

            #region 求出来选择的节点.
            string nodesOfSMS = "";
            string nodesOfEmail = "";
            foreach (BP.WF.Node mynd in nds)
            {
                foreach (string key in this.Request.Params.AllKeys)
                {
                    if (key.Contains("CB_SMS_" + mynd.NodeID) 
                        && nodesOfSMS.Contains( mynd.NodeID+"")==false )
                        nodesOfSMS += mynd.NodeID+",";

                    if (key.Contains("CB_Email_" + mynd.NodeID) 
                        && nodesOfEmail.Contains(mynd.NodeID + "") == false)
                        nodesOfEmail += mynd.NodeID + ",";
                }
            }

            //节点.
            msg.MailNodes = nodesOfEmail;
            msg.SMSNodes = nodesOfSMS;
            #endregion 求出来选择的节点.


            #region 短信保存.
            //短信推送方式。
            if (this.RB_SMS_0.Checked)
                msg.SMSPushWay = 0;

            if (this.RB_SMS_1.Checked)
                msg.SMSPushWay = 1;

            if (this.RB_SMS_2.Checked)
                msg.SMSPushWay = 2;

            if (this.RB_SMS_3.Checked)
                msg.SMSPushWay = 3;

            //短信手机字段.
            msg.SMSField = this.DDL_SMS_Fields.SelectedValue;
            //替换变量
            string smsstr = this.TB_SMS.Text;
            smsstr = smsstr.Replace("@WebUser.Name", BP.Web.WebUser.Name);
            smsstr = smsstr.Replace("@WebUser.No", BP.Web.WebUser.No);

            System.Data.DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
           // smsstr = smsstr.Replace("@RDT",);
            //短信内容模版.
            msg.SMSDoc_Real = smsstr;
            #endregion 短信保存.

            #region 邮件保存.
            //邮件.
            if (this.RB_Email_0.Checked)
                msg.MailPushWay = 0;

            if (this.RB_Email_1.Checked)
                msg.MailPushWay = 1;

            if (this.RB_Email_2.Checked)
                msg.MailPushWay = 2;

            if (this.RB_Email_3.Checked)
                msg.MailPushWay = 3;

            //邮件标题与内容.
            msg.MailTitle_Real = this.TB_Email_Title.Text;
            msg.MailDoc_Real = this.TB_Email_Doc.Text;

            //邮件地址.
            msg.MailAddress = this.DDL_Email.SelectedValue;

            #endregion 邮件保存.

            //保存.
            if (string.IsNullOrEmpty(msg.MyPK) == true)
            {
                msg.MyPK = BP.DA.DBAccess.GenerGUID();
                msg.Insert();
            }
            else
            {
                msg.Update();
            }

            //转向他.
            this.Response.Redirect("PushMessage.aspx?FK_Node=" + this.FK_Node + "&FK_Event=" + this.FK_Event + "&MyPK" + this.MyPK, true);
        }

        protected void Btn_Back_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("PushMessage.aspx?FK_Node=" + this.FK_Node + "&FK_Event=" + this.FK_Event + "&MyPK" + this.MyPK, true);
        }
    }
}