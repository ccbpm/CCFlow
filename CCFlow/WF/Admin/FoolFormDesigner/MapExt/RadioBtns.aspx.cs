using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.MapDef
{
    public partial class RadioBtns : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.Request.QueryString["KeyOfEn"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {

        }
    }
}