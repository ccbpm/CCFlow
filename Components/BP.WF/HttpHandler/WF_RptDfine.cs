using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Rpt;
using BP.WF.Template;
using BP.Web.Controls;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_RptDfine : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_RptDfine(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 流程列表
        /// </summary>
        /// <returns></returns>
        public string Flowlist_Init()
        {
            DataSet ds = new DataSet();
            string sql = "SELECT No,Name,ParentNo FROM WF_FlowSort ORDER BY ParentNo, Idx";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sort";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }
            ds.Tables.Add(dt);

            sql = "SELECT No,Name,FK_FlowSort FROM WF_Flow ORDER BY FK_FlowSort, Idx";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Flows";
            if (SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        # region 属性
        public string RptNo
        {
            get
            {
                return this.getUTF8ToString("RptNo");
            }
        }
        #endregion

        #region 功能列表
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("MyStartFlow", "我发起的流程");
            ht.Add("MyJoinFlow", "我参与的流程");

            RptDfine rd = new RptDfine(this.FK_Flow);

            #region 增加本部门发起流程的查询.
            if (rd.MyDeptRole == 0)
            {
                /*如果仅仅部门领导可以查看: 检查当前人是否是部门领导人.*/
                if (DBAccess.IsExitsTableCol("Port_Dept", "Leader") == true)
                {
                    string sql = "SELECT Leader FROM Port_Dept WHERE No='" + BP.Web.WebUser.FK_Dept + "'";
                    string strs = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    if (strs != null && strs.Contains(BP.Web.WebUser.No) == true)
                    {
                        ht.Add("MyDeptFlow", "我本部门发起的流程");
                    }
                }
            }

            if (rd.MyDeptRole == 1)
            {
                /*如果部门下所有的人都可以查看: */
                ht.Add("MyDeptFlow", "我本部门发起的流程");
            }

            if (rd.MyDeptRole == 2)
            {
                /*如果部门下所有的人都可以查看: */
                ht.Add("MyDeptFlow", "我本部门发起的流程");
            }

            #endregion 增加本部门发起流程的查询.



            //   ht.Add("MyDeptFlow", "我本部门发起的流程");
            //  ht.Add("MySubDeptFlow", "我本部门与子部门发起的流程");
            // ht.Add("AdvFlowsSearch", "高级查询");

            return BP.Tools.Json.ToJsonEntitiesNoNameMode(ht);
        }
        #endregion

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        #region MyStartFlow.htm 我发起的流程
        public string MyStartFlow_Init()
        {
            if (string.IsNullOrWhiteSpace(this.RptNo))
                return "err@参数RptNo不能为空";

            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                return "err@参数FK_Flow不能为空";

            string pageSize = getUTF8ToString("pageSize");
            string fcid = string.Empty;
            DataSet ds = new DataSet();
            Dictionary<string, string> vals = null;
            string rptmd = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            MapAttr ar = null;

            //报表信息，包含是否显示关键字查询RptIsSearchKey，过滤条件枚举/下拉字段RptSearchKeys，时间段查询方式RptDTSearchWay，时间字段RptDTSearchKey
            MapData md = new MapData();
            md.No = this.RptNo;
            md.RetrieveFromDBSources();

            string cfgfix = "_SearchAttrs";
            UserRegedit ur = new UserRegedit();
            ur.AutoMyPK = false;
            ur.MyPK = WebUser.No + RptNo + cfgfix;

            if (ur.RetrieveFromDBSources() == 0)
            {
                ur.MyPK = WebUser.No + RptNo + cfgfix;
                ur.FK_Emp = WebUser.No;
                ur.CfgKey = RptNo + cfgfix;
                ur.Insert();
            }

            vals = ur.GetVals();
            md.SetPara("T_SearchKey", ur.SearchKey);

            if(md.RptDTSearchWay != DTSearchWay.None)
            {
                ar = new MapAttr(rptmd, md.RptDTSearchKey);
                md.SetPara("T_DateLabel", ar.Name);

                if(md.RptDTSearchWay == DTSearchWay.ByDate)
                {
                    md.SetPara("T_DTFrom", ur.GetValStringByKey(UserRegeditAttr.DTFrom));
                    md.SetPara("T_DTTo", ur.GetValStringByKey(UserRegeditAttr.DTTo));
                }
                else
                {
                    md.SetPara("T_DTFrom", ur.GetValStringByKey(UserRegeditAttr.DTFrom));
                    md.SetPara("T_DTTo", ur.GetValStringByKey(UserRegeditAttr.DTTo));
                }
            }

            #region //增加显示列信息
            DataRow row = null;
            DataTable dt = new DataTable("Sys_MapAttr");
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));

            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, RptNo, MapAttrAttr.Idx);

            foreach (MapAttr attr in attrs)
            {
                row = dt.NewRow();
                row["No"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;

                if (attr.HisAttr.IsFKorEnum)
                    row["No"] = attr.KeyOfEn + "Text";

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            #endregion

            #region //增加枚举/外键字段信息
            attrs = new MapAttrs(rptmd);
            dt = new DataTable("FilterCtrls");
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("DataType", typeof(int));
            dt.Columns.Add("DefaultValue", typeof(string));
            dt.Columns.Add("ValueField", typeof(string));
            dt.Columns.Add("TextField", typeof(string));
            dt.Columns.Add("ParentField", typeof(string));
            string[] ctrls = md.RptSearchKeys.Split('*');
            DataTable dtNoName = null;

            foreach (string ctrl in ctrls)
            {
                //增加判断，如果URL中有传参，则不进行此SearchAttr的过滤条件显示
                if (string.IsNullOrWhiteSpace(ctrl) || !string.IsNullOrEmpty(context.Request.QueryString[ctrl]))
                    continue;

                ar = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ctrl) as MapAttr;
                if (ar == null)
                    continue;

                row = dt.NewRow();
                row["Id"] = ctrl;
                row["Name"] = ar.Name;
                row["DataType"] = ar.MyDataType;

                switch (ar.UIContralType)
                {
                    case UIContralType.DDL:
                        row["Type"] = "combo";
                        fcid = "DDL_" + ar.KeyOfEn;

                        if (vals.ContainsKey(fcid))
                        {
                            if (vals[fcid] == "mvals")
                            {
                                AtPara ap = new AtPara(ur.MVals);
                                row["DefaultValue"] = ap.GetValStrByKey(ar.KeyOfEn);
                            }
                            else
                            {
                                row["DefaultValue"] = vals[fcid];
                            }
                        }

                        switch (ar.LGType)
                        {
                            //case DDLShowType.BindSQL:
                            //    string sql = ar.UIBindKey;

                            //    if (sql.Contains("@Web"))
                            //    {
                            //        sql = sql.Replace("@WebUser.No", WebUser.No);
                            //        sql = sql.Replace("@WebUser.Name", WebUser.Name);
                            //        sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                            //        sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);
                            //    }

                            //    if (sql.Contains("@"))
                            //        throw new Exception("不允许使用除@WebUser之外的变量");

                            //    dtNoName = DBAccess.RunSQLReturnTable(sql);
                            //    dtNoName.TableName = ar.KeyOfEn;
                            //    ds.Tables.Add(dtNoName);

                            //    row["ValueField"] = "No";
                            //    row["TextField"] = "Name";
                            //    break;
                            //case DDLShowType.Boolean:
                            //    dtNoName = GetNoNameDataTable(ar.KeyOfEn);
                            //    dtNoName.Rows.Add("all", "全部");
                            //    dtNoName.Rows.Add("1", "是");
                            //    dtNoName.Rows.Add("0", "否");
                            //    ds.Tables.Add(dtNoName);

                            //    row["ValueField"] = "No";
                            //    row["TextField"] = "Name";
                            //    break;
                            case FieldTypeS.FK:
                                Entities ens = ar.HisAttr.HisFKEns;
                                ens.RetrieveAll();
                                EntitiesTree treeEns = ens as EntitiesTree;

                                if (treeEns != null)
                                {
                                    row["Type"] = "combotree";
                                    dtNoName = ens.ToDataTableField();
                                    dtNoName.TableName = ar.KeyOfEn;
                                    ds.Tables.Add(dtNoName);

                                    row["ValueField"] = "No";
                                    row["TextField"] = "Name";
                                    row["ParentField"] = "ParentNo";
                                }
                                else
                                {
                                    EntitiesSimpleTree treeSimpEns = ens as EntitiesSimpleTree;

                                    if (treeSimpEns != null)
                                    {
                                        row["Type"] = "combotree";
                                        dtNoName = ens.ToDataTableField();
                                        dtNoName.TableName = ar.KeyOfEn;
                                        ds.Tables.Add(dtNoName);

                                        row["ValueField"] = "No";
                                        row["TextField"] = "Name";
                                        row["ParentField"] = "ParentNo";
                                    }
                                    else
                                    {
                                        dtNoName = GetNoNameDataTable(ar.KeyOfEn);
                                        dtNoName.Rows.Add("all", "全部");

                                        foreach (Entity en in ens)
                                        {
                                            dtNoName.Rows.Add(en.GetValStringByKey(ar.HisAttr.UIRefKeyValue),
                                                    en.GetValStringByKey(ar.HisAttr.UIRefKeyText));
                                        }

                                        ds.Tables.Add(dtNoName);

                                        row["ValueField"] = "No";
                                        row["TextField"] = "Name";
                                    }
                                }
                                break;
                            //case DDLShowType.Gender:
                            //    dtNoName = GetNoNameDataTable(ar.KeyOfEn);
                            //    dtNoName.Rows.Add("all", "全部");
                            //    dtNoName.Rows.Add("1", "男");
                            //    dtNoName.Rows.Add("0", "女");
                            //    ds.Tables.Add(dtNoName);

                            //    row["ValueField"] = "No";
                            //    row["TextField"] = "Name";
                            //    break;
                            case FieldTypeS.Enum:
                                dtNoName = GetNoNameDataTable(ar.KeyOfEn);
                                dtNoName.Rows.Add("all", "全部");

                                SysEnums enums = new SysEnums(ar.UIBindKey);

                                foreach (SysEnum en in enums)
                                    dtNoName.Rows.Add(en.IntKey.ToString(), en.Lab);

                                ds.Tables.Add(dtNoName);

                                row["ValueField"] = "No";
                                row["TextField"] = "Name";
                                break;
                            default:
                                break;
                        }
                        break;
                    //case UIContralType.CheckBok:
                    //    row["Type"] = "checkbox";
                    //    fcid = "CB_" + ar.KeyOfEn;

                    //    if (vals.ContainsKey(fcid))
                    //        row["DefaultValue"] = Convert.ToBoolean(int.Parse(vals[fcid]));
                    //    break;
                    default:
                        break;
                }

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            #endregion

            #region //增加第一页数据
            GEEntitys ges = new GEEntitys(rptmd);
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
            qo = InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            md.SetPara("T_total", qo.GetCount());
            qo.DoQuery("OID", string.IsNullOrWhiteSpace(pageSize) ? SystemConfig.PageSize : int.Parse(pageSize), 1);
            ds.Tables.Add(ges.ToDataTableField("MainData"));
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));
            #endregion

            return BP.Tools.Json.DataSetToJson(ds, true);
        }

        public string MyStartFlow_Search()
        {
            string vals = getUTF8ToString("vals");
            string searchKey = getUTF8ToString("key");
            string dtFrom = getUTF8ToString("dtFrom");
            string dtTo = getUTF8ToString("dtTo");
            string mvals = getUTF8ToString("mvals");
            string pagesize = getUTF8ToString("pageSize");
            int pageidx = int.Parse(getUTF8ToString("pageIdx"));
            string rptmd = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + RptNo + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            ur.SearchKey = searchKey;
            ur.DTFrom_Data = dtFrom;
            ur.DTTo_Data = dtTo;
            ur.Vals = vals;
            ur.MVals = mvals;
            ur.Update();

            DataSet ds = new DataSet();
            MapData md = new MapData(RptNo);
            MapAttrs attrs = new MapAttrs(rptmd);
            GEEntitys ges = new GEEntitys(rptmd);
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
            qo = InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            md.SetPara("T_total", qo.GetCount());
            qo.DoQuery("OID", string.IsNullOrWhiteSpace(pagesize) ? SystemConfig.PageSize : int.Parse(pagesize), pageidx);
            ds.Tables.Add(ges.ToDataTableField("MainData"));
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            return BP.Tools.Json.DataSetToJson(ds, true);
        }

        /// <summary>
        /// 初始化QueryObject
        /// </summary>
        /// <param name="qo"></param>
        /// <returns></returns>
        private QueryObject InitQueryObject(QueryObject qo, MapData md, Attrs attrs, MapAttrs rptAttrs, UserRegedit ur)
        {
            Dictionary<string, string> kvs = null;
            List<string> keys = new List<string>();
            string cfg = "_SearchAttrs";
            string searchKey = "";
            string val = null;

            kvs = ur.GetVals();
            qo.addAnd();

            #region 关键字查询
            if (md.RptIsSearchKey)
                searchKey = ur.SearchKey;

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                qo.addLeftBracket();
                qo.AddWhere("abc", "all");
                qo.addRightBracket();
            }
            else
            {
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
                        qo.addLeftBracket();
                        if (SystemConfig.AppCenterDBVarStr == "@")
                            qo.AddWhere(attr.Key, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                        continue;
                    }

                    qo.addOr();

                    if (SystemConfig.AppCenterDBVarStr == "@")
                        qo.AddWhere(attr.Key, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                    else
                        qo.AddWhere(attr.Key, " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                }

                qo.MyParas.Add("SKey", searchKey);
                qo.addRightBracket();
            }
            #endregion

            #region Url传参条件
            foreach (Attr attr in attrs)
            {
                if (string.IsNullOrEmpty(context.Request.QueryString[attr.Key]))
                    continue;

                qo.addAnd();
                qo.addLeftBracket();

                val = context.Request.QueryString[attr.Key];

                switch (attr.MyDataType)
                {
                    case DataType.AppBoolean:
                        qo.AddWhere(attr.Key, Convert.ToBoolean(int.Parse(val)));
                        break;
                    case DataType.AppDate:
                    case DataType.AppDateTime:
                    case DataType.AppString:
                        qo.AddWhere(attr.Key, val);
                        break;
                    case DataType.AppDouble:
                    case DataType.AppFloat:
                    case DataType.AppMoney:
                        qo.AddWhere(attr.Key, double.Parse(val));
                        break;
                    case DataType.AppInt:
                        qo.AddWhere(attr.Key, int.Parse(val));
                        break;
                    default:
                        break;
                }

                qo.addRightBracket();

                if (keys.Contains(attr.Key) == false)
                    keys.Add(attr.Key);
            }
            #endregion

            #region 过滤条件
            foreach (MapAttr attr1 in rptAttrs)
            {
                Attr attr = attr1.HisAttr;
                //此处做判断，如果在URL中已经传了参数，则不算SearchAttrs中的设置
                if (keys.Contains(attr.Key))
                    continue;

                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                string selectVal = string.Empty;
                string cid = string.Empty;

                switch (attr.UIContralType)
                {
                    //case UIContralType.TB:
                    //    switch (attr.MyDataType)
                    //    {
                    //        case DataType.AppDate:
                    //        case DataType.AppDateTime:
                    //            if (attr.MyDataType == DataType.AppDate)
                    //                cid = "D_" + attr.Key;
                    //            else
                    //                cid = "DT_" + attr.Key;

                    //            if (kvs.ContainsKey(cid) == false || string.IsNullOrWhiteSpace(kvs[cid]))
                    //                continue;

                    //            selectVal = kvs[cid];

                    //            qo.addAnd();
                    //            qo.addLeftBracket();
                    //            qo.AddWhere(attr.Key, selectVal);
                    //            qo.addRightBracket();
                    //            break;
                    //        default:
                    //            cid = "TB_" + attr.Key;

                    //            if (kvs.ContainsKey(cid) == false || string.IsNullOrWhiteSpace(kvs[cid]))
                    //                continue;

                    //            selectVal = kvs[cid];

                    //            qo.addAnd();
                    //            qo.addLeftBracket();
                    //            qo.AddWhere(attr.Key, " LIKE ", "%" + selectVal + "%");
                    //            qo.addRightBracket();
                    //            break;
                    //    }
                    //    break;
                    case UIContralType.DDL:
                        cid = "DDL_" + attr.Key;

                        if (kvs.ContainsKey(cid) == false || string.IsNullOrWhiteSpace(kvs[cid]))
                            continue;

                        selectVal = kvs[cid];

                        if (selectVal == "all" || selectVal == "-1")
                            continue;

                        if (selectVal == "mvals")
                        {
                            /* 如果是多选值 */
                            AtPara ap = new AtPara(ur.MVals);
                            string instr = ap.GetValStrByKey(attr.Key);

                            if (string.IsNullOrEmpty(instr))
                            {
                                if (attr.Key == "FK_Dept" || attr.Key == "FK_Unit")
                                {
                                    if (attr.Key == "FK_Dept")
                                    {
                                        selectVal = WebUser.FK_Dept;
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

                        qo.addRightBracket();
                        break;
                    //case UIContralType.CheckBok:
                    //    cid = "CB_" + attr.Key;

                    //    if (kvs.ContainsKey(cid) == false || string.IsNullOrWhiteSpace(kvs[cid]))
                    //        continue;

                    //    selectVal = kvs[cid];

                    //    qo.addAnd();
                    //    qo.addLeftBracket();
                    //    qo.AddWhere(attr.Key, int.Parse(selectVal));
                    //    qo.addRightBracket();
                    //    break;
                    default:
                        break;
                }
            }
            #endregion

            #region 日期处理
            if (md.RptDTSearchWay != DTSearchWay.None)
            {
                string dtKey = md.RptDTSearchKey;
                string dtFrom = ur.GetValStringByKey(UserRegeditAttr.DTFrom).Trim();
                string dtTo = ur.GetValStringByKey(UserRegeditAttr.DTTo).Trim();

                if (string.IsNullOrEmpty(dtFrom) == true)
                {
                    if (md.RptDTSearchWay == DTSearchWay.ByDate)
                        dtFrom = "1900-01-01";
                    else
                        dtFrom = "1900-01-01 00:00";
                }

                if (string.IsNullOrEmpty(dtTo) == true)
                {
                    if (md.RptDTSearchWay == DTSearchWay.ByDate)
                        dtFrom = "2999-01-01";
                    else
                        dtFrom = "2999-01-01 00:00";
                }

                if (md.RptDTSearchWay == DTSearchWay.ByDate)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = dtKey + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = dtKey + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }

                if (md.RptDTSearchWay == DTSearchWay.ByDateTime)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = dtKey + " >= '" + dtFrom + " 00:00'";
                    qo.addAnd();
                    qo.SQL = dtKey + " <= '" + dtTo + " 23:59'";
                    qo.addRightBracket();
                }
            }
            #endregion

            return qo;
        }

        private DataTable GetNoNameDataTable(string tableName)
        {
            DataTable dt = new DataTable(tableName);
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));

            return dt;
        }
        #endregion MyStartFlow.htm 我发起的流程

        /// <summary>
        /// 我的流程查询.
        /// </summary>
        /// <returns></returns>
        //public string MyStartFlow_Init()
        //{
        //    string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "RptMy";

        //    DataSet ds = new DataSet();

        //    //字段描述.
        //    MapAttrs attrs = new MapAttrs(fk_mapdata);
        //    DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
        //    ds.Tables.Add(dtAttrs);

        //    //数据.
        //    GEEntitys ges = new GEEntitys(fk_mapdata);

        //    //设置查询条件.
        //    QueryObject qo = new QueryObject(ges);
        //    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);

        //    //查询.
        //    // qo.DoQuery(BP.WF.Data.GERptAttr.OID, 15, this.PageIdx);

        //    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
        //    {
        //        DataTable dt = qo.DoQueryToTable();
        //        dt.TableName = "dt";
        //        ds.Tables.Add(dt);
        //    }
        //    else
        //    {
        //        qo.DoQuery();
        //        ds.Tables.Add(ges.ToDataTableField("dt"));
        //    }
        //    return BP.Tools.Json.DataSetToJson(ds, false);
        //}

        public string MyDeptFlow_Init()
        {
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "RptMyDept";

            DataSet ds = new DataSet();

            //字段描述.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //数据.
            GEEntitys ges = new GEEntitys(fk_mapdata);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);

            //查询.
            // qo.DoQuery(BP.WF.Data.GERptAttr.OID, 15, this.PageIdx);

            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                DataTable dt = qo.DoQueryToTable();
                dt.TableName = "dt";
                ds.Tables.Add(dt);
            }
            else
            {
                qo.DoQuery();
                ds.Tables.Add(ges.ToDataTableField("dt"));
            }
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string MyJoinFlow_Init()
        {
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "RptMyJoin";

            DataSet ds = new DataSet();

            //字段描述.
            MapAttrs attrs = new MapAttrs(fk_mapdata);
            DataTable dtAttrs = attrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(dtAttrs);

            //数据.
            GEEntitys ges = new GEEntitys(fk_mapdata);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);
            qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");

            if (SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                DataTable dt = qo.DoQueryToTable();
                dt.TableName = "dt";
                ds.Tables.Add(dt);
            }
            else
            {
                qo.DoQuery();
                ds.Tables.Add(ges.ToDataTableField("dt"));
            }
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
    }
}
