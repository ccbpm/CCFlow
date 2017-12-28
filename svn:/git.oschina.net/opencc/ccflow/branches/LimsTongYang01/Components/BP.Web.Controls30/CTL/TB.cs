using System;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using BP.DA;
using BP.En;
namespace BP.Web.Controls
{
    /// <summary>
    /// BPListBox 的摘要说明。
    /// </summary>
    [System.Drawing.ToolboxBitmap(typeof(System.Web.UI.WebControls.TextBox))]
    public class TB : System.Web.UI.WebControls.TextBox
    {
        #region 扩展属性



        #region 处理 DataHelpKey 的扩展属性
        /// <summary>
        /// 帮助的Key. 
        /// 用于ens 的帮助
        /// </summary>
        public string RefKey
        {
            get
            {
                return (string)this.ViewState["RefKey"];
            }
            set
            {
                this.ViewState["RefKey"] = value;
            }
        }
        /// <summary>
        /// 帮助的RefText. 
        /// 用于ens 的帮助
        /// </summary>
        public string RefText
        {
            get
            {
                return (string)this.ViewState["RefText"];
            }
            set
            {
                this.ViewState["RefText"] = value;
            }
        }

        /// <summary>
        /// 帮助的Key. 
        /// </summary>
        public string DataHelpKey
        {
            get
            {
                return (string)this.ViewState["DataHelpKey"];
            }
            set
            {
                this.ViewState["DataHelpKey"] = value;
            }
        }
        /// <summary>
        /// 属性Key
        /// </summary>
        public string AttrKey
        {
            get
            {
                return (string)this.ViewState["AttrKey"];
            }
            set
            {
                this.ViewState["AttrKey"] = value;
            }
        }
        #endregion

