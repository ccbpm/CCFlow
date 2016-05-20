using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;
namespace CCFlow.WF.Admin
{

    public partial class WF_Admin_CanReturnNodes : BP.Web.WebPage
    {
        #region 属性
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
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
            BP.WF.Node mynd = new BP.WF.Node();
            mynd.NodeID = this.FK_Node;
            mynd.RetrieveFromDBSources();

            this.Pub1.AddTable("width='100%'");
            this.Pub1.AddCaptionLeft("为“" + mynd.Name + "”, 设置可退回的节点。");

            NodeReturns rnds = new NodeReturns();
            rnds.Retrieve(NodeReturnAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            int idx = 0;
            foreach (BP.WF.Node nd in nds)
            {
                if (nd.NodeID == this.FK_Node)
                    continue;

                CheckBox cb = new CheckBox();
                cb.Text = nd.Name;
                cb.ID = "CB_" + nd.NodeID;
                cb.Checked = rnds.IsExits(NodeReturnAttr.ReturnTo, nd.NodeID);

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("第" + nd.Step + "步");
                this.Pub1.AddTD(cb);
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD();
            Button btn = new Button();
            btn.Text = "保存";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.AddTD(btn);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEndWithHR();
            this.Pub1.AddFieldSet("特别说明:", "1，只有节点属性的退回规则被设置成退回制订的节点，此功能才有效。<br> 2，设置退回的节点如果是当前节点下一步骤的节点，设置无意义，系统不做检查，退回时才做检查。");
        }

        void btn_Click(object sender, EventArgs e)
        {

            NodeReturns rnds = new NodeReturns();
            rnds.Delete(NodeReturnAttr.FK_Node, this.FK_Node);

            BP.WF.Nodes nds = new Nodes();
            nds.Retrieve(BP.WF.Template.NodeAttr.FK_Flow, this.FK_Flow);

            int i = 0;
            foreach (BP.WF.Node nd in nds)
            {
                CheckBox cb = this.Pub1.GetCBByID("CB_" + nd.NodeID);
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;

                NodeReturn nr = new NodeReturn();
                nr.FK_Node = this.FK_Node;
                nr.ReturnTo = nd.NodeID;
                nr.Insert();
                i++;
            }
            if (i == 0)
            {
                this.Alert("请您选择要退回的节点。");
                return;
            }
            this.WinCloseWithMsg("设置成功");
        }
    }
}