using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Sys;
using BP.Difference;
using BP.WF.Template.Frm;
using System.Text.RegularExpressions;
using BP.Tools;
using Newtonsoft.Json.Linq;

namespace BP.WF
{
    /// <summary>
    /// 表单引擎api
    /// </summary>
    public class CCFormAPI : Dev2Interface
    {
        /// <summary>
        /// 获得Pop的字段的值
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="pk"></param>
        /// <returns>返回data, No=编号,Name=名称两个列. </returns>
        /// <exception cref="Exception"></exception>
        public static DataTable GenerPopData2022(string pk, string fieldName)
        {
            //判断该字段是否启用了pop返回值？
            string sql = "SELECT  Tag1 AS VAL FROM Sys_FrmEleDB WHERE RefPKVal=" + pk + " AND EleID='" + fieldName + "'";
            string emps = "";
            DataTable dtVals = DBAccess.RunSQLReturnTable(sql);

            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Name");

            //获取接受人并格式化接受人, 
            if (dtVals.Rows.Count > 0)
            {
                foreach (DataRow dr in dtVals.Rows)
                    emps += dr[0].ToString() + ",";
            }

            if (emps.Contains(",") && emps.Contains(";"))
            {
                /*如果包含,; 例如 zhangsan,张三;lisi,李四;*/
                string[] myemps1 = emps.Split(';');
                foreach (string str in myemps1)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;

                    string[] ss = str.Split(',');
                    DataRow dr = dt.NewRow();
                    dr[0] = ss[0];
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            emps = emps.Replace(";", ",");
            emps = emps.Replace("；", ",");
            emps = emps.Replace("，", ",");
            emps = emps.Replace("、", ",");
            emps = emps.Replace("@", ",");

            // 把它加入接受人员列表中.
            string[] myemps = emps.Split(',');
            foreach (string s in myemps)
            {
                if (DataType.IsNullOrEmpty(s))
                    continue;

                DataRow dr = dt.NewRow();
                dr[0] = s;
                dt.Rows.Add(dr);
            }
            return dt;
        }
        /// <summary>
        /// 仅获取表单数据
        /// </summary>
        /// <param name="enName"></param>
        /// <param name="pkval"></param>
        /// <param name="atParas"></param>
        /// <param name="specDtlFrmID"></param>
        /// <returns></returns>
        private static DataSet GenerDBForVSTOExcelFrmModelOfEntity(string enName, object pkval, string atParas, string specDtlFrmID = null)
        {
            DataSet myds = new DataSet();

            #region 主表

            Entity en = BP.En.ClassFactory.GetEn(enName);
            en.PKVal = pkval;

            // if (DataType.IsNullOrEmpty(pkval)==false)
            en.Retrieve();


            //设置外部传入的默认值.
            if (BP.Difference.SystemConfig.isBSsystem == true)
            {
                // 处理传递过来的参数。
                //2019-07-25 zyt改造
                foreach (string k in HttpContextHelper.RequestParamKeys)
                {
                    en.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }
            }

            //主表数据放入集合.
            DataTable mainTable = en.ToDataTableField();
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);

            #region 主表 Sys_MapData
            string sql = "SELECT * FROM Sys_MapData WHERE 1=2 ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";

            Map map = en.EnMapInTime;
            DataRow dr = dt.NewRow();
            dr[MapDataAttr.No] = enName;
            dr[MapDataAttr.Name] = map.EnDesc;
            dr[MapDataAttr.PTable] = map.PhysicsTable;
            dt.Rows.Add(dr);
            myds.Tables.Add(dt);
            #endregion 主表 Sys_MapData

            #region 主表 Sys_MapAttr
            sql = "SELECT * FROM Sys_MapAttr WHERE 1=2 ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";
            foreach (Attr attr in map.Attrs)
            {
                dr = dt.NewRow();
                dr[MapAttrAttr.MyPK] = enName + "_" + attr.Key;
                dr[MapAttrAttr.Name] = attr.Desc;

                dr[MapAttrAttr.MyDataType] = attr.MyDataType;   //数据类型.
                dr[MapAttrAttr.MinLen] = attr.MinLength;   //最小长度.
                dr[MapAttrAttr.MaxLen] = attr.MaxLength;   //最大长度.

                // 设置他的逻辑类型.
                dr[MapAttrAttr.LGType] = 0; //逻辑类型.
                switch (attr.MyFieldType)
                {
                    case FieldType.Enum:
                        dr[MapAttrAttr.LGType] = 1;
                        dr[MapAttrAttr.UIBindKey] = attr.UIBindKey;

                        //增加枚举字段.
                        if (myds.Tables.Contains(attr.UIBindKey) == false)
                        {
                            string mysql = "SELECT IntKey AS No, Lab as Name FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + attr.UIBindKey + "' ORDER BY IntKey ";
                            DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
                            dtEnum.TableName = attr.UIBindKey;
                            myds.Tables.Add(dtEnum);
                        }

                        break;
                    case FieldType.FK:
                        dr[MapAttrAttr.LGType] = 2;

                        Entities ens = attr.HisFKEns;
                        dr[MapAttrAttr.UIBindKey] = ens.ToString();

                        //把外键字段也增加进去.
                        if (myds.Tables.Contains(ens.ToString()) == false && attr.UIIsReadonly == false)
                        {
                            ens.RetrieveAll();
                            DataTable mydt = ens.ToDataTableDescField();
                            mydt.TableName = ens.ToString();
                            myds.Tables.Add(mydt);
                        }
                        break;
                    default:
                        break;
                }

                // 设置控件类型.
                dr[MapAttrAttr.UIContralType] = (int)attr.UIContralType;
                dt.Rows.Add(dr);
            }
            myds.Tables.Add(dt);
            #endregion 主表 Sys_MapAttr

            #region //主表 Sys_MapExt 扩展属性
            ////主表的配置信息.
            //sql = "SELECT * FROM Sys_MapExt WHERE 1=2";
            //dt = DBAccess.RunSQLReturnTable(sql);
            //dt.TableName = "Sys_MapExt";
            //myds.Tables.Add(dt);
            #endregion //主表 Sys_MapExt 扩展属性

            #endregion

            #region 从表
            foreach (EnDtl item in map.Dtls)
            {
                #region  把从表的数据放入集合.

                Entities dtls = item.Ens;

                QueryObject qo = qo = new QueryObject(dtls);

                if (dtls.ToString().Contains("CYSheBeiUse") == true)
                    qo.addOrderBy("RDT"); //按照日期进行排序，不用也可以.

                qo.AddWhere(item.RefKey, pkval);
                DataTable dtDtl = qo.DoQueryToTable();

                dtDtl.TableName = item.EnsName; //修改明细表的名称.
                myds.Tables.Add(dtDtl); //加入这个明细表.

                #endregion 把从表的数据放入.

                #region 从表 Sys_MapDtl (相当于mapdata)

                Entity dtl = dtls.GetNewEntity;
                map = dtl.EnMap;

                //明细表的 描述 . 
                sql = "SELECT * FROM Sys_MapDtl WHERE 1=2";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_MapDtl_For_" + item.EnsName;

                dr = dt.NewRow();
                dr[MapDtlAttr.No] = item.EnsName;
                dr[MapDtlAttr.Name] = item.Desc;
                dr[MapDtlAttr.PTable] = dtl.EnMap.PhysicsTable;
                dt.Rows.Add(dr);
                myds.Tables.Add(dt);

                #endregion 从表 Sys_MapDtl (相当于mapdata)

                #region 明细表 Sys_MapAttr
                sql = "SELECT * FROM Sys_MapAttr WHERE 1=2";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_MapAttr_For_" + item.EnsName;
                foreach (Attr attr in map.Attrs)
                {
                    dr = dt.NewRow();
                    dr[MapAttrAttr.MyPK] = enName + "_" + attr.Key;
                    dr[MapAttrAttr.Name] = attr.Desc;

                    dr[MapAttrAttr.MyDataType] = attr.MyDataType;   //数据类型.
                    dr[MapAttrAttr.MinLen] = attr.MinLength;   //最小长度.
                    dr[MapAttrAttr.MaxLen] = attr.MaxLength;   //最大长度.

                    // 设置他的逻辑类型.
                    dr[MapAttrAttr.LGType] = 0; //逻辑类型.
                    switch (attr.MyFieldType)
                    {
                        case FieldType.Enum:
                            dr[MapAttrAttr.LGType] = 1;
                            dr[MapAttrAttr.UIBindKey] = attr.UIBindKey;

                            //增加枚举字段.
                            if (myds.Tables.Contains(attr.UIBindKey) == false)
                            {
                                string mysql = "SELECT IntKey AS No, Lab as Name FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + attr.UIBindKey + "' ORDER BY IntKey ";
                                DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
                                dtEnum.TableName = attr.UIBindKey;
                                myds.Tables.Add(dtEnum);
                            }
                            break;
                        case FieldType.FK:
                            dr[MapAttrAttr.LGType] = 2;

                            Entities ens = attr.HisFKEns;
                            dr[MapAttrAttr.UIBindKey] = ens.ToString();

                            //把外键字段也增加进去.
                            if (myds.Tables.Contains(ens.ToString()) == false && attr.UIIsReadonly == false)
                            {
                                ens.RetrieveAll();
                                DataTable mydt = ens.ToDataTableDescField();
                                mydt.TableName = ens.ToString();
                                myds.Tables.Add(mydt);
                            }
                            break;
                        default:
                            break;
                    }

                    // 设置控件类型.
                    dr[MapAttrAttr.UIContralType] = (int)attr.UIContralType;
                    dt.Rows.Add(dr);
                }
                myds.Tables.Add(dt);
                #endregion 明细表 Sys_MapAttr

            }
            #endregion

            return myds;
        }
        /// <summary>
        /// 仅获取表单数据
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="pkval">主键</param>
        /// <param name="atParas">参数</param>
        /// <param name="specDtlFrmID">指定明细表的参数，如果为空就标识主表数据，否则就是从表数据.</param>
        /// <returns>数据</returns>
        public static DataSet GenerDBForVSTOExcelFrmModel(string frmID, object pkval, string atParas, string specDtlFrmID = null)
        {
            //如果是一个实体类.
            if (frmID.ToUpper().Contains("BP."))
            {
                // 执行map同步.
                Entities ens = BP.En.ClassFactory.GetEns(frmID + "s");
                Entity myen = ens.GetNewEntity;
                myen.DTSMapToSys_MapData();
                return GenerDBForVSTOExcelFrmModelOfEntity(frmID, pkval, atParas, specDtlFrmID = null);
            }

            //数据容器,就是要返回的对象.
            DataSet myds = new DataSet();

            //映射实体.
            MapData md = new MapData(frmID);

            Map map = md.GenerHisMap();

            Entity en = null;
            if (map.Attrs.Contains("OID") == true)
            {
                //实体.
                GEEntity wk = new GEEntity(frmID);
                wk.OID = int.Parse(pkval.ToString());
                if (wk.RetrieveFromDBSources() == 0)
                    wk.Insert();

                ExecEvent.DoFrm(md, EventListFrm.FrmLoadBefore, wk, null);

                en = wk;
            }

            if (map.Attrs.Contains("MyPK") == true)
            {
                //实体.
                GEEntityMyPK wk = new GEEntityMyPK(frmID);
                wk.setMyPK(pkval.ToString());
                if (wk.RetrieveFromDBSources() == 0)
                    wk.Insert();
                ExecEvent.DoFrm(md, EventListFrm.FrmLoadBefore, wk, null);
                en = wk;
            }

            //加载事件.

            //把参数放入到 En 的 Row 里面。
            if (DataType.IsNullOrEmpty(atParas) == false)
            {
                AtPara ap = new AtPara(atParas);
                foreach (string key in ap.HisHT.Keys)
                {
                    switch (key)
                    {
                        case "FrmID":
                        case "FK_MapData":
                            continue;
                        default:
                            break;
                    }

                    if (en.Row.ContainsKey(key) == true) //有就该变.
                        en.Row[key] = ap.GetValStrByKey(key);
                    else
                        en.Row.Add(key, ap.GetValStrByKey(key)); //增加他.
                }
            }

            //属性.
            MapExt me = null;
            DataTable dtMapAttr = null;
            MapExts mes = null;

            #region 表单模版信息.（含主、从表的，以及从表的枚举/外键相关数据）.
            //增加表单字段描述.
            string sql = "SELECT * FROM Sys_MapData WHERE No='" + frmID + "' ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            myds.Tables.Add(dt);

            //增加表单字段描述.
            sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "' ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";
            myds.Tables.Add(dt);

            //增加从表信息.
            sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData='" + frmID + "' ";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapDtl";
            myds.Tables.Add(dt);


            //主表的配置信息.
            sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + frmID + "'";
            dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapExt";
            myds.Tables.Add(dt);

            #region 加载 从表表单模版信息.（含 从表的枚举/外键相关数据）
            foreach (MapDtl item in md.MapDtls)
            {
                #region 返回指定的明细表的数据.
                if (DataType.IsNullOrEmpty(specDtlFrmID) == true)
                {
                }
                else
                {
                    if (item.No != specDtlFrmID)
                        continue;
                }
                #endregion 返回指定的明细表的数据.

                //明细表的主表描述
                sql = "SELECT * FROM Sys_MapDtl WHERE No='" + item.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_MapDtl_For_" + (DataType.IsNullOrEmpty(item.Alias) ? item.No : item.Alias);
                myds.Tables.Add(dt);

                //明细表的表单描述
                sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + item.No + "'";
                dtMapAttr = DBAccess.RunSQLReturnTable(sql);
                dtMapAttr.TableName = "Sys_MapAttr_For_" + (DataType.IsNullOrEmpty(item.Alias) ? item.No : item.Alias);
                myds.Tables.Add(dtMapAttr);

                //明细表的配置信息.
                sql = "SELECT * FROM Sys_MapExt WHERE FK_MapData='" + item.No + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "Sys_MapExt_For_" + (DataType.IsNullOrEmpty(item.Alias) ? item.No : item.Alias);
                myds.Tables.Add(dt);

                #region 从表的 外键表/枚举
                mes = new MapExts(item.No);
                foreach (DataRow dr in dtMapAttr.Rows)
                {
                    string lgType = dr["LGType"].ToString();
                    //不是枚举/外键字段
                    if (lgType.Equals("0"))
                        continue;

                    string uiBindKey = dr["UIBindKey"].ToString();
                    string mypk = dr["MyPK"].ToString();

                    #region 枚举字段
                    if (lgType.Equals("1"))
                    {
                        // 如果是枚举值, 判断是否存在.
                        if (myds.Tables.Contains(uiBindKey) == true)
                            continue;

                        string mysql = "SELECT IntKey AS No, Lab as Name FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
                        DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
                        dtEnum.TableName = uiBindKey;
                        myds.Tables.Add(dtEnum);
                        continue;
                    }
                    #endregion

                    string UIIsEnable = dr["UIIsEnable"].ToString();
                    if (UIIsEnable.Equals("0")) //字段未启用
                        continue;

                    #region 外键字段
                    // 检查是否有下拉框自动填充。
                    string keyOfEn = dr["KeyOfEn"].ToString();

                    #region 处理下拉框数据范围. for 小杨.
                    me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                    if (me != null) //有范围限制时
                    {
                        string fullSQL = me.Doc.Clone() as string;
                        if (DataType.IsNullOrEmpty(fullSQL) == true)
                            throw new Exception("err@没有给AutoFullDLL配置SQL：MapExt：=" + me.MyPK + ",原始的配置SQL为:" + me.Doc);
                        fullSQL = fullSQL.Replace("~", ",");
                        fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);

                        dt = DBAccess.RunSQLReturnTable(fullSQL);

                        dt.TableName = mypk;
                        myds.Tables.Add(dt);
                        continue;
                    }
                    #endregion 处理下拉框数据范围.
                    else //无范围限制时
                    {
                        // 判断是否存在.
                        if (myds.Tables.Contains(uiBindKey) == true)
                            continue;

                        myds.Tables.Add(BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey));
                    }
                    #endregion 外键字段
                }
                #endregion 从表的 外键表/枚举

            }
            #endregion 加载 从表表单模版信息.（含 从表的枚举/外键相关数据）

