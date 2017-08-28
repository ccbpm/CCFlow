using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.MapDef.Rpt
{
    public partial class S6_Power : BP.Web.PageBase
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];

            }
        }
        public string RptNo
        {
            get
            {
                return this.Request.QueryString["RptNo"];

            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];

            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }
    }
}