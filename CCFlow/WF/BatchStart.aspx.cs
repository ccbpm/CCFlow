using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
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
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, WebUser.No) == false)
            {
                
            }

            Flow fl = new Flow(this.FK_Flow);
            this.Page.Title = fl.Name;
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            if (fl.BatchStartFields.Length == 0)
            {
                this.Pub1.AddFieldSet("流程属性设置错误");
                this.Pub1.Add("您需要在流程属性里设置批量发起需要填写的字段。");
                this.Pub1.AddFieldSetEnd();
            }

            MapExts mes = new MapExts(this.FK_MapData);

            BP.WF.Node nd = new BP.WF.Node(int.Parse(this.FK_Flow + "01"));
            Work wk = nd.HisWork;
            wk.ResetDefaultVal();

            this.Pub1.AddTable();
            this.Pub1.AddCaptionMsg("批量发起:" + fl.Name);

            #region 输出标题.
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");

            string str1 = "<INPUT id='checkedAll' onclick=\"SelectAllBS(this);\" value='选择' type='checkbox' name='checkedAll' >全部选择";
            this.Pub1.AddTDTitle("align='left'", str1);

            //this.Pub1.AddTDTitle("align='left'", "");

            string[] strs = fl.BatchStartFields.Split(',');
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                foreach (MapAttr attr in attrs)
                {
                    if (str != attr.KeyOfEn)
                        continue;
                    this.Pub1.AddTDTitle(attr.Name);
                }
            }
            this.Pub1.AddTREnd();
            #endregion 输出标题.

            #region 输出标题.
            for (int i = 1; i <= this.RowNum; i++)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(i);
                CheckBox cbIdx = new CheckBox();
                cbIdx.Checked = false;
                cbIdx.Text = "发起否?";
                cbIdx.ID = "CB_IDX_" + i;
                this.Pub1.AddTD(cbIdx);

                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
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
                                            this.Pub1.AddTD(tb);
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

                                            this.Pub1.AddTD(tb);
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
                                        this.Pub1.AddTD(tb);
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
                                        this.Pub1.AddTD(tb);
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
                                        this.Pub1.AddTD(cb);
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

                                        this.Pub1.AddTD(tb);
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

                                        this.Pub1.AddTD(tb);
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
                                        this.Pub1.AddTD(tb);
                                        break;
                                    case BP.DA.DataType.AppRate:
                                        if (attr.UIIsEnable)
                                            tb.Attributes["class"] = "TBNum";
                                        else
                                            tb.Attributes["class"] = "TBReadonly";
                                        tb.ShowType = TBType.Moneny;
                                        tb.Text = attr.DefVal;
                                        tb.Attributes["style"] = "width: " + attr.GetValStrByKey("UIWidth") + "px; text-align: right; height: 19px;";
                                        this.Pub1.AddTD(tb);
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
                                    this.Pub1.AddTD(ddle);
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
                                this.Pub1.AddTD(ddl1);
                                break;
                            default:
                                break;
                        }
                    }
                }
                this.Pub1.AddTREnd();
            }
            #endregion 输出标题.

            this.Pub1.AddTableEnd();

            #region 处理扩展属性.
            if (mes.Count != 0)
            {
                this.Page.RegisterClientScriptBlock("s81",
              "<script language='JavaScript' src='/WF/Scripts/jquery-1.4.1.min.js' ></script>");
                this.Page.RegisterClientScriptBlock("b81",
             "<script language='JavaScript' src='/WF/CCForm/MapExt.js' defer='defer' type='text/javascript' ></script>");
                this.Pub1.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");
                this.Page.RegisterClientScriptBlock("dCd",
    "<script language='JavaScript' src='/DataUser/JSLibData/" + this.FK_MapData + ".js' ></script>");

                for (int i = 1; i <= this.RowNum; i++)
                {
                    foreach (MapExt me in mes)
                    {
                        switch (me.ExtType)
                        {
                            case MapExtXmlList.DDLFullCtrl: // 自动填充.
                                DDL ddlOper = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + i);
                                if (ddlOper == null)
                                    continue;
                                ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                                break;
                            case MapExtXmlList.ActiveDDL:
                                DDL ddlPerant = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + i);
                                string val, valC;
                                DataTable dt;
                                if (ddlPerant == null)
                                    continue;
#warning 此处需要优化
                                string ddlC = "ContentPlaceHolder1_BatchStart1_DDL_" + me.AttrsOfActive + "_" + i;
                                //  ddlPerant.Attributes["onchange"] = " isChange=true; DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";
                                ddlPerant.Attributes["onchange"] = "DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";

                                DDL ddlChild = this.Pub1.GetDDLByID("DDL_" + me.AttrsOfActive + "_" + i);
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
                                DDL ddlFull = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + i);
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
                                TextBox tbAuto = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + i);
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
                                        DDL ddlC1 = this.Pub1.GetDDLByID("DDL_" + ctlID + "_" + i);
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
                                TextBox tbCheck = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + i);
                                if (tbCheck != null)
                                    tbCheck.Attributes[me.Tag2] += " rowPK=" + i + "; " + me.Tag1 + "(this);";
                                break;
                            case MapExtXmlList.PopVal: //弹出窗.
                                TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + i);
                                //  tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                tb.Attributes["ondblclick"] = " ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.Link: // 超链接.
                                //TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                //tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.RegularExpression://正则表达式,对数据控件处理
                                TextBox tbExp = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + i);
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
            this.Pub1.Add(btn);

            #region 文件上传.
            this.Pub1.AddFieldSet("通过Excel导入方式发起:<a href='/DataUser/BatchStartFlowTemplete/" + this.FK_Flow + ".xls'><img src='/WF/Img/FileType/xls.gif' />下载Excel模版</a>");
            this.Pub1.Add("文件名:");
            FileUpload fu = new FileUpload();
            fu.ID = "File1";
            this.Pub1.Add(fu);
            btn = new Button();
            btn.Text = "导入";
            btn.ID = "Btn_Imp";
            btn.Click += new EventHandler(btn_Upload_Click);
            this.Pub1.Add(btn);
            this.Pub1.AddFieldSetEnd();
            #endregion 文件上传.
        }
        void btn_Upload_Click(object sender, EventArgs e)
        {
            FileUpload FileUpLoad1 = this.FindControl("File1") as FileUpload;
            if (FileUpLoad1.HasFile)
            {
                //判断文件是否小于10Mb   
                if (FileUpLoad1.PostedFile.ContentLength < 10485760)
                {
                    try
                    {
                        string fileName = Server.MapPath("~/DataUser/Temp/") + this.FK_Flow + this.Page.Session.SessionID + ".xls";

                        //上传文件并指定上传目录的路径. 
                        FileUpLoad1.PostedFile.SaveAs(fileName);

                        /*注意->这里为什么不是:FileUpLoad1.PostedFile.FileName  
                        * 而是:FileUpLoad1.FileName?  
                        * 前者是获得客户端完整限定(客户端完整路径)名称  
                        * 后者FileUpLoad1.FileName只获得文件名.  
                        */

                        //当然上传语句也可以这样写(貌似废话):   
                        //FileUpLoad1.SaveAs(@"D:\"+FileUpLoad1.FileName);   
                        //lblMessage.Text = "上传成功!";
                    }
                    catch (Exception ex)
                    {
                        BP.Sys.PubClass.Alert(ex.Message);
                        //lblMessage.Text = "出现异常,无法上传!";
                        //lblMessage.Text += ex.Message;   
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
                // lblMessage.Text = "尚未选择文件!";
            }
        }
        void btn_Send_Click(object sender, EventArgs e)
        {
            Flow fl = new Flow(this.FK_Flow);
            this.Page.Title = fl.Name;
            MapAttrs attrs = new MapAttrs("ND" + int.Parse(this.FK_Flow + "01"));
            string[] strs = fl.BatchStartFields.Split(',');
            string infos = "";
            for (int i = 1; i <= 12; i++)
            {
                CheckBox mycb = this.Pub1.GetCBByID("CB_IDX_" + i);
                if (mycb.Checked == false)
                    continue;

                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                Hashtable ht = new Hashtable();
                #region 给属性赋值.
                bool isChange = false;
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                            continue;

                        if (attr.LGType == FieldTypeS.Normal)
                        {
                            TB tb = this.Pub1.GetTBByID("TB_" + attr.KeyOfEn + "_" + i);
                            if (tb != null)
                            {
                                if (tb.Text != attr.DefVal)
                                    isChange = true;

                                ht.Add(str, tb.Text);
                                continue;
                            }

                            CheckBox cb = this.Pub1.GetCBByID("CB_" + attr.KeyOfEn + "_" + i);
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
                            DDL ddl = this.Pub1.GetDDLByID("DDL_" + attr.KeyOfEn + "_" + i);
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

                string info = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, workid, ht).ToMsgOfHtml();

                infos += "<br><fieldset width='100%' ><legend>&nbsp;&nbsp;第 (" + i + ") 条工作启动成功&nbsp;</legend>";
                infos += info;
                infos += "</fieldset>";

                #endregion 开始发起流程.
            }
            this.Pub1.Clear();

            this.Pub1.AddH2("&nbsp;&nbsp;发起信息");
            infos = infos.Replace("@@", "@");
            this.Pub1.Add(infos.Replace("@", "<br>@"));
        }

        
    }
}