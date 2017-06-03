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
    public partial class TaskPoolApplySmall :BP.Web.WebPage
    {
        #region 属性.
        public string _PageSamll = null;
        public string PageSmall
        {
            get
            {
                if (_PageSamll == null)
                {
                    if (this.PageID.ToLower().Contains("smallsingle"))
                        _PageSamll = "SmallSingle";
                    else if (this.PageID.ToLower().Contains("small"))
                        _PageSamll = "Small";
                    else
                        _PageSamll = "";
                }
                return _PageSamll;
            }
        }
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

        public void BindList()
        {

           
        }
        public DataTable dt = null;
        string timeKey;
        protected void Page_Load(object sender, EventArgs e)
        {
            timeKey = DateTime.Now.ToString("yyyyMMddHHmmss");
            this.FK_Flow = this.Request.QueryString["FK_Flow"];
            dt = BP.WF.Dev2Interface.DB_TaskPoolOfMyApply();
            if (dt.Rows.Count == 0)
            {
                this.Pub1.AddMsgOfInfoV2LongTitle("<a href='TaskPoolSharing.aspx' >1.任务池</a> -  》2.申请下来工作",
                    "当前任务池里没有您申请下来的待办工作......");
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

            int colspan = 11;
            this.Pub1.AddTable("width='100%' align=left");
            this.Pub1.AddCaptionMsgLong("<a href='TaskPoolSharing.aspx' >1.任务池</a> -  》2.申请下来工作");

            string extStr = "";
            if (this.IsHungUp)
                extStr = "&IsHungUp=1";

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("width=40%", "标题");

            if (this.GroupBy != "FlowName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=FlowName" + extStr + "&T=" + this.timeKey + "' >流程</a>");

            if (this.GroupBy != "NodeName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=NodeName" + extStr + "&T=" + this.timeKey + "' >节点</a>");

            if (this.GroupBy != "StarterName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=StarterName" + extStr + "&T=" + this.timeKey + "' >发起人</a>");

                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=PRI" + extStr + "&T=" + this.timeKey + "' >优先级</a>");

            this.Pub1.AddTDTitle("发起日期");
            this.Pub1.AddTDTitle("接受日期");
            this.Pub1.AddTDTitle("应完成日期");
            this.Pub1.AddTDTitle("状态");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            int i = 0;
            bool is1 = false;
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
                    this.Pub1.AddTD("colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>");
                else
                    this.Pub1.AddTD("colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g.Replace(",", "") + "</b>");
                this.Pub1.AddTREnd();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[this.GroupBy].ToString() + "," != g)
                        continue;
                    string sdt = dr["SDT"] as string;
                    is1 = this.Pub1.AddTR(is1, "ID='" + gIdx + "_" + i + "'");
                    i++;

                    int isRead = int.Parse(dr["IsRead"].ToString());
                    this.Pub1.AddTDIdx(i);
                    if (BP.WF.Glo.IsWinOpenEmpWorks == true)
                    {
                        if (isRead == 0)
                            this.Pub1.AddTD("Class=TTD onclick=\"SetImg('" + appPath + "','I" + gIdx + "_" + i + "')\"", "<a href=\"javascript:WinOpenIt('" + appPath + "WF/MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&IsRead=0&T=" + this.timeKey + "');\" ><img class=Icon src='/WF/Img/Mail_UnRead.png' id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>");
                        else
                            this.Pub1.AddTD("Class=TTD", "<a href=\"javascript:WinOpenIt('/WF/MyFlow.aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "');\"  ><img src='/WF/Img/Mail_Read.png' id='I" + gIdx + "_" + i + "' class=Icon />" + dr["Title"].ToString() + "</a>");
                    }
                    else
                    {
                        if (isRead == 0)
                            this.Pub1.AddTD("Class=TTD onclick=\"SetImg('" + appPath + "','I" + gIdx + "_" + i + "')\" ", "<a href=\"MyFlow" + this.PageSmall + ".aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&IsRead=0&T=" + this.timeKey + "\" ><img class=Icon src='/WF/Img/Mail_UnRead.png' id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>");
                        else
                            this.Pub1.AddTD("Class=TTD ", "<a href=\"MyFlow" + this.PageSmall + ".aspx?FK_Flow=" + dr["FK_Flow"] + "&FK_Node=" + dr["FK_Node"] + "&FID=" + dr["FID"] + "&WorkID=" + dr["WorkID"] + "&T=" + this.timeKey + "\" ><img class=Icon src='/WF/Img/Mail_Read.png' id='I" + gIdx + "_" + i + "' />" + dr["Title"].ToString() + "</a>");
                    }

                    if (this.GroupBy != "FlowName")
                        this.Pub1.AddTD(dr["FlowName"].ToString());

                    if (this.GroupBy != "NodeName")
                        this.Pub1.AddTD(dr["NodeName"].ToString());

                    if (this.GroupBy != "StarterName")
                        this.Pub1.AddTD(dr["Starter"].ToString() + " " + dr["StarterName"]);

                        this.Pub1.AddTD("<img class=Icon src='/WF/Img/PRI/" + dr["PRI"].ToString() + ".png' class=Icon />");

                    this.Pub1.AddTD(dr["RDT"].ToString().Substring(5));
                    this.Pub1.AddTD(dr["ADT"].ToString().Substring(5));
                    this.Pub1.AddTD(dr["SDT"].ToString().Substring(5));

                    DateTime mysdt = DataType.ParseSysDate2DateTime(sdt);
                    if (cdt >= mysdt)
                    {
                        if (cdt.ToString("yyyy-MM-dd") == mysdt.ToString("yyyy-MM-dd"))
                            this.Pub1.AddTDCenter("<img src='/WF/Img/TolistSta/0.png' class='Icon'/><font color=green>正常</font>");
                        else
                            this.Pub1.AddTDCenter("<img src='/WF/Img/TolistSta/2.png' class='Icon'/><font color=red>逾期</font>");
                    }
                    else
                    {
                        this.Pub1.AddTDCenter("<img src='/WF/Img/TolistSta/0.png' class='Icon'/><font color=green>正常</font>");
                    }
                    this.Pub1.AddTD(dr["FlowNote"].ToString());
                    this.Pub1.AddTD("<a href=\"javascript:PutOne('" + dr["WorkID"].ToString() + "')\" ><img src='/WF/Img/Action/put.png' class='Icon'/>放入</a>");
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
        }
    }
}