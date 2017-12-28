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
	/// ManagerTools ��ժҪ˵����
	/// </summary>
	public partial class ManagerTools : BP.Web.WebPageAdmin
	{
		protected System.Web.UI.WebControls.Label Label1;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //if (Web.WebUser.No.IndexOf("8888") > 1 || Web.WebUser.No.IndexOf("admin") != -1 || Web.WebUser.No.IndexOf("8888") == 0)
            //{
            //    //this.ToErrorPage("��û��Ȩ��ʹ�ô˹��ܣ�");
            //}
            //else
            //{
            //    if (Web.WebUser.No == "288888")
            //    {
            //        //this.ToErrorPage("��û��Ȩ��ʹ�ô˹��ܣ�");
            //    }
            //    else
            //    {
            //        this.ToErrorPage("��û��Ȩ��ʹ�ô˹��ܣ�");
            //    }
            //}


            DataSet ds = new DataSet();
            ds.ReadXml(BP.Sys.SystemConfig.PathOfXML + "AdminTools.xml");

            DataTable mydt = ds.Tables[0];

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ICON", typeof(string)));
            dt.Columns.Add(new DataColumn("����", typeof(string)));
            dt.Columns.Add(new DataColumn("�ṩ��", typeof(string)));
            dt.Columns.Add(new DataColumn("����", typeof(string)));

            //dt.Columns.Add( new DataColumn("����", typeof(string)));

            DataRow dr1 = dt.NewRow();
            dr1["ICON"] = "<Img src='./../WF/Img/Btn/Do.gif' border=0/>";
            dr1["����"] = "<a href='EditWebconfig.aspx' >ȫ������</A>";
            dr1["�ṩ��"] = "admin";
            dr1["����"] = "��������ȫ�ֵ�վ����Ϣ";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["ICON"] = "<Img src='./../Img/Btn/Do.gif' border=0/>";
            dr1["����"] = "<a href='./../Port/ChangePass.aspx' >�޸�����</A>";
            dr1["�ṩ��"] = "admin";
            dr1["����"] = "��������ȫ�ֵ�վ����Ϣ";
            dt.Rows.Add(dr1);


            foreach (DataRow mydr in mydt.Rows)
            {
                if (mydr["Enable"].ToString().Trim() == "0")
                    continue;

                DataRow dr = dt.NewRow();
                dr["ICON"] = "<Img src='" + mydr["ICON"] + "' />";
                dr["����"] = "<a href='" + mydr["URL"] + "' >" + mydr["Name"] + "</A>";
                dr["�ṩ��"] = mydr["DFor"];
                dr["����"] = mydr["DESC"];
                dt.Rows.Add(dr);
            }

            this.UCSys1.AddTable();
            this.UCSys1.AddCaptionLeft(this.GenerCaption("����Ա����"));
            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitle("");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTDTitle("�ṩ��");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTREnd();

            foreach (DataRow dr in dt.Rows)
            {
                this.UCSys1.AddTR();
                this.UCSys1.AddTD(dr["ICON"].ToString());
                this.UCSys1.AddTD(dr["����"].ToString());
                this.UCSys1.AddTD(dr["�ṩ��"].ToString());
                this.UCSys1.AddTD(dr["����"].ToString());
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
            //    this.Ucsys1.Add(this.GenerCaption("����ִ��"));
            // this.Ucsys1.AddHR();
            // this.Response.Write(this.GenerTablePage(dt, "����Ա����"));
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
