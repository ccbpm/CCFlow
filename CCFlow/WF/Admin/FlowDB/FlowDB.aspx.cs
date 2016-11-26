using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP;
using BP.Web;
using BP.En;
using BP.DA;
using BP.Web.Controls;
using BP.WF;
using BP.Sys;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_FlowDB : BP.Web.WebPage
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

        /// <summary>
        /// 是否查询
        /// </summary>
        public bool IsSearch
        {
            get
            {
                return (Request.QueryString["IsSearch"] ?? "0") == "1";
            }
        }

        public string Depts
        {
            get
            {
                return this.Request.QueryString["Depts"];
            }
        }

        public string Emps
        {
            get
            {
                return this.Request.QueryString["Emps"];
            }
        }

        public string DeptsText
        {
            get
            {
                return Request.QueryString["DeptsText"];
            }
        }

        public string EmpsText
        {
            get
            {
                return Request.QueryString["EmpsText"];
            }
        }

        public string DateFrom
        {
            get
            {
                return this.Request.QueryString["DateFrom"];
            }
        }

        public string DateTo
        {
            get
            {
                return this.Request.QueryString["DateTo"];
            }
        }

        public string Keywords
        {
            get
            {
                return Request.QueryString["Keywords"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //add by dgq 不要在这里做转，影响其他地方使用
            //this.Response.Redirect("../../Comm/Search.aspx?EnsName=BP.WF.Data.Monitors", true);
            //return;

            if (this.DoType == "DelIt")
            {
                try
                {
                    WorkFlow wf = new WorkFlow(this.FK_Flow, this.WorkID);
                    wf.DoDeleteWorkFlowByReal(true);
                }
                catch (Exception ex)
                {
                    this.Response.Write(ex.Message);
                    this.Alert(ex.Message);
                }
                return;
            }

            Pub3.Add("<div style='width:100%; padding: 2px; height: auto;background-color:#E0ECFF; line-height:30px'>");

            Pub3.Add("部门：");
            var tb = new TB();
            tb.ID = "TB_Dept";
            tb.Text = DeptsText;
            tb.Style.Add("width", "100px");
            Pub3.Add(tb);

            var hid = new HiddenField();
            hid.ID = "Hid_Dept";
            hid.Value = Depts;
            Pub3.Add(hid);

            Pub3.Add("<a class='easyui-linkbutton' href=\"javascript:openSelectDept('" + hid.ClientID + "','" + tb.ClientID + "')\" data-options=\"iconCls:'icon-department',plain:true\" title='选择部门'> </a>&nbsp;&nbsp;");

            Pub3.Add("发起人：");
            tb = new TB();
            tb.ID = "TB_FQR";
            tb.Text = EmpsText;
            tb.Style.Add("width", "100px");
            Pub3.Add(tb);

            hid = new HiddenField();
            hid.ID = "Hid_FQR";
            hid.Value = Emps;
            Pub3.Add(hid);

            Pub3.Add("<a class='easyui-linkbutton' href=\"javascript:openSelectEmp('" + hid.ClientID + "','" + tb.ClientID + "')\" data-options=\"iconCls:'icon-user',plain:true\" title='选择发起人'> </a>&nbsp;&nbsp;");

            Pub3.Add("发起日期：");
            tb = new TB();
            tb.ID = "TB_DateFrom";
            tb.Text = DateFrom;
            tb.Style.Add("width", "80px");
            tb.Attributes["onfocus"] = "WdatePicker();";
            Pub3.Add(tb);

            Pub3.AddSpace(1);
            Pub3.Add("到");
            Pub3.AddSpace(1);

            tb = new TB();
            tb.ID = "TB_DateTo";
            tb.Text = DateTo;
            tb.Style.Add("width", "80px");
            tb.Attributes["onfocus"] = "WdatePicker();";
            Pub3.Add(tb);
            Pub3.AddSpace(2);

            Pub3.Add("关键字：");
            tb = new TB();
            tb.ID = "TB_KeyWords";
            tb.Text = Keywords;
            tb.Style.Add("width", "100px");
            Pub3.Add(tb);

            var lbtn = new LinkBtn(false, NamesOfBtn.Search, "查询");
            lbtn.Click += new EventHandler(lbtn_Click);

            Pub3.AddSpace(1);
            Pub3.Add(lbtn);

            Pub3.AddDivEnd();

            if (IsSearch)
                BindSearch();
        }

        void lbtn_Click(object sender, EventArgs e)
        {
            var depts = (Pub3.FindControl("Hid_Dept") as HiddenField).Value;
            var emps = (Pub3.FindControl("Hid_FQR") as HiddenField).Value;
            var deptsText = Pub3.GetTBByID("TB_Dept").Text;
            var empsText = Pub3.GetTBByID("TB_FQR").Text;
            var dateFrom = Pub3.GetTBByID("TB_DateFrom").Text;
            var dateTo = Pub3.GetTBByID("TB_DateTo").Text;
            var keywords = Pub3.GetTBByID("TB_KeyWords").Text;

            var url = string.Format("FlowDB.aspx?FK_Flow={0}&WorkID={1}&IsSearch=1&Depts={2}&DeptsText={3}&Emps={4}&EmpsText={5}&DateFrom={6}&DateTo={7}&Keywords={8}", FK_Flow, WorkID, depts, deptsText, emps, empsText, dateFrom, dateTo, keywords);

            Response.Redirect(url, true);
        }

        private void BindSearch()
        {
            Flow fl = new Flow(this.FK_Flow);
            var gwfs = new GenerWorkFlows();
            var qo = new QueryObject(gwfs);
            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, FK_Flow);

            if (!string.IsNullOrWhiteSpace(Depts))
            {
                qo.addAnd();
                qo.AddWhereIn(GenerWorkFlowAttr.FK_Dept, "(" + Depts + ")");
            }

            if (!string.IsNullOrWhiteSpace(Emps))
            {
                qo.addAnd();
                qo.AddWhereIn(GenerWorkFlowAttr.Starter, "(" + Emps.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Aggregate(string.Empty, (curr, next) => curr + "'" + next + "',").TrimEnd(',') + ")");
            }

            if (!string.IsNullOrWhiteSpace(DateFrom))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.RDT, ">", DateFrom);
            }

            if (!string.IsNullOrWhiteSpace(DateTo))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.RDT, "<=", DateTo);
            }

            if (!string.IsNullOrWhiteSpace(Keywords))
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.Title, "LIKE", "%" + Keywords + "%");
            }

            qo.addOrderBy(GenerWorkFlowAttr.RDT);

            var url = string.Format("FlowDB.aspx?FK_Flow={0}&WorkID={1}&IsSearch=1&Depts={2}&DeptsText={3}&Emps={4}&EmpsText={5}&DateFrom={6}&DateTo={7}&Keywords={8}", FK_Flow, WorkID, Depts, DeptsText, Emps, EmpsText, DateFrom, DateTo, Keywords);

            Pub2.BindPageIdxEasyUi(qo.GetCount(), url, this.PageIdx, SystemConfig.PageSize);

            qo.DoQuery(gwfs.GetNewEntity.PK, SystemConfig.PageSize, this.PageIdx);

            this.Pub1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%'");

            Pub1.AddTR();
            Pub1.AddTDGroupTitle("colspan='8'", fl.Name);
            Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("style='text-align:center'", "序号");
            this.Pub1.AddTDGroupTitle("部门");
            this.Pub1.AddTDGroupTitle("发起人");
            this.Pub1.AddTDGroupTitle("发起时间");
            this.Pub1.AddTDGroupTitle("当前停留节点");
            this.Pub1.AddTDGroupTitle("标题");
            this.Pub1.AddTDGroupTitle("处理人");
            this.Pub1.AddTDGroupTitle("操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            foreach (GenerWorkFlow item in gwfs)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(SystemConfig.PageSize * (this.PageIdx - 1) + idx);
                this.Pub1.AddTD(item.DeptName);
                this.Pub1.AddTD(item.StarterName);
                this.Pub1.AddTD(item.RDT);
                this.Pub1.AddTD(item.NodeName);
                this.Pub1.AddTD(item.Title);
                this.Pub1.AddTD(item.TodoEmps);

                this.Pub1.AddTDBegin();
                this.Pub1.Add("<a href=\"javascript:WinOpen('./../../WFRpt.aspx?WorkID=" + item.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + item.FID + "','ds'); \" class='easyui-linkbutton'>轨迹</a>&nbsp;");
                //this.Pub1.Add("<a href=\"javascript:WinOpen('../../../WFRpt.aspx?WorkID=" + item.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + item.FID + "&FK_Node=" + item.FK_Node + "','ds'); \" >报告</a>-");
                this.Pub1.Add("<a href=\"javascript:DelIt('" + item.FK_Flow + "','" + item.WorkID + "');\" class='easyui-linkbutton' data-options=\"iconCls:'icon-delete'\" onclick=\"return confirm('您确定要删除吗？');\">删除</a>&nbsp;");
                this.Pub1.Add("<a href=\"javascript:FlowShift('" + item.FK_Flow + "','" + item.WorkID + "');\" class='easyui-linkbutton'>移交</a>&nbsp;");
                this.Pub1.Add("<a href=\"javascript:FlowSkip('" + item.FK_Flow + "','" + item.WorkID + "');\" class='easyui-linkbutton'>跳转</a>");
                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTableEnd();
        }
    }
}