using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using BP.Sys;
using System.Text.RegularExpressions;

namespace CCFlow.WF.Admin.AttrNode
{
    public partial class SubFlows : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 节点编号
        /// </summary>
        private int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        #endregion 属性.


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FrmSubFlow frmSubFlow = new FrmSubFlow(this.FK_Node);

                //控件状态
                if (frmSubFlow.SFSta == FrmSubFlowSta.Disable)
                    this.RB_Disable.Checked = true;

                if (frmSubFlow.SFSta == FrmSubFlowSta.Enable)
                    this.RB_Enable.Checked = true;

                if (frmSubFlow.SFSta == FrmSubFlowSta.Readonly)
                    this.RB_Readonly.Checked = true;

                //显示方式
                if (frmSubFlow.SFShowModel == FrmWorkShowModel.Table)
                    this.RB_Table.Checked = true;

                if (frmSubFlow.SFShowModel == FrmWorkShowModel.Free)
                    this.RB_Free.Checked = true;

                this.TB_SFCaption.Text = frmSubFlow.SFCaption;
                this.TB_SFDefInfo.Text = frmSubFlow.SFDefInfo;

                //高度,宽度.
                this.SF_H.Text = frmSubFlow.SF_H.ToString();
                this.SF_W.Text = frmSubFlow.SF_W.ToString();
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            FrmSubFlow frmSubFlow = new FrmSubFlow(this.FK_Node);

            //显示方式
            if (this.RB_Table.Checked)
                frmSubFlow.SFShowModel = FrmWorkShowModel.Table;
            if (this.RB_Free.Checked)
                frmSubFlow.SFShowModel = FrmWorkShowModel.Free;

            //控件状态 禁用
            if (this.RB_Disable.Checked)
                frmSubFlow.SFSta = FrmSubFlowSta.Disable;

            if (this.RB_Enable.Checked)
                frmSubFlow.SFSta = FrmSubFlowSta.Enable;

            if (this.RB_Readonly.Checked)
                frmSubFlow.SFSta = FrmSubFlowSta.Readonly;

            //标题
            frmSubFlow.SFCaption = string.IsNullOrWhiteSpace(this.TB_SFCaption.Text.Trim()) ? "" : this.TB_SFCaption.Text.Trim();

            //可手工启动的子流程
            if (string.IsNullOrWhiteSpace(this.TB_SFDefInfo.Text.Trim())==false)
            {
                string[] flows = this.TB_SFDefInfo.Text.Trim().Split(',');
                string errorMsg = "";
                foreach (string flowNo in flows) //101,,,,102也是错误格式
                {
                    if (string.IsNullOrEmpty(flowNo))
                        continue;

                    Flow flEn = new Flow();
                    flEn.No = flowNo;
                    if (flEn.IsExits == false)
                        errorMsg = "@流程编号[" + flowNo + "]不存在";
                }

                if (string.IsNullOrWhiteSpace(errorMsg) == false)
                {
                    BP.Sys.PubClass.Alert(errorMsg);
                    return;
                }
                frmSubFlow.SFDefInfo = this.TB_SFDefInfo.Text.Trim();
            }

            //高度.
            frmSubFlow.SF_H = float.Parse(this.SF_H.Text.Trim());
            frmSubFlow.SF_W = float.Parse(this.SF_W.Text.Trim());
            frmSubFlow.Update();
        }
    }
}