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
	/// Test ��ժҪ˵����
	/// </summary>
    public partial class TestCash : BP.Web.WebPageAdmin
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
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
