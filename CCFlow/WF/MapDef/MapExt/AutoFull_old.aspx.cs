//edited by liuxc,2015-3-6,增加该页面的easyui样式
using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web.Controls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;
using BP.DA;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_AutoFullUI : BP.Web.WebPage
    {
        #region 属性
        /// <summary>
        /// 执行类型
        /// </summary>
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public int FType
        {
            get
            {
                return int.Parse(this.Request.QueryString["FType"]);
            }
        }
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.RefNo == null)
            {
                /*请选择要设置的字段*/
                MapAttrs mattrs = new MapAttrs();
                mattrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData);

                //this.Pub1.AddFieldSet("请选择要设置的字段");
                this.Pub1.AddEasyUiPanelInfoBegin("选择要设置的字段");
                this.Pub1.AddUL("class='navlist'");

                foreach (MapAttr en in mattrs)
                {

                    if (en.UIVisible == false && en.UIIsEnable == false)
                        continue;

                    //this.Pub1.AddLi("?FK_MapData=" + this.FK_MapData + "&RefNo=" + en.MyPK + "&ExtType=AutoFull", en.KeyOfEn + " - " + en.Name);
                    this.Pub1.Add("<li style=\"font-weight:bold\"><div><a href=\"?FK_MapData=" + this.FK_MapData +
                                  "&RefNo=" + en.MyPK + "&ExtType=AutoFull\"><span class=\"nav\">" + en.KeyOfEn + " - " +
                                  en.Name + "</span></a></div></li>");
                    ;
                }

                this.Pub1.AddULEnd();
                this.Pub1.AddEasyUiPanelInfoEnd();
                return;
            }

            MapAttr mattr = new MapAttr(this.RefNo);
            Attr attr = mattr.HisAttr;
            //this.Page.Title = "为[" + mattr.KeyOfEn + "][" + mattr.Name + "]设置自动完成"; // this.ToE("GuideNewField");

            switch (attr.MyDataType)
            {
                case BP.DA.DataType.AppRate:
                case BP.DA.DataType.AppMoney:
                case BP.DA.DataType.AppInt:
                case BP.DA.DataType.AppFloat:
                    BindNumType(mattr);
                    break;
                case BP.DA.DataType.AppString:
                    BindStringType(mattr);
                    break;
                default:
                    BindStringType(mattr);
                    break;
            }

        }
        public void BindStringType(MapAttr mattr)
        {
            BindNumType(mattr);
        }
        public void BindNumType(MapAttr mattr_del)
        {
            bool isItem = true;
            int idx = 1;

            MapExt me = new MapExt();
            me.MyPK = this.RefNo + "_AutoFull";
            me.RetrieveFromDBSources();

            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
            this.Pub1.AddCaptionLeft("为字段[" + this.RefNo + "]设置自动完成." + BP.WF.Glo.GenerHelpCCForm("设置自动计算", "http://ccform.mydoc.io/?v=5769&t=20135", "ss"));


            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            RadioBtn rb = new RadioBtn();
            rb.GroupName = "s";
            rb.Text = "方式0：不做任何设置。";
            rb.ID = "RB_Way_0";
            if (me.Tag == "0")
                rb.Checked = true;
            this.Pub1.AddTD(rb);
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTDBegin();
            rb = new RadioBtn();
            rb.GroupName = "s";
            rb.Text = "方式1：本表单中数据计算。"; //"";
            rb.ID = "RB_Way_1";
            if (me.Tag == "1")
                rb.Checked = true;

            this.Pub1.Add(rb);
            this.Pub1.Add("比如:@单价*@数量，或者：@DanJia*@ShuLiang , “@”后面的字符必须是本表的字段。");
            this.Pub1.AddBR();

            TextBox tb = new TextBox();
            tb.ID = "TB_JS";
            //tb.Width = 450;
            tb.Style.Add("width", "99%");
            tb.TextMode = TextBoxMode.SingleLine;
            //tb.Rows = 2;
            if (me.Tag == "1")
                tb.Text = me.Doc;

            this.Pub1.Add(tb);
            //this.Pub1.AddFieldSetEnd();
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            // 方式2 利用SQL自动填充
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTDBegin();
            rb = new RadioBtn();
            rb.GroupName = "s";
            rb.Text =  "方式2：利用SQL自动填充(此功能已经在ccflow5中取消，需要此功能的请把逻辑放入的节点或者表单事件里完成)。";
            rb.ID = "RB_Way_2";
            rb.Enabled = false;
            if (me.Tag == "2")
                rb.Checked = true;
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            this.Pub1.Add( "比如:Select Addr From 商品表 WHERE No=@FK_Pro  FK_Pro是本表中的任意字段名<BR>");

            tb = new TextBox();
            tb.ID = "TB_SQL";
            //tb.Width = 450;
            tb.Style.Add("width", "99%");
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            if (me.Tag == "2")
                tb.Text = me.Doc;
            this.Pub1.Add(tb);

            //this.Pub1.AddFieldSetEnd();
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            // 方式3 本表单中外键列
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTDBegin();

            rb = new RadioBtn();
            rb.GroupName = "s";
            rb.Text = "方式3：本表单中外键列。" ;
            // rb.Text = "方式3：本表单中外键列</font></b>";
            rb.ID = "RB_Way_3";
            if (me.Tag == "3")
                rb.Checked = true;

            //if (mattr.HisAutoFull == AutoFullWay.Way3_FK)

            //this.Pub1.AddFieldSet(rb);
            this.Pub1.Add(rb);
            this.Pub1.AddBR();
            this.Pub1.Add("比如:表单中有商品编号列,需要填充商品地址、供应商电话。");
            this.Pub1.AddBR();

            // 让它等于外键表的一个值。
            Attrs attrs = null;
            MapData md = new MapData();
            md.No = this.FK_MapData;
            if (md.RetrieveFromDBSources() == 0)
            {
                attrs = md.GenerHisMap().HisFKAttrs;
            }
            else
            {
                MapDtl mdtl = new MapDtl();
                mdtl.No = this.FK_MapData;
                attrs = mdtl.GenerMap().HisFKAttrs;
            }

            if (attrs.Count > 0)
            {
            }
            else
            {
                rb.Enabled = false;
                if (rb.Checked)
                    rb.Checked = false;
                this.Pub1.Add("@本表没有外键字段。");
            }

            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr)
                    continue;

                rb = new RadioBtn();
                rb.Text = attr.Desc;
                rb.ID = "RB_FK_" + attr.Key;
                rb.GroupName = "sd";

                if (me.Doc.Contains(attr.Key))
                    rb.Checked = true;

                this.Pub1.Add(rb);
                DDL ddl = new DDL();
                ddl.ID = "DDL_" + attr.Key;

                string sql = "";
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.Informix:
                        continue;
                        //sql = "Select fname as 'No' ,fDesc as 'Name' FROM Sys_FieldDesc WHERE tableName='" + attr.HisFKEn.EnMap.PhysicsTable + "'";
                        //break;
                    case DBType.MySQL:
                        //sql = "Select COLUMN_NAME as No,COLUMN_NAME as Name from information_schema.COLUMNS WHERE TABLE_NAME='" + attr.HisFKEn.EnMap.PhysicsTable + "'";
                        break;
                    default:
                        sql = "Select name as 'No' ,Name as 'Name' from syscolumns WHERE ID=OBJECT_ID('" + attr.HisFKEn.EnMap.PhysicsTable + "')";
                        break;
                }

                //  string sql = "Select fname as 'No' ,fDesc as 'Name' FROM Sys_FieldDesc WHERE tableName='" + attr.HisFKEn.EnMap.PhysicsTable + "'";
                //string sql = "Select NO , NAME  FROM Port_Emp ";

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    //  ddl.Items.Add(new ListItem(this.ToE("Field") + dr[0].ToString() + " " + this.ToE("Desc") + " " + dr[1].ToString(), dr[0].ToString()));
                    ListItem li = new ListItem(dr[0].ToString() + "；" + dr[1].ToString(), dr[0].ToString());
                    if (me.Doc.Contains(dr[0].ToString()))
                        li.Selected = true;

                    ddl.Items.Add(li);
                }

                this.Pub1.Add(ddl);
                this.Pub1.AddBR();
            }

            //this.Pub1.AddFieldSetEnd();
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            // 方式3 本表单中外键列
            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTDBegin();

            rb = new RadioBtn();
            rb.GroupName = "s";
            rb.Text =  "方式4：对一个从表的列求值。";
            rb.ID = "RB_Way_4";
            if (me.Tag == "4")
                rb.Checked = true;

            //this.Pub1.AddFieldSet(rb);
            this.Pub1.Add(rb);
            this.Pub1.Add( "比如:对从表中的列求值。");
            this.Pub1.AddBR();

            // 让它对一个从表求和、求平均、求最大、求最小值。
            MapDtls dtls = new MapDtls(this.FK_MapData);
            if (dtls.Count > 0)
            {
            }
            else
            {
                rb.Enabled = false;
                if (rb.Checked)
                    rb.Checked = false;
                // this.Pub1.Add("@没有从表。");
            }
            foreach (MapDtl dtl in dtls)
            {
                DDL ddlF = new DDL();
                ddlF.ID = "DDL_" + dtl.No + "_F";
                MapAttrs mattrs1 = new MapAttrs(dtl.No);
                int count = 0;
                foreach (MapAttr mattr1 in mattrs1)
                {
                    if (mattr1.LGType != FieldTypeS.Normal)
                        continue;

                    if (mattr1.KeyOfEn == MapAttrAttr.MyPK)
                        continue;

                    if (mattr1.IsNum == false)
                        continue;
                    switch (mattr1.KeyOfEn)
                    {
                        case "OID":
                        case "RefOID":
                        case "FID":
                            continue;
                        default:
                            break;
                    }
                    count++;
                    ListItem li = new ListItem(mattr1.Name, mattr1.KeyOfEn);
                    if (me.Tag  == "4")
                        if (me.Doc.Contains("=" + mattr1.KeyOfEn))
                            li.Selected = true;
                    ddlF.Items.Add(li);
                }
                if (count == 0)
                    continue;

                rb = new RadioBtn();
                rb.Text = dtl.Name;
                rb.ID = "RB_" + dtl.No;
                rb.GroupName = "dtl";
                if (me.Doc.Contains(dtl.No))
                    rb.Checked = true;

                this.Pub1.Add(rb);

                DDL ddl = new DDL();
                ddl.ID = "DDL_" + dtl.No + "_Way";
                ddl.Items.Add(new ListItem("求合计", "SUM"));
                ddl.Items.Add(new ListItem("求平均", "AVG"));
                ddl.Items.Add(new ListItem("求最大", "MAX"));
                ddl.Items.Add(new ListItem("求最小", "MIN"));
                this.Pub1.Add(ddl);

                if (me.Tag  == "4")
                {
                    if (me.Doc.Contains("SUM"))
                        ddl.SetSelectItem("SUM");
                    if (me.Doc.Contains("AVG"))
                        ddl.SetSelectItem("AVG");
                    if (me.Doc.Contains("MAX"))
                        ddl.SetSelectItem("MAX");
                    if (me.Doc.Contains("MIN"))
                        ddl.SetSelectItem("MIN");
                }

                this.Pub1.Add(ddlF);
                this.Pub1.AddBR();
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            
            this.Pub1.AddTableEnd();
            this.Pub1.AddBR();

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
            this.Pub1.AddSpace(1);

            btn = new LinkBtn(false, NamesOfBtn.SaveAndClose, "保存并关闭");
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Click(object sender, EventArgs e)
        {
            MapAttr mattrNew = new MapAttr(this.RefNo);

            MapExt me = new MapExt();
            me.MyPK =   this.RefNo + "_AutoFull";
            me.RetrieveFromDBSources();
            me.FK_MapData = this.FK_MapData;
            me.AttrOfOper = mattrNew.KeyOfEn;
            me.ExtType = MapExtXmlList.AutoFull;
            if (this.Pub1.GetRadioButtonByID("RB_Way_0").Checked)
            {
                me.Tag = "0";
            }

            // JS 方式。
            if (this.Pub1.GetRadioButtonByID("RB_Way_1").Checked)
            {
                me.Tag = "1";
                me.Doc = this.Pub1.GetTextBoxByID("TB_JS").Text;

                /*检查字段是否填写正确.*/
                MapAttrs attrsofCheck = new MapAttrs(this.FK_MapData);
                string docC = me.Doc;
                foreach (MapAttr attrC in attrsofCheck)
                {
                    if (attrC.IsNum == false)
                        continue;
                    docC = docC.Replace("@" + attrC.KeyOfEn, "");
                    docC = docC.Replace("@" + attrC.Name, "");
                }

                if (docC.Contains("@"))
                {
                    this.Alert("您填写的表达公式不正确，导致一些数值类型的字段没有被正确的替换。" + docC);
                    return;
                }
            }

            // 外键方式。
            if (this.Pub1.GetRadioButtonByID("RB_Way_2").Checked)
            {
                me.Tag = "2";
                me.Doc = this.Pub1.GetTextBoxByID("TB_SQL").Text;

                //mattr.HisAutoFull = AutoFullWay.Way2_SQL;
                //mattr.AutoFullDoc = this.Pub1.GetTextBoxByID("TB_SQL").Text;
            }

            // 本表单中外键列。
            string doc = "";
            if (this.Pub1.GetRadioButtonByID("RB_Way_3").Checked)
            {
                me.Tag = "3";

               // mattr.HisAutoFull = AutoFullWay.Way3_FK;
                MapData md = new MapData(this.FK_MapData);
                Attrs attrs = md.GenerHisMap().HisFKAttrs;
                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr)
                        continue;

                    if (this.Pub1.GetRadioButtonByID("RB_FK_" + attr.Key).Checked == false)
                        continue;
                    // doc = " SELECT " + this.Pub1.GetDDLByID("DDL_" + attr.Key).SelectedValue + " FROM " + attr.HisFKEn.EnMap.PhysicsTable + " WHERE NO=@" + attr.Key;
                    doc = "@AttrKey=" + attr.Key + "@Field=" + this.Pub1.GetDDLByID("DDL_" + attr.Key).SelectedValue + "@Table=" + attr.HisFKEn.EnMap.PhysicsTable;
                }
                me.Doc = doc;
            }

            // 本表单中从表列。
            if (this.Pub1.GetRadioButtonByID("RB_Way_4").Checked)
            {
                me.Tag = "4";

                MapDtls dtls = new MapDtls(this.FK_MapData);
             //   mattr.HisAutoFull = AutoFullWay.Way4_Dtl;
                foreach (MapDtl dtl in dtls)
                {
                    try
                    {
                        if (this.Pub1.GetRadioButtonByID("RB_" + dtl.No).Checked == false)
                            continue;
                    }
                    catch
                    {
                        continue;
                    }
                    //  doc = "SELECT " + this.Pub1.GetDDLByID( "DDL_"+dtl.No + "_Way").SelectedValue + "(" + this.Pub1.GetDDLByID("DDL_"+dtl.No+"_F").SelectedValue + ") FROM " + dtl.No + " WHERE REFOID=@OID";
                    doc = "@Table=" + dtl.No + "@Field=" + this.Pub1.GetDDLByID("DDL_" + dtl.No + "_F").SelectedValue + "@Way=" + this.Pub1.GetDDLByID("DDL_" + dtl.No + "_Way").SelectedValue;
                }
                me.Doc = doc;
            }

            try
            {
                me.Save();
            }
            catch (Exception ex)
            {
                this.ResponseWriteRedMsg(ex);
                return;
            }

            this.Alert("保存成功");
            this.Pub1.Clear();
            //Button btn = sender as Button;
            var btn = sender as LinkBtn;
            if (btn.ID.Contains("Close"))
            {
                this.WinClose();
                return;
            }
            else
            {
                this.Response.Redirect(this.Request.RawUrl, true);
            }
        }
        public void BindStringType()
        {
        }
        public string GetCaption
        {
            get
            {
                if (this.DoType == "Add")
                    return "新增向导" + " - <a href='Do.aspx?DoType=ChoseFType'>选择类型</a> - " + "编辑";
                else
                    return "<a href='Do.aspx?DoType=ChoseFType&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo + "'>选择类型</a> - " + "编辑";
            }
        }
    }
}