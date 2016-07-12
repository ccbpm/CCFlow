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
namespace CCFlow.WF.MapDef
{
    public partial class Comm_MapDef_EditSQL : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 调用来源
        /// </summary>
        public string FromApp
        {
            get
            {
                return this.Request.QueryString["FromApp"];
            }
        }
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
        public string FType
        {
            get
            {
                return this.Request.QueryString["FType"];
            }
        }
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "编辑外部表字段";
            MapAttr attr = null;
            if (this.RefNo == null)
            {
                attr = new MapAttr();
                string sfKey = this.Request.QueryString["SFKey"];
                SFTable sf = new SFTable(sfKey);
                attr.KeyOfEn = sf.FK_Val;
                attr.UIBindKey = sfKey;
                attr.Name = sf.Name;
                this.Title = "编辑外部表字段";
            }
            else
            {
                attr = new MapAttr(this.RefNo);
                this.Title = "修改外部表字段";
            }
            BindTable(attr);
        }

        int idx = 1;
        public void BindTable(MapAttr mapAttr)
        {
            bool isItem = false;
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft(this.Title);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("值");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段中文名"); // 字段中文名称
            TB tb = new TB();
            tb.ID = "TB_Name";
            tb.Text = mapAttr.Name;
            tb.Attributes["width"] = "100%";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段英文名"); // "字段英文名称"
            tb = new TB();
            if (this.RefNo != null)
            {
                this.Pub1.AddTD(mapAttr.KeyOfEn);
            }
            else
            {
                tb = new TB();
                tb.ID = "TB_KeyOfEn";
                tb.Text = mapAttr.KeyOfEn;
                this.Pub1.AddTD(tb);
            }

            if (string.IsNullOrEmpty(mapAttr.KeyOfEn))
                this.Pub1.AddTD("字母/数字/下划线组合");
            else
                this.Pub1.AddTD("<a href=\"javascript:clipboardData.setData('Text','" + mapAttr.KeyOfEn + "');alert('已经copy到粘帖版上');\" ><img src='../Img/Btn/Copy.gif' class='ICON' />复制字段名</a></TD>");

            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("外键表/类"); // 字段中文名称
            tb = new TB();
            tb.ID = "TB_UIBindKey";
            tb.Text = mapAttr.UIBindKey;
            tb.Attributes["width"] = "100%";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("默认值"); // "默认值"

            tb = new TB();
            tb.ID = "TB_DefVal";
            tb.Text = mapAttr.Name;
            tb.Attributes["width"] = "100%";
            tb.Text = mapAttr.DefValReal;
            this.Pub1.AddTD(tb);
            

            if (mapAttr.UIBindKey.Contains("."))
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('../Comm/Search.aspx?EnsName=" + mapAttr.UIBindKey + "','df');\" >打开</a>");
            else
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('../MapDef/SFTableEditData.aspx?RefNo=" + mapAttr.UIBindKey + "','df');\" >打开</a>");
            this.Pub1.AddTREnd();

            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否可编辑");
            this.Pub1.Add("<TD>");
            RadioButton rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_0";
            rb.Text = "不可编辑";  //"不可编辑";
            rb.GroupName = "s";
            if (mapAttr.UIIsEnable)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_1";
            rb.Text = "可编辑"; //"可编辑";
            rb.GroupName = "s";

            if (mapAttr.UIIsEnable)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

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
            this.Pub1.AddTREnd();
            #endregion 是否可界面可见

            #region 合并单元格数
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("合并单元格数");
            DDL ddl1 = new DDL();
            ddl1.ID = "DDL_ColSpan";
            //for (int i = 1; i < 12; i++)
            //{
            //    ddl1.Items.Add(new ListItem(i.ToString(), i.ToString()));
            //}

            ddl1.Items.Add(new ListItem("1", "1"));
            ddl1.Items.Add(new ListItem("3", "3"));
            ddl1.Items.Add(new ListItem("4", "4"));

            ddl1.SetSelectItem(mapAttr.ColSpan.ToString());
            this.Pub1.AddTD(ddl1);

            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();
            #endregion 是否可编辑

            #region 字段分组
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("字段分组");
            DDL ddlGroup = new DDL();
            ddlGroup.ID = "DDL_GroupID";
            GroupFields gfs = new GroupFields();
            gfs.RetrieveFieldGroup(mapAttr.FK_MapData);


            ddlGroup.Bind(gfs, GroupFieldAttr.OID, GroupFieldAttr.Lab);
            if (mapAttr.GroupID == 0)
                mapAttr.GroupID = this.GroupField;

            ddlGroup.SetSelectItem(mapAttr.GroupID);

            this.Pub1.AddTD("colspan=2", ddlGroup);
            this.Pub1.AddTD();  //( this.to "隶属分组");
            this.Pub1.AddTREnd();
            #endregion 字段分组

            #region 扩展功能.
            if (this.RefNo != null)
            {
                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/AutoFullDLL.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "')\">设置列表过滤</a>");
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/ActiveDDL.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn +  "&MyPK="+MapExtXmlList.ActiveDDL+"_"+mapAttr.MyPK+"')\">设置级联动(如:省份,城市联动)</a>");
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/DDLFullCtrl.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "&MyPK="+MapExtXmlList.DDLFullCtrl+"_"+mapAttr.MyPK+"')\">设置自动填充</a>");
                this.Pub1.AddTREnd();
            }
            #endregion 扩展功能.

            #region 字段按钮
            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD colspan=4>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Save_Click);
            btn.CssClass = "Btn";
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose1";
            btn.CssClass = "Btn";
            btn.Text = "关闭";
            btn.Attributes["onclick"] = " window.close(); return false;";
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = "保存并关闭"; //"保存并关闭";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndNew";
            btn.CssClass = "Btn";
            btn.Text = "保存并新建";
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

                if (mapAttr.FK_MapData.Contains("ND") == true)
                {
                    string myUrl = "EleBatch.aspx?KeyOfEn=" + mapAttr.KeyOfEn + "&FK_MapData=" + mapAttr.FK_MapData + "&EleType=MapAttr";
                    this.Pub1.Add("<a href='" + myUrl + "' target='M" + mapAttr.KeyOfEn + "' ><img src='../Img/Btn/Apply.gif' border=0>批处理</a>");
                }
            }

            string url = "Do.aspx?DoType=AddF&MyPK=" + mapAttr.FK_MapData + "&IDX=" + mapAttr.Idx;
            this.Pub1.Add("<a href='" + url + "' ><img src='../Img/Btn/New.gif' border=0>新建</a></TD>");

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            #endregion 字段按钮
        }
        void btn_Save_Click(object sender, EventArgs e)
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
                if (this.RefNo == null || this.RefNo == "")
                {
                    attr.MyPK = this.MyPK + "_" + this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    attr.KeyOfEn =  this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    attr.UIContralType = UIContralType.DDL;
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.LGType = FieldTypeS.Normal;
                    attr.DefVal = "";
                    attr.UIBindKey = this.Request.QueryString["SFKey"];
                    attr.UIIsEnable = true;
                    if (attr.IsExits == true)
                        throw new Exception("@字段名["+attr.KeyOfEn+"]已经存在，保存失败。");

                    attr = (MapAttr)this.Pub1.Copy(attr);
                }
                else
                {
                    attr.MyPK = this.RefNo;
                    attr.Retrieve();
                    attr = (MapAttr)this.Pub1.Copy(attr);
                }

                attr.FK_MapData = this.MyPK;
                attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                attr.ColSpan = this.Pub1.GetDDLByID("DDL_ColSpan").SelectedItemIntVal;
                attr.DefVal = this.Pub1.GetTBByID("TB_DefVal").Text;
                attr.UIBindKey = this.Pub1.GetTBByID("TB_UIBindKey").Text;

                string field = attr.KeyOfEn;
                if (this.RefNo == null || this.RefNo == "")
                {
                    attr.Insert(); //首先插入数据表现数据.

                    //插入隐藏数据.
                    attr.KeyOfEn = field + "T";
                    attr.UIVisible = false;
                    attr.UIIsEnable = false; //让其不是隐藏字段.
                    attr.Insert();

                    //还原以前的值.
                    attr.KeyOfEn = field;
                    attr.MyPK = this.MyPK + "_" + field;
                    attr.UIVisible = true;
                }
                else
                    attr.Update();

                switch (btn.ID)
                {
                    case "Btn_SaveAndClose":
                        this.WinClose();
                        return;
                    case "Btn_SaveAndNew":
                        this.Response.Redirect("Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + attr.Idx + "&GroupField=" + this.GroupField, true);
                        return;
                    default:
                        break;
                }
                this.Response.Redirect("EditSQL.aspx?DoType=Edit&MyPK=" + this.MyPK + "&RefNo=" + attr.MyPK + "&GroupField=" + this.GroupField, true);
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
                    return "增加新字段向导  - <a href='Do.aspx?DoType=ChoseFType&GroupField=" + this.GroupField + "'>选择类型</a> -" + "编辑字段";
                else
                    return "编辑字段"; // "编辑字段";
            }
        }
    }

}