using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class CHRoleUI : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

                this.TB_TimeLimit.Text = nd.TimeLimit.ToString();
                this.TB_TSpanHour.Text = nd.TSpanHour.ToString();

                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_TAlertRole, "CHAlertRole", (int)nd.TAlertRole);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_TAlertWay, "CHAlertWay", (int)nd.TAlertWay);

                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_WAlertRole, "CHAlertRole", (int)nd.WAlertRole);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_WAlertWay, "CHAlertWay", (int)nd.WAlertWay);
                BP.Web.Controls.Glo.DDL_BindEnum(this.DDL_TWay, "TWay", (int)nd.TWay); //节假日计算.

                this.TB_WarningHour.Text = nd.WarningHour.ToString();
                this.TB_WarningDay.Text = nd.WarningDay.ToString();
                
                this.TB_TCent.Text = nd.TCent.ToString();
                switch (nd.HisCHWay)
                {
                    case BP.WF.CHWay.None:
                        this.RB_None.Checked = true;
                        break;
                    case BP.WF.CHWay.ByTime:
                        this.RB_ByTime.Checked = true;
                        break;
                    case BP.WF.CHWay.ByWorkNum:
                        this.RB_ByWorkNum.Checked = true;
                        break;
                    default:
                        break;
                }

                //是否质量考核点.
                this.CB_IsEval.Checked = nd.IsEval;
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            //执行保存.
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            nd.TimeLimit = int.Parse(this.TB_TimeLimit.Text);
            nd.TSpanHour = int.Parse(this.TB_TSpanHour.Text);
            nd.WarningHour = int.Parse(this.TB_WarningHour.Text);
            nd.WarningDay = int.Parse(this.TB_WarningDay.Text);
            nd.TCent = int.Parse(this.TB_TCent.Text);

            nd.TAlertRole =  (BP.WF.CHAlertRole) int.Parse(this.DDL_TAlertRole.SelectedValue);
            nd.TAlertWay = (BP.WF.CHAlertWay)int.Parse(this.DDL_TAlertWay.SelectedValue);

            nd.WAlertRole = (BP.WF.CHAlertRole)int.Parse(this.DDL_WAlertRole.SelectedValue);
            nd.WAlertWay = (BP.WF.CHAlertWay)int.Parse(this.DDL_WAlertWay.SelectedValue);
            nd.TWay = (BP.DA.TWay)int.Parse(this.DDL_TWay.SelectedValue); //节假日计算方式.

            if (this.RB_None.Checked)
                nd.HisCHWay = BP.WF.CHWay.None;

            if (this.RB_ByTime.Checked)
                nd.HisCHWay = BP.WF.CHWay.ByTime;

            if (this.RB_ByWorkNum.Checked)
                nd.HisCHWay = BP.WF.CHWay.ByWorkNum;

            nd.IsEval = this.CB_IsEval.Checked;
            nd.Update();
        }
    }
}