        #region 普通的扩展属性
        /// <summary>
        /// 目标
        /// </summary>
        public object Tag
        {
            get
            {
                return ViewState["Tag"].ToString();
            }
            set
            {
                ViewState["Tag"] = value;
            }
        }
        public bool IsHelpKey
        {
            get
            {
                string v = ViewState["HelpKey"] as string;
                if (v == "1")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    ViewState["HelpKey"] = "1";
                else
                    ViewState["HelpKey"] = "0";
            }
        }
        /// <summary>
        /// TB类型
        /// </summary>
        private TBType _ShowType = TBType.TB;
        /// <summary>
        /// TB类型
        /// </summary>
        public TBType ShowType
        {
            get
            {
                return _ShowType;
            }
            set
            {
                this._ShowType = value;
                if (this._ShowType == TBType.EnsOfMany)
                    this.IsHelpKey = true;

                string appPath = "/Front/";  // System.Web.HttpContext.Current.Request.CurrentExecutionFilePath ; 
                string url = "";
                string script = "";

                switch (_ShowType)
                {
                    case TBType.Ens: //如果是要制定的Ens.
                        //this.Width=Unit.Pixel(this.DefaultWith);
                        url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/UIEns.aspx?EnsName=" + this.DataHelpKey + "&IsDataHelp=1";
                        script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 500px; dialogWidth:800px; dialogTop: 150px; dialogLeft: 170px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                        this.Attributes["onmousedown"] = script;
                        //this.ToolTip="右健高级查找并选择。";
                        break;
                    case TBType.EnsOfMany: //如果是要制定的Ens.
                        //this.Width=Unit.Pixel(this.DefaultWith);
                        url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/UIDataHelpEnsValues.aspx?EnsName=" + this.DataHelpKey + "&IsDataHelp=1&RefKey=" + this.RefKey + "&RefText=" + this.RefText;
                        script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 400px; dialogWidth:600px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                        this.Attributes["onmousedown"] = script;
                        //this.ToolTip =
                        this.ToolTip = "右健高级查找并选择。";
                        break;
                    case TBType.Self:
                        if (this.DataHelpKey == null)
                            throw new Exception("@您没有指定要邦定Key.");
                        break;
                    case TBType.Date:
                        this.Columns = 10;
                        this.MaxLength = 20;
                        if (this.Text == null || this.Text == null)
                            this.Text = DataType.CurrentData;
                        this.Attributes["class"] = "TBcalendar";
                        break;
                    case TBType.DateTime:
                        this.Attributes["class"] = "TBcalendar";
                       // this.Columns = 16;
                        if (this.Text == null || this.Text == null)
                            this.Text = DataType.CurrentDataTime;
                        if (this.ReadOnly == false)
                        {
                            this.MaxLength = 20;
                            //this.Attributes["OnKeyPress"]="javascript:return VirtyDatetime(this);";
                        }
                        this.Attributes["class"] = "TBcalendar";
                        break;
                    //case TBType.Email:
                    //    if (this.Text == null || this.Text == null)
                    //        this.Text = "@";
                    //    break;
                    case TBType.Moneny:
                    case TBType.Decimal:
                    case TBType.Float:
                        this.MaxLength = 14;
                        this.CssClass = "TBNum";
                        this.Columns = 12;
                        //this.Attributes["size"]="14";
                        this.Attributes["OnKeyPress"] += "return VirtyNum(this);";
                        if (this.Text == null || this.Text == "" || this.Text == "&nbsp;" || this.Text == "0")
                            this.Text = "0.00";
                        if (this.Text.IndexOf(".") == -1)
                            this.Text = this.Text + ".00";

                        if (_ShowType == TBType.Moneny)
                        {
                            this.Attributes["onblur"] += "this.value=VirtyMoney(this.value);";
                        }
                        break;
                    case TBType.Num:
                    case TBType.Int:
                        this.MaxLength = 14;
                        this.CssClass = "TBNum";
                        //this.Attributes["text-align"]="right";
                        //this.Attributes["size"]="14";
                        this.Columns = 8;
                        this.Attributes["OnKeyPress"] = "javascript:return VirtyNum(this);";
                        if (this.Text == null || this.Text == null)
                            this.Text = "0";

                        if (this.ReadOnly)
                            this.CssClass = "TBNumReadonly";
                        else
                            this.CssClass = "TBNum";
                        break;
                    case TBType.TB:
                        if (this.EnsName != null && this.IsHelpKey)
                        {
                            url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/HelperOfTB.aspx?EnsName=" + this.EnsName + "&AttrKey=" + this.AttrKey;
                            script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                            this.Attributes["onmousedown"] = script;
                            //this.ToolTip="右健选择或设置定义默认值。";
                        }
                        if (this.ReadOnly)
                        {
                            this.CssClass = "TBReadonly";
                        }
                        else
                        {
                            this.CssClass = "TB";
                        }
                        break;
                    case TBType.Area:
                        if (this.EnsName != null && this.IsHelpKey)
                        {
                            url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/HelperOfTB.aspx?EnsName=" + this.EnsName + "&AttrKey=" + this.AttrKey;
                            script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                            this.Attributes["onmousedown"] = script;
                        }
                        else
                        {
                            this.Rows = 8;
                            this.TextMode = TextBoxMode.MultiLine;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 与属性相关的类名称
        /// </summary>
        public string EnsName
        {
            get
            {
                return ViewState["EnsName"] as string;
            }
            set
            {
                ViewState["EnsName"] = value;
            }
        }
        #endregion

        #endregion

        #region 构造函数
        public TB()
        {
            //this.CssClass="TB"+WebUser.Style;
            if (this.TextMode == System.Web.UI.WebControls.TextBoxMode.MultiLine)
            {
                this.Attributes["onkeydown"] = "javascript:if(event.keyCode == 13) event.keyCode = 9";
            }

            //this.Attributes["onmouseover"] = "TBOnfocus(this)";
            //this.Attributes["onmouseout"] = "TBOnblur(this)";
        }
        public void LoadMapAttr(Attr attr)
        {
            this.MaxLength = attr.MaxLength;
            this.AttrKey = attr.Key;
            this.ReadOnly = attr.UIIsReadonly;
            this.Attributes["size"] = attr.UIWidth.ToString();
            this.Visible = attr.UIVisible;
            this.DataHelpKey = attr.UIBindKey;
            if (attr.UIWidth == 0)
                this.Columns = attr.MaxLength;
            else
                this.Columns = attr.UIWidthInt;

            switch (attr.MyDataType)
            {
                case DataType.AppInt:
                case DataType.AppFloat:
                case DataType.AppDouble:
                    this.ShowType = TBType.Num;
                    break;
                case DataType.AppDate:
                    this.ShowType = TBType.Date;
                    this.Attributes["onfocus"] = "WdatePicker();";
                    break;
                case DataType.AppDateTime:
                    this.ShowType = TBType.DateTime;
                    this.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                    break;
                case DataType.AppMoney:
                    this.ShowType = TBType.Moneny;
                    break;
                case DataType.AppString:
                    this.ShowType = TBType.TB;
                    break;
                default:
                    this.ShowType = TBType.TB;
                    break;
            }
            this.PreRender += new System.EventHandler(this.TBPreRender);
        }
        public void LoadMapAttr(BP.Sys.MapAttr attr)
        {
            this.MaxLength = attr.MaxLen;
            this.AttrKey = attr.KeyOfEn;
            this.ReadOnly = !attr.UIIsEnable;

            this.Visible = attr.UIVisible;
            this.DataHelpKey = attr.UIBindKey;

            switch (attr.MyDataType)
            {
                case DataType.AppInt:
                case DataType.AppFloat:
                case DataType.AppDouble:
                    this.ShowType = TBType.Num;
                    break;
                case DataType.AppDate:
                    this.ShowType = TBType.Date;
                    this.Attributes["onfocus"] = "WdatePicker();";
                    break;
                case DataType.AppDateTime:
                    this.ShowType = TBType.DateTime;
                    this.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                    break;
                case DataType.AppMoney:
                    this.ShowType = TBType.Moneny;
                    break;
                case DataType.AppString:
                    this.ShowType = TBType.TB;
                    break;
                default:
                    break;
            }
            this.PreRender += new System.EventHandler(this.TBPreRender);
        }
        #endregion

        #region 处理不同的数值类型(在页面呈现时间)
        private void TBPreRender(object sender, System.EventArgs e)
        {
            if (this.IsHelpKey == false)
                return;

            if (this.ReadOnly)
            {
                if (this.Text == "&nbsp;")
                    this.Text = null;

                if (this.TextMode == TextBoxMode.MultiLine)
                {
                    //this.Attributes["style"]="height=100%;width=100%;";
                    this.Attributes["onkeydown"] += " return ; if(event.keyCode==13)  event.keyCode=13;";
                }
                else
                {
                    // this.CssClass = "TBReadonly" + WebUser.Style;
                    this.CssClass = "TBReadonly"; // +WebUser.Style;
                }
                return;
            }

            if (this.Text == "&nbsp;")
                this.Text = null;

            //RenderJavaScript();
            string script, url;
            string appPath = this.Page.Request.ApplicationPath;  // System.Web.HttpContext.Current.Request.CurrentExecutionFilePath ; 
            if (appPath == "/")
                return;

            switch (ShowType)
            {
                case TBType.Ens: //如果是要制定的Ens.
                    //this.Width=Unit.Pixel(this.DefaultWith);
                    url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/UIEns.aspx?EnsName=" + this.DataHelpKey + "&IsDataHelp=1";
                    script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 500px; dialogWidth:800px; dialogTop: 150px; dialogLeft: 170px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                    this.Attributes["onmousedown"] = script;
                    //this.ToolTip="右健高级查找并选择。";
                    break;
                case TBType.EnsOfMany: //如果是要制定的Ens.
                    //this.Width=Unit.Pixel(this.DefaultWith);
                    url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/UIDataHelpEnsValues.aspx?EnsName=" + this.DataHelpKey + "&IsDataHelp=1&RefKey=" + this.RefKey + "&RefText=" + this.RefText;
                    script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 400px; dialogWidth:600px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                    this.Attributes["onmousedown"] = script;
                    //this.ToolTip =
                    this.ToolTip = "右健高级查找并选择。";
                    break;
                case TBType.Self:
                    if (this.DataHelpKey == null)
                        throw new Exception("@您没有指定要邦定Key.");
                    break;
                case TBType.Date:
                    this.Columns = 10;
                    this.MaxLength = 20;
                    if (this.Text == null || this.Text == null)
                        this.Text = DataType.CurrentData;

                    if (this.ReadOnly == false)
                        this.Attributes["onfocus"] = "WdatePicker();";

                  //  this.Attributes["onfocus"] = "calendar();";

                    //this.Attributes["onmousedown"] = "javascript:ShowDateTime('" + appPath + "', this );";
                    //this.Attributes[""] = "javascript:ShowDateTime('" + appPath + "', this );";
                    break;
                case TBType.DateTime:
                    this.Columns = 20;
                    if (this.Text == null || this.Text == null)
                        this.Text = DataType.CurrentDataTime;

                    if (this.ReadOnly == false)
                        this.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";

                    //this.Attributes["onmousedown"] = "javascript:ShowDateTime('" + appPath + "', this );";
                    //if (this.ReadOnly == false)
                    //{
                    //    this.Attributes["onmousedown"] = "javascript:ShowDateTime('" + appPath + "',this);";
                    //    this.MaxLength = 20;
                    //    //this.Attributes["OnKeyPress"]="javascript:return VirtyDatetime(this);";
                    //}
                    break;
                case TBType.Email:
                    if (this.Text == null || this.Text == null)
                        this.Text = "@";
                    break;
                case TBType.Moneny:
                      this.MaxLength = 14;
                    this.CssClass = "TBNum";
                    this.Columns = 12;
                    this.Attributes["OnKeyPress"] = "javascript:return VirtyNum(this);";
                    this.Attributes["onblur"] = "this.value=VirtyMoney(this.value);";

                    if (this.Text == null || this.Text == "" || this.Text == "&nbsp;" || this.Text == "0")
                        this.Text = "0.00";
                    if (this.Text.IndexOf(".") == -1)
                        this.Text = this.Text + ".00";
                    break;
                case TBType.Decimal:
                case TBType.Float:
                    this.MaxLength = 14;
                    this.CssClass = "TBNum";
                    this.Columns = 12;
                    //this.Attributes["size"]="14";
                    this.Attributes["OnKeyPress"] = "javascript:return VirtyNum(this);";
                    if (this.Text == null || this.Text == "" || this.Text == "&nbsp;" || this.Text == "0")
                        this.Text = "0.00";
                    if (this.Text.IndexOf(".") == -1)
                        this.Text = this.Text + ".00";
                    break;
                case TBType.Num:
                case TBType.Int:
                    this.MaxLength = 14;
                    this.CssClass = "TBNum";
                    //this.Attributes["text-align"]="right";
                    //this.Attributes["size"]="14";
                    this.Columns = 8;
                    this.Attributes["OnKeyPress"] = "javascript:return VirtyNum(this);";
                    if (this.Text == null || this.Text == null)
                        this.Text = "0";
                    break;
                case TBType.TB:
                    if (this.EnsName != null && this.IsHelpKey)
                    {
                        url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/HelperOfTB.aspx?EnsName=" + this.EnsName + "&AttrKey=" + this.AttrKey;
                        script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                        this.Attributes["onmousedown"] = script;
                        //this.ToolTip="右健选择或设置定义默认值。";
                    }
                    break;
                case TBType.Area:
                    if (this.EnsName != null && this.IsHelpKey)
                    {
                        url = appPath + "Comm/RefFunc/DataHelp.htm?" + appPath + "Comm/HelperOfTB.aspx?EnsName=" + this.EnsName + "&AttrKey=" + this.AttrKey;
                        script = " if ( event.button != 2)  return; str=" + this.ClientID + ".value;str= window.showModalDialog('" + url + "&Key=\'+str, '','dialogHeight: 500px; dialogWidth:850px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no'); if ( str==undefined) return ; " + this.ClientID + ".value=str ; ";
                        this.Attributes["onmousedown"] = script;
                    }
                    else
                    {
                        this.Rows = 8;
                        this.TextMode = TextBoxMode.MultiLine;
                    }
                    break;
                default:
                    break;
            }
            /*
            if (this.TextMode!= TextBoxMode.MultiLine)
            {
                this.Attributes.Add("onmouseover","DGTBOnOn(this)");
                this.Attributes.Add("onmouseout","DGTBOnOut(this)");
            }
            */
        }
        #endregion

        #region 取出扩展的属性（用于方便取信息）
        /// <summary>
        /// 取出扩展TextExt属性
        /// </summary>
        public object TextExt
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value.ToString();
            }
        }
        /// <summary>
        /// 取出扩展Int属性
        /// </summary>
        public int TextExtInt
        {
            get
            {
                return int.Parse(this.Text);
            }
            set
            {
                this.Text = value.ToString();
            }
        }
        /// <summary>
        /// 取出扩展Float属性
        /// </summary>
        public float TextExtFloat
        {
            get
            {
                return float.Parse(this.Text.Trim());
            }
            set
            {
                this.Text = value.ToString();
            }
        }
        /// <summary>
        /// 取出扩展Float属性
        /// </summary>
        public decimal TextExtMoney
        {
            get
            {
                return decimal.Parse(this.Text.Trim());
            }
            set
            {
                this.Text = value.ToString("0.00");
            }
        }
        /// <summary>
        /// 取出扩展Decimal属性
        /// </summary>
        public decimal TextExtDecimal
        {
            get
            {
                string str = this.Text.Trim();
                if (str.Length == 0)
                    str = "0";
                try
                {
                    return decimal.Parse(str);
                }
                catch
                {
                    this.Text = "0";
                    return 0;
                }
            }
            set
            {
                this.Text = Decimal.Round(value, 2).ToString();
            }
        }
        /// <summary>
        /// 取出扩展日期属性
        /// </summary>
        public string TextExtDate
        {
            get
            {
                return DataType.StringToDateStr(this.Text.Trim());
            }
            set
            {
                this.Text = DataType.StringToDateStr(value);
            }
        }
        #endregion

    }
}
