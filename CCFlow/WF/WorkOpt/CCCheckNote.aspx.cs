using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.DA;


namespace CCFlow.WF.WorkOpt
{
    public partial class CCCheckNote : BP.Web.WebPage
    {
        #region 参数
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
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
                string note = BP.WF.Dev2Interface.GetCheckInfo(this.FK_Flow, this.WorkID, this.FK_Node,"");
                if (DataType.IsNullOrEmpty(note))
                {
                    BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.FK_Node, this.WorkID, this.FID,"已阅", "阅知");
                    note = "已阅";
                }
                this.TextBox1.Text = note;
            }
        }

        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);

            BP.WF.Dev2Interface.WriteTrackWorkCheck(this.FK_Flow, this.FK_Node, this.WorkID,
                this.FID, this.TextBox1.Text, fwc.FWCOpLabel);

            //设置审核完成.
            BP.WF.Dev2Interface.Node_CC_SetSta(this.FK_Node, this.WorkID, BP.Web.WebUser.No, BP.WF.Template.CCSta.CheckOver);

            this.WinCloseWithMsg("审核成功");
        }
    }
}