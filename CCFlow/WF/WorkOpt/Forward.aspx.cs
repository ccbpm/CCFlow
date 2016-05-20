using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_Forward_UI : PageBase
    {
        #region 属性.
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                try
                {
                    return Int64.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FlowNo"];
                if (s == null)
                    s = this.Request.QueryString["FK_Flow"];
                return s;
            }
        }
        public string FK_Dept
        {
            get
            {
                string s = this.Request.QueryString["FK_Dept"];
                if (string.IsNullOrEmpty(s))
                    return WebUser.FK_Dept;
                return s;
            }
        }
        public bool IsEUI
        {
            get
            {
                string s = this.Request.QueryString["IsEUI"];
                if (s == null)
                    return false;
                else
                    return true;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

            this.ToolBar1.Add("<b>选择部门确定移交人:</b>");

            Depts depts = new Depts();
            depts.RetrieveAllFromDBSource();
            this.ToolBar1.AddDDL("DDL_Dept");
            DDL ddl = this.ToolBar1.GetDDLByID("DDL_Dept");
            ddl.AutoPostBack = true;
            ddl.EnableViewState = false;

            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            foreach (Dept dept in depts)
            {
                ListItem li = new ListItem();
                li.Text = BP.DA.DataType.GenerSpace(dept.Grade - 1) + dept.Name;
                li.Text = BP.DA.DataType.GenerSpace(dept.Grade - 1) + dept.Name;

                li.Text = li.Text.Replace("&nbsp;", "_");
                li.Value = dept.No;
                ddl.Items.Add(li);

                if (this.FK_Dept == li.Value)
                    li.Selected = true;
            }

            this.ToolBar1.AddBtn(NamesOfBtn.Forward, "移交");
            //  if (this.IsEUI == false)
            //{
            this.ToolBar1.AddBtn(NamesOfBtn.Cancel, "取消");
            this.ToolBar1.GetBtnByID(NamesOfBtn.Forward).Attributes["onclick"] = " return confirm('您确定要执行吗？');";
            this.ToolBar1.GetBtnByID(NamesOfBtn.Cancel).Click += new EventHandler(Forward_Click);

           // this.ToolBar1.AddLab("ds", "&nbsp;请选择移交人，输入移交原因，点移交按钮执行工作移交。");
            this.ToolBar1.GetBtnByID(NamesOfBtn.Forward).Click += new EventHandler(Forward_Click);
            this.BindLB();
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            this.Response.Redirect("Forward.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.Request.QueryString["FK_Node"] + "&FK_Flow=" + this.Request.QueryString["FK_Flow"] + "&FK_Dept=" + ddl.SelectedItemStringVal, true);
        }

        void Forward_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.ID)
            {
                case NamesOfBtn.Cancel:
                    this.Response.Redirect("../MyFlow.aspx?FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FID=" + this.FID, true);
                    //this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "e", "<script language=javascript>history.go(-1);</script>", true);
                    return;
                default:
                    break;
            }

            try
            {
                string msg = this.Top.GetTextBoxByID("TB_Doc").Text;
                if (msg == "请输入移交原因...")
                    throw new Exception("@您必须输入移交原因。");

                string sql = "";
                sql = " SELECT No,Name FROM Port_Emp WHERE FK_Dept='" + this.FK_Dept + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                string toEmp = "";
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["No"].ToString() == WebUser.No)
                        continue;
                    RadioButton rb = this.Top.GetRadioButtonByID("RB_" + dr["No"]);
                    if (rb == null || rb.Checked == false)
                        continue;
                    toEmp = dr["No"].ToString();
                }

                if (toEmp == "")
                {
                    this.Alert("请选择要移交的人员。");
                    return;
                }

                string info = BP.WF.Dev2Interface.Node_Shift(this.FK_Flow, this.FK_Node, this.WorkID, this.FID, toEmp, msg);

                this.Session["info"] = info;
                this.Response.Redirect(BP.WF.Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?DoType=Msg&FK_Flow=" + this.FK_Flow, true);
                return;
            }
            catch (Exception ex)
            {
                ShiftWork fw = new ShiftWork();
                fw.CheckPhysicsTable();

                Log.DefaultLogWriteLineWarning(ex.Message);
                this.Alert("工作移交出错：" + ex.Message);
            }
        }
        /// <summary>
        /// 绑定
        /// </summary>
        public void BindLB()
        {
            // 当前用的员工权限。
            string sql = "";
            // sql = " SELECT No,Name FROM Port_Emp WHERE NO IN (SELECT FK_EMP FROM Port_EmpDept WHERE FK_Dept IN (SELECT FK_Dept FROM Port_EmpDept WHERE fk_emp='" + BP.Web.WebUser.No + "') ) or FK_Dept Like '" + BP.Web.WebUser.FK_Dept + "%'";
            sql = " SELECT No,Name FROM Port_Emp WHERE FK_Dept='" + this.FK_Dept + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int colIdx = -1;

            this.Top.AddTable("style='border:0px;'");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["No"].ToString() == WebUser.No)
                    continue;

                colIdx++;
                if (colIdx == 0)
                    this.Top.AddTR();

                string no = dr["No"].ToString();
                string name = dr["Name"].ToString();
                RadioButton rb = new RadioButton();
                rb.ID = "RB_" + no;
                rb.Text = no + " " + name;
                rb.GroupName = "s";
                this.Top.AddTD("style='border:0px;'", rb);

                if (colIdx == 2)
                {
                    colIdx = -1;
                    this.Top.AddTREnd();
                }
            }
            this.Top.AddTableEnd();

            // 已经非配或者自动分配的任务。
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.IsEnable, 1,
                GenerWorkerListAttr.IsPass, 0);

            int nodeID = 0;
            foreach (GenerWorkerList wl in wls)
            {
                RadioButton cb = this.Top.GetRadioButtonByID("RB_" + wl.FK_Emp);
                if (cb != null)
                    cb.Checked = true;

                nodeID = wl.FK_Node;
            }
            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 10;
            tb.Columns = 70;
            tb.ID = "TB_Doc";


            BP.WF.Node nd = new BP.WF.Node(nodeID);
            if (nd.FocusField != "")
            {
                tb.Text = this.Request.QueryString["Info"];

                //Work wk = nd.HisWork;
                //wk.OID = this.WorkID;
                //wk.RetrieveFromDBSources();
                //tb.Text = wk.GetValStringByKey(nd.FocusField);
            }
            this.Top.Add(tb);
            //新增
            Label lb = new Label();
            lb.Text = "<div style='float:left;display:block;width:100%'><a href=javascript:TBHelp('Forward_Doc','" + "ND" + FK_Node + "'" + ")><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Emps.gif' align='middle' border=0 />选择词汇</a>&nbsp;&nbsp;</div>";
            this.Top.Add(lb);


        }
    }
}
