using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.DA;

namespace CCOA.WF.Comm
{
    public partial class Rpt2DBAs : System.Web.UI.Page
    {
        #region 属性.
        public string Rpt2Name
        {
            get
            {
                return this.Request.QueryString["Rpt2Name"];
            }
        }
        /// <summary>
        /// 实体.
        /// </summary>
        public Rpt2Base Rpt2Entity
        {
            get
            {
                return BP.En.ClassFactory.GetRpt2Base(this.Request.QueryString["Rpt2Name"]);
            }
        }
        /// <summary>
        /// 选择的属性(可能默认为空)
        /// </summary>
        public string AttrOfSelected
        {
            get
            {
                return this.Request.QueryString["AttrOfSelect"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}