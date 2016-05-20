using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.AttrNode
{
    public partial class TurnToDeal : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                int nodeID = int.Parse(this.Request.QueryString["FK_Node"]);
                return nodeID;
            }
        }
        #endregion 属性.

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                BP.WF.Node nd = new BP.WF.Node();
                nd.NodeID = this.FK_Node;
                nd.RetrieveFromDBSources();

                switch (nd.HisTurnToDeal)
                {
                    case BP.WF.TurnToDeal.CCFlowMsg:
                        this.RB_CCFlowMsg.Checked = true;
                        break;
                    case BP.WF.TurnToDeal.SpecMsg:
                        this.RB_SpecMsg.Checked = true;
                        this.TB_SpecMsg.Text = nd.TurnToDealDoc;
                        break;
                    case BP.WF.TurnToDeal.SpecUrl:
                    case BP.WF.TurnToDeal.TurnToByCond:
                        this.RB_SpecUrl.Checked = true;
                        this.TB_SpecURL.Text = nd.TurnToDealDoc;
                        break;
                }
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            int nodeID = int.Parse(this.FK_Node.ToString());
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs("ND" + nodeID);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            //遍历页面radiobutton
            if (this.RB_CCFlowMsg.Checked)
            {
                nd.HisTurnToDeal = BP.WF.TurnToDeal.CCFlowMsg;
            }
            else if (this.RB_SpecMsg.Checked)
            {
                nd.HisTurnToDeal = BP.WF.TurnToDeal.SpecMsg;
                nd.TurnToDealDoc = this.TB_SpecMsg.Text;
            }
            else
            {
                nd.HisTurnToDeal = BP.WF.TurnToDeal.SpecUrl;
                nd.TurnToDealDoc = this.TB_SpecURL.Text;
            }
            //执行保存操作
            nd.Update();

            BP.Sys.PubClass.Alert("保存成功.");
        }
    }
}