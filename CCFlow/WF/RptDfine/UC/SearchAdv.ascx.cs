using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Web.Controls;
using BP.En;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.Port;
using BP.WF.Rpt;
using BP;

namespace CCFlow.WF.Rpt
{
    public partial class SearchAdv : BP.Web.UC.UCBase3
    {
        #region 属性.
        /// <summary>
        /// 编号名称
        /// </summary>
        public string RptNo
        {
            get
            {
                string s = this.Request.QueryString["RptNo"];
                if (DataType.IsNullOrEmpty(s))
                {
                    return "ND1MyRpt";// "ND68MyRpt";
                }
                return s;
            }
        }

        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (DataType.IsNullOrEmpty(s))
                    return "068";
                return s;
            }
        }

        public Entities _HisEns = null;
        public Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    if (this.RptNo != null)
                    {
                        if (this._HisEns == null)
                            _HisEns = BP.En.ClassFactory.GetEns(this.RptNo);
                    }
                }
                return _HisEns;
            }
        }

        public MapRpt currMapRpt = null;

        /// <summary>
        /// 当前的查询
        /// </summary>
        public string CurrentQuery { get; set; }

        /// <summary>
        /// 非Model的查询URL
        /// </summary>
        public string NormalSearchUrl { get; set; }

        /// <summary>
        /// 当前显示的查询PK
        /// </summary>
        public string CurrentUR_PK
        {
            get { return Request.QueryString["CurrentUR_PK"]; }
        }

        /// <summary>
        /// 标识当前是否处于查询界面
        /// </summary>
        public bool IsSearch
        {
            get
            {
                var isSearch = false;
                bool.TryParse(Request.QueryString["IsSearch"], out isSearch);
                return isSearch;
            }
        }

        /// <summary>
        /// 是否收缩结果界面
        /// </summary>
        public bool ResultCollapsed = true;

        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 处理查询权限， 此处不要修改，以Search.ascx为准.

            currMapRpt = new MapRpt(this.RptNo);
            Entity en = this.HisEns.GetNewEntity;
            Flow fl = new Flow(this.currMapRpt.FK_Flow);

            //DDL ddl = new DDL();
            //ddl.ID = "GoTo";
            //ddl.Items.Add(new ListItem("查询", "Search"));
            //ddl.Items.Add(new ListItem("高级查询", "SearchAdv"));
            //ddl.Items.Add(new ListItem("分组分析", "Group"));
            //ddl.Items.Add(new ListItem("交叉报表", "D3"));
            //ddl.Items.Add(new ListItem("对比分析", "Contrast"));
            //ddl.SetSelectItem(this.PageID);

            #endregion 处理查询权限

            this.NormalSearchUrl = string.Format("SearchAdv.aspx?RptNo={0}&FK_Flow={1}", this.RptNo, this.FK_Flow);

            # region // 绑定查询模版.
            UserRegedits urs = new UserRegedits();
            urs.Retrieve(UserRegeditAttr.FK_Emp, WebUser.No, UserRegeditAttr.CfgKey, this.RptNo + "_SearchAdv");

            //1.获取我的所有查询列表
            //NumKey保存查询的名称
            UserRegedit currUR = null;  //当前显示的查询
            var idx = 1;
            foreach (UserRegedit ur in urs)
            {
                if (ur.MyPK.Contains("SearchAdvModel"))
                {
                    Pub1.AddLi(
                        string.Format(
                            "<div><a href='SearchAdv.aspx?RptNo={0}&FK_Flow={1}&CurrentUR_PK={2}'><span class='nav' style='{5}'>{3}. {4}</span></a></div>",
                            this.RptNo, this.FK_Flow, ur.MyPK, idx++, ur.NumKey, ur.MyPK == this.CurrentUR_PK ? "font-weight:bold" : ""));

                    if (ur.MyPK == this.CurrentUR_PK)
                        currUR = ur;

                    continue;
                }

                //如果当前没有选中保存过的查询，则使用_SearchAdv保存的查询显示，这个查询是用户最后一次没有保存过的查询
                if (currUR != null) continue;

                currUR = ur;
                ViewState["newur"] = ur.MyPK;
            }

            //如果这是第一次打开查询，则保存一条_SearchAdv记录
            if (currUR == null)
            {
                currUR = new UserRegedit(WebUser.No, this.RptNo + "_SearchAdv");
                ViewState["newur"] = currUR.MyPK;
            }
            #endregion

            this.CurrentQuery = string.IsNullOrWhiteSpace(currUR.NumKey)
                                    ? "查询条件"
                                    : string.Format("[ {0} ] - 查询条件", currUR.NumKey);

            #region //绑定查询条件

            var paras = currUR.Paras;

            MapAttrs attrs = currMapRpt.MapAttrs;
            string[] conds = paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var condIdx = 0;    //此序列号为标识查询条件的序列号，这样在修改一个查询条件时，传过来的序列号，可以识别是哪个条件，原来的用查询条件的字段来标识是不准确的，因为一个字段可以出现在多个查询条件中

            foreach (string cond in conds)
            {
                if (DataType.IsNullOrEmpty(cond))
                    continue;

                //参数.
                AtPara ap = new AtPara(cond);
                string attrKey = ap.GetValStrByKey("AttrKey");

                //获得属性.
                MapAttr attr = attrs.Filter(MapAttrAttr.KeyOfEn, attrKey) as MapAttr;
                if (attr == null)
                    continue;

                Pub2.AddTR();
                //1.增加左括号.
                var ddl = new DDL();
                ddl.ID = "DDL_LeftBreak_" + condIdx;
                ddl.Style.Add("width", "40px");
                ddl.Items.Add(new ListItem("", ""));
                ddl.Items.Add(new ListItem("(", "("));

                if (!string.IsNullOrWhiteSpace(ap.GetValStrByKey("LeftBreak")))
                    ddl.SelectedIndex = 1;

                Pub2.AddTD("style='width:45px'", ddl);

                //2.增加字段, 此处不允许修改，用文本框显示
                var tb = new TB();
                tb.ID = "TB_Attr_" + condIdx;
                tb.Text = attr.Name;
                tb.ReadOnly = true;
                Pub2.AddTD("style='width:100px'", tb);

                //3.判断该字段类型,确定可使用的运算符.加入到DDL中，并选中当前查询条件的运算符
                ddl = new DDL();
                ddl.ID = "DDL_Exp_" + condIdx;

                switch (attr.LGType)
                {
                    case FieldTypeS.Normal: //普通类型,包含【文本、数字、日期、是否】
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本，可用运算符为：等于、不等于、包含、不包含
                                ddl.Items.Add(new ListItem("等于", "="));
                                ddl.Items.Add(new ListItem("不等于", "!="));
                                ddl.Items.Add(new ListItem("包含", "LIKE"));
                                ddl.Items.Add(new ListItem("不包含", "NOT LIKE"));
                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:  //数字，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于、包含
                                ddl.Items.Add(new ListItem("等于", "="));
                                ddl.Items.Add(new ListItem("不等于", "!="));
                                ddl.Items.Add(new ListItem("大于", ">"));
                                ddl.Items.Add(new ListItem("大于等于", ">="));
                                ddl.Items.Add(new ListItem("小于", "<"));
                                ddl.Items.Add(new ListItem("小于等于", "<="));
                                ddl.Items.Add(new ListItem("包含", "IN"));
                                break;
                            case DataType.AppDate:
                            case DataType.AppDateTime:  //日期，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于
                                ddl.Items.Add(new ListItem("等于", "="));
                                ddl.Items.Add(new ListItem("不等于", "!="));
                                ddl.Items.Add(new ListItem("大于", ">"));
                                ddl.Items.Add(new ListItem("大于等于", ">="));
                                ddl.Items.Add(new ListItem("小于", "<"));
                                ddl.Items.Add(new ListItem("小于等于", "<="));
                                break;
                            case DataType.AppBoolean:   //布尔，可用运算符为：等于、不等于
                                ddl.Items.Add(new ListItem("等于", "="));
                                ddl.Items.Add(new ListItem("不等于", "!="));
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum: //枚举的. 
                        ddl.Items.Add(new ListItem("等于", "="));
                        ddl.Items.Add(new ListItem("不等于", "!="));
                        //ddl.Items.Add(new ListItem("包含", "IN"));
                        //ddl.Items.Add(new ListItem("不包含", "NOT IN")); //因为后面使用的是DDL，所以现在不支持“包含”与“不包含”运算
                        break;
                    case FieldTypeS.FK: // 外键.
                        ddl.Items.Add(new ListItem("等于", "="));
                        ddl.Items.Add(new ListItem("不等于", "!="));
                        //ddl.Items.Add(new ListItem("包含", "IN"));
                        //ddl.Items.Add(new ListItem("不包含", "NOT IN"));
                        break;
                    default:
                        break;
                }

                var exp = ap.GetValStrByKey("Exp");

                if (!string.IsNullOrWhiteSpace(exp) && exp.Length > 0)
                    ddl.SelectedValue = exp;

                Pub2.AddTD("style='width:80px'", ddl);

                //4.增加查询条件值
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal: //普通类型,包含【文本、数字、日期、是否】
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:  //文本/数字都使用文本框
                                tb = new TB();
                                tb.ID = "TB_Val_" + condIdx;
                                tb.Text = ap.GetValStrByKey("Val");
                                Pub2.AddTD(tb);
                                break;
                            case DataType.AppDate:
                                tb = new TB();
                                tb.ID = "TB_Val_" + condIdx;
                                tb.ShowType = TBType.Date;
                                tb.Attributes["onfocus"] = "WdatePicker();";
                                tb.Text = ap.GetValStrByKey("Val");
                                Pub2.AddTD(tb);
                                break;
                            case DataType.AppDateTime:  //日期，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于
                                tb = new TB();
                                tb.ID = "TB_Val_" + condIdx;
                                tb.ShowType = TBType.DateTime;
                                tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
                                tb.Text = ap.GetValStrByKey("Val");
                                Pub2.AddTD(tb);
                                break;
                            case DataType.AppBoolean:   //布尔，可用运算符为：等于、不等于
                                ddl = new DDL();
                                ddl.ID = "DDL_Val_" + condIdx;
                                ddl.Items.Add(new ListItem("", ""));
                                ddl.Items.Add(new ListItem("是", "1"));
                                ddl.Items.Add(new ListItem("否", "0"));
                                ddl.SetSelectItem(ap.GetValStrByKey("Val"));
                                Pub2.AddTD(ddl);
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum: //枚举的. 
                        ddl = new DDL();
                        ddl.ID = "DDL_Val_" + condIdx;
                        ddl.SelfBindSysEnum(attr.UIBindKey);
                        ddl.SetSelectItem(ap.GetValIntByKey("Val"));
                        Pub2.AddTD(ddl);
                        break;
                    case FieldTypeS.FK: // 外键.
                        ddl = new DDL();
                        ddl.ID = "DDL_Val_" + condIdx;
                        ddl.BindEntities(attr.HisAttr.HisFKEns, attr.UIRefKey, attr.UIRefKeyText);
                        ddl.SetSelectItem(ap.GetValStrByKey("Val"));
                        Pub2.AddTD(ddl);
                        break;
                    default:
                        break;
                }

                //5.增加右括号
                ddl = new DDL();
                ddl.ID = "DDL_RightBreak_" + condIdx;
                ddl.Style.Add("width", "40px");
                ddl.Items.Add(new ListItem("", ""));
                ddl.Items.Add(new ListItem(")", ")"));

                if (!string.IsNullOrWhiteSpace(ap.GetValStrByKey("RightBreak")))
                    ddl.SelectedIndex = 1;

                Pub2.AddTD("style='width:45px'", ddl);

                //6.增加条件之间的逻辑运算符
                ddl = new DDL();
                ddl.ID = "DDL_Union_" + condIdx;
                ddl.Items.Add(new ListItem("并且", "And"));
                ddl.Items.Add(new ListItem("或者", "Or"));
                ddl.SetSelectItem(ap.GetValStrByKey("Union"));
                Pub2.AddTD("style='width:60px'", ddl);

                Pub2.AddTDBegin();

                Pub2.Add("<a href='javascript:void(0)' class='easyui-linkbutton' onclick=\"selectAttr('" + condIdx + "','" + attr.KeyOfEn + "')\" data-options=\"iconCls:'icon-edit'\" title='修改'></a>" + Environment.NewLine);

                var btn = new LinkBtn(false, "Btn_Delete_" + condIdx, "");
                btn.SetDataOption("iconCls", "'icon-delete'");
                btn.Attributes.Add("onclick", "return confirm('您确定要删除该查询条件吗？');");
                btn.Attributes.Add("title", "删除");
                btn.Click += new EventHandler(btn_Delete_Click);
                Pub2.Add(btn);
                Pub2.AddSpace(1);

                //最后一个条件，有“增加”按钮，点击后弹出选择查询条件字段的对话框
                if (condIdx == conds.Length - 1)
                {
                    Pub2.Add(
                    "<a href='javascript:void(0)' class='easyui-linkbutton' onclick=\"selectAttr('','')\" data-options=\"iconCls:'icon-add'\" title='增加'></a>" + Environment.NewLine);
                }

                Pub2.AddTDEnd();
                Pub2.AddTREnd();
                condIdx++;
            }

            if (conds.Length == 0 || condIdx != conds.Length)
            {
                //增加一个空白条件
                Pub2.AddTR();

                var ddl1 = new DDL();
                ddl1.ID = "DDL_LeftBreak";
                ddl1.Style.Add("width", "55px");
                ddl1.Items.Add(new ListItem("", ""));
                ddl1.Items.Add(new ListItem("(", " ( "));
                Pub2.AddTD("style='width:60px'", ddl1);

                var tb1 = new TB();
                tb1.ID = "TB_Attr";
                tb1.Text = string.Empty;
                tb1.ReadOnly = true;
                Pub2.AddTD("style='width:140px'", tb1);
                Pub2.AddTD("style='width:80px'", "");
                Pub2.AddTD();
                Pub2.AddTD("style='width:60px'", "");
                Pub2.AddTD("style='width:60px'", "");
                Pub2.AddTDBegin("style='width:100px'");

                Pub2.Add(
                    "<a href='javascript:void(0)' class='easyui-linkbutton' onclick=\"selectAttr('','')\" data-options=\"iconCls:'icon-add'\">增加</a>&nbsp;");

                Pub2.AddTDEnd();
                Pub2.AddTREnd();
            }

            if (currUR.MyPK.Contains("SearchAdvModel"))
            {
                btnSaveSearchDirect.Visible = true;
                btnDeleteModel.Visible = true;
            }
            else if (conds.Length > 0)
            {
                btnSaveSearch.Visible = true;
                btnDeleteModel.Visible = false;
            }
            #endregion

            //选择字段窗体
            LB_Attrs.Items.Clear();

            foreach (MapAttr attr in attrs)
            {
                LB_Attrs.Items.Add(new ListItem(attr.Name, attr.KeyOfEn));
            }

            if (IsSearch)
                this.SetDGData();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var currUR = GetCurrentUserRegedit();
                currUR.Paras = GenerateParas(currUR);
                currUR.GenerSQL = GenerateSQLByQueryObject(currUR).SQLWithOutPara;
                currUR.Save();
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, true, true);
                return;
            }

            SetDGData(1);
        }

        /// <summary>
        /// 保存并生成SQL查询语句
        /// </summary>
        private void SaveAndGenerateSQL(UserRegedit currUR)
        {
            var paras = currUR.Paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            currMapRpt = new MapRpt(this.RptNo);
            MapAttrs attrs = currMapRpt.MapAttrs;

            var newParas = string.Empty;
            var newSQL = string.Format("SELECT * FROM {0} WHERE ", HisEns.GetNewEntity.EnMap.PhysicsTable);
            AtPara ap = null;
            Control ctrl = null;
            MapAttr attr = null;

            for (var i = 0; i < paras.Length; i++)
            {
                //组织paras
                ap = new AtPara(paras[i]);
                ap.SetVal("LeftBreak", (Pub2.FindControl("DDL_LeftBreak_" + i) as DDL).SelectedItemStringVal);
                ap.SetVal("Exp", (Pub2.FindControl("DDL_Exp_" + i) as DDL).SelectedItemStringVal);
                ap.SetVal("RightBreak", (Pub2.FindControl("DDL_RightBreak_" + i) as DDL).SelectedItemStringVal);
                ap.SetVal("Union", (Pub2.FindControl("DDL_Union_" + i) as DDL).SelectedItemStringVal);

                ctrl = Pub2.FindControl("TB_Val_" + i);

                if (ctrl != null)
                {
                    ap.SetVal("Val", (ctrl as TB).Text);
                }
                else
                {
                    ctrl = Pub2.FindControl("DDL_Val_" + i);

                    if (ctrl == null)
                        throw new Exception("查询条件值未找到");

                    ap.SetVal("Val", (ctrl as DDL).SelectedItemStringVal);
                }

                newParas += ap.GenerAtParaStrs() + "^";

                //组织SQL
                if (string.IsNullOrWhiteSpace(ap.GetValStrByKey("AttrKey")) || (string.IsNullOrWhiteSpace(ap.GetValStrByKey("Exp")) && string.IsNullOrWhiteSpace(ap.GetValStrByKey("Val")))) continue;  //去除最后增加的无效的条件

                attr = attrs.Filter(MapAttrAttr.KeyOfEn, ap.GetValStrByKey("AttrKey")) as MapAttr;
                newSQL += ap.GetValStrByKey("LeftBreak");
                newSQL += ap.GetValStrByKey("AttrKey") + " ";
                newSQL += ap.GetValStrByKey("Exp") + " ";

                switch (attr.LGType)
                {
                    case FieldTypeS.Normal: //普通类型,包含【文本、数字、日期、是否】
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本，可用运算符为：等于、不等于、包含、不包含
                                switch (ap.GetValStrByKey("Exp"))
                                {
                                    case "LIKE":
                                    case "NOT LIKE":
                                        newSQL += string.Format("'%{0}%' ", ap.GetValStrByKey("Val"));
                                        break;
                                    default:
                                        newSQL += string.Format("'{0}' ", ap.GetValStrByKey("Val"));
                                        break;
                                }

                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:      //数字，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于、包含
                                switch (ap.GetValStrByKey("Exp"))
                                {
                                    case "IN":
                                        newSQL += "(" + ap.GetValStrByKey("Val").Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Aggregate(string.Empty, (curr, next) => curr + next + ",").TrimEnd(',') + ") ";
                                        break;
                                    default:
                                        newSQL += ap.GetValStrByKey("Val") + " ";
                                        break;
                                }

                                break;
                            case DataType.AppDate:
                            case DataType.AppDateTime:  //日期，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于
                                newSQL += string.Format("'{0}' ", ap.GetValStrByKey("Val"));
                                break;
                            case DataType.AppBoolean:   //布尔，可用运算符为：等于、不等于
                                newSQL += ap.GetValStrByKey("Val") + " ";
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum: //枚举的. 
                        newSQL += ap.GetValStrByKey("Val") + " ";
                        break;
                    case FieldTypeS.FK: // 外键.
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本
                                newSQL += string.Format("'{0}' ", ap.GetValStrByKey("Val"));
                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney: //数字
                                newSQL += ap.GetValStrByKey("Val") + " ";
                                break;
                        }
                        break;
                    default:
                        break;
                }

                newSQL += ap.GetValStrByKey("RightBreak") + " ";
                newSQL += ap.GetValStrByKey("Union") + " ";
            }

            currUR.GenerSQL = newSQL.TrimEnd("And ".ToCharArray()).TrimEnd("Or ".ToCharArray());
            currUR.Save();
        }

        /// <summary>
        /// 生成Paras
        /// </summary>
        private string GenerateParas(UserRegedit currUR)
        {
            var paras = currUR.Paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var newParas = string.Empty;
            AtPara ap = null;
            Control ctrl = null;

            for (var i = 0; i < paras.Length; i++)
            {
                //组织paras
                ap = new AtPara(paras[i]);
                ap.SetVal("LeftBreak", (Pub2.FindControl("DDL_LeftBreak_" + i) as DDL).SelectedItemStringVal);
                ap.SetVal("Exp", (Pub2.FindControl("DDL_Exp_" + i) as DDL).SelectedItemStringVal);
                ap.SetVal("RightBreak", (Pub2.FindControl("DDL_RightBreak_" + i) as DDL).SelectedItemStringVal);
                ap.SetVal("Union", (Pub2.FindControl("DDL_Union_" + i) as DDL).SelectedItemStringVal);

                ctrl = Pub2.FindControl("TB_Val_" + i);

                if (ctrl != null)
                {
                    ap.SetVal("Val", (ctrl as TB).Text);
                }
                else
                {
                    ctrl = Pub2.FindControl("DDL_Val_" + i);

                    if (ctrl == null)
                        throw new Exception("查询条件值未找到");

                    ap.SetVal("Val", (ctrl as DDL).SelectedItemStringVal);
                }

                newParas += ap.GenerAtParaStrs() + "^";
            }

            return newParas;
        }

        /// <summary>
        /// 生成SQL
        /// </summary>
        /// <returns></returns>
        private string GenerateSQL(UserRegedit currUR)
        {
            var paras = currUR.Paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            currMapRpt = new MapRpt(this.RptNo);
            MapAttrs attrs = currMapRpt.MapAttrs;

            var newSQL = string.Format("SELECT * FROM {0} WHERE ", HisEns.GetNewEntity.EnMap.PhysicsTable);
            AtPara ap = null;
            MapAttr attr = null;

            for (var i = 0; i < paras.Length; i++)
            {
                ap = new AtPara(paras[i]);

                if (string.IsNullOrWhiteSpace(ap.GetValStrByKey("AttrKey")) || (string.IsNullOrWhiteSpace(ap.GetValStrByKey("Exp")) && string.IsNullOrWhiteSpace(ap.GetValStrByKey("Val")))) continue;  //去除最后增加的无效的条件

                //组织SQL
                attr = attrs.Filter(MapAttrAttr.KeyOfEn, ap.GetValStrByKey("AttrKey")) as MapAttr;
                newSQL += ap.GetValStrByKey("LeftBreak");
                newSQL += ap.GetValStrByKey("AttrKey") + " ";
                newSQL += ap.GetValStrByKey("Exp") + " ";

                switch (attr.LGType)
                {
                    case FieldTypeS.Normal: //普通类型,包含【文本、数字、日期、是否】
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本，可用运算符为：等于、不等于、包含、不包含
                                switch (ap.GetValStrByKey("Exp"))
                                {
                                    case "LIKE":
                                    case "NOT LIKE":
                                        newSQL += string.Format("'%{0}%' ", ap.GetValStrByKey("Val"));
                                        break;
                                    default:
                                        newSQL += string.Format("'{0}' ", ap.GetValStrByKey("Val"));
                                        break;
                                }

                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:   //数字，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于、包含
                                switch (ap.GetValStrByKey("Exp"))
                                {
                                    case "IN":
                                        newSQL += "(" + ap.GetValStrByKey("Val").Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Aggregate(string.Empty, (curr, next) => curr + next + ",").TrimEnd(',') + ") ";
                                        break;
                                    default:
                                        newSQL += ap.GetValStrByKey("Val") + " ";
                                        break;
                                }

                                break;
                            case DataType.AppDate:
                            case DataType.AppDateTime:  //日期，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于
                                newSQL += string.Format("'{0}' ", ap.GetValStrByKey("Val"));
                                break;
                            case DataType.AppBoolean:   //布尔，可用运算符为：等于、不等于
                                newSQL += ap.GetValStrByKey("Val") + " ";
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum: //枚举的. 
                        newSQL += ap.GetValStrByKey("Val") + " ";
                        break;
                    case FieldTypeS.FK: // 外键.
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本
                                newSQL += string.Format("'{0}' ", ap.GetValStrByKey("Val"));
                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:
                                newSQL += ap.GetValStrByKey("Val") + " ";
                                break;
                        }
                        break;
                    default:
                        break;
                }

                newSQL += ap.GetValStrByKey("RightBreak") + " ";
                newSQL += ap.GetValStrByKey("Union") + " ";
            }

            return newSQL.TrimEnd("And ".ToCharArray()).TrimEnd("Or ".ToCharArray());
        }

        /// <summary>
        /// 生成SQL
        /// </summary>
        /// <returns></returns>
        private QueryObject GenerateSQLByQueryObject(UserRegedit currUR)
        {
            Entities ens = this.HisEns;
            QueryObject qo = new QueryObject(ens);

            var paras = currUR.Paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            currMapRpt = new MapRpt(this.RptNo);
            MapAttrs attrs = currMapRpt.MapAttrs;

            AtPara ap = null;
            MapAttr attr = null;
            var ps = new List<AtPara>();
            var breaks = new List<bool>();  //标识括号的数列，true为左括号，false为右括号

            //去除无效的条件
            for (var i = 0; i < paras.Length; i++)
            {
                ap = new AtPara(paras[i]);

                if (string.IsNullOrWhiteSpace(ap.GetValStrByKey("AttrKey")) || (string.IsNullOrWhiteSpace(ap.GetValStrByKey("Exp")) && string.IsNullOrWhiteSpace(ap.GetValStrByKey("Val")))) continue;  //去除最后增加的无效的条件
                if (ap.GetValStrByKey("LeftBreak") == "(")
                    breaks.Add(true);

                if (ap.GetValStrByKey("RightBreak") == ")")
                    breaks.Add(false);

                ps.Add(ap);
            }

            //初步检测查询条件的准确性，主要检测括号是否成对
            var errorMsg = "查询条件错误，括号不成对，请检查！";

            if (breaks.Count % 2 != 0)
                throw new Exception(errorMsg);

            while (breaks.Count > 0)
            {
                var isDo = false;   //标识已经发现一成对的括号

                for (var i = 0; i < breaks.Count - 1; i++)
                {
                    if (breaks[i] && !breaks[i + 1])
                    {
                        breaks.RemoveRange(i, 2);
                        isDo = true;
                        break;
                    }
                }

                if (!isDo)
                    throw new Exception(errorMsg);
            }

            var dictParas = new Dictionary<string, int>();  //查询中用到的参数/出现次数，以便于使参数名不重复
            var paramName = string.Empty;

            for (var i = 0; i < ps.Count; i++)
            {
                //组织SQL
                attr = attrs.Filter(MapAttrAttr.KeyOfEn, ps[i].GetValStrByKey("AttrKey")) as MapAttr;
                if (ps[i].GetValStrByKey("LeftBreak") == "(")
                    qo.addLeftBracket();

                paramName = attr.KeyOfEn;

                if (dictParas.ContainsKey(paramName))
                {
                    dictParas[paramName]++;
                    paramName += dictParas[paramName];
                }
                else
                {
                    dictParas.Add(attr.KeyOfEn, 1);
                    paramName += 1;
                }

                switch (attr.LGType)
                {
                    case FieldTypeS.Normal: //普通类型,包含【文本、数字、日期、是否】
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本，可用运算符为：等于、不等于、包含、不包含
                                switch (ps[i].GetValStrByKey("Exp"))
                                {
                                    case "LIKE":
                                    case "NOT LIKE":
                                        qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), string.Format("%{0}% ", ps[i].GetValStrByKey("Val")), paramName);
                                        break;
                                    default:
                                        qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                                        break;
                                }

                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:  //数字，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于、包含
                                switch (ps[i].GetValStrByKey("Exp"))
                                {
                                    case "IN":
                                        qo.AddWhere(attr.KeyOfEn, "IN",
                                                      ps[i].GetValStrByKey("Val").Split(" ".ToCharArray(),
                                                                                     StringSplitOptions.
                                                                                         RemoveEmptyEntries).Aggregate(
                                                                                             string.Empty,
                                                                                             (curr, next) =>
                                                                                             curr + next + ",").TrimEnd(
                                                                                                 ','), paramName);
                                        break;
                                    default:
                                        qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                                        break;
                                }

                                break;
                            case DataType.AppDate:
                            case DataType.AppDateTime:  //日期，可用运算符为：等于、不等于、大于、大于等于、小于、小于等于
                                qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                                break;
                            case DataType.AppBoolean:   //布尔，可用运算符为：等于、不等于
                                qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                                break;
                            default:
                                break;
                        }
                        break;
                    case FieldTypeS.Enum: //枚举的. 
                        qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                        break;
                    case FieldTypeS.FK: // 外键.
                        switch (attr.MyDataType)
                        {
                            case DataType.AppString:    //文本
                                qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                                break;
                            case DataType.AppInt:
                            case DataType.AppFloat:
                            case DataType.AppDouble:
                            case DataType.AppMoney:  //数字
                                qo.AddWhere(attr.KeyOfEn, ps[i].GetValStrByKey("Exp"), ps[i].GetValStrByKey("Val"), paramName);
                                break;
                        }
                        break;
                    default:
                        break;
                }

                if (ps[i].GetValStrByKey("RightBreak") == ")")
                    qo.addRightBracket();

                //最后一个连接符不加进去
                if (i < ps.Count - 1)
                {
                    if (ps[i].GetValStrByKey("Union") == "And")
                        qo.addAnd();
                    else if (ps[i].GetValStrByKey("Union") == "Or")
                        qo.addOr();
                }
            }

            return qo;
        }

        protected void btnSaveSearch_Click(object sender, EventArgs e)
        {
            UserRegedit currUR = null;

            try
            {
                currUR = GetCurrentUserRegedit();
                currUR.Paras = GenerateParas(currUR);
                currUR.GenerSQL = GenerateSQLByQueryObject(currUR).SQLWithOutPara;
                currUR.Save();
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, true, true);
                return;
            }

            var url = string.Format("SearchAdv.aspx?RptNo={0}&FK_Flow={1}", this.RptNo,
                                    this.FK_Flow);

            if (!currUR.MyPK.Contains("SearchAdvModel"))
            {
                var modelName = this.hiddenAttrKey.Value;
                var urModel = new UserRegedit();
                urModel.AutoMyPK = false;
                urModel.FK_Emp = WebUser.No;
                urModel.CfgKey = this.RptNo + "_SearchAdv";

                var lastModelName =
                    BP.DA.DBAccess.RunSQLReturnString(
                        string.Format(
                            "SELECT TOP 1 SUR.MyPK FROM Sys_UserRegedit sur WHERE sur.CfgKey = '{0}' AND sur.MyPK LIKE '{1}{0}Model%' ORDER BY sur.MyPK DESC",
                            urModel.CfgKey, urModel.FK_Emp));

                urModel.MyPK = string.Format("{0}{1}Model{2}", urModel.FK_Emp, urModel.CfgKey,
                                             lastModelName == null
                                                 ? "001"
                                                 : (int.Parse(
                                                     lastModelName.Substring(
                                                         string.Format("{0}{1}Model", urModel.FK_Emp, urModel.CfgKey)
                                                             .Length)) + 1).ToString("000"));
                urModel.NumKey = modelName;
                urModel.GenerSQL = currUR.GenerSQL;
                urModel.Paras = currUR.Paras;
                urModel.Insert();

                url += "&CurrentUR_PK=" + urModel.MyPK;
            }
            else
            {
                url += "&CurrentUR_PK=" + currUR.MyPK;
            }

            Response.Redirect(url, true);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            MapData md = new MapData(this.RptNo);
            Entities ens = md.HisEns;
            Entity en = ens.GetNewEntity;
            var currUR = GetCurrentUserRegedit();
            var qo = GenerateSQLByQueryObject(currUR);

            DataTable dt = qo.DoQueryToTable();
            DataTable myDT = new DataTable();
            MapAttrs attrs = new MapAttrs(this.RptNo);

            foreach (MapAttr attr in attrs)
            {
                myDT.Columns.Add(new DataColumn(attr.Name, typeof(string)));
            }

            foreach (DataRow dr in dt.Rows)
            {
                DataRow myDR = myDT.NewRow();

                foreach (MapAttr attr in attrs)
                {
                    if (dt.Columns.Contains(attr.Field + "Text"))
                        myDR[attr.Name] = dr[attr.Field + "Text"];
                    else
                        myDR[attr.Name] = dr[attr.Field];
                }

                myDT.Rows.Add(myDR);
            }

            try
            {
                this.ExportDGToExcel(myDT, en.EnDesc);
            }
            catch (Exception ex)
            {
                this.Alert(ex);
            }

            this.SetDGData(this.PageIdx);
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            var hattr = hiddenAttrKey.Value.Split('^');
            if (hattr.Length < 2)
            {
                this.Alert("参数错误", true, true);
                return;
            }

            var edittingCondIdx = hattr[0];
            var newAttr = hattr[1];
            var isNewCond = string.IsNullOrWhiteSpace(edittingCondIdx);
            var condIdx = -1;
            int.TryParse(edittingCondIdx, out condIdx);

            var currUR = GetCurrentUserRegedit();
            var isFirstCond = string.IsNullOrWhiteSpace(currUR.Paras);

            if (isFirstCond)
            {
                //第一个查询条件的所用字段的保存
                currUR.Paras = "@LeftBreak=" + (Pub2.FindControl("DDL_LeftBreak") as DDL).SelectedItemStringVal;
                currUR.Paras += "@AttrKey=" + newAttr;
                currUR.Paras += "@Exp=@Val=@RightBreak=@Union=";
                currUR.Save();
            }
            else
            {
                var paras = currUR.Paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                AtPara ap = null;
                Control ctrl = null;

                currUR.Paras = string.Empty;

                for (var i = 0; i < paras.Length; i++)
                {
                    ap = new AtPara(paras[i]);
                    ap.SetVal("LeftBreak", (Pub2.FindControl("DDL_LeftBreak_" + i) as DDL).SelectedItemStringVal);

                    if (i == condIdx && !string.IsNullOrWhiteSpace(edittingCondIdx))
                    {
                        ap.SetVal("AttrKey", newAttr);
                        ap.SetVal("Exp", string.Empty);
                        ap.SetVal("Val", string.Empty);
                    }
                    else
                    {
                        ap.SetVal("Exp", (Pub2.FindControl("DDL_Exp_" + i) as DDL).SelectedItemStringVal);

                        ctrl = Pub2.FindControl("TB_Val_" + i);

                        if (ctrl != null)
                        {
                            ap.SetVal("Val", (ctrl as TB).Text);
                        }
                        else
                        {
                            ctrl = Pub2.FindControl("DDL_Val_" + i);

                            if (ctrl == null)
                                throw new Exception("查询条件值未找到");

                            ap.SetVal("Val", (ctrl as DDL).SelectedItemStringVal);
                        }
                    }

                    ap.SetVal("RightBreak", (Pub2.FindControl("DDL_RightBreak_" + i) as DDL).SelectedItemStringVal);
                    ap.SetVal("Union", (Pub2.FindControl("DDL_Union_" + i) as DDL).SelectedItemStringVal);

                    currUR.Paras += ap.GenerAtParaStrs() + "^";
                }

                if (isNewCond)
                {
                    //保存新加的查询条件
                    currUR.Paras += "@LeftBreak=@AttrKey=" + newAttr;
                    currUR.Paras += "@Exp=@Val=@RightBreak=@Union=";
                }

                try
                {
                    //生成SQL
                    currUR.GenerSQL = GenerateSQLByQueryObject(currUR).SQLWithOutPara;
                    currUR.Save();
                }
                catch (Exception ex)
                {
                    this.Alert(ex.Message, true, true);
                    return;
                }
            }

            Response.Redirect(string.Format("SearchAdv.aspx?RptNo={0}&FK_Flow={1}&CurrentUR_PK={2}", this.RptNo,
                                            this.FK_Flow, this.CurrentUR_PK));
        }

        protected void btn_Delete_Click(object sender, EventArgs e)
        {
            var condIdx = int.Parse((sender as LinkBtn).ID.Replace("Btn_Delete_", ""));
            var currUR = GetCurrentUserRegedit();

            var paras = currUR.Paras.Split("^".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var newParas = string.Empty;

            for (var i = 0; i < paras.Length; i++)
            {
                if (i == condIdx) continue;

                newParas += paras[i] + "^";
            }

            currUR.Paras = newParas;

            try
            {
                currUR.GenerSQL = GenerateSQLByQueryObject(currUR).SQLWithOutPara;
            }
            catch (Exception ex)
            {
                this.Alert(ex, true);
                return;
            }

            if (currUR.MyPK.Contains("SearchAdvModel"))
                currUR.AutoMyPK = false;

            currUR.Save();

            Response.Redirect(string.Format("SearchAdv.aspx?RptNo={0}&FK_Flow={1}&CurrentUR_PK={2}", this.RptNo,
                                            this.FK_Flow, this.CurrentUR_PK));
        }

        protected void btnDeleteModel_Click(object sender, EventArgs e)
        {
            var currUR = new UserRegedit();
            currUR.MyPK = this.CurrentUR_PK;
            currUR.Delete();

            Response.Redirect(NormalSearchUrl);
        }
        /// <summary>
        /// 获取当前操作的记录
        /// </summary>
        /// <returns></returns>
        private UserRegedit GetCurrentUserRegedit()
        {
            var currUR = new UserRegedit();

            if (DataType.IsNullOrEmpty(this.CurrentUR_PK))
            {
                currUR.MyPK = ViewState["newur"].ToString();
            }
            else
            {
                currUR.MyPK = this.CurrentUR_PK;
            }

            currUR.Retrieve();

            if (currUR.MyPK.Contains("SearchAdvModel"))
                currUR.AutoMyPK = false;

            return currUR;
        }

        public Entities SetDGData()
        {
            return this.SetDGData(this.PageIdx);
        }

        public Entities SetDGData(int pageIdx)
        {
            this.ResultCollapsed = false;

            #region 执行数据分页查询，并绑定分页控件.

            Entities ens = this.HisEns;
            Entity en = ens.GetNewEntity;
            var currUR = GetCurrentUserRegedit();
            QueryObject qo = null;

            try
            {
                qo = GenerateSQLByQueryObject(currUR);
            }
            catch (Exception ex)
            {
                this.Alert(ex.Message, true, true);
                return null;
            }

            this.Pub3.Clear();
            this.Pub3.BindPageIdxEasyUi(qo.GetCount(),
                                        this.PageID + ".aspx?RptNo=" + this.RptNo + "&EnsName=" + this.RptNo +
                                        "&FK_Flow=" + this.FK_Flow + "&CurrentUR_PK=" + this.CurrentUR_PK + "&IsSearch=true", pageIdx,
                                        SystemConfig.PageSize);

            qo.DoQuery(en.PK, SystemConfig.PageSize, pageIdx);
            #endregion 执行数据分页查询，并绑定分页控件.

            //绑定数据.
            this.BindEns(ens, null);

            return ens;
        }

        private string GenerEnUrl(Entity en, Attrs attrs)
        {
            string url = "";
            foreach (Attr attr in attrs)
            {
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        if (attr.IsPK)
                            url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                        break;
                    case UIContralType.DDL:
                        url += "&" + attr.Key + "=" + en.GetValStringByKey(attr.Key);
                        break;
                }
            }
            return url;
        }
        /// <summary>
        /// 绑定实体集合.
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="ctrlId"></param>
        public void BindEns(Entities ens, string ctrlId)
        {
            #region 定义变量.
            MapData md = new MapData(this.RptNo);
            if (this.Page.Title == "")
                this.Page.Title = md.Name;

            this.UCSys1.Controls.Clear();
            Entity myen = ens.GetNewEntity;
            string pk = myen.PK;
            string clName = myen.ToString();
            Attrs attrs = myen.EnMap.Attrs;
            #endregion 定义变量.

            #region  生成表格标题
            this.UCSys1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width: 100%'");
            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitleGroup("序");
            this.UCSys1.AddTDTitleGroup("标题");
            foreach (Attr attr in attrs)
            {
                if (attr.IsRefAttr
                    || attr.Key == "Title"
                    || attr.Key == "MyNum")
                    continue;

                this.UCSys1.AddTDTitleGroup(attr.Desc);
            }
            this.UCSys1.AddTREnd();
            #endregion  生成表格标题

            #region 用户界面属性设置
            int pageidx = this.PageIdx - 1;
            int idx = SystemConfig.PageSize * pageidx;

            #endregion 用户界面属性设置

            #region 数据输出.
            foreach (Entity en in ens)
            {
                #region 处理keys
                string style = WebUser.Style;
                string url = this.GenerEnUrl(en, attrs);
                #endregion

                #region 输出字段。
                idx++;
                this.UCSys1.AddTR();
                this.UCSys1.AddTDIdx(idx);
                // this.UCSys1.AddTD("<a href=\"javascript:WinOpen('./../WFRpt.htm?FK_Flow=" + this.currMapRpt.FK_Flow + "&WorkID=" + en.GetValStrByKey("OID") + "','tdr');\" ><img src='/WF/Img/Track.png' border=0 />" + en.GetValByKey("Title") + "</a>");
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WFRpt.htm?FK_Flow=" + this.currMapRpt.FK_Flow + "&WorkID=" + en.GetValStrByKey("OID") + "','tdr');\" >" + en.GetValByKey("Title") + "</a>");

                //this.UCSys1.AddTD("<img src='/WF/Img/Track.png' border=0 />" + en.GetValByKey("Title")  );

                foreach (Attr attr in attrs)
                {
                    if (attr.IsRefAttr || attr.Key == "MyNum" || attr.Key == "Title")
                        continue;

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        string s = en.GetValRefTextByKey(attr.Key);
                        if (DataType.IsNullOrEmpty(s))
                        {
                            switch (attr.Key)
                            {
                                case "FK_NY": // 2012-01
                                    s = en.GetValStringByKey(attr.Key);
                                    break;
                                default: //其他的情况，把编码输出出来.
                                    s = en.GetValStringByKey(attr.Key);
                                    break;
                            }
                        }
                        this.UCSys1.AddTD(s);
                        continue;
                    }

                    //if (attr.UIHeight != 0)
                    //{
                    //    this.UCSys1.AddTDDoc("...", "<p alt=\"" + en.GetValStringByKey(attr.Key) + "\" >...</p>");
                    //    continue;
                    //}

                    string str = en.GetValStrByKey(attr.Key);
                    //if (focusField == attr.Key)
                    //{
                    //    str = "<b><font color='blue' >" + str + "</font></a>";
                    //}

                    switch (attr.MyDataType)
                    {
                        case DataType.AppDate:
                        case DataType.AppDateTime:
                            if (str == "" || str == null)
                                str = "&nbsp;";
                            this.UCSys1.AddTD(str);
                            break;
                        case DataType.AppString:
                            if (str == "" || str == null)
                                str = "&nbsp;";

                            if (attr.UIHeight != 0)
                                this.UCSys1.AddTDDoc(str, str);
                            else
                                this.UCSys1.AddTD(str);
                            break;
                        case DataType.AppBoolean:
                            if (str == "1")
                                this.UCSys1.AddTD("是");
                            else
                                this.UCSys1.AddTD("否");
                            break;
                        case DataType.AppFloat:
                        case DataType.AppInt:
                        case DataType.AppDouble:
                            this.UCSys1.AddTDNum(str);
                            break;
                        case DataType.AppMoney:
                            this.UCSys1.AddTDNum(decimal.Parse(str).ToString("0.00"));
                            break;
                        default:
                            throw new Exception("no this case ...");
                    }
                }
                this.UCSys1.AddTREnd();
                #endregion 输出字段。
            }
            #endregion 数据输出.

            #region 计算一下是否可以求出合计,主要是判断是否有数据类型在这个Entities中。
            bool IsHJ = false;
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText
                    || attr.Key == "Title"
                    || attr.Key == "MyNum")
                    continue;

                if (attr.UIVisible == false)
                    continue;

                if (attr.UIContralType == UIContralType.DDL)
                    continue;

                if (attr.Key == "OID" ||
                    attr.Key == "MID"
                    || attr.Key == "FID"
                    || attr.Key == "PWorkID"
                    || attr.Key.ToUpper() == "WORKID")
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppDouble:
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppMoney:
                        IsHJ = true;
                        break;
                    default:
                        break;
                }

                if (IsHJ)
                    break;
            }
            #endregion 计算一下是否可以求出合计,主要是判断是否有数据类型在这个Entities中。

            #region  输出合计。
            //edited by liuxc,2015.5.14,解决合计行错列问题
            if (IsHJ)
            {
                this.UCSys1.Add("<TR class='Sum' >");
                this.UCSys1.AddTD();
                this.UCSys1.AddTD("合计");
                foreach (Attr attr in attrs)
                {
                    //if (attr.Key == "MyNum")
                    //    continue;
                    if (attr.MyFieldType == FieldType.RefText
                    || attr.Key == "Title"
                    || attr.Key == "MyNum")
                        continue;

                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        this.UCSys1.AddTD();
                        continue;
                    }

                    //if (attr.UIVisible == false)
                    //    continue;

                    if (attr.Key == "OID" || attr.Key == "MID"
                        || attr.Key.ToUpper() == "WORKID"
                        || attr.Key == "FID")
                    {
                        this.UCSys1.AddTD();
                        continue;
                    }

                    switch (attr.MyDataType)
                    {
                        case DataType.AppDouble:
                            this.UCSys1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                            this.UCSys1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            this.UCSys1.AddTDNum(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.UCSys1.AddTDJE(ens.GetSumDecimalByKey(attr.Key));
                            break;
                        default:
                            this.UCSys1.AddTD();
                            break;
                    }
                }
                /*结束循环*/
                //this.UCSys1.AddTD();
                this.UCSys1.AddTREnd();
            }
            #endregion
            this.UCSys1.AddTableEnd();
        }
    }
}