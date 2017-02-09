using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Sys;
using BP.En;
using BP.Web.Controls;
using BP.DA;
using BP.Web;
using BP.Tools;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_GroupField : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 分组
        /// </summary>
        public int GroupField
        {
            get
            {
                string str= this.Request.QueryString["GroupField"];
                if (str == "" || str == null)
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 相关编号
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string s = this.Request.QueryString["FK_MapData"];
                if (s == null)
                {
                    s = this.Request.QueryString["RefNo"];
                    if (s == null)
                        return "t"; 
                    return s;
                }
                return s;
            }
        }
        #endregion 属性.

       public void Btn_Save_Check_Click(object sender, EventArgs e)
        {
            string sta =  this.TB_Check_Name.Text;
            if (sta.Length == 0)
            {
                this.Alert("审核岗位不能为空");
                return;
            }

            string prx = this.TB_Check_Pix.Text;
            if (prx.Length == 0)
            {
                prx = chs2py.convert(sta);
            }

            MapAttr attr = new MapAttr();
            int i = attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_Note");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_Checker");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.FK_MapData, MapAttrAttr.KeyOfEn, prx + "_RDT");

            if (i > 0)
            {
                this.Alert("前缀已经使用：" + prx + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。");
                return;
            }

            GroupField gf = new GroupField();
            gf.Lab = sta;
            gf.EnName = this.FK_MapData;
            gf.Insert();

            attr = new MapAttr();
            attr.FK_MapData = this.FK_MapData;
            attr.KeyOfEn = prx + "_Note";
            attr.Name = "审核意见"; // sta;  // this.ToE("CheckNote", "审核意见");
            attr.MyDataType = DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.UIIsEnable = true;
            attr.UIIsLine = true;
            attr.MaxLen = 4000;
            attr.ColSpan = 4;
            attr.GroupID = gf.OID;
            attr.UIHeight = 23 * 3;
            attr.Idx = 1;
            attr.Insert();
            attr.Update("Idx", 1);


            attr = new MapAttr();
            attr.FK_MapData = this.FK_MapData;
            attr.KeyOfEn = prx + "_Checker";
            attr.Name = "审核人";// "审核人";
            attr.MyDataType = DataType.AppString;
            attr.UIContralType = UIContralType.TB;
            attr.MaxLen = 50;
            attr.MinLen = 0;
            attr.UIIsEnable = true;
            attr.UIIsLine = false;
            attr.DefVal = "@WebUser.No";
            attr.UIIsEnable = false;
            attr.GroupID = gf.OID;
            attr.IsSigan = true;
            attr.Idx = 2;
            attr.Insert();
            attr.Update("Idx", 2);

            attr = new MapAttr();
            attr.FK_MapData = this.FK_MapData;
            attr.KeyOfEn = prx + "_RDT";
            attr.Name = "审核日期"; // "审核日期";
            attr.MyDataType = DataType.AppDateTime;
            attr.UIContralType = UIContralType.TB;
            attr.UIIsEnable = true;
            attr.UIIsLine = false;
            attr.DefVal = "@RDT";
            attr.UIIsEnable = false;
            attr.GroupID = gf.OID;
            attr.Idx = 3;
            attr.Insert();
            attr.Update("Idx", 3);


            this.WinCloseWithMsg("保存成功"); // "增加成功，您可以调整它的位置与修改字段的标签。"
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "字段分组";

            switch (this.DoType)
            {
                case "FunList":
                    //this.Pub1.AddFieldSet("字段分组向导");
                    //this.Pub1.AddUL();
                    //this.Pub1.AddLi("<b><a href='GroupField.aspx?DoType=NewGroup&FK_MapData=" + this.FK_MapData + "'>新建空白字段分组</a></b><br><font color=green>空白字段分组，建立后可以把相关的字段放入此分组里。</font>");
                    //this.Pub1.AddLi("<b><a href='GroupField.aspx?DoType=NewCheckGroup&FK_MapData=" + this.FK_MapData + "'>新建审核分组</a></b><br><font color=green>系统会让您输入审核的信息，并创建审核分组。</font>");
                    //this.Pub1.AddLi("<b><a href='GroupField.aspx?DoType=NewEvalGroup&FK_MapData=" + this.FK_MapData+"'>创建工作质量考核字段分组</a></b><br><font color=green>创建质量考核: EvalEmpNo,EvalEmpName,EvalCent,EvalNote 4个必要的字段。</font>");
                    //this.Pub1.AddULEnd();
                    //this.Pub1.AddFieldSetEnd();
                    return;
                case "NewEvalGroup":

                    GroupField gf = new GroupField();
                    gf.Lab = "工作质量考核";
                    gf.EnName = this.FK_MapData;
                    gf.Insert();

                    MapAttr attr = new MapAttr();
                    attr.FK_MapData = this.FK_MapData;
                    attr.KeyOfEn = BP.WF.WorkSysFieldAttr.EvalNote; 
                    attr.Name = "审核意见"; 
                    attr.MyDataType = DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = true;
                    attr.MaxLen = 500;
                    attr.GroupID = gf.OID;
                    attr.UIHeight = 23 ;
                    attr.ColSpan = 4;
                    attr.Idx = 1;
                    attr.Insert();

                    attr = new MapAttr();
                    attr.FK_MapData = this.FK_MapData;
                    attr.KeyOfEn = BP.WF.WorkSysFieldAttr.EvalCent; 
                    attr.Name = "分值"; 
                    attr.MyDataType = DataType.AppFloat;
                    attr.UIContralType = UIContralType.TB;
                    attr.MaxLen = 50;
                    attr.MinLen = 0;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = false;
                    attr.DefVal = "0";
                    attr.UIIsEnable = false;
                    attr.GroupID = gf.OID;
                    attr.IsSigan = true;
                    attr.Idx = 2;
                    attr.Insert();

                    attr = new MapAttr();
                    attr.FK_MapData = this.FK_MapData;
                    attr.KeyOfEn = BP.WF.WorkSysFieldAttr.EvalEmpNo; 
                    attr.Name = "被评估人编号"; 
                    attr.MyDataType = DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = false;
                    attr.DefVal = "";
                    attr.UIIsEnable = false;
                    attr.GroupID = gf.OID;
                    attr.Idx = 3;
                    attr.Insert();

                     attr = new MapAttr();
                    attr.FK_MapData = this.FK_MapData;
                    attr.KeyOfEn = BP.WF.WorkSysFieldAttr.EvalEmpName; 
                    attr.Name = "被评估人名称";  
                    attr.MyDataType = DataType.AppString;
                    attr.UIContralType = UIContralType.TB;
                    attr.UIIsEnable = true;
                    attr.UIIsLine = false;
                    attr.DefVal = "";
                    attr.UIIsEnable = false;
                    attr.GroupID = gf.OID;
                    attr.Idx = 4;
                    attr.Insert();
                    this.WinCloseWithMsg("保存成功"); // "增加成功，您可以调整它的位置与修改字段的标签。"
                    return;
                default:
                    break;
            }
        }

        public void btn_SaveBlank_Click(object sender, EventArgs e)
        {
            GroupField en = new GroupField();
            en.EnName = this.FK_MapData;

            if (this.GroupField == 0)
            {
                en.Lab = this.TB_Blank_Name.Text;
                en.Insert();
            }
            else
            {
                en.OID = this.GroupField;
                en.RetrieveFromDBSources();
                en.Lab = this.TB_Blank_Name.Text;
                en.Update();
            }

            this.Response.Redirect("GroupField.aspx?FK_MapData=" + this.FK_MapData + "&GroupField=" + en.OID, true);


            //Btn btn = sender as Btn;
            //switch (btn.ID)
            //{
            //    case "Btn_SaveAndClose":
            //        this.WinClose();
            //        break;
            //    case "Btn_NewField":
            //        this.Session["GroupField"] = en.OID;
            //        this.Response.Redirect("FieldTypeList.aspx?DoType=AddF&FK_MapData=" + this.FK_MapData + "&GroupField=" + en.OID, true);
            //        break;
            //    case "Btn_CopyField":
            //        this.Response.Redirect("CopyFieldFromNode.aspx?FK_Node=" + this.FK_MapData + "&GroupField=" + en.OID, true);
            //        break;
            //    default:
            //        this.Response.Redirect("GroupField.aspx?FK_MapData=" + this.FK_MapData + "&GroupField=" + en.OID, true);
            //        break;
            //}
        }

        protected void TB_Check_Name_TextChanged(object sender, EventArgs e)
        {
            string name = this.TB_Check_Name.Text;
            if (string.IsNullOrEmpty(name) == true)
                return;

            this.TB_Check_Pix.Text = BP.DA.DataType.ParseStringToPinyinJianXie(name);
        }

        protected void Btn_Edit_Save_Click(object sender, EventArgs e)
        {
            GroupField gf = new GroupField(this.GroupField);
            gf.Lab = this.TB_Edit_Name.Text;
            if (gf.Lab.Length == 0)
            {
                this.Alert("不能为空");
                return;
            }
            gf.Update();
        }

        protected void Btn_Edit_SaveAndClose_Click(object sender, EventArgs e)
        {
            Btn_Edit_Save_Click(null, null);
            this.WinClose();
        }

        protected void Btn_Edit_Del_Click(object sender, EventArgs e)
        {
            GroupField gf = new GroupField();
            gf.OID = this.GroupField;
            gf.Delete();

            BP.WF.Template.MapFrmFool md = new BP.WF.Template.MapFrmFool(this.FK_MapData);
            md.DoCheckFixFrmForUpdateVer();
            this.WinClose(); 
        }

        protected void Btn_Edit_DelAll_Click(object sender, EventArgs e)
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.GroupID, this.GroupField);
            foreach (MapAttr attr in attrs)
            {
                if (attr.HisEditType != EditType.Edit)
                    continue;
                if (attr.KeyOfEn == "FID")
                    continue;
                attr.Delete();
            }

            GroupField gf = new GroupField();
            gf.OID = this.GroupField;
            gf.Delete();

            this.WinClose();// ("删除成功。");
        }

        
    }

}