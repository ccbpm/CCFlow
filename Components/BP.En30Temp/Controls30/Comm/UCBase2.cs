using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;

namespace BP.Web.UC
{
    /// <summary>
    /// Well 的摘要说明。
    /// </summary>
    public class UCBase2 : BP.Web.UC.UCBase
    {
        #region 对于 ViewState 的操作。
        /// <summary>
        /// 设置 ViewState Value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="DefaultVal"></param>
        public void SetValueByKey(string key, object val, object DefaultVal)
        {
            if (val == null)
                ViewState[key] = DefaultVal;
            else
                ViewState[key] = val;
        }
        public void SetValueByKey(string key, object val)
        {
            ViewState[key] = val;
        }
        /// <summary>
        /// 取出Val
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueByKey(string key)
        {
            try
            {
                return ViewState[key].ToString();
            }
            catch
            {
                return null;
            }
        }
        public bool GetValueByKeyBool(string key)
        {
            if (this.GetValueByKey(key) == "1")
                return true;
            return false;
        }
        /// <summary>
        /// ss
        /// </summary>
        /// <param name="key">ss</param>
        /// <param name="DefaultVal">ss</param>
        /// <returns></returns>
        public string GetValueByKey_del(string key, string DefaultVal)
        {
            try
            {
                return ViewState[key].ToString();
            }
            catch
            {
                return DefaultVal;
            }
        }
        /// <summary>
        /// 按照key 取出来,bool 的植. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public bool GetBoolValusByKey_del(string key, bool DefaultValue)
        {
            try
            {
                return bool.Parse(this.GetValueByKey(key));
            }
            catch
            {
                return DefaultValue;
            }
        }
        /// <summary>
        /// 取出int valus , 如果没有就返回 DefaultValue ;
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetIntValueByKey_del(string key, int DefaultValue)
        {
            try
            {
                return int.Parse(ViewState[key].ToString());
            }
            catch
            {
                return DefaultValue;
            }
        }
        #endregion 

        #region 与 entity 有关系的操作
        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="en"></param>
        public void SetCtrlValues(Entity en)
        {
            Attrs attrs = en.EnMap.Attrs;
            foreach (System.Web.UI.Control ctl in this.Controls)
            {
                #region 排除非预定的控件
                if (ctl == null || DataType.IsNullOrEmpty(ctl.ID))
                    continue;

                string id = ctl.ID;
                if (id.Contains("TB_") || id.Contains("CB_") || id.Contains("DDL_"))
                {
                    /*约定的控件*/
                }
                else
                {
                    continue;
                }
                #endregion 排除非预定的控件

                #region 设置控件信息。
                foreach (Attr attr in attrs)
                {
                    string myID = "TB_" + attr.Key;
                    if (myID == id)
                    {
                        TextBox tb = ctl as TextBox;
                        if (tb != null)
                        {
                            tb.Text = en.GetValStrByKey(attr.Key);
                            tb.MaxLength = attr.MaxLength;
                            continue;
                        }

                        BP.Web.Controls.TB tbSelf = ctl as BP.Web.Controls.TB;
                        if (tbSelf != null)
                        {                           
                            tbSelf.Text = en.GetValStrByKey(attr.Key);
                            tb.MaxLength = attr.MaxLength;
                            continue;
                        }
                    }

                    myID = "DDL_" + attr.Key;
                    if (myID == id)
                    {
                        DropDownList ddl = ctl as DropDownList;
                        if (ddl != null)
                        {
                            if (ddl.Items.Count == 0)
                            {
                                if (attr.IsEnum)
                                    BP.Web.Controls.Glo.DDL_BindEnum(ddl, attr.UIBindKey, en.GetValIntByKey(attr.Key));

                                if (attr.IsFK)
                                    BP.Web.Controls.Glo.DDL_BindEns(ddl, attr.UIBindKey, en.GetValStrByKey(attr.Key));
                            }

                            foreach (ListItem li in ddl.Items)
                            {
                                li.Selected = false;
                            }

                            string val = en.GetValStrByKey(attr.Key);
                            foreach (ListItem li in ddl.Items)
                            {
                                if (li.Value == val)
                                {
                                    li.Selected = true;
                                    break;
                                }
                            }

                            //try
                            //{
                            //    ddl.SelectedValue = val;
                            //    continue;
                            //}
                            //catch(Exception ex)
                            //{
                            //    throw new Exception(ex.Message+" - "+val);
                            //}
                        }

                        BP.Web.Controls.DDL ddlSelf = ctl as BP.Web.Controls.DDL;
                        if (ddlSelf != null)
                        {
                            ddlSelf.SetSelectItem(  en.GetValStrByKey(attr.Key));
                            continue;
                        }
                    }


                    myID = "CB_" + attr.Key;
                    if (myID == id)
                    {
                        CheckBox cb = ctl as CheckBox;
                        if (cb != null)
                        {
                            if (en.GetValIntByKey(attr.Key) == 1)
                                cb.Checked = true;
                            else
                                cb.Checked = false;
                            continue;
                        }
                    }
                }
                #endregion 设置控件信息。
            }
        }
        public string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = this.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
        private void AddAttrDescVal(string desc, string doc)
        {
            this.AddTDDesc(desc);
            this.AddTD("width=40%", doc);
        }
        private void AddAttrDescVal(string desc, string doc, int colspan)
        {
            this.AddTDDesc(desc);
            this.AddTD(" class=TD align=left width=40% colspan=" + colspan, doc);
        }

