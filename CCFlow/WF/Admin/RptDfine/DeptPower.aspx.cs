using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Port;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;

public partial class WF_MapDef_Rpt_DeptPower :WebPage
{
    #region attr
    public string FK_Flow
    {
        get
        {
            string s = this.Request.QueryString["FK_Flow"];
            if (s == null)
                s = "007";
            return s;
        }
    }
    public string Emps
    {
        get
        {
            string s = this.Request.QueryString["Emps"];
            return s;
        }
    }
    public string FK_MapData
    {
        get
        {
            string s = this.Request.QueryString["FK_MapData"];
            if (s == null)
                s = "ND7Rpt";
            return s;
        }
    }
    /// <summary>
    /// 步骤
    /// </summary>
    public new int Step
    {
        get
        {
            try
            {
                return int.Parse(this.Request.QueryString["Idx"]);
            }
            catch
            {
                return 1;
            }
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "报表查询权限控制";
        if (this.Emps == null)
        {
            this.BindDept();
        }
        else
        {
            this.BindEmps();
        }
    }
    public void BindDept()
    {
        this.Pub1.AddTable("width='90%'");
        this.Pub1.AddCaptionLeft("第一步:选择要设置的人员");
        Depts depts = new Depts();
        depts.RetrieveAll();
        Emps emps = new Emps();
        emps.RetrieveAll();
        string ctlIDs = "";
        foreach (Dept dept in depts)
        {
            CheckBox cbDept = new CheckBox();
            cbDept.Text = "<b>"+dept.Name+"</b>";
            cbDept.ID = "CB_S_all" + dept.No;

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle(cbDept);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin();

            string ctlDeptIDs = "";
            int idx = 0;
            foreach (Emp emp in emps)
            {
                if (emp.FK_Dept != dept.No)
                    continue;
                idx++;
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + emp.No;
                cb.Text = emp.Name;
                this.Pub1.Add(cb);
                ctlIDs += cb.ID + ",";
                ctlDeptIDs += cb.ID + ",";
                if (idx == 5)
                {
                    this.Pub1.AddHR();
                    idx = 0;
                }
            }

            cbDept.Attributes["onclick"] = "SetSelected(this,'" + ctlDeptIDs + "')";
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
        }
        this.Pub1.AddTableEnd();

        CheckBox cball = new CheckBox();
        cball.Text = "CB_All_S";
        cball.Text = "<b>选择全部</b>&nbsp;&nbsp;&nbsp;&nbsp;";
        cball.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";
        this.Pub1.Add(cball);

        Button btn = new Button();
        btn.Text = "选择要设置的权限人员后:下一步";
        btn.ID = "Btn_Next";
        btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_Next_Click);
        this.Pub1.Add(btn);
    }
    void btn_Next_Click(object sender, EventArgs e)
    {
        Emps emps = new Emps();
        emps.RetrieveAll();
        string strs = "";
        foreach (Emp emp in emps)
        {
            CheckBox cb = this.Pub1.GetCBByID("CB_" + emp.No);
            if (cb == null)
                continue;
            if (cb.Checked == false)
                continue;
            strs += emp.No + ",";
            
        }

        if (strs == "")
        {
            this.Alert("请选择要设置权限的人员，可以一次性设置多个.");
            return;
        }
        this.Response.Redirect("DeptPower.aspx?Emps="+strs+"&FK_Flow="+this.FK_Flow, true);
    }
    public void BindEmps()
    {
        this.Pub1.AddTable("width='100%'");
        this.Pub1.AddCaptionLeft("<a href='DeptPower.aspx?FK_Flow=" + this.FK_Flow+"&IsSave=1' >返回</a> =》 第二步:部门查询权限设置");
        Depts depts = new Depts();
        depts.RetrieveAll();
        string ctlIDs = "";
        this.Pub1.AddTR();
        this.Pub1.AddTDBegin();
        foreach (Dept dept in depts)
        {
            CheckBox cb = new CheckBox();
            cb.ID = "CB_" + dept.No;
            cb.Text = dept.No+" - "+dept.Name;
            this.Pub1.AddBR();
            this.Pub1.Add(DataType.GenerSpace(dept.No.Length));
            this.Pub1.Add(cb);
            ctlIDs += cb.ID + ",";
        }
        this.Pub1.AddTDEnd();
        this.Pub1.AddTREnd();
        this.Pub1.AddTableEnd();

        CheckBox cball = new CheckBox();
        cball.Text = "CB_All_S";
        cball.Text = "<b>选择全部</b>&nbsp;&nbsp;&nbsp;&nbsp;";
        cball.Attributes["onclick"] = "SetSelected(this,'" + ctlIDs + "')";
        this.Pub1.Add(cball);

        Button btn = new Button();
        btn.Text = "保存";
        btn.ID = "Btn_Save";
        btn.CssClass = "Btn";
        this.Pub1.Add(btn);
        btn.Click += new EventHandler(btn_Save_Click);

        btn = new Button();
        btn.Text = "保存并关闭";
        btn.ID = "Btn_SaveAndClose";
        btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_Save_Click);
        this.Pub1.Add(btn);
    }
    void btn_Save_Click(object sender, EventArgs e)
    {
        BP.WF.Port.DeptFlowSearch dfs = new BP.WF.Port.DeptFlowSearch();
        dfs.CheckPhysicsTable();

        Depts ens = new Depts();
        ens.RetrieveAll();

        //删除已经有的数据。
        string[] empStrs = this.Emps.Split(',');
        foreach (string s in empStrs)
        {
            if (DataType.IsNullOrEmpty(s))
                continue;
            DBAccess.RunSQL("DELETE FROM WF_DeptFlowSearch WHERE FK_Flow='" + this.FK_Flow + "' AND FK_Emp='" + s + "'");
        }

        dfs.FK_Flow = this.FK_Flow;
        foreach (Dept en in ens)
        {
            CheckBox cb = this.Pub1.GetCBByID("CB_" + en.No);
            if (cb == null)
                continue;
            if (cb.Checked == false)
                continue;

            dfs.FK_Dept = en.No;
            foreach (string s in empStrs)
            {
                if (DataType.IsNullOrEmpty(s))
                    continue;
                dfs.MyPK = en.No + "_" + s + "_" + this.FK_Flow;
                dfs.FK_Emp = s;
                dfs.Insert();
            }
        }
        Button btn = sender as Button;
        if (btn.ID == "Btn_SaveAndClose")
            this.WinCloseWithMsg("保存成功");
        else
            this.Alert("保存成功!!");
    }
}