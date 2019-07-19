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
        #region 属性.
        /// <summary>
        /// 查询类型
        /// </summary>
        public string SearchType
        {
            get
            {
                string val = this.GetRequestVal("SearchType");

                if(val==null || val.Equals(""))
                    val = this.GetRequestVal("GroupType");
                return val;
            }
           
           
        }

        /// <summary>
        /// 分析类型
        /// </summary>
        public string GroupType
        {
            get
            { 
                return this.GetRequestVal("GroupType");
            }
        }

        public bool IsContainsNDYF
        {
            get
            {
                return this.GetRequestValBoolen("IsContainsNDYF");
            }
        }

        /// <summary>
        /// 部门编号
        /// </summary>
        public string FK_Dept
        {
            get
            {
                string str = this.GetRequestVal("FK_Dept");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
            set
            {
                string val = value;
                if (val == "all")
                    return;

                if (this.FK_Dept == null)
                {
                    this.FK_Dept = value;
                    return;
                }
            }

        }
        #endregion 属性.

        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_RptDfine(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_RptDfine()
        {
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
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PARENTNO"].ColumnName = "ParentNo";
            }
            ds.Tables.Add(dt);

            sql = "SELECT No,Name,FK_FlowSort FROM WF_Flow WHERE IsCanStart=1 ORDER BY FK_FlowSort, Idx";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Flows";
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
            }
            ds.Tables.Add(dt);

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        
        #region 功能列表
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public string Default_Init()
        {
            Hashtable ht = new Hashtable();
            ht.Add("My", "我发起的流程");
            ht.Add("MyJoin", "我审批的流程");

            RptDfine rd = new RptDfine(this.FK_Flow);
            Paras ps = new Paras();
            #region 增加本部门发起流程的查询.
            if (rd.MyDeptRole == 0)
            {
                /*如果仅仅部门领导可以查看: 检查当前人是否是部门领导人.*/
                if (DBAccess.IsExitsTableCol("Port_Dept", "Leader") == true)
                {
                    ps.SQL = "SELECT Leader FROM Port_Dept WHERE No=" + SystemConfig.AppCenterDBVarStr + "No";
                    ps.Add("No", BP.Web.WebUser.FK_Dept);
                    //string sql = "SELECT Leader FROM Port_Dept WHERE No='" + BP.Web.WebUser.FK_Dept + "'";
                    string strs = DBAccess.RunSQLReturnStringIsNull(ps, null);
                    if (strs != null && strs.Contains(BP.Web.WebUser.No) == true)
                    {
                        ht.Add("MyDept", "我本部门发起的流程");
                    }
                }
            }

            if (rd.MyDeptRole == 1)
            {
                /*如果部门下所有的人都可以查看: */
                ht.Add("MyDept", "我本部门发起的流程");
            }

            if (rd.MyDeptRole == 2)
            {
                /*如果部门下所有的人都可以查看: */
                ht.Add("MyDept", "我本部门发起的流程");
            }
            #endregion 增加本部门发起流程的查询.

            Flow fl = new Flow(this.FK_Flow);
            ht.Add("FlowName", fl.Name);

            string advEmps = SystemConfig.AppSettings["AdvEmps"];
            if (advEmps != null && advEmps.Contains(BP.Web.WebUser.No) == true)
            {
                ht.Add("Adminer", "高级查询");
            }
            else
            {
                string data = fl.GetParaString("AdvSearchRight");
                data=","+data+",";
                if (data.Contains(BP.Web.WebUser.No + ",") == true)
                {
                    ht.Add("Adminer", "高级查询");
                }
            }

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
        public string FlowSearch_Init()
        {
            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                return "err@参数FK_Flow不能为空";

            string pageSize = GetRequestVal("pageSize");
            string fcid = string.Empty;
            DataSet ds = new DataSet();
            Dictionary<string, string> vals = null;
            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.SearchType;

            //报表信息，包含是否显示关键字查询RptIsSearchKey，过滤条件枚举/下拉字段RptSearchKeys，时间段查询方式RptDTSearchWay，时间字段RptDTSearchKey
            MapData md = new MapData();
            md.No = rptNo;
            if (md.RetrieveFromDBSources() == 0)
            {
                /*如果没有找到，就让其重置一下.*/
                BP.WF.Rpt.RptDfine rd = new RptDfine(this.FK_Flow);

                if (this.SearchType == "My")
                    rd.DoReset(this.SearchType, "我发起的流程");

                if (this.SearchType == "MyJoin")
                    rd.DoReset(this.SearchType, "我审批的流程");

                if(this.SearchType =="MyDept")
                    rd.DoReset(this.SearchType, "本部门发起的流程");

                if (this.SearchType == "Adminer")
                    rd.DoReset(this.SearchType, "高级查询");

                md.RetrieveFromDBSources();
            }

            MapAttr ar = null;

            string cfgfix = "_SearchAttrs";
            UserRegedit ur = new UserRegedit();
            ur.AutoMyPK = false;
            ur.MyPK = WebUser.No + rptNo + cfgfix;

            if (ur.RetrieveFromDBSources() == 0)
            {
                ur.MyPK = WebUser.No + rptNo + cfgfix;
                ur.FK_Emp = WebUser.No;
                ur.CfgKey = rptNo + cfgfix;
              
                ur.Insert();
            }

            vals = ur.GetVals();
            md.SetPara("RptDTSearchWay", (int)md.RptDTSearchWay);
            md.SetPara("RptDTSearchKey", md.RptDTSearchKey);
            md.SetPara("RptIsSearchKey", md.RptIsSearchKey);

            md.SetPara("T_SearchKey", ur.SearchKey);

            if (md.RptDTSearchWay != DTSearchWay.None)
            {
                ar = new MapAttr(rptNo, md.RptDTSearchKey);
                md.SetPara("T_DateLabel", ar.Name);

                if (md.RptDTSearchWay == DTSearchWay.ByDate)
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

            //判断是否含有导出至模板的模板文件，如果有，则显示导出至模板按钮RptExportToTmp
            string tmpDir = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + rptNo;
            if(System.IO.Directory.Exists(tmpDir))
            {
                if (System.IO.Directory.GetFiles(tmpDir, "*.xls*").Length > 0)
                    md.SetPara("T_RptExportToTmp", "1");
            }

            #region //增加显示列信息
            DataRow row = null;
            DataTable dt = new DataTable("Sys_MapAttr");
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));

            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, MapAttrAttr.Idx);

            foreach (MapAttr attr in attrs)
            {
                row = dt.NewRow();
                row["No"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;

                if (attr.HisAttr.IsFKorEnum)
                    row["No"] = attr.KeyOfEn + "Text";

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            #endregion

            #region //增加枚举/外键字段信息
            attrs = new MapAttrs(rptNo);
            dt = new DataTable("FilterCtrls");
            dt.Columns.Add("Id", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("DataType", typeof(int));
            dt.Columns.Add("DefaultValue", typeof(string));
            dt.Columns.Add("ValueField", typeof(string));
            dt.Columns.Add("TextField", typeof(string));
            dt.Columns.Add("ParentField", typeof(string));
            dt.Columns.Add("W", typeof(string));
            string[] ctrls = md.RptSearchKeys.Split('*');
            DataTable dtNoName = null;

            foreach (string ctrl in ctrls)
            {
                //增加判断，如果URL中有传参，则不进行此SearchAttr的过滤条件显示
                if (string.IsNullOrWhiteSpace(ctrl) || !DataType.IsNullOrEmpty(context.Request.QueryString[ctrl]))
                    continue;

                ar = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ctrl) as MapAttr;
                if (ar == null)
                    continue;

                row = dt.NewRow();
                row["Id"] = ctrl;
                row["Name"] = ar.Name;
                row["DataType"] = ar.MyDataType;
                row["W"] = ar.UIWidth; //宽度.

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
                                    EntitiesTree treeSimpEns = ens as EntitiesTree;

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
                    default:
                        break;
                }

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            #endregion

            #region //增加第一页数据
            GEEntitys ges = new GEEntitys(rptNo);
            QueryObject qo = new QueryObject(ges);

            switch (this.SearchType)
            {
                case "My": //我发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //我部门发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //我参与的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    return "err@" + this.SearchType + "标记错误.";
            }

            qo = InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);

            qo.AddWhere( " AND  WFState > 1 ");
            qo.AddWhere(" AND FID = 0 ");

            md.SetPara("T_total", qo.GetCount());
            qo.DoQuery("OID", string.IsNullOrWhiteSpace(pageSize) ? SystemConfig.PageSize : int.Parse(pageSize), 1);
            ds.Tables.Add(ges.ToDataTableField("MainData"));
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));
            #endregion

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        
        public string FlowSearch_Done()
        {
            string vals = this.GetRequestVal("vals");
            string searchKey = GetRequestVal("key");
            string dtFrom = GetRequestVal("dtFrom");
            string dtTo = GetRequestVal("dtTo");
            string mvals = GetRequestVal("mvals");
            string pageSize = GetRequestVal("pageSize");
            int pageIdx = int.Parse(GetRequestVal("pageIdx"));

            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.SearchType;
            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + rptNo + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            ur.SearchKey = searchKey;
            ur.DTFrom_Data = dtFrom;
            ur.DTTo_Data = dtTo;
            ur.Vals = vals;
            ur.MVals = mvals;
            ur.Update();

            DataSet ds = new DataSet();
            MapData md = new MapData(rptNo);
            MapAttrs attrs = new MapAttrs(rptNo);
            GEEntitys ges = new GEEntitys(rptNo);
            QueryObject qo = new QueryObject(ges);

            switch (this.SearchType)
            {
                case "My": //我发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //我部门发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //我参与的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    return "err@" + this.SearchType + "标记错误.";
            }

 
            qo = InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            qo.AddWhere(" AND  WFState > 1 "); //排除空白，草稿数据.


            md.SetPara("T_total", qo.GetCount());
            qo.DoQuery("OID", string.IsNullOrWhiteSpace(pageSize) ? SystemConfig.PageSize : int.Parse(pageSize), pageIdx);
            ds.Tables.Add(ges.ToDataTableField("MainData"));
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public string FlowSearch_Exp()
        {
            string vals = this.GetRequestVal("vals");
            string searchKey = GetRequestVal("key");
            string dtFrom = GetRequestVal("dtFrom");
            string dtTo = GetRequestVal("dtTo");
            string mvals = GetRequestVal("mvals");
  

            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.SearchType;
            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + rptNo + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            ur.SearchKey = searchKey;
            ur.DTFrom_Data = dtFrom;
            ur.DTTo_Data = dtTo;
            ur.Vals = vals;
            ur.MVals = mvals;
            ur.Update();

           
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, MapAttrAttr.Idx);

            DataSet ds = new DataSet();
            MapData md = new MapData(rptNo);
            //MapAttrs attrs = new MapAttrs(rptNo);
            GEEntitys ges = new GEEntitys(rptNo);
            QueryObject qo = new QueryObject(ges);

            string title = "数据导出";
            switch (this.SearchType)
            {
                case "My": //我发起的.
                    title = "我发起的流程";
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //我部门发起的.
                    title = "我部门发起的流程";
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //我参与的.
                    title = "我参与的流程";
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    return "err@" + this.SearchType + "标记错误.";
            }


            qo = InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            qo.AddWhere(" AND  WFState > 1 "); //排除空白，草稿数据.
            qo.addOrderByDesc("OID");
            Attrs attrsa = new Attrs();
            foreach(MapAttr attr in attrs){
                attrsa.Add(attr.HisAttr);
            }
            
            string filePath = ExportDGToExcel(qo.DoQueryToTable(), ges.GetNewEntity, title, attrsa);


            return filePath;
        }
        /// <summary>
        /// 流程分組分析 1.获取查询条件 2.获取分组的枚举或者外键值 3.获取分析的信息列表进行求和、求平均
        /// </summary>
        /// <returns></returns>
        public string FlowGroup_Init()
        {
            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                return "err@参数FK_Flow不能为空";

            string fcid = string.Empty;
            DataSet ds = new DataSet();
            Dictionary<string, string> vals = null;
            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.GroupType;

            //报表信息，包含是否显示关键字查询RptIsSearchKey，过滤条件枚举/下拉字段RptSearchKeys，时间段查询方式RptDTSearchWay，时间字段RptDTSearchKey
            MapData md = new MapData();
            md.No = rptNo;
            if (md.RetrieveFromDBSources() == 0)
            {
                /*如果没有找到，就让其重置一下.*/
                BP.WF.Rpt.RptDfine rd = new RptDfine(this.FK_Flow);

                if (this.GroupType == "My")
                    rd.DoReset(this.GroupType, "我发起的流程");

                if (this.GroupType == "MyJoin")
                    rd.DoReset(this.GroupType, "我审批的流程");

                if (this.GroupType == "MyDept")
                    rd.DoReset(this.GroupType, "本部门发起的流程");

                if (this.GroupType == "Adminer")
                    rd.DoReset(this.GroupType, "高级查询");

                md.RetrieveFromDBSources();
            }

            MapAttr ar = null;

            //查询条件的信息表
            string cfgfix = "_SearchAttrs";
            UserRegedit ur = new UserRegedit();
            ur.AutoMyPK = false;
            ur.MyPK = WebUser.No + rptNo + cfgfix;

            if (ur.RetrieveFromDBSources() == 0)
            {
                ur.MyPK = WebUser.No + rptNo + cfgfix;
                ur.FK_Emp = WebUser.No;
                ur.CfgKey = rptNo + cfgfix;

                ur.Insert();
            }

            //分组条件存储的信息表
            cfgfix = "_GroupAttrs";
            UserRegedit groupUr = new UserRegedit();
            groupUr.AutoMyPK = false;
            groupUr.MyPK = WebUser.No + rptNo + cfgfix;

            if (groupUr.RetrieveFromDBSources() == 0)
            {
                groupUr.MyPK = WebUser.No + rptNo + cfgfix;
                groupUr.FK_Emp = WebUser.No;
                groupUr.CfgKey = rptNo + cfgfix;

                groupUr.Insert();
            }
           
          

            vals = ur.GetVals();
            md.SetPara("RptDTSearchWay", (int)md.RptDTSearchWay);
            md.SetPara("RptDTSearchKey", md.RptDTSearchKey);
            md.SetPara("RptIsSearchKey", md.RptIsSearchKey);
            md.SetPara("T_SearchKey", ur.SearchKey);

            if (md.RptDTSearchWay != DTSearchWay.None)
            {
                ar = new MapAttr(rptNo, md.RptDTSearchKey);
                md.SetPara("T_DateLabel", ar.Name);

                if (md.RptDTSearchWay == DTSearchWay.ByDate)
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

            //判断是否含有导出至模板的模板文件，如果有，则显示导出至模板按钮RptExportToTmp
            string tmpDir = BP.Sys.SystemConfig.PathOfDataUser + @"TempleteExpEns\" + rptNo;
            if (System.IO.Directory.Exists(tmpDir))
            {
                if (System.IO.Directory.GetFiles(tmpDir, "*.xls*").Length > 0)
                    md.SetPara("T_RptExportToTmp", "1");
            }

            #region //显示的内容
            DataRow row = null;
            DataTable dt = new DataTable("Group_MapAttr");
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Checked", typeof(string));
           

            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, MapAttrAttr.Idx);

            foreach (MapAttr attr in attrs)
            {
                if (attr.UIContralType == UIContralType.DDL)
                {
                    DataRow dr = dt.NewRow();
                    dr["Field"] = attr.KeyOfEn;
                    dr["Name"] = attr.HisAttr.Desc;

                    // 根据状态 设置信息.
                    if (groupUr.Vals.IndexOf(attr.KeyOfEn) != -1)
                        dr["Checked"] = "true";

                    if (groupUr.Vals.IndexOf(attr.KeyOfEn) != -1)
                        dr["Checked"] = "true";

                    dt.Rows.Add(dr);
                } 
            }
            ds.Tables.Add(dt);
            #endregion

            #region //分析的内容
            dt = new DataTable("Analysis_MapAttr");
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Checked", typeof(string));

            //如果不存在分析项手动添加一个分析项
            DataRow dtr = dt.NewRow();
            dtr["Field"] = "Group_Number";
            dtr["Name"] = "数量";
            dtr["Checked"] = "true";
            dt.Rows.Add(dtr);

            DataTable ddlDt = new DataTable();
            ddlDt.TableName = "Group_Number";
            ddlDt.Columns.Add("No");
            ddlDt.Columns.Add("Name");
            ddlDt.Columns.Add("Selected");
            DataRow ddlDr = ddlDt.NewRow();
            ddlDr["No"] = "SUM";
            ddlDr["Name"] = "求和";
            ddlDr["Selected"] = "true";
            ddlDt.Rows.Add(ddlDr);
            ds.Tables.Add(ddlDt);


            foreach (MapAttr attr in attrs)
            {
                if (attr.IsPK || attr.IsNum == false)
                    continue;
                if (attr.UIContralType == UIContralType.TB == false)
                    continue;
                if (attr.UIVisible == false)
                    continue;
                if (attr.HisAttr.MyFieldType == FieldType.FK)
                    continue;
                if (attr.HisAttr.MyFieldType == FieldType.Enum)
                    continue;
                if (attr.KeyOfEn == "OID" || attr.KeyOfEn == "WorkID" || attr.KeyOfEn == "MID")
                    continue;

                dtr = dt.NewRow();
                dtr["Field"] = attr.KeyOfEn;
                dtr["Name"] = attr.HisAttr.Desc;


                // 根据状态 设置信息.
                if (groupUr.Vals.IndexOf(attr.KeyOfEn) != -1)
                    dtr["Checked"] = "true";
                dt.Rows.Add(dtr);

                ddlDt = new DataTable();
                ddlDt.Columns.Add("No");
                ddlDt.Columns.Add("Name");
                ddlDt.Columns.Add("Selected");
                ddlDt.TableName = attr.KeyOfEn;

                ddlDr = ddlDt.NewRow();
                ddlDr["No"] = "SUM";
                ddlDr["Name"] = "求和";
                if (groupUr.Vals.IndexOf("@" + attr.KeyOfEn + "=SUM") != -1)
                    ddlDr["Selected"] = "true";
                ddlDt.Rows.Add(ddlDr);

                ddlDr = ddlDt.NewRow();
                ddlDr["No"] = "AVG";
                ddlDr["Name"] = "求平均";
                if (groupUr.Vals.IndexOf("@" + attr.KeyOfEn + "=AVG") != -1)
                    ddlDr["Selected"] = "true";
                ddlDt.Rows.Add(ddlDr);

                if (this.IsContainsNDYF)
                {
                    ddlDr = ddlDt.NewRow();
                    ddlDr["No"] = "AMOUNT";
                    ddlDr["Name"] = "求累计";
                    if (groupUr.Vals.IndexOf("@" + attr.KeyOfEn + "=AMOUNT") != -1)
                        ddlDr["Selected"] = "true";
                    ddlDt.Rows.Add(ddlDr);
                }

                ds.Tables.Add(ddlDt);

            }
            ds.Tables.Add(dt);
            #endregion

            #region //增加枚举/外键字段信息
            attrs = new MapAttrs(rptNo);
            dt = new DataTable("FilterCtrls");
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("DataType", typeof(int));
            dt.Columns.Add("DefaultValue", typeof(string));
            dt.Columns.Add("ValueField", typeof(string));
            dt.Columns.Add("TextField", typeof(string));
            dt.Columns.Add("ParentField", typeof(string));
            dt.Columns.Add("W", typeof(string));
            string[] ctrls = md.RptSearchKeys.Split('*');
            DataTable dtNoName = null;

            foreach (string ctrl in ctrls)
            {
                //增加判断，如果URL中有传参，则不进行此SearchAttr的过滤条件显示
                if (string.IsNullOrWhiteSpace(ctrl) || !DataType.IsNullOrEmpty(context.Request.QueryString[ctrl]))
                    continue;

                ar = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ctrl) as MapAttr;
                if (ar == null)
                    continue;

                row = dt.NewRow();
                row["Field"] = ctrl;
                row["Name"] = ar.Name;
                row["DataType"] = ar.MyDataType;
                row["W"] = ar.UIWidth; //宽度.

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
                                    EntitiesTree treeSimpEns = ens as EntitiesTree;

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
                   
                    default:
                        break;
                }

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);
            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));
            #endregion

            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string FlowGropu_Done()
        {
            
            if (!this.GroupType.Equals("My") && this.GroupType.Equals("MyJoin") && this.GroupType.Equals("MyDept") && this.GroupType.Equals("Adminer"))
                 return "info@<img src='../Img/Pub/warning.gif' /><b><font color=red>"+ this.GroupType + "标记错误.</font></b>";
            DataSet ds = new DataSet();
            ds = FlowGroupDoneSet();
            if (ds == null)
                return "info@<img src='../Img/Pub/warning.gif' /><b><font color=red> 您没有选择显示的内容</font></b>";

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        public DataSet FlowGroupDoneSet()
        {
            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.GroupType;
            DataSet ds = new DataSet();
            MapData md = new MapData(rptNo);
            MapAttrs attrs = new MapAttrs(rptNo);
            GEEntitys ges = new GEEntitys(rptNo);
            GEEntity en = new GEEntity(rptNo);
            Map map = en.EnMapInTime;



            UserRegedit groupUr = new UserRegedit(WebUser.No,rptNo+"_GroupAttrs");
            //分组的参数
            string groupVals = groupUr.Vals;
            //查询条件
            //分组
            string groupKey = "";
            Attrs AttrsOfNum = new Attrs();//列
            string Condition = ""; //处理特殊字段的条件问题。

            //根据注册表信息获取里面的分组信息
            string StateNumKey = groupVals.Substring(groupVals.IndexOf("@StateNumKey") + 1);
            string[] statNumKeys = StateNumKey.Split('@');
            foreach (string ct in statNumKeys)
            {
                if (ct.Split('=').Length != 2)
                    continue;
                string[] paras = ct.Split('=');

                //判断paras[0]的类型
                int dataType = 2;
                if (paras[0].Equals("Group_Number"))
                {
                    AttrsOfNum.Add(new Attr("Group_Number", "Group_Number", 1, DataType.AppInt, false, "数量"));
                }
                else
                {
                    Attr attr = GetAttrByKey(attrs,paras[0]);
                    AttrsOfNum.Add(attr);
                    dataType = attr.MyDataType;
                }

                if (paras[0].Equals("Group_Number"))
                {
                    groupKey += " count(*) \"" + paras[0] + "\",";
                }
                else
                {
                    switch (paras[1])
                    {
                        case "SUM":
                            if (dataType == 2)
                                groupKey += " SUM(" + paras[0] + ") \"" + paras[0] + "\",";
                            else
                                groupKey += " round ( SUM(" + paras[0] + "), 4) \"" + paras[0] + "\",";
                            break;
                        case "AVG":
                            groupKey += " round (AVG(" + paras[0] + "), 4)  \"" + paras[0] + "\",";
                            break;
                        case "AMOUNT":
                            if (dataType == 2)
                                groupKey += " SUM(" + paras[0] + ") \"" + paras[0] + "\",";
                            else
                                groupKey += " round ( SUM(" + paras[0] + "), 4) \"" + paras[0] + "\",";
                            break;
                        default:
                            throw new Exception("没有判断的情况.");
                    }

                }

            }
            bool isHaveLJ = false; // 是否有累计字段。
            if (StateNumKey.IndexOf("AMOUNT@") != -1)
                isHaveLJ = true;

            if (groupKey == "")
            {
                return null;
            }

            /* 如果包含累计数据，那它一定需要一个月份字段。业务逻辑错误。*/
            groupKey = groupKey.Substring(0, groupKey.Length - 1);
            BP.DA.Paras ps = new Paras();
            // 生成 sql.
            string selectSQL = "SELECT ";
            string groupBy = " GROUP BY ";
            Attrs AttrsOfGroup = new Attrs();

            string SelectedGroupKey = groupVals.Substring(0, groupVals.IndexOf("@StateNumKey")); // 为保存操作状态的需要。
            if (!DataType.IsNullOrEmpty(SelectedGroupKey))
            {
                bool isSelected = false;
                string[] SelectedGroupKeys = SelectedGroupKey.Split('@');
                foreach (string key in SelectedGroupKeys)
                {
                    if (key.Contains("=") == true)
                        continue;
                    selectSQL += key + " \"" + key + "\",";
                    groupBy += key + ",";
                    // 加入组里面。
                    AttrsOfGroup.Add(GetAttrByKey(attrs,key), false, false);

                }
            }

            string groupList = this.GetRequestVal("GroupList");
            if (!DataType.IsNullOrEmpty(SelectedGroupKey))
            {
                /* 如果是年月 分组， 并且如果内部有 累计属性，就强制选择。*/
                if (groupList.IndexOf("FK_NY") != -1 && isHaveLJ)
                {
                    selectSQL += "FK_NY,";
                    groupBy += "FK_NY,";
                    SelectedGroupKey += "@FK_NY";
                    // 加入组里面。
                    AttrsOfGroup.Add(GetAttrByKey(attrs,"FK_NY"), false, false);
                }
            }

            groupBy = groupBy.Substring(0, groupBy.Length - 1);

            if (groupBy.Equals(" GROUP BY"))
                return null;

                

            string orderByReq = this.GetRequestVal("OrderBy");

            string orderby = "";

            if (orderByReq != null && (selectSQL.Contains(orderByReq) || groupKey.Contains(orderByReq)))
            {
                orderby = " ORDER BY " + orderByReq;
                string orderWay = this.GetRequestVal("OrderWay");
                if (!DataType.IsNullOrEmpty(orderWay) && !orderWay.Equals("Up"))
                    orderby += " DESC ";
            }

            //查询语句
            QueryObject qo = new QueryObject(ges);

            switch (this.GroupType)
            {
                case "My": //我发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowStarter, WebUser.No);
                    break;
                case "MyDept": //我部门发起的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FK_Dept, WebUser.FK_Dept);
                    break;
                case "MyJoin": //我参与的.
                    qo.AddWhere(BP.WF.Data.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
                    break;
                case "Adminer":
                    break;
                default:
                    return null;
            }

            //查询注册信息表
            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + rptNo + "_SearchAttrs";
            ur.RetrieveFromDBSources();
            qo = InitQueryObject(qo, md, ges.GetNewEntity.EnMap.Attrs, attrs, ur);
            qo.AddWhere(" AND  WFState > 1 "); //排除空白，草稿数据.

            DataTable dt2 = qo.DoGroupQueryToTable(selectSQL + groupKey, groupBy, orderby);

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
             dt.TableName = "GroupSearch";
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
                 foreach (Attr attr in AttrsOfNum)
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

                 string sql = "";
                 string whereOFLJ = "";
                 AtPara ap = new AtPara(ur.Vals);
                 /// #region 获得查询数据.
                 foreach (string str in ap.HisHT.Keys)
                 {
                     Object val = ap.GetValStrByKey(str);
                     if (val.Equals("all"))
                     {
                         continue;
                     }
                     if (str != "FK_NY")
                         whereOFLJ += " " + str + " =" + SystemConfig.AppCenterDBVarStr + str + "   AND ";

                 }

                 // 添加累计汇总数据.
                 foreach (DataRow dr in dt.Rows)
                 {
                     foreach (Attr attr in AttrsOfNum)
                     {
                         if (StateNumKey.IndexOf(attr.Key + "=AMOUNT") == -1)
                             continue;

                         //形成查询sql.
                         if (whereOFLJ.Length > 0)
                             sql = "SELECT SUM(" + attr.Key + ") FROM " + ges.GetNewEntity.EnMap.PhysicsTable + whereOFLJ + " AND ";
                         else
                             sql = "SELECT SUM(" + attr.Key + ") FROM " + ges.GetNewEntity.EnMap.PhysicsTable + " WHERE ";

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

             // 为表扩充外键
             foreach (Attr attr in AttrsOfGroup)
             {
                 dt.Columns.Add(attr.Key + "T", typeof(string));
             }
             foreach (Attr attr in AttrsOfGroup)
             {
                 if (attr.UIBindKey.IndexOf(".") == -1)
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
                     Entity myen = attr.HisFKEn;
                     string val = dr[attr.Key].ToString();
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
                         else if (val.Substring(0, 2) == "63")
                         {
                             try
                             {
                                 BP.Port.Dept Dept = new BP.Port.Dept(val);
                                 dr[attr.Key + "T"] = Dept.Name;
                             }
                             catch
                             {
                                 dr[attr.Key + "T"] = val;
                             }
                         }
                         else
                         {
                             dr[attr.Key + "T"] = val;
                         }
                     }
                 }
             }
             ds.Tables.Add(dt);
             ds.Tables.Add(AttrsOfNum.ToMapAttrs.ToDataTableField("AttrsOfNum"));
             ds.Tables.Add(AttrsOfGroup.ToMapAttrs.ToDataTableField("AttrsOfGroup"));


            return ds;
        }

        /// <summary>
        /// 执行导出
        /// </summary>
        /// <returns></returns>
        public string FlowGroup_Exp()
        {
            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.GroupType;
            var desc = "";

            if (this.GroupType.Equals("My"))
                desc="我发起的流程";
            else if (this.GroupType.Equals("MyJoin")) 
                 desc="我审批的流程";
            else if(this.GroupType.Equals("MyDept"))
                 desc="本部门发起的流程";
            else if( this.GroupType.Equals("Adminer"))
                 desc="高级查询";
            else
                return "info@<img src='../Img/Pub/warning.gif' /><b><font color=red>" + this.GroupType + "标记错误.</font></b>";
            
            DataSet ds = new DataSet();
            ds = FlowGroupDoneSet();
            if (ds == null)
                return "info@<img src='../Img/Pub/warning.gif' /><b><font color=red> 您没有选择显示的内容</font></b>";

            //获取注册信息表
            UserRegedit ur = new UserRegedit(WebUser.No, rptNo + "_GroupAttrs");

       

            string filePath = ExportGroupExcel(ds,desc , ur.Vals);


            return filePath;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public string FlowContrastDtl_Init()
        {
            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                return "err@参数FK_Flow不能为空";

            string pageSize = GetRequestVal("pageSize");
            string fcid = string.Empty;
            DataSet ds = new DataSet();
            Dictionary<string, string> vals = null;
            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.SearchType;

            GEEntitys ges = new GEEntitys(rptNo);

            //属性集合.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, MapAttrAttr.Idx);
            DataRow row = null;
            DataTable dt = new DataTable("Sys_MapAttr");
            dt.Columns.Add("No", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            foreach (MapAttr attr in attrs)
            {
                row = dt.NewRow();
                row["No"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;

                if (attr.HisAttr.IsFKorEnum)
                    row["No"] = attr.KeyOfEn + "Text";

                dt.Rows.Add(row);
            }

            ds.Tables.Add(dt);

            //查询结果
            QueryObject qo = new QueryObject(ges);

            string[] strs = this.context.Request.Form.ToString().Split('&');
            foreach (string str in strs)
            {
                if (str.IndexOf("FK_Flow") != -1 || str.IndexOf("SearchType") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];

                if (key == "OID" || key == "MyPK")
                    continue;

                if (key == "FK_Dept")
                {
                    this.FK_Dept = mykey[1];
                    continue;
                }
                bool isExist = false;
                foreach (MapAttr attr in attrs)
                {
                    if (attr.KeyOfEn.ToUpper().Equals(key.ToUpper()))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist == false)
                    continue;

                if (mykey[1] == "mvals")
                {
                    //如果用户多项选择了，就要找到它的选择项目.

                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + rptNo + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    string val = "";
                    if (instr == null || instr == "")
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                                val = WebUser.FK_Dept;
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
                        qo.AddWhereIn(mykey[0], instr);
                    }
                }
                else
                {
                    qo.AddWhere(mykey[0], mykey[1]);
                }
                qo.addAnd();
            }

            if (this.FK_Dept != null && (this.GetRequestVal("FK_Emp") == null
                || this.GetRequestVal("FK_Emp") == "all"))
            {
                if (this.FK_Dept.Length == 2)
                {
                    qo.AddWhere("FK_Dept", " = ", "all");
                    qo.addAnd();
                }
                else
                {
                    if (this.FK_Dept.Length == 8)
                    {
                        qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
                    }
                    else
                    {
                        qo.AddWhere("FK_Dept", " like ", this.FK_Dept + "%");
                    }

                    qo.addAnd();
                }
            }

            qo.AddHD();

            dt = qo.DoQueryToTable();
            dt.TableName = "Group_Dtls";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }


        /// <summary>
        /// 执行导出
        /// </summary>
        /// <returns></returns>
        public string FlowGroupDtl_Exp()
        {
            if (string.IsNullOrWhiteSpace(this.FK_Flow))
                return "err@参数FK_Flow不能为空";

            string pageSize = GetRequestVal("pageSize");
            string fcid = string.Empty;
            Dictionary<string, string> vals = null;
            string rptNo = "ND" + int.Parse(this.FK_Flow) + "Rpt" + this.SearchType;

            GEEntitys ges = new GEEntitys(rptNo);

            //属性集合.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, MapAttrAttr.Idx);

           
            //查询结果
            QueryObject qo = new QueryObject(ges);

            string[] strs = this.context.Request.Form.ToString().Split('&');
            foreach (string str in strs)
            {
                if (str.IndexOf("FK_Flow") != -1 || str.IndexOf("SearchType") != -1)
                    continue;

                string[] mykey = str.Split('=');
                string key = mykey[0];

                if (key == "OID" || key == "MyPK")
                    continue;

                if (key == "FK_Dept")
                {
                    this.FK_Dept = mykey[1];
                    continue;
                }
                bool isExist = false;
                foreach (MapAttr attr in attrs)
                {
                    if (attr.KeyOfEn.ToUpper().Equals(key.ToUpper()))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (isExist == false)
                    continue;

                if (mykey[1] == "mvals")
                {
                    //如果用户多项选择了，就要找到它的选择项目.

                    UserRegedit sUr = new UserRegedit();
                    sUr.MyPK = WebUser.No + rptNo + "_SearchAttrs";
                    sUr.RetrieveFromDBSources();

                    /* 如果是多选值 */
                    string cfgVal = sUr.MVals;
                    AtPara ap = new AtPara(cfgVal);
                    string instr = ap.GetValStrByKey(key);
                    string val = "";
                    if (instr == null || instr == "")
                    {
                        if (key == "FK_Dept" || key == "FK_Unit")
                        {
                            if (key == "FK_Dept")
                                val = WebUser.FK_Dept;
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
                        qo.AddWhereIn(mykey[0], instr);
                    }
                }
                else
                {
                    qo.AddWhere(mykey[0], mykey[1]);
                }
                qo.addAnd();
            }

            if (this.FK_Dept != null && (this.GetRequestVal("FK_Emp") == null
                || this.GetRequestVal("FK_Emp") == "all"))
            {
                if (this.FK_Dept.Length == 2)
                {
                    qo.AddWhere("FK_Dept", " = ", "all");
                    qo.addAnd();
                }
                else
                {
                    if (this.FK_Dept.Length == 8)
                    {
                        qo.AddWhere("FK_Dept", " = ", this.FK_Dept);
                    }
                    else
                    {
                        qo.AddWhere("FK_Dept", " like ", this.FK_Dept + "%");
                    }

                    qo.addAnd();
                }
            }

            qo.AddHD();

            DataTable dt = qo.DoQueryToTable();
            Attrs newAttrs = new Attrs();
            foreach (MapAttr attr in attrs)
            {
                if(attr.KeyOfEn.ToUpper().Equals("OID"))
                    continue;
               
                    newAttrs.Add(attr.HisAttr);
            }

            string filePath = ExportDGToExcel(dt, ges.GetNewEntity, rptNo, newAttrs);


            return filePath;
        }

        /// <summary>
        /// 通过一个key 得到它的属性值。
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>attr</returns>
        public Attr GetAttrByKey(MapAttrs mapAttrs,string key)
        {
            foreach (MapAttr attr in mapAttrs)
            {
                if (attr.KeyOfEn.ToUpper() == key.ToUpper())
                {
                    return attr.HisAttr;
                }
            }
            return null;
           
        }

        /// <summary>
        /// 初始化QueryObject
        /// </summary>
        /// <param name="qo"></param>
        /// <returns></returns>
        public QueryObject InitQueryObject(QueryObject qo, MapData md, Attrs attrs, MapAttrs rptAttrs, UserRegedit ur)
        {
            Dictionary<string, string> kvs = null;
            List<string> keys = new List<string>();
            string cfg = "_SearchAttrs";
            string searchKey = "";
            string val = null;

            kvs = ur.GetVals();

            if (this.SearchType != "Adminer")
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
                        if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(attr.Key, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                        else
                            qo.AddWhere(attr.Key, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                        continue;
                    }

                    qo.addOr();

                    if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
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
                if (DataType.IsNullOrEmpty(context.Request.QueryString[attr.Key]))
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

                            if (DataType.IsNullOrEmpty(instr))
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

                if (DataType.IsNullOrEmpty(dtFrom) == true)
                {
                    if (md.RptDTSearchWay == DTSearchWay.ByDate)
                        dtFrom = "1900-01-01";
                    else
                        dtFrom = "1900-01-01 00:00";
                }

                if (DataType.IsNullOrEmpty(dtTo) == true)
                {
                    if (md.RptDTSearchWay == DTSearchWay.ByDate)
                        dtTo = "2999-01-01";
                    else
                        dtTo = "2999-12-31 23:59";
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
