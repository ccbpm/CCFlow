using System;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

using BP.En;
using Microsoft.Web.UI.WebControls;
using BP.Web ; 
using BP.Port;
using BP.DA;
using BP.Sys;

namespace BP.Web.Controls
{

	public class ToolbarTB: Microsoft.Web.UI.WebControls.ToolbarTextBox 
	{
		/// <summary>
		/// Renders ToolbarItem attributes.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter to receive markup.</param>
		protected override void WriteItemAttributes(HtmlTextWriter writer)
		{
			base.WriteItemAttributes(writer);

			/*
			string css = CssClass;
			if (css.Length > 0)
			{
				writer.WriteAttribute("class", css);
			}

			string style = String.Empty;

			Color color = ForeColor;
			if (!color.IsEmpty)
			{
				style += "color:" + ColorTranslator.ToHtml(color) + ";";
			}

			color = BackColor;
			if (!color.IsEmpty)
			{
				style += "background-color:" + ColorTranslator.ToHtml(color) + ";";
			}

			color = BorderColor;
			if (!color.IsEmpty)
			{
				style += "border-color:" + ColorTranslator.ToHtml(color) + ";";
			}

			BorderStyle bs = BorderStyle;
			Unit unit = BorderWidth;

			if (bs != BorderStyle.NotSet)
			{
				style += "border-style:" + Enum.Format(typeof(BorderStyle), bs, "G") + ";";
			}

			if (!unit.IsEmpty) 
			{
				style += "border-width:" + unit.ToString(CultureInfo.InvariantCulture) + ";";

				if ((bs == BorderStyle.NotSet) && (unit.Value != 0.0))
				{
					style += "border-style:solid;";
				}
			}

			FontInfo font = Font;

			string[] names = font.Names;
			if (names.Length > 0)
			{
				style += "font-family:";
				for (int i = 0; i < names.Length; i++)
				{
					if (i > 0)
					{
						style += ",";
					}
					style += names[i];
				}
				style += ";";
			}

			FontUnit fu = font.Size;
			if (!fu.IsEmpty)
			{
				style += "font-size:" + fu.ToString(CultureInfo.InvariantCulture) + ";";
			}

			if (font.Bold)
			{
				style += "font-weight:bold;";
			}
			if (font.Italic)
			{
				style += "font-style:italic;";
			}

			bool underline = font.Underline;
			bool overline = font.Overline;
			bool strikeout = font.Strikeout;
			string td = String.Empty;

			if (underline)
				td = "underline";
			if (overline)
				td += " overline";
			if (strikeout)
				td += " line-through";
			if (td.Length > 0)
				style += "text-decoration:" + td + ";";

			unit = Height;
			if (!unit.IsEmpty)
			{
				style += "height:" + unit.ToString(CultureInfo.InvariantCulture) + ";";
			}

			unit = Width;
			if (!unit.IsEmpty)
			{
				style += "width:" + unit.ToString(CultureInfo.InvariantCulture) + ";";
			}

			style += Style.CssText;

			writer.WriteAttribute("style", style);

			//writer.WriteAttribute("type", (TextMode == ToolbarMyTextBoxMode.Password) ? "password" : "text");

			if (Columns > 0)
			{
				writer.WriteAttribute("size", Columns.ToString());
			}
			if (MaxLength > 0)
			{
				writer.WriteAttribute("maxlength", MaxLength.ToString());
			}
			if (ReadOnly)
			{
				writer.WriteAttribute("readonly", ReadOnly.ToString());
			}

			/*
			if ((Text != String.Empty) && (TextMode != ToolbarMyTextBoxMode.Password))
			{
				writer.WriteAttribute("value", Text, true);
			}
			*/

//			if (Enabled)
//			{
//				writer.WriteAttribute("onpropertychange", "window.document.all." + HelperID + ".value=value");
//				writer.WriteAttribute("onchange", "window.document.all." + HelperID + ".value=" + ParentToolbar.ClientID + ".getItem(" + Index + ").getAttribute('value')");
//				writer.WriteAttribute("onkeyup", "window.document.all." + HelperID + ".value=" + ParentToolbar.ClientID + ".getItem(" + Index + ").getAttribute('value')");
//			}
//			else
//			{
//				writer.WriteAttribute("disabled", "true");
//			}
//
//			Toolbar parent = ParentToolbar;
			/*
			string script = "if (event.keyCode==13){event.returnValue=false;";
			if (Enabled && (parent != null) && (parent.Page != null))
			{
				string postBackRef = "if (" + parent.ClientID + ".getAttribute('_submitting') == null){" + parent.ClientID + ".setAttribute('_submitting', 'true');window.setTimeout('" + parent.Page.GetPostBackEventReference(_TextBox).Replace("'", "\\'") + "', 0, 'JScript');}";
				if (AutoPostBack)
				{
					// Blur will cause a postback when AutoPostBack is true
					script += "blur();";

					// Add the blur postback handler
					writer.WriteAttribute("_origVal", (TextMode != ToolbarMyTextBoxMode.Password) ? Text : String.Empty, true);
					writer.WriteAttribute("onblur", "JScript:if (value != _origVal)" + postBackRef);
				}
				else
				{
					// Do the postback
					script += postBackRef + ";";
				}
			}
			script += "}";
			*/
			//writer.WriteAttribute("onkeydown", script);

			 
		}

		/// <summary>
		/// 编号
		/// </summary>
		public string SelfNo
		{
			get
			{
				return (string)ViewState["SelfNo"];
			}
			set
			{
				ViewState["SelfNo"]=value;
			}
		}
		/// <summary>
		/// from ToolbarTextBox
		/// </summary>
		public ToolbarTB()
		{			
			this.CssClass="TB"+WebUser.Style ; 				
		}	
		/// <summary>
		/// from ToolbarTextBox
		/// </summary>
		/// <param name="id"></param>
		public ToolbarTB(string id )
		{
			this.ID=id;
			this.CssClass="TB"+WebUser.Style ; 			 
		}
		/// <summary>
		/// from ToolbarTextBox
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="text">text</param>
		public ToolbarTB(string id,  string text)
		{
			this.ID=id;
			this.Text=text;
			this.CssClass="TB"+WebUser.Style;
		}
		/// <summary>
		/// ToolbarTB
		/// </summary>
		/// <param name="id"></param>
		/// <param name="text"></param>
		/// <param name="selfNo"></param>
		public ToolbarTB(string id,  string text, string selfNo)
		{
			this.ID = id;
			this.Text =text;
			this.SelfNo= selfNo;
			this.CssClass="TB"+WebUser.Style;
		}
		/// <summary>
		/// ToolbarTB
		/// </summary>
		/// <param name="attr">attr</param>
		public ToolbarTB(Attr attr)
		{
			this.MaxLength =attr.MaxLength;
			this.Text = attr.DefaultVal.ToString() ; 
			this.ReadOnly = attr.UIIsReadonly ;
			this.Style.Clear();
			this.CssClass="DGTB"+WebUser.Style;
		}
	}

}