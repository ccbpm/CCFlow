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
using BP.Web.UC;

namespace CCFlow.Web.Comm.UI
{
	/// <summary>
	/// ErrPage 的摘要说明。
	/// </summary>
	public partial class ToErrPage:BP.Web.WebPage
	{
	 
		private string ErrorId
		{
			get
			{
				if (ViewState["ErrorId"]==null)
					return "info" ; 
				else					
			        return ViewState["ErrorId"].ToString();
			}
			set
			{
				ViewState["ErrorId"]=value;
			}
		}	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{ 
				try
				{
					this.ErrorId=this.Page.Request.QueryString["errorid"];
				}
				catch
				{
					this.ErrorId="info";
				}				
				this.UCSys1.Add(this.Msg); //.BindMyMsg(this.Msg);
			}
		}
		private string Msg
		{
			get
			{ 
				if (Session["info"]==null)
                    Session["info"] =   "@没有找到错误信息． "; //"@没有找到错误信息． ";
				return Session["info"].ToString();	 
			}
		}
		/// <summary>
		/// DealPage
		/// </summary>
		private void DealPage()
		{
//			string mess ; 
//			switch (this.ErrorId)
//			{
//				case "NoUserNoSession":
//					mess="你的能录时间太长";
//				case "ddd":
//				default :
//			}
//			this.LabMess.Text=mess;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//this.IsAuthenticate=false ;
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

		private void Btn1_Click(object sender, System.EventArgs e)
		{
			this.WinClose();
			 
		}

		private void Btn2_Click(object sender, System.EventArgs e)
		{
			this.Session["info"]=this.Msg;
			this.Response.Redirect("../FAQ/Ask.aspx",true);
		}

		private void Btn3_Click(object sender, System.EventArgs e)
		{
		
		}
	}
}
