using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.Sys;
using BP.Sys.XML;
using BP.DA;
using BP.Web;
using BP.Web.Controls;

public partial class ReturnValTBFullCtrl : BP.Web.UC.UCBase3
{
    string FK_MapExt
    {
        get
        {
            return this.Request.QueryString["FK_MapExt"];
        }
    }
    string Val
    {
        get
        {
            string s = this.Request.QueryString["CtrlVal"];
            if ( s==null || s == "")
                s = null;
            return s;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        MapExt ext = new MapExt(this.FK_MapExt);
        string sql = ext.TagOfSQL_autoFullTB;
        if (this.Val != null)
            sql = sql.Replace("@Key", this.Val);

        sql = sql.Replace("$", "");
        DataTable dt = DBAccess.RunSQLReturnTable(sql);
        BindIt(dt);
    }
    public void BindIt(DataTable dt)
    {
        this.AddTable("width=80%");
        this.AddTR();
        this.AddTDBegin("class=Title colspan=" + dt.Columns.Count);
        this.Add("关键字");
        TextBox tb = new TextBox();
        tb.ID = "TB_Key";
        tb.Text = this.Val;
        this.Add(tb);

        Button btn = new Button();
        btn.CssClass = "Btn";
        btn.ID = "Btn_Search";
        btn.Text = "查找";
        btn.Click += new EventHandler(btn_Search_Click);
        this.Add(btn);
        this.AddTDEnd();
        this.AddTREnd();

        this.AddTR();
        this.AddTDTitle("选择");
        foreach (DataColumn dc in dt.Columns)
        {
            if (dc.ColumnName == "No" || dc.ColumnName == "Name")
                continue;
            this.AddTDTitle(dc.ColumnName);
        }
        this.AddTREnd();

        foreach (DataRow dr in dt.Rows)
        {
            this.AddTR();
            RadioButton rb = new RadioButton();
            rb.Text = dr["No"].ToString() + "," + dr["Name"].ToString();
            rb.ID = "RB_" + dr["No"];
            rb.GroupName = "sd";
            this.AddTD(rb);

            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName == "No" || dc.ColumnName == "Name")
                    continue;

                this.AddTD(dr[dc.ColumnName].ToString());
            }
            this.AddTREnd();
        }
        this.AddTableEndWithHR();
        btn = new Button();
        btn.ID = "s";
        btn.CssClass = "Btn";
        btn.Text = "确定";
        btn.Click += new EventHandler(btn_Click);
        this.Add(btn);
    }
    void btn_Search_Click(object sender, EventArgs e)
    {
        string key = this.GetTextBoxByID("TB_Key").Text;
        this.Response.Redirect("FrmReturnValTBFullCtrl.aspx?FK_MapExt=" + this.FK_MapExt + "&CtrlVal=" + key, true);
    }
    void btn_Click(object sender, EventArgs e)
    {
        MapExt ext = new MapExt(this.FK_MapExt);
        string sql = ext.TagOfSQL_autoFullTB;
        if (this.Val != null)
            sql = sql.Replace("@Key", this.Val);

        sql = sql.Replace("$", "");

        string val = "";
        DataTable dt = DBAccess.RunSQLReturnTable(sql);
        foreach (DataRow dr in dt.Rows)
        {
            RadioButton rb = this.GetRadioButtonByID("RB_" + dr["No"]);
            if (rb.Checked)
            {
                val = dr["No"].ToString();
                this.WinClose(val);
                return;

            }
        }
        this.WinClose(val);
    }
}