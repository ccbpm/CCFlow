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
using BP.WF.Template;
using BP.WF.Data;
using BP.WF.HttpHandler;

namespace BP.Frm
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class Frm_Bill : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public Frm_Bill(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public Frm_Bill()
        {
        }
        #endregion 构造方法.

        /// <summary>
        /// 发起列表.
        /// </summary>
        /// <returns></returns>
        public string Start_Init()
        {
            //获得发起列表. 
            DataSet ds = BP.Frm.Dev2Interface.DB_StartFlows(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
       
        /// <summary>
        /// 草稿列表
        /// </summary>
        /// <returns></returns>
        public string Draft_Init()
        {
            //草稿列表.
            DataTable dt = BP.Frm.Dev2Interface.DB_Draft(this.FrmID, BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        /// <summary>
        /// 单据初始化
        /// </summary>
        /// <returns></returns>
        public string MyBill_Init()
        {
            //获得发起列表. 
            DataSet ds = BP.Frm.Dev2Interface.DB_StartFlows(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public string DoMethod_ExeSQL()
        {
            FrmMethodFunc func = new FrmMethodFunc(this.MyPK);
            string doc = func.MethodDoc_SQL;

            GEEntity en = new GEEntity(func.FrmID, this.WorkID);
            doc = Glo.DealExp(doc, en, null); //替换里面的内容.

            try
            {
                DBAccess.RunSQLs(doc);
                if (func.MsgSuccess.Equals(""))
                    func.MsgSuccess = "执行成功.";

                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败(DoMethod_ExeSQL).";
                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
        }
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <returns></returns>
        public string DoMethodPara_ExeSQL()
        {
            FrmMethodFunc func = new FrmMethodFunc(this.MyPK);
            string doc = func.MethodDoc_SQL;

            GEEntity en = new GEEntity(func.FrmID, this.WorkID);
            doc = Glo.DealExp(doc, en, null); //替换里面的内容.

            #region 替换参数变量.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.MyPK);
            foreach (MapAttr item in attrs)
            {
                if (item.UIContralType == UIContralType.TB)
                {
                    doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("TB_" + item.KeyOfEn));
                    continue;
                }

                if (item.UIContralType == UIContralType.DDL)
                {
                    doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("DDL_" + item.KeyOfEn));
                    continue;
                }


                if (item.UIContralType == UIContralType.CheckBok)
                {
                    doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("CB_" + item.KeyOfEn));
                    continue;
                }

                if (item.UIContralType == UIContralType.RadioBtn)
                {
                    doc = doc.Replace("@" + item.KeyOfEn, this.GetRequestVal("RB_" + item.KeyOfEn));
                    continue;
                }
            }
            #endregion 替换参数变量.

            #region 开始执行SQLs.
            try
            {
                DBAccess.RunSQLs(doc);
                if (func.MsgSuccess.Equals(""))
                    func.MsgSuccess = "执行成功.";

                return func.MsgSuccess;
            }
            catch (Exception ex)
            {
                if (func.MsgErr.Equals(""))
                    func.MsgErr = "执行失败.";

                return "err@" + func.MsgErr + " @ " + ex.Message;
            }
            #endregion 开始执行SQLs.

            return "err@" + func.MethodDocTypeOfFunc + ",执行的类型没有解析.";
        }

        #region 单据处理.
        /// <summary>
        /// 创建空白的WorkID.
        /// </summary>
        /// <returns></returns>
        public string MyBill_CreateBlankWorkID()
        {
            return BP.Frm.Dev2Interface.CreateBlankWork(this.FrmID, BP.Web.WebUser.No, null).ToString();
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyBill_SaveIt()
        {
            //执行保存.
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            rpt = BP.Sys.PubClass.CopyFromRequest(rpt, context.Request) as GEEntity;

            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", (int)BillState.Editing);
            rpt.Update();

            string str = BP.Frm.Dev2Interface.SaveWork(this.FrmID, this.WorkID);
            return str;
        }
        private Hashtable GetMainTableHT()
        {
            Hashtable htMain = new Hashtable();
            foreach (string key in this.context.Request.Form.Keys)
            {
                if (key == null)
                    continue;

                if (key.Contains("TB_"))
                {

                    string val = context.Request.Form[key];
                    if (htMain.ContainsKey(key.Replace("TB_", "")) == false)
                    {
                        val = HttpUtility.UrlDecode(val, Encoding.UTF8);
                        htMain.Add(key.Replace("TB_", ""), val);
                    }
                    continue;
                }

                if (key.Contains("DDL_"))
                {
                    htMain.Add(key.Replace("DDL_", ""), context.Request.Form[key]);
                    continue;
                }

                if (key.Contains("CB_"))
                {
                    htMain.Add(key.Replace("CB_", ""), context.Request.Form[key]);
                    continue;
                }

                if (key.Contains("RB_"))
                {
                    htMain.Add(key.Replace("RB_", ""), context.Request.Form[key]);
                    continue;
                }
            }
            return htMain;
        }

        public string MyBill_SaveAsDraft()
        {
            string str = BP.Frm.Dev2Interface.SaveWork(this.FrmID, this.WorkID);
            return str;
        }
        public string MyBill_Delete()
        {
            return BP.Frm.Dev2Interface.MyBill_Delete(this.FrmID, this.WorkID);
        }
        #endregion 单据处理.

        #region 获取查询条件
        public string Search_ToolBar()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();

            //根据FrmID获取Mapdata
            MapData md = new MapData(this.FrmID);


            ds.Tables.Add(md.ToDataTableField("Sys_MapData"));

            //获取字段属性
            MapAttrs attrs = new MapAttrs(this.FrmID);

            #region //增加枚举/外键字段信息

            dt.Columns.Add("Field", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.TableName = "Attrs";

            string[] ctrls = md.RptSearchKeys.Split('*');
            DataTable dtNoName = null;

            MapAttr mapattr;
            DataRow dr = null;
            foreach (string ctrl in ctrls)
            {
                //增加判断，如果URL中有传参，则不进行此SearchAttr的过滤条件显示
                if (string.IsNullOrWhiteSpace(ctrl) || !DataType.IsNullOrEmpty(context.Request.QueryString[ctrl]))
                    continue;

                mapattr = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ctrl) as MapAttr;
                dr = dt.NewRow();
                dr["Field"] = mapattr.KeyOfEn;
                dr["Name"] = mapattr.Name;
                dr["Width"] = mapattr.UIWidth;
                dt.Rows.Add(dr);

                Attr attr = mapattr.HisAttr;
                if (mapattr == null)
                    continue;

                if (attr.IsEnum == true)
                {
                    SysEnums ses = new SysEnums(mapattr.UIBindKey);
                    DataTable dtEnum = ses.ToDataTableField();
                    dtEnum.TableName = mapattr.KeyOfEn;
                    ds.Tables.Add(dtEnum);
                    continue;
                }
                if (attr.IsFK == true)
                {
                    Entities ensFK = attr.HisFKEns;
                    ensFK.RetrieveAll();

                    DataTable dtEn = ensFK.ToDataTableField();
                    dtEn.TableName = attr.Key;
                    ds.Tables.Add(dtEn);
                }
                //绑定SQL的外键
                if (attr.UIDDLShowType == BP.Web.Controls.DDLShowType.BindSQL)
                {
                    //获取SQl
                    string sql = attr.UIBindKey;
                    sql = BP.WF.Glo.DealExp(sql, null, null);
                    DataTable dtSQl = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataColumn col in dtSQl.Columns)
                    {
                        string colName = col.ColumnName.ToLower();
                        switch (colName)
                        {
                            case "no":
                                col.ColumnName = "No";
                                break;
                            case "name":
                                col.ColumnName = "Name";
                                break;
                            case "parentno":
                                col.ColumnName = "ParentNo";
                                break;
                            default:
                                break;
                        }
                    }
                    dtSQl.TableName = attr.Key;
                    if (ds.Tables.Contains(attr.Key) == false)
                        ds.Tables.Add(dtSQl);

                }

            }

            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);

        }
            #endregion 查询条件


        public string Search_Init()
        {
            DataSet ds = new DataSet();

            #region 查询显示的列
            MapAttrs mapattrs = new MapAttrs();
            mapattrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);

            DataRow row = null;
            DataTable dt = new DataTable("Attrs");
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            dt.Columns.Add("LGType", typeof(int));

            //设置标题、单据号位于开始位置


            foreach (MapAttr attr in mapattrs)
            {
                string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                if (attr.UIVisible == false)
                    continue;
                row = dt.NewRow();
                row["KeyOfEn"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;
                row["LGType"] = attr.LGType;
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            #endregion 查询显示的列

            #region 查询语句

            MapData md = new MapData(this.FrmID);


            //取出来查询条件.
            BP.Sys.UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + "_" + this.FrmID + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

            QueryObject qo = new QueryObject(rpts);

            #region 关键字字段.
            string keyWord = ur.SearchKey;

            if (md.GetParaBoolen("IsSearchKey") && DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
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
                string enumKey = ","; //求出枚举值外键.
                foreach (Attr attr in attrs)
                {
                    switch (attr.MyFieldType)
                    {
                        case FieldType.Enum:
                            enumKey = "," + attr.Key + "Text,";
                            break;
                        case FieldType.FK:

                            continue;
                        default:
                            break;
                    }

                    if (attr.MyDataType != DataType.AppString)
                        continue;

                    //排除枚举值关联refText.
                    if (attr.MyFieldType == FieldType.RefText)
                    {
                        if (enumKey.Contains("," + attr.Key + ",") == true)
                            continue;
                    }

                    if (attr.Key == "FK_Dept")
                        continue;

                    i++;
                    if (i == 1)
                    {
                        /* 第一次进来。 */
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
                qo.MyParas.Add("SKey", keyWord);
                qo.addRightBracket();

            }
            else
            {
                qo.AddHD();
            }
            #endregion 关键字段查询

            #region 时间段的查询
            if (md.GetParaInt("DTSearchWay") != (int)DTSearchWay.None && DataType.IsNullOrEmpty(ur.DTFrom) == false)
            {
                string dtFrom = ur.DTFrom; // this.GetTBByID("TB_S_From").Text.Trim().Replace("/", "-");
                string dtTo = ur.DTTo; // this.GetTBByID("TB_S_To").Text.Trim().Replace("/", "-");

                //按日期查询
                if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDate)
                {
                    qo.addAnd();
                    qo.addLeftBracket();
                    dtTo += " 23:59:59";
                    qo.SQL = md.GetParaString("DTSearchKey") + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = md.GetParaString("DTSearchKey") + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }

                if (md.GetParaInt("DTSearchWay") == (int)DTSearchWay.ByDateTime)
                {
                    //取前一天的24：00
                    if (dtFrom.Trim().Length == 10) //2017-09-30
                        dtFrom += " 00:00:00";
                    if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                        dtFrom += ":00";

                    dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                    if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                        dtTo += " 24:00";

                    qo.addAnd();
                    qo.addLeftBracket();
                    qo.SQL = md.GetParaString("DTSearchKey") + " >= '" + dtFrom + "'";
                    qo.addAnd();
                    qo.SQL = md.GetParaString("DTSearchKey") + " <= '" + dtTo + "'";
                    qo.addRightBracket();
                }
            }
            #endregion 时间段的查询

            #region 外键或者枚举的查询

            //获得关键字.
            AtPara ap = new AtPara(ur.Vals);
            foreach (string str in ap.HisHT.Keys)
            {
                var val = ap.GetValStrByKey(str);
                if (val.Equals("all"))
                    continue;
                qo.addAnd();
                qo.addLeftBracket();
                qo.AddWhere(str, ap.GetValStrByKey(str));
                qo.addRightBracket();
            }
            #endregion 外键或者枚举的查询

            #endregion 查询语句

            //获得行数.
            ur.SetPara("RecCount", qo.GetCount());
            ur.Save();

            qo.DoQuery("OID", this.PageSize, this.PageIdx);

            DataTable mydt = rpts.ToDataTableField();
            mydt.TableName = "DT";

            ds.Tables.Add(mydt); //把数据加入里面.



            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 查询初始化
        /// </summary>
        /// <returns></returns>
        public string SearchData_Init()
        {
            DataSet ds = new DataSet();
            string sql = "";

            string tSpan = this.GetRequestVal("TSpan");
            if (tSpan == "")
                tSpan = null;

            #region 1、获取时间段枚举/总数.
            SysEnums ses = new SysEnums("TSpan");
            DataTable dtTSpan = ses.ToDataTableField();
            dtTSpan.TableName = "TSpan";
            ds.Tables.Add(dtTSpan);

            GenerBill gb = new GenerBill();
            gb.CheckPhysicsTable();


            sql = "SELECT TSpan as No, COUNT(WorkID) as Num FROM Frm_Bill WHERE FrmID='" + this.FrmID + "'  AND Starter='" + WebUser.No + "' AND BillState >= 1 GROUP BY TSpan";

            DataTable dtTSpanNum = BP.DA.DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow drEnum in dtTSpan.Rows)
            {
                string no = drEnum["IntKey"].ToString();
                foreach (DataRow dr in dtTSpanNum.Rows)
                {
                    if (dr["No"].ToString() == no)
                    {
                        drEnum["Lab"] = drEnum["Lab"].ToString() + "(" + dr["Num"] + ")";
                        break;
                    }
                }
            }
            #endregion

            #region 2、处理流程类别列表.
            sql = " SELECT  A.BillState as No, B.Lab as Name, COUNT(WorkID) as Num FROM Frm_Bill A, Sys_Enum B ";
            sql += " WHERE A.BillState=B.IntKey AND B.EnumKey='BillState' AND  A.Starter='" + WebUser.No + "' AND BillState >=1";
            if (tSpan.Equals("-1") == false)
                sql += "  AND A.TSpan=" + tSpan;

            sql += "  GROUP BY A.BillState, B.Lab  ";

            DataTable dtFlows = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dtFlows.Columns[0].ColumnName = "No";
                dtFlows.Columns[1].ColumnName = "Name";
                dtFlows.Columns[2].ColumnName = "Num";
            }
            dtFlows.TableName = "Flows";
            ds.Tables.Add(dtFlows);
            #endregion

            #region 3、处理流程实例列表.
            string sqlWhere = "";
            sqlWhere = "(1 = 1)AND Starter = '" + WebUser.No + "' AND BillState >= 1";
            if (tSpan.Equals("-1") == false)
            {
                sqlWhere += "AND (TSpan = '" + tSpan + "') ";
            }

            if (this.FK_Flow != null)
            {
                sqlWhere += "AND (FrmID = '" + this.FrmID + "')  ";
            }
            else
            {
                // sqlWhere += ")";
            }
            sqlWhere += "ORDER BY RDT DESC";

            string fields = " WorkID,FrmID,FrmName,Title,BillState, Starter, StarterName,Sender,RDT ";

            if (SystemConfig.AppCenterDBType == DBType.Oracle)
                sql = "SELECT " + fields + " FROM (SELECT * FROM Frm_Bill WHERE " + sqlWhere + ") WHERE rownum <= 50";
            else if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP 50 " + fields + " FROM Frm_Bill WHERE " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                sql = "SELECT  " + fields + " FROM Frm_Bill WHERE " + sqlWhere + " LIMIT 50";

            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                mydt.Columns[0].ColumnName = "WorkID";
                mydt.Columns[1].ColumnName = "FrmID";
                mydt.Columns[2].ColumnName = "FrmName";
                mydt.Columns[3].ColumnName = "Title";
                mydt.Columns[4].ColumnName = "BillState";
                mydt.Columns[5].ColumnName = "Starter";
                mydt.Columns[6].ColumnName = "StarterName";
                mydt.Columns[7].ColumnName = "Sender";
                mydt.Columns[8].ColumnName = "RDT";
            }

            mydt.TableName = "Frm_Bill";
            if (mydt != null)
            {
                mydt.Columns.Add("TDTime");
                foreach (DataRow dr in mydt.Rows)
                {
                    //   dr["TDTime"] =  GetTraceNewTime(dr["FK_Flow"].ToString(), int.Parse(dr["WorkID"].ToString()), int.Parse(dr["FID"].ToString()));
                }
            }
            #endregion

            ds.Tables.Add(mydt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 查询.

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

        #region 获得demo信息.
        public string MethodDocDemoJS_Init()
        {
            var func = new FrmMethodFunc(this.MyPK);
            return func.MethodDoc_JavaScript_Demo;
        }
        public string MethodDocDemoSQL_Init()
        {
            var func = new FrmMethodFunc(this.MyPK);
            return func.MethodDoc_SQL_Demo;
        }
        #endregion 获得demo信息.

    }
}
