using System;
using System.Web.UI.WebControls;
using System.Drawing;
using Microsoft.Web.UI.WebControls;

namespace BP.Web.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class BPPageView :Microsoft.Web.UI.WebControls.PageView
	{
		public BPPageView()
		{
			this.PreRender += new System.EventHandler(this.BPPageViewPreRender);
			this.BorderColor=Color.Black;
			this.BorderStyle=BorderStyle.Inset;
		}
		public BPPageView(string id)
		{
			this.ID = id;
			this.PreRender += new System.EventHandler(this.BPPageViewPreRender);

			this.BorderColor=Color.Black;
			this.BorderStyle=BorderStyle.Inset;
			
		}
        private void BPPageViewPreRender(object sender, System.EventArgs e)
        {
            this.CssClass = "BPPageView" + WebUser.Style;
            this.BorderColor = Color.FromName("ControlDark");
            this.BorderStyle = BorderStyle.Inset;
        }

	}
	 
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	public class BPMultiPage:Microsoft.Web.UI.WebControls.MultiPage
	{
		public void AddBPPageView(string id)
		{
			BPPageView pv = new BPPageView(id) ; 
			this.Controls.Add(pv);
		}
		public void AddPageView(string id)
		{
			PageView pv = new PageView() ;
			pv.ID = id;
			
			this.Controls.Add(pv);
		}
		public BPMultiPage()
		{
			this.PreRender += new System.EventHandler(this.BPMultiPagePreRender);
		}
		private void BPMultiPagePreRender( object sender, System.EventArgs e )
		{
			this.SetUserStyle();
			 
		}
		public void SetUserStyle()
		{
			this.CssClass="BPMultiPage"+WebUser.Style;
		}
	}
	
}
