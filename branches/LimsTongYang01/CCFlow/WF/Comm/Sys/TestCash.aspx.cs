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

namespace CCFlow.Web.Comm.Sys
{
	/// <summary>
	/// Test 的摘要说明。
	/// </summary>
    public partial class TestCash : BP.Web.WebPageAdmin
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
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

		protected void Button1_Click(object sender, System.EventArgs e)
		{
            //BP.Port.HYs hys = new BP.Port.HYs();
            //hys.RetrieveAll();
 
            //foreach(BP.Port.HY hy in hys)
            //{
            //    this.UCSys1.AddP(hy.No);
            //    this.UCSys1.AddP(hy.Name);
            //}
		}

		protected void Button2_Click(object sender, System.EventArgs e)
		{
			string dt =DateTime.Now.ToString(BP.DA.DataType.SysDataTimeFormat);

			DateTime mydt= BP.DA.DataType.ParseSysDate2DateTime(dt);

		}
	}
}
