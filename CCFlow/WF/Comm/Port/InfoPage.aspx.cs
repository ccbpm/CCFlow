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
using BP.Web;

namespace CCFlow.Web.Comm
{
	/// <summary>
	/// ErrPage ��ժҪ˵����
	/// </summary>
	public partial class Info : BP.Web.WebPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSA OUR");

            this.UCSys1.AddFieldSet("��ʾ");
            this.UCSys1.AddMsgOfInfo("<img src='../CSS/images/ccflowInfo.jpg' />", this.Msg);
            this.UCSys1.AddFieldSetEnd(); 

            this.Session["Info"] = null;
            this.Session["info"] = null;
        }
        private string Msg
        {
            get
            {
                string msg = this.Session["info"] as string;
                if (msg == null)
                    msg = this.Application["info" + WebUser.No] as string;
                if (msg == null)
                {
                    msg = "@��ʾ��Ϣ��ʧ��"; // "@û���ҵ���Ϣ��������;�������ҵ�����";
                }
                return msg;
            }
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
	}
}
