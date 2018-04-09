using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;


namespace CCFlow.WF.MapDef
{
    public partial class InputCheck : System.Web.UI.Page
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
                return MapExtXmlList.InputCheck;
            }
        }

        public string MyPK
        {
            get
            {
                string s = this.Request.QueryString["MyPK"];
                if (s == "")
                    s = this.Request.QueryString["PK"];
                if (s == "")
                    s = null;
                return s;
            }
        }

        public string RefNo
        {
            get
            {
                string s = this.Request.QueryString["RefNo"];
                if (s == null)
                    s = this.Request.QueryString["No"];
                if (s == null)
                    s = this.Request.QueryString["MyPK"];
                return s;
            }
        }

        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }

        public string Lab
        {
            get { return "脚本验证"; }
        }
        #endregion 属性。

        string temFile = "s@xa";

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = this.Lab;

            switch (this.DoType)
            {
                case "Del":
                    MapExt mm = new MapExt();
                    mm.MyPK = this.MyPK;
                    mm.Retrieve();
                    mm.Delete();
                    this.Response.Redirect("InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo, true);
                    return;
                default:
                    break;
            }

            if (this.MyPK != null || this.DoType == "New")
            {
                Edit_InputCheck();
                return;
            }

            MapExts mes = new MapExts();
            mes.Retrieve(MapExtAttr.ExtType, this.ExtType,
                MapExtAttr.FK_MapData, this.FK_MapData);

            this.MapJS(mes);
        }

        public void Edit_InputCheck()
        {
            MapExt me = null;
            if (this.MyPK == null)
            {
                me = new MapExt();
                this.Pub1.AddEasyUiPanelInfoBegin("新建:" + this.Lab, "icon-new");
            }
            else
            {
                me = new MapExt(this.MyPK);
                this.Pub1.AddEasyUiPanelInfoBegin("编辑:" + this.Lab, "icon-edit");
            }
            me.FK_MapData = this.FK_MapData;
            temFile = me.Tag;

            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
            MapAttr attr = new MapAttr(this.RefNo);
            this.Pub1.AddTRGroupTitle(2, attr.KeyOfEn + " - " + attr.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTD("函数库来源:");
            this.Pub1.AddTDBegin();

            RadioButton rb = new RadioButton();
            rb.Text = "ccflow系统js函数库.";
            rb.ID = "RB_0";
            rb.AutoPostBack = true;
            if (me.DoWay == 0)
                rb.Checked = true;
            else
                rb.Checked = false;
            rb.GroupName = "s";
            rb.CheckedChanged += new EventHandler(rb_CheckedChanged);
            this.Pub1.Add(rb);

            rb = new RadioButton();
            rb.AutoPostBack = true;
            rb.Text = "我自定义的函数库.";
            rb.CheckedChanged += new EventHandler(rb_CheckedChanged);
            rb.GroupName = "s";
            rb.ID = "RB_1";
            rb.AutoPostBack = true;
            if (me.DoWay == 1)
                rb.Checked = true;
            else
                rb.Checked = false;
            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("colspan=2", "函数列表");
            this.Pub1.AddTREnd();
            this.Pub1.AddTR();

            ListBox lb = new ListBox();
            lb.Attributes["width"] = "100%";
            lb.AutoPostBack = false;
            lb.ID = "LB1";
            this.Pub1.AddTD("colspan=2", lb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();

            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveInputCheck_Click);

            this.Pub1.AddTD(btn);
            this.Pub1.AddTD("<a class='easyui-linkbutton' data-options=\"iconCls:'icon-back'\" href='InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "'>返回</a>");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            this.Pub1.AddEasyUiPanelInfoEnd();
            rb_CheckedChanged(null, null);
        }

        void rb_CheckedChanged(object sender, EventArgs e)
        {
            string path = BP.Sys.SystemConfig.PathOfData + "\\JSLib\\";
            RadioButton rb = this.Pub1.GetRadioButtonByID("RB_0");
            if (rb.Checked == false)
                path = BP.Sys.SystemConfig.PathOfDataUser + "\\JSLib\\";

            ListBox lb = this.Pub1.FindControl("LB1") as ListBox;
            lb.Items.Clear();
            lb.AutoPostBack = false;
            lb.SelectionMode = ListSelectionMode.Multiple;
            lb.Rows = 10;

            string file = temFile;
            if (DataType.IsNullOrEmpty(temFile) == false)
            {
                file = file.Substring(file.LastIndexOf('\\') + 4);
                file = file.Replace(".js", "");
            }
            else
            {
                file = "!!!";
            }

            MapExts mes = new MapExts();
            mes.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.AttrOfOper, this.OperAttrKey,
                MapExtAttr.ExtType, this.ExtType);

            string[] dirs = System.IO.Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                string[] strs = Directory.GetFiles(dir);
                foreach (string s in strs)
                {
                    if (s.Contains(".js") == false)
                        continue;

                    ListItem li = new ListItem(s.Replace(path, "").Replace(".js", ""), s);

                    if (s.Contains(file))
                        li.Selected = true;

                    lb.Items.Add(li);
                }
            }
        }

        void btn_SaveInputCheck_Click(object sender, EventArgs e)
        {
            ListBox lb = this.Pub1.FindControl("LB1") as ListBox;

            // 检查路径. 没有就创建它。
            string pathDir = BP.Sys.SystemConfig.PathOfDataUser + "\\JSLibData\\";
            if (Directory.Exists(pathDir) == false)
                Directory.CreateDirectory(pathDir);

            // 删除已经存在的数据.
            MapExt me = new MapExt();
            me.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.ExtType, this.ExtType,
                MapExtAttr.AttrOfOper, this.OperAttrKey);

            foreach (ListItem li in lb.Items)
            {
                if (li.Selected == false)
                    continue;

                me = (MapExt)this.Pub1.Copy(me);
                me.ExtType = this.ExtType;

                // 操作的属性.
                me.AttrOfOper = this.OperAttrKey;

                int doWay = 0;
                if (this.Pub1.GetRadioButtonByID("RB_0").Checked == false)
                    doWay = 1;

                me.DoWay = doWay;
                me.Doc = BP.DA.DataType.ReadTextFile(li.Value);
                FileInfo info = new FileInfo(li.Value);
                me.Tag2 = info.Directory.Name;

                //获取函数的名称.
                string func = me.Doc;
                func = me.Doc.Substring(func.IndexOf("function") + 8);
                func = func.Substring(0, func.IndexOf("("));
                me.Tag1 = func.Trim();

                // 检查路径,没有就创建它.
                FileInfo fi = new FileInfo(li.Value);
                me.Tag = li.Value;
                me.FK_MapData = this.FK_MapData;
                me.ExtType = this.ExtType;
                me.MyPK = this.FK_MapData + "_" + me.ExtType + "_" + me.AttrOfOper + "_" + me.Tag1;
                try
                {
                    me.Insert();
                }
                catch
                {
                    me.Update();
                }
            }

            #region 把所有的js 文件放在一个文件里面。
            MapExts mes = new MapExts();
            mes.Retrieve(MapExtAttr.FK_MapData, this.FK_MapData,
                MapExtAttr.ExtType, this.ExtType);

            string js = "";
            foreach (MapExt me1 in mes)
            {
                js += "\r\n" + BP.DA.DataType.ReadTextFile(me1.Tag);
            }

            if (File.Exists(pathDir + "\\" + this.FK_MapData + ".js"))
                File.Delete(pathDir + "\\" + this.FK_MapData + ".js");

            BP.DA.DataType.WriteFile(pathDir + "\\" + this.FK_MapData + ".js", js);
            #endregion 把所有的js 文件放在一个文件里面。

            this.Response.Redirect("InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&RefNo=" + this.RefNo, true);
        }

        public void MapJS(MapExts ens)
        {
            this.Pub1.AddTableNormal();
            this.Pub1.AddTRGroupTitle(5, this.Lab);

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitleCenter("字段");
            this.Pub1.AddTDGroupTitleCenter("类型");
            this.Pub1.AddTDGroupTitleCenter("验证函数中文名");
            this.Pub1.AddTDGroupTitleCenter("显示");
            this.Pub1.AddTDGroupTitleCenter("操作");
            this.Pub1.AddTREnd();

            MapAttrs attrs = new MapAttrs(this.FK_MapData);

            foreach (MapAttr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                MapExt myEn = null;

                foreach (MapExt en in ens)
                {
                    if (en.AttrOfOper == attr.KeyOfEn)
                    {
                        myEn = en;
                        break;
                    }
                }

                if (myEn == null)
                {
                    this.Pub1.AddTRTX();
                    this.Pub1.AddTD(attr.KeyOfEn + "-" + attr.Name);
                    this.Pub1.AddTD("无");
                    this.Pub1.AddTD("无");
                    this.Pub1.AddTD("无");
                    this.Pub1.AddTDBegin();
                    this.Pub1.AddEasyUiLinkButton("编辑",
                                                  "InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType +
                                                  "&RefNo=" + attr.MyPK + "&OperAttrKey=" + attr.KeyOfEn + "&DoType=New",
                                                  "icon-edit");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
                else
                {
                    this.Pub1.AddTRTX();
                    this.Pub1.AddTD(attr.KeyOfEn + "-" + attr.Name);

                    if (myEn.DoWay == 0)
                        this.Pub1.AddTD("系统函数");
                    else
                        this.Pub1.AddTD("自定义函数");

                    string file = myEn.Tag;
                    file = file.Substring(file.LastIndexOf('\\') + 4);
                    file = file.Replace(".js", "");

                    this.Pub1.AddTDA("InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&MyPK=" + myEn.MyPK + "&RefNo=" + attr.MyPK + "&OperAttrKey=" + attr.KeyOfEn, file);
                    this.Pub1.AddTD(myEn.Tag2 + "=" + myEn.Tag1 + "(this);");
                    this.Pub1.AddTDBegin();
                    this.Pub1.AddEasyUiLinkButton("删除",
                                                  "javascript:DoDel('" + myEn.MyPK + "','" + this.FK_MapData + "','" +
                                                  this.ExtType + "');", "icon-delete");
                    this.Pub1.AddTDEnd();
                    this.Pub1.AddTREnd();
                }
            }

            this.Pub1.AddTableEnd();
        }
    }
}