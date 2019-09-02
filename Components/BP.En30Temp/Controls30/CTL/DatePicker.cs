using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

namespace BP.Web.Controls
{
    public enum CalendarEnum
    {
        None,
        LongDateTime
    }
	/// <summary>
	/// WebCustomControl1 的摘要说明。
	/// </summary>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:DatePicker runat=server></{0}:DatePicker>")]
	public class DatePicker : System.Web.UI.WebControls.WebControl , IPostBackEventHandler
	{
		private const string _BUTTONDEFAULTSTYLE = "BORDER-RIGHT: gray 1px solid; BORDER-TOP: gray 1px solid; BORDER-LEFT: gray 1px solid; CURSOR:hand; BORDER-BOTTOM: gray 1px solid;";
		//选择日期按钮的默认样式s
　   　///按钮默认文本
		private const string _BUTTONDEFAULTTEXT = "...";
		private System.Web.UI.WebControls.Calendar _Calendar;

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
			_Calendar = new Calendar();
			_Calendar.ID = MyCalendarID;
			_Calendar.SelectedDate = DateTime.Parse(Text);
			_Calendar.TitleFormat = TitleFormat.MonthYear;
			_Calendar.NextPrevFormat = NextPrevFormat.ShortMonth;
			_Calendar.CellSpacing = 0;
			_Calendar.Font.Size = FontUnit.Parse("9pt");
			_Calendar.Font.Name = "Verdana";
			_Calendar.SelectedDayStyle.BackColor = ColorTranslator.FromHtml("#333399");
			_Calendar.SelectedDayStyle.ForeColor = ColorTranslator.FromHtml("White");
			_Calendar.DayStyle.BackColor = ColorTranslator.FromHtml("#CCCCCC");
			_Calendar.TodayDayStyle.BackColor = ColorTranslator.FromHtml("#999999");
			_Calendar.TodayDayStyle.ForeColor = ColorTranslator.FromHtml("Aqua");
			_Calendar.DayHeaderStyle.Font.Size = FontUnit.Parse("8pt");
			_Calendar.DayHeaderStyle.Font.Bold = true;
			_Calendar.DayHeaderStyle.Height = Unit.Parse("8pt");
			_Calendar.DayHeaderStyle.ForeColor = ColorTranslator.FromHtml("#333333");
			_Calendar.NextPrevStyle.Font.Size = FontUnit.Parse("8pt"); 
			_Calendar.NextPrevStyle.Font.Bold = true;
			_Calendar.NextPrevStyle.ForeColor = ColorTranslator.FromHtml("White");
			_Calendar.TitleStyle.Font.Size = FontUnit.Parse("12pt"); 
			_Calendar.TitleStyle.Font.Bold = true;
			_Calendar.TitleStyle.Height = Unit.Parse("12pt");
			_Calendar.TitleStyle.ForeColor = ColorTranslator.FromHtml("White");
			_Calendar.TitleStyle.BackColor = ColorTranslator.FromHtml("#333399");
			_Calendar.OtherMonthDayStyle.ForeColor = ColorTranslator.FromHtml("#999999");
			_Calendar.NextPrevFormat = NextPrevFormat.CustomText;
			_Calendar.NextMonthText = "下月";
			_Calendar.PrevMonthText = "上月";
			_Calendar.Style.Add("display","none"); //默认不显示下拉日历控件
			_Calendar.SelectionChanged += new EventHandler(_Calendar_SelectionChanged);
			this.Controls.Add(_Calendar);
		}
		[Category("Appearance"), //该属性所属类别，参见图
		DefaultValue(""), //属性默认值
		Description("设置该日期控件的值。") //属性的描述
		]
		public string Text
		{
			get
			{
				EnsureChildControls();
				return (ViewState["Text"] == null)?System.DateTime.Today.ToString("yyyy-MM-dd"):ViewState["Text"].ToString();
			}
			set
			{
				EnsureChildControls();
				DateTime dt = System.DateTime.Today;
				try
				{
					dt = DateTime.Parse(value);
				}
				catch
				{
					throw new ArgumentOutOfRangeException("请输入日期型字符串（例如：1981-04-29）！");
				}
				ViewState["Text"] = DateFormat == CalendarEnum.LongDateTime?dt.ToString("yyyy-MM-dd"):dt.ToString("yyyy-M-d");
			}
		}
		//重载服务器控件的Enabled属性，将选择日期按钮变灰（禁用）
		public override bool Enabled
		{
			get{EnsureChildControls();return ViewState["Enabled"] == null?true:(bool)ViewState["Enabled"];}
			set{EnsureChildControls();ViewState["Enabled"] = value;}
		}
		public string ButtonStyle
		{
			get
			{
				EnsureChildControls();
				object o = ViewState["ButtonSytle"];
				return (o == null)?_BUTTONDEFAULTSTYLE:o.ToString();
			}
			set
			{
				EnsureChildControls();
				ViewState["ButtonSytle"] = value;
			}
		}

