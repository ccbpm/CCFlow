using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
namespace CCFlow.WF.UC
{
    public partial class SMS : BP.Web.UC.UCBase3
    {
        public Int32 WorkID
        {
            get
            {
                try
                {
                    return Int32.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return 0;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            this.Page.Title = "SMS";
            this.AddMsgOfInfo("错误", "没有安装短信发送设备.");
            //return;


            //string sql = "SELECT No,Name,Tel FROM WF_Emp WHERE NO IN (select FK_Emp from WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node=" + this.NodeID + ")";
            //DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //if (dt.Rows.Count == 0)
            //{
            //    this.WinCloseWithMsg("对不起，接受人员没有设置短消息提醒。");
            //    return;
            //}

            //this.AddFieldSet("发手机短信提醒");
            //this.AddTable();
            //this.AddTR();
            //this.Add("<TD class=BigDoc>");
            //bool isHave = false;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    CheckBox cb = new CheckBox();
            //    cb.ID = "CB_" + dr["No"].ToString();
            //    BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(dr["No"].ToString());
            //    if (emp.Tel.Length > 10)
            //    {
            //        cb.Checked = true;
            //        cb.Text = emp.No + " " + emp.Name + " ( " + emp.Tel + ")";
            //        isHave = true;
            //    }
            //    else
            //    {
            //        cb.Text = emp.No + " " + emp.Name + " (没有设置手机号)";
            //        cb.Checked = false;
            //        cb.Enabled = false;
            //        this.Add(cb);
            //    }
            //}
            //this.AddTDEnd();
            //this.AddTREnd();

            //this.AddTR();
            //this.Add("<TD class=BigDoc>");
            //TextBox tb = new TextBox();
            //tb.Attributes["width"] = "100%";
            //tb.TextMode = TextBoxMode.MultiLine;
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //tb.Text = "您好：\t\n您有工作需要处理" + nd.Name + " ; \t\n" + WebUser.Name;
            //tb.Columns = 50;
            //tb.Rows = 7;
            //this.Add(tb);
            //this.AddTDEnd();
            //this.AddTREnd();

            //this.AddTR();
            //this.Add("<TD>");
            //Btn btn = new Btn();
            //btn.Text = "发送手机消息";
            //btn.Click += new EventHandler(btn_Click);
            //btn.Enabled = isHave;

            //this.Add(btn);
            //this.AddTDEnd();
            //this.AddTREnd();
            //this.AddTableEnd();

            //this.AddFieldSetEnd();

        }

        void btn_Click(object sender, EventArgs e)
        {
            this.WinCloseWithMsg("发送成功");
        }
    }

}