using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.DA;
using BP.Web.Controls;

public partial class CCFlow_Comm_Sys_EnumList : BP.Web.WebPageAdmin
{
    public void BindRefNo()
    {
        SysEnumMain sem = new SysEnumMain(this.RefNo);

        this.UCSys1.AddTableNormal();
        this.UCSys1.AddTRGroupTitle(3,
                                    "<a href='EnumList.aspx?T=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                    "'>列表</a> - <a href='EnumList.aspx?DoType=New&T=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                    "'>新建</a> - 编辑：<b>" + sem.Name + "</b>");

        this.UCSys1.AddTR();
        LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.AddTDGroupTitle("colspan=3", btn);
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTR();
        // this.UCSys1.AddTDTitle("序");
        this.UCSys1.AddTDGroupTitle("项目");
        this.UCSys1.AddTDGroupTitle("采集");
        this.UCSys1.AddTDGroupTitle("说明");
        this.UCSys1.AddTREnd();

        SysEnums ses = new SysEnums();
        ses.Retrieve(SysEnumAttr.EnumKey, this.RefNo, SysEnumAttr.IntKey);

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("style='text-align:right; width:60px'", "编号");
        TextBox tb = new TextBox();
        tb.ID = "TB_No";
        tb.Text = this.RefNo;
        tb.Enabled = false;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("不可修改");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("style='text-align:right; width:60px'", "名称");
        tb = new TextBox();
        tb.ID = "TB_Name";
        tb.Text = sem.Name;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("");
        this.UCSys1.AddTREnd();

        int myNum = 0;
        foreach (SysEnum se in ses)
        {
            this.UCSys1.AddTR();
            this.UCSys1.AddTD(se.IntKey);
            tb = new TextBox();
            tb.ID = "TB_" + se.IntKey;
            tb.Text = se.Lab;
            tb.Columns = 50;
            this.UCSys1.AddTD(tb);
            this.UCSys1.AddTD("");
            this.UCSys1.AddTREnd();
            myNum = se.IntKey;
        }

        myNum++;
        //每次追加10个
        for (int i = myNum; i < 10 + myNum; i++)
        {
            this.UCSys1.AddTR();
            this.UCSys1.AddTD(i);
            tb = new TextBox();
            tb.ID = "TB_" + i;
            tb.Columns = 50;
            this.UCSys1.AddTD(tb);
            this.UCSys1.AddTD("");
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }
    public void BindNew()
    {
        //this.UCSys1.AddTable();
        this.UCSys1.AddTableNormal();
        this.UCSys1.AddTRGroupTitle(3,
                                     "<a href='EnumList.aspx?T=" +
                                     DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                     "'>列表</a> - <b>新建</b>");
        //this.UCSys1.AddCaptionLeftTX("<a href=EnumList.aspx ><img src='./../../Img/Btn/Home.gif' border=0 />枚举值列表</a> - <img src='./../../Img/Btn/New.gif' />新建枚举值");

        this.UCSys1.AddTR();
        //Button btn = new Button();
        //btn.ID = "Btn_Save";
        //btn.CssClass = "Btn";
        //btn.Text = "  Save  ";
        LinkBtn btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
        btn.Click += new EventHandler(btn_New_Click);
        this.UCSys1.AddTDGroupTitle("colspan=3", btn);
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTR();
        this.UCSys1.AddTDGroupTitle("项目");
        this.UCSys1.AddTDGroupTitle("采集");
        this.UCSys1.AddTDGroupTitle("说明");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("style='text-align:right; width:60px'", "编号");
        TextBox tb = new TextBox();
        tb.ID = "TB_No";
        tb.Columns = 50;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("编号系统唯一并且以字母或下划线开头");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("style='text-align:right; width:60px'", "名称");
        tb = new TextBox();
        tb.ID = "TB_Name";
        tb.Columns = 50;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("不能为空");
        this.UCSys1.AddTREnd();
        for (int i = 0; i < 20; i++)
        {
            this.UCSys1.AddTR();
            this.UCSys1.AddTD(i);
            tb = new TextBox();
            tb.ID = "TB_" + i;
            tb.Columns = 50;
            this.UCSys1.AddTD(tb);
            this.UCSys1.AddTD("");
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }

    void btn_Click(object sender, EventArgs e)
    {
        //原有个数
        SysEnums souceSes = new SysEnums();
        souceSes.Retrieve(SysEnumAttr.EnumKey, this.RefNo, SysEnumAttr.IntKey);

        SysEnums ses = new SysEnums();
        for (int i = 0; i < souceSes.Count + 10; i++)
        {
            TextBox tb = this.UCSys1.GetTextBoxByID("TB_" + i);
            if (tb == null)
                continue;
            if (string.IsNullOrEmpty(tb.Text))
                continue;

            SysEnum se = new SysEnum();
            se.IntKey = i;
            se.Lab = tb.Text.Trim();
            se.Lang = BP.Web.WebUser.SysLang;
            se.EnumKey = this.RefNo;
            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
            ses.AddEntity(se);
        }

        if (ses.Count == 0)
        {
            this.Alert("枚举项目不能为空.");
            return;
        }

        ses.Delete(SysEnumAttr.EnumKey, this.RefNo);

        string lab = "";
        foreach (SysEnum se in ses)
        {
            se.Save();
            lab += "@" + se.IntKey + "=" + se.Lab;
        }
        SysEnumMain main = new SysEnumMain(this.RefNo);
        main.Name = UCSys1.GetTextBoxByID("TB_Name").Text;
        main.CfgVal = lab;
        main.Update();
        this.Alert("保存成功.");
    }

    void btn_New_Click(object sender, EventArgs e)
    {
        string no = this.UCSys1.GetTextBoxByID("TB_No").Text;
        string name = this.UCSys1.GetTextBoxByID("TB_Name").Text;
        SysEnumMain m = new SysEnumMain();
        m.No = no;
        if (m.RetrieveFromDBSources() == 1)
        {
            this.Alert("枚举编号:" + m.No + " 已经被:" + m.Name + "占用");
            return;
        }
        m.Name = name;
        if (string.IsNullOrEmpty(name))
        {
            this.Alert("枚举名称不能为空");
            return;
        }

        SysEnums ses = new SysEnums();
        for (int i = 0; i < 20; i++)
        {
            TextBox tb = this.UCSys1.GetTextBoxByID("TB_" + i);
            if (tb == null)
                continue;
            if (string.IsNullOrEmpty(tb.Text))
                continue;

            SysEnum se = new SysEnum();
            se.IntKey = i;
            se.Lab = tb.Text.Trim();
            se.Lang = BP.Web.WebUser.SysLang;
            se.EnumKey = m.No;
            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
            ses.AddEntity(se);
        }

        if (ses.Count == 0)
        {
            this.Alert("枚举项目不能为空.");
            return;
        }

        string lab = "";
        foreach (SysEnum se in ses)
        {
            se.Save();
            lab += "@" + se.IntKey + "=" + se.Lab;
        }

        m.Lang = BP.Web.WebUser.SysLang;
        m.CfgVal = lab;
        m.Insert();
        this.Response.Redirect("EnumList.aspx?RefNo=" + m.No + "&T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "枚举值编辑";
        if (this.DoType == "Del")
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.RefNo);

            this.UCSys1.AddEasyUiPanelInfoBegin("删除确认", "icon-delete");

            if (attrs.Count != 0)
            {
                //this.UCSys1.AddFieldSet("<a href='EnumList.aspx' ><img src='./../../Img/Btn/Home.gif' border=0/>返回列表</a> - 删除确认");
                //this.UCSys1.Add("此枚举值已经被其它的字段所引用，您不能删除它。");
                //this.UCSys1.AddH2("<a href='EnumList.aspx' >返回列表</a>");
                //this.UCSys1.AddFieldSetEnd();
                this.UCSys1.Add("<b>此枚举值已经被其它的字段所引用，您不能删除它。</b>");
                this.UCSys1.Add("<a class='easyui-linkbutton' data-options=\"iconCls:'icon-back'\" href='EnumList.aspx?T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "' >返回列表</a>");
                this.UCSys1.AddEasyUiPanelInfoEnd();
                return;
            }

            //this.UCSys1.AddFieldSet("<a href='EnumList.aspx' ><img src='./../../Img/Btn/Home.gif' border=0/>返回列表</a> - 删除确认");
            SysEnumMain m = new SysEnumMain(this.RefNo);
            this.UCSys1.AddEasyUiLinkButton("确定删除：" + m.Name + "？", "EnumList.aspx?RefNo=" + this.RefNo + "&DoType=DelReal&T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                            "icon-delete");
            this.UCSys1.AddSpace(1);
            this.UCSys1.AddEasyUiLinkButton("取消", "EnumList.aspx?T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                                            "icon-undo");
            this.UCSys1.AddEasyUiPanelInfoEnd();
            return;
        }

        if (this.DoType == "DelReal")
        {
            SysEnumMain m = new SysEnumMain();
            m.No = this.RefNo;
            m.Delete();
            SysEnums ses = new SysEnums();
            ses.Delete(SysEnumAttr.EnumKey, this.RefNo);
            //this.Response.Redirect("EnumList.aspx", true);
            this.Response.Redirect("EnumList.aspx?T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), true);
            return;
        }

        if (this.DoType == "New")
        {
            this.BindNew();
            return;
        }

        if (this.RefNo != null)
        {
            this.BindRefNo();
            return;
        }

        //this.UCSys1.AddTable();
        //this.UCSys1.AddCaptionLeftTX("<img src='./../../Img/Btn/Home.gif' border=0/>列表 - <a href='EnumList.aspx?DoType=New' ><img border=0 src='./../../Img/Btn/New.gif' >新建</a>");
        this.UCSys1.AddTableNormal();
        this.UCSys1.AddTRGroupTitle(5,
                                    "<b>列表</b> - <a href='EnumList.aspx?DoType=New&T=" +
                                    DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                                    "'>新建</a>");
        this.UCSys1.AddTR();
        this.UCSys1.AddTDGroupTitleCenter("序");
        this.UCSys1.AddTDGroupTitle("编号");
        this.UCSys1.AddTDGroupTitle("名称");
        this.UCSys1.AddTDGroupTitle("信息");
        this.UCSys1.AddTDGroupTitle("操作");
        this.UCSys1.AddTREnd();

        SysEnumMains sems = new SysEnumMains();
        sems.RetrieveAll();
        int i = 0;
        foreach (SysEnumMain se in sems)
        {
            i++;
            //this.UCSys1.AddTR();
            this.UCSys1.AddTR(i % 2 == 0);
            this.UCSys1.AddTDIdx(i);
            this.UCSys1.AddTD(se.No);
            this.UCSys1.AddTDA("EnumList.aspx?RefNo=" + se.No + "&T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), se.Name);
            this.UCSys1.AddTD(se.CfgVal);
            //this.UCSys1.AddTDA("EnumList.aspx?RefNo=" + se.No + "&DoType=Del", "<img src='./../../Img/Btn/Delete.gif' border=0 />删除");
            this.UCSys1.AddTDBegin();
            this.UCSys1.AddEasyUiLinkButton("删除", "EnumList.aspx?RefNo=" + se.No + "&DoType=Del&T=" + DateTime.Now.ToString("yyyyMMddHHmmssfff"), "icon-delete");
            this.UCSys1.AddTDEnd();
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }
}