        private void AddAttrDescValDoc(string desc, string doc, int colspan)
        {
            if (colspan == 4)
            {
                this.AddTR();
                this.AddTDDesc(desc, colspan);
                this.AddTREnd();

                this.AddTR();
                this.AddTDBigDoc(" class=BigDoc  align=left colspan=4", doc);
                this.AddTREnd();

            }
            else
            {
                this.AddTDBegin(" class=BigDoc align=left colspan=" + colspan);
                this.Add("<b>" + desc + "</b><br>");
                this.Add(doc);
                this.AddTDEnd();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        public void BindViewEn(Entity en, string tableAttr)
        {
            this.Attributes["visibility"] = "hidden";
          //  this.AddTable("width=100%");
        //    this.AddTable("width=90%");

            this.AddTable(tableAttr );

            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            Map map = en.EnMap;
            Attrs attrs = map.Attrs;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                if (attr.MyFieldType == FieldType.RefText)
                    continue;


                val = en.GetValByKey(attr.Key);

                #region 判断是否单列显示
                if (attr.UIIsLine)
                {
                    if (isAddTR == false)
                    {
                        this.AddTD();
                        this.AddTD();
                        this.AddTDEnd();
                    }

                    isLeft = true;
                    isAddTR = true; /*让他下次从0开始。*/
                    if (attr.UIHeight != 0)
                    {
                        /*大块文本采集, 特殊处理。*/
                        if (val.ToString().Length == 0 && en.IsEmpty == false && attr.Key == "Doc")
                            val = en.GetValDocHtml();
                        else
                            val = DataType.ParseText2Html(val.ToString() );

                        this.AddAttrDescValDoc(attr.Desc, val.ToString(), 4);
                        continue;
                    }

                    this.AddTR();
                    if (attr.MyDataType == DataType.AppBoolean)
                    {
                        if (val.ToString() == "1")
                            this.AddAttrDescVal("", "<b>是</b> " + attr.Desc, 3);
                        else
                            this.AddAttrDescVal("", "<b>否</b> " + attr.Desc, 3);
                    }
                    else
                        this.AddAttrDescVal(attr.Desc, val.ToString(), 3);

                    this.AddTREnd();
                    continue;
                }

                #endregion 判断是否单列显示 // 结束要显示单行的情况。
                if (isLeft)
                    this.AddTR();
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        this.AddAttrDescVal(attr.Desc, val.ToString(), 1);
                        //if (attr.UIHeight != 0)
                        //{
                        //    if (val.ToString().Length == 0 && en.IsEmpty == false && attr.Key == "Doc")
                        //        val = en.GetValDocHtml();
                        //    else
                        //        val = DataType.ParseText2Html(val as string);

                        //    this.AddAttrDescValDoc(attr.Desc, val.ToString(), 2);
                        //}
                        //else
                        //{
                        //    this.AddAttrDescVal(attr.Desc, val.ToString(), 1);
                        //}
                         
                        break;
                    case UIContralType.DDL:
                        this.AddAttrDescVal(attr.Desc, en.GetValRefTextByKey(attr.Key), 1 );
                        break;
                    case UIContralType.CheckBok:
                        if (en.GetValBooleanByKey(attr.Key))
                            this.AddAttrDescVal(attr.Desc, "是", 1);
                        else
                            this.AddAttrDescVal(attr.Desc, "否", 1);
                        break;
                    default:
                        break;
                }

                if (isLeft == false)
                    this.AddTREnd();
                isLeft = !isLeft;
            }  // 结束循环.
            this.AddTableEnd();
        }
        #endregion

        #region table green
        /// <summary>
        /// 
        /// </summary>
        public void AddTableGreen()
        {
            this.Add("<Table class='TableGreen' cellpadding='0' cellspacing='0' >");
        }
        public void AddTableGreen(string attr)
        {
            this.Add("<Table  " + attr + " class='TableGreen' cellpadding='1' cellspacing='1' >");
        }
        public void AddTDTitleGreen(string html)
        {
            this.Add("\n<TD class='TitleGreen' nowrap=true  >" + html + "</TD>");
        }
        public void AddTDTitleGreen(string attr, string html)
        {
            this.Add("\n<TD class='TitleGreen' " + attr + " nowrap=true  >" + html + "</TD>");
        }
        #endregion

