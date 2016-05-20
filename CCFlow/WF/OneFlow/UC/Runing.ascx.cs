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

 
    public partial class WF_OneFlow_UC_Runing : BP.Web.UC.UCBase3
    {
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
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                return s;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerRuning(WebUser.No, this.FK_Flow);
            this.Page.Title = "在途工作";
            if (WebUser.IsWap)
            {
                this.BindWap();
                return;
            }

            string appPath = this.Request.ApplicationPath;
            int colspan = 6;
            this.Pub1.AddTable("border=1px align=center width='100%'");
            if (WebUser.IsWap)
                this.Pub1.AddCaption("<img src='/WF/Img/Home.gif' >&nbsp;<a href='Home.aspx' >Home</a>-<img src='/WF/Img/EmpWorks.gif' >" + "在途工作");
            else
                this.Pub1.AddCaptionLeft("<img src='/WF/Img/Runing.gif' >&nbsp;<b>运行中</b>");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("nowarp=true", "序");
            this.Pub1.AddTDTitle("nowarp=true", "标题");
            if (this.GroupBy != "NodeName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=NodeName&FK_Flow=" + this.FK_Flow + "' >当前节点</a>");

            if (this.GroupBy != "StarterName")
                this.Pub1.AddTDTitle("<a href='" + this.PageID + ".aspx?GroupBy=StarterName&FK_Flow=" + this.FK_Flow + "' >发起人</a>");

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
            string title = null;
            string workid = null;
            string fk_flow = null;
            int gIdx = 0;
            string[] gVals = groupVals.Split('@');
            foreach (string g in gVals)
            {
                if (string.IsNullOrEmpty(g))
                    continue;

                gIdx++;
                this.Pub1.AddTR();
                this.Pub1.AddTD("colspan=" + colspan + " class=Sum onclick=\"GroupBarClick('" + appPath + "','" + gIdx + "')\" ", "<div style='text-align:left; float:left' ><img src='/WF/Style/Min.gif' alert='Min' id='Img" + gIdx + "'   border=0 />&nbsp;<b>" + g + "</b>");
                this.Pub1.AddTREnd();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr[this.GroupBy].ToString() != g)
                        continue;
                    i++;
                    this.Pub1.AddTR("ID='" + gIdx + "_" + i + "'");
                    this.Pub1.AddTDIdx(i);

                    BP.WF.WFState wfstate = (WFState)int.Parse(dr["WFState"].ToString());
                    title = "<img src='/WF/Img/WFState/" + wfstate + ".png' class='Icon' />" + dr["Title"].ToString();

                    workid = dr["WorkID"].ToString();
                    fk_flow = dr["FK_Flow"].ToString();

                    this.Pub1.Add("<TD width='50%' class=TTD><a href=\"javascript:WinOpen('" + appPath + "WF/WFRpt.aspx?WorkID=" + workid + "&FK_Flow=" + fk_flow + "&FID=" + dr["FID"] + "')\" >" + title + "</a></TD>");

                    if (this.GroupBy != "NodeName")
                        this.Pub1.AddTD(dr["NodeName"].ToString());

                    if (this.GroupBy != "StarterName")
                        this.Pub1.AddTD(dr["StarterName"].ToString());

                    this.Pub1.AddTD(dr["RDT"].ToString().Substring(5));
                    this.Pub1.AddTDBegin();
                    this.Pub1.Add("<a href=\"javascript:UnSend('" + appPath + "','" + this.PageSmall + "','" + dr["FID"] + "','" + workid + "','" + fk_flow + "');\" ><img src='/WF/Img/Action/UnSend.png' border=0 class=Icon />撤消发送</a>");
                    this.Pub1.Add("<a href=\"javascript:Press('" + appPath + "','" + dr["FID"] + "','" + workid + "','" + fk_flow + "');\" ><img src='/WF/Img/Action/Press.png' border=0 class=Icon />催办</a>");

                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTRSum();
            this.Pub1.AddTD("colspan=" + colspan, "&nbsp;");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
        public void BindWap()
        {
            this.Clear();
            this.AddFieldSet("<img src='/WF/Img/Home.gif' ><a href='Home.aspx' >Home</a>-<img src='/WF/Img/EmpWorks.gif' >在途工作" );
            string sql = " SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B  WHERE A.WorkID=B.WorkID   AND B.FK_EMP='" + BP.Web.WebUser.No + "' AND B.IsEnable=1";
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            int i = 0;
            bool is1 = true;
            //this.Add("<Table border=0 width='100%'>");
            this.AddUL();
            foreach (GenerWorkFlow gwf in gwfs)
            {
                i++;
                is1 = this.AddTR(is1);
                this.AddTDBegin("border=0");

                //this.AddUL();
                //  this.AddLi("MyFlow.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow, gwf.Title + gwf.NodeName);
                this.AddLi(gwf.Title + gwf.NodeName);

                this.Add("<a href=\"javascript:Do('您确认吗？','MyFlowInfo.aspx?DoType=UnSend&FID=" + gwf.FID + "&WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "');\" ><img src='/WF/Img/btn/delete.gif' border=0 />撤消</a>");
                this.Add("<a href=\"javascript:WinOpen('./../WF/WFRpt.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FID=0')\" ><img src='/WF/Img/btn/rpt.gif' border=0 />报告</a>");
            }
            this.AddULEnd();

            this.AddFieldSetEnd();
        }
        public void BindWap_bal()
        {
            this.Clear();

            int colspan = 7;
            this.AddTable("width='100%' align=center");
            this.AddTR();
            this.Add("<TD class=TitleTop colspan=" + colspan + "></TD>");
            this.AddTREnd();

            this.AddTR();
            if (WebUser.IsWap)
                this.Add("<TD align=left class=TitleMsg colspan=" + colspan + "><img src='/WF/Img/Home.gif' ><a href='Home.aspx' >Home</a>-<img src='/WF/Img/EmpWorks.gif' >在途工作</TD>");
            else
                this.Add("<TD class=TitleMsg colspan=" + colspan + " align=left><img src='/WF/Img/Runing.gif' ><b>在途工作</b></TD>");
            this.AddTREnd();

            this.AddTR();
            this.AddTDTitle("nowarp=true", "序");
            this.AddTDTitle("nowarp=true", "名称");
            this.AddTDTitle("nowarp=true", "当前节点");
            this.AddTDTitle("nowarp=true", "发起日期");
            this.AddTDTitle("nowarp=true", "发起人");
            this.AddTDTitle("nowarp=true", "操作");
            this.AddTDTitle("nowarp=true", "报告");

            this.AddTREnd();

            string sql = "  SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B  WHERE A.WorkID=B.WorkID   AND B.FK_EMP='" + BP.Web.WebUser.No + "' AND B.IsEnable=1";
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            int i = 0;
            bool is1 = false;
            foreach (GenerWorkFlow gwf in gwfs)
            {
                i++;
                is1 = this.AddTR(is1);
                this.AddTD(i);
                this.AddTDA("MyFlow.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow, gwf.Title);
                this.AddTD(gwf.NodeName);
                this.AddTD(gwf.RDT);
                this.AddTD(gwf.StarterName);
                this.AddTD("<a href=\"javascript:Do('您确认吗？','MyFlowInfo.aspx?DoType=UnSend&FID=" + gwf.FID + "&WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "');\" ><img src='/WF/Img/Btn/delete.gif' border=0 />撤消</a>");
                this.AddTD("<a href=\"javascript:WinOpen('./../WF/WFRpt.aspx?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FID=0')\" ><img src='/WF/Img/Btn/rpt.gif' border=0 />报告</a>");
                this.AddTREnd();
            }

            this.AddTRSum();
            this.AddTD("colspan=" + colspan, "&nbsp;");
            this.AddTREnd();
            this.AddTableEnd();
        }
    }
 