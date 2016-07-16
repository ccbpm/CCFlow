using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.DA;

public partial class CCFlow_Comm_Sys_SFTableList : BP.Web.WebPageAdmin
{
    public void BindIt()
    {
        SFTable sem = new SFTable(this.RefNo);

        this.UCSys1.AddTable("width=500px  ");
        this.UCSys1.AddCaptionLeft("<a href=SFTableList.aspx ><img src='./../../Img/Btn/Home.gif' border=0>列表</a> -<a href='SFTableList.aspx?DoType=New' ><img src='./../../Img/Btn/New.gif' border=0/>新建</a>- <img src='./../../Img/Btn/Edit.gif' border />编辑:" + sem.No + " " + sem.Name);
        this.UCSys1.AddTR();
        this.UCSys1.AddTDTitle("项目");
        this.UCSys1.AddTDTitle("采集");
        this.UCSys1.AddTDTitle("说明");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("编号");
        TextBox tb = new TextBox();
        tb.ID = "TB_No";
        tb.Text = this.RefNo;
        tb.Enabled = false;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("不可修改");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("名称");
        tb = new TextBox();
        tb.ID = "TB_Name";
        tb.Text = sem.Name;
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTR();
        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.CssClass = "Btn";
        btn.Text = " Save  ";
        btn.Click += new EventHandler(btn_Click);
        this.UCSys1.AddTD("colspan=2", btn);
        if (this.RefNo.Contains(".") == false)
            this.UCSys1.AddTD("<a href='./../FoolFormDesigner/SFTableEditData.aspx?RefNo=" + this.RefNo + "' >编辑数据</a>");
        else
            this.UCSys1.AddTD("<a href='./../Ens.aspx?EnsName=" + this.RefNo + "' >编辑数据</a>");

        this.UCSys1.AddTREnd();
        this.UCSys1.AddTableEnd();
    }
    public void BindNew()
    {
        this.UCSys1.AddTable("width=500px class=Table");
        this.UCSys1.AddCaptionLeft("<a href=SFTableList.aspx ><img src='./../../Img/Btn/Home.gif' border=0 />列表</a> - <img src='./../../Img/Btn/New.gif' />新建编码表");

        this.UCSys1.AddTR();
        this.UCSys1.AddTDTitle("项目");
        this.UCSys1.AddTDTitle("采集");
        this.UCSys1.AddTDTitle("说明");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("编号");
        TextBox tb = new TextBox();
        tb.ID = "TB_No";
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("编号系统唯一并且以字母或下划线开头");
        this.UCSys1.AddTREnd();

        this.UCSys1.AddTRSum();
        this.UCSys1.AddTD("名称");
        tb = new TextBox();
        tb.ID = "TB_Name";
        this.UCSys1.AddTD(tb);
        this.UCSys1.AddTD("不能为空");
        this.UCSys1.AddTREnd();

        //this.UCSys1.AddTRSum();
        //this.UCSys1.AddTD("类型");
        //DropDownList ddl =new DropDownList();
        //ddl.ID = "DDL_Type";
        //ddl.Items.Add(new ListItem("物理表", "0"));
        //ddl.Items.Add(new ListItem("视图", "0"));
        //this.UCSys1.AddTD(ddl);
        //this.UCSys1.AddTD("");
        //this.UCSys1.AddTREnd();

        //this.UCSys1.AddTRSum();
        //this.UCSys1.AddTD("colspan=3","视图SQL:(对类型是视图有效，查询语句必须包含No,Name两列.)");
        //this.UCSys1.AddTREnd();

        //this.UCSys1.AddTRSum();
        //tb = new TextBox();
        //tb.ID = "TB_SQL";
        //tb.TextMode = TextBoxMode.MultiLine;
        //tb.Columns = 70;
        //tb.Rows = 10;
        //this.UCSys1.AddTD("colspan=3",tb);
        //this.UCSys1.AddTREnd();


        this.UCSys1.AddTR();
        Button btn = new Button();
        btn.ID = "Btn_Save";
        btn.Text = "  Save  ";
        btn.CssClass = "Btn";
        btn.Click += new EventHandler(btn_New_Click);
        this.UCSys1.AddTD("colspan=3", btn);
        this.UCSys1.AddTREnd();
        this.UCSys1.AddTableEnd();
    }

    void btn_Click(object sender, EventArgs e)
    {
        string no = this.UCSys1.GetTextBoxByID("TB_No").Text;
        string name = this.UCSys1.GetTextBoxByID("TB_Name").Text;
        SFTable m = new SFTable();
        m.No = no;
        m.RetrieveFromDBSources();
        m.Name = name;
        if (string.IsNullOrEmpty(name))
        {
            this.Alert("编码表名称不能为空");
            return;
        }
      //  m.HisSFTableType = SFTableType.SFTable;
        m.Save();
        this.Response.Redirect("SFTableList.aspx?RefNo=" + m.No, true);
         
    }