        #region table Win
        /// <summary>
        /// 
        /// </summary>
        public void AddTableWin()
        {
            this.Add("<Table class='TableWin' cellpadding='0' cellspacing='0' >");
        }
        public void AddTableWin(string attr)
        {
            this.Add("<Table  " + attr + " class='TableWin' cellpadding='1' cellspacing='1' >");
        }
        public void AddTDTitleWin(string html)
        {
            this.Add("\n<TD class='TitleWin' nowrap=true  >" + html + "</TD>");
        }
        public void AddTDTitleWin(string attr, string html)
        {
            this.Add("\n<TD class='TitleWin' " + attr + " nowrap=true  >" + html + "</TD>");
        }

        public void AddTDTitleWin(string attr, System.Web.UI.Control ctl )
        {
            this.Add("\n<TD class='TitleWin' " + attr + " nowrap=true  >");
            this.Controls.Add(ctl);
            this.Add("</TD>");
        }
        #endregion

        #region table 默认的风格。
        /// <summary>
        /// 
        /// </summary>
        public void AddTableDef()
        {
            this.Add("<Table class='TableDef' cellpadding='0' cellspacing='0'>");
        }

        public void AddTableDef(string attr)
        {
            this.Add("<Table " + attr + " class='TableDef' cellpadding='1' cellspacing='1' >");
        }
        public void AddTDTitleDef(string html)
        {
            this.Add("\n<TD class='TitleDef' nowrap=true >" + html + "</TD>");
        }
        public void AddTDTitleDef(string attr, string html)
        {
            this.Add("\n<TD class='TitleDef' " + attr + " nowrap=true  >" + html + "</TD>");
        }
        public void AddTableBarDef(string title, int col)
        {
            this.Add("\n<TR class='TR'>");
            this.Add("\n<TD class='TitleDef'  colspan=" + col + " >&nbsp;&nbsp;<img src='/img/dot.gif' border=0 />&nbsp;&nbsp;" + title + "</TD>");
            this.Add("\n</TR>");
        }
        public void AddTableBarDef(string imgUrl, string title, int col, string leftMore)
        {
            string msg = "";
            if (imgUrl != null)
                msg = "<table border=0 width='100%'  class='BarTable'  ><TR class='BarTableTR' ><TD class='BarTableTD' >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";
            else
                msg = "<table border=0 width='100%'  class='BarTable'  ><TR class='BarTableTR' ><TD class='BarTableTD' >&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";

            //this.AddCaption(msg);

            this.Add("\n<TR class='TR' height='0%' >");
            this.Add("\n<TD class='TitleDef' height='0%' colspan=" + col + " >" + msg + "</TD>");
            this.Add("\n</TR>");

            //this.Add("\n<TR class='TR'>");
            //this.Add("\n<TD class='TitleDef'  colspan=" + col + " >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "<img src='" + imgUrl + "' align=right border=0 /></TD>");
            //this.Add("\n</TR>");
        }
        public void AddTableBarGreen(string imgUrl, string title, int col, string leftMore)
        {
            string msg = "";
            if (imgUrl != null)
                msg = "<table border=0 width='100%'   ><TR class='BarTableTR' ><TD  >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";
            else
                msg = "<table border=0 width='100%'    ><TR   ><TD class='BarTableTD' >&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";

            //this.AddCaption(msg);

            this.Add("\n<TR class='TR' height='0%' >");
            this.Add("\n<TD class='TitleGreen' height='0%' colspan=" + col + " >" + msg + "</TD>");
            this.Add("\n</TR>");

            //this.Add("\n<TR class='TR'>");
            //this.Add("\n<TD class='TitleDef'  colspan=" + col + " >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "<img src='" + imgUrl + "' align=right border=0 /></TD>");
            //this.Add("\n</TR>");
        }
        public void AddTableBarBlue(string imgUrl, string title, int col, string leftMore)
        {
            string msg = "";
            if (imgUrl != null)
                msg = "<table border=0 width='100%'   ><TR class='BarTableTR' ><TD  >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";
            else
                msg = "<table border=0 width='100%'    ><TR   ><TD class='BarTableTD' >&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";

            //this.AddCaption(msg);

            this.Add("\n<TR class='TR' height='0%' >");
            this.Add("\n<TD class='TitleBlue' height='0%' colspan=" + col + " >" + msg + "</TD>");
            this.Add("\n</TR>");

            //this.Add("\n<TR class='TR'>");
            //this.Add("\n<TD class='TitleDef'  colspan=" + col + " >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "<img src='" + imgUrl + "' align=right border=0 /></TD>");
            //this.Add("\n</TR>");
        }
        public new void AddTableBarRed(string imgUrl, string title, int col, string leftMore)
        {
            string msg = "";
            if (imgUrl != null)
                msg = "<table border=0 width='100%'   ><TR class='BarTableTR' ><TD  >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";
            else
                msg = "<table border=0 width='100%'    ><TR   ><TD class='BarTableTD' >&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";

            //this.AddCaption(msg);

            this.Add("\n<TR class='TR' height='0%' >");
            this.Add("\n<TD class='TitleRed' height='0%' colspan=" + col + " >" + msg + "</TD>");
            this.Add("\n</TR>");

            //this.Add("\n<TR class='TR'>");
            //this.Add("\n<TD class='TitleDef'  colspan=" + col + " >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "<img src='" + imgUrl + "' align=right border=0 /></TD>");
            //this.Add("\n</TR>");
        }
        #endregion

