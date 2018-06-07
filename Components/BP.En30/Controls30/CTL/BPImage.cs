using System;
using System.Web.UI.WebControls;
using System.Data ; 
using BP.DA;
using BP.En;
using System.ComponentModel;
using BP.Port;


namespace BP.Web.Controls
{
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.Image))]

	public class BPImage:System.Web.UI.WebControls.Image 
	{
		public BPImage()
		{
			this.CssClass="BPImage"+WebUser.Style;		 
		}	 	
		 
	}
}
