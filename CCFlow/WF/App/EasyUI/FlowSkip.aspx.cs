using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.Data;
using CCFlow.AppDemoLigerUI.Base;

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
            string key = HttpUtility.UrlDecode(Request["key"], System.Text.Encoding.UTF8);
            if (!string.IsNullOrEmpty(key))
            {

                string sql = "select * from Port_Emp where Name like '%" + key + "%'";

                System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

                //dt.Columns.Add("FK_DeptName");

                //foreach (DataRow row in dt.Rows)
                //{

                //    string depSql = "select Name from V_Inc where No='" + row["FK_Dept"] + "' ";

                //    DataTable deptDt = BP.DA.DBAccess.RunSQLReturnTable(depSql);

                //    if (deptDt.Rows.Count > 0)
                //    {
                //        row["FK_DeptName"] = deptDt.Rows[0]["Name"];
                //    }

                //}

               // result = CommonDbOperator.GetJsonFromTableEasy(dt, count);
                result = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            Response.Clear();
            Response.Write(result);
            Response.End();

        }

        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                string str = this.TB_SkipToEmp.Text.Trim();
                BP.WF.Port.Emp emp = new BP.WF.Port.Emp();
                emp.No = str;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    BP.Sys.PubClass.Alert("人员编号输入错误:" + str);
                    return;
                }
                string note = this.TB_Note.Text.Trim();

                BP.WF.Node nd = new Node(int.Parse(this.DDL_SkipToNode.SelectedItem.Value));

               WorkFlow work = new WorkFlow(this.FK_Flow,WorkID);

                //执行发送.
               //string info =  BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, null, null, nd.NodeID, emp.No).ToMsgOfText();
               string info =  BP.WF.Dev2Interface.Flow_Schedule(this.WorkID, nd.NodeID, emp.No);
               BP.WF.Dev2Interface.WriteTrack(this.FK_Flow, nd.NodeID, this.WorkID, work.FID,note, ActionType.Shift, null, null, "跳转");
                // 提示.
                BP.Sys.PubClass.Alert("已经成功的跳转给:" + str + ",跳转到:" + nd.Name+","+info );
                BP.Sys.PubClass.WinClose("ss");
            }
            catch(Exception ex)
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