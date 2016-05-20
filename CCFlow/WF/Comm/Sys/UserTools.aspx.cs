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
	/// UserTools 的摘要说明。
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
			dt.Columns.Add( new DataColumn("名称", typeof(string)));
			dt.Columns.Add( new DataColumn("描述", typeof(string)));

			foreach(DataRow mydr in mydt.Rows)
			{
				if ( mydr["Enable"].ToString().Trim() =="0")
					continue;

				DataRow dr =dt.NewRow();
				dr["ICON"] = "<Img src='"+mydr["ICON"]+"' />";
				dr["名称"] = "<a href='"+mydr["URL"]+"' >"+mydr["Name"]+"</A>";
				dr["描述"] = mydr["DESC"] ;
				dt.Rows.Add(dr);
			}
		 
			this.Response.Write(this.GenerTablePage( dt ,"用户工具")) ; 
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
