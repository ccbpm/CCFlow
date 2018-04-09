using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.DA;


namespace CCFlow.WF.WorkOpt
{
    public partial class StopFlow : BP.Web.WebPage
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
            
        }

        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            string info = this.TextBox1.Text;
            if (DataType.IsNullOrEmpty(info))
            {
                BP.Sys.PubClass.Alert("请输入强制终止流程的原因。");
                return;
            }

            string infoEnd = BP.WF.Dev2Interface.Flow_DoFlowOverByCoercion(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, info);
            this.ToMsg("结束流程提示:<hr>" + infoEnd, "info");
        }

        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            Glo.SessionMsg = msg;
            this.Response.Redirect("../MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID, true);
        }
    }
}