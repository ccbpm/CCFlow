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
using BP;


namespace CCFlow.Web.Comm.Sys
{
	/// <summary>
	/// TestHost 的摘要说明。
	/// </summary>
    public partial class TestHost : BP.Web.WebPageAdmin
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在此处放置用户代码以初始化页面
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

		protected void Button1_Do_Click(object sender, System.EventArgs e)
		{
			DateTime dtS = DateTime.Now; 
			int time = int.Parse(this.TextBox_RunTime.Text) ; 


			if (this.RadioButton_Select.Checked)
				this.BindSelect();

			if (this.RadioButton_RunSQL.Checked)
				this.BindConn();

			if (this.RadioButton_OutHtml.Checked)
				this.BindConn();

			if (this.RadioButton_RunSQLRVal.Checked)
				this.BindRunSqlRVal();


			DateTime dtE = DateTime.Now;
			TimeSpan ts  =dtE  - dtS;
			this.UCSys1.AddMsgOfInfo("执行记录(毫秒)：","执行时间:"+ts.TotalMilliseconds+"毫秒"+ts.TotalMilliseconds/time+" 次/毫秒。");
			this.UCSys1.AddMsgOfInfo("执行记录(秒)：","执行时间:"+ts.TotalSeconds+"秒"+ts.TotalSeconds/time+" 次/毫秒。");
		}

		public void BindRunSqlRVal()
		{
			int time = int.Parse(this.TextBox_RunTime.Text) ; 

			string sql=this.TextBox_SQL.Text  ; 
			for(int i=0; i<=time ; i++)
			{
				object ss = BP.DA.DBAccess.RunSQLReturnVal(sql);
			}
		}

		public void BindSelect()
		{
			int time = int.Parse(this.TextBox_RunTime.Text) ; 

			string sql=this.TextBox_SQL.Text  ; 
			for(int i=0; i<=time ; i++)
			{
				int ss =BP.DA.DBAccess.RunSQL(sql);
			}
		}

		public void BindConn()
		{
			int time = int.Parse(this.TextBox_RunTime.Text) ; 
			DateTime dtS = DateTime.Now; 
			string sql=this.TextBox_SQL.Text  ; 
			for(int i=0; i<=time ; i++)
			{
				DataTable db = BP.DA.DBAccess.RunSQLReturnTable(sql);
			}
		
		}
	}
}
