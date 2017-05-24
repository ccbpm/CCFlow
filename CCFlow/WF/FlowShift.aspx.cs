using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            string[] str = this.TB_Emp.Text.Trim().Trim().Replace('(', '|').Replace(')', '|').Split("|".ToCharArray());

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

            //执行移交.
            BP.WF.Dev2Interface.Node_Shift(FK_Flow, FK_Node, this.WorkID, FID, emp.No, note);

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