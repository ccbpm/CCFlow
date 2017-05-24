using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Data;

namespace CCFlow.AppDemoLigerUI
{
    public partial class FlowSkip : System.Web.UI.Page
    {
        #region 属性
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
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            Nodes nds = new Nodes(this.FK_Flow);

            foreach (BP.WF.Node nd in nds)
            {
                this.DDL_SkipToNode.Items.Add(new ListItem("步骤:" + nd.Step + "名称:" + nd.Name,
                    nd.NodeID.ToString()));
            }

            string type = Request["action"];

            if (!string.IsNullOrEmpty(type))
            {
                if (type.Equals("getEmp"))
                {
                    GetEmps();
                }
            }
        }

        private void GetEmps()
        {
            string result = "";
            string key3 = Request.QueryString["key"];
            string top = Request.QueryString["limit"];

            if (string.IsNullOrEmpty(top))
            {
                top = "10";
            }

            string realKey = HttpUtility.UrlDecode(key3, System.Text.Encoding.UTF8);
            string sql = "select top " + top + " * from port_emp where Name like '%" + realKey + "%' or No like'%" + realKey + "%'";
            System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            System.Collections.ArrayList dic = new System.Collections.ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();

                foreach (DataColumn dc in dt.Columns)
                {
                    drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }

                dic.Add(drow);
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            result = jss.Serialize(dic);

            Response.Clear();
            Response.Write(result);
            Response.End();
        }

        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                string[] str = this.TB_SkipToEmp.Text.Trim().Replace('(', '|').Replace(')', '|').Split("|".ToCharArray());

                if (str.Length < 3)
                {
                    BP.Sys.PubClass.Alert("请选择人员！");
                    return;
                }

                BP.WF.Port.Emp emp = new BP.WF.Port.Emp();
                emp.No = str[1];

                if (emp.RetrieveFromDBSources() == 0)
                {
                    BP.Sys.PubClass.Alert("人员编号输入错误:" + str);
                    return;
                }

                string note = this.TB_Note.Text.Trim();
                BP.WF.Node nd = new Node(int.Parse(this.DDL_SkipToNode.SelectedItem.Value));
                WorkFlow work = new WorkFlow(this.FK_Flow, WorkID);

                //执行发送.
                //string info =  BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, null, null, nd.NodeID, emp.No).ToMsgOfText();
                string info = BP.WF.Dev2Interface.Flow_Schedule(this.WorkID, nd.NodeID, emp.No);
                BP.WF.Dev2Interface.WriteTrack(this.FK_Flow, nd.NodeID, this.WorkID, work.FID, note, ActionType.Shift, null, null, "跳转");
                // 提示.
                BP.Sys.PubClass.Alert("已经成功的跳转给:" + emp.Name + ",跳转到:" + nd.Name + "," + info);
                BP.Sys.PubClass.WinClose("ss");
            }
            catch (Exception ex)
            {
                BP.Sys.PubClass.Alert(ex.Message);
            }
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }
    }
}