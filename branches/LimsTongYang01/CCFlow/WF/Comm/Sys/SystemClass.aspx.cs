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
	/// SystemClass 的摘要说明。
	/// </summary>
    public partial class SystemClass : BP.Web.WebPageAdmin
	{
		protected BP.Web.Controls.Btn Btn1;
		/// <summary>
		/// 控制页面的访问权限。
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
            this.UCSys1.AddTDBar("基本属性");
            this.UCSys1.Add("</TR>");

            this.UCSys1.Add("<TR>");
            this.UCSys1.AddTD("类名" + en.ToString() + "实体名称:" + en.EnMap.EnDesc + "物理表:" +
                en.EnMap.PhysicsTable + "类型:" + en.EnMap.EnType + "EnDBUrl:" + en.EnMap.EnDBUrl + " 编码结构" + en.EnMap.CodeLength + "编码长度:" + en.EnMap.CodeLength + "存放位置:" + en.EnMap.DepositaryOfEntity + " Map存放位置:" + en.EnMap.DepositaryOfMap);
            this.UCSys1.Add("</TR>");

            this.UCSys1.Add("<TR>");
            this.UCSys1.AddTDBar("映射信息");
            this.UCSys1.Add("</TR>");

            this.UCSys1.Add("<TR>");

            this.UCSys1.AddTable();
            this.UCSys1.Add("<TR>");

            this.UCSys1.AddTDTitle("ID");
            this.UCSys1.AddTDTitle("属性");
            this.UCSys1.AddTDTitle("字段");
            this.UCSys1.AddTDTitle("数据类型");
            this.UCSys1.AddTDTitle("数据类型");
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

		//	this.GenerLabel(this.Label1,"系统实体");
            if (this.EnName != null)
            {
                this.BindEn(this.EnName);
                return;
            }

          //  this.ToolBar1.Unload += new System.EventHandler(this.BPToolBar1_ButtonClick);
            if (this.IsPostBack == false)
            {
                //this.BPToolBar1.AddLab("sss","系统实体管理");
                //this.BPToolBar1.AddBtn(NamesOfBtn.Statistic,"注册");
                //this.BPToolBar1.AddLab("sss","选择一个实体");
                //this.BPToolBar1.AddBtn(NamesOfBtn.Edit);
                this.ToolBar1.AddLab("ss2", "基类");
                this.ToolBar1.AddTB("TB_EnsName");
                this.ToolBar1.AddBtn(NamesOfBtn.Search);
                this.ToolBar1.AddBtn(NamesOfBtn.Export);
                this.ToolBar1.AddBtn(NamesOfBtn.FileManager, "生成SQL");
                this.ToolBar1.AddBtn(NamesOfBtn.Card, "增加注释");
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
            this.UCSys1.AddTDTitle("类名");
            this.UCSys1.AddTDTitle("描述");
            this.UCSys1.AddTDTitle("物理表");
            this.UCSys1.AddTDTitle("类型");
            this.UCSys1.AddTDTitle("操作1");
            this.UCSys1.AddTDTitle("操作2");
            this.UCSys1.AddTDTitle("操作3");
           // this.UCSys1.AddTDTitle("操作4");
        //   this.UCSys1.AddTDTitle("操作5");
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
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('SystemClassDtl.aspx?EnsName=" + en.ToString() + "','dtl') ; \" >字段</a>");
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('SystemClassDtl.aspx?EnsName=" + en.ToString() + "&Type=Check','dtl') ; \" >体检</a>");

                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('../RefFunc/UIEn.aspx?EnName=" + en.ToString() + "&Type=Check','dtl') ; \" >界面</a>");
                //this.UCSys1.AddTD("<a href=\"javascript:WinOpen('EnsCfg.aspx?EnsName=" + en.ToString() + "&Type=Check','dtl') ; \" >设置</a>");
            //    this.UCSys1.AddTD("<a href=\"javascript:WinOpen('EnsAppCfg.aspx?EnsName=" + en.ToString() + "s&Type=Check','dtl') ; \" >属性设置</a>");
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
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
        /// 为每个字段增加注释
        /// </summary>
        public void AddNote()
        { 

        }
	}
}
