using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.WF;
using BP.En;
using BP.Sys;
using BP.Sys.XML;
using BP.DA;
using BP.Web;
using BP.Web.Controls;
using BP;

namespace CCFlow.WF
{
    public partial class Pub : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #region 方法
        public bool IsReadonly
        {
            get
            {
                return (bool)this.ViewState["IsReadonly"];
            }
            set
            {
                ViewState["IsReadonly"] = value;
            }
        }

        public bool IsShowDtl
        {
            get
            {
                return (bool)this.ViewState["IsShowDtl"];
            }
            set
            {
                ViewState["IsShowDtl"] = value;
            }
        }

        public void SetValByKey(string key, string val)
        {
            TB en = (TB)this.FindControl("TB_" + key);
            if (en != null)
            {
                en.Text = val;
            }
        }

        public object GetValByKey(string key)
        {
            TB en = (TB)this.FindControl("TB_" + key);
            if (en == null) return null;
            return en.Text;
        }
        #endregion

        public void BindAttrs(Attrs attrs)
        {
            //this.HisEn =en;
            bool isReadonly = false;
            this.IsReadonly = false;
            this.IsShowDtl = false;
            this.Attributes["visibility"] = "hidden";
            this.Add("<table width='100%' id='a1' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.Key == "MyNum")
                    continue;

                if (isLeft)
                {
                    isAddTR = true;
                    this.AddTR();
                }

                val = attr.DefaultVal;
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.SetValByKey(attr.Key, val.ToString());
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;
                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {
                            //不可见控件
                            TB tb = new TB();
                            tb.LoadMapAttr(attr);
                            tb.ID = "TB_" + attr.Key;
                            tb.Attributes["Visible"] = "false";
                            this.Controls.Add(tb);
                            continue;
                        }
                        else
                        {
                            if (attr.UIHeight != 0)
                            {
                                TB area = new TB();
                                area.LoadMapAttr(attr);
                                area.ID = "TB_" + attr.Key;
                                area.Text = val.ToString();
                                area.Rows = 8;
                                area.TextMode = TextBoxMode.MultiLine;
                                area.Attributes["onchange"] += "Change('TB_" + attr.Key + "');";
                                if (isReadonly)
                                    area.Enabled = false;
                                this.AddContral(attr.Desc, area);
                            }
                            else
                            {
                                TB tb = new TB();
                                tb.LoadMapAttr(attr);

                                tb.ID = "TB_" + attr.Key;
                                if (isReadonly)
                                    tb.Enabled = false;
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        break;
                                }
                                tb.Attributes["width"] = "100%";
                                tb.Attributes["onchange"] += "Change('TB_" + attr.Key + "');";
                                this.AddContral(attr.Desc, tb);
                            }
                        }
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    CheckBox cb = new CheckBox();
                    if (attr.DefaultVal.ToString() == "1")
                        cb.Checked = true;
                    else
                        cb.Checked = false;

                    if (isReadonly)
                        cb.Enabled = false;
                    else
                        cb.Enabled = attr.UIVisible;

                    cb.ID = "CB_" + attr.Key;
                    cb.Attributes["onmousedown"] = "Change('CB_" + attr.Key + "')";
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    if (isReadonly || !attr.UIIsReadonly)
                    {
                        /* 如果是 DDLIsEnable 的, 就要找到. */
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            /* 如果是 enum 类型 */
                            int enumKey = 0;
                            try
                            {
                                enumKey = int.Parse(val.ToString());
                            }
                            catch
                            {
                                throw new Exception("默认值错误：" + attr.Key + " = " + val.ToString());
                            }

                            BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);

                            //DDL ddl = new DDL(attr,text,en.Lab,false);
                            DDL ddl = new DDL();
                            ddl.Items.Add(new ListItem(enEnum.Lab, val.ToString()));
                            ddl.Items[0].Selected = true;
                            ddl.Enabled = false;
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddContral(attr.Desc, ddl, true);
                        }
                        else
                        {
                            /* 如果是 ens 类型 */
                            Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                            Entity en1 = ens.GetNewEntity;
                            en1.SetValByKey(attr.UIRefKeyValue, val.ToString());
                            string lab = "";
                            try
                            {
                                en1.Retrieve();
                                lab = en1.GetValStringByKey(attr.UIRefKeyText);
                            }
                            catch
                            {
                                if (SystemConfig.IsDebug == false)
                                {
                                    lab = "" + val.ToString();
                                }
                                else
                                {
                                    lab = "" + val.ToString();
                                }
                            }

                            DDL ddl = new DDL(attr, val.ToString(), lab, false,
                                this.Page.Request.ApplicationPath);

                            ddl.ID = "DDL_" + attr.Key;
                            this.AddContral(attr.Desc, ddl, true);
                        }
                    }
                    else
                    {
                        /* 可以使用的情况. */
                        DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                        ddl1.ID = "DDL_" + attr.Key;
                        ddl1.Attributes["onchange"] = "Change('DDL_" + attr.Key + "')";
                        this.AddContral(attr.Desc, ddl1, true);
                    }
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    //					Sys.SysEnums enums = new BP.Sys.SysEnums(attr.UIBindKey); 
                    //					foreach(SysEnum en in enums)
                    //					{
                    //						return ;
                    //					}
                }

                if (isLeft == false)
                {
                    isAddTR = false;
                    this.AddTREnd();
                }

                isLeft = !isLeft;
            } // 结束循环.
            //补充TR
            if (isAddTR == true)
            {
                this.AddTD("");
                this.AddTD("");
                this.AddTREnd();
            }
            this.Add("</TABLE>");
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="attrs"></param>
        /// <param name="pa"></param>
        public void BindAttrs(Attrs attrs, AtPara pa)
        {
            bool isReadonly = false;
            this.IsReadonly = false;
            this.IsShowDtl = false;
            this.Attributes["visibility"] = "hidden";
            this.Add("<table width='100%' id='a1' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.Key == "MyNum")
                    continue;

                if (isLeft)
                {
                    isAddTR = true;
                    this.AddTR();
                }

                val = pa.GetValStrByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.SetValByKey(attr.Key, val.ToString());
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;
                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {

                            TB tb = new TB();
                            tb.LoadMapAttr(attr);
                            tb.ID = "TB_" + attr.Key;
                            tb.Attributes["Visible"] = "false";
                            this.Controls.Add(tb);
                            continue;
                        }
                        else
                        {
                            if (attr.UIHeight != 0)
                            {
                                TB area = new TB();
                                area.LoadMapAttr(attr);
                                area.ID = "TB_" + attr.Key;
                                area.Text = val.ToString();
                                area.Rows = 8;
                                area.TextMode = TextBoxMode.MultiLine;
                                area.Attributes["onchange"] += "Change('TB_" + attr.Key + "');";
                                if (isReadonly)
                                    area.Enabled = false;
                                this.AddContral(attr.Desc, area);
                            }
                            else
                            {
                                TB tb = new TB();
                                tb.LoadMapAttr(attr);

                                tb.ID = "TB_" + attr.Key;
                                if (isReadonly)
                                    tb.Enabled = false;
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        break;
                                }
                                tb.Attributes["width"] = "100%";
                                tb.Attributes["onchange"] += "Change('TB_" + attr.Key + "');";
                                this.AddContral(attr.Desc, tb);
                            }
                        }
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    val = pa.GetValBoolenByKey(attr.Key);
                    CheckBox cb = new CheckBox();
                    cb.Checked = (bool)val;

                    if (isReadonly)
                        cb.Enabled = false;
                    else
                    {
                        cb.Enabled = attr.UIVisible;
                        cb.Attributes["onmousedown"] = "Change('CB_" + attr.Key + "')";
                    }

                    cb.ID = "CB_" + attr.Key;
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    if (isReadonly || !attr.UIIsReadonly)
                    {
                        /* 如果是 DDLIsEnable 的, 就要找到. */
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            /* 如果是 enum 类型 */
                            int enumKey = 0;
                            try
                            {
                                enumKey = int.Parse(val.ToString());
                            }
                            catch
                            {
                                throw new Exception("默认值错误：" + attr.Key + " = " + val.ToString());
                            }

                            BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);

                            //DDL ddl = new DDL(attr,text,en.Lab,false);
                            DDL ddl = new DDL();
                            ddl.Items.Add(new ListItem(enEnum.Lab, val.ToString()));
                            ddl.Items[0].Selected = true;
                            ddl.Enabled = false;
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddContral(attr.Desc, ddl, true);
                        }
                        else
                        {
                            /* 如果是 ens 类型 */
                            Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                            Entity en1 = ens.GetNewEntity;
                            en1.SetValByKey(attr.UIRefKeyValue, val.ToString());
                            string lab = "";
                            try
                            {
                                en1.Retrieve();
                                lab = en1.GetValStringByKey(attr.UIRefKeyText);
                            }
                            catch
                            {
                                if (SystemConfig.IsDebug == false)
                                {
                                    lab = "" + val.ToString();
                                }
                                else
                                {
                                    lab = "" + val.ToString();
                                }
                            }

                            DDL ddl = new DDL(attr, val.ToString(), lab, false,
                                this.Page.Request.ApplicationPath);

                            ddl.ID = "DDL_" + attr.Key;
                            this.AddContral(attr.Desc, ddl, true);
                        }
                    }
                    else
                    {
                        /* 可以使用的情况. */
                        DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                        ddl1.ID = "DDL_" + attr.Key;
                        ddl1.Attributes["onchange"] = "Change('DDL_" + attr.Key + "')";
                        this.AddContral(attr.Desc, ddl1, true);
                    }
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    //					Sys.SysEnums enums = new BP.Sys.SysEnums(attr.UIBindKey); 
                    //					foreach(SysEnum en in enums)
                    //					{
                    //						return ;
                    //					}
                }

                if (isLeft == false)
                {
                    isAddTR = false;
                    this.AddTREnd();
                }

                isLeft = !isLeft;
            } // 结束循环.
            //补充TR
            if (isAddTR == true)
            {
                this.AddTD("");
                this.AddTD("");
                this.AddTREnd();
            }
            this.Add("</TABLE>");
        }

        /// <summary>
        /// 绑定数据返回只读，不包含TB控件
        /// </summary>
        /// <param name="attrs"></param>
        /// <param name="pa"></param>
        public void BindAttrsForHtml(Attrs attrs, AtPara pa)
        {
            this.Add("<table width='100%' id='a1' border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.Key == "MyNum")
                    continue;

                if (isLeft)
                {
                    isAddTR = true;
                    this.AddTR();
                }

                val = pa.GetValStrByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        //this.SetValByKey(attr.Key, val.ToString());
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* 如果是多值的.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;
                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {
                            continue;
                        }
                        else
                        {
                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                    val = decimal.Parse(val.ToString()).ToString("0.00");
                                    break;
                                default:
                                    val = val.ToString();
                                    break;
                            }

                            this.Add("<td class='FDesc' nowrap width=1% > " + attr.Desc + "</td>");
                            this.Add("<td class='TD' nowrap width='30%'>");
                            this.Add(val.ToString());
                            this.Add("</td>");
                        }
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    CheckBox cb = new CheckBox();
                    if (attr.DefaultVal.ToString() == "1")
                        cb.Checked = true;
                    else
                        cb.Checked = false;
                                        
                    cb.ID = "CB_RD_" + attr.Key;
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    /* 如果是 DDLIsEnable 的, 就要找到. */
                    if (attr.MyFieldType == FieldType.Enum)
                    {
                        /* 如果是 enum 类型 */
                        int enumKey = 0;
                        try
                        {
                            enumKey = int.Parse(val.ToString());
                        }
                        catch
                        {
                            throw new Exception("默认值错误：" + attr.Key + " = " + val.ToString());
                        }

                        BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);

                        this.Add("<td class='FDesc' nowrap width=1% > " + attr.Desc + "</td>");
                        this.Add("<td class='TD' nowrap width='30%'>");
                        this.Add(enEnum.Lab);
                        this.Add("</td>");
                    }
                    else
                    {
                        /* 如果是 ens 类型 */
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        Entity en1 = ens.GetNewEntity;
                        en1.SetValByKey(attr.UIRefKeyValue, val.ToString());
                        string lab = "";
                        try
                        {
                            en1.Retrieve();
                            lab = en1.GetValStringByKey(attr.UIRefKeyText);
                        }
                        catch
                        {
                            if (SystemConfig.IsDebug == false)
                            {
                                lab = "" + val.ToString();
                            }
                            else
                            {
                                lab = "" + val.ToString();
                            }
                        }

                        this.Add("<td class='FDesc' nowrap width=1% > " + attr.Desc + "</td>");
                        this.Add("<td class='TD' nowrap width='30%'>");
                        this.Add(lab);
                        this.Add("</td>");
                    }
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {
                    //					Sys.SysEnums enums = new BP.Sys.SysEnums(attr.UIBindKey); 
                    //					foreach(SysEnum en in enums)
                    //					{
                    //						return ;
                    //					}
                }

                if (isLeft == false)
                {
                    isAddTR = false;
                    this.AddTREnd();
                }

                isLeft = !isLeft;
            } // 结束循环.
            //补充TR
            if (isAddTR == true)
            {
                this.AddTD("");
                this.AddTD("");
                this.AddTREnd();
            }
            this.Add("</TABLE>");
        }

        public void AddContral(string desc, CheckBox cb)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td class='TD' nowrap >"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }

        /// <summary>
        /// 增加空件
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="tb"></param>
        public void AddContral(string desc, TB tb)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            tb.Attributes["style"] = "width=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td>");

            this.Add("<td class='TD' nowrap width='30%'>");
            this.Add(tb);
            this.Add("</td>");
        }

        public void AddContralDoc(string desc, TB tb)
        {
            this.Add("<td colspan='2' width='500px' >" + desc + "<br>");
            if (tb.ReadOnly)
                tb.Attributes["Class"] = "TBReadonly";
            this.Add(tb);
            this.Add("</td>");
        }
        public void AddContralDoc(string desc, TB tb, int colspanOfctl)
        {
            this.Add("<td  colspan='" + colspanOfctl + "' width='100%'>" + desc + "<br>");
            tb.Columns = 0;
            tb.CssClass = "TBDoc";
            this.Add(tb);
            this.Add("</td>");
        }
    }
}
