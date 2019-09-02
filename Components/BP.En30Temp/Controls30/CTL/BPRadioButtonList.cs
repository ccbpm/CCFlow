using System;
using System.Web.UI.WebControls;
using BP.DA;
using BP.En;
using System.Data;
using System.ComponentModel;


namespace BP.Web.Controls
{
	/// <summary>
	/// BPListBox 的摘要说明。
	/// </summary>
	[System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.HyperLink))]
	public class BPRadioButtonList : System.Web.UI.WebControls.RadioButtonList
	{
		public void SetSelectItem(Object val)
		{
			foreach(ListItem li in this.Items)
			{
				if (li.Value==val.ToString())
				{
					li.Selected=true;
					break;
				}
			}
		}
		public BPRadioButtonList()
		{
			
			 
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			base.Render (writer);

		}

		/// <summary>
		///  Name No . 
		/// </summary>
		/// <param name="dt"></param> 
		public void BindTable(DataTable dt)
		{
			this.Items.Clear();
			foreach (DataRow dr in dt.Rows)
			{
				this.Items.Add(new ListItem(dr["No"].ToString()+dr["Name"].ToString(),dr["No"].ToString()));
			}
		}			
	}
}
