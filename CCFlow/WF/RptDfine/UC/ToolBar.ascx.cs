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
using BP.En;
using BP.WF.Template;
using BP.WF;
using BP.WF.Data;
using BP.Sys;
using BP.DA;
using BP.Web;
using BP.Web.Controls;
using BP.Web.UC;
using BP.Sys.XML;
using BP.Port;
using BP;

namespace CCFlow.WF.Rpt.UC
{
    public partial class ToolBar : BP.Web.UC.UCBase3
    {
        #region 方法
        public new string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        /// <summary>
        /// 是否添加查询按钮
        /// </summary>
        public bool _AddSearchBtn = true;

        public void AddSpt(string id)
        {
            this.Add("&nbsp;");
        }

        /// <summary>
        /// 增加一个easyui-linkbutton样式的按钮
        /// </summary>
        /// <param name="id">按钮ID，可从NamesOfBtn选取</param>
        public void AddLinkBtn(string id)
        {
            var btn = new LinkBtn(true, id, GetTextById(id));

            switch (id)
            {
                case NamesOfBtn.Delete:
                    btn.Attributes["onclick"] = "return confirm('您确定要执行删除吗？');";
                    break;
                default:
                    break;
            }

            this.Add(btn);
        }

        /// <summary>
        /// 增加一个easyui-linkbutton样式的按钮
        /// </summary>
        /// <param name="id">按钮ID，可从NamesOfBtn选取</param>
        /// <param name="text">按钮的文本</param>
        public void AddLinkBtn(string id, string text)
        {
            var btn = new LinkBtn(true, id, text);

            switch (id)
            {
                case NamesOfBtn.Delete:
                    btn.Attributes["onclick"] = "return confirm('您确定要执行删除吗？');";
                    break;
                default:
                    break;
            }

            this.Add(btn);
        }

        private string GetTextById(string id)
        {
            string text = "";
            switch (id)
            {
                case NamesOfBtn.UnDo:
                    text = "撤消操作";
                    break;
                case NamesOfBtn.Do:
                    text = "执行";
                    break;
                case NamesOfBtn.ChoseField:
                    text = "选择字段";
                    break;
                case NamesOfBtn.DataGroup:
                    text = "分组查询";
                    break;
                case NamesOfBtn.Copy:
                    text = "复制";
                    break;
                case NamesOfBtn.Go:
                    text = "转到";
                    break;
                case NamesOfBtn.ExportToModel:
                    text = "模板";
                    break;
                case NamesOfBtn.ExportByTemplate:
                    text = "导出至模板";
                    break;
                case NamesOfBtn.DataCheck:
                    text = "数据检查";
                    break;
                case NamesOfBtn.DataIO:
                    text = "数据导入";
                    break;
                case NamesOfBtn.Statistic:
                    text = "统计";
                    break;
                case NamesOfBtn.Balance:
                    text = "持平";
                    break;
                case NamesOfBtn.Down:
                    text = "下降";
                    break;
                case NamesOfBtn.Up:
                    text = "上升";
                    break;
                case NamesOfBtn.Chart:
                    text = "图形";
                    break;
                case NamesOfBtn.Rpt:
                    text = "报表";
                    break;
                case NamesOfBtn.ChoseCols:
                    text = "选择列查询";
                    break;
                case NamesOfBtn.Excel:
                    text = "导出全部";
                    break;
                case NamesOfBtn.Excel_S:
                    text = "导出当前";
                    break;
                case NamesOfBtn.Xml:
                    text = "导出到Xml";
                    break;
                case NamesOfBtn.Send:
                    text = "发送";
                    break;
                case NamesOfBtn.Reply:
                    text = "回复";
                    break;
                case NamesOfBtn.Forward:
                    text = "转发";
                    break;
                case NamesOfBtn.Next:
                    text = "下一个";
                    break;
                case NamesOfBtn.Previous:
                    text = "上一个";
                    break;
                case NamesOfBtn.Selected:
                    text = "选择";
                    break;
                case NamesOfBtn.Add:
                    text = "增加";
                    break;
                case NamesOfBtn.Adjunct:
                    text = "附件";
                    break;
                case NamesOfBtn.AllotTask:
                    text = "分批任务";
                    break;
                case NamesOfBtn.Apply:
                    text = "申请";
                    break;
                case NamesOfBtn.ApplyTask:
                    text = "申请任务";
                    break;
                case NamesOfBtn.Back:
                    text = "后退";
                    break;
                case NamesOfBtn.Card:
                    text = "卡片";
                    break;
                case NamesOfBtn.Close:
                    text = "关闭";
                    break;
                case NamesOfBtn.Confirm:
                    text = "确定";
                    break;
                case NamesOfBtn.Delete:
                    text = "删除";
                    break;
                case NamesOfBtn.Edit:
                    text = "编辑";
                    break;
                case NamesOfBtn.EnList:
                    text = "列表";
                    break;
                case NamesOfBtn.Cancel:
                    text = "取消";
                    break;
                case NamesOfBtn.Export:
                    text = "导出";
                    break;
                case NamesOfBtn.FileManager:
                    text = "文件管理";
                    break;
                case NamesOfBtn.Help:
                    text = "帮助";
                    break;
                case NamesOfBtn.Insert:
                    text = "插入";
                    break;
                case NamesOfBtn.LogOut:
                    text = "注销";
                    break;
                case NamesOfBtn.Messagers:
                    text = "消息";
                    break;
                case NamesOfBtn.New:
                    text = "新建";
                    //  text = "新建";
                    break;
                case NamesOfBtn.Print:
                    text = "打印";
                    break;
                case NamesOfBtn.Refurbish:
                    text = "刷新";
                    break;
                case NamesOfBtn.Reomve:
                    text = "移除";
                    break;
                case NamesOfBtn.Save:
                    text = "保存";
                    break;
                case NamesOfBtn.SaveAndClose:
                    text = "保存并关闭";
                    break;
                case NamesOfBtn.SaveAndNew:
                    text = "保存并新建";
                    break;
                case NamesOfBtn.SaveAsDraft:
                    text = "保存草稿";
                    break;
                case NamesOfBtn.Search:
                    text = "查找(F)";
                    break;
                case NamesOfBtn.SelectAll:
                    text = "选择全部";
                    break;
                case NamesOfBtn.SelectNone:
                    text = "不选";
                    break;
                case NamesOfBtn.View:
                    text = "查看";
                    break;
                case NamesOfBtn.Update:
                    text = "更新";
                    break;
                default:
                    throw new Exception("@没有定义ToolBarBtn 标记 " + id);
            }

            return text;
        }

