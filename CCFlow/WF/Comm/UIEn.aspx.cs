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
    /// UIEnNew ��ժҪ˵�� 
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

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: �õ����� ASP.NET Web ���������������ġ�
            //
            InitializeComponent();
            base.OnInit(e);
        }
        /// <summary>
        /// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
        /// �˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
