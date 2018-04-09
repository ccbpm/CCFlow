using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_BatchSmall : BP.Web.WebPage
    {
        #region 属性.
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return "ND" + this.FK_Node;
            }
        }

        #endregion 属性.

        /// <summary>
        /// 获取批量处理的节点
        /// </summary>
        public int BindNodeList()
        {
            string sql = "SELECT a.NodeID, a.Name,a.FlowName, COUNT(WorkID) AS NUM  FROM WF_Node a, WF_EmpWorks b WHERE A.NodeID=b.FK_Node AND B.FK_Emp='" + WebUser.No + "' AND b.WFState NOT IN (7) AND a.BatchRole!=0 GROUP BY A.NodeID, a.Name,a.FlowName ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (dt.Rows.Count == 0)
            {
                this.Pub1.AddCaptionMsg("批处理");
                this.Pub1.Add("<div style='margin-left:30px;margin-top:20px;font-size:14px;' ><img src='Img/info.png' align='middle' />当前没有批处理的工作......</div>");
                return 0;
            }

            this.Pub1.AddTable("width=100%");
            this.Pub1.AddCaptionMsg("批处理");

            if (dt.Rows.Count == 0)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDBegin();

                this.Pub1.Add("<img src='Img/info.png' align='middle' />当前没有批处理的工作.");

                this.Pub1.AddTDEnd();
                this.Pub1.AddTREnd();
                this.Pub1.AddTableEnd();
                return 0;
            }

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin();

            this.Pub1.AddBR();
            this.Pub1.AddUL();
            foreach (DataRow dr in dt.Rows)
            {
                this.Pub1.Add("<Li style='list-style-type:square; color:#959595;'><a href='Batch.aspx?FK_Node=" + dr["NodeID"]
                    + "'  style=\"text-decoration:none; font-size:14px; font-weight:normal;\">" + dr["FlowName"].ToString() + " --> " + dr["Name"].ToString() + "(" + dr["Num"] + ")" + "</a></Li>");
                this.Pub1.AddBR();
            }
            this.Pub1.AddULEnd();

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            return dt.Rows.Count;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["FK_Node"] == null)
            {
                // 如果没有接收到节点ID参数，就绑定当前人员可以执行批量审核的待办工作.
                int num = this.BindNodeList();
                return;
            }

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Flow fl = nd.HisFlow;
            string sql = "";

            if (nd.HisRunModel == RunModel.SubThread)
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                          + " a , WF_EmpWorks b WHERE a.OID=B.FID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                          + " AND b.FK_Emp='" + WebUser.No + "'";
            }
            else
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            }


            // string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (nd.HisBatchRole == BatchRole.None)
            {
                this.Pub1.AddFieldSetRed("错误", "节点(" + nd.Name + ")不能执行批量处理操作.");
                return;
            }

            string inSQL = "SELECT WorkID FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "' AND WFState!=7 AND FK_Node=" + this.FK_Node;
            Works wks = nd.HisWorks;
            wks.RetrieveInSQL(inSQL);

            BtnLab btnLab = new BtnLab(this.FK_Node);
            this.Pub1.AddTable("width='100%'");

            //移动按钮位置
            if (nd.HisBatchRole == BatchRole.Group)
                this.Pub1.AddCaptionMsgLong("<a href='Batch.aspx'>返回</a>&nbsp;&nbsp;<input  ID=\"btnGroup\" type=\"button\" value=\"合卷批复\" CssClass=\"Btn\" onclick=\"BatchGroup()\" />");
            else
                this.Pub1.AddCaptionMsgLong(nd.FlowName + " - <a href='Batch.aspx'>返回</a>");

            #region 生成标题.
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序号");
            string str1 = "<INPUT id='checkedAll' onclick='SelectAll()'  text='选择' value='选择' type='checkbox' name='checkedAll'>";
            this.Pub1.AddTDTitle("align='left'", str1 + "选择");
            this.Pub1.AddTDTitle("标题");
            this.Pub1.AddTDTitle("发起人");
            this.Pub1.AddTDTitle("接受日期");

            // 显示出来字段. BatchParas 的规则为 @字段中文名=fieldName@字段中文名1=fieldName1 
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            string[] strs = nd.BatchParas.Split(',');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str)
                    || str.Contains("@PFlowNo") == true)
                    continue;

                foreach (MapAttr attr in attrs)
                {
                    if (str != attr.KeyOfEn)
                        continue;
                    this.Pub1.AddTDTitle(attr.Name);
                }
            }
            this.Pub1.AddTREnd();
            #endregion 生成标题.

            GERpt rpt = nd.HisFlow.HisGERpt;
            bool is1 = false;
            int idx = 0;
            foreach (Work wk in wks)
            {
                idx++;
                if (idx == nd.BatchListCount)
                    break;

                #region 显示必要的列.
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(idx);
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + wk.OID.ToString();
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["WorkID"].ToString() != wk.OID.ToString())
                        continue;
                    cb.Text = "选择";
                    this.Pub1.AddTD(cb);

                    //this.Pub1.AddTD("<a href=\"javascript:WinOpen('MyFlow.aspx?WorkID=" + wk.OID + "&FK_Node=" + this.FK_Node + "&FK_Flow="+nd.FK_Flow+"','s')\" >" + dr["Title"].ToString() + "</a>");
                    this.Pub1.AddTD("<a href=\"javascript:WinOpen('FlowFormTree/Default.aspx?WorkID=" + wk.OID + "&FK_Node=" + this.FK_Node + "&IsSend=0&FK_Flow=" + nd.FK_Flow + "','s')\" >" + dr["Title"].ToString() + "</a>");
                    this.Pub1.AddTD(dr["Starter"].ToString());
                    this.Pub1.AddTD(dr["ADT"].ToString());
                    break;
                }
                #endregion 显示必要的列.

                #region 显示出来自定义的字段数据..
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) || str.Contains("@PFlowNo") == true)
                        continue;
                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                            continue;

                        TB tb = new TB();
                        tb.ID = "TB_" + attr.KeyOfEn + "_" + wk.OID;
                        switch (attr.LGType)
                        {
                            case FieldTypeS.Normal:
                                switch (attr.MyDataType)
                                {
                                    case BP.DA.DataType.AppString:
                                        if (attr.UIRows == 1)
                                        {
                                            tb.Text = wk.GetValStringByKey(attr.KeyOfEn);
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
                                            tb.Text = wk.GetValStringByKey(attr.KeyOfEn);
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
                                        tb.Text = wk.GetValStrByKey(attr.KeyOfEn);

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
                                        tb.Text = wk.GetValStrByKey(attr.KeyOfEn); // en.GetValStrByKey(attr.KeyOfEn);

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
                                        cb = new CheckBox();
                                        //cb.Width = 350;
                                        cb.Text = attr.Name;
                                        cb.ID = "CB_" + attr.KeyOfEn + "_" + wk.OID;
                                        cb.Checked = attr.DefValOfBool;
                                        cb.Enabled = attr.UIIsEnable;
                                        cb.Checked = wk.GetValBooleanByKey(attr.KeyOfEn);

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
                                        tb.Text = wk.GetValIntByKey(attr.KeyOfEn).ToString("0.00");

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
                                    ddle.ID = "DDL_" + attr.KeyOfEn + "_" + wk.OID;
                                    ddle.BindSysEnum(attr.UIBindKey);
                                    ddle.SetSelectItem(wk.GetValIntByKey(attr.KeyOfEn));
                                    ddle.Enabled = attr.UIIsEnable;
                                    ddle.Attributes["tabindex"] = attr.Idx.ToString();
                                    if (attr.UIIsEnable)
                                    {
                                        //add by dgq 2013-4-9,添加内容修改后的事件
                                        ddle.Attributes["onchange"] = "Change('" + attr.FK_MapData + "')";
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
                                ddl1.ID = "DDL_" + attr.KeyOfEn + "_" + wk.OID;
                                ddl1.Attributes["tabindex"] = attr.Idx.ToString();
                                if (ddl1.Enabled)
                                {
                                    EntitiesNoName ens = attr.HisEntitiesNoName;
                                    ens.RetrieveAll();
                                    ddl1.BindEntities(ens);
                                    ddl1.SetSelectItem(wk.GetValStrByKey(attr.KeyOfEn));
                                }
                                else
                                {
                                    ddl1.Attributes["style"] = "width: " + attr.UIWidth + "px;height: 19px;";
                                    if (ddl1.Enabled == true)
                                        ddl1.Enabled = false;
                                    ddl1.Attributes["Width"] = attr.UIWidth.ToString();
                                    ddl1.Items.Add(new ListItem(wk.GetValRefTextByKey(attr.KeyOfEn), wk.GetValStrByKey(attr.KeyOfEn)));
                                }
                                ddl1.Enabled = attr.UIIsEnable;
                                this.Pub1.AddTD(ddl1);
                                break;
                            default:
                                break;
                        }
                    }
                }
                #endregion 显示出来自定义的字段数据..

                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEndWithHR();

            MapExts mes = new MapExts(this.FK_MapData);

            #region 处理扩展属性.
            if (mes.Count != 0)
            {
                this.Page.RegisterClientScriptBlock("s81",
              "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Scripts/jquery-1.4.1.min.js' ></script>");
                this.Page.RegisterClientScriptBlock("b81",
             "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/CCForm/MapExt.js' defer='defer' type='text/javascript' ></script>");
                this.Pub1.Add("<div id='divinfo' style='width: 155px; position: absolute; color: Lime; display: none;cursor: pointer;align:left'></div>");
                this.Page.RegisterClientScriptBlock("dCd",
    "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "DataUser/JSLibData/" + this.FK_MapData + ".js' ></script>");

                foreach (Work wk in wks)
                {
                    foreach (MapExt me in mes)
                    {
                        switch (me.ExtType)
                        {
                            case MapExtXmlList.DDLFullCtrl: // 自动填充.
                                DDL ddlOper = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + wk.OID);
                                if (ddlOper == null)
                                    continue;
                                ddlOper.Attributes["onchange"] = "DDLFullCtrl(this.value,\'" + ddlOper.ClientID + "\', \'" + me.MyPK + "\')";
                                break;
                            case MapExtXmlList.ActiveDDL:
                                DDL ddlPerant = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + wk.OID);
                                string val, valC;
                                if (ddlPerant == null || wk.OID < 100)
                                    continue;

#warning 此处需要优化
                                string ddlC = "ContentPlaceHolder1_Batch1_DDL_" + me.AttrsOfActive + "_" + wk.OID;
                                //  ddlPerant.Attributes["onchange"] = " isChange=true; DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";
                                ddlPerant.Attributes["onchange"] = "DDLAnsc(this.value, \'" + ddlC + "\', \'" + me.MyPK + "\')";

                                DDL ddlChild = this.Pub1.GetDDLByID("DDL_" + me.AttrsOfActive + "_" + wk.OID);
                                val = ddlPerant.SelectedItemStringVal;
                                if (ddlChild.Items.Count == 0)
                                    valC = wk.GetValStrByKey(me.AttrsOfActive);
                                else
                                    valC = ddlChild.SelectedItemStringVal;

                                string mysql = me.Doc.Replace("@Key", val);
                                if (mysql.Contains("@"))
                                {
                                    mysql = BP.WF.Glo.DealExp(mysql, wk, null);
                                }

                                ddlChild.Bind(DBAccess.RunSQLReturnTable(mysql), "No", "Name");
                                if (ddlChild.SetSelectItem(valC) == false)
                                {
                                    ddlChild.Items.Insert(0, new ListItem("请选择" + valC, valC));
                                    ddlChild.SelectedIndex = 0;
                                }
                                break;
                            case MapExtXmlList.AutoFullDLL: //自动填充下拉框的范围.
                                DDL ddlFull = this.Pub1.GetDDLByID("DDL_" + me.AttrOfOper + "_" + wk.OID);
                                if (ddlFull == null)
                                    continue;

                                string valOld = wk.GetValStrByKey(me.AttrOfOper);

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
                                TextBox tbAuto = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + wk.OID);
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
                                        DDL ddlC1 = this.Pub1.GetDDLByID("DDL_" + ctlID + "_" + wk.OID);
                                        if (ddlC1 == null)
                                        {
                                            //me.Tag = "";
                                            // me.Update();
                                            continue;
                                        }
                                        sql = myCtl[1].Replace("~", "'");
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
                                TextBox tbCheck = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + wk.OID);
                                if (tbCheck != null)
                                    tbCheck.Attributes[me.Tag2] += " rowPK=" + wk.OID + "; " + me.Tag1 + "(this);";
                                break;
                            case MapExtXmlList.PopVal: //弹出窗.
                                TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + wk.OID);
                                //  tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                tb.Attributes["ondblclick"] = " ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.Link: // 超链接.
                                //TB tb = this.Pub1.GetTBByID("TB_" + me.AttrOfOper + "_" + mydtl.OID);
                                //tb.Attributes["ondblclick"] = " isChange=true; ReturnVal(this,'" + me.Doc + "','sd');";
                                break;
                            case MapExtXmlList.RegularExpression://正则表达式,对数据控件处理
                                TextBox tbExp = this.Pub1.GetTextBoxByID("TB_" + me.AttrOfOper + "_" + wk.OID);
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
            if (nd.HisBatchRole == BatchRole.Ordinary)
            {
                /*如果普通的批处理.*/
                btn.CssClass = "Btn";
                btn.ID = "Btn_Send";
                if (nd.BatchParas_IsSelfUrl == true)
                {
                    btn.Text = "选择要批量处理的工作";
                }
                else
                {
                    btn.Text = "批量处理:" + btnLab.SendLab;
                }
                btn.Click += new EventHandler(btn_Send_Click);
                btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
                this.Pub1.Add(btn);
                if (btnLab.ReturnEnable == false)
                {
                    btn = new Button();
                    btn.CssClass = "Btn";
                    btn.ID = "Btn_Return";
                    btn.Text = "批量处理:" + btnLab.ReturnEnable;
                    btn.Click += new EventHandler(btnDelete_Return_Click);
                    btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
                    this.Pub1.Add(btn);
                }
            }

            if (nd.HisBatchRole == BatchRole.Group)
            {
                /*如果分组审核？*/
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_Group";
                //btn.Text = btnLab.SendLab;
                btn.Text = "合卷批复";
                btn.Click += new EventHandler(btn_Group_Click);
                btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
                this.Pub1.Add(btn);
            }

            if (btnLab.ReturnEnable == false)
            {
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_Return";
                btn.Text = "批量处理:" + btnLab.ReturnEnable;
                btn.Click += new EventHandler(btnDelete_Return_Click);
                btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
                this.Pub1.Add(btn);
            }

            if (btnLab.DeleteEnable != 0)
            {
                btn = new Button();
                btn.CssClass = "Btn";
                btn.ID = "Btn_Del";
                btn.Text = "批量处理:" + btnLab.DeleteLab;
                btn.Click += new EventHandler(btnDelete_Click);
                btn.Attributes["onclick"] = " return confirm('您确定要执行吗？');";
                this.Pub1.Add(btn);
            }
        }

        #region  批量分组启动子流程
        void btn_Group_Click(object sender, EventArgs e)
        {
            string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string[] strs = nd.BatchParas.Split(',');
            MapAttrs attrs = new MapAttrs(this.FK_MapData);

            string ids = "";
            int idx = -1;
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                CheckBox cb = this.Pub1.GetCBByID("CB_" + workid);
                if (cb == null || cb.Checked == false)
                    continue;

                idx++;
                if (idx == nd.BatchListCount)
                    break;

                ids += workid + ",";
                Hashtable ht = new Hashtable();

                #region 给属性赋值.
                //bool isChange = false;
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
                            TB tb = this.Pub1.GetTBByID("TB_" + attr.KeyOfEn + "_" + workid);
                            if (tb != null)
                            {
                                //if (tb.Text != attr.DefVal)
                                //    isChange = true;

                                ht.Add(str, tb.Text);
                                continue;
                            }

                            cb = this.Pub1.GetCBByID("CB_" + attr.KeyOfEn + "_" + workid);
                            if (cb != null)
                            {
                                //if (cb.Checked != attr.DefValOfBool)
                                //    isChange = true;

                                if (cb.Checked)
                                    ht.Add(str, 1);
                                else
                                    ht.Add(str, 0);
                                continue;
                            }
                        }
                        else
                        {
                            DDL ddl = this.Pub1.GetDDLByID("DDL_" + attr.KeyOfEn + "_" + workid);
                            if (ddl != null)
                            {
                                //if (ddl.SelectedItemStringVal != attr.DefVal)
                                //    isChange = true;
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

                //执行保存.
                BP.WF.Dev2Interface.Node_SaveWork(nd.FK_Flow, this.FK_Node, workid, ht);
            }

            if (ids == "")
            {
                this.Alert("您没有选择工作.");
            }
            else
            {
                string[] paras = nd.BatchParas.Split(',');
                string[] mystrs = paras[0].Split('=');
                if (mystrs.Length == 2)
                {
                    string flowNo = mystrs[1];



                    // BP.Sys.PubClass.WinOpen("MyFlow.aspx?FK_Flow=" + flowNo + "&FK_Node=" + flowNo + "01&DoFunc=SetParentFlow&CFlowNo=" + nd.FK_Flow + "&WorkIDs=" + ids, 1000, 900);
                    //this.Response.Redirect("MyFlow.aspx?FK_Flow=" + flowNo + "&FK_Node=" + flowNo + "01&DoFunc=SetParentFlow&CFlowNo=" + nd.FK_Flow + "&WorkIDs=" + ids, true);
                    string url = "MyFlow.aspx?FK_Flow=" + flowNo + "&FK_Node=" + flowNo + "01&DoFunc=SetParentFlow&CFlowNo=" + nd.FK_Flow + "&WorkIDs=" + ids;
                    WinOpen(url);
                }
                else
                {
                    this.Alert("您没有指定流程父节点.");
                    return;
                }
            }
        }
        #endregion  批量分组启动子流程

        #region  批量发送
        //发送
        void btn_Send_Click(object sender, EventArgs e)
        {
            //string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            string sql = string.Format("SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}'", WebUser.No, this.FK_Node);

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string[] strs = nd.BatchParas.Split(',');
            MapAttrs attrs = new MapAttrs(this.FK_MapData);

            string msg = "";
            int idx = -1;
            string workids = null;
            if (nd.BatchParas_IsSelfUrl == true)
                workids = "";

            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                if (idx == nd.BatchListCount)
                    break;

                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                CheckBox cb = this.Pub1.GetCBByID("CB_" + workid);
                if (cb == null || cb.Checked == false)
                    continue;

                // 如果是自定义的,就记录workids, 让其转到
                if (nd.BatchParas_IsSelfUrl == true)
                {
                    workids += "," + workid;
                    continue;
                }

                Hashtable ht = new Hashtable();

                #region 给属性赋值.
                //bool isChange = false;
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
                            TB tb = this.Pub1.GetTBByID("TB_" + attr.KeyOfEn + "_" + workid);
                            if (tb != null)
                            {
                                //if (tb.Text != attr.DefVal)
                                //    isChange = true;

                                ht.Add(str, tb.Text);
                                continue;
                            }

                            cb = this.Pub1.GetCBByID("CB_" + attr.KeyOfEn + "_" + workid);
                            if (cb != null)
                            {
                                //if (cb.Checked != attr.DefValOfBool)
                                //    isChange = true;

                                if (cb.Checked)
                                    ht.Add(str, 1);
                                else
                                    ht.Add(str, 0);
                                continue;
                            }
                        }
                        else
                        {
                            DDL ddl = this.Pub1.GetDDLByID("DDL_" + attr.KeyOfEn + "_" + workid);
                            if (ddl != null)
                            {
                                //if (ddl.SelectedItemStringVal != attr.DefVal)
                                //    isChange = true;
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

                msg += "<fieldset>";
                msg += "<legend>@对工作(" + dr["Title"] + ")处理情况如下。</legend>";
                BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(nd.FK_Flow, workid, ht);
                msg += objs.ToMsgOfHtml();
                msg += "</fieldset>";
            }

            if (nd.BatchParas_IsSelfUrl == true)
            {
                if (workids == "")
                    this.Alert("您没有选择工作.");
                else
                    this.Response.Redirect("BatchSelfDeal.aspx?FK_Node=" + this.FK_Node + "&FK_Flow=" + nd.FK_Flow + "&WorkIDs=" + workids, true);
                return;
            }

            if (msg == "")
            {
                this.Alert("您没有选择工作.");
            }
            else
            {
                this.Pub1.Clear();
                msg += "<a href='Batch.aspx'>返回...</a>";
                this.Pub1.AddMsgOfInfo("批量处理信息", msg);
            }
        }
        #endregion

        #region 批量删除
        void btnDelete_Click(object sender, EventArgs e)
        {
            string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                Int64 fid = Int64.Parse(dr["FID"].ToString());
                CheckBox cb = this.Pub1.GetCBByID("CB_" + workid);
                if (cb == null || cb.Checked == false)
                    continue;

                msg += "@对工作(" + dr["Title"] + ")处理情况如下。<br>";
                string mes = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(nd.FK_Flow, workid, "批量退回", true);
                msg += mes;
                msg += "<hr>";
            }

            if (msg == "")
            {
                this.Alert("您没有选择工作.");
            }
            else
            {
                this.Pub1.Clear();
                msg += "<a href='Batch.aspx'>返回...</a>";
                this.Pub1.AddMsgOfInfo("批量处理信息", msg);
            }
        }
        #endregion 批量删除


        //批量退回
        void btnDelete_Return_Click(object sender, EventArgs e)
        {
            string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            string msg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                Int64 fid = Int64.Parse(dr["FID"].ToString());
                CheckBox cb = this.Pub1.GetCBByID("CB_" + workid);
                if (cb == null || cb.Checked == false)
                    continue;

                msg += "@对工作(" + dr["Title"] + ")处理情况如下。<br>";
                BP.WF.SendReturnObjs objs = null;// BP.WF.Dev2Interface.Node_ReturnWork(nd.FK_Flow, workid,fid,this.FK_Node,"批量退回");
                msg += objs.ToMsgOfHtml();
                msg += "<hr>";
            }

            if (msg == "")
            {
                this.Alert("您没有选择工作.");
            }
            else
            {
                this.Pub1.Clear();
                msg += "<a href='Batch.aspx'>返回...</a>";
                this.Pub1.AddMsgOfInfo("批量处理信息", msg);
            }
        }

        protected void btnGroup_Click(object sender, EventArgs e)
        {
            btn_Group_Click(sender, e);
        }

    }
}