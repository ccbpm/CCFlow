using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.DA;

namespace BP.Web.Comm
{
    /// <summary>
    /// UIEnNew 的摘要说明 
    /// </summary>
    public partial class UIEn : WebPage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string url = this.Request.RawUrl;
            url = url.Replace("/Comm/", "/Comm/RefFunc/");
            this.Response.Redirect(url,true);
            return;

           

        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }
        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
