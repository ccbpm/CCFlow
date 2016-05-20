using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP;
using BP.Sys;
using BP.Web.Controls;

namespace CCFlow.WF.MapDef.MapExtUI
{
    public partial class ActiveDDLUI : BP.Web.WebPage
    {
        #region 属性。
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 操作的Key
        /// </summary>
        public string OperAttrKey
        {
            get
            {
                return this.Request.QueryString["OperAttrKey"];
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string ExtType
        {
            get
            {
                return MapExtXmlList.ActiveDDL;
            }
        }
        public string Lab = null;
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            MapExt me = new MapExt();
           // me.MyPK = this.FK_MapData + "_" + this.ExtType + "_" + me.AttrOfOper + "_" + me.AttrsOfActive;
             
          int i=  me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                        MapExtAttr.AttrOfOper, this.RefNo);

            this.Pub1.AddEasyUiPanelInfoBegin("为下拉框[" + this.RefNo + "]设置联动.", "icon-edit");
            me.FK_MapData = this.FK_MapData;

            this.Pub1.AddTableNormal();

            MapAttrs attrs = new MapAttrs(this.FK_MapData);

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("联动下拉框：");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_Attr";
            foreach (MapAttr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.UIIsEnable == false)
                    continue;

                if (attr.UIContralType != UIContralType.DDL)
                    continue;

                if (attr.KeyOfEn == this.RefNo)
                    continue;

                ddl.Items.Add(new ListItem(attr.KeyOfEn + " - " + attr.Name, attr.KeyOfEn));
            }
            ddl.SetSelectItem(me.AttrsOfActive);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("要实现联动的菜单");
            this.Pub1.AddTREnd();

            // 数据源列表.
            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("colspan=1", "数据源");

            ddl = new DDL();
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            ddl.BindEntitiesNoName(srcs);

            this.Pub1.AddTD(ddl);
           // this.Pub1.AddTD("ccform允许从其他数据源中取数据,<a href=\"javascript:WinOpen('/WF/Comm/Search.aspx?EnsName=BP.Sys.SFDBSrcs')\" ><img src='/WF/Img/Setting.png' border=0/>设置/新建数据源</a>, <a href=\"javascript:window.localhost.href=window.localhost.href;\" >刷新数据源</a>");
            this.Pub1.AddTD("ccform允许从其他数据源中取数据,<a href=\"javascript:WinOpen('/WF/Comm/Search.aspx?EnsName=BP.Sys.SFDBSrcs','d2')\" ><img src='/WF/Img/Setting.png' border=0/>设置/新建数据源</a>, <a href=\"javascript:window.localhost.href=window.localhost.href;\" >刷新数据源</a>");

            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin("colspan='3'");
            RadioButton rb = new RadioButton();
            rb.Text = "通过sql获取联动";
            rb.GroupName = "sdr";
            rb.ID = "RB_0";
            if (me.DoWay == 0)
                rb.Checked = true;

            this.Pub1.Add("在下面文本框中输入一个SQL,具有编号，标签列，用来绑定下从动下拉框。<br />");
            this.Pub1.Add("比如:SELECT No, Name FROM CN_City WHERE FK_SF = '@Key' ");
            this.Pub1.AddBR();
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Text = me.Doc;
            tb.Columns = 80;
            tb.CssClass = "TH";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 7;
            tb.Style.Add("width", "99%");
            this.Pub1.Add(tb);
            this.Pub1.AddBR();
            this.Pub1.Add("说明:@Key是ccflow约定的关键字，是主下拉框传递过来的值");
            //this.Pub1.AddFieldSetEnd();

            rb = new RadioButton();
            rb.Text = "通过编码标识获取";
            rb.GroupName = "sdr";
            rb.Enabled = false;
            rb.ID = "RB_1";
            if (me.DoWay == 1)
                rb.Checked = true;

            //this.Pub1.AddFieldSet(rb);
            this.Pub1.Add("主菜单是编号的是从动菜单编号的前几位，不必联动内容。<br />");
            this.Pub1.Add("比如: 主下拉框是省份，联动菜单是城市。");
            //this.Pub1.AddFieldSetEnd();

            //this.Pub1.Add("</TD>");
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveJiLian_Click);
            this.Pub1.AddTD("colspan=2", btn);

            if (i != 0)
            {
                btn = new LinkBtn(false, NamesOfBtn.Delete, "删除");
                btn.Click += new EventHandler(btn_Del_Click);
                this.Pub1.AddTD("colspan=1", btn);
            }

            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            this.Pub1.AddEasyUiPanelInfoEnd();
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            MapExt me = new MapExt();
            // me.MyPK = this.FK_MapData + "_" + this.ExtType + "_" + me.AttrOfOper + "_" + me.AttrsOfActive;
             me.Delete(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, MapExtXmlList.ActiveDDL,
                        MapExtAttr.AttrOfOper, this.RefNo);

            //关闭.
            BP.Sys.PubClass.Alert("删除成功.");
            BP.Sys.PubClass.WinClose();

        }
        void btn_SaveJiLian_Click(object sender, EventArgs e)
        {
            MapExt me = new MapExt();
            me.MyPK = this.MyPK;
            if (me.MyPK.Length > 2)
                me.RetrieveFromDBSources();
            me = (MapExt)this.Pub1.Copy(me);
            me.ExtType = this.ExtType;
            me.Doc = this.Pub1.GetTextBoxByID("TB_Doc").Text;
            me.AttrOfOper = this.RefNo;
            me.AttrsOfActive = this.Pub1.GetDDLByID("DDL_Attr").SelectedItemStringVal;
            if (me.AttrsOfActive == me.AttrOfOper)
            {
                this.Alert("两个项目不能相同.");
                return;
            }
            try
            {
                if (this.Pub1.GetRadioButtonByID("RB_1").Checked)
                    me.DoWay = 1;
                else
                    me.DoWay = 0;
            }
            catch
            {
                me.DoWay = 0;
            }

            me.FK_MapData = this.FK_MapData;
            try
            {
                me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper + "_" + me.AttrsOfActive;
                if (me.Doc.Contains("No") == false || me.Doc.Contains("Name") == false)
                    throw new Exception("在您的SQL表达式里，必须有No,Name 还两个列，分别标识编码与名称。");
                me.Save();
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
                return;
            }
            this.Response.Redirect("ActiveDDL.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&MyPK=" + me.MyPK, true);
        }
    }
}