    void btn_New_Click(object sender, EventArgs e)
    {
        string no = this.UCSys1.GetTextBoxByID("TB_No").Text;
        string name = this.UCSys1.GetTextBoxByID("TB_Name").Text;
        SFTable m = new SFTable();
        m.No = no;
        if (m.RetrieveFromDBSources() == 1)
        {
            this.Alert("编码表编号:" + m.No + " 已经被:" + m.Name + "占用");
            return;
        }
        m.Name = name;
        if (string.IsNullOrEmpty(name))
        {
            this.Alert("编码表名称不能为空");
            return;
        }
        //  m.HisSFTableType = SFTableType.SFTable;
        m.Insert();
        this.Response.Redirect("SFTableList.aspx?RefNo=" + m.No, true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        this.Title = "编码表编辑";
        if (this.DoType == "Del")
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.UIBindKey, this.RefNo);
            if (attrs.Count != 0)
            {
                this.UCSys1.AddFieldSet("<a href='SFTableList.aspx' ><img src='./../../Img/Btn/Home.gif' border=0/>返回列表</a> - 删除确认");
                this.UCSys1.Add("此编码表已经被其它的字段所引用，您不能删除它。");
                this.UCSys1.AddH2("<a href='SFTableList.aspx' >返回列表</a>");
                this.UCSys1.AddFieldSetEnd();
                return;
            }

            this.UCSys1.AddFieldSet("<a href='SFTableList.aspx' ><img src='./../../Img/Btn/Home.gif' border=0/>返回列表</a> - 删除确认");
            SFTable m = new SFTable(this.RefNo);
            this.UCSys1.AddH2("<a href='SFTableList.aspx?RefNo=" + this.RefNo + "&DoType=DelReal' >删除:" + m.Name + " 确认.</a>");
            this.UCSys1.AddFieldSetEnd();
            return;
        }

        if (this.DoType == "DelReal")
        {
            SFTable m = new SFTable();
            m.No = this.RefNo;
            m.Delete();
            SFTables ses = new SFTables();
           // ses.Delete(SFTableAttr.SFTableKey, this.RefNo);
            this.Response.Redirect("SFTableList.aspx", true);
            return;
        }

        if (this.DoType == "New")
        {
            this.BindNew();
            return;
        }

        if (this.RefNo != null)
        {
            this.BindIt();
            return;
        }

        this.UCSys1.AddTable("class=Table width=500px");
        this.UCSys1.AddCaption("<img src='./../../Img/Btn/Home.gif' border=0/>列表 - <a href='SFTableList.aspx?DoType=New' ><img border=0 src='./../../Img/Btn/New.gif' >新建</a>");
        this.UCSys1.AddTR();
        this.UCSys1.AddTDTitle("序");
        this.UCSys1.AddTDTitle("编号");
        this.UCSys1.AddTDTitle("名称");
        //this.UCSys1.AddTDTitle("类型");
        this.UCSys1.AddTDTitle("描述");
        this.UCSys1.AddTDTitle("操作");
        this.UCSys1.AddTREnd();

        SFTables sems = new SFTables();
        sems.RetrieveAll();
        int i = 0;
        foreach (SFTable se in sems)
        {
            i++;
            this.UCSys1.AddTR();
            this.UCSys1.AddTDIdx(i);
            this.UCSys1.AddTD(se.No);
            this.UCSys1.AddTDA("SFTableList.aspx?RefNo=" + se.No, se.Name);
          //  this.UCSys1.AddTD(se.SFTableTypeT);
            this.UCSys1.AddTD(se.TableDesc);

            this.UCSys1.AddTDA("SFTableList.aspx?RefNo=" + se.No + "&DoType=Del", "<img src='./../../Img/Btn/Delete.gif' border=0 />删除");

            //switch (se.HisSFTableType)
            //{
            //    case SFTableType.SFTable:
            //        this.UCSys1.AddTDA("SFTableList.aspx?RefNo=" + se.No + "&DoType=Del", "<img src='./../../Img/Btn/Delete.gif' border=0 />删除");
            //        break;
            //    case SFTableType.ClsLab:
            //    case SFTableType.SysTable:
            //    default:
            //        this.UCSys1.AddTD();
            //        break;
            //}
            this.UCSys1.AddTREnd();
        }
        this.UCSys1.AddTableEnd();
    }
}