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
using BP.Web;
using BP.En;
using BP.DA;
using BP.Sys;
using BP.Web.Controls;
using CCFlow.Web.Comm;
using BP.Web.Comm;

public partial class CCFlow_Comm_UIEnsV10 : BP.Web.WebPage
{
    #region 属性
    public string GroupKey
    {
        get
        {
            return this.Request.QueryString["GroupKey"] as string;
        }
    }
    public new string EnsName
    {
        get
        {
            string str = this.Request.QueryString["EnsName"];
            if (str == null)
                return "BP.Edu.BTypes";
            return str;
        }
    }
    public new Entity HisEn
    {
        get
        {
            return this.HisEns.GetNewEntity;
        }
    }
    public new Entities HisEns
    {
        get
        {
            Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);
            return ens;
        }
    }
    #endregion 属性

    protected void Page_Load(object sender, EventArgs e)
    {
        //BP.Sys.GEEntity en = new GEEntity("BP.WF.Flow", "001");
        this.Page.RegisterClientScriptBlock("s",
        "<link href='./Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

        string info = this.Session["info1"] as string;
        if (info != null)
        {
            if (info.Length > 2)
            {
                this.ResponseWriteRedMsg(info);
                this.Session["info1"] = "";
                this.Session["Info"] = null;
            }
        }

        this.ToolBar1.AddLinkBtn(NamesOfBtn.Save,"保存(S)"  );
        this.ToolBar1.AddLinkBtn(NamesOfBtn.Delete, "删除(D)");

        this.ToolBar1.GetLinkBtnByID("Btn_Save").Click += new EventHandler(ToolBar1_ButtonClick);
        //   this.ToolBar1.GetLinkBtnByID("Btn_New").Click += new EventHandler(ToolBar1_ButtonClick);
        this.ToolBar1.GetLinkBtnByID("Btn_Delete").Click += new EventHandler(ToolBar1_ButtonClick);

        // this.ToolBar1.Add("<input type=button value='设置' onclick=\"OpenAttrs('" + this.EnsName + "')\"  >");
        //this.ToolBar1.AddLab("sw", "<input type=button  id='ToolBar1$Btn_P' name='ToolBar1$Btn_P'  onclick=\"javascript:OpenAttrs('" + this.EnsName + "');\"  value='设置(P)'  />");

        this.Bind();

        Entity en = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;

        if (this.GroupKey == null)
        {
            //this.Pub1.Add(  this.GenerCaption(en.EnDesc + en.EnMap.TitleExt) );
        }
        else
        {
            BP.Web.GroupXml xml = new GroupXml(this.GroupKey);
            string[] strs = xml.Name.Split('@');

            this.Pub1.MenuSelfBegin();
            foreach (string str in strs)
            {
                if (str == null)
                    continue;

                string[] ss = str.Split('=');
                if (ss.Length == 0)
                    continue;

                string url = this.PageID + ".aspx?EnsName=" + ss[0] + "&GroupKey=" + this.GroupKey;

                if (str.Contains(this.EnsName) == true)
                    this.Pub1.MenuSelfItemS(url, str, "_self");
                else
                    this.Pub1.MenuSelfItem(url, str, "_self");
            }
            this.Pub1.MenuSelfEnd();

        }
    }

    public void Bind()
    {
        #region 生成标题
        Entity en = this.HisEn;
        Map map = this.HisEn.EnMap;
        EnCfg cfg = new EnCfg(en.ToString());

        UIConfig uicfg = new UIConfig(en);

        Attrs attrs = map.Attrs;
        if (attrs.Count>=4)
        this.ucsys1.Add("<table border=0 cellpadding='0'  style='border-collapse: collapse;width:100%' cellspacing='0'  >");
        else
            this.ucsys1.Add("<table border=0 cellpadding='0'  style='border-collapse: collapse;width:50%' cellspacing='0'  >");

        this.ucsys1.AddTR();
        CheckBox cb = new CheckBox();
        string str1 = "<INPUT id='checkedAll' onclick='SelectAll()' type='checkbox' name='checkedAll'>";
        this.ucsys1.AddTDGroupTitle(str1);
        foreach (Attr attr in attrs)
        {
            if (attr.UIVisible == false)
                continue;
            this.ucsys1.AddTDGroupTitle(attr.Desc);
        }

        if (map.IsHaveFJ)
        {
            this.ucsys1.AddTDGroupTitle("附件");
        }
        this.ucsys1.AddTDGroupTitle();
        this.ucsys1.AddTREnd();
        #endregion 生成标题

        this.Title = en.EnDesc;

        Entities ens = this.HisEns;
        QueryObject qo = new QueryObject(ens);

        #region 用户界面属性设置- del
        //BP.Web.Comm.UIRowStyleGlo tableStyle = (UIRowStyleGlo)ens.GetEnsAppCfgByKeyInt("UIRowStyleGlo"); // 界面风格。
        //bool IsEnableDouclickGlo = ens.GetEnsAppCfgByKeyBoolen("IsEnableDouclickGlo"); // 是否启用双击
        //bool IsEnableRefFunc = ens.GetEnsAppCfgByKeyBoolen("IsEnableRefFunc"); // 是否显示相关功能。
        //bool IsEnableFocusField = ens.GetEnsAppCfgByKeyBoolen("IsEnableFocusField"); //是否启用焦点字段。
        //bool isShowOpenICON = ens.GetEnsAppCfgByKeyBoolen("IsEnableOpenICON"); //是否启用 OpenICON 。
        //string FocusField = null;
        //if (IsEnableFocusField)
        //    FocusField = ens.GetEnsAppCfgByKeyString("FocusField");

        //int WinCardH = ens.GetEnsAppCfgByKeyInt("WinCardH"); // 弹出窗口高度
        //int WinCardW = ens.GetEnsAppCfgByKeyInt("WinCardW"); // 弹出窗口宽度.
        #endregion 用户界面属性设置

        #region 生成翻页
        try
        {
            this.ucsys2.Clear();
            this.ucsys2.BindPageIdx(qo.GetCount(), BP.Sys.SystemConfig.PageSize, this.PageIdx, "Ens.aspx?EnsName=" + this.EnsName);
            qo.DoQuery(en.PK, BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
        }
        catch (Exception ex)
        {
            //自动创建表.
            Log.DebugWriteInfo(ex.Message);

            ens.GetNewEntity.CheckPhysicsTable();
            return;
        }
        #endregion 生成翻页

        en.PKVal = "0";
        ens.AddEntity(en);
        DDL ddl = new DDL();
        bool is1 = false;

        #region 生成数据
        int i = 0;
        foreach (Entity dtl in ens)
        {
            string urlExt = "\"javascript:ShowEn('./RefFunc/UIEn.aspx?EnsName=" + ens.ToString() + "&PK=" + dtl.PKVal + "', 'cd');\"";
            i++;
            if (Equals(dtl.PKVal, "0"))
            {
                this.ucsys1.AddTRSum();
                this.ucsys1.AddTDIdx("<b>*</b>");
            }
            else
            {
                is1 = this.ucsys1.AddTR(is1, "ondblclick=" + urlExt);
                cb = new CheckBox();
                cb.ID = "IDX_" + dtl.PKVal;
                cb.Text = i.ToString();
                this.ucsys1.AddTDIdx(cb);
            }
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.Key == "OID")
                    continue;

                string val = dtl.GetValByKey(attr.Key).ToString();
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        TB tb = new TB();
                        tb.LoadMapAttr(attr);
                        this.ucsys1.AddTD(tb);
                        tb.ID = "TB_" + attr.Key + "_" + dtl.PKVal;
                        switch (attr.MyDataType)
                        {
                            case DataType.AppMoney:
                                tb.TextExtMoney = decimal.Parse(val);
                                break;
                            default:
                                tb.Text = val;
                                break;
                        }

                        if (attr.IsNum && attr.IsFKorEnum == false)
                        {
                            if (tb.Enabled)
                            {
                                // OnKeyPress="javascript:return VirtyNum(this);"
                                //  tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";
                                // tb.Attributes["onkeyup"] += "javascript:C" + dtl.PKVal + "();C" + attr.Key + "();";
                                tb.Attributes["class"] = "TBNum";
                            }
                            else
                            {
                                //   tb.Attributes["onpropertychange"] += "C" + attr.Key + "();";
                                tb.Attributes["class"] = "TBNumReadonly";
                            }
                        }
                        break;
                    case UIContralType.DDL:
                        if (attr.UIIsReadonly)
                        {
                            ddl = new DDL();
                            ddl.LoadMapAttr(attr);
                            ddl.ID = "DDL_" + attr.Key + "_" + dtl.PKVal;
                            //  this.ucsys1.AddTD(ddl);
                            this.ucsys1.AddTD(ddl);
                            ddl.SetSelectItem(val);
                        }
                        else
                        {
                            this.ucsys1.AddTD(dtl.GetValRefTextByKey(attr.Key));
                        }
                        break;
                    case UIContralType.CheckBok:
                        cb = new CheckBox();
                        cb.ID = "CB_" + attr.Key + "_" + dtl.PKVal;
                        //cb.Text = attr.Name;
                        if (val == "1")
                            cb.Checked = true;
                        else
                            cb.Checked = false;
                        this.ucsys1.AddTDCenter(cb);
                        break;
                    default:
                        break;
                }
            }
            if (map.IsHaveFJ)
            {
                string ext = dtl.GetValStrByKey("MyFileExt");
                if (ext == null || ext.Length > 1)
                    this.ucsys1.AddTD("<a href='" + cfg.FJWebPath + "/" + dtl.PKVal + "." + ext + "' target=_blank ><img src='../Images/FileType/" + dtl.GetValStrByKey("MyFileExt") + ".gif' border=0/>" + dtl.GetValStrByKey("MyFileName") + "</a>");
                else
                    this.ucsys1.AddTD();
            }
            if (uicfg.IsEnableOpenICON && dtl.PKVal.ToString().Length >= 2  )
                this.ucsys1.Add("<TD class='TD' style='cursor:hand;' nowrap=true><a href=" + urlExt + " ><img src='../Img/Btn/Open.gif' border=0/></a></TD>");
            else
                this.ucsys1.AddTD();
            this.ucsys1.AddTREnd();
        }

        #region 生成合计，屏蔽
        //if (false)
        //{
        //    this.ucsys1.AddTRSum();
        //    this.ucsys1.AddTD("colspan=1", "合计");
        //    foreach (Attr attr in attrs)
        //    {
        //        if (attr.UIVisible == false)
        //            continue;

        //        if (attr.IsNum && attr.IsFKorEnum == false)
        //        {
        //            TB tb = new TB();
        //            tb.ID = "TB_" + attr.Key;
        //            tb.Text = attr.DefaultVal.ToString();
        //            tb.ShowType = attr.HisTBType;
        //            tb.ReadOnly = true;
        //            tb.Font.Bold = true;
        //            tb.BackColor = System.Drawing.Color.FromName("#FFFFFF");

        //            switch (attr.MyDataType)
        //            {
        //                case DataType.AppRate:
        //                case DataType.AppMoney:
        //                    tb.TextExtMoney = ens.GetSumDecimalByKey(attr.Key);
        //                    break;
        //                case DataType.AppInt:
        //                    tb.TextExtInt = ens.GetSumIntByKey(attr.Key);
        //                    break;
        //                case DataType.AppFloat:
        //                    tb.TextExtFloat = ens.GetSumFloatByKey(attr.Key);
        //                    break;
        //                default:
        //                    break;
        //            }
        //            this.ucsys1.AddTD(tb);
        //        }
        //        else
        //        {
        //            this.ucsys1.AddTD();
        //        }
        //    }
        //    if (map.IsHaveFJ)
        //        this.ucsys1.AddTD();

        //    this.ucsys1.AddTD();
        //    this.ucsys1.AddTREnd();
        //}
        #endregion 生成合计

        #endregion 生成数据

        this.ucsys1.AddTableEnd();
    }
    public void Save()
    {
        string msg = null;
        Entities dtls = BP.En.ClassFactory.GetEns(this.EnsName);
        Entity en = dtls.GetNewEntity;
        QueryObject qo = new QueryObject(dtls);
        qo.DoQuery(en.PK, BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
        Map map = dtls.GetNewEntity.EnMap;
        foreach (Entity dtl in dtls)
        {
            this.ucsys1.Copy(dtl, dtl.PKVal.ToString(), map);
            try
            {
                dtl.Update();
            }
            catch (Exception ex)
            {
                Log.DebugWriteInfo(ex.Message);
                msg += "<hr>" + ex.Message;
            }
        }

        // BP.Sys.MapDtl
        en = this.ucsys1.Copy(en, "0", map);
        if (en.IsBlank == false )
        {
            if (en.IsNoEntity)
            {
                if (en.EnMap.IsAutoGenerNo)
                    en.SetValByKey("No", en.GenerNewNoByKey("No"));
            }

            try
            {
                if (en.PKVal.ToString() == "0")
                {
                }
                else
                {
                    en.Insert();
                }
            }
            catch (Exception ex)
            {
                //异常处理..
                Log.DebugWriteInfo(ex.Message);
                //msg += "<hr>" + ex.Message;
            }
        }

        if (msg != null)
        {
            this.Session["info1"] = msg;
        //this.ResponseWriteRedMsg(msg);
            this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&PageIdx=" + this.PageIdx, true);
        }
        else
        {
            this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&PageIdx=" + this.PageIdx, true);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void ExpExcel()
    {
        //Entity mdtl = this.HisEn;
        //this.GenerLabel(this.Label1, mdtl.EnDesc);
        //this.Title = mdtl.Name;

        //GEDtls dtls = this.HisEns;
        //QueryObject qo = null;
        //qo = new QueryObject(dtls);
        //qo.DoQuery();

        ////DataTable dt = dtls.ToDataTableDesc();
        //// this.GenerExcel(dtls.ToDataTableDesc(), mdtl.Name + ".xls");
        //this.GenerExcel_pri_Text(dtls.ToDataTableDesc(), mdtl.Name + "@" + WebUser.No + "@" + DataType.CurrentData + ".xls");

        //this.ExportDGToExcelV2(dtls, this.Title + ".xls");
        //dtls.GetNewEntity.CheckPhysicsTable();
        //this.Response.Redirect("Ens.aspx?EnsName=" + this.EnsName + "&RefPKVal=" + this.RefPKVal, true);
    }

    void ToolBar1_ButtonClick(object sender, EventArgs e)
    {
        var btn = sender as LinkBtn;
        switch (btn.ID)
        {
            case NamesOfBtn.New:
            case NamesOfBtn.Save:
            case NamesOfBtn.SaveAndNew:
                this.Save();
                break;
            case NamesOfBtn.SaveAndClose:
                this.Save();
                this.WinClose();
                break;
            case NamesOfBtn.Delete:
                BP.En.Entities dtls = this.HisEns;
                QueryObject qo = new QueryObject(dtls);
                qo.DoQuery(dtls.GetNewEntity.PK, BP.Sys.SystemConfig.PageSize, this.PageIdx, false);
                string msg1 = "";
                foreach (BP.En.Entity dtl in dtls)
                {
                    CheckBox cb = this.ucsys1.GetCBByID("IDX_" + dtl.PKVal);
                    if (cb == null)
                        continue;
                    try
                    {
                        if (cb.Checked)
                            dtl.Delete();
                    }
                    catch (Exception ex)
                    {
                        msg1 += "@删除期间错误:" + ex.Message;
                    }
                }

                if (msg1 != "")
                    this.Alert(msg1);

                this.ucsys1.Clear();
                this.Bind();
                break;
            case NamesOfBtn.Excel:
                this.ExpExcel();
                break;
            default:
                BP.Sys.PubClass.Alert("当前版本不支持此功能。");
                break;
        }
    }
    /// <summary>
    /// 生成列的计算
    /// </summary>
    /// <param name="pk"></param>
    /// <param name="attrs"></param>
    /// <param name="attr"></param>
    /// <returns></returns>
    public string GenerAutoFull(string pk, MapAttrs attrs, MapAttr attr)
    {
        return "";
        //string left = "\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value = ";
        //string right = attr.AutoFullDoc;
        //foreach (MapAttr mattr in attrs)
        //{
        //    string tbID = "TB_" + mattr.Key + "_" + pk;
        //    TB tb = this.ucsys1.GetTBByID(tbID);
        //    if (tb == null)
        //        continue;
        //    right = right.Replace("@" + mattr.Name, " parseFloat( document.forms[0]." + this.ucsys1.GetTBByID(tbID).ClientID + ".value.replace( ',' ,  '' ) ) ");
        //    right = right.Replace("@" + mattr.Key, " parseFloat( document.forms[0]." + this.ucsys1.GetTBByID(tbID).ClientID + ".value.replace( ',' ,  '' ) ) ");
        //}

        //string s = left + right;
        //s += "\t\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value= VirtyMoney(document.forms[0]." + this.ucsys1.GetTBByID("TB_" + attr.Key + "_" + pk).ClientID + ".value ) ;";
        //return s += " C" + attr.Key + "();";
    }

    public string GenerSum(MapAttr mattr, GEDtls dtls)
    {
        return "";
        //string left = "\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + mattr.Key).ClientID + ".value = ";
        //string right = "";
        //int i = 0;
        //foreach (GEDtl dtl in dtls)
        //{
        //    string tbID = "TB_" + mattr.Key + "_" + dtl.PKVal;
        //    TB tb = this.ucsys1.GetTBByID(tbID);
        //    if (i == 0)
        //        right += " parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";
        //    else
        //        right += " +parseFloat( document.forms[0]." + tb.ClientID + ".value.replace( ',' ,  '' ) )  ";

        //    i++;
        //}

        //string s = left + right + " ;";
        //switch (mattr.MyDataType)
        //{
        //    case BP.DA.DataType.AppMoney:
        //    case BP.DA.DataType.AppRate:
        //        return s += "\t\n  document.forms[0]." + this.ucsys1.GetTBByID("TB_" + mattr.Key).ClientID + ".value= VirtyMoney(document.forms[0]." + this.ucsys1.GetTBByID("TB_" + mattr.Key).ClientID + ".value ) ;";
        //    default:
        //        return s;
        //}
    }

}
