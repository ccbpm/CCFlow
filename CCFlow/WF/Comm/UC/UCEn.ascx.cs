//===========================================================================
// ���ļ�����Ϊ ASP.NET 2.0 Web ��Ŀת����һ�����޸ĵġ�
// �����Ѹ��ģ��������޸�Ϊ���ļ���App_Code\Migrated\comm\uc\Stub_ucen_ascx_cs.cs���ĳ������ 
// �̳С�
// ������ʱ�������������� Web Ӧ�ó����е�������ʹ�øó������󶨺ͷ��� 
// ��������ҳ��
// ����������ҳ��comm\uc\ucen.ascx��Ҳ���޸ģ��������µ�������
// �йش˴���ģʽ�ĸ�����Ϣ����ο� http://go.microsoft.com/fwlink/?LinkId=46995
//===========================================================================
using System.Linq;

namespace CCFlow.Web.Comm.UC
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Web.UI;
    using BP.En;
    using BP.Sys;
    using BP.Sys.XML;
    using BP.DA;
    using BP.Web;
    using BP.Web.Controls;
    using BP;
    using System.Collections.Generic;
    /// <summary>
    ///	UCEn ��ժҪ˵����
    /// </summary>
    public partial class UCEn : BP.Web.UC.UCBase3
    {
        public static string GetRefstrs(string keys, Entity en, Entities hisens)
        {
            return "";

            string refstrs = "";
            string path = System.Web.HttpContext.Current.Request.ApplicationPath;
            int i = 0;

            #region ����һ�Զ��ʵ��༭
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    continue;

                    //  string url = path + "/Comm/UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    string url = "./Comm/UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    try
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                        }
                        catch
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
                        throw ex;
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinShowModalDialog('" + url + "','onVsM'); \"  >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href=\"javascript:WinShowModalDialog('" + url + "','onVsM'); \"  >" + vsM.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            #region �������ŵ� ����
            RefMethods myreffuncs = en.EnMap.HisRefMethods;
            if (myreffuncs.Count > 0)
            {
                foreach (RefMethod func in myreffuncs)
                {
                    if (func.Visable == false)
                        continue;
                    if (func.IsCanBatch == true)
                        continue;

                    // string url = path + "/Comm/RefMethod.htm?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                    string url = "./Comm/RefMethod.htm?Index=" + func.Index + "&EnsName=" + hisens.ToString() + keys;
                    if (func.Warning == null)
                    {
                        if (func.Target == null)
                            refstrs += "[" + func.GetIcon(path) + "<a href='" + url + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                        else
                            refstrs += "[" + func.GetIcon(path) + "<a href=\"javascript:WinOpen('" + url + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                    }
                    else
                    {
                        if (func.Target == null)
                            refstrs += "[" + func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { window.location.href='" + url + "' }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                        else
                            refstrs += "[" + func.GetIcon(path) + "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { WinOpen('" + url + "','" + func.Target + "') }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>]";
                    }
                }
            }
            #endregion

            #region ����������ϸ
            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    //string url = path + "/Comm/UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&Key=" + enDtl.RefKey + "&Val=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;
                    string url = path + "/Comm/UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString();
                    try
                    {
                        try
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                        }
                        catch
                        {
                            i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                        }
                    }
                    catch (Exception ex)
                    {
                        enDtl.Ens.GetNewEntity.CheckPhysicsTable();
                        throw ex;
                    }

                    if (i == 0)
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "', 'dtl" + enDtl.RefKey + "'); \" >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a href=\"javascript:WinOpen('" + url + "', 'dtl" + enDtl.RefKey + "'); \"  >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion
            return refstrs;
        }

        public UCEn()
        {
        }

        public void AddContral()
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% ></td>"));
            this.Controls.Add(new LiteralControl("<td class='TD' nowrap ></TD>"));
        }
        public void AddContral(string desc, CheckBox cb)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td class='TD' nowrap >"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }
        public void AddContral(string desc, CheckBox cb, int colspan)
        {
            this.Controls.Add(new LiteralControl("<td class='FDesc' nowrap width=1% > " + desc + "</td>"));
            this.Controls.Add(new LiteralControl("<td class='TD' nowrap colspan='" + colspan + "'>"));
            this.Controls.Add(cb);
            this.Controls.Add(new LiteralControl("</td>"));
        }
        //		public void AddContral(string desc, string val)
        public void AddContral(string desc, string val)
        {
            this.Add("<TD class='FDesc' > " + desc + "</TD>");
            this.Add("<TD class='TD' > " + val + "</TD>");
        }
        public void AddContral(string desc, TB tb, string helpScript)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            //if (tb.ReadOnly == false)
            //    desc += "<font color=red><b>*</b></font>";

            // tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            tb.Attributes["Width"] = "100%";

            this.Add("<td class='FDesc' nowrap width=1% >" + desc + "</td>");
            this.Add("<td class='TD' nowrap  >" + helpScript);
            this.Add(tb);
            this.AddTDEnd(); // ("</td>");
        }
        public void AddContral(string desc, TB tb, string helpScript, int colspan)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            //if (tb.ReadOnly == false)
            //    desc += "<font color=red><b>*</b></font>";

            //  tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb);
                return;
            }

            //   tb.Attributes["Width"] = "30%";

            this.Add("<td class='FDesc' nowrap width=1% >" + desc + "</td>");

            if (colspan < 3)
            {
                this.Add("<td class='TD' nowrap colspan=" + colspan + " width='30%' >" + helpScript);
            }
            else
            {
                this.Add("<td class='TD' nowrap colspan=" + colspan + " width='80%' >" + helpScript);
            }

            this.Add(tb);
            this.AddTDEnd(); // ("</td>");
        }
        public void AddContral(string desc, TB tb, int colSpanOfCtl)
        {
            if (tb.ReadOnly)
            {
                if (tb.Attributes["Class"] == "TBNum")
                    tb.Attributes["Class"] = "TBNumReadonly";
                else
                    tb.Attributes["Class"] = "TBReadonly";
            }

            //if (tb.ReadOnly == false)
            //    desc += "<font color=red><b>*</b></font>";


            //  tb.Attributes["style"] = "width=100%;height=100%";
            if (tb.TextMode == TextBoxMode.MultiLine)
            {
                AddContralDoc(desc, tb, colSpanOfCtl);
                return;
            }

            this.Add("<td class='FDesc' nowrap width=1% > " + desc + "</td>");

            if (colSpanOfCtl < 3)
                this.Add("<td class='TD' nowrap colspan=" + colSpanOfCtl + " width='30%' >");
            else
                this.Add("<td class='TD' nowrap colspan=" + colSpanOfCtl + " width='80%' >");

            this.Add(tb);
            this.AddTDEnd();
        }
        /// <summary>
        /// ���ӿռ�
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
            this.AddTDEnd(); // ("</td>");
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


        #region ����
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
            TB tb = new TB();
            tb.ID = "TB_" + key;
            tb.Text = val;
            tb.Visible = false;
            this.Controls.Add(tb);
        }
        public object GetValByKey(string key)
        {
            TB en = (TB)this.FindControl("TB_" + key);
            return en.Text;
        }
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

                if (isLeft && isAddTR)
                {
                    this.AddTR();
                }

                isAddTR = true;
                val = attr.DefaultVal;
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.SetValByKey(attr.Key, val.ToString());
                        isAddTR = false;
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* ����Ƕ�ֵ��.*/
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
                            //this.AddContral(attr.Desc,area);
                            //this.SetValByKey(attr.Key, val.ToString() );
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
                                    case DataType.AppDate:
                                        tb.Text =DataType.CurrentData;
                                        break;
                                    case DataType.AppDateTime:
                                        tb.Text = DataType.CurrentDataTime;
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        break;
                                }
                                tb.Attributes["width"] = "100%";
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
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    if (isReadonly || !attr.UIIsReadonly)
                    {
                        /* ����� DDLIsEnable ��, ��Ҫ�ҵ�. */
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            /* ����� enum ���� */
                            int enumKey = 0;
                            try
                            {
                                enumKey = int.Parse(val.ToString());
                            }
                            catch
                            {
                                throw new Exception("Ĭ��ֵ����" + attr.Key + " = " + val.ToString());
                            }

                            BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);

                            //DDL ddl = new DDL(attr,text,en.Lab,false);
                            DDL ddl = new DDL();
                            ddl.Items.Add(new ListItem(enEnum.Lab, val.ToString()));
                            ddl.Items[0].Selected = true;
                            ddl.Enabled = false;
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddContral(attr.Desc, ddl, true);
                            //this.Controls.Add(ddl);
                        }
                        else
                        {
                            /* ����� ens ���� */
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
                                    //lab="û�й�����ֵ"+val.ToString()+"Class="+attr.UIBindKey+"EX="+ex.Message;
                                }
                            }

                            DDL ddl = new DDL(attr, val.ToString(), lab, false,
                                this.Page.Request.ApplicationPath);

                            ddl.ID = "DDL_" + attr.Key;
                            this.AddContral(attr.Desc, ddl, true);
                            //this.Controls.Add(ddl);
                        }
                    }
                    else
                    {
                        /* ����ʹ�õ����. */
                        DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                        ddl1.ID = "DDL_" + attr.Key;
                        this.AddContral(attr.Desc, ddl1, true);
                        //	this.Controls.Add(ddl1);
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
                    this.AddTREnd();

                isLeft = !isLeft;
            } // ����ѭ��.

            this.Add("</TABLE>");
        }
        //		public void BindReadonly(Entity en )
        public void BindReadonly(Entity en)
        {
            this.HisEn = en;
            //this.IsReadonly = isReadonly;
            //this.IsShowDtl = isShowDtl;
            this.Attributes["visibility"] = "hidden";
            this.Controls.Clear();
            this.AddTable(); //("<table   width='100%' id='AutoNumber1'  border='1' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            bool isLeft = true;
            object val = null;
            bool isAddTR = true;
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (isLeft && isAddTR)
                {
                    this.Add("<tr>");
                }
                isAddTR = true;
                val = en.GetValByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        this.AddContral(attr.Desc, val.ToString().ToString());
                        isAddTR = false;
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* ����Ƕ�ֵ��.*/
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
                            this.SetValByKey(attr.Key, val.ToString());
                            continue;
                        }
                        else
                        {

                            if (attr.UIHeight != 0)
                            {
                                this.AddContral(attr.Desc, val.ToString());
                            }
                            else
                            {

                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        //this.AddContral(attr.Desc, val.ToString().ToString("0.00")  );
                                        break;
                                    default:
                                        this.AddContral(attr.Desc, val.ToString());
                                        break;
                                }
                            }
                        }

                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    if (en.GetValBooleanByKey(attr.Key))
                        this.AddContral(attr.Desc, "��");
                    else
                        this.AddContral(attr.Desc, "��");
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    this.AddContral(attr.Desc, val.ToString());
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
                    this.AddTREnd();

                isLeft = !isLeft;
            } // ����ѭ��.

            this.Add("</TABLE>");



            if (en.IsExit(en.PK, en.PKVal) == false)
                return;

            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
                refstrs += "<hr>";
            this.Add(refstrs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        /// <param name="isReadonly"></param>
        /// <param name="isShowDtl"></param>
        //		public void Bind3Item(Entity en, bool isReadonly, bool isShowDtl)
        public void Bind3Item(Entity en, bool isReadonly, bool isShowDtl)
        {
            AttrDescs ads = new AttrDescs(en.ToString());
            this.HisEn = en;
            this.IsReadonly = isReadonly;
            this.IsShowDtl = isShowDtl;
            this.Controls.Clear();
            this.Attributes["visibility"] = "hidden";
            this.Controls.Clear();
            this.Add("<table   width='100%' id='AutoNumber1'  border='0' cellpadding='0' cellspacing='0' style='border-collapse: collapse' bordercolor='#111111' >");
            object val = null;
            Attrs attrs = en.EnMap.Attrs;
            foreach (Attr attr in attrs)
            {

                if (attr.Key == "MyNum")
                    continue;

                val = en.GetValByKey(attr.Key);
                if (attr.UIContralType == UIContralType.TB)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        continue;
                    }
                    else if (attr.MyFieldType == FieldType.MultiValues)
                    {
                        /* ����Ƕ�ֵ��.*/
                        LB lb = new LB(attr);
                        lb.Visible = true;

                        lb.Height = 128;
                        lb.SelectionMode = ListSelectionMode.Multiple;
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        ens.RetrieveAll();
                        this.AddTR();
                        this.Controls.Add(lb);
                    }
                    else
                    {
                        if (attr.UIVisible == false)
                        {
                            this.SetValByKey(attr.Key, val.ToString());
                            continue;
                        }
                        else
                        {
                            if (attr.UIHeight != 0)
                            {
                                /* doc �ı����͡���*/
                                TB area = new TB();
                                area.LoadMapAttr(attr);
                                area.ID = "TB_" + attr.Key;
                                area.Text = val.ToString();
                                area.Rows = 8;
                                area.Columns = 30;

                                area.TextMode = TextBoxMode.MultiLine;
                                area.Attributes["height"] = "100px";
                                //area.Attributes["width"]="100px";
                                area.IsHelpKey = false;

                                area.Attributes.Add("class", "TextArea1");

                                if (isReadonly)
                                    area.Enabled = false;

                                this.AddTR();
                                this.Add("<TD colspan=3 class='FDesc' >" + attr.Desc + "</TD>");
                                this.AddTREnd();

                                this.AddTR();
                                this.Add("<TD colspan=3 class='TD' height='250' >");
                                this.Add(area);
                                this.Add("</TD>");
                                this.AddTREnd();
                                continue;
                            }
                            else
                            {
                                TB tb = new TB();
                                tb.ID = "TB_" + attr.Key;
                                tb.IsHelpKey = false;

                                if (isReadonly || attr.UIIsReadonly)
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
                                this.AddTR();
                                this.AddContral(attr.Desc, tb);

                                /*
                                AttrDesc ad = ads.GetEnByKey(AttrDescAttr.Attr,  attr.Key ) as AttrDesc;
                                if (ad!=null)
                                    this.AddContral(attr.Desc,tb);
                                else
                                {
                                    //this.AddContral(attr.Desc,tb);

                                    tb.Attributes["width"]="";

                                    //this.AddTR();
                                    this.Add("<TD class='FDesc' width='1%' >"+attr.Desc+"</TD>");
                                    this.Add("<TD class='TD' colspan=2 >");
                                    this.Add(tb);
                                    this.Add("</TD>");
                                    this.AddTREnd();
                                    continue;
                                }
                                */

                            }
                        }
                    }
                }
                else if (attr.UIContralType == UIContralType.CheckBok)
                {
                    CheckBox cb = new CheckBox();
                    cb.Checked = en.GetValBooleanByKey(attr.Key);

                    if (isReadonly || !attr.UIIsReadonly)
                        cb.Enabled = false;
                    else
                        cb.Enabled = attr.UIVisible;


                    cb.ID = "CB_" + attr.Key;
                    this.AddTR();
                    this.AddContral(attr.Desc, cb);
                }
                else if (attr.UIContralType == UIContralType.DDL)
                {
                    if (isReadonly || !attr.UIIsReadonly)
                    {
                        /* ����� DDLIsEnable ��, ��Ҫ�ҵ�. */
                        if (attr.MyFieldType == FieldType.Enum)
                        {
                            /* ����� enum ���� */
                            int enumKey = int.Parse(val.ToString());
                            BP.Sys.SysEnum enEnum = new BP.Sys.SysEnum(attr.UIBindKey, "CH", enumKey);

                            //DDL ddl = new DDL(attr,text,en.Lab,false);
                            DDL ddl = new DDL();
                            ddl.Items.Add(new ListItem(enEnum.Lab, val.ToString()));
                            ddl.Items[0].Selected = true;
                            ddl.Enabled = false;
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddTR();
                            this.AddContral(attr.Desc, ddl, false);
                            //this.Controls.Add(ddl);
                        }
                        else
                        {
                            /* ����� ens ���� */
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
                                    //lab="û�й�����ֵ"+val.ToString()+"Class="+attr.UIBindKey+"EX="+ex.Message;
                                }
                            }

                            DDL ddl = new DDL(attr, val.ToString(), lab, false, this.Page.Request.ApplicationPath);
                            ddl.ID = "DDL_" + attr.Key;

                            this.AddTR();
                            this.AddContral(attr.Desc, ddl, false);
                            //this.Controls.Add(ddl);
                        }
                    }
                    else
                    {
                        /* ����ʹ�õ����. */
                        DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                        ddl1.ID = "DDL_" + attr.Key;
                        //ddl1.SelfBindKey = ens.ToString();
                        //ddl1.SelfEnsRefKey = attr.UIRefKeyValue;
                        //ddl1.SelfEnsRefKeyText = attr.UIRefKeyText;

                        this.AddTR();
                        this.AddContral(attr.Desc, ddl1, true);
                    }
                }
                else if (attr.UIContralType == UIContralType.RadioBtn)
                {

                }

                AttrDesc ad1 = ads.GetEnByKey(AttrDescAttr.Attr, attr.Key) as AttrDesc;
                if (ad1 == null)
                    this.AddTD("class='Note'", "&nbsp;");
                else
                    this.AddTD("class='Note'", ad1.Desc);

                this.AddTREnd();
            } //����ѭ��.

            #region �鿴�Ƿ���� MyFile�ֶ�����о���Ϊ�Ǹ�����
            if (en.EnMap.Attrs.Contains("MyFileName"))
            {
                /* �������������ֶΡ�*/
                string fileName = en.GetValStringByKey("MyFileName");
                string filePath = en.GetValStringByKey("MyFilePath");
                string fileExt = en.GetValStringByKey("MyFileExt");

                string url = "";
                if (fileExt != "")
                {
                    // ϵͳ����·����
                    string path = this.Request.PhysicalApplicationPath.ToLower();
                    string path1 = filePath.ToLower();
                    path1 = path1.Replace(path, "");
                    url = "&nbsp;&nbsp;<a href='../" + path1 + "/" + en.PKVal + "." + fileExt + "' target=_blank ><img src='../../Img/FileType/" + fileExt + ".gif' border=0 />" + fileName + "</a>";
                }

                this.AddTR();
                this.AddTD("align=right nowrap=true class='FDesc'", "������ͼƬ:");
                HtmlInputFile file = new HtmlInputFile();
                file.ID = "file";
                file.Attributes.Add("style", "width:60%");
                this.Add("<TD colspan=2  class='FDesc' >");
                this.Add(file);
                this.Add(url);
                if (fileExt != "")
                {
                    Button btn1 = new Button();
                    btn1.Text = "�Ƴ�";
                    btn1.ID = "Btn_DelFile";
                    btn1.CssClass = "Btn";
                    btn1.Attributes.Add("class", "Btn1");

                    btn1.Attributes["onclick"] += " return confirm('�˲���Ҫִ���Ƴ�������ͼƬ���Ƿ������');";
                    this.Add(btn1);
                }
                this.Add("</TD>");

                this.AddTREnd();
            }
            #endregion

            #region save button .
            this.AddTR();
            this.Add("<TD align=center colspan=3 >");


            Button btn = new Button();
            if (en.HisUAC.IsInsert)
            {
                btn = new Button();
                btn.ID = "Btn_New";
                btn.Text = "  �� ��  ";
                btn.CssClass = "Btn";
                btn.Attributes.Add("class", "Btn1");

                this.Add(btn);
                this.Add("&nbsp;");
            }

            if (en.HisUAC.IsUpdate)
            {
                btn = new Button();
                btn.ID = "Btn_Save";
                btn.CssClass = "Btn";
                btn.Text = "  ��  ��  ";
                btn.Attributes.Add("class", "Btn1");

                this.Add(btn);
                this.Add("&nbsp;");
            }


            if (en.HisUAC.IsDelete)
            {
                btn = new Button();
                btn.ID = "Btn_Del";
                btn.CssClass = "Btn";
                btn.Text = "  ɾ  ��  ";
                btn.Attributes.Add("class", "Btn1");

                btn.Attributes["onclick"] = " return confirm('��ȷ��Ҫִ��ɾ����');";
                this.Add(btn);
                this.Add("&nbsp;");
            }

            this.Add("&nbsp;<input class='Btn' type=button onclick='javascript:window.close()' value='  ��  ��  ' />");

            this.Add("</TD>");
            this.AddTREnd();
            #endregion

            this.AddTableEnd();

            if (isShowDtl == false)
                return;


            if (en.IsExit(en.PK, en.PKVal) == false)
                return;

            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }
            this.Add("<HR>");

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }
            Entities hisens = en.GetNewEntities;

            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
                refstrs += "<hr>";

            this.Add(refstrs);
        }
        private void btn_Click(object sender, EventArgs e)
        {
        }

        #region ��v2. ���ͳ��޸� 2014-10-14.
        /// <summary>
        /// �����н�����ԣ�ʹ��EasyUI������
        /// </summary>
        /// <param name="en"></param>
        /// <param name="enName"></param>
        /// <param name="isReadonly"></param>
        /// <param name="isShowDtl"></param>
        public void BindV2(Entity en, string enName, bool isReadonly, bool isShowDtl)
        {
            this.BindV2(en, enName, isReadonly, isShowDtl, null);
        }

        /// <summary>
        /// �����н�����ԣ�ʹ��EasyUI������
        /// </summary>
        /// <param name="en"></param>
        /// <param name="enName"></param>
        /// <param name="isReadonly"></param>
        /// <param name="isShowDtl"></param>
        /// <param name="noKey"></param>
        public void BindV2(Entity en, string enName, bool isReadonly, bool isShowDtl, string noKey)
        {
            if (enName == null)
                enName = en.ToString();

            this.HisEn = en;
            this.IsReadonly = isReadonly;
            this.IsShowDtl = isShowDtl;
            this.Controls.Clear();
            bool isLeft = true;
            object val = null;
            Map map = en.EnMap;
            Attrs attrs = map.Attrs;
            EnCfg cf = new EnCfg(enName);
            string[] gTitles = cf.GroupTitle.Split('@'); // ��ȡ�����ǩ.
            int idxL1, idxL2;
            string tabTitle;
            var idx = 0;
            var dictGroups = gTitles
                .Select(g => g.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                .Where(g => g.Length == 2)
                .ToDictionary(g => g[0], g => g[1]);
            var haveFile = en.EnMap.Attrs.Contains("MyFileName") || en.EnMap.HisAttrFiles.Count > 0 || en.EnMap.Attrs.Contains("MyFileNum");

            //����˴�û�з��飬��Ĭ������һ�����飬2015-8-4,added by liuxc
            if (dictGroups.Count == 0)
            {
                var firstAttr = attrs.Cast<Attr>().FirstOrDefault(a => a.Key != "MyNum");
                if (firstAttr != null)
                    dictGroups.Add(firstAttr.Key, map.EnDesc);
            }

            UIConfig cfg = new UIConfig(en);

            //���������ֶ��빦�ܵļ�.
            RefMethods rms = map.HisRefMethods;
            string rmKeys = "";
            foreach (RefMethod item in rms)
            {
                if (item.RefAttrKey == null)
                    continue;
                rmKeys += "," + item.RefAttrKey + ",";
            }

            //����tab������
            if (dictGroups.Count > 1)
            {
                this.Add("<div id='tab-tools'>");
                this.Add("<a href='javascript:void(0)' class='easyui-menubutton' data-options=\"plain:true,menu:'#tab-menu'\">ѡ</a>");
                this.Add("</div>");

                //����tab�����˵�
                this.Add("<div class='easyui-menu' id='tab-menu'>");

                foreach (var de in dictGroups)
                {
                    idxL1 = de.Value.IndexOf(',');
                    idxL2 = de.Value.IndexOf("��");

                    //ȥ����Ӣ��/���������ڵ����ݣ�����ֻ��ʾ����֮ǰ�����ݣ���ֹ�������
                    if (idxL1 > 0)
                        tabTitle = de.Value.Substring(0, idxL1);
                    else if (idxL2 > 0)
                        tabTitle = de.Value.Substring(0, idxL2);
                    else
                        tabTitle = de.Value;

                    this.Add(string.Format("<div onclick=\"{2}selectTab('{0}')\">{1}.{0}</div>", tabTitle, ++idx, Environment.NewLine));
                }

                if (haveFile)
                    this.Add("<div onclick=\"selectTab('��������')\">" + (++idx) + ".��������</div>");

                this.Add("</div>");
            }

            //Ϊ���������ʾ֮ǰ�򿪵ı�ǩ�������ӵ������򣬼�¼��ǰ�򿪵ı�ǩtitle
            //�ڱ����¼�����󣬻�ȡ���������м�¼�ı�ǩtitle,����title���뵽redirect��url������,��ҳ��ǰ�˵�JS�в���title��ѡ�иñ�ǩ
            //added by liuxc,2014-11-07
            var hiddenField = new HiddenField { ID = "Hid_CurrentTab" };
            this.Add(hiddenField);

            this.Add(
                "<div id='nav-tab' class='easyui-tabs' data-options=\"" + (dictGroups.Count > 1 ? "tools:'#tab-tools'," : string.Empty) + "fit:true,onSelect:function(title){ $('#" +
                hiddenField.ClientID + "').val(title); }\" style='width:740px;height:auto'>");

            // ѭ�������������.
            string[] garr = null;

            foreach (var g in dictGroups)
            {
                garr = g.Value.Split(',');
                this.Add(string.Format("<div title='{0}' data-g='{0}' data-gd='{1}'>", garr[0], garr.Length > 1 ? garr[1] : ""));
                this.AddTable("class='Table' cellpadding='0' cellspacing='0' style='width:100%;'");
                //this.Add("<caption>" + g.Value + "</caption>");
                isLeft = true;
                string currKey = null;

                foreach (Attr attr in attrs)
                {
                    if (attr.Key == "MyNum")
                        continue;

                    if (dictGroups.ContainsKey(attr.Key))
                    {
                        if (currKey == g.Key)
                            break;

                        currKey = attr.Key;

                        if (currKey != g.Key)
                            continue;
                    }
                    else
                    {
                        if (currKey != g.Key)
                            continue;
                    }

                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    val = en.GetValByKey(attr.Key);
                    
                    if (attr.UIVisible == false)
                    {
                        this.SetValByKey(attr.Key, val.ToString());
                        continue;
                    }

                    #region �ж��Ƿ�����ʾ
                    if (attr.UIIsLine)
                    {
                        if (isLeft == false)
                        {
                            this.AddTD();
                            this.AddTD();
                            this.AddTREnd();
                            isLeft = true;
                        }

                        this.AddTR();
                        if (attr.UIHeight != 0)
                        {
                            /*����ı��ɼ�, ���⴦��*/
                            if (val.ToString().Length == 0 && en.IsEmpty == false && attr.Key == "Doc")
                                val = en.GetValDocText();

                            /* doc �ı����͡���*/
                            if (attr.UIIsReadonly || isReadonly)
                            {
                                TB areaR = new TB();
                                areaR.ID = "TB_" + attr.Key;
                                areaR.Text = val.ToString();
                                areaR.Rows = 5;
                                areaR.TextMode = TextBoxMode.MultiLine;
                                areaR.ReadOnly = true;
                                this.AddContral(attr.DescHelper, areaR, 4);
                            }
                            else
                            {
                                EditerType type = cfg.EditerType;
                                switch (type)
                                {
                                    case EditerType.FKEditer:
                                    case EditerType.Sina:
                                        HtmlInputHidden input = new HtmlInputHidden();
                                        //  input.Attributes["id"] = "TBH_"+attr.Key;
                                        input.ID = "TBH_" + attr.Key;
                                        input.Value = val as string;
                                        this.Add("<td class='FDesc'  colspan='4' width='50%' >");
                                        this.Add(input);
                                        this.Add("<iframe ID='eWebEditor1' src='./Ctrl/editor/editor.htm?id=" + input.ClientID + "&style=coolblue' frameborder='0' scrolling='no' width='600' HEIGHT='350'></iframe>");
                                        this.AddTDEnd();
                                        break;
                                    default:
                                        TB area = new TB();
                                        area.LoadMapAttr(attr);
                                        area.ID = "TB_" + attr.Key;
                                        area.Text = val.ToString();
                                        area.Rows = 5;
                                        area.TextMode = TextBoxMode.MultiLine;
                                        area.IsHelpKey = true;
                                        this.AddContral(attr.DescHelper, area, 4);
                                        break;
                                }
                            }
                            this.AddTREnd();

                            isLeft = true;
                            continue;
                        }

                        switch (attr.UIContralType)
                        {
                            case UIContralType.TB:
                                TB tb = new TB();
                                tb.ID = "TB_" + attr.Key;
                                // tb.LoadMapAttr(attr);
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                        tb.Attributes["Class"] = "TBNum";
                                        tb.ShowType = TBType.Moneny;
                                        break;
                                    case DataType.AppInt:
                                    case DataType.AppFloat:
                                    case DataType.AppDouble:
                                        tb.Attributes["Class"] = "TBNum";
                                        tb.Text = val.ToString();
                                        tb.ShowType = TBType.Num;
                                        break;
                                    case DataType.AppDate:
                                        tb.Text = val.ToString();
                                        tb.ShowType = TBType.Date;
                                        tb.Attributes["Class"] = "TBcalendar";
                                        tb.IsHelpKey = true;
                                        if (attr.UIIsReadonly == false)
                                            tb.Attributes["onfocus"] = "WdatePicker();";
                                        break;
                                    case DataType.AppDateTime:
                                        tb.Text = val.ToString();
                                        tb.ShowType = TBType.Date;
                                        tb.Attributes["Class"] = "TBcalendar";
                                        tb.IsHelpKey = true;
                                        if (attr.UIIsReadonly == false)
                                            tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        tb.Attributes["Class"] = "TB";
                                        break;
                                }

                                if (isReadonly || attr.UIIsReadonly)
                                    tb.ReadOnly = true;

                                tb.Enabled = true;
                                if (attr.Key == noKey && attr.UIIsReadonly == false && attr.UIVisible == true)
                                {
                                    string path = this.Request.ApplicationPath;
                                    //string helpScript = "";
                                    string helpScript = "<a href=\"javascript:alert('�����ı�����˫����ϵͳ����ֲ��Ҵ��ڡ�');\" ><img src='./Img/Btn/Search.gif' border=0 /></a>";
                                    tb.Attributes["ondblclick"] = "OpenHelperTBNo('" + path + "','BP.Port.Taxpayers',this)";
                                    this.AddContral(attr.DescHelper, tb, helpScript, 3);
                                }
                                else
                                {
                                    tb.Columns = 80;
                                    this.AddContral(attr.DescHelper, tb, 3);
                                }
                                tb.Dispose();
                                break;
                            case UIContralType.DDL:
                                if (isReadonly || !attr.UIIsReadonly)
                                {
                                    DDL ddl = new DDL();
                                    ddl.ID = "DDL_" + attr.Key;
                                    //ddl.CssClass = "easyui-combobox";// "DDL" + WebUser.Style;
                                    string text = en.GetValRefTextByKey(attr.Key);
                                    if (text == null || text == "")
                                        text = val.ToString();

                                    ListItem li = new ListItem(text, val.ToString());
                                    li.Attributes["class"] = "TB";
                                    ddl.Items.Add(li);
                                    ddl.Enabled = false;
                                    this.AddContral(attr.DescHelper, ddl, true, 3);
                                }
                                else
                                {

                                    DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                                    ddl1.ID = "DDL_" + attr.Key;
                                    this.AddContral(attr.DescHelper, ddl1, true,3);

                                    /* ����ʹ�õ����. */
                                  //  DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                                    //DDL ddlEnum = new DDL();
                                    //ddlEnum.BindSysEnum(attr.UIBindKey, (int)val);
                                    //ddlEnum.ID = "DDL_" + attr.Key;
                                    //this.AddContral(attr.DescHelper, ddlEnum, true, 3);

                                }
                                break;
                            case UIContralType.CheckBok:
                                CheckBox cb = new CheckBox();
                                cb.Checked = en.GetValBooleanByKey(attr.Key);
                                cb.ID = "CB_" + attr.Key;

                                if (isReadonly)
                                    cb.Enabled = false;
                                else
                                    cb.Enabled = attr.UIIsReadonly;

                                cb.Text = attr.Desc;
                                //this.AddContral("", cb, 3);
                                if (attr.HelperUrl == null)
                                    this.AddContral("", cb, 3);
                                else
                                    this.AddContral(attr.DescHelperIcon, cb, 3);
                                break;
                            default:
                                break;
                        }
                        this.AddTREnd();
                        isLeft = true;
                        continue;
                    }
                    #endregion �ж��Ƿ�����ʾ // ����Ҫ��ʾ���е������

                    if (isLeft)
                        this.AddTR();

                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            if (attr.UIHeight != 0)
                            {
                                if (val.ToString().Length == 0 && en.IsEmpty == false && attr.Key == "Doc")
                                    val = en.GetValDocText();

                                EditerType type = cfg.EditerType;
                                switch (type)
                                {
                                    case EditerType.None:
                                        /* doc �ı����͡���*/
                                        if (attr.UIIsReadonly || isReadonly)
                                        {
                                            TB areaR = new TB();
                                            areaR.ID = "TB_" + attr.Key;
                                            areaR.Text = val.ToString();
                                            areaR.Rows = 8;
                                            areaR.TextMode = TextBoxMode.MultiLine;
                                            areaR.ReadOnly = true;
                                            this.AddContral(attr.DescHelper, areaR);
                                        }
                                        else
                                        {
                                            TB area = new TB();
                                            area.LoadMapAttr(attr);
                                            area.ID = "TB_" + attr.Key;
                                            area.Text = val.ToString();
                                            area.Rows = 8;
                                            area.TextMode = TextBoxMode.MultiLine;
                                            area.IsHelpKey = true;
                                            this.AddContral(attr.DescHelper, area);
                                        }
                                        break;
                                    case EditerType.Sina:
                                    case EditerType.FKEditer:
                                        /* doc �ı����͡���*/
                                        HtmlInputHidden input = new HtmlInputHidden();
                                        input.Attributes["id"] = "txtContent";
                                        //input.ID = attr.Key;
                                        this.Add("<td class='FDesc' colspan='2' nowrap width='50%' >" + attr.Desc + "<br>");
                                        this.Add(input);
                                        this.Add("<iframe ID='eWebEditor1' src='./Ctrl/Edit/editor.htm?id=txtContent&style=coolblue' frameborder='0' scrolling='no' width='600' HEIGHT='350'></iframe>");
                                        this.AddTDEnd();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                TB tb = new TB();
                                tb.ID = "TB_" + attr.Key;
                                switch (attr.MyDataType)
                                {
                                    case DataType.AppMoney:
                                        try
                                        {
                                            tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                            tb.Attributes["Class"] = "TBNum";
                                        }
                                        catch
                                        {
                                            tb.Text = decimal.Parse("0").ToString("0.00");
                                            tb.Attributes["Class"] = "TBNum";
                                            //  Log.DebugWriteWarning( "@"+en.ToString()+"Attr="+attr.Key=" Val="+val +" Ex="+ex.Message );
                                        }
                                        tb.ShowType = TBType.Moneny;
                                        break;
                                    case DataType.AppInt:
                                    case DataType.AppFloat:
                                    case DataType.AppDouble:
                                        tb.ShowType = TBType.Float;
                                        tb.CssClass = isReadonly || attr.UIIsReadonly ? "TBNumReadonly" : "TBNum";
                                        tb.Text = val.ToString();
                                        break;
                                    case DataType.AppDate:
                                        tb.Text = val.ToString();
                                        tb.ShowType = TBType.Date;
                                        tb.IsHelpKey = true;
                                        if (attr.UIIsReadonly == false)
                                            tb.Attributes["onfocus"] = "WdatePicker();";
                                        break;
                                    case DataType.AppDateTime:
                                        tb.Text = val.ToString();
                                        tb.ShowType = TBType.Date;
                                        tb.IsHelpKey = true;
                                        if (attr.UIIsReadonly == false)
                                            tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                        break;
                                    default:
                                        tb.Text = val.ToString();
                                        tb.Attributes["Class"] = "TB";
                                        break;
                                }

                                if (isReadonly || attr.UIIsReadonly)
                                {
                                    tb.ReadOnly = true;
                                }

                                if (attr.IsPK && DataType.IsNullOrEmpty(val.ToString()) == false)
                                {
                                    tb.ReadOnly = true;
                                }


                                if (attr.Key == noKey && attr.UIIsReadonly == false && attr.UIVisible == true)
                                {
                                    string path = this.Request.ApplicationPath;
                                    //string helpScript = "";
                                    string helpScript = "<a href=\"javascript:alert('�����ı�����˫����ϵͳ����ֲ��Ҵ��ڡ�');\" ><img src='./Img/Btn/Search.gif' border=0 /></a>";
                                    tb.Attributes["ondblclick"] = "OpenHelperTBNo('" + path + "','BP.Port.Taxpayers',this)";
                                    this.AddContral(attr.DescHelper, tb, helpScript);
                                }
                                else
                                {
                                    this.AddContral(attr.DescHelper, tb);
                                }
                            }
                            break;
                        case UIContralType.DDL:
                            if (isReadonly || !attr.UIIsReadonly)
                            {
                                DDL ddl = new DDL();
                                ddl.ID = "DDL_" + attr.Key;
                                ddl.CssClass = "DDL" + WebUser.Style;
                                string text = en.GetValRefTextByKey(attr.Key);
                                if (text == null || text == "")
                                    text = val.ToString();

                                ListItem li = new ListItem(text, val.ToString());
                                li.Attributes["class"] = "TB";
                                ddl.Items.Add(li);
                                ddl.Enabled = false;
                                this.AddContral(attr.DescHelper, ddl, true);
                            }
                            else
                            {
                                /* ����ʹ�õ����. */
                                if (attr.UIDDLShowType == DDLShowType.BindSQL)
                                {
                                    DDL ddlSQL =new DDL();
                                    ddlSQL.ID = "DDL_" + attr.Key;
                                    string sql = attr.UIBindKey;
                                    if (sql.Contains("@Web") == true)
                                    {
                                        sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                                        sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                                        sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                                        sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                                    }

                                    if (sql.Contains("@") == true)
                                    {
                                        foreach (Attr myattr in en.EnMap.Attrs)
                                        {
                                            sql = sql.Replace("@"+myattr.Key, en.GetValStrByKey(myattr.Key));
                                            if (sql.Contains("@") == false)
                                                break;
                                        }
                                    }

                                    //�������Դ,��ִ�а�.
                                    DataTable dt= BP.DA.DBAccess.RunSQLReturnTable(sql);
                                    ddlSQL.Bind(dt, "No", "Name", val.ToString());

                                    this.AddContral(attr.DescHelper, ddlSQL, true);
                                }
                                else
                                {
                                    DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                                    ddl1.ID = "DDL_" + attr.Key;
                                    this.AddContral(attr.DescHelper, ddl1, true);
                                }

                            }

                            break;
                        case UIContralType.CheckBok:
                            CheckBox cb = new CheckBox();
                            cb.Checked = en.GetValBooleanByKey(attr.Key);

                            if (isReadonly)
                                cb.Enabled = false;
                            else
                                cb.Enabled = attr.UIIsReadonly;

                            cb.ID = "CB_" + attr.Key;
                            cb.Text = attr.Desc;
                            if (attr.HelperUrl == null)
                                this.AddContral("", cb);
                            else
                                this.AddContral(attr.DescHelperIcon, cb);
                            break;
                        default:
                            break;
                    }

                    if (isLeft == false)
                        this.AddTREnd();
                    isLeft = !isLeft;

                    #region �����ֶ���ع���.
                    if (rmKeys.Contains("," + attr.Key + ",") == true)
                    {
                        #region �������Ӵ�.
                        string html = "";
                        foreach (RefMethod func in rms)
                        {
                            if (func.RefAttrKey != attr.Key)
                                continue;
                            func.HisEn = en;
                            if (func.RefMethodType == RefMethodType.Func)
                            {
                                /*����ǹ���.*/
                                string mykeys = "&" + en.PK + "=" + en.PKVal + "&r=" + DateTime.Now.ToString("MMddhhmmss");
                               // string url = "../RefMethod.aspx?Index=" + func.Index + "&EnsName=" + en.GetNewEntities.ToString() + mykeys;
                                string url = "../RefMethod.htm?Index=" + func.Index + "&EnsName=" + en.GetNewEntities.ToString() + mykeys;

                                if (func.Warning == null)
                                {
                                    if (func.Target == null)
                                        html = "<a href='" + url + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>";
                                    else
                                        html = "<a href=\"javascript:WinOpen('" + url + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>";
                                }
                                else
                                {
                                    if (func.Target == null)
                                        html = "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { window.location.href='" + url + "' }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>";
                                    else
                                        html = "<a href=\"javascript: if ( confirm('" + func.Warning + "') ) { WinOpen('" + url + "','" + func.Target + "') }\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>";
                                }
                            }
                            else
                            {
                                /* �����link.*/
                                string myurl = func.Do(null) as string;
                                int h = func.Height;

                                if (func.Target == null)
                                    html = "<a href='" + myurl + "' ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>";
                                else
                                    html = "<a href=\"javascript:WinOpen('" + myurl + "','" + func.Target + "')\" ToolTip='" + func.ToolTip + "' >" + func.Title + "</a>";
                            }


                        }
                        #endregion

                        if (isLeft == false)
                        {
                            //this.AddTDDesc("");
                            this.AddTD("colspan=2", html);
                            this.AddTREnd();
                        }
                        else
                        {
                            this.AddTR();
                            this.AddTDDesc("");
                            this.AddTD("colspan=3", html);
                            this.AddTREnd();
                        }

                        isLeft = true;

                    }
                    #endregion �����ֶ���ع���.

                }

                // ����ѭ��.
                if (isLeft == false)
                {
                    AddContral();
                    this.AddTREnd();
                }

                this.AddTableEnd();
                this.AddDivEnd();
            } // ��������ѭ��.

            if (haveFile)
            {
                this.Add(string.Format("<div title='{0}' data-g='{0}' data-gd='{1}'>", "��������", ""));
                this.AddTable("class='Table' cellpadding='0' cellspacing='0' style='width:100%;'");

                //�����������ڴˣ�2015-8-4��added by liuxc
                if (en.EnMap.Attrs.Contains("MyFileName"))
                {
                    string fileName = en.GetValStringByKey("MyFileName");
                    string filePath = en.GetValStringByKey("MyFilePath");
                    string fileExt = en.GetValStringByKey("MyFileExt");
                    string url = "";

                    if (fileExt == "")
                    {
                        /*����Ƿ����ֹ��ϴ��ĸ���*/
                        BP.Sys.Glo.InitEntityAthInfo(en);
                        fileExt = en.GetValStringByKey("MyFileExt");
                    }


                    if (fileExt != "")
                    {
                        // ϵͳ����·����
                        string path = this.Request.PhysicalApplicationPath.ToLower();
                        string path1 = filePath.ToLower();
                        path1 = path1.Replace(path, "");
                        // url = "&nbsp;&nbsp;<a href='" + cf.FJWebPath + "/" + en.PKVal + "." + fileExt + "' target=_blank ><img src='"+this.Request.ApplicationPath+"Images/FileType/" + fileExt + ".gif' border=0 />" + fileName + "</a>";
                        url = "&nbsp;&nbsp;<a href='../Do.aspx?DoType=DownFile&EnName=" + enName + "&PK=" + en.PKVal + "' target=_blank ><img src='./../../Img/FileType/" + fileExt + ".gif' border=0 />" + fileName + "</a>";
                    }
                   

                    this.AddTR();
                    this.AddTD("nowrap=true class='FDesc' ", en.EnMap.GetAttrByKey("MyFileName").Desc);

                    HtmlInputFile file = new HtmlInputFile();
                    file.ID = "file";
                    this.AddTD(file);

                    this.AddTDBegin("colspan=2");
                    this.Add(url);
                    if (fileExt != "")
                    {
                        System.Web.UI.WebControls.ImageButton btn1 = new System.Web.UI.WebControls.ImageButton();
                        btn1.ImageUrl = "../../Img/Btn/Delete.gif";

                        //  btn1.Text = "�Ƴ�";
                        btn1.Attributes.Add("class", "Btn1");
                        btn1.ID = "Btn_DelFile";
                        btn1.Attributes["onclick"] += " return confirm('�˲���Ҫִ���Ƴ��������Ƿ������');";
                        this.Add(btn1);
                    }

                    this.AddTDEnd();
                    this.AddTREnd();
                }

                #region �����Կؼ���
                AttrFiles fileAttrs = en.EnMap.HisAttrFiles;
                SysFileManagers sfs = new SysFileManagers(en.ToString(), en.PKVal.ToString());
                foreach (AttrFile attrF in fileAttrs)
                {
                    this.AddTR();
                    this.AddTD("nowrap=true class='FDesc' ", attrF.FileName);
                    HtmlInputFile file = new HtmlInputFile();
                    file.ID = "F" + attrF.FileNo;
                    file.Attributes["width"] = "100%";
                    this.AddTD(file);

                    /* �ж��Ƿ����ļ�û���ļ����Ƴ�����*/
                    SysFileManager sf = sfs.GetEntityByKey(SysFileManagerAttr.AttrFileNo, attrF.FileNo) as SysFileManager;
                    if (sf == null)
                    {
                        this.AddTD();
                        this.AddTD();
                    }
                    else
                    {
                        this.Add("<TD class=TD colspan=2>");
                        string lab = "&nbsp;<a href='" + cf.FJWebPath + "/" + sf.OID + "." + sf.MyFileExt + "' target=_blank ><img src='./../../Img/FileType/" + sf.MyFileExt + ".gif' border=0 />" + sf.MyFileName + "." + sf.MyFileExt + "</a>";
                        this.Add(lab);

                        System.Web.UI.WebControls.ImageButton btn_m = new System.Web.UI.WebControls.ImageButton();
                        btn_m.ImageUrl = "../../Img/Btn/Delete.gif";

                        //btn_m.ImageUrl = "./Img/Btn/Del.gif";
                        btn_m.Attributes.Add("class", "Btn1");
                        btn_m.ID = "Btn_DelFile" + attrF.FileNo;
                        btn_m.Attributes["onclick"] += " return confirm('�˲���Ҫִ���Ƴ��������Ƿ������');";
                        this.Add(btn_m);
                        this.AddTDEnd();
                    }

                    this.AddTREnd();
                }
                #endregion �����Կؼ���

                if (en.EnMap.Attrs.Contains("MyFileNum"))
                {
                    this.AddTR();
                    this.AddTD("nowrap=true class='FDesc' ", " ");
                    if (en.PKVal == null || Equals(en.PKVal, "") || Equals(en.PKVal, "0"))
                        this.Add("<TD  colspan=3 ><a href=\"javascript:alert('���ڱ������ִ�С�');\" target=_self>���������ϴ�(���ڱ������ִ��)</a></TD>");
                    else
                        this.Add("<TD  colspan=3 ><a href=\"../FileManager.aspx?EnName=" + en.ToString() + "&PK=" + en.PKVal + "\" target=_self >���������ϴ�&�༭</a></TD>");
                    this.AddTREnd();
                }

                this.AddTableEnd();
                this.AddDivEnd();
            }

            this.AddDivEnd();

            if (isShowDtl == false)
                return;

            int num = map.AttrsOfOneVSM.Count + map.HisRefMethods.Count + map.Dtls.Count;
            //edited by liuxc,2015-11-10,δ���治��ʾ
            if (num > 0 && en.IsExit(en.PK, en.PKVal) == false)
            {
                //string endMsg = "";
                // ���������Ϣ
                //AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
                //foreach (AttrOfOneVSM vsM in oneVsM)
                //    endMsg += "[<a href=\"javascript:alert('���ڱ������д��');\" >" + vsM.Desc + "</a>]";

                //RefMethods myreffuncs = en.EnMap.HisRefMethods;
                //foreach (RefMethod func in myreffuncs)
                //{
                //    if (func.Visable == false)
                //        continue;
                //    endMsg += "[<a href=\"javascript:alert('���ڱ����ִ�С�');\" >" + func.Title + "</a>]";
                //}

                //if (isShowDtl)
                //{
                //    EnDtls enDtls = en.EnMap.Dtls;
                //    foreach (EnDtl enDtl in enDtls)
                //        endMsg += "[<a href=\"javascript:alert('���ڱ������д��')\" >" + enDtl.Desc + "</a>]";
                //}

                //if (endMsg != "")
                //{
                //    this.Add("<table border=0><TR><TD class=TD><font style='font-size:12px' >" + endMsg + "</font></TD></TR></table>");
                //}
                return;
            }
            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }
            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }

            Entities hisens = en.GetNewEntities;
            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            if (isShowDtl)
                refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
            {
                this.Add("<font style='font-size:12px' >" + refstrs + "</font>");
                // this.Add("<HR><table class=Table border=0 ><TR class=TR ><TD class=TD ><font style='font-size:12px' >" + refstrs + "</font></TD></TR></table>");
            }
            //this.Add(refstrs);
        }
        #endregion ��v2.

        public void Bind(Entity en, string enName, bool isReadonly, bool isShowDtl)
        {
            this.Bind(en, enName, isReadonly, isShowDtl, null);
        }
        public void Bind(Entity en, string enName, bool isReadonly, bool isShowDtl, string noKey)
        {
            if (enName == null)
                enName = en.ToString();

            this.HisEn = en;
            this.IsReadonly = isReadonly;
            this.IsShowDtl = isShowDtl;
            this.Controls.Clear();
            this.AddTable("class='Table cellpadding='1' cellspacing='1' border='1' style='width:100%'");
            bool isLeft = true;
            object val = null;
            Map map = en.EnMap;
            Attrs attrs = map.Attrs;
            EnCfg cf = new EnCfg(enName);

            UIConfig cfg = new UIConfig(en);

            string[] gTitles = cf.GroupTitle.Split('@');
            foreach (Attr attr in attrs)
            {
                if (attr.Key == "MyNum")
                    continue;
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                val = en.GetValByKey(attr.Key);
                if (attr.UIVisible == false)
                {
                    this.SetValByKey(attr.Key, val.ToString());
                    continue;
                }
                foreach (string g in gTitles)
                {
                    if (g.Contains(attr.Key + "="))
                    {
                        if (isLeft == false)
                        {
                            this.AddTD();
                            this.AddTD();
                            this.AddTREnd();
                        }

                        string[] ss = g.Split('=');
                        this.AddTRSum();
                        this.AddTD("colspan=4 class=FDesc", "<b>" + ss[1] + "</b>");
                        this.AddTREnd();
                        isLeft = true;
                        break;
                    }
                }

                #region �ж��Ƿ�����ʾ
                if (attr.UIIsLine)
                {
                    if (isLeft == false)
                    {
                        this.AddTD();
                        this.AddTD();
                        this.AddTREnd();
                        isLeft = true;
                    }

                    this.AddTR();
                    if (attr.UIHeight != 0)
                    {
                        /*����ı��ɼ�, ���⴦��*/
                        if (val.ToString().Length == 0 && en.IsEmpty == false && attr.Key == "Doc")
                            val = en.GetValDocText();

                        /* doc �ı����͡���*/
                        if (attr.UIIsReadonly || isReadonly)
                        {
                            TB areaR = new TB();
                            areaR.ID = "TB_" + attr.Key;
                            areaR.Text = val.ToString();
                            areaR.Rows = 5;
                            areaR.TextMode = TextBoxMode.MultiLine;
                            areaR.ReadOnly = true;
                            this.AddContral(attr.Desc, areaR, 4);
                        }
                        else
                        {
                            EditerType type = cfg.EditerType;
                            switch (type)
                            {
                                case EditerType.FKEditer:
                                case EditerType.Sina:
                                    HtmlInputHidden input = new HtmlInputHidden();
                                    //  input.Attributes["id"] = "TBH_"+attr.Key;
                                    input.ID = "TBH_" + attr.Key;
                                    input.Value = val as string;
                                    this.Add("<td class='FDesc'  colspan='4' width='50%'>");
                                    this.Add(input);
                                    this.Add("<iframe ID='eWebEditor1' src='./Ctrl/editor/editor.htm?id=" + input.ClientID + "&style=coolblue' frameborder='0' scrolling='no' width='600' HEIGHT='350'></iframe>");
                                    this.AddTDEnd();
                                    break;
                                default:
                                    TB area = new TB();
                                    area.LoadMapAttr(attr);
                                    area.ID = "TB_" + attr.Key;
                                    if (val != null)
                                        area.Text = val.ToString();
                                    area.Rows = 5;
                                    area.TextMode = TextBoxMode.MultiLine;
                                    area.IsHelpKey = true;
                                    this.AddContral(attr.Desc, area, 4);
                                    break;
                            }
                        }
                        this.AddTREnd();

                        isLeft = true;
                        continue;
                    }

                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            TB tb = new TB();
                            tb.ID = "TB_" + attr.Key;
                            // tb.LoadMapAttr(attr);
                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                    tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                    tb.Attributes["Class"] = "TBNum";
                                    tb.ShowType = TBType.Moneny;
                                    break;
                                case DataType.AppInt:
                                case DataType.AppFloat:
                                case DataType.AppDouble:
                                    tb.Attributes["Class"] = "TBNum";
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.Num;
                                    break;
                                case DataType.AppDate:
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.Date;
                                    tb.Attributes["Class"] = "TBcalendar";
                                    tb.IsHelpKey = true;
                                    if (attr.UIIsReadonly == false)
                                        tb.Attributes["onfocus"] = "WdatePicker();";
                                    break;
                                case DataType.AppDateTime:
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.Date;
                                    tb.Attributes["Class"] = "TBcalendar";
                                    tb.IsHelpKey = true;
                                    if (attr.UIIsReadonly == false)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                    break;
                                default:
                                    tb.Text = val.ToString();
                                    tb.Attributes["Class"] = "TB";
                                    break;
                            }

                            if (isReadonly || attr.UIIsReadonly)
                                tb.ReadOnly = true;

                            tb.Enabled = true;
                            if (attr.Key == noKey && attr.UIIsReadonly == false && attr.UIVisible == true)
                            {
                                string path = this.Request.ApplicationPath;
                                //string helpScript = "";
                                string helpScript = "<a href=\"javascript:alert('�����ı�����˫����ϵͳ����ֲ��Ҵ��ڡ�');\" ><img src='./Img/Btn/Search.gif' border=0 /></a>";
                                tb.Attributes["ondblclick"] = "OpenHelperTBNo('" + path + "','BP.Port.Taxpayers',this)";
                                this.AddContral(attr.Desc, tb, helpScript, 3);
                            }
                            else
                            {
                                tb.Columns = 80;
                                this.AddContral(attr.Desc, tb, 3);
                            }
                            break;
                        case UIContralType.DDL:
                            if (isReadonly || !attr.UIIsReadonly)
                            {
                                DDL ddl = new DDL();
                                ddl.ID = "DDL_" + attr.Key;
                                string text = en.GetValRefTextByKey(attr.Key);
                                if (text == null || text == "")
                                    text = val.ToString();

                                ListItem li = new ListItem(text, val.ToString());
                                ddl.Items.Add(li);
                                ddl.Enabled = false;
                                this.AddContral(attr.Desc, ddl, true, 3);
                            }
                            else
                            {
                                /* ����ʹ�õ����. */
                                if (attr.UIDDLShowType == DDLShowType.BindSQL)
                                {
                                    DDL ddlSQL = new DDL();
                                    ddlSQL.ID = "DDL_" + attr.Key;
                                    string sql = attr.UIBindKey;
                                    if (sql.Contains("@Web") == true)
                                    {
                                        sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                                        sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                                        sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                                        sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                                    }

                                    if (sql.Contains("@") == true)
                                    {
                                        foreach (Attr myattr in en.EnMap.Attrs)
                                        {
                                            sql = sql.Replace("@" + myattr.Key, en.GetValStrByKey(myattr.Key));
                                            if (sql.Contains("@") == false)
                                                break;
                                        }
                                    }

                                    //�������Դ,��ִ�а�.
                                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                                    ddlSQL.Bind(dt, "No", "Name", val.ToString());

                                    this.AddContral(attr.DescHelper, ddlSQL, true,3);
                                }
                                else
                                {
                                    DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                                    ddl1.ID = "DDL_" + attr.Key;

                                    this.AddContral(attr.DescHelper, ddl1, true,3);
                                }

                                ///* ����ʹ�õ����. */
                                //DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                                //ddl1.ID = "DDL_" + attr.Key;
                                //this.AddContral(attr.Desc, ddl1, true, 3);
                            }
                            break;
                        case UIContralType.CheckBok:
                            CheckBox cb = new CheckBox();
                            cb.Checked = en.GetValBooleanByKey(attr.Key);
                            cb.ID = "CB_" + attr.Key;

                            if (isReadonly)
                                cb.Enabled = false;
                            else
                                cb.Enabled = attr.UIIsReadonly;

                            cb.Text = attr.Desc;
                            this.AddContral("", cb, 3);
                            break;
                        default:
                            break;
                    }
                    this.AddTREnd();
                    isLeft = true;
                    continue;
                }
                #endregion �ж��Ƿ�����ʾ // ����Ҫ��ʾ���е������

                if (isLeft)
                    this.AddTR();

                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        if (attr.UIHeight != 0)
                        {
                            if (val.ToString().Length == 0 && en.IsEmpty == false && attr.Key == "Doc")
                                val = en.GetValDocText();

                            EditerType type = cfg.EditerType;
                            switch (type)
                            {
                                case EditerType.None:
                                    /* doc �ı����͡���*/
                                    if (attr.UIIsReadonly || isReadonly)
                                    {
                                        TB areaR = new TB();
                                        areaR.ID = "TB_" + attr.Key;
                                        areaR.Text = val.ToString();
                                        areaR.Rows = 8;
                                        areaR.TextMode = TextBoxMode.MultiLine;
                                        areaR.ReadOnly = true;
                                        this.AddContral(attr.DescHelper, areaR);
                                    }
                                    else
                                    {
                                        TB area = new TB();
                                        area.LoadMapAttr(attr);
                                        area.ID = "TB_" + attr.Key;
                                        area.Text = val.ToString();
                                        area.Rows = 8;
                                        area.TextMode = TextBoxMode.MultiLine;
                                        area.IsHelpKey = true;
                                        this.AddContral(attr.DescHelper, area);
                                    }
                                    break;
                                case EditerType.Sina:
                                case EditerType.FKEditer:
                                    /* doc �ı����͡���*/
                                    HtmlInputHidden input = new HtmlInputHidden();
                                    input.Attributes["id"] = "txtContent";
                                    //input.ID = attr.Key;
                                    this.Add("<td class='FDesc'  colspan='2' nowrap width='50%' >" + attr.Desc + "<br>");
                                    this.Add(input);
                                    this.Add("<iframe ID='eWebEditor1' src='./Ctrl/Edit/editor.htm?id=txtContent&style=coolblue' frameborder='0' scrolling='no' width='600' HEIGHT='350'></iframe>");
                                    this.AddTDEnd();

                                    //TB area = new TB();
                                    //area.LoadMapAttr(attr);
                                    //area.ID = "TB_" + attr.Key;
                                    //area.Text = val.ToString();
                                    //area.Rows = 8;
                                    //area.TextMode = TextBoxMode.MultiLine;
                                    //area.IsHelpKey = true;
                                    //this.AddContral(attr.Desc, area);

                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            TB tb = new TB();
                            tb.ID = "TB_" + attr.Key;
                            switch (attr.MyDataType)
                            {
                                case DataType.AppMoney:
                                    try
                                    {
                                        tb.Text = decimal.Parse(val.ToString()).ToString("0.00");
                                        tb.Attributes["Class"] = "TBNum";
                                    }
                                    catch
                                    {
                                        tb.Text = decimal.Parse("0").ToString("0.00");
                                        tb.Attributes["Class"] = "TBNum";
                                        //  Log.DebugWriteWarning( "@"+en.ToString()+"Attr="+attr.Key=" Val="+val +" Ex="+ex.Message );
                                    }
                                    tb.ShowType = TBType.Moneny;
                                    break;
                                case DataType.AppInt:
                                case DataType.AppFloat:
                                case DataType.AppDouble:
                                    tb.Attributes["Class"] = "TBNum";
                                    tb.ShowType = TBType.Float;
                                    tb.Text = val.ToString();
                                    break;
                                case DataType.AppDate:
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.Date;
                                    tb.IsHelpKey = true;
                                    if (attr.UIIsReadonly == false)
                                        tb.Attributes["onfocus"] = "WdatePicker();";
                                    break;
                                case DataType.AppDateTime:
                                    tb.Text = val.ToString();
                                    tb.ShowType = TBType.Date;
                                    tb.IsHelpKey = true;
                                    if (attr.UIIsReadonly == false)
                                        tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                    break;
                                default:
                                    tb.Text = val.ToString();
                                    tb.Attributes["Class"] = "TB";
                                    break;
                            }

                            if (isReadonly || attr.UIIsReadonly)
                                tb.ReadOnly = true;

                            if (attr.IsPK && DataType.IsNullOrEmpty(tb.Text) == false)
                                tb.ReadOnly = true;

                            if (attr.Key == noKey && attr.UIIsReadonly == false && attr.UIVisible == true)
                            {
                                string path = this.Request.ApplicationPath;
                                //string helpScript = "";
                                string helpScript = "<a href=\"javascript:alert('�����ı�����˫����ϵͳ����ֲ��Ҵ��ڡ�');\" ><img src='./Img/Btn/Search.gif' border=0 /></a>";
                                tb.Attributes["ondblclick"] = "OpenHelperTBNo('" + path + "','BP.Port.Taxpayers',this)";
                                this.AddContral(attr.DescHelper, tb, helpScript);
                            }
                            else
                            {
                                this.AddContral(attr.DescHelper, tb);
                            }
                        }
                        break;
                    case UIContralType.DDL:
                        if (isReadonly || !attr.UIIsReadonly)
                        {
                            DDL ddl = new DDL();
                            ddl.ID = "DDL_" + attr.Key;
                            ddl.CssClass = "DDL" + WebUser.Style;
                            string text = en.GetValRefTextByKey(attr.Key);
                            if (text == null || text == "")
                                text = val.ToString();

                            ListItem li = new ListItem(text, val.ToString());
                            li.Attributes["class"] = "TB";
                            ddl.Items.Add(li);
                            ddl.Enabled = false;
                            this.AddContral(attr.DescHelper, ddl, true);
                        }
                        else
                        {
                            /* ����ʹ�õ����. */
                            if (attr.UIDDLShowType == DDLShowType.BindSQL)
                            {
                                DDL ddlSQL = new DDL();
                                ddlSQL.ID = "DDL_" + attr.Key;
                                string sql = attr.UIBindKey;
                                if (sql.Contains("@Web") == true)
                                {
                                    sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                                    sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                                    sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                                    sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                                }

                                if (sql.Contains("@") == true)
                                {
                                    foreach (Attr myattr in en.EnMap.Attrs)
                                    {
                                        sql = sql.Replace("@" + myattr.Key, en.GetValStrByKey(myattr.Key));
                                        if (sql.Contains("@") == false)
                                            break;
                                    }
                                }

                                //�������Դ,��ִ�а�.
                                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                                ddlSQL.Bind(dt, "No", "Name", val.ToString());
                                this.AddContral(attr.DescHelper, ddlSQL, true);
                            }
                            else
                            {
                                DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                                ddl1.ID = "DDL_" + attr.Key;
                                this.AddContral(attr.DescHelper, ddl1, true);
                            }

                            ///* ����ʹ�õ����. */
                            //DDL ddl1 = new DDL(attr, val.ToString(), "enumLab", true, this.Page.Request.ApplicationPath);
                            //ddl1.ID = "DDL_" + attr.Key;
                            //this.AddContral(attr.DescHelper, ddl1, true);
                        }
                        break;
                    case UIContralType.CheckBok:
                        CheckBox cb = new CheckBox();
                        cb.Checked = en.GetValBooleanByKey(attr.Key);

                        if (isReadonly)
                            cb.Enabled = false;
                        else
                            cb.Enabled = attr.UIIsReadonly;

                        cb.ID = "CB_" + attr.Key;
                        cb.Text = attr.Desc;
                        if (attr.HelperUrl == null)
                            this.AddContral("", cb);
                        else
                            this.AddContral(attr.DescHelperIcon, cb);
                        break;
                    default:
                        break;
                }

                if (isLeft == false)
                    this.AddTREnd();
                isLeft = !isLeft;
            }

            // ����ѭ��.
            if (isLeft == false)
            {
                AddContral();
                this.AddTREnd();
            }

            #region �鿴�Ƿ���� MyFile�ֶ�����о���Ϊ�Ǹ�����
            if (en.EnMap.Attrs.Contains("MyFileName"))
            {
                /* �������������ֶΡ�*/
                string fileName = en.GetValStringByKey("MyFileName");
                string filePath = en.GetValStringByKey("MyFilePath");
                string fileExt = en.GetValStringByKey("MyFileExt");
                string url = "";
                if (fileExt == "")
                {
                    /*����Ƿ����ֹ��ϴ��ĸ���*/
                    BP.Sys.Glo.InitEntityAthInfo(en);
                    fileExt = en.GetValStringByKey("MyFileExt");
                }

                if (fileExt != "")
                {
                    // ϵͳ����·����
                    string path = this.Request.PhysicalApplicationPath.ToLower();
                    string path1 = filePath.ToLower();
                    path1 = path1.Replace(path, "");
                    // url = "&nbsp;&nbsp;<a href='" + cf.FJWebPath + "/" + en.PKVal + "." + fileExt + "' target=_blank ><img src='"+this.Request.ApplicationPath+"Images/FileType/" + fileExt + ".gif' border=0 />" + fileName + "</a>";
                    url = "&nbsp;&nbsp;<a href='../Do.aspx?DoType=DownFile&EnName=" + enName + "&PK=" + en.PKVal + "' target=_blank ><img src='./../../Img/FileType/" + fileExt + ".gif' border=0 />" + fileName + "</a>";
                }

                this.AddTR();
                this.AddTD("nowrap=true class='FDesc' ", en.EnMap.GetAttrByKey("MyFileName").Desc);

                HtmlInputFile file = new HtmlInputFile();
                file.ID = "file";
                this.AddTD(file);

                this.AddTDBegin("colspan=2");
                this.Add(url);
                if (fileExt != "")
                {
                    System.Web.UI.WebControls.ImageButton btn1 = new System.Web.UI.WebControls.ImageButton();
                    btn1.ImageUrl = "../../Img/Btn/Delete.gif";

                    //  btn1.Text = "�Ƴ�";
                    btn1.Attributes.Add("class", "Btn1");
                    btn1.ID = "Btn_DelFile";
                    btn1.Attributes["onclick"] += " return confirm('�˲���Ҫִ���Ƴ��������Ƿ������');";
                    this.Add(btn1);
                }
                this.AddTDEnd();

                this.AddTREnd();
            }
            #endregion

            #region �����Կؼ���
            AttrFiles fileAttrs = en.EnMap.HisAttrFiles;
            SysFileManagers sfs = new SysFileManagers(en.ToString(), en.PKVal.ToString());
            foreach (AttrFile attrF in fileAttrs)
            {
                this.AddTR();
                this.AddTD("nowrap=true class='FDesc' ", attrF.FileName);
                HtmlInputFile file = new HtmlInputFile();
                file.ID = "F" + attrF.FileNo;
                file.Attributes["width"] = "100%";
                this.AddTD(file);

                /* �ж��Ƿ����ļ�û���ļ����Ƴ�����*/
                SysFileManager sf = sfs.GetEntityByKey(SysFileManagerAttr.AttrFileNo, attrF.FileNo) as SysFileManager;
                if (sf == null)
                {
                    this.AddTD();
                    this.AddTD();
                }
                else
                {
                    this.Add("<TD class=TD colspan=2>");
                    string lab = "&nbsp;<a href='" + cf.FJWebPath + sf.OID + "." + sf.MyFileExt + "' target=_blank ><img src='../../Img/FileType/" + sf.MyFileExt + ".gif' border=0 />" + sf.MyFileName + "." + sf.MyFileExt + "</a>";
                    this.Add(lab);

                    System.Web.UI.WebControls.ImageButton btn_m = new System.Web.UI.WebControls.ImageButton();
                    btn_m.ImageUrl = "../../Img/Btn/Delete.gif";

                    //btn_m.ImageUrl = "./Img/Btn/Del.gif";
                    btn_m.Attributes.Add("class", "Btn1");
                    btn_m.ID = "Btn_DelFile" + attrF.FileNo;
                    btn_m.Attributes["onclick"] += " return confirm('�˲���Ҫִ���Ƴ��������Ƿ������');";
                    this.Add(btn_m);
                    this.AddTDEnd();
                }
                this.AddTREnd();
            }
            #endregion �����Կؼ���

            if (en.EnMap.Attrs.Contains("MyFileNum"))
            {
                this.AddTR();
                this.AddTD("nowrap=true class='FDesc' ", " ");
                if (en.PKVal == null || Equals(en.PKVal, "") || Equals(en.PKVal, "0"))
                    this.Add("<TD  colspan=3 ><a href=\"javascript:alert('���ڱ������ִ�С�');\" target=_self>���������ϴ�(���ڱ������ִ��)</a></TD>");
                else
                    this.Add("<TD  colspan=3 ><a href=\"../FileManager.aspx?EnName=" + en.ToString() + "&PK=" + en.PKVal + "\" target=_self >���������ϴ�&�༭</a></TD>");
                this.AddTREnd();
            }
            this.AddTableEnd();

            if (isShowDtl == false)
                return;

            int num = map.AttrsOfOneVSM.Count + map.HisRefMethods.Count + map.Dtls.Count;
            //edited by liuxc,2015-11-10,δ���治��ʾ
            if (num > 0 && en.IsExit(en.PK, en.PKVal) == false)
            {
                //string endMsg = "";
                // ���������Ϣ
                //AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
                //foreach (AttrOfOneVSM vsM in oneVsM)
                //    endMsg += "[<a href=\"javascript:alert('���ڱ������д��');\" >" + vsM.Desc + "</a>]";

                //RefMethods myreffuncs = en.EnMap.HisRefMethods;
                //foreach (RefMethod func in myreffuncs)
                //{
                //    if (func.Visable == false)
                //        continue;
                //    endMsg += "[<a href=\"javascript:alert('���ڱ����ִ�С�');\" >" + func.Title + "</a>]";
                //}


                //if (isShowDtl)
                //{
                //    EnDtls enDtls = en.EnMap.Dtls;
                //    foreach (EnDtl enDtl in enDtls)
                //        endMsg += "[<a href=\"javascript:alert('���ڱ������д��')\" >" + enDtl.Desc + "</a>]";
                //}

                //if (endMsg != "")
                //{
                //    this.Add("<table border=0><TR><TD class=TD><font style='font-size:12px' >" + endMsg + "</font></TD></TR></table>");
                //}

                return;
            }

            string refstrs = "";
            if (en.IsEmpty)
            {
                refstrs += "";
                return;
            }

            string keys = "&PK=" + en.PKVal.ToString();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.Enum ||
                    attr.MyFieldType == FieldType.FK ||
                    attr.MyFieldType == FieldType.PK ||
                    attr.MyFieldType == FieldType.PKEnum ||
                    attr.MyFieldType == FieldType.PKFK)
                    keys += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
            }


            Entities hisens = en.GetNewEntities;
            keys += "&r=" + System.DateTime.Now.ToString("ddhhmmss");
            if (isShowDtl)
                refstrs = GetRefstrs(keys, en, en.GetNewEntities);
            if (refstrs != "")
            {
                this.Add("<font style='font-size:12px' >" + refstrs + "</font>");
                // this.Add("<HR><table class=Table border=0 ><TR class=TR ><TD class=TD ><font style='font-size:12px' >" + refstrs + "</font></TD></TR></table>");
            }
            //this.Add(refstrs);
        }

        //		public static string GetRefstrs(string keys, Entity en, Entities hisens)
        //		{
        //			string refstrs="";
        //			string path=System.Web.HttpContext.Current.Request.ApplicationPath;
        //			int i = 0;
        //
        //			#region ����һ�Զ��ʵ��༭
        //			AttrsOfOneVSM oneVsM= en.EnMap.AttrsOfOneVSM;
        //			if ( oneVsM.Count > 0 )
        //			{
        //				foreach(AttrOfOneVSM vsM in oneVsM)
        //				{
        //					string url=path+"/Comm/UIEn1ToM.aspx?EnsName="+en.ToString()+"&AttrKey="+vsM.EnsOfMM.ToString()+keys;
        //					string sql="SELECT COUNT(*) FROM "+vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable+" WHERE "+vsM.AttrOfOneInMM+"='"+en.PKVal+"'";
        //					try
        //					{
        //						i=DBAccess.RunSQLReturnValInt(sql);
        //					}
        //					catch(Exception ex)
        //					{
        //						vsM.EnsOfMM.GetNewEntity.CheckPhysicsTable();
        //						throw ex;
        //					}
        //			 
        //					if (i==0)
        //						refstrs+="[<a href=\"javascript:WinShowModalDialog('"+url+"','onVsM'); \"  >"+vsM.Desc+"</a>]";
        //					else
        //						refstrs+="[<a href=\"javascript:WinShowModalDialog('"+url+"','onVsM'); \"  >"+vsM.Desc+"-"+i+"</a>]";
        //				}
        //			}
        //			#endregion
        //
        //			#region �������ŵ���ع���
        ////			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
        ////			if ( reffuncs.Count > 0  )
        ////			{
        ////				foreach(SysUIEnsRefFunc en1 in reffuncs)
        ////				{
        ////					string url=path+"/Comm/RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
        ////					refstrs+="[<a href=\"javascript:WinOpen('"+url+"','ref'); \"  >"+en1.Name+"</a>]";
        ////				}
        ////			}
        //			#endregion
        //
        //			#region �������ŵ� ����
        //			RefMethods myreffuncs = en.EnMap.HisRefMethods ;
        //			if ( myreffuncs.Count > 0  )
        //			{
        //				foreach(RefMethod func in myreffuncs)
        //				{
        //					if (func.Visable==false)
        //						continue;
        //
        //					string url=path+"/Comm/RefMethod.htm?Index="+func.Index+"&EnsName="+hisens.ToString()+keys;
        //
        //					if (func.Warning==null)
        //					{
        //						if (func.Target==null)
        //							refstrs+="["+func.GetIcon( path ) +"<a href='"+url+"' ToolTip='"+func.ToolTip+"' >"+func.Title+"</a>]";
        //						else
        //							refstrs+="["+func.GetIcon( path )+"<a href=\"javascript:WinOpen('"+url+"','"+func.Target+"')\" ToolTip='"+func.ToolTip+"' >"+func.Title+"</a>]";
        //					}
        //					else
        //					{
        //						if (func.Target==null)
        //							refstrs+="["+func.GetIcon( path )+"<a href=\"javascript: if ( confirm('"+func.Warning+"') ) { window.location.href='"+url+"' }\" ToolTip='"+func.ToolTip+"' >"+func.Title+"</a>]";
        //						else
        //							refstrs+="["+func.GetIcon( path )+"<a href=\"javascript: if ( confirm('"+func.Warning+"') ) { WinOpen('"+url+"','"+func.Target+"') }\" ToolTip='"+func.ToolTip+"' >"+func.Title+"</a>]";
        //					}
        //				}
        //			}
        //			#endregion
        //
        //			#region ����������ϸ
        //			EnDtls enDtls= en.EnMap.Dtls;
        //			if ( enDtls.Count > 0 )
        //			{
        //				foreach(EnDtl enDtl in enDtls)
        //				{
        //					string url=path+"/Comm/UIEnDtl.aspx?EnsName="+enDtl.EnsName+"&Key="+enDtl.RefKey+"&Val="+en.PKVal.ToString()+"&MainEnsName="+en.ToString()+keys;
        //					try
        //					{
        //						 i=DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM "+enDtl.Ens.GetNewEntity.EnMap.PhysicsTable+" WHERE "+enDtl.RefKey+"='"+en.PKVal+"'"); 
        //					}
        //					catch(Exception ex)
        //					{
        //						enDtl.Ens.GetNewEntity.CheckPhysicsTable();
        //						throw ex;
        //					}
        //
        //					if (i==0)					 
        //						refstrs+="[<a href=\"javascript:WinOpen('"+url+"', 'dtl"+enDtl.RefKey+"'); \" >"+enDtl.Desc+"</a>]";
        //					else
        //						refstrs+="[<a href=\"javascript:WinOpen('"+url+"', 'dtl"+enDtl.RefKey+"'); \"  >"+enDtl.Desc+"-"+i+"</a>]";
        //				}
        //			}
        //			#endregion
        //			return refstrs;
        //		}
        //		public static string GetRefstrs1(string keys, Entity en, Entities hisens)
        //		{
        //			string refstrs="";
        //
        //			#region ����һ�Զ��ʵ��༭
        //			AttrsOfOneVSM oneVsM= en.EnMap.AttrsOfOneVSM;
        //			if ( oneVsM.Count > 0 )
        //			{
        //				foreach(AttrOfOneVSM vsM in oneVsM)
        //				{
        //					string url="UIEn1ToM.aspx?EnsName="+en.ToString()+"&AttrKey="+vsM.EnsOfMM.ToString()+keys;
        //					string sql="SELECT COUNT(*) FROM "+vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable+" WHERE "+vsM.AttrOfOneInMM+"='"+en.PKVal+"'";
        //					int i=DBAccess.RunSQLReturnValInt(sql);
        //			 
        //					if (i==0)
        //						refstrs+="[<a href='"+url+"'  >"+vsM.Desc+"</a>]";
        //					else
        //						refstrs+="[<a href='"+url+"'  >"+vsM.Desc+"-"+i+"</a>]";
        //				 
        //				}
        //			}
        //			#endregion
        //
        //			#region �������ŵ���ع���
        ////			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
        ////			if ( reffuncs.Count > 0  )
        ////			{
        ////				foreach(SysUIEnsRefFunc en1 in reffuncs)
        ////				{
        ////					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
        ////					refstrs+="[<a href='"+url+"' >"+en1.Name+"</a>]";
        ////				}
        ////			}
        //			#endregion	 
        //
        //			#region ����������ϸ
        //			EnDtls enDtls= en.EnMap.Dtls;
        //			if ( enDtls.Count > 0 )
        //			{
        //				foreach(EnDtl enDtl in enDtls)
        //				{
        //					string url="UIEnDtl.aspx?EnsName="+enDtl.EnsName+"&Key="+enDtl.RefKey+"&Val="+en.PKVal.ToString()+"&MainEnsName="+en.ToString()+keys;
        //
        //					int i=DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM "+enDtl.Ens.GetNewEntity.EnMap.PhysicsTable+" WHERE "+enDtl.RefKey+"='"+en.PKVal+"'");
        //					if (i==0)					 
        //						refstrs+="[<a href='"+url+"'  >"+enDtl.Desc+"</a>]";
        //					else
        //						refstrs+="[<a href='"+url+"'  >"+enDtl.Desc+"-"+i+"</a>]";
        //				}
        //			}
        //			#endregion
        //			
        //
        //			return refstrs;
        //		}
        //		public void Delete()
        //		public Entity GetEnData(Entity en)

        public Entity GetEnData(Entity en)
        {
            string key = "";
            try
            {
                foreach (Attr attr in en.EnMap.Attrs)
                {
                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (attr.Key == "MyNum")
                        continue;

                    if (attr.UIVisible == false)
                        continue;


                    key = attr.Key;

                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            if (attr.UIVisible)
                            {
                                if (attr.UIHeight == 0)
                                {
                                    en.SetValByKey(attr.Key, this.GetTBByID("TB_" + attr.Key).Text);
                                    continue;
                                }
                                else
                                {
                                    if (this.IsExit("TB_" + attr.Key))
                                    {
                                        en.SetValByKey(attr.Key, this.GetTBByID("TB_" + attr.Key).Text);
                                        continue;
                                    }

                                    if (this.IsExit("TBH_" + attr.Key))
                                    {
                                        HtmlInputHidden input = (HtmlInputHidden)this.FindControl("TBH_" + attr.Key);
                                        en.SetValByKey(attr.Key, input.Value);
                                        continue;
                                    }

                                    if (this.IsExit("TBF_" + attr.Key))
                                    {
                                        //  FredCK.FCKeditorV2.FCKeditor fck = (FredCK.FCKeditorV2.FCKeditor)this.FindControl("TB_" + attr.Key);
                                        // en.SetValByKey(attr.Key, fck.Value);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                en.SetValByKey(attr.Key, this.GetValByKey(attr.Key));
                            }
                            break;
                        case UIContralType.DDL:
                            try
                            {
                                en.SetValByKey(attr.Key, this.GetDDLByKey("DDL_" + attr.Key).SelectedItem.Value);
                            }
                            catch
                            {
                            }
                            break;
                        case UIContralType.CheckBok:
                            en.SetValByKey(attr.Key, this.GetCBByKey("CB_" + attr.Key).Checked);
                            break;
                        case UIContralType.RadioBtn:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetEnData error :" + ex.Message + " key = " + key);
            }
            return en;
        }
        //		public DDL GetDDLByKey(string key)
        public DDL GetDDLByKey(string key)
        {
            return (DDL)this.FindControl(key);
        }
        //		public CheckBox GetCBByKey(string key)
        public CheckBox GetCBByKey(string key)
        {
            return (CheckBox)this.FindControl(key);
        }
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (this.IsPostBack)
            {
                //	this.Bind(this.HisEn,this.IsReadonly,this.IsShowDtl ) ; 	
            }
        }
        public Entity HisEn = null;

        public static string GetRefstrs1(string keys, Entity en, Entities hisens)
        {
            string refstrs = "";

            #region ����һ�Զ��ʵ��༭
            AttrsOfOneVSM oneVsM = en.EnMap.AttrsOfOneVSM;
            if (oneVsM.Count > 0)
            {
                foreach (AttrOfOneVSM vsM in oneVsM)
                {
                    string url = "UIEn1ToM.aspx?EnsName=" + en.ToString() + "&AttrKey=" + vsM.EnsOfMM.ToString() + keys;
                    int i = 0;

                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*)  as NUM FROM " + vsM.EnsOfMM.GetNewEntity.EnMap.PhysicsTable + " WHERE " + vsM.AttrOfOneInMM + "=" + en.PKVal);
                    }

                    if (i == 0)
                        refstrs += "[<a href='" + url + "'  >" + vsM.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "'  >" + vsM.Desc + "-" + i + "</a>]";

                }
            }
            #endregion

            #region �������ŵ���ع���
            //			SysUIEnsRefFuncs reffuncs = en.GetNewEntities.HisSysUIEnsRefFuncs ;
            //			if ( reffuncs.Count > 0  )
            //			{
            //				foreach(SysUIEnsRefFunc en1 in reffuncs)
            //				{
            //					string url="RefFuncLink.aspx?RefFuncOID="+en1.OID.ToString()+"&MainEnsName="+hisens.ToString()+keys;
            //					refstrs+="[<a href='"+url+"' >"+en1.Name+"</a>]";
            //				}
            //			}
            #endregion

            #region ����������ϸ
            EnDtls enDtls = en.EnMap.Dtls;
            if (enDtls.Count > 0)
            {
                foreach (EnDtl enDtl in enDtls)
                {
                    string url = "UIEnDtl.aspx?EnsName=" + enDtl.EnsName + "&RefKey=" + enDtl.RefKey + "&RefVal=" + en.PKVal.ToString() + "&MainEnsName=" + en.ToString() + keys;

                    int i = 0;

                    try
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "='" + en.PKVal + "'");
                    }
                    catch
                    {
                        i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM " + enDtl.Ens.GetNewEntity.EnMap.PhysicsTable + " WHERE " + enDtl.RefKey + "=" + en.PKVal);
                    }
                    if (i == 0)
                        refstrs += "[<a href='" + url + "'  >" + enDtl.Desc + "</a>]";
                    else
                        refstrs += "[<a href='" + url + "'  >" + enDtl.Desc + "-" + i + "</a>]";
                }
            }
            #endregion

            return refstrs;
        }

        #region Web ������������ɵĴ���
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: �õ����� ASP.NET Web ���������������ġ�
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		�����֧������ķ��� - ��Ҫʹ�ô���༭��
        ///		�޸Ĵ˷��������ݡ�
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion


    }
}
