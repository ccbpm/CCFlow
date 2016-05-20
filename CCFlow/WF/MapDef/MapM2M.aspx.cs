using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_MapM2M : BP.Web.WebPage
    {

        #region 属性
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string NoOfObj
        {
            get
            {
                return this.Request.QueryString["NoOfObj"];
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            MapData md = new MapData(this.FK_MapData);
            this.Title = md.Name + " - 设计一对多";
            MapM2M m2m = new MapM2M(this.FK_MapData, this.NoOfObj);
            if (m2m.HisM2MType == M2MType.M2MM)
            {
                this.Response.Redirect("MapM2MM.aspx?FK_MapData=" + this.FK_MapData + "&NoOfObj=" + this.NoOfObj, true);
                return;
            }

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("一对多属性");
            int idx = 1;
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("编号");
            TB tb = new TB();
            tb.ID = "TB_" + MapM2MAttr.NoOfObj;
            tb.Text = m2m.NoOfObj;
            if (m2m.IsExits)
                tb.Enabled = false;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("描述");
            tb = new TB();
            tb.ID = "TB_" + MapM2MAttr.Name;
            tb.Text = m2m.Name;
            tb.Columns = 50;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("主体数据源<font color=red>*</font>");
            tb = new TB();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 3;
            tb.ID = "TB_" + MapM2MAttr.DBOfObjs;
            tb.Text = m2m.DBOfObjs;
            tb.Columns = 50;

            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();
            this.Pub1.AddTREnd();


            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("分组数据源");
            tb = new TB();
            tb.ID = "TB_" + MapM2MAttr.DBOfGroups;
            tb.Text = m2m.DBOfGroups;
            tb.Columns = 50;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 3;
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("框架宽度");
            tb = new TB();
            tb.ID = "TB_" + MapM2MAttr.W;
            tb.ShowType = TBType.Num;
            tb.Text = m2m.W.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("框架高度");
            tb = new TB();
            tb.ID = "TB_" + MapM2MAttr.H;
            tb.ShowType = TBType.Num;
            tb.Text = m2m.H.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("记录呈现列数");
            tb = new TB();
            tb.ID = "TB_" + MapM2MAttr.Cols;
            tb.ShowType = TBType.Num;
            tb.Text = m2m.Cols.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("显示方式");
            DDL ddl = new DDL();
            ddl.ID = "DDL_" + MapM2MAttr.ShowWay;
            ddl.BindSysEnum("FrmUrlShowWay", (int)m2m.ShowWay);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("请参考手册");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD( "权限" );
            this.Pub1.AddTDBegin("colspan=2");

            CheckBox cb = new CheckBox();
            cb.Checked = m2m.IsDelete;
            cb.Text = "是否可以删除?";
            cb.ID = "CB_IsDelete";
            this.Pub1.Add(cb);

            cb = new CheckBox();
            cb.Checked = m2m.IsDelete;
            cb.Text = "是否可以增加?";
            cb.ID = "CB_IsInsert";
            this.Pub1.Add(cb);


            cb = new CheckBox();
            cb.Checked = m2m.IsCheckAll;
            cb.Text = "是否显示选择全部？";
            cb.ID = "CB_IsCheckAll";
            this.Pub1.Add(cb);

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            GroupFields gfs = new GroupFields(md.No);
            this.Pub1.AddTR1();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("显示在分组");
            ddl = new DDL();
            ddl.ID = "DDL_GroupField";
            ddl.BindEntities(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab, false, AddAllLocation.None);
            ddl.SetSelectItem(m2m.GroupID);
            this.Pub1.AddTD("colspan=2", ddl);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            this.Pub1.AddTDBegin("colspan=4 align=center");

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_SaveAndClose";
            btn.Text = " 保存并关闭 ";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            if (m2m.IsExits)
            {
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_Del";
                btn.Text = "删除";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Del_Click);
                this.Pub1.Add(btn);
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            this.Pub1.AddFieldSet("SQL事例");
            this.Pub1.Add("主体数据源:");
            this.Pub1.AddBR("SELECT No,Name,FK_Dept FROM Port_Emp");
            this.Pub1.AddBR();
            this.Pub1.Add("分组数据源:");
            this.Pub1.AddBR("SELECT No,Name FROM Port_Dept ");
            this.Pub1.AddFieldSetEnd();
        }
        void btn_Save_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            try
            {
                MapM2M m2m = new MapM2M(this.FK_MapData, this.NoOfObj);
                m2m = (MapM2M)this.Pub1.Copy(m2m);
                m2m.FK_MapData = this.FK_MapData;
                m2m.ShowWay = (FrmShowWay)this.Pub1.GetDDLByID("DDL_ShowWay").SelectedItemIntVal;
                m2m.FK_MapData = this.FK_MapData;
                if (string.IsNullOrEmpty(m2m.NoOfObj))
                    m2m.NoOfObj = this.NoOfObj;

                if (string.IsNullOrEmpty(m2m.NoOfObj))
                    m2m.NoOfObj = DataType.ParseStringToPinyin(m2m.Name);

                GroupFields gfs = new GroupFields(m2m.FK_MapData);
                m2m.GroupID = this.Pub1.GetDDLByID("DDL_GroupField").SelectedItemIntVal;
                m2m.HisM2MType = M2MType.M2M;
                m2m.Save();

                if (btn.ID.Contains("AndC"))
                {
                    this.WinClose();
                    return;
                }
                this.Response.Redirect("MapM2M.aspx?DoType=Edit&NoOfObj=" + m2m.NoOfObj + "&FK_MapData=" + this.FK_MapData, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                MapM2M dtl = new MapM2M();
                dtl.MyPK = this.FK_MapData + "_" + this.NoOfObj;
                dtl.Delete();
                this.WinClose();
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }

    }
}