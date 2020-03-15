using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Data;
using System.Web.Services;
using BP.WF.Template;
using BP.Sys;
using BP.Web;
using BP.DA;
using BP.En;
using System.Drawing;

namespace CCFlow.WF.CCForm
{
    /// <summary>
    /// CCFormAPI 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class CCFormAPI : System.Web.Services.WebService
    {
        #region 与单据相关的接口代码.
        /// <summary>
        /// 获得单据模版信息
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="billTemplateNo">单据模版编号</param>
        /// <param name="ds">返回的数据源</param>
        /// <param name="bytes">返回的字节</param>
        [WebMethod]
        public void GenerBillTemplate(string userNo, string sid, Int64 workID, string billTemplateNo,
            ref DataSet ds, ref byte[] bytes)
        {
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = BP.Web.WebUser.No;

            BP.WF.Dev2Interface.Port_Login(userNo);
            BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workID);

            //是否可以查看该工作.
            bool b = BP.WF.Dev2Interface.Flow_IsCanViewTruck(gwf.FK_Flow, gwf.WorkID);
            if (b == false && 1 == 2)
                throw new Exception("err@[" + userNo + "]无权查看该流程,WorkID=" + workID);

            string frmID = "ND" + int.Parse(gwf.FK_Flow) + "Rpt";
            BP.WF.Data.GERpt rpt = new BP.WF.Data.GERpt("ND" + int.Parse(gwf.FK_Flow) + "Rpt", workID);
            DataTable dt = rpt.ToDataTableField();
            dt.TableName = "Main";
            ds.Tables.Add(dt);

            #region 处理bool类型.
            foreach (Attr item in rpt.EnMap.Attrs)
            {
                if (item.MyDataType == DataType.AppBoolean)
                {
                    dt.Columns.Add(item.Key + "Text", typeof(string));

                    foreach (DataRow dr in dt.Rows)
                    {
                        string val = dr[item.Key].ToString();
                        if (val == "1")
                            dr[item.Key + "Text"] = "√";
                        else
                            dr[item.Key + "Text"] = "  ";
                    }
                }
            }
            #endregion 处理bool类型.

            //把从表数据加入里面去.
            //MapDtls dtls = new MapDtls("ND" + gwf.FK_Node);
            //foreach (MapDtl item in dtls)
            //{
            //    GEDtls dtlEns = new GEDtls(item.No);
            //    dtlEns.Retrieve(GEDtlAttr.RefPK, workID);

            //    DataTable dtDtl = dtlEns.ToDataTableField(item.No);
            //    ds.Tables.Add(dtDtl);
            //}

            //把从表数据加入里面去.
            MapDtls dtls = new MapDtls("ND" + gwf.FK_Node);
            foreach (MapDtl item in dtls)
            {
                GEDtls dtlEns = new GEDtls(item.No);
                dtlEns.Retrieve(GEDtlAttr.RefPK, workID);
                DataTable Dtl = dtlEns.ToDataTableField(item.No);
                ds.Tables.Add(Dtl);
                foreach (GEDtl dtl in dtlEns)
                {
                    DataTable dtDtl = dtl.ToDataTableField(item.No);

                    #region 处理bool类型.
                    foreach (Attr dtlitem in dtl.EnMap.Attrs)
                    {
                        if (dtlitem.MyDataType == DataType.AppBoolean)
                        {
                            if (Dtl.Columns.Contains(dtlitem.Key + "Text"))
                            {
                                continue;
                            }
                            Dtl.Columns.Add(dtlitem.Key + "Text", typeof(string));

                            foreach (DataRow dr in Dtl.Rows)
                            {
                                string val = dr[dtlitem.Key].ToString();
                                if (val == "1")
                                    dr[dtlitem.Key + "Text"] = "√";
                                else
                                    dr[dtlitem.Key + "Text"] = "  ";
                            }
                        }
                    }
                    #endregion 处理bool类型.
                }

            }

            //生成模版的文件流.
            BP.WF.Template.BillTemplate template = new BP.WF.Template.BillTemplate(billTemplateNo);
            bytes = template.GenerTemplateFile();
            return;
        }

