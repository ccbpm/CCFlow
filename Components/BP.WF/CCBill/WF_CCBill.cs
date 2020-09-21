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
using BP.NetPlatformImpl;
using BP.CCBill.Template;

namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill()
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
            DataSet ds = BP.CCBill.Dev2Interface.DB_StartBills(BP.Web.WebUser.No);

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
            DataTable dt = BP.CCBill.Dev2Interface.DB_Draft(this.FrmID, BP.Web.WebUser.No);

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
            DataSet ds = BP.CCBill.Dev2Interface.DB_StartBills(BP.Web.WebUser.No);

            //返回组合
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public string DoMethod_ExeSQL()
        {
            MethodFunc func = new MethodFunc(this.MyPK);
            string doc = func.MethodDoc_SQL;

            GEEntity en = new GEEntity(func.FrmID, this.WorkID);
            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.
            string sql = MidStrEx(doc, "/*", "*/");
            try
            {
                DBAccess.RunSQLs(sql);
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
            MethodFunc func = new MethodFunc(this.MyPK);
            string doc = func.MethodDoc_SQL;

            GEEntity en = new GEEntity(func.FrmID, this.WorkID);

            #region 替换参数变量.
            if (doc.Contains("@") == true)
            {
                MapAttrs attrs = new MapAttrs();
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.MyPK);
                foreach (MapAttr item in attrs)
                {
                    if (doc.Contains("@") == false)
                        break;
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
            }
            #endregion 替换参数变量.

            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.
            string sql = MidStrEx(doc, "/*", "*/");
            #region 开始执行SQLs.
            try
            {
                DBAccess.RunSQLs(sql);
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
        /// <summary>
        /// 执行url.
        /// </summary>
        /// <returns></returns>
        public string DoMethodPara_ExeUrl()
        {
            MethodFunc func = new MethodFunc(this.MyPK);
            string doc = func.MethodDoc_Url;

            GEEntity en = new GEEntity(func.FrmID, this.WorkID);

            #region 替换参数变量.
            if (doc.Contains("@") == true)
            {
                MapAttrs attrs = new MapAttrs();
                attrs.Retrieve(MapAttrAttr.FK_MapData, this.MyPK);
                foreach (MapAttr item in attrs)
                {
                    if (doc.Contains("@") == false)
                        break;
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
            }
            #endregion 替换参数变量.

            doc = BP.WF.Glo.DealExp(doc, en, null); //替换里面的内容.

            #region 开始执行SQLs.
            try
            {

                DataType.ReadURLContext(doc, 99999);
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
        public string MyBill_CreateBlankBillID()
        {
            string billNo = this.GetRequestVal("BillNo");
            return BP.CCBill.Dev2Interface.CreateBlankBillID(this.FrmID, BP.Web.WebUser.No, null, billNo).ToString();
        }
        /// <summary>
        /// 创建空白的DictID.
        /// </summary>
        /// <returns></returns>
        public string MyDict_CreateBlankDictID()
        {
            return BP.CCBill.Dev2Interface.CreateBlankDictID(this.FrmID, BP.Web.WebUser.No, null).ToString();
        }
        /// <summary>
        /// 执行保存 @hongyan
        /// </summary>
        /// <returns></returns>
        public string MyBill_SaveIt()
        {
            //创建entity 并执行copy方法.
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            Attrs attrs = rpt.EnMap.Attrs;

            //try
            //{
            //    //执行保存.
            //    rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;
            //}
            //catch (Exception ex)
            //{
            //    return "err@方法：MyBill_SaveIt错误，在执行 CopyFromRequest 期间" + ex.Message;
            //}
            //执行copy ，这部分与上一个方法重复了.
            try
            {
                Hashtable ht = this.GetMainTableHT();
                foreach (string item in ht.Keys)
                {
                    rpt.SetValByKey(item, ht[item]);
                }
            }
            catch (Exception ex)
            {
                return "err@方法：MyBill_SaveIt错误，在执行  GetMainTableHT 期间" + ex.Message;
            }
            //执行保存.
            try
            {
                rpt.OID = this.WorkID;
                rpt.SetValByKey("BillState", (int)BillState.Editing);
                rpt.Update();
                string str = BP.CCBill.Dev2Interface.SaveWork(this.FrmID, this.WorkID);
                return str;
            }
            catch (Exception ex)
            {
                return "err@方法：MyBill_SaveIt 错误，在执行 SaveWork 期间出现错误:" + ex.Message;
            }
        }
        public string MyBill_Submit()
        {
            //执行保存.
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            //rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;
            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", (int)BillState.Over);
            rpt.Update();

            string str = BP.CCBill.Dev2Interface.SubmitWork(this.FrmID, this.WorkID);
            return str;
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyDict_SaveIt()
        {
            //执行保存.
            MapData md = new MapData(this.FrmID);
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            //rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;
            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            //执行保存前事件
            ExecEvent.DoFrm(md, EventListFrm.SaveBefore, rpt, null);

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", (int)BillState.Editing);
            rpt.Update();

            string str = BP.CCBill.Dev2Interface.SaveWork(this.FrmID, this.WorkID);

            //执行保存后事件
            ExecEvent.DoFrm(md, EventListFrm.SaveAfter, rpt, null);
            return str;
        }

        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string MyDict_Submit()
        {
            //  throw new Exception("dddssds");
            //执行保存.
            MapData md = new MapData(this.FrmID);
            GEEntity rpt = new GEEntity(this.FrmID, this.WorkID);
            //rpt = BP.Pub.PubClass.CopyFromRequest(rpt) as GEEntity;

            Hashtable ht = GetMainTableHT();
            foreach (string item in ht.Keys)
            {
                rpt.SetValByKey(item, ht[item]);
            }

            //执行保存前事件
            ExecEvent.DoFrm(md, EventListFrm.SaveBefore, rpt, null);

            rpt.OID = this.WorkID;
            rpt.SetValByKey("BillState", (int)BillState.Over);
            rpt.Update();

            string str = BP.CCBill.Dev2Interface.SaveWork(this.FrmID, this.WorkID);

            //执行保存后事件
            ExecEvent.DoFrm(md, EventListFrm.SaveAfter, rpt, null);
            return str;
        }

        public string GetFrmEntitys()
        {
            GEEntitys rpts = new GEEntitys(this.FrmID);
            QueryObject qo = new QueryObject(rpts);
            qo.AddWhere("BillState", " != ", 0);
            qo.DoQuery();
            return BP.Tools.Json.ToJson(rpts.ToDataTableField());
        }
        private Hashtable GetMainTableHT()
        {
            Hashtable htMain = new Hashtable();
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (key == null || key == "")
                    continue;
                string mykey = key.Replace("TB_", "");
                mykey = key.Replace("DDL_", "");
                mykey = key.Replace("CB_", "");
                mykey = key.Replace("RB_", "");


                if (key.Contains("TB_"))
                {

                    string val = HttpContextHelper.RequestParams(key);
                    if (htMain.ContainsKey(key.Replace("TB_", "")) == false)
                    {
                        val = HttpUtility.UrlDecode(val, Encoding.UTF8);
                        htMain.Add(key.Replace("TB_", ""), val);
                    }
                    continue;
                }

                if (key.Contains("DDL_"))
                {
                    htMain.Add(key.Replace("DDL_", ""), HttpContextHelper.RequestParams(key));
                    continue;
                }

                if (key.Contains("CB_"))
                {
                    htMain.Add(key.Replace("CB_", ""), HttpContextHelper.RequestParams(key));
                    continue;
                }

                if (key.Contains("RB_"))
                {
                    htMain.Add(key.Replace("RB_", ""), HttpContextHelper.RequestParams(key));
                    continue;
                }
            }
            return htMain;
        }

        public string MyBill_SaveAsDraft()
        {
            string str = BP.CCBill.Dev2Interface.SaveWork(this.FrmID, this.WorkID);
            return str;
        }
        //删除单据
        public string MyBill_Delete()
        {
            return BP.CCBill.Dev2Interface.MyBill_Delete(this.FrmID, this.WorkID);
        }

        public string MyBill_Deletes()
        {
            return BP.CCBill.Dev2Interface.MyBill_DeleteDicts(this.FrmID, this.GetRequestVal("WorkIDs"));
        }

        //删除实体
        public string MyDict_Delete()
        {
            return BP.CCBill.Dev2Interface.MyDict_Delete(this.FrmID, this.WorkID);
        }

        public string MyEntityTree_Delete()
        {
            return BP.CCBill.Dev2Interface.MyEntityTree_Delete(this.FrmID, this.GetRequestVal("BillNo"));
        }
        /// <summary>
        /// 复制单据数据
        /// </summary>
        /// <returns></returns>
        public string MyBill_Copy()
        {
            return BP.CCBill.Dev2Interface.MyBill_Copy(this.FrmID, this.WorkID);
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
            dt.PrimaryKey = new DataColumn[] { dt.Columns["Field"] };
            ds.Tables.Add(dt);
            string[] ctrls = md.RptSearchKeys.Split('*');
            DataTable dtNoName = null;

            MapAttr mapattr;
            DataRow dr = null;
            foreach (string ctrl in ctrls)
            {
                //增加判断，如果URL中有传参，则不进行此SearchAttr的过滤条件显示
                if (string.IsNullOrWhiteSpace(ctrl) || !DataType.IsNullOrEmpty(HttpContextHelper.RequestParams(ctrl)))
                    continue;

                mapattr = attrs.GetEntityByKey(MapAttrAttr.KeyOfEn, ctrl) as MapAttr;
                if (mapattr == null)
                    continue;

                dr = dt.NewRow();
                dr["Field"] = mapattr.KeyOfEn;
                dr["Name"] = mapattr.Name;
                dr["Width"] = mapattr.UIWidth;
                dt.Rows.Add(dr);

                Attr attr = mapattr.HisAttr;
                if (mapattr == null)
                    continue;

                if (attr.Key.Equals("FK_Dept"))
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
                if (DataType.IsNullOrEmpty(attr.UIBindKey) == false
                    && ds.Tables.Contains(attr.Key) == false)
                {
                    DataTable dtSQl = BP.Pub.PubClass.GetDataTableByUIBineKey(attr.UIBindKey);
                    foreach (DataColumn col in dtSQl.Columns)
                    {
                        string colName = col.ColumnName.ToLower();
                        switch (colName)
                        {
                            case "no":
                            case "NO":
                                col.ColumnName = "No";
                                break;
                            case "name":
                            case "NAME":
                                col.ColumnName = "Name";
                                break;
                            case "parentno":
                            case "PARENTNO":
                                col.ColumnName = "ParentNo";
                                break;
                            default:
                                break;
                        }
                    }
                    dtSQl.TableName = attr.Key;
                    ds.Tables.Add(dtSQl);

                }

            }




            //数据查询权限除只查看自己创建的数据外增加部门的查询条件
            SearchDataRole searchDataRole = (SearchDataRole)md.GetParaInt("SearchDataRole");
            if (searchDataRole != SearchDataRole.ByOnlySelf)
            {
                DataTable dd = GetDeptDataTable(searchDataRole, md);
                if (dd.Rows.Count == 0 && md.GetParaInt("SearchDataRoleByDeptStation") == 1)
                    dd = GetDeptAndSubLevel();
                if (dd.Rows.Count != 0)
                {
                    //增加部门的查询条件
                    if (dt.Rows.Contains("FK_Dept") == false)
                    {
                        dr = dt.NewRow();
                        dr["Field"] = "FK_Dept";
                        dr["Name"] = "部门";
                        dr["Width"] = 120;
                        dt.Rows.Add(dr);
                    }

                    dd.TableName = "FK_Dept";
                    ds.Tables.Add(dd);

                }
            }

            return BP.Tools.Json.ToJson(ds);

        }
        #endregion 查询条件

        private DataTable GetDeptDataTable(SearchDataRole searchDataRole, MapData md)
        {
            //增加部门的外键
            DataTable dt = new DataTable();
            string sql = "";
            if (searchDataRole == SearchDataRole.ByDept)
            {
                sql = "SELECT D.No,D.Name From Port_Dept D,Port_DeptEmp E WHERE D.No=E.FK_Dept AND E.FK_Emp='" + WebUser.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            if (searchDataRole == SearchDataRole.ByDeptAndSSubLevel)
            {
                dt = GetDeptAndSubLevel();
            }
            if (searchDataRole == SearchDataRole.ByStationDept)
            {
                sql = "SELECT D.No,D.Name From Port_Dept D WHERE D.No IN(SELECT F.FK_Dept FROM Frm_StationDept F,Port_DeptEmpStation P Where F.FK_Station = P.FK_Station AND F.FK_Frm='" + md.No + "' AND P.FK_Emp='" + WebUser.No + "')";
                dt = DBAccess.RunSQLReturnTable(sql);
            }
            foreach (DataColumn col in dt.Columns)
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

                    default:
                        break;
                }

            }
            return dt;
        }

        private DataTable GetDeptAndSubLevel()
        {
            //获取本部门和兼职部门
            string sql = "SELECT D.No,D.Name From Port_Dept D,Port_DeptEmp E WHERE D.No=E.FK_Dept AND E.FK_Emp='" + WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.PrimaryKey = new DataColumn[] { dt.Columns["No"] };
            DataTable dd = dt.Copy();
            foreach (DataRow dr in dd.Rows)
            {
                GetSubLevelDeptByParentNo(dt, dr[0].ToString());
            }
            return dt;
        }

        private void GetSubLevelDeptByParentNo(DataTable dt, string parentNo)
        {
            string sql = "SELECT No,Name FROM Port_Dept Where ParentNo='" + parentNo + "'";
            DataTable dd = DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow dr in dd.Rows)
            {
                if (dt.Rows.Contains(dr[0].ToString()) == true)
                    continue;
                dt.Rows.Add(dr.ItemArray);

                GetSubLevelDeptByParentNo(dt, dr[0].ToString());

            }
        }


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
            dt.Columns.Add("AtPara", typeof(string));

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
                row["AtPara"] = attr.GetValStringByKey("AtPara");
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            #endregion 查询显示的列

            #region 查询语句
            MapData md = new MapData(this.FrmID);

            //取出来查询条件.
            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + "_" + this.FrmID + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

            QueryObject qo = new QueryObject(rpts);
            bool isFirst = true; //是否第一次拼接SQL

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
                        isFirst = false;
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
            else if (DataType.IsNullOrEmpty(md.GetParaString("RptStringSearchKeys")) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值
                int idx = 0;

                //获取查询的字段
                string[] searchFields = md.GetParaString("RptStringSearchKeys").Split('*');
                foreach (String str in searchFields)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    string[] items = str.Split(',');
                    if (items.Length == 2 && DataType.IsNullOrEmpty(items[0]) == true)
                        continue;
                    field = items[0];
                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                        continue;
                    idx++;
                    if (idx == 1)
                    {
                        isFirst = false;
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(field, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + field + ",'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                        else
                            qo.AddWhere(field, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + field + "||'%'");
                        qo.MyParas.Add(field, fieldValue);
                        continue;
                    }
                    qo.addAnd();

                    if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                        qo.AddWhere(field, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + SystemConfig.AppCenterDBVarStr + field + ",'%')") : ("'%'+" + SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                    else
                        qo.AddWhere(field, " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + field + "||'%'");
                    qo.MyParas.Add(field, fieldValue);


                }
                if (idx != 0)
                    qo.addRightBracket();
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
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
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

                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
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
                if (isFirst == false)
                    qo.addAnd();
                else
                    isFirst = false;

                qo.addLeftBracket();


                if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                {
                    var typeVal = BP.Sys.Glo.GenerRealType(attrs, str, ap.GetValStrByKey(str));
                    qo.AddWhere(str, typeVal);

                }
                else
                {
                    qo.AddWhere(str, ap.GetValStrByKey(str));
                }

                qo.addRightBracket();
            }
            #endregion 外键或者枚举的查询

            #region 设置隐藏字段的过滤查询
            FrmBill frmBill = new FrmBill(this.FrmID);
            string hidenField = frmBill.GetParaString("HidenField");

            if (WebUser.No.Equals("admin") == false && DataType.IsNullOrEmpty(hidenField) == false)
            {
                hidenField = hidenField.Replace("[%]", "%");
                foreach (string field in hidenField.Split(';'))
                {
                    if (field == "")
                        continue;
                    if (field.Split(',').Length != 3)
                        throw new Exception("单据" + frmBill.Name + "的过滤设置规则错误：" + hidenField + ",请联系管理员检查");
                    string[] str = field.Split(',');
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.addLeftBracket();

                    string val = str[2].Replace("WebUser.No", WebUser.No);
                    val = val.Replace("WebUser.Name", WebUser.Name);
                    val = val.Replace("WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
                    val = val.Replace("WebUser.FK_DeptName", WebUser.FK_DeptName);
                    val = val.Replace("WebUser.FK_Dept", WebUser.FK_Dept);
                    //val = val.Replace("WebUser.OrgNo", WebUser.OrgNo);

                    //获得真实的数据类型.
                    if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    {
                        var valType = BP.Sys.Glo.GenerRealType(attrs,
                            str[0], str[2]);
                        qo.AddWhere(str[0], str[1], val);
                    }
                    else
                    {
                        qo.AddWhere(str[0], str[1], val);
                    }
                    qo.addRightBracket();
                    continue;
                }

            }

            #endregion 设置隐藏字段的查询

            #endregion 查询语句

            if (isFirst == false)
                qo.addAnd();

            qo.AddWhere("BillState", "!=", 0);
            if ((SearchDataRole)md.GetParaInt("SearchDataRole") != SearchDataRole.SearchAll)
            {
                //默认查询本部门的单据
                if ((SearchDataRole)md.GetParaInt("SearchDataRole") == SearchDataRole.ByOnlySelf && DataType.IsNullOrEmpty(hidenField) == true
                    || (md.GetParaInt("SearchDataRoleByDeptStation") == 0 && DataType.IsNullOrEmpty(ap.GetValStrByKey("FK_Dept")) == true))
                {
                    //qo.addAnd();
                    //qo.AddWhere("Starter", "=", WebUser.No);
                }
            }



            //获得行数.
            ur.SetPara("RecCount", qo.GetCount());
            ur.Save();

            //获取配置信息
            string fieldSet = frmBill.FieldSet;
            string oper = "";
            if (DataType.IsNullOrEmpty(fieldSet) == false)
            {
                string ptable = rpts.GetNewEntity.EnMap.PhysicsTable;
                dt = new DataTable("Search_FieldSet");
                dt.Columns.Add("Field");
                dt.Columns.Add("Type");
                dt.Columns.Add("Value");
                DataRow dr;
                string[] strs = fieldSet.Split('@');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;
                    string[] item = str.Split('=');
                    if (item.Length == 2)
                    {
                        if (item[1].Contains(",") == true)
                        {
                            string[] ss = item[1].Split(',');
                            foreach (string s in ss)
                            {
                                dr = dt.NewRow();
                                dr["Field"] = attrs.GetAttrByKey(s).Desc;
                                dr["Type"] = item[0];
                                dt.Rows.Add(dr);

                                oper += item[0] + "(" + ptable + "." + s + ")" + ",";
                            }
                        }
                        else
                        {
                            dr = dt.NewRow();
                            dr["Field"] = attrs.GetAttrByKey(item[1]).Desc;
                            dr["Type"] = item[0];
                            dt.Rows.Add(dr);

                            oper += item[0] + "(" + ptable + "." + item[1] + ")" + ",";
                        }
                    }
                }
                oper = oper.Substring(0, oper.Length - 1);
                DataTable dd = qo.GetSumOrAvg(oper);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow ddr = dt.Rows[i];
                    ddr["Value"] = dd.Rows[0][i];
                }
                ds.Tables.Add(dt);
            }



            if (DataType.IsNullOrEmpty(ur.OrderBy) == false && DataType.IsNullOrEmpty(ur.OrderWay) == false)
                qo.DoQuery("OID", this.PageSize, this.PageIdx, ur.OrderBy, ur.OrderWay);
            else
            {

                //如果是总部，按照部门部门排序
                if (this.FrmID == "Bill_ZongBuHuaMingCe")
                    qo.addOrderBy("PS_DepartCode");
                //如果是区域，按照区域公司排序
                else if (this.FrmID == "Bill_QuYuRenYuanShanJu")
                    qo.addOrderBy("PS_DepartCode");
                //如果是基地，按照基地名称排序
                else if (this.FrmID == "Bill_JDGSRYSJ")
                    qo.addOrderBy("PS_BaseCode");
                else
                    qo.addOrderBy("OID");

                qo.DoQuery("OID", this.PageSize, this.PageIdx);
            }

            DataTable mydt = rpts.ToDataTableField();
            mydt.TableName = "DT";

            ds.Tables.Add(mydt); //把数据加入里面.

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string GenerBill_Init()
        {
            GenerBills bills = new GenerBills();
            bills.Retrieve(GenerBillAttr.Starter, WebUser.No);
            return bills.ToJson();
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

            sql = "SELECT TSpan as No, COUNT(WorkID) as Num FROM Frm_GenerBill WHERE FrmID='" + this.FrmID + "'  AND Starter='" + WebUser.No + "' AND BillState >= 1 GROUP BY TSpan";

            DataTable dtTSpanNum = DBAccess.RunSQLReturnTable(sql);
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
            sql = " SELECT  A.BillState as No, B.Lab as Name, COUNT(WorkID) as Num FROM Frm_GenerBill A, Sys_Enum B ";
            sql += " WHERE A.BillState=B.IntKey AND B.EnumKey='BillState' AND  A.Starter='" + WebUser.No + "' AND BillState >=1";
            if (tSpan.Equals("-1") == false)
                sql += "  AND A.TSpan=" + tSpan;

            sql += "  GROUP BY A.BillState, B.Lab  ";

            DataTable dtFlows = DBAccess.RunSQLReturnTable(sql);
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
                sql = "SELECT " + fields + " FROM (SELECT * FROM Frm_GenerBill WHERE " + sqlWhere + ") WHERE rownum <= 50";
            else if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                sql = "SELECT  TOP 50 " + fields + " FROM Frm_GenerBill WHERE " + sqlWhere;
            else if (SystemConfig.AppCenterDBType == DBType.MySQL || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                sql = "SELECT  " + fields + " FROM Frm_GenerBill WHERE " + sqlWhere + " LIMIT 50";

            DataTable mydt = DBAccess.RunSQLReturnTable(sql);
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

        #region 单据导出
        public string Search_Exp()
        {
            FrmBill frmBill = new FrmBill(this.FrmID);
            GEEntitys rpts = new GEEntitys(this.FrmID);

            string name = "数据导出";
            string filename = frmBill.Name + "_" + DataType.CurrentDataTimeCNOfLong + ".xls";
            string filePath = ExportDGToExcel(Search_Data(), rpts.GetNewEntity, null, null, filename);
            return filePath;
        }

        public DataTable Search_Data()
        {
            DataSet ds = new DataSet();

            #region 查询语句

            MapData md = new MapData(this.FrmID);


            //取出来查询条件.
            UserRegedit ur = new UserRegedit();
            ur.MyPK = WebUser.No + "_" + this.FrmID + "_SearchAttrs";
            ur.RetrieveFromDBSources();

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

            QueryObject qo = new QueryObject(rpts);

            #region 关键字字段.
            string keyWord = ur.SearchKey;
            bool isFirst = true; //是否第一次拼接SQL

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
                        isFirst = false;
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
            else if (DataType.IsNullOrEmpty(md.GetParaString("RptStringSearchKeys")) == false)
            {
                string field = "";//字段名
                string fieldValue = "";//字段值
                int idx = 0;

                //获取查询的字段
                string[] searchFields = md.GetParaString("RptStringSearchKeys").Split('*');
                foreach (String str in searchFields)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //字段名
                    string[] items = str.Split(',');
                    if (items.Length == 2 && DataType.IsNullOrEmpty(items[0]) == true)
                        continue;
                    field = items[0];
                    //字段名对应的字段值
                    fieldValue = ur.GetParaString(field);
                    if (DataType.IsNullOrEmpty(fieldValue) == true)
                        continue;
                    idx++;
                    if (idx == 1)
                    {
                        isFirst = false;
                        /* 第一次进来。 */
                        qo.addLeftBracket();
                        if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                            qo.AddWhere(field, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + field + ",'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                        else
                            qo.AddWhere(field, " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + field + "||'%'");
                        qo.MyParas.Add(field, fieldValue);
                        continue;
                    }
                    qo.addAnd();

                    if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                        qo.AddWhere(field, " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + SystemConfig.AppCenterDBVarStr + field + ",'%')") : ("'%'+" + SystemConfig.AppCenterDBVarStr + field + "+'%'"));
                    else
                        qo.AddWhere(field, " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + field + "||'%'");
                    qo.MyParas.Add(field, fieldValue);


                }
                if (idx != 0)
                    qo.addRightBracket();
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
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
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

                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
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
                if (isFirst == false)
                    qo.addAnd();
                else
                    isFirst = false;

                qo.addLeftBracket();


                if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                {
                    var typeVal = BP.Sys.Glo.GenerRealType(attrs, str, ap.GetValStrByKey(str));
                    qo.AddWhere(str, typeVal);

                }
                else
                {
                    qo.AddWhere(str, ap.GetValStrByKey(str));
                }

                qo.addRightBracket();
            }
            #endregion 外键或者枚举的查询

            #region 设置隐藏字段的过滤查询
            FrmBill frmBill = new FrmBill(this.FrmID);
            string hidenField = frmBill.GetParaString("HidenField");

            if (DataType.IsNullOrEmpty(hidenField) == false)
            {
                hidenField = hidenField.Replace("[%]", "%");
                foreach (string field in hidenField.Split(';'))
                {
                    if (field == "")
                        continue;
                    if (field.Split(',').Length != 3)
                        throw new Exception("单据" + frmBill.Name + "的过滤设置规则错误：" + hidenField + ",请联系管理员检查");
                    string[] str = field.Split(',');
                    if (isFirst == false)
                        qo.addAnd();
                    else
                        isFirst = false;
                    qo.addLeftBracket();
                    string val = str[2].Replace("WebUser.No", WebUser.No);
                    val = val.Replace("WebUser.Name", WebUser.Name);
                    val = val.Replace("WebUser.FK_DeptNameOfFull", WebUser.FK_DeptNameOfFull);
                    val = val.Replace("WebUser.FK_DeptName", WebUser.FK_DeptName);
                    val = val.Replace("WebUser.FK_Dept", WebUser.FK_Dept);

                    //获得真实的数据类型.
                    if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                    {
                        var valType = BP.Sys.Glo.GenerRealType(attrs,
                            str[0], val);
                        qo.AddWhere(str[0], str[1], valType);
                    }
                    else
                    {
                        qo.AddWhere(str[0], str[1], val);
                    }
                    qo.addRightBracket();
                    continue;
                }

            }

            #endregion 设置隐藏字段的查询



            if (isFirst == false)
                qo.addAnd();

            qo.AddWhere("BillState", "!=", 0);

            if ((SearchDataRole)md.GetParaInt("SearchDataRole") != SearchDataRole.SearchAll)
            {
                //默认查询本部门的单据
                if ((SearchDataRole)md.GetParaInt("SearchDataRole") == SearchDataRole.ByOnlySelf && DataType.IsNullOrEmpty(hidenField) == true
                || (md.GetParaInt("SearchDataRoleByDeptStation") == 0 && DataType.IsNullOrEmpty(ap.GetValStrByKey("FK_Dept")) == true))
                {
                    qo.addAnd();
                    qo.AddWhere("Starter", "=", WebUser.No);
                }

            }

            #endregion 查询语句
            //如果是总部，按照部门部门排序
            if (this.FrmID == "Bill_ZongBuHuaMingCe")
                qo.addOrderBy("PS_DepartCode");
            //如果是区域，按照区域公司排序
            else if (this.FrmID == "Bill_QuYuRenYuanShanJu")
                qo.addOrderBy("PS_DepartCode");
            //如果是基地，按照基地名称排序
            else if (this.FrmID == "Bill_JDGSRYSJ")
                qo.addOrderBy("PS_BaseCode");
            else
                qo.addOrderBy("OID");
            return qo.DoQueryToTable();

        }
        #endregion  执行导出

        #region 单据导入
        public string ImpData_Done()
        {
            if (SystemConfig.CustomerNo.Equals("ASSET") == true)
                return ImpData_ASSETDone();
            var files = HttpContextHelper.RequestFiles();
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请选择要导入的数据信息。";

            string errInfo = "";

            string ext = ".xls";
            string fileName = System.IO.Path.GetFileName(HttpContextHelper.RequestFiles(0).FileName);
            if (fileName.Contains(".xlsx"))
                ext = ".xlsx";


            //设置文件名
            string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + ext;

            //文件存放路径
            string filePath = SystemConfig.PathOfTemp + "\\" + fileNewName;
            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), filePath);

            //从excel里面获得数据表.
            DataTable dt = DBLoad.ReadExcelFileToDataTable(filePath);

            //删除临时文件
            System.IO.File.Delete(filePath);

            if (dt.Rows.Count == 0)
                return "err@无导入的数据";

            //获得entity.
            FrmBill bill = new FrmBill(this.FrmID);
            GEEntitys rpts = new GEEntitys(this.FrmID);
            GEEntity en = new GEEntity(this.FrmID);



            string noColName = ""; //编号(针对实体表单).
            string nameColName = ""; //名称(针对实体表单).

            BP.En.Map map = en.EnMap;
            Attr attr = map.GetAttrByKey("BillNo");
            noColName = attr.Desc; //
            String codeStruct = bill.EnMap.CodeStruct;
            attr = map.GetAttrByKey("Title");
            nameColName = attr.Desc; //

            //定义属性.
            Attrs attrs = map.Attrs;

            int impWay = this.GetRequestValInt("ImpWay");

            #region 清空方式导入.
            //清空方式导入.
            int count = 0;//导入的行数
            int changeCount = 0;//更新的行数
            String successInfo = "";
            if (impWay == 0)
            {
                rpts.ClearTable();
                GEEntity myen = new GEEntity(this.FrmID);

                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();
                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();
                    myen.OID = 0;

                    //判断是否是自增序列，序列的格式
                    if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                        no = no.PadLeft(System.Int32.Parse(codeStruct), '0');

                    myen.SetValByKey("BillNo", no);
                    if (bill.EntityType == EntityType.FrmDict)
                    {
                        if (myen.Retrieve("BillNo", no) == 1)
                        {
                            errInfo += "err@编号[" + no + "][" + name + "]重复.";
                            continue;
                        }
                    }


                    //给实体赋值
                    errInfo += SetEntityAttrVal(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }

            #endregion 清空方式导入.

            #region 更新方式导入
            if (impWay == 1 || impWay == 2)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();

                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();
                    //判断是否是自增序列，序列的格式
                    if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                    {
                        no = no.PadLeft(System.Int32.Parse(codeStruct), '0');
                    }
                    GEEntity myen = rpts.GetNewEntity as GEEntity;
                    myen.SetValByKey("BillNo", no);
                    if (myen.Retrieve("BillNo", no) == 1 && bill.EntityType == EntityType.FrmDict)
                    {
                        //给实体赋值
                        errInfo += SetEntityAttrVal(no, dr, attrs, myen, dt, 1, bill);
                        changeCount++;
                        successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的更新成功</span><br/>";
                        continue;
                    }


                    //给实体赋值
                    errInfo += SetEntityAttrVal(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }
            #endregion

            return "errInfo=" + errInfo + "@Split" + "count=" + count + "@Split" + "successInfo=" + successInfo + "@Split" + "changeCount=" + changeCount;
        }

        private string SetEntityAttrVal(string no, DataRow dr, Attrs attrs, GEEntity en, DataTable dt, int saveType, FrmBill fbill)
        {

            //单据数据不存在
            if (saveType == 0)
            {
                Int64 oid = 0;
                if (fbill.EntityType == EntityType.FrmDict)
                    oid = BP.CCBill.Dev2Interface.CreateBlankDictID(fbill.No, WebUser.No, null);
                if (fbill.EntityType == EntityType.FrmBill)
                    oid = BP.CCBill.Dev2Interface.CreateBlankBillID(fbill.No, WebUser.No, null);
                en.OID = oid;
                en.RetrieveFromDBSources();
            }

            string errInfo = "";
            //按照属性赋值.
            foreach (Attr item in attrs)
            {
                if (item.Key.Equals("BillNo") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, no);
                    continue;
                }
                if (item.Key.Equals("Title") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, dr[item.Desc].ToString());
                    continue;
                }

                if (dt.Columns.Contains(item.Desc) == false)
                    continue;

                //枚举处理.
                if (item.MyFieldType == FieldType.Enum)
                {
                    string val = dr[item.Desc].ToString();

                    SysEnum se = new SysEnum();
                    int i = se.Retrieve(SysEnumAttr.EnumKey, item.UIBindKey, SysEnumAttr.Lab, val);

                    if (i == 0)
                    {
                        errInfo += "err@枚举[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    en.SetValByKey(item.Key, se.IntKey);
                    continue;
                }

                //外键处理.
                if (item.MyFieldType == FieldType.FK)
                {
                    string val = dr[item.Desc].ToString();
                    Entity attrEn = item.HisFKEn;
                    int i = attrEn.Retrieve("Name", val);
                    if (i == 0)
                    {
                        errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    if (i != 1)
                    {
                        errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]重复..";
                        continue;
                    }

                    //把编号值给他.
                    en.SetValByKey(item.Key, attrEn.GetValByKey("No"));
                    continue;
                }

                //boolen类型的处理..
                if (item.MyDataType == DataType.AppBoolean)
                {
                    string val = dr[item.Desc].ToString();
                    if (val == "是" || val == "有")
                        en.SetValByKey(item.Key, 1);
                    else
                        en.SetValByKey(item.Key, 0);
                    continue;
                }

                string myval = dr[item.Desc].ToString();
                en.SetValByKey(item.Key, myval);
            }
            if (DataType.IsNullOrEmpty(en.GetValStrByKey("BillNo")) == true && DataType.IsNullOrEmpty(fbill.BillNoFormat) == false)
                en.SetValByKey("BillNo", Dev2Interface.GenerBillNo(fbill.BillNoFormat, en.OID, en, fbill.No));

            if (DataType.IsNullOrEmpty(en.GetValStrByKey("Title")) == true && DataType.IsNullOrEmpty(fbill.TitleRole) == false)
                en.SetValByKey("Title", Dev2Interface.GenerTitle(fbill.TitleRole, en));

            en.SetValByKey("BillState", (int)BillState.Editing);
            en.Update();

            GenerBill gb = new GenerBill();
            gb.WorkID = en.OID;
            if (gb.RetrieveFromDBSources() == 0)
            {
                gb.BillState = BillState.Over; //初始化状态.
                gb.Starter = BP.Web.WebUser.No;
                gb.StarterName = BP.Web.WebUser.Name;
                gb.FrmName = fbill.Name; //单据名称.
                gb.FrmID = fbill.No; //单据ID
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.FK_FrmTree = fbill.FK_FormTree; //单据类别.
                gb.RDT = DataType.CurrentDataTime;
                gb.NDStep = 1;
                gb.NDStepName = "启动";
                gb.Insert();

            }
            else
            {
                gb.BillState = BillState.Editing;
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.Update();
            }

            return errInfo;
        }

        #endregion
        /**
         * 针对于北京农芯科技的单据导入的处理
         */
        public string ImpData_ASSETDone()
        {
            var files = HttpContextHelper.RequestFiles();
            if (HttpContextHelper.RequestFilesCount == 0)
                return "err@请选择要导入的数据信息。";

            string errInfo = "";

            string ext = ".xls";
            string fileName = System.IO.Path.GetFileName(HttpContextHelper.RequestFiles(0).FileName);
            if (fileName.Contains(".xlsx"))
                ext = ".xlsx";


            //设置文件名
            string fileNewName = DateTime.Now.ToString("yyyyMMddHHmmssff") + ext;

            //文件存放路径
            string filePath = SystemConfig.PathOfTemp + "\\" + fileNewName;
            HttpContextHelper.UploadFile(HttpContextHelper.RequestFiles(0), filePath);

            //从excel里面获得数据表.
            DataTable dt = DBLoad.ReadExcelFileToDataTable(filePath);

            //删除临时文件
            System.IO.File.Delete(filePath);

            if (dt.Rows.Count == 0)
                return "err@无导入的数据";

            //获得entity.
            FrmBill bill = new FrmBill(this.FrmID);
            GEEntitys rpts = new GEEntitys(this.FrmID);
            GEEntity en = new GEEntity(this.FrmID);


            string noColName = ""; //编号(唯一值)
            string nameColName = ""; //名称
            BP.En.Map map = en.EnMap;
            //获取表单的主键，合同类的(合同编号),人员信息类的(身份证号),其他(BillNo)
            bool isContractBill = false;
            bool isPersonBill = false;

            if (dt.Columns.Contains("合同编号") == true)
            {
                noColName = "合同编号";
                isContractBill = true;
            }

            else if (dt.Columns.Contains("身份证号") == true)
            {
                noColName = "身份证号";
                isPersonBill = true;
            }

            else
            {

                Attr attr = map.GetAttrByKey("BillNo");
                noColName = attr.Desc;
                attr = map.GetAttrByKey("Title");
                nameColName = attr.Desc;
            }


            string codeStruct = bill.EnMap.CodeStruct;


            //定义属性.
            Attrs attrs = map.Attrs;

            int impWay = this.GetRequestValInt("ImpWay");

            #region 清空方式导入.
            //清空方式导入.
            int count = 0;//导入的行数
            int changeCount = 0;//更新的行数
            String successInfo = "";
            if (impWay == 0)
            {
                rpts.ClearTable();
                GEEntity myen = new GEEntity(this.FrmID);

                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();
                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();
                    myen.OID = 0;

                    if (isContractBill == false && isPersonBill == false)
                    {
                        //判断是否是自增序列，序列的格式
                        if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                            no = no.PadLeft(System.Int32.Parse(codeStruct), '0');

                        myen.SetValByKey("BillNo", no);
                        if (bill.EntityType == EntityType.FrmDict)
                        {
                            if (myen.Retrieve("BillNo", no) == 1)
                            {
                                errInfo += "err@编号[" + no + "][" + name + "]重复.";
                                continue;
                            }
                        }
                    }



                    //给实体赋值
                    errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }

            #endregion 清空方式导入.

            #region 更新方式导入
            if (impWay == 1 || impWay == 2)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    //如果是实体单据,导入的excel必须包含BillNo
                    if (bill.EntityType == EntityType.FrmDict && dt.Columns.Contains(noColName) == false)
                        return "err@导入的excel不包含编号列";
                    string no = "";
                    if (dt.Columns.Contains(noColName) == true)
                        no = dr[noColName].ToString();

                    string name = "";
                    if (dt.Columns.Contains(nameColName) == true)
                        name = dr[nameColName].ToString();

                    GEEntity myen = rpts.GetNewEntity as GEEntity;
                    //合同类
                    if (isContractBill == true || isPersonBill == true)
                    {
                        Attr attr = map.GetAttrByDesc(noColName);
                        myen.SetValByKey(attr.Key, no);
                        //存在就编辑修改数据
                        if (myen.Retrieve(attr.Key, no) == 1)
                        {
                            //给实体赋值
                            errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 1, bill);
                            changeCount++;
                            successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的更新成功</span><br/>";
                            continue;
                        }
                        else
                        {
                            //给实体赋值
                            errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 0, bill);
                            count++;
                            successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                            continue;
                        }

                    }
                    else
                    {
                        //判断是否是自增序列，序列的格式
                        if (DataType.IsNullOrEmpty(codeStruct) == false && DataType.IsNullOrEmpty(no) == false)
                        {
                            no = no.PadLeft(System.Int32.Parse(codeStruct), '0');
                        }
                        myen.SetValByKey("BillNo", no);
                        if (myen.Retrieve("BillNo", no) == 1 && bill.EntityType == EntityType.FrmDict)
                        {
                            //给实体赋值
                            errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 1, bill);
                            changeCount++;
                            successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的更新成功</span><br/>";
                            continue;
                        }
                    }

                    //给实体赋值
                    errInfo += SetEntityAttrValForASSET(no, dr, attrs, myen, dt, 0, bill);
                    count++;
                    successInfo += "&nbsp;&nbsp;<span>" + noColName + "为" + no + "," + nameColName + "为" + name + "的导入成功</span><br/>";
                }
            }
            #endregion

            return "errInfo=" + errInfo + "@Split" + "count=" + count + "@Split" + "successInfo=" + successInfo + "@Split" + "changeCount=" + changeCount;
        }
        private string SetEntityAttrValForASSET(string no, DataRow dr, Attrs attrs, GEEntity en, DataTable dt, int saveType, FrmBill fbill)
        {

            //单据数据不存在
            if (saveType == 0)
            {
                Int64 oid = 0;
                if (fbill.EntityType == EntityType.FrmDict)
                    oid = BP.CCBill.Dev2Interface.CreateBlankDictID(fbill.No, WebUser.No, null);
                if (fbill.EntityType == EntityType.FrmBill)
                    oid = BP.CCBill.Dev2Interface.CreateBlankBillID(fbill.No, WebUser.No, null);
                en.OID = oid;
                en.RetrieveFromDBSources();
            }
            string errInfo = "";


            //按照属性赋值.
            foreach (Attr item in attrs)
            {
                if (item.Key.Equals("BillNo") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, no);
                    continue;
                }
                if (item.Key.Equals("Title") && dt.Columns.Contains(item.Desc) == true)
                {
                    en.SetValByKey(item.Key, dr[item.Desc].ToString());
                    continue;
                }

                if (dt.Columns.Contains(item.Desc) == false)
                    continue;
                string val = dr[item.Desc].ToString();
                //枚举处理.
                if (item.MyFieldType == FieldType.Enum)
                {
                    SysEnum se = new SysEnum();
                    int i = se.Retrieve(SysEnumAttr.EnumKey, item.UIBindKey, SysEnumAttr.Lab, val);

                    if (i == 0)
                    {
                        errInfo += "err@枚举[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                        continue;
                    }

                    en.SetValByKey(item.Key, se.IntKey);
                    //en.SetValByKey(item.Key.Replace("Code",""), val);
                    continue;
                }


                //外键处理.
                if (item.MyFieldType == FieldType.FK)
                {
                    if (item.Key == "PS_DepartCode")
                    {
                        string keyNo = BP.DA.DBAccess.RunSQLReturnString("select No from Port_Dept where Name='" + val + "' and parentNo='1002001'");
                        if (!string.IsNullOrWhiteSpace(keyNo))
                        {
                            en.SetValByKey("PS_DepartName", val);
                            en.SetValByKey("PS_DepartCodeT", val);
                            en.SetValByKey(item.Key, keyNo);
                        }
                        continue;
                    }
                    else
                    {
                        Entity attrEn = item.HisFKEn;
                        int i = attrEn.Retrieve("Name", val);
                        if (i == 0)
                        {
                            errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]不存在.";
                            continue;
                        }

                        if (i != 1)
                        {
                            errInfo += "err@外键[" + item.Key + "][" + item.Desc + "]，值[" + val + "]重复..";
                            continue;
                        }

                        //把编号值给他.
                        en.SetValByKey(item.Key, attrEn.GetValByKey("No"));
                        if (item.Key.EndsWith("BaseCode") == true)
                            en.SetValByKey(item.Key.Replace("BaseCode", "BaseName"), val);
                        else
                            en.SetValByKey(item.Key.Replace("Code", ""), val);
                    }
                    continue;
                }
                //外部数据源
                if (item.MyFieldType == FieldType.Normal && item.MyDataType == DataType.AppString && item.UIContralType == UIContralType.DDL)
                {
                    if (item.Key == "PS_DepartCode")
                    {
                        string keyNo = BP.DA.DBAccess.RunSQLReturnString("select No from Port_Dept where Name='" + val + "' and parentNo='1002001'");
                        if (!string.IsNullOrWhiteSpace(keyNo))
                        {
                            en.SetValByKey("PS_DepartName", val);
                            en.SetValByKey("PS_DepartCodeT", val);
                            en.SetValByKey(item.Key, keyNo);
                        }
                        continue;
                    }
                    else
                    {
                        string uiBindKey = item.UIBindKey;
                        if (DataType.IsNullOrEmpty(uiBindKey) == true)
                            errInfo += "err@外部数据源[" + item.Key + "][" + item.Desc + "]，绑定的外键为空";
                        DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
                        if (mydt.Rows.Count == 0)
                            errInfo += "err@外部数据源[" + item.Key + "][" + item.Desc + "],对应的外键没有获取到外键列表";
                        bool isHave = false;

                        //给赋值名称
                        if (item.Key.EndsWith("BaseCode") == true)
                            en.SetValByKey(item.Key.Replace("BaseCode", "BaseName"), val);
                        else
                            en.SetValByKey(item.Key.Replace("Code", ""), val);

                        en.SetValByKey(item.Key + "T", val);
                        foreach (DataRow mydr in mydt.Rows)
                        {
                            if (mydr["Name"].ToString().Equals(val) == true)
                            {
                                en.SetValByKey(item.Key, mydr["No"].ToString());
                                isHave = true;
                                break;
                            }
                        }

                        if (isHave == false)
                            errInfo += "err@外部数据源[" + item.Key + "][" + item.Desc + "],没有获取到" + val + "对应的Code值";

                    }
                    continue;
                }

                //boolen类型的处理..
                if (item.MyDataType == DataType.AppBoolean)
                {
                    if (val == "是" || val == "有")
                        en.SetValByKey(item.Key, 1);
                    else
                        en.SetValByKey(item.Key, 0);
                    continue;
                }
                if (item.MyDataType == DataType.AppDate)
                {
                    if (DataType.IsNullOrEmpty(val) == false)
                    {

                    }

                }

                if (item.Key == "PS_DepartCode")
                {
                    string keyNo = BP.DA.DBAccess.RunSQLReturnString("select No from Port_Dept where Name='" + val + "' and parentNo='1002001'");
                    if (!string.IsNullOrWhiteSpace(keyNo))
                    {
                        en.SetValByKey("PS_DepartName", val);
                        en.SetValByKey("PS_DepartCodeT", val);
                        en.SetValByKey(item.Key, keyNo);
                    }
                    continue;
                }
                if (item.Key.EndsWith("BaseName") == true)
                {
                    Depts depts = new Depts();
                    depts.Retrieve(DeptAttr.Name, val);
                    if (depts.Count != 0)
                        en.SetValByKey(item.Key.Replace("BaseName", "BaseCode"), (depts[0] as Dept).No);
                    en.SetValByKey(item.Key, val);
                    continue;
                }
                else
                {
                    if (item.Key=="CI_SmallBusinessFormatCode")
                    {
                        string mypk = "MultipleChoiceSmall_" + fbill.No + "_" + item.Key;
                        MapExt mapExt = new MapExt();
                        mapExt.MyPK = mypk;
                        if (mapExt.RetrieveFromDBSources() == 1 && mapExt.DoWay == 3 && DataType.IsNullOrEmpty(mapExt.Tag3) == false)
                        {
                            string newVal = "," + val + ",";
                            string keyVal = "";
                            DataTable dataTable = BP.Pub.PubClass.GetDataTableByUIBineKey(mapExt.Tag3);
                            foreach (DataRow drr in dataTable.Rows)
                            {
                                if (drr["Name"] != null && (newVal.Contains("," + drr["Name"].ToString() + ",") == true || newVal.Contains("," + drr["Name"].ToString() + "，") == true || newVal.Contains("，" + drr["Name"].ToString() + ",") == true || newVal.Contains("，" + drr["Name"].ToString() + "，") == true))
                                    keyVal += drr["No"].ToString() + ",";
                            }
                            keyVal = keyVal.Length > 0 ? keyVal.Substring(0, keyVal.Length - 1) : "";

                            en.SetValByKey(item.Key, keyVal);
                            en.SetValByKey(item.Key.Replace("Code", ""), val);
                            en.SetValByKey(item.Key + "T", val);
                        }
                        else
                        {
                            en.SetValByKey(item.Key, val);
                        }
                    }
                    else if (item.Key=="PS_PostCode")
                    {
                        string mypk = "MultipleChoiceSmall_" + fbill.No + "_" + item.Key;
                        MapExt mapExt = new MapExt();
                        mapExt.MyPK = mypk;
                        if (mapExt.RetrieveFromDBSources() == 1 && mapExt.DoWay == 3 && DataType.IsNullOrEmpty(mapExt.Tag3) == false)
                        {
                            string newVal = "," + val + ",";
                            string keyVal = "";
                            DataTable dataTable = BP.Pub.PubClass.GetDataTableByUIBineKey(mapExt.Tag3);
                            dataTable = DBAccess.RunSQLReturnTable("select * FROM PORT_STATION WHERE FK_StationType IN(SELECT NO FROM Port_StationType WHERE NAME LIKE '农发总部%')");
                            foreach (DataRow drr in dataTable.Rows)
                            {
                                if (drr["Name"] != null && (newVal.Contains("," + drr["Name"].ToString() + ",") == true || newVal.Contains("," + drr["Name"].ToString() + "，") == true || newVal.Contains("，" + drr["Name"].ToString() + ",") == true || newVal.Contains("，" + drr["Name"].ToString() + "，") == true))
                                    keyVal += drr["No"].ToString() + ",";
                            }
                            keyVal = keyVal.Length > 0 ? keyVal.Substring(0, keyVal.Length - 1) : "";

                            en.SetValByKey(item.Key, keyVal);
                            en.SetValByKey("PS_PostName", val);
                            en.SetValByKey("PS_PostCodeT", val);
                        }
                        else
                        {
                            en.SetValByKey(item.Key, val);
                        }
                        continue;
                    }
                    else
                    {
                        if (item.IsNum)
                        {
                            if (DataType.IsNullOrEmpty(val) == true || val.Equals("null") == true)
                                val = "0";
                        }
                        en.SetValByKey(item.Key, val);
                    }


                }


            }
            if (DataType.IsNullOrEmpty(en.GetValStrByKey("BillNo")) == true && DataType.IsNullOrEmpty(fbill.BillNoFormat) == false)
                en.SetValByKey("BillNo", Dev2Interface.GenerBillNo(fbill.BillNoFormat, en.OID, en, fbill.No));

            if (DataType.IsNullOrEmpty(en.GetValStrByKey("Title")) == true && DataType.IsNullOrEmpty(fbill.TitleRole) == false)
                en.SetValByKey("Title", Dev2Interface.GenerTitle(fbill.TitleRole, en));
            en.SetValByKey("Rec", WebUser.No);
            en.SetValByKey("BillState", (int)BillState.Editing);
            en.DirectUpdate();

            en.RunSQL("UPDATE "+en.EnMap.FK_MapData+ " set WFState=3 WHERE OID="+ en.OID+"");

            GenerBill gb = new GenerBill();
            gb.WorkID = en.OID;
            if (gb.RetrieveFromDBSources() == 0)
            {
                gb.BillState = BillState.Over; //初始化状态.
                gb.Starter = BP.Web.WebUser.No;
                gb.StarterName = BP.Web.WebUser.Name;
                gb.FrmName = fbill.Name; //单据名称.
                gb.FrmID = fbill.No; //单据ID
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.FK_FrmTree = fbill.FK_FormTree; //单据类别.
                gb.RDT = DataType.CurrentDataTime;
                gb.NDStep = 1;
                gb.NDStepName = "启动";
                gb.Insert();

            }
            else
            {
                gb.BillState = BillState.Editing;
                if (en.Row.ContainsKey("Title") == true)
                    gb.Title = en.GetValStringByKey("Title");
                if (en.Row.ContainsKey("BillNo") == true)
                    gb.BillNo = en.GetValStringByKey("BillNo");
                gb.Update();
            }

            return errInfo;
        }

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
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.

        #region 获得demo信息.
        public string MethodDocDemoJS_Init()
        {
            var func = new MethodFunc(this.MyPK);
            return func.MethodDoc_JavaScript_Demo;
        }
        public string MethodDocDemoSQL_Init()
        {
            var func = new MethodFunc(this.MyPK);
            return func.MethodDoc_SQL_Demo;
        }
        #endregion 获得demo信息.

        #region 处理SQL文中注释信息.

        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            int startindex, endindex;
            string tmpstr = string.Empty;
            string tmpstr2 = string.Empty;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return sourse;
                int i = 0;
                while (startindex != -1)
                {
                    if (i == 0)
                    {
                        endindex = sourse.IndexOf(endstr);
                        if (startindex != 0)
                        {
                            endindex = endindex - startindex;
                        }
                        tmpstr = sourse.Remove(startindex, endindex + endstr.Length);
                    }
                    else
                    {
                        endindex = tmpstr.IndexOf(endstr);
                        if (startindex != 0)
                        {
                            endindex = endindex - startindex;
                        }
                        tmpstr = tmpstr.Remove(startindex, endindex + endstr.Length);

                    }

                    if (endindex == -1)
                        return tmpstr;
                    // tmpstr = tmpstr.Substring(endindex + endstr.Length);
                    startindex = tmpstr.IndexOf(startstr);
                    i++;
                }
                //result = tmpstr.Remove(endindex);

            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineInfo("MidStrEx Err:" + ex.Message);
            }
            return tmpstr;
        }
        #endregion 处理SQL文中注释信息..
    }
}
