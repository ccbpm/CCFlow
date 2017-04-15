using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.AttrNode
{
    public partial class BlockModelUI : System.Web.UI.Page
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                BP.WF.Node nd = new BP.WF.Node();
                nd.NodeID = this.FK_Node;
                nd.RetrieveFromDBSources();

                this.TB_Alert.Text = nd.BlockAlert; //提示信息.

                switch (nd.BlockModel)
                {
                    case BP.WF.BlockModel.None:
                        this.RB_None.Checked = true;
                        break;
                    case BP.WF.BlockModel.CurrNodeAll:
                        this.RB_CurrNodeAll.Checked = true;
                        break;
                    case BP.WF.BlockModel.SpecSubFlow:
                        this.RB_SpecSubFlow.Checked = true;
                        this.TB_SpecSubFlow.Text = nd.BlockExp;
                        break;
                    case BP.WF.BlockModel.BySQL:
                         this.RB_SQL.Checked=true;
                        this.TB_SQL.Text = nd.BlockExp;
                        break;
                    case BP.WF.BlockModel.ByExp:
                        this.RB_Exp.Checked=true;
                        this.TB_Exp.Text = nd.BlockExp;
                        break;
                    default:
                        break;
                }
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            nd.BlockAlert = this.TB_Alert.Text ; //提示信息.

            if (this.RB_None.Checked)
                nd.BlockModel = BP.WF.BlockModel.None;

            if (this.RB_CurrNodeAll.Checked)
            {
                nd.BlockModel = BP.WF.BlockModel.CurrNodeAll;
            }

            if (this.RB_SpecSubFlow.Checked)
            {
                nd.BlockModel = BP.WF.BlockModel.SpecSubFlow;
                nd.BlockExp = this.TB_SpecSubFlow.Text;
            }

            if (this.RB_SQL.Checked)
            {
                nd.BlockModel = BP.WF.BlockModel.BySQL;
                nd.BlockExp = this.TB_SQL.Text;
            }

            if (this.RB_Exp.Checked)
            {
                nd.BlockModel = BP.WF.BlockModel.ByExp;
                nd.BlockExp = this.TB_Exp.Text;
            }

            nd.BlockAlert = this.TB_Alert.Text;
            nd.Update();
        }
    }
}