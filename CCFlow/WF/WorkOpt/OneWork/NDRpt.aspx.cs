using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.WF;
using BP.Web;
using BP.En;
using BP.Sys;
namespace CCFlow.WF.OneWork
{
public partial class WF_WorkOpt_OneWork_NDRpt : System.Web.UI.Page
{
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    public string WorkID
    {
        get
        {
            return this.Request.QueryString["WorkID"];
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "Rpt";
        MapData md = new MapData(fk_mapdata);
        Entity en = md.HisEn;
        en.PKVal = this.WorkID;
        en.RetrieveFromDBSources();
        this.UCEn1.IsReadonly = true;
        this.UCEn1.BindColumn4(en, fk_mapdata);
    }
}
}