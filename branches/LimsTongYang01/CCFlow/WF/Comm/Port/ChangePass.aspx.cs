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
using BP.En;
using BP.Web;
using BP.Web.Pub ;
using BP.Port;

namespace CCFlow.Web.Comm.UI.WF
{
	/// <summary>
	/// ChangePass ��ժҪ˵����
	/// </summary>
	public partial class ChangePass12 : BP.Web.WebPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
            //  this.Btn_C.Click += new System.EventHandler(this.Btn_C_Click);

            this.Label1.Text = this.GenerCaption("��ʾ:Ϊϵͳ��ȫ�붨���޸�����");
            //  this.GenerLabel(this.Label1, );
            // �ڴ˴������û������Գ�ʼ��ҳ��
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//this.SubPageMessage="�޸�����";
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

        private void Btn_Save_Click(object sender, System.EventArgs e)
        {
            try
            {
                Emp ep = new Emp(WebUser.No);
                if (!ep.Pass.Equals(this.TB1.Text))
                {
                    this.Alert("����������!");
                    //  this.ResponseWriteRedMsg("����������");
                    return;
                }

                if (this.TB2.Text.Equals(this.TB3.Text))
                {
                    ep.Pass = this.TB2.Text;
                    ep.Update();
                    this.Alert("�޸ĳɹ�,���ס������!");
                    // this.ResponseWriteBlueMsg("");
                    //this.Response.Redirect("../wel.aspx",true);
                    return;
                }
                else
                {
                    this.Alert("������������벻һ�£�");
                    // this.ResponseWriteRedMsg();
                    return;
                }
            }
            catch (System.Exception ex)
            {
                this.ResponseWriteRedMsg("���� " + ex.Message);
            }
        }
	}
}
