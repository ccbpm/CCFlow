using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.CCFormDesigner
{
    public partial class FrmAttr : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //根据不同表单类型，转向不同的表单属性设置界面.
            string frmID = this.Request.QueryString["FrmID"];
            BP.Sys.MapData md = new BP.Sys.MapData(frmID);

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm || md.HisFrmType == BP.Sys.FrmType.Column4Frm)
            {
                this.Response.Redirect("../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataExts&PK=" + frmID, true);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.Url)
            {
                this.Response.Redirect("../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataURLs&PK=" + frmID, true);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.WordFrm)
            {
                this.Response.Redirect("../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataWords&PK=" + frmID, true);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
            {
                this.Response.Redirect("../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataExcels&PK=" + frmID, true);
                return;
            }
        }
    }
}