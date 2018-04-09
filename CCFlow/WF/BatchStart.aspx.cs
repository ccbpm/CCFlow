using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Rpt;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CCFlow.WF
{
    public partial class BatchStartMy : System.Web.UI.Page
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return "ND" + int.Parse(this.FK_Flow + "01");
            }
        }
        public int RowNum
        {
            get
            {
                return 12;
            }
        }
        public int ShowTabIndex { get; set; }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, WebUser.No) == false)
            {

            }

            Flow fl = new Flow(this.FK_Flow);
            //this.Page.Title = fl.Name;
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            if (fl.BatchStartFields.Length == 0)
            {
                this.Pub2.AddEasyUiPanelInfo("流程属性设置错误", "<b>您需要在流程属性里设置批量发起需要填写的字段。</b>", "icon-no");
                fsexcel.Visible = false;
                this.Pub1.AddEasyUiPanelInfo("流程属性设置错误", "<b>您需要在流程属性里设置批量发起需要填写的字段。</b>", "icon-no");
            }

            MapExts mes = new MapExts(this.FK_MapData);

            BP.WF.Node nd = new BP.WF.Node(int.Parse(this.FK_Flow + "01"));
            Work wk = nd.HisWork;
            wk.ResetDefaultVal();

            this.Master.Page.Title = "批量发起：" + fl.Name;

            this.Pub2.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:98%'");
            #region 输出标题.
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("style='width:60px'", "序");

            string str1 = "<INPUT id='checkedAll' onclick=\"SelectAllBS(this);\" value='选择' type='checkbox' name='checkedAll' ><label for='checkedAll'>选择全部</a>";
            this.Pub2.AddTDTitle("align='left' style='width:100px' nowrap", str1);

            int fieldCount = 0;
            string[] strs = fl.BatchStartFields.Split(',');
            List<string> endStrs = new List<string>();  //过滤后的批量发起字段列表
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                    continue;

                if (endStrs.Contains(str))
                    continue;
                
                foreach (MapAttr attr in attrs)
                {
                    if (str != attr.KeyOfEn)
                        continue;

                    endStrs.Add(str);
                    this.Pub2.AddTDTitle(attr.Name);
                    fieldCount++;
                }
            }

            if(fieldCount==0)
            {
                this.Pub2.Clear();
                this.Pub2.AddEasyUiPanelInfo("流程属性设置错误", "<b>您需要在流程属性里设置批量发起需要填写的字段。</b>", "icon-no");
                fsexcel.Visible = false;
                this.Pub1.AddEasyUiPanelInfo("流程属性设置错误", "<b>您需要在流程属性里设置批量发起需要填写的字段。</b>", "icon-no");
                return;
            }

            this.Pub2.AddTREnd();
            #endregion 输出标题.

            #region 输出标题.
            for (int i = 1; i <= this.RowNum; i++)
            {
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(i);
                CheckBox cbIdx = new CheckBox();
                cbIdx.Checked = false;
                cbIdx.Text = "发起";
                cbIdx.ID = "CB_IDX_" + i;
                this.Pub2.AddTD(cbIdx);

                foreach (string str in endStrs)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;
                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                            continue;

                        TB tb = new TB();
                        tb.ID = "TB_" + attr.KeyOfEn + "_" + i;
                        switch (attr.LGType)
                        {
                            case FieldTypeS.Normal:
                                switch (attr.MyDataType)
                                {
                                    case BP.DA.DataType.AppString:
                                        if (attr.UIRows == 1)
                                        {
                                            tb.Text = attr.DefVal;
                                            tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 15px;padding: 0px;margin: 0px;";
                                            if (attr.UIIsEnable)
                                                tb.CssClass = "TB";
                                            else
                                                tb.CssClass = "TBReadonly";
                                            this.Pub2.AddTD(tb);
                                        }
                                        else
                                        {
                                            tb.TextMode = TextBoxMode.MultiLine;
                                            tb.Text = attr.DefVal;

                                            tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left;padding: 0px;margin: 0px;";
                                            tb.Attributes["maxlength"] = attr.MaxLen.ToString();
                                            tb.Rows = attr.UIRows;

                                            if (attr.UIIsEnable)
                                                tb.CssClass = "TBDoc";
                                            else
                                                tb.CssClass = "TBReadonly";

                                            this.Pub2.AddTD(tb);
                                        }
                                        break;
                                    case BP.DA.DataType.AppDate:
                                        tb.ShowType = TBType.Date;
                                        tb.Text = attr.DefVal;

                                        if (attr.UIIsEnable)
                                            tb.Attributes["onfocus"] = "WdatePicker();";

                                        if (attr.UIIsEnable)
                                            tb.Attributes["class"] = "TB";
                                        else
                                            tb.Attributes["class"] = "TBReadonly";

                                        tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 19px;";
                                        this.Pub2.AddTD(tb);
                                        break;
                                    case BP.DA.DataType.AppDateTime:
                                        tb.ShowType = TBType.DateTime;
                                        tb.Text = attr.DefVal; // en.GetValStrByKey(attr.KeyOfEn);

                                        if (attr.UIIsEnable)
                                            tb.Attributes["class"] = "TBcalendar";
                                        else
                                            tb.Attributes["class"] = "TBReadonly";

                                        if (attr.UIIsEnable)
                                            tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                        tb.Attributes["style"] = "width: " + attr.UIWidth + "px; text-align: left; height: 19px;";
                                        this.Pub2.AddTD(tb);
                                        break;
                                    case BP.DA.DataType.AppBoolean:
                                        CheckBox cb = new CheckBox();
                                        //cb.Width = 350;
                                        cb.Text = attr.Name;
                                        cb.ID = "CB_" + attr.KeyOfEn + "_" + i;
                                        cb.Checked = attr.DefValOfBool;
                                        cb.Enabled = attr.UIIsEnable;
                                        cb.Checked = attr.DefValOfBool;

                                        if (cb.Enabled == false)
                                            cb.Enabled = false;
                                        else
                                        {
                                            //add by dgq 2013-4-9,添加内容修改后的事件
                                            // cb.Attributes["onmousedown"] = "Change('" + attr.FK_MapData + "')";
                                            cb.Enabled = true;
                                        }
                                        this.Pub2.AddTD(cb);
                                        break;
                                    case BP.DA.DataType.AppDouble:
                                    case BP.DA.DataType.AppFloat:
                                        tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;word-break: keep-all;";
                                        tb.Text = attr.DefVal;

                                        if (attr.UIIsEnable)
                                        {
                                            //增加验证
                                            tb.Attributes.Add("onkeyup", @"Change('" + attr.FK_MapData + "');");
                                            tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');TB_ClickNum(this,0);");
                                            tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                            tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'float');";
                                            tb.Attributes["class"] = "TBNum";
                                        }
                                        else
                                            tb.Attributes["class"] = "TBReadonly";

                                        this.Pub2.AddTD(tb);
                                        break;
                                    case BP.DA.DataType.AppInt:
                                        tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;word-break: keep-all;";
                                        tb.Text = attr.DefVal;

                                        if (attr.UIIsEnable)
                                        {
                                            //增加验证
                                            tb.Attributes.Add("onkeyup", @"Change('" + attr.FK_MapData + "');");
                                            tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d]/g,'');TB_ClickNum(this,0);");
                                            tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                            tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'int');";
                                            tb.Attributes["class"] = "TBNum";
                                        }
                                        else
                                            tb.Attributes["class"] = "TBReadonly";

                                        this.Pub2.AddTD(tb);
                                        break;
                                    case BP.DA.DataType.AppMoney:
                                        if (attr.UIIsEnable)
                                        {
                                            //增加验证
                                            tb.Attributes.Add("onkeyup", @"Change('" + attr.FK_MapData + "');");
                                            tb.Attributes.Add("onblur", @"value=value.replace(/[^-?\d+\.*\d*$]/g,'');TB_ClickNum(this,'0.00');");
                                            tb.Attributes.Add("onClick", "TB_ClickNum(this)");
                                            tb.Attributes["OnKeyPress"] += @"javascript:return  VirtyNum(this,'float');";
                                            tb.Attributes["class"] = "TBNum";
                                        }
                                        else
                                            tb.Attributes["class"] = "TBReadonly";
                                        //  tb.ShowType = TBType.Moneny;
                                        tb.Text = attr.DefVal;

                                        tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;";
                                        this.Pub2.AddTD(tb);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            case FieldTypeS.Enum:
                                if (attr.UIContralType == UIContralType.DDL)
                                {
                                    DDL ddle = new DDL();
                                    ddle.ID = "DDL_" + attr.KeyOfEn + "_" + i;
                                    ddle.BindSysEnum(attr.UIBindKey);
                                    ddle.SetSelectItem(attr.DefVal);
                                    ddle.Enabled = attr.UIIsEnable;
                                    ddle.Attributes["tabindex"] = attr.Idx.ToString();
                                    if (attr.UIIsEnable)
                                    {
                                        //add by dgq 2013-4-9,添加内容修改后的事件
                                        //   ddle.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                                    }
                                    //    ddle.Enabled = false;
                                    this.Pub2.AddTD(ddle);
                                }
                                else
                                {

                                }
                                break;
                            case FieldTypeS.FK:
                                DDL ddl1 = new DDL();
                                ddl1.ID = "DDL_" + attr.KeyOfEn + "_" + i;
                                ddl1.Attributes["tabindex"] = attr.Idx.ToString();
                                if (ddl1.Enabled)
                                {
                                    EntitiesNoName ens = attr.HisEntitiesNoName;
                                    ens.RetrieveAll();
                                    ddl1.BindEntities(ens);
                                    ddl1.SetSelectItem(attr.DefVal);
                                    //add by dgq 2013-4-9,添加内容修改后的事件
                                    //  ddl1.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
                                }
                                else
                                {
                                    ddl1.Attributes["style"] = "width: " + attr.UIWidth + "px;height: 19px;";
                                    if (ddl1.Enabled == true)
                                        ddl1.Enabled = false;
                                    ddl1.Attributes["Width"] = attr.UIWidth.ToString();
                                    ddl1.Items.Add(new ListItem(attr.DefVal, attr.DefVal));
                                }
                                ddl1.Enabled = attr.UIIsEnable;
                                this.Pub2.AddTD(ddl1);
                                break;
                            default:
                                break;
                        }
                    }
                }
                this.Pub2.AddTREnd();
            }
            #endregion 输出标题.

            this.Pub2.AddTableEnd();

            #region 处理扩展属性.
            if (mes.Count != 0)
            {
                this.Page.RegisterClientScriptBlock("s81",
              "<script language='JavaScript' src='/WF/Scripts/jquery-1.4.1.min.js' ></script>");
                this.Page.RegisterClientScriptBlock("b81",
             "<script language='JavaScript' src='/WF/CCForm/MapExt.js' defer='defer' type='text/javascript' ></script>");
                this.Pub2.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");
                this.Page.RegisterClientScriptBlock("dCd",
    "<script language='JavaScript' src='/DataUser/JSLibData/" + this.FK_MapData + ".js' ></script>");

                for (int i = 1; i <= this.RowNum; i++)
                {
                    foreach (MapExt me in mes)
                    {
                        if (!strs.Contains(me.AttrOfOper))
                            continue;
                        switch (me.ExtType)
                        {
                            case MapExtXmlList.DDLFullCtrl: // 自动填充.
                                DDL ddlOper = this.Pub2.GetDDLByID("DDL_" + me.AttrOfOper + "_" + i);
                                if (ddlOper == null)
                                    continue;
                                ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                                break;
                            case MapExtXmlList.ActiveDDL:
                                DDL ddlPerant = this.Pub2.GetDDLByID("DDL_" + me.AttrOfOper + "_" + i);
                                string val, valC;
                                DataTable dt;
                                if (ddlPerant == null)
                                    continue;
#warning 此处需要优化
                                string ddlC = "ContentPlaceHolder1_BatchStart1_DDL_" + me.AttrsOfActive + "_" + i;
                                //  ddlPerant.Attributes["onchange"] = " isChange=true; DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";
                                ddlPerant.Attributes["onchange"] = "DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";

                                DDL ddlChild = this.Pub2.GetDDLByID("DDL_" + me.AttrsOfActive + "_" + i);
                                val = ddlPerant.SelectedItemStringVal;
                                if (ddlChild.Items.Count == 0)
                                    valC = wk.GetValStrByKey(me.AttrsOfActive);
                                else
                                    valC = ddlChild.SelectedItemStringVal;

                                string mysql = me.Doc.Replace("@Key", val);
                                if (mysql.Contains("@") && i >= 100)
                                {
                                    mysql = BP.WF.Glo.DealExp(mysql, wk, null);
                                }
                                else
                                {
                                    continue;
                                }

                                dt = DBAccess.RunSQLReturnTable(mysql);

                                ddlChild.Bind(dt, "No", "Name");
                                if (ddlChild.SetSelectItem(valC) == false)
                                {
                                    ddlChild.Items.Insert(0, new ListItem("请选择" + valC, valC));
                                    ddlChild.SelectedIndex = 0;
                                }
                                //  ddlChild.Attributes["onchange"] = " isChange=true;";

                                break;
                            case MapExtXmlList.AutoFullDLL: //自动填充下拉框的范围.
                                DDL ddlFull = this.Pub2.GetDDLByID("DDL_" + me.AttrOfOper + "_" + i);
                                if (ddlFull == null)
                                    continue;

                                string valOld = wk.GetValStrByKey(me.AttrOfOper);
                                //string valOld =ddlFull.SelectedItemStringVal;

                                string fullSQL = me.Doc.Replace("@WebUser.No", WebUser.No);
                                fullSQL = fullSQL.Replace("@WebUser.Name", WebUser.Name);
                                fullSQL = fullSQL.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                fullSQL = fullSQL.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                                fullSQL = fullSQL.Replace("@Key", this.Request.QueryString["Key"]);

                                if (fullSQL.Contains("@"))
                                {
                                    Attrs attrsFull = wk.EnMap.Attrs;
                                    foreach (Attr attr in attrsFull)
                                    {
                                        if (fullSQL.Contains("@") == false)
                                            break;
                                        fullSQL = fullSQL.Replace("@" + attr.Key, wk.GetValStrByKey(attr.Key));
                                    }
                                }

                                //if (fullSQL.Contains("@"))
                                //{
                                //    /*从主表中取数据*/
                                //    Attrs attrsFull = this.MainEn.EnMap.Attrs;
                                //    foreach (Attr attr in attrsFull)
                                //    {
                                //        if (fullSQL.Contains("@") == false)
                                //            break;

                                //        if (fullSQL.Contains("@" + attr.Key) == false)
                                //            continue;

                                //        fullSQL = fullSQL.Replace("@" + attr.Key, this.MainEn.GetValStrByKey(attr.Key));
                                //    }
                                //}

                                ddlFull.Items.Clear();
                                ddlFull.Bind(DBAccess.RunSQLReturnTable(fullSQL), "No", "Name");
                                if (ddlFull.SetSelectItem(valOld) == false)
                                {
                                    ddlFull.Items.Insert(0, new ListItem("请选择" + valOld, valOld));
                                    ddlFull.SelectedIndex = 0;
                                }
                                // ddlFull.Attributes["onchange"] = " isChange=true;";
                                break;
                            case MapExtXmlList.TBFullCtrl: // 自动填充.
                                TextBox tbAuto = this.Pub2.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + i);
                                if (tbAuto == null)
                                    continue;
                                // tbAuto.Attributes["onkeyup"] = " isChange=true; DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";
                                tbAuto.Attributes["onkeyup"] = " DoAnscToFillDiv(this,this.value,\'" + tbAuto.ClientID + "\', \'" + me.MyPK + "\');";

                                tbAuto.Attributes["AUTOCOMPLETE"] = "OFF";
                                if (me.Tag != "")
                                {
                                    /* 处理下拉框的选择范围的问题 */
                                    string[] strsTmp = me.Tag.Split('$');
                                    foreach (string str in strsTmp)
                                    {
                                        string[] myCtl = str.Split(':');
                                        string ctlID = myCtl[0];
                                        DDL ddlC1 = this.Pub2.GetDDLByID("DDL_" + ctlID + "_" + i);
                                        if (ddlC1 == null)
                                        {
                                            //me.Tag = "";
                                            // me.Update();
                                            continue;
                                        }

                                        string sql = myCtl[1].Replace("~", "'");
                                        sql = sql.Replace("@WebUser.No", WebUser.No);
                                        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                                        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                                        sql = sql.Replace("@Key", tbAuto.Text.Trim());
                                        dt = DBAccess.RunSQLReturnTable(sql);
                                        string valC1 = ddlC1.SelectedItemStringVal;
                                        ddlC1.Items.Clear();
                                        foreach (DataRow dr in dt.Rows)
                                            ddlC1.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                                        ddlC1.SetSelectItem(valC1);
                                    }
                                }
                                break;
                            case MapExtXmlList.InputCheck:
                                TextBox tbCheck = this.Pub2.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + i);
                                if (tbCheck != null)
                                    tbCheck.Attributes[me.Tag2] += " rowPK=" + i + "; " + me.Tag1 + "(this);";
                                break;
                            case MapExtXmlList.PopVal: //弹出窗.
                                TB tb = this.Pub2.GetTBByID("TB_" + me.AttrOfOper + "_" + i);
                                //  tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                tb.Attributes["ondblclick"] = " ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.Link: // 超链接.
                                //TB tb = this.Pub2.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                //tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.RegularExpression://正则表达式,对数据控件处理
                                TextBox tbExp = this.Pub2.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + i);
                                if (tbExp == null || me.Tag == "onsubmit")
                                    continue;
                                //验证输入的正则格式
                                string regFilter = me.Doc;
                                if (regFilter.LastIndexOf("/g") < 0 && regFilter.LastIndexOf('/') < 0)
                                    regFilter = "'" + regFilter + "'";
                                //处理事件
                                tbExp.Attributes.Add("" + me.Tag + "", "return txtTest_Onkeyup(this," + regFilter + ",'" + me.Tag1 + "')");//[me.Tag] += "this.value=this.value.replace(" + regFilter + ",'')";
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            #endregion 拓展属性

            Button btn = new Button();
            btn.Text = "执行发起";
            btn.ID = "Btn_Start";
            btn.Click += new EventHandler(btn_Send_Click);
            btn.OnClientClick = "return checkType()";
            this.Pub2.Add(btn);
            //#region 文件上传.
            //if (!IsPostBack)
            //{
            //    this.Pub2.AddFieldSet("通过Excel导入方式发起:<a href='/DataUser/BatchStartFlowTemplete/" + this.FK_Flow + ".xls'><img src='/WF/Img/FileType/xls.gif' />下载Excel模版</a>");
            //    this.Pub2.Add("文件名:");
            //    FileUpload fu = new FileUpload();
            //    fu.ID = "File1";
            //    this.Pub2.Add(fu);
            //    btn = new Button();
            //    btn.Text = "导入";
            //    btn.ID = "Btn_Imp";
            //    btn.Click += new EventHandler(btn_Upload_Click);
            //    this.Pub2.Add(btn);
            //    this.Pub2.AddFieldSetEnd();
            //}
            //#endregion 文件上传.
        }

        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            this.ShowTabIndex = 1;

            if (File1.HasFile)
            {
                //判断文件是否小于10Mb   
                if (File1.PostedFile.ContentLength < 10485760)
                {
                    try
                    {
                        string ext = System.IO.Path.GetExtension(File1.FileName).ToLower();
                        string fileName = Server.MapPath("~/DataUser/Temp/") + this.FK_Flow + this.Page.Session.SessionID + ext;

                        //上传文件并指定上传目录的路径. 
                        File1.PostedFile.SaveAs(fileName);

                        Flow flow = new Flow(this.FK_Flow);
                        string[] sfields = flow.BatchStartFields.Split(",".ToCharArray(),
                                                                      StringSplitOptions.RemoveEmptyEntries);
                        MapAttrs attrs = new MapAttrs(this.FK_MapData);
                        MapAttr attr;
                        Dictionary<int, MapAttr> fields = new Dictionary<int, MapAttr>();   //列号，字段
                        DataTable dt = new DataTable();   //excel中提取的数据，列名为attr.KeyOfEn
                        IWorkbook wb;
                        ISheet sheet;
                        IRow row, headRow;
                        DataRow dr;
                        ICell cell;
                        string cellValue = string.Empty;
                        Hashtable ht = null;
                        int tint;
                        float tfloat;
                        double tdouble;
                        DateTime tdt;
                        SysEnum en;
                        EntityNoName enn;
                        int lastRowNum = 0;

                        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                        {
                            if (ext.Equals(".xls"))
                                wb = new HSSFWorkbook(fs);
                            else
                                wb = new XSSFWorkbook(fs);

                            sheet = wb.GetSheetAt(0);

                            //检索行数不能少于3行，第1行为主标题行，第2行为字段标题行，以下为内容行
                            //固定模式，从第3行开始检索数据
                            if (sheet.LastRowNum < 3)
                                CloseAndException(wb, fs, "文件行数错误，不能低于3行！");

                            headRow = sheet.GetRow(1);

                            //检索标题行，获取所有的字段
                            foreach (ICell c in headRow.Cells)
                            {
                                cellValue = c.StringCellValue;

                                if (string.IsNullOrWhiteSpace(cellValue))
                                    continue;

                                attr = attrs.GetEntityByKey(MapAttrAttr.Name, cellValue) as MapAttr;

                                //未查到字段，或查到的字段不在设置的提取字段集合中，去掉
                                if (attr == null || sfields.Contains(attr.KeyOfEn) == false)
                                    continue;

                                fields.Add(c.ColumnIndex, attr);
                                dt.Columns.Add(attr.KeyOfEn);

                                switch (attr.LGType)
                                {
                                    case FieldTypeS.Enum:
                                        dt.Columns[dt.Columns.Count - 1].ExtendedProperties.Add("Source", new SysEnums(attr.UIBindKey));
                                        break;
                                    case FieldTypeS.FK:
                                        dt.Columns[dt.Columns.Count - 1].ExtendedProperties.Add("Source", attr.HisEntitiesNoName);
                                        break;
                                }
                            }

                            if (fields.Count == 0)
                                CloseAndException(wb, fs, "上传文件中，未检索到要提取的字段，请重新确认数据的正确性！");

                            //added by liuxc,2017-1-12，增加判断真实最后一行的逻辑
                            lastRowNum = sheet.LastRowNum;
                            bool isRowEmpty = false;

                            for (int r = 2; r <= sheet.LastRowNum; r++)
                            {
                                row = sheet.GetRow(r);

                                if (row == null) continue;

                                isRowEmpty = true;

                                foreach (KeyValuePair<int, MapAttr> field in fields)
                                {
                                    cell = row.GetCell(field.Key);

                                    if (cell == null)
                                    {
                                        if (field.Value.UIIsInput)
                                            CloseAndException(wb, fs,
                                                                 string.Format("{0}，数据不能为空，请填写！", RptExportTemplateCell.GetCellName(field.Key, r)));
                                        continue;
                                    }

                                    cellValue = GetCellValue(cell, cell.CellType, field.Value.MyDataType);

                                    if(!string.IsNullOrWhiteSpace(cellValue))
                                    {
                                        isRowEmpty = false;
                                        break;
                                    }
                                }

                                if(isRowEmpty)
                                {
                                    lastRowNum = r;
                                    break;
                                }
                            }

                            //遍历行，提取数据，存于dt中
                            //提取过程中，进行数据有效性验证，不通过验证直接退出遍历，提示错误
                            for (int r = 2; r <= lastRowNum; r++)
                            {
                                row = sheet.GetRow(r);

                                if (row == null) continue;

                                dr = dt.NewRow();

                                foreach (KeyValuePair<int, MapAttr> field in fields)
                                {
                                    cell = row.GetCell(field.Key);

                                    if (cell == null)
                                    {
                                        if (field.Value.UIIsInput)
                                            CloseAndException(wb, fs,
                                                                 string.Format("{0}，数据不能为空，请填写！", RptExportTemplateCell.GetCellName(field.Key, r)));
                                        continue;
                                    }

                                    cellValue = GetCellValue(cell, cell.CellType, field.Value.MyDataType);

                                    if (cell.CellType == CellType.Error || cell.CellType == CellType.Unknown)
                                        CloseAndException(wb, fs,
                                                          string.Format("{0}，数据错误，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                    if (field.Value.UIIsInput && string.IsNullOrWhiteSpace(cellValue))
                                        CloseAndException(wb, fs,
                                                             string.Format("{0}，数据不能为空，请填写！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                    switch (field.Value.LGType)
                                    {
                                        case FieldTypeS.Normal:
                                            //再判断数据格式
                                            switch (field.Value.MyDataType)
                                            {
                                                case DataType.AppInt:
                                                    if (int.TryParse(cellValue, out tint) == false)
                                                        CloseAndException(wb, fs, string.Format("{0}，数据不为整数，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                                    dr[field.Value.KeyOfEn] = tint;
                                                    break;
                                                case DataType.AppFloat:
                                                case DataType.AppMoney:
                                                    if (float.TryParse(cellValue, out tfloat) == false)
                                                        CloseAndException(wb, fs, string.Format("{0}，数据不为Float，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                                    dr[field.Value.KeyOfEn] = tfloat;
                                                    break;
                                                case DataType.AppDouble:
                                                    if (double.TryParse(cellValue, out tdouble) == false)
                                                        CloseAndException(wb, fs, string.Format("{0}，数据不为Double，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                                    dr[field.Value.KeyOfEn] = tdouble;
                                                    break;
                                                case DataType.AppDate:
                                                    if (DateTime.TryParse(cellValue, out tdt) == false)
                                                        CloseAndException(wb, fs, string.Format("{0}，数据不为日期类型，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                                    dr[field.Value.KeyOfEn] = tdt.Date;
                                                    break;
                                                case DataType.AppDateTime:
                                                    if (DateTime.TryParse(cellValue, out tdt) == false)
                                                        CloseAndException(wb, fs, string.Format("{0}，数据不为日期时间类型，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                                    dr[field.Value.KeyOfEn] = tdt;
                                                    break;
                                                case DataType.AppBoolean:
                                                    if (new[] { "是", "否" }.Contains(cellValue) == false)
                                                        CloseAndException(wb, fs, string.Format("{0}，数据不为是/否，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                                    dr[field.Value.KeyOfEn] = cellValue == "是";
                                                    break;
                                                default:
                                                    dr[field.Value.KeyOfEn] = cellValue;
                                                    break;
                                            }
                                            break;
                                        case FieldTypeS.Enum:
                                            en = (dt.Columns[field.Value.KeyOfEn].ExtendedProperties["Source"] as SysEnums).GetEntityByKey(SysEnumAttr.Lab, cellValue) as SysEnum;

                                            if (en == null)
                                                CloseAndException(wb, fs, string.Format("{0}，数据不是有效值，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                            dr[field.Value.KeyOfEn] = en.IntKey;
                                            break;
                                        case FieldTypeS.FK:
                                            enn = (dt.Columns[field.Value.KeyOfEn].ExtendedProperties["Source"] as EntitiesNoName).GetEntityByKey("Name", cellValue) as EntityNoName;

                                            if (enn == null)
                                                CloseAndException(wb, fs, string.Format("{0}，数据不是有效值，请修改！", RptExportTemplateCell.GetCellName(cell.ColumnIndex, r)));

                                            dr[field.Value.KeyOfEn] = enn.No;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                dt.Rows.Add(dr);
                            }

                            wb.Close();
                        }

                        //执行遍历，发起所有流程
                        Flow fl = new Flow(this.FK_Flow);
                        this.Page.Title = fl.Name;
                        string infos = "";
                        Int64 workid;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                            ht = new Hashtable();

                            foreach (DataColumn col in dt.Columns)
                            {
                                ht.Add(col.ColumnName, dt.Rows[i][col.ColumnName]);
                            }

                            // 开始发起流程.
                            string info = string.Empty;

                            try
                            {
                                info = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, workid, ht).ToMsgOfHtml();
                                infos += "<br><fieldset width='100%' ><legend>&nbsp;&nbsp;第 (" + i + ") 条工作启动成功&nbsp;</legend>";
                                infos += info;
                            }
                            catch (Exception ex)
                            {
                                infos += "<br><fieldset width='100%' ><legend>&nbsp;&nbsp;第 (" + i + ") 条工作启动失败&nbsp;</legend>";
                                infos += "<b>" + ex.Message + "</b>";
                            }
                            finally
                            {
                                infos += "</fieldset>";
                            }
                        }

                        this.Pub1.Clear();
                        this.Pub1.AddH2("&nbsp;&nbsp;发起信息");
                        infos = infos.Replace("@@", "@");
                        this.Pub1.Add(infos.Replace("@", "<br>@"));
                        this.Pub1.AddBR();
                        this.Pub1.AddBR();
                    }
                    catch (Exception ex)
                    {
                        BP.Sys.PubClass.Alert(ex.Message);
                    }
                }
                else
                {
                    BP.Sys.PubClass.Alert("请选择xls文件");
                }
            }
            else
            {
                BP.Sys.PubClass.Alert("尚未选择文件!");
            }
        }

        /// <summary>
        /// 关闭EXCEL流并报异常
        /// </summary>
        /// <param name="wb"></param>
        /// <param name="fs"></param>
        /// <param name="exMsg"></param>
        private void CloseAndException(IWorkbook wb, FileStream fs, string exMsg)
        {
            wb.Close();
            fs.Close();
            throw new Exception(exMsg);
        }

        void btn_Send_Click(object sender, EventArgs e)
        {
            Flow fl = new Flow(this.FK_Flow);
            this.Page.Title = fl.Name;
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.FK_Flow + "01"));
            string[] strs = fl.BatchStartFields.Split(',');
            string infos = "";
            this.ShowTabIndex = 0;
            for (int i = 1; i <= 12; i++)
            {
                CheckBox mycb = this.Pub2.GetCBByID("CB_IDX_" + i);
                if (mycb.Checked == false)
                    continue;

                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                Hashtable ht = new Hashtable();
                #region 给属性赋值.
                bool isChange = false;
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;
                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                            continue;

                        if (attr.LGType == FieldTypeS.Normal)
                        {
                            TB tb = this.Pub2.GetTBByID("TB_" + attr.KeyOfEn + "_" + i);
                            if (tb != null)
                            {
                                if (tb.Text != attr.DefVal)
                                    isChange = true;

                                ht.Add(str, tb.Text);
                                continue;
                            }

                            CheckBox cb = this.Pub2.GetCBByID("CB_" + attr.KeyOfEn + "_" + i);
                            if (cb != null)
                            {
                                if (cb.Checked != attr.DefValOfBool)
                                    isChange = true;

                                if (cb.Checked)
                                    ht.Add(str, 1);
                                else
                                    ht.Add(str, 0);
                                continue;
                            }
                        }
                        else
                        {
                            DDL ddl = this.Pub2.GetDDLByID("DDL_" + attr.KeyOfEn + "_" + i);
                            if (ddl != null)
                            {
                                if (ddl.SelectedItemStringVal != attr.DefVal)
                                    isChange = true;
                                if (attr.LGType == FieldTypeS.Enum)
                                    ht.Add(str, ddl.SelectedItemIntVal);
                                else
                                    ht.Add(str, ddl.SelectedItemStringVal);
                                continue;
                            }
                        }
                    }
                }
                #endregion 给属性赋值.

                #region 开始发起流程.
                if (isChange == false)
                    continue;

                string info = string.Empty;

                try
                {
                    info = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, workid, ht).ToMsgOfHtml();
                    infos += "<br><fieldset width='100%' ><legend>&nbsp;&nbsp;第 (" + i + ") 条工作启动成功&nbsp;</legend>";
                    infos += info;
                }
                catch (Exception ex)
                {
                    infos += "<br><fieldset width='100%' ><legend>&nbsp;&nbsp;第 (" + i + ") 条工作启动失败&nbsp;</legend>";
                    infos += "<b>" + ex.Message + "</b>";
                }
                finally
                {
                    infos += "</fieldset>";
                }

                #endregion 开始发起流程.
            }
            this.Pub2.Clear();

            this.Pub2.AddH2("&nbsp;&nbsp;发起信息");
            infos = infos.Replace("@@", "@");
            this.Pub2.Add(infos.Replace("@", "<br>@"));
            this.Pub2.AddBR();
            this.Pub2.AddSpace(2);

            Button btn = new Button();
            btn.Text = "继续发起";
            btn.ID = "Btn_Continue";
            btn.OnClientClick = "location.href='BatchStart.aspx?t=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff") +
                                "'";
            this.Pub2.Add(btn);
        }

        /// <summary>
        /// 获取单元格值的字符串形式
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <param name="cellType">单元格值类型</param>
        /// <returns></returns>
        private string GetCellValue(ICell cell, CellType cellType, int dataType)
        {
            string s = string.Empty;

            switch (cellType)
            {
                case CellType.Blank:
                    s = string.Empty;
                    break;
                case CellType.Boolean:
                    s = cell.BooleanCellValue.ToString();
                    break;
                case CellType.Error:
                    s = "ERROR";
                    break;
                case CellType.Formula:
                    s = GetCellValue(cell, cell.CachedFormulaResultType, dataType);
                    break;
                case CellType.Numeric:
                    switch (dataType)
                    {
                        case DataType.AppDate:
                            s = cell.DateCellValue.Date.ToString("yyyy-MM-dd");
                            break;
                        case DataType.AppDateTime:
                            s = cell.DateCellValue.Date.ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        default:
                            s = cell.NumericCellValue.ToString();
                            break;
                    }
                    break;
                case CellType.String:
                    s = (cell.StringCellValue ?? string.Empty).Replace("\n", "");
                    break;
                case CellType.Unknown:
                    s = "UNKNOWN";
                    break;
            }

            return s;
        }
    }

    /// <summary>
    /// 系统类扩展辅助
    /// </summary>
    public static class EnhanceHelper
    {
        /// <summary>
        /// 自定义扩展：判断元素数组中是否含有指定元素
        /// <para></para>
        /// <para>added by liuxc,2016-10-13</para>
        /// </summary>
        /// <param name="arr">元素数组</param>
        /// <param name="element">查找元素</param>
        /// <returns></returns>
        public static bool Contains<T>(this T[] arr, T element) where T : class
        {
            if (arr == null) return false;

            foreach (T ele in arr)
            {
                if (Equals(ele, element))
                    return true;
            }

            return false;
        }
    }
}