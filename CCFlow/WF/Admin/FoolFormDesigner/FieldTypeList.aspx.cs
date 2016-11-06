using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.UC;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    public partial class FieldTypeList : BP.Web.WebPage
    {
        #region 属性.
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
                string str = this.Request.QueryString["FK_MapData"];
                if (str == null)
                    str = this.MyPK;
                return str;
            }
        }
        public string Idx
        {
            get
            {
                return this.Request.QueryString["Idx"];
            }
        }
        public int GroupField
        {
            get
            {
                string str = this.Request.QueryString["GroupID"];
                if (str == null)
                    str = this.Request.QueryString["GroupField"];
                if (str == null)
                    return 0;
                return int.Parse(str);
            }
        }
        
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void RB_PY_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string name = this.TB_Name.Text;
            if (string.IsNullOrEmpty(name) == true)
                return;

            if (rb.ID == "RB_PY_0")
                this.TB_No.Text = BP.DA.DataType.ParseStringToPinyin(name);
            else
                this.TB_No.Text = BP.DA.DataType.ParseStringToPinyinJianXie(name);
        }

        protected void TB_Name_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            string text = this.TB_Name.Text;
            if (string.IsNullOrEmpty(text) == true)
                return;

            if (this.RB_PY_0.Checked)
                this.TB_No.Text = BP.DA.DataType.ParseStringToPinyin(text);
            else
                this.TB_No.Text = BP.DA.DataType.ParseStringToPinyinJianXie(text);

        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            string no = this.TB_No.Text.Replace(" ","");
            if (string.IsNullOrEmpty(no) == true)
            {
                this.Alert("请输入字段名.");
                return;
            }

            MapAttrs attrs = new MapAttrs();
            int i = attrs.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, no);
            if (i != 0)
            {
                this.Alert("字段名：" + no + "已经存在.");
                return;
            }

            //求出选择的字段类型.
            MapAttr attr = new MapAttr();
            attr.Name = this.TB_Name.Text;
            attr.KeyOfEn = no;
            attr.FK_MapData = this.FK_MapData;
            attr.LGType = FieldTypeS.Normal;
            attr.MyPK = this.FK_MapData + "_" + no;
            attr.GroupID = this.GroupField;

            if (this.RB_String.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppString;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppString + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_Int.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppInt;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppInt + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_Money.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppMoney;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppMoney + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_Float.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppFloat;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppFloat + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_Double.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppDouble;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDouble + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_Data.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppDate;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDate + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_DataTime.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppDateTime;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppDateTime + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

            if (this.RB_Boolen.Checked)
            {
                attr.UIWidth = 100;
                attr.UIHeight = 23;
                attr.UIVisible = true;
                attr.UIIsEnable = true;
                attr.ColSpan = 0;
                attr.MinLen = 0;
                attr.MaxLen = 50;
                attr.MyDataType = DataType.AppBoolean;
                attr.Insert();
                this.Response.Redirect("EditF.aspx?MyPK=" + attr.MyPK + "&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + no + "&FType=" + DataType.AppBoolean + "&DoType=Edit&GroupField=" + this.GroupField, true);
            }

        }
    }
}