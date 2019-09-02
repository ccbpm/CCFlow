using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

namespace BP.Web.Controls
{
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:ImgEn runat=server></{0}:ImgEn>")]
	public class ImgEn : System.Web.UI.WebControls.WebControl , IPostBackEventHandler
	{
		private const string _BUTTONDEFAULTSTYLE = "BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; BORDER-LEFT: gray 1px solid; CURSOR:hand; BORDER-BOTTOM: gray 1px solid;";
		//选择Label按钮的默认样式s
　   　///按钮默认文本
		private const string _BUTTONDEFAULTTEXT = "...";
		private System.Web.UI.WebControls.Label _Label;
		/// <summary>
		/// Controls
		/// </summary>
		public override ControlCollection Controls
		{
			get
			{
				EnsureChildControls(); //确认子控件集都已被创建
				return base.Controls;
			}
		}

		//创建子控件（服务器日历控件）
		protected override void CreateChildControls()
		{
			Controls.Clear();
			_Label = new Label();
			_Label.ID = MyLabelID;
			_Label.Font.Size = FontUnit.Parse("9pt");
			_Label.Font.Name = "Verdana";

			this.Controls.Add(_Label);
		}
		[Category("Appearance"), //该属性所属类别，参见图
		DefaultValue(""), //属性默认值
		Description("设置该Label控件的值。") //属性的描述
		]
		public string Text
		{
			get
			{
				EnsureChildControls();
				return (string)ViewState["Text"];
			}
			set
			{
				ViewState["Text"] = value;
			}
		}
		//重载服务器控件的Enabled属性，将选择Label按钮变灰（禁用）
		public override bool Enabled
		{
			get{EnsureChildControls();return ViewState["Enabled"] == null?true:(bool)ViewState["Enabled"];}
			set{EnsureChildControls();ViewState["Enabled"] = value;}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string MyLabelID //复合控件ID
		{
			get
			{
				EnsureChildControls();
				return this.ClientID+"_MyLabel";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string MyLabelName //复合控件名称
		{
			get
			{
				EnsureChildControls();
				return this.UniqueID+":MyLabel";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string ImgEnInputID //复合控件中输入框的ID
		{
			get
			{
				EnsureChildControls();
				return this.ClientID+"_DateInput";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string ImgEnInputName //复合控件中输入框的名称
		{
			get
			{
				EnsureChildControls();
				return this.UniqueID+":DateInput";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string ImgEnButtonID //复合控件中按钮的ID
		{
			get
			{
				EnsureChildControls();
				return this.ClientID+"_DateButton";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]

		public string ImgEnButtonName //复合控件中按钮的名称
		{
			get
			{
				EnsureChildControls();
				return this.UniqueID+":DateButton";
			}
		}

		public string ButtonText
		{
			get
			{
				EnsureChildControls();
				return ViewState["ButtonText"] == null?_BUTTONDEFAULTTEXT:(string)ViewState["ButtonText"];
			}
			set
			{
				EnsureChildControls();
				ViewState["ButtonText"] = value;
			}
		}

		/// <summary>
		/// 将此控件呈现给指定的输出参数。
		/// </summary>
		/// <param name="output"> 要写出到的 HTML 编写器 </param>

		protected override void Render(HtmlTextWriter output)
		{
			//在页面中输出控件时，产生一个表格（二行二列），以下是表格的样式
			output.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
			output.AddAttribute(HtmlTextWriterAttribute.Border, "0");
			output.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");

			output.AddStyleAttribute("LEFT", this.Style["LEFT"]);
			output.AddStyleAttribute("TOP", this.Style["TOP"]);
			output.AddStyleAttribute("POSITION", "absolute");

			if (Width != Unit.Empty)
			{
				output.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
			}
			else
			{
				output.AddStyleAttribute(HtmlTextWriterStyle.Width, "200px");
			}

			output.RenderBeginTag(HtmlTextWriterTag.Table); //输出表格
			output.RenderBeginTag(HtmlTextWriterTag.Tr); //表格第一行
			output.AddAttribute(HtmlTextWriterAttribute.Width, "90%");
			output.RenderBeginTag(HtmlTextWriterTag.Td);

			//以下是第一行第一列中文本框的属性及其样式设置

			if (!Enabled)
			{
				output.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "true");
			}

			output.AddAttribute(HtmlTextWriterAttribute.Type, "Text");
			output.AddAttribute(HtmlTextWriterAttribute.Id, ImgEnInputID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, ImgEnInputName);
			output.AddAttribute(HtmlTextWriterAttribute.Value, Text);
			output.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
			output.AddStyleAttribute(HtmlTextWriterStyle.Height, "100%");
			output.AddStyleAttribute(HtmlTextWriterStyle.FontFamily, Font.Name);
			output.AddStyleAttribute(HtmlTextWriterStyle.FontSize, Font.Size.ToString());
			output.AddStyleAttribute(HtmlTextWriterStyle.FontWeight, Font.Bold?"bold":"");
			output.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(BackColor));
			output.AddStyleAttribute(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(ForeColor));
			output.RenderBeginTag(HtmlTextWriterTag.Input); //输出文本框
			output.RenderEndTag();
			output.RenderEndTag();
			output.AddAttribute(HtmlTextWriterAttribute.Width, "*");
			output.RenderBeginTag(HtmlTextWriterTag.Td);

			//以下是第一行第二列中按钮的属性及其样式设置

			if (!Enabled)
			{
				output.AddAttribute(HtmlTextWriterAttribute.Disabled, "true");
			}

			output.AddAttribute(HtmlTextWriterAttribute.Type, "Submit");
			output.AddAttribute(HtmlTextWriterAttribute.Id, ImgEnButtonID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, ImgEnButtonName);
			output.AddAttribute(HtmlTextWriterAttribute.Value, ButtonText);
			output.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
			//output.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.GetPostBackEventReference(this)); //点击按钮时需要回传服务器来触发后面的OnClick事件

			//output.AddAttribute(HtmlTextWriterAttribute.Style, ButtonStyle);
			output.RenderBeginTag(HtmlTextWriterTag.Input); //输出按钮
			output.RenderEndTag();
			output.RenderEndTag();

			output.RenderEndTag();
			output.RenderBeginTag(HtmlTextWriterTag.Tr);
			output.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
			output.RenderBeginTag(HtmlTextWriterTag.Td);
			_Label.RenderControl(output); //将日历子控件输出
			output.RenderEndTag();
			output.RenderEndTag();
			output.RenderEndTag();
		}

		//复合控件必须继承IpostBackEventHandler接口，才能继承RaisePostBackEvent事件
		public void RaisePostBackEvent(string eventArgument)
		{
			OnClick(EventArgs.Empty);
		}
		protected virtual void OnClick(EventArgs e)
		{
			//			//点击选择日期按钮时，如果日历子控件没有显示则显示出来并将文本框的值赋值给日历子控件
			//			if (_Calendar.Attributes["display"] != "")
			//			{
			//				_Calendar.SelectedDate = DateTime.Parse(Text);
			//				_Calendar.Style.Add("display","");
			//			}
		}

		//复合控件中的日历控件Label变化事件
		private void _Label_SelectionChanged(object sender, EventArgs e)
		{
			return ;

			//当选择的Label变化时，将所选Label赋值给文本框并将日历子控件隐藏
			//Text = _Label.SelectedDate.ToString();
			//_Label.Style.Add("display","none");
		}
	}
}

	
	 
