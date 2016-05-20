using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.WatchOneFlow
{
    public partial class FlowShift : System.Web.UI.Page
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse( this.Request.QueryString["WorkID"]);
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            string str = this.TB_Emp.Text.Trim();
            BP.WF.Port.Emp emp = new BP.WF.Port.Emp();
            emp.No = str;
            if (emp.RetrieveFromDBSources() == 0)
            {
                BP.Sys.PubClass.Alert("人员编号输入错误:"+str);
                return;
            }

            string note = this.TB_Note.Text.Trim();

            //执行移交.
            BP.WF.Dev2Interface.Node_Shift(this.FK_Flow,this.FK_Node, this.WorkID, this.FID, emp.No, note);

            // 提示.
            BP.Sys.PubClass.Alert("已经成功的移交给:" + str);
            BP.Sys.PubClass.WinClose("ss");
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }
    }
}