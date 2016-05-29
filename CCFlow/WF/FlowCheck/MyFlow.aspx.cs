using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.FlowCheck
{
    public partial class MyFlow : BP.Web.WebPage
    {
        #region 参数.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                string nodeIDStr= this.Request.QueryString["FK_Node"];
                if (nodeIDStr==null)
                    nodeIDStr=this.Request.QueryString["NodeID"];

                if (nodeIDStr==null)
                    nodeIDStr=this.FK_Flow+"01";
                return int.Parse(nodeIDStr);
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
        #endregion 参数.

        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.Node nd = new Node(this.FK_Node);

            if (this.IsPostBack == false)
            {
                this.RB_OK.Attributes["onclick"] = "Check(this)";
                this.RB_UnOK.Attributes["onclick"] = "Check(this)";

                BP.WF.GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                gwf.RetrieveFromDBSources();


                if (nd.IsStartNode == true
                    || nd.IsCanReturn == false
                    || gwf.WFState == WFState.ReturnSta
                    || gwf.WFState == WFState.Askfor)
                {
                    /*如果是开始节点，加签回复状态，退回状态, 当前节点不允许退回状态.*/
                    this.RB_UnOK.Enabled = false;
                    this.DDL_Nodes.Visible = false;
                    this.DDL_ReturnSendModel.Visible = false;

                }
                else
                {
                    //获得可以退回的节点.
                    DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node, this.WorkID, this.FID);

                    //绑定到下拉框.
                    BP.Web.Controls.Glo.DDL_BindDataTable(this.DDL_Nodes, dt);
                    BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_ReturnSendModel, "ReturnSendModel", 0);
                }


                #region 判断当前节点是否有分支？



                #endregion 判断当前节点是否有分支？



            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            string checkMsg=this.TB_Doc.Text;

            BP.WF.GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            // 如果是发送.
            if (this.RB_OK.Checked)
            {
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);

                //写入审核信息.
                BP.WF.Dev2Interface.WriteTrack(this.FK_Flow, this.FK_Node, this.WorkID, this.FID,
                  checkMsg, ActionType.WorkCheck, "", null);

                this.ToMsg(objs.ToMsgOfHtml(), "Info");
                return;
            }

            //获得要退回的节点.
            int returnToNodeID = int.Parse(this.DDL_Nodes.SelectedValue);

            //执行退回.
            string msg= BP.WF.Dev2Interface.Node_ReturnWork(this.FK_Flow, this.WorkID, 
                this.FID, this.FK_Node, returnToNodeID, checkMsg, false);

            this.ToMsg(msg, "Info");
        }

        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);

            //if (this.PageID.Contains("Single") == true)
            //    this.Response.Redirect("MyFlowInfoSmallSingle.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
            //else if (this.PageID.Contains("Small"))
            //    this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
            //else
            //    this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
    }
}