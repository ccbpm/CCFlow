using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.WF.Template;
using BP.En;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 表单引擎api
    /// </summary>
    public class CCFormAPI:Dev2Interface
    {
        /// <summary>
        /// 生成报表
        /// </summary>
        /// <param name="templeteFilePath">模版路径</param>
        /// <param name="ds">数据源</param>
        /// <returns>生成单据的路径</returns>
        public static void Frm_GenerBill(string templeteFullFile, string saveToDir, string saveFileName,
            BillFileType fileType, DataSet ds, string fk_mapData)
        {

            MapData md = new MapData(fk_mapData);
            GEEntity entity = md.GenerGEEntityByDataSet(ds);

            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();
            rtf.HisEns.Clear();
            rtf.EnsDataDtls.Clear();

            rtf.HisEns.AddEntity(entity);
            var dtls = entity.Dtls;

            foreach (var item in dtls)
                rtf.EnsDataDtls.Add(item);

            rtf.MakeDoc(templeteFullFile, saveToDir, saveFileName, null, false);
        }
        /// <summary>
        /// 仅仅获取数据
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="pkval">主键</param>
        /// <returns>数据</returns>
        public static DataSet GenerDBForVSTOExcelFrmModel(string frmID, int pkval)
        {
            //数据容器,就是要返回的对象.
            DataSet myds = new DataSet();

            //增加表单字段描述.
            string sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "' ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapAttr";
            myds.Tables.Add(dt);

            //映射实体.
            MapData md = new MapData(frmID);

            //实体.
            GEEntity wk = new GEEntity(frmID);
            wk.OID = pkval;
            if (wk.RetrieveFromDBSources() == 0)
                wk.Insert();

            #region 把主从表数据放入里面.
            if (BP.Sys.SystemConfig.IsBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in System.Web.HttpContext.Current.Request.QueryString.AllKeys)
                {
                    wk.SetValByKey(k, System.Web.HttpContext.Current.Request.QueryString[k]);
                }
            }

            // 执行表单事件..
            string msg = md.FrmEvents.DoEventNode(FrmEventList.FrmLoadBefore, wk);
            if (string.IsNullOrEmpty(msg) == false)
                throw new Exception("err@错误:" + msg);

            //重设默认值.
            wk.ResetDefaultVal();

            //执行装载填充.
            MapExt me = new MapExt();
            me.MyPK = frmID + "_" + MapExtXmlList.PageLoadFull;
            if (me.RetrieveFromDBSources() == 1)
            {
                //执行通用的装载方法.
                MapAttrs attrs = new MapAttrs(frmID);
                MapDtls dtls = new MapDtls(frmID);
                wk = BP.WF.Glo.DealPageLoadFull(wk, me, attrs, dtls) as GEEntity;
            }

            //增加主表数据.
            DataTable mainTable = wk.ToDataTableField(md.No);
            mainTable.TableName = "MainTable";
            myds.Tables.Add(mainTable);

            ////把附件的数据放入.
            //if (md.FrmAttachments.Count > 0)
            //{
            //    sql = "SELECT * FROM Sys_FrmAttachmentDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
            //    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //    dt.TableName = "Sys_FrmAttachmentDB";
            //    myds.Tables.Add(dt);
            //}
            //// 图片附件数据放入
            //if (md.FrmImgAths.Count > 0)
            //{
            //    sql = "SELECT * FROM Sys_FrmImgAthDB where RefPKVal=" + workID + " AND FK_MapData='ND" + fk_node + "'";
            //    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            //    dt.TableName = "Sys_FrmImgAthDB";
            //    myds.Tables.Add(dt);
            //}

            //把从表的数据放入.
            if (md.MapDtls.Count > 0)
            {
                foreach (MapDtl dtl in md.MapDtls)
                {
                    GEDtls dtls = new GEDtls(dtl.No);
                    QueryObject qo = null;
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
                            case DtlOpenType.ForFID: // 按流程ID来控制.
                                qo.AddWhere(GEDtlAttr.FID, pkval);
                                break;
                        }
                    }
                    catch
                    {
                        dtls.GetNewEntity.CheckPhysicsTable();
                    }
                    DataTable dtDtl = qo.DoQueryToTable();

                    // 为明细表设置默认值.
                    MapAttrs dtlAttrs = new MapAttrs(dtl.No);
                    foreach (MapAttr attr in dtlAttrs)
                    {
                        //处理它的默认值.
                        if (attr.DefValReal.Contains("@") == false)
                            continue;

                        foreach (DataRow dr in dtDtl.Rows)
                            dr[attr.KeyOfEn] = attr.DefVal;
                    }

                    dtDtl.TableName = dtl.No; //修改明细表的名称.
                    myds.Tables.Add(dtDtl); //加入这个明细表, 如果没有数据，xml体现为空.
                }
            }
            #endregion

            #region 把主表的- 外键表/枚举 加入 DataSet.
            DataTable dtMapAttr = myds.Tables["Sys_MapAttr"];
            MapExts mes = md.MapExts;
            foreach (DataRow dr in dtMapAttr.Rows)
            {
                string uiBindKey = dr["UIBindKey"].ToString();
                string lgType = dr["LGType"].ToString();
                if (lgType == "1")
                {
                    // 判断是否存在.
                    if (myds.Tables.Contains(uiBindKey) == true)
                        continue;

                    string mysql = "SELECT IntKey AS No, Lab as Name FROM Sys_Enum WHERE EnumKey='" + uiBindKey + "' ORDER BY IntKey ";
                    DataTable dtEnum = BP.DA.DBAccess.RunSQLReturnTable(mysql);
                    dtEnum.TableName = uiBindKey;
                    myds.Tables.Add(dtEnum);
                    continue;
                }

                if (lgType != "2" )
                    continue;

                string UIIsEnable = dr["UIIsEnable"].ToString();
                if (UIIsEnable == "0")
                    continue;

                if (string.IsNullOrEmpty(uiBindKey) == true)
                {
                    string myPK = dr["MyPK"].ToString();
                    /*如果是空的*/
                    //   throw new Exception("@属性字段数据不完整，流程:" + fl.No + fl.Name + ",节点:" + nd.NodeID + nd.Name + ",属性:" + myPK + ",的UIBindKey IsNull ");
                }

                // 检查是否有下拉框自动填充。
                string keyOfEn = dr["KeyOfEn"].ToString();
                string fk_mapData = dr["FK_MapData"].ToString();

                #region 处理下拉框数据范围. for 小杨.
                me = mes.GetEntityByKey(MapExtAttr.ExtType, MapExtXmlList.AutoFullDLL, MapExtAttr.AttrOfOper, keyOfEn) as MapExt;
                if (me != null)
                {
                    string fullSQL = me.Doc.Clone() as string;
                    fullSQL = fullSQL.Replace("~", ",");
                    fullSQL = BP.WF.Glo.DealExp(fullSQL, wk, null);
                    dt = DBAccess.RunSQLReturnTable(fullSQL);
                    dt.TableName = keyOfEn; //可能存在隐患，如果多个字段，绑定同一个表，就存在这样的问题.
                    myds.Tables.Add(dt);
                    continue;
                }
                #endregion 处理下拉框数据范围.

                // 判断是否存在.
                if (myds.Tables.Contains(uiBindKey) == true)
                    continue;

                myds.Tables.Add(BP.Sys.PubClass.GetDataTableByUIBineKey(uiBindKey));
            }
            #endregion End把外键表加入DataSet

            //返回生成的dataset.
            return myds;
        }
    }
}
