using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web.Controls;
using BP;

namespace CCFlow.Web.Comm.UI
{
	/// <summary>
	/// SystemClass ��ժҪ˵����
	/// </summary>
    public partial class SystemClass : BP.Web.WebPageAdmin
	{
		protected BP.Web.Controls.Btn Btn1;
		/// <summary>
		/// ����ҳ��ķ���Ȩ�ޡ�
		/// </summary>
		/// <returns></returns>
		protected override string WhoCanUseIt()
		{ 
			return  ",admin,8888,";
		}
		public new string EnName
		{
			get
			{
				return this.Request.QueryString["EnName"];
			}
		}
        public TextBox TB_EnsName
        {
            get
            {
                return this.ToolBar1.GetTextBoxByID("TB_EnsName");
            }
        }
        public void BindEn(string enName)
        {
            Entity en =BP.En.ClassFactory.GetEn(enName);
            Map map = en.EnMap;

            this.UCSys1.AddTable();
            this.UCSys1.Add("<TR>");
            this.UCSys1.AddTDBar("��������");
            this.UCSys1.Add("</TR>");

            this.UCSys1.Add("<TR>");
            this.UCSys1.AddTD("����" + en.ToString() + "ʵ������:" + en.EnMap.EnDesc + "�����:" +
                en.EnMap.PhysicsTable + "����:" + en.EnMap.EnType + "EnDBUrl:" + en.EnMap.EnDBUrl + " ����ṹ" + en.EnMap.CodeLength + "���볤��:" + en.EnMap.CodeLength + "���λ��:" + en.EnMap.DepositaryOfEntity + " Map���λ��:" + en.EnMap.DepositaryOfMap);
            this.UCSys1.Add("</TR>");

            this.UCSys1.Add("<TR>");
            this.UCSys1.AddTDBar("ӳ����Ϣ");
            this.UCSys1.Add("</TR>");

            this.UCSys1.Add("<TR>");

            this.UCSys1.AddTable();
            this.UCSys1.Add("<TR>");

            this.UCSys1.AddTDTitle("ID");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTDTitle("�ֶ�");
            this.UCSys1.AddTDTitle("��������");
            this.UCSys1.AddTDTitle("��������");
            this.UCSys1.Add("<TR>");

            this.UCSys1.Add("</TR>");
           
            this.UCSys1.Add("</Table>");
            this.UCSys1.Add("</TR>");
            this.UCSys1.Add("</Table>");
        }
        public void BindEns()
        { 

        }
		protected void Page_Load(object sender, System.EventArgs e)
		{

		//	this.GenerLabel(this.Label1,"ϵͳʵ��");
            if (this.EnName != null)
            {
                this.BindEn(this.EnName);
                return;
            }

          //  this.ToolBar1.Unload += new System.EventHandler(this.BPToolBar1_ButtonClick);
            if (this.IsPostBack == false)
            {
                //this.BPToolBar1.AddLab("sss","ϵͳʵ�����");
                //this.BPToolBar1.AddBtn(NamesOfBtn.Statistic,"ע��");
                //this.BPToolBar1.AddLab("sss","ѡ��һ��ʵ��");
                //this.BPToolBar1.AddBtn(NamesOfBtn.Edit);
                this.ToolBar1.AddLab("ss2", "����");
                this.ToolBar1.AddTB("TB_EnsName");
                this.ToolBar1.AddBtn(NamesOfBtn.Search);
                this.ToolBar1.AddBtn(NamesOfBtn.Export);
                this.ToolBar1.AddBtn(NamesOfBtn.FileManager, "����SQL");
                this.ToolBar1.AddBtn(NamesOfBtn.Card, "����ע��");
                this.TB_EnsName.Text = "BP.En.Entity";
            }

			this.Bind();
		}
        public void Bind()
        {
            this.UCSys1.Controls.Clear();
            ArrayList al = null;
            try
            {
                string info = this.TB_EnsName.Text;
                if (info.Length == 0)
                    info = "BP.En.Entity";

                this.TB_EnsName.Text = info;
                al = BP.En.ClassFactory.GetObjects(info);
            }
            catch (Exception ex)
            {
                this.ResponseWriteBlueMsg(ex.Message);
                return;
            }

            this.UCSys1.AddTable();
            this.UCSys1.Add("<TR>");
            this.UCSys1.AddTDTitle("ID");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTDTitle("�����");
            this.UCSys1.AddTDTitle("����");
            this.UCSys1.AddTDTitle("����1");
            this.UCSys1.AddTDTitle("����2");
            this.UCSys1.AddTDTitle("����3");
           // this.UCSys1.AddTDTitle("����4");
        //   this.UCSys1.AddTDTitle("����5");
            this.UCSys1.Add("</TR>");

            int i = 0;
            foreach (Object obj in al)
            {
                i++;
                Entity en = null;
                try
                {
                    en = obj as Entity;
                    string s = en.EnDesc;
                    if (en == null)
                        continue;
                    this.UCSys1.AddTREnd();
                }
                catch
                {
                    continue;
                }

                this.UCSys1.Add("<TR window.location.href='SystemClass.aspx?EnName=" + en.ToString() + "' onmouseover='TROver(this)' onmouseout='TROut(this)' >");
                this.UCSys1.AddTDIdx(i);
                this.UCSys1.AddTD(en.ToString());
                this.UCSys1.AddTD(en.EnDesc);
                this.UCSys1.AddTD(en.EnMap.PhysicsTable);
                this.UCSys1.AddTD(en.EnMap.EnType.ToString());
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('SystemClassDtl.aspx?EnsName=" + en.ToString() + "','dtl') ; \" >�ֶ�</a>");
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('SystemClassDtl.aspx?EnsName=" + en.ToString() + "&Type=Check','dtl') ; \" >���</a>");

                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('../RefFunc/UIEn.aspx?EnName=" + en.ToString() + "&Type=Check','dtl') ; \" >����</a>");
                //this.UCSys1.AddTD("<a href=\"javascript:WinOpen('EnsCfg.aspx?EnsName=" + en.ToString() + "&Type=Check','dtl') ; \" >����</a>");
            //    this.UCSys1.AddTD("<a href=\"javascript:WinOpen('EnsAppCfg.aspx?EnsName=" + en.ToString() + "s&Type=Check','dtl') ; \" >��������</a>");
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

		private void BPToolBar1_ButtonClick(object sender, System.EventArgs e)
		{
			try
			{
                Button btn = (Button)sender;
				switch(btn.ID)
				{
					case NamesOfBtn.Search:
						this.Bind();
						return;
					case NamesOfBtn.Export:
						//this.ExportEntityToExcel(this.TB_EnsName.Text);
						return;
					case NamesOfBtn.FileManager:
					//	this.ResponseWriteBlueMsg(this.GenerCreateTableSQL(this.TB_EnsName.Text));
						return;
                    case NamesOfBtn.Card:
                        return;
					default:
						throw new Exception("error");
				}
			}
			catch(Exception ex)
			{
				this.ResponseWriteRedMsg(ex);
			}
		}
        /// <summary>
        /// Ϊÿ���ֶ�����ע��
        /// </summary>
        public void AddNote()
        { 

        }
	}
}
