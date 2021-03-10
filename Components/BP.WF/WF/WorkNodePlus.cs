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
                    sf.MyPK = pkval;
                    sf.RetrieveFromDBSources();
                }

                //把草稿移交给当前人员. - 更新控制表.
                gwfSubFlow.Starter = WebUser.No;
                gwfSubFlow.StarterName = WebUser.Name;
                gwfSubFlow.Update();
                //把草稿移交给当前人员. - 更新工作人员列表.
                DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET FK_Emp='"+WebUser.No+"',FK_EmpText='"+BP.Web.WebUser.Name+"' WHERE WorkID="+gwfSubFlow.WorkID);
                //更新track表.
                //DBAccess.RunSQL("UPDATE ND"+int.Parse(gwfSubFlow.FK_Flow) +"Track SET FK_Emp='" + WebUser.No + "',FK_EmpText='" + WebUser.Name + "' WHERE WorkID=" + gwfSubFlow.WorkID);

                //启动子流程. 并把两个字段，写入子流程.
                BP.WF.Dev2Interface.Node_SendWork(gwfSubFlow.FK_Flow, gwfSubFlow.WorkID, null, null);
            }
        }
        /// <summary>
        /// 生成单据
        /// </summary>
        /// <param name="wn"></param>
        public static void GenerRtfBillTemplate(WorkNode wn)
        {
            BillTemplates reffunc = wn.HisNode.BillTemplates;

            #region 生成单据信息
            Int64 workid = wn.HisWork.OID;
            int nodeId = wn.HisNode.NodeID;
            string flowNo = wn.HisNode.FK_Flow;
            #endregion

            DateTime dtNow = DateTime.Now;
            Flow fl = wn.HisNode.HisFlow;
            string year = dtNow.Year.ToString();
            string billInfo = "";
            foreach (BillTemplate func in reffunc)
            {
                if (func.TemplateFileModel != TemplateFileModel.RTF)
                    continue;

                string file = year + "_" + wn.ExecerDeptNo + "_" + func.No + "_" + workid + ".doc";
                BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();

                Works works;
                string[] paths;
                string path;
                try
                {
                    #region 把数据放入 单据引擎。
                    rtf.HisEns.Clear(); //主表数据。
                    rtf.EnsDataDtls.Clear(); // 明细表数据.

                    // 找到主表数据.
                    rtf.HisGEEntity = new GEEntity(wn.rptGe.ClassID);
                    rtf.HisGEEntity.Row = wn.rptGe.Row;

                    // 把每个节点上的工作加入到报表引擎。
                    rtf.AddEn(wn.HisWork);
                    rtf.ensStrs += ".ND" + wn.HisNode.NodeID;

                    //把当前work的Dtl 数据放进去了。
                    System.Collections.Generic.List<Entities> al = wn.HisWork.GetDtlsDatasOfList();

                    foreach (Entities ens in al)
                        rtf.AddDtlEns(ens);
                    #endregion 把数据放入 单据引擎。

                    #region 生成单据

                    paths = file.Split('_');
                    path = paths[0] + "/" + paths[1] + "/" + paths[2] + "/";
                    string billUrl = wn.VirPath + "DataUser/Bill/" + path + file;
                    if (func.HisBillFileType == BillFileType.PDF)
                    {
                        billUrl = billUrl.Replace(".doc", ".pdf");
                        billInfo += "<img src='./Img/FileType/PDF.gif' /><a href='" + billUrl + "' target=_blank >" + func.Name + "</a>";
                    }
                    else
                    {
                        billInfo += "<img src='./Img/FileType/doc.gif' /><a href='" + billUrl + "' target=_blank >" + func.Name + "</a>";
                    }

                    path = BP.WF.Glo.FlowFileBill + year + "\\" + wn.ExecerDeptNo + "\\" + func.No + "\\";
                    // path = AppDomain.CurrentDomain.BaseDirectory + path;
                    if (System.IO.Directory.Exists(path) == false)
                        System.IO.Directory.CreateDirectory(path);

                    rtf.MakeDoc(func.TempFilePath + ".rtf", path, file);
                    #endregion

                    #region 转化成pdf.
                    if (func.HisBillFileType == BillFileType.PDF)
                    {
                        string rtfPath = path + file;
                        string pdfPath = rtfPath.Replace(".doc", ".pdf");
                        try
                        {
                            Glo.Rtf2PDF(rtfPath, pdfPath);
                        }
                        catch (Exception ex)
                        {
                            wn.addMsg("RptError", BP.WF.Glo.multilingual("生成报表数据错误:{0}.", "WorkNode", "rpt_error", ex.Message));

                        }
                    }
                    #endregion

                    #region 保存单据
                    Bill bill = new Bill();
                    bill.MyPK = wn.HisWork.FID + "_" + wn.HisWork.OID + "_" + wn.HisNode.NodeID + "_" + func.No;
                    bill.FID = wn.HisWork.FID;
                    bill.WorkID = wn.HisWork.OID;
                    bill.FK_Node = wn.HisNode.NodeID;
                    bill.FK_Dept = wn.ExecerDeptNo;
                    bill.FK_Emp = wn.Execer;
                    bill.Url = billUrl;
                    bill.RDT = DataType.CurrentDataTime;
                    bill.FullPath = path + file;
                    bill.FK_NY = DataType.CurrentYearMonth;
                    bill.FK_Flow = wn.HisNode.FK_Flow;
                    //  bill.FK_BillType = func.FK_BillType;
                    bill.FK_Flow = wn.HisNode.FK_Flow;
                    bill.Emps = wn.rptGe.GetValStrByKey("Emps");
                    bill.FK_Starter = wn.rptGe.GetValStrByKey("Rec");
                    bill.StartDT = wn.rptGe.GetValStrByKey("RDT");
                    bill.Title = wn.rptGe.GetValStrByKey("Title");
                    bill.FK_Dept = wn.rptGe.GetValStrByKey("FK_Dept");
                    try
                    {
                        bill.Save();
                    }
                    catch
                    {
                        bill.Update();
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    BP.WF.DTS.InitBillDir dir = new BP.WF.DTS.InitBillDir();
                    dir.Do();
                    path = BP.WF.Glo.FlowFileBill + year + "\\" + wn.ExecerDeptNo + "\\" + func.No + "\\";

                    string[] para1 = new string[4];
                    para1[0] = BP.WF.Glo.FlowFileBill;
                    para1[1] = ex.Message;
                    para1[2] = file;
                    para1[3] = path;
                    string msgErr1 = BP.WF.Glo.multilingual("@生成单据失败,请让管理员检查目录设置:[{0}].@Err:{1},@File={2},@Path:{3}.", "WorkNode", "wf_eng_error_2", para1);
                    string msgErr2 = BP.WF.Glo.multilingual("@系统已经做了可能性的修复，请您再发送一次，如果问题仍然存在请联系管理员。", "WorkNode", "wf_eng_error_3");
                    string msgErr3 = BP.WF.Glo.multilingual("@其它信息:{0}.", "WorkNode", "other_info", ex.Message);

                    billInfo += "@<font color=red>" + msgErr1 + "</font>@<hr>" + msgErr2;
                    throw new Exception(msgErr1 + msgErr3);
                }
            } // end 生成循环单据。

            if (billInfo != "")
                billInfo = "@" + billInfo;
            wn.addMsg(SendReturnMsgFlag.BillInfo, billInfo);
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
                                continue;
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
            #endregion qinfaliang, 编写同步的业务逻辑,执行错误就抛出异常.
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
                    sqlUnion += " SELECT " + BP.Sys.Glo.UserNoWhitOutAS + " as FK_Emp FROM Port_Emp WHERE FK_Dept='" + WebUser.FK_Dept + "' ";
                    sqlUnion += " UNION ";
                    sqlUnion += " SELECT FK_Emp FROM Port_DeptEmp WHERE FK_Dept='" + WebUser.FK_Dept + "'";
                }

                //主部门的人员.
                if (nd.GenerWorkerListDelRole == 3)
                {
                    sqlUnion += " SELECT " + BP.Sys.Glo.UserNo + " FROM Port_Emp WHERE FK_Dept='" + WebUser.FK_Dept + "' ";
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
                station = DBAccess.RunSQLReturnVal(station).ToString();
                string stationEmp = "SELECT FK_Emp FROM Port_DeptEmpStation where FK_Station = " + station + "";
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
    }
}
