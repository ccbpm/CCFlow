using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

public partial class WF_OneFlow_UC_StartList : BP.Web.UC.UCBase3
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Page.Title = "工作发起";
        Flows fls = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfEntities(WebUser.No);
        string appPath = this.Request.ApplicationPath;

        string pageid = this.Request.RawUrl.ToLower();
        if (pageid.Contains("small"))
        {
            if (pageid.Contains("single"))
                pageid = "SmallSingle";
            else
                pageid = "Small";
            this.AddBR();
        }
        else
        {
            pageid = "";
        }

        int colspan = 4;
        this.AddTable("width='960px' align=center border=0");
        this.AddTR();
        this.AddCaptionLeft("<img src='/WF/Img/Start.gif' > <b>发起</b>");
        this.AddTREnd();

        this.AddTR();
        this.AddTDTitle("序");
        this.AddTDTitle("名称");
        this.AddTDTitle("流程图");
        this.AddTDTitle("描述");
        this.AddTREnd();

        int i = 1;
        string fk_sort = null;
        int idx = 0;
        int gIdx = 0;
        foreach (Flow fl in fls)
        {
            if (fl.FlowAppType == FlowAppType.DocFlow)
                continue;
            idx++;
            //2012.9.16by李健
            if (fl.FK_FlowSort != fk_sort)
            {
                gIdx++;
                this.AddTDB("colspan=" + colspan + " class=Sum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + fl.FK_FlowSortText + "</b>");
                this.AddTREnd();
                fk_sort = fl.FK_FlowSort;
            }

            if (fl.FK_FlowSort == fk_sort)
            {
                this.AddTR("ID='" + gIdx + "_" + idx + "'");
                this.AddTDIdx(i++);
            }
            else
            {
                //    gIdx++;
                //    this.AddTDB("colspan=5 class=Sum onclick=\"GroupBarClick('" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='./Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + fl.FK_FlowSortText + "</b>");
                //    this.AddTREnd();
                //    fk_sort = fl.FK_FlowSort;
                //    continue;
            }

            fk_sort = fl.FK_FlowSort;
            if (fl.StartGuidePara1 == "")
            {
                this.AddTD("<a href=\"" + appPath + "WF/OneFlow/MyFlow.aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01\" >" + fl.Name + "</a>");
            }
            else
                this.AddTD("<a href=\"javascript:StartListUrl('" + appPath + "','" + fl.StartGuidePara1 + "?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01','" + fl.No + "','" + pageid + "')\" >" + fl.Name + "</a>");

            this.AddTD("<a href=\"javascript:WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fl.No + "&DoType=Chart','sd');\"  >打开</a>");
            this.AddTD(fl.Note);
            this.AddTREnd();
        }
        this.AddTRSum();
        this.AddTD("colspan=" + colspan, "&nbsp;");
        this.AddTREnd();
        this.AddTableEnd();
    }
}