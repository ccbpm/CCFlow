using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;
namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_Personalize_GetTask : BP.Web.WebPage
    {
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.RefNo == null)
            {
                this.BindList();
                return;
            }

            if (this.Step == 1)
            {
                this.BindStep1();
                return;
            }

            if (this.Step == 2)
            {
                this.BindStep1();
                return;
            }

            if (this.Step == 3)
            {
                this.BindStep3();
                return;
            }
        }
        public void BindStep1()
        {
            this.Pub1.AddTable("width='70%'");
            this.Pub1.AddCaptionLeft("设置流程条跳转审批规则:<a href='GetTask.aspx'>返回</a> ");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("节点编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("步骤");

            this.Pub1.AddTDTitle("可跳转审核的节点");

            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            BP.WF.GetTasks jcs = new GetTasks();
            jcs.Retrieve(NodeAttr.FK_Flow, this.RefNo);

            int idx = 0;
            foreach (BP.WF.GetTask jc in jcs)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD(jc.NodeID);
                this.Pub1.AddTD(jc.Name);
                this.Pub1.AddTD(jc.Step);
                this.Pub1.AddTD(jc.CheckNodes);

                if (jc.Step == 1 || jc.Step == 2)
                {
                    this.Pub1.AddTD();
                }
                else
                {
                    switch (jc.HisDeliveryWay)
                    {
                        case DeliveryWay.ByBindEmp:
                        case DeliveryWay.ByStation:
                            if (jc.CheckNodes.Length > 2)
                                this.Pub1.AddTD("<a href=\"javascript:EditIt('" + this.RefNo + "','" + jc.NodeID + "');\" >编辑</a>");
                            else
                                this.Pub1.AddTD("<a href=\"javascript:EditIt('" + this.RefNo + "','" + jc.NodeID + "');\" >创建</a>");
                            break;
                        default:
                            this.Pub1.AddTD("访问规则非按人员或按部门");
                            break;
                    }
                }
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
        public void BindStep3()
        {
            this.Pub1.AddTable("width='70%'");
            this.Pub1.AddCaptionLeft("选择可以跳转审核的节点");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("节点编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("步骤");
            this.Pub1.AddTDTitle("选择");
            this.Pub1.AddTREnd();

            BP.WF.GetTasks jcs = new GetTasks();
            jcs.Retrieve(NodeAttr.FK_Flow, this.RefNo);

            BP.WF.GetTask myjc = new GetTask(this.FK_Node);
            string nodes = myjc.CheckNodes;

            int idx = 0;
            foreach (BP.WF.GetTask jc in jcs)
            {
                if (jc.Step == 1)
                    continue;

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD(jc.NodeID);
                this.Pub1.AddTD(jc.Name);
                this.Pub1.AddTD(jc.Step);

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + jc.NodeID;
                cb.Text = "选择";
                if (nodes.Contains(jc.NodeID.ToString()))
                {
                    cb.Checked = true;
                    cb.Text = "<font color=green>" + jc.Name + "</font>";
                }
                else
                {
                    cb.Checked = false;
                }

                this.Pub1.AddTD(cb);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTR();
            this.Pub1.Add("<TD colspan=5>");

            Button btn = new Button();
            btn.Text = "保存";
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.Text = "Cancel";
            btn.ID = "Btn_Cancel";
            btn.CssClass = "Btn";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            this.Pub1.Add("</TD>");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEndWithHR();

        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.ID == "Btn_Cancel")
            {
                this.WinClose();
                return;
            }

            BP.WF.GetTasks jcs = new GetTasks();
            jcs.Retrieve(NodeAttr.FK_Flow, this.RefNo);

            string strs = "";
            foreach (GetTask jc in jcs)
            {
                if (jc.Step == 1)
                    continue;

                if (this.Pub1.GetCBByID("CB_" + jc.NodeID).Checked == false)
                    continue;

                strs += "'" + jc.NodeID + "',";
            }

            if (strs.Length == 0)
            {
                this.Alert("您没有设置可以跳转审核的节点。");
                return;
            }
            strs = strs.Substring(0, strs.Length - 1);
            GetTask myjc = new GetTask(this.FK_Node);
            myjc.CheckNodes = strs;
            myjc.Update();
            this.WinCloseWithMsg("保存成功...");
        }
        public void BindList()
        {
            Flows fls = new Flows();
            fls.RetrieveAll();
            //        this.Pub1.AddTable();
            this.Pub1.AddTable("width='70%'");
            this.Pub1.AddCaptionLeft("设置流程条跳转审批规则");
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
                this.Pub1.AddTD("<a href='GetTask.aspx?RefNo=" + fl.No + "&FK_Node=" + int.Parse(fl.No) + "01' >" + fl.Name + "</a>");

                this.Pub1.AddTD("<a href=\"javascript:WinOpen('/WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fl.No + "&DoType=Chart','sd');\"  >打开</a>");
                this.Pub1.AddTD(fl.Note);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTRSum();
            this.Pub1.AddTD("colspan=" + 5, "&nbsp;");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
    }
}