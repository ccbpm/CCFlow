using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;

public partial class WF_Admin_DBInstall : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Pub1.AddH3("GPM 数据库修复与安装工具");
        this.Pub1.AddHR();
        if (this.Request.QueryString["DoType"] == "OK")
        {
            this.Pub1.AddFieldSet("提示");
            this.Pub1.Add("GPM数据库初始化成功.");
            this.Pub1.AddBR("<a href='/GPM/Login.aspx?DoType=Logout' >登陆 admin 初试化密码是 pub .</a>");
            this.Pub1.AddFieldSetEnd();
            return;
        }

        if (BP.DA.DBAccess.IsExitsObject("Port_Emp") == true)
        {
            this.Pub1.AddFieldSet("提示");
            this.Pub1.Add("数据已经安装，如果您要重新安装，您需要手工的清除数据库里对象。");
            this.Pub1.AddFieldSetEnd();
            this.Pub1.AddBR();

            this.Pub1.AddFieldSet("修复数据表");
            this.Pub1.Add("把最新的版本的与当前的数据表结构，做一个自动修复, 修复内容：缺少列，缺少列注释，列注释不完整或者有变化。 <br> <a href='DBInstall.aspx?DoType=FixDB' >执行...</a>。");
            this.Pub1.AddFieldSetEnd();
            this.Pub1.AddBR();

            this.Pub1.AddFieldSet("登录GPM");
            this.Pub1.Add(" 如果您确认安装完毕，请登录GPM  <a href='/GPM/Login.aspx?DoType=Logout' >登录GPM</a>。");
            this.Pub1.AddFieldSetEnd();
            this.Pub1.AddBR();


            if (this.Request.QueryString["DoType"] == "FixDB")
            {
                string rpt = BP.Sys.PubClass.DBRpt(BP.DA.DBCheckLevel.High);
                this.Pub1.AddMsgGreen("同步数据表结构成功, 部分错误不会影响系统运行.",
                    "执行成功，希望在系统每次升级后执行此功能，不会对你的数据库数据产生影响。<br><br> <a href='/'>现在登陆GPM.</a>");
            }
            return;
        }

        RadioButton rb = new RadioButton();

        this.Pub1.AddFieldSet("选择数据库安装类型.");
        rb = new RadioButton();
        rb.Text = "SQLServer2000,2005,2008";
        rb.ID = "MSSQL";
        if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MSSQL)
            rb.Checked = true;

        rb.GroupName = "sd";
        rb.Checked = true;
        rb.Enabled = false;
        this.Pub1.Add(rb);
        this.Pub1.AddBR();

        rb = new RadioButton();
        rb.Text = "Oracle,Oracle 10g";
        rb.ID = "RB_Oracle";
        rb.GroupName = "sd";
        if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.Oracle)
            rb.Checked = true;
        rb.Enabled = false;
        this.Pub1.Add(rb);
        this.Pub1.AddBR();

        rb = new RadioButton();
        rb.Text = "DB2";
        rb.ID = "RB_DB2";
        rb.GroupName = "sd";
        if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.DB2)
            rb.Checked = true;
        rb.Enabled = false;
        this.Pub1.Add(rb);
        this.Pub1.AddBR();

        rb = new RadioButton();
        rb.Text = "MySQL";
        rb.ID = "RB_MYSQL";
        rb.GroupName = "sd";
        if (BP.Sys.SystemConfig.AppCenterDBType == BP.DA.DBType.MySQL)
            rb.Checked = true;
        rb.Enabled = false;
        this.Pub1.Add(rb);
        this.Pub1.AddBR();
        this.Pub1.AddFieldSetEnd();

        this.Pub1.AddFieldSet("是否需要安装CCIM.");
        rb = new RadioButton();
        rb.Text = "是";
        rb.ID = "RB_CCIM_Y";
        rb.Checked = true;
        rb.GroupName = "ccim";
        this.Pub1.Add(rb);
        this.Pub1.AddBR();
        rb = new RadioButton();
        rb.Text = "否";
        rb.ID = "RB_CCIM_N";
        rb.GroupName = "ccim";
        this.Pub1.Add(rb);
        this.Pub1.AddBR();
        this.Pub1.AddFieldSetEnd();

        this.Pub1.AddFieldSet("应用环境模拟.");
        rb = new RadioButton();
        rb.Text = "集团公司，企业单位。";
        rb.ID = "RB_Inc";
        rb.GroupName = "hj";
        rb.Checked = true;
        rb.Enabled = false;
        this.Pub1.Add(rb);
        this.Pub1.AddBR();

        rb = new RadioButton();
        rb.Text = "政府机关，事业单位。";
        rb.ID = "RB_Gov";
        rb.GroupName = "hj";
        rb.Enabled = false;
        this.Pub1.Add(rb);
        this.Pub1.AddBR();
        this.Pub1.AddFieldSetEndBR();
       
        Button btn = new Button();
        btn.ID = "Btn_s";
        btn.Text = "下一步";
        btn.UseSubmitBehavior = false;
        btn.OnClientClick = "this.disabled=true;";
        btn.Click += new EventHandler(btn_Click);
        this.Pub1.Add(btn);
    }
    void btn_Click(object sender, EventArgs e)
    {
        string lang = "CH";
        string hj = "Inc";

        // BP.Sys.FrmSubFlow frs = new BP.Sys.FrmSubFlow();
        //if (this.Pub1.GetRadioButtonByID("RB_Inc").Checked)
        //    hj = "Inc";
        //if (this.Pub1.GetRadioButtonByID("RB_Gov").Checked)
        //    hj = "Gov";

        hj = "Inc";
        //运行。
        BP.GPM.Glo.DoInstallDataBase(lang, hj);

        //安装CCIM
        if (this.Pub1.GetRadioButtonByID("RB_CCIM_Y").Checked)
        {
            BP.GPM.Glo.DoInstallCCIM(lang, hj);
        }

        //加注释.
        BP.Sys.PubClass.AddComment();

        this.Response.Redirect("DBInstall.aspx?DoType=OK", true);
    }
}