        [WebMethod]
        public string SaveBillDesingerTemplate_2019(string billNo, string frmID, byte[] bytes)
        {
            try
            {
                string filePath = SystemConfig.PathOfDataUser + "CyclostyleFile\\VSTO\\";
                string fileName = billNo + "_" + frmID + ".docx";
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string fileFullPath = filePath + fileName;

                if (File.Exists(fileFullPath))
                    File.Delete(fileFullPath);

                FileStream fs = new FileStream(fileFullPath, System.IO.FileMode.CreateNew);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();

                return "模版上传成功";
            }
            catch (Exception ex)
            {

                return "err@:" + ex.Message;
            }
        }

        /// <summary>
        /// 获取模版
        /// </summary>
        /// <param name="billNo"></param>
        /// <param name="frmID"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetBillDesingerTemplate_2019(string billNo, string frmID)
        {
            MethodReturnMessage<byte[]> msg = null;

            try
            {
                string filePath = SystemConfig.PathOfDataUser + "CyclostyleFile\\VSTO\\";
                string fileName = billNo + "_" + frmID + ".docx";
                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                string fileFullPath = filePath + fileName;

                if (File.Exists(fileFullPath))
                {
                    FileStream fs = new FileStream(fileFullPath, FileMode.Open);
                    var buffer = new byte[fs.Length];
                    fs.Position = 0;
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();

                    msg = new MethodReturnMessage<byte[]>
                    {
                        Success = true,
                        Message = "读取文件成功",
                        Data = buffer
                    };
                }
                else
                {
                    msg = new MethodReturnMessage<byte[]>
                    {
                        Success = false,
                        Message = "模版不存在",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                msg = new MethodReturnMessage<byte[]>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return LitJson.JsonMapper.ToJson(msg);
        }

        #endregion

        #region 与公文相关的接口.
        /// <summary>
        /// 获得文件签名
        /// </summary>
        /// <param name="userNo">用户号</param>
        /// <param name="bytes">签名</param>
        [WebMethod]
        public void WordFileGenerSiganture(string userNo, ref byte[] bytes)
        {
            string filePath = SystemConfig.PathOfDataUser + "Siganture\\" + userNo + ".jpg";
            if (System.IO.File.Exists(filePath) == false)
                filePath = SystemConfig.PathOfDataUser + "Siganture\\UnSiganture.jpg";

            //怎么把文件转化为字节， 把字节转化为文件，请参考。http://www.cnblogs.com/yy981420974/p/8193081.html
            FileStream stream = new FileInfo(filePath).OpenRead();
            bytes = new Byte[stream.Length];

            //从流中读取字节块并将该数据写入给定缓冲区buffer中
            stream.Read(bytes, 0, Convert.ToInt32(stream.Length));
        }
        /// <summary>
        /// 获得Word文件 - 未开发完成.
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="oid">表单主键</param>
        /// <returns></returns>
        [WebMethod]
        public void WordFileGener(string userNo, string sid, Int64 workID, ref byte[] bytes)
        {

            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = BP.Web.WebUser.No;

            //if (DataType.IsNullOrEmpty(userNo) == true)
            //    userNo = "admin";

            BP.WF.Dev2Interface.Port_Login(userNo);
            BP.WF.GenerWorkFlow gwf = new BP.WF.GenerWorkFlow(workID);

            bool b = BP.WF.Dev2Interface.Flow_IsCanViewTruck(gwf.FK_Flow, gwf.WorkID);
            if (b == false && 1 == 2)
                throw new Exception("err@[" + userNo + "]无权查看该流程,WorkID=" + workID);

            string frmID = "ND" + int.Parse(gwf.FK_Flow) + "Rpt";

            MapData md = new MapData(frmID);

            //创建excel表单描述，让其保存到excel表单指定的字段里, 扩展多个表单映射同一张表.
            MapFrmWord mfe = new MapFrmWord(frmID);

            //返回文件模版.
            md.WordGenerFile(workID.ToString(), ref bytes, mfe.DBSave);
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">sid</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="byt">文件流</param>
        [WebMethod]
        public void WordFileSave(string userNo, string sid, string flowNo, Int64 workid, byte[] byt)
        {
            //执行登录.
            if (BP.Web.WebUser.NoOfRel != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo);

            // 登录名丢失.
            try
            {
                string strName = WebUser.Name;
            }
            catch
            {
                BP.WF.Dev2Interface.Port_Login(userNo);
            }

            string frmID = "ND" + flowNo + "Rpt";

            //执行保存文件.
            MapData md = new MapData(frmID);

            //创建excel表单描述，让其保存到excel表单指定的字段里, 扩展多个表单映射同一张表.
            MapFrmExcel mfe = new MapFrmExcel(md.No);
            md.WordSaveFile(workid.ToString(), byt, mfe.DBSave); //把文件保存到该实体对应的数据表的 DBFile 列中。

        }
        #endregion 与公文相关的接口

        #region 与表单相关的接口.
        /// <summary>
        /// 获得Excel文件
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="oid">表单主键</param>
        /// <returns></returns>
        [WebMethod]
        public bool GenerExcelFile(string userNo, string sid, string frmID, string pkValue, ref byte[] bytes)
        {

            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = BP.Web.WebUser.No;

            if (DataType.IsNullOrEmpty(userNo))
                userNo = "admin";


            BP.WF.Dev2Interface.Port_Login(userNo);

            //如果是一个实体类.
            if (frmID.Contains("BP."))
            {
                // 执行map同步.
                Entities ens = BP.En.ClassFactory.GetEns(frmID + "s");
                Entity en = ens.GetNewEntity;

                //en.DTSMapToSys_MapData();
                //MapData md = new MapData(frmID);
                var md = en.DTSMapToSys_MapData();

                //创建excel表单描述，让其保存到excel表单指定的字段里, 扩展多个表单映射同一张表.
                MapFrmExcel mfe = new MapFrmExcel(md.No);

                return md.ExcelGenerFile(pkValue, ref bytes, mfe.DBSave);
            }
            else
            {
                MapData md = new MapData(frmID);

                //创建excel表单描述，让其保存到excel表单指定的字段里, 扩展多个表单映射同一张表.
                MapFrmExcel mfe = new MapFrmExcel(md.No);

                return md.ExcelGenerFile(pkValue, ref bytes, mfe.DBSave);
            }
        }
        /// <summary>
        /// 生成vsto模式的数据
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="sid"></param>
        /// <param name="frmID"></param>
        /// <param name="oid"></param>
        /// <param name="atParas">参数</param>
        /// <returns></returns>
        [WebMethod]
        public System.Data.DataSet GenerDBForVSTOExcelFrmModel(string userNo, string sid, string frmID, string pkValue, string atParas)
        {
            //让他登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //解析这个表单.
            return BP.WF.CCFormAPI.GenerDBForVSTOExcelFrmModel(frmID, pkValue, atParas);
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单编号</param>
        /// <param name="mainEnPKOID">主键（OID）</param>
        /// <param name="mainTableAtParas">主表数据（"@KeyOfEn=value@..."）</param>
        /// <param name="dsDtlsChange">从表数据（新）</param>
        /// <param name="dsDtlsOld">从表数据（原始）</param>
        /// <param name="byt">文件流</param>
        [WebMethod]
        public void SaveExcelFile(string userNo, string sid, string frmID, string pkValue, string mainTableAtParas, System.Data.DataSet dsDtlsChange, System.Data.DataSet dsDtlsOld, byte[] byt)
        {
            //执行登录.
            if (BP.Web.WebUser.NoOfRel != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo);

            // 登录名丢失.
            try
            {
                string strName = WebUser.Name;
            }
            catch
            {
                BP.WF.Dev2Interface.Port_Login(userNo);
            }

            MapData md = null;

            #region  保存excel文件流
            if (frmID.Contains("BP."))
            {
                Entities ens = BP.En.ClassFactory.GetEns(frmID + "s");
                Entity en = ens.GetNewEntity;
                md = en.DTSMapToSys_MapData();

                //创建excel表单描述，让其保存到excel表单指定的字段里, 扩展多个表单映射同一张表.
                MapFrmExcel mfe = new MapFrmExcel(md.No);
                md.ExcelSaveFile(pkValue, byt, mfe.DBSave);
            }
            else
            {
                //执行保存文件.
                md = new MapData(frmID);

                //创建excel表单描述，让其保存到excel表单指定的字段里, 扩展多个表单映射同一张表.
                MapFrmExcel mfe = new MapFrmExcel(md.No);
                md.ExcelSaveFile(pkValue, byt, mfe.DBSave); //把文件保存到该实体对应的数据表的 DBFile 列中。
            }
            #endregion  保存excel文件流

            #region 保存从表
            if (dsDtlsChange != null)
            {
                //截去『BP.XXX.』以便下方的“new MapDtls(frmID)”能正常取值
                string tempFrmID = frmID;
                if (frmID.Contains("BP."))
                    frmID = frmID.Substring(frmID.LastIndexOf(".") + 1);

                //明细集合.
                MapDtls dtls = new MapDtls(frmID);

                if (dtls.Count == 0)
                    dtls = new MapDtls(tempFrmID);

                //保存从表
                foreach (System.Data.DataTable dt in dsDtlsChange.Tables)
                {
                    foreach (MapDtl dtl in dtls)
                    {
                        //if (dt.TableName != dtl.No) //!++ TO DO: BP.XXX.YYYYYs != YYYYY
                        //	continue;
                        if (dt.TableName.Contains("BP."))
                        {
                            var tname = dt.TableName.Substring(dt.TableName.LastIndexOf(".") + 1,
                                dt.TableName.Length - dt.TableName.LastIndexOf(".") - 2);
                            if (tname != dtl.No)
                                continue;
                        }
                        else
                        {
                            if (dt.TableName != dtl.No && dt.TableName != dtl.Alias)    //使用明细表编号与别名验证，符合之一，即代表是此明细表的数据表，added by liuxc,2017-10-13
                                continue;
                        }
                        if (dtl.IsReadonly) //从表是否只读
                            continue;

                        #region 执行删除操作.
                        if (dtl.IsDelete == true) //从表是否可删除行
                        {
                            #region 根据原始数据,与当前数据求出已经删除的oids .
                            DataTable dtDtlOld = dsDtlsOld.Tables[dt.TableName]; //这里要用原始（打开excel时获取到的）表名『BP.XXX.YYYYY』
                            foreach (DataRow dr in dtDtlOld.Rows)
                            {
                                string oidOld = dr["OID"].ToString();

                                bool isHave = false;
                                //遍历变更的数据.
                                foreach (DataRow dtNew in dt.Rows)
                                {
                                    string oidNew = dtNew["OID"].ToString();
                                    if (oidOld == oidNew)
                                    {
                                        isHave = true;
                                        break;
                                    }
                                }

                                //如果不存在.
                                if (isHave == false)
                                    DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE OID=" + oidOld);
                            }
                            #endregion 根据原始数据,与当前数据求出已经删除的oids .
                        }
                        #endregion 执行删除操作.


                        #region 执行更新操作.
                        if (dtl.IsUpdate == true) //从表【是否可更新行】
                        {
                            //获取dtls
                            GEDtls daDtls = new GEDtls(dtl.No);

                            //获得主表事件.
                            FrmEvents fes = new FrmEvents(dtl.No); //获得事件.
                            GEEntity mainEn = null;
                            if (fes.Count > 0)
                                mainEn = dtl.GenerGEMainEntity(pkValue);

                            //求出从表实体类.
                            BP.Sys.FormEventBaseDtl febd = null;
                            if (dtl.FEBD != "")
                            {
                                febd = BP.Sys.Glo.GetFormDtlEventBaseByEnName(dtl.No);
                                if (mainEn == null)
                                    mainEn = dtl.GenerGEMainEntity(pkValue);
                            }

                            // 更新数据.
                            foreach (DataRow dr in dt.Rows)
                            {
                                GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                                string oid = dr["OID"].ToString();
                                daDtl.OID = int.Parse(string.IsNullOrWhiteSpace(oid) ? "0" : oid);
                                if (daDtl.OID > 100)
                                    daDtl.RetrieveFromDBSources();

                                daDtl.ResetDefaultVal();

                                //明细列.
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    //设置属性.
                                    daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                                }

                                daDtl.SetValByKey(dtl.RefPK, pkValue);
                                daDtl.RDT = DataType.CurrentDataTime;

                                #region 从表保存前处理事件.
                                if (fes.Count > 0)
                                {
                                    string msg = fes.DoEventNode(FrmEventListDtl.RowSaveBefore, mainEn);
                                    if (msg != null)
                                        throw new Exception(msg);
                                }

                                if (febd != null)
                                {
                                    febd.HisEn = mainEn;
                                    febd.HisEnDtl = daDtl;
                                    febd.DoIt(FrmEventListDtl.RowSaveBefore, febd.HisEn, daDtl, null);
                                }

                                #endregion 从表保存前处理事件.

                                //执行保存.
                                if (daDtl.OID > 100)
                                {
                                    daDtl.RefPK = pkValue;
                                    daDtl.Update();
                                }
                                if (daDtl.OID <= 100 && dtl.IsInsert == true) //从表【是否可新增行】.
                                {
                                    if (daDtl.IsBlank == false)
                                    {
                                        daDtl.RefPK = pkValue;
                                        daDtl.InsertAsOID(DBAccess.GenerOID("Dtl"));
                                    }
                                }

                                #region 从表保存后处理事件。

                                if (fes.Count > 0)
                                {
                                    string msg = fes.DoEventNode(FrmEventListDtl.RowSaveAfter, daDtl);
                                    if (msg != null)
                                        throw new Exception(msg);
                                }

                                if (febd != null)
                                {
                                    febd.HisEn = mainEn;
                                    febd.HisEnDtl = daDtl;

                                    febd.DoIt(FrmEventListDtl.RowSaveAfter, mainEn, daDtl, null);
                                }

                                #endregion 处理事件.
                            }
                        }
                        #endregion 执行更新操作.
                    }
                }
            }
            #endregion 保存从表结束

            #region 求主表的主键类型.
            string pkType = null;
            string sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "' AND KeyOfEn='OID' ";
            if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 1)
                pkType = "OID";

            if (pkType == null)
            {
                sql = "SELECT KeyOfEn FROM Sys_MapAttr WHERE FK_MapData='" + frmID + "' AND KeyOfEn='MyPK' ";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 1)
                    pkType = "MyPK";
            }

            if (pkType == null)
                pkType = "No";

            #endregion 求主表的主键类型.

            //求出从表实体类.
            BP.Sys.FormEventBase frmEvent = null;
            if (md.FormEventEntity != "")
                frmEvent = BP.Sys.Glo.GetFormEventBaseByEnName(md.No);


            #region 处理EntityMyPK 类型的实体保存。
            if (pkType == "MyPK")
            {
                /* 具有MyPK 的实体，为了简便判断. */
                GEEntityMyPK wk = new GEEntityMyPK(frmID, pkValue);
                wk.ResetDefaultVal();

                if (mainTableAtParas != null)
                {
                    AtPara ap = new AtPara(mainTableAtParas);
                    foreach (string str in ap.HisHT.Keys)
                    {
                        if (wk.Row.ContainsKey(str))
                            wk.SetValByKey(str, ap.GetValStrByKey(str));
                        else
                            wk.Row.Add(str, ap.GetValStrByKey(str));
                    }
                }
                wk.MyPK = pkValue;

                //保存前执行事件.
                md.DoEvent(FrmEventList.SaveBefore, wk);

                // 保存实体.
                wk.Save();

                //保存前执行事件.
                md.DoEvent(FrmEventList.SaveAfter, wk);


                //保存前(执行类事件.)
                if (frmEvent != null)
                    frmEvent.DoIt(FrmEventList.SaveAfter, wk, null);
            }
            #endregion 处理 EntityMyPK 类型的实体保存。

            #region 处理 EntityOID 类型的实体保存。
            if (pkType == "OID")
            {
                GEEntity wk = new GEEntity(frmID, pkValue);
                wk.ResetDefaultVal();

                if (mainTableAtParas != null)
                {
                    AtPara ap = new AtPara(mainTableAtParas);
                    foreach (string str in ap.HisHT.Keys)
                    {
                        if (wk.Row.ContainsKey(str))
                            wk.SetValByKey(str, ap.GetValStrByKey(str));
                        else
                            wk.Row.Add(str, ap.GetValStrByKey(str));
                    }
                }
                wk.OID = Int64.Parse(pkValue);

                //执行事件.
                md.DoEvent(FrmEventList.SaveBefore, wk);

                //保存.
                wk.Save();

                //执行事件.
                md.DoEvent(FrmEventList.SaveAfter, wk);


            }
            #endregion 处理 EntityOID 类型的实体保存。

        }
        /// <summary>
        /// 保存一个文件
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="pkValue">主键</param>
        /// <param name="byt">文件流.</param>
        [WebMethod]
        public void SaveFrmAth(string userNo, string sid, string frmID, int nodeID, Int64 workID, byte[] byt, string guid)
        {
            BP.WF.Dev2Interface.Port_Login(userNo);
            MapData md = new MapData(frmID);
            FrmAttachments aths = new FrmAttachments(frmID);
            if (aths.Count == 0)
            {
                BP.Sys.CCFormAPI.CreateOrSaveAthMulti(md.No, "Ath", "附件", 100, 100);
                aths = new FrmAttachments(frmID);
            }
            FrmAttachment ath = aths[0] as FrmAttachment;

            //把文件写入.
            string rootPath = Context.Server.MapPath("~/" + ath.SaveTo);
            string fileName = guid + "." + System.Drawing.Imaging.ImageFormat.Jpeg.ToString();
            string filePath = rootPath + fileName;
            if (System.IO.File.Exists(filePath) == true)
                System.IO.File.Delete(filePath);
            BP.DA.DataType.WriteFile(filePath, byt);

            FileInfo info = new FileInfo(filePath);
            FrmAttachmentDB dbUpload = new FrmAttachmentDB();
            dbUpload.MyPK = guid;
            dbUpload.NodeID = nodeID;
            dbUpload.Sort = null;
            dbUpload.FK_FrmAttachment = ath.MyPK;
            dbUpload.FK_MapData = ath.FK_MapData;
            dbUpload.FileExts = info.Extension;
            dbUpload.FileFullName = filePath;
            dbUpload.FileName = fileName;
            dbUpload.FileSize = (float)info.Length;
            dbUpload.RDT = DataType.CurrentDataTimess;
            dbUpload.Rec = userNo;
            dbUpload.RecName = BP.Web.WebUser.Name;
            dbUpload.FK_Dept = WebUser.FK_Dept;
            dbUpload.FK_DeptName = WebUser.FK_DeptName;
            dbUpload.RefPKVal = workID.ToString();

            dbUpload.UploadGUID = guid;
            dbUpload.DirectSave();

        }
        /// <summary>
        /// 级联接口
        /// </summary>
        /// <param name="userNo">用户</param>
        /// <param name="sid">安全校验码</param>
        /// <param name="pkValue">表单主键值（WorkId）</param>
        /// <param name="mapExtMyPK">逻辑逐渐值</param>
        /// <param name="cheaneKey">级联父字段的值（No)</param>
        /// <param name="paras">『主表/子表整行』的【所有字段】（@Key=Val@Key1=Val1@Key2=Val2）</param>
        /// <returns>查询的要填充数据</returns>
        [WebMethod]
        public DataTable MapExtGenerAcitviDDLDataTable(string userNo, string sid, string pkValue, string mapExtMyPK, string cheaneKey,
            string paras)
        {
            BP.WF.Dev2Interface.Port_Login(userNo);

            MapExt me = new MapExt(mapExtMyPK);

            string sql = me.DocOfSQLDeal.Clone() as string;
            sql = sql.Replace("@Key", cheaneKey);
            sql = sql.Replace("@key", cheaneKey);
            sql = sql.Replace("@Val", cheaneKey);
            sql = sql.Replace("@val", cheaneKey);

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            sql = sql.Replace("@OID", pkValue.ToString());
            sql = sql.Replace("@WorkID", pkValue.ToString());

            if (sql.Contains("@") == true)
            {
                string[] strs = paras.Split('@');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s)
                        || s.Contains("=") == false)
                        continue;

                    string[] mykv = s.Split('=');
                    sql = sql.Replace("@" + mykv[0], mykv[1]);

                    if (sql.Contains("@") == false)
                        break;
                }
            }
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
        #endregion 与表单相关的接口.

