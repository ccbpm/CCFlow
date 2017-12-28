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
	/// TestHost ��ժҪ˵����
	/// </summary>
    public partial class TestHost : BP.Web.WebPageAdmin
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// �ڴ˴������û������Գ�ʼ��ҳ��
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
			this.UCSys1.AddMsgOfInfo("ִ�м�¼(����)��","ִ��ʱ��:"+ts.TotalMilliseconds+"����"+ts.TotalMilliseconds/time+" ��/���롣");
			this.UCSys1.AddMsgOfInfo("ִ�м�¼(��)��","ִ��ʱ��:"+ts.TotalSeconds+"��"+ts.TotalSeconds/time+" ��/���롣");
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