        public void AddTDTitleItem(string html)
        {
            this.Add("\n<TD class='TDItem' nowrap=true  >" + html + "</TD>");
        }

        #region table Blue
        /// <summary>
        /// 
        /// </summary>
        public void AddTableBlue()
        {
            this.Add("<Table class='TableBlue' cellpadding='0' cellspacing='0' >");
        }
        public void AddTableBlue(string attr)
        {
            this.Add("<Table  " + attr + " class='TableBlue' cellpadding='1' cellspacing='1' >");
        }
        public void AddTDTitleBlue(string html)
        {
            this.Add("\n<TD class='TitleBlue' nowrap=true  >" + html + "</TD>");
        }
        public void AddTDTitleBlue(string attr, string html)
        {
            this.Add("\n<TD class='TitleBlue' " + attr + " nowrap=true  >" + html + "</TD>");
        }
        #endregion

        #region table Red
        public void AddTableRed()
        {
            this.Add("<Table class='TableRed' cellpadding='0' cellspacing='0' >");
        }
        public void AddTableRed(string attr)
        {
            this.Add("<Table class='TableRed' " + attr + "  cellpadding='0' cellspacing='0' >");
        }
        public void AddTDTitleRed(string html)
        {
            this.Add("\n<TD class='TitleRed' nowrap=true  >" + html + "</TD>");
        }
        public void AddTDTitleRed(string attr, string html)
        {
            this.Add("\n<TD class='TitleRed' " + attr + " nowrap=true  >" + html + "</TD>");
        }
        public void AddTDTitleRed(string imgUrl, string title, int col, string leftMore)
        {
            string msg = "";
            if (imgUrl == null)
                msg = "<table border=0 width='100%'  class='BarTable'  ><TR class='BarTableTR' ><TD class='BarTableTD' >&nbsp;&nbsp;" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";
            else
                msg = "<table border=0 width='100%'  class='BarTable'  ><TR class='BarTableTR' ><TD class='BarTableTD' >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />" + title + "</TD><TD class='BarTableTD' align='right'>" + leftMore + "</TD></TR></Table>";

            //this.AddCaption(msg);
            this.Add("\n<TR class='TR' height='0%' >");
            this.Add("\n<TD class='TitleRed' height='0%' colspan=" + col + " >" + msg + "</TD>");
            this.Add("\n</TR>");
            //this.Add("\n<TR class='TR'>");
            //this.Add("\n<TD class='TitleDef'  colspan=" + col + " >&nbsp;&nbsp;<img src='" + imgUrl + "' border=0 />&nbsp;&nbsp;" + title + "<img src='" + imgUrl + "' align=right border=0 /></TD>");
            //this.Add("\n</TR>");
        }
        #endregion


        #region ucbase3 移动
        public string FK_Sort
        {
            get
            {
                string s= this.Request.QueryString["FK_Sort"];
                if (s == "")
                    return null;
                return s;
            }
        }
        public new int RefOID
        {
            get
            {
                string s = this.Request.QueryString["RefOID"];
                if (s == "")
                    s = this.Request.QueryString["OID"];
                if (s == "")
                    return 0;

                return int.Parse(s);
            }
        }
        public string MyPK
        {
            get
            {
                string s = this.Request.QueryString["MyPK"];
                if (s == "")
                    s = this.Request.QueryString["PK"];
                if (s == "")
                    s = null;
                return s;
            }
        }
        public string FK_Type
        {
            get
            {
                string s = this.Request.QueryString["FK_Type"];
                if (s == "")
                    return null;
                return s;
            }
        }
        public void AddDateTime(DateTime dt)
        {
        }
        public void BindMenu(BP.Sys.XML.XmlMenus ens)
        {
            this.AddTable("width='100%'");
            this.AddTR();
            foreach (BP.Sys.XML.XmlMenu en in ens)
            {
                this.AddTD("<a href='" + en.Url + "'>" + en.Name + "</a>");
            }
            this.AddTREnd();
            this.AddTableEnd();
        }
        #endregion

    }
}