        public void AddBtn(string id)
        {
            Btn btn = new Btn();
            btn.Attributes["class"] = "Btn";
            btn.ID = id;
            btn.Text = GetTextById(id);

            this.Add(btn);
        }
        public void AddLab(string id, string lab)
        {
            Label en = new Label();
            en.ID = id;
            en.Text = lab;
            this.Add(en);
        }
        public bool IsExitsContral(string ctrlID)
        {
            if (this.FindControl(ctrlID) == null)
                return false;
            return true;
        }
        public void AddBtn(string id, string text)
        {
            Btn en = new Btn();
            en.ID = id;
            en.Text = text;
            //en.Attributes["class"] = "Btn";
            if (id == "Btn_Delete")
                en.Attributes["onclick"] = "return confirm('您确定要执行删除吗？');";
            this.Add(en);
        }

        public void Btn_Click(object sender, EventArgs e)
        {

        }
        public void AddTB(string id, string text)
        {
            TB en = new TB();
            en.ID = id;
            en.Text = text;
            this.Add(en);
        }
        public void AddTB(string id)
        {
            this.AddTB(id);
        }
        public void AddTB(TB tb)
        {
            this.Add(tb);
        }
        public void AddDDL(DDL ddl)
        {
            this.Add(ddl);
        }
        public void AddDDL(string ddl)
        {
            DDL d = new DDL();
            d.ID = ddl;
            this.Add(d);
        }

