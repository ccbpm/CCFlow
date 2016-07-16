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

public partial class WF_MapDef_UC_LeftBar : BP.Web.UC.UCBase3
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.AddFieldSet("表单设计");
        this.AddUL();
        this.AddLi("明细设计");
        this.AddLi("从表设计");
        this.AddLi("明细设计");
        this.AddLi("明细设计");
        this.AddULEnd();
        this.AddFieldSetEnd(); 
    }
}
