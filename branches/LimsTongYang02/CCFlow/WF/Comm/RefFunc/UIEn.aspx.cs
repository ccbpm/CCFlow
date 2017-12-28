using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;

namespace CCFlow.WF.Comm.RefFunc
{
    public partial class UIEn : System.Web.UI.Page
    {
        /// <summary>
        /// 是否隐藏左侧功能栏
        /// </summary>
        public bool HiddenLeft
        {
            get { return this.RefLeft1.ItemCount == 0; }
        }
        /// <summary>
        /// 是否隐藏上部菜单栏
        /// </summary>
        public bool HiddenTop
        {
            get
            {
                var en = this.UIEn1.GetEnDa;
                return en == null ? true : en.EnMap.EnType == EnType.View;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string enName = this.Request.QueryString["EnName"];
            if (enName == null || enName == "")
                enName = this.Request.QueryString["EnsName"];

            if (enName.Contains(".") == false)
            {
                this.Response.Redirect("SysMapEn.aspx?EnsName=" + enName + "&PK=" + this.Request["PK"], true);
                return;
            }
        }
    }
}