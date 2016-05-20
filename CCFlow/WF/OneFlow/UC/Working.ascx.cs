using System;
using System.Data;
using System.Collections.Generic;
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

public partial class WF_OneFlow_UC_Working : BP.Web.UC.UCBase3
{
    public string FK_Flow
    {
        get
        {
            string s = this.Request.QueryString["FK_Flow"];
            if (s == null)
                return this.ViewState["FK_Flow"] as string;
            return s;
        }
        set
        {
            this.ViewState["FK_Flow"] = value;
        }
    }
    public bool IsHungUp
    {
        get
        {
            string s = this.Request.QueryString["IsHungUp"];
            if (s == null)
                return false;
            else
                return true;
        }
    }
    public string GroupBy
    {
        get
        {
            string s = this.Request.QueryString["GroupBy"];
            if (s == null)
            {
                if (this.DoType == "CC")
                    s = "Starter";
                else
                    s = "FlowName";
            }
            return s;
        }
    }
    public void BindList()
    {
        if (this.GroupBy == "PRI")
        {
            this.BindList_PRI();
            return;
        }
        string appPath = this.Request.ApplicationPath;
        string groupVals = "";
        foreach (DataRow dr in dt.Rows)
        {
            if (groupVals.Contains("@" + dr[this.GroupBy].ToString() + ","))
                continue;
            groupVals += "@" + dr[this.GroupBy].ToString() + ",";
        }

        int colspan = 9;
        //if (this.PageSmall != "")
        //    this.Pub1.AddBR();
        this.Pub1.AddTable("width='100%' align=center");

        if (this.IsHungUp)
            this.Pub1.AddCaptionLeft("<img src='/WF/Img/WFState/HungUp.png' class=Icon>&nbsp;<a href='HungUp.aspx?FK_Flow=" + this.FK_Flow + "&IsHungUp=1' ><b>挂起</b></a>");
        else
            this.Pub1.AddCaptionLeft("<img src='/WF/Img/Runing.gif' class=Icon >&nbsp;<b>待办</b>");

        string extStr = "";
        if (this.IsHungUp)
            extStr = "&IsHungUp=1";

        this.Pub1.AddTR();
        this.Pub1.AddTDTitle("ID");
        this.Pub1.AddTDTitle("width=40%", "流程标题");

        if (this.GroupBy != "NodeName")
            this.Pub1.AddTDTitle("<a href='Working.aspx?GroupBy=NodeName" + extStr + "&FK_Flow=" + this.FK_Flow + "' >节点</a>");

        if (this.GroupBy != GenerWorkFlowAttr.StarterName)
            this.Pub1.AddTDTitle("<a href='Working.aspx?GroupBy=StarterName" + extStr + "&FK_Flow=" + this.FK_Flow + "' >发起人</a>");

     //   if (isPRI)
        //    this.Pub1.AddTDTitle("<a href='Working.aspx?GroupBy=PRI" + extStr + "&FK_Flow=" + this.FK_Flow + "' >优先级</a>");

        this.Pub1.AddTDTitle("发起日期");
        this.Pub1.AddTDTitle("应完成日期");
        this.Pub1.AddTDTitle("状态");
        this.Pub1.AddTREnd();

        int i = 0;
        DateTime cdt = DateTime.Now;
        string[] gVals = groupVals.Split('@');
        int gIdx = 0;
        foreach (string g in gVals)
        {
            if (string.IsNullOrEmpty(g))
                continue;

            gIdx++;

            this.Pub1.AddTR();
            if (this.GroupBy == GenerWorkFlowAttr.Starter)
                this.Pub1.AddTD("colspan=" + colspan + " class=Sum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>");
            else
                this.Pub1.AddTD("colspan=" + colspan + " class=Sum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>");
            this.Pub1.AddTREnd();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr[this.GroupBy].ToString() + "," != g)
                    continue;

                string sdt = dr[GenerWorkFlowAttr.SDTOfFlow] as string;
                this.Pub1.AddTR("ID='" + gIdx + "_" + i + "'");
                i++;

                this.Pub1.AddTDIdx(i);

                this.Pub1.AddTD("Class=TTD width='50%'", "<a href=\"MyFlow.aspx?FK_Flow=" + dr[GenerWorkFlowAttr.FK_Flow] + "&FK_Node=" + dr[GenerWorkFlowAttr.FK_Node] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "\" > <img class=Icon src='/WF/Img/Mail_Read.png' id='I" + gIdx + "_" + i + "' />" + dr[GenerWorkFlowAttr.Title].ToString());

                if (this.GroupBy != GenerWorkFlowAttr.NodeName)
                    this.Pub1.AddTD(dr[GenerWorkFlowAttr.NodeName].ToString());

                if (this.GroupBy != GenerWorkFlowAttr.StarterName)
                    this.Pub1.AddTD(dr[GenerWorkFlowAttr.Starter].ToString() + " " + dr[GenerWorkFlowAttr.StarterName]);


                this.Pub1.AddTD("width='9%' nowarp", dr[GenerWorkFlowAttr.RDT].ToString().Substring(5));
              //  this.Pub1.AddTD("width='9%' nowarp", dr["ADT"].ToString().Substring(5));
                this.Pub1.AddTD("width='5%' nowarp", dr[GenerWorkFlowAttr.SDTOfFlow].ToString().Substring(5));

                DateTime mysdt = DataType.ParseSysDate2DateTime(sdt);
                if (cdt >= mysdt)
                {
                    if (cdt.ToString("yyyy-MM-dd") == mysdt.ToString("yyyy-MM-dd"))
                        this.Pub1.AddTDCenter("正常");
                    else
                        this.Pub1.AddTDCenter("<font color=red>逾期</font>");
                }
                else
                {
                    this.Pub1.AddTDCenter("正常");
                }
                this.Pub1.AddTREnd();
            }
        }
        this.Pub1.AddTRSum();
        this.Pub1.AddTD("colspan=" + colspan, "&nbsp;");
        this.Pub1.AddTREnd();
        this.Pub1.AddTableEnd();
        return;
    }
    public void BindList_PRI()
    {
        string appPath = this.Request.ApplicationPath;
        int colspan = 10;
        //if (this.PageSmall != "")
        //    this.Pub1.AddBR();

        string extStr = "";
        if (this.IsHungUp)
            extStr = "&IsHungUp=1";

        this.Pub1.AddTable("width='100%' align=center");

        if (this.IsHungUp)
            this.Pub1.AddCaptionLeft("<img src='/WF/Img/WFState/HungUp.png' class=Icon  >&nbsp;<a href='" + PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&IsHungUp=1' ><b>挂起</b></a>");
        else
            this.Pub1.AddCaptionLeft("<img src='/WF/Img/Runing.gif' class=Icon  >&nbsp;<a href='" + PageID + ".aspx?FK_Flow=" + this.FK_Flow + "' ><b>待办工作</b></a>");
        
        this.Pub1.AddTR();
        this.Pub1.AddTDTitle("ID");
        this.Pub1.AddTDTitle("width='40%'", "流程标题");

        //if (this.GroupBy != "FlowName")
        //    this.Pub1.AddTDTitle("<a href='Working.aspx?GroupBy=FlowName" + extStr + "&FK_Flow=" + this.FK_Flow + "' >流程</a>");

        if (this.GroupBy != GenerWorkFlowAttr.NodeName)
            this.Pub1.AddTDTitle("<a href='Working.aspx?GroupBy=NodeName" + extStr + "&FK_Flow=" + this.FK_Flow + "' >节点</a>");

        if (this.GroupBy != GenerWorkFlowAttr.StarterName)
            this.Pub1.AddTDTitle("<a href='Working.aspx?GroupBy=StarterName" + extStr + "&FK_Flow=" + this.FK_Flow + "' >发起人</a>");

        //  this.Pub1.AddTDTitle("优先级");
        this.Pub1.AddTDTitle("发起日期");
        this.Pub1.AddTDTitle("应完成日期");
        this.Pub1.AddTDTitle("状态");
        this.Pub1.AddTREnd();

        int i = 0;
        DateTime cdt = DateTime.Now;
        int gIdx = 0;
        SysEnums ses = new SysEnums("PRI");
        foreach (SysEnum se in ses)
        {
            gIdx++;
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=" + colspan + " class=Sum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<img src='/WF/Img/PRI/" + se.IntKey + ".png' class=ImgPRI />");
            this.Pub1.AddTREnd();

            string pri = se.IntKey.ToString();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["PRI"].ToString() != pri)
                    continue;

                string sdt = dr["SDTOfFlow"] as string;
                this.Pub1.AddTR("ID='" + gIdx + "_" + i + "'");
                i++;
                this.Pub1.AddTDIdx(i);
                this.Pub1.AddTD("Class=TTD  width='50%' ", "<a href=\"MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "\" >" + dr["Title"].ToString());

                //if (this.GroupBy != "FlowName")
                //    this.Pub1.AddTD(dr["FlowName"].ToString());

                if (this.GroupBy != "NodeName")
                    this.Pub1.AddTD(dr["NodeName"].ToString());

                if (this.GroupBy != GenerWorkFlowAttr.StarterName)
                    this.Pub1.AddTD(dr[GenerWorkFlowAttr.StarterName].ToString() + " " + dr[GenerWorkFlowAttr.StarterName]);

                this.Pub1.AddTD(dr["RDT"].ToString().Substring(5));
                this.Pub1.AddTD(dr["ADT"].ToString().Substring(5));
                this.Pub1.AddTD(dr["SDT"].ToString().Substring(5));

                DateTime mysdt = DataType.ParseSysDate2DateTime(sdt);
                if (cdt >= mysdt)
                {
                    if (cdt.ToString("yyyy-MM-dd") == mysdt.ToString("yyyy-MM-dd"))
                        this.Pub1.AddTDCenter("正常");
                    else
                        this.Pub1.AddTDCenter("<font color=red>逾期</font>");
                }
                else
                {
                    this.Pub1.AddTDCenter("正常");
                }
                this.Pub1.AddTREnd();
            }
        }
        this.Pub1.AddTRSum();
        this.Pub1.AddTD("colspan=" + colspan, "&nbsp;");
        this.Pub1.AddTREnd();
        this.Pub1.AddTableEnd();
        return;
    }
    public DataTable dt = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        this.FK_Flow = this.Request.QueryString["FK_Flow"];
        if (this.IsHungUp)
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList(this.FK_Flow);
        else
            dt = BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No, this.FK_Flow);
        this.BindList();
    }
}