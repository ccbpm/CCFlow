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
using BP.Web;

namespace BP.Web.Portal
{
	/// <summary>
	/// Exit 的摘要说明。
	/// </summary>
	public partial class Exit :  WebPage
	{

		private string url
		{
			get
			{
				try
				{
					return (string)ViewState["url"];
				}
				catch
				{
				   return "Wel.aspx";
				}
			}
			set
			{
				ViewState["url"]=value;
			}
		}
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			this.Session["url"]=this.Request.RawUrl;
			try
			{
				WebUser.Exit();
				//this.WinClose();
                this.Response.Redirect("/GPM/Login.aspx?DoType=Logout");
            }
			catch(System.Exception ex)
			{
				this.ResponseWriteRedMsg(ex) ; 
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
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

		private void LinkButton1_Click(object sender, System.EventArgs e)
		{
			System.Web.HttpContext.Current.Response.Redirect(System.Web.HttpContext.Current.Request.ApplicationPath+"/SignIn.aspx?url="+this.url); //this.ToSignInPage();
		}

		private void Btn_O_Click(object sender, System.EventArgs e)
		{			

			try
			{
				WebUser.Exit();
				this.ResponseWriteBlueMsg("现在，您已经安全的退出了系统，谢谢使用，再见!");
			}
			catch(System.Exception ex)
			{
				this.ResponseWriteRedMsg(ex) ; 
			}
		}

		private void Btn1_Click(object sender, System.EventArgs e)
		{
			this.Response.Redirect("SignIn.aspx",true);
		}
	}
}
