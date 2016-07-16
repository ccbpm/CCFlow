using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.MapDef
{
    public partial class BodyAttr :BP.Web.WebPage
    {
        #region 属性.
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_Mapdata"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == false)
            {
                BP.Sys.MapData md = new BP.Sys.MapData(this.FK_MapData);
                this.TB_Attr.Text = md.BodyAttr;
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            BP.Sys.MapData md = new BP.Sys.MapData(this.FK_MapData);
            md.BodyAttr = this.TB_Attr.Text;
            md.Update();
            this.WinClose();
        }
    }
}