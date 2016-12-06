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
    public partial class Comm_MapDef_EditEnum : BP.Web.WebPage
    {
        #region 属性.
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
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string EnumKey
        {
            get
            {
                return this.Request.QueryString["EnumKey"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "编辑枚举类型"; 
            MapAttr attr = new MapAttr();
            if (this.MyPK == null)
            {
                if (this.EnumKey != null)
                {
                    SysEnumMain se = new SysEnumMain(this.EnumKey);
                    attr.KeyOfEn = this.EnumKey;
                    attr.UIBindKey = this.EnumKey;
                    attr.Name = se.Name;
                }
                attr.UIContralType = UIContralType.DDL;
            }
            else
            {
                attr = new MapAttr(this.MyPK);
            }
            attr.FK_MapData = this.FK_MapData;

            BindEnum(attr);
        }
        int idx = 1;
        public void BindEnum(MapAttr mapAttr)
        {
            this.Pub1.AddTable(" width='450px'");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("ID");
            this.Pub1.AddTDTitle("项目");
            this.Pub1.AddTDTitle("采集");
            this.Pub1.AddTDTitle("说明");
            this.Pub1.AddTREnd();

            bool isItem = false;
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
            if (this.MyPK != null)
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
                this.Pub1.AddTD("<a href=\"javascript:clipboardData.setData('Text','" + mapAttr.KeyOfEn + "');alert('已经copy到粘帖版上');\" ><img src='../../Img/Btn/Copy.gif' class='ICON' />复制字段名</a></TD>");
            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("默认值");

            DDL ddl = new DDL();
            ddl.ID = "DDL";
            ddl.BindSysEnum(mapAttr.UIBindKey);
            ddl.SetSelectItem(mapAttr.DefVal);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("<a href='SysEnum.aspx?EnumKey=" + mapAttr.UIBindKey + "'>编辑</a>");
            this.Pub1.AddTREnd();


            #region 是否可编辑
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("是否可编辑");
            this.Pub1.AddTDBegin();
            RadioButton rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_0";
            rb.Text = "不可编辑";
            rb.GroupName = "s";
            if (mapAttr.UIIsEnable)
                rb.Checked = false;
            else
                rb.Checked = true;

            this.Pub1.Add(rb);
            rb = new RadioButton();
            rb.ID = "RB_UIIsEnable_1";
            rb.Text = "可编辑";
            rb.GroupName = "s";

            if (mapAttr.UIIsEnable)
                rb.Checked = true;
            else
                rb.Checked = false;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            #endregion 是否可编辑


            #region 展示控件
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("控件类型");
            this.Pub1.AddTDBegin();

            rb = new RadioButton();
            rb.ID = "RB_Ctrl_0";
            rb.Text = "下拉框";
            rb.GroupName = "Ctrl";
            if (mapAttr.UIContralType == UIContralType.DDL)
                rb.Checked = true;
            this.Pub1.Add(rb);

            rb = new RadioButton();
            rb.ID = "RB_Ctrl_1";
            rb.Text = "单选按钮";
            rb.GroupName = "Ctrl";
            if (mapAttr.UIContralType == UIContralType.RadioBtn)
                rb.Checked = true;

            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();

            ddl = new DDL();
            ddl.ID = "DDL_RBShowModel";
            ddl.Items.Add(new ListItem("横向展示", "0"));
            ddl.Items.Add(new ListItem("竖向展示", "1"));
            ddl.SetSelectItem(mapAttr.RBShowModel);
            this.Pub1.AddTD(ddl);

            this.Pub1.AddTREnd();
            #endregion 展示控件

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

            #region 是否可界面可见
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD(""); //是否启用高级设置
            this.Pub1.AddTDBegin();

            CheckBox cb = new CheckBox();
            cb.ID = "CB_IsEnableJS";
            cb.Text = "是否启用高级JS设置";
            cb.Checked = mapAttr.IsEnableJS; //是否启用高级JS设置？
            this.Pub1.Add(cb);

            this.Pub1.AddTDEnd();

            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();
            #endregion 是否可界面可见

            #region 合并单元格数
            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("合并单元格数");
            ddl = new DDL();
            ddl.ID = "DDL_ColSpan";

            ddl.Items.Add(new ListItem("1", "1"));
            ddl.Items.Add(new ListItem("3", "3"));
            //ddl.Items.Add(new ListItem("4", "4"));
            ddl.SetSelectItem(mapAttr.ColSpan.ToString());
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("对傻瓜表单有效");
            this.Pub1.AddTREnd();
            #endregion 合并单元格数

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
            this.Pub1.AddTD(ddlGroup);
            this.Pub1.AddTD("修改隶属分组");
            this.Pub1.AddTREnd();
            #endregion 字段分组

            #region 扩展设置.
            if (this.MyPK != null)
            {
                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                string html = "<a href=\"javascript:WinOpen('./MapExt/DDLFullCtrl.aspx?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "&MyPK=" + mapAttr.FK_MapData + "_" + MapExtXmlList.DDLFullCtrl + "_" + mapAttr.KeyOfEn + "')\">下拉框自动完成</a>";
                html += " - <a href=\"javascript:WinOpen('./MapExt/ActiveDDL.aspx?FK_MapData=" + mapAttr.FK_MapData + "&AttrOfOper=" + mapAttr.KeyOfEn + "&MyPK=" + MapExtXmlList.ActiveDDL + "_" + mapAttr.MyPK + "')\">设置级联动(如:省份,城市联动)</a>";
                html += " - <a href=\"javascript:WinOpen('./MapExt/RadioBtns.htm?FK_MapData=" + mapAttr.FK_MapData + "&KeyOfEn=" + mapAttr.KeyOfEn + "&MyPK=" + mapAttr.FK_MapData + "_" + MapExtXmlList.DDLFullCtrl + "_" + mapAttr.KeyOfEn + "')\">高级JS设置</a>";
                this.Pub1.AddTD("colspan=3", html);
                this.Pub1.AddTREnd();
            }
            #endregion 扩展设置.

            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD colspan=4 >");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = " 保存 ";
            btn.Click += new EventHandler(Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndClose";
            btn.CssClass = "Btn";
            btn.Text = "保存并关闭";
            btn.Click += new EventHandler(Save_Click);
            this.Pub1.Add(btn);

            btn = new Button();
            btn.ID = "Btn_SaveAndNew";
            btn.CssClass = "Btn";
            btn.Text = "保存并新建";
            btn.Click += new EventHandler(Save_Click);
            this.Pub1.Add(btn);
            if (this.MyPK != null)
            {
                //btn = new Button();
                //btn.ID = "Btn_AutoFull";
                //btn.CssClass = "Btn";
                //btn.Text = "扩展设置";
                ////  btn.Click += new EventHandler(btn_Save_Click);
                //btn.Attributes["onclick"] = "javascript:WinOpen('./MapExt/AutoFull.aspx?RefNo=" + this.RefNo + "&FK_MapData=" + mapAttr.FK_MapData + "',''); return false;";
                //this.Pub1.Add(btn);

                if (mapAttr.HisEditType == EditType.Edit)
                {
                    btn = new Button();
                    btn.ID = "Btn_Del";
                    btn.CssClass = "Btn";
                    btn.Text = "删除";
                    btn.Click += new EventHandler(Save_Click);
                    btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                    this.Pub1.Add(btn);
                }

                string myUrl = "EleBatch.aspx?KeyOfEn=" + mapAttr.KeyOfEn + "&FK_MapData=" + mapAttr.FK_MapData + "&EleType=MapAttr";
                this.Pub1.Add("<a href='" + myUrl + "' target='M" + mapAttr.KeyOfEn + "' ><img src='../../Img/Btn/Apply.gif' border=0>批处理</a>");
            }

            string url = "FieldTypeList.aspx?DoType=AddF&FK_MapData=" + mapAttr.FK_MapData + "&IDX=" + mapAttr.Idx;
            this.Pub1.Add("<a href='" + url + "'><img src='../../Img/Btn/New.gif' border=0>新建</a></TD>");

            this.Pub1.AddTREnd();
            this.Pub1.AddTableEndWithBR();
        }
        void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                switch (btn.ID)
                {
                    case "Btn_Del":
                        MapAttr attrDel = new MapAttr();
                        attrDel.MyPK = this.MyPK;
                        attrDel.Delete();
                        this.WinClose();
                        return;
                    default:
                        break;
                }

                MapAttr attr = new MapAttr();
                attr.MyPK = this.MyPK;
                if (this.MyPK != null)
                    attr.Retrieve();

                attr = (MapAttr)this.Pub1.Copy(attr);
                attr.FK_MapData = this.FK_MapData;
                attr.DefVal = this.Pub1.GetDDLByID("DDL").SelectedItemStringVal;
                attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                attr.ColSpan = this.Pub1.GetDDLByID("DDL_ColSpan").SelectedItemIntVal;
                
                //是否启用高级JS设置.
                attr.IsEnableJS = this.Pub1.GetCBByID("CB_IsEnableJS").Checked;


                //单选按钮的展现方式.
                attr.RBShowModel = this.Pub1.GetDDLByID("DDL_RBShowModel").SelectedItemIntVal;

                if (this.Pub1.GetRadioButtonByID("RB_Ctrl_0").Checked)
                    attr.UIContralType = UIContralType.DDL;
                else
                    attr.UIContralType = UIContralType.RadioBtn;

                attr.MyDataType = BP.DA.DataType.AppInt;
                attr.HisEditType = EditType.Edit;
                attr.LGType = FieldTypeS.Enum;

                if (this.MyPK == null)
                {
                    attr.UIBindKey = this.EnumKey;
                    attr.MyPK = this.FK_MapData + "_" + this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    attr.KeyOfEn =  this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    if (attr.IsExits == true)
                    {
                        this.Alert("@字段["+attr.KeyOfEn+"]已经存在.");
                        return;
                    }
                    attr.Insert();
                }
                else
                {
                    attr.Update();
                }

                switch (btn.ID)
                {
                    case "Btn_SaveAndClose":
                        this.WinClose();
                        return;
                    case "Btn_SaveAndNew":
                        this.Response.Redirect("FieldTypeList.aspx?FK_MapData=" + this.FK_MapData + "&IDX=" + attr.Idx + "&GroupField=" + this.GroupField + "&EnumKey="+this.EnumKey, true);
                        return;
                    default:
                        break;
                }
                if (this.MyPK == null)
                    this.Response.Redirect("EditEnum.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&MyPK=" + attr.MyPK + "&GroupField=" + this.GroupField + "&EnumKey=" + this.EnumKey, true);
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
                    return "增加新字段向导 - <a href='Do.aspx?DoType=ChoseFType&GroupField=" + this.GroupField + "&FK_MapData="+this.FK_MapData+"'>选择类型</a>";
                else
                    return " <a href='FieldTypeList.aspx?DoType=ChoseFType&FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + "&GroupField=" + this.GroupField + "'>编辑</a>";
            }
        }
    }

}