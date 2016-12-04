using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_JumpCheck : BP.Web.WebPage
    {

        #region 属性.
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        public int ToNode
        {
            get
            {
                return int.Parse(this.Request.QueryString["ToNode"]);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.

        public void BindWorkList()
        {
            string pageid = this.Request.RawUrl.ToLower();
            if (pageid.Contains("small"))
            {
                if (pageid.Contains("single"))
                    pageid = "SmallSingle";
                else
                    pageid = "Small";
            }
            else
            {
                pageid = "";
            }
            int colspan = 10;
            this.Pub1.AddTable("width='100%'");
            this.Pub1.AddCaptionMsg("取回审批");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("标题");
            this.Pub1.AddTDTitle("发起人");
            this.Pub1.AddTDTitle("发起时间");
            this.Pub1.AddTDTitle("停留节点");
            this.Pub1.AddTDTitle("当前处理人");
            this.Pub1.AddTDTitle("到达时间");
            this.Pub1.AddTDTitle("应完成时间");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            // 根据发起人的权限来判断，是否具有操作此人员的权限。
            GetTasks jcs = new GetTasks(this.FK_Flow);
            string canDealNodes = "";
            int idx = 1;
            foreach (BP.WF.GetTask jc in jcs)
            {
                /* 判断我是否可以处理当前点数据？ */
                if (jc.Can_I_Do_It() == false)
                    continue;

                canDealNodes += "''";
                DataTable dt = DBAccess.RunSQLReturnTable("SELECT * FROM WF_EmpWorks WHERE FK_Node IN (" + jc.CheckNodes + ") AND FK_Flow='" + this.FK_Flow + "' AND FK_Dept LIKE '" + BP.Web.WebUser.FK_Dept + "%'");
                if (dt.Rows.Count == 0)
                {
                    if (BP.Web.WebUser.FK_Dept.Length >= 4)
                        dt = DBAccess.RunSQLReturnTable("SELECT * FROM WF_EmpWorks WHERE FK_Node IN (" + jc.CheckNodes + ") AND FK_Flow='" + this.FK_Flow + "' AND FK_Dept LIKE '" + BP.Web.WebUser.FK_Dept.Substring(0, 2) + "%'");
                    else
                        dt = DBAccess.RunSQLReturnTable("SELECT * FROM WF_EmpWorks WHERE FK_Node IN (" + jc.CheckNodes + ") AND FK_Flow='" + this.FK_Flow + "' AND FK_Dept LIKE '" + BP.Web.WebUser.FK_Dept + "%'");
                }

                this.Pub1.AddTR();
                this.Pub1.Add("<TD  class=TRSum colspan=" + colspan + " align=left>" + jc.Name + " ;  =》可跳转审核的节点:" + jc.CheckNodes + "</TD>");
                this.Pub1.AddTREnd();
                foreach (DataRow dr in dt.Rows)
                {
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx++);
                    this.Pub1.AddTD(dr["Title"].ToString());
                    this.Pub1.AddTD(dr["Starter"].ToString());
                    this.Pub1.AddTD(dr["RDT"].ToString());
                    this.Pub1.AddTD(dr["NodeName"].ToString());
                    this.Pub1.AddTD(dr["FK_EmpText"].ToString());
                    this.Pub1.AddTD(dr["ADT"].ToString());
                    this.Pub1.AddTD(dr["SDT"].ToString());
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('WFRpt.aspx?WorkID=" + dr["WorkID"] + "&FK_Flow=" + this.FK_Flow + "&FID=" + dr["FID"] + "')\">报告</a> - <a href=\"javascript:Tackback('" + this.FK_Flow + "','" + dr["FK_Node"] + "','" + jc.NodeID + "','" + dr["WorkID"] + "')\">取回</a>");
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string pageid = this.Request.RawUrl.ToLower();
            if (pageid.Contains("small"))
            {
                if (pageid.Contains("single"))
                    pageid = "SmallSingle";
                else
                    pageid = "Small";
            }
            else
            {
                pageid = "";
            }

            if (this.DoType == "Tackback")
            {
                /* */
                try
                {
                    string s = BP.WF.Dev2Interface.Node_Tackback(this.FK_Node, this.WorkID, this.ToNode);
                    //  s=s.Replace(
                    this.Pub1.AddTable("width='90%' align=left");
                    this.Pub1.AddCaptionMsg("取回审批-<a href=GetTask" + pageid + ".aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + ">返回</a>");
                    this.Pub1.AddTR();
                    this.Pub1.AddTDBegin();
                    this.Pub1.AddMsgGreen("取回成功", "<h3>工作已经放入您的待办里</h3><hr><a href='MyFlow" + pageid + ".aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.ToNode + "&WorkID=" + this.WorkID + "&FID=0' >点这里请执行</a>。<br><br>");
                    this.Pub1.AddTDEnd();

                    this.Pub1.AddTR();
                    this.Pub1.AddTD(s);
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTableEnd();
                }
                catch (Exception ex)
                {
                    this.Pub1.AddMsgOfWarning("错误", ex.Message);
                }
                return;
            }

            if (this.FK_Flow != null)
            {
                this.BindWorkList();
                return;
            }

            Flows fls = new Flows();
            BP.En.QueryObject qo = new QueryObject(fls);
            qo.addOrderBy(FlowAttr.FK_FlowSort);
            qo.DoQuery();

            this.Pub1.AddTable("width='100%' align=left");
            this.Pub1.AddCaptionMsg("取回审批");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("流程类别");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("流程图");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTREnd();

            int i = 0;
            bool is1 = false;
            string fk_sort = null;
            foreach (Flow fl in fls)
            {
                if (fl.FlowAppType == FlowAppType.DocFlow)
                    continue;

                i++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(i);
                if (fl.FK_FlowSort == fk_sort)
                    this.Pub1.AddTD();
                else
                    this.Pub1.AddTDB(fl.FK_FlowSortText);

                fk_sort = fl.FK_FlowSort;

                this.Pub1.AddTD("<a href='GetTask" + pageid + ".aspx?FK_Flow=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01' >" + fl.Name + "</a>");

                this.Pub1.AddTD("<a href=\"javascript:WinOpen('/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&FK_Flow=" + fl.No + "&DoType=Chart','sd');\"  >打开</a>");
                this.Pub1.AddTD(fl.Note);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}