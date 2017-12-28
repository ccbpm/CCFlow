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
	/// ErrPage ��ժҪ˵����
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
                    Session["info"] =   "@û���ҵ�������Ϣ�� "; //"@û���ҵ�������Ϣ�� ";
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
//					mess="�����¼ʱ��̫��";
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
			// CODEGEN���õ����� ASP.NET Web ���������������ġ�
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
