using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SDKFlows_PopSelectVal : BP.Web.WebPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "Pop返回值测试";
        this.Pub1.AddFieldSet("选择一个合同编号:");
        this.Pub1.AddUL();
        for (int i = 0; i < 9; i++)
        {
            RadioButton rb = new RadioButton();
            rb.Text = "Name 000" + i;
            rb.ID = "RB_"+i;
            rb.GroupName = "sd";
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
        }
        this.Pub1.AddULEnd();

        Button btn = new Button();
        btn.ID = "s";
        btn.Text = "OK";
        btn.Click += new EventHandler(btn_Click);
        this.Pub1.Add(btn);

        this.Pub1.AddFieldSetEnd();
 
    }

    void btn_Click(object sender, EventArgs e)
    {
        string val = "请选择一个项目.";
        for (int i = 0; i < 9; i++)
        {

            RadioButton rb = this.Pub1.GetRadioButtonByID("RB_" + i);
            if (rb.Checked)
                val = "000" + i.ToString();
        }

        string clientscript = "<script language='javascript'> window.returnValue = '" + val + "'; window.close(); </script>";
        this.Page.Response.Write(clientscript);

      //  this.WinClose(val);
    }
}