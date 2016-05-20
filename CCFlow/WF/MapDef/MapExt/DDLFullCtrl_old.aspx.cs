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
    public partial class DDLFullCtrlUI :  BP.Web.WebPage
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
                return MapExtXmlList.DDLFullCtrl;
            }
        }
        public string Lab = null;
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            MapExts mes = new MapExts();

            if (this.DoType == "EditAutoFullDtl")
            {
                this.EditAutoFullDtl_DDL();
                return;
            }
            if (this.DoType == "EditAutoJL")
            {
                this.EditAutoJL();
                return;
            }

            
            MapExt me = new MapExt();
            me.MyPK = this.FK_MapData + "_" + this.ExtType + "_" + me.AttrOfOper + "_" + me.AttrsOfActive;
            if (me.RetrieveFromDBSources() == 0)
            {
                me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, MapExtXmlList.DDLFullCtrl,
                        MapExtAttr.AttrOfOper, this.RefNo);
            }

            this.Pub1.AddTableNormal();
            this.Pub1.AddCaption("为下拉框["+this.RefNo+"]设置自动填充.");

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("colspan=3", "自动填充SQL：");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Text = me.Doc;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Columns = 80;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD("colspan=3", tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveAutoFull_Click);
            this.Pub1.AddTDBegin("colspan=3");
            this.Pub1.Add(btn);

            if (me.IsExits == true)
            {
                //this.Pub1.AddTD("<a href=\"DDLFullCtrl.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "&ExtType=" + this.ExtType + "&DoType=EditAutoJL\" >级连下拉框</a>-<a href=\"DDLFullCtrl.aspx?MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullDtl\" >填充从表</a>");
                this.Pub1.AddSpace(1);
                this.Pub1.AddEasyUiLinkButton("级连下拉框",
                                         "DDLFullCtrl.aspx?MyPK=" + me.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" +
                                         this.RefNo + "&ExtType=" + this.ExtType + "&DoType=EditAutoJL");
                this.Pub1.AddSpace(1);
                this.Pub1.AddEasyUiLinkButton("填充从表",
                                         "DDLFullCtrl.aspx?MyPK=" + me.MyPK + "&FK_MapData=" + this.FK_MapData + "&ExtType=" +
                                         this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullDtl");
            }
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();


            #region 输出事例
            //his.Pub1.AddFieldSet("填充事例:");
            this.Pub1.AddEasyUiPanelInfoBegin("填充事例：");
            string sql = "SELECT dizhi as Addr, fuzeren as Manager FROM Prj_Main WHERE No = '@Key'";
            this.Pub1.Add(sql.Replace("~", "\""));
            this.Pub1.AddBR("<hr><b>说明：</b>根据用户当前选择下拉框的实例（比如:选择一个工程）把相关此实例的其它属性放在控件中");
            this.Pub1.Add("（比如：工程的地址，负责人。）");
            this.Pub1.AddBR("<b>备注：</b><br />1.只有列名与本表单中字段名称匹配才能自动填充上去。<br>2.sql查询出来的是一行数据，@Key 是当前选择的值。");
            //this.Pub1.AddFieldSetEnd();
            this.Pub1.AddEasyUiPanelInfoEnd();
            #endregion 输出事例
        }
        void btn_SaveAutoFull_Click(object sender, EventArgs e)
        {
            MapExt me = new MapExt();

            //删除指定的数据,避免插入重复.
            me.Delete(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, this.ExtType, MapExtAttr.AttrOfOper, this.RefNo);

            me.MyPK = this.MyPK;
            if (me.MyPK.Length > 2)
                me.RetrieveFromDBSources();

            me = (MapExt)this.Pub1.Copy(me);
            me.ExtType = this.ExtType;
            me.Doc = this.Pub1.GetTextBoxByID("TB_Doc").Text;
            me.AttrOfOper = this.RefNo; // this.Pub1.GetDDLByID("DDL_Oper").SelectedItemStringVal;
            me.FK_MapData = this.FK_MapData;
            me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper;

            try
            {
                me.Save();
            }
            catch (Exception ex)
            {
                //this.Alert(ex.Message);
                this.Pub1.AlertMsg_Warning("SQL错误", ex.Message);
                return;
            }
            this.Response.Redirect("DDLFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo+"&MyPK="+me.MyPK, true);
        }
    /// <summary>
    /// 新建文本框自动完成
    /// </summary>
    public void EditAutoFullDtl_DDL()
    {
        //this.Pub1.AddFieldSet("<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a> -设置自动填充从表");
        MapExt myme = new MapExt(this.MyPK);
        MapDtls dtls = new MapDtls(myme.FK_MapData);
        string[] strs = myme.Tag1.Split('$');

        if (dtls.Count == 0)
        {
            this.Pub1.Clear();
            Pub1.AddEasyUiPanelInfo("设置自动填充从表", "<p>该表单下没有从表，所以您不能为从表设置自动填充。<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a></p>");
            return;
        }

        //this.Pub1.AddTable("border=0  align=left ");
        Pub1.AddTableNormal();
        Pub1.AddTRGroupTitle("<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a> - 设置自动填充从表");

        bool is1 = false;
        foreach (MapDtl dtl in dtls)
        {
            is1 = this.Pub1.AddTR(is1);
            TextBox tb = new TextBox();
            tb.ID = "TB_" + dtl.No;
            tb.Style.Add("width", "100%");
            tb.Columns = 80;
            tb.Rows = 3;
            tb.TextMode = TextBoxMode.MultiLine;
            foreach (string s in strs)
            {
                if (s == null)
                    continue;

                if (s.Contains(dtl.No + ":") == false)
                    continue;
                string[] ss = s.Split(':');
                tb.Text = ss[1];
            }

            this.Pub1.AddTDBegin();
            this.Pub1.AddB("&nbsp;&nbsp;" + dtl.Name + " - 从表");
            this.Pub1.AddBR();
            this.Pub1.Add(tb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
        }

        //this.Pub1.AddTableEndWithHR();
        Pub1.AddTableEnd();
        Pub1.AddBR();

        //Button mybtn = new Button();
        var mybtn = new LinkBtn(false, NamesOfBtn.Save, "保存");
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullDtl_Click);
        this.Pub1.Add(mybtn);
        Pub1.AddSpace(1);

        mybtn = new LinkBtn(false, NamesOfBtn.Cancel, "取消");
        mybtn.Click += new EventHandler(mybtn_SaveAutoFullDtl_Click);
        this.Pub1.Add(mybtn);
        
        //this.Pub1.AddFieldSetEnd();
    }

       
        void mybtn_SaveAutoFullDtl_Click(object sender, EventArgs e)
        {
            var btn = sender as LinkBtn;
            if (btn.ID == NamesOfBtn.Cancel)
            {
                this.Response.Redirect("DDLFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
                return;
            }

            MapExt myme = new MapExt(this.MyPK);
            MapDtls dtls = new MapDtls(myme.FK_MapData);
            string info = "";
            string error = "";
            foreach (MapDtl dtl in dtls)
            {
                TextBox tb = this.Pub1.GetTextBoxByID("TB_" + dtl.No);
                if (tb.Text.Trim() == "")
                    continue;
                try
                {
                    //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(tb.Text);
                    //MapAttrs attrs = new MapAttrs(dtl.No);
                    //string err = "";
                    //foreach (DataColumn dc in dt.Columns)
                    //{
                    //    if (attrs.IsExits(MapAttrAttr.KeyOfEn, dc.ColumnName) == false)
                    //    {
                    //        err += "<br>列" + dc.ColumnName + "不能与从表 属性匹配.";
                    //    }
                    //}
                    //if (err != "")
                    //{
                    //    error += "在为("+dtl.Name+")检查sql设置时出现错误:"+err;
                    //}
                }
                catch (Exception ex)
                {
                    this.Alert("SQL ERROR: " + ex.Message);
                    return;
                }
                info += "$" + dtl.No + ":" + tb.Text;
            }

            if (error != "")
            {
                this.Pub1.AddEasyUiPanelInfo("错误", "设置错误,请更正:<br />" + error, "icon-no");
                return;
            }
            myme.Tag1 = info;
            myme.Update();
            this.Response.Redirect("DDLFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
        }

        public void EditAutoJL()
        {
            MapExt myme = new MapExt(this.MyPK);
            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            string[] strs = myme.Tag.Split('$');

            this.Pub1.AddTableNormal();
            this.Pub1.AddTRGroupTitle("<a href='DDLFullCtrl.aspx?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a> - 设置级连菜单");

            foreach (MapAttr attr in attrs)
            {
                if (attr.LGType == FieldTypeS.Normal)
                    continue;
                if (attr.UIIsEnable == false)
                    continue;

                TextBox tb = new TextBox();
                tb.ID = "TB_" + attr.KeyOfEn;
                tb.Style.Add("width", "100%");
                tb.Columns = 90;
                tb.Rows = 4;
                tb.TextMode = TextBoxMode.MultiLine;

                foreach (string s in strs)
                {
                    if (s == null)
                        continue;

                    if (s.Contains(attr.KeyOfEn + ":") == false)
                        continue;

                    string[] ss = s.Split(':');
                    tb.Text = ss[1];
                }

                this.Pub1.AddTR();
                this.Pub1.AddTD(attr.Name + " " + attr.KeyOfEn + " 字段");
                this.Pub1.AddTREnd();

                this.Pub1.AddTR();
                this.Pub1.AddTD(tb);
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTableEnd();
            Pub1.AddBR();

            var mybtn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            mybtn.Click += new EventHandler(mybtn_SaveAutoFullJilian_Click);
            this.Pub1.Add(mybtn);
            Pub1.AddSpace(1);

            mybtn = new LinkBtn(false, NamesOfBtn.Cancel, "取消");
            mybtn.Click += new EventHandler(mybtn_SaveAutoFullJilian_Click);
            this.Pub1.Add(mybtn);
        }

        void mybtn_SaveAutoFullJilian_Click(object sender, EventArgs e)
        {
            var btn = sender as LinkBtn;

            if (btn != null && btn.ID == NamesOfBtn.Cancel)
            {
                this.Response.Redirect("DDLFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
                return;
            }

            MapExt myme = new MapExt(this.MyPK);
            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            string info = "";
            foreach (MapAttr attr in attrs)
            {
                if (attr.LGType == FieldTypeS.Normal)
                    continue;

                if (attr.UIIsEnable == false)
                    continue;

                TextBox tb = this.Pub1.GetTextBoxByID("TB_" + attr.KeyOfEn);
                if (tb.Text.Trim() == "")
                    continue;

                try
                {
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(tb.Text);
                    if (tb.Text.Contains("@Key") == false)
                        throw new Exception("缺少@Key参数。");

                    if (dt.Columns.Contains("No") == false || dt.Columns.Contains("Name") == false)
                        throw new Exception("在您的sql表单公式中必须有No,Name两个列，来绑定下拉框。");
                }
                catch (Exception ex)
                {
                    this.Alert("SQL ERROR: " + ex.Message);
                    return;
                }
                info += "$" + attr.KeyOfEn + ":" + tb.Text;
            }
            myme.Tag = info;
            myme.Update();
            this.Alert("保存成功.");
        }
    }
}