        /// <summary>
        /// 获取VSTO插件版本号
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public String GetVstoExtensionVersion()
        {
            //return BP.Sys.SystemConfig.AppSettings["VstoExtensionVersion"];//2017-05-02 14:53:02：不再在web.config中配置VSTO版本号
            return "1.1.0.4";
        }
        /// <summary>
        /// 获取VSTO插件版本号
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public String GetVstoCCFormWordExtensionVersion()
        {
            //return BP.Sys.SystemConfig.AppSettings["VstoExtensionVersion"];//2017-05-02 14:53:02：不再在web.config中配置VSTO版本号
            return "1.1.0.4";
        }
        /// <summary>
        /// 获取VSTO-Doc的插件
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public String DocVersion()
        {
            //return BP.Sys.SystemConfig.AppSettings["VstoExtensionVersion"];//2017-05-02 14:53:02：不再在web.config中配置VSTO版本号
            return "1.0.0.0";
        }
        [WebMethod]
        public void RecordMsg(Int64 workID, string msg)
        {
            //VSTOMsg vMsg = new VSTOMsg();
            //vMsg.WorkID = workID;
            //vMsg.ErrorMsg = msg;
            //vMsg.RDT = DateTime.Now.ToString(BP.DA.DataType.SysDataTimeFormat);
            //vMsg.IsDelete = false;
            //vMsg.DirectSave();
        }
        public class ReportImage
        {
            public string ext;
            public string fileName;
            public byte[] bytesData;
            public string mypk;
        }

