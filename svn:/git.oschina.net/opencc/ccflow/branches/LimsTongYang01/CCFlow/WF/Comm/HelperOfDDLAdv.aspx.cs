using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.Sys.XML;


public partial class CCFlow_Comm_SearchAdv : BP.Web.WebPage
{
    public new string EnsName
    {
        get
        {
            return this.Request.QueryString["EnsName"];
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Bind();
    }

    /// <summary>
    /// EnsName
    /// </summary>
    public void Bind()
    {
        BP.Sys.UserRegedit urd = new UserRegedit(WebUser.No, this.EnsName + "_SearchAttrs");
        Entities ens = ClassFactory.GetEns(this.EnsName);
        Entity en = ens.GetNewEntity;

        this.Ucsys1.AddTable();
        this.Ucsys1.AddCaptionLeft("查询条件高级设置");

        //  AttrsOfSearch attrs = en.EnMap.AttrsOfSearch;
        //  Attrs attrs = en.EnMap.AttrsOfSearch;
        AttrSearchs attrs = en.EnMap.SearchAttrs;
        foreach (AttrSearch attr in attrs)
        {
            if (attr.Key.Contains("Text"))
                continue;

            if (attr.HisAttr.UIBindKey == null)
                continue;

            //if (attr.IsHidden)
            //    continue;

            //if (attr.DefaultVal.Length >= 8)
            //    continue;

            //if (attr.SymbolEnable == true)
            //    continue;

            this.Ucsys1.AddTR();
            // this.Ucsys1.AddTD(attr.Desc);
            /* 可以使用的情况. */
            //    Attr myattr = en.EnMap.GetAttrByKey(attr.Key);

            DDL ddl1 = new DDL(attr.HisAttr, urd.CfgKey, "enumLab", true, this.Page.Request.ApplicationPath);

            ddl1.ID = "DDL_" + attr.Key;

            this.Ucsys1.AddContral(attr.HisAttr.Desc, ddl1, true);
            this.Ucsys1.AddTREnd();
        }


        this.Ucsys1.AddTR();
        Btn btn = new Btn();
        btn.Text = "  确  定  ";
        btn.ID = "Btn_OK";

        btn.Click += new EventHandler(btn_Click_OK);
        this.Ucsys1.AddTD(btn);

        btn = new Btn();
        btn.Text = "  取  消  ";
        btn.ID = "Btn_Cancel";
        btn.Click += new EventHandler(btn_Click);
        this.Ucsys1.AddTD(btn);
        this.Ucsys1.AddTREnd();
        this.Ucsys1.AddTableEnd();
    }

    void btn_Click_OK(object sender, EventArgs e)
    {

        string clientscript = "<script language='javascript'> window.returnValue = 'ok'; window.close(); </script>";
        this.Page.Response.Write(clientscript);
    }
    void btn_Click(object sender, EventArgs e)
    {
        this.WinClose();
    }
}
