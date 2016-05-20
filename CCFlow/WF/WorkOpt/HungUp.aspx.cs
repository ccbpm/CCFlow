using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using BP.DA;
using BP.Web;
namespace CCFlow.WF
{

    public partial class WF_Hurry : BP.Web.WebPage
    {
        #region 属性.
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
                try
                {
                    return Int64.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            HungUp hu = new HungUp();
            hu.MyPK = this.WorkID + "_" + this.FK_Node;
            int i = hu.RetrieveFromDBSources();

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                this.Pub1.AddFieldSet("错误","当前是开始节点，或者工作不存在.");
                return;
            }

            this.Pub1.AddFieldSet("对工作<b>(" + gwf.Title + ")</b>挂起方式");
            RadioButton rb = new RadioButton();
            rb.GroupName = "s";
            rb.Text = "永久挂起";
            rb.ID = "RB_HungWay0";
            if (hu.HungUpWay == 0)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddBR();

            rb = new RadioButton();
            rb.GroupName = "s";
            rb.Text = "在指定的日期自动解除挂起.<br>";
            rb.ID = "RB_HungWay1";
            if (hu.HungUpWay == HungUpWay.SpecDataRel )
                rb.Checked = true;
            else
                rb.Checked = false;
            this.Pub1.Add(rb);

            this.Pub1.Add("&nbsp;&nbsp;&nbsp;&nbsp;解除流程挂起的日期:");
            BP.Web.Controls.TB tb = new BP.Web.Controls.TB();
            tb.ReadOnly = false;
            tb.ID = "TB_RelData";
            if (hu.DTOfUnHungUpPlan.Length == 0)
            {
                DateTime dt = DateTime.Now.AddDays(7);
                hu.DTOfUnHungUpPlan = dt.ToString(DataType.SysDataTimeFormat);
            }
            tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
            tb.Text = hu.DTOfUnHungUpPlan;
            this.Pub1.Add(tb);

            this.Pub1.AddBR();
            this.Pub1.Add("挂起原因(可以为空):");

            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_Note";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Columns = 70;
            tb.Height = 50;
            this.Pub1.Add(tb);
            this.Pub1.AddFieldSetEnd(); 

            this.Pub1.Add("&nbsp;&nbsp;&nbsp;&nbsp;");
            Button btn = new Button();
            btn.ID = "Btn_OK";

            if (gwf.WFState == WFState.HungUp)
                btn.Text = " 取消挂起 ";
            else
                btn.Text = " 挂起 ";

            btn.Click += new EventHandler(btn_Click);
            btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_Cancel";
            btn.Text = " 返回 ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.ID == "Btn_Cancel")
            {
                this.Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID, true);
                return;
            }

            try
            {
                HungUpWay way = HungUpWay.SpecDataRel;
                RadioButton rb = this.Pub1.GetRadioButtonByID("RB_HungWay0");
                if (rb.Checked)
                    way = HungUpWay.Forever;

                string reldata = this.Pub1.GetTBByID("TB_RelData").Text;
                string note = this.Pub1.GetTBByID("TB_Note").Text;
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.WFState == WFState.HungUp)
                    BP.WF.Dev2Interface.Node_UnHungUpWork(this.FK_Flow, this.WorkID, note);
                else
                    BP.WF.Dev2Interface.Node_HungUpWork(this.FK_Flow, this.WorkID, (int)way, reldata, note);

                this.WinCloseWithMsg("执行成功.");
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
                //this.Pub1.AddMsgOfWarning.Response.Write(ex);
            }
        }
    }
}