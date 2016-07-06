using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Schema;
using BP.DA;
using BP.En;
using BP.Sys;

namespace CCFlow.WF.MapDef
{
    public partial class SFWS : BP.Web.WebPage
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
            SFTable main = new SFTable {FK_SFDBSrc = string.Empty}; //此处FK_SFDBSrc的默认值为local，需要将其设为空，否则下方报错

            if (this.RefNo != null)
            {
                main.No = this.RefNo;
                main.Retrieve();
            }

            this.BindSFTable(main);
        }

        public void BindSFTable(SFTable en)
        {
            bool isItem = false;
            string star = "<font color=red><b>(*)</b></font>";
            this.Ucsys1.AddTable();

            #region 生成标题.
            if (this.FromApp == "SL")
            {
                if (this.RefNo == null)
                    this.Ucsys1.AddCaption("新建WebService数据源接口");
                else
                    this.Ucsys1.AddCaption("编辑WebService数据源接口");
            }
            else
            {
                this.Ucsys1.AddCaption("<a href='Do.aspx?DoType=AddF&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'><img src='/WF/Img/Btn/Back.gif'>返回</a> - <a href='Do.aspx?DoType=AddSFWS&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "'>WebService数据源接口</a> - 新建WebService数据源接口");
            }

            if (this.RefNo == null)
                this.Title = "新建WebService数据源接口";
            else
                this.Title = "编辑WebService数据源接口";

            #endregion 生成标题.

            int idx = 0;
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("Idx");
            this.Ucsys1.AddTDTitle("项目");
            this.Ucsys1.AddTDTitle("采集");
            this.Ucsys1.AddTDTitle("备注");
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("接口英文名称" + star);
            var tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + SFTableAttr.No;
            tb.Text = en.No;
            if (this.RefNo == null)
                tb.Enabled = true;
            else
                tb.Enabled = false;

            if (tb.Text == "")
                tb.Text = "SF_";

            this.Ucsys1.AddTD(tb);
            this.Ucsys1.AddTDBigDoc("必须以字母或者下划线开头，不能包含特殊字符。");
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("接口中文名称" + star);
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + SFTableAttr.Name;
            tb.Text = en.Name;
            this.Ucsys1.AddTD(tb);
            this.Ucsys1.AddTD("WebService中的接口方法的中文名称。");
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("数据源" + star);
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            SFDBSrcs srcs = new SFDBSrcs();
            BP.En.QueryObject qo = new QueryObject(srcs);
            qo.AddWhere(SFDBSrcAttr.DBSrcType, " = ", "100");
            qo.DoQuery();
            ddl.Bind(srcs, en.FK_SFDBSrc);
            ddl.ID = "DDL_" + SFTableAttr.FK_SFDBSrc;
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged);
            this.Ucsys1.AddTD(ddl);
            this.Ucsys1.AddTD("选择数据源,点击这里<a href=\"javascript:WinOpen('/WF/Comm/Sys/SFDBSrcNewGuide.aspx?DoType=New')\">创建</a>，<a href='SFWS.aspx?DoType=New&MyPK=" + this.MyPK + "&Idx='>刷新</a>。");
            this.Ucsys1.AddTREnd();

            var rt = en.TableDesc.Split(',');

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("选择接口" + star);
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_" + SFTableAttr.TableDesc;

            if (srcs.Count > 0)
            {
                var ms = GetWebServiceMethods(!string.IsNullOrWhiteSpace(en.FK_SFDBSrc) ? (SFDBSrc)srcs.GetEntityByKey(SFDBSrcAttr.No, en.FK_SFDBSrc) : (SFDBSrc)srcs[0]);

                foreach (var m in ms)
                {
                    ddl.Items.Add(new ListItem(m.Value, m.Key));
                }

                ddl.SetSelectItem(rt.Length == 2 ? rt[0] : ms.Count > 0 ? ms.First().Key : string.Empty);
            }

            this.Ucsys1.AddTD(ddl);
            this.Ucsys1.AddTDBigDoc("选择WebService中的接口方法名。");
            this.Ucsys1.AddTREnd();
            
            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("colspan=3", "接口参数定义" + star + "支持ccform表达式，允许有@WebUser.No,@WebUser.Name,@WebUser.FK_Dept变量。");
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + SFTableAttr.SelectStatement;
            tb.Text = en.SelectStatement;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 3;
            tb.Columns = 70;
            this.Ucsys1.AddTD("colspan=3", tb);
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("colspan=3", "如：WorkId=@WorkID&FK_Flow=@FK_Flow&FK_Node=@FK_Node&SearchType=1，带@的参数值在运行时自动使用发起流程的相关参数值替换，而不带@的参数值使用后面的赋值；参数个数与WebServices接口方法的参数个数一致，且顺序一致，且值均为字符类型。");
            this.Ucsys1.AddTREnd();
            
            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("返回值类型" + star);
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_ResultType";

            ddl.Items.Add(new ListItem("DataTable数据表", "DataTable"));
            ddl.Items.Add(new ListItem("DataSet数据集", "DataSet"));
            ddl.Items.Add(new ListItem("Json字符串", "Json"));
            ddl.Items.Add(new ListItem("Xml字符串", "Xml"));

            if (rt.Length == 2)
                ddl.SetSelectItem(rt[1]);

            this.Ucsys1.AddTDBegin();
            this.Ucsys1.Add(ddl);
            this.Ucsys1.AddBR();
            this.Ucsys1.Add("注意：所有返回值类型都需有No,Name这两列。" +
                "<script type='text/javascript'>" +
                "   var info = '1. DataTable数据表，必须为DataTable命名。\\n" +
                "2. DataSet数据集，只取数据集里面的第1个DataTable。\\n" +
                "3. Json字符串，格式如：\\n" +
                            "[\\n" +
                            "  {\"No\":\"001\",\"Name\":\"生产部\"},\\n" +
                            "  {\"No\":\"002\",\"Name\":\"研发部\"},\\n" +
                            "  ...\\n" +
                            "]\\n" +
                            "4. Xml字符串，格式如：\\n" +
                            "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\\n" +
                            "<Array>\\n" +
                            "  <Item>\\n" +
                            "    <No>001</No>\\n" +
                            "    <Name>生产部</Name>\\n" +
                            "  </Item>\\n" +
                            "  <Item>\\n" +
                            "    <No>002</No>\\n" +
                            "    <Name>研发部</Name>\\n" +
                            "  </Item>\\n" +
                            "  ...\\n" +
                            "</Array>';" +
                "</script>" +
                "<a href='javascript:void(0)' onclick='alert(info)'>格式说明</a>");
            this.Ucsys1.AddTDEnd();
            this.Ucsys1.AddTDBigDoc("选择WebService中的接口方法返回值的类型。");
            this.Ucsys1.AddTREnd();
            
            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.AddTD("返回数据结构");
            ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_" + SFTableAttr.CodeStruct;
            ddl.BindSysEnum(SFTableAttr.CodeStruct, (int)en.CodeStruct);
            this.Ucsys1.AddTD(ddl);
            this.Ucsys1.AddTD("WebService接口返回的数据结构，用于在下拉框中不同格式的展现。");
            this.Ucsys1.AddTREnd();

            isItem = this.Ucsys1.AddTR(isItem);
            this.Ucsys1.AddTDIdx(idx++);
            this.Ucsys1.Add("<TD colspan=3 align=center>");
            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            if (this.RefNo == null)
                btn.Text = "创建";
            else
                btn.Text = "保存";
            btn.Click += new EventHandler(btn_Save_Click);
            this.Ucsys1.Add(btn);

            if (this.FromApp != "SL")
            {
                btn = new Button();
                btn.ID = "Btn_Add";
                btn.CssClass = "Btn";
                btn.Text = "添加到表单"; ; // "添加到表单";
                btn.Attributes["onclick"] = " return confirm('您确认吗？');";
                btn.Click += new EventHandler(btn_Add_Click);
                if (this.RefNo == null)
                    btn.Enabled = false;
                this.Ucsys1.Add(btn);
            }

            btn = new Button();
            btn.ID = "Btn_Del";
            btn.CssClass = "Btn";
            btn.Text = "删除";
            btn.Attributes["onclick"] = " return confirm('您确认吗？');";
            if (this.RefNo == null)
                btn.Enabled = false;

            btn.Click += new EventHandler(btn_Del_Click);
            this.Ucsys1.Add(btn);
            this.Ucsys1.Add("</TD>");
            this.Ucsys1.AddTREnd();
            this.Ucsys1.AddTableEnd();
        }

        void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selSrc = this.Ucsys1.GetDDLByID("DDL_" + SFTableAttr.FK_SFDBSrc).SelectedItemStringVal;
            var ddlMethods = this.Ucsys1.GetDDLByID("DDL_" + SFTableAttr.TableDesc);
            ddlMethods.Items.Clear();
            var dbsrc = new SFDBSrc(selSrc);
            var ms = GetWebServiceMethods(dbsrc);

            foreach (var m in ms)
            {
                ddlMethods.Items.Add(new ListItem(m.Value, m.Key));
            }
        }

        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public Dictionary<string, string> GetWebServiceMethods(SFDBSrc dbsrc)
        {
            if (dbsrc == null || string.IsNullOrWhiteSpace(dbsrc.IP)) return new Dictionary<string, string>();

            var wsurl = dbsrc.IP.ToLower();
            if (!wsurl.EndsWith(".asmx") && !wsurl.EndsWith(".svc"))
                throw new Exception("@失败:" + dbsrc.No + " 中WebService地址不正确。");

            wsurl += wsurl.EndsWith(".asmx") ? "?wsdl" : "?singleWsdl";

            #region //解析WebService所有方法列表
            var methods = new Dictionary<string, string>(); //名称Name，全称Text
            var wc = new WebClient();
            var stream = wc.OpenRead(wsurl);
            var sd = ServiceDescription.Read(stream);
            var eles = sd.Types.Schemas[0].Elements.Values.Cast<XmlSchemaElement>();
            var s = new StringBuilder();
            XmlSchemaComplexType ctype = null;
            XmlSchemaSequence seq = null;
            XmlSchemaElement res = null;

            foreach (var ele in eles)
            {
                if (ele == null) continue;

                var resType = string.Empty;
                var mparams = string.Empty;

                //获取接口返回元素
                res = eles.FirstOrDefault(o => o.Name == (ele.Name + "Response"));

                if (res != null)
                {
                    //1.接口名称 ele.Name
                    //2.接口返回值类型
                    ctype = res.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                        resType = (seq.Items[0] as XmlSchemaElement).SchemaTypeName.Name;
                    else
                        continue;// resType = "void";   //去除不返回结果的接口

                    //3.接口参数
                    ctype = ele.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                    {
                        foreach (XmlSchemaElement pe in seq.Items)
                        {
                            mparams += pe.SchemaTypeName.Name + " " + pe.Name + ", ";
                        }

                        mparams = mparams.TrimEnd(", ".ToCharArray());
                    }

                    methods.Add(ele.Name, string.Format("{0} {1}({2})", resType, ele.Name, mparams));
                }
            }

            stream.Close();
            stream.Dispose();
            wc.Dispose();
            #endregion

            return methods;
        }

        void btn_Add_Click(object sender, EventArgs e)
        {
            SFTable table = new SFTable(this.RefNo);
            if (table.GetTableSQL.Rows.Count == 0)
            {
                this.Alert("该表里[" + this.RefNo + "]中没有数据，您需要维护数据才能加入。");
                return;
            }

            this.Response.Redirect("Do.aspx?DoType=AddSFSQLAttr&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&RefNo=" + this.RefNo + "&FromApp=" + this.FromApp, true);
            this.WinClose();
            return;
        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                SFTable main = new SFTable();
                main.No = this.RefNo;
                main.RetrieveFromDBSources();
                main = (SFTable)this.Ucsys1.Copy(main);

                #region 检查必填项.

                if (main.FK_SFDBSrc.Length == 0)
                    throw new Exception("必须选择数据源");

                if (main.TableDesc.Length == 0)
                    throw new Exception("必须选择接口");

                if (main.No.Length == 0)
                    throw new Exception("接口英文名称不能为空");

                if (main.Name.Length == 0)
                    throw new Exception("接口中文名称不能为空");

                var restype = this.Ucsys1.GetDDLByID("DDL_ResultType").SelectedItemStringVal;
                if (restype.Length == 0)
                    throw new Exception("选择接口返回值类型");

                #endregion 检查必填项.

                if (this.RefNo == null)
                {
                    if (main.IsExits == true)
                        throw new Exception("编号[" + main.No + "]已经存在");
                }

                //设置它的数据源类型.
                main.SrcType = SrcType.WebServices;
                main.TableDesc += "," + restype;

                if (this.RefNo == null)
                    main.FK_Val = main.No;

                main.Save();

                //重新生成
                this.Response.Redirect("SFWS.aspx?RefNo=" + main.No + "&MyPK=" + this.MyPK + "&IDX=" + this.IDX + "&FromApp=" + this.FromApp + "&t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
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
                qo.AddWhere(MapAttrAttr.MyDataType, DataType.AppString);
                qo.addAnd();
                qo.AddWhere(MapAttrAttr.UIBindKey, this.RefNo);
                int i = qo.DoQuery();
                if (i == 0)
                {
                    BP.Sys.SFTable m = new SFTable();
                    m.No = this.RefNo;
                    m.Delete();
                    this.ToWFMsgPage("WebService接口删除成功");
                    return;
                }

                string msg = "错误:下列数据已经引用了WebService接口,您不能删除它。";
                foreach (MapAttr attr in attrs)
                    msg += "\t\n" + attr.Field + "" + attr.Name + " 表" + attr.FK_MapData;

                throw new Exception(msg);
            }
            catch (Exception ex)
            {
                BP.Sys.PubClass.ToErrorPage(ex.Message);
            }
        }
    }
}