        public DDL GetDDLByKey(string key)
        {
            return this.FindControl(key) as DDL;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public QueryObject GetnQueryObjectOracle(Entities ens, Entity en)
        {
            QueryObject qo = this.InitQueryObjectByEns(ens, en.EnMap.IsShowSearchKey, en.EnMap.DTSearchWay, en.EnMap.DTSearchKey,
                en.EnMap.Attrs, en.EnMap.AttrsOfSearch, en.EnMap.SearchAttrs);
            string pk = en.PK;
            if (pk == "No")
                qo.addOrderBy("No");
            if (pk == "OID")
                qo.addOrderBy("OID");

            if (pk == "WorkID")
                qo.addOrderByDesc("WorkID");

            return qo;
        }
        public QueryObject InitQueryObjectByEns(Entities ens, bool IsShowSearchKey, DTSearchWay dw, string dtKey, Attrs attrs, AttrsOfSearch attrsOfSearch, AttrSearchs searchAttrs)
        {
            QueryObject qo = new QueryObject(ens);

            #region 关键字
            string keyVal = "";
            //Attrs attrs = en.EnMap.Attrs;
            if (IsShowSearchKey)
            {
                TB keyTB = this.GetTBByID("TB_Key");
                if (keyTB != null)
                {
                    keyVal = keyTB.Text.Trim();
                }
                else
                {
                    UserRegedit ur = new UserRegedit();
                    //ur.MyPK = WebUser.No + ens.GetNewEntity.ClassID + "_SearchAttrs";
                    QueryObject urObj = new QueryObject(ur);
                    urObj.AddWhere("MyPK",WebUser.No + ens.GetNewEntity.ClassID + "_SearchAttrs");
                    urObj.DoQuery();
                    
                    keyVal = ur.SearchKey;
                }
                this.Page.Session["SKey"] = keyVal;
            }

            if (keyVal.Length >= 1)
            {
                Attr attrPK = new Attr();
                foreach (Attr attr in attrs)
                {
                    if (attr.IsPK)
                    {
                        attrPK = attr;
                        break;
                    }
                }
                int i = 0;
                foreach (Attr attr in attrs)
                {

                    switch (attr.MyFieldType)
                    {
                        case FieldType.Enum:
                        case FieldType.FK:
                        case FieldType.PKFK:
                            continue;
                        default:
                            break;
                    }


                    if (attr.MyDataType != DataType.AppString)
                        continue;

                    if (attr.MyFieldType == FieldType.RefText)
                        continue;

                    if (attr.Key == "FK_Dept")
                        continue;

                    i++;
                    if (i == 1)
                    {
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        switch (SystemConfig.AppCenterDBType)
                        {
                            case BP.DA.DBType.Oracle:
                            case BP.DA.DBType.Informix:
                                qo.AddWhere(attr.Key, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                                break;
                            case BP.DA.DBType.MySQL:
                                qo.AddWhere(attr.Key, " LIKE ", "CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')");
                                break;
                            default:
                                qo.AddWhere(attr.Key, " LIKE ", " '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'");
                                break;
                        }
                        continue;
                    }
                    qo.addOr();

                    switch (SystemConfig.AppCenterDBType)
                    {
                        case BP.DA.DBType.Oracle:
                        case BP.DA.DBType.Informix:
                            qo.AddWhere(attr.Key, " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                            break;
                        case BP.DA.DBType.MySQL:
                            qo.AddWhere(attr.Key, " LIKE ", "CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')");
                            break;
                        default:
                            qo.AddWhere(attr.Key, " LIKE ", "'%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'");
                            break;
                    }
                }
                qo.MyParas.Add("SKey", keyVal);
                qo.addRightBracket();
            }
            else
            {
                qo.addLeftBracket();
                qo.AddWhere("abc", "all");
                qo.addRightBracket();
            }
            #endregion

            #region 普通属性
            string opkey = ""; // 操作符号。
            foreach (AttrOfSearch attr in attrsOfSearch)
            {
                if (attr.IsHidden)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.AddWhere(attr.RefAttrKey, attr.DefaultSymbol, attr.DefaultValRun);
                    qo.addRightBracket();
                    continue;
                }

                if (attr.SymbolEnable == true)
                {
                    opkey = this.GetDDLByKey("DDL_" + attr.Key).SelectedItemStringVal;
                    if (opkey == "all")
                        continue;
                }
                else
                {
                    opkey = attr.DefaultSymbol;
                }

                qo.addAnd();
                qo.addLeftBracket();

                if (attr.DefaultVal.Length >= 8)
                {
                    string date = "2005-09-01";
                    try
                    {
                        /* 就可能是年月日。 */
                        string y = this.GetDDLByKey("DDL_" + attr.Key + "_Year").SelectedItemStringVal;
                        string m = this.GetDDLByKey("DDL_" + attr.Key + "_Month").SelectedItemStringVal;
                        string d = this.GetDDLByKey("DDL_" + attr.Key + "_Day").SelectedItemStringVal;
                        date = y + "-" + m + "-" + d;

                        if (opkey == "<=")
                        {
                            DateTime dt = DataType.ParseSysDate2DateTime(date).AddDays(1);
                            date = dt.ToString(DataType.SysDataFormat);
                        }
                    }
                    catch
                    {
                    }

                    qo.AddWhere(attr.RefAttrKey, opkey, date);
                }
                else
                {
                    qo.AddWhere(attr.RefAttrKey, opkey, this.GetTBByID("TB_" + attr.Key).Text);
                }
                qo.addRightBracket();
            }
            #endregion

            #region 外键
            foreach (AttrSearch attr1 in searchAttrs)
            {
                Attr attr = attr1.HisAttr;

                if (attr.MyFieldType == FieldType.RefText)
                    continue;


                DDL ddl = this.GetDDLByKey("DDL_" + attr.Key);
                if (ddl.Items.Count == 0)
                    continue;

                string selectVal = ddl.SelectedItemStringVal;
                if (selectVal == "all")
                    continue;

                if (selectVal == "mvals")
                {
                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + this.EnsName + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(attr.Key);
                    if (instr == null || instr == "")
                    {
                        if (attr.Key == "FK_Dept" || attr.Key == "FK_Unit")
                        {
                            if (attr.Key == "FK_Dept")
                            {
                                selectVal = WebUser.FK_Dept;
                                ddl.SelectedIndex = 0;
                            }

                            //if (attr.Key == "FK_Unit")
                            //{
                            //    selectVal = WebUser.FK_Unit;
                            //    ddl.SelectedIndex = 0;
                            //}
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        instr = instr.Replace("..", ".");
                        instr = instr.Replace(".", "','");
                        instr = instr.Substring(2);
                        instr = instr.Substring(0, instr.Length - 2);

                        qo.addAnd();
                        qo.addLeftBracket();
                        qo.AddWhereIn(attr.Key, "(" + instr + ")");
                        qo.addRightBracket();
                        continue;
                    }
                }


                qo.addAnd();
                qo.addLeftBracket();

                if (attr.UIBindKey == "BP.Port.Depts" || attr.UIBindKey == "BP.Port.Units")  //判断特殊情况。
                    qo.AddWhere(attr.Key, " LIKE ", selectVal + "%");
                else
                    qo.AddWhere(attr.Key, selectVal);

                //qo.AddWhere(attr.Key,this.GetDDLByKey("DDL_"+attr.Key).SelectedItemStringVal ) ;
                qo.addRightBracket();
            }
            #endregion.

            if (dw != DTSearchWay.None)
            {
                string dtFrom = this.GetTBByID("TB_S_From").Text.Trim();
                string dtTo = this.GetTBByID("TB_S_To").Text.Trim();
                if (dw == DTSearchWay.ByDate)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = dtKey + " >= '" + dtFrom + " 01:01'";
                    qo.addAnd();
                    qo.SQL = dtKey + " <= '" + dtTo + " 23:59'";
                    qo.addRightBracket();

                    //qo.AddWhere(dtKey, ">=", dtFrom+" 01:01");
                    //qo.addAnd();
                    //qo.AddWhere(dtKey, "<=", dtTo + " 23:59");
                    //qo.addRightBracket();
                }

                if (dw == DTSearchWay.ByDateTime)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = dtKey + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = dtKey + " <= '" + dtTo + "'";
                    qo.addRightBracket();

                    //qo.addAnd();
                    //qo.addLeftBracket();
                    //qo.AddWhere(dtKey, ">=", dtFrom);
                    //qo.addAnd();
                    //qo.AddWhere(dtKey, "<=", dtTo);
                    //qo.addRightBracket();
                }
            }

            //  throw new Exception(qo.SQL);
            return qo;
        }
        public QueryObject GetnQueryObject(Entities ens, Entity en)
        {
            if (en.EnMap.EnDBUrl.DBType == DBType.Oracle)
                return this.GetnQueryObjectOracle(ens, en);

            QueryObject qo = this.InitQueryObjectByEns(ens, en.EnMap.IsShowSearchKey, en.EnMap.DTSearchWay, en.EnMap.DTSearchKey, en.EnMap.Attrs,
                en.EnMap.AttrsOfSearch, en.EnMap.SearchAttrs);

            switch (en.PK)
            {
                case "No":
                    qo.addOrderBy("No");
                    break;
                case "OID":
                    qo.addOrderBy("OID");
                    break;
                default:
                    break;
            }
            return qo;
        }

        public void SaveSearchState(string ensName, string key)
        {
            if (string.IsNullOrEmpty(ensName))
                throw new Exception("@EnsName 为空" + ensName);

            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + ensName + "_SearchAttrs";
            ur.RetrieveFromDBSources();
            ur.FK_Emp = WebUser.No;
            ur.CfgKey = "SearchAttrs";
            ur.SearchKey = key;

            if (key == "" || key == null)
            {
                try
                {
                    ur.SearchKey = this.GetTBByID("TB_Key").Text;
                }
                catch
                {
                }
            }

            //查询时间.
            try
            {
                ur.DTFrom_Data = this.GetTBByID("TB_S_From").Text;
                ur.DTTo_Data = this.GetTBByID("TB_S_To").Text;
                ur.DTFrom_Datatime = this.GetTBByID("TB_S_From").Text;
                ur.DTTo_Datatime = this.GetTBByID("TB_S_To").Text;
            }
            catch
            {
            }

            string str = "";
            foreach (Control ti in this.Controls)
            {
                if (ti.ID == null)
                    continue;

                if (ti.ID.IndexOf("DDL_") == -1)
                    continue;

                DDL ddl = (DDL)ti;
                if (ddl.Items.Count == 0)
                    continue;

                str += "@" + ti.ID + "=" + ddl.SelectedItemStringVal;
            }
            ur.FK_Emp = WebUser.No;
            ur.CfgKey = ensName + "_SearchAttrs";
            ur.Vals = str;
            try
            {
                ur.SearchKey = this.GetTBByID("TB_Key").Text;
            }
            catch
            {
            }
            ur.Save();
        }
        public void InitFuncEn(UAC uac, Entity en)
        {
            if (en.EnMap.EnType == EnType.View)
                uac.Readonly();

            //this.AddLab("Lab_Key1","标题:"+en.EnDesc);
            if (uac.IsInsert)
            {
                if (en.EnMap.EnType != EnType.Dtl)
                {
                    this.AddBtn(NamesOfBtn.New, "新建(N)");
                    this.GetBtnByID(NamesOfBtn.New).UseSubmitBehavior = false;
                    this.GetBtnByID(NamesOfBtn.New).OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                }
            }

            if (uac.IsUpdate)
            {
                this.AddBtn(NamesOfBtn.Save, "保存(S)");
                this.AddBtn(NamesOfBtn.SaveAndClose, "保存并关闭");

                this.GetBtnByID(NamesOfBtn.Save).UseSubmitBehavior = false;
                this.GetBtnByID(NamesOfBtn.Save).OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                // this.AddBtn(NamesOfBtn.SaveAndClose, this.ToE("SaveAndClose", "保存并关闭(C)")   );
            }

            if (uac.IsInsert && uac.IsUpdate)
            {
                if (en.EnMap.EnType != EnType.Dtl)
                {
                    this.AddBtn(NamesOfBtn.SaveAndNew, "保存并新建(R)");

                    this.GetBtnByID(NamesOfBtn.SaveAndNew).UseSubmitBehavior = false;
                    this.GetBtnByID(NamesOfBtn.SaveAndNew).OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                    //this.AddBtn(NamesOfBtn.SaveAndClose );
                }
            }

            string pkval = en.PKVal.ToString();

            if (uac.IsDelete && pkval != "0" && pkval.Length >= 1)
            {
                this.AddBtn(NamesOfBtn.Delete, "删除(D)");

                //  this.GetBtnByID(NamesOfBtn.Delete).UseSubmitBehavior = false;
                // this.GetBtnByID(NamesOfBtn.Delete).OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
            }

            if (uac.IsAdjunct)
            {
                this.AddBtn(NamesOfBtn.Adjunct);
                if (en.IsEmpty == false)
                {
                    int i = DBAccess.RunSQLReturnValInt("select COUNT(OID) from Sys_FileManager WHERE RefTable='" + en.ToString() + "' AND RefKey='" + en.PKVal + "'");
                    if (i != 0)
                    {
                        this.GetBtnByID(NamesOfBtn.Adjunct).Text += "-" + i;
                    }
                }
            }
        }
        public void InitToolbarOfMapRpt(BP.WF.Flow fl, BP.WF.Rpt.MapRpt currMapRpt, string RptNo, Entity en, int pageidx)
        {
            Map map = en.EnMap;
            this.InitByMapV2(map, 1, RptNo);

            //特殊处理权限.
            AttrSearchs searchs = map.SearchAttrs;
            string defVal = "";
            System.Data.DataTable dt = null;
            foreach (AttrSearch attr in searchs)
            {
                DDL mydll = this.GetDDLByKey("DDL_" + attr.Key);
                if (mydll == null)
                    continue;
                defVal = mydll.SelectedItemStringVal;
                mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + RptNo + "','" + attr.Key + "')";

                switch (attr.HisAttr.Key)
                {
                    case "FK_NY":
                        dt = DBAccess.RunSQLReturnTable("SELECT DISTINCT FK_NY FROM " + currMapRpt.PTable + " WHERE FK_NY!='' ORDER BY FK_NY");
                        mydll.Items.Clear();
                        mydll.Items.Add(new ListItem("=>月份", "all"));
                        foreach (DataRow dr in dt.Rows)
                            mydll.Items.Add(new ListItem(dr[0].ToString(), dr[0].ToString()));
                        mydll.SetSelectItem(defVal);
                        break;
                    case "FlowStarter":
                    case "FlowEnder":
                        string sql = "";
                        switch (fl.HisFlowDeptDataRightCtrlType)
                        {
                            case FlowDeptDataRightCtrlType.MyDeptOnly: // 我的部门.
                                sql = "SELECT No,Name FROM WF_Emp WHERE FK_Dept='" + WebUser.FK_Dept + "' AND No IN (SELECT DISTINCT FlowStarter FROM " + currMapRpt.PTable + " WHERE FlowStarter!='')";
                                break;
                            case FlowDeptDataRightCtrlType.MyDeptAndBeloneToMyDeptOnly: //我的部门，或者隶属我部门下面的部门.
#warning 这里有错误，怎么递归循环出来?
                                sql = "SELECT No,Name FROM WF_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "'  AND FK_Flow='" + currMapRpt.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + currMapRpt.PTable + " WHERE FlowStarter!='')";
                                break;
                            case FlowDeptDataRightCtrlType.BySpecFlowDept: // 指定权限.
                                sql = "SELECT No,Name FROM WF_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "'  AND FK_Flow='" + currMapRpt.FK_Flow + "') AND No IN (SELECT DISTINCT FlowStarter FROM " + currMapRpt.PTable + " WHERE FlowStarter!='')";
                                break;
                            case FlowDeptDataRightCtrlType.AnyoneAndAnydept:  // 任何部门.
                                sql = "SELECT No,Name FROM WF_Emp WHERE No IN (SELECT DISTINCT FlowStarter FROM " + currMapRpt.PTable + " WHERE FlowStarter!='')";
                                break;
                            default:
                                break;
                        }
                        dt = DBAccess.RunSQLReturnTable(sql);

                        mydll.Items.Clear();
                        if (attr.Key == NDXRptBaseAttr.FlowStarter)
                            mydll.Items.Add(new ListItem("=>发起人", "all"));
                        else
                            mydll.Items.Add(new ListItem("=>结束人", "all"));

                        foreach (DataRow dr in dt.Rows)
                        {
                            mydll.Items.Add(new ListItem(dr[1].ToString(), dr[0].ToString()));
                        }
                        mydll.SetSelectItem(defVal);
                        mydll.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + RptNo + "','" + attr.Key + "')";
                        break;
                    case "FK_Dept":
                        if (WebUser.No != "admin")
                        {
                            dt = DBAccess.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + currMapRpt.FK_Flow + "')");
                            if (dt.Rows.Count == 0)
                            {
                                BP.WF.Port.DeptFlowSearch dfs = new BP.WF.Port.DeptFlowSearch();
                                dfs.FK_Dept = WebUser.FK_Dept;
                                dfs.FK_Emp = WebUser.No;
                                dfs.FK_Flow = currMapRpt.FK_Flow;
                                dfs.MyPK = WebUser.FK_Dept + "_" + WebUser.No + "_" + currMapRpt.FK_Flow;
                                dfs.Insert();
                                dt = DBAccess.RunSQLReturnTable("SELECT No,Name,ParentNo FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM  WF_DeptFlowSearch WHERE FK_Emp='" + WebUser.No + "' AND FK_Flow='" + currMapRpt.FK_Flow + "')");
                            }
                            mydll.Items.Clear();
                            //部门下拉改为树形字符串展示，类似流程属性中的“流程类别”下拉，edited by liuxc,2016-11-7
                            //1.获取当前DataTable中的根节点部门
                            string root = string.Empty;

                            foreach(DataRow row in dt.Rows)
                            {
                                if(dt.Select(string.Format("No='{0}'", row["ParentNo"] == null || row["ParentNo"] == DBNull.Value ? string.Empty : row["ParentNo"].ToString())).Length == 0)
                                {
                                    root = row["ParentNo"].ToString();
                                    break;
                                }
                            }
                            //2.格式化字符串，增加项
                            DDL.MakeTree(dt, "ParentNo", root, "No", "Name", mydll, -1);
                            //foreach (DataRow dr in dt.Rows)
                            //    mydll.Items.Add(new ListItem(dr["Name"].ToString(), dr["No"].ToString()));
                        }

                        if (mydll.Items.Count >= 2)
                        {
                            ListItem liMvals = new ListItem("*多项组合..", "mvals");
                            liMvals.Attributes.CssStyle.Add("style", "color:green");
                            liMvals.Attributes.Add("color", "green");
                            liMvals.Attributes.Add("style", "color:green");
                        }
                        mydll.SetSelectItem(defVal);
                        break;
                    default:
                        break;
                }
            }
        }
        public void InitByMapV2(Map map, int page)
        {
            string str = this.Page.Request.QueryString["EnsName"];
            if (str == null)
                str = this.Page.Request.QueryString["EnsName"];

            if (str == null)
                return;

            InitByMapV2(map, page, str);
        }
        /// <summary>
        /// 初始化map
        /// </summary>
        /// <param name="map">map</param>
        /// <param name="i">选择的页</param>
        public void InitByMapV2(Map map, int page, string ensName)
        {
            UserRegedit ur = new UserRegedit(WebUser.No, ensName + "_SearchAttrs");
            string cfgKey = ur.Vals;
            this.InitByMapV2(map.IsShowSearchKey, map.DTSearchWay, map.AttrsOfSearch, map.SearchAttrs, null, page, ur);

            #region 设置默认值
            string[] keys = cfgKey.Split('@');
            foreach (Control ti in this.Controls)
            {
                if (ti.ID == null)
                    continue;

                if (ti.ID == "TB_Key")
                {
                    TB tb = (TB)ti;
                    tb.Text = ur.SearchKey;
                    continue;
                }

                if (ti.ID == "TB_S_From")
                {
                    TB tb = (TB)ti;
                    if (map.DTSearchWay == DTSearchWay.ByDate)
                    {
                        tb.Text = DateTime.Parse(ur.DTFrom_Datatime).ToString("yyyy-MM-dd"); //ur.DTFrom_Data;
                        tb.Attributes["onfocus"] = "WdatePicker();";
                    }
                    else
                        tb.Text = ur.DTFrom_Datatime;
                    continue;
                }

                if (ti.ID == "TB_S_To")
                {
                    TB tb = (TB)ti;
                    if (map.DTSearchWay == DTSearchWay.ByDate)
                    {
                        tb.Text = DateTime.Parse(ur.DTTo_Datatime).ToString("yyyy-MM-dd"); //ur.DTTo_Data;
                        tb.Attributes["onfocus"] = "WdatePicker();";
                    }
                    else
                        tb.Text = ur.DTFrom_Datatime;
                    continue;
                }


                if (ti.ID.IndexOf("DDL_") == -1)
                    continue;

                if (cfgKey.IndexOf(ti.ID) == -1)
                    continue;

                foreach (string key in keys)
                {
                    if (key.Length < 3)
                        continue;

                    if (key.IndexOf(ti.ID) == -1)
                        continue;

                    string[] vals = key.Split('=');

                    DDL ddl = (DDL)ti;
                    bool isHave = ddl.SetSelectItem(vals[1]);
                    if (isHave == false)
                    {
                        /*没有有找到要选择的人员*/
                        try
                        {
                            Attr attr = map.GetAttrByKey(vals[0].Replace("DDL_", ""));
                            ddl.SetSelectItem(vals[1], attr);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            #endregion 设置默认值

            #region 处理级联关系

            // 获取大类的选择集合。
            AttrSearchs bigAttrs = new AttrSearchs();
            foreach (AttrSearch attr in map.SearchAttrs)
            {
                if (attr.RelationalDtlKey == null)
                    continue;
                bigAttrs.Add(attr);
            }

            // 遍历他们，为他们生成事件。
            foreach (AttrSearch attr in bigAttrs)
            {
                AttrSearch smallAttr = null;
                foreach (AttrSearch attr1 in map.SearchAttrs)
                {
                    if (attr1.Key == attr.RelationalDtlKey)
                        smallAttr = attr1;
                }

                if (smallAttr == null)
                    throw new Exception("@您设置的级联子菜单键值在查询集合属性里面不存在。");

                Entities ens = smallAttr.HisAttr.HisFKEns;
                ens.RetrieveAll();
                Entity en = smallAttr.HisAttr.HisFKEn;

                // 增加事件.
                DDL ddl = this.GetDDLByID("DDL_" + attr.Key);
                // ddl.Attributes["onchange"] = "Redirect" + attr.Key + "(this.options.selectedIndex)";

                ddl.Attributes["onchange"] = "Redirect" + attr.Key + "()";
                DDL ddlSmil = this.GetDDLByID("DDL_" + attr.RelationalDtlKey);
                string script = "";
                // 判断级联的方式，是按照编号规则级联还是按照外键级联。
                if (en.EnMap.Attrs.Contains(attr.Key))
                {
                    /*按照外键或者枚举类型级联 */
                }
                else
                {
                    /*按照编号规则级联。*/
                    script = "\t\n<script type='text/javascript'>";
                    script += "\t\n<!--";
                    string arrayStrs = "";
                    bool isfirst = true;
                    foreach (EntityNoName en1 in ens)
                    {
                        if (isfirst)
                        {
                            isfirst = false;
                            arrayStrs += "[\"" + en1.Name + "\",\"" + en1.No + "\"]";
                        }
                        else
                        {
                            arrayStrs += ",[\"" + en1.Name + "\",\"" + en1.No + "\"]";
                        }
                    }
                    script += "\t\n var data" + attr.Key + " = new Array(" + arrayStrs + "); ";
                    script += "\t\n Redirect" + attr.Key + "();";
                    //数据联动
                    script += "\t\n function Redirect" + attr.Key + "(){";
                    // script += "\t\n alert('sss')";
                    script += "\t\n	var ddlBig" + attr.Key + " = document.getElementById(\"" + ddl.ClientID + "\");";
                    script += "\t\n	var ddlSmall" + attr.Key + " = document.getElementById(\"" + ddlSmil.ClientID + "\");";
                    script += "\t\n	var value_Big" + attr.Key + " = getSelectValue" + attr.Key + "( ddlBig" + attr.Key + " );";
                    script += "\t\n	var value_Big_length" + attr.Key + " = value_Big" + attr.Key + ".length;";
                    script += "\t\n	var index" + attr.Key + " = 0;";
                    script += "\t\n	ddlSmall" + attr.Key + ".options.length = 0;";
                    script += "\t\n	for(i=0; i<data" + attr.Key + ".length; i++){					";
                    script += "\t\n		if(data" + attr.Key + "[i][1].substr(0, value_Big_length" + attr.Key + ") == value_Big" + attr.Key + "){";
                    script += "\t\n			ddlSmall" + attr.Key + ".options[index" + attr.Key + "++] = new Option(data" + attr.Key + "[i][0],data" + attr.Key + "[i][1]);";
                    script += "\t\n		}";
                    script += "\t\n	}";
                    script += "\t\n	ddlSmall" + attr.Key + ".options[0].selected = true;";
                    script += "\t\n	}";
                    script += " //获取指定下拉列表的值";
                    script += "\t\n function getSelectValue" + attr.Key + "(oper) { ";
                    script += "\t\n	return oper.options[oper.options.selectedIndex].value;";
                    script += "\t\n	} ";
                    script += "\t\n	//-->";
                    script += "\t\n	</script> ";
                }
                // 注册他
                this.Page.RegisterClientScriptBlock(attr.Key, script);
            }
            #endregion
        }
        /// <summary>
        /// 查询方式
        /// </summary>
        /// <param name="isShowKey"></param>
        /// <param name="sw"></param>
        /// <param name="dtSearchKey"></param>
        /// <param name="attrsOfSearch"></param>
        /// <param name="attrsOfFK"></param>
        /// <param name="attrD1"></param>
        /// <param name="page"></param>
        /// <param name="ur"></param>
        public void InitByMapV2(bool isShowKey, DTSearchWay sw, AttrsOfSearch attrsOfSearch, AttrSearchs attrsOfFK, Attrs attrD1, int page, UserRegedit ur)
        {
            int keysNum = 0;
            // 关键字。
            if (isShowKey)
            {
                this.AddLab("Lab_Key", "关键字:&nbsp;");
                TB tb = new TB();
                tb.ID = "TB_Key";
                tb.Columns = 13;
                this.AddTB(tb);
                keysNum++;
            }
            this.Add("&nbsp;");

            if (sw != DTSearchWay.None)
            {
                Label lab = new Label();
                lab.ID = "Lab_From";
                lab.Text = "日期从:";
                this.Add(lab);
                TB tbDT = new TB();
                tbDT.ID = "TB_S_From";
                if (sw == DTSearchWay.ByDate)
                    tbDT.ShowType = TBType.Date;
                if (sw == DTSearchWay.ByDateTime)
                    tbDT.ShowType = TBType.DateTime;
                this.Add(tbDT);

                lab = new Label();
                lab.ID = "Lab_To";
                lab.Text = "到:";
                this.Add(lab);

                tbDT = new TB();
                tbDT.ID = "TB_S_To";
                if (sw == DTSearchWay.ByDate)
                    tbDT.ShowType = TBType.Date;
                if (sw == DTSearchWay.ByDateTime)
                    tbDT.ShowType = TBType.DateTime;
                this.Add(tbDT);
            }


            // 非外键属性。
            foreach (AttrOfSearch attr in attrsOfSearch)
            {
                if (attr.IsHidden)
                    continue;

                this.AddLab("Lab_" + attr.Key, attr.Lab);
                keysNum++;

                if (attr.SymbolEnable == true)
                {
                    DDL ddl = new DDL();
                    ddl.ID = "DDL_" + attr.Key;
                    ddl.SelfBindKey = "BP.Sys.Operators";
                    ddl.SelfEnsRefKey = "No";
                    ddl.SelfEnsRefKeyText = "Name";
                    ddl.SelfDefaultVal = attr.DefaultSymbol;
                    ddl.SelfAddAllLocation = AddAllLocation.None;
                    ddl.SelfIsShowVal = false; ///不让显示编号
                    //ddl.ID="DDL_"+attr.Key;
                    //ddl.SelfBind();
                    this.AddDDL(ddl);
                    this.GetDDLByKey("DDL_" + attr.Key).SelfBind();
                }

                if (attr.DefaultVal.Length >= 8)
                {
                    DateTime mydt = BP.DA.DataType.ParseSysDate2DateTime(attr.DefaultVal);

                    DDL ddl = new DDL();
                    ddl.ID = "DDL_" + attr.Key + "_Year";
                    ddl.SelfBindKey = "BP.Pub.NDs";
                    ddl.SelfEnsRefKey = "No";
                    ddl.SelfEnsRefKeyText = "Name";
                    ddl.SelfDefaultVal = mydt.ToString("yyyy");
                    ddl.SelfAddAllLocation = AddAllLocation.None;
                    ddl.SelfIsShowVal = false; ///不让显示编号
                    this.AddDDL(ddl);
                    ddl.SelfBind();
                    //ddl.SelfBind();

                    ddl = new DDL();
                    ddl.ID = "DDL_" + attr.Key + "_Month";
                    ddl.SelfEnsRefKey = "No";
                    ddl.SelfEnsRefKeyText = "Name";
                    ddl.SelfDefaultVal = mydt.ToString("MM");
                    ddl.SelfAddAllLocation = AddAllLocation.None;
                    ddl.SelfIsShowVal = false; ///不让显示编号
                    //	ddl.SelfBind();
                    this.AddDDL(ddl);
                    ddl.SelfBind();

                    ddl = new DDL();
                    ddl.ID = "DDL_" + attr.Key + "_Day";
                    ddl.SelfBindKey = "BP.Pub.Days";
                    ddl.SelfEnsRefKey = "No";
                    ddl.SelfEnsRefKeyText = "Name";
                    ddl.SelfDefaultVal = mydt.ToString("dd");
                    ddl.SelfAddAllLocation = AddAllLocation.None;
                    ddl.SelfIsShowVal = false; ///不让显示编号
                    //ddl.SelfBind();
                    this.AddDDL(ddl);
                    this.GetDDLByKey(ddl.ID).SelfBind();

                }
                else
                {
                    TB tb = new TB();
                    tb.ID = "TB_" + attr.Key;
                    tb.Text = attr.DefaultVal;
                    tb.Columns = attr.TBWidth;
                    this.AddTB(tb);
                }
            }

            string ensName = this.Page.Request.QueryString["EnsName"];
            string cfgVal = "";

            cfgVal = ur.Vals;


            // 外键属性查询。			 
            bool isfirst = true;
            foreach (AttrSearch attr1 in attrsOfFK)
            {
                Attr attr = attr1.HisAttr;

                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                DDL ddl = new DDL();
                ddl.ID = "DDL_" + attr.Key;
                this.AddDDL(ddl);
                keysNum++;
                //if (keysNum == 3 || keysNum == 6 || keysNum == 9)
                //    this.AddBR("b_" + keysNum);
                if (attr.MyFieldType == FieldType.Enum)
                {
                    this.GetDDLByKey("DDL_" + attr.Key).BindSysEnum(attr.UIBindKey, false, AddAllLocation.TopAndEndWithMVal);
                    this.GetDDLByKey("DDL_" + attr.Key).Items[0].Text = ">>" + attr.Desc;

                    this.GetDDLByKey("DDL_" + attr.Key).Attributes["onclick"] = "DDL_mvals_OnChange(this,'" + ensName + "','" + attr.Key + "')";
                    // this.GetDDLByKey("DDL_" + attr.Key).Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + ur.MyPK + "','" + attr.Key + "')";

                    // ddl.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + ur.MyPK + "','" + attr.Key + "')";
                }
                else
                {
                    ListItem liMvals = new ListItem("*多项组合..", "mvals");
                    liMvals.Attributes.CssStyle.Add("style", "color:green");
                    liMvals.Attributes.Add("color", "green");
                    liMvals.Attributes.Add("style", "color:green");

                    // liMvals.Attributes.Add("onclick", "alert('sss')");

                    switch (attr.UIBindKey)
                    {
                        case "BP.Port.Depts":
                            ddl.Items.Clear();
                            BP.Port.Depts depts = new BP.Port.Depts();
                            depts.RetrieveAll();
                            //edited by liuxc,2016-11-7,将部门下拉中的项格式化成树形字符串展示，类似“流程属性”中的流程类别下拉显示
                            //foreach (BP.Port.Dept dept in depts)
                            //{
                            //    string space = "";
                            //    //   space = space.PadLeft(dept.Grade - 1, '-');
                            //    ListItem li = new ListItem(space + dept.Name, dept.No);
                            //    this.GetDDLByKey("DDL_" + attr.Key).Items.Add(li);
                            //}
                            //格式化项字符串，增加项
                            DataTable dt = depts.ToDataTableField();
                            DDL.MakeTree(dt, "ParentNo", "0", "No", "Name", ddl, -1);

                            if (depts.Count > SystemConfig.MaxDDLNum)
                                this.AddLab("lD", "<a href=\"javascript:onDDLSelectedMore('DDL_" + attr.Key + "', '" + this.EnsName + "', 'BP.Port.Depts', 'No','Name')\" >...</a>");

                            if (ddl.Items.Count >= 2)
                                ddl.Items.Add(liMvals);

                            ddl.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + ensName + "','" + attr.Key + "')";
                            break;
                        //case "BP.Port.Units":
                        //    ddl.Items.Clear();
                        //    BP.Port.Units units = new BP.Port.Units();
                        //    units.RetrieveAll();
                        //    foreach (BP.Port.Unit unit in units)
                        //    {
                        //        string space = "";
                        //        space = space.PadLeft(unit.No.Length / 2 - 1, '-');
                        //        ListItem li = new ListItem(space + unit.Name, unit.No);
                        //        this.GetDDLByKey("DDL_" + attr.Key).Items.Add(li);
                        //    }
                        //    if (units.Count > SystemConfig.MaxDDLNum)
                        //        this.AddLab("lD", "<a href=\"javascript:onDDLSelectedMore('DDL_" + attr.Key + "', '" + this.EnsName + "', 'BP.Port.Units', 'No','Name')\" >...</a>");

                        //    if (ddl.Items.Count >= 2)
                        //        ddl.Items.Add(liMvals);

                        //    ddl.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + ensName + "','" + attr.Key + "')";
                        // break;
                        default:
                            ddl.Items.Clear();
                            if (attr.MyDataType == DataType.AppBoolean)
                            {
                                ddl.Items.Add(new ListItem(">>" + attr.Desc, "all"));
                                ddl.Items.Add(new ListItem("是", "1"));
                                ddl.Items.Add(new ListItem("否", "0"));
                                break;
                            }
                            Entities ens = attr.HisFKEns;
                            ens.RetrieveAll();
                            ddl.Items.Add(new ListItem(">>" + attr.Desc, "all"));
                            foreach (Entity en in ens)
                                ddl.Items.Add(new ListItem(en.GetValStrByKey("Name"), en.GetValStrByKey("No")));

                            if (ddl.Items.Count >= 2)
                                ddl.Items.Add(liMvals);

                            ddl.Attributes["onchange"] = "DDL_mvals_OnChange(this,'" + ensName + "','" + attr.Key + "')";
                            break;
                    }
                }
                if (isfirst)
                    isfirst = false;
            }
            if (_AddSearchBtn)
                this.AddLinkBtn(NamesOfBtn.Search, " 查询 ");
        }
    }

}