        [WebMethod]
        public string GetReportImagesData(long workID, string createReportType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(createReportType))
                    return null;

                string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                BP.DA.Paras ps = new BP.DA.Paras();


                switch (createReportType)
                {
                    case "1":
                        ps.SQL = "SELECT FileFullName,FileExts,MyPK,FileName  FROM Sys_FrmAttachmentDB  WHERE  RefPKVal=" + dbStr + "RefPKVal";
                        ps.Add(BP.Sys.FrmAttachmentDBAttr.RefPKVal, workID);
                        break;
                    case "2":
                        ps.SQL = "SELECT FileFullName,FileExts,MyPK,FileName  FROM Sys_FrmAttachmentDB  WHERE " +
                                 "RefPKVal in(SELECT WorkID FROM WF_GenerWorkFlow WHERE PWORKID=" + dbStr + "PWORKID)";
                        ps.Add("PWORKID", workID);
                        break;
                    default:
                        break;
                }
                DataTable dt = DBAccess.RunSQLReturnTable(ps);

                List<ReportImage> reImgsList = new List<ReportImage>();
                foreach (DataRow dr in dt.Rows)
                {
                    FileStream fs = new FileStream(dr["FileFullName"].ToString(), FileMode.Open);
                    long size = fs.Length;
                    byte[] bytes = new byte[size];
                    fs.Read(bytes, 0, bytes.Length);
                    fs.Close();

                    reImgsList.Add(new ReportImage
                    {
                        ext = dr["FileExts"].ToString(), //frmDB.FileExts,
                        fileName = dr["FileName"].ToString(), //frmDB.FileName,
                        bytesData = bytes,
                        mypk = dr["MyPK"].ToString() //frmDB.MyPK
                    });
                }

