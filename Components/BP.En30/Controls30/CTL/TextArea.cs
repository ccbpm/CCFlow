using System;
using System.Web.UI.WebControls;
using System.ComponentModel;

using System.Drawing;
using BP.DA ; 
using BP.En;
namespace BP.Web.Controls
{
	
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.TextBox))]
	public class TextArea:System.Web.UI.WebControls.TextBox
	{
		public  TextArea()
		{
			TextMode=TextBoxMode.MultiLine;
			this.Attributes["width"]="100%";
			//this.CssClass="TB"+WebUser.Style;
			//this.Attributes["onkeydown"]="javascript:if(event.keyCode == 13) event.keyCode = 9";
		}
	}
	
}
