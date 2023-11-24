using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF.Rpt;
using BP.Difference;
using BP.WF.Data;
using BP.WF.Admin;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Rpt : DirectoryPageBase
    {
        #region 属性.
        /// <summary>
        /// 查询类型: 0=我参与的,1=我发起的,2=抄送给我的,3=全部.
        /// </summary>
        public int SearchType
        {
            get
            {
                string val = this.GetRequestVal("SearchType");
                if (val == null || val == "")
                    return 2;
                return int.Parse(val);
            }
        }
        /// <summary>
        /// 分析类型: 0=我参与的,1=我发起的,2=抄送给我的,3=全部.
        /// </summary>
        public int GroupType
        {
            get
            {
                string val = this.GetRequestVal("GroupType");
                if (val == null || val == "")
                    return 2;
                return int.Parse(val);
            }
        }
        /// <summary>
        /// 状态：0=进行中，1=已经完成，2=全部.
        /// </summary>
        public int WFSta
        {
            get
            {
                string val = this.GetRequestVal("WFSta");
                if (val == null || val == "")
                    return 2;
                return int.Parse(val);
            }
        }
        #endregion 属性.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Rpt()
        {
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void SearchFlow_InitFields()
        {
            string frmID = "FlowRpt" + this.FlowNo;
            MapData md = new MapData();
            md.No = frmID;
            if (md.RetrieveFromDBSources() == 0)
            {
                Node nd=new Node(int.Parse(this.FlowNo + "01"));
                MapData mymd = new MapData(nd.NodeFrmID);

                md.Name = nd.Name;
                md.PTable = mymd.PTable;
                md.EntityType = EntityType.SingleFrm;
                md.Insert();
            }
            md.ClearCache();
            MapAttr attr=new MapAttr();
            attr.FrmID = frmID;
            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.OID) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.OID);
                attr.setName("OID");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIVisible(false);
                attr.setIdx(-100);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }
            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.Title) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.Title);
                attr.setName("标题");
                attr.setIdx(-99);
                attr.setUIVisible(true);
                attr.setMyDataType(DataType.AppString);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }
            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.WFState) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.WFState);
                attr.setName("状态");
                attr.setMyDataType(DataType.AppInt);
                attr.setUIVisible(true);
                attr.setLGType(FieldTypeS.Enum);
                attr.setUIContralType(UIContralType.DDL);
                attr.setUIBindKey(NDXRptBaseAttr.WFState);
                attr.setIdx(-98);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }

            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.BillNo) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.BillNo);
                attr.setName("单号");
                attr.setIdx(-97);
                attr.setUIVisible(true);
                attr.setMyDataType(DataType.AppString);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }
            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.FlowEmps) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.FlowEmps);
                attr.setName("参与人");
                attr.setIdx(-97);
                attr.setUIVisible(false);
                attr.setMyDataType(DataType.AppString);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }
            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.FlowStarter) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.FlowStarter);
                attr.setName("发起人");
                attr.setIdx(-96);
                attr.setUIVisible(true);
                attr.setMyDataType(DataType.AppString);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }
            if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.FlowStartRDT) == false)
            {
                attr = new MapAttr();
                attr.FrmID = frmID;
                attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.FlowStartRDT);
                attr.setName("发起日期");
                attr.setIdx(-95);
                attr.setUIVisible(true);
                attr.setMyDataType(DataType.AppDateTime);
                attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
                attr.DirectInsert();
            }
            //if (attr.IsExit(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.KeyOfEn, NDXRptBaseAttr.TodoEmps) == false)
            //{
            //    attr.SetValByKey(MapAttrAttr.KeyOfEn, NDXRptBaseAttr.TodoEmps);
            //    attr.setName("待办人员");
            //    attr.setIdx(-94);
            //    attr.setUIVisible(true);
            //    attr.setMyPK(attr.FrmID + "_" + attr.KeyOfEn);
            //    attr.DirectInsert();
            //}
        }
     
        public string SearchFlow_Init()
        {
            string frmID = "FlowRpt" + this.FlowNo;

            MapData md = new MapData();
            md.No = frmID;
            if (md.RetrieveFromDBSources() == 0)
            {
                SearchFlow_InitFields(); 
                md.RetrieveFromDBSources();
            }

            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            string keyWord = this.GetRequestVal("KeyWords");

            //数据.
            GEEntitys ges = new GEEntitys(frmID);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);

            #region 控制状态.
            //已经完成的.
            if (this.WFSta == 1)
                qo.AddWhere(NDXRptBaseAttr.WFState, "=", 3); //必须的条件.

            //进行中的.
            if (this.WFSta == 0)
            {
                qo.AddWhere(NDXRptBaseAttr.WFState, "!=", 3); //必须的条件.
                qo.addAnd();
                qo.AddWhere(NDXRptBaseAttr.WFState, ">=", 1); //必须的条件.
            }

            //全部的.
            if (this.WFSta == 2)
                qo.AddWhere(NDXRptBaseAttr.WFState, ">=", 1); //必须的条件.
            #endregion 控制状态.

            #region 时间范围.
            if (DataType.IsNullOrEmpty(dtTo) == false)
            {
                qo.addAnd();
                qo.addLeftBracket();
                qo.AddWhere(NDXRptBaseAttr.FlowStartRDT, ">=", dtFrom);
                qo.addAnd();
            //    qo.AddWhere(NDXRptBaseAttr.FlowStartRDT, "<=", dtTo);
                qo.SQL = "(  FlowStartRDT <=  '" + dtTo + "' )";
                qo.addRightBracket();
            }
            #endregion 时间范围.

            #region 权限范围..
            //我参与的
            if (this.SearchType == 0)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
            }

            //我发起的
            if (this.SearchType == 1)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.FlowStarter, WebUser.No);
            }
            #endregion 权限范围..

            #region 关键字..
            if (DataType.IsNullOrEmpty(keyWord) == false)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.Title, " LIKE ", "%"+keyWord+"%");
            }
            #endregion 关键字..

            //增加关键字查询.
            // if (this.SearchType == 2) { if (isHaveAnd == true)
            DataTable dt = qo.DoQueryToTable();
            dt.TableName = "dt";
            return BP.Tools.Json.ToJson(dt);
        }

        public string GroupFlow_Init()
        {
            DataSet ds = new DataSet();
            string frmID = "FlowRpt" + this.FlowNo;
            MapData md = new MapData();
            md.No = frmID;
            if (md.RetrieveFromDBSources() == 0)
            {
                SearchFlow_InitFields();
                md.RetrieveFromDBSources();
            }
            //表单中的字段属性
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.Idx);
            #region 分组条件
            string groupBy = this.GetRequestVal("GroupBy");
            if (DataType.IsNullOrEmpty(groupBy) == true)
                groupBy = "";
            DataTable dt = new DataTable("Group_MapAttr");
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));
            foreach (MapAttr attr in attrs)
            {
                if (attr.UIContralType == UIContralType.DDL || attr.UIContralType == UIContralType.RadioBtn)
                {
                    DataRow dr = dt.NewRow();
                    dr["Field"] = attr.KeyOfEn;
                    dr["Name"] = attr.HisAttr.Desc;

                    // 根据状态 设置信息.
                    if (groupBy.IndexOf(attr.KeyOfEn) != -1)
                        dr["Checked"] = true;
                    else
                        dr["Checked"] = false;
                    dt.Rows.Add(dr);
                }
            }
            ds.Tables.Add(dt);
            #endregion 分组条件
            #region 分析项目
            string searchField = this.GetRequestVal("SearchField");
            if (DataType.IsNullOrEmpty(searchField) == true)
                searchField = "";
            dt = new DataTable("Analysis_MapAttr");
            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));

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
            ddlDt.Columns.Add("Selected", typeof(bool));
            DataRow ddlDr = ddlDt.NewRow();
            ddlDr["No"] = "SUM";
            ddlDr["Name"] = "求和";
            ddlDr["Selected"] =true;
            ddlDt.Rows.Add(ddlDr);
            ds.Tables.Add(ddlDt);
            foreach (MapAttr attr in attrs)
            {
                if (attr.ItIsPK || attr.ItIsNum == false)
                    continue;
                if (attr.UIContralType != UIContralType.TB)
                    continue;
                if (attr.UIVisible == false)
                    continue;
                if (attr.HisAttr.MyFieldType == FieldType.FK)
                    continue;
                if (attr.HisAttr.MyFieldType == FieldType.Enum)
                    continue;
                if (attr.KeyOfEn.Equals("OID") || attr.KeyOfEn.Equals("WorkID") || attr.KeyOfEn.Equals("MID"))
                    continue;

                dtr = dt.NewRow();
                dtr["Field"] = attr.KeyOfEn;
                dtr["Name"] = attr.HisAttr.Desc;


                // 根据状态 设置信息.
                if (searchField.IndexOf(attr.KeyOfEn) != -1)
                    dtr["Checked"] = true;
                else
                    dtr["Checked"] = false;
                dt.Rows.Add(dtr);

                ddlDt = new DataTable();
                ddlDt.Columns.Add("No");
                ddlDt.Columns.Add("Name");
                ddlDt.Columns.Add("Selected");
                ddlDt.TableName = attr.KeyOfEn;

                ddlDr = ddlDt.NewRow();
                ddlDr["No"] = "SUM";
                ddlDr["Name"] = "求和";
                if (searchField.IndexOf("@" + attr.KeyOfEn + "=SUM") != -1)
                    ddlDr["Selected"] = "true";
                ddlDt.Rows.Add(ddlDr);

                ddlDr = ddlDt.NewRow();
                ddlDr["No"] = "AVG";
                ddlDr["Name"] = "求平均";
                if (searchField.IndexOf("@" + attr.KeyOfEn + "=AVG") != -1)
                    ddlDr["Selected"] = "true";
                ddlDt.Rows.Add(ddlDr);
                ds.Tables.Add(ddlDt);

            }
            ds.Tables.Add(dt);
            #endregion 分析项目
            return BP.Tools.Json.DataSetToJson(ds, false);
        }

        public string GroupFlow_Search()
        {
            DataSet ds = new DataSet();
            string frmID = "FlowRpt" + this.FlowNo;
            GEEntitys ges = new GEEntitys(frmID);

            //表单中的字段属性
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, frmID, MapAttrAttr.Idx);


            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            string keyWord = this.GetRequestVal("KeyWords");

            MapAttrs AttrsOfNum = new MapAttrs();//列
            string groupKey = "";
            string Condition = ""; //处理特殊字段的条件问题。

            //根据注册表信息获取里面的分组信息
            string StateNumKey = this.GetRequestVal("StateNumKey");
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
                    AttrsOfNum.AddEntity(new Attr("Group_Number", "Group_Number", 1, DataType.AppInt, false, "数量").ToMapAttr);
                }
                else
                {
                    MapAttr attr = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn,paras[0]) as MapAttr;
                    AttrsOfNum.AddEntity(attr);
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
            Paras ps = new Paras();
            // 生成 sql.
            string selectSQL = "SELECT ";
            string groupBy = " GROUP BY ";
            MapAttrs AttrsOfGroup = new MapAttrs();

            string SelectedGroupKey = this.GetRequestVal("SelectedGroupKey"); // 为保存操作状态的需要。
            if (!DataType.IsNullOrEmpty(SelectedGroupKey))
            {
                string[] SelectedGroupKeys = SelectedGroupKey.Split('@');
                foreach (string key in SelectedGroupKeys)
                {
                    if (key.Contains("=") == true)
                        continue;
                    selectSQL += key + " \"" + key + "\",";
                    groupBy += key + ",";
                    // 加入组里面。
                    AttrsOfGroup.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, key) as MapAttr);

                }
            }

            /*string groupList = this.GetRequestVal("GroupList");
            if (!DataType.IsNullOrEmpty(SelectedGroupKey))
            {
                *//* 如果是年月 分组， 并且如果内部有 累计属性，就强制选择。*//*
                if (groupList.IndexOf("FK_NY") != -1 && isHaveLJ)
                {
                    selectSQL += "FK_NY,";
                    groupBy += "FK_NY,";
                    SelectedGroupKey += "@FK_NY";
                    // 加入组里面。
                    AttrsOfGroup.AddEntity(attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, "FK_NY"));
                }
            }*/

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

            #region 控制状态.
            //已经完成的.
            if (this.WFSta == 1)
                qo.AddWhere(NDXRptBaseAttr.WFState, "=", 3); //必须的条件.

            //进行中的.
            if (this.WFSta == 0)
            {
                qo.addSQL(NDXRptBaseAttr.WFState + "!=3");
                qo.addAnd();
                qo.addSQL(NDXRptBaseAttr.WFState + ">=1");
            }

            //全部的.
            if (this.WFSta == 2)
                qo.AddWhere(NDXRptBaseAttr.WFState, ">=", 1); //必须的条件.
            #endregion 控制状态.

            #region 时间范围.
            if (DataType.IsNullOrEmpty(dtTo) == false)
            {
                qo.addAnd();
                qo.addLeftBracket();
                qo.AddWhere(NDXRptBaseAttr.FlowStartRDT, ">=", dtFrom);
                qo.addAnd();
                qo.SQL = "(  FlowStartRDT <=  '" + dtTo + "' )";
                qo.addRightBracket();
            }
            #endregion 时间范围.

            #region 权限范围..
            //我参与的
            if (this.SearchType == 0)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
            }

            //我发起的
            if (this.SearchType == 1)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.FlowStarter, WebUser.No);
            }
            #endregion 权限范围..

            #region 关键字..
            if (DataType.IsNullOrEmpty(keyWord) == false)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.Title, " LIKE ", "%" + keyWord + "%");
            }
            #endregion 关键字..
            //执行查询
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
            foreach (MapAttr attr in AttrsOfGroup)
            {
                dt.Columns[attr.KeyOfEn].DataType = typeof(string);
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
                foreach (MapAttr attr in AttrsOfNum)
                {
                    if (StateNumKey.IndexOf(attr.KeyOfEn + "=AMOUNT") == -1)
                        continue;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppInt:
                            dt.Columns.Add(attr.KeyOfEn + "Amount", typeof(int));
                            break;
                        default:
                            dt.Columns.Add(attr.KeyOfEn + "Amount", typeof(decimal));
                            break;
                    }
                }

                string sql = "";
                string whereOFLJ = "";
                AtPara ap = new AtPara("");
                /// #region 获得查询数据.
                foreach (string str in ap.HisHT.Keys)
                {
                    Object val = ap.GetValStrByKey(str);
                    if (val.Equals("all"))
                    {
                        continue;
                    }
                    if (str != "FK_NY")
                        whereOFLJ += " " + str + " =" + BP.Difference.SystemConfig.AppCenterDBVarStr + str + "   AND ";

                }

                // 添加累计汇总数据.
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (MapAttr attr in AttrsOfNum)
                    {
                        if (StateNumKey.IndexOf(attr.KeyOfEn + "=AMOUNT") == -1)
                            continue;

                        //形成查询sql.
                        if (whereOFLJ.Length > 0)
                            sql = "SELECT SUM(" + attr.KeyOfEn + ") FROM " + ges.GetNewEntity.EnMap.PhysicsTable + whereOFLJ + " AND ";
                        else
                            sql = "SELECT SUM(" + attr.KeyOfEn + ") FROM " + ges.GetNewEntity.EnMap.PhysicsTable + " WHERE ";

                        foreach (MapAttr attr1 in AttrsOfGroup)
                        {
                            switch (attr1.KeyOfEn)
                            {
                                case "FK_NY":
                                    sql += " FK_NY <= '" + dr["FK_NY"] + "' AND FK_ND='" + dr["FK_NY"].ToString().Substring(0, 4) + "' AND ";
                                    break;
                                case "FK_Dept":
                                    sql += attr1.KeyOfEn + "='" + dr[attr1.KeyOfEn] + "' AND ";
                                    break;
                                case "FK_SJ":
                                case "FK_XJ":
                                    sql += attr1.KeyOfEn + " LIKE '" + dr[attr1.KeyOfEn] + "%' AND ";
                                    break;
                                default:
                                    sql += attr1.KeyOfEn + "='" + dr[attr1.KeyOfEn] + "' AND ";
                                    break;
                            }
                        }

                        sql = sql.Substring(0, sql.Length - "AND ".Length);
                        if (attr.MyDataType == DataType.AppInt)
                            dr[attr.KeyOfEn + "Amount"] = DBAccess.RunSQLReturnValInt(sql, 0);
                        else
                            dr[attr.KeyOfEn + "Amount"] = DBAccess.RunSQLReturnValDecimal(sql, 0, 2);
                    }
                }
            }

            // 为表扩充外键
            foreach (MapAttr attr in AttrsOfGroup)
            {
                dt.Columns.Add(attr.KeyOfEn + "T", typeof(string));
            }
            foreach (MapAttr attr in AttrsOfGroup)
            {
                if (attr.KeyOfEn.Equals("WFState") == true)
                    attr.UIBindKey = "WFState";
                if (attr.UIBindKey.IndexOf(".") == -1)
                {
                    /* 说明它是枚举类型 */
                    SysEnums ses = new SysEnums(attr.UIBindKey);
                    foreach (DataRow dr in dt.Rows)
                    {
                        int val = 0;
                        try
                        {
                            val = int.Parse(dr[attr.KeyOfEn].ToString());
                        }
                        catch
                        {
                            dr[attr.KeyOfEn + "T"] = " ";
                            continue;
                        }

                        foreach (SysEnum se in ses)
                        {
                            if (se.IntKey == val)
                                dr[attr.KeyOfEn + "T"] = se.Lab;
                        }
                    }
                    continue;
                }
                foreach (DataRow dr in dt.Rows)
                {
                    Entity myen = attr.HisAttr.HisFKEn;
                    string val = dr[attr.KeyOfEn].ToString();
                    myen.SetValByKey(attr.KeyOfEn + "Text", val);
                    try
                    {
                        myen.Retrieve();
                        dr[attr.KeyOfEn + "T"] = myen.GetValStrByKey(attr.UIRefKeyText);
                    }
                    catch
                    {
                        if (val == null || val.Length <= 1)
                        {
                            dr[attr.KeyOfEn + "T"] = val;
                        }
                        else if (val.Substring(0, 2) == "63")
                        {
                            try
                            {
                                BP.Port.Dept Dept = new BP.Port.Dept(val);
                                dr[attr.KeyOfEn + "T"] = Dept.Name;
                            }
                            catch
                            {
                                dr[attr.KeyOfEn + "T"] = val;
                            }
                        }
                        else
                        {
                            dr[attr.KeyOfEn + "T"] = val;
                        }
                    }
                }
            }
            ds.Tables.Add(dt);
            ds.Tables.Add(AttrsOfNum.ToDataTableField("AttrsOfNum"));
            ds.Tables.Add(AttrsOfGroup.ToDataTableField("AttrsOfGroup"));
            return BP.Tools.Json.ToJson(ds);
        }

        public string ContrastDtlFlow_Init()
        {
            string frmID = "FlowRpt" + this.FlowNo;

            MapData md = new MapData();
            md.No = frmID;
            if (md.RetrieveFromDBSources() == 0)
            {
                SearchFlow_InitFields();
                md.RetrieveFromDBSources();
            }

            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            string keyWord = this.GetRequestVal("KeyWords");

            //数据.
            GEEntitys ges = new GEEntitys(frmID);

            //设置查询条件.
            QueryObject qo = new QueryObject(ges);

            #region 控制状态.
            //已经完成的.
            if (this.WFSta == 1)
                qo.addSQL(NDXRptBaseAttr.WFState + "=3"); //必须的条件.

            //进行中的.
            if (this.WFSta == 0)
            {
                qo.addSQL(NDXRptBaseAttr.WFState + "!=3");
                qo.addAnd();
                qo.addSQL(NDXRptBaseAttr.WFState + ">=1");
            }

            //全部的.
            if (this.WFSta == 2)
                qo.addSQL(NDXRptBaseAttr.WFState + ">=1"); //必须的条件.
            #endregion 控制状态.

            #region 时间范围.
            if (DataType.IsNullOrEmpty(dtTo) == false)
            {
                qo.addAnd();
                qo.addLeftBracket();
                qo.AddWhere(NDXRptBaseAttr.FlowStartRDT, ">=", dtFrom);
                qo.addAnd();
                qo.SQL = "(  FlowStartRDT <=  '" + dtTo + "' )";
                qo.addRightBracket();
            }
            #endregion 时间范围.

            #region 权限范围..
            //我参与的
            if (this.SearchType == 0)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.FlowEmps, " LIKE ", "%" + WebUser.No + "%");
            }

            //我发起的
            if (this.SearchType == 1)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.FlowStarter, WebUser.No);
            }
            #endregion 权限范围..

            #region 关键字..
            if (DataType.IsNullOrEmpty(keyWord) == false)
            {
                qo.addAnd();
                qo.AddWhere(BP.WF.GERptAttr.Title, " LIKE ", "%" + keyWord + "%");
            }
            #endregion 关键字..

            #region 其他条件
            string[] strs = HttpContextHelper.Request.Form.AllKeys;
            foreach (string key in strs)
            {
                if (key.Equals("FlowNo") || key.Equals("DTFrom") || key.Equals("DTTo")
                    || key.Equals("KeyWords") || key.Equals("WFSta") || key.Equals("SearchType"))
                    continue;
                string val = this.GetRequestVal(key);
                qo.addAnd();
                qo.AddWhere(key, val);
            }

            #endregion 其他条件

            DataTable dt = qo.DoQueryToTable();
            dt.TableName = "dt";
            return BP.Tools.Json.ToJson(dt);
        }
    }
}
