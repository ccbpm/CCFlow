using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.AttrNode
{
    public partial class CHOvertimeRole : System.Web.UI.Page
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
                BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

                //绑定节点集合.
                BP.WF.Nodes nds = new BP.WF.Nodes(nd.FK_Flow);
                foreach (BP.WF.Node mynd in nds)
                {
                    this.DDL_Node.Items.Add(new ListItem(mynd.Name, mynd.NodeID.ToString()));
                }

                switch (nd.HisOutTimeDeal)
                {
                    case BP.WF.Template.OutTimeDeal.None:
                        this.RB_0.Checked = true;
                        break;
                    case BP.WF.Template.OutTimeDeal.AutoTurntoNextStep: //自动运行到下一步.
                        this.RB_1.Checked = true;
                        break;
                    case BP.WF.Template.OutTimeDeal.AutoJumpToSpecNode: // 自动转向指定的步骤
                        this.RB_2.Checked = true;
                        //设置当前的选择.
                        this.DDL_Node.SelectedValue = nd.DoOutTime;
                        break;
                    case BP.WF.Template.OutTimeDeal.AutoShiftToSpecUser: //移交给指定的人员.
                        this.RB_3.Checked = true;
                        this.TB_3_Shift.Text = nd.DoOutTime;
                        break;
                    case BP.WF.Template.OutTimeDeal.SendMsgToSpecUser: //向指定的人员发消息.
                        this.TB_4_SendMsg.Text = nd.DoOutTime;
                        this.RB_4.Checked = true;
                        break;
                    case BP.WF.Template.OutTimeDeal.DeleteFlow: //删除流程.
                        this.RB_5.Checked = true;
                        break;
                    case BP.WF.Template.OutTimeDeal.RunSQL: //运行SQL
                        this.RB_6.Checked = true;
                        this.TB_6_SQL.Text = nd.DoOutTime;
                        break;
                }
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            if (this.RB_0.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.None;
            }

            if (this.RB_1.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.AutoTurntoNextStep;
            }

            if (this.RB_2.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.AutoJumpToSpecNode;
                nd.DoOutTime = this.DDL_Node.SelectedValue;
            }

            if (this.RB_3.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.AutoShiftToSpecUser;
                nd.DoOutTime = this.TB_3_Shift.Text;
            }

            if (this.RB_4.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.SendMsgToSpecUser;
                nd.DoOutTime = this.TB_4_SendMsg.Text;
            }

            if (this.RB_5.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.DeleteFlow;
            }

            if (this.RB_6.Checked)
            {
                nd.HisOutTimeDeal = BP.WF.Template.OutTimeDeal.RunSQL;
                nd.DoOutTime = this.TB_6_SQL.Text;
            }

            //是否质量考核节点.
            nd.IsEval = this.CB_IsEval.Checked;

            //执行更新.
            nd.Update();

        }
    }
}