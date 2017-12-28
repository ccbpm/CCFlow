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
using BP.Sys;
using BP.DA;

namespace CCFlow.Web.Comm.Sys
{
	/// <summary>
	/// Cash ��ժҪ˵����
	/// </summary>
    public partial class Cash : BP.Web.WebPageAdmin
	{
		public new  string DoType
		{
			get
			{
				string str = this.Request.QueryString["DoType"];
				if (str==null)
					return "App";
				return str ;
			}
		}
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.UCSys1.AddMsgOfInfo("�ڴ����", "[<a href='Cash.aspx?DoType=App'>Application</a>][<a href='Cash.aspx?DoType=Sess'>Session</a>][<a href='Cash.aspx?DoType=Cash'>Cash</a>]");

            switch (this.DoType)
            {
                case "App":
                    this.BindApp();
                    break;
                case "Sess":
                    this.BindSess();
                    break;
                default:
                    this.BindCash();
                    break;
            }
        }
        public void BindApp()
        {
            this.UCSys1.AddTable();
            this.UCSys1.AddCaption(" Application ");
            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitle("IDX");
            this.UCSys1.AddTDTitle("���");
            this.UCSys1.AddTDTitle("ֵ");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTREnd();
            int i = 0;
            foreach (object obj in System.Web.HttpContext.Current.Application)
            {
                i++;
                this.UCSys1.AddTR();
                this.UCSys1.AddTDIdx(i);
                this.UCSys1.AddTD(obj.ToString());
                this.UCSys1.AddTD(System.Web.HttpContext.Current.Application[obj.ToString()].ToString());
                this.UCSys1.AddTD(obj.GetType().ToString() );
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
        }
        public void BindSess()
        {
            this.UCSys1.AddTable();
            this.UCSys1.AddCaption(" Session ");
            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitle("IDX");
            this.UCSys1.AddTDTitle("���");
            this.UCSys1.AddTDTitle("ֵ");
            this.UCSys1.AddTREnd();
            int i = 0;
            foreach (object obj in System.Web.HttpContext.Current.Session)
            {
                i++;
                this.UCSys1.AddTR();
                this.UCSys1.AddTDIdx(i);
                this.UCSys1.AddTD(obj.ToString());
                this.UCSys1.AddTD(System.Web.HttpContext.Current.Session[obj.ToString()].ToString());
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
        }
        public void BindCash()
        {
            this.UCSys1.AddTable();
            this.UCSys1.AddCaption(" Cash ");
            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitle("IDX");
            this.UCSys1.AddTDTitle("���");
            this.UCSys1.AddTDTitle("ֵ");
            this.UCSys1.AddTREnd();
            int i = 0;
            foreach (object obj in System.Web.HttpContext.Current.Cache)
            {
                i++;
                this.UCSys1.AddTR();
                this.UCSys1.AddTDIdx(i);
                this.UCSys1.AddTD(obj.ToString());

                this.UCSys1.AddTD(obj.GetType().Name);
                //this.UCSys1.AddTD(obj.GetType().Module.ToString());
                //this.UCSys1.AddTD(obj.GetType().IsSerializable);
                //this.UCSys1.AddTD(obj.GetType().Name);
                //this.UCSys1.AddTD(obj.GetType().Name);

                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
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
