using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class AutoStart : System.Web.UI.Page
    {
        #region 属性
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
            if (!IsPostBack)
            {
                BP.WF.Template.FlowSheet en = new BP.WF.Template.FlowSheet(FK_Flow);
                BP.WF.FlowRunWay frw = en.FlowRunWay;
                switch (frw)
                {
                    case FlowRunWay.HandWork:
                        this.RB_HandWork.Checked = true;
                        break;
                    case FlowRunWay.SpecEmp:
                        this.RB_SpecEmp.Checked = true;
                        this.TB_SpecEmp.Text = en.RunObj;
                        break;
                    case FlowRunWay.DataModel:
                        this.TB_DataModel.Text = en.RunObj;
                        this.RB_DataModel.Checked = true;
                        break;
                    case FlowRunWay.InsertModel:
                        this.RB_InsertModel.Checked = true;
                        break;
                    default:
                        break;
                }
            }
        }
        protected void BtnSave_Click(object sender, EventArgs e)
        {
            //执行保存.
            BP.WF.Flow en = new BP.WF.Flow(this.FK_Flow);
            if (this.RB_HandWork.Checked)
            {
                en.HisFlowRunWay = BP.WF.FlowRunWay.HandWork;
            }

            if (this.RB_SpecEmp.Checked)
            {
                en.HisFlowRunWay = BP.WF.FlowRunWay.SpecEmp;
                en.RunObj = this.TB_SpecEmp.Text;
            }

            if (this.RB_DataModel.Checked)
            {
                en.RunObj = this.TB_DataModel.Text;
                en.HisFlowRunWay = BP.WF.FlowRunWay.DataModel;
            }

            if (this.RB_InsertModel.Checked)
            {
                en.HisFlowRunWay = BP.WF.FlowRunWay.InsertModel;
            }
            en.DirectUpdate();
        }
    }
}