using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.UC;
namespace CCFlow.WF.MapDef
{

    public partial class Comm_MapDef_SFTable : BP.Web.WebPage
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
        public string FK_SFTable
        {
            get
            {
                string str= this.Request.QueryString["FK_SFTable"];
                if (str==null)
                    str=this.Request.QueryString["RefNo"];
                return str;
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
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
            SFTable main = new SFTable();
            if (this.FK_SFTable != null)
            {
                main.No = this.FK_SFTable;
                main.Retrieve();
            }
            this.BindSFTable(main);
        }
        public void BindSFTable(SFTable en)
        {
            string star = "<font color=red><b>(*)</b></font>";
            this.Ucsys1.AddTable();

            if (this.FromApp == "SL")
            {
                if (this.FK_SFTable == null)
                    this.Ucsys1.AddCaption("新建表");
                else
                    this.Ucsys1.AddCaption("编辑表");
            }
            else
            {
                if (this.FK_SFTable == null)
                    this.Ucsys1.AddCaption("<a href='FieldTypeList.htm?DoType=AddF&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>返回</a> - <a href='SFList.aspx?DoType=AddSFTable&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'>外键</a> - 新建表");
                else
                    this.Ucsys1.AddCaption("<a href='FieldTypeList.htm?DoType=AddF&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>返回</a> - <a href='SFList.aspx?DoType=AddSFTable&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "'>外键</a> - 编辑表");
            }

            if (this.FK_SFTable == null)
                this.Title = "新建表";
            else
                this.Title = "编辑表";

            int idx = 0;
            bool isItem = false;
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("Idx");
            this.Ucsys1.AddTDTitle("项目");
            this.Ucsys1.AddTDTitle("采集");

         
           // this.Ucsys1.AddFieldSet("帮助", help);

            this.Ucsys1.AddTDTitle("备注");
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("数据源" + star);

            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_FK_SFDBSrc";
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveDBSrc();
            ddl.Bind(srcs, en.FK_SFDBSrc);
            this.Ucsys1.AddTD(ddl);

          //  this.Ucsys1.AddTD("外键表的数据源必须与该表单的数据源一致.");
            this.Ucsys1.AddTD("选择数据源,点击这里<a href=\"javascript:WinOpen('/WF/Comm/Sys/SFDBSrcNewGuide.htm?DoType=New')\">创建</a>，<a href='SFTable.aspx?FK_MapData=" + this.FK_MapData + "&FK_SFTable=" + this.FK_SFTable + "&Idx='>刷新</a>。");
            this.Ucsys1.AddTREnd();


            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("表中文名称" + star);
            BP.Web.Controls.TB tb = new BP.Web.Controls.TB();
            tb.ID = "TB_Name";
            tb.Text = en.Name;
            tb.AutoPostBack = true;
            tb.TextChanged += new EventHandler(tbName_TextChanged);
            this.Ucsys1.AddTD(tb);
            this.Ucsys1.AddTD("该表的中文名称，比如：物料类别，科目。");
            this.Ucsys1.AddTREnd();


            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("表英文名称" + star);
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_No";
            tb.Text = en.No;
            if (this.FK_SFTable == null)
                tb.Enabled = true;
            else
                tb.Enabled = false;

            if (tb.Text == "")
                tb.Text = "";          

            this.Ucsys1.AddTD(tb);
            this.Ucsys1.AddTDBigDoc("必须以字母或者下划线开头，不能包含特殊字符。");
            this.Ucsys1.AddTREnd();

          

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("数据结构");
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_" + SFTableAttr.CodeStruct;
            ddl.BindSysEnum(SFTableAttr.CodeStruct, (int)en.CodeStruct);
            this.Ucsys1.AddTD(ddl);
            this.Ucsys1.AddTD("用于在下拉框中不同格式的展现.");
            this.Ucsys1.AddTREnd();
           

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.Add("<TD colspan=3 align=center>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            if (this.FK_SFTable == null)
                btn.Text = "创建";
            else
                btn.Text = "保存";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Ucsys1.Add(btn);


            btn = new Button();
            btn.ID = "Btn_Edit";
            btn.Text = "编辑数据"; // "编辑数据"
            if (this.FK_SFTable == null)
                btn.Enabled = false;
            if (en.IsClass)
                btn.Attributes["onclick"] = "WinOpen('../Search.htm?EnsName=" + en.No + "', 'dg'); return false;";
            else
                btn.Attributes["onclick"] = "WinOpen('SFTableEditData.aspx?FK_SFTable=" + this.FK_SFTable + "', 'dg'); return false;";

            this.Ucsys1.Add(btn);

            if (this.FromApp != "SL")
            {
                btn = new Button();
                btn.ID = "Btn_Add";
                btn.CssClass = "Btn";

                btn.Text = "添加到表单"; ; // "添加到表单";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Add_Click);
                if (this.FK_SFTable == null)
                    btn.Enabled = false;
            }

            this.Ucsys1.Add(btn);
            btn = new Button();
            btn.ID = "Btn_Del";
            btn.CssClass = "Btn";

            btn.Text = "删除";
            btn.Attributes["onclick"] = " return confirm('您确认吗？');";
            if (this.FK_SFTable == null)
                btn.Enabled = false;

            btn.Click += new EventHandler(btn_Del_Click);
            this.Ucsys1.Add(btn);

            //string help = "<ul>";
            //help += "<li>输入:新表名或已经存在的表名或者视图，必须是英文字母或者下划线。</li>";
            //help += "<li>如果该表或视图已经存在本机中，系统就会把他注册到ccform的数据源（Sys_SFTable）里，您可以打开Sys_SFTable查看ccform对外部数据源管理的信息。</li>";
            //help += "<li>如果不存在ccform就会自动创建表，该表有No,Name两个列，并且初始化3笔数据，您可以对该表进行编辑。</li>";
            //help += "</ul>";

            this.Ucsys1.Add( BP.WF.Glo.GenerHelpCCForm("帮助",null,null) );

            this.Ucsys1.Add("</TD>");
            this.Ucsys1.AddTREnd();
            this.Ucsys1.AddTableEnd();
        }

        void tbName_TextChanged(object sender, EventArgs e)
        {
            if (this.Ucsys1.GetTBByID("TB_No").Text == "")
                this.Ucsys1.GetTBByID("TB_No").Text = "SF_"+BP.DA.DataType.ParseStringToPinyin(this.Ucsys1.GetTBByID("TB_Name").Text);
        }
        void btn_Add_Click(object sender, EventArgs e)
        {
            SFTable table = new SFTable(this.FK_SFTable);
            if (table.HisEns.Count == 0)
            {
                this.Alert("该表里[" + this.FK_SFTable + "]中没有数据，您需要维护数据才能加入");
                return;
            }

            this.Response.Redirect("EditTable.aspx?FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "&FK_SFTable=" + this.FK_SFTable + "&FromApp=" + this.FromApp, true);
            this.WinClose();
            return;
        }
        void btn_EditData_Click(object sender, EventArgs e)
        {
            //this.Response.Redirect("SFTable.aspx?DoType=Edit&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&RefNo=" + this.RefNo, true);
            //this.WinClose();
            return;
        }
        void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {

                SFTable main = new SFTable();
                main.No = this.FK_SFTable;
                if (this.FK_SFTable != null)
                    main.RetrieveFromDBSources();

                main = (SFTable)this.Ucsys1.Copy(main);
                if (main.No.Length == 0 || main.Name.Length == 0)
                    throw new Exception("编号与名称不能为空");

                if (this.FK_SFTable == null)
                {
                    main.No = this.Ucsys1.GetTBByID("TB_No").Text;
                    if (main.IsExits)
                    {
                        string sql = "select No,Name FROM " + main.No + " WHERE 1=2";
                        try
                        {
                            BP.DA.DBAccess.RunSQLReturnTable(sql);
                        }
                        catch
                        {
                            this.Alert("错误:表或视图不存在No,Name列不符合约定规则  Key=" + main.No);
                            return;
                        }
                    }
                }
                else
                {
                    main.No = this.FK_SFTable;
                    main.RetrieveFromDBSources();
                    main = (SFTable)this.Ucsys1.Copy(main);

                    if (main.No.Length == 0 || main.Name.Length == 0)
                        throw new Exception("编号与名称不能为空");
                }

                if (main.Name.Length == 0)
                    throw new Exception("编号与名称不能为空");

                if (main.No.Length == 0)
                    throw new Exception("编号与名称不能为空");

                if (this.FK_SFTable == null)
                    main.FK_Val = main.No;

                main.Save();

                //重新生成
                this.Response.Redirect("SFTable.aspx?FK_SFTable=" + main.No + "&FK_MapData=" + this.FK_MapData + "&IDX=" + this.IDX + "&FromApp=" + this.FromApp, true);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message);
            }
        }
        void btn_Del_Click(object sender, EventArgs e)
        {
            try
            {
                // 检查这个类型是否被使用？
                MapAttrs attrs = new MapAttrs();
                QueryObject qo = new QueryObject(attrs);
                qo.AddWhere(MapAttrAttr.MyDataType, (int)FieldTypeS.FK);
                qo.addAnd();
                qo.AddWhere(MapAttrAttr.KeyOfEn, this.FK_SFTable);
                int i = qo.DoQuery();
                if (i == 0)
                {
                    BP.Sys.SFTable m = new SFTable();
                    m.No = this.FK_SFTable;
                    m.Delete();
                    this.ToWFMsgPage("外键删除成功");
                    return;
                }

                string msg = "错误:下列数据已经引用了外键您不能删除它。";
                foreach (MapAttr attr in attrs)
                    msg += "\t\n" + attr.Field + "" + attr.Name + " 表" + attr.FK_MapData;

                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                this.ToErrorPage(ex.Message);
            }

        }
    }

}