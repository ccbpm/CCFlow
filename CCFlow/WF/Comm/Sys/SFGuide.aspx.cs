using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Data;
using BP.Web.Controls;

namespace CCFlow.WF.Comm.Sys
{
    public partial class SFGuide : System.Web.UI.Page
    {
        #region 属性.
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
                    return 1;
                }
            }
        }
        public string FK_SFDBSrc
        {
            get
            {
                return this.Request.QueryString["FK_SFDBSrc"];
            }
        }
        public string MyPK
        {
            get
            {
                return this.Request.QueryString["MyPK"];
            }
        }
        public string Idx
        {
            get
            {
                return "1";
            }
        }
        public string GroupField
        {
            get
            {
                return "GroupField";
            }
        }
        /// <summary>
        /// 判断表符合规则的数据
        /// </summary>
        private Dictionary<int, string[]> regs = new Dictionary<int, string[]>
                                                     {
                                                         {0, new[] {"id", "no", "pk"}},
                                                         {1, new[] {"name", "title"}},
                                                         {2, new[] {"parentid", "parentno"}}
                                                     };

        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = Request.QueryString["method"];

            if (string.IsNullOrWhiteSpace(method))
                return;

            string resultString = string.Empty;
            string sfno = Request.QueryString["sfno"];
            SFTable sftable = null;
            DataTable dt = null;
            StringBuilder s = null;

            try
            {
                switch (method)
                {
                    case "getinfo": //获取数据源字典表信息
                        if (string.IsNullOrWhiteSpace(sfno))
                            resultString = ReturnJson(false, "参数不正确", false);
                        else
                        {
                            sftable = new SFTable(sfno);
                            dt = sftable.ToDataTableField("info");

                            foreach (DataColumn col in dt.Columns)
                                col.ColumnName = col.ColumnName.ToUpper();

                            resultString = ReturnJson(true, BP.Tools.Json.ToJson(dt), true);// "{\"success\": true, \"data\": " + +"\"}";
                        }
                        break;
                    case "saveinfo":    //保存
                        bool isnew = Convert.ToBoolean(Request.QueryString["isnew"]);
                        sfno = Request.QueryString["NO"];
                        string name = Request.QueryString["NAME"];
                        int srctype = int.Parse(Request.QueryString["SRCTYPE"]);
                        int codestruct = int.Parse(Request.QueryString["CODESTRUCT"]);
                        string defval = Request.QueryString["DEFVAL"];
                        string sfdbsrc = Request.QueryString["FK_SFDBSRC"];
                        string srctable = Request.QueryString["SRCTABLE"];
                        string columnvalue = Request.QueryString["COLUMNVALUE"];
                        string columntext = Request.QueryString["COLUMNTEXT"];
                        string parentvalue = Request.QueryString["PARENTVALUE"];
                        string tabledesc = Request.QueryString["TABLEDESC"];
                        string selectstatement = Request.QueryString["SELECTSTATEMENT"];

                        //判断是否已经存在
                        sftable = new SFTable();
                        sftable.No = sfno;

                        if (isnew && sftable.RetrieveFromDBSources() > 0)
                        {
                            resultString = ReturnJson(false, "字典编号" + sfno + "已经存在，不允许重复。", false);
                        }
                        else
                        {
                            sftable.Name = name;
                            sftable.SrcType = (SrcType) srctype;
                            sftable.CodeStruct = (CodeStruct) codestruct;
                            sftable.DefVal = defval;
                            sftable.FK_SFDBSrc = sfdbsrc;
                            sftable.SrcTable = srctable;
                            sftable.ColumnValue = columnvalue;
                            sftable.ColumnText = columntext;
                            sftable.ParentValue = parentvalue;
                            sftable.TableDesc = tabledesc;
                            sftable.SelectStatement = selectstatement;

                            switch(sftable.SrcType)
                            {
                                case SrcType.BPClass:
                                    string[] nos = sftable.No.Split('.');
                                    sftable.FK_Val = "FK_" + nos[nos.Length - 1].TrimEnd('s');
                                    sftable.FK_SFDBSrc = "local";
                                    break;
                                default:
                                    sftable.FK_Val = "FK_" + sftable.No;
                                    break;
                            }

                            try
                            {
                                sftable.Save();
                                resultString = ReturnJson(true, "保存成功！", false);
                            }
                            catch(Exception ex)
                            {
                                resultString = ReturnJson(false, ex.Message, false);
                            }
                        }
                        break;
                    case "getclass": //获取类列表
                        string stru = Request.QueryString["struct"];
                        int st = 0;

                        if (string.IsNullOrWhiteSpace(stru) || !int.TryParse(stru, out st))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else
                        {
                            string error = string.Empty;
                            ArrayList arr = null;
                            SFTables sfs = new SFTables();
                            Entities ens = null;
                            SFTable sf = null;
                            sfs.Retrieve(SFTableAttr.SrcType, (int)SrcType.BPClass);
                            
                            try
                            {
                                switch (st)
                                {
                                    case 0:
                                        arr = ClassFactory.GetObjects("BP.En.EntityNoName");
                                        break;
                                    case 1:
                                        arr = ClassFactory.GetObjects("BP.En.EntitySimpleTree");
                                        break;
                                    default:
                                        arr = new ArrayList();
                                        break;
                                }
                            }
                            catch(Exception ex)
                            {
                                error = "ClassFactory.GetObjects错误:" + ex.Message;
                            }

                            if (string.IsNullOrWhiteSpace(error))
                            {
                                s = new StringBuilder("[");

                                foreach (BP.En.Entity en in arr)
                                {
                                    try
                                    {
                                        if (en == null)
                                            continue;

                                        ens = en.GetNewEntities;

                                        if (ens == null)
                                            continue;

                                        sf = sfs.GetEntityByKey(ens.ToString()) as SFTable;

                                        if ((sf != null && sf.No != sfno) ||
                                            string.IsNullOrWhiteSpace(ens.ToString()))
                                            continue;

                                        s.Append(string.Format(
                                            "{{\"NO\":\"{0}\",\"NAME\":\"{0}[{1}]\",\"DESC\":\"{1}\"}},", ens,
                                            en.EnDesc));
                                    }
                                    catch
                                    {
                                        //BP.DA.Log
                                        //  BP.DA.Log.DefaultLogWriteLine(ex.Message);
                                        continue;
                                    }
                                }

                                resultString = ReturnJson(true, s.ToString().TrimEnd(',') + "]", true);
                            }
                            else
                            {
                                resultString = ReturnJson(false, error, false);
                            }
                        }
                        break;
                    case "getsrcs": //获取数据源列表
                        string type = Request.QueryString["type"];
                        int itype;
                        bool onlyWS = false;

                        SFDBSrcs srcs = new SFDBSrcs();

                        if (!string.IsNullOrWhiteSpace(type) && int.TryParse(type, out itype))
                        {
                            onlyWS = true;
                            srcs.Retrieve(SFDBSrcAttr.DBSrcType, itype);
                        }
                        else
                        {
                            srcs.RetrieveAll();
                        }

                        dt = srcs.ToDataTableField();

                        foreach (DataColumn col in dt.Columns)
                            col.ColumnName = col.ColumnName.ToUpper();

                        if (!onlyWS)
                        {
                            List<DataRow> wsRows = new List<DataRow>();

                            foreach (DataRow r in dt.Rows)
                            {
                                if (Equals(r["DBSRCTYPE"], (int)DBSrcType.WebServices))
                                    wsRows.Add(r);
                            }

                            foreach (DataRow r in wsRows)
                                dt.Rows.Remove(r);
                        }

                        resultString = ReturnJson(true, BP.Tools.Json.ToJson(dt), true);
                        break;
                    case "gettvs": //获取表/视图列表
                        string src = Request.QueryString["src"];

                        if (string.IsNullOrWhiteSpace(src))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else
                        {
                            SFDBSrc sr = new SFDBSrc(src);
                            dt = sr.GetTables();

                            foreach (DataColumn col in dt.Columns)
                                col.ColumnName = col.ColumnName.ToUpper();

                            resultString = ReturnJson(true, BP.Tools.Json.ToJson(dt), true);
                        }
                        break;
                    case "getcols": //获取表/视图的列信息
                        src = Request.QueryString["src"];
                        string table = Request.QueryString["table"];

                        if (string.IsNullOrWhiteSpace(src))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else if (string.IsNullOrWhiteSpace(table))
                        {
                            resultString = ReturnJson(true, "[]", true);
                        }
                        else
                        {
                            SFDBSrc sr = new SFDBSrc(src);
                            dt = sr.GetColumns(table);

                            foreach (DataColumn col in dt.Columns)
                                col.ColumnName = col.ColumnName.ToUpper();

                            foreach (DataRow r in dt.Rows)
                            {
                                r["NAME"] = r["NO"] +
                                            (r["NAME"] == null || r["NAME"] == DBNull.Value ||
                                             string.IsNullOrWhiteSpace(r["NAME"].ToString())
                                                 ? ""
                                                 : string.Format("[{0}]", r["NAME"]));
                            }

                            string re = BP.Tools.Json.DataTableToJson(dt);
                            resultString = ReturnJson(true, re, true);
                        }
                        break;
                    case "getmtds": //获取WebService方法列表
                        src = Request.QueryString["src"];

                        if (string.IsNullOrWhiteSpace(src))
                        {
                            resultString = ReturnJson(false, "参数不正确", false);
                        }
                        else
                        {
                            SFDBSrc sr = new SFDBSrc(src);

                            if (sr.DBSrcType != DBSrcType.WebServices)
                            {
                                resultString = ReturnJson(false, "数据源“" + sr.Name + "”不是WebService数据源", false);
                            }
                            else
                            {
                                List<WSMethod> mtds = GetWebServiceMethods(sr);

                                resultString = ReturnJson(true, LitJson.JsonMapper.ToJson(mtds), true);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                resultString = ReturnJson(false, ex.Message, false);
            }

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(resultString);
            Response.End();
            //#region Step = 1

            //if (this.Step == 1)
            //{
            //    BP.Sys.SFDBSrcs ens = new BP.Sys.SFDBSrcs();
            //    ens.RetrieveAll();

            //    Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "第1步：选择下拉框数据来源类型");
            //    Pub1.AddTREnd();

            //    Pub1.AddTR();
            //    Pub1.AddTDBegin();
            //    Pub1.AddUL("class='navlist'");

            //    this.Pub1.AddLi("<a href='/WF/Admin/FoolFormDesigner/SFTable.aspx?DoType=New&MyPK=" + this.MyPK + "&Idx=" + this.Idx + "&FromApp=SL' ><b><img src='./Img/Table.png' border=0 style='width:17px;height:17px;' />外键型表或视图</b></a> -  比如：岗位、税种、行业、科目，本机上一个表组成一个下拉框。");
            //    this.Pub1.AddLi("<a href='/WF/Admin/FoolFormDesigner/SFSQL.aspx?DoType=New&MyPK=" + this.MyPK + "&Idx=" + this.Idx + "&FromApp=SL'><b><img src='./Img/View.png' border=0  style='width:17px;height:17px;' />外部表SQL数据源</b></a> -  比如：配置一个SQL通过数据库连接或获取的外部数据，组成一个下拉框。");
            //    this.Pub1.AddLi("<a href='/WF/Admin/FoolFormDesigner/SFWS.aspx?DoType=AddSFWebServeces&MyPK=" + this.MyPK + "&FType=Class&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "&FromApp=SL'><b><img src='./Img/WS.png' border=0 />WebServices数据</b></a> -  比如：通过调用Webservices接口获得数据，组成一个下拉框。");

            //   // this.Pub1.AddLi("<a href='/WF/Admin/FoolFormDesigner/SFWS.aspx?DoType=AddSFWebServeces&MyPK=" + this.MyPK + "&FType=Class&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "&FromApp=SL'><b><img src='./Img/WS.png' border=0 />WebServices数据</b></a> -  比如：通过调用Webservices接口获得数据，组成一个下拉框。");


            //   //// Pub1.AddLi("<div><a href='SFGuide.aspx?Step=12&FK_SFDBsrc=local'><img src='../../Img/New.gif' align='middle' /><span>创建本地编码字典表</span></a></div>");
            //   // foreach (BP.Sys.SFDBSrc item in ens)
            //   // {
            //   //     Pub1.AddLi("<div><a href='SFGuide.aspx?Step=2&FK_SFDBSrc=" + item.No + "'><span class='nav'>" +item.IP+ item.No + "  -  " + item.Name + "</span></a></div>");
            //   // }

            //   // this.Pub1.AddFieldSet("新增下拉框(外键、外部表、WebServices)字段(通常只有编号名称两个列)");
            //   // this.Pub1.AddUL();
            //   // this.Pub1.AddLi("<a href='Do.aspx?DoType=AddSFTable&MyPK=" + this.MyPK + "&FType=Class&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "'><b>外键型</b></a> -  比如：岗位、税种、行业、科目，本机上一个表组成一个下拉框。");
            //   // this.Pub1.AddLi("<a href='Do.aspx?DoType=AddSFSQL&MyPK=" + this.MyPK + "&FType=Class&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "'><b>外部表</b></a> -  比如：配置一个SQL通过数据库连接或获取的外部数据，组成一个下拉框。");
            //   // this.Pub1.AddLi("<a href='Do.aspx?DoType=AddSFWebServeces&MyPK=" + this.MyPK + "&FType=Class&Idx=" + this.Idx + "&GroupField=" + this.GroupField + "'><b>WebServices</b></a> -  比如：通过调用Webservices接口获得数据，组成一个下拉框。");
            //   // this.Pub1.AddULEnd();
            //   // this.Pub1.AddFieldSetEnd();

            //    //创建.
            //    //Pub1.AddLi("<div><a href=\"javascript:WinOpen('./SFDBSrcNewGuide.htm?DoType=New')\" ><img src='../../Img/New.gif' align='middle' /><span class='nav'>新建数据源</span></a></div>");
            //    //Pub1.AddLi("<div><a href='SFGuide.aspx?Step=22'><img src='../../Img/New.gif' align='middle' /><span class='nav'>新建WebService数据源</span></a></div>");


            //    Pub1.AddULEnd();
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTREnd();


            //    Pub1.AddTableEnd();
            //}
            //#endregion

            //#region Step = 2

            //if (this.Step == 2)
            //{
            //    SFDBSrc src = new SFDBSrc(this.FK_SFDBSrc);

            //    Pub1.Add("<div class='easyui-layout' data-options=\"fit:true\">");
            //    Pub1.Add(string.Format("<div data-options=\"region:'west',split:true,title:'选择 {0} 表/视图'\" style='width:200px;'>",
            //                           src.No));

            //    var lb = new LB();
            //    lb.ID = "LB_Table";
            //    lb.BindByTableNoName(src.GetTables());
            //    lb.Style.Add("width", "100%");
            //    lb.Style.Add("height", "100%");
            //    lb.AutoPostBack = true;
            //    lb.SelectedIndexChanged += new EventHandler(lb_SelectedIndexChanged);
            //    Pub1.Add(lb);

            //    Pub1.AddDivEnd();

            //    Pub1.Add("<div data-options=\"region:'center',title:'第2步：请填写基础信息'\" style='padding:5px;'>");
            //    Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");

            //    var dbType = src.DBSrcType;
            //    if (dbType == DBSrcType.Localhost)
            //    {
            //        switch (SystemConfig.AppCenterDBType)
            //        {
            //            case DBType.MSSQL:
            //                dbType = DBSrcType.SQLServer;
            //                break;
            //            case DBType.Oracle:
            //                dbType = DBSrcType.Oracle;
            //                break;
            //            case DBType.MySQL:
            //                dbType = DBSrcType.MySQL;
            //                break;
            //            case DBType.Informix:
            //                dbType = DBSrcType.Informix;
            //                break;
            //            default:
            //                throw new Exception("没有涉及到的连接测试类型...");
            //        }
            //    }

            //    var islocal = (src.DBSrcType == DBSrcType.Localhost).ToString().ToLower();

            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("style='width:100px'", "值(编号)：");
            //    var ddl = new DDL();
            //    ddl.ID = "DDL_ColValue";
            //    ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}','{2}',{3})", src.No, src.DBName, dbType, islocal));
            //    Pub1.AddTDBegin();
            //    Pub1.Add(ddl);
            //    Pub1.Add("&nbsp;编号列，比如：类别编号");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("标签(名称)：");
            //    ddl = new DDL();
            //    ddl.ID = "DDL_ColText";
            //    ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}','{2}',{3})", src.No, src.DBName, dbType, islocal));
            //    Pub1.AddTDBegin();
            //    Pub1.Add(ddl);
            //    Pub1.Add("&nbsp;显示的列，比如：类别名称");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("父结点值(字段)：");
            //    ddl = new DDL();
            //    ddl.ID = "DDL_ColParentNo";
            //    ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}','{2}',{3})", src.No, src.DBName, dbType, islocal));
            //    Pub1.AddTDBegin();
            //    Pub1.Add(ddl);
            //    Pub1.Add("&nbsp;如果是树类型实体，该列设置有效，比如：上级类别编号");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("字典表类型：");

            //    ddl = new DDL();
            //    ddl.ID = "DDL_CodeStruct";
            //    ddl.SelfBindSysEnum(SFTableAttr.CodeStruct);
            //    ddl.Attributes.Add("onchange", string.Format("generateSQL('{0}','{1}','{2}',{3})", src.No, src.DBName, dbType, islocal));
            //    Pub1.AddTD(ddl);
            //    Pub1.AddTREnd();

            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("查询语句：");
            //    var tb = new TB();
            //    tb.ID = "TB_SelectStatement";
            //    tb.TextMode = TextBoxMode.MultiLine;
            //    tb.Columns = 60;
            //    tb.Rows = 10;
            //    tb.Style.Add("width", "99%");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("<br />&nbsp;说明：查询语句可以修改，但请保证查询语句的准确性及有效性！");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTableEnd();
            //    Pub1.AddBR();
            //    Pub1.AddBR();
            //    Pub1.AddSpace(1);

            //    var btn = new LinkBtn(false, NamesOfBtn.Next, "下一步");
            //    btn.Click += new EventHandler(btn_Click);
            //    Pub1.Add(btn);
            //    Pub1.AddSpace(1);

            //    Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");

            //    Pub1.AddDivEnd();
            //    Pub1.AddDivEnd();

            //    if (!IsPostBack && lb.Items.Count > 0)
            //    {
            //        lb.SelectedIndex = 0;
            //        ShowSelectedTableColumns();
            //    }
            //}
            //#endregion

            //#region Step = 12

            //if (this.Step == 12)
            //{
            //    Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("colspan='2'", "第2步：创建");
            //    Pub1.AddTREnd();

            //    TextBox tb = new TextBox();
            //    tb.ID = "TB_No";
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("style='width:100px'", "表英文名称：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;必须以字母或者下画线开头");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.Name;
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "表中文名称：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;显示的标签");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.TableDesc;
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "描述：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;表描述");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTableEnd();
            //    Pub1.AddBR();
            //    Pub1.AddBR();
            //    Pub1.AddSpace(1);

            //    var btn = new LinkBtn(false, NamesOfBtn.Apply, "执行创建");
            //    btn.Click += new EventHandler(btn_Create_Local_Click);
            //    Pub1.Add(btn);
            //    Pub1.AddSpace(1);

            //    Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");
            //}
            //#endregion

            //#region Step = 22

            //if (this.Step == 22)
            //{
            //    Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("colspan='2'", "第2步：创建本地WebService数据源表");
            //    Pub1.AddTREnd();

            //    TextBox tb = new TextBox();
            //    tb.ID = "TB_No";
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("style='width:100px'", "WebService数据源编号：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;必须以字母或者下画线开头，比如：HR,CRM");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.Name;
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "WebService数据源名称：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;显示的数据源名称,比如:人力资源系统,客户关系管理系统.");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.TableDesc;
            //    tb.Style.Add("width", "300px");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "WebService连接Url：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;WebService地址,比如:http://127.0.0.1/CCFormTester.asmx");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.SrcTable;
            //    tb.Style.Add("width", "300px");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "WebService接口名称：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;比如：GetEmps");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.SelectStatement;
            //    tb.Style.Add("width", "300px");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "WebService接口参数：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("格式说明");
            //    Pub1.AddTDDoc("如：WorkId=@WorkID&FK_Flow=@FK_Flow&FK_Node=@FK_Node&SearchType=1，带@的参数值在运行时自动使用发起流程的相关参数值替换，而不带@的参数值使用后面的赋值；参数个数与WebServices接口方法的参数个数一致，且顺序一致，且值均为字符类型。");
            //    Pub1.AddTREnd();


            //    Pub1.AddTableEnd();
            //    Pub1.AddBR();
            //    Pub1.AddBR();
            //    Pub1.AddSpace(1);

            //    var btn = new LinkBtn(false, NamesOfBtn.Apply, "执行创建");
            //    btn.Click += new EventHandler(btn_Create_WebService_Click);
            //    Pub1.Add(btn);
            //    Pub1.AddSpace(1);

            //    Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");
            //}

            //#endregion

            //#region Step = 3

            //if (this.Step == 3)
            //{
            //    Pub1.AddTable("class='Table' cellSpacing='1' cellPadding='1'  border='1' style='width:100%'");
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("colspan='2'", "第3步：创建");
            //    Pub1.AddTREnd();

            //    TextBox tb = new TextBox();
            //    tb.ID = "TB_No";
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("style='width:100px'", "表英文名称：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;必须以字母或者下画线开头");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.Name;
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "表中文名称：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;显示的标签");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    tb = new TextBox();
            //    tb.ID = "TB_" + SFTableAttr.TableDesc;
            //    Pub1.AddTR();
            //    Pub1.AddTDGroupTitle("", "描述：");
            //    Pub1.AddTDBegin();
            //    Pub1.Add(tb);
            //    Pub1.Add("&nbsp;表描述");
            //    Pub1.AddTDEnd();
            //    Pub1.AddTREnd();

            //    Pub1.AddTableEnd();
            //    Pub1.AddBR();
            //    Pub1.AddBR();
            //    Pub1.AddSpace(1);

            //    var btn = new LinkBtn(false, NamesOfBtn.Apply, "执行创建");
            //    btn.Click += new EventHandler(btn_Create_Click);
            //    Pub1.Add(btn);
            //    Pub1.AddSpace(1);

            //    Pub1.Add("<a href='" + Request.UrlReferrer + "' class='easyui-linkbutton'>上一步</a>");
            //}
            //#endregion
        }

        void lb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedTableColumns();
        }


        void btn_Create_WebService_Click(object sender, EventArgs e)
        {
            //SFTable table = new SFTable();
            //table = this.Pub1.Copy(table) as SFTable;
            //table.SrcType = SrcType.WebServices;  //9表示WebService数据源表类型，added by liuxc,2015-9-12


            //if (string.IsNullOrWhiteSpace(table.No) || string.IsNullOrWhiteSpace(table.Name) || string.IsNullOrWhiteSpace(table.TableDesc) || string.IsNullOrWhiteSpace(table.SrcTable))
            //{
            //    EasyUiHelper.AddEasyUiMessagerAndBack(this, "@编号、名称、Url、接口名称必须填写.", "错误", "error");
            //    return;
            //}

            //if (table.IsExit(SFTableAttr.No, table.No))
            //{
            //    EasyUiHelper.AddEasyUiMessagerAndBack(this, "@对象（" + table.No + "）已经存在.", "错误", "error");
            //    return;
            //}

            //table.Save();

            //EasyUiHelper.AddEasyUiMessagerAndGo(this, "创建成功！", "提示", "info", "SFGuide.aspx?Step=1");
        }

        void btn_Create_Click(object sender, EventArgs e)
        {
            //SFTable table = new SFTable();
            //table = this.Pub1.Copy(table) as SFTable;
            //table.FK_SFDBSrc = Request.QueryString["FK_SFDBSrc"];
            //table.SrcTable = this.Request.QueryString["LB_Table"];
            //table.FK_Val = "FK_" + table.SrcTable;
            //table.ColumnText = this.Request.QueryString["DDL_ColText"];
            //table.ColumnValue = this.Request.QueryString["DDL_ColValue"];
            //table.ParentValue = this.Request.QueryString["DDL_ColParentNo"];
            //table.SelectStatement = Uri.UnescapeDataString(Request.QueryString["TB_SelectStatement"]);
            //table.CodeStruct = (CodeStruct)int.Parse(Request.QueryString["DDL_CodeStruct"]);

            //if (table.CodeStruct == CodeStruct.Tree)
            //{
            //  //  table.IsTree = true;
            //}
            //else
            //{
            //   // table.IsTree = false;
            //    table.ParentValue = null;
            //}

            //if (BP.DA.DBAccess.IsExitsObject(table.No))
            //{
            //    EasyUiHelper.AddEasyUiMessagerAndBack(this, "@对象（" + table.No + "）已经存在.", "错误", "error");
            //    return;
            //}

            //var sql = "CREATE VIEW " + table.No + ""
            //          + " AS "
            //          + table.SelectStatement;

            //BP.DA.DBAccess.RunSQL(sql);

            //table.Save();

            //EasyUiHelper.AddEasyUiMessagerAndGo(this, "创建成功！查看数据……", "提示", "info", "../FoolFormDesigner/SFTableEditData.aspx?FK_SFTable=" + table.No);
        }

        void btn_Create_Local_Click(object sender, EventArgs e)
        {
            //    SFTable table = new SFTable();
            //    table = this.Pub1.Copy(table) as SFTable;
            //    table.FK_SFDBSrc = Request.QueryString["FK_SFDBSrc"];
            //    table.SrcTable = table.No;
            //    table.ColumnText = "Name";
            //    table.ColumnValue = "No";
            // //   table.IsTree = false;

            //    if (BP.DA.DBAccess.IsExitsObject(table.No))
            //    {
            //        //判断已经存在的表是否符合NoName规则，如果符合，则自动加入到SFTable中
            //        var src = new SFDBSrc(this.FK_SFDBSrc);
            //        var columns = src.GetColumns(table.No);
            //        var cols = new List<string>();

            //        foreach (DataRow dr in columns.Rows)
            //            cols.Add(dr["name"].ToString());

            //        var regColValue = cols.FirstOrDefault(o => regs[0].Contains(o.ToLower()));
            //        var regColText = cols.FirstOrDefault(o => regs[1].Contains(o.ToLower()));
            //        var regColParentNo = cols.FirstOrDefault(o => regs[2].Contains(o.ToLower()));

            //        if(regColValue != null && regColText != null && regColParentNo != null)
            //        {
            //            table.CodeStruct = CodeStruct.Tree;
            //           // table.IsTree = true;
            //            table.ColumnValue = regColValue;
            //            table.ColumnText = regColText;
            //            table.ParentValue = regColParentNo;
            //            table.FK_SFDBSrc = "local";

            //            table.Save();
            //            EasyUiHelper.AddEasyUiMessagerAndGo(this, "您所创建的“" + table.No + "” 名称的字典表，本地库已经存在，已成功注册！编辑数据……", "提示", "info", "../FoolFormDesigner/SFTableEditData.aspx?FK_SFTable=" + table.No);
            //        }
            //        else if(regColValue != null && regColText != null)
            //        {
            //            table.CodeStruct = 0;
            //            //table.IsTree = false;
            //            table.ColumnValue = regColValue;
            //            table.ColumnText = regColText;
            //            table.ParentValue = null;
            //            table.FK_SFDBSrc = "local";

            //            table.Save();
            //            EasyUiHelper.AddEasyUiMessagerAndGo(this, "您所创建的“" + table.No + "” 名称的字典表，本地库已经存在，已成功注册！编辑数据……", "提示", "info", "../FoolFormDesigner/SFTableEditData.aspx?FK_SFTable=" + table.No);
            //        }
            //        else
            //        {
            //            EasyUiHelper.AddEasyUiMessagerAndBack(this, "@对象（" + table.No + "）已经存在.", "错误", "error");
            //        }

            //        return;
            //    }

            //    var sql = new StringBuilder();
            //    sql.AppendLine(string.Format("CREATE TABLE dbo.{0}", table.No));
            //    sql.AppendLine("(");
            //    sql.AppendLine("No    NVARCHAR(50) NOT NULL PRIMARY KEY,");
            //    sql.AppendLine("Name  NVARCHAR(100) NULL");
            //    sql.AppendLine(") ");

            //    BP.DA.DBAccess.RunSQL(sql.ToString());

            //    sql.Clear();
            //    sql.Append("INSERT INTO [dbo].[{0}] ([No], [Name]) VALUES ('0{1}', 'Item{1}')");

            //    for (var i = 1; i < 4; i++)
            //    {
            //        BP.DA.DBAccess.RunSQL(string.Format(sql.ToString(), table.No, i));
            //    }

            //    sql.Clear();
            //    sql.AppendFormat(
            //        "EXECUTE sp_addextendedproperty N'MS_Description', N'{0}', N'SCHEMA', N'dbo', N'TABLE', N'{1}', NULL, NULL",
            //        table.Name, table.No);

            //    BP.DA.DBAccess.RunSQL(sql.ToString());

            //    table.Save();

            //    EasyUiHelper.AddEasyUiMessagerAndGo(this, "创建成功！编辑数据……", "提示", "info", "../FoolFormDesigner/SFTableEditData.aspx?FK_SFTable=" + table.No);
        }

        void btn_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(Pub1.GetTBByID("TB_SelectStatement").Text))
            //{
            //    EasyUiHelper.AddEasyUiMessagerAndBack(this, "查询语句不能为空", "错误", "error");
            //    return;
            //}

            //string url = "SFGuide.aspx?Step=3&FK_SFDBSrc=" + this.FK_SFDBSrc + "&DDL_ColValue=" + this.Pub1.GetDDLByID("DDL_ColValue").SelectedItemStringVal;
            //url += "&LB_Table=" + this.Pub1.GetLBByID("LB_Table").SelectedItemStringVal;
            //url += "&DDL_ColText=" + this.Pub1.GetDDLByID("DDL_ColText").SelectedItemStringVal;
            //url += "&DDL_ColParentNo=" + this.Pub1.GetDDLByID("DDL_ColParentNo").SelectedItemStringVal;
            //url += "&TB_SelectStatement=" + Uri.EscapeDataString(Pub1.GetTBByID("TB_SelectStatement").Text);
            //url += "&DDL_CodeStruct=" + Pub1.GetDDLByID("DDL_CodeStruct").SelectedItemStringVal;

            //Response.Redirect(url, true);
        }

        /// <summary>
        /// 加载选中表的所有列信息
        /// </summary>
        private void ShowSelectedTableColumns()
        {
            //var src = new SFDBSrc(this.FK_SFDBSrc);
            //var colTables = src.GetColumns(this.Pub1.GetLBByID("LB_Table").SelectedItemStringVal);
            //colTables.Columns.Add("text", typeof(string));

            //var cols = new List<string>();
            //string type;
            //var length = 0;
            //foreach (DataRow dr in colTables.Rows)
            //{
            //    cols.Add(dr["name"].ToString());
            //    type = dr["type"].ToString().ToLower();
            //    length = int.Parse(dr["length"].ToString());

            //    dr["text"] = dr["name"] + " (" + (LengthTypes.Contains(type) ?
            //        (string.Format("{0}{1}", type,
            //        (length == -1 || length == 0) ?
            //        (MaxTypes.Contains(type) ? "(max)" : "")
            //          : string.Format("({0})", length))) : type) + ")";
            //}

            ////自动判断是否符合规则
            //var regColValue = cols.FirstOrDefault(o => regs[0].Contains(o.ToLower()));
            //var regColText = cols.FirstOrDefault(o => regs[1].Contains(o.ToLower()));
            //var regColParentNo = cols.FirstOrDefault(o => regs[2].Contains(o.ToLower()));

            //var ddl = this.Pub1.GetDDLByID("DDL_ColValue");
            //ddl.Items.Clear();
            //ddl.Bind(colTables, "name", "text");

            //if (regColValue != null)
            //    ddl.SetSelectItem(regColValue);

            //ddl = this.Pub1.GetDDLByID("DDL_ColText");
            //ddl.Items.Clear();
            //ddl.Bind(colTables, "name", "text");

            //if (regColText != null)
            //    ddl.SetSelectItem(regColText);

            //ddl = this.Pub1.GetDDLByID("DDL_ColParentNo");
            //ddl.Items.Clear();
            //ddl.Bind(colTables, "name", "text");

            //if (regColParentNo != null)
            //    ddl.SetSelectItem(regColParentNo);

            //Pub1.GetTBByID("TB_SelectStatement").Text = string.Empty;
            //Pub1.GetDDLByID("DDL_CodeStruct").SetSelectItem((regColValue != null && regColText != null &&
            //                                                  regColParentNo != null)
            //                                                      ? "1"
            //                                                      : "0");
        }

        private string[] LengthTypes = new[] { "char", "nchar", "varchar", "nvarchar", "varbinary", "varchar2" };   //varchar2为Oracle数据库中的字段类型
        private string[] MaxTypes = new[] { "nvarchar", "varbinary", "varchar" }; //如:nvarchar(max) 则maxLength为-1

        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public List<WSMethod> GetWebServiceMethods(SFDBSrc dbsrc)
        {
            if (dbsrc == null || string.IsNullOrWhiteSpace(dbsrc.IP)) return new List<WSMethod>();

            var wsurl = dbsrc.IP.ToLower();
            if (!wsurl.EndsWith(".asmx") && !wsurl.EndsWith(".svc"))
                throw new Exception("@失败:" + dbsrc.No + " 中WebService地址不正确。");

            wsurl += wsurl.EndsWith(".asmx") ? "?wsdl" : "?singleWsdl";

            #region //解析WebService所有方法列表
            //var methods = new Dictionary<string, string>(); //名称Name，全称Text
            List<WSMethod> mtds = new List<WSMethod>();
            WSMethod mtd = null;
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
                    mtd = new WSMethod();
                    //1.接口名称 ele.Name
                    mtd.NO = ele.Name;
                    mtd.PARAMS = new Dictionary<string, string>();
                    //2.接口返回值类型
                    ctype = res.SchemaType as XmlSchemaComplexType;
                    seq = ctype.Particle as XmlSchemaSequence;

                    if (seq != null && seq.Items.Count > 0)
                        mtd.RETURN = resType = (seq.Items[0] as XmlSchemaElement).SchemaTypeName.Name;
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
                            mtd.PARAMS.Add(pe.Name, pe.SchemaTypeName.Name);
                        }

                        mparams = mparams.TrimEnd(", ".ToCharArray());
                    }

                    mtd.NAME = string.Format("{0} {1}({2})", resType, ele.Name, mparams);
                    mtds.Add(mtd);
                    //methods.Add(ele.Name, string.Format("{0} {1}({2})", resType, ele.Name, mparams));
                }
            }

            stream.Close();
            stream.Dispose();
            wc.Dispose();
            #endregion

            return mtds;
        }

        /// <summary>
        /// 生成返给前台页面的JSON字符串信息
        /// </summary>
        /// <param name="success">是否操作成功</param>
        /// <param name="msg">消息</param>
        /// <param name="haveMsgJsoned">msg是否已经JSON化</param>
        /// <returns></returns>
        private string ReturnJson(bool success, string msg, bool haveMsgJsoned)
        {
            string kh = haveMsgJsoned ? "" : "\"";
            return "{\"success\":" + success.ToString().ToLower() + ",\"msg\":" + kh + (haveMsgJsoned ? msg : msg.Replace("\"", "'")) +
                   kh + "}";
        }

        public class WSMethod
        {
            public string NO { get; set; }

            public string NAME { get; set; }

            public Dictionary<string, string> PARAMS { get; set; }

            public string RETURN { get; set; }
        }
    }
}