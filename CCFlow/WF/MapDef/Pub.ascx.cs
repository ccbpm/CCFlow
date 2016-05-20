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
using BP.En;
using BP.Sys;

public partial class WF_MapDef_Pub : BP.Web.UC.UCBase3
{
    #region 变量
    public string FK_MapData
    {
        get
        {
            return this.Request.QueryString["FK_MapData"];
        }
    }
    public string ExtType
    {
        get
        {
            string s = this.Request.QueryString["ExtType"];
            if (s == "")
                s = null;
            return s;
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public void ShowExtMenu()
    {
        this.Add("\t\n<div id='tabsJ'  align='center'>");
        MapExtXmls fss = new MapExtXmls();
        fss.RetrieveAll();

        this.AddUL();
        foreach (MapExtXml fs in fss)
        {
            if (this.ExtType == fs.No)
            {
               // this.Lab = fs.Name;
                this.AddLiB("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + fs.No, "<span>" + fs.Name + "</span>");
            }
            else
                this.AddLi("MapExt.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + fs.No, "<span>" + fs.Name + "</span>");
        }
        this.AddLi("<a href='MapExt.aspx?FK_MapData=" + this.FK_MapData + "'><span>帮助</span></a>");
        this.AddULEnd();
        this.AddDivEnd();  
    }
}
