using System;
using System.Web.UI.WebControls;
using System.Drawing;

namespace BP.Web.Controls
{
	/// <summary>
	/// BPTabSpt
	/// </summary>
	public class BPTabSpt : Microsoft.Web.UI.WebControls.TabSeparator
	{
		/// <summary>
		/// TabSeparator
		/// </summary>
		public BPTabSpt()	
		{
		}
		/// <summary>
		/// TabSeparator
		/// </summary>
		/// <param name="id"></param>
		public BPTabSpt(string id )
		{ 
			 
		}
	}
	/// <summary>
	/// from Microsoft.Web.UI.WebControls.Tab
	/// </summary>
	public class BPTab : Microsoft.Web.UI.WebControls.Tab
	{
		/// <summary>
		/// Tab
		/// </summary>
		public BPTab()
		{
	 
		}
		/// <summary>
		/// Tab
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="text">text</param>
		public BPTab(string id, string text )
		{ 
			this.ID=id;
			this.Text = text;
			//this.Render+
		}
		/// <summary>
		/// ww
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="pathID"></param>
		protected internal virtual new void Render(System.Web.UI.HtmlTextWriter writer, Microsoft.Web.UI.WebControls.RenderPathID pathID)
		{
			base.Render (writer, pathID);
            //this.DefaultStyle.CssText="DefaultStyle"+WebUser.Style;
            //this.SelectedStyle.CssText="SelectedStyle"+WebUser.Style;
            ////this.CssClass="TabStrip"+WebUser.Style; 
            //this.DefaultStyle.CssText="TabDefaultStyle"+WebUser.Style;
            //this.HoverStyle.CssText="TabHoverStyle"+WebUser.Style;
            //this.SelectedStyle.CssText="TabSelectedStyle"+WebUser.Style;
            //this.DefaultStyle.CssText="TabSepDefaultStyle"+WebUser.Style;
		}

	}	 
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	public class BPTabStrip:Microsoft.Web.UI.WebControls.TabStrip
	{
		/// <summary>
		/// 获得 BPTab by key.
		/// </summary>
		/// <param name="key">key</param>
		/// <returns></returns>
		public BPTab GetBPTab(string key)
		{
			//return (BPTab)this.Items[];
			for(int i = 0 ;  i < this.Items.Count ;i++  )
			{
				try
				{
					BPTab tab= (BPTab)this.Items[i];
					if (tab.ID==key)
						return tab;
				}
				catch
				{
				}
			}
			throw new Exception("@没有Tab "+key);
		}
		/// <summary>
		/// BPTabStrip
		/// </summary>
		public BPTabStrip()
		{
			

			this.PreRender += new System.EventHandler(this.BPTabStripPreRender);
		}
		/// <summary>
		/// BPTabStripPreRender
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BPTabStripPreRender( object sender, System.EventArgs e )
		{
            //this.CssClass="TabStrip"+WebUser.Style;
            //this.TabDefaultStyle.CssText="StripDefaultStyle"+WebUser.Style;
            //this.TabHoverStyle.CssText="StripHoverStyle"+WebUser.Style;
            //this.TabSelectedStyle.CssText="StripSelectedStyle"+WebUser.Style;
            //this.SepDefaultStyle.CssText="StripDefaultStyle"+WebUser.Style;

			//this.SetUserStyle();
		}	
		
		public void SetUserStyle()
		{		
			this.Font.Bold=true;
			this.TabDefaultStyle.Add("border","solid 1px black");
			this.TabDefaultStyle.Add("background","#dddddd;padding-left:5px;padding-right:5px");
			this.TabHoverStyle.Add("color","red");
			this.TabSelectedStyle.Add("border","solid 1px black");
			this.TabSelectedStyle.Add("border-bottom","none");
			this.Font.Bold=true;
			this.TabSelectedStyle.Add("background","white");
			this.TabSelectedStyle.Add("padding-left","5px");
			this.TabSelectedStyle.Add("padding-right","5px");
			this.TabSelectedStyle.Add("background","white"); 
			this.SepDefaultStyle.Add("border-bottom","solid 1px #000000");
  
		}
		
		/// <summary>
		/// 增加spt
		/// </summary>
		/// <param name="en"></param>
		public void AddSpt(string id)
		{
			BPTabSpt en = new BPTabSpt(id);
			this.Items.Add(en);			 
		}
		/// <summary>
		/// 增加spt
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="text">text</param>
		public void AddTab(string id, string text)
		{
			BPTab en = new BPTab(id,text);
			this.Items.Add(en);
		}
		/// <summary>
		/// 增加spt
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="text">text</param>
		/// <param name="DefaultImageUrl">默认的图片</param>
		/// <param name="HoverImageUrl">悬停的图片</param>
        public void AddTab(string id, string text, string DefaultImageUrl, string HoverImageUrl)
        {
            BPTab en = new BPTab(id, text);
            en.DefaultImageUrl = System.Web.HttpContext.Current.Request.ApplicationPath + DefaultImageUrl;
            en.HoverImageUrl = System.Web.HttpContext.Current.Request.ApplicationPath + HoverImageUrl;
            this.Items.Add(en);
        }
	}
	
}
