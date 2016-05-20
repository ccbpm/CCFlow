using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;

namespace CCFlow.WF
{
    public partial class JZFlows : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //BP.WF.Flows fls = new BP.WF.Flows();
            //fls.RetrieveAll();

            //FlowSorts ens = new FlowSorts();
            //ens.RetrieveAll();

            //DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(BP.Web.WebUser.No);

            //int cols = 3; //定义显示列数 从0开始。
            //decimal widthCell = 100 / cols;
            //this.Pub1.AddTable("width=100% border=0");
            //this.Pub1.AddCaptionMsg("发起流程");

            //int idx = -1;
            //bool is1 = false;

            //string timeKey = "s" + this.Session.SessionID + DateTime.Now.ToString("yyMMddHHmmss");
            //foreach (FlowSort en in ens)
            //{
            //    if (en.ParentNo == "0"
            //        || en.ParentNo == ""
            //        || en.No == "")
            //        continue;

            //    idx++;
            //    if (idx == 0)
            //        is1 = this.Pub1.AddTR(is1);

            //    this.Pub1.AddTDBegin("width='" + widthCell + "%' border=0 valign=top");
            //    //输出类别.
            //    //this.Pub1.AddFieldSet(en.Name);
            //    this.Pub1.AddB(en.Name);
            //    this.Pub1.AddUL();

            //    #region 输出流程。
            //    foreach (Flow fl in fls)
            //    {
            //        if (fl.FlowAppType == FlowAppType.DocFlow)
            //            continue;

            //        if (fl.FK_FlowSort != en.No)
            //            continue;

            //        bool isHaveIt = false;
            //        foreach (DataRow dr in dt.Rows)
            //        {
            //            if (dr["No"].ToString() != fl.No)
            //                continue;
            //            isHaveIt = true;
            //            break;
            //        }

            //        string extUrl = "";
            //        if (fl.IsBatchStart)
            //            extUrl = "<div style='float:right;'><a href='/WF/BatchStart.aspx?FK_Flow=" + fl.No + "' >批量发起</a>|<a href='/WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No + "'>查询</a>|<a href=\"javascript:WinOpen('/WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fl.No + "&DoType=Chart&T=" + timeKey + "','sd');\"  >图</a></div>";
            //        else
            //            extUrl = "<div style='float:right;'><a  href='/WF/Rpt/Search.aspx?RptNo=ND" + int.Parse(fl.No) + "MyRpt&FK_Flow=" + fl.No + "'>查询</a>|<a href=\"javascript:WinOpen('/WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fl.No + "&DoType=Chart&T=" + timeKey + "','sd');\"  >流程图</a></div>";

            //        if (isHaveIt)
            //        {
            //            if (Glo.IsWinOpenStartWork == 1)
            //                this.Pub1.AddLiB("<a href=\"javascript:WinOpenIt('MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01&T=" + timeKey + "');\" >" + fl.Name + "</a> - " + extUrl);
            //            else if (Glo.IsWinOpenStartWork == 2)
            //                this.Pub1.AddLiB("<a href=\"javascript:WinOpenIt('/WF/OneFlow/MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01&T=" + timeKey + "');\" >" + fl.Name + "</a> - " + extUrl);
            //            else
            //                this.Pub1.AddLiB("<a href='MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01' >" + fl.Name + "</a> - " + extUrl);
            //        }
            //        else
            //        {
            //            this.Pub1.AddLi(fl.Name);

            //        }
            //    }
            //    #endregion 输出流程。

            //    this.Pub1.AddULEnd();

            //    this.Pub1.AddTDEnd();
            //    if (idx == cols - 1)
            //    {
            //        idx = -1;
            //        this.Pub1.AddTREnd();
            //    }
            //}

            //while (idx != -1)
            //{
            //    idx++;
            //    if (idx == cols - 1)
            //    {
            //        idx = -1;
            //        this.Pub1.AddTD();
            //        this.Pub1.AddTREnd();
            //    }
            //    else
            //    {
            //        this.Pub1.AddTD();
            //    }
            //}
            //this.Pub1.AddTableEnd();

        }
    }
}