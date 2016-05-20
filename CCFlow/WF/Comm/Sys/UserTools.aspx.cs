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

namespace CCFlow.Web.WF.Comm.Sys
{
	/// <summary>
	/// UserTools ��ժҪ˵����
	/// </summary>
    public partial class UserTools : BP.Web.WebPageAdmin
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			DataSet ds = new DataSet();
			ds.ReadXml(BP.Sys.SystemConfig.PathOfXML+"UserTools.xml");
			DataTable mydt=ds.Tables[0];
			DataTable dt = new DataTable();
			dt.Columns.Add( new DataColumn("ICON", typeof(string)));
			dt.Columns.Add( new DataColumn("����", typeof(string)));
			dt.Columns.Add( new DataColumn("����", typeof(string)));

			foreach(DataRow mydr in mydt.Rows)
			{
				if ( mydr["Enable"].ToString().Trim() =="0")
					continue;

				DataRow dr =dt.NewRow();
				dr["ICON"] = "<Img src='"+mydr["ICON"]+"' />";
				dr["����"] = "<a href='"+mydr["URL"]+"' >"+mydr["Name"]+"</A>";
				dr["����"] = mydr["DESC"] ;
				dt.Rows.Add(dr);
			}
		 
			this.Response.Write(this.GenerTablePage( dt ,"�û�����")) ; 
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
