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
    public partial class TBFullCtrlUI :  BP.Web.WebPage
    {
        #region 属性。
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string OperAttrKey
        {
            get
            {
                return this.Request.QueryString["OperAttrKey"];
            }
        }
        public string ExtType
        {
            get
            {
                return MapExtXmlList.TBFullCtrl;
            }
        }

        public string Lab = null;
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType == "EditAutoFullDtl")
            {
                this.EditAutoFullDtl_TB();
                return;
            }
            if (this.DoType == "EditAutoFullM2M")
            {
                this.EditAutoFullM2M_TB();
                return;
            }
            if (this.DoType == "EditAutoJL")
            {
                this.EditAutoJL();
                return;
            }


            this.EditAutoFull_TB();
        }
        /// <summary>
        /// 新建文本框自动完成
        /// </summary>
        public void EditAutoFullM2M_TB()
        {
            MapExt myme = new MapExt(this.MyPK);
            MapM2Ms m2ms = new MapM2Ms(myme.FK_MapData);

            if (m2ms.Count == 0)
            {
                this.Pub1.Clear();
                //this.Pub1.AddFieldSet("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");
                //this.Pub1.Add("该表单下没有从表，所以您不能为从表设置自动填充。");
                //this.Pub1.AddFieldSetEnd();
                Pub1.AddEasyUiPanelInfo("设置自动填充从表", "<p>该表单下没有从表，所以您不能为从表设置自动填充。<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a></p>");
                return;
            }

            Pub1.AddTableNormal();
            Pub1.AddTRGroupTitle("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");

            string[] strs = myme.Tag2.Split('$');
            bool isHaveM2M = false;
            bool isHaveM2MM = false;
            foreach (MapM2M m2m in m2ms)
            {
                if (m2m.HisM2MType == M2MType.M2M)
                    isHaveM2M = true;
                if (m2m.HisM2MType == M2MType.M2MM)
                    isHaveM2MM = true;

                TextBox tb = new TextBox();
                tb.ID = "TB_" + m2m.NoOfObj;
                tb.Columns = 70;
                tb.Style.Add("width", "100%");
                tb.Rows = 5;
                tb.TextMode = TextBoxMode.MultiLine;
                foreach (string s in strs)
                {
                    if (s == null)
                        continue;

                    if (s.Contains(m2m.NoOfObj + ":") == false)
                        continue;

                    string[] ss = s.Split(':');
                    tb.Text = ss[1];
                }

                //this.Pub1.AddFieldSet("编号:" + m2m.NoOfObj + ",名称:" + m2m.Name);
                Pub1.AddTR();
                Pub1.AddTDBegin();
                Pub1.Add("编号:" + m2m.NoOfObj + ",名称:" + m2m.Name);
                Pub1.Add(tb);
                Pub1.AddTDEnd();
                Pub1.AddTREnd();
                //this.Pub1.AddFieldSetEnd();
            }

            //this.Pub1.AddHR();
            Pub1.AddTableEnd();
            Pub1.AddBR();

            //Button mybtn = new Button();
            var mybtn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            mybtn.Click += new EventHandler(mybtn_SaveAutoFullM2M_Click);
            this.Pub1.Add(mybtn);
            Pub1.AddSpace(1);

            mybtn = new LinkBtn(false, NamesOfBtn.Cancel, "取消");
            mybtn.Click += new EventHandler(mybtn_SaveAutoFullM2M_Click);
            this.Pub1.Add(mybtn);
            Pub1.AddBR();
            //this.Pub1.AddFieldSetEnd();

            if (isHaveM2M)
            {
                //this.Pub1.AddFieldSet("帮助:一对多");
                Pub1.AddEasyUiPanelInfoBegin("帮助:一对多", "icon-help");
                this.Pub1.Add("在主表相关数据发生变化后，一对多数据要发生变化，变化的格式为：");
                this.Pub1.AddBR("实例：SELECT No,Name FROM WF_Emp WHERE FK_Dept='@Key' ");
                this.Pub1.AddBR("相关内容的值发生改变时而自动填充checkbox。");
                this.Pub1.AddBR("注意:");
                this.Pub1.AddBR("1，@Key 是主表字段传递过来的变量。");
                this.Pub1.AddBR("2，必须并且仅有No,Name两个列，顺序不要颠倒。");
                //this.Pub1.AddFieldSetEnd();
                Pub1.AddEasyUiPanelInfoEnd();
            }

            if (isHaveM2MM)
            {
                //this.Pub1.AddFieldSet("帮助:一对多多");
                Pub1.AddEasyUiPanelInfoBegin("帮助:一对多多", "icon-help");
                this.Pub1.Add("在主表相关数据发生变化后，一对多多数据要发生变化，变化的格式为：");
                this.Pub1.AddBR("实例：SELECT a.FK_Emp M1ID, a.FK_Station as M2ID, b.Name as M2Name FROM " + BP.WF.Glo.EmpStation + " a, Port_Station b WHERE  A.FK_Station=B.No and a.FK_Emp='@Key'");
                this.Pub1.AddBR("相关内容的值发生改变时而自动填充checkbox。");
                this.Pub1.AddBR("注意:");
                this.Pub1.AddBR("1，@Key 是主表字段传递过来的变量。");
                this.Pub1.AddBR("2，必须并且仅有3个列 M1ID,M2ID,M2Name，顺序不要颠倒。第1列的ID对应列表的ID，第2，3列对应的是列表数据源的ID与名称。");
                //this.Pub1.AddFieldSetEnd();
                Pub1.AddEasyUiPanelInfoEnd();
            }
        }
        void mybtn_SaveAutoFullM2M_Click(object sender, EventArgs e)
        {
            //Button btn = sender as Button;
            var btn = sender as LinkBtn;
            if (btn.ID == NamesOfBtn.Cancel)
            {
                this.Response.Redirect("TBFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
                return;
            }

            MapExt myme = new MapExt(this.MyPK);
            MapM2Ms m2ms = new MapM2Ms(myme.FK_MapData);
            string info = "";
            string error = "";
            foreach (MapM2M m2m in m2ms)
            {
                TextBox tb = this.Pub1.GetTextBoxByID("TB_" + m2m.NoOfObj);
                if (tb.Text.Trim() == "")
                    continue;
                try
                {
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(tb.Text);
                    string err = "";
                    if (dt.Columns[0].ColumnName != "No")
                        err += "第1列不是No.";
                    if (dt.Columns[1].ColumnName != "Name")
                        err += "第2列不是Name.";

                    if (err != "")
                    {
                        error += "在为(" + m2m.Name + ")检查sql设置时出现错误：请确认列的顺序是否正确为大小写是否匹配。" + err;
                    }
                }
                catch (Exception ex)
                {
                    this.Alert("SQL ERROR: " + ex.Message);
                    return;
                }
                info += "$" + m2m.NoOfObj + ":" + tb.Text;
            }

            if (error != "")
            {
                this.Pub1.AddEasyUiPanelInfo("错误", "设置错误,请更正:<br />" + error, "icon-no");
                //this.Pub1.AddMsgOfWarning("设置错误,请更正:", error);
                return;
            }
            myme.Tag2 = info;
            myme.Update();
            this.Response.Redirect("TBFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
        }
        /// <summary>
        /// 新建文本框自动完成
        /// </summary>
        public void EditAutoFullDtl_TB()
        {
            MapExt myme = new MapExt(this.MyPK);
            MapDtls dtls = new MapDtls(myme.FK_MapData);

            if (dtls.Count == 0)
            {
                this.Pub1.Clear();
                Pub1.AddEasyUiPanelInfo("设置自动填充从表", "<p>该表单下没有从表，所以您不能为从表设置自动填充。<a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a></p>");
                return;
            }

            Pub1.AddTableNormal();
            Pub1.AddTRGroupTitle("设置自动填充从表. <a href='?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a>");

            string[] strs = myme.Tag1.Split('$');
            foreach (MapDtl dtl in dtls)
            {
                TextBox tb = new TextBox();
                tb.ID = "TB_" + dtl.No;
                tb.Columns = 70;
                tb.Style.Add("width", "100%");
                tb.Rows = 5;
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

                //this.Pub1.AddFieldSet("编号:" + dtl.No + ",名称:" + dtl.Name);
                Pub1.AddTR();
                Pub1.AddTDBegin();
                Pub1.Add("编号:" + dtl.No + ",名称:" + dtl.Name);
                Pub1.AddBR();
                Pub1.Add(tb);
                Pub1.AddBR();

                string fs = "可填充的字段:";
                MapAttrs attrs = new MapAttrs(dtl.No);
                foreach (MapAttr item in attrs)
                {
                    if (item.KeyOfEn == "OID" || item.KeyOfEn == "RefPKVal")
                        continue;
                    fs += item.KeyOfEn + ",";
                }

                this.Pub1.Add(fs.Substring(0, fs.Length - 1));
                //this.Pub1.AddFieldSetEnd();
                Pub1.AddTDEnd();
                Pub1.AddTREnd();
            }

            //this.Pub1.AddHR();
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
            Pub1.AddBR();
            //this.Pub1.AddFieldSetEnd();

            //this.Pub1.AddFieldSet("帮助:");
            Pub1.AddEasyUiPanelInfoBegin("帮助", "icon-help");
            this.Pub1.Add("在这里您需要设置一个查询语句");
            this.Pub1.AddBR("例如：SELECT XLMC AS suozaixianlu, bustype as V_BusType FROM [V_XLVsBusType] WHERE jbxx_htid='@Key'");
            this.Pub1.AddBR("这个查询语句要与从表的列对应上就可以在文本框的值发生改变时而自动填充。");
            this.Pub1.AddBR("注意:");
            this.Pub1.AddBR("1，@Key 是主表字段传递过来的变量。");
            this.Pub1.AddBR("2，从表列字段字名，与填充sql列字段大小写匹配。");
            //this.Pub1.AddFieldSetEnd();
            Pub1.AddEasyUiPanelInfoEnd();
        }
        void mybtn_SaveAutoFullDtl_Click(object sender, EventArgs e)
        {
            var btn = sender as LinkBtn;
            if (btn.ID == NamesOfBtn.Cancel)
            {
                this.Response.Redirect("TBFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
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
                //this.Pub1.AddMsgOfWarning("设置错误,请更正:", error);
                return;
            }
            myme.Tag1 = info;
            myme.Update();
            this.Response.Redirect("TBFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
        }
        public void EditAutoFull_TB()
        {
            MapExt me = new MapExt();
            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.RefNo;
            me.ExtType = this.ExtType;
            
                me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.ExtType, this.ExtType, MapExtAttr.AttrOfOper, this.RefNo);

            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = this.RefNo;
            me.ExtType = this.ExtType;

            //this.Pub1.AddTable("border=0");
            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
            this.Pub1.AddCaptionLeft("为字段[" + this.RefNo + "]设置自动填充." + BP.WF.Glo.GenerHelpCCForm("主表中的自动完成", "http://ccform.mydoc.io/?v=5769&t=20175", null));

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("自动填充SQL:");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Text = me.Doc;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Columns = 80;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD( tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle( "关键字查询的SQL:");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            tb = new TextBox();
            tb.ID = "TB_Tag";
            tb.Text = me.Tag;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Columns = 80;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD( tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            this.Pub1.AddTDBegin("colspan=1");

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveAutoFull_Click);
            this.Pub1.Add(btn);

            if (me.IsExits == true)
            {
                string mypk = this.ExtType + "_" + this.FK_MapData + "_" + this.RefNo;
                this.Pub1.AddSpace(1);
                this.Pub1.Add("<a class='easyui-linkbutton' href=\"TBFullCtrl.aspx?MyPK=" + mypk + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "&ExtType=" + this.ExtType + "&DoType=EditAutoJL\" >级联下拉框</a>");
                this.Pub1.AddSpace(1);
                this.Pub1.Add("<a class='easyui-linkbutton' href=\"TBFullCtrl.aspx?MyPK=" + mypk + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullDtl\" >填充从表</a>");
                this.Pub1.AddSpace(1);
                this.Pub1.Add("<a class='easyui-linkbutton' href=\"TBFullCtrl.aspx?MyPK=" + mypk + "&FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo + "&DoType=EditAutoFullM2M\" >填充一对多</a>");
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            #region 输出事例
            ////this.Pub1.AddFieldSet("帮助");
            //this.Pub1.AddEasyUiPanelInfoBegin("帮助", "icon-help");
            //this.Pub1.AddB("For oracle:");
            //string sql = "自动填充SQL:<br />SELECT No as ~No~ , Name as ~Name~, Name as ~mingcheng~ FROM WF_Emp WHERE No LIKE '@Key%' AND ROWNUM<=15";
            //sql += "<br />关键字查询SQL:<br>SELECT No as ~No~ , Name as ~Name~, Name as ~mingcheng~ FROM WF_Emp WHERE No LIKE '@Key%'  ";
            //this.Pub1.AddBR(sql.Replace("~", "\""));

            //this.Pub1.AddB("<br />For sqlserver:");
            //sql = "自动填充SQL:<br />SELECT TOP 15 No, Name , Name as mingcheng FROM WF_Emp WHERE No LIKE '@Key%'";
            //sql += "<br />关键字查询SQL:<br>SELECT  No, Name , Name as mingcheng FROM WF_Emp WHERE No LIKE '@Key%'";
            //this.Pub1.AddBR(sql.Replace("~", "\""));
            //this.Pub1.AddB("<br />注意:");
            //this.Pub1.AddBR("1,文本框自动完成填充事例: 必须有No,Name两列，它用于显示下列出的提示列表。");
            //this.Pub1.AddBR("2,设置合适的记录数量，能够改善系统执行效率。");
            //this.Pub1.AddBR("3,@Key 是系统约定的关键字，就是当用户输入一个字符后ccform就会传递此关键字到数据库查询把结果返回给用户。");
            //this.Pub1.AddBR("4,其它的列与本表单的字段名相同则可自动填充，要注意大小写匹配。");
            //this.Pub1.AddBR("5,关键字查询sql是用来，双点文本框时弹出的查询语句，如果为空就按自动填充的sql计算。");
            ////this.Pub1.AddFieldSetEnd();
            //this.Pub1.AddEasyUiPanelInfoEnd();
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
            me.AttrOfOper = this.RefNo;
            me.FK_MapData = this.FK_MapData;
            me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper;
            try
            {
                //DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(me.Doc);
                //if (string.IsNullOrEmpty(me.Tag) == false)
                //{
                //    dt = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
                //    if (dt.Columns.Contains("Name") == false || dt.Columns.Contains("No") == false)
                //        throw new Exception("在您的sql表达式里，必须有No,Name 还两个列。");
                //}

                //if (this.ExtType == MapExtXmlList.TBFullCtrl)
                //{
                //    if (dt.Columns.Contains("Name") == false || dt.Columns.Contains("No") == false)
                //        throw new Exception("在您的sql表达式里，必须有No,Name 还两个列。");
                //}

                //MapAttrs attrs = new MapAttrs(this.FK_MapData);
                //foreach (DataColumn dc in dt.Columns)
                //{
                //    if (dc.ColumnName.ToLower() == "no" || dc.ColumnName.ToLower() == "name")
                //        continue;

                //    if (attrs.Contains(MapAttrAttr.KeyOfEn, dc.ColumnName) == false)
                //        throw new Exception("@系统没有找到您要匹配的列(" + dc.ColumnName + ")，注意:您要指定的列名区分大小写。");
                //}
                me.Save();
            }
            catch (Exception ex)
            {
                this.Pub1.AlertMsg_Warning("SQL错误", ex.Message);
                return;
            }
            this.Response.Redirect("TBFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "&MyPK=" + this.MyPK + "", true);
        }

        public void EditAutoJL()
        {
          //  string mypk= this.DoType
            string mypk = this.ExtType+  "_" + this.FK_MapData + "_" + this.RefNo;
            MapExt myme = new MapExt(mypk);
            MapAttrs attrs = new MapAttrs(myme.FK_MapData);
            string[] strs = myme.Tag.Split('$');

            this.Pub1.AddTableNormal();
            this.Pub1.AddTRGroupTitle("<a href='TBFullCtrl.aspx?ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "'>返回</a> - 设置级连菜单");

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
                this.Response.Redirect("TBFullCtrl.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo, true);
                return;
            }

            string mypk = this.ExtType + "_" + this.FK_MapData + "_" + this.RefNo;


            MapExt myme = new MapExt(mypk);
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