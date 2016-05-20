using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;

namespace CCFlow.WF
{
    public partial class FlowRpt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.Flows fls = new BP.WF.Flows();
            fls.RetrieveAll();

            FlowSorts ens = new FlowSorts();
            ens.RetrieveAll();

            DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);

            int cols = 3; //定义显示列数 从0开始。
            decimal widthCell = 100 / cols;
            this.Pub1.AddTable("width=100% border=0");
            int idx = -1;
            bool is1 = false;

            string timeKey = "s" + this.Session.SessionID + DateTime.Now.ToString("yyMMddHHmmss");
            foreach (FlowSort en in ens)
            {
                if (en.ParentNo == "0"
                    || en.ParentNo == ""
                    || en.No == "")
                    continue;

                idx++;
                if (idx == 0)
                    is1 = this.Pub1.AddTR(is1);

                this.Pub1.AddTDBegin("width='" + widthCell + "%' border=0 valign=top");
                //输出类别.
                //this.Pub1.AddFieldSet(en.Name);
                this.Pub1.AddB(en.Name);
                this.Pub1.AddUL();

                #region 输出流程。
                foreach (Flow fl in fls)
                {
                    if (fl.FlowAppType == FlowAppType.DocFlow)
                        continue;

                    //如果该目录下流程数量为空就返回.
                    if (fls.GetCountByKey(BP.WF.Template.FlowAttr.FK_FlowSort, en.No) == 0)
                        continue;

                    if (fl.FK_FlowSort != en.No)
                        continue;

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["No"].ToString() != fl.No)
                            continue;
                        break;
                    }

                    this.Pub1.AddLi(" <a  href=\"javascript:WinOpen('/WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No + "');\" >" + fl.Name + "</a> ");
                }
                #endregion 输出流程。

                this.Pub1.AddULEnd();

                this.Pub1.AddTDEnd();
                if (idx == cols - 1)
                {
                    idx = -1;
                    this.Pub1.AddTREnd();
                }
            }

            while (idx != -1)
            {
                idx++;
                if (idx == cols - 1)
                {
                    idx = -1;
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                }
                else
                {
                    this.Pub1.AddTD();
                }
            }
            this.Pub1.AddTableEnd();

        }
    }
}