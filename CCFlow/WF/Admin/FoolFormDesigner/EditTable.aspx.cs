﻿using System;
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
    public partial class Comm_MapDef_EditTable : BP.Web.WebPage
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
        /// 调用来源
        /// </summary>
        public string FromApp
        {
            get
            {
                return this.Request.QueryString["FromApp"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string FK_SFTable
        {
            get
            {
                return this.Request.QueryString["FK_SFTable"];
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
            this.Title = "编辑外键类型字段";
            MapAttr attr = null;
            if (this.MyPK == null)
            {
                attr = new MapAttr();
                SFTable sf = new SFTable(this.FK_SFTable);
                attr.KeyOfEn = sf.FK_Val;
                attr.FK_MapData = this.FK_MapData;
                attr.UIBindKey = this.FK_SFTable;
                attr.Name = sf.Name;
                this.Title = "编辑外键类型字段";
            }
            else
            {
                attr = new MapAttr(this.MyPK);
                this.Title = "修改外键类型字段";
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
            if (this.MyPK == null)
            {
                tb = new TB();
                tb.ID = "TB_KeyOfEn";
                tb.Text = mapAttr.KeyOfEn;
                this.Pub1.AddTD(tb);
            }
            else
            {
                this.Pub1.AddTD(mapAttr.KeyOfEn);
            }

            if (string.IsNullOrEmpty(mapAttr.KeyOfEn))
                this.Pub1.AddTD("字母/数字/下划线组合");
            else
                this.Pub1.AddTD("<a href=\"javascript:clipboardData.setData('Text','" + mapAttr.KeyOfEn + "');alert('已经copy到粘帖版上');\" ><img src='../../Img/Btn/Copy.gif' class='ICON' />复制字段名</a></TD>");

            this.Pub1.AddTREnd();


            isItem = this.Pub1.AddTR(isItem);
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("外键表/类"); // 字段中文名称
            this.Pub1.AddTD(mapAttr.UIBindKey);
            this.Pub1.AddTD("不可更改.");
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
            {
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('../../Comm/Search.htm?EnsName=" + mapAttr.UIBindKey + "','df');\" >数据维护</a>");
            }
            else
            {
                SFTable sf = new SFTable(mapAttr.UIBindKey);
                if (sf.SrcType == SrcType.TableOrView)
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('SFTableEditData.htm?FK_SFTable=" + mapAttr.UIBindKey + "','df');\" >数据维护</a>");
                else
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('SFSQLDataView.aspx?FK_SFTable=" + mapAttr.UIBindKey + "','df');\" >查看</a>");
            }
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
            ddl1.Items.Add(new ListItem("1", "1"));
            ddl1.Items.Add(new ListItem("3", "3"));
            //ddl1.Items.Add(new ListItem("4", "4"));

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
            if (this.MyPK != null)
            {
                isItem = this.Pub1.AddTR(isItem);
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/AutoFullDLL.htm?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "')\">设置列表过滤</a>");
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/ActiveDDL.htm?FK_MapData=" + mapAttr.FK_MapData + "&AttrOfOper=" + mapAttr.KeyOfEn + "&MyPK=" + MapExtXmlList.ActiveDDL + "_" + mapAttr.MyPK + "')\">设置级联动(如:省份,城市联动)</a>");
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('./MapExt/DDLFullCtrl.htm?FK_MapData=" + mapAttr.FK_MapData + "&RefNo=" + mapAttr.KeyOfEn + "&MyPK=" + MapExtXmlList.DDLFullCtrl + "_" + mapAttr.MyPK + "')\">设置自动填充</a>");
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

            if (this.MyPK != null)
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
                    this.Pub1.Add("<a href='" + myUrl + "' target='M" + mapAttr.KeyOfEn + "' ><img src='../../Img/Btn/Apply.gif' border=0>批处理</a>");
                }
            }

            string url = "FieldTypeList.htm?DoType=AddF&FK_MapData=" + mapAttr.FK_MapData + "&IDX=" + mapAttr.Idx + "&GroupField=" + this.GroupField;
            this.Pub1.Add("<a href='" + url + "' ><img src='../../Img/Btn/New.gif' border=0>新建</a></TD>");

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
                        attrDel.MyPK = this.MyPK;
                        attrDel.Delete();
                        this.WinClose();
                        return;
                    default:
                        break;
                }

                MapAttr attr = new MapAttr();
                attr = (MapAttr)this.Pub1.Copy(attr);
                attr.UIBindKey = this.FK_SFTable;
                attr.UIContralType = UIContralType.DDL;
                attr.LGType = FieldTypeS.FK;

                /*普通类型的外部字典字段.*/
                if (this.MyPK == null || this.MyPK == "")
                {
                    attr.MyPK = this.FK_MapData + "_" + this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    attr.KeyOfEn = this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                    if (attr.IsExits == true)
                    {
                        this.Alert("@字段名[" + attr.KeyOfEn + "]，已经存在。");
                        return;
                    }
                    attr.DefVal = "";
                    attr.UIIsEnable = true;
                }
                else
                {
                    attr.MyPK = this.MyPK;
                    attr.Retrieve();
                    attr.UIContralType = UIContralType.DDL;
                    attr.LGType = FieldTypeS.FK;
                }


                if (attr.UIBindKey.Contains(".") == false)
                {

                    attr = (MapAttr)this.Pub1.Copy(attr);
                    attr.FK_MapData = this.FK_MapData;
                    attr.LGType = FieldTypeS.Normal;

                    //控件类型为 DDL, 逻辑类型为Normal
                    attr.UIContralType = UIContralType.DDL;
                    attr.MyDataType = BP.DA.DataType.AppString;

                    attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                    attr.ColSpan = this.Pub1.GetDDLByID("DDL_ColSpan").SelectedItemIntVal;
                    attr.DefVal = this.Pub1.GetTBByID("TB_DefVal").Text;

                    if (string.IsNullOrEmpty(this.FK_SFTable) == false)
                        attr.UIBindKey = this.FK_SFTable;

                    attr.UIContralType = UIContralType.DDL;
                    attr.LGType = FieldTypeS.FK;

                    if (this.MyPK == null || this.MyPK == "")
                        attr.Insert();
                    else
                        attr.Update();

                    #region 为它增加隐藏字段.
                    MapAttr attrH = new MapAttr();
                    attrH.Copy(attr);
                    attrH.KeyOfEn = attr.KeyOfEn + "T";
                    attrH.Name = attr.Name;
                    attrH.UIContralType = UIContralType.TB;
                    attrH.MinLen = 0;
                    attrH.MaxLen = 60;
                    attrH.MyDataType = BP.DA.DataType.AppString;
                    attrH.UIVisible = false;
                    attrH.UIIsEnable = false;
                    attrH.MyPK = attrH.FK_MapData + "_" + attrH.KeyOfEn;
                    attrH.LGType = FieldTypeS.Normal;
                    attrH.HisEditType = EditType.UnDel;
                    attrH.Save();
                    #endregion
                }
                else
                {
                    /*普通类型的外部字典字段.*/
                    if (this.MyPK == null || this.MyPK == "")
                    {
                        attr.MyPK = this.FK_MapData + "_" + this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                        attr.KeyOfEn = this.Pub1.GetTBByID("TB_KeyOfEn").Text;
                        if (attr.IsExits == true)
                        {
                            this.Alert("@字段名[" + attr.KeyOfEn + "]，已经存在。");
                            return;
                        }
                        attr.DefVal = "";
                        attr.UIIsEnable = true;
                    }
                    else
                    {
                        attr.MyPK = this.MyPK;
                        attr.Retrieve();
                    }

                    attr = (MapAttr)this.Pub1.Copy(attr);
                    attr.FK_MapData = this.FK_MapData;

                    attr.UIContralType = UIContralType.DDL;
                    attr.LGType = FieldTypeS.FK;

                    attr.GroupID = this.Pub1.GetDDLByID("DDL_GroupID").SelectedItemIntVal;
                    attr.ColSpan = this.Pub1.GetDDLByID("DDL_ColSpan").SelectedItemIntVal;
                    attr.DefVal = this.Pub1.GetTBByID("TB_DefVal").Text;
                    attr.MyDataType = BP.DA.DataType.AppString;
                    attr.UIContralType = UIContralType.DDL;
                    attr.Save();
                }

                switch (btn.ID)
                {
                    case "Btn_SaveAndClose":
                        this.WinClose();
                        return;
                    case "Btn_SaveAndNew":
                        this.Response.Redirect("FieldTypeList.htm?DoType=AddF&FK_MapData=" + this.FK_MapData + "&IDX=" + attr.Idx + "&GroupField=" + this.GroupField + "&FK_SFTable=" + this.FK_SFTable, true);
                        return;
                    default:
                        break;
                }
                this.Response.Redirect("EditTable.aspx?DoType=Edit&FK_MapData=" + this.FK_MapData + "&MyPK=" + attr.MyPK + "&GroupField=" + this.GroupField + "&FK_SFTable=" + this.FK_SFTable, true);
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
                    return "增加新字段向导  - <a href='FieldTypeList.htm?DoType=ChoseFType&GroupField=" + this.GroupField + "&FK_MapData=" + this.FK_MapData + "'>选择类型</a> -" + "编辑字段";
                else
                    return "编辑字段"; // "编辑字段";
            }
        }
    }

}