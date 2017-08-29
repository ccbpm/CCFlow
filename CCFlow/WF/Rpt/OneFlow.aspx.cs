using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;

namespace CCFlow.WF.Rpt
{
    public partial class OneFlow : System.Web.UI.Page
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (s == null)
                    s = "007";
                return s;
            }
        }
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                    s = "ND" + int.Parse(this.FK_Flow) + "Rpt";
                s = s.Replace("ND00", "ND");
                s = s.Replace("ND0", "ND");
                return s;
            }
        }
        /// <summary>
        /// 报表编号
        /// </summary>
        public string RptNo
        {
            get
            {
                string s = this.Request.QueryString["RptNo"];
                if (s == null)
                    s = "ND" + int.Parse(this.FK_Flow) + "MyRpt";

                s = s.Replace("ND00", "ND");
                s = s.Replace("ND0", "ND");
                return s;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //Title = "报表设计：" + fl.Name;
            var fl = new Flow(FK_Flow);
            BP.WF.Rpt.MapRpt rpt = new BP.WF.Rpt.MapRpt(RptNo);

            Title = "报表设计：" + fl.Name + "(" + rpt.Name + ")";
        }
    }
}