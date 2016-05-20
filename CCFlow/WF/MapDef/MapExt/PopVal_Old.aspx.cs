using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
using BP.Tools;

namespace CCFlow.WF.MapDef
{
    public partial class PopVal_old : BP.Web.WebPage
    {
        #region 属性.
        public string FK_MapData
        {
            get
            {
                return this.Request["FK_MapData"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "为字段[" + this.RefNo + "]设置开窗返回值";

            int idx = 1;
            bool isItem = false;

            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
            this.Pub1.AddCaptionLeft("为字段[" + this.RefNo + "]设置开窗返回值" + BP.WF.Glo.GenerHelpCCForm("帮助", "http://ccform.mydoc.io/?v=5769&t=20182", ""));
            
            MapExt me = new MapExt();
            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.RefNo;
            me.ExtType = MapExtXmlList.PopVal;
            if (this.MyPK == null)
            {
                me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, MapExtXmlList.PopVal, MapExtAttr.AttrOfOper, this.RefNo);
            }
            else
            {
                me.MyPK = this.MyPK;
                me.RetrieveFromDBSources();
            }

            me.FK_MapData = this.FK_MapData;
            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("序");
            this.Pub1.AddTDGroupTitle("项目");
            this.Pub1.AddTDGroupTitle("采集");
            this.Pub1.AddTDGroupTitle("说明");
            this.Pub1.AddTREnd();

            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("设置类型：");
            this.Pub1.AddTDBegin();

            RadioButton rb = new RadioButton();
            rb.Text = "ccform内置";
            rb.ID = "RB_PopValWorkModel_1";
            rb.GroupName = "sd";
            if (me.PopValWorkModel == 1)
                rb.Checked = true;
            this.Pub1.Add(rb);

            rb = new RadioButton();
            rb.ID = "RB_PopValWorkModel_0";
            rb.Text = "自定义URL";
            rb.GroupName = "sd";
            if (me.PopValWorkModel == 0)
                rb.Checked = true;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("如果是自定义URL,仅填写URL字段.");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("URL：");
            TextBox tb = new TextBox();
            tb.ID = "TB_" + MapExtAttr.Doc;
            tb.Text = me.Doc;
            tb.Columns = 50;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("colspan=3", "URL填写说明:请输入一个弹出窗口的url,当操作员关闭后返回值就会被设置在当前控件中<br />测试URL:http://localhost/WF/SDKFlowDemo/PopSelectVal.aspx.");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("数据分组SQL：");
            tb = new TextBox();
            tb.ID = "TB_" + MapExtAttr.Tag1;
            tb.Text = me.Tag1;
            tb.Columns = 50;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("数据源SQL：");
            tb = new TextBox();
            tb.ID = "TB_" + MapExtAttr.Tag2;
            tb.Text = me.Tag2;
            tb.Columns = 50;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD("colspan=2", tb);
            this.Pub1.AddTREnd();

            #region 选择方式
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("选择方式：");
            this.Pub1.AddTDBegin();

            rb = new RadioButton();
            rb.Text = "多项选择";
            rb.ID = "RB_Tag3_0";
            rb.GroupName = "dd";
            if (me.PopValSelectModel == 0)
                rb.Checked = true;
            else
                rb.Checked = false;
            this.Pub1.Add(rb);

            rb = new RadioButton();
            rb.ID = "RB_Tag3_1";
            rb.Text = "单项选择";
            rb.GroupName = "dd";
            if (me.PopValSelectModel == 1)
                rb.Checked = true;
            else
                rb.Checked = false;
            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            #endregion 选择方式

            #region 呈现方式
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("数据源呈现方式：");
            this.Pub1.AddTDBegin();
            rb = new RadioButton();
            rb.Text = "表格方式";
            rb.ID = "RB_Tag4_0";
            rb.GroupName = "dsd";
            if (me.PopValShowModel == 0)
                rb.Checked = true;
            else
                rb.Checked = false;
            this.Pub1.Add(rb);

            rb = new RadioButton();
            rb.ID = "RB_Tag4_1";
            rb.Text = "目录方式";
            rb.GroupName = "dsd";
            if (me.PopValShowModel == 1)
                rb.Checked = true;
            else
                rb.Checked = false;
            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            #endregion 呈现方式

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("返回值格式：");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_PopValFormat";
            ddl.BindSysEnum("PopValFormat");
            ddl.SetSelectItem(me.PopValFormat);
            this.Pub1.AddTD("colspan=2", ddl);
            this.Pub1.AddTREnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SavePopVal_Click);
            this.Pub1.AddTD("colspan=3", btn);
            this.Pub1.AddTREnd();
        }

        void btn_SavePopVal_Click(object sender, EventArgs e)
        {
            //删除.
            MapExt me = new MapExt();
            me.Delete(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, MapExtXmlList.PopVal, MapExtAttr.AttrOfOper, this.RefNo);
            
            me.MyPK = this.MyPK;
            if (me.MyPK.Length > 2)
                me.RetrieveFromDBSources();

            me = (MapExt)this.Pub1.Copy(me);
            me.ExtType = MapExtXmlList.PopVal; 
            me.Doc = this.Pub1.GetTextBoxByID("TB_Doc").Text;
            me.AttrOfOper = this.RefNo; 
            me.SetPara("PopValFormat", this.Pub1.GetDDLByID("DDL_PopValFormat").SelectedItemStringVal);

            RadioButton rb = this.Pub1.GetRadioButtonByID("RB_PopValWorkModel_1");
            if (rb.Checked)
                me.PopValWorkModel = 1;
            else
                me.PopValWorkModel = 0;

            rb = this.Pub1.GetRadioButtonByID("RB_Tag3_0");
            if (rb.Checked)
                me.PopValSelectModel = 0;
            else
                me.PopValSelectModel = 1;

            rb = this.Pub1.GetRadioButtonByID("RB_Tag4_0");
            if (rb.Checked)
                me.PopValShowModel = 0;
            else
                me.PopValShowModel = 1;

            me.FK_MapData = this.FK_MapData;
            me.MyPK = me.ExtType + "_" + this.FK_MapData + "_" + me.AttrOfOper;
            me.Save();
            this.Response.Redirect("PopVal.aspx?FK_MapData=" + this.FK_MapData + "&MyPK="+ this.MyPK + "&RefNo=" + this.RefNo, true);
        }
         
    }
}