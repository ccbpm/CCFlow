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

namespace CCFlow.WF
{
    public partial class WF_Runing :  BP.Web.WebPage
    {
        #region 属性.
        public string GroupBy
        {
            get
            {
                string s = this.Request.QueryString["GroupBy"];
                if (s == null)
                    s = "FlowName";
                return s;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt;

            string fk_flow=this.Request.QueryString["FK_Flow"];
            if (string.IsNullOrEmpty(fk_flow))
                dt= BP.WF.Dev2Interface.DB_GenerRuning();
            else
                dt= BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No,fk_flow);

            this.Page.Title = "在途";
            if (dt.Rows.Count == 0)
            {
                this.Pub1.AddMsgOfInfoV2("在途",
                    "当前没有在途工作......");
                return;
            }

          
            string appPath = BP.WF.Glo.CCFlowAppPath; 

            int colspan = 6;
            this.Pub1.AddTable("border=1px align=center width='100%'");

            if (WebUser.IsWap)
                this.Pub1.AddCaptionMsg("<a href='Home.aspx' >Home</a>-<img src='Img/EmpWorks.gif' >在途工作");
            else
                this.Pub1.AddCaptionMsg("在途工作");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("nowarp=true", "序");
            this.Pub1.AddTDTitle("nowarp=true width='40%'", "标题");

            if (this.GroupBy != "FlowName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=FlowName&FK_Flow=" + fk_flow + "' >流程</a>");

            if (this.GroupBy != "NodeName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=NodeName&FK_Flow=" + fk_flow + "' >当前节点</a>");

            if (this.GroupBy != GenerWorkFlowAttr.StarterName)
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=StarterName&FK_Flow=" + fk_flow + "' >发起人</a>");

            this.Pub1.AddTDTitle("nowarp=true", "发起日期");
            this.Pub1.AddTDTitle("nowarp=true", "操作");
            this.Pub1.AddTREnd();

            string groupVals = "";
            foreach (DataRow dr in dt.Rows)
            {
                if (groupVals.Contains("@" + dr[this.GroupBy]))
                    continue;
                groupVals += "@" + dr[this.GroupBy];
            }

            int i = 0;
            bool is1 = false;
            string title = null;
            string workid = null;
            fk_flow = null;
            int gIdx = 0;
            string[] gVals = groupVals.Split('@');
            foreach (string g in gVals)
            {
                if (string.IsNullOrEmpty(g))
                    continue;

                gIdx++;
                this.Pub1.AddTR();
                this.Pub1.AddTD("colspan=" + colspan + " class=TRSum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g + "</b>");
                this.Pub1.AddTREnd();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[this.GroupBy].ToString() != g)
                        continue;
                    i++;
                    is1=this.Pub1.AddTR(is1, "ID='" + gIdx + "_" + i + "'");
                    this.Pub1.AddTDIdx(i);

                    BP.WF.WFState wfstate = (WFState)int.Parse(dr["WFState"].ToString());
                    title = "<img src='Img/WFState/" + wfstate + ".png' class='Icon' />" + dr["Title"].ToString();

                    workid = dr["WorkID"].ToString();
                    fk_flow = dr["FK_Flow"].ToString();

                    this.Pub1.Add("<TD class=TTD><a href=\"javascript:WinOpen('WFRpt.aspx?WorkID=" + workid + "&FK_Flow=" + fk_flow + "&FID=" + dr["FID"] + "')\" >" + title + "</a></TD>");
                    //  this.Pub1.AddTDDoc(title, 50, title);
                    if (this.GroupBy != "FlowName")
                        this.Pub1.AddTD(dr["FlowName"].ToString());

                    if (this.GroupBy != "NodeName")
                        this.Pub1.AddTD(dr["NodeName"].ToString());

                    if (this.GroupBy != GenerWorkFlowAttr.StarterName)
                        this.Pub1.AddTD(dr[GenerWorkFlowAttr.StarterName].ToString());

                    this.Pub1.AddTD(dr["RDT"].ToString());
                    this.Pub1.AddTDBegin();
                    this.Pub1.Add("<a href=\"javascript:UnSend('" + appPath + "','','" + dr["FID"] + "','" + workid + "','" + fk_flow + "');\" ><img src='Img/Action/UnSend.png' border=0 class=Icon />撤消</a>");
                    this.Pub1.Add("&nbsp;&nbsp;<a href=\"javascript:Press('" + appPath + "','" + dr["FID"] + "','" + workid + "','" + fk_flow + "');\" ><img src='Img/Action/Press.png' border=0 class=Icon />催办</a>");

                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
            }
             
            this.Pub1.AddTableEnd();
        }
    }
}