using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Data;
using BP.DA;
using BP;
using BP.WF;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.WF.Rpt;
using BP.Sys.XML;

namespace CCFlow.WF.Rpt.UC
{
    public partial class Group : BP.Web.UC.UCBase3
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (s == null)
                {
                    // throw new Exception("丢失FK_Flow参数.");
                    s = "068";
                }
                s = s.Replace("ND", "");
                s = s.Replace("Rpt", "");
                return s;
            }
        }
        public  string RptNo
        {
            get
            {
                string s = this.Request.QueryString["RptNo"];
                if (DataType.IsNullOrEmpty(s))
                    return "ND" + int.Parse(this.FK_Flow) + "MyRpt";
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
        /// <summary>
        /// key
        /// </summary>
        public string Key
        {
            get
            {
                try
                {
                    return this.ToolBar1.GetTBByID("TB_Key").Text;
                }
                catch
                {
                    return null;
                }
            }
        }
        public UserRegedit currUR = null;
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsFY
        {
            get
            {
                string str = this.Request.QueryString["IsFY"];
                if (str == null || str == "0")
                    return false;
                return true;
            }
        }
        public string NumKey
        {
            get
            {
                string str = this.Request.QueryString["NumKey"];
                if (str == null)
                    return ViewState["NumKey"] as string;
                else
                    return str;
            }
            set
            {
                ViewState["NumKey"] = value;
            }
        }
        public string OrderBy
        {
            get
            {
                string str = this.Request.QueryString["OrderBy"];
                if (str == null)
                    return ViewState["OrderBy"] as string;
                else
                    return str;
            }
            set
            {
                ViewState["OrderBy"] = value;
            }
        }

        public string OrderWay
        {
            get
            {
                string str = this.Request.QueryString["OrderWay"];
                if (str == null)
                    return ViewState["OrderWay"] as string;
                else
                    return str;
            }
            set
            {
                ViewState["OrderWay"] = value;
            }
        }
        public bool IsReadonly
        {
            get
            {
                string i = this.Request.QueryString["IsReadonly"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }
        public bool IsShowSum
        {
            get
            {
                string i = this.Request.QueryString["IsShowSum"];
                if (i == "1")
                    return true;
                else
                    return false;
            }
        }
        public bool IsContainsNDYF
        {
            get
            {
                if (this.ViewState["IsContinueNDYF"].ToString().ToUpper() == "TRUE")
                    return true;
                else
                    return false;
            }
        }
        public string CfgVal
        {
            get
            {
                return this.ViewState["CfgVal"].ToString();
            }
            set
            {
                this.ViewState["CfgVal"] = value;
            }
        }
        public MapRpt currMapRpt = null;
        public Entity HisEn = null;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 处理查询权限-通用部分，不要修改直接从 /uc/Search.ascx 中copy.

            this.currMapRpt = new MapRpt(this.RptNo);
            this.HisEn = this.HisEns.GetNewEntity;
            Flow fl = new Flow(this.currMapRpt.FK_Flow);

            this.Page.Title = "分组分析 - " + fl.Name;

            //初始化查询工具栏.
            this.ToolBar1.InitToolbarOfMapRpt(fl, currMapRpt, this.RptNo, this.HisEn, 1);
            this.ToolBar1.AddLinkBtn(BP.Web.Controls.NamesOfBtn.Export); //增加导出.

            //增加转到.
            this.ToolBar1.Add("&nbsp;");
            DDL ddl = new DDL();
            ddl.ID = "GoTo";
            ddl.Items.Add(new ListItem("查询", "Search"));
            //  ddl.Items.Add(new ListItem("高级查询", "SearchAdv"));
            ddl.Items.Add(new ListItem("分组分析", "Group"));
            ddl.Items.Add(new ListItem("交叉报表", "D3"));
            ddl.Items.Add(new ListItem("对比分析", "Contrast"));
            ddl.SetSelectItem(this.PageID);
            this.ToolBar1.AddDDL(ddl);
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new EventHandler(ddl_SelectedIndexChanged_Goto);

            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Search).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            this.ToolBar1.GetLinkBtnByID(NamesOfBtn.Export).Click += new System.EventHandler(this.ToolBar1_ButtonClick);
            #endregion 处理查询权限

            this.CB_IsShowPict.Text = "显示图形";
            this.currUR = new UserRegedit(WebUser.No, this.RptNo + "_Group");

            #region 处理分组字段的选择状
            if (this.IsPostBack == false)
            {
                string reAttrs = this.Request.QueryString["Attrs"];
                // string reAttrs = null; 
                if (DataType.IsNullOrEmpty(reAttrs))
                    reAttrs = this.currUR.Vals;

                this.CfgVal = this.currUR.Vals;
                this.CheckBoxList1.Items.Clear();
                foreach (Attr attr in this.HisEn.EnMap.Attrs)
                {
                    if (attr.UIContralType == UIContralType.DDL)
                    {
                        ListItem li = new ListItem(attr.Desc, attr.Key);
                        if (reAttrs != null)
                            li.Selected = reAttrs.Contains(attr.Key);

                        // 根据状态 设置信息.
                        li.Selected = this.CfgVal.Contains(attr.Key);

                        //加入项目列表.
                        this.CheckBoxList1.Items.Add(li);
                    }
                }

                if (this.CheckBoxList1.Items.Count == 0)
                    throw new Exception(this.currMapRpt.Name + "@没有外键条件，不适合做分组查询"); //没有外键条件，不适合做分组查询。

                if (this.CheckBoxList1.Items.Count == 1)
                    this.CheckBoxList1.Enabled = false;
            }
            #endregion  处理分组字段的选择状态

            #region 处理变量。
            if (this.OrderBy != null)
            {
                /*检查是否有排序的要求. */

                if (this.OrderBy != null)
                    currUR.OrderBy = this.OrderBy;

                if (this.OrderWay == "Up")
                    currUR.OrderWay = "DESC";
                else
                    currUR.OrderWay = "";

                if (this.NumKey == null)
                    this.NumKey = currUR.NumKey;
            }

            this.OrderBy = currUR.OrderBy;
            this.OrderWay = currUR.OrderWay;
            this.CfgVal = currUR.Vals;

            //如果包含年度年月.
            if (this.HisEn.EnMap.Attrs.Contains("FK_NY")
                && this.HisEn.EnMap.Attrs.Contains("FK_ND"))
                this.ViewState["IsContinueNDYF"] = "TRUE";
            else
                this.ViewState["IsContinueNDYF"] = "FALSE";
            #endregion  处理变量。

            #region 增加排序
            if (this.IsPostBack == false)
                this.CB_IsShowPict.Checked = currUR.IsPic;
            #endregion

            this.BindNums();

            if (this.IsPostBack == false)
                this.BingDataGrade();

            this.CB_IsShowPict.CheckedChanged += new EventHandler(State_Changed);
            this.CheckBoxList1.SelectedIndexChanged += new EventHandler(State_Changed);
        }
        void ddl_SelectedIndexChanged_Goto(object sender, EventArgs e)
        {
            DDL ddl = sender as DDL;
            string item = ddl.SelectedItemStringVal;
            string tKey = DateTime.Now.ToString("MMddhhmmss");
            this.Response.Redirect(item + ".aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow + "&T=" + tKey, true);
        }

        public void BindNums()
        {
            this.UCSys2.Clear();

            // 查询出来关于它的活动列配置。
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.RptNo);

            Attrs attrs = this.HisEn.EnMap.Attrs;
            attrs.AddTBInt("MyNum", 1, "流程数量", true, true);

            this.UCSys2.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%'");

            foreach (Attr attr in attrs)
            {
                #region 排除不必要的字段。
                if (attr.UIContralType != UIContralType.TB)
                    continue;

                //edited by liuxc,2014-12-01
                if (attr.UIVisible == false && attr.Key != "MyNum")
                    continue;

                if (attr.IsNum == false)
                    continue;

                switch (attr.Key)
                {
                    case NDXRptBaseAttr.OID:
                    case NDXRptBaseAttr.FID:
                    case "MID":
                    case NDXRptBaseAttr.PWorkID:
                    case NDXRptBaseAttr.FlowEndNode:
                    case "WorkID":
                        continue;
                    default:
                        break;
                }
                #endregion 排除不必要的字段。

                #region 排除配置以外的不需要计算的属性
                bool isHave = false;
                // 有没有配置抵消它的属性。
                foreach (ActiveAttr aa in aas)
                {
                    if (aa.AttrKey != attr.Key)
                        continue;

                    CheckBox cb1 = new CheckBox();
                    cb1.ID = "CB_" + attr.Key;
                    cb1.Text = attr.Desc;
                    cb1.AutoPostBack = true;

                    if (this.CfgVal.IndexOf("@" + attr.Key) == -1)
                        cb1.Checked = false; /* 如果不包含 key .*/
                    else
                        cb1.Checked = true;

                    cb1.CheckedChanged += new EventHandler(State_Changed);

                    this.UCSys2.AddTR();
                    this.UCSys2.AddTD(cb1);
                    this.UCSys2.AddTD();
                    this.UCSys2.AddTREnd();
                    isHave = true;
                }
                if (isHave)
                    continue;
                #endregion 排除配置以外的不需要计算的属性

                #region 开始把数值类型的字段增加到列表里.
                this.UCSys2.AddTR();
                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + attr.Key;
                cb.Text = attr.Desc;
                cb.AutoPostBack = true;
                cb.CheckedChanged += new EventHandler(State_Changed);

                if (this.CfgVal.IndexOf("@" + attr.Key) == -1)
                    cb.Checked = false; /* 如果不包含 key .*/
                else
                    cb.Checked = true;

                this.UCSys2.Add("<TD style='font-size:12px;text-align:left'>");
                this.UCSys2.Add(cb);
                this.UCSys2.Add("</TD>");

                #region 处理计算方式。
                DDL ddl = new DDL();
                ddl.ID = "DDL_" + attr.Key;
                ddl.Items.Add(new ListItem("求和", "SUM"));
                ddl.Items.Add(new ListItem("求平均", "AVG"));
                if (this.IsContainsNDYF)
                    ddl.Items.Add(new ListItem("求累计", "AMOUNT"));

                ddl.Items.Add(new ListItem("求最大", "MAX"));
                ddl.Items.Add(new ListItem("求最小", "MIN"));

                #region 处理分析项目的选中。
                if (this.CfgVal.IndexOf("@" + attr.Key + "=AVG") != -1)
                {
                    ddl.SetSelectItem("AVG"); // = 1;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=SUM") != -1)
                {
                    ddl.SetSelectItem("SUM");  
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=AMOUNT") != -1)
                {
                    ddl.SetSelectItem("AMOUNT");  
                    //ddl.SelectedIndex = 2;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=MAX") != -1)
                {
                    ddl.SelectedIndex = 3;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=MIN") != -1)
                {
                    ddl.SelectedIndex = 4;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=BZC") != -1)
                {
                    ddl.SelectedIndex = 5;
                }
                else if (this.CfgVal.IndexOf("@" + attr.Key + "=LSXS") != -1)
                {
                    ddl.SelectedIndex = 6;
                }
                #endregion 处理分析项目的选中。

                ddl.AutoPostBack = true;
                ddl.SelectedIndexChanged += new EventHandler(State_Changed);

                this.UCSys2.Add("<TD style='font-size:12px;text-align:left'>");
                this.UCSys2.Add(ddl);
                this.UCSys2.AddTDEnd();
                this.UCSys2.AddTREnd();
                #endregion 处理计算方式。

                if (DataType.IsNullOrEmpty(this.NumKey))
                {
                    this.NumKey = attr.Key;
                    this.UCSys2.GetCBByID("CB_" + attr.Key).Checked = true;
                }
                #endregion 开始把数值类型的字段增加到列表里.
            }
            this.UCSys2.AddTableEnd();
        }
        /// <summary>
        /// 处理什么都没有选择,都没有选择让其选择第一个项目。
        /// </summary>
        public void DealChoseNone()
        {
            #region 检查数值列表是否被选择?
            System.Web.UI.ControlCollection ctls = this.UCSys2.Controls;
            bool isCheck = false;
            foreach (Control ct in ctls)
            {
                if (ct.ID == null)
                    continue;

                if (ct.ID.IndexOf("CB_") == -1)
                    continue;

                string key = ct.ID.Substring("CB_".Length);
                CheckBox cb = this.UCSys2.GetCBByID("CB_" + key);
                if (cb.Checked == false)
                    continue;
                isCheck = true;
                break;
            }

            if (isCheck == false)
            {
                foreach (Control ct in ctls)
                {
                    if (ct.ID == null)
                        continue;

                    if (ct.ID.IndexOf("CB_") == -1)
                        continue;

                    string key = ct.ID.Substring("CB_".Length);
                    CheckBox cb = this.UCSys2.GetCBByID("CB_" + key);
                    cb.Checked = true;
                    break;
                }
            }
            #endregion 检查数值列表是否被选择?

            #region 检查分组列表是否被选择?
            isCheck = false;
            foreach (ListItem li in this.CheckBoxList1.Items)
            {
                if (li.Selected)
                {
                    isCheck = true;
                    break;
                }
            }

            if (isCheck == false)
            {
                foreach (ListItem li in this.CheckBoxList1.Items)
                {
                    li.Selected = true;
                    break;
                }
            }
            #endregion 检查分组列表是否被选择?
        }

        #region 方法
        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <returns></returns>
        public DataTable BingDataGrade()
        {
            ///解决任何数据项目都没有选择的情况.
            this.DealChoseNone();

            Entities ens = this.HisEns;
            Entity en = this.HisEn;

            // 查询出来关于它的活动列配置.
            ActiveAttrs aas = new ActiveAttrs();
            aas.RetrieveBy(ActiveAttrAttr.For, this.RptNo);

            Paras myps = new Paras();
            Attrs attrs = this.HisEn.EnMap.Attrs;

            // 找到分组的数据. 
            string sqlOfGroupKey = "";
            Attrs attrsOfNum = new Attrs(); //定义字段属性集合变量.
            System.Web.UI.ControlCollection ctls = this.UCSys2.Controls;
            string StateNumKey = "StateNumKey@"; // 为保存操作状态的需要。
            string Condition = ""; //处理特殊字段的条件问题。
            foreach (Control ct in ctls)
            {
                if (ct.ID == null)
                    continue;
                if (ct.ID.IndexOf("CB_") == -1)
                    continue;

                CheckBox cb = ct as CheckBox; 
                if (cb.Checked == false)
                    continue;

                string key = ct.ID.Replace("CB_", "");

                attrsOfNum.Add(attrs.GetAttrByKey(key));

                #region 特殊处理配置的东西.
                DDL ddl = this.UCSys2.GetDDLByID("DDL_" + key);
                if (ddl == null)
                {
                    ActiveAttr aa = (ActiveAttr)aas.GetEnByKey(ActiveAttrAttr.AttrKey, key);
                    if (aa == null)
                        continue;

                    Condition += aa.Condition;
                    sqlOfGroupKey += " round (" + aa.Exp + ", 4) AS " + key + ",";
                    StateNumKey += key + "=Checked@"; // 记录状态
                    continue;
                }
                #endregion 特殊处理配置的东西.

                #region 生成sqlOfGroupKey.
                switch (ddl.SelectedItemStringVal)
                {
                    case "SUM":
                        sqlOfGroupKey += " round ( SUM(" + key + "), 4) " + key + ",";
                        StateNumKey += key + "=SUM@"; // 记录状态
                        break;
                    case "AVG":
                        sqlOfGroupKey += " round (AVG(" + key + "), 4)  " + key + ",";
                        StateNumKey += key + "=AVG@"; // 记录状态
                        break;
                    case "AMOUNT":
                        sqlOfGroupKey += " round ( SUM(" + key + "), 4) " + key + ",";
                        StateNumKey += key + "=AMOUNT@"; // 记录状态
                        break;
                    default:
                        throw new Exception("没有判断的情况.");
                }
                #endregion 生成sqlOfGroupKey.

            }

            //求是否有累计字段。
            bool isHaveLJ = false; // 是否有累计字段。
            if (StateNumKey.IndexOf("AMOUNT@") != -1)
                isHaveLJ = true;

            if (sqlOfGroupKey == "")
            {
                this.UCSys1.AddMsgOfWarning("警告",
                    "<img src='/WF/Img/Pub/warning.gif' /><b><font color=red>您没有选择分析的数据</font></b>"); //您没有选择分析的数据。
                return null;
            }

            /* 如果包含累计数据，那它一定需要一个月份字段。业务逻辑错误。*/

            // 把最后的一个逗号去了。
            sqlOfGroupKey = sqlOfGroupKey.Substring(0, sqlOfGroupKey.Length - 1);

            Paras ps = new Paras();
            // 生成 sql.
            string selectSQL = "SELECT ";
            string groupBy = " GROUP BY ";
            Attrs AttrsOfGroup = new Attrs();
            string SelectedGroupKey = "SelectedGroupKey=@"; // 为保存操作状态的需要。
            foreach (ListItem li in this.CheckBoxList1.Items)
            {
                if (li.Value == "FK_NY")
                {
                    /* 如果是年月 分组， 并且如果内部有 累计属性，就强制选择。*/
                    if (isHaveLJ)
                        li.Selected = true;
                }

                if (li.Selected)
                {
                    selectSQL += li.Value + ",";
                    groupBy += li.Value + ",";

                    // 加入组里面。
                    AttrsOfGroup.Add(attrs.GetAttrByKey(li.Value), false, false);
                    SelectedGroupKey += li.Value + "@";
                }
            }

            //去掉最后一个逗号.
            groupBy = groupBy.Substring(0, groupBy.Length - 1);

            //“显示图形”的可用性，只有显示内容只选择一项时，才可用 added by liuxc,2014-11-20
            if (!(CB_IsShowPict.Enabled = AttrsOfGroup.Count == 1))
            {
                CB_IsShowPict.Checked = false;
                TB_H.Enabled = false;
                TB_W.Enabled = false;
                lbtnApply.Enabled = false;

            }
            else
            {
                TB_H.Enabled = true;
                TB_W.Enabled = true;
                lbtnApply.Enabled = true;
            }

            #region 生成Where  通过这个过程产生两个 where.
            // 找到 WHERE 数据。
            string where = " WHERE ";
            string whereOfLJ = " WHERE "; // 累计的 where.
            string url = "";
            foreach (Control item in this.ToolBar1.Controls)
            {
                #region 屏蔽特殊情况.
                if (item.ID == null)
                    continue;
                if (item.ID.IndexOf("DDL_") == -1)
                    continue;
                if (item.ID.IndexOf("DDL_Form_") == 0 || item.ID.IndexOf("DDL_To_") == 0)
                    continue;

                string key = item.ID.Substring("DDL_".Length);
                DDL ddl = (DDL)item;
                if (ddl.SelectedItemStringVal == "all")
                    continue;
                string val = ddl.SelectedItemStringVal;
                if (val == null)
                    continue;
                #endregion 屏蔽特殊情况.

                #region 判断多选 case.
                if (val == "mvals")
                {
                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + this.RptNo + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    if (DataType.IsNullOrEmpty(instr))
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                            {
                                val = WebUser.FK_Dept;
                                ddl.SelectedIndex = 0;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace(".", "','");
                        instr = instr.Substring(2);
                        instr = instr.Substring(0, instr.Length - 2);
                        where += " " + key + " IN (" + instr + ")  AND ";
                        continue;
                    }
                }
                #endregion 判断多选 case.

                #region 判断其他字段.
                where += " " + key + " =" + SystemConfig.AppCenterDBVarStr + key + "   AND ";
                if (key != "FK_NY")
                    whereOfLJ += " " + key + " =" + SystemConfig.AppCenterDBVarStr + key + "   AND ";
                myps.Add(key, val);
                #endregion 判断其他字段
            }
            #endregion

            #region 加上 where like 条件
            if (en.EnMap.IsShowSearchKey == true
                && this.ToolBar1.GetTBByID("TB_Key").Text.Trim().Length > 1)
            {
                string key = this.ToolBar1.GetTBByID("TB_Key").Text.Trim();
                if (key.Length > 1)
                {
                    string whereLike = "";
                    bool isAddOr = false;
                    foreach (Attr likeKey in attrs)
                    {
                        if (likeKey.IsNum)
                            continue;
                        if (likeKey.IsRefAttr)
                            continue;

                        if (likeKey.UIContralType != UIContralType.TB)
                            continue;

                        switch (likeKey.MyDataType)
                        {
                            case DataType.AppDate:
                            case DataType.AppDateTime:
                            case DataType.AppBoolean:
                                continue;
                            default:
                                break;
                        }

                        switch (likeKey.Field)
                        {
                            case "MyFileExt":
                            case "MyFilePath":
                            case "WebPath":
                                continue;
                            default:
                                break;
                        }

                        if (isAddOr == false)
                        {
                            isAddOr = true;
                            whereLike += "      " + likeKey.Field + " LIKE '%" + key + "%' ";
                        }
                        else
                        {
                            whereLike += "   OR   " + likeKey.Field + " LIKE '%" + key + "%'";
                        }
                    }
                    whereLike += "          ";
                    where += "(" + whereLike + ")";
                }
            }
            #endregion

            #region 加上日期时间段.
            if (en.EnMap.DTSearchWay != DTSearchWay.None)
            {
                string dtFrom = this.ToolBar1.GetTBByID("TB_S_From").Text.Trim();
                string dtTo = this.ToolBar1.GetTBByID("TB_S_To").Text.Trim();
                string field = en.EnMap.DTSearchKey;
                string addAnd = "";
                if (en.EnMap.IsShowSearchKey && this.ToolBar1.GetTBByID("TB_Key").Text.Trim().Length > 0)
                    addAnd = " AND ";

                if (en.EnMap.DTSearchWay == DTSearchWay.ByDate)
                {
                    where += addAnd + "( " + field + ">='" + dtFrom + " 01:01' AND " + field + "<='" + dtTo + " 23:59')     ";
                }
                else
                {
                    where += addAnd + "(";
                    where += field + " >='" + dtFrom + "' AND " + field + "<='" + dtTo + "'";
                    where += ")";
                }
            }
            #endregion

            where = where.Replace("AND  AND", " AND ");

            if (where == " WHERE ")
            {
                where = "" + Condition.Replace("and", "");
                whereOfLJ = "" + Condition.Replace("and", "");
            }
            else
            {
                if (where.EndsWith(" AND "))
                    where = where.Substring(0, where.Length - " AND ".Length) + Condition;
                else
                    where = where + Condition;

                whereOfLJ = whereOfLJ.Substring(0, whereOfLJ.Length - " AND ".Length) + Condition;
            }

            string orderByReq = this.Request.QueryString["OrderBy"];
            string orderby = "";

            if (orderByReq != null && this.OrderBy != null
                && (selectSQL.Contains(orderByReq) || sqlOfGroupKey.Contains(orderByReq)))
            {
                orderby = " ORDER BY " + this.OrderBy;
                if (this.OrderWay != "Up")
                    orderby += " DESC ";
            }

            // 组装成需要的 sql 
            string sql = "";
            sql = selectSQL + sqlOfGroupKey + " FROM " + this.currMapRpt.PTable + where + groupBy + orderby;

            // 物理表。
            // this.ResponseWriteBlueMsg(sql);
            myps.SQL = sql;
            DataTable dt2 = DBAccess.RunSQLReturnTable(myps);
            // this.Response.Write(sql);

            DataTable dt1 = dt2.Clone();
            dt1.Columns.Add("IDX", typeof(int));

            #region 对他进行分页面
            int myIdx = 0;
            foreach (DataRow dr in dt2.Rows)
            {
                myIdx++;
                DataRow mydr = dt1.NewRow();
                mydr["IDX"] = myIdx;
                foreach (DataColumn dc in dt2.Columns)
                {
                    mydr[dc.ColumnName] = dr[dc.ColumnName];
                }
                dt1.Rows.Add(mydr);
            }
            #endregion

            #region 处理 Int 类型的分组列。
            DataTable dt = dt1.Clone();
            dt.Rows.Clear();
            foreach (Attr attr in AttrsOfGroup)
            {
                dt.Columns[attr.Key].DataType = typeof(string);
            }
            foreach (DataRow dr in dt1.Rows)
            {
                dt.ImportRow(dr);
            }
            #endregion

            // 处理这个物理表 , 如果有累计字段, 就扩展它的列。
            if (isHaveLJ)
            {
                // 首先扩充列.
                foreach (Attr attr in attrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                        continue;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppInt:
                            dt.Columns.Add(attr.Key + "Amount", typeof(int));
                            break;
                        default:
                            dt.Columns.Add(attr.Key + "Amount", typeof(decimal));
                            break;
                    }
                }

                // 添加累计汇总数据.
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (Attr attr in attrsOfNum)
                    {
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                            continue;

                        //形成查询sql.
                        if (whereOfLJ.Length > 10)
                            sql = "SELECT SUM(" + attr.Key + ") FROM " + this.currMapRpt.PTable + whereOfLJ + " AND ";
                        else
                            sql = "SELECT SUM(" + attr.Key + ") FROM " + this.currMapRpt.PTable + " WHERE ";

                        foreach (Attr attr1 in AttrsOfGroup)
                        {
                            switch (attr1.Key)
                            {
                                case "FK_NY":
                                    sql += " FK_NY <= '" + dr["FK_NY"] + "' AND FK_ND='" + dr["FK_NY"].ToString().Substring(0, 4) + "' AND ";
                                    break;
                                case "FK_Dept":
                                    sql += attr1.Key + "='" + dr[attr1.Key] + "' AND ";
                                    break;
                                case "FK_SJ":
                                case "FK_XJ":
                                    sql += attr1.Key + " LIKE '" + dr[attr1.Key] + "%' AND ";
                                    break;
                                default:
                                    sql += attr1.Key + "='" + dr[attr1.Key] + "' AND ";
                                    break;
                            }
                        }

                        sql = sql.Substring(0, sql.Length - "AND ".Length);
                        if (attr.MyDataType == DataType.AppInt)
                            dr[attr.Key + "Amount"] = DBAccess.RunSQLReturnValInt(sql, 0);
                        else
                            dr[attr.Key + "Amount"] = DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    }
                }
            }

            //开始输出数据table.
            this.UCSys1.Clear();
            this.UCSys1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%'");

            #region 输出 分组 列头
            if (StateNumKey.IndexOf("=AMOUNT") != -1)
            {
                /* 如果包含累计 */

                // 增加分组条件。
                this.UCSys1.AddTR();  // 开始第一列。
                this.UCSys1.Add("<td rowspan=2 class='Title'>ID</td>");
                foreach (Attr attr in AttrsOfGroup)
                {
                    this.UCSys1.Add("<td rowspan=2 class='GroupTitle'>" + attr.Desc + "</td>");
                }
                // 增加数据列
                foreach (Attr attr in attrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1)
                    {
                        /*  如果本数据列 包含累计 */
                        this.UCSys1.Add("<td  colspan=2 class='GroupTitle' >" + attr.Desc + "</td>");
                    }
                    else
                    {
                        this.UCSys1.Add("<td  rowspan=2 class='GroupTitle' >" + attr.Desc + "</td>");
                    }
                }
                this.UCSys1.AddTREnd();  // end 开始第一列

                this.UCSys1.AddTR();
                foreach (Attr attr in attrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                        continue;

                    this.UCSys1.Add("<td class='GroupTitle'>本月</td>"); //本月 this.ToE("OrderCondErr")
                    this.UCSys1.Add("<td class='GroupTitle'>累计</td>"); //累计
                }
                this.UCSys1.AddTR();
            }
            else  /* 如果不包含累计 */
            {
                this.UCSys1.AddTR();
                this.UCSys1.AddTDGroupTitle("style='text-align:center'", "序");

                // 分组条件
                foreach (Attr attr in AttrsOfGroup)
                {
                    if (this.OrderBy == attr.Key)
                    {
                        switch (this.OrderWay)
                        {
                            case "Down":
                                this.UCSys1.AddTDGroupTitle("<a href='" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&OrderBy=" + attr.Key + "&OrderWay=Up' >" + attr.Desc + "<img src='/WF/Img/ArrDown.gif' border=0/></a>");
                                break;
                            case "Up":
                            default:
                                this.UCSys1.AddTDGroupTitle("<a href='" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&OrderBy=" + attr.Key + "&OrderWay=Down' >" + attr.Desc + "<img src='/WF/Img/ArrUp.gif' border=0/></a>");
                                break;
                        }
                    }
                    else
                    {
                        this.UCSys1.AddTDGroupTitle("<a href='" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&OrderBy=" + attr.Key + "&OrderWay=Down' >" + attr.Desc + "</a>");
                    }
                }

                // 分组数据
                foreach (Attr attr in attrsOfNum)
                {
                    string lab = "";
                    if (StateNumKey.Contains(attr.Key + "=SUM"))
                    {
                        lab = "(合计)" + attr.Desc;
                    }
                    else
                    {
                        lab = "(平均)" + attr.Desc;
                    }

                    if (this.OrderBy == attr.Key)
                    {
                        switch (this.OrderWay)
                        {
                            case "Down":
                                if (this.NumKey == attr.Key)
                                    this.UCSys1.AddTDGroupTitle(lab + "<a href='" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "&OrderWay=Up'><img src='/WF/Img/ArrDown.gif' border=0/></a>");
                                else
                                    this.UCSys1.AddTDGroupTitle("<a href=\"" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "\" >" + lab + "</a><a href='" + this.PageID + ".aspx?RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "&OrderWay=Up&FK_Flow=" + this.FK_Flow + "'><img src='/WF/Img/ArrDown.gif' border=0/></a>");
                                break;
                            case "Up":
                            default:
                                if (this.NumKey == attr.Key)
                                    this.UCSys1.AddTDGroupTitle(lab + "<a href='" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&OrderBy=" + attr.Key + "&NumKey=" + attr.Key + "&OrderWay=Down'><img src='/WF/Img/ArrUp.gif' border=0/></a>");
                                else
                                    this.UCSys1.AddTDGroupTitle("<a href=\"" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "\" >" + lab + "</a><a href='" + this.PageID + ".aspx?RptNo=" + this.RptNo + "&OrderBy=" + attr.Key + "&NumKey=" + attr.Key + "&OrderWay=Down&FK_Flow=" + this.FK_Flow + "'><img src='/WF/Img/ArrUp.gif' border=0/></a>");
                                break;
                        }
                    }
                    else
                    {
                        if (this.NumKey == attr.Key)
                            this.UCSys1.AddTDGroupTitle(lab + "<a href='" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "' ><img src='/WF/Img/ArrDownUp.gif' border=0/></a>");
                        else
                            this.UCSys1.AddTDGroupTitle("<a href=\"" + this.PageID + ".aspx?FK_Flow=" + this.FK_Flow + "&DoType=" + this.DoType + "&RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "\" >" + lab + "</a><a href='" + this.PageID + ".aspx?RptNo=" + this.RptNo + "&NumKey=" + attr.Key + "&OrderBy=" + attr.Key + "&FK_Flow=" + this.FK_Flow + "' ><img src='/WF/Img/ArrDownUp.gif' border=0/></a>");
                    }
                }
                this.UCSys1.AddTDGroupTitle("挖掘");
                this.UCSys1.AddTREnd();
            }
            #endregion 生成表头

            #region 生成要查询条件
            string YSurl = "GroupDtl.aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow;    //edited by liuxc,2014-12-18
            string keys = "";
            if (this.ToolBar1.FindControl("TB_S_From") != null)
            {
                YSurl += "&DTFrom=" + this.ToolBar1.GetTBByID("TB_S_From").Text;
                YSurl += "&DTTo=" + this.ToolBar1.GetTBByID("TB_S_To").Text;
            }
            if (this.ToolBar1.FindControl("TB_Key") != null)
            {
                YSurl += "&Key=" + this.ToolBar1.GetTBByID("TB_Key").Text;
            }

            // 分组的信息中是否包含部门？
            bool IsHaveFK_Dept = false;
            foreach (Attr attr in AttrsOfGroup)
            {
                if (attr.Key == "FK_Dept")
                {
                    IsHaveFK_Dept = true;
                    break;
                }
            }
            foreach (AttrSearch a23 in en.EnMap.SearchAttrs)
            {
                Attr attrS = a23.HisAttr;
                if (attrS.MyFieldType == FieldType.RefText)
                    continue;

                if (IsHaveFK_Dept && attrS.Key == "FK_Dept")
                    continue;

                DDL ddl = this.ToolBar1.GetDDLByKey("DDL_" + attrS.Key);
                if (ddl == null)
                {
                    throw new Exception(attrS.Key);
                }

                string val = this.ToolBar1.GetDDLByKey("DDL_" + attrS.Key).SelectedItemStringVal;
                if (val == "all")
                    continue;
                keys += "&" + attrS.Key + "=" + val;
            }
            YSurl = YSurl + keys;
            #endregion

            #region 扩充table 的外键，并且把外键或者枚举的中文名放入里面。
            // 为表扩充外键
            foreach (Attr attr in AttrsOfGroup)
            {
                dt.Columns.Add(attr.Key + "T", typeof(string));
            }
            foreach (Attr attr in AttrsOfGroup)
            {
                if (attr.IsEnum)
                {
                    /* 说明它是枚举类型 */
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    foreach (DataRow dr in dt.Rows)
                    {
                        int val = 0;
                        try
                        {
                            val = int.Parse(dr[attr.Key].ToString());
                        }
                        catch
                        {
                            dr[attr.Key + "T"] = " ";
                            continue;
                        }

                        foreach (SysEnum se in ses)
                        {
                            if (se.IntKey == val)
                                dr[attr.Key + "T"] = se.Lab;
                        }
                    }
                    continue;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    string val = dr[attr.Key].ToString();
                    if (attr.UIBindKey.Contains(".") == false)
                    {
                        try
                        {
                            dr[attr.Key + "T"] = DBAccess.RunSQLReturnStringIsNull("SELECT Name FROM " + attr.UIBindKey + " WHERE No='" + val + "'", val);
                        }
                        catch
                        {
                            dr[attr.Key + "T"] = val;
                        }
                        continue;
                    }

                    Entity myen = attr.HisFKEn;
                    myen.SetValByKey(attr.UIRefKeyValue, val);
                    try
                    {
                        myen.Retrieve();
                        dr[attr.Key + "T"] = myen.GetValStrByKey(attr.UIRefKeyText);
                    }
                    catch
                    {
                        if (val == null || val.Length <= 1)
                        {
                            dr[attr.Key + "T"] = val;
                        }
                        else
                        {
                            dr[attr.Key + "T"] = val;
                        }
                    }
                }
            }
            #endregion 扩充table 的外键，并且把外键或者枚举的中文名放入里面。

            #region 生成datagrade表体
            int i = 0;
            bool is1 = false;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                url = YSurl.Clone() as string;
                // 产生url .
                foreach (Attr attr in AttrsOfGroup)
                    url += "&" + attr.Key + "=" + dr[attr.Key].ToString();

                is1 = this.UCSys1.AddTR(is1);
                this.UCSys1.AddTDIdx(int.Parse(dr["IDX"].ToString()));
                // 分组条件
                foreach (Attr attr in AttrsOfGroup)
                {
                    this.UCSys1.AddTD(dr[attr.Key + "T"].ToString());
                }

                // 分组数据
                foreach (Attr attr in attrsOfNum)
                {
                    decimal obj = 0;
                    try
                    {
                        obj = decimal.Parse(dr[attr.Key].ToString());
                    }
                    catch
                    {
                        // throw new Exception(dr[attr.Key].ToString() +"@SQL="+ sql +"@"+ex.Message +"@Attr="+attr.Key );
                    }

                    switch (attr.MyDataType)
                    {
                        case DataType.AppMoney:
                            if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                            {
                                this.UCSys1.AddTDJE(obj);
                                this.UCSys1.AddTDJE(decimal.Parse(dr[attr.Key + "Amount"].ToString()));
                            }
                            else
                            {
                                this.UCSys1.AddTDJE(obj);
                            }
                            break;
                        default:
                            if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                            {
                                this.UCSys1.AddTDNum(obj);
                                this.UCSys1.AddTDNum(decimal.Parse(dr[attr.Key + "Amount"].ToString()));
                            }
                            else
                            {
                                this.UCSys1.AddTDNum(obj);
                            }
                            break;
                    }
                }
                this.UCSys1.AddTD("<a href=\"javascript:WinOpen('" + url + "',900,900);\"  class='easyui-linkbutton'>详细</a>");
                //this.UCSys1.AddTD("<a href=\"javascript:OpenEasyUiDialog('" + url + "','eudlgframe','详细信息',700,432,'icon-table',false,null,null,document.getElementById('mainDiv'))\" class='easyui-linkbutton'>详细</a>");
                this.UCSys1.AddTREnd();
            }

            #region  加入合计信息。
            this.UCSys1.AddTR("class='TRSum'");
            this.UCSys1.AddTD("汇总");
            foreach (Attr attr in AttrsOfGroup)
            {
                this.UCSys1.AddTD();
            }

            //不显示合计列。
            string NoShowSum = SystemConfig.GetConfigXmlEns("NoShowSum", this.RptNo);
            if (NoShowSum == null)
                NoShowSum = "";

            Attrs attrsOfNum1 = attrsOfNum.Clone();
            decimal d = 0;
            foreach (Attr attr in attrsOfNum)
            {
                if (NoShowSum.Contains("@" + attr.Key + "@"))
                {
                    bool isHave = false;
                    foreach (ActiveAttr aa in aas)
                    {
                        if (aa.AttrKey != attr.Key)
                            continue;

                        isHave = true;
                        /* 如果它是一个计算列 */
                        string exp = aa.ExpApp;
                        if (exp == null || exp == "")
                        {
                            this.UCSys1.AddTD();
                            break;
                        }
                        foreach (Attr myattr in attrsOfNum1)
                        {
                            if (exp.IndexOf("@" + myattr.Key + "@") != -1)
                            {
                                d = 0;
                                foreach (DataRow dr1 in dt.Rows)
                                {
                                    try
                                    {
                                        d += decimal.Parse(dr1[myattr.Key].ToString());
                                    }
                                    catch
                                    {
                                    }
                                }

                                exp = exp.Replace("@" + myattr.Key + "@", d.ToString());
                            }
                        }
                        this.UCSys1.AddTDNum(DataType.ParseExpToDecimal(exp));
                    }

                    if (isHave == false)
                        this.UCSys1.AddTD();
                    continue;
                }

                switch (attr.MyDataType)
                {
                    case DataType.AppMoney:
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key].ToString());
                            this.UCSys1.AddTDJE(d);

                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key + "Amount"].ToString());
                            this.UCSys1.AddTDJE(d);
                        }
                        else
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                            {
                                try
                                {
                                    d += decimal.Parse(dr1[attr.Key].ToString());
                                }
                                catch
                                {
                                }
                            }

                            if (StateNumKey.IndexOf(attr.Key + "=AVG") < 1)
                            {
                                this.UCSys1.AddTDJE(d);
                            }
                            else
                            {
                                if (dt.Rows.Count == 0)
                                    this.UCSys1.AddTD();
                                else
                                    this.UCSys1.AddTDJE(d / dt.Rows.Count);
                            }
                        }
                        break;
                    default:
                        if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") != -1) /*  如果本数据列 包含累计 */
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key].ToString());
                            this.UCSys1.AddTDNum(d);

                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                                d += decimal.Parse(dr1[attr.Key + "Amount"].ToString());
                            this.UCSys1.AddTDNum(d);
                        }
                        else
                        {
                            d = 0;
                            foreach (DataRow dr1 in dt.Rows)
                            {
                                try
                                {
                                    d += decimal.Parse(dr1[attr.Key].ToString());
                                }
                                catch
                                {
                                }
                            }

                            if (StateNumKey.IndexOf(attr.Key + "=AVG") < 1)
                            {
                                this.UCSys1.AddTDNum(d);
                            }
                            else
                            {
                                if (dt.Rows.Count == 0)
                                    this.UCSys1.AddTD();
                                else
                                    this.UCSys1.AddTDJE(d / dt.Rows.Count);
                            }
                        }
                        break;
                }
            }
            this.UCSys1.AddTD();
            this.UCSys1.AddTREnd();
            #endregion

            this.UCSys1.AddTableEnd();
            #endregion 生成表体

            #region 生成图形
            if (this.CB_IsShowPict.Checked)
            {
                //使用tab页显示图表，填充tab框架
                UCSys3.Add("<div class='easyui-tabs' data-options=\"fit:true\">" + Environment.NewLine);
                UCSys3.Add("    <div title='分组数据' data-options=\"iconCls:'icon-table'\" style='padding:5px'>" + Environment.NewLine);
                UCSys4.Add("    </div>" + Environment.NewLine);

                /* 如果是 1 纬 */
                string colOfGroupField = "";
                string colOfGroupName = "";
                string colOfNumField = "";
                string colOfNumName = "";
                int chartHeight = int.Parse(this.TB_H.Text);
                int chartWidth = int.Parse(this.TB_W.Text);

                if (isHaveLJ)
                {
                    /*  如果有累计, 就按照累计字段分析。*/
                    colOfGroupField = AttrsOfGroup[0].Key;
                    colOfGroupName = AttrsOfGroup[0].Desc;

                    colOfNumName = attrsOfNum[0].Desc;
                    if (dt.Columns.Contains(attrsOfNum[0].Key + "AMOUNT"))
                        colOfNumField = attrsOfNum[0].Key + "AMOUNT";
                    else
                        colOfNumField = attrsOfNum[0].Key;
                }
                else
                {
                    colOfGroupField = AttrsOfGroup[0].Key;
                    colOfGroupName = AttrsOfGroup[0].Desc;

                    if (NumKey == null)
                    {
                        colOfNumName = attrsOfNum[0].Desc;
                        colOfNumField = attrsOfNum[0].Key;
                    }
                    else
                    {
                        //  colOfNumField = attrsOfNum[0].Key;
                        colOfNumName = attrs.GetAttrByKey(NumKey).Desc; // this.UCSys1.get;
                        colOfNumField = NumKey;
                    }
                }

                string colOfNumName1 = "";
                if (StateNumKey.Contains(this.NumKey + "=SUM"))
                    colOfNumName1 = "(合计)" + colOfNumName;
                else
                    colOfNumName1 = "(平均)" + colOfNumName;

                if (dt.Columns.Contains(colOfNumField) == false)
                {
                    foreach (Attr item in attrsOfNum)
                    {
                        if (dt.Columns.Contains(item.Key))
                        {
                            colOfNumField = item.Key;
                            break;
                        }
                    }
                }
                try
                {
                    //Chart chart = new Chart();
                    //chart.Width = new Unit(chartWidth);
                    //chart.Height = new Unit(chartHeight);
                    //chart.Titles.Add("对比分析 - 柱状图");
                    //var chartArea = chart.ChartAreas.Add("MainChartArea");
                    //var legend = chart.Legends.Add("MainLegend");
                    //legend.Docking = Docking.Bottom;
                    //var serie = chart.Series.Add("MainSerie");
                    ////serie.LegendText = colOfNumName1;
                    //serie.Legend = "MainLegend";
                    //serie.ChartArea = "MainChartArea";
                    //serie.ChartType = SeriesChartType.Line;
                    //chartArea.AxisX.Title = colOfGroupName;
                    //chartArea.AxisX.TitleAlignment = System.Drawing.StringAlignment.Far;
                    //chartArea.AxisY.Title = colOfNumName1;
                    //chartArea.AxisY.TitleAlignment = System.Drawing.StringAlignment.Far;

                    //chart.DataSource = dt;

                    //serie.XValueMember = colOfGroupField + "T";
                    //serie.YValueMembers = colOfNumField;

                    //chart.DataBind();

                    //UCSys3.Add(chart);

                    var yfields = new Dictionary<string, string>();

                    foreach (Attr attr in attrsOfNum)
                    {
                        yfields.Add(attr.Key, attr.Desc);
                    }

                    //增加柱状图
                    UCSys4.Add("    <div id='column_chart_div' title='柱状图' data-options=\"iconCls:'icon-columnchart'\" style='padding:5px;text-align:center'>" + Environment.NewLine);
                    UCSys4.GenerateColumnChart(dt, AttrsOfGroup[0].Key + "T", AttrsOfGroup[0].Desc, yfields, this.Page.Title.Substring("分组分析 - ".Length), chartWidth, chartHeight);
                    UCSys4.Add("    </div>" + Environment.NewLine);

                    //增加饼图
                    UCSys4.Add("    <div id='pie_chart_div' title='饼状图' data-options=\"iconCls:'icon-piechart'\" style='padding:5px;text-align:center'>" + Environment.NewLine);
                    UCSys4.GeneratePieChart(dt, AttrsOfGroup[0].Key + "T", AttrsOfGroup[0].Desc, yfields, this.Page.Title.Substring("分组分析 - ".Length), chartWidth, chartHeight);
                    UCSys4.Add("    </div>" + Environment.NewLine);

                    //增加拆线图
                    UCSys4.Add("    <div id='line_chart_div' title='折线图' data-options=\"iconCls:'icon-linechart'\" style='padding:5px;text-align:center'>" + Environment.NewLine);
                    UCSys4.GenerateLineChart(dt, AttrsOfGroup[0].Key + "T", AttrsOfGroup[0].Desc, yfields, this.Page.Title.Substring("分组分析 - ".Length), chartWidth, chartHeight);
                    UCSys4.Add("    </div>" + Environment.NewLine);

                    UCSys4.Add("</div>" + Environment.NewLine);
                   
                }
                catch (Exception ex)
                {
                    this.ResponseWriteRedMsg("@产生图片文件出现错误:" + ex.Message);
                }
            }
            #endregion

            #region 保存操作状态
            currUR.Vals = SelectedGroupKey + StateNumKey;
            currUR.CfgKey = this.RptNo + "_Group";
            currUR.FK_Emp = WebUser.No;
            currUR.OrderBy = this.OrderBy;
            currUR.OrderWay = this.OrderWay;
            currUR.IsPic = this.CB_IsShowPict.Checked;
            currUR.GenerSQL = myps.SQL;
            currUR.NumKey = this.NumKey;
            currUR.Paras = "";
            foreach (Para para in myps)
            {
                currUR.Paras += "@" + para.ParaName + "=" + para.val;
            }
            currUR.Save();

            this.SetValueByKey("Vals", currUR.Vals);
            this.SetValueByKey("CfgKey", currUR.CfgKey);
            this.SetValueByKey("OrderBy", currUR.OrderBy);
            this.SetValueByKey("OrderWay", currUR.OrderWay);
            this.SetValueByKey("IsPic", currUR.IsPic);
            this.SetValueByKey("SQL", currUR.GenerSQL);
            this.SetValueByKey("NumKey", currUR.NumKey);
            this.CfgVal = currUR.Vals;
            #endregion

            return dt1;
        }

        public DataTable DealTable(DataTable dt)
        {
            DataTable dtCopy = new DataTable();

            #region 把他们转换为 string 类型。
            foreach (DataColumn dc in dt.Columns)
                dtCopy.Columns.Add(dc.ColumnName, typeof(string));

            foreach (DataRow dr in dt.Rows)
                dtCopy.ImportRow(dr);
            #endregion

            Entity en = this.currMapRpt.HisEn;
            Map map = en.EnMap;
            MapAttrs attrs = this.currMapRpt.MapAttrs;
            foreach (DataColumn dc in dt.Columns)
            {
                bool isLJ = false;
                Attr attr = null;
                try
                {
                    attr = map.GetAttrByKey(dc.ColumnName);
                    isLJ = false;
                }
                catch
                {
                    try
                    {
                        attr = map.GetAttrByKey(dc.ColumnName + "AMOUNT");
                        isLJ = true;
                    }
                    catch
                    {
                    }
                }

                if (attr == null)
                    continue;

                if (attr.UIBindKey == null || attr.UIBindKey == "")
                {
                    if (isLJ)
                        dtCopy.Columns[attr.Key.ToUpper() + "AMOUNT"].ColumnName = "累计";
                    else
                        dtCopy.Columns[attr.Key.ToUpper()].ColumnName = attr.Desc;
                    continue;
                }

                // 设置标签 
                if (attr.UIBindKey.IndexOf(".") != -1)
                {
                    //  Entity en1 = BP.En.ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                    Entity en1 = attr.HisFKEn;
                    string pk = en1.PK;
                    foreach (DataRow dr in dtCopy.Rows)
                    {
                        if (dr[attr.Key] == DBNull.Value)
                            continue;

                        string val = (string)dr[attr.Key];
                        if (val == null || val == "")
                            continue;

                        en1.SetValByKey(pk, dr[attr.Key]);
                        int i = en1.RetrieveFromDBSources();
                        if (i == 0)
                            continue;

                        dr[attr.Key] = en1.GetValStrByKey(attr.UIRefKeyValue) + en1.GetValStrByKey(attr.UIRefKeyText);
                    }
                }
                else if (attr.UIBindKey.Length >= 2)
                {
                    foreach (DataRow mydr in dtCopy.Rows)
                    {
                        if (mydr[attr.Key] == DBNull.Value)
                            continue;

                        int intVal = int.Parse(mydr[attr.Key].ToString());
                        SysEnum se = new SysEnum(attr.UIBindKey, intVal);
                        mydr[attr.Key] = se.Lab;
                    }
                }
                dtCopy.Columns[attr.Key.ToUpper()].ColumnName = attr.Desc;
            }

            try
            {
                dtCopy.Columns["MYNUM"].ColumnName = "个数";
            }
            catch
            {
            }
            return dtCopy;
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
        #endregion

        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            var btn = (LinkBtn)sender;
            switch (btn.ID)
            {
                case NamesOfBtn.Help:
                    break;
                case NamesOfBtn.Excel:
                    DataTable dt = this.BingDataGrade();
                    this.ExportDGToExcel(this.DealTable(dt), this.HisEn.EnDesc);
                    return;
                default:
                    this.ToolBar1.SaveSearchState(this.RptNo, this.Key);
                    this.BingDataGrade();
                    return;
            }
        }

        void State_Changed(object sender, EventArgs e)
        {
            this.BingDataGrade();
        }

        void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BingDataGrade();
        }

        protected void lbtnApply_Click(object sender, EventArgs e)
        {
            this.BingDataGrade();
        }
    }
}