                return LitJson.JsonMapper.ToJson(reImgsList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region       公文主文件

        [WebMethod]
        public DataSet WordDoc_GetTempDocMainData(string userNo, string sid, string frmID, string pkValue, string atParas, string tempNo)
        {
            //让他登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //解析这个表单.
            DataSet ds = BP.WF.CCFormAPI.GenerDBForVSTOExcelFrmModel(frmID, pkValue, atParas);

            string sql = "SELECT * from wf_DocTemplate WHERE No='" + tempNo + "' ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "TempFields";
            ds.Tables.Add(dt);

            return ds;
        }
        [WebMethod]
        public string WordDoc_GetWordFile(string flowNo, int nodeId, string userNo, long workId)
        {
            MethodReturnMessage<byte[]> msg = null;
            try
            {
                string tableName = "ND" + int.Parse(flowNo) + "Rpt";

                string str = "WordFile";
                if (BP.DA.DBAccess.IsExitsTableCol(tableName, str) == false)
                {
                    /*如果没有此列，就自动创建此列.*/
                    string sql = "ALTER TABLE " + tableName + " ADD  " + str + " image ";

                    if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                        sql = "ALTER TABLE " + tableName + " ADD  " + str + " image ";

                    BP.DA.DBAccess.RunSQL(sql);
                }

                byte[] bytes = BP.DA.DBAccess.GetByteFromDB(tableName, "OID", workId.ToString(), "WordFile");

                if (bytes == null)
                {
                    Microsoft.Office.Interop.Word.Application docApp = new Microsoft.Office.Interop.Word.Application();
                    Microsoft.Office.Interop.Word.Document doc;
                    object miss = System.Reflection.Missing.Value;
                    string strContext; //文档内容  
                    doc = docApp.Documents.Add(ref miss, ref miss, ref miss, ref miss);
                    docApp.Selection.ParagraphFormat.LineSpacing = 15;
                    //页眉    
                    //docApp.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdOutlineView;
                    //docApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryHeader;
                    //docApp.ActiveWindow.ActivePane.Selection.InsertAfter("[页眉内容]");
                    //docApp.Selection.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                    //docApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;
                    //页尾    
                    //docApp.ActiveWindow.View.Type = Microsoft.Office.Interop.Word.WdViewType.wdOutlineView;
                    //docApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryFooter;
                    //docApp.ActiveWindow.ActivePane.Selection.InsertAfter("[页尾内容]");
                    //docApp.Selection.Paragraphs.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                    //docApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;

                    strContext = "欢迎使用ccflow word";
                    doc.Paragraphs.Last.Range.Text = strContext;

                    string rootPath = BP.Sys.SystemConfig.PathOfDataUser + "\\worddoc\\";

                    if (!System.IO.Directory.Exists(rootPath))
                        System.IO.Directory.CreateDirectory(rootPath);

                    string fileName = userNo + "_" + flowNo + "_" + workId + ".docx";
                    string fullFilePath = rootPath + fileName;

                    //保存文件    
                    doc.SaveAs(fullFilePath, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);
                    doc.Close(ref miss, ref miss, ref miss);
                    docApp.Quit(ref miss, ref miss, ref miss);

                    bytes = BP.DA.DataType.ConvertFileToByte(fullFilePath);

                    WordDoc_SaveWordFile(flowNo, nodeId, userNo, workId, bytes);

                    File.Delete(fullFilePath);
                }

                msg = new MethodReturnMessage<byte[]>
                {
                    Success = true,
                    Message = "读取文件成功",
                    Data = bytes
                };
            }
            catch (Exception ex)
            {
                msg = new MethodReturnMessage<byte[]>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }


            return LitJson.JsonMapper.ToJson(msg);
        }

        [WebMethod]
        public string WordDoc_SaveWordFile(string flowNo, int nodeId, string userNo, long workId, byte[] bytes)
        {
            MethodReturnMessage<string> msg = null;
            try
            {
                string tableName = "ND" + int.Parse(flowNo) + "Rpt";

                BP.DA.DBAccess.SaveBytesToDB(bytes, tableName, "OID", workId, "WordFile");

                msg = new MethodReturnMessage<string>
                {
                    Success = true,
                    Message = "读取文件成功",
                    Data = ""
                };

            }
            catch (Exception ex)
            {
                msg = new MethodReturnMessage<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = ""
                };

            };

            return LitJson.JsonMapper.ToJson(msg);
        }
        #endregion
    }

    /// <summary>
    /// 返回信息格式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MethodReturnMessage<T>
    {
        /// <summary>
        /// 是否运行成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 信息    
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public T Data { get; set; }
    }
}
