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
using BP.Web.Pub ;
using BP.Port;

namespace CCFlow.Web.Comm.UI.WF
{
	/// <summary>
	/// ChangePass 的摘要说明。
	/// </summary>
	public partial class ChangePass12 : BP.Web.WebPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            //  this.Btn_C.Click += new System.EventHandler(this.Btn_C_Click);

            this.Label1.Text = this.GenerCaption("提示:为系统安全请定期修改密码");
            //  this.GenerLabel(this.Label1, );
            // 在此处放置用户代码以初始化页面
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//this.SubPageMessage="修改密码";
			//
			// CODEGEN：该调用是 ASP.NET Web 窗体设计器所必需的。
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

        private void Btn_Save_Click(object sender, System.EventArgs e)
        {
            try
            {
                Emp ep = new Emp(WebUser.No);
                if (!ep.Pass.Equals(this.TB1.Text))
                {
                    this.Alert("旧密码有误!");
                    //  this.ResponseWriteRedMsg("旧密码有误！");
                    return;
                }

                if (this.TB2.Text.Equals(this.TB3.Text))
                {
                    ep.Pass = this.TB2.Text;
                    ep.Update();
                    this.Alert("修改成功,请记住新密码!");
                    // this.ResponseWriteBlueMsg("");
                    //this.Response.Redirect("../wel.aspx",true);
                    return;
                }
                else
                {
                    this.Alert("两次输入的密码不一致！");
                    // this.ResponseWriteRedMsg();
                    return;
                }
            }
            catch (System.Exception ex)
            {
                this.ResponseWriteRedMsg("错误： " + ex.Message);
            }
        }
	}
}
