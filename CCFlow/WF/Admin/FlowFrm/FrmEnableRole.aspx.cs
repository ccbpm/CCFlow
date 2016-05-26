using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;

namespace CCFlow.WF.Admin.FlowFrm
{
    public partial class FrmEnableRole : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 表单ID.
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
               Node nd= new Node(this.FK_Node);
               BP.WF.Template.FrmNode fn = new BP.WF.Template.FrmNode(nd.FK_Flow, nd.NodeID, this.FK_MapData);
               switch (fn.FrmEnableRole)
               {
                   case BP.WF.Template.FrmEnableRole.Allways:
                       this.RB_0.Checked = true;
                       break;
                   case BP.WF.Template.FrmEnableRole.WhenHaveData:
                       this.RB_1.Checked = true;
                       break;
                   case BP.WF.Template.FrmEnableRole.WhenHaveFrmPara:
                       this.RB_2.Checked = true;
                       break;
                   case BP.WF.Template.FrmEnableRole.ByFrmFields:
                       this.RB_3.Checked = true;
                       break;
                   case BP.WF.Template.FrmEnableRole.BySQL:
                       this.RB_4.Checked = true;
                       this.TB_SQL.Text = fn.FrmEnableExp;
                       break;
                   case BP.WF.Template.FrmEnableRole.Disable:
                       this.RB_5.Checked = true;
                       break;
                   default:
                       throw new Exception("@没有判断的模式.");
               }
            }
        }

        protected void Btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            Node nd = new Node(this.FK_Node);
            BP.WF.Template.FrmNode fn = new BP.WF.Template.FrmNode(nd.FK_Flow, nd.NodeID, this.FK_MapData);

            if (this.RB_0.Checked)
                fn.FrmEnableRole = BP.WF.Template.FrmEnableRole.Allways;

            if (this.RB_1.Checked)
                fn.FrmEnableRole = BP.WF.Template.FrmEnableRole.WhenHaveData;

            if (this.RB_2.Checked)
                fn.FrmEnableRole = BP.WF.Template.FrmEnableRole.WhenHaveFrmPara;

            if (this.RB_3.Checked)
                fn.FrmEnableRole = BP.WF.Template.FrmEnableRole.ByFrmFields;

            if (this.RB_4.Checked)
            {
                fn.FrmEnableRole = BP.WF.Template.FrmEnableRole.BySQL;
                fn.FrmEnableExp = this.TB_SQL.Text;
            }

            if (this.RB_5.Checked)
                fn.FrmEnableRole = BP.WF.Template.FrmEnableRole.Disable;

            fn.Update();

            BP.Sys.PubClass.WinClose(fn.FrmEnableRoleText);
            //BP.Sys.PubClass.WinCloseAndAlertMsg("保存成功.");
        }
    }
}