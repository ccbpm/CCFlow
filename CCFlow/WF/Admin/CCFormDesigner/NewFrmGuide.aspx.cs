using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Data;
using BP.WF;
using BP.Sys;
using BP.DA;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using BP.Web;

namespace CCFlow.WF.Admin.CCFormDesigner
{
    /// <summary>
    /// 负责人：秦发亮.
    /// </summary>
    public partial class NewFrmGuide : System.Web.UI.Page
    {
        #region 参数
        public int Step
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["Step"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FrmType
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FrmType"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DBSrc
        {
            get
            {
                string val = this.Request.QueryString["DBSrc"];
                if (val == null || val == "")
                    return "local";
                return val;
            }
        }
        /// <summary>
        /// 表单类别
        /// </summary>
        public string FK_FrmSort
        {
            get
            {
                return this.Request.QueryString["FK_FrmSort"];
            }
        }
        #endregion 参数

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Step == 0)
            {
                this.BindStep0();
            }

            if (this.Step == 1)
            {
                this.BindStep1();
            }
        }
        public void BindStep0()
        {
            this.Pub1.AddTable(" style='width:100%;' ");

            this.Pub1.AddCaption("表单创建向导:请选择要创建的表单类型。");

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin();

            this.Pub1.AddFieldSet("<a class='title' href='?Step=1&FrmType=" + (int)BP.Sys.FrmType.FreeFrm + "&FK_FrmSort=" + this.FK_FrmSort + "&DBSrc=" + this.DBSrc + "' >创建自由表单</a>");


            this.Pub1.Add("<div class='con-list' style='float:left'>");
            this.Pub1.AddUL();
            this.Pub1.AddLi("自由表单是ccbpm推荐使用的表单.");
            this.Pub1.AddLi("他有丰富的界面元素,可以满足不同的应用需求.");
            this.Pub1.AddLi("采用了关系数据库存储格式，可以导出到xml存储，可以运行在任何设备上,实现与平台无关.");
            this.Pub1.AddLi("可以导入导出，格式不受影响，java , .net 移动终端都可以使用.");
            this.Pub1.AddULEnd();
            this.Pub1.Add("</div>");

            this.Pub1.Add("<div style='float:right'>");
            this.Pub1.Add("<a class='link-img' href='http://blog.csdn.net/jflows/article/details/50034329'  target='_blank'><img src='./Img/ziyouForm.png' width='400px' ></a>");
            //this.Pub1.Add("<a href='http://ccflow.org/' ><img src='./Img/FrmType/FixFrm.png' width='400px' ></a>");
            this.Pub1.Add("</div>");

            this.Pub1.AddFieldSetEnd();


            this.Pub1.AddFieldSet("<a class='title' href='?Step=1&FrmType=" + (int)BP.Sys.FrmType.FoolForm + "&FK_FrmSort=" + this.FK_FrmSort + "&DBSrc=" + this.DBSrc + "' >创建傻瓜表单</a>");
            this.Pub1.Add("<div class='con-list' style='float:left'>");
            this.Pub1.AddUL();
            this.Pub1.AddLi("傻瓜表单与自由表单就是展示格式不同,其他的与自由表单一样.");
            this.Pub1.AddLi("傻瓜表单有固定的列与行，格式简洁、清新、实用.");
            this.Pub1.AddULEnd();
            this.Pub1.Add("</div>");
            
            this.Pub1.Add("<div style='float:right'>");
            this.Pub1.Add("<a class='link-img' href='http://ccbpm.mydoc.io/?v=5404&t=17922' target='_blank' ><img src='./Img/shaguaForm.png' width='400px' ></a>");
            this.Pub1.Add("</div>");
            this.Pub1.AddFieldSetEnd();

            this.Pub1.AddFieldSet("<a class='title' href='?Step=1&FrmType=" + (int)BP.Sys.FrmType.ExcelFrm + "&FK_FrmSort=" + this.FK_FrmSort + "&DBSrc=" + this.DBSrc + "' >创建Excel表单</a>");
            this.Pub1.Add("<div class='con-list' style='float:left'>");
            this.Pub1.AddUL();
            this.Pub1.AddLi("Excel表单以excel表单模版为基础展现给用户，数据的展现与采集以excel文件为基础。");
            this.Pub1.AddLi("您可以设置每个excel的单元格对应一个表的一个字段,");
            this.Pub1.AddLi("用户在保存数据的时候可以保存到excel文件背后的数据表里。");
            this.Pub1.AddLi("数据表可以用于综合分析，而excel文件用于数据展现。");
            this.Pub1.AddLi("使用excel表单必须运行在IE浏览器上，需要支持activeX插件。");
            this.Pub1.AddULEnd();
            this.Pub1.Add("</div>");

            this.Pub1.Add("<div style='float:right'>");
            this.Pub1.Add("<a class='link-img' href='http://ccbpm.mydoc.io/?v=5404&t=17922' target='_blank' ><img src='./Img/excelForm.jpg' width='400px' ></a>");
            this.Pub1.Add("</div>");
            this.Pub1.AddFieldSetEnd();


            this.Pub1.AddFieldSet("<a class='title' href='?Step=1&FrmType=" + (int)BP.Sys.FrmType.WordFrm + "&FK_FrmSort=" + this.FK_FrmSort + "&DBSrc=" + this.DBSrc + "' >创建Word表单</a>");
            this.Pub1.Add("<div class='con-list' style='float:left'>");
            this.Pub1.AddUL();
            this.Pub1.AddLi("Word表单以Word表单模版为基础展现给用户，数据的展现与采集以Word文件为基础。");
            this.Pub1.AddLi("您可以设置每个Word的标签对应一个表的一个字段,");
            this.Pub1.AddLi("用户在保存数据的时候可以保存到Word文件背后的数据表里。");
            this.Pub1.AddLi("数据表可以用于综合分析，而excel文件用于数据展现。");
            this.Pub1.AddLi("使用Word表单必须运行在IE浏览器上，需要支持activeX插件。");
            this.Pub1.AddLi("Word表单多用于公文。");
            this.Pub1.AddULEnd();
            this.Pub1.Add("</div>");

            this.Pub1.Add("<div style='float:right'>");
            this.Pub1.Add("<a class='link-img' href='http://ccbpm.mydoc.io/?v=5404&t=17922' target='_blank' ><img src='./Img/wordForm.jpg' width='400px' ></a>");
            this.Pub1.Add("</div>");
            this.Pub1.AddFieldSetEnd();

            this.Pub1.AddFieldSet("<a class='title' href='?Step=1&FrmType=" + (int)BP.Sys.FrmType.Url + "&FK_FrmSort=" + this.FK_FrmSort + "&DBSrc=" + this.DBSrc + "' >嵌入式表单</a>");
            this.Pub1.Add("<div class='con-list' style='float:left'>");
            this.Pub1.AddUL();
            this.Pub1.AddLi("自己编写一个表单jsp,aspx,php .... ");
            this.Pub1.AddLi("通过本功能把他注册到ccbpm的表单系统中,可以被其他的程序引用，比如流程引擎。");
            this.Pub1.AddLi("如果要被驰骋工作流程引擎调用，自己定义的表单需要有一个约定的 Save javascript 函数,");
            this.Pub1.AddLi("用于保存整个表单的数据，如果保存的时候异常，就抛出错误。");
            this.Pub1.AddLi("驰骋工作流引擎在调用您的表单的时候，就需要传入一个数值类型的主键，参数名称为OID,");
            this.Pub1.AddLi("该嵌入式表单获取这个参数做为主键处理。");
            this.Pub1.AddLi("该表单用于特殊用户格式要求的表单，或者客户现在已有的表单。");
            this.Pub1.AddULEnd();
            this.Pub1.Add("</div>");

            this.Pub1.Add("<div style='float:right'>");
            this.Pub1.Add("<a class='link-img' href='http://blog.csdn.net/jflows/article/details/50150457' target='_blank' ><img src='./Img/selfForm.png' width='400px' ></a>");
            this.Pub1.Add("</div>");
            this.Pub1.AddFieldSetEnd();

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        public void BindStep1()
        {
            this.Pub1.AddTable();
            this.Pub1.AddCaption("表单创建向导:填写表单基础信息");

            int idx = 0;
            SysEnum se = new SysEnum(MapDataAttr.FrmType, this.FrmType);
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("表单类型");
            this.Pub1.AddTD(se.Lab);
            this.Pub1.AddTD("返回上一步可以更改");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("数据源");
            BP.Sys.SFDBSrc srcs = new SFDBSrc(this.DBSrc);
            this.Pub1.AddTD(srcs.Name);
            this.Pub1.AddTD("您可以把表单创建不同的数据源上.");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("创建路径");
            //更改绑定样式 qin
            //BP.Sys.SysFormTrees trees = new SysFormTrees();

            //去除对应数据源根目录
            //trees.Retrieve(SysFormTreeAttr.DBSrc, this.DBSrc, SysFormTreeAttr.IsDir, "0");
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Sys_FormTree WHERE DBSrc='local' AND IsDir='0'");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_FrmTree";
            BP.Web.Controls.DDL.MakeTree(dt, "ParentNo", "0", "No", "Name", ddl,-1);

            //设置选择的值.
            BP.Web.Controls.Glo.DDL_SetSelectVal(ddl,this.FK_FrmSort);

            //ddl.Bind(trees, this.DBSrc);
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("表单类别.");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("<font color='Red'>*</font>表单名称");
            TextBox tb = new TextBox();
            tb.ID = "TB_Name";
            tb.Columns = 90;
            tb.AutoPostBack = true;
            tb.TextChanged += new EventHandler(tb_TextChanged);
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("1到30个字符");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("<font color='Red'>*</font>表单编号");
            tb = new TextBox();
            tb.ID = "TB_No";
            tb.Columns = 90;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("以字母或者下划线开头，不能包含中文或者其他特殊字符.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("<font color='Red'>*</font>数据表");
            tb = new TextBox();
            tb.ID = "TB_PTable";
            tb.Columns = 90;

            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("只能以字母或者下划线开头，不能包含中文与其它特殊字符。");
            this.Pub1.AddTREnd();

            #region 快速填写.
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTD("快速填写");
            this.Pub1.AddTDBegin();
            RadioButton rb = new RadioButton();
            rb.ID = "RB0";
            rb.Text = "生成全拼名称";
            rb.CheckedChanged += new EventHandler(tb_TextChanged);
            rb.Checked = true;
            rb.AutoPostBack = true;
            rb.GroupName = "ss";
            this.Pub1.Add(rb);

            rb = new RadioButton();
            rb.ID = "RB1";
            rb.Text = "生成简拼名称";
            rb.CheckedChanged += new EventHandler(tb_TextChanged);
            rb.GroupName = "ss";
            rb.AutoPostBack = true;
            this.Pub1.Add(rb);
            this.Pub1.AddTDEnd();
            this.Pub1.AddTD("注意:允许多个表单指定同一个表.");
            this.Pub1.AddTREnd();
            #endregion 快速填写.

            #region 表单生成方式.
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            if ((BP.Sys.FrmType)(this.FrmType) != BP.Sys.FrmType.Url)
            {
                if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.FreeFrm ||
                    (BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.FoolForm )
                {

                    this.Pub1.AddTD("表单生成方式");
                    this.Pub1.AddTDBegin("colspan=2");
                    rb = new RadioButton();
                    rb.ID = "RB_FrmGenerMode_0";
                    rb.Text = "直接生成表单";
                    rb.Checked = true;
                    rb.GroupName = "s2s";
                    this.Pub1.Add(rb);
                    this.Pub1.AddBR();

                    rb = new RadioButton();
                    rb.ID = "RB_FrmGenerMode_1";
                    rb.Text = "从ccfrom云表单库中选择一个表单模版导入";
                    rb.GroupName = "s2s";
                    this.Pub1.Add(rb);
                    this.Pub1.AddBR();

                    rb = new RadioButton();
                    rb.ID = "RB_FrmGenerMode_2";
                    rb.Text = "从本机或其他数据库的的表导入表结构";
                    rb.GroupName = "s2s";
                    this.Pub1.Add(rb);
                    this.Pub1.AddBR();
                }
                //ExcelFrm,WordFrm 只保留上传
                if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.ExcelFrm ||
                    (BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.WordFrm)
                {
                    this.Pub1.AddTD("<font color='Red'>*</font>上传" + se.Lab+"（默认模版）");
                    this.Pub1.AddTDBegin("colspan=2");

                    FileUpload fUp = new FileUpload();
                    fUp.ID = "fUpFrm";
                    fUp.Width = 300;

                    this.Pub1.Add("&nbsp;&nbsp;&nbsp;");
                    this.Pub1.Add(fUp);
                
                }
                this.Pub1.AddTDEnd();
            }
            else
            {
                this.Pub1.AddTD("<font color='Red'>*</font>表单Url");
                this.Pub1.AddTDBegin("colspan=2");
                tb = new TextBox();
                tb.ID = "TB_Url";
                tb.Columns = 40;

                this.Pub1.Add(tb);
                this.Pub1.Add("&nbsp;&nbsp;&nbsp;请正确填写表单链接,支持全局变量@Ho");
                this.Pub1.AddTDEnd();
            }
            this.Pub1.AddTREnd();


            //  //ExcelFrm,WordFrm 只保留上传
            //if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.ExcelFrm ||
            //    (BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.WordFrm)
            //{
            //    this.Pub1.AddTRend();
            //    this.Pub1.AddTREnd();
            //}

            #endregion 表单生成方式.

            #region 操作按钮放到table中，布局缩放不会乱
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);
            this.Pub1.AddTDBegin(" colspan='3' ");

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "下一步";
            btn.Click += new EventHandler(BindStep1_Click);
            this.Pub1.Add(btn);
            this.Pub1.Add("<input type='button' value='返回上一步' onclick='Back();' />");

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            #endregion

            this.Pub1.AddTableEnd();
        }
        void tb_TextChanged(object sender, EventArgs e)
        {
            string name = this.Pub1.GetTextBoxByID("TB_Name").Text;
            string val = "";

            if (this.Pub1.GetRadioButtonByID("RB0").Checked)
                val = BP.DA.DataType.ParseStringToPinyin(name);
            else
                val = BP.DA.DataType.ParseStringToPinyinJianXie(name);

            this.Pub1.GetTextBoxByID("TB_No").Text = val;
            this.Pub1.GetTextBoxByID("TB_PTable").Text = "CCFrom_" + val;
        }
        void BindStep1_Click(object sender, EventArgs e)
        {
            MapData md = new MapData();
            md.Name = this.Pub1.GetTextBoxByID("TB_Name").Text;
            md.No = this.Pub1.GetTextBoxByID("TB_No").Text;
            md.PTable = this.Pub1.GetTextBoxByID("TB_PTable").Text;
            md.FK_FrmSort = this.Pub1.GetDDLByID("DDL_FrmTree").SelectedValue;
            md.FK_FormTree = this.Pub1.GetDDLByID("DDL_FrmTree").SelectedValue;
            md.AppType = "0";//独立表单
            md.DBSrc = this.DBSrc;

            if (md.Name.Length == 0 || md.No.Length == 0 || md.PTable.Length == 0)
            {
                BP.Sys.PubClass.Alert("必填项不能为空.");
                return;
            }

            if (md.IsExits == true)
            {
                BP.Sys.PubClass.Alert("表单ID:" + md.No + "已经存在.");
                return;
            }

            md.HisFrmTypeInt = this.FrmType; //表单类型.

            switch ((BP.Sys.FrmType)(this.FrmType))
            {
                //自由，傻瓜，SL表单不做判断
                case BP.Sys.FrmType.FreeFrm:
                case BP.Sys.FrmType.FoolForm:
                    break;
                case BP.Sys.FrmType.Url:
                    string url = this.Pub1.GetTextBoxByID("TB_Url").Text;
                    if (string.IsNullOrEmpty(url))
                    {
                        BP.Sys.PubClass.Alert("必填项不可以为空");
                        return;
                    }
                    md.Url = url;
                    break;
                //如果是以下情况，导入模式
                case BP.Sys.FrmType.WordFrm:
                case BP.Sys.FrmType.ExcelFrm:
                    var file = Request.Files[0];
                    string savePath = null;
                    var ext = Path.GetExtension(file.FileName).ToLower(); //后缀

                    ext = Path.GetExtension(file.FileName).ToLower();

                    if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.ExcelFrm &&
                        ext != ".xls" && ext != ".xlsx")
                    {
                        BP.Sys.PubClass.Alert("上传的Excel文件格式错误.");
                        return;
                    }

                    if ((BP.Sys.FrmType)(this.FrmType) == BP.Sys.FrmType.WordFrm &&
                        ext != ".doc" && ext != ".docx")
                    {
                        BP.Sys.PubClass.Alert("上传的Word文件格式错误.");
                        return;
                    }
                    savePath = BP.Sys.SystemConfig.PathOfDataUser + "FrmOfficeTemplate\\";
                    if (Directory.Exists(savePath) == false)
                        Directory.CreateDirectory(savePath);
                    file.SaveAs(savePath + this.Pub1.GetTextBoxByID("TB_No").Text + ext);

                    break;
                default:
                    throw new Exception("未知表单类型.");
            }
            md.Insert();

            if (md.HisFrmType == BP.Sys.FrmType.WordFrm || md.HisFrmType == BP.Sys.FrmType.ExcelFrm)
            {
                /*把表单模版存储到数据库里 */
                this.Response.Redirect("/WF/Comm/En.htm?EnsName=BP.WF.Template.MapFrmExcels&PK=" + md.No, true);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm && this.Pub1.GetRadioButtonByID("RB_FrmGenerMode_2").Checked)
            {
                this.Response.Redirect("../FoolFormDesigner/ImpTableField.htm?DoType=New&FK_MapData=" + md.No);
                return;
            }

            if (md.HisFrmType == BP.Sys.FrmType.FreeFrm)
            {
                this.Response.Redirect("FormDesigner.htm?FK_MapData=" + md.No);
            }

            if (md.HisFrmType == BP.Sys.FrmType.FoolForm)
            {
                this.Response.Redirect("../FoolFormDesigner/Designer.htm?IsFirst=1&FK_MapData=" + md.No);
            }

        }
    }
}