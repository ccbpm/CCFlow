using System;
using System.Linq;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Web;
using BP.Sys;
using BP.Port;
using BP.DA;
using BP.Sys.XML;
using BP.En;
using BP.Web.Controls;

public partial class CCFlow_Comm_Sys_EnsAppCfg : BP.Web.WebPageAdmin
{

    public new string EnsName
    {
        get
        {
            if (this.Request.QueryString["EnsName"] == null)
                return "BP.GE.Infos";
            return this.Request.QueryString["EnsName"];
        }
    }
    /// <summary>
    /// 获取当前页面是否存在于easyui-dialog的层中的标识inlayer，在层中时inlayer="1"
    /// </summary>
    public string InLayer
    {
        get
        {
            return Request.QueryString["inlayer"];
        }
    }
    public void BindSelectCols()
    {
        //this.UCSys1.AddTable("width=100%");
        this.UCSys1.AddTableNormal();
        this.UCSys1.AddTRGroupTitle(3,
                                    "<a href='?EnsName=" + this.EnsName + "&t=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") + "&inlayer=" + this.InLayer +
                                    "'>基本配置</a> - <b>选择列</b> - <a href='EnsDataIO.aspx?EnsName=" + this.EnsName +
                                    "&t=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") + "&inlayer=" + this.InLayer +
                                    "' >数据导入导出</a>");

        //this.UCSys1.AddCaptionLeftTX("<a href='?EnsName=" + this.EnsName + "&T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "'>基本配置</a> - <b>选择列</b> - <a href='EnsDataIO.aspx?EnsName=" + this.EnsName + "' >数据导入导出</a>");

        this.UCSys1.AddTR();
        this.UCSys1.AddTDGroupTitle("style='width:60px;text-align:center'", "序");
        this.UCSys1.AddTDGroupTitle("style='width:40px;text-align:center'", "<input type='checkbox' onclick=\"javascript:$('input[type=checkbox]').attr('checked', this.checked)\" />");
        this.UCSys1.AddTDGroupTitle("列");
        this.UCSys1.AddTREnd();

        Entity en = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;
        Attrs attrs = en.EnMap.HisCfgAttrs;
        UIConfig cfg = new UIConfig(en);
        var showColumns = cfg.ShowColumns;
        bool is1 = false;
        var idx = 1;
        CheckBox cb = null;

        foreach (Attr attr in en.EnMap.Attrs)
        {
            if (attr.UIVisible == false)
                continue;

            if (attr.Key == "MyNum")
                continue;

            is1 = this.UCSys1.AddTR(is1);
            this.UCSys1.AddTDIdx(idx++);

            cb = new CheckBox();
            cb.ID = "CB_" + attr.Key;
            cb.Checked = showColumns.Contains(attr.Key);
            this.UCSys1.AddTD("style='text-align:center'", cb);

            this.UCSys1.AddTD("<label for='" + cb.ClientID + "'>" + attr.Desc + "</label>");

            this.UCSys1.AddTREnd();
        }

        this.UCSys1.AddTableEnd();
        this.UCSys1.AddBR();
        this.UCSys1.AddSpace(1);

        //var btns = new Button();
        LinkBtn btns = new LinkBtn(false, NamesOfBtn.Save, "保存");
        //btns.ID = "Btn_Save";
        //btns.CssClass = "Btn";
        //btns.Text = "保存";
        btns.Click += new EventHandler(btns_Click);
        this.UCSys1.Add(btns);
        this.UCSys1.AddSpace(1);

        //btns = new Button();
        btns = new LinkBtn(false, NamesOfBtn.SaveAndClose, "保存并关闭");
        //btns.ID = "Btn_SaveAndClose";
        //btns.CssClass = "Btn";
        //btns.Text = "保存并关闭";
        btns.Click += new EventHandler(btns_Click);
        this.UCSys1.Add(btns);
    }

    void btns_Click(object sender, EventArgs e)
    {
        if (WebUser.No != "admin") return;

        Entity en = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;
        UIConfig cfg = new UIConfig(en);

        if (string.IsNullOrWhiteSpace(this.DoType) == false)
        {
            var val = string.Empty;
            CheckBox cb = null;

            foreach (Control ctrl in this.UCSys1.Controls)
            {
                cb = ctrl as CheckBox;
                if (cb == null || cb.Checked == false) continue;

                val += cb.ID.Substring("CB_".Length) + ",";
            }

            cfg.HisAP.SetVal("ShowColumns", val);
            cfg.Save();
        }

        //Button btn = sender as Button;
        LinkBtn btn = sender as LinkBtn;
        if (btn.ID.Contains("Close"))
            this.WinClose("");
    }

    public void BindAdv()
    {
        //this.UCSys1.AddTable("width=100%");
        this.UCSys1.AddTableNormal();
        this.UCSys1.AddTRGroupTitle(3,
                                    "<b>基本配置</b> - <a href='?EnsName=" + this.EnsName + "&DoType=SelectCols&t=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") + "&inlayer=" + this.InLayer +
                                    "'>选择列</a> - <a href='EnsDataIO.aspx?EnsName=" + this.EnsName + "&t=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") + "&inlayer=" + this.InLayer +
                                    "' >数据导入导出</a>");
        //this.UCSys1.AddCaptionLeftTX("<b>基本配置</b> - <a href='?EnsName=" + this.EnsName + "&DoType=SelectCols&T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "'>选择列</a> - <a href='EnsDataIO.aspx?EnsName=" + this.EnsName + "' >数据导入导出</a>");

        this.UCSys1.AddTR();
        this.UCSys1.AddTDGroupTitle("配置项");
        this.UCSys1.AddTDGroupTitle("内容");
        this.UCSys1.AddTDGroupTitle("信息");
        this.UCSys1.AddTREnd();

        Entity en1 = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;
        Attrs attrs = en1.EnMap.HisCfgAttrs;
        UIConfig cfg = new UIConfig(en1);
        bool is1 = false;
        foreach (Attr attr in attrs)
        {
            if (attr.IsRefAttr)
                continue;

            if (attr.UIVisible == false)    //added by liuxc,2015-8-7
                continue;

            is1 = this.UCSys1.AddTR(is1);
            this.UCSys1.AddTD(attr.Key);
            this.UCSys1.AddTD(attr.Desc);
            switch (attr.UIContralType)
            {
                case UIContralType.DDL:
                    BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
                    ddl.ID = "DDL_" + attr.Key;

                    SysEnums ses = new SysEnums(attr.Key, attr.UITag);
                    ddl.BindSysEnum(attr.Key);

                    if (DataType.IsNullOrEmpty(cfg.HisAP.GetValStrByKey(attr.Key)))
                        ddl.SetSelectItem(attr.DefaultVal.ToString());
                    else
                        ddl.SetSelectItem(cfg.HisAP.GetValIntByKey(attr.Key));

                    this.UCSys1.AddTD(ddl);
                    break;
                case UIContralType.CheckBok:
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + attr.Key;
                    cb.Text = attr.Desc;
                    if (DataType.IsNullOrEmpty(cfg.HisAP.GetValStrByKey(attr.Key)))
                    {
                        if (attr.DefaultVal.ToString() == "0")
                            cb.Checked = false;
                        else
                            cb.Checked = true;
                    }
                    else
                    {
                        cb.Checked = cfg.HisAP.GetValBoolenByKey(attr.Key);// en.CfgValOfBoolen;
                    }
                    this.UCSys1.AddTD(cb);
                    break;
                default:
                    TextBox tb = new TextBox();
                    tb.ID = "TB_" + attr.Key;
                    if (cfg.HisAP.GetValStrByKey(attr.Key) == null)
                        tb.Text = attr.DefaultVal.ToString();
                    else
                        tb.Text = cfg.HisAP.GetValStrByKey(attr.Key);
                    tb.Attributes["width"] = "100%";
                    this.UCSys1.AddTD(tb);
                    break;
            }
            this.UCSys1.AddTREnd();
        }

        this.UCSys1.AddTableEnd();
        this.UCSys1.AddBR();
        this.UCSys1.AddSpace(1);

        //Button btn = new Button();
        LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
        //btn.ID = "Btn_Save";
        //btn.CssClass = "Btn";
        //btn.Text = "保存";
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.Add(btn);
        this.UCSys1.AddSpace(1);

        //btn = new Button();
        btn = new LinkBtn(false, NamesOfBtn.SaveAndClose, "保存并关闭");
        //btn.ID = "Btn_SaveAndClose";
        //btn.CssClass = "Btn";
        //btn.Text = "保存并关闭";
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.Add(btn);
    }
    public void BindNormal()
    {
        //EnsAppCfgs ens = new EnsAppCfgs();
        //ens.Retrieve(EnsAppCfgAttr.EnsName, this.EnsName);

        Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);

        UIConfig cfg = new UIConfig(ens.GetNewEntity);

        EnsAppXmls xmls = new EnsAppXmls();
        xmls.Retrieve("EnsName", this.EnsName);

        //this.UCSys1.AddTable("width=100%");
        this.UCSys1.AddTableNormal();

        if (BP.Web.WebUser.No == "admin")
        {
            //this.UCSys1.AddCaptionLeftTX("<a href='?EnsName=" + this.EnsName + "'>基本设置</a> - <a href='?EnsName=" + this.EnsName + "&DoType=Adv'>高级设置</a> - <a href='EnsDataIO.aspx?EnsName=" + this.EnsName + "' >导入导出</a>");
            this.UCSys1.AddTRGroupTitle(4,
                                        "<a href='?EnsName=" + this.EnsName + "'>基本设置</a> - <a href='?EnsName=" +
                                        this.EnsName + "&DoType=Adv&inlayer=" + this.InLayer + "'>高级设置</a> - <a href='EnsDataIO.aspx?EnsName=" +
                                        this.EnsName + "&t=" +
                                        DateTime.Now.ToString("yyyyMMddHHmmssfff") + "&inlayer=" + this.InLayer +
                                        "' >导入导出</a>");
        }
        else
        {
            //this.UCSys1.AddCaptionLeftTX("基本设置");
            this.UCSys1.AddTRGroupTitle(4, "基本设置");
        }

        this.UCSys1.AddTR();

        this.UCSys1.AddTR();
        this.UCSys1.AddTDGroupTitle("配置项");
        this.UCSys1.AddTDGroupTitle("内容");
        this.UCSys1.AddTDGroupTitle("信息");
        this.UCSys1.AddTDGroupTitle("备注");
        this.UCSys1.AddTREnd();

        bool is1 = false;
        foreach (EnsAppXml xml in xmls)
        {
            is1 = this.UCSys1.AddTR(is1);
            this.UCSys1.AddTD(xml.No);
            this.UCSys1.AddTD(xml.Name);
            switch (xml.DBType)
            {
                case "Enum":
                    BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
                    ddl.ID = "DDL_" + xml.No;

                    SysEnums ses = new SysEnums(xml.EnumKey, xml.EnumVals);
                    ddl.BindSysEnum(xml.EnumKey);

                    if (cfg.HisAP.GetValStrByKey(xml.No) == null)
                        ddl.SetSelectItem(xml.DefValInt);
                    else
                        ddl.SetSelectItem(cfg.HisAP.GetValIntByKey(xml.No));
                    this.UCSys1.AddTD(ddl);
                    break;
                case "Boolen":
                    CheckBox cb = new CheckBox();
                    cb.ID = "CB_" + xml.No;
                    cb.Text = xml.Name;
                    if (cfg.HisAP.GetValStrByKey(xml.No) == null)
                        cb.Checked = xml.DefValBoolen;
                    else
                        cb.Checked = cfg.HisAP.GetValBoolenByKey(xml.No);
                    this.UCSys1.AddTD(cb);
                    break;
                default:
                    TextBox tb = new TextBox();
                    tb.ID = "TB_" + xml.No;
                    if (cfg.HisAP.GetValStrByKey(xml.No) == null)
                        tb.Text = xml.DefVal;
                    else
                        tb.Text = cfg.HisAP.GetValStrByKey(xml.No);
                    tb.Attributes["width"] = "100%";
                    this.UCSys1.AddTD(tb);
                    break;
            }
            this.UCSys1.AddTDBigDoc(xml.Desc);
            this.UCSys1.AddTREnd();
        }

        if (xmls.Count == 0)
        {
            this.UCSys1.AddTableEnd();
            return;
        }

        this.UCSys1.AddTableEnd();
        this.UCSys1.AddBR();
        this.UCSys1.AddSpace(1);

        //Button btn = new Button();
        LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
        //btn.ID = "Btn_Save";
        //btn.Text = "保存";
        //btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.Add(btn);
        this.UCSys1.AddSpace(1);

        //btn = new Button();
        btn = new LinkBtn(false, NamesOfBtn.SaveAndClose, "保存并关闭");
        //btn.ID = "Btn_SaveAndClose";
        //btn.CssClass = "Btn";
        //btn.Text = "保存并关闭";
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.Add(btn);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        switch (this.DoType)
        {
            case "SelectCols":
                this.BindSelectCols();
                break;
            default:
                this.BindAdv();
                //this.BindNormal();
                break;
        }
    }
    void btn_Click(object sender, EventArgs e)
    {
        //if (this.DoType == null)
        //{
        //    UIConfig cfg=new UIConfig( BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity);
        //    EnsAppXmls xmls = new EnsAppXmls();
        //    xmls.Retrieve("EnsName", this.EnsName);
        //    foreach (EnsAppXml xml in xmls)
        //    {
        //        string val = "";
        //        switch (xml.DBType)
        //        {
        //            case "Enum":
        //                val = this.UCSys1.GetDDLByID("DDL_" + xml.No).SelectedItemStringVal;
        //                break;
        //            case "Boolen":
        //                if (this.UCSys1.GetCBByID("CB_" + xml.No).Checked)
        //                    val = "1";
        //                else
        //                    val = "0";
        //                break;
        //            default:
        //                val = this.UCSys1.GetTextBoxByID("TB_" + xml.No).Text;
        //                break;
        //        }

        //        cfg.HisAP.SetVal(xml.No, val);

        //        cfg.Save();
        //    }
        //}

        if (WebUser.No == "admin")
        {
            Entity en1 = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;
            Attrs attrs = en1.EnMap.HisCfgAttrs;

            UIConfig cfg = new UIConfig(en1);

            if (string.IsNullOrWhiteSpace(this.DoType))
            {
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr)
                        continue;

                    if (attr.UIVisible == false)
                        continue;

                    string val = "";
                    switch (attr.UIContralType)
                    {
                        case UIContralType.DDL:
                            val = this.UCSys1.GetDDLByID("DDL_" + attr.Key).SelectedItemStringVal;
                            break;
                        case UIContralType.CheckBok:
                            if (this.UCSys1.GetCBByID("CB_" + attr.Key).Checked)
                                val = "1";
                            else
                                val = "0";
                            break;
                        default:
                            val = this.UCSys1.GetTextBoxByID("TB_" + attr.Key).Text;
                            break;
                    }

                    cfg.HisAP.SetVal(attr.Key, val);
                }

                cfg.Save();
            }
        }

        //Button btn = sender as Button;
        LinkBtn btn = sender as LinkBtn;
        if (btn.ID.Contains("Close"))
            this.WinClose("");
    }
}
