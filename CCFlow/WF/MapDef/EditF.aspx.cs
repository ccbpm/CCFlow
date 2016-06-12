using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using BP.Web.Controls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;
using BP.DA;
using BP.WF.Template;
using BP.WF.Template.XML;
using BP.Sys.XML;
using BP.WF.XML;

namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_EditF : BP.Web.WebPage
    {
        #region 属性。
        /// <summary>
        /// GroupField
        /// </summary>
        public int GroupField
        {
            get
            {
                string s = this.Request.QueryString["GroupField"];
                if (s == "" || s == null)
                    return 0;
                return int.Parse(s);
            }
        }
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
        public string FK_MapData
        {
            get
            {
                string str= this.Request.QueryString["FK_MapData"];
                return str;
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
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.MyPK == null)
                throw new Exception("Mypk==null");

            this.Title = "编辑字段";

            switch (this.DoType)
            {
                case "Add":
                    this.Add();
                    break;
                case "Edit":
                    MapAttr attr = new MapAttr();
                    attr.MyPK = this.RefNo;
                    attr.RetrieveFromDBSources();
                    attr.MyDataType = this.FType;

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        this.Response.Redirect("EditSQL.aspx?MyPK="+this.MyPK+"&RefNo="+this.RefNo, true);
                        return;
                    }

                    switch (attr.MyDataType)
                    {
                        case BP.DA.DataType.AppString:
                            this.EditString(attr);
                            break;
                        case BP.DA.DataType.AppDateTime:
                        case BP.DA.DataType.AppDate:
                        case BP.DA.DataType.AppInt:
                        case BP.DA.DataType.AppFloat:
                        case BP.DA.DataType.AppMoney:
                        case BP.DA.DataType.AppDouble:
                            this.EditInt(attr);
                            break;
                        case BP.DA.DataType.AppBoolean:
                            this.EditBool(attr);
                            break;
                        default:
                            throw new Exception("为考虑的类型" + this.FType);
                    }
                    break;
                default:
                    break;
            }
        }
        public void Add()
        {
            MapAttr attr = new MapAttr();
            attr.MyDataType = this.FType;
            attr.FK_MapData = this.MyPK;
            attr.UIIsEnable = true;
            switch (this.FType)
            {
                case DataType.AppString:
                    this.EditString(attr);
                    break;
                case DataType.AppInt:
                case DataType.AppDateTime:
                case DataType.AppDate:
                case DataType.AppFloat:
                case DataType.AppDouble:
                case DataType.AppMoney:
                    this.EditInt(attr);
                    break;
                case DataType.AppBoolean:
                    this.EditBool(attr);
                    break;
                default:
                    break;
            }
        }
        int idx = 1;
        bool isItem = false;
        public void EditBeforeAdd(MapAttr mapAttr)
        {
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("备注");
            this.Pub1.AddTREnd();

            if (mapAttr.IsTableAttr)
            {
                /* if here is table attr, It's will let use can change data type. */
                isItem=this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("改变数据类型");
                DDL ddlType = new DDL();
                ddlType.ID = "DDL_DTType";
                SysDataTypes xmls = new SysDataTypes();
                xmls.RetrieveAll();

                ddlType.Bind(xmls, "No", "Name");

                ddlType.SetSelectItem(mapAttr.MyDataTypeS);

                ddlType.AutoPostBack = true;
                ddlType.SelectedIndexChanged += new EventHandler(ddlType_SelectedIndexChanged);

                this.Pub1.AddTD(ddlType);
                this.Pub1.AddTD("");
                this.Pub1.AddTREnd();
            }

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段中文名称");
            TB tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = mapAttr.Name;
            tb.Attributes["width"] = "100%";

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段英文名");
            tb = new TB();
            tb.ID = "TB_KeyOfEn";
            tb.Text = mapAttr.KeyOfEn;
            if (this.RefNo != null)
                tb.Enabled = false;

            tb.Attributes["onkeyup"] = "return IsDigit(this);";
            this.Pub1.AddTD(tb);

            if (string.IsNullOrEmpty(mapAttr.KeyOfEn))
                this.Pub1.AddTD("字母/数字/下划线组合");
            else
                this.Pub1.AddTD("<a href=\"javascript:clipboardData.setData('Text','" + mapAttr.KeyOfEn + "');alert('已经copy到粘帖版上');\" ><img src='../Img/Btn/Copy.gif' class='ICON' />复制字段名</a></TD>");

            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("默认值");
            tb = new TB();
            tb.ID = "TB_DefVal";
            tb.Text = mapAttr.DefValReal;
           // tb.Columns = 70;
           // tb.Attributes["width"] = "100px";

            switch (this.FType)
            {
                case BP.DA.DataType.AppDouble:
                case BP.DA.DataType.AppInt:
                case BP.DA.DataType.AppFloat:
                    this.Pub1.AddTDNum(tb);
                    tb.ShowType = TBType.Num;
                    tb.Text = mapAttr.DefVal;
                    if (tb.Text == "")
                        tb.Text = "0";
                    break;
                case BP.DA.DataType.AppMoney:
                case BP.DA.DataType.AppRate:
                    this.Pub1.AddTDNum(tb);
                    tb.ShowType = TBType.Moneny;
                    break;
                default:
                    this.Pub1.AddTD(tb);
                    break;
            }

            tb.ShowType = mapAttr.HisTBType;
            switch (this.FType)
            {
                case DataType.AppDateTime:
                case DataType.AppDate:
                    CheckBox cb = new CheckBox();
                    cb.Text = "默认系统当前日期";
                    cb.ID = "CB_DefVal";
                    if (mapAttr.DefValReal == "@RDT")
                        cb.Checked = true;
                    else
                        cb.Checked = false;
                    cb.AutoPostBack = true;
                    cb.CheckedChanged += new EventHandler(cb_CheckedChanged_rdt);
                    this.Pub1.AddTD(cb);
                    break;
                case DataType.AppString:
                    DDL ddl = new DDL();
                    ddl.AutoPostBack = true;

                    BP.WF.XML.DefVals vals = new BP.WF.XML.DefVals();
                    vals.Retrieve("Lang", WebUser.SysLang);
                    foreach (BP.WF.XML.DefVal def in vals)
                        ddl.Items.Add(new ListItem(def.Name, def.Val));

                    //ddl.Items.Add(new ListItem("选择系统约定默认值", ""));
                    //ddl.Items.Add(new ListItem("操作员编号", "@WebUser.No"));
                    //ddl.Items.Add(new ListItem("操作员名称", "@WebUser.Name"));
                    //ddl.Items.Add(new ListItem("隶属部门编号", "@WebUser.FK_Dept"));
                    //ddl.Items.Add(new ListItem("隶属部门名称", "@WebUser.FK_DeptName"));

                    //ddl.Items.Add(new ListItem("当前日期-1", "@yyyy年mm月dd日"));
                    //ddl.Items.Add(new ListItem("当前日期-2", "@yy年mm月dd日"));

                    //ddl.Items.Add(new ListItem("当前年度", "@FK_ND"));
                    //ddl.Items.Add(new ListItem("当前月份", "@FK_YF"));

                    ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged_DefVal);
                    ddl.SetSelectItem(mapAttr.DefValReal);
                    ddl.ID = "DDL_SelectDefVal";
                    this.Pub1.AddTD(ddl);
                    break;
                default:
                    this.Pub1.AddTD("&nbsp;");
                    break;
            }
            this.Pub1.AddTREnd();

            #region 是否可以为空.
            switch (this.FType)
            {
                case BP.DA.DataType.AppDouble:
                case BP.DA.DataType.AppInt:
                case BP.DA.DataType.AppFloat:
                case BP.DA.DataType.AppMoney:
                case BP.DA.DataType.AppRate:
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD("是否可以为空");
                    DDL ddlIsNull = new DDL();
                    ddlIsNull.Items.Add(new ListItem("不能为空,按照默认值计算.", "0"));
                    ddlIsNull.Items.Add(new ListItem("可以为空,与默认值无关.", "1"));
                    ddlIsNull.ID = "DDL_IsNull";

                    if (mapAttr.MinLen == 0)
                        ddlIsNull.SetSelectItem(0);
                    else
                        ddlIsNull.SetSelectItem(1);

                    this.Pub1.AddTD("colspan=2", ddlIsNull);
                    this.Pub1.AddTREnd();
                    break;
                default:
                    break;
            }
            #endregion 是否可以为空.


            RadioButton rb = new RadioButton();
            if (MapData.IsEditDtlModel == false)
            {
                //this.Pub1.AddTR();
                //this.Pub1.AddTD("界面上是否可见");
                //this.Pub1.Add("<TD>");
                //rb = new RadioButton();
                //rb.ID = "RB_UIVisible_0";
                //rb.Text = "不 可 见";
                //rb.GroupName = "s1";
                //if (mapAttr.UIVisible)
                //    rb.Checked = false;
                //else
                //    rb.Checked = true;
                //this.Pub1.Add(rb);

                //rb = new RadioButton();
                //rb.ID = "RB_UIVisible_1";
                //rb.Text = "可见 ";
                //rb.GroupName = "s1";

                //if (mapAttr.UIVisible)
                //    rb.Checked = true;
                //else
                //    rb.Checked = false;
                //this.Pub1.Add(rb);
                //this.Pub1.Add("</TD>");
                //this.Pub1.AddTD("控制是否显示在页面上");
                //this.Pub1.AddTREnd();
            }

            #region 是否可编辑.
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否可编辑");
            this.Pub1.Add("<TD>");

            rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_0";
            rb.Text = "不可编辑";
            rb.GroupName = "s";
            rb.Checked = !mapAttr.UIIsEnable;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_1";
            rb.Text = "可编辑";
            rb.GroupName = "s";
            rb.Checked = mapAttr.UIIsEnable;

            this.Pub1.Add(rb);
            this.Pub1.Add("</TD>");
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            #endregion 是否可编辑.


            #region 是否可界面可见
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否界面可见"); //是否界面可见
            this.Pub1.AddTDBegin();
            rb = new RadioButton();
            rb.ID = "RB_UIVisible_0";
            rb.Text = "不可见"; // 界面不可见
            rb.GroupName = "sa3";
            if (mapAttr.UIVisible)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            if (mapAttr.IsTableAttr)
                rb.Enabled = false;

            rb = new RadioButton();
            rb.ID = "RB_UIVisible_1";
            rb.Text = "界面可见"; // 界面可见;
            rb.GroupName = "sa3";

            if (mapAttr.UIVisible)
                rb.Checked = true;
            else
                rb.Checked = false;

            if (mapAttr.IsTableAttr)
                rb.Enabled = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("不可见则为隐藏字段.");
            //   this.Pub1.AddTD("控制该它在表单的界面里是否可见");
            this.Pub1.AddTREnd();
            #endregion 是否可界面可见


       


        }

        void ddl_SelectedIndexChanged_DefVal(object sender, EventArgs e)
        {
            this.Pub1.GetTBByID("TB_DefVal").Text = this.Pub1.GetDDLByID("DDL_SelectDefVal").SelectedItemStringVal;
        }

        void ddlType_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapAttr attr = new MapAttr(this.RefNo);
            attr.MyDataTypeS = this.Pub1.GetDDLByID("DDL_DTType").SelectedItemStringVal;
            attr.Update();
            this.Response.Redirect("EditF.aspx?DoType=" + this.DoType + "&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo + "&FType=" + attr.MyDataType + "&GroupField=" + this.GroupField, true);
            // this.Response.Redirect(this.Request.RawUrl, true);
        }
        public void EditBeforeEnd(MapAttr mapAttr)
        {
            #region 合并单元格数
            isItem=this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("合并单元格数");
            DDL ddl1 = new DDL();
            ddl1.ID = "DDL_ColSpan";
            for (int i = 1; i < 12; i++)
            {
                ddl1.Items.Add(new ListItem(i.ToString(), i.ToString()));
            }
            ddl1.SetSelectItem(mapAttr.ColSpan.ToString());
            this.Pub1.AddTD(ddl1);

            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();
            #endregion 合并单元格数

            #region 字段分组
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段分组");
            DDL ddlGroup = new DDL();
            ddlGroup.ID = "DDL_GroupID";
            GroupFields gfs = new GroupFields(mapAttr.FK_MapData);
            ddlGroup.Bind(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab);
            if (mapAttr.GroupID == 0)
                mapAttr.GroupID = this.GroupField;

            ddlGroup.SetSelectItem(mapAttr.GroupID);

            this.Pub1.AddTD("colspan=3", ddlGroup);
            this.Pub1.AddTREnd();
            #endregion 字段分组

            #region 是否是数字签名字段
            if (mapAttr.UIIsEnable == false && mapAttr.MyDataType == DataType.AppString && mapAttr.LGType == FieldTypeS.Normal)
            {
                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);

                this.Pub1.AddTD("签名模式");

                DDL ddl = new DDL();
                ddl.ID = "DDL_SignType";
                ddl.Items.Add(new ListItem("无", "0"));
                ddl.Items.Add(new ListItem("图片签名", "1"));
                ddl.Items.Add(new ListItem("山东CA签名", "2"));
                ddl.Items.Add(new ListItem("广东CA签名", "3"));

                ddl.SetSelectItem((int)mapAttr.SignType);

                this.Pub1.AddTD(ddl);
                if (mapAttr.SignType == SignType.CA)
                {
                    TB sigan = new TB();
                    sigan.ID = "TB_SiganField";
                    sigan.Text = mapAttr.Para_SiganField;
                    this.Pub1.AddTD(sigan);
                }else
                if (mapAttr.SignType == SignType.GDCA)
                {
                    TB sigan = new TB();
                    sigan.ID = "TB_SiganField";
                    sigan.Text = mapAttr.Para_SiganField;
                    this.Pub1.AddTD(sigan);
                }
                else if (mapAttr.SignType == SignType.Pic)
                {
                    DDL ddlPic = new DDL();
                    ddlPic.ID = "DDL_PicType";
                    ddlPic.Items.Add(new ListItem("自动签名", "0"));
                    ddlPic.Items.Add(new ListItem("手动签名", "1"));
                    ddlPic.SetSelectItem((int)mapAttr.PicType);
                    this.Pub1.AddTD(ddlPic);
                }
                else
                {
                    this.Pub1.AddTD();
                }
                this.Pub1.AddTREnd();
            }
            #endregion 字段分组

            #region 字段分组
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字体大小");

            DDL ddlFont = new DDL();
            ddlFont.ID = "DDL_FontSize";
            ddlFont.Items.Add(new ListItem("默认", "0"));
            for (int i = 10; i < 28; i++)
                ddlFont.Items.Add(new ListItem(i + "px", i.ToString()));

            ddlFont.SetSelectItem(mapAttr.Para_FontSize);
            this.Pub1.AddTD(ddlFont);


            //是否必填字段.
            this.Pub1.Add("<TD>");
            RadioButton rb = new RadioButton();
            rb.ID = "RB_UIIsInput_0";
            rb.Text = "非必填字段";
            rb.GroupName = "si";
            rb.Checked = !mapAttr.UIIsInput;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsInput_1";
            rb.Text = "必填";
            rb.GroupName = "si";
            rb.Checked = mapAttr.UIIsInput;
            this.Pub1.Add(rb);
            this.Pub1.Add("</TD>");

            this.Pub1.AddTREnd();
            #endregion 字段分组

            #region 扩展设置.
            if (this.RefNo != null)
            {
                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/PopVal.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "&MyPK=" + MapExtXmlList.PopVal + "_" + mapAttr.MyPK + "')\">设置开窗返回值</a>");
                string html = "<a href=\"javascript:WinOpen('./MapExt/RegularExpression.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "&OperAttrKey="+mapAttr.MyPK+"')\">正则表达式</a>";
                html += " - <a href=\"javascript:WinOpen('./MapExt/TBFullCtrl.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "&MyPK=" + mapAttr.FK_MapData + "_" + MapExtXmlList.TBFullCtrl + "_" + mapAttr.KeyOfEn + "')\">文本框自动完成</a>";
                html += " - <a href=\"javascript:WinOpen('./MapExt/AotuGenerNo.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "')\">自动生成编号</a>";

                this.Pub1.AddTD(html);
                //this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/AutoFull.aspx?FK_MapData=" + mapAttr.FK_MapData + "&ExtType=AutoFull&RefNo=" + mapAttr.MyPK + "')\">自动计算</a>");
                html = "<a href=\"javascript:WinOpen('./MapExt/AutoFull.aspx?FK_MapData=" + mapAttr.FK_MapData + "&ExtType=AutoFull&RefNo=" + mapAttr.MyPK + "')\">自动计算</a>";
                html += " - <a href=\"javascript:WinOpen('./MapExt/InputCheck.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.MyPK + "')\">脚本验证</a>";
                this.Pub1.AddTD(html);
                this.Pub1.AddTREnd();
            }
            #endregion pop返回值.


            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD colspan=4 align=center>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = "保存并关闭"; // "保存并关闭";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.CssClass = "Btn";
            btn.ID = "Btn_SaveAndNew";
            btn.Text = "保存新建"; // "保存新建";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            if (this.RefNo != null)
            {
                if (mapAttr.HisEditType == EditType.Edit)
                {
                    btn = new Button();
                    btn.ID = "Btn_Del";
                    btn.CssClass = "Btn";
                    btn.Text = "删除";
                    btn.Click += new EventHandler(btn_Save_Click);
                    btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                    this.Pub1.Add(btn);
                }

                string myUrl = "EleBatch.aspx?KeyOfEn=" + mapAttr.KeyOfEn + "&FK_MapData=" + mapAttr.FK_MapData + "&EleType=MapAttr";
                this.Pub1.Add("<a href='" + myUrl + "' target='M" + mapAttr.KeyOfEn + "' ><img src='../Img/Btn/Apply.gif' border=0>批处理</a>");
            }

            string url = "Do.aspx?DoType=AddF&MyPK=" + mapAttr.FK_MapData + "&IDX=" + mapAttr.Idx;
            btn = new Button();
            btn.ID = "Btn_New";
            btn.CssClass = "Btn";
            btn.Text = "新建";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_Back";
            btn.CssClass = "Btn";
            btn.Text = "返回";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEndWithBR();
        }
        public void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.ID)
            {
                case "Btn_New":
                    MapAttr mapAttr = new MapAttr(this.RefNo);
                    string url = "Do.aspx?DoType=AddF&MyPK=" + mapAttr.FK_MapData + "&IDX=" + mapAttr.Idx + "&GroupField = " + this.GroupField;
                    this.Response.Redirect(url, true);
                    return;
                case "Btn_Back":
                    string url1 = "Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&GroupField = " + this.GroupField;
                    this.Response.Redirect(url1, true);
                    return;
                default:
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapAttr"></param>
        public void EditString(MapAttr mapAttr)
        {
            this.EditBeforeAdd(mapAttr);
            TB tb = new TB();
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("最小长度");
            tb = new TB();
            tb.ID = "TB_MinLen";
            tb.CssClass = "TBNum";
            tb.Text = mapAttr.MinLen.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("最大长度");
            tb = new TB();
            tb.ID = "TB_MaxLen";
            tb.CssClass = "TBNum";
            tb.Text = mapAttr.MaxLen.ToString();

            //DDL cb = new DDL();
            //cb.ID = "DDL_TBType";
            //cb.Items.Add(new ListItem("单行文本框", "0"));
            //cb.Items.Add(new ListItem("多行文本框", "1"));
            //cb.Items.Add(new ListItem("Sina编辑框", "2"));
            //cb.Items.Add(new ListItem("FCKEditer编辑框", "3"));

            this.Pub1.AddTD(tb);

            DDL ddlBig = new DDL();
            ddlBig.ID = "DDL_TBModel";
            ddlBig.BindSysEnum("TBModel", mapAttr.TBModel);
            ddlBig.AutoPostBack = true;
            ddlBig.SelectedIndexChanged += new EventHandler(ddlBig_SelectedIndexChanged);

            CheckBox cb = new CheckBox();
            cb.ID = "CB_IsBigField";
            cb.Text = "是否是超长字段";
            cb.Checked = mapAttr.Para_IsImgField;


            this.Pub1.AddTDBegin();
            this.Pub1.Add(ddlBig);
            this.Pub1.Add(cb);
           // this.Pub1.Add("<a href=''>关于超长字段</a>");
            this.Pub1.AddTDEnd();

           // this.Pub1.AddTD(ddlBig);

            //CheckBox cb = new CheckBox();
            //cb.CheckedChanged += new EventHandler(cb_CheckedChanged);
            //cb.ID = "CB_IsM";
            //cb.Text = this.ToE("IsBigDoc", "是否大块文本(对傻瓜表单有效)");
            //cb.AutoPostBack = true;
            //if (mapAttr.MaxLen >= 3000)
            //{
            //    cb.Checked = true;
            //    tb.Enabled = false;
            //}
            //else
            //{
            //    cb.Checked = false;
            //    tb.Enabled = true;
            //}
            //this.Pub1.AddTD(cb);
            //if (mapAttr.IsTableAttr)
            //    cb.Enabled = false;

            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("文本框宽度");
            tb = new TB();
            tb.ID = "TB_UIWidth";
            tb.CssClass = "TBNum";
            tb.Text = mapAttr.UIWidth.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTDB("对从表有效");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("高度");
            tb = new TB();
            tb.ID = "TB_UIHeight";
            tb.CssClass = "TBNum";
            tb.Text = mapAttr.UIHeight.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            this.EditBeforeEnd(mapAttr);
           
        }

        void ddlBig_SelectedIndexChanged(object sender, EventArgs e)
        {
            DDL ddl = this.Pub1.GetDDLByID("DDL_TBModel");
            if (ddl.SelectedItemIntVal != 0)
            {
                this.Pub1.GetTBByID("TB_MaxLen").Text = "4000";
                this.Pub1.GetTBByID("TB_UIHeight").Text = "390";
                this.Pub1.GetTBByID("TB_MaxLen").Enabled = false;
                this.Pub1.GetCBByID("CB_IsBigField").Enabled = false;
                //this.Pub1.GetCBByID("CB_IsBigField").Visible = false;
            }
            else
            {
                this.Pub1.GetTBByID("TB_MaxLen").Enabled = true;
                this.Pub1.GetTBByID("TB_MaxLen").Text = "400";
                this.Pub1.GetCBByID("CB_IsBigField").Enabled = true;
               // this.Pub1.GetCBByID("CB_IsBigField").Visible = true;
            }
        }
        void cb_CheckedChanged_rdt(object sender, EventArgs e)
        {
            CheckBox cb = this.Pub1.GetCBByID("CB_DefVal");
            if (cb.Checked)
            {
                this.Pub1.GetTBByID("TB_DefVal").Text = "@RDT";
            }
            else
            {
                this.Pub1.GetTBByID("TB_DefVal").Text = "";
            }
        }
        void cb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = this.Pub1.GetCBByID("CB_IsM");
            if (cb.Checked)
            {
                this.Pub1.GetTBByID("TB_MaxLen").Enabled = false;
                this.Pub1.GetTBByID("TB_MaxLen").Text = "4000";
                this.Pub1.GetTBByID("TB_UIHeight").Text = "90";
            }
            else
            {
                this.Pub1.GetTBByID("TB_MaxLen").Enabled = true;
                this.Pub1.GetTBByID("TB_MaxLen").Text = "50";
            }
        }
        public void EditInt(MapAttr mapAttr)
        {
            this.EditBeforeAdd(mapAttr);

            TB tb = new TB();
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("文本框宽度");
            tb = new TB();
            tb.ID = "TB_UIWidth";
            tb.CssClass = "TBNum";
            tb.Text = mapAttr.UIWidth.ToString();
            this.Pub1.AddTD(tb);
            this.Pub1.AddTDB("对从表有效");
            this.Pub1.AddTREnd();
            this.EditBeforeEnd(mapAttr);
        }
        public void EditBool(MapAttr mapAttr)
        {
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID"); // 项目
            this.Pub1.AddTDTitle("项目"); // 项目
            this.Pub1.AddTDTitle("采集");   // 值
            this.Pub1.AddTDTitle("描述"); // 描述
            this.Pub1.AddTREnd();

            if (mapAttr.IsTableAttr)
            {
                /* if here is table attr, It's will let use can change data type. */
                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("改变数据类型");
                DDL ddlType = new DDL();
                ddlType.ID = "DDL_DTType";
                SysDataTypes xmls = new  SysDataTypes();
                xmls.RetrieveAll();
                
                ddlType.Bind(xmls, "No", "Name");
                ddlType.SetSelectItem(mapAttr.MyDataTypeS);

                ddlType.AutoPostBack = true;
                ddlType.SelectedIndexChanged += new EventHandler(ddlType_SelectedIndexChanged);

                this.Pub1.AddTD(ddlType);
                this.Pub1.AddTD("");
                this.Pub1.AddTREnd();
            }

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段中文名");
            TB tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = mapAttr.Name;
            tb.Attributes["width"] = "100%";

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段英文名");
            tb = new TB();
            tb.ID = "TB_KeyOfEn";
            tb.Text = mapAttr.KeyOfEn;

            if (this.RefNo != null)
                tb.Enabled = false;

            this.Pub1.AddTD(tb);

            if (string.IsNullOrEmpty(mapAttr.KeyOfEn))
                this.Pub1.AddTD("字母/数字/下划线组合");
            else
                this.Pub1.AddTD("<a href=\"javascript:clipboardData.setData('Text','" + mapAttr.KeyOfEn + "');alert('已经copy到粘帖版上');\" ><img src='../Img/Btn/Copy.gif' class='ICON' />复制字段名</a></TD>");

            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("默认值");
            CheckBox cb = new CheckBox();
            cb.ID = "CB_DefVal";
            cb.Text = "请选择";
            cb.Checked = mapAttr.DefValOfBool;

            this.Pub1.AddTD(cb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否可编辑");
            this.Pub1.Add("<TD>");

            RadioButton rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_0";
            rb.Text = "不可编辑";
            rb.GroupName = "s";
            rb.Checked = !mapAttr.UIIsEnable;
            this.Pub1.Add(rb);


            rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_1";
            rb.Text = "可编辑";
            rb.GroupName = "s";
            rb.Checked = mapAttr.UIIsEnable;
            this.Pub1.Add(rb);

            this.Pub1.Add("</TD>");
            this.Pub1.AddTD();
            // this.Pub1.AddTD(this.ToE("IsReadonly", "是否只读"));
            this.Pub1.AddTREnd();


            #region 是否可单独行显示
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("呈现方式"); //呈现方式;
            this.Pub1.AddTDBegin();
            rb = new RadioButton();
            rb.ID = "RB_UIIsLine_0";
            rb.Text = "两列显示"; // 两行
            rb.GroupName = "sa";
            if (mapAttr.UIIsLine)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsLine_1";
            rb.Text = "整行显示"; // "一行";
            rb.GroupName = "sa";

            if (mapAttr.UIIsLine)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("对傻瓜表单有效");

            //this.Pub1.AddTD("控制该它在表单的显示方式");
            this.Pub1.AddTREnd();
            #endregion 是否可编辑

            #region 是否可界面可见
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否界面可见"); //是否界面可见
            this.Pub1.AddTDBegin();
            rb = new RadioButton();
            rb.ID = "RB_UIVisible_0";
            rb.Text = "不可见"; // 界面不可见
            rb.GroupName = "sa3";
            if (mapAttr.UIVisible)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIVisible_1";
            rb.Text = "界面可见"; // 界面可见;
            rb.GroupName = "sa3";

            if (mapAttr.UIVisible)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("不可见则为隐藏字段.");
            //this.Pub1.AddTD("控制该它在表单的界面里是否可见");
            this.Pub1.AddTREnd();
            #endregion 是否可界面可见

            this.EditBeforeEnd(mapAttr);
        }
        public void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                switch (btn.ID)
                {
                    case "Btn_Del":
                         MapAttr attrDel = new MapAttr();
                        attrDel.MyPK = this.RefNo;
                        attrDel.Delete();
                        this.WinClose();
                        return;
                    default:
                        break;
                }

                MapAttr attr = new MapAttr();
                if (this.RefNo != null)
                {
                    attr.MyPK = this.RefNo;
                    try
                    {
                        attr.Retrieve();
                    }
                    catch
                    {
                        attr.CheckPhysicsTable();
                        attr.Retrieve();
                    }

                    attr = (MapAttr)this.Pub1.Copy(attr);
                    attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                    attr.ColSpan = this.Pub1.GetDDLByID("DDL_ColSpan").SelectedItemIntVal;
                    if (attr.UIIsEnable == false && attr.MyDataType == DataType.AppString)
                    {
                        try
                        {
                            attr.IsSigan = this.Pub1.GetCBByID("CB_IsSigan").Checked;
                        }
                        catch
                        {
                        }
                    }

                    switch (this.FType)
                    {
                        case DataType.AppBoolean:
                            attr.MyDataType = BP.DA.DataType.AppBoolean;
                            attr.DefValOfBool = this.Pub1.GetCBByID("CB_DefVal").Checked;
                            break;
                        case DataType.AppDateTime:
                        case DataType.AppDate:
                            attr.DefValReal = this.Pub1.GetTBByID("TB_DefVal").Text;
                            //if (this.Pub1.GetCBByID("CB_DefVal").Checked)
                            //    attr.DefValReal = "1";
                            //else
                            //    attr.DefValReal = "0";
                            break;
                        case DataType.AppString:
                            attr.UIBindKey = this.Pub1.GetDDLByID("DDL_TBModel").SelectedItemStringVal;
                            if (attr.TBModel == 2)
                            {
                                attr.MaxLen = 4000;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    attr = (MapAttr)this.Pub1.Copy(attr);
                    attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                    attr.ColSpan = this.Pub1.GetDDLByID("DDL_ColSpan").SelectedItemIntVal;

                    MapAttrs attrS = new MapAttrs(this.MyPK);
                    int idx = 0;
                    foreach (MapAttr en in attrS)
                    {
                        idx++;
                        en.Idx = idx;
                        en.Update();
                        if (en.KeyOfEn == attr.KeyOfEn)
                            throw new Exception("字段已经存在 Key=" + attr.KeyOfEn);
                    }
                    if (this.IDX == null || this.IDX == "")
                        attr.Idx = 0;
                    else
                        attr.Idx = int.Parse(this.IDX) - 1;

                    attr.MyDataType = this.FType;
                    switch (this.FType)
                    {
                        case DataType.AppBoolean:
                            attr.MyDataType = BP.DA.DataType.AppBoolean;
                            attr.UIContralType = UIContralType.CheckBok;
                            attr.DefValOfBool = this.Pub1.GetCBByID("CB_DefVal").Checked;
                            break;
                        case DataType.AppString:
                            attr.UIBindKey = this.Pub1.GetDDLByID("DDL_TBModel").SelectedItemStringVal;
                            break;
                        default:
                            break;
                    }
                }

                // 增加是否为空, 对数字类型的字段有效.
                try
                {
                    attr.MinLen = this.Pub1.GetDDLByID("DDL_IsNull").SelectedItemIntVal;
                }
                catch
                {
                }

                //数字签名.
                try
                {
                    //签名类型.
                    attr.SignType = (SignType)this.Pub1.GetDDLByID("DDL_SignType").SelectedItemIntVal;

                    if (attr.SignType == SignType.Pic)
                        attr.PicType = (PicType)this.Pub1.GetDDLByID("DDL_PicType").SelectedItemIntVal;//是否为自动签名
                    else if (attr.SignType == SignType.CA)
                        attr.Para_SiganField = this.Pub1.GetTBByID("TB_SiganField").Text;//数字签名字段.

                }
                catch
                {

                }

                attr.Para_FontSize = this.Pub1.GetDDLByID("DDL_FontSize").SelectedItemIntVal;

                //保存数字签名.
                Response.Buffer = true;
                attr.FK_MapData = this.MyPK;
                attr.MyPK = this.RefNo;

                //执行一次update 处理mapdata的计算的业务逻辑.
                MapData md = new MapData();
                md.No = attr.FK_MapData;
                if (md.RetrieveFromDBSources() == 1)
                    md.Update();

                attr.Save();

                switch (btn.ID)
                {
                    case "Btn_SaveAndClose":
                        this.WinClose();
                        return;
                    case "Btn_SaveAndNew":
                        this.Response.Redirect("Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&GroupField=" + attr.GroupID, true);
                        return;
                    default:
                        break;
                }
                this.Response.Redirect("EditF.aspx?DoType=Edit&MyPK=" + this.MyPK + "&RefNo=" + attr.MyPK + "&FType=" + this.FType + "&GroupField=" + attr.GroupID, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        public string GetCaption
        {

            get
            {
                if (this.DoType == "Add")
                    return "增加新字段向导 - <a href='Do.aspx?DoType=ChoseFType&GroupField=" + this.GroupField + "' > 返回类型选择 </a> - " + "编辑";
                else
                    return "<a href='Do.aspx?DoType=ChoseFType&MyPK=" + this.MyPK + "&RefNo=" + this.RefNo + "&GroupField=" + this.GroupField + "'><img src='/WF/Img/Btn/Back.gif'>返回</a></a> - " + "编辑";
            }
        }
    }

}