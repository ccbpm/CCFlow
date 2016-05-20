using System;
using System.Collections.Generic;
using System.Data;
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

namespace CCFlow.WF
{
    public partial class WF_HungUp : BP.Web.WebPage
    {
        #region 属性.
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
                        s = "Rec";
                    else
                        s = "FlowName";
                }
                return s;
            }
        }
        #endregion 属性.
         
        public DataTable dt = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
            if (dt.Rows.Count == 0)
            {
                this.Pub1.AddMsgOfInfoV2("挂起",
                    "当前没有挂起工作......");
                return;
            }

            string groupVals = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (groupVals.Contains("@" + dr[this.GroupBy].ToString() + ","))
                    continue;
                groupVals += "@" + dr[this.GroupBy].ToString() + ",";
            }

            int colspan = 9;

            this.Pub1.AddTable("width='100%' align=left");
            this.Pub1.AddCaptionMsg("挂起工作");

            string extStr = "";
            if (this.IsHungUp)
                extStr = "&IsHungUp=1";

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("标题");

            if (this.GroupBy != "FlowName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=FlowName" + extStr + "' >流程</a>");

            if (this.GroupBy != "NodeName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=NodeName" + extStr + "' >节点</a>");

            if (this.GroupBy != "StarterName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=StarterName" + extStr + "' >发起人</a>");


            this.Pub1.AddTDTitle("发起日期");
            this.Pub1.AddTDTitle("接受日期");
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
                if (this.GroupBy == "Rec")
                    this.Pub1.AddTD("colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='./Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>");
                else
                    this.Pub1.AddTD("colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='./Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>");
                this.Pub1.AddTREnd();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[this.GroupBy].ToString() + "," != g)
                        continue;

                    string sdt = dr[GenerWorkFlowAttr.SDTOfFlow] as string;

                    this.Pub1.AddTR("ID='" + gIdx + "_" + i + "'");
                    i++;

                    this.Pub1.AddTDIdx(i);
                    if (BP.WF.Glo.IsWinOpenEmpWorks == true)
                    {
                        this.Pub1.AddTD("Class=TTD onclick=\"SetImg('I" + gIdx + "_" + i + "')\"", "<a href=\"javascript:WinOpenIt('MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&IsRead=0');\" ><img src='/WF/Img/Mail_UnRead.png' id='I" + gIdx + "_" + i + "' border=0/>" + dr["Title"].ToString());
                    }
                    else
                    {
                        this.Pub1.AddTD("Class=TTD", "<a href=\"MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "\" >" + dr["Title"].ToString());
                    }

                    if (this.GroupBy != "FlowName")
                        this.Pub1.AddTD(dr["FlowName"].ToString());

                    if (this.GroupBy != "NodeName")
                        this.Pub1.AddTD(dr["NodeName"].ToString());

                    if (this.GroupBy != "StarterName")
                        this.Pub1.AddTD(dr["Starter"].ToString() + " " + dr["StarterName"]);

                        this.Pub1.AddTD("<img src='/WF/Img/PRI/" + dr["PRI"].ToString() + ".png' class=ImgPRI />");

                    this.Pub1.AddTD(dr["RDT"].ToString().Substring(5));
                    this.Pub1.AddTD(dr["SDTOfFlow"].ToString().Substring(5));
                    this.Pub1.AddTD(dr["SDTOfNode"].ToString().Substring(5));

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
            this.Pub1.AddTableEnd();
        }
    }
}