		[
		DefaultValue(CalendarEnum.LongDateTime),
		]

		public CalendarEnum DateFormat
		{
			get
			{
				EnsureChildControls();
				object format = ViewState["DateFormat"];
				return format == null?CalendarEnum.LongDateTime:(CalendarEnum)format;
			}
			set
			{
				EnsureChildControls();
				ViewState["DateFormat"] = value;
				DateTime dt = DateTime.Parse(Text);
				Text=DateFormat == CalendarEnum.LongDateTime?dt.ToString("yyyy-MM-dd"):dt.ToString("yyyy-M-d");
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]

		public string MyCalendarID //复合控件ID
		{
			get
			{
				EnsureChildControls();
				return this.ClientID+"_MyCalendar";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]

		public string MyCalendarName //复合控件名称
		{
			get
			{
				EnsureChildControls();
				return this.UniqueID+":MyCalendar";
			}
		}

		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]

		public string DatePickerInputID //复合控件中输入框的ID
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

		public string DatePickerInputName //复合控件中输入框的名称
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

		public string DatePickerButtonID //复合控件中按钮的ID
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

		public string DatePickerButtonName //复合控件中按钮的名称
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
			output.AddAttribute(HtmlTextWriterAttribute.Id, DatePickerInputID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, DatePickerInputName);
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
			output.AddAttribute(HtmlTextWriterAttribute.Id, DatePickerButtonID);
			output.AddAttribute(HtmlTextWriterAttribute.Name, DatePickerButtonName);
			output.AddAttribute(HtmlTextWriterAttribute.Value, ButtonText);
			output.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
//            output.AddAttribute(HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference(this)); //点击按钮时需要回传服务器来触发后面的OnClick事件

			output.AddAttribute(HtmlTextWriterAttribute.Style, ButtonStyle);
			output.RenderBeginTag(HtmlTextWriterTag.Input); //输出按钮
			output.RenderEndTag();
			output.RenderEndTag();

			output.RenderEndTag();
			output.RenderBeginTag(HtmlTextWriterTag.Tr);
			output.AddAttribute(HtmlTextWriterAttribute.Colspan, "2");
			output.RenderBeginTag(HtmlTextWriterTag.Td);
			_Calendar.RenderControl(output); //将日历子控件输出
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
			//点击选择日期按钮时，如果日历子控件没有显示则显示出来并将文本框的值赋值给日历子控件
			if (_Calendar.Attributes["display"] != "")
			{
				_Calendar.SelectedDate = DateTime.Parse(Text);
				_Calendar.Style.Add("display","");
			}
		}

		//复合控件中的日历控件日期变化事件
		private void _Calendar_SelectionChanged(object sender, EventArgs e)
		{
			//当选择的日期变化时，将所选日期赋值给文本框并将日历子控件隐藏
			Text = _Calendar.SelectedDate.ToString();
			_Calendar.Style.Add("display","none");
		}
	}
}

	
	 
