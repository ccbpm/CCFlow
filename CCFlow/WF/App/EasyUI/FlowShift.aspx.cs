using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CCFlow.AppDemoLigerUI.Base;
using System.Data;

namespace CCFlow.AppDemoLigerUI
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
        public int WorkID
        {
            get
            {
                return int.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public int FID
        {
            get
            {
                return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        #endregion 属性
        protected void Page_Load(object sender, EventArgs e)
        {

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
              //  result = CommonDbOperator.GetJsonFromTableEasy(dt, count);

                result = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            Response.Clear();
            Response.Write(result);
            Response.End();

        }

        protected void Btn_OK_Click(object sender, EventArgs e)
        {
            string str = this.TB_Emp.Text.Trim();
            BP.WF.Port.Emp emp = new BP.WF.Port.Emp();
            emp.No = str;
            if (emp.RetrieveFromDBSources() == 0)
            {
                BP.Sys.PubClass.Alert("人员编号输入错误:" + str);
                return;
            }

            string note = this.TB_Note.Text.Trim();

            //执行移交.
            BP.WF.Dev2Interface.Node_Shift(FK_Flow,this.WorkID,FK_Node,FID,emp.No, note);

            // 提示.
            BP.Sys.PubClass.Alert("已经成功的移交给:" + emp.Name);
            BP.Sys.PubClass.WinClose("ok");
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            BP.Sys.PubClass.WinClose();
        }
    }
}