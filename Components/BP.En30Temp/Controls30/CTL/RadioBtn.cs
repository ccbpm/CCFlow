using System;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En ; 
using System.Data;
using System.ComponentModel;
 

namespace BP.Web.Controls
{
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.RadioButton))]
	public class RadioBtn : System.Web.UI.WebControls.RadioButton
	{
		public RadioBtn()
		{
		}
//		public RadioBtn(Attr attr)
//		{
////			this.
////			this.MaxLength =attr.MaxLength;
////			//this.Width = Unit.Pixel(attr.UIWidth ); 
////			this.DefaultWith = attr.UIWidth;			
////			 
////			this.ReadOnly = attr.UIIsReadonly ;
////			this.ShowType=attr.UITBShowType ;
////			this.Attributes["size"]=attr.UIWidth.ToString();
////		
////			this.Visible =attr.UIVisible ;			 
////			this.DataHelpKey=attr.UIBindKey ;
////			this.ShowType = attr.UITBShowType ;
////			this.DataHelpKey = attr.UIBindKey;
////		   
////			this.Style.Clear();
////			//this.Style.Add("width",attr.UIWidth.ToString()+"px") ;			
//		}

		/// <summary>
		///  Name No . 
		/// </summary>
		/// <param name="dt"></param> 
		public void BindTable(DataTable dt)
		{
//			foreach (DataRow dr in dt.Rows)
//			{
//				//this.a
//			}
		}			
	}
}
