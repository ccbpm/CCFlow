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
        public string FromGroupField
        {
            get
            {
                return this.Request.QueryString["FromGroupField"];
            }
        }
        /// <summary>
        /// 相关编号
        /// </summary>
        public new string RefNo
        {
            get
            {
                string s = this.Request.QueryString["RefNo"];
                if (s == null)
                    return "t";
                else
                    return s;
            }
        }
        #endregion 属性.

        void btn_Check_Click(object sender, EventArgs e)
        {
            string sta = this.Pub1.GetTBByID("TB_Sta").Text.Trim();
            if (sta.Length == 0)
            {
                this.Alert("审核岗位不能为空");
                return;
            }

            string prx = this.Pub1.GetTBByID("TB_Prx").Text.Trim();
            if (prx.Length == 0)
            {
                prx = chs2py.convert(sta);
            }

            MapAttr attr = new MapAttr();
            int i = attr.Retrieve(MapAttrAttr.FK_MapData, this.RefNo, MapAttrAttr.KeyOfEn, prx + "_Note");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.RefNo, MapAttrAttr.KeyOfEn, prx + "_Checker");
            i += attr.Retrieve(MapAttrAttr.FK_MapData, this.RefNo, MapAttrAttr.KeyOfEn, prx + "_RDT");

            if (i > 0)
            {
                this.Alert("前缀已经使用：" + prx + " ， 请确认您是否增加了这个审核分组或者，请您更换其他的前缀。");
                return;
            }

            GroupField gf = new GroupField();
            gf.Lab = sta;
            gf.EnName = this.RefNo;
            gf.Insert();

            attr = new MapAttr();
            attr.FK_MapData = this.RefNo;
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
            attr.FK_MapData = this.RefNo;
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
            attr.FK_MapData = this.RefNo;
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
                    this.Pub1.AddFieldSet("字段分组向导");
                    this.Pub1.AddUL();
                    this.Pub1.AddLi("<b><a href='GroupField.aspx?DoType=NewGroup&RefNo=" + this.RefNo + "'>新建空白字段分组</a></b><br><font color=green>空白字段分组，建立后可以把相关的字段放入此分组里。</font>");
                    this.Pub1.AddLi("<b><a href='GroupField.aspx?DoType=NewCheckGroup&RefNo=" + this.RefNo + "'>新建审核分组</a></b><br><font color=green>系统会让您输入审核的信息，并创建审核分组。</font>");
                    this.Pub1.AddLi("<b><a href='GroupField.aspx?DoType=NewEvalGroup&RefNo=" + this.RefNo+"'>创建工作质量考核字段分组</a></b><br><font color=green>创建质量考核: EvalEmpNo,EvalEmpName,EvalCent,EvalNote 4个必要的字段。</font>");
                    this.Pub1.AddULEnd();
                    this.Pub1.AddFieldSetEnd();
                    return;
                case "NewCheckGroup":
                    this.Pub1.AddFieldSet("<a href=GroupField.aspx?DoType=FunList&RefNo=" + this.RefNo + " >字段分组向导</a> - " + "审核分组");
                    TB tbc = new TB();
                    tbc.ID = "TB_Sta";
                    this.Pub1.Add("审核岗位<font color=red>*</font>");
                    this.Pub1.Add(tbc);
                    this.Pub1.AddBR("<font color=green>比如:分局长审核、科长审核、总经理审核。</font>");
                    this.Pub1.AddBR();

                    tbc = new TB();
                    tbc.ID = "TB_Prx";
                    this.Pub1.Add("字段前缀:");
                    this.Pub1.Add(tbc);
                    this.Pub1.AddBR("<font color=green>用于自动创建字段，请输入英文字母或者字母数字组合。系统自动依据您的输入产生字段。如：XXX_Note，审核意见。XXX_RDT审核时间。XXX_Checker审核人，为空系统自动用拼音表示。</font>");
                    this.Pub1.AddBR();
                    this.Pub1.AddHR();
                    Btn btnc = new Btn();
                    btnc.Click += new EventHandler(btn_Check_Click);
                    btnc.Text = "保存";

                    this.Pub1.Add(btnc);
                    this.Pub1.AddFieldSetEnd();
                    return;
                case "NewEvalGroup":

                    GroupField gf = new GroupField();
                    gf.Lab = "工作质量考核";
                    gf.EnName = this.RefNo;
                    gf.Insert();

                    MapAttr attr = new MapAttr();
                    attr.FK_MapData = this.RefNo;
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
                    attr.FK_MapData = this.RefNo;
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
                    attr.FK_MapData = this.RefNo;
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
                    attr.FK_MapData = this.RefNo;
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
                case "NewGroup":
                    GroupFields mygfs = new GroupFields(this.RefNo);
                    GroupField gf1 = new GroupField();
                    gf1.Idx = mygfs.Count;
                    gf1.Lab = "新建字段分组"; // "新建字段分组";
                    gf1.EnName = this.RefNo;
                    if (gf1.IsExit(GroupFieldAttr.EnName, gf1.EnName, GroupFieldAttr.Lab, gf1.Lab) == false)
                    {
                        gf1.Insert();
                    }
                    this.Response.Redirect("GroupField.aspx?RefNo=" + this.RefNo + "&RefOID=" + gf1.OID, true);
                    return;
                default:
                    break;
            }

            #region edit operation
            GroupField en = new GroupField(this.RefOID);
            this.Pub1.Add("<Table border=0 align=center>");
            this.Pub1.AddCaptionLeft("字段分组");
            this.Pub1.AddTR();

            this.Pub1.AddTD("分组名称");

            TB tb = new TB();
            tb.ID = "TB_Lab_" + en.OID;
            tb.Text = en.Lab;
            tb.Columns = 50;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD align=center colspan=2>");
            Btn btn = new Btn();
            btn.Text = "保存";
            btn.ID = "Btn_Save";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
            btn = new Btn();
            btn.Text = "保存并关闭";
            btn.ID = "Btn_SaveAndClose";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            btn = new Btn();
            btn.Text = "新建字段";
            btn.ID = "Btn_NewField";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

            //btn = new Btn();
            //btn.Text = this.ToE("CopyField", "复制字段");
            //btn.ID = "Btn_CopyField";
            //btn.Click += new EventHandler(btn_Click);
            //this.Pub1.Add(btn);


            btn = new Btn();
            btn.Text = "仅删除分组";
            btn.ID = "Btn_Delete";
            btn.Click += new EventHandler(btn_del_Click);
            btn.Attributes["onclick"] = " return confirm('您确认吗？');";
            this.Pub1.Add(btn);

            btn = new Btn();
            btn.Text = "删除分组与数据";
            btn.ID = "Btn_DeleteAll";
            btn.Click += new EventHandler(btn_delAll_Click);
            btn.Attributes["onclick"] = " return confirm('您确认吗？');";
            this.Pub1.Add(btn);

            this.Pub1.Add("</TD>");
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
          //  this.Pub1.Add("把相近的字段放在一个大单元格里，只是为了显示所用没有任何计算意义。");
            #endregion

        }

        void btnC_Click(object sender, EventArgs e)
        {
            BP.WF.Node mynd = new BP.WF.Node(this.RefNo);
            BP.WF.Nodes nds = new BP.WF.Nodes(mynd.FK_Flow);
            foreach (BP.WF.Node nd in nds)
            {
                if ("ND" + nd.NodeID == this.RefNo)
                    continue;

                GroupFields gfs = new GroupFields("ND" + nd.NodeID);
                foreach (GroupField gf in gfs)
                {
                    string id = "CB_" + gf.OID;
                    if (this.Pub1.GetCBByID(id).Checked == false)
                        continue;

                    MapAttrs attrs = new MapAttrs();
                    attrs.Retrieve(MapAttrAttr.GroupID, gf.OID);
                    if (attrs.Count == 0)
                        continue;

                }
            }
        }
        void btn_del_Click(object sender, EventArgs e)
        {
            GroupField gf = new GroupField(this.RefOID);
            gf.Delete();
            
            BP.WF.Template.MapFoolForm md = new BP.WF.Template.MapFoolForm(gf.EnName);
            md.DoCheckFixFrmForUpdateVer();
            
            this.WinClose();// ("删除成功。");
        }

        void btn_delAll_Click(object sender, EventArgs e)
        {
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.GroupID, this.RefOID);
            foreach (MapAttr attr in attrs)
            {
                if (attr.HisEditType != EditType.Edit)
                    continue;
                if (attr.KeyOfEn == "FID")
                    continue;
                attr.Delete();
            }

            MapDtls dtls = new MapDtls();
            dtls.Retrieve(MapDtlAttr.GroupID, this.RefOID);
            foreach (MapDtl dtl in dtls)
                dtl.Delete();

            GroupField gf = new GroupField(this.RefOID);
            gf.Delete();
            this.WinClose();// ("删除成功。");
        }

        void btn_Click(object sender, EventArgs e)
        {
            GroupField en = new GroupField(this.RefOID);
            en.Lab = this.Pub1.GetTBByID("TB_Lab_" + en.OID).Text;
            en.Update();

            Btn btn = sender as Btn;
            switch (btn.ID)
            {
                case "Btn_SaveAndClose":
                    this.WinClose();
                    break;
                case "Btn_NewField":
                    this.Session["GroupField"] = this.RefOID;
                    this.Response.Redirect("Do.aspx?DoType=AddF&MyPK=" + this.RefNo + "&GroupField=" + this.RefOID, true);
                    break;
                case "Btn_CopyField":
                    this.Response.Redirect("CopyFieldFromNode.aspx?FK_Node=" + this.RefNo + "&GroupField=" + this.RefOID, true);
                    break;
                default:
                    this.Response.Redirect("GroupField.aspx?RefNo=" + this.RefNo + "&RefOID=" + this.RefOID, true);
                    break;
            }
        }
    }

}