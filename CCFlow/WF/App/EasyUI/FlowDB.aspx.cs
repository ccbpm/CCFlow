using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.En;
using BP.DA;
using BP.WF;

namespace CCFlow.App.FlowDB
{
    public partial class FlowDB : BP.Web.WebPage
    {
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (string.IsNullOrEmpty(s))
                    s = "200";
                return s;
            }
        }
        public int WorkID
        {
            get
            {
                string s = this.Request.QueryString["WorkID"];
                if (string.IsNullOrEmpty(s))
                    s = "0";
                return int.Parse(s);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (this.DoType== "DelIt")
            {
                try
                {
                    WorkFlow wf = new WorkFlow(this.FK_Flow, this.WorkID);
                    wf.DoDeleteWorkFlowByReal(true);
                }
                catch (Exception ex)
                {
                    this.Response.Write(ex.Message);

                    BP.Sys.PubClass.Alert(ex.Message);
               
                }
                return;
            }

            Flow fl = new Flow(this.FK_Flow);

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FK_Flow, this.FK_Flow, GenerWorkFlowAttr.FK_Dept);
            this.Pub1.AddTable("width='100%'");
            this.Pub1.AddCaptionLeft(fl.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("部门");
            this.Pub1.AddTDTitle("发起人");
            this.Pub1.AddTDTitle("发起时间");
            this.Pub1.AddTDTitle("当前停留节点");
            this.Pub1.AddTDTitle("标题");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            foreach (GenerWorkFlow item in gwfs)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD(item.DeptName);
                this.Pub1.AddTD(item.StarterName);
                this.Pub1.AddTD(item.RDT);
                this.Pub1.AddTD(item.NodeName);
                this.Pub1.AddTDB(item.Title);

                this.Pub1.AddTDBegin();
                this.Pub1.Add("<a href=\"javascript:WinOpen('/WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + item.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + item.FID + "','ds'); \" >轨迹图</a>-");
                this.Pub1.Add("<a href=\"javascript:WinOpen('./../WFRpt.aspx?WorkID=" + item.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + item.FID + "','ds'); \" >报告</a>-");
                this.Pub1.Add("[<a href=\"javascript:DelIt('" + item.FK_Flow + "','" + item.WorkID + "');\"><img src='/WF/Img/Btn/Delete.gif' border=0/>删除</a>]");
                this.Pub1.Add("[<a href=\"javascript:FlowShift('" + item.FK_Flow + "','" + item.WorkID + "');\">移交</a>]");
                this.Pub1.Add("[<a href=\"javascript:FlowSkip('" + item.FK_Flow + "','" + item.WorkID + "');\">跳转</a>]");
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
        
    }
}