            #endregion 表单模版信息.（含主、从表的，以及从表的枚举/外键相关数据）.

            #region 主表数据
            if (BP.Difference.SystemConfig.isBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in HttpContextHelper.RequestParamKeys)
                {
                    en.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }
            }

            // 执行表单事件..
            string msg = ExecEvent.DoFrm(md, EventListFrm.FrmLoadBefore, en);
            if (DataType.IsNullOrEmpty(msg) == false)
                throw new Exception("err@错误:" + msg);

            //重设默认值.
            en.ResetDefaultVal();

            //执行装载填充.
            me = new MapExt();
            if (me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, frmID) == 1)
            {
                //执行通用的装载方法.
                MapAttrs attrs = new MapAttrs(frmID);
                MapDtls dtls = new MapDtls(frmID);
                en = DealPageLoadFull(en, me, attrs, dtls) as GEEntity;
            }

            //增加主表数据.
            DataTable mainTable = en.ToDataTableField(md.No);
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);

            #endregion 主表数据

            #region  从表数据
            foreach (MapDtl dtl in md.MapDtls)
            {
                #region 返回指定的明细表的数据.
                if (DataType.IsNullOrEmpty(specDtlFrmID) == true)
                {
                }
                else
                {
                    if (dtl.No != specDtlFrmID)
                        continue;
                }
                #endregion 返回指定的明细表的数据.

                GEDtls dtls = new GEDtls(dtl.No);
                QueryObject qo = null;

                if (dtl.RefPK == "")
                {
                    try
                    {
                        qo = new QueryObject(dtls);
                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                qo.AddWhere(GEDtlAttr.RefPK, pkval);
                                qo.addAnd();
                                qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                qo.AddWhere(GEDtlAttr.RefPK, pkval);
                                break;
                            case DtlOpenType.ForPWorkID: // 按工作ID来控制
                                qo.AddWhere(GEDtlAttr.RefPK, DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + pkval));
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                qo.AddWhere(GEDtlAttr.FID, pkval);
                                break;
                        }
                    }
                    catch
                    {
                        dtls.GetNewEntity.CheckPhysicsTable();
                    }
                }
                else
                {
                    qo = new QueryObject(dtls);
                    qo.AddWhere(dtl.RefPK, pkval);
                }

                //条件过滤.
                if (DataType.IsNullOrEmpty(dtl.FilterSQLExp) == false)
                {
                    string[] strs = dtl.FilterSQLExp.Split('=');
                    qo.addAnd();
                    qo.AddWhere(strs[0], strs[1]);
                }

                //排序.
                if (DataType.IsNullOrEmpty(dtl.OrderBySQLExp) == false)
                {
                    qo.addOrderBy(dtl.OrderBySQLExp);
                }


                //从表
                DataTable dtDtl = qo.DoQueryToTable();

                // 为明细表设置默认值.
                MapAttrs mattrs = new MapAttrs(dtl.No);
                foreach (MapAttr attr in mattrs)
                {
                    //处理它的默认值.
                    if (attr.DefValReal.Contains("@") == false)
                        continue;

                    foreach (DataRow dr in dtDtl.Rows)
                        dr[attr.KeyOfEn] = attr.DefVal;
                }

                dtDtl.TableName = DataType.IsNullOrEmpty(dtl.Alias) ? dtl.No : dtl.Alias; //edited by liuxc,2017-10-10.如果有别名，则使用别名，没有则使用No
                myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
            }
            #endregion 从表数据

            #region 主表的 外键表/枚举
            dtMapAttr = myds.Tables["Sys_MapAttr"];
            mes = md.MapExts;
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string uiBindKey = dr["UIBindKey"] as string;
                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue;

                string myPK = dr["MyPK"].ToString();
                string lgType = dr["LGType"].ToString();

                if (lgType.Equals("1"))
                {
                    // 如果是枚举值, 判断是否存在., 
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;

                    string mysql = "SELECT IntKey AS No, Lab as Name FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
                    DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
                    dtEnum.TableName = uiBindKey;
                    myds.Tables.Add(dtEnum);
                    continue;
                }

                if (lgType.Equals("2") == false)
                    continue;

                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable.Equals("0"))
                    continue;

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null)
                {
                    string fullSQL = me.Doc.Clone() as string;
                    if (DataType.IsNullOrEmpty(fullSQL) == true)
                        throw new Exception("err@没有给AutoFullDLL配置SQL：MapExt：=" + me.MyPK + ",原始的配置SQL为:" + me.Doc);
                    fullSQL = fullSQL.Replace("~", ",");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);
                    dt = DBAccess.RunSQLReturnTable(fullSQL);
                    dt.TableName = myPK; //可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                    myds.Tables.Add(dt);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                dt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey);
                dt.TableName = uiBindKey;
                myds.Tables.Add(dt);
            }
            #endregion 主表的 外键表/枚举

            string name = "";
            foreach (DataTable item in myds.Tables)
            {
                name += item.TableName + ",";
            }
            //返回生成的dataset.
            return myds;
        }
        /// <summary>
        /// 执行PageLoad装载数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="en"></param>
        /// <param name="mattrs"></param>
        /// <param name="dtls"></param>
        /// <returns></returns>
        public static Entity DealPageLoadFull(Entity en, MapExt item, MapAttrs mattrs, MapDtls dtls, bool isSelf = false, int nodeID = 0, long workID = 0)
        {
            if (item == null)
                return en;

            DataTable dt = null;
            string sql = item.Doc;
            /* 如果有填充主表的sql  */
            sql = Glo.DealExp(sql, en, null);
            string fk_dbSrc = item.DBSrcNo;
            //填充方式，0=sql，1=url,2=CCFromRef.js , 3=webapi
            string doWay = item.DoWay;

            SFDBSrc sfdb = null;
            //如果是sql方式填充
            if (doWay.Equals("0") || doWay.Equals("None"))
            {
                if (DataType.IsNullOrEmpty(fk_dbSrc) == false && fk_dbSrc.Equals("local") == false)
                    sfdb = new SFDBSrc(fk_dbSrc);
                if (string.IsNullOrEmpty(sql) == false)
                {
                    if (string.IsNullOrEmpty(sql) == false)
                    {
                        int num = Regex.Matches(sql.ToUpper(), "WHERE").Count;
                        if (num == 1)
                        {
                            string sqlext = sql.Substring(0, sql.ToUpper().IndexOf("WHERE"));
                            sqlext = sql.Substring(sqlext.Length + 1);
                            if (sqlext.Contains("@"))
                                throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                        }
                        if (num > 1 && sql.Contains("@"))
                            throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                        if (sfdb != null)
                            dt = sfdb.RunSQLReturnTable(sql);
                        else
                            dt = DBAccess.RunSQLReturnTable(sql);

                        Attrs attrs = en.EnMap.Attrs;

                        if (dt.Rows.Count == 1)
                        {
                            DataRow dr = dt.Rows[0];
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //去掉一些不需要copy的字段.
                                switch (dc.ColumnName)
                                {
                                    case WorkAttr.OID:
                                    case WorkAttr.FID:
                                    case WorkAttr.Rec:
                                    case WorkAttr.MD5:
                                    case GERptAttr.FlowEnder:
                                    case GERptAttr.FlowEnderRDT:
                                    case GERptAttr.AtPara:
                                    case GERptAttr.PFlowNo:
                                    case GERptAttr.PWorkID:
                                    case GERptAttr.PNodeID:
                                    case GERptAttr.BillNo:
                                    case GERptAttr.FlowDaySpan:
                                    case "RefPK":
                                    case WorkAttr.RecText:
                                        continue;
                                    default:
                                        break;
                                }

                                //如果不包含数据库.
                                if (attrs.Contains(dc.ColumnName) == false)
                                    continue;

                                //开始赋值.
                                if (string.IsNullOrEmpty(en.GetValStringByKey(dc.ColumnName)) || en.GetValStringByKey(dc.ColumnName) == "0" || en.GetValStringByKey(dc.ColumnName).Contains("0.0"))
                                {
                                    en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                    continue;
                                }

                                //获取attr
                                Entity entity = mattrs.GetEntityByKey("KeyOfEn", dc.ColumnName);
                                if (entity != null)
                                {
                                    MapAttr attr = (MapAttr)entity;
                                    if (attr.LGType == FieldTypeS.Enum && en.GetValStringByKey(dc.ColumnName).Equals("-1"))
                                    {
                                        en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                        continue;
                                    }
                                    continue;
                                }

                            }
                        }
                    }
                }
            }
            //如果是webapi方式填充
            else if (doWay.Equals("3"))
            {
                //请求地址
                string apiUrl = sql;
                //设置请求头
                Hashtable headerMap = new Hashtable();

                //设置返回值格式
                headerMap.Add("Content-Type", "application/json");
                //设置token，用于接口校验
                headerMap.Add("Authorization", WebUser.Token);

                try
                {
                    //post方式请求数据
                    string postData = BP.Tools.PubGlo.HttpPostConnect(apiUrl, headerMap, "");
                    //数据序列化
                    JObject jsonData = postData.ToJObject();
                    //code=200，表示请求成功，否则失败
                    if (!jsonData["code"].ToString().Equals("200"))
                        return en;

                    //获取返回的数据
                    JObject data = jsonData["data"].ToString().ToJObject();
                    //获取主表数据
                    string mainTable = data["mainTable"].ToString();
                    dt = Json.ToDataTable(mainTable);

                    //获取全部附件数据
                    JObject athsJSON = jsonData["aths"].ToString().ToJObject();
                    for (int i = 0; i < athsJSON.Count; i++)
                    {
                        //获取附件
                        JToken athDatas = athsJSON[i];
                        //获取附件组件ID
                        string FK_FrmAttachment = athDatas["attachmentid"].ToString();
                        //获取当前组件中的附件数据
                        JObject athArryData = athDatas["attachmentdbs"].ToString().ToJObject();
                        //填充附件数据
                        for (int k = 0; k < athArryData.Count; k++)
                        {
                            JToken athData = athArryData[k];
                            //生成mypk主键值
                            string guid = DBAccess.GenerGUID();
                            FrmAttachment attachment = new FrmAttachment(FK_FrmAttachment);

                            //是否要先删除掉原有附件？根据实际需求，再做调整
                            //FrmAttachmentDBs attachmentDBs = new FrmAttachmentDBs();
                            //attachmentDBs.Retrieve(FrmAttachmentDBAttr.RefPKVal, workID, FrmAttachmentDBAttr.FK_MapData, attachment.FrmID);
                            //attachmentDBs.Delete();

                            //插入数据
                            FrmAttachmentDB attachmentDB = new FrmAttachmentDB();
                            attachmentDB.setMyPK(guid);
                            attachmentDB.FK_FrmAttachment = FK_FrmAttachment;
                            attachmentDB.FrmID = attachment.FrmID;
                            attachmentDB.RefPKVal = workID.ToString();
                            attachmentDB.FID = 0;//先默认为0
                            attachmentDB.Rec = athData["rec"].ToString();//执行人
                            attachmentDB.FileFullName = athData["fileFullName"].ToString();//附件全路径
                            attachmentDB.FileName = athData["fileName"].ToString();//附件名称
                            attachmentDB.FileExts = athData["fileExts"].ToString();//文件类型
                            attachmentDB.Sort = athData["sort"].ToString();//附件类型
                            attachmentDB.DeptNo = athData["fk_dept"].ToString();//上传人所在部门
                            attachmentDB.DeptName = athData["fk_deptName"].ToString();//上传人所在部门名称
                            attachmentDB.RecName = athData["recName"].ToString();//上传人名称
                            attachmentDB.RDT = athData["rdt"].ToString();//上传时间
                            attachmentDB.UploadGUID = guid;
                            attachment.Insert();
                        }
                    }

                    //获取从表数据
                    JObject dtlJSON = jsonData["dtls"].ToString().ToJObject();
                    for (int i = 0; i < dtlJSON.Count; i++)
                    {
                        JToken dtlDatas = dtlJSON[i];
                        //获取从表编号
                        string dtlNo = dtlDatas["dtlNo"].ToString();
                        //定义map
                        MapDtl dtl = new MapDtl(dtlNo);
                        //插入之前判断
                        GEDtls gedtls = null;
                        try
                        {
                            gedtls = new GEDtls(dtl.No);
                            if (dtl.DtlOpenType == DtlOpenType.ForFID)
                            {
                                if (gedtls.RetrieveByAttr(GEDtlAttr.RefPK, workID) > 0)
                                    continue;
                            }
                            else
                            {
                                //如果存在数据，默认先删除
                                if (gedtls.RetrieveByAttr(GEDtlAttr.RefPK, en.PKVal) > 0)
                                    gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);
                            }
                        }
                        catch (Exception ex)
                        {
                            (gedtls.GetNewEntity as GEDtl).CheckPhysicsTable();
                        }
                        //获取从表数据
                        JObject dtlArryData = dtlDatas["dtl"].ToString().ToJObject();
                        for (int k = 0; k < dtlArryData.Count; k++)
                        {
                            //获取一条数据
                            JToken dtlData = dtlArryData[k];
                            //从表数据
                            string dtlDataStr = dtlData["dtlData"].ToString();
                            //从表附件数据
                            JObject dtlAthData = dtlData["dtlAths"].ToString().ToJObject();
                            //从表数据字符串，转换成datatable
                            DataTable dtlDt = Json.ToDataTable(dtlDataStr);
                            //执行数据插入
                            foreach (DataRow dr in dtlDt.Rows)
                            {
                                GEDtl gedtl = gedtls.GetNewEntity as GEDtl;
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                }

                                switch (dtl.DtlOpenType)
                                {
                                    case DtlOpenType.ForEmp:  // 按人员来控制.
                                        gedtl.RefPK = en.PKVal.ToString();
                                        gedtl.FID = long.Parse(en.PKVal.ToString());
                                        break;
                                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                                        gedtl.RefPK = en.PKVal.ToString();
                                        gedtl.FID = long.Parse(en.PKVal.ToString());
                                        break;
                                    case DtlOpenType.ForFID: // 按流程ID来控制.
                                        gedtl.RefPK = workID.ToString();
                                        gedtl.FID = long.Parse(en.PKVal.ToString());
                                        break;
                                }
                                gedtl.RDT = DataType.CurrentDateTime;
                                gedtl.Rec = WebUser.No;
                                gedtl.Insert();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("接口请求失败,message:" + ex.Message.ToString());
                }
            }
            if (string.IsNullOrEmpty(item.Tag1)
                || item.Tag1.Length < 15)
                return en;

            // 填充从表.
            foreach (MapDtl dtl in dtls)
            {
                //如果有数据，就不要填充了.

                string[] sqls = item.Tag1.Split('$');
                foreach (string mysql in sqls)
                {
                    if (string.IsNullOrEmpty(mysql))
                        continue;
                    if (mysql.Contains(dtl.No + ":") == false)
                        continue;
                    if (mysql.Equals(dtl.No + ":") == true)
                        continue;

                    #region 处理sql.
                    sql = Glo.DealSQLExp(mysql.Replace(dtl.No + ":", "").ToString(), en, null);
                    #endregion 处理sql.

                    if (string.IsNullOrEmpty(sql))
                        continue;

                    int num = Regex.Matches(sql.ToUpper(), "WHERE").Count;
                    if (num == 1)
                    {
                        string sqlext = sql.Substring(0, sql.ToUpper().IndexOf("WHERE"));
                        sqlext = sql.Substring(sqlext.Length + 1);
                        if (sqlext.Contains("@"))
                            throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    }
                    if (num > 1 && sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);

                    if (isSelf == true)
                    {
                        MapDtl mdtlSln = new MapDtl();
                        mdtlSln.No = dtl.No + "_" + nodeID;
                        int result = mdtlSln.RetrieveFromDBSources();
                        if (result != 0)
                        {
                            dtl.DtlOpenType = mdtlSln.DtlOpenType;
                        }
                    }



                    GEDtls gedtls = null;

                    try
                    {
                        gedtls = new GEDtls(dtl.No);
                        if (dtl.DtlOpenType == DtlOpenType.ForFID)
                        {
                            if (gedtls.RetrieveByAttr(GEDtlAttr.RefPK, workID) > 0)
                                continue;
                        }
                        else
                        {
                            if (gedtls.RetrieveByAttr(GEDtlAttr.RefPK, en.PKVal) > 0)
                                continue;
                        }


                        //gedtls.Delete(GEDtlAttr.RefPK, en.PKVal);
                    }
                    catch (Exception ex)
                    {
                        (gedtls.GetNewEntity as GEDtl).CheckPhysicsTable();
                    }

                    sql = sql.StartsWith(dtl.No + "=") ? sql.Substring((dtl.No + "=").Length) : sql;
                    if (sfdb != null)
                        dt = sfdb.RunSQLReturnTable(sql);
                    else
                        dt = DBAccess.RunSQLReturnTable(sql);

                    foreach (DataRow dr in dt.Rows)
                    {
                        GEDtl gedtl = gedtls.GetNewEntity as GEDtl;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            gedtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }

                        switch (dtl.DtlOpenType)
                        {
                            case DtlOpenType.ForEmp:  // 按人员来控制.
                                gedtl.RefPK = en.PKVal.ToString();
                                gedtl.FID = long.Parse(en.PKVal.ToString());
                                break;
                            case DtlOpenType.ForWorkID: // 按工作ID来控制
                                gedtl.RefPK = en.PKVal.ToString();
                                gedtl.FID = long.Parse(en.PKVal.ToString());
                                break;
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                gedtl.RefPK = workID.ToString();
                                gedtl.FID = long.Parse(en.PKVal.ToString());
                                break;
                        }
                        gedtl.RDT = DataType.CurrentDateTime;
                        gedtl.Rec = WebUser.No;
                        gedtl.Insert();
                    }
                }
            }
            return en;
        }

        /// <summary>
        /// 执行PageLoad装载数据
        /// </summary>
        /// <param name="item"></param>
        /// <param name="en"></param>
        /// <param name="mattrs"></param>
        /// <returns></returns>
        public static Entity DealPageLoadFullVue(Entity en, MapExts mapExts, MapAttrs mattrs)
        {
            DataTable dt = null;
            string sql = "";
            string paras = "";
            foreach (string key in en.Row.Keys)
            {
                paras += "@" + key + "=" + en.GetValByKey(key);
            }
            foreach (MapExt mapExt in mapExts)
            {
                //填充主表
                if (mapExt.ExtModel.Equals(MapExtXmlList.PageLoadFullMainTable))
                {

                    string data = mapExt.GetFullData(paras, en.GetValStringByKey("OID"));
                    if (DataType.IsNullOrEmpty(data) == true)
                        throw new Exception("主表填充失败:没有获取到数据");
                    //data转换成JSON
                    dt = BP.Tools.Json.ToDataTable(data);
                    if (dt.Rows.Count == 1)
                    {
                        DataRow dr = dt.Rows[0];
                        foreach (DataColumn dc in dt.Columns)
                        {
                            //去掉一些不需要copy的字段.
                            switch (dc.ColumnName)
                            {
                                case WorkAttr.OID:
                                case WorkAttr.FID:
                                case WorkAttr.Rec:
                                case WorkAttr.MD5:
                                case GERptAttr.FlowEnder:
                                case GERptAttr.FlowEnderRDT:
                                case GERptAttr.AtPara:
                                case GERptAttr.PFlowNo:
                                case GERptAttr.PWorkID:
                                case GERptAttr.PNodeID:
                                case GERptAttr.BillNo:
                                case GERptAttr.FlowDaySpan:
                                case "RefPK":
                                case WorkAttr.RecText:
                                    continue;
                                default:
                                    break;
                            }

                            //如果不包含数据库.
                            bool isHave = false;
                            foreach (MapAttr attr in mattrs)
                            {
                                if (attr.KeyOfEn.Equals(dc.ColumnName))
                                {
                                    isHave = true;
                                    break;
                                }
                            }
                            if (isHave == false)
                                continue;

                            //开始赋值.
                            if (string.IsNullOrEmpty(en.GetValStringByKey(dc.ColumnName)) || en.GetValStringByKey(dc.ColumnName) == "0" || en.GetValStringByKey(dc.ColumnName).Contains("0.0"))
                            {
                                en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                continue;
                            }

                            //获取attr
                            Entity entity = mattrs.GetEntityByKey("KeyOfEn", dc.ColumnName);
                            if (entity != null)
                            {
                                MapAttr attr = (MapAttr)entity;
                                if (attr.LGType == FieldTypeS.Enum && en.GetValStringByKey(dc.ColumnName).Equals("-1"))
                                {
                                    en.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                    continue;
                                }
                                continue;
                            }

                        }
                    }

                }
                //填充从表
                if (mapExt.ExtModel.Equals(MapExtXmlList.PageLoadFullDtl))
                {
                    mapExt.GetFullDataDtl(paras, en.GetValStringByKey("OID"));
                }
                //填充下拉框
                if (mapExt.ExtModel.Equals(MapExtXmlList.PageLoadFullDDL))
                {

                }
            }

            return en;
        }
        /// <summary>
        /// 获取上传附件集合信息
        /// </summary>
        /// <param name="athDesc"></param>
        /// <param name="pkval"></param>
        /// <param name="FK_FrmAttachment"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="pworkid"></param>
        /// <param name="isContantSelf"></param>
        /// <param name="fk_node"></param>
        /// <param name="fk_mapData"></param>
        /// <returns></returns>
        public static BP.Sys.FrmAttachmentDBs GenerFrmAttachmentDBs(FrmAttachment athDesc, string pkval, string FK_FrmAttachment,
           Int64 workid = 0, Int64 fid = 0, Int64 pworkid = 0, bool isContantSelf = true, int fk_node = 0, string fk_mapData = null)
        {
            if (pkval == null)
                pkval = "0"; //解决预览的时候的错误.

            BP.Sys.FrmAttachmentDBs dbs = new BP.Sys.FrmAttachmentDBs();
            //查询使用的workId
            string ctrlWayId = "";
            if (FK_FrmAttachment.Contains("AthMDtl") == true || athDesc.GetParaBoolen("IsDtlAth") == true)
                ctrlWayId = pkval;
            else
            {
                MapData mapData = new MapData(athDesc.FrmID);
                if (mapData.EntityType == EntityType.FrmDict || mapData.EntityType == EntityType.FrmBill)
                    ctrlWayId = pkval;
                else
                    ctrlWayId = BP.WF.Dev2Interface.GetAthRefPKVal(workid, pworkid, fid, fk_node, fk_mapData, athDesc);
            }


            //如果是空的，就返回空数据结构. @lizhen.
            if (ctrlWayId.Equals("0") == true)
                return dbs;

            BP.En.QueryObject qo = new BP.En.QueryObject(dbs);
            //从表附件
            if (FK_FrmAttachment.Contains("AthMDtl") || athDesc.GetParaBoolen("IsDtlAth") == true)
            {
                /*如果是一个明细表的多附件，就直接按照传递过来的PK来查询.*/
                qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pkval);
                qo.addAnd();
                qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);
                qo.DoQuery();
                return dbs;
            }
            if (athDesc.HisCtrlWay == AthCtrlWay.MySelfOnly || athDesc.HisCtrlWay == AthCtrlWay.PK)
            {
                qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, pkval);
                qo.addAnd();
                qo.AddWhere(FrmAttachmentDBAttr.FK_FrmAttachment, FK_FrmAttachment);
                if (isContantSelf == false)
                {
                    qo.addAnd();
                    qo.AddWhere(FrmAttachmentDBAttr.Rec, "!=", WebUser.No);
                }
                qo.addOrderBy("Idx,RDT");
                qo.DoQuery();
                return dbs;
            }

            /* 继承模式 */
            if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                qo.AddWhere(FrmAttachmentDBAttr.RefPKVal, ctrlWayId);
            else
                qo.AddWhereIn(FrmAttachmentDBAttr.RefPKVal, "('" + ctrlWayId + "','" + pkval + "')");

            qo.addAnd();
            qo.AddWhere(FrmAttachmentDBAttr.NoOfObj, athDesc.NoOfObj);

            if (isContantSelf == false)
            {
                qo.addAnd();
                qo.AddWhere(FrmAttachmentDBAttr.Rec, "!=", WebUser.No);
            }
            qo.addOrderBy("Idx,RDT");
            qo.DoQuery();
            return dbs;
        }
        /// <summary>
        /// 获取从表数据，用于显示dtl.htm 
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="pkval">主键</param>
        /// <param name="atParas">参数</param>
        /// <param name="specDtlFrmID">指定明细表的参数，如果为空就标识主表数据，否则就是从表数据.</param>
        /// <returns>数据</returns>
        public static DataSet GenerDBForCCFormDtl(string frmID, MapDtl dtl, int pkval, string atParas, string dtlRefPKVal, Int64 fid)
        {
            //数据容器,就是要返回的对象.
            DataSet myds = new DataSet();

            //实体.
            GEEntity en = new GEEntity(frmID);
            en.OID = pkval;
            if (en.RetrieveFromDBSources() == 0)
                en.Insert();

            //把参数放入到 En 的 Row 里面。
            if (DataType.IsNullOrEmpty(atParas) == false)
            {
                AtPara ap = new AtPara(atParas);
                foreach (string key in ap.HisHT.Keys)
                {
                    try
                    {
                        if (en.Row.ContainsKey(key) == true) //有就该变.
                            en.Row[key] = ap.GetValStrByKey(key);
                        else
                            en.Row.Add(key, ap.GetValStrByKey(key)); //增加他.
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(key);
                    }
                }
            }
            if (BP.Difference.SystemConfig.isBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in HttpContextHelper.RequestParamKeys)
                {
                    en.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }
            }


            #region 加载从表表单模版信息.

            DataTable Sys_MapDtl = dtl.ToDataTableField("Sys_MapDtl");
            myds.Tables.Add(Sys_MapDtl);

            //明细表的表单描述
            MapAttrs attrs = dtl.MapAttrs;
            DataTable Sys_MapAttr = attrs.ToDataTableField("Sys_MapAttr");
            myds.Tables.Add(Sys_MapAttr);

            //明细表的配置信息.
            MapExts mes = dtl.MapExts;
            DataTable Sys_MapExt = mes.ToDataTableField("Sys_MapExt");
            myds.Tables.Add(Sys_MapExt);

            //启用附件，增加附件信息
            DataTable Sys_FrmAttachment = dtl.FrmAttachments.ToDataTableField("Sys_FrmAttachment");
            myds.Tables.Add(Sys_FrmAttachment);
            #endregion 加载从表表单模版信息.

            #region 把从表的- 外键表/枚举 加入 DataSet.

            MapExt me = null;
            DataTable ddlTable = new DataTable();
            ddlTable.Columns.Add("No");
            foreach (MapAttr attr in attrs)
            {

                //没有绑定外键
                string uiBindKey = attr.UIBindKey;
                if (DataType.IsNullOrEmpty(uiBindKey) == true)
                    continue;

                #region 枚举字段
                if (attr.LGType == FieldTypeS.Enum)
                {
                    // 如果是枚举值, 判断是否存在.
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;
                    string mysql = "SELECT IntKey AS No, Lab as Name FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
                    DataTable dtEnum = DBAccess.RunSQLReturnTable(mysql);
                    dtEnum.TableName = uiBindKey;

                    dtEnum.Columns[0].ColumnName = "No";
                    dtEnum.Columns[1].ColumnName = "Name";

                    myds.Tables.Add(dtEnum);
                    continue;
                }
                #endregion

                // 检查是否有下拉框自动填充。
                string keyOfEn = attr.KeyOfEn;

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL,
                    MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null && myds.Tables.Contains(uiBindKey) == false) //是否存在.
                {
                    string fullSQL = me.Doc.Clone() as string;
                    fullSQL = fullSQL.Replace("~", "'");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, en, null);

                    if (DataType.IsNullOrEmpty(fullSQL) == true)
                        throw new Exception("err@没有给AutoFullDLL配置SQL：MapExt：=" + me.MyPK + ",原始的配置SQL为:" + me.Doc);

                    DataTable dt = DBAccess.RunSQLReturnTable(fullSQL);

                    if (uiBindKey.ToLower().Equals("blank"))
                        dt.TableName = keyOfEn;
                    else
                        dt.TableName = uiBindKey;

                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                    {
                        if (dt.Columns.Contains("NO") == true)
                            dt.Columns["NO"].ColumnName = "No";
                        if (dt.Columns.Contains("NAME") == true)
                            dt.Columns["NAME"].ColumnName = "Name";
                        if (dt.Columns.Contains("PARENTNO") == true)
                            dt.Columns["PARENTNO"].ColumnName = "ParentNo";
                    }

                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                    {
                        if (dt.Columns.Contains("no") == true)
                            dt.Columns["no"].ColumnName = "No";
                        if (dt.Columns.Contains("name") == true)
                            dt.Columns["name"].ColumnName = "Name";
                        if (dt.Columns.Contains("parentno") == true)
                            dt.Columns["parentno"].ColumnName = "ParentNo";
                    }

                    myds.Tables.Add(dt);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                #region 外键字段

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                    continue;

                // 获得数据.
                DataTable mydt = BP.Pub.PubClass.GetDataTableByUIBineKey(uiBindKey, en.Row);
                if (uiBindKey.ToLower().Equals("blank"))
                    mydt.TableName = keyOfEn;
                if (mydt == null)
                {
                    DataRow ddldr = ddlTable.NewRow();
                    ddldr["No"] = uiBindKey;
                    ddlTable.Rows.Add(ddldr);
                }
                else
                {
                    myds.Tables.Add(mydt);
                }
                #endregion 外键字段
            }
            ddlTable.TableName = "UIBindKey";
            myds.Tables.Add(ddlTable);
            #endregion 把从表的- 外键表/枚举 加入 DataSet.

            #region 把主表数据放入.

            //重设默认值.
            en.ResetDefaultVal();


            //增加主表数据.
            DataTable mainTable = en.ToDataTableField(frmID);
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);
            #endregion 把主表数据放入.

            #region  把从表的数据放入.
            DataTable dtDtl = GetDtlInfo(dtl, en, dtlRefPKVal);
            //从表集合为空时填充从表的情况
            if (dtDtl.Rows.Count == 0)
            {
                GEDtl endtl = null;
                //1.行初始化字段，设置了改字段值时默认就添加枚举值集合的行数据，一般不再新增从表数据
                if (DataType.IsNullOrEmpty(dtl.InitDBAttrs) == false)
                {
                    string[] keys = dtl.InitDBAttrs.Split(',');

                    MapAttr attr = null;
                    foreach (string keyOfEn in keys)
                    {
                        Entity ent = dtl.MapAttrs.GetEntityByKey(dtl.No + "_" + keyOfEn);
                        if (ent == null)
                            continue;
                        attr = ent as MapAttr;
                        if (DataType.IsNullOrEmpty(attr.UIBindKey) == true)
                            continue;
                        DataTable dt = null;
                        //枚举字段
                        if (attr.LGType == FieldTypeS.Enum && attr.MyDataType == DataType.AppInt)
                            dt = myds.Tables[attr.UIBindKey];
                        //外键、外部数据源
                        if ((attr.LGType == FieldTypeS.FK && attr.MyDataType == DataType.AppString)
                            || (attr.LGType == FieldTypeS.Normal && attr.MyDataType == DataType.AppString && attr.UIContralType == UIContralType.DDL))
                            dt = myds.Tables[attr.UIBindKey];
                        if (dt == null)
                            continue;
                        foreach (DataRow dr in dt.Rows)
                        {
                            endtl = new GEDtl(dtl.No);
                            endtl.ResetDefaultVal();
                            endtl.SetValByKey(keyOfEn, dr[0]);
                            endtl.RefPK = dtlRefPKVal;
                            endtl.FID = fid;
                            endtl.Insert();
                        }

                    }
                }
                //2.从表装载填充
                me = mes.GetEntityByKey("ExtModel", MapExtXmlList.PageLoadFullDtl) as MapExt;
                if (me != null && me.DoWay.Equals("1") && DataType.IsNullOrEmpty(me.Doc) == false)
                {
                    string sql = Glo.DealSQLExp(me.Doc, en, null);
                    int num = Regex.Matches(sql.ToUpper(), "WHERE").Count;
                    if (num == 1)
                    {
                        string sqlext = sql.Substring(0, sql.ToUpper().IndexOf("WHERE"));
                        sqlext = sql.Substring(sqlext.Length + 1);
                        if (sqlext.Contains("@"))
                            throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    }
                    if (num > 1 && sql.Contains("@"))
                        throw new Exception("设置的sql有错误可能有没有替换的变量:" + sql);
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        endtl = new GEDtl(dtl.No);
                        foreach (DataColumn dc in dt.Columns)
                        {
                            endtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                        }
                        endtl.RefPK = dtlRefPKVal;
                        endtl.FID = fid;

                        endtl.RDT = DataType.CurrentDateTime;
                        endtl.Rec = WebUser.No;
                        endtl.Insert();
                    }
                }

                dtDtl = GetDtlInfo(dtl, en, dtlRefPKVal);
            }



            // 为明细表设置默认值.
            MapAttrs mattrs = new MapAttrs(dtl.No);
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIContralType == UIContralType.TB)
                    continue;

                //处理它的默认值.
                if (attr.DefValReal.Contains("@") == false)
                    continue;

                foreach (DataRow dr in dtDtl.Rows)
                {
                    if (dr[attr.KeyOfEn] == null || DataType.IsNullOrEmpty(dr[attr.KeyOfEn].ToString()) == true)
                        dr[attr.KeyOfEn] = attr.DefVal;
                }
            }

            dtDtl.TableName = "DBDtl"; //修改明细表的名称.
            myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
            #endregion 把从表的数据放入.

            //放入一个空白的实体，用与获取默认值.
            GEDtl dtlBlank = new GEDtl(dtl.No);
            dtlBlank.ResetDefaultVal();

            myds.Tables.Add(dtlBlank.ToDataTableField("Blank"));

            return myds;
        }
        public static DataTable GetDtlInfo(MapDtl dtl, GEEntity en, string dtlRefPKVal, bool isReload = false)
        {
            QueryObject qo = null;
            GEDtls dtls = new GEDtls(dtl.No);
            try
            {
                qo = new QueryObject(dtls);
                switch (dtl.DtlOpenType)
                {
                    case DtlOpenType.ForEmp:  // 按人员来控制.
                        qo.AddWhere(GEDtlAttr.RefPK, dtlRefPKVal);
                        qo.addAnd();
                        qo.AddWhere(GEDtlAttr.Rec, WebUser.No);
                        break;
                    case DtlOpenType.ForWorkID: // 按工作ID来控制
                    case DtlOpenType.ForPWorkID:
                        qo.AddWhere(GEDtlAttr.RefPK, dtlRefPKVal);

                        break;
                    case DtlOpenType.ForFID: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.FID, dtlRefPKVal);
                        break;
                    case DtlOpenType.ForWorkIDAndSpecEmpNo: // 按工作ID来控制
                        qo.AddWhere(GEDtlAttr.RefPK, dtlRefPKVal);
                        qo.addAnd();
                        string attr = dtl.GetParaString("DtlOpenPara");
                        if (DataType.IsNullOrEmpty(attr) == true)
                            throw new Exception("err@当前数据显示规则按照ForWorkIDAndSpecEmpNo计算，但是您没有设置人员账号字段名称,");
                        qo.AddWhere(attr, WebUser.No);
                        break;
                    default:
                        qo.AddWhere(GEDtlAttr.RefPK, dtlRefPKVal);
                        break;
                }

                //条件过滤.
                string exp = dtl.FilterSQLExp;
                if (DataType.IsNullOrEmpty(exp) == false)
                {
                    exp = Glo.DealExp(exp, en);
                    exp = exp.Replace("''", "'");

                    if (exp.Substring(0, 5).ToLower().Contains("and") == false)
                        exp = " AND " + exp;
                    qo.SQL = exp;
                }

                //排序.
                if (DataType.IsNullOrEmpty(dtl.OrderBySQLExp) == false)
                {
                    qo.addOrderBy(dtl.OrderBySQLExp);
                }
                else
                {
                    //增加排序.
                    qo.addOrderBy("Idx");
                }

                qo.DoQuery();
                //放入一个空白的实体，用与获取默认值.
                GEDtl dtlBlank = new GEDtl(dtl.No);
                dtlBlank.ResetDefaultVal();
                if (dtls.Count == 0 && dtl.RowsOfList != 0 && dtl.ItIsInsert == true)
                {
                    for (int i = 0; i < dtl.RowsOfList; i++)
                    {
                        GEDtl geDtl = new GEDtl(dtl.No);
                        geDtl.Copy(dtlBlank);
                        dtls.AddEntity(geDtl);
                    }
                }
                return dtls.ToDataTableField();
            }
            catch (Exception ex)
            {
                dtl.IntMapAttrs();
                dtls.GetNewEntity.CheckPhysicsTable();
                CacheFrmTemplate.Remove(dtl.No);
                Cache.SetMap(dtl.No, null);
                Cache.SQL_Cache.Remove(dtl.No);
                if (isReload == false)
                    return GetDtlInfo(dtl, en, dtlRefPKVal, true);
                else
                    throw new Exception("获取从表[" + dtl.Name + "]失败,错误:" + ex.Message);
            }

        }
    }


}
