using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BP.DA;
using BP.WF;
using BP.Web;

namespace CCFlow.AppDemoLigerUI
{
    public partial class ProjectDelInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Flows flows = new Flows();
            BP.En.QueryObject obj = new BP.En.QueryObject(flows);
            obj.addOrderBy("Name");
            obj.DoQuery();
            ListItem item = new ListItem();
            item.Value = "0";
            item.Text = "======流程======";
            DDL_Flow.Items.Add(item);
            foreach (Flow fl in flows)
            {
                if (fl.FK_FlowSort == "107")
                    continue;

                item = new ListItem();
                item.Value = fl.No;
                item.Text = fl.Name;
                DDL_Flow.Items.Add(item);
            }

            //添加状态
            item = new ListItem();
            item.Value = "0";
            item.Text = "===流程状态===";
            DDL_WFState.Items.Add(item);
            string sql = "select * from Sys_Enum where EnumKey='WFStateApp' order by IntKey";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                item = new ListItem();
                item.Value = row["IntKey"].ToString();
                item.Text = row["Lab"].ToString();
                DDL_WFState.Items.Add(item);
            }
        }
    }
}