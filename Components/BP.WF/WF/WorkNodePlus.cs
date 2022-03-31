using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Web;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;
using System.Text;
using BP.Tools;

namespace BP.WF
{
    /// <summary>
    /// WorkNode的附加类: 2020年06月09号
    /// 1， 因为worknode的类方法太多，为了能够更好的减轻代码逻辑.
    /// 2.  一部分方法要移动到这里来. 
    /// </summary>
    public class WorkNodePlus
    {
        /// <summary>
        /// 处理数据源.
        /// </summary>
        /// <param name="fl"></param>
        /// <param name="nd"></param>
        /// <param name="track"></param>
        public static void AddNodeFrmTrackDB(Flow fl, Node nd, Track track, Work wk)
        {
            //如果是单个表单.
            if (nd.HisFormType == NodeFormType.Develop
                 || nd.HisFormType == NodeFormType.FoolTruck
                || nd.HisFormType == NodeFormType.FoolForm)
            {
                //接点表单保存NDXXRpt
                string frmID = "ND" + Int32.Parse(fl.No) + "Rpt";
                string dtlJson = AddNodeFrmDtlDB(nd.NodeID, track.WorkID,nd.NodeFrmID);
                BP.Sys.FrmDBVer.AddFrmDBTrack(frmID, track.WorkID.ToString(), track.MyPK, wk.ToJson(), dtlJson);
                return;
            }
            if (nd.HisFormType == NodeFormType.RefOneFrmTree)
            {
                string dtlJson = AddNodeFrmDtlDB(nd.NodeID, track.WorkID, nd.NodeFrmID);
                BP.Sys.FrmDBVer.AddFrmDBTrack(nd.NodeFrmID, track.WorkID.ToString(), track.MyPK, wk.ToJson(), dtlJson);
                return;
            }

            //如果是多个表单.
            if (nd.HisFormType == NodeFormType.SheetTree)
            {
                FrmNodes fns = new FrmNodes(fl.No,nd.NodeID);

                foreach (FrmNode fn in fns)
                {
                    if (fn.FrmSln == FrmSln.Readonly)
                        continue;

                    BP.Sys.GEEntity ge = new GEEntity(fn.FK_Frm);
                    ge.OID = track.WorkID;
                    if (ge.RetrieveFromDBSources() == 0)
                        continue;
                    string dtlJson = AddNodeFrmDtlDB(nd.NodeID, track.WorkID, fn.FK_Frm);
                    BP.Sys.FrmDBVer.AddFrmDBTrack(fn.FK_Frm, track.WorkID.ToString(), track.MyPK, ge.ToJson(), dtlJson);
                }
                return;
            }

            //if (fl.HisNodes)
            // FrmDBVer.AddFrmDBTrack()

        }
        /// <summary>
        /// 获取表单从表的数据集合
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="workID"></param>
        /// <param name="frmID"></param>
        /// <returns></returns>
        public static string AddNodeFrmDtlDB(int nodeId, Int64 workID,string frmID)
        {
            MapData mapData = new MapData(frmID);
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            MapDtls dtls = mapData.MapDtls;
            DataSet ds = new DataSet();
            DataTable dt = null;
            foreach (MapDtl dtl in dtls)
            {
                string dtlRefPKVal = BP.WF.Dev2Interface.GetDtlRefPKVal(gwf.WorkID, gwf.PWorkID, gwf.FID, nodeId, dtl.FK_MapData, dtl);
                dt = CCFormAPI.GetDtlInfo(dtl, null, dtlRefPKVal);
                dt.TableName = dtl.PTable;
                ds.Tables.Add(dt);
            }
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 发送草稿实例 2020.10.27 fro 铁路局.
        /// </summary>
        public static void SendDraftSubFlow(WorkNode wn)
        {
            //如果不允许发送草稿子流程.
            if (wn.HisNode.IsSendDraftSubFlow == false)
                return;

            //查询出来该流程实例下的所有草稿子流程.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, wn.WorkID, GenerWorkFlowAttr.WFState, 1);

            //子流程配置信息.
            SubFlowHandGuide sf = null;

            //开始发送子流程.
            foreach (GenerWorkFlow gwfSubFlow in gwfs)
            {
                //获得配置信息.
                if (sf == null || sf.FK_Flow != gwfSubFlow.FK_Flow)
                {
                    string pkval = wn.HisGenerWorkFlow.FK_Flow + "_" + gwfSubFlow.FK_Flow + "_0";
                    sf = new SubFlowHandGuide();
                    sf.setMyPK(pkval);
                    sf.RetrieveFromDBSources();
                }

                //把草稿移交给当前人员. - 更新控制表.
                gwfSubFlow.Starter = WebUser.No;
                gwfSubFlow.StarterName = WebUser.Name;
                gwfSubFlow.Update();
                //把草稿移交给当前人员. - 更新工作人员列表.
                DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET FK_Emp='" + WebUser.No + "',FK_EmpText='" + BP.Web.WebUser.Name + "' WHERE WorkID=" + gwfSubFlow.WorkID);
                //更新track表.
                //DBAccess.RunSQL("UPDATE ND"+int.Parse(gwfSubFlow.FK_Flow) +"Track SET FK_Emp='" + WebUser.No + "',FK_EmpText='" + WebUser.Name + "' WHERE WorkID=" + gwfSubFlow.WorkID);

                //启动子流程. 并把两个字段，写入子流程.
                BP.WF.Dev2Interface.Node_SendWork(gwfSubFlow.FK_Flow, gwfSubFlow.WorkID, null, null);
            }
        }
        /// <summary>
        /// 当要发送是检查流程是否可以允许发起.
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="wk">开始节点工作</param>
        /// <returns></returns>
        public static bool CheckIsCanStartFlow_SendStartFlow(Flow flow, Work wk)
        {
            StartLimitRole role = flow.StartLimitRole;
            if (role == StartLimitRole.None)
                return true;

            string sql = "";
            string ptable = flow.PTable;

            if (role == StartLimitRole.ColNotExit)
            {
                /* 指定的列名集合不存在，才可以发起流程。*/
                if (DataType.IsNullOrEmpty(flow.StartLimitPara) == true)
                    throw new Exception("err@流程发起限制规则出现错误,StartLimitRole.ColNotExit , 没有配置参数. ");

                //求出原来的值.
                string[] strs = flow.StartLimitPara.Split(',');
                string val = "";
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str) == true)
                        continue;
                    try
                    {
                        val += wk.GetValStringByKey(str);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("@流程设计错误,您配置的检查参数(" + flow.StartLimitPara + "),中的列(" + str + ")已经不存在表单里.");
                    }
                }


                //找出已经发起的全部流程.
                sql = "SELECT " + flow.StartLimitPara + " FROM " + ptable + " WHERE  WFState NOT IN(0,1) AND FlowStarter='" + WebUser.No + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string v = dr[0] + "" + dr[1] + "" + dr[2];
                    if (v == val)
                        return false;
                }
                return true;
            }

            // 配置的sql,执行后,返回结果是 0 .
            if (role == StartLimitRole.ResultIsZero)
            {
                sql = BP.WF.Glo.DealExp(flow.StartLimitPara, wk, null);
                if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                    return true;
                else
                    return false;
            }

            // 配置的sql,执行后,返回结果是 <> 0 .
            if (role == StartLimitRole.ResultIsNotZero)
            {
                sql = BP.WF.Glo.DealExp(flow.StartLimitPara, wk, null);
                if (DBAccess.RunSQLReturnValInt(sql, 0) != 0)
                    return true;
                else
                    return false;
            }

            //为子流程的时候，该子流程只能被调用一次.
            if (role == StartLimitRole.OnlyOneSubFlow)
            {
                sql = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + wk.OID;
                string pWorkidStr = DBAccess.RunSQLReturnStringIsNull(sql, "0");
                if (pWorkidStr == "0")
                    return true;

                sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pWorkidStr + " AND FK_Flow='" + flow.No + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 || dt.Rows.Count == 1)
                    return true;

                //  string title = dt.Rows[0]["Title"].ToString();
                string starter = dt.Rows[0]["Starter"].ToString();
                string rdt = dt.Rows[0]["RDT"].ToString();

                throw new Exception(flow.StartLimitAlert + "@该子流程已经被[" + starter + "], 在[" + rdt + "]发起，系统只允许发起一次。");
            }

            return true;
        }
        /// <summary>
        /// 开始执行数据同步,在流程运动的过程中，
        /// 数据需要同步到不同的表里去.
        /// </summary>
        /// <param name="fl">流程</param>
        /// <param name="gwf">实体</param>
        /// <param name="rpt">实体</param>
        public static void DTSData(Flow fl, GenerWorkFlow gwf, GERpt rpt, Node currNode, bool isStopFlow)
        {
            //判断同步类型.
            if (fl.DTSWay == DataDTSWay.None)
                return;

            bool isActiveSave = false;
            // 判断是否符合流程数据同步条件.
            switch (fl.DTSTime)
            {
                case FlowDTSTime.AllNodeSend://所有节点发送后
                    isActiveSave = true;
                    break;
                case FlowDTSTime.SpecNodeSend://指定节点发送后
                    if (fl.DTSSpecNodes.Contains(currNode.NodeID.ToString()) == true)
                        isActiveSave = true;
                    break;
                case FlowDTSTime.WhenFlowOver://流程结束时
                    if (isStopFlow)
                        isActiveSave = true;
                    break;
                default:
                    break;
            }
            if (isActiveSave == false)
                return;

            #region zqp, 编写同步的业务逻辑,执行错误就抛出异常.
            if (fl.DTSWay == DataDTSWay.Syn)
            {
                //获取同步字段
                string[] dtsArray = fl.DTSFields.Split('@');
                //本系统字段
                string lcAttrs = "";
                //业务系统字段
                string ywAttrs = "";

                for (int i = 0; i < dtsArray.Length; i++)
                {
                    //获取本系统字段
                    lcAttrs += dtsArray[i].Split('=')[0] + ",";
                    //获取业务系统字段
                    ywAttrs += dtsArray[i].Split('=')[1] + ",";
                }

                string[] lcArr = lcAttrs.TrimEnd(',').Split(',');//取出对应的主表字段
                string[] ywArr = ywAttrs.TrimEnd(',').Split(',');//取出对应的业务表字段

                //判断本系统表是否存在
                string sql = "SELECT " + lcAttrs.TrimEnd(',') + " FROM " + fl.PTable.ToUpper() + " WHERE OID=" + rpt.OID;
                DataTable lcDt = DBAccess.RunSQLReturnTable(sql);
                if (lcDt.Rows.Count == 0)
                    throw new Exception("没有找到业务表数据.");

                //获取配置的，要同步的业务表
                BP.Sys.SFDBSrc src = new BP.Sys.SFDBSrc(fl.DTSDBSrc);
                sql = "SELECT " + ywAttrs.TrimEnd(',') + " FROM " + fl.DTSBTable.ToUpper();
                //获取业务表，是否有数据
                DataTable ywDt = src.RunSQLReturnTable(sql);

                //插入字段字符串
                string values = "";
                //更新字段字符串
                string upVal = "";

                //循环本系统表，组织同步语句
                for (int i = 0; i < lcArr.Length; i++)
                {
                    //系统类别
                    switch (src.DBSrcType)
                    {
                        case Sys.DBSrcType.Localhost:
                            switch (SystemConfig.AppCenterDBType)
                            {
                                case DBType.MSSQL:
                                    break;
                                case DBType.Oracle:
                                    //如果是时间类型，要进行转换
                                    if (ywDt.Columns[ywArr[i]].DataType == typeof(DateTime))
                                    {
                                        if (!DataType.IsNullOrEmpty(lcDt.Rows[0][lcArr[i].ToString()].ToString()) && lcDt.Rows[0][lcArr[i].ToString()] != "@RDT")
                                            values += "to_date('" + lcDt.Rows[0][lcArr[i].ToString()] + "','YYYY-MM-DD'),";
                                        else
                                            values += "'',";
                                        continue;
                                    }
                                    values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                                    upVal += upVal + ywArr[i] + "='" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                                    break;
                                case DBType.MySQL:
                                    break;
                                case DBType.Informix:
                                    break;
                                default:
                                    throw new Exception("没有涉及到的连接测试类型...");
                            }
                            break;
                        case Sys.DBSrcType.SQLServer:
                            break;
                        case Sys.DBSrcType.MySQL:
                            break;
                        case Sys.DBSrcType.Oracle:
                            //如果是时间类型，要进行转换
                            if (ywDt.Columns[ywArr[i]].DataType == typeof(DateTime))
                            {
                                if (!DataType.IsNullOrEmpty(lcDt.Rows[0][lcArr[i].ToString()].ToString()) && lcDt.Rows[0][lcArr[i].ToString()] != "@RDT")
                                    values += "to_date('" + lcDt.Rows[0][lcArr[i].ToString()] + "','YYYY-MM-DD'),";
                                else
                                    values += "'',";
                                continue;
                            }
                            values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                            upVal += upVal + ywArr[i] + "='" + lcDt.Rows[0][lcArr[i].ToString()] + "',";
                            continue;
                        default:
                            throw new Exception("暂时不支您所使用的数据库类型!");
                    }
                    values += "'" + lcDt.Rows[0][lcArr[i].ToString()] + "',";

                }

                values = values.Substring(0, values.Length - 1);
                upVal = upVal.Substring(0, upVal.Length - 1);

                //查询对应的业务表中是否存在这条记录
                sql = "SELECT * FROM " + fl.DTSBTable.ToUpper() + " WHERE " + fl.DTSBTablePK + "='" + lcDt.Rows[0][fl.DTSBTablePK] + "'";
                DataTable dt = src.RunSQLReturnTable(sql);
                //如果存在，执行更新，如果不存在，执行插入
                if (dt.Rows.Count > 0)
                    sql = "UPDATE " + fl.DTSBTable.ToUpper() + " SET " + upVal + " WHERE " + fl.DTSBTablePK + "='" + lcDt.Rows[0][fl.DTSBTablePK] + "'";
                else
                    sql = "INSERT INTO " + fl.DTSBTable.ToUpper() + "(" + ywAttrs.TrimEnd(',') + ") VALUES(" + values + ")";

                try
                {
                    src.RunSQL(sql);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            if (fl.DTSWay == DataDTSWay.WebAPI)
            {
                //推送的数据
                string info = "{";
                //推送的主表数据
                string mainTable = "";
                mainTable += "\"mainTable\":";
                mainTable += "{";
                MapAttrs mattrs = new MapAttrs(currNode.NodeFrmID);
                foreach (MapAttr attr in mattrs)
                {
                    if (attr.KeyOfEn.Equals("Title") || attr.KeyOfEn.Equals("BillNo"))
                        continue;
                    if (attr.KeyOfEn.Equals("AtPara") || attr.KeyOfEn.Equals("BillState"))
                        continue;
                    if (attr.KeyOfEn.Equals("RDT") || attr.KeyOfEn.Equals("OrgNo"))
                        continue;
                    if (attr.KeyOfEn.Equals("FK_Dept") || attr.KeyOfEn.Equals("FID"))
                        continue;
                    if (attr.KeyOfEn.Equals("Starter") || attr.KeyOfEn.Equals("StarterName"))
                        continue;
                    if (attr.KeyOfEn.Equals("OID") || attr.KeyOfEn.Equals("Rec"))
                        continue;
                    mainTable += "\"" + attr.KeyOfEn + "\":\"" + rpt.GetValStrByKey(attr.KeyOfEn) + "\",";
                }
                mainTable += "\"oid\":\"" + gwf.WorkID + "\"";
                mainTable += "}";

                //推送的从表数据
                string dtls = "[";
                string dtlData = "";

                MapDtls mapDtls = new MapDtls();
                mapDtls.Retrieve(MapDtlAttr.FK_MapData, currNode.NodeFrmID);
                foreach (MapDtl dtl in mapDtls)
                {
                    dtlData += "{";
                    dtlData += "\"dtlNo\":\"" + dtl.No + "\",";
                    //多个从表的数据
                    string dtlList = "[";
                    //每一行数据
                    string dtlOne = "";
                    //每一行的字段数据
                    string dtlKeys = "";
                    //从表附件
                    string dtlAths = "[";
                    string dtlAth = "";

                    MapAttrs dtlAttrs = new MapAttrs(dtl.No);
                    GEDtls geDtls = new GEDtls(dtl.No);
                    geDtls.Retrieve(GEDtlAttr.RefPK, gwf.WorkID);
                    foreach (GEDtl geDtl in geDtls)
                    {
                        dtlKeys = "{";
                        foreach (MapAttr attr in dtlAttrs)
                        {
                            if (attr.KeyOfEn.Equals("OID") || attr.KeyOfEn.Equals("RefPK"))
                                continue;
                            if (attr.KeyOfEn.Equals("FID") || attr.KeyOfEn.Equals("RDT"))
                                continue;
                            if (attr.KeyOfEn.Equals("Rec") || attr.KeyOfEn.Equals("AthNum"))
                                continue;
                            dtlKeys += "\"" + attr.KeyOfEn + "\":\"" + geDtl.GetValByKey(attr.KeyOfEn) + "\",";
                        }
                        if (!DataType.IsNullOrEmpty(dtlKeys))
                            dtlKeys = dtlKeys.Substring(0, dtlKeys.Length - 1);
                        dtlKeys += "}";

                        FrmAttachmentDBs attachmentDBs = new FrmAttachmentDBs();
                        attachmentDBs.Retrieve(FrmAttachmentDBAttr.FK_MapData, dtl.No, FrmAttachmentDBAttr.RefPKVal, geDtl.OID);
                        foreach (FrmAttachmentDB frmAttachmentDB in attachmentDBs)
                        {
                            dtlAth += "{";
                            dtlAth += "\"fileFullName\":\"" + frmAttachmentDB.FileFullName + "\",";
                            dtlAth += "\"fileName\":\"" + frmAttachmentDB.FileName + "\",";
                            dtlAth += "\"sort\":\"" + frmAttachmentDB.Sort + "\",";
                            dtlAth += "\"fileExts\":\"" + frmAttachmentDB.FileExts + "\",";
                            dtlAth += "\"rdt\":\"" + frmAttachmentDB.RDT + "\",";
                            dtlAth += "\"rec\":\"" + frmAttachmentDB.Rec + "\",";
                            dtlAth += "\"myPK\":\"" + frmAttachmentDB.MyPK + "\",";
                            dtlAth += "\"recName\":\"" + frmAttachmentDB.RecName + "\",";
                            dtlAth += "\"fk_dept\":\"" + frmAttachmentDB.FK_Dept + "\",";
                            dtlAth += "\"fk_deptName\":\"" + frmAttachmentDB.FK_DeptName + "\"";
                            dtlAth += "},";
                        }
                        if (!DataType.IsNullOrEmpty(dtlAth))
                            dtlAth = dtlAth.Substring(0, dtlAth.Length - 1);
                        dtlAth += "]";
                        dtlOne += "{";
                        dtlOne += "\"dtlData\":" + dtlKeys + ",";
                        dtlOne += "\"dtlAths\":[" + dtlAth + "";
                        dtlOne += "},";
                    }
                    if (!DataType.IsNullOrEmpty(dtlOne))
                        dtlOne = dtlOne.Substring(0, dtlOne.Length - 1);
                    dtlList += dtlOne;
                    dtlList += "]";

                    dtlData += "\"dtl\":" + dtlList + "";
                    dtlData += "},";
                }
                if (!DataType.IsNullOrEmpty(dtlData))
                    dtlData = dtlData.Substring(0, dtlData.Length - 1);
                dtls += dtlData;
                dtls += "]";

                //附件数据
                string aths = "[";
                string ath = "";

                FrmAttachments attachments = new FrmAttachments();
                attachments.Retrieve(FrmAttachmentAttr.FK_MapData, currNode.NodeFrmID, FrmAttachmentAttr.FK_Node, 0);
                foreach (FrmAttachment attachment in attachments)
                {
                    FrmAttachmentDBs dbs = new FrmAttachmentDBs();
                    dbs.Retrieve(FrmAttachmentDBAttr.FK_FrmAttachment, attachment.MyPK, FrmAttachmentDBAttr.FK_MapData, currNode.NodeFrmID, FrmAttachmentDBAttr.RefPKVal, gwf.WorkID);
                    if (dbs.Count <= 0)
                        continue;
                    ath += "{";
                    ath += "\"attachmentid\":\"" + attachment.MyPK + "\",";

                    string athdb = "";
                    foreach (FrmAttachmentDB db in dbs)
                    {
                        athdb += "{";
                        athdb += "\"fileFullName\":\"" + db.FileFullName + "\",";
                        athdb += "\"fileName\":\"" + db.FileName + "\",";
                        athdb += "\"sort\":\"" + db.Sort + "\",";
                        athdb += "\"fileExts\":\"" + db.FileExts + "\",";
                        athdb += "\"rdt\":\"" + db.RDT + "\",";
                        athdb += "\"myPK\":\"" + db.MyPK + "\",";
                        athdb += "\"refPKVal\":\"" + db.RefPKVal + "\",";
                        athdb += "\"rec\":\"" + db.Rec + "\",";
                        athdb += "\"recName\":\"" + db.RecName + "\",";
                        athdb += "\"fk_dept\":\"" + db.FK_Dept + "\",";
                        athdb += "\"fk_deptName\":\"" + db.FK_DeptName + "\",";
                        athdb += "},";
                    }
                    if (!DataType.IsNullOrEmpty(athdb))
                        athdb = athdb.Substring(0, athdb.Length - 1);
                    ath += "\"athdb\":" + athdb + "";
                    ath += "},";
                }
                if (!DataType.IsNullOrEmpty(ath))
                    ath = ath.Substring(0, ath.Length - 1);
                aths += ath;
                aths += "]";

                info += mainTable;
                info += ",\"dtls\":" + dtls;
                info += ",\"aths\":" + aths;
                info += "}";

                string apiUrl = fl.DTSWebAPI;
                Hashtable headerMap = new Hashtable();
                //设置token
                string token = "";
                //如果对接系统的token
                if (!DataType.IsNullOrEmpty(WebUser.Token))
                    token = WebUser.Token;
                else
                    token = WebUser.SID;

                //设置返回值格式
                headerMap.Add("Content-Type", "application/json");
                //设置token，用于接口校验
                headerMap.Add("Authorization", token);
                //执行POST
                string postData = BP.WF.Glo.HttpPostConnect(apiUrl, headerMap, info.ToString());
                var res = postData.ToJObject();
                if (!res["code"].ToString().Equals("200"))
                {
                    BP.DA.Log.DebugWriteInfo("同步失败:WebAPI请求地址：" + apiUrl + ",请求内容：" + postData);
                    BP.DA.Log.DebugWriteInfo("失败数据:" + info);
                }
                return;

            }
            #endregion zqp, 编写同步的业务逻辑,执行错误就抛出异常.
            return;
        }
        /// <summary>
        /// 处理协作模式下的删除规则
        /// </summary>
        /// <param name="nd">节点</param>
        /// <param name="gwf"></param>
        public static void GenerWorkerListDelRole(Node nd, GenerWorkFlow gwf)
        {
            if (nd.GenerWorkerListDelRole == 0)
                return;

            //按照部门删除,同部门下的人员+兼职部门.
            if (nd.GenerWorkerListDelRole == 1 ||
                nd.GenerWorkerListDelRole == 3 ||
                nd.GenerWorkerListDelRole == 4)
            {
                //定义本部门的人员. 主部门+兼职部门.
                string sqlUnion = "";
                if (nd.GenerWorkerListDelRole == 1)
                {
                    sqlUnion += " SELECT " + BP.Sys.Base.Glo.UserNoWhitOutAS + " as FK_Emp FROM Port_Emp WHERE FK_Dept='" + WebUser.FK_Dept + "' ";
                    sqlUnion += " UNION ";
                    sqlUnion += " SELECT FK_Emp FROM Port_DeptEmp WHERE FK_Dept='" + WebUser.FK_Dept + "'";
                }

                //主部门的人员.
                if (nd.GenerWorkerListDelRole == 3)
                {
                    sqlUnion += " SELECT " + BP.Sys.Base.Glo.UserNo + " FROM Port_Emp WHERE FK_Dept='" + WebUser.FK_Dept + "' ";
                }

                //兼职部门的人员.
                if (nd.GenerWorkerListDelRole == 4)
                {
                    sqlUnion += " SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Dept='" + WebUser.FK_Dept + "'";
                }

                //获得要删除的人员.
                string sql = " SELECT FK_Emp FROM WF_GenerWorkerlist WHERE ";
                sql += " WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=0 ";
                sql += " AND FK_Emp IN (" + sqlUnion + ")";

                //获得要删除的数据.
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string empNo = dt.Rows[i][0].ToString();
                    if (empNo.Equals(WebUser.No) == true)
                        continue;
                    sql = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + empNo + "'";
                    DBAccess.RunSQL(sql);
                }
            }

            //按照岗位删除.
            if (nd.GenerWorkerListDelRole == 2)
            {
                NodeStations nss = new NodeStations();
                nss.Retrieve(NodeStationAttr.FK_Node, gwf.FK_Node);
                if (nss.Count == 0)
                    throw new Exception("err@流程设计错误: 您设置了待办按照岗位删除的规则,但是在当前节点上，您没有设置岗位。");
                //定义岗位人员
                string station = "SELECT FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + WebUser.No + "'";
                station = DBAccess.RunSQLReturnString(station);
                string stationEmp = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station ='" + station + "'";
                //获得要删除的人员.
                string sql = " SELECT FK_Emp FROM WF_GenerWorkerlist WHERE ";
                sql += " WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=0 ";
                sql += " AND FK_Emp IN (" + stationEmp + ")";
                //获得要删除的数据.

                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string empNo = dt.Rows[i][0].ToString();
                    if (empNo == WebUser.No)
                        continue;
                    sql = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + empNo + "'";
                    DBAccess.RunSQL(sql);
                }
            }
        }
        /// <summary>
        /// 处理发送返回，断头路节点.
        /// </summary>
        public static WorkNode IsSendBackNode(WorkNode wn)
        {
            if (wn.HisNode.IsSendBackNode == false)
                return wn;  //如果不是断头路节点，就让其返回.
            if (wn.HisGenerWorkFlow.WFState == WFState.ReturnSta)
            {
                //是退回状态且原路返回的情况
                string sql = "SELECT ReturnNode, Returner, ReturnerName, IsBackTracking ";
                sql += " FROM WF_ReturnWork  ";
                sql += " WHERE WorkID=" + wn.WorkID + " ORDER BY RDT DESC";
                DataTable mydt = DBAccess.RunSQLReturnTable(sql);
                if (mydt.Rows.Count != 0 && mydt.Rows[0][3].ToString().Equals("1") == true)
                {
                    wn.JumpToNode = new Node(int.Parse(mydt.Rows[0]["ReturnNode"].ToString()));
                    wn.JumpToEmp = mydt.Rows[0]["Returner"].ToString();
                    return wn;
                }

            }
            if (wn.HisNode.HisToNDNum != 0)
                throw new Exception("err@流程设计错误:当前节点是发送自动返回节点，但是当前节点不能有到达的节点.");

            if (wn.HisNode.HisRunModel != RunModel.Ordinary)
                throw new Exception("err@流程设计错误:只能是线性节点才能设置[发送并返回]属性,当前节点是[" + wn.HisNode.HisRunModel.ToString() + "]");

            //判断是否是最后一个人？
            bool isLastOne = false;
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, wn.WorkID,
                GenerWorkerListAttr.FK_Node, wn.HisNode.NodeID, GenerWorkerListAttr.IsPass, 0);
            if (gwls.Count == 1)
                isLastOne = true; //如果只有一个，本人就是lastOne.

            //WorkNode wn= this.GetPreviousWorkNode();
            //this.JumpToEmp = wn.HisWork.Rec; //对于绑定的表单有问题.
            //this.JumpToNode = wn.HisNode;

            if (isLastOne == true || wn.HisNode.TodolistModel == TodolistModel.QiangBan)
            {
                string ptable = "ND" + int.Parse(wn.HisFlow.No) + "Track";

                var mysql = "";
                if (wn.HisNode.HisRunModel == RunModel.SubThread)
                    mysql = "SELECT NDFrom,EmpFrom FROM " + ptable + " WHERE (WorkID =" + wn.WorkID + " AND FID=" + wn.HisGenerWorkFlow.FID + ") AND ActionType!= " + (int)ActionType.UnSend + " AND NDTo = " + wn.HisNode.NodeID + " AND(NDTo != NDFrom) AND NDFrom In(Select Node From WF_Direction Where ToNode=" + wn.HisNode.NodeID + " AND FK_Flow='" + wn.HisFlow.No + "')";
                else
                    mysql = "SELECT NDFrom,EmpFrom FROM " + ptable + " WHERE WorkID =" + wn.WorkID + " AND ActionType!= " + (int)ActionType.UnSend + " AND NDTo = " + wn.HisNode.NodeID + " AND(NDTo != NDFrom) AND NDFrom In(Select Node From WF_Direction Where ToNode=" + wn.HisNode.NodeID + " AND FK_Flow='" + wn.HisFlow.No + "')";

                //DataTable mydt = DBAccess.RunSQLReturnTable("SELECT FK_Node,FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + this.WorkID + " AND FK_Node!=" + this.HisNode.NodeID + " ORDER BY RDT DESC ");
                DataTable mydt = DBAccess.RunSQLReturnTable(mysql);
                if (mydt.Rows.Count == 0)
                    throw new Exception("系统错误，没有找到上一个节点.");

                wn.JumpToEmp = mydt.Rows[0][1].ToString();
                var priNodeID = int.Parse(mydt.Rows[0][0].ToString());
                wn.JumpToNode = new Node(priNodeID);

                //清除选择，防止在自动发送到该节点上来.
                wn.HisGenerWorkFlow.Paras_ToNodes = "";
                wn.HisGenerWorkFlow.DirectUpdate();

                //清除上次发送的选择,不然下次还会自动发送到当前的节点上来.
                mysql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + wn.JumpToNode.NodeID + " AND WorkID=" + wn.WorkID;
                DBAccess.RunSQL(mysql);
            }
            return wn;
        }
        /// <summary>
        /// 处理 askfor 状态
        /// </summary>
        /// <param name="wn"></param>
        public static SendReturnObjs DealAskForState(WorkNode wn)
        {
            /*如果是加签状态, 就判断加签后，是否要返回给执行加签人.*/
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, wn.HisNode.NodeID,
                GenerWorkerListAttr.WorkID, wn.WorkID);

            bool isDeal = false;
            AskforHelpSta askForSta = AskforHelpSta.AfterDealSend;
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsPassInt == (int)AskforHelpSta.AfterDealSend)
                {
                    /*如果是加签后，直接发送就不处理了。*/
                    isDeal = true;
                    askForSta = AskforHelpSta.AfterDealSend;

                    // 更新workerlist, 设置所有人员的状态为已经处理的状态,让它走到下一步骤去.
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + wn.WorkID);

                    //写入日志.
                    wn.AddToTrack(ActionType.ForwardAskfor, item.FK_Emp, item.FK_EmpText,
                        wn.HisNode.NodeID, wn.HisNode.Name, BP.WF.Glo.multilingual("加签后向下发送，直接发送给下一步处理人。", "WorkNode", "send_to_next"));

                }

                if (item.IsPassInt == (int)AskforHelpSta.AfterDealSendByWorker)
                {
                    /*如果是加签后，在由我直接发送。*/
                    item.IsPassInt = 0;

                    isDeal = true;
                    askForSta = AskforHelpSta.AfterDealSendByWorker;

                    // 更新workerlist, 设置所有人员的状态为已经处理的状态.
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + wn.WorkID);

                    // 把发起加签人员的状态更新过来，让他可见待办工作.
                    item.IsPassInt = 0;
                    item.Update();

                    // 更新流程状态.
                    wn.HisGenerWorkFlow.WFState = WFState.AskForReplay;
                    wn.HisGenerWorkFlow.Update();

                    //让加签人，设置成工作未读。
                    BP.WF.Dev2Interface.Node_SetWorkUnRead(wn.WorkID, item.FK_Emp);

                    // 从临时变量里获取回复加签意见.
                    string replyInfo = wn.HisGenerWorkFlow.Paras_AskForReply;

                    ////写入日志.
                    //this.AddToTrack(ActionType.ForwardAskfor, item.FK_Emp, item.FK_EmpText,
                    //    this.HisNode.NodeID, this.HisNode.Name,
                    //    "加签后向下发送，并转向加签人发起人（" + item.FK_Emp + "，" + item.FK_EmpText + "）。<br>意见:" + replyInfo);

                    //写入日志.
                    wn.AddToTrack(ActionType.ForwardAskfor, item.FK_Emp, item.FK_EmpText,
                        wn.HisNode.NodeID, wn.HisNode.Name, BP.WF.Glo.multilingual("回复意见:{0}.", "WorkNode", "reply_comments", replyInfo));

                    //加入系统变量。
                    wn.addMsg(SendReturnMsgFlag.VarToNodeID, wn.HisNode.NodeID.ToString(), SendReturnMsgType.SystemMsg);
                    wn.addMsg(SendReturnMsgFlag.VarToNodeName, wn.HisNode.Name, SendReturnMsgType.SystemMsg);
                    wn.addMsg(SendReturnMsgFlag.VarAcceptersID, item.FK_Emp, SendReturnMsgType.SystemMsg);
                    wn.addMsg(SendReturnMsgFlag.VarAcceptersName, item.FK_EmpText, SendReturnMsgType.SystemMsg);

                    //加入提示信息.
                    wn.addMsg(SendReturnMsgFlag.SendSuccessMsg, BP.WF.Glo.multilingual("已经转给加签的发起人({0},{1})", "WorkNode", "send_to_the_operator", item.FK_Emp.ToString(), item.FK_EmpText), SendReturnMsgType.Info);

                    //删除当前操作员临时增加的工作列表记录, 如果不删除就会导致第二次加签失败.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.Delete(GenerWorkerListAttr.FK_Node, wn.HisNode.NodeID,
                        GenerWorkerListAttr.WorkID, wn.WorkID, GenerWorkerListAttr.FK_Emp, wn.Execer);

                    //调用发送成功事件.
                    string sendSuccess = ExecEvent.DoNode(EventListNode.SendSuccess, wn);
                    wn.HisMsgObjs.AddMsg("info21", sendSuccess, sendSuccess, SendReturnMsgType.Info);

                    //执行时效考核.
                    Glo.InitCH(wn.HisFlow, wn.HisNode, wn.WorkID, 0, wn.HisGenerWorkFlow.Title);

                    //返回发送对象.
                    return wn.HisMsgObjs;
                }
            }

            if (isDeal == false)
                throw new Exception(BP.WF.Glo.multilingual("@流程引擎错误，不应该找不到加签的状态.", "WorkNode", "wf_eng_error_1"));

            return null;
        }
        /// <summary>
        /// 执行分河流状态
        /// </summary>
        /// <param name="wn"></param>
        public static void DealHeLiuState(WorkNode wn)
        {
            /*   如果是合流点 检查当前是否是合流点如果是，则检查分流上的子线程是否完成。*/
            /*检查是否有子线程没有结束*/
            Paras ps = new Paras();
            ps.SQL = "SELECT WorkID,FK_Emp,FK_EmpText,FK_NodeText FROM WF_GenerWorkerList WHERE FID=" + ps.DBStr + "FID AND IsPass=0 AND IsEnable=1";
            ps.Add(WorkAttr.FID, wn.WorkID);

            DataTable dtWL = DBAccess.RunSQLReturnTable(ps);
            string infoErr = "";
            if (dtWL.Rows.Count != 0)
            {
                if (wn.HisNode.ThreadKillRole == ThreadKillRole.None
                    || wn.HisNode.ThreadKillRole == ThreadKillRole.ByHand)
                {
                    infoErr += BP.WF.Glo.multilingual("@您不能向下发送，有如下子线程没有完成。", "WorkNode", "cannot_send_to_next_1");

                    foreach (DataRow dr in dtWL.Rows)
                    {
                        string op = BP.WF.Glo.multilingual("@操作员编号:{0},{1}", "WorkNode", "current_operator", dr["FK_Emp"].ToString(), dr["FK_EmpText"].ToString());
                        string nd = BP.WF.Glo.multilingual("停留节点:{0}.", "WorkNode", "current_node", dr["FK_NodeText"].ToString());
                        //infoErr += "@操作员编号:" + dr["FK_Emp"] + "," + dr["FK_EmpText"] + ",停留节点:" + dr["FK_NodeText"];
                        infoErr += op + ";" + nd;
                    }

                    if (wn.HisNode.ThreadKillRole == ThreadKillRole.ByHand)
                        infoErr += BP.WF.Glo.multilingual("@请通知他们处理完成,或者强制删除子流程您才能向下发送.", "WorkNode", "cannot_send_to_next_2");

                    else
                        infoErr += BP.WF.Glo.multilingual("@请通知他们处理完成,您才能向下发送.", "WorkNode", "cannot_send_to_next_3");

                    //抛出异常阻止它向下运动。
                    throw new Exception(infoErr);
                }

                if (wn.HisNode.ThreadKillRole == ThreadKillRole.ByAuto)
                {
                    //删除每个子线程，然后向下运动。
                    foreach (DataRow dr in dtWL.Rows)
                        BP.WF.Dev2Interface.Flow_DeleteSubThread(Int64.Parse(dr[0].ToString()), BP.WF.Glo.multilingual("合流点发送时自动删除", "WorkNode", "auto_delete"));
                }
            }
        }

        /// <summary>
        /// 子流程运行结束后
        /// </summary>
        /// <param name="wn"></param>
        public static String SubFlowEvent(WorkNode wn)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(wn.WorkID);
            //判断是否是子流程
            if (gwf.PWorkID == 0)
                return "";
            string msg = "";
            //子流程运行到指定节点后父流程自动运行到下一个节点
            if (gwf.WFState != WFState.Complete)
            {
                SubFlows subFlows = new SubFlows();
                subFlows.Retrieve(SubFlowAttr.FK_Node, gwf.PNodeID, SubFlowAttr.SubFlowNo, wn.HisFlow.No);
                if (subFlows.Count == 0)
                    return "";
                SubFlow subFlow = subFlows[0] as SubFlow;

                if (subFlow.SubFlowNodeID == 0 || wn.HisNode.NodeID != subFlow.SubFlowNodeID)
                    return "";

                if (subFlow.ParentFlowSendNextStepRole == SubFlowRunModel.SpecifiedNodes)
                {
                    //获取父流程实例信息
                    GenerWorkFlow pgwf = new GenerWorkFlow(gwf.PWorkID);
                    if (pgwf.FK_Node == gwf.PNodeID)
                    {
                        SendReturnObjs returnObjs = BP.WF.Dev2Interface.Node_SendWork(gwf.PFlowNo, gwf.PWorkID);
                        msg = "父流程自动运行到下一个节点，" + returnObjs.ToMsgOfHtml();

                    }
                    return msg;
                }
                return "";
            }

            //子流程结束后父流程/同级子流程的处理
            if (gwf.WFState == WFState.Complete)
            {
                //先判断当前流程是下级子流程还是同级子流程
                Int64 slworkid = gwf.GetParaInt("SLWorkID");
                SubFlows subFlows = new SubFlows();
                SubFlow subFlow = null;
                //下级子流程
                if (slworkid == 0)
                {
                    //判断子流程中的设置关系
                    subFlows.Retrieve(SubFlowAttr.FK_Node, gwf.PNodeID, SubFlowAttr.SubFlowNo, wn.HisFlow.No);
                    if (subFlows.Count == 0)
                        return "";
                    subFlow = subFlows[0] as SubFlow;

                    //把子流程的数据反填到父流程中
                    SubFlowOver_CopyDataToParantFlow(subFlow, wn.HisGenerWorkFlow.PWorkID, wn);

                    int pnodeId = gwf.PNodeID;
                    FrmSubFlow nd = new FrmSubFlow(pnodeId);
                    switch (nd.AllSubFlowOverRole)
                    {
                        case AllSubFlowOverRole.None: //父节点不设置所有子流程结束规则
                                                      //发送成功后显示父流程的待办
                            if (subFlow.SubFlowHidTodolist == true)
                            {
                                GenerWorkFlow pgwf = new GenerWorkFlow(gwf.PWorkID);
                                string mysql = "SELECT COUNT(WorkID) as Num FROM WF_GenerWorkFlow WHERE PWorkID=" + gwf.PWorkID + " AND FK_Flow='" + wn.HisFlow.No + "' AND WFState !=3 ";
                                if (DBAccess.RunSQLReturnValInt(mysql, 0) == 0)
                                {
                                    DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 Where WorkID=" + pgwf.WorkID + " AND FK_Node=" + pgwf.FK_Node);

                                }
                            }

                            //单个子流程控制父流程运行到下一个节点
                            if (subFlow.ParentFlowSendNextStepRole == SubFlowRunModel.FlowOver)
                                msg = SubFlowOver_ParentFlowAutoSendNextSetp(false, wn.HisGenerWorkFlow.PWorkID, wn.HisGenerWorkFlow);
                            //单个子流程控制父流程结束
                            if (subFlow.ParentFlowOverRole == SubFlowRunModel.FlowOver)
                                msg = SubFlowOver_ParentFlowOver(false, wn.HisGenerWorkFlow.PWorkID);
                            break;
                        case AllSubFlowOverRole.SendParentFlowToNextStep://父流程设置所有子流程结束后，父流程运行到下一个节点
                            msg = SubFlowOver_ParentFlowAutoSendNextSetp(true, wn.HisGenerWorkFlow.PWorkID, wn.HisGenerWorkFlow);
                            break;
                        case AllSubFlowOverRole.OverParentFlow://父流程设置所有子流程结束后，父流程结束
                            msg = SubFlowOver_ParentFlowOver(true, wn.HisGenerWorkFlow.PWorkID);
                            break;
                        default: break;

                    }

                    return msg;
                }

                //同级子流程
                string slFlowNo = gwf.GetParaString("SLFlowNo");
                Int32 slNodeID = gwf.GetParaInt("SLNodeID");
                subFlows.Retrieve(SubFlowAttr.FK_Node, slNodeID, SubFlowAttr.SubFlowNo, wn.HisFlow.No);
                if (subFlows.Count == 0)
                    return "";
                subFlow = subFlows[0] as SubFlow;
                //子流程运行结束后父流程是否自动往下运行一步
                msg = SubFlowOver_DealSLSubFlow(slworkid, subFlow, wn.HisGenerWorkFlow);

                return msg;
            }
            return "";

        }
        /// <summary>
        /// 子流程运行结束后把数据反填到父流程中
        /// </summary>
        /// <param name="subFlow"></param>
        /// <param name="pworkid"></param>
        /// <param name="wn"></param>
        private static void SubFlowOver_CopyDataToParantFlow(SubFlow subFlow, Int64 pworkid, WorkNode wn)
        {
            //子流程结束后，数据不反填到父流程中
            if (subFlow.BackCopyRole == BackCopyRole.None)
                return;
            Node nd = new Node(subFlow.FK_Node);
            Work pwk = nd.HisWork;
            pwk.OID = pworkid;
            pwk.RetrieveFromDBSources();

            GERpt prpt = new BP.WF.Data.GERpt("ND" + int.Parse(subFlow.FK_Flow) + "Rpt");
            prpt.OID = pworkid;
            prpt.RetrieveFromDBSources();


            //判断是否启用了数据字段反填规则
            if (subFlow.BackCopyRole == BackCopyRole.AutoFieldMatch || subFlow.BackCopyRole == BackCopyRole.MixedMode)
            {
                //子流程数据拷贝到父流程中
                pwk.Copy(wn.HisWork);
                prpt.Copy(wn.HisWork);
            }
            // 子流程数据拷贝到父流程中
            if ((subFlow.BackCopyRole == BackCopyRole.FollowSetFormat || subFlow.BackCopyRole == BackCopyRole.MixedMode)
                && DataType.IsNullOrEmpty(subFlow.ParentFlowCopyFields) == false)
            {
                Work wk = wn.HisWork;
                Attrs attrs = wk.EnMap.Attrs;
                //获取子流程的签批字段
                string keyOfEns = "";
                string keyVals = ""; //签批字段存储的值
                foreach (Attr attr in attrs)
                {
                    if (attr.UIContralType == UIContralType.SignCheck)
                    {
                        keyOfEns += attr.Field + ",";
                        continue;
                    }

                }

                //父流程把子流程不同字段进行匹配赋值
                AtPara ap = new AtPara(subFlow.ParentFlowCopyFields);
                foreach (String str in ap.HisHT.Keys)
                {
                    Object val = ap.GetValStrByKey(str);
                    if (DataType.IsNullOrEmpty(val.ToString()) == true)
                        continue;
                    pwk.SetValByKey(val.ToString(), wk.GetValByKey(str));
                    prpt.SetValByKey(val.ToString(), wk.GetValByKey(str));
                    if (keyOfEns.Contains(str + ",") == true)
                        keyVals += wk.GetValByKey(str);
                }
                if (DataType.IsNullOrEmpty(keyVals) == false)
                {
                    string trackPTable = "ND" + int.Parse(wn.HisFlow.No) + "Track";
                    //把子流程的签批字段对应的审核信息拷贝到父流程中
                    keyVals = keyVals.Substring(1);
                    string sql = "SELECT * FROM " + trackPTable + " WHERE ActionType=22 AND WorkID=" + wn.WorkID + " AND NDFrom IN(" + keyVals + ")";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    Tracks tracks = new Tracks();
                    BP.En.QueryObject.InitEntitiesByDataTable(tracks, dt, null);
                    foreach (Track t in tracks)
                    {

                        t.WorkID = pwk.OID;
                        t.FID = pwk.FID;
                        t.FK_Flow = subFlow.FK_Flow;
                        t.HisActionType = ActionType.WorkCheck;
                        t.setMyPK(DBAccess.GenerOIDByGUID().ToString());
                        t.Insert();
                    }
                }

            }
            pwk.Update();
            prpt.Update();

        }
        /// <summary>
        /// 子流程运行结束后，父流程自动运行到下一个节点
        /// </summary>
        /// <param name="isAllSubFlowOver"></param>
        /// <param name="pworkid"></param>
        /// <param name="gwf"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string SubFlowOver_ParentFlowAutoSendNextSetp(bool isAllSubFlowOver, Int64 pworkid, GenerWorkFlow gwf)
        {
            //所有子流程结束后，父流程自动运行到下一个节点
            if (isAllSubFlowOver == true)
            {
                if (BP.WF.Dev2Interface.Flow_NumOfSubFlowRuning(pworkid) != 0)
                    return "";
            }

            #region 检查父流程是否符合自动运行到下一个节点的条件
            GenerWorkFlow pgwf = new GenerWorkFlow();
            pgwf.WorkID = pworkid;
            if (pgwf.RetrieveFromDBSources() == 0)
                return ""; // 父流程被删除了也不能执行。

            if (pgwf.WFState == WFState.Complete)
                return ""; //父流程已经完成也不能执行.

            //检查父流程的当前停留的节点是否还是发起子流程的节点？
            if (gwf.PNodeID != pgwf.FK_Node)
                return "";
            #endregion 检查父流程是否符合自动运行到下一个节点的条件

            //获得父流程.
            string[] strs = pgwf.TodoEmps.Split(';');
            strs = strs[0].Split(',');
            string empNo = strs[0];
            if (DataType.IsNullOrEmpty(empNo) == true)
                throw new Exception("err@没有找到父流程的处理人.");

            //当前登录用户.
            string currUserNo = BP.Web.WebUser.No;
            try
            {
                Emp emp = new Emp(empNo);
                //让父流程的userNo登录.
                BP.WF.Dev2Interface.Port_Login(emp.No);
                SendReturnObjs objs = null;
                try
                {
                    objs = BP.WF.Dev2Interface.Node_SendWork(pgwf.FK_Flow, pgwf.WorkID);
                    //切换到当前流程节点.
                    BP.WF.Dev2Interface.Port_Login(currUserNo);
                }
                catch (Exception ex)
                {
                    BP.WF.Dev2Interface.Port_Login(currUserNo);
                    throw new Exception(ex.Message);
                }

                return "@成功让父流程运行到下一个节点." + objs.ToMsgOfHtml();
            }
            catch (Exception ex)
            {
                //切换到当前流程节点.
                BP.WF.Dev2Interface.Port_Login(currUserNo);
                string info = "这个错误";
                if (ex.Message.Contains("WorkOpt/") == true)
                {
                    info += "@流程设计错误:自动运行到的下一个节点的接收人规则是由上一个人员来选择的,导致到不能自动运行到下一步骤.";
                    return info;
                }

                return "@在最后一个子流程完成后，让父流程的节点自动发送时，出现错误:" + ex.Message;
            }
        }
        /// <summary>
        /// 子流程结束后，父流程自动结束
        /// </summary>
        /// <param name="isAllSubFlowOver"></param>
        /// <param name="pworkid"></param>
        /// <returns></returns>
        public static string SubFlowOver_ParentFlowOver(bool isAllSubFlowOver, Int64 pworkid)
        {
            //所有子流程结束后，父流程自动结束
            if (isAllSubFlowOver == true)
            {
                if (BP.WF.Dev2Interface.Flow_NumOfSubFlowRuning(pworkid) == 0)
                    return "";
            }
            GenerWorkFlow gwf = new GenerWorkFlow(pworkid);
            if (gwf.WFState == WFState.Complete)
                return "";

            return BP.WF.Dev2Interface.Flow_DoFlowOver(gwf.WorkID, "父流程[" + gwf.FlowName + "],标题为[" + gwf.Title + "]成功结束");
        }
        /// <summary>
        /// 子流程结束后，处理同级子流程
        /// </summary>
        /// <param name="slWorkID"></param>
        /// <param name="subFlow"></param>
        /// <param name="gwf"></param>
        /// <returns></returns>
        private static string SubFlowOver_DealSLSubFlow(Int64 slWorkID, SubFlow subFlow, GenerWorkFlow gwf)
        {
            if (subFlow.IsAutoSendSLSubFlowOver == 0)
                return "";
            string slFlowNo = gwf.GetParaString("SLFlowNo");
            Int32 slNodeID = gwf.GetParaInt("SLNodeID");
            Flow fl = new Flow(slFlowNo);
            GenerWorkFlow subgwf = new GenerWorkFlow(slWorkID);
            if (subFlow.IsAutoSendSLSubFlowOver == 1)
            {
                if (subgwf.FK_Node != slNodeID)
                    return "";

                //主流程自动运行到一下节点
                SendReturnObjs returnObjs = BP.WF.Dev2Interface.Node_SendWork(slFlowNo, slWorkID);
                string sendSuccess = "同级子流程[" + fl.Name + "]程自动运行到下一个节点，发送过程如下：\n @接收人" + returnObjs.VarAcceptersName + "\n @下一步[" + returnObjs.VarCurrNodeName + "]启动";
                return sendSuccess;
            }
            //结束父流程
            if (subFlow.IsAutoSendSLSubFlowOver == 2)
            {
                if (subgwf.WFState == WFState.Complete)
                    return "";
                return BP.WF.Dev2Interface.Flow_DoFlowOver(slWorkID, "同级子流程流程[" + fl.Name + "],WorkID为[" + slWorkID + "]成功结束");
            }
            return "";
        }
    }
}
