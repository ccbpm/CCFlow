using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.Tools;
using BP.Web;
using BP.WF.Template;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using BP.Difference;
using BP.WF.Template.SFlow;
using System.Collections.Generic;
using BP.WF.Port;
using System.Threading;
using System.Web;
using NPOI.SS.Formula.Functions;

namespace BP.WF
{
    /// <summary>
    /// 此接口为程序员二次开发使用,在阅读代码前请注意如下事项.
    /// 1, CCFlow 的对外的接口都是以静态方法来实现的.
    /// 2, 以 DB_ 开头的是需要返回结果集合的接口.
    /// 3, 以 Flow_ 是流程接口.
    /// 4, 以 Node_ 是节点接口.
    /// 5, 以 Port_ 是组织架构接口.
    /// 6, 以 DTS_ 是调度. data tranr system.
    /// 8, 以 WorkOpt_ 用工作处理器相关的接口。
    /// 9, 以 Frm_ 处理文档打印相关的工作。
    /// </summary>
    public class Dev2Interface
    {

        #region 数量.
        /// <summary>
        /// 待办工作数量
        /// </summary>
        public static int Todolist_EmpWorks
        {
            get
            {
                string userNo = BP.Web.WebUser.No;

                Paras ps = new Paras();
                string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                string wfSql = "AND  1=1  ";

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    wfSql = " AND OrgNo='" + WebUser.OrgNo + "'";

                /*不是授权状态*/
                if (BP.WF.Glo.IsEnableTaskPool == true)
                {
                    ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1 " + wfSql;
                }
                else
                {
                    ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp " + wfSql;
                }

                ps.Add("FK_Emp", userNo);

                //获取授权给他的人员列表.
                //wfSql = "  1=1  ";
                Auths aths = new Auths();
                aths.Retrieve(AuthAttr.AutherToEmpNo, userNo);
                foreach (Auth auth in aths)
                {
                    string todata = auth.TakeBackDT.Replace("-", "");
                    if (DataType.IsNullOrEmpty(auth.TakeBackDT) == false)
                    {
                        int mydt = int.Parse(todata);
                        int nodt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        if (mydt < nodt)
                            continue;
                        ps.SQL += " UNION ";

                        if (auth.AuthType == AuthorWay.SpecFlows)
                            ps.SQL += "SELECT Count(*) FROM WF_EmpWorks  WHERE  FK_Emp='" + auth.Auther + "' AND FK_Flow='" + auth.FlowNo + "' " + wfSql + "";
                        else
                            ps.SQL += "SELECT Count(*) FROM WF_EmpWorks  WHERE  FK_Emp='" + auth.Auther + "' " + wfSql + "";


                    }
                }

                return DBAccess.RunSQLReturnValInt(ps);
            }
        }

        /// <summary>
        /// 抄送数量
        /// </summary>
        public static int Todolist_CCWorks
        {
            get
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT count(MyPK) as Num FROM WF_CCList WHERE CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp AND Sta=0";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps, 0);
            }
        }
        /// <summary>
        /// 根据workid返回抄送数据
        /// </summary>
        public static string Node_CCWorks(int WorkID, int CCType = -1, string keyword = "")
        {
            string sql = "select  * from wf_cclist where workid=" + WorkID;
            if (CCType >= 0)
            {
                sql += " and CCType=" + CCType;
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " and (CCToName LIKE '%" + keyword + "%' OR CCToDeptName LIKE '%" + keyword + "%' OR CCToOrgName LIKE '%" + keyword + "%')";
            }
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);

        }
        /// <summary>
        /// 返回挂起流程数量
        /// </summary>
        public static int Todolist_HungupNum
        {
            get
            {
                string sql = "";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    sql = "SELECT COUNT(WorkID) AS Num FROM WF_GenerWorkFlow WHERE WFState=4 AND Sender LIKE '" + BP.Web.WebUser.No + "%' ";
                else
                    sql = "SELECT COUNT(WorkID) AS Num FROM WF_GenerWorkFlow WHERE WFState=4 AND Sender LIKE '" + BP.Web.WebUser.No + "%' AND OrgNo='" + BP.Web.WebUser.OrgNo + "' ";

                return DBAccess.RunSQLReturnValInt(sql);
            }
        }
        /// <summary>
        /// 退回给当前用户的数量
        /// </summary>
        public static int Todolist_ReturnNum
        {
            get
            {
                string sql = "SELECT  COUNT(WorkID) AS Num from WF_GenerWorkFlow where WFState=5 and TodoEmps like '%" + WebUser.No + ";%' ";
                return DBAccess.RunSQLReturnValInt(sql);
            }
        }
        /// <summary>
        /// 待办逾期的数量
        /// </summary>
        public static int Todolist_OverWorkNum
        {
            get
            {
                string sql = "";
                if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                {
                    sql = "SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Emp='" + WebUser.No + "' AND STR_TO_DATE(SDT,'%Y-%m-%d %H:%i') < now()";

                }
                else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                {

                    sql = "SELECT COUNT(*) from (SELECT *  FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Emp='" + WebUser.No + "' AND REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd hh24:mi:ss')) > 0 ";
                    sql += "UNION SELECT * FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Emp='" + WebUser.No + "' AND REGEXP_LIKE(SDT, '^[0-9]{4}-[0-9]{2}-[0-9]{2}$') AND (sysdate - TO_DATE(SDT, 'yyyy-mm-dd')) > 0 )";


                }
                else if (SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                {
                    sql = "SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Emp='" + WebUser.No + "' AND  to_timestamp(CASE WHEN SDT='无' THEN '' ELSE SDT END, 'yyyy-mm-dd hh24:MI:SS') < NOW()";
                }
                else
                {
                    sql = "SELECT COUNT(*) FROM WF_GenerWorkerlist WHERE IsPass=0 AND FK_Emp='" + WebUser.No + "' AND  convert(varchar(100),SDT,120) < CONVERT(varchar(100), GETDATE(), 120)";
                }
                return DBAccess.RunSQLReturnValInt(sql);
            }
        }
        /// <summary>
        /// 在途的工作数量
        /// </summary>
        public static int Todolist_Runing
        {
            get
            {
                string sql;
                int state = (int)WFState.Runing;

                Paras ps = new Paras();
                ps.SQL = "SELECT count( distinct A.WorkID ) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND  A.WorkID=B.WorkID AND B.FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp AND B.IsEnable=1 AND (B.IsPass=1 OR B.IsPass<-1) AND A.WFState NOT IN ( 0, 1, 3 )";
                ps.Add("FK_Emp", WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps);
            }
        }
        /// <summary>
        /// 获取草稿箱流程数量
        /// </summary>
        public static int Todolist_Draft
        {
            get
            {
                /*获取数据.*/
                string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();
                ps.SQL = "SELECT count(a.WorkID ) as Num FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter";
                ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps);
            }
        }
        /// <summary>
        /// 会签的数量
        /// </summary>
        public static int Todolist_HuiQian
        {
            get
            {
                /*获取数据.*/
                string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();
                string sql = "SELECT count(*)";
                sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B ";
                sql += " WHERE A.WorkID=B.WorkID and a.FK_Node=b.FK_Node ";
                sql += " AND (B.IsPass=90 OR A.AtPara LIKE '%HuiQianZhuChiRen=" + WebUser.No + "%') ";
                sql += " AND B.FK_Emp=" + dbStr + "FK_Emp";
                //ps.SQL = "SELECT COUNT(workid) as Num FROM WF_GenerWorkerlist WHERE FK_Emp=" + dbStr + "FK_Emp AND IsPass=90";
                ps.SQL = sql;
                ps.Add(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps);
            }
        }
        /// <summary>
        /// 获取已经完成流程数量
        /// </summary>
        /// <returns></returns>
        public static int Todolist_Complete
        {
            get
            {
                /* 如果不是删除流程注册表. */
                Paras ps = new Paras();
                string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT count(WorkID) Num FROM WF_GenerWorkFlow WHERE (Emps LIKE '%@" + WebUser.No + "@%' OR Emps LIKE '%@" + WebUser.No + ",%' OR Emps LIKE '%," + WebUser.No + "@%') AND WFState=" + (int)WFState.Complete;
                return DBAccess.RunSQLReturnValInt(ps, 0);
            }
        }
        /// <summary>
        /// 我发起的流程在处理的工作数量
        /// </summary>
        public static int MyStart_Runing
        {
            get
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT COUNT(DISTINCT a.WorkID) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND A.TodoEmps  not like '%" + WebUser.No + ",%' AND A.Starter=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Starter AND B.FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND B.IsPass != -2 ";
                ps.Add("Starter", WebUser.No);
                ps.Add("FK_Emp", WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps);
            }
        }
        /// <summary>
        /// 我发起已完成的流程
        /// </summary>
        public static int MyStart_Complete
        {
            get
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT count(WorkID) Num FROM WF_GenerWorkFlow WHERE  Starter=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Starter AND WFState=" + (int)WFState.Complete;
                ps.Add("Starter", WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps, 0);
            }
        }
        /// <summary>
        /// 共享任务个数
        /// </summary>
        public static int Todolist_Sharing
        {
            get
            {
                Paras ps = new Paras();
                string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Sharing;
                string sql;
                string realSql = null;

                /*不是授权状态*/
                ps.SQL = "SELECT COUNT(WorkID) FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps);

            }
        }
        /// <summary>
        /// 申请下来的工作个数
        /// </summary>
        public static int Todolist_Apply
        {
            get
            {
                if (BP.WF.Glo.IsEnableTaskPool == false)
                    return 0;

                Paras ps = new Paras();
                string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Takeback;
                string sql;
                string realSql;
                /*不是授权状态*/
                ps.SQL = "SELECT COUNT(WorkID) FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps);
            }
        }
        #endregion 数量.

        #region 自动执行
        /// <summary>
        /// 处理延期的任务.根据节点属性的设置
        /// </summary>
        /// <returns>返回处理的消息</returns>
        public static string DTS_DealDeferredWork()
        {
            BP.WF.DTS.DTS_DealDeferredWork en = new BP.WF.DTS.DTS_DealDeferredWork();
            en.Do();

            return "执行成功..";
        }
        /// <summary>
        /// 自动执行开始节点数据
        /// 说明:根据自动执行的流程设置，自动启动发起的流程。
        /// 比如：您根据ccflow的自动启动流程的设置，自动启动该流程，不使用ccflow的提供的服务程序，您需要按如下步骤去做。
        /// 1, 写一个自动调度的程序。
        /// 2，根据自己的时间需要调用这个接口。
        /// </summary>
        /// <param name="fl">流程实体,您可以 new Flow(flowNo); 来传入.</param>
        public static void DTS_AutoStarterFlow(Flow fl)
        {
            #region 读取数据.
            BP.Sys.MapExt me = new BP.Sys.MapExt();
            int i = me.Retrieve(MapExtAttr.FK_MapData, "ND" + int.Parse(fl.No) + "01",
                MapExtAttr.ExtType, "PageLoadFull");
            if (i == 0)
            {
                BP.DA.Log.DebugWriteError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (DataType.IsNullOrEmpty(sql))
                {
                    continue;
                }

                string[] tempStrs = sql.Split('=');
                string dtlName = tempStrs[0];
                DataTable dtlTable = DBAccess.RunSQLReturnTable(sql.Replace(dtlName + "=", ""));
                dtlTable.TableName = dtlName;
                ds.Tables.Add(dtlTable);
            }
            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Columns.Contains("Starter") == false)
            {
                errMsg += "@配值的主表中没有Starter列.";
            }

            if (dtMain.Columns.Contains("MainPK") == false)
            {
                errMsg += "@配值的主表中没有MainPK列.";
            }

            if (errMsg.Length > 2)
            {
                BP.DA.Log.DebugWriteError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);
                return;
            }
            #endregion 检查数据源是否正确.

            #region 处理流程发起.

            string nodeTable = "ND" + int.Parse(fl.No) + "01";
            MapData md = new MapData(nodeTable);

            foreach (DataRow dr in dtMain.Rows)
            {
                string mainPK = dr["MainPK"].ToString();
                string sql = "SELECT OID FROM " + md.PTable + " WHERE MainPK='" + mainPK + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count != 0)
                    continue; /*说明已经调度过了*/

                string starter = dr["Starter"].ToString();
                if (BP.Web.WebUser.No != starter)
                {
                    BP.Web.WebUser.Exit();
                    BP.Port.Emp emp = new BP.Port.Emp();
                    emp.UserID = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        BP.DA.Log.DebugWriteInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + emp.UserID + "不存在。");
                        continue;
                    }

                    BP.Web.WebUser.SignInOfGener(emp);
                }

                #region  给值.
                Work wk = fl.NewWork();
                foreach (DataColumn dc in dtMain.Columns)
                {
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                }

                if (ds.Tables.Count != 0)
                {
                    string refPK = dr["MainPK"].ToString();
                    MapDtls dtls = wk.HisNode.MapData.MapDtls; // new MapDtls(nodeTable);
                    foreach (MapDtl dtl in dtls)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName != dtl.No)
                            {
                                continue;
                            }

                            //删除原来的数据。
                            GEDtl dtlEn = dtl.HisGEDtl;
                            dtlEn.Delete(GEDtlAttr.RefPK, wk.OID.ToString());

                            // 执行数据插入。
                            foreach (DataRow drDtl in dt.Rows)
                            {
                                if (drDtl["RefMainPK"].ToString() != refPK)
                                {
                                    continue;
                                }

                                dtlEn = dtl.HisGEDtl;

                                foreach (DataColumn dc in dt.Columns)
                                {
                                    dtlEn.SetValByKey(dc.ColumnName, drDtl[dc.ColumnName].ToString());
                                }

                                dtlEn.RefPK = wk.OID.ToString();
                                dtlEn.Insert();
                            }
                        }
                    }
                }
                #endregion  给值.

                // 处理发送信息.
                Node nd = fl.HisStartNode;
                try
                {
                    WorkNode wn = new WorkNode(wk, nd);
                    string msg = wn.NodeSend().ToMsgOfHtml();
                    //Log.DefaultLogWriteLineInfo(msg);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DebugWriteError(ex.Message);
                }
            }
            #endregion 处理流程发起.

        }
        #endregion

        #region 数据集合接口(如果您想获取一个结果集合的接口，都是以DB_开头的.)

        #region 与子流程相关.
        /// <summary>
        /// 获取流程事例的运行轨迹数据.
        /// 说明：使用这些数据可以生成流程的操作日志.
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <returns>GenerWorkFlows</returns>
        public static GenerWorkFlows DB_SubFlows(Int64 workid)
        {
            GenerWorkFlows gwf = new GenerWorkFlows();
            gwf.Retrieve(GenerWorkFlowAttr.PWorkID, workid);
            return gwf;
        }
        #endregion 获取流程事例的轨迹图

        #region 获取流程事例的轨迹图
        /// <summary>
        /// 获取流程时间轴，包含了分合流流程，父子流程，延续子流程
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public static DataTable DB_GenerTrackTable(string fk_flow, Int64 workid, Int64 fid, bool isWx = false)
        {
            string sqlOfWhere1 = "";
            DataTable dt = null;
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();

            if (fid == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                ps.Add("WorkID11", workid);
                ps.Add("WorkID12", workid);
            }
            else
            { //获取分合流的数据
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                ps.Add("FID11", fid);
                ps.Add("FID12", fid);
            }

            string sql = "";

            //@hongyan
            if (isWx)
            {
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    sql = "SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT FK_NodeText,NDTo,NDToT,EmpFrom,EmpFromT FK_EmpText,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
                    sql += " UNION SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
                    sql += " ORDER BY RDT ASC ";
                }
                else
                {
                    sql = "SELECT * From";
                    sql += "(SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT FK_NodeText,NDTo,NDToT,EmpFrom,EmpFromT FK_EmpText,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
                    sql += "UNION SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1 + ")";
                    sql += " AS Track  ORDER BY RDT ASC ";
                }

            }
            else
            {
                if (SystemConfig.AppCenterDBType == DBType.Oracle)
                {
                    sql = "SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
                    sql += " UNION SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
                    sql += "  ORDER BY RDT ASC ";
                }
                else
                {
                    sql = "SELECT * From";
                    sql += "(SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1;
                    sql += "UNION SELECT '' ExtField,MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1 + ")";
                    sql += " AS Track  ORDER BY RDT ASC ";

                }

            }

            ps.SQL = sql;


            try
            {
                dt = DBAccess.RunSQLReturnTable(ps);
            }
            catch
            {
                // 处理track表.
                Track.CreateOrRepairTrackTable(fk_flow);
                dt = DBAccess.RunSQLReturnTable(ps);
            }

            //把列名转化成区分大小写.
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["MYPK"].ColumnName = "MyPK";
                dt.Columns["ACTIONTYPE"].ColumnName = "ActionType";
                dt.Columns["ACTIONTYPETEXT"].ColumnName = "ActionTypeText";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["NDFROM"].ColumnName = "NDFrom";
                dt.Columns["NDFROMT"].ColumnName = "NDFromT";
                dt.Columns["NDTO"].ColumnName = "NDTo";
                dt.Columns["NDTOT"].ColumnName = "NDToT";
                dt.Columns["EMPFROM"].ColumnName = "EmpFrom";
                dt.Columns["EMPFROMT"].ColumnName = "EmpFromT";
                dt.Columns["EMPTO"].ColumnName = "EmpTo";
                dt.Columns["EMPTOT"].ColumnName = "EmpToT";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["WORKTIMESPAN"].ColumnName = "WorkTimeSpan";
                dt.Columns["MSG"].ColumnName = "Msg";
                dt.Columns["NODEDATA"].ColumnName = "NodeData";
                dt.Columns["EXER"].ColumnName = "Exer";
                dt.Columns["TAG"].ColumnName = "Tag";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["mypk"].ColumnName = "MyPK";
                dt.Columns["actiontype"].ColumnName = "ActionType";
                dt.Columns["actiontypetext"].ColumnName = "ActionTypeText";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["ndfrom"].ColumnName = "NDFrom";
                dt.Columns["ndfromt"].ColumnName = "NDFromT";
                dt.Columns["ndto"].ColumnName = "NDTo";
                dt.Columns["ndtot"].ColumnName = "NDToT";
                dt.Columns["empfrom"].ColumnName = "EmpFrom";
                dt.Columns["empfromt"].ColumnName = "EmpFromT";
                dt.Columns["empto"].ColumnName = "EmpTo";
                dt.Columns["emptot"].ColumnName = "EmpToT";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["worktimespan"].ColumnName = "WorkTimeSpan";
                dt.Columns["msg"].ColumnName = "Msg";
                dt.Columns["nodedata"].ColumnName = "NodeData";
                dt.Columns["exer"].ColumnName = "Exer";
                dt.Columns["tag"].ColumnName = "Tag";
            }

            //把track加入里面去.
            dt.TableName = "Track";
            return dt;
        }
        /// <summary>
        /// 获取一个流程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="userNo">操作员编号</param>
        /// <returns></returns>
        public static DataTable DB_GenerNDxxxRpt(string fk_flow, string userNo)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Rpt WHERE FlowStarter=" + dbstr + "FlowStarter  ORDER BY RDT";
            ps.Add(GERptAttr.FlowStarter, userNo);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion 获取流程事例的轨迹图

        #region 获取能够发送或者抄送人员的列表.
        /// <summary>
        /// 获取可以执行指定节点人的列表
        /// </summary>
        /// <param name="fk_node">节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns></returns>
        public static DataSet DB_CanExecSpecNodeEmps(int fk_node, Int64 workid)
        {
            DataSet ds = new DataSet();
            Paras ps = new Paras();
            ps.SQL = "SELECT No,Name,FK_Dept FROM Port_Emp ";
            DataTable dtEmp = DBAccess.RunSQLReturnTable(ps);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);

            ps = new Paras();
            ps.SQL = "SELECT No,Name FROM Port_Dept ";
            DataTable dtDept = DBAccess.RunSQLReturnTable(ps);
            dtDept.TableName = "Depts";
            ds.Tables.Add(dtDept);
            return ds;
        }
        /// <summary>
        /// 获得可以抄送的人员列表
        /// </summary>
        /// <param name="fk_node">节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns></returns>
        public static DataSet DB_CanCCSpecNodeEmps(int fk_node, Int64 workid)
        {
            DataSet ds = new DataSet();
            Paras ps = new Paras();
            ps.SQL = "SELECT No,Name,FK_Dept FROM Port_Emp ";
            DataTable dtEmp = DBAccess.RunSQLReturnTable(ps);
            dtEmp.TableName = "Emps";
            ds.Tables.Add(dtEmp);


            ps = new Paras();
            ps.SQL = "SELECT No,Name FROM Port_Dept ";
            DataTable dtDept = DBAccess.RunSQLReturnTable(ps);
            dtDept.TableName = "Depts";
            ds.Tables.Add(dtDept);

            return ds;
        }
        #endregion 获取能够发送或者抄送人员的列表.

        #region 获取操送列表
        /// <summary>
        /// 获取指定人员的抄送列表
        /// 说明:可以根据这个列表生成指定用户的抄送数据.
        /// </summary>
        /// <param name="domain">域.</param>
        /// <returns>返回该人员的所有抄送列表,结构同表WF_CCList.</returns>
        public static DataTable DB_CCList(string domain = null, string fk_flow = null)
        {
            string sqlWhere = "";
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                sqlWhere = " AND OrgNo='" + WebUser.OrgNo + "'";
            // 增加一个Sta列.
            Paras ps = new Paras();
            if (domain == null)
            {
                if (fk_flow == null)
                {
                    ps.SQL = "SELECT a.MyPK,A.Title,A.FK_Flow,A.FlowName,A.WorkID,A.Doc,A.Rec,A.RDT,A.FID,B.FK_Node,B.NodeName,B.WFSta,A.Sta FROM WF_CCList A, WF_GenerWorkFlow B WHERE A.CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo AND B.WorkID=A.WorkID AND A.Sta >=0  ";
                    ps.Add("CCTo", WebUser.No);
                }
                else
                {
                    ps.SQL = "SELECT a.MyPK,A.Title,A.FK_Flow,A.FlowName,A.WorkID,A.Doc,A.Rec,A.RDT,A.FID,B.FK_Node,B.NodeName,B.WFSta,A.Sta FROM WF_CCList A, WF_GenerWorkFlow B WHERE A.CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo  AND B.FK_Flow=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Flow AND B.WorkID=A.WorkID AND A.Sta >=0 ";
                    ps.Add("CCTo", WebUser.No);
                    ps.Add("FK_Flow", fk_flow);
                }
            }
            else
            {
                if (fk_flow == null)
                {
                    ps.SQL = "SELECT a.MyPK,A.Title,A.FK_Flow,A.FlowName,A.WorkID,A.Doc,A.Rec,A.RDT,A.FID,B.FK_Node,B.NodeName,B.WFSta,A.Sta FROM WF_CCList A, WF_GenerWorkFlow B WHERE A.CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo AND B.WorkID=A.WorkID AND B.Domain=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Domain AND A.Sta >=0 ";
                    ps.Add("CCTo", WebUser.No);
                    ps.Add("Domain", domain);
                }
                else
                {
                    ps.SQL = "SELECT a.MyPK,A.Title,A.FK_Flow,A.FlowName,A.WorkID,A.Doc,A.Rec,A.RDT,A.FID,B.FK_Node,B.NodeName,B.WFSta,A.Sta FROM WF_CCList A, WF_GenerWorkFlow B WHERE A.CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo AND B.WorkID=A.WorkID AND B.FK_Flow=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Flow AND B.Domain=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Domain AND A.Sta >=0 ";
                    ps.Add("CCTo", WebUser.No);
                    ps.Add("Domain", domain);
                    ps.Add("FK_Flow", fk_flow);
                }
            }
            //ps.SQL = ps.SQL + sqlWhere;//
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["MYPK"].ColumnName = "MyPK";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["DOC"].ColumnName = "DOC";
                dt.Columns["REC"].ColumnName = "REC";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["STA"].ColumnName = "Sta";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["mypk"].ColumnName = "MyPK";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["doc"].ColumnName = "DOC";
                dt.Columns["rec"].ColumnName = "REC";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["sta"].ColumnName = "Sta";
            }
            return dt;
        }
        public static DataTable DB_CCList(CCSta sta, string domain = null, string fk_flow = null)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            Paras ps = new Paras();
            if (domain == null)
            {
                if (fk_flow == null)
                {
                    ps.SQL = "SELECT * FROM WF_CCList WHERE Sta=" + dbStr + "Sta AND CCTo=" + dbStr + "CCTo  ORDER BY RDT DESC";
                    ps.Add("Sta", (int)sta);
                    ps.Add("CCTo", WebUser.No);
                }
                else
                {
                    ps.SQL = "SELECT * FROM WF_CCList WHERE Sta=" + dbStr + "Sta AND CCTo=" + dbStr + "CCTo  AND FK_Flow=" + dbStr + "FK_Flow  ORDER BY RDT DESC";
                    ps.Add("Sta", (int)sta);
                    ps.Add("CCTo", WebUser.No);
                    ps.Add("FK_Flow", fk_flow);

                }
            }
            else
            {
                if (fk_flow == null)
                {
                    ps.SQL = "SELECT * FROM WF_CCList WHERE Sta=" + dbStr + "Sta AND CCTo=" + dbStr + "CCTo AND Domain=" + dbStr + "Domain  ORDER BY RDT DESC";
                    ps.Add("Sta", (int)sta);
                    ps.Add("CCTo", WebUser.No);
                    ps.Add("Domain", domain);
                }
                else
                {
                    ps.SQL = "SELECT * FROM WF_CCList WHERE Sta=" + dbStr + "Sta AND CCTo=" + dbStr + "CCTo AND Domain=" + dbStr + "Domain AND FK_Flow=" + dbStr + "FK_Flow  ORDER BY RDT DESC";
                    ps.Add("Sta", (int)sta);
                    ps.Add("CCTo", WebUser.No);
                    ps.Add("Domain", domain);
                    ps.Add("FK_Flow", fk_flow);
                }
            }

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["MYPK"].ColumnName = "MyPK";
                dt.Columns["STA"].ColumnName = "Sta";

                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["DOC"].ColumnName = "DOC";
                dt.Columns["REC"].ColumnName = "REC";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["FID"].ColumnName = "FID";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["mypk"].ColumnName = "MyPK";
                dt.Columns["sta"].ColumnName = "Sta";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["doc"].ColumnName = "DOC";
                dt.Columns["rec"].ColumnName = "REC";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["fid"].ColumnName = "FID";
            }
            return dt;
        }
        /// <summary>
        /// 获取指定人员的抄送列表(未读)
        /// </summary>
        /// <param name="FK_Emp">人员编号,如果是null,则返回所有的.</param>
        /// <returns>返回该人员的未读的抄送列表</returns>
        public static DataTable DB_CCList_UnRead(string FK_Emp, string domain = null, string fk_flow = null)
        {
            return DB_CCList(CCSta.UnRead, domain, fk_flow);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已读)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已读的抄送列表</returns>
        public static DataTable DB_CCList_Read(string domain = null, string fk_flow = null)
        {
            return DB_CCList(CCSta.Read, domain, fk_flow);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已删除)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已删除的抄送列表</returns>
        public static DataTable DB_CCList_Delete(string domain, string fk_flow = null)
        {
            return DB_CCList(CCSta.Del, domain, fk_flow);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已回复)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已删除的抄送列表</returns>
        public static DataTable DB_CCList_CheckOver(string domain)
        {
            return DB_CCList(CCSta.CheckOver, domain);
        }
        #endregion

        #region 获取当前操作员可以发起的流程集合
        /// <summary>
        /// 获取指定人员能够发起流程的集合.
        /// 说明:利用此接口可以生成用户的发起的流程列表.
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <returns>BP.WF.Flows 可发起的流程对象集合,如何使用该方法形成发起工作列表,请参考:\WF\UC\Start.ascx</returns>
        public static Flows DB_GenerCanStartFlowsOfEntities(string userNo)
        {
            string sql = "";
            // 采用新算法.
            sql = "SELECT FK_Flow FROM V_FlowStarterBPM WHERE FK_Emp='" + userNo + "'";

            Flows fls = new Flows();
            BP.En.QueryObject qo = new BP.En.QueryObject(fls);
            qo.AddWhereInSQL("No", sql);
            qo.addAnd();
            qo.AddWhere(FlowAttr.IsCanStart, true);
            qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
            qo.DoQuery();
            return fls;

        }
        /// <summary>
        /// 获得指定人的流程发起列表
        /// </summary>
        /// <param name="userNo">发起人编号</param>
        /// <returns></returns>
        public static DataTable DB_StarFlows(string userNo, string domain = null)
        {
            DataTable dt = DB_Start(userNo, domain);
            DataView dv = new DataView(dt);
            dv.Sort = "Idx";
            return dv.Table;
        }
        public static DataTable DB_Start(string userNo = null, string domain = null)
        {
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = WebUser.UserID;

            //任何人都可以启动.
            string sql0 = "";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
            {
                sql0 = "SELECT A.FK_Flow FROM WF_Node A ";
                sql0 += " WHERE  NodePosType=0  AND DeliveryWay=4  ";
            }
            else
            {
                sql0 = "SELECT A.FK_Flow FROM WF_Node A, WF_Flow B ";
                sql0 += " WHERE A.NodePosType=0  AND A.DeliveryWay=4 AND A.FK_Flow=B.No ";
                sql0 += " AND B.OrgNo='" + BP.Web.WebUser.OrgNo + "'";
            }

            //按岗位计算.
            string sql1 = "SELECT A.FK_Flow FROM WF_Node A, WF_NodeStation B, Port_DeptEmpStation C";
            sql1 += " WHERE  A.NodePosType=0  AND A.NodeID=B.FK_Node AND B.FK_Station=C.FK_Station ";
            sql1 += "  AND (A.DeliveryWay=0 OR A.DeliveryWay=14) ";
            sql1 += "  AND C.FK_Emp='" + userNo + "'";

            //按部门计算.
            string sql2 = "SELECT A.FK_Flow FROM WF_Node A, WF_NodeDept B, Port_DeptEmp C ";
            sql2 += " WHERE  A.NodePosType=0  AND A.NodeID=B.FK_Node AND B.FK_Dept=C.FK_Dept ";
            sql2 += "  AND A.DeliveryWay=1 ";
            sql2 += "  AND C.FK_Emp='" + userNo + "' ";

            //按人员计算.
            string sql3 = "SELECT A.FK_Flow FROM WF_Node A, WF_NodeEmp B ";
            sql3 += " WHERE  A.NodePosType=0  AND  A.NodeID=B.FK_Node ";
            sql3 += "  AND A.DeliveryWay=3";
            sql3 += "  AND B.FK_Emp='" + userNo + "' ";

            // 按照岗位与部门的交集计算.
            string sql4 = "SELECT A.FK_Flow FROM WF_Node A, WF_NodeDept B, WF_NodeStation C, Port_DeptEmpStation D ";
            sql4 += " WHERE  A.NodePosType=0 AND A.NodeID=B.FK_Node AND A.NodeID=C.FK_Node ";
            sql4 += "  AND A.DeliveryWay=9 ";
            sql4 += "  AND B.FK_Dept =D.FK_Dept ";
            sql4 += "  AND C.FK_Station =D.FK_Station ";
            sql4 += "  AND D.FK_Emp='" + userNo + "' ";

            // 按照设置的组织计算. WF_FlowOrg
            string sql5 = "";
            string sqlUnion = "";
            sqlUnion += sql0 + "  UNION ";
            sqlUnion += sql1 + " UNION ";
            sqlUnion += sql2 + " UNION ";
            sqlUnion += sql3 + " UNION ";
            sqlUnion += sql4 + "  ";

            string sql = "SELECT A.No,A.Name,A.IsBatchStart,A.FK_FlowSort,B.Name AS FK_FlowSortText,B.Domain,A.IsStartInMobile,A.Idx,A.WorkModel";
            sql += " FROM WF_Flow A, WF_FlowSort B ";
            sql += " WHERE A.FK_FlowSort=B.No AND A.No IN (" + sqlUnion + ")";

            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                sql += " AND ( B.OrgNo='" + WebUser.OrgNo + "' )";

            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                sql += " AND A.OrgNo='" + WebUser.OrgNo + "'";

            sql += " ORDER BY B.Idx, A.Idx";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return dt;

            // string sql
            //  string view = "T" + WebUser.No;
            //  string sqlView = " CREATE VIEW T(FK_Flow, FlowName, FK_Emp, OrgNo) AS " + sql;
            //创建临时视图..
            //DBAccess.RunSQL(sql);

            //            SELECT
            //    A.FK_Flow,
            //	A.FlowName,
            //	C.No AS FK_Emp,
            //	B.OrgNo
            //FROM

            //    WF_Node A,
            //    WF_FlowOrg B,
            //	Port_Emp C
            //WHERE

            //    A.FK_Flow = B.FlowNo

            //     AND((B.OrgNo = C.OrgNo) OR((B.OrgNo IS NULL) AND(C.OrgNo IS NULL)))
            //	AND(
            //    A.DeliveryWay = 22
            //    OR A.DeliveryWay = 51)

        }
        public static DataTable DB_Start11(string userNo = null, string domain = null)
        {
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = WebUser.UserID;

            //查询出来所有的不控制的流程.
            string sql = "SELECT A.No,A.Name,A.IsBatchStart,A.FK_FlowSort,A.FK_FlowSortText,A.IsStartInMobile,A.WorkModel,A.Idx ";
            sql += " FROM WF_Flow A, WF_Flow B WHERE A.No=B.FK_Flow AND B.NodePosType=0 AND B.DeliveryWay=1 ";
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sql += " A.OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            if (DataType.IsNullOrEmpty(domain) == false)
                sql += "  AND A.DoMain='" + domain + "'";
            sql += " ORDER BY A.Idx ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);


            return dt;
        }
        /// <summary>
        /// 获得一个人可以发起的流程列表 
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable DB_Start_Old(string userNo = null, string domain = null)
        {
            if (DataType.IsNullOrEmpty(userNo) == true)
                userNo = WebUser.UserID;

            string sqlEnd = "";
            if (DataType.IsNullOrEmpty(domain) == false)
                sqlEnd = "  AND C.DoMain='" + domain + "'";

            // 组成查询的sql. .sql部分有变动.
            string sql = "SELECT A.No,A.Name,a.IsBatchStart,a.FK_FlowSort,C.Name AS FK_FlowSortText,C.Domain,A.IsStartInMobile, A.Idx,A.WorkModel";
            sql += " FROM WF_Flow A, V_FlowStarterBPM B, WF_FlowSort C  ";

            sql += " WHERE A.No=B.FK_Flow AND A.IsCanStart=1 AND A.FK_FlowSort=C.No  AND B.FK_Emp='" + userNo + "' " + sqlEnd;

            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                sql += " AND ( B.OrgNo='" + WebUser.OrgNo + "' )";

            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                sql += " AND A.OrgNo='" + WebUser.OrgNo + "'";

            sql += " ORDER BY C.Idx, A.Idx";

            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                sql = "(" + sql + ") UNION ";
                String sqlTable = " (SELECT A.FK_Flow AS FK_Flow,A.FlowName As FlowName,C.No AS FK_Emp,B.OrgNo AS OrgNo From WF_Node A,WF_FlowOrg B,Port_Emp C WHERE A.FK_Flow=B.FlowNo AND A.DeliveryWay = 22 AND B.OrgNo='" + WebUser.OrgNo + "') B";
                sql += " (SELECT A.No,A.Name,a.IsBatchStart,a.FK_FlowSort,C.Name AS FK_FlowSortText,C.Domain,A.IsStartInMobile, A.Idx,A.WorkModel";
                sql += " FROM WF_Flow A, " + sqlTable + " , WF_FlowSort C  ";

                sql += " WHERE A.No=B.FK_Flow AND A.IsCanStart=1 AND A.FK_FlowSort=C.No  AND B.FK_Emp='" + userNo + "'  ORDER BY C.Idx, A.Idx)  ";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["ISBATCHSTART"].ColumnName = "IsBatchStart";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["FK_FLOWSORTTEXT"].ColumnName = "FK_FlowSortText";
                dt.Columns["ISSTARTINMOBILE"].ColumnName = "IsStartInMobile";
                dt.Columns["IDX"].ColumnName = "Idx";
                dt.Columns["WORKMODEL"].ColumnName = "WorkModel";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["no"].ColumnName = "No";
                dt.Columns["name"].ColumnName = "Name";
                dt.Columns["isbatchstart"].ColumnName = "IsBatchStart";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["fk_flowsorttext"].ColumnName = "FK_FlowSortText";
                dt.Columns["isstartinmobile"].ColumnName = "IsStartInMobile";
                dt.Columns["idx"].ColumnName = "Idx";
                dt.Columns["workmodel"].ColumnName = "WorkModel";

            }
            return dt;
        }
        public static DataTable DB_GenerCanStartFlowsTree(string userNo)
        {
            //发起.
            DataTable table = DB_Start(userNo);
            table.Columns.Add("ParentNo");
            table.Columns.Add("ICON");
            string flowSort = string.Format("select No,Name,ParentNo from WF_FlowSort");

            DataTable sortTable = DBAccess.RunSQLReturnTable(flowSort);
            foreach (DataRow row in sortTable.Rows)
            {
                DataRow newRow = table.NewRow();
                newRow["No"] = row["No"];
                newRow["Name"] = row["Name"];
                newRow["ParentNo"] = row["ParentNo"];
                newRow["ICON"] = "icon-tree_folder";
                table.Rows.Add(newRow);
            }

            foreach (DataRow row in table.Rows)
            {
                if (DataType.IsNullOrEmpty(row["ParentNo"].ToString()))
                {
                    row["ParentNo"] = row["FK_FlowSort"];
                }
                if (DataType.IsNullOrEmpty(row["ICON"].ToString()))
                {
                    row["ICON"] = "icon-4";
                }
            }
            return table;
        }


        /// <summary>
        /// 获取(同表单)合流点上的子线程
        /// 说明:如果您要想在合流点看到所有的子线程运行的状态.
        /// </summary>
        /// <param name="nodeIDOfHL">合流点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>与表WF_GenerWorkerlist结构类同的datatable.</returns>
        public static DataTable DB_GenerHLSubFlowDtl_TB(int nodeIDOfHL, Int64 workid)
        {
            Node nd = new Node(nodeIDOfHL);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            GenerWorkerLists wls = new GenerWorkerLists();
            QueryObject qo = new QueryObject(wls);
            qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FK_Node,
                nd.FromNodes[0].GetValByKey(NodeAttr.NodeID));

            DataTable dt = qo.DoQueryToTable();
            if (dt.Rows.Count == 1)
            {
                qo.clear();
                qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
                return qo.DoQueryToTable();
            }
            return dt;
        }
        /// <summary>
        /// 获取(异表单)合流点上的子线程
        /// </summary>
        /// <param name="nodeIDOfHL">合流点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>与表WF_GenerWorkerlist结构类同的datatable.</returns>
        public static DataTable DB_GenerHLSubFlowDtl_YB(int nodeIDOfHL, Int64 workid)
        {
            Node nd = new Node(nodeIDOfHL);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            GenerWorkerLists wls = new GenerWorkerLists();
            QueryObject qo = new QueryObject(wls);
            qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsPass, 0);
            return qo.DoQueryToTable();
        }
        #endregion 获取当前操作员可以发起的流程集合

        #region 流程草稿
        /// <summary>
        /// 获取当前操作员的指定流程的流程草稿数据
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>返回草稿数据集合,列信息. OID=工作ID,Title=标题,RDT=记录日期,FK_Flow=流程编号,FID=流程ID, FK_Node=节点ID</returns>
        public static DataTable DB_GenerDraftDataTable(string flowNo = null, string domain = null)
        {
            /*获取数据.*/
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (DataType.IsNullOrEmpty(domain) == true)
            {
                if (flowNo == null)
                {
                    ps.SQL = "SELECT WorkID,Title,FK_Flow,FlowName,RDT,FlowNote,AtPara,FK_Node,FID FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter ORDER BY RDT";
                    ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                }
                else
                {
                    ps.SQL = "SELECT WorkID,Title,FK_Flow,FlowName,RDT,FlowNote,AtPara,FK_Node,FID FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter AND FK_Flow=" + dbStr + "FK_Flow ORDER BY RDT";
                    ps.Add(GenerWorkFlowAttr.FK_Flow, flowNo);
                    ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                }

            }
            else
            {
                if (flowNo == null)
                {
                    ps.SQL = "SELECT WorkID,Title,FK_Flow,FlowName,RDT,FlowNote,AtPara,FK_Node,FID FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter AND Domain=" + dbStr + "Domain ORDER BY RDT";
                    ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                    ps.Add(GenerWorkFlowAttr.Domain, domain);
                }
                else
                {
                    ps.SQL = "SELECT WorkID,Title,FK_Flow,FlowName,RDT,FlowNote,AtPara,FK_Node,FID FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter AND FK_Flow=" + dbStr + "FK_Flow AND Domain=" + dbStr + "Domain ORDER BY RDT";
                    ps.Add(GenerWorkFlowAttr.FK_Flow, flowNo);
                    ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                    ps.Add(GenerWorkFlowAttr.Domain, domain);
                }
            }

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["atpara"].ColumnName = "AtPara";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
            }
            return dt;
        }
        #endregion 流程草稿

        #region 我关注的流程
        /// <summary>
        /// 获得我关注的流程列表
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">操作员编号</param>
        /// <param name="domain">域</param>
        /// <returns>返回当前关注的流程列表.</returns>
        public static DataTable DB_Focus(string flowNo = null, string userNo = null, string domain = null)
        {
            if (flowNo == "")
                flowNo = null;

            if (userNo == null)
                userNo = BP.Web.WebUser.No;

            //if (domain == null)
            //    domain = ""; 

            //执行sql.
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE AtPara LIKE  '%F_" + userNo + "=1%'";
            if (flowNo != null)
            {
                ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE AtPara LIKE  '%F_" + userNo + "=1%' AND FK_Flow=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Flow";
                ps.Add("FK_Flow", flowNo);
            }
            if (DataType.IsNullOrEmpty(domain) == false && DataType.IsNullOrEmpty(flowNo) == false)
            {
                ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE AtPara LIKE  '%F_" + userNo + "=1%' AND FK_Flow=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Flow AND  Domain=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Domain";
                ps.Add("FK_Flow", flowNo);
                ps.Add("Domain", domain);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            //添加oracle的处理
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["FK_NY"].ColumnName = "FK_NY";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PNODEID"].ColumnName = "PNodeID";
                dt.Columns["PFID"].ColumnName = "PFID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["GUID"].ColumnName = "GUID";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
                dt.Columns["TSPAN"].ColumnName = "TSpan";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";

                // dt.Columns["CFLOWNO"].ColumnName = "CFlowNo";
                // dt.Columns["CWORKID"].ColumnName = "CWorkID";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["billno"].ColumnName = "BillNo";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["fk_ny"].ColumnName = "FK_NY";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["sdtofflow"].ColumnName = "SDTOfFlow";
                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pnodeid"].ColumnName = "PNodeID";
                dt.Columns["pfid"].ColumnName = "PFID";
                dt.Columns["pemp"].ColumnName = "PEmp";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["atpara"].ColumnName = "AtPara";
                dt.Columns["emps"].ColumnName = "Emps";
                dt.Columns["guid"].ColumnName = "GUID";
                dt.Columns["weeknum"].ColumnName = "WeekNum";
                dt.Columns["tspan"].ColumnName = "TSpan";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["systype"].ColumnName = "SysType";

                // dt.Columns["CFLOWNO"].ColumnName = "CFlowNo";
                // dt.Columns["CWORKID"].ColumnName = "CWorkID";
            }
            return dt;
        }
        #endregion 我关注的流程

        #region 获取当前操作员的共享工作
        /// <summary>
        /// 获得待办
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <param name="fk_node">节点ID</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="domain">域</param>
        /// <returns></returns>
        public static DataTable DB_Todolist(string userNo, int fk_node = 0, string flowNo = null,
            string domain = null)
        {
            string sql = "";
            sql = "SELECT A.* FROM WF_GenerWorkFlow A, WF_FlowSort B, WF_Flow C, WF_GenerWorkerlist D ";
            sql += " WHERE (WFState=2 OR WFState=5 OR WFState=8)";
            sql += " AND A.FK_FlowSort=B.No ";
            sql += " AND A.FK_Flow=C.No ";
            sql += " AND A.FK_Node=D.FK_Node ";
            sql += " AND A.WorkID=D.WorkID ";
            sql += " AND D.IsPass=0  ";  // = 90 是会签主持人.
            sql += " AND D.FK_Emp='" + userNo + "'";

            //节点ID.
            if (fk_node != 0)
                sql += " AND A.FK_Node=" + fk_node;

            //流程编号.
            if (flowNo != null)
                sql += " AND C.No='" + flowNo + "'";

            //域.
            if (domain != null)
                sql += " AND B.Domain='" + domain + "'";


            //  sql += "  ORDER BY  B.Idx, C.Idx, A.RDT DESC ";
            sql += "  ORDER BY  A.RDT DESC ";


            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //添加oracle的处理
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["TITLE"].ColumnName = "Title";
                // dt.Columns["ISREAD"].ColumnName = "IsRead";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["title"].ColumnName = "Title";
                // dt.Columns["ISREAD"].ColumnName = "IsRead";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["billno"].ColumnName = "BillNo";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["atpara"].ColumnName = "AtPara";
            }

            return dt;
        }
        /// <summary>
        /// 获取当前人员待处理的工作
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="fk_node">指定的节点，如果为0就是所有的节点.</param>
        /// <param name="showWhat">WFState状态，=5退回的，=2是正在运行的.</param>
        /// <param name="domain">域名</param>
        /// <returns>返回待办WF_EmpWorks的视图待办.</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable(string userNo,
            int nodeID = 0, string wfstate = null, string domain = null, string flowNo = null, string orderBy = "ADT")
        {
            if (DataType.IsNullOrEmpty(userNo) == true)
                throw new Exception("err@登录信息丢失.");
            if (DataType.IsNullOrEmpty(orderBy) == true)
                orderBy = "ADT";
            string whereSQL = " ";

            if (DataType.IsNullOrEmpty(domain) == false)
                whereSQL = " AND A.Domain='" + domain + "'";

            if (DataType.IsNullOrEmpty(wfstate) == false)
                whereSQL = " AND A.WFState='" + wfstate + "'";

            if (nodeID != 0)
                whereSQL = " AND A.FK_Node='" + nodeID + "'";

            if (flowNo != null)
                whereSQL = " AND A.FK_Flow='" + flowNo + "'";

            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                whereSQL = " AND A.OrgNo='" + WebUser.OrgNo + "'";

            //获得授权信息.
            Auths aths = new Auths();
            aths.Retrieve(AuthAttr.AutherToEmpNo, userNo);

            string sql;
            /*不是授权状态*/
            if (BP.Difference.SystemConfig.CustomerNo == "TianYe")
            {
                if (BP.WF.Glo.IsEnableTaskPool == true)
                {
                    sql = "SELECT A.* FROM BPM.WF_EmpWorks A, BPM.WF_Flow B, BPM.WF_FlowSort C WHERE A.FK_Flow=B.No AND B.FK_FlowSort=C.No AND A.FK_Emp='" + userNo + "' AND A.TaskSta=0 " + whereSQL + " ORDER BY C.Idx, B.Idx, ADT DESC ";
                }
                else
                {
                    sql = "SELECT A.* FROM BPM.WF_EmpWorks A, BPM.WF_Flow B, BPM.WF_FlowSort C WHERE A.FK_Flow=B.No AND B.FK_FlowSort=C.No AND A.FK_Emp='" + userNo + "'  " + whereSQL + " ORDER BY C.Idx,B.Idx, A.ADT DESC ";
                }
            }
            else
            {
                if (BP.WF.Glo.IsEnableTaskPool == true)
                    sql = "SELECT  A.*, null as Auther FROM WF_EmpWorks A WHERE  TaskSta=0 AND A.FK_Emp='" + userNo + "' " + whereSQL + "";
                else
                    sql = "SELECT  A.*, null as Auther FROM WF_EmpWorks A WHERE  A.FK_Emp='" + userNo + "' " + whereSQL + "";

                foreach (Auth ath in aths)
                {

                    string todata = ath.TakeBackDT.Replace("-", "");
                    if (DataType.IsNullOrEmpty(ath.TakeBackDT) == false)
                    {
                        int mydt = int.Parse(todata);
                        int nodt = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                        if (mydt < nodt)
                            continue;
                        sql += " UNION ";

                        if (ath.AuthType == AuthorWay.SpecFlows)
                            sql += "SELECT A.*,'" + ath.Auther + "' as Auther FROM WF_EmpWorks A WHERE  FK_Emp='" + ath.Auther + "' AND FK_Flow='" + ath.FlowNo + "' " + whereSQL + "";
                        else
                            sql += "SELECT A.*,'" + ath.Auther + "' as Auther FROM WF_EmpWorks A WHERE  FK_Emp='" + ath.Auther + "' " + whereSQL + "";


                    }
                }
                //如果按照流程顺序排序
                if (orderBy.Equals("FlowIdx"))
                    sql += " ORDER BY FlowSortIdx, FlowIdx ASC";
                else
                    sql += " ORDER BY " + orderBy + " DESC";

            }

            //获得待办.
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //添加oracle的处理
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["ISREAD"].ColumnName = "IsRead";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                //dt.Columns["WORKERDEPT"].ColumnName = "WorkerDept";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["ADT"].ColumnName = "ADT";
                dt.Columns["SDT"].ColumnName = "SDT";
                dt.Columns["FK_EMP"].ColumnName = "FK_Emp";
                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["PRESSTIMES"].ColumnName = "PressTimes";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                //dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["LISTTYPE"].ColumnName = "ListType";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["isread"].ColumnName = "IsRead";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                //  dt.Columns["workerdept"].ColumnName = "WorkerDept";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["adt"].ColumnName = "ADT";
                dt.Columns["sdt"].ColumnName = "SDT";
                dt.Columns["fk_emp"].ColumnName = "FK_Emp";
                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["presstimes"].ColumnName = "PressTimes";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["billno"].ColumnName = "BillNo";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["listtype"].ColumnName = "ListType";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["atpara"].ColumnName = "AtPara";

            }
            return dt;
        }
        /// <summary>
        /// 近期发起的工作.
        /// </summary>
        /// <param name="flowNo"></param>
        /// <returns></returns>
        public static DataTable DB_RecentStart(string flowNo = null)
        {
            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            qo.Top = 300;
            if (DataType.IsNullOrEmpty(flowNo) == false)
            {
                qo.AddWhere(GenerWorkFlowAttr.FK_Flow, flowNo);
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                qo.DoQuery();
            }
            else
            {
                qo.AddWhere(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
                qo.DoQuery();
            }
            return gwfs.ToDataTableField();
        }
        /// <summary>
        /// 超时工作
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable DB_Timeout(string userNo = null)
        {
            if (userNo == null)
                userNo = BP.Web.WebUser.No;

            string sql = "SELECT A.Title,A.WorkID, A.FK_Flow as FlowNo, A.FlowName,A.FK_Node as NodeID,A.NodeName,";
            sql += " A.Starter,A.StarterName,A.PRI,B.SDT,B.IsRead,A.WFState,A.RDT,A.BillNo ";
            sql += " FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B ";
            sql += " WHERE A.WorkID = B.WorkID AND A.FK_Node = B.FK_Node AND B.SDT >= '" + DataType.CurrentDate + (SystemConfig.AppCenterDBType == DBType.MSSQL ? "' AND LEN(B.SDT) > 8" : "' AND LENGTH(B.SDT) > 8 ");
            sql += " AND B.FK_Emp ='" + userNo + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //添加oracle的处理
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FLOWNO"].ColumnName = "FlowNo";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NODEID"].ColumnName = "NodeID";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["PRI"].ColumnName = "PRI";
                dt.Columns["ISREAD"].ColumnName = "IsRead";
                dt.Columns["SDT"].ColumnName = "SDT";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["RDT"].ColumnName = "RDT";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["flowno"].ColumnName = "FlowNo";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["nodeid"].ColumnName = "NodeID";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["pri"].ColumnName = "PRI";
                dt.Columns["isread"].ColumnName = "IsRead";
                dt.Columns["sdt"].ColumnName = "SDT";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["rdt"].ColumnName = "RDT";
                dt.Columns["billno"].ColumnName = "BillNo";
            }
            return dt;
        }
        /// <summary>
        /// 获得待办(包括被授权的待办)
        /// 区分是自己的待办，还是被授权的待办通过数据源的 FK_Emp 字段来区分。
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_Todolist(string userNo = null)
        {
            if (userNo == null)
            {
                userNo = BP.Web.WebUser.No;
                if (WebUser.IsAuthorize == false)
                    throw new Exception("@授权登录的模式下不能调用此接口.");
            }

            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            //string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + "  OR WFState=" + (int)WFState.AskForReplay + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + " OR WFState=" + (int)WFState.Fix;
            string wfSql = "  1=1  ";

            /*不是授权状态*/
            if (BP.WF.Glo.IsEnableTaskPool == true)
            {
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1 ";
            }
            else
            {
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
            }

            ps.Add("FK_Emp", userNo);

            //获取授权给他的人员列表.
            Auths aths = new Auths();
            aths.Retrieve(AuthAttr.Auther, userNo);
            foreach (Auth auth in aths)
            {
                //判断是否授权到期.
                if (auth.TakeBackDT == "")
                    continue;

                switch (auth.AuthType)
                {
                    case AuthorWay.All:
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                        {
                            ps.SQL += " UNION  SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + auth.Auther + "' AND TaskSta!=1  ";
                        }
                        else
                        {
                            ps.SQL += " UNION  SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + auth.Auther + "' ";
                        }
                        break;
                    case AuthorWay.SpecFlows:
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                        {
                            ps.SQL += " UNION SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + auth.Auther + "' AND  FK_Flow = '" + auth.FlowNo + "' AND TaskSta!=0 ";
                        }
                        else
                        {
                            ps.SQL += " UNION SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + auth.Auther + "' AND  FK_Flow = '" + auth.FlowNo + "'  ";
                        }
                        break;
                    case AuthorWay.None: //非授权状态下.
                        continue;
                    default:
                        throw new Exception("no such way...");
                }
            }
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获得已完成数据统计列表
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <returns>具有FlowNo,FlowName,Num三个列的指定人员的待办列表</returns>
        public static DataTable DB_FlowCompleteGroup(string userNo)
        {
            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT FK_Flow as No,FlowName,COUNT(*) Num FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + userNo + "@%' AND FID=0 AND WFState=" + (int)WFState.Complete + " GROUP BY FK_Flow,FlowName";
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取指定页面已经完成流程
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="pageSize">每页的数量</param>
        /// <param name="pageIdx">第几页</param>
        /// <returns>用户编号</returns>
        public static DataTable DB_FlowComplete(string userNo, string flowNo, int pageSize, int pageIdx)
        {
            /* 如果不是删除流程注册表. */
            GenerWorkFlows ens = new GenerWorkFlows();
            QueryObject qo = new QueryObject(ens);
            if (flowNo != null)
            {
                qo.AddWhere(GenerWorkFlowAttr.FK_Flow, flowNo);
                qo.addAnd();
            }
            qo.AddWhere(GenerWorkFlowAttr.FID, 0);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.WFState, (int)WFState.Complete);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", " '%@" + userNo + "@%'");
            /**小周鹏修改-----------------------------START**/
            // qo.DoQuery(GenerWorkFlowAttr.WorkID,pageSize, pageIdx);
            qo.DoQuery(GenerWorkFlowAttr.WorkID, pageSize, pageIdx, GenerWorkFlowAttr.RDT, true);
            /**小周鹏修改-----------------------------END**/
            return ens.ToDataTableField();
        }
        /// <summary>
        /// 查询指定流程中已完成的流程
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="pageCount"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIdx"></param>
        /// <param name="strFlow"></param>
        /// <returns></returns>
        public static DataTable DB_FlowComplete(string userNo, int pageCount, int pageSize, int pageIdx, string strFlow)
        {
            /* 如果不是删除流程注册表. */
            GenerWorkFlows ens = new GenerWorkFlows();
            QueryObject qo = new QueryObject(ens);
            qo.AddWhere(GenerWorkFlowAttr.FID, 0);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.WFState, (int)WFState.Complete);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.Emps, " LIKE ", " '%@" + userNo + "@%'");
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, strFlow);
            qo.DoQuery(GenerWorkFlowAttr.WorkID, pageSize, pageIdx);
            return ens.ToDataTableField();
        }
        /// <summary>
        /// 查询指定流程中已完成的公告流程
        /// </summary>
        /// <param name="pageCount">页数</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIdx">页码</param>
        /// <param name="strFlow">流程编号</param>
        /// <returns></returns>
        public static DataTable DB_FlowComplete(string strFlow, int pageSize, int pageIdx)
        {

            /* 如果不是删除流程注册表. */
            GenerWorkFlows ens = new GenerWorkFlows();
            QueryObject qo = new QueryObject(ens);
            qo.AddWhere(GenerWorkFlowAttr.FID, 0);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.WFState, (int)WFState.Complete);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.FK_Flow, strFlow);
            qo.DoQuery(GenerWorkFlowAttr.WorkID, pageSize, pageIdx);
            return ens.ToDataTableField();

        }

        /// <summary>
        /// 获取某一个人已完成的流程
        /// </summary>
        /// <param name="userNo">用户编码</param>
        /// <param name="isMyStart">是否是用户发起的</param>
        /// <returns></returns>
        public static DataTable DB_FlowComplete(string userNo, bool isMyStart = false)
        {
            DataTable dt = null;
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            if (isMyStart == true)
            {
                ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE T.Starter=" + dbstr + "Starter AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
                ps.Add("Starter", userNo);
            }
            else
                ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE (T.Emps LIKE '%@" + userNo + "@%' OR  T.Emps LIKE '%@" + userNo + ",%') AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";

            dt = DBAccess.RunSQLReturnTable(ps);

            //需要翻译.
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["TYPE"].ColumnName = "Type";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["PFLOWNO"].ColumnName = "PflowNo";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PNODEID"].ColumnName = "PNodeID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["DOMAIN"].ColumnName = "Domain";
                dt.Columns["SENDDT"].ColumnName = "SendDT";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["type"].ColumnName = "Type";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["sdtofflow"].ColumnName = "SDTOfFlow";
                dt.Columns["pflowno"].ColumnName = "PflowNo";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pnodeid"].ColumnName = "PNodeID";
                dt.Columns["pemp"].ColumnName = "PEmp";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["billno"].ColumnName = "BillNo";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["atpara"].ColumnName = "AtPara";
                dt.Columns["emps"].ColumnName = "Emps";
                dt.Columns["domain"].ColumnName = "Domain";
                dt.Columns["senddt"].ColumnName = "SendDT";
                dt.Columns["weeknum"].ColumnName = "WeekNum";
            }
            return dt;
        }
        /// <summary>
        /// 获取已经完成流程
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_TongJi_FlowComplete()
        {
            DataTable dt = null;

            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT T.FK_Flow, T.FlowName, COUNT(T.WorkID) as Num FROM WF_GenerWorkFlow T WHERE (T.Emps LIKE '%@" + WebUser.No + "@%' OR  T.Emps LIKE '%@" + WebUser.No + ",%') AND T.FID=0 AND T.WFSta=" + (int)WFSta.Complete + " GROUP BY T.FK_Flow,T.FlowName";
            dt = DBAccess.RunSQLReturnTable(ps);

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NUM"].ColumnName = "Num";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["num"].ColumnName = "Num";
            }

            return dt;

        }
        /// <summary>
        /// 获取已经完成流程
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_FlowComplete()
        {
            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE (T.Emps LIKE '%@" + WebUser.No + "@%' OR T.Emps LIKE '%@" + WebUser.No + ",%') AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            //需要翻译.
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["TYPE"].ColumnName = "Type";
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";
                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["TITLE"].ColumnName = "Title";
                dt.Columns["WFSTA"].ColumnName = "WFSta";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["SDTOFFLOW"].ColumnName = "SDTOfFlow";
                dt.Columns["PFLOWNO"].ColumnName = "PflowNo";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";
                dt.Columns["PNODEID"].ColumnName = "PNodeID";
                dt.Columns["PEMP"].ColumnName = "PEmp";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";
                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
                dt.Columns["EMPS"].ColumnName = "Emps";
                dt.Columns["DOMAIN"].ColumnName = "Domain";
                dt.Columns["SENDDT"].ColumnName = "SendDT";
                dt.Columns["WEEKNUM"].ColumnName = "WeekNum";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["type"].ColumnName = "Type";
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";
                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["title"].ColumnName = "Title";
                dt.Columns["wfsta"].ColumnName = "WFSta";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["sdtofflow"].ColumnName = "SDTOfFlow";
                dt.Columns["pflowno"].ColumnName = "PflowNo";
                dt.Columns["pworkid"].ColumnName = "PWorkID";
                dt.Columns["pnodeid"].ColumnName = "PNodeID";
                dt.Columns["pemp"].ColumnName = "PEmp";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["billno"].ColumnName = "BillNo";
                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["tasksta"].ColumnName = "TaskSta";
                dt.Columns["atpara"].ColumnName = "AtPara";
                dt.Columns["emps"].ColumnName = "Emps";
                dt.Columns["domain"].ColumnName = "Domain";
                dt.Columns["senddt"].ColumnName = "SendDT";
                dt.Columns["weeknum"].ColumnName = "WeekNum";
            }
            return dt;
        }
        ///// <summary>
        ///// 获取某一个人已完成的工作
        ///// </summary>
        ///// <param name="userNo"></param>
        ///// <returns></returns>
        //public static DataTable DB_FlowComplete(string userNo)
        //{

        //    /* 如果不是删除流程注册表. */
        //    Paras ps = new Paras();
        //    string dbstr =  BP.Difference.SystemConfig.AppCenterDBVarStr;
        //    ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE (T.Emps LIKE '%@" + userNo + "@%' OR  T.Emps LIKE '%@" + userNo + ",%') AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
        //    return DBAccess.RunSQLReturnTable(ps);

        //}
        /// <summary>
        /// 获取某一个人某个流程已完成的工作
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable DB_FlowComplete(string userNo, string flowNo)
        {
            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE (T.Emps LIKE '%@" + userNo + "@%' OR  T.Emps LIKE '%@" + userNo + ",%') AND T.FK_Flow='" + flowNo + "' AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取已经完成
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_FlowCompleteAndCC()
        {
            DataTable dt = DB_FlowComplete();
            DataTable ccDT = DB_CCList_CheckOver(WebUser.No);

            try
            {
                dt.Columns.Add("MyPK");
                dt.Columns.Add("Sta");
            }
            catch (Exception)
            {

            }

            foreach (DataRow row in ccDT.Rows)
            {
                DataRow newRow = dt.NewRow();

                foreach (DataColumn column in ccDT.Columns)
                {
                    foreach (DataColumn dtColumn in dt.Columns)
                    {
                        if (column.ColumnName == dtColumn.ColumnName)
                        {
                            newRow[column.ColumnName] = row[dtColumn.ColumnName];
                        }

                    }

                }
                newRow["Type"] = "CC";
                dt.Rows.Add(newRow);
            }
            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        public static DataTable DB_FlowComplete2(string fk_flow, string title)
        {

            /* 如果不是删除流程注册表. */
            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            if (DataType.IsNullOrEmpty(fk_flow))
            {
                if (DataType.IsNullOrEmpty(title))
                {
                    ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow!='010' order by RDT desc";
                }
                else
                {
                    ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' and Title Like '%" + title + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow!='010' order by RDT desc";
                }
            }
            else
            {
                if (DataType.IsNullOrEmpty(title))
                {
                    ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow='" + fk_flow + "' order by RDT desc";
                }
                else
                {
                    ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' and Title Like '%" + title + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow='" + fk_flow + "' order by RDT desc";
                }
            }
            return DBAccess.RunSQLReturnTable(ps);

        }

        public static DataTable DB_FlowCompleteAndCC2(string fk_flow, string title)
        {
            DataTable dt = DB_FlowComplete2(fk_flow, title);
            DataTable ccDT = DB_CCList_CheckOver(WebUser.No);
            try
            {
                dt.Columns.Add("MyPK");
                dt.Columns.Add("Sta");
            }
            catch (Exception)
            {

            }

            foreach (DataRow row in ccDT.Rows)
            {
                DataRow newRow = dt.NewRow();

                foreach (DataColumn column in ccDT.Columns)
                {
                    foreach (DataColumn dtColumn in dt.Columns)
                    {
                        if (column.ColumnName == dtColumn.ColumnName)
                        {
                            newRow[column.ColumnName] = row[dtColumn.ColumnName];
                        }

                    }

                }
                newRow["Type"] = "CC";
                dt.Rows.Add(newRow);
            }
            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        /// <summary>
        /// 获得任务池的工作列表
        /// </summary>
        /// <returns>任务池的工作列表</returns>
        public static DataTable DB_TaskPool()
        {
            if (BP.WF.Glo.IsEnableTaskPool == false)
            {
                throw new Exception("@你必须在Web.config中启用IsEnableTaskPool才可以执行此操作。");
            }

            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Sharing;
            string sql;
            string realSql = null;

            /*不是授权状态*/
            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
            ps.Add("FK_Emp", BP.Web.WebUser.No);

            //@杜. 这里需要翻译.
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["ISREAD"].ColumnName = "IsRead";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";

                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                // dt.Columns["WORKERDEPT"].ColumnName = "WorkerDept";
                dt.Columns["FK_EMP"].ColumnName = "FK_Emp";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";

                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";

                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";

                dt.Columns["LISTTYPE"].ColumnName = "ListType";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["isread"].ColumnName = "IsRead";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["pworkid"].ColumnName = "PWorkID";

                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                //  dt.Columns["workerdept"].ColumnName = "WorkerDept";
                dt.Columns["fk_emp"].ColumnName = "FK_Emp";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";

                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["billno"].ColumnName = "BillNo";

                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["tasksta"].ColumnName = "TaskSta";

                dt.Columns["listtype"].ColumnName = "ListType";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["atpara"].ColumnName = "AtPara";
            }


            return dt;
        }
        /// <summary>
        /// 获得我从任务池里申请下来的工作列表
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_TaskPoolOfMyApply()
        {
            if (BP.WF.Glo.IsEnableTaskPool == false)
            {
                throw new Exception("@你必须在Web.config中启用IsEnableTaskPool才可以执行此操作。");
            }

            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Takeback;
            string sql;
            string realSql;

            /*不是授权状态*/
            // ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY FK_Flow,ADT DESC ";
            //ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY ADT DESC ";
            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp";

            // ps.SQL = "select v1.*,v2.name,v3.name as ParentName from (" + realSql + ") as v1 left join JXW_Inc v2 on v1.WorkID=v2.OID left join Jxw_Inc V3 on v1.PWorkID = v3.OID ORDER BY v1.ADT DESC";

            ps.Add("FK_Emp", BP.Web.WebUser.No);

            //@杜. 这里需要翻译.
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["ISREAD"].ColumnName = "IsRead";
                dt.Columns["STARTER"].ColumnName = "Starter";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["WFSTATE"].ColumnName = "WFState";
                dt.Columns["FK_DEPT"].ColumnName = "FK_Dept";
                dt.Columns["DEPTNAME"].ColumnName = "DeptName";
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["PWORKID"].ColumnName = "PWorkID";

                dt.Columns["PFLOWNO"].ColumnName = "PFlowNo";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";
                //   dt.Columns["WORKERDEPT"].ColumnName = "WorkerDept";
                dt.Columns["FK_EMP"].ColumnName = "FK_Emp";
                dt.Columns["FK_FLOWSORT"].ColumnName = "FK_FlowSort";

                dt.Columns["SYSTYPE"].ColumnName = "SysType";
                dt.Columns["SDTOFNODE"].ColumnName = "SDTOfNode";
                dt.Columns["GUESTNO"].ColumnName = "GuestNo";
                dt.Columns["GUESTNAME"].ColumnName = "GuestName";
                dt.Columns["BILLNO"].ColumnName = "BillNo";

                dt.Columns["FLOWNOTE"].ColumnName = "FlowNote";
                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";
                dt.Columns["TODOEMPSNUM"].ColumnName = "TodoEmpsNum";
                dt.Columns["TODOSTA"].ColumnName = "TodoSta";
                dt.Columns["TASKSTA"].ColumnName = "TaskSta";

                dt.Columns["LISTTYPE"].ColumnName = "ListType";
                dt.Columns["SENDER"].ColumnName = "Sender";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["isread"].ColumnName = "IsRead";
                dt.Columns["starter"].ColumnName = "Starter";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["wfstate"].ColumnName = "WFState";
                dt.Columns["fk_dept"].ColumnName = "FK_Dept";
                dt.Columns["deptname"].ColumnName = "DeptName";
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["pworkid"].ColumnName = "PWorkID";

                dt.Columns["pflowno"].ColumnName = "PFlowNo";
                dt.Columns["fk_node"].ColumnName = "FK_Node";
                //   dt.Columns["workerdept"].ColumnName = "WorkerDept";
                dt.Columns["fk_emp"].ColumnName = "FK_Emp";
                dt.Columns["fk_flowsort"].ColumnName = "FK_FlowSort";

                dt.Columns["systype"].ColumnName = "SysType";
                dt.Columns["sdtofnode"].ColumnName = "SDTOfNode";
                dt.Columns["guestno"].ColumnName = "GuestNo";
                dt.Columns["guestname"].ColumnName = "GuestName";
                dt.Columns["billno"].ColumnName = "BillNo";

                dt.Columns["flownote"].ColumnName = "FlowNote";
                dt.Columns["todoemps"].ColumnName = "TodoEmps";
                dt.Columns["todoempsnum"].ColumnName = "TodoEmpsNum";
                dt.Columns["todosta"].ColumnName = "TodoSta";
                dt.Columns["tasksta"].ColumnName = "TaskSta";

                dt.Columns["listtype"].ColumnName = "ListType";
                dt.Columns["sender"].ColumnName = "Sender";
                dt.Columns["atpara"].ColumnName = "AtPara";
            }

            return dt;
        }
        /// <summary>
        /// 将要执行的工作
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_FutureTodolist()
        {
            string sql = "SELECT A.* FROM WF_GenerWorkFlow A, WF_SelectAccper B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + BP.Web.WebUser.No + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                string columnName = "";
                foreach (DataColumn col in dt.Columns)
                {
                    columnName = col.ColumnName.ToUpper();
                    switch (columnName)
                    {
                        case "ATPARA":
                            col.ColumnName = "AtPara";
                            break;
                        case "WORKID":
                            col.ColumnName = "WorkID";
                            break;
                        case "FK_FLOW":
                            col.ColumnName = "FK_Flow";
                            break;
                        case "FK_FLOWSORT":
                            col.ColumnName = "FK_FlowSort";
                            break;
                        case "FK_NODE":
                            col.ColumnName = "FK_Node";
                            break;
                        case "FLOWNAME":
                            col.ColumnName = "FlowName";
                            break;
                        case "NODENAME":
                            col.ColumnName = "NodeName";
                            break;
                        case "BILLNO":
                            col.ColumnName = "BillNo";
                            break;
                        case "PWORKID":
                            col.ColumnName = "PWorkID";
                            break;
                        case "ORGNO":
                            col.ColumnName = "OrgNo";
                            break;
                        case "WFSTATE":
                            col.ColumnName = "WFSate";
                            break;
                        case "WFSTA":
                            col.ColumnName = "WFSta";
                            break;
                        case "TITLE":
                            col.ColumnName = "Title";
                            break;
                        case "STARTER":
                            col.ColumnName = "Starter";
                            break;
                        case "STARTERNAME":
                            col.ColumnName = "StarterName";
                            break;
                        case "EMPS":
                            col.ColumnName = "Emps";
                            break;
                        case "TODOEMPS":
                            col.ColumnName = "TodoEmps";
                            break;
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 获得指定流程挂起工作列表
        /// </summary>
        /// <param name="flowNo">流程编号,如果编号为空则返回所有的流程挂起工作列表.</param>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static string DB_GenerHungupList(string flowNo = null)
        {
            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            int state = (int)WFState.Hungup;
            qo.AddWhere(GenerWorkFlowAttr.WFState, state);
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.Sender, " LIKE ", "%" + BP.Web.WebUser.No + ",%");
            if (flowNo != null)
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.FK_Flow, flowNo);
            }
            qo.addOrderByDesc(GenerWorkFlowAttr.HungupTime);
            qo.DoQuery();
            return gwfs.ToJson();
        }
        /// <summary>
        /// 获得逻辑删除的流程
        /// </summary>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerDeleteWorkList()
        {
            return DB_GenerDeleteWorkList(WebUser.No, null);
        }
        /// <summary>
        /// 获得逻辑删除的流程:根据流程编号
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <param name="fk_flow">流程编号(可以为空)</param>
        /// <returns>WF_GenerWorkFlow数据结构的集合</returns>
        public static DataTable DB_GenerDeleteWorkList(string userNo, string fk_flow)
        {
            string sql;
            int state = (int)WFState.Delete;
            if (DataType.IsNullOrEmpty(fk_flow))
            {
                sql = "SELECT A.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1   ";
            }
            else
            {
                sql = "SELECT A.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 ";
            }
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            return gwfs.ToDataTableField();
        }
        #endregion 获取当前操作员的共享工作

        #region 获取流程数据
        /// <summary>
        /// 根据流程状态获取指定流程数据
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="sta">流程状态</param>
        /// <returns>数据表OID,Title,RDT,FID</returns>
        public static DataTable DB_NDxxRpt(string fk_flow, WFState sta)
        {
            Flow fl = new Flow(fk_flow);
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            string sql = "SELECT OID,Title,RDT,FID FROM " + fl.PTable + " WHERE WFState=" + (int)sta + " AND Rec=" + dbstr + "Rec";
            Paras ps = new Paras();
            ps.SQL = sql;
            ps.Add("Rec", BP.Web.WebUser.No);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion

        #region 工作部件的数据源获取。
        /// <summary>
        /// 获取当前节点可以退回的节点
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <returns>No节点编号,Name节点名称,Rec记录人,RecName记录人名称</returns>
        public static DataTable DB_GenerWillReturnNodes(Int64 workid)
        {
            DataTable dt = new DataTable("obt");
            dt.Columns.Add("No", typeof(string)); // 节点ID
            dt.Columns.Add("Name", typeof(string)); // 节点名称.
            dt.Columns.Add("Rec", typeof(string)); // 被退回节点上的操作员编号.
            dt.Columns.Add("RecName", typeof(string)); // 被退回节点上的操作员名称.
            dt.Columns.Add("IsBackTracking", typeof(string)); // 该节点是否可以退回并原路返回？ 0否, 1是.
            dt.Columns.Add("AtPara", typeof(string)); // 该节点是否可以退回并原路返回？ 0否, 1是.


            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            Node nd = new Node(gwf.FK_Node);

            //判断流程实例的节点和当前节点是否相同
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                if (Flow_IsCanDoCurrentWork(gwf.WorkID, WebUser.No) == false)
                    throw new Exception("@当前工作处于{" + gwf.NodeName + "}节点,您({" + WebUser.Name + "})没有处理权限.");
            }
            //增加退回到父流程节点的设计.
            if (nd.IsStartNode == true)
            {
                /*如果是开始的节点有可能退回到子流程上去.*/
                if (gwf.PWorkID == 0)
                    throw new Exception("@当前节点是开始节点并且不是子流程，您不能执行退回。");

                GenerWorkerLists gwls = new GenerWorkerLists();
                int i = gwls.Retrieve(GenerWorkerListAttr.WorkID, gwf.PWorkID, GenerWorkerListAttr.RDT);

                string nodes = "";
                foreach (GenerWorkerList gwl in gwls)
                {
                    DataRow dr = dt.NewRow();
                    dr["No"] = gwl.FK_Node.ToString();

                    if (nodes.Contains(gwl.FK_Node.ToString() + ",") == true)
                        continue;

                    nodes += gwl.FK_Node.ToString() + ",";

                    dr["Name"] = gwl.FK_NodeText;
                    dr["Rec"] = gwl.FK_Emp;
                    dr["RecName"] = gwl.FK_EmpText;
                    dr["IsBackTracking"] = "0";

                    dt.Rows.Add(dr);
                }
                return dt;
            }

            if (nd.IsSubThread == true)
            {
                /*如果是子线程，它只能退回它的上一个节点，现在写死了，其它的设置不起作用了。*/
                Nodes nds = nd.FromNodes;
                foreach (Node ndFrom in nds)
                {
                    Work wk;
                    switch (ndFrom.HisRunModel)
                    {
                        case RunModel.FL:
                        case RunModel.FHL:
                            wk = ndFrom.HisWork;
                            wk.OID = gwf.FID;
                            if (wk.RetrieveFromDBSources() == 0)
                                continue;
                            break;
                        case RunModel.SubThreadSameWorkID:
                        case RunModel.SubThreadUnSameWorkID:
                            wk = ndFrom.HisWork;
                            wk.OID = workid;
                            if (wk.RetrieveFromDBSources() == 0)
                                continue;
                            break;
                        case RunModel.Ordinary:
                        default:
                            throw new Exception("流程设计异常，子线程的上一个节点不能是普通节点。");
                    }

                    if (ndFrom.NodeID == gwf.FK_Node)
                        continue;


                    string mysql = "SELECT  a.FK_Emp as Rec, a.FK_EmpText as RecName FROM WF_GenerWorkerlist a WHERE a.FK_Node=" + ndFrom.NodeID + " AND  (a.WorkID=" + workid + " AND a.FID=" + gwf.FID + " )  ORDER BY RDT DESC ";
                    DataTable mydt = DBAccess.RunSQLReturnTable(mysql);
                    if (mydt.Rows.Count == 0)
                        continue;

                    DataRow dr = dt.NewRow();
                    dr["No"] = ndFrom.NodeID.ToString();
                    dr["Name"] = ndFrom.Name;
                    dr["Rec"] = mydt.Rows[0][0];
                    dr["RecName"] = mydt.Rows[0][1];

                    if (ndFrom.IsBackTracking == true)
                        dr["IsBackTracking"] = "1";
                    else
                        dr["IsBackTracking"] = "0";

                    dt.Rows.Add(dr);
                } //结束循环.

                if (dt.Rows.Count == 0)
                    throw new Exception("err@没有获取到应该退回的节点列表.");
                return dt;
            }

            string sql = "";

            WorkNode wn = new WorkNode(workid, gwf.FK_Node);
            WorkNodes wns = new WorkNodes();
            switch (nd.HisReturnRole)
            {
                case ReturnRole.CanNotReturn:
                    return dt;
                case ReturnRole.ReturnAnyNodes:
                    if (nd.IsHL || nd.IsFLHL)
                    {
                        /*如果当前点是分流，或者是分合流，就不按退回规则计算了。*/
                        sql = "SELECT A.FK_Node AS No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking FROM WF_GenerWorkerlist a, WF_Node b WHERE a.FK_Node=b.NodeID AND a.FID=" + gwf.FID + " AND a.WorkID=" + workid + " AND a.FK_Node!=" + gwf.FK_Node + " AND a.IsPass=1 ORDER BY RDT DESC ";
                        dt = DBAccess.RunSQLReturnTable(sql);
                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                        {
                            dt.Columns["NO"].ColumnName = "No";
                            dt.Columns["NAME"].ColumnName = "Name";
                            dt.Columns["REC"].ColumnName = "Rec";
                            dt.Columns["RECNAME"].ColumnName = "RecName";
                            dt.Columns["ISBACKTRACKING"].ColumnName = "IsBackTracking";
                        }
                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                        {
                            dt.Columns["no"].ColumnName = "No";
                            dt.Columns["name"].ColumnName = "Name";
                            dt.Columns["rec"].ColumnName = "Rec";
                            dt.Columns["recname"].ColumnName = "RecName";
                            dt.Columns["isbacktracking"].ColumnName = "IsBackTracking";
                        }
                        return dt;
                    }

                    if (nd.TodolistModel == TodolistModel.Order)
                        sql = "SELECT A.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking, a.AtPara FROM WF_GenerWorkerlist a, WF_Node b WHERE a.FK_Node=b.NodeID AND (a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 AND a.FK_Node!=" + gwf.FK_Node + ") OR (a.FK_Node=" + gwf.FK_Node + " AND a.IsPass <0)  ORDER BY a.RDT DESC";
                    else
                        sql = "SELECT a.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking, a.AtPara FROM WF_GenerWorkerlist a,WF_Node b WHERE a.FK_Node=b.NodeID AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 AND a.FK_Node!=" + gwf.FK_Node + " AND a.AtPara NOT LIKE '%@IsHuiQian=1%' ORDER BY a.RDT DESC";

                    // sql = "SELECT A.NDFrom AS No, A.NDFromT AS Name, A.EmpFrom AS Rec, A.EmpFromT AS RecName, B.IsBackTracking, A.Msg FROM ND" + int.Parse(nd.FK_Flow) + "Track A, WF_Node B WHERE A.NDFrom=B.NodeID AND A.WorkID = " + workid + " AND A.ActionType in(" + (int)ActionType.Start + "," + (int)ActionType.Forward + "," + (int)ActionType.ForwardFL + "," + (int)ActionType.ForwardHL + ") AND A.NDFrom != " + fk_node + " ORDER BY A.RDT DESC";
                    // ss
                    // Log.DebugWriteWarning(sql);

                    dt = DBAccess.RunSQLReturnTable(sql);

                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                    {
                        dt.Columns["NO"].ColumnName = "No";
                        dt.Columns["NAME"].ColumnName = "Name";
                        dt.Columns["REC"].ColumnName = "Rec";
                        dt.Columns["RECNAME"].ColumnName = "RecName";
                        dt.Columns["ISBACKTRACKING"].ColumnName = "IsBackTracking";
                        dt.Columns["ATPARA"].ColumnName = "AtPara"; //参数.
                    }
                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                    {
                        dt.Columns["no"].ColumnName = "No";
                        dt.Columns["name"].ColumnName = "Name";
                        dt.Columns["rec"].ColumnName = "Rec";
                        dt.Columns["recname"].ColumnName = "RecName";
                        dt.Columns["isbacktracking"].ColumnName = "IsBackTracking";

                        dt.Columns["atpara"].ColumnName = "AtPara"; //参数.
                    }

                    #region 增加上，可以退回到延续流程的节点.  @lizhen
                    if (gwf.PWorkID != 0)
                    {
                        /* 满足省联社的需求.
                         * 1. 判断是否有延续子流程，延续到当前节点上来？，如果有则把父流程的节点也显示出来。
                         * 2. 
                         */
                        sql = "SELECT FK_Flow,FK_Node FROM WF_NodeSubFlow WHERE YanXuToNode LIKE '%" + gwf.FK_Node + "%'";
                        DataTable mydt = DBAccess.RunSQLReturnTable(sql);

                        foreach (DataRow drSubFlow in mydt.Rows)
                        {
                            string flowNo = drSubFlow[0].ToString();
                            string nodeID = drSubFlow[1].ToString();

                            GenerWorkerLists gwls = new GenerWorkerLists();
                            int i = gwls.Retrieve(GenerWorkerListAttr.WorkID, gwf.PWorkID, GenerWorkerListAttr.FK_Node);
                            string nodes = "";
                            foreach (GenerWorkerList gwl in gwls)
                            {
                                DataRow dr = dt.NewRow();
                                dr["No"] = gwl.FK_Node.ToString();
                                if (nodes.Contains(gwl.FK_Node.ToString() + ",") == true)
                                    continue;

                                nodes += gwl.FK_Node.ToString() + ",";

                                BP.WF.Flow fl = new Flow(gwl.FK_Flow);

                                dr["Name"] = fl.Name + ":" + gwl.FK_NodeText;
                                dr["Rec"] = gwl.FK_Emp;
                                dr["RecName"] = gwl.FK_EmpText;
                                dr["IsBackTracking"] = "0";
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                    #endregion 增加上，可以退回到延续流程的节点.

                    //GenerWorkFlow gwf = new GenerWorkFlow(workid);
                    //if (gwf.PNodeID==  )

                    return dt;
                case ReturnRole.ReturnPreviousNode:

                    if (nd.IsHL || nd.IsFLHL)
                    {
                        /*如果当前点是分流，或者是分合流，就不按退回规则计算了。*/
                        //  if (nd.IsSubThread==true)
                        sql = "SELECT A.FK_Node AS No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking, a.AtPara FROM WF_GenerWorkerlist a," +
                        " WF_Node b WHERE a.FK_Node=b.NodeID AND a.FID=0 AND a.WorkID=" + workid + " AND a.IsPass=1 AND A.FK_Node!=" + gwf.FK_Node + " ORDER BY A.RDT ASC ";

                        dt = DBAccess.RunSQLReturnTable(sql);
                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                        {
                            dt.Columns["NO"].ColumnName = "No";
                            dt.Columns["NAME"].ColumnName = "Name";
                            dt.Columns["REC"].ColumnName = "Rec";
                            dt.Columns["RECNAME"].ColumnName = "RecName";
                            dt.Columns["ISBACKTRACKING"].ColumnName = "IsBackTracking";
                            dt.Columns["ATPARA"].ColumnName = "AtPara"; //参数.
                        }

                        while (1 == 1)
                        {
                            if (dt.Rows.Count == 1)
                                break;
                            dt.Rows.RemoveAt(dt.Rows.Count - 1);
                        }
                        return dt;
                    }

                    WorkNode mywnP = wn.GetPreviousWorkNode();


                    if (nd.TodolistModel == TodolistModel.Order)
                    {
                        sql = "SELECT A.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking,a.AtPara FROM WF_GenerWorkerlist a, WF_Node b WHERE a.FK_Node=b.NodeID AND (a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 AND a.FK_Node=" + mywnP.HisNode.NodeID + ") OR (a.FK_Node=" + mywnP.HisNode.NodeID + " AND a.IsPass <0)  ORDER BY a.RDT DESC";
                        dt = DBAccess.RunSQLReturnTable(sql);
                    }
                    else
                    {
                        sql = "SELECT A.FK_Node as \"No\",a.FK_NodeText as \"Name\", a.FK_Emp as \"Rec\", a.FK_EmpText as \"RecName\", b.IsBackTracking as \"IsBackTracking\", a.AtPara as \"AtPara\"  FROM WF_GenerWorkerlist a,WF_Node b WHERE a.FK_Node=b.NodeID AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 AND a.FK_Node=" + mywnP.HisNode.NodeID + "  AND ( A.AtPara NOT LIKE '%@IsHuiQian=1%' OR a.AtPara IS NULL) ORDER BY a.RDT DESC ";
                        DataTable mydt = DBAccess.RunSQLReturnTable(sql);

                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                        {
                            dt.Columns["NO"].ColumnName = "No";
                            dt.Columns["NAME"].ColumnName = "Name";
                            dt.Columns["REC"].ColumnName = "Rec";
                            dt.Columns["RECNAME"].ColumnName = "RecName";
                            dt.Columns["ISBACKTRACKING"].ColumnName = "IsBackTracking";
                            dt.Columns["ATPARA"].ColumnName = "AtPara";
                        }

                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                        {
                            dt.Columns["no"].ColumnName = "No";
                            dt.Columns["name"].ColumnName = "Name";
                            dt.Columns["rec"].ColumnName = "Rec";
                            dt.Columns["recname"].ColumnName = "RecName";
                            dt.Columns["isbacktracking"].ColumnName = "IsBackTracking";
                            dt.Columns["atpara"].ColumnName = "AtPara";
                        }

                        if (mydt.Rows.Count != 0)
                        {
                            return mydt;
                        }

                        //有可能是跳转过来的节点.//edited by liuxc,2017-05-26,改RDT排序为CDT排序，更准确，以避免有时找错上一步节点的情况发生
                        if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                        {
                            sql = "SELECT top 1 A.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking,a.AtPara FROM WF_GenerWorkerlist a,WF_Node b WHERE a.FK_Node=b.NodeID AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 ORDER BY a.CDT DESC ";
                        }
                        else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                        {
                            sql = "SELECT a.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking,a.AtPara FROM WF_GenerWorkerlist a,WF_Node b WHERE a.FK_Node=b.NodeID AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 AND rownum =1  ORDER BY a.CDT DESC ";
                        }
                        else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                        {
                            sql = "SELECT a.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking,a.AtPara FROM WF_GenerWorkerlist a,WF_Node b WHERE a.FK_Node=b.NodeID AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 ORDER BY a.CDT DESC LIMIT 1";
                        }
                        else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                        {
                            sql = "SELECT a.FK_Node as No,a.FK_NodeText as Name, a.FK_Emp as Rec, a.FK_EmpText as RecName, b.IsBackTracking,a.AtPara FROM WF_GenerWorkerlist a,WF_Node b WHERE a.FK_Node=b.NodeID AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1 ORDER BY a.CDT DESC LIMIT 1";
                        }
                        else
                        {
                            throw new Exception("获取上一步节点，未涉及的数据库类型");
                        }

                        dt = DBAccess.RunSQLReturnTable(sql);

                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                        {
                            dt.Columns["NO"].ColumnName = "No";
                            dt.Columns["NAME"].ColumnName = "Name";
                            dt.Columns["REC"].ColumnName = "Rec";
                            dt.Columns["RECNAME"].ColumnName = "RecName";
                            dt.Columns["ISBACKTRACKING"].ColumnName = "IsBackTracking";
                            dt.Columns["ATPARA"].ColumnName = "AtPara";
                        }
                        if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                        {
                            dt.Columns["no"].ColumnName = "No";
                            dt.Columns["name"].ColumnName = "Name";
                            dt.Columns["rec"].ColumnName = "Rec";
                            dt.Columns["recname"].ColumnName = "RecName";
                            dt.Columns["isbacktracking"].ColumnName = "IsBackTracking";
                            dt.Columns["atpara"].ColumnName = "AtPara";
                        }
                        return dt;
                    }
                    break;
                case ReturnRole.ReturnSpecifiedNodes: //退回指定的节点。
                    if (wns.Count == 0)
                    {
                        wns.GenerByWorkID(wn.HisNode.HisFlow, workid);
                    }

                    NodeReturns rnds = new NodeReturns();
                    rnds.Retrieve(NodeReturnAttr.FK_Node, gwf.FK_Node);
                    if (rnds.Count == 0)
                        throw new Exception("@流程设计错误，您设置该节点可以退回指定的节点，但是指定的节点集合为空，请在节点属性设置它的制订节点。");

                    foreach (NodeReturn item in rnds)
                    {
                        GenerWorkerLists gwls = new GenerWorkerLists();
                        int i = gwls.Retrieve(GenerWorkerListAttr.FK_Node, item.ReturnTo,
                            GenerWorkerListAttr.WorkID, workid);
                        if (i == 0)
                            continue;

                        foreach (GenerWorkerList gwl in gwls)
                        {
                            DataRow dr = dt.NewRow();
                            dr["No"] = gwl.FK_Node.ToString();
                            dr["Name"] = gwl.FK_NodeText;
                            dr["Rec"] = gwl.FK_Emp;
                            dr["RecName"] = gwl.FK_EmpText;
                            Node mynd = new Node(item.FK_Node);
                            if (mynd.IsBackTracking == true) //是否可以原路返回.
                                dr["IsBackTracking"] = "1";
                            else
                                dr["IsBackTracking"] = "0";

                            dt.Rows.Add(dr);
                        }
                    }

                    // if (dt.Rows.Count == 0)
                    //   throw new Exception("err@" + rnds.ToJson());

                    break;
                case ReturnRole.ByReturnLine: //按照流程图画的退回线执行退回.
                    Directions dirs = new Directions();
                    dirs.Retrieve(DirectionAttr.Node, gwf.FK_Node);
                    if (dirs.Count == 0)
                    {
                        throw new Exception("@流程设计错误:当前节点没有画向后退回的退回线,更多的信息请参考退回规则.");
                    }

                    foreach (Direction dir in dirs)
                    {
                        Node toNode = new Node(dir.ToNode);
                        sql = "SELECT a.FK_Emp,a.FK_EmpText FROM WF_GenerWorkerlist a, WF_Node b WHERE   a.FK_Node=" + toNode.NodeID + " AND a.WorkID=" + workid + " AND a.IsEnable=1 AND a.IsPass=1";
                        DataTable dt1 = DBAccess.RunSQLReturnTable(sql);
                        if (dt1.Rows.Count == 0)
                        {
                            continue;
                        }

                        DataRow dr = dt.NewRow();
                        dr["No"] = toNode.NodeID.ToString();
                        dr["Name"] = toNode.Name;
                        dr["Rec"] = dt1.Rows[0][0];
                        dr["RecName"] = dt1.Rows[0][1];
                        if (toNode.IsBackTracking == true)
                            dr["IsBackTracking"] = "1";
                        else
                            dr["IsBackTracking"] = "0";



                        dt.Rows.Add(dr);
                    }
                    break;
                default:
                    throw new Exception("@没有判断的退回类型。");
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["REC"].ColumnName = "Rec";
                dt.Columns["RECNAME"].ColumnName = "RecName";
                dt.Columns["ISBACKTRACKING"].ColumnName = "IsBackTracking";
                dt.Columns["ATPARA"].ColumnName = "AtPara";
            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["no"].ColumnName = "No";
                dt.Columns["name"].ColumnName = "Name";
                dt.Columns["rec"].ColumnName = "Rec";
                dt.Columns["recname"].ColumnName = "RecName";
                dt.Columns["isbacktracking"].ColumnName = "IsBackTracking";
                dt.Columns["atpara"].ColumnName = "AtPara";
            }

            if (dt.Rows.Count == 0)
                throw new Exception("@没有计算出来要退回的节点，请管理员确认节点退回规则是否合理？当前节点名称:" + nd.Name + ",退回规则:" + nd.HisReturnRole.ToString());
            return dt;
        }
        #endregion 工作部件的数据源获取

        #region 获取当前操作员的在途工作

        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// 该接口为在途菜单提供数据,在在途工作中，可以执行撤销发送。
        /// </summary>
        /// <param name="userNo">操作员</param>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="isMyStarter">是否仅仅查询我发起的在途流程</param>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string userNo, string fk_flow,
            bool isMyStarter = false, string domain = null, bool isContainFuture = false)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();

            string domainSQL = "";
            if (domain == null)
                domainSQL = " AND A.Domain='" + domain + "' ";

            //获取用户当前所在的节点
            String currNode = "";
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    currNode = "(SELECT FK_Node FROM (SELECT G.FK_Node FROM WF_GenerWorkFlow A,WF_GenerWorkerlist G WHERE G.WorkID = A.WorkID AND G.FK_Emp='" + WebUser.No + "' " + domainSQL + " Order by G.RDT DESC ) WHERE RowNum=1)";
                    break;
                case DBType.MySQL:
                case DBType.PostgreSQL:
                case DBType.UX:
                    currNode = "(SELECT  FK_Node FROM WF_GenerWorkerlist G WHERE   G.WorkID = A.WorkID AND FK_Emp='" + WebUser.No + "' Order by RDT DESC LIMIT 1)";
                    break;
                case DBType.MSSQL:
                    currNode = "(SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist G WHERE  G.WorkID = A.WorkID AND FK_Emp='" + WebUser.No + "' Order by RDT DESC)";
                    break;
                default:
                    break;
            }


            //授权模式.
            string sql = "";
            string futureSQL = "";
            if (isContainFuture == true)
            {
                switch (DBAccess.AppCenterDBType)
                {
                    case DBType.MySQL:
                        futureSQL = " UNION SELECT A.WorkID,A.StarterName,A.Title,A.DeptName,D.Name AS NodeName,A.RDT,B.FK_Node,A.FK_Flow,A.FID,A.FlowName,C.EmpName AS TodoEmps," + currNode + " AS CurrNode ,1 AS RunType,A.WFState, 0 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_SelectAccper B,"
                                + "(SELECT GROUP_CONCAT(B.EmpName SEPARATOR ';') AS EmpName, B.WorkID,B.FK_Node FROM WF_GenerWorkFlow A, WF_SelectAccper B WHERE A.WorkID = B.WorkID  group By B.FK_Node,B.WorkID) C,WF_Node D"
                                + " WHERE A.WorkID = B.WorkID AND B.WorkID = C.WorkID AND B.FK_Node = C.FK_Node AND A.FK_Node = D.NodeID AND B.FK_Emp = '" + WebUser.No + "' " + domainSQL
                                + " AND B.FK_Node Not in(Select DISTINCT FK_Node From WF_GenerWorkerlist G where G.WorkID = B.WorkID)AND A.WFState NOT IN(0,1,3)";
                        break;
                    case DBType.MSSQL:
                        futureSQL = " UNION SELECT A.WorkID,A.StarterName,A.Title,A.DeptName,D.Name AS NodeName,A.RDT,B.FK_Node,A.FK_Flow,A.FID,A.FlowName,C.EmpName AS TodoEmps ," + currNode + " AS CurrNode ,1 AS RunType,A.WFState, 0 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_SelectAccper B,"
                                + "(SELECT EmpName=STUFF((Select ';'+FK_Emp+','+EmpName From WF_SelectAccper t Where t.FK_Node=B.FK_Node FOR xml path('')) , 1 , 1 , '') , B.WorkID,B.FK_Node FROM WF_GenerWorkFlow A, WF_SelectAccper B WHERE A.WorkID = B.WorkID  group By B.FK_Node,B.WorkID) C,WF_Node D"
                                + " WHERE A.WorkID = B.WorkID AND B.WorkID = C.WorkID AND B.FK_Node = C.FK_Node AND A.FK_Node = D.NodeID AND B.FK_Emp = '" + WebUser.No + "' " + domainSQL
                                + " AND B.FK_Node Not in(Select DISTINCT FK_Node From WF_GenerWorkerlist G where G.WorkID = B.WorkID)AND A.WFState NOT IN(0,1,3)";
                        break;
                    default:
                        break;

                }
            }

            if (DataType.IsNullOrEmpty(fk_flow))
            {
                if (isMyStarter == true)
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps," + currNode + " AS CurrNode,0 AS RunType,a.WFState, 0 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND  A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass != -2 AND   (B.IsPass=1 or B.IsPass < -1) AND  A.Starter=" + dbStr + "Starter AND A.WFState NOT IN ( 0, 1, 3 )";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    sql += " UNION SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps,A.FK_Node AS CurrNode,0 AS RunType,a.WFState, 1 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B,WF_Node C WHERE   A.WorkID=B.WorkID AND A.FK_Node=C.NodeID AND B.FK_Node=C.NodeID AND C.WhoExeIt=0 AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass = 0 AND B.WhoExeIt=1 AND  A.Starter=" + dbStr + "Starter AND A.WFState NOT IN ( 0, 1, 3 )";
                    ps.SQL = sql;
                    ps.Add("FK_Emp", userNo);
                    ps.Add("Starter", userNo);
                }
                else
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType,a.WFState, 0 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND  A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass != -2 AND  (B.IsPass=1 or B.IsPass < -1) AND A.WFState NOT IN ( 0, 1, 3 )";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    sql += " UNION SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps,A.FK_Node AS CurrNode,0 AS RunType,a.WFState, 1 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B,WF_Node C WHERE   A.WorkID=B.WorkID AND A.FK_Node=C.NodeID AND B.FK_Node=C.NodeID AND C.WhoExeIt=0 AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass = 0 AND B.WhoExeIt=1  AND A.WFState NOT IN ( 0, 1, 3 )";

                    ps.SQL = sql;
                    ps.Add("FK_Emp", userNo);
                }
            }
            else
            {
                if (isMyStarter == true)
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType,a.WFState, 0 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND  A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass != -2 AND  (B.IsPass=1 or B.IsPass < -1 ) AND  A.Starter=" + dbStr + "Starter AND A.WFState NOT IN ( 0, 1, 3 )";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    sql += " UNION SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps,A.FK_Node AS CurrNode,0 AS RunType,a.WFState, 1 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B,WF_Node C WHERE   A.FK_Flow=" + dbStr + "FK_Flow AND A.WorkID=B.WorkID AND A.FK_Node=C.NodeID AND B.FK_Node=C.NodeID AND C.WhoExeIt=0 AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass = 0 AND B.WhoExeIt=1 AND  A.Starter=" + dbStr + "Starter AND A.WFState NOT IN ( 0, 1, 3 )";
                    ps.SQL = sql;
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", userNo);
                    ps.Add("Starter", userNo);
                }
                else
                {
                    sql = "SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps ," + currNode + " AS CurrNode,0 AS RunType,a.WFState, 0 AS WhoExeIt  FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.TodoEmps  not like '%" + WebUser.No + ",%' AND  A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass != -2 AND (B.IsPass=1 or B.IsPass < -1 ) AND A.WFState NOT IN ( 0, 1, 3 )";
                    if (isContainFuture == true)
                    {
                        sql += futureSQL;
                    }
                    sql += " UNION SELECT DISTINCT a.WorkID,a.StarterName,a.Title,a.DeptName,a.NodeName,a.RDT,a.FK_Node,a.FK_Flow,a.FID ,a.FlowName,a.TodoEmps,A.FK_Node AS CurrNode,0 AS RunType,a.WFState, 1 AS WhoExeIt FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B,WF_Node C WHERE   A.FK_Flow=" + dbStr + "FK_Flow AND A.WorkID=B.WorkID AND A.FK_Node=C.NodeID AND B.FK_Node=C.NodeID AND C.WhoExeIt=0 AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND B.IsPass = 0 AND B.WhoExeIt=1  AND A.WFState NOT IN ( 0, 1, 3 )";

                    ps.SQL = sql;
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", userNo);
                }
            }

            ps.SQL = ps.SQL + " Order By RDT DESC";
            //获得sql.
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["WORKID"].ColumnName = "WorkID";
                dt.Columns["STARTERNAME"].ColumnName = "StarterName";
                dt.Columns["TITLE"].ColumnName = "Title";

                dt.Columns["NODENAME"].ColumnName = "NodeName";
                dt.Columns["RDT"].ColumnName = "RDT";

                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";

                dt.Columns["FID"].ColumnName = "FID";
                dt.Columns["FK_NODE"].ColumnName = "FK_Node";

                dt.Columns["FLOWNAME"].ColumnName = "FlowName";

                dt.Columns["DEPTNAME"].ColumnName = "DeptName";

                dt.Columns["TODOEMPS"].ColumnName = "TodoEmps";

                dt.Columns["CURRNODE"].ColumnName = "CurrNode";
                dt.Columns["RUNTYPE"].ColumnName = "RunType";
                dt.Columns["WHOEXEIT"].ColumnName = "WhoExeIt";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["workid"].ColumnName = "WorkID";
                dt.Columns["startername"].ColumnName = "StarterName";
                dt.Columns["title"].ColumnName = "Title";

                dt.Columns["nodename"].ColumnName = "NodeName";
                dt.Columns["rdt"].ColumnName = "RDT";

                dt.Columns["fk_flow"].ColumnName = "FK_Flow";

                dt.Columns["fid"].ColumnName = "FID";
                dt.Columns["fk_node"].ColumnName = "FK_Node";

                dt.Columns["flowname"].ColumnName = "FlowName";

                dt.Columns["deptname"].ColumnName = "DeptName";

                dt.Columns["todoemps"].ColumnName = "TodoEmps";

                dt.Columns["currnode"].ColumnName = "CurrNode";
                dt.Columns["runtype"].ColumnName = "RunType";
                dt.Columns["whoexeit"].ColumnName = "WhoExeIt";
            }

            return dt;
        }
        /// <summary>
        /// 在途统计:用于流程查询
        /// </summary>
        /// <returns>返回 FK_Flow,FlowName,Num 三个列.</returns>
        public static DataTable DB_TongJi_Runing()
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();

            ps.SQL = "SELECT FK_Flow,FlowName, Count(WorkID) as Num FROM WF_GenerWorkFlow WHERE Emps like '%@" + WebUser.No + "," + WebUser.Name + "@%' AND TodoEmps NOT like '%" + WebUser.No + "," + WebUser.Name + ";%' AND WFState != 3  GROUP BY FK_Flow, FlowName";
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dt.Columns["FK_FLOW"].ColumnName = "FK_Flow";
                dt.Columns["FLOWNAME"].ColumnName = "FlowName";
                dt.Columns["NUM"].ColumnName = "Num";
            }

            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dt.Columns["fk_flow"].ColumnName = "FK_Flow";
                dt.Columns["flowname"].ColumnName = "FlowName";
                dt.Columns["num"].ColumnName = "Num";
            }

            return dt;
        }
        /// <summary>
        /// 统计流程状态
        /// </summary>
        /// <returns>返回：流程类别编号，名称，流程编号，流程名称，TodoSta0代办中,TodoSta1预警中,TodoSta2预期中,TodoSta3已办结. </returns>
        public static DataTable DB_TongJi_TodoSta()
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();

            ps.SQL = "SELECT a.FK_Flow,a.FlowName, Count(a.WorkID) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) GROUP BY A.FK_Flow, A.FlowName";
            ps.Add("FK_Emp", WebUser.No);

            return DBAccess.RunSQLReturnTable(ps);
        }
        public static DataTable DB_GenerRuning2(string userNo, string fk_flow, string titleKey)
        {
            string sql;
            int state = (int)WFState.Runing;
            if (DataType.IsNullOrEmpty(fk_flow))
            {
                if (DataType.IsNullOrEmpty(titleKey))
                {
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) and A.FK_Flow!='010'";
                }
                else
                {
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) and A.FK_Flow!='010' and A.Title Like '%" + titleKey + "%'";
                }
            }
            else
            {
                if (DataType.IsNullOrEmpty(titleKey))
                {
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 )";
                }
                else
                {
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 ) and A.Title Like '%" + titleKey + "%' ";
                }
            }

            return DBAccess.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// </summary>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string userNo = null, bool isContainFuture = false, string domain = null)
        {
            if (userNo == null)
                userNo = WebUser.No;

            DataTable dt = DB_GenerRuning(userNo, null, false, domain, isContainFuture);

            /*暂时屏蔽type的拼接，拼接后转json会报错 于庆海修改*/
            /*dt.Columns.Add("Type");
            foreach (DataRow row in dt.Rows)
            {
                row["Type"] = "RUNNING";
            }*/

            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        /// <summary>
        /// 获取某一个人的在途（参与、未完成的工作、历史任务）
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable DB_GenerRuning(string userNo)
        {
            DataTable dt = DB_GenerRuning(userNo, null);
            dt.Columns.Add("Type");

            foreach (DataRow row in dt.Rows)
            {
                row["Type"] = "RUNNING";
            }

            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        /// <summary>
        /// 把抄送的信息也发送
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_GenerRuningAndCC()
        {
            DataTable dt = DB_GenerRuning();
            DataTable ccDT = DB_CCList_CheckOver(WebUser.No);
            try
            {
                dt.Columns.Add("MyPK");
                dt.Columns.Add("Sta");
            }
            catch (Exception)
            {

            }

            foreach (DataRow row in ccDT.Rows)
            {
                DataRow newRow = dt.NewRow();

                foreach (DataColumn column in ccDT.Columns)
                {
                    foreach (DataColumn dtColumn in dt.Columns)
                    {
                        if (column.ColumnName == dtColumn.ColumnName)
                        {
                            newRow[column.ColumnName] = row[dtColumn.ColumnName];
                        }
                    }
                }
                newRow["Type"] = "CC";
                dt.Rows.Add(newRow);
            }
            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        /// <summary>
        /// 为什么需要这个接口？
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fk_flow"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static DataTable DB_GenerRuning3(string name, string fk_flow, string title)
        {
            DataTable dt = DB_GenerRuning2(name, fk_flow, title);

            dt.Columns.Add("Type");

            foreach (DataRow row in dt.Rows)
            {
                row["Type"] = "RUNNING";
            }

            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        public static DataTable DB_GenerRuningAndCC2(string name, string fk_flow, string title)
        {
            DataTable dt = DB_GenerRuning3(name, fk_flow, title);
            DataTable ccDT = DB_CCList_CheckOver(WebUser.No);
            try
            {
                dt.Columns.Add("MyPK");
                dt.Columns.Add("Sta");
            }
            catch (Exception)
            {

            }

            foreach (DataRow row in ccDT.Rows)
            {
                DataRow newRow = dt.NewRow();

                foreach (DataColumn column in ccDT.Columns)
                {
                    foreach (DataColumn dtColumn in dt.Columns)
                    {
                        if (column.ColumnName == dtColumn.ColumnName)
                        {
                            newRow[column.ColumnName] = row[dtColumn.ColumnName];
                        }

                    }

                }
                newRow["Type"] = "CC";
                dt.Rows.Add(newRow);
            }
            dt.DefaultView.Sort = "RDT DESC";
            return dt.DefaultView.ToTable();
        }
        #endregion 获取当前操作员的共享工作

        #region 获取当前的批处理工作
        /// <summary>
        /// 获取当前节点的批处理工作
        /// </summary>
        /// <param name="FK_Node"></param>
        /// <returns></returns>
        public static DataTable GetBatch(int FK_Node)
        {

            BP.WF.Node nd = new BP.WF.Node(FK_Node);
            Flow fl = nd.HisFlow;
            string fromTable = "";


            fromTable = fl.PTable;

            string sql = "SELECT a.*, b.Starter,b.Title as STitle,b.ADT,b.WorkID FROM " + fromTable
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            // string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return dt;
        }

        #endregion 获取当前的批处理工作
        #endregion

        #region 登陆接口

        /// <summary>
        /// 按照token登录 2021.07.01 采用新方式.
        /// </summary>
        /// <param name="token"></param>
        public static string Port_LoginByToken(string token)
        {
            try
            {
                if (DataType.IsNullOrEmpty(token))
                    throw new Exception("err@ token 不能为空.");
                token = token.Trim();
                if (DataType.IsNullOrEmpty(token) == true || token.Contains(" "))
                    throw new Exception("err@非法的Token.");


                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    Token mytk = new Token();
                    mytk.MyPK = token;
                    if (mytk.RetrieveFromDBSources() == 0)
                        throw new Exception("@token=[" + token + "]失效");
                    BP.WF.Dev2Interface.Port_Login(mytk.EmpNo, mytk.OrgNo);
                    return mytk.EmpNo;
                }

                //如果是宽泛模式.
                if (SystemConfig.TokenModel == 0)
                {
                    Token tk = new Token();
                    tk.MyPK = token;
                    if (tk.RetrieveFromDBSources() == 0)
                        throw new Exception("err@ token 过期或失效.");

                    BP.Web.WebUser.No = tk.EmpNo;
                    BP.Web.WebUser.Name = tk.EmpName;

                    BP.Web.WebUser.FK_Dept = tk.DeptNo;
                    BP.Web.WebUser.FK_DeptName = tk.DeptName;

                    BP.Web.WebUser.OrgNo = tk.OrgNo;
                    BP.Web.WebUser.OrgName = tk.OrgName;
                    return tk.EmpNo;
                }


                string sql = "SELECT No,OrgNo FROM WF_Emp WHERE AtPara LIKE '%" + token + "%'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count != 1)
                    throw new Exception("err@token失效." + token);

                string no = dt.Rows[0][0].ToString();
                string orgNo = dt.Rows[0][1] != null ? dt.Rows[0][1].ToString() : "";
                //执行登录.
                BP.WF.Dev2Interface.Port_Login(no, orgNo);
                return no;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userID">人员编号</param>
        /// <param name="orgNo">组织结构编码</param>
        /// <returns></returns>
        public static void Port_Login(string userID, string orgNo = null, string deptNo = null)
        {
            /* 仅仅传递了人员编号，就按照人员来取.*/
            BP.Port.Emp emp = new BP.Port.Emp();
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                if (orgNo == null)
                    throw new Exception("err@缺少OrgNo参数.");
                emp.No = orgNo + "_" + userID;
                emp.OrgNo = orgNo;
                if (emp.RetrieveFromDBSources() == 0)
                    throw new Exception("err@用户名:" + emp.GetValByKey("No") + "不存在.");

                WebUser.SignInOfGener(emp);
                return;
            }

            emp.No = userID;

            int i = emp.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("err@用户名:" + emp.GetValByKey("No") + "不存在.");

            string sql = "";
            // 两个同是是Null.
            if (DataType.IsNullOrEmpty(deptNo) == true && DataType.IsNullOrEmpty(orgNo) == true)
            {
                if (DataType.IsNullOrEmpty(emp.FK_Dept) == true)
                {
                    //寻找随机的部门编号登陆》
                    //如果选择的组织不存在，就从他的隶属部门里去找一个部门。
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql = "SELECT A.FK_Dept,B.OrgNo FROM Port_DeptEmp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + emp.No + "'";
                    else
                        sql = "SELECT A.FK_Dept,B.OrgNo FROM Port_DeptEmp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + emp.No + "' AND  B.OrgNo!='' AND B.OrgNo IS NOT NULL ";

                    deptNo = DBAccess.RunSQLReturnStringIsNull(sql, "");
                    if (DataType.IsNullOrEmpty(deptNo) == true)
                        throw new Exception("err@用户[" + emp.No + "]没有部门，无法登陆.");
                    emp.FK_Dept = deptNo;
                    WebUser.SignInOfGener(emp);
                    return;
                }
                WebUser.SignInOfGener(emp);
                return;
            }

            //如果部门编号不为空.
            if (DataType.IsNullOrEmpty(deptNo) == false)
            {
                if (emp.FK_Dept.Equals(deptNo) == false)
                {
                    //判断当前人员是否在该部门下
                    sql = "SELECT COUNT(*) From Port_DeptEmp WHERE FK_Emp='" + emp.No + "' AND FK_Dept='" + deptNo + "'";
                    if (DBAccess.RunSQLReturnValInt(sql) == 0)
                        throw new Exception("err@用户[" + emp.No + "]不在部门[" + deptNo + "]，无法登陆.");
                }
                emp.FK_Dept = deptNo;
                WebUser.SignInOfGener(emp);
                return;
            }

            if (orgNo == null)
                return;

            //当前人员的 orgNo 是组织编号.
            if (emp.OrgNo.Equals(orgNo) == true)
            {
                if (DataType.IsNullOrEmpty(emp.FK_Dept) == true)
                {
                    //当前人员部门不存在。
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql = "SELECT A.FK_Dept,B.OrgNo FROM Port_DeptEmp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + emp.No + "'";
                    else
                        sql = "SELECT A.FK_Dept,B.OrgNo FROM Port_DeptEmp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + emp.No + "' AND  B.OrgNo='" + orgNo + "'";

                    deptNo = DBAccess.RunSQLReturnStringIsNull(sql, "");
                    if (DataType.IsNullOrEmpty(deptNo) == true)
                        throw new Exception("err@用户[" + emp.No + "]没有部门，无法登陆.");
                    emp.FK_Dept = deptNo;
                    WebUser.SignInOfGener(emp);
                    return;
                }
                WebUser.SignInOfGener(emp);
                return;
            }



            if (emp.OrgNo.Equals(orgNo) == false)
            {
                //如果选择的组织不存在，就从他的隶属部门里去找一个部门。
                sql = "SELECT A.FK_Dept FROM Port_DeptEmp A, Port_Dept B WHERE A.FK_Dept=B.No AND A.FK_Emp='" + emp.No + "' AND B.OrgNo='" + orgNo + "' ";
                deptNo = DBAccess.RunSQLReturnString(sql);
                if (DataType.IsNullOrEmpty(deptNo) == true)
                    throw new Exception("err@用户[" + emp.No + "]在OrgNo[" + orgNo + "]下没有部门.");


            }

            emp.FK_Dept = deptNo;
            WebUser.SignInOfGener(emp);
        }
        /// <summary>
        /// 注销当前登录
        /// </summary>
        public static void Port_SigOut()
        {
            WebUser.Exit();
        }
        /// <summary>
        /// 获取未读的消息
        /// 用于消息提醒.
        /// </summary>
        /// <param name="userNo">用户ID</param>
        public static string Port_SMSInfo(string userNo)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT MyPK, EmailTitle  FROM sys_sms where SendTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SendTo AND IsAlert=0";
            ps.Add("SendTo", userNo);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += "@" + dr[0] + "=" + dr[1].ToString();
            }
            ps = new Paras();
            ps.SQL = "UPDATE  Sys_SMS SET IsAlert=1 WHERE  SendTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SendTo AND IsAlert=0";
            ps.Add("SendTo", userNo);
            DBAccess.RunSQL(ps);
            return strs;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userNo">信息接收人</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        public static void Port_SendMsg(string userNo, string msgTitle, string msgDoc, string msgFlag)
        {
            Port_SendMsg(userNo, msgTitle, msgDoc, msgFlag, BP.WF.SMSMsgType.Self, null, 0, 0, 0);
        }
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="logDev">设备</param>
        /// <returns></returns>
        public static string Port_GenerToken(string logDev = "PC", string token = "")
        {

            if (DataType.IsNullOrEmpty(token) == true)
                token = DBAccess.GenerGUID();

            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                Token mytk = new Token();
                mytk.MyPK = token;
                mytk.EmpNo = WebUser.No;
                mytk.EmpName = WebUser.Name;

                mytk.DeptNo = WebUser.FK_Dept;
                mytk.DeptName = WebUser.FK_DeptName;

                mytk.OrgNo = WebUser.OrgNo;
                mytk.OrgName = WebUser.OrgName;
                mytk.Insert();
                BP.Web.WebUser.Token = mytk.MyPK;
                return mytk.MyPK;
            }

            //单点模式,严格模式.
            if (SystemConfig.TokenModel == 1)
                return Port_GenerToken_2021(BP.Web.WebUser.No, logDev, 0, false);

            //记录token.
            BP.Port.Token tk = new Token();
            tk.MyPK = token;

            tk.EmpNo = BP.Web.WebUser.No;
            tk.EmpName = BP.Web.WebUser.Name;

            tk.DeptNo = BP.Web.WebUser.FK_Dept;
            tk.DeptName = BP.Web.WebUser.FK_DeptName;

            tk.OrgNo = BP.Web.WebUser.OrgNo;
            //  tk.OrgName = BP.Web.WebUser.OrgName;
            tk.RDT = DataType.CurrentDateTime; //记录日期.

            if (logDev.Equals("PC"))
                tk.SheBei = 0;
            else
                tk.SheBei = 1;
            tk.Insert();

            WebUser.Token = tk.MyPK;
            return tk.MyPK;
        }
        /// <summary>
        /// 外部写入token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="logDev"></param>
        /// <returns></returns>
        public static string Port_WriteToken(string token, string logDev = "PC")
        {
            if (DataType.IsNullOrEmpty(token) == true)
                throw new Exception("err@传入的Token值为空,请使用Port_GenerToken方法");
            if (SystemConfig.TokenModel == 1)
            {
                if (logDev == null)
                    logDev = "PC";

                string key = "Token_" + logDev; //para的参数.

                BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
                emp.SetPara("Online", "1");
                emp.SetPara(key, token);
                emp.Update();
                WebUser.Token = token;
                return token;
            }
            //记录token.
            BP.Port.Token tk = new Token();
            tk.MyPK = token;
            int i = tk.RetrieveFromDBSources();
            if (i == 1)
            {
                if (tk.EmpNo.Equals(WebUser.No) == false)
                    throw new Exception("err@传入的Token值在Port_Token中已经存在但是与登录账号不匹配");
                WebUser.Token = tk.MyPK;

                tk.DeptNo = BP.Web.WebUser.FK_Dept;
                tk.DeptName = BP.Web.WebUser.FK_DeptName;
                tk.OrgNo = BP.Web.WebUser.OrgNo;
                tk.OrgName = BP.Web.WebUser.OrgName;
                tk.Update();
                return token;
            }
            tk.EmpNo = BP.Web.WebUser.No;
            tk.EmpName = BP.Web.WebUser.Name;

            tk.DeptNo = BP.Web.WebUser.FK_Dept;
            tk.DeptName = BP.Web.WebUser.FK_DeptName;

            tk.OrgNo = BP.Web.WebUser.OrgNo;
            tk.OrgName = BP.Web.WebUser.OrgName;
            tk.RDT = DataType.CurrentDateTime; //记录日期.

            if (logDev.Equals("PC"))
                tk.SheBei = 0;
            else
                tk.SheBei = 1;
            tk.Insert();

            WebUser.Token = tk.MyPK;
            return tk.MyPK;

        }

        /// <summary>
        /// 获取Toekn. 旧版本.
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="logDev">登录设备</param>
        /// <param name="activeMinutes">有效时间</param>
        /// <param name="isGenerNewToken">是否生成新token.</param>
        /// <returns></returns>
        private static string Port_GenerToken_2021(string userNo, string logDev = "PC", int activeMinutes = 0, bool isGenerNewToken = false)
        {
            if (DataType.IsNullOrEmpty(logDev))
                logDev = "PC";

            if (activeMinutes == 0)
                activeMinutes = 300; //默认为300分钟.

            string key = "Token_" + logDev; //para的参数.
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                userNo = BP.Web.WebUser.OrgNo + "_" + userNo;
            }
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(userNo);
            emp.SetPara("Online", "1");
            emp.OrgNo = BP.Web.WebUser.OrgNo;

            string myGuid = emp.GetParaString(key); //获得token.
            string guidOID_Dt = emp.GetParaString(key + "_DT"); // token 的过期日期.

            #region 如果是第1次登录，就生成新的token.
            if (isGenerNewToken == true || DataType.IsNullOrEmpty(myGuid) == true || DataType.IsNullOrEmpty(guidOID_Dt) == true)
            {
                string guid = DBAccess.GenerGUID();
                emp.SetPara(key, guid);

                DateTime dt = DateTime.Now;
                dt = dt.AddMinutes(activeMinutes);

                emp.SetPara(key + "_DT", dt.ToString("yyyy-MM-dd HH:mm:ss"));
                emp.Update();
                WebUser.Token = guid;
                return guid;
            }
            #endregion 如果是第1次登录，就生成新的token.

            //判断是否超时.
            DateTime dtTo = DataType.ParseSysDateTime2DateTime(guidOID_Dt);
            if (dtTo < DateTime.Now)
            {
                //超时，就返回一个新的token.
                DateTime dtUpdate = DateTime.Now;
                dtUpdate = dtUpdate.AddMinutes(activeMinutes);

                myGuid = DBAccess.GenerGUID(); //生成新的GUID.

                emp.SetPara(key + "_DT", dtUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
                emp.SetPara(key, myGuid);
                emp.Update();
                WebUser.Token = myGuid;
            }
            else
            {
                //重新登录，需要重新计算超时时间
                DateTime dtUpdate = DateTime.Now;
                dtUpdate = dtUpdate.AddMinutes(activeMinutes);
                emp.SetPara(key + "_DT", dtUpdate.ToString("yyyy-MM-dd HH:mm:ss"));
                emp.Update();
            }
            WebUser.Token = myGuid;
            return myGuid;
        }
        /// <summary>
        /// 验证用户的合法性
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="Token">密钥</param>
        /// <returns>是否匹配</returns>
        public static bool Port_CheckUserLogin(string userNo, string SID)
        {
            return true;

            if (DataType.IsNullOrEmpty(userNo))
                return false;

            if (DataType.IsNullOrEmpty(SID))
                return false;

            Paras ps = new Paras();
            ps.SQL = "SELECT SID FROM Port_Emp WHERE No=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "No";
            ps.Add("No", userNo);

            string mysid = DBAccess.RunSQLReturnStringIsNull(ps, null);
            if (mysid == null)
                throw new Exception("@没有取得用户(" + userNo + ")的SID.");


            if (mysid == SID)
                return true;
            return false;
        }
        /// <summary>
        /// 发送邮件与消息(如果传入4大流程参数将会增加一个工作链接)
        /// </summary>
        /// <param name="userNo">信息接收人</param>
        /// <param name="title">标题</param>
        /// <param name="msgDoc">内容</param>
        /// <param name="msgFlag">消息标志</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        public static void Port_SendMsg(string userNo, string title, string msgDoc, string msgFlag, string msgType,
            string flowNo, Int64 nodeID, Int64 workID, Int64 fid, string pushModel = null)
        {
            string url = "";
            if (workID != 0)
            {
                //url = Glo.HostURL + "WF/Do.htm?Token=" + userNo + "_" + workID + "_" + nodeID;
                //url = url.Replace("//", "/");
                //url = url.Replace("//", "/");
                //if (msgType == BP.WF.SMSMsgType.DoPress)
                //    url = url + "&DoType=OF";
                //if (msgType == BP.WF.SMSMsgType.CC)
                //    url = url + "&DoType=DoOpenCC";

                //msgDoc += " <hr>打开工作: " + url;
            }
            if (DataType.IsNullOrEmpty(msgFlag) == true)
                msgFlag = "WKAlt" + nodeID + "_" + workID;

            string atParas = "@FK_Flow=" + flowNo + "@WorkID=" + workID + "@NodeID=" + nodeID + "@FK_Node=" + nodeID;
            BP.WF.SMS.SendMsg(userNo, title, msgDoc, msgFlag, msgType, atParas, workID, pushModel, url);
        }
        /// <summary>
        /// 写入消息
        /// 用途可以处理提醒.
        /// </summary>
        /// <param name="sendToUserNo">发送给的操作员ID</param>
        /// <param name="sendDT">发送时间，如果null 则表示立刻发送。</param>
        /// <param name="title">标题</param>
        /// <param name="doc">内容</param>
        /// <param name="msgFlag">消息标记</param>
        /// <returns>写入成功或者失败.</returns>
        public static bool Port_WriteToSMS(string sendToUserNo, string sendDT, string title, string doc, string msgFlag, Int64 workid)
        {
            SMS.SendMsg(sendToUserNo, title, doc, msgFlag, "Info", "", workid);
            return true;
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sendToEmpNo">接收人</param>
        /// <param name="smsDoc">消息内容</param>
        /// <param name="emailTitle">邮件标题</param>
        /// <param name="msgType">消息类型(例如工作到达后、发送成功后)</param>
        /// <param name="msgGroupFlag">消息分组（与消息类型有关联）</param>
        /// <param name="sendEmpNo">发送人</param>
        /// <param name="openUrl">连接URL</param>
        /// <param name="pushModel">可以接受消息的类型(如邮件、短信、丁丁、微信等)</param>
        /// <param name="msgPK">唯一标志,防止发送重复.</param>
        /// <param name="atParas">参数.</param>
        public static void Port_SendMessage(string sendToEmpNo, string smsDoc, string emailTitle, string msgType, string msgGroupFlag,
            string sendEmpNo, string openUrl, string pushModel, Int64 workID, string msgPK = null, string atParas = null)
        {
            BP.Port.Emp emp = null;
            try
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    emp = new BP.Port.Emp(WebUser.OrgNo + "_" + sendToEmpNo);
                }
                else
                {
                    emp = new BP.Port.Emp(sendToEmpNo);
                }
            }
            catch (Exception ex)
            {
                return; //没有这个人的信息，就return.
            }

            SMS sms = new SMS();
            if (DataType.IsNullOrEmpty(msgPK) == false)
            {
                /*如果有唯一标志,就判断是否有该数据，没有该数据就允许插入.*/
                if (sms.IsExit(SMSAttr.MyPK, msgPK) == true)
                    return;
                sms.setMyPK(msgPK);
            }
            else
            {
                sms.setMyPK(DBAccess.GenerGUID());
            }

            sms.HisEmailSta = MsgSta.UnRun;
            sms.HisMobileSta = MsgSta.UnRun;

            if (sendEmpNo == null)
            {
                sms.Sender = WebUser.No;
            }
            else
            {
                sms.Sender = sendEmpNo;
            }

            //发送给人员ID , 有可能这个人员空的.
            sms.SendToEmpNo = sendToEmpNo;

            #region 邮件信息
            //邮件地址.
            sms.Email = emp.Email;
            //邮件标题.
            sms.Title = emailTitle;
            sms.DocOfEmail = smsDoc;
            #endregion 邮件信息

            #region 短消息信息
            sms.Mobile = emp.Tel;
            sms.MobileInfo = smsDoc;
            sms.Title = emailTitle;
            #endregion 短消息信息

            // 其他属性.
            sms.RDT = DataType.CurrentDateTime;

            sms.MsgType = msgType; // 消息类型.

            sms.MsgFlag = msgGroupFlag; // 消息分组标志,用于批量删除.

            sms.AtPara = atParas;

            sms.WorkID = workID;


            sms.SetPara("OpenUrl", openUrl);
            sms.SetPara("PushModel", pushModel);

            // 先保留本机一份.
            sms.Insert();
        }


        /// <summary>
        /// 获取最新的消息
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="dateLastTime">上次获取的时间</param>
        /// <returns>返回消息：返回两个列的数据源MsgType,Num.</returns>
        public static DataTable Port_GetNewMsg(string userNo, string dateLastTime)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT MsgType , Count(*) as Num FROM Sys_SMS WHERE SendTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SendTo AND RDT >" + BP.Difference.SystemConfig.AppCenterDBVarStr + "RDT Group By MsgType";
            ps.Add(BP.WF.SMSAttr.SendTo, userNo);
            ps.Add(BP.WF.SMSAttr.RDT, dateLastTime);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            return dt;
        }
        /// <summary>
        /// 获取最新的消息
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns></returns>
        public static DataTable Port_GetNewMsg(string userNo)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT MsgType , Count(*) as Num FROM Sys_SMS WHERE SendTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SendTo  Group By MsgType";
            ps.Add(BP.WF.SMSAttr.SendTo, userNo);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            return dt;
        }
        #endregion 登陆接口

        #region 与流程有关的接口
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeFrom">节点从</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="msg">信息</param>
        /// <param name="at">活动类型</param>
        /// <param name="tag">参数:用@符号隔开比如, @PWorkID=101@PFlowNo=003</param>
        /// <param name="cFlowInfo">子流程信息</param>
        public static void WriteTrack(string flowNo, int nodeFromID, string nodeFromName, Int64 workid, Int64 fid, string msg, ActionType at, string tag,
            string cFlowInfo, string writeImg, string optionMsg = null, string empNoTo = null, string empNameTo = null, string empNoFrom = null, string empNameFrom = null, string rdt = null, string fwcView = null)
        {
            if (at == ActionType.CallChildenFlow)
            {
                if (DataType.IsNullOrEmpty(cFlowInfo) == true)
                {
                    throw new Exception("@必须输入信息cFlowInfo信息,在 CallChildenFlow 模式下.");
                }
            }

            if (DataType.IsNullOrEmpty(optionMsg))
            {
                optionMsg = Track.GetActionTypeT(at);
            }

            Track t = new Track();
            t.WorkID = workid;
            t.FID = fid;

            //记录日期.
            DateTime d;
            if (DataType.IsNullOrEmpty(rdt))
            {
                t.RDT = DataType.CurrentDateTimess;
            }
            else
            {
                t.RDT = rdt;
            }

            t.HisActionType = at;
            t.ActionTypeText = optionMsg;

            // Node nd = new Node(nodeFrom);
            t.NDFrom = nodeFromID;
            t.NDFromT = nodeFromName;

            if (empNoFrom == null)
            {
                t.EmpFrom = WebUser.No;
            }
            else
            {
                t.EmpFrom = empNoFrom;
            }

            if (empNameFrom == null)
            {
                t.EmpFromT = WebUser.Name;
            }
            else
            {
                t.EmpFromT = empNameFrom;
            }

            t.FK_Flow = flowNo;

            t.NDTo = nodeFromID;
            t.NDToT = nodeFromName;

            if (empNoTo == null)
            {
                t.EmpTo = WebUser.No;
                t.EmpToT = WebUser.Name;
            }
            else
            {
                t.EmpTo = empNoTo;
                t.EmpToT = empNameTo;
            }

            t.WriteDB = writeImg;
            t.Msg = msg;
            t.NodeData = "@DeptNo=" + WebUser.FK_Dept + "@DeptName=" + WebUser.FK_DeptName;

            if (tag != null)
            {
                t.Tag = tag;
            }
            if (fwcView != null)
            {
                t.Tag = t.Tag + fwcView;
            }

            try
            {
                t.Insert();

            }
            catch
            {
                t.CheckPhysicsTable();
                t.Insert();
                //t.DirectInsert();
            }

            #region 特殊判断.
            if (at == ActionType.CallChildenFlow)
            {
                /* 如果是吊起子流程，就要向它父流程信息里写数据，让父流程可以看到能够发起那些流程数据。*/
                AtPara ap = new AtPara(tag);
                BP.WF.GenerWorkFlow gwf = new GenerWorkFlow(ap.GetValInt64ByKey(GenerWorkFlowAttr.PWorkID));
                t.WorkID = gwf.WorkID;

                t.NDFrom = gwf.FK_Node;
                t.NDFromT = gwf.NodeName;

                t.NDTo = t.NDFrom;
                t.NDToT = t.NDFromT;

                t.FK_Flow = gwf.FK_Flow;

                t.HisActionType = ActionType.StartChildenFlow;
                t.Tag = "@CWorkID=" + workid + "@CFlowNo=" + flowNo;
                t.Msg = cFlowInfo;
                t.Insert();
            }
            #endregion 特殊判断.
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeFrom">节点从</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">fID</param>
        /// <param name="msg">信息</param>
        public static void WriteTrackInfo(string flowNo, int nodeFrom, string ndFromName, Int64 workid, Int64 fid, string msg, string optionMsg)
        {
            WriteTrack(flowNo, nodeFrom, ndFromName, workid, fid, msg, ActionType.Info, null, null, optionMsg);
        }
        /// <summary>
        /// 写入工作审核日志:
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="msg">审核信息</param>
        /// <param name="optionName">操作名称(比如:科长审核、部门经理审批),如果为空就是"审核".</param>
        public static void Node_WriteWorkCheck(Int64 workid, string msg, string optionName, string writeImg, string fwcView = null)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            gwf.RetrieveFromDBSources();

            string flowNo = gwf.FK_Flow;
            int currNodeID = gwf.FK_Node;
            Int64 fid = gwf.FID;

            //求主键 2017.10.6以前的逻辑.
            string tag = currNodeID + "_" + workid + "_" + fid + "_" + BP.Web.WebUser.No;

            //求当前是否是会签.  zhangsan,张三;李四;王五;
            string nodeName = gwf.NodeName;
            Node nd = new Node(currNodeID);
            if (nd.IsStartNode == false)
            {
                if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianing && gwf.HuiQianZhuChiRen != BP.Web.WebUser.No)
                {
                    nodeName = nd.Name + "(会签)";
                }
            }

            //待办抢办模式，一个节点只能有一条记录.
            Paras ps = new Paras();
            if (nd.TodolistModel == TodolistModel.QiangBan || nd.TodolistModel == TodolistModel.Sharing)
            {
                //先删除其他人员写入的数据. 此脚本是2016.11.30号的,为了解决柳州的问题，需要扩展.
                ps.SQL = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE  WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.NDFrom, currNodeID);
                DBAccess.RunSQL(ps);

                //写入日志.
                WriteTrack(flowNo, currNodeID, nodeName, workid, fid, msg, ActionType.WorkCheck, tag, null, writeImg, optionName, null, null, null, null, null, fwcView);
                return;
            }

            string trackTable = "ND" + int.Parse(flowNo) + "Track";
            ps.SQL = "UPDATE  " + trackTable + " SET NDFromT=" + dbStr + "NDFromT, Msg=" + dbStr + "Msg, RDT=" + dbStr +
                     "RDT,NodeData=" + dbStr + "NodeData WHERE ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + dbStr + "NDFrom AND  NDTo=" + dbStr + "NDTo AND WorkID=" + dbStr + "WorkID AND FID=" + dbStr + "FID AND EmpFrom=" + dbStr + "EmpFrom";
            ps.Add(TrackAttr.NDFromT, nodeName);
            ps.Add(TrackAttr.Msg, msg);
            ps.Add(TrackAttr.NDFrom, currNodeID);
            ps.Add(TrackAttr.NDTo, currNodeID);
            ps.Add(TrackAttr.WorkID, workid);
            ps.Add(TrackAttr.FID, fid);
            ps.Add(TrackAttr.EmpFrom, WebUser.No);
            ps.Add(TrackAttr.RDT, DataType.CurrentDateTimess);
            ps.Add(TrackAttr.NodeData, "@DeptNo=" + WebUser.FK_Dept + "@DeptName=" + WebUser.FK_DeptName);

            int num = DBAccess.RunSQL(ps);

            if (num == 1 && DataType.IsNullOrEmpty(writeImg) == false && writeImg.Contains("data:image/png;base64,") == true)
            {
                string myPK = DBAccess.RunSQLReturnStringIsNull("SELECT MyPK From " + trackTable + " Where ActionType=" + (int)ActionType.WorkCheck + " AND  NDFrom=" + currNodeID + " AND  NDTo=" + currNodeID + " AND WorkID=" + workid + " AND FID=" + fid + " AND EmpFrom='" + WebUser.No + "'", "");
                DBAccess.SaveBigTextToDB(writeImg, trackTable, "MyPK", myPK, "WriteDB");
            }

            if (num > 1)
            {
                ps.Clear();
                ps.SQL = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE  NDFrom=" + dbStr + "NDFrom AND  NDTo=" + dbStr + "NDTo AND WorkID=" + dbStr + "WorkID AND FID=" + dbStr + "FID AND EmpFrom=" + dbStr + "EmpFrom";
                ps.Add(TrackAttr.NDFrom, currNodeID);
                ps.Add(TrackAttr.NDTo, currNodeID);
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.FID, fid);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);
                DBAccess.RunSQL(ps);
                num = 0;
            }

            if (num == 0)
            {
                //如果没有更新到，就写入.
                WriteTrack(flowNo, currNodeID, nodeName, workid, fid, msg, ActionType.WorkCheck, tag, null, writeImg, optionName, null, null, null, null, null, fwcView);
            }
        }

        public static void WriteTrackWorkCheckForTangRenYiYao(string flowNo, int currNodeID, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            //WorkNode wn = new WorkNode(workid, currNodeID);
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            gwf.RetrieveFromDBSources();

            //求主键 2017.10.6以前的逻辑.
            string tag = gwf.Paras_LastSendTruckID + "_" + currNodeID + "_" + workid + "_" + fid + "_" + BP.Web.WebUser.No;

            string nodeName = gwf.NodeName;
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                nodeName = nodeName + "(会签)";
            }

            Node nd = new Node(currNodeID);
            //待办抢办模式，一个节点只能有一条记录.
            Paras ps = new Paras();
            if (nd.TodolistModel == TodolistModel.QiangBan || nd.TodolistModel == TodolistModel.Sharing)
            {
                //先删除其他人员写入的数据. 此脚本是2016.11.30号的,为了解决柳州的问题，需要扩展.
                ps.SQL = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE  WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND ActionType=" + (int)ActionType.WorkCheck + " AND Tag LIKE '" + gwf.Paras_LastSendTruckID + "%'";
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.NDFrom, currNodeID);
                DBAccess.RunSQL(ps);

                ////先删除其他人员写入的数据.
                ////string sql = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE  Tag LIKE '" + gwf.Paras_LastSendTruckID + "%' AND EmpFrom='"+BP.Web.WebUser.No+"' ";
                //string sql = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE  Tag LIKE '" + gwf.Paras_LastSendTruckID + "%'";
                //DBAccess.RunSQL(ps);
                //写入日志
                WriteTrack(flowNo, currNodeID, nodeName, workid, fid, msg, ActionType.WorkCheck, tag, null, optionName);
            }
            else
            {
                ps.SQL = "UPDATE  ND" + int.Parse(flowNo) + "Track SET NDFromT=" + dbStr + "NDFromT, Msg=" + dbStr + "Msg,RDT=" + dbStr +
                         "RDT WHERE  Tag=" + dbStr + "Tag";
                ps.Add(TrackAttr.NDFromT, nodeName);
                ps.Add(TrackAttr.Msg, msg);
                ps.Add(TrackAttr.Tag, tag);
                ps.Add(TrackAttr.RDT, DataType.CurrentDateTimess);
                if (DBAccess.RunSQL(ps) == 0)
                {
                    //如果没有更新到，就写入.
                    WriteTrack(flowNo, currNodeID, nodeName, workid, fid, msg, ActionType.WorkCheck, tag, null, optionName, null, null);
                }
            }
        }
        /// <summary>
        /// 写入日志组件
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeFrom"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="msg"></param>
        /// <param name="optionName"></param>
        public static void WriteTrackDailyLog(string flowNo, int nodeFrom, string nodeFromName, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            string today = DataType.CurrentDate;

            Paras ps = new Paras();
            ps.SQL = "UPDATE  ND" + int.Parse(flowNo) + "Track SET Msg=" + dbStr + "Msg WHERE  RDT LIKE '" + today + "%' AND WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND ActionType=" + (int)ActionType.WorkCheck;
            ps.Add(TrackAttr.Msg, msg);
            ps.Add(TrackAttr.WorkID, workid);
            ps.Add(TrackAttr.NDFrom, nodeFrom);
            ps.Add(TrackAttr.EmpFrom, WebUser.No);
            if (DBAccess.RunSQL(ps) == 0)
            {
                //如果没有更新到，就写入.
                WriteTrack(flowNo, nodeFrom, nodeFromName, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
            }
        }
        /// <summary>
        /// 写入周报组件 一旦写入数据,只可更新   每周一次 qin
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeFrom"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="msg"></param>
        /// <param name="optionName"></param>
        public static void WriteTrackWeekLog(string flowNo, int nodeFrom, string nodeFromName, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            DateTime dTime = DateTime.Now;
            DateTime startWeek = dTime.AddDays(1 - Convert.ToInt32(dTime.DayOfWeek.ToString("d"))); //本周第一天

            Hashtable ht = new Hashtable();//当前日期所属的week包含哪些日期
            for (int i = 1; i < 7; i++)
            {
                ht.Add(i + 1, startWeek.AddDays(i).ToString("yyyy-MM-dd"));
            }
            ht.Add(1, startWeek.ToString("yyyy-MM-dd"));

            bool isExitWeek = false;  //本周是否已经有插入数据
            string insertDate = null;
            DataTable dt;
            string sql = null;

            foreach (DictionaryEntry de in ht)
            {
                sql = "SELECT * FROM ND" + int.Parse(flowNo) +
                    "Track  WHERE  RDT LIKE '" + de.Value.ToString() + "%' AND WorkID=" + workid + "  AND NDFrom='" +
                    nodeFrom + "' AND EmpFrom='" + WebUser.No + "' AND ActionType=" + (int)ActionType.WorkCheck;

                if (DBAccess.RunSQLReturnCOUNT(sql) != 0)
                {
                    isExitWeek = true;
                    insertDate = de.Value.ToString();
                    break;
                }
            }

            //如果本周已经插入了记录，那么更新 
            if (isExitWeek)
            {
                Paras ps = new Paras();
                ps.SQL = "UPDATE  ND" + int.Parse(flowNo) + "Track SET RDT='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',Msg=" + dbStr + "Msg WHERE  RDT LIKE '" + insertDate + "%' AND WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.Msg, msg);
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.NDFrom, nodeFrom);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);

                DBAccess.RunSQL(ps);
            }
            else
            {
                WriteTrack(flowNo, nodeFrom, nodeFromName, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
            }
        }
        /// <summary>
        /// 写入月报组件  同周报一样每月一条记录 qin
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeFrom"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="msg"></param>
        /// <param name="optionName"></param>
        public static void WriteTrackMonthLog(string flowNo, int nodeFrom, string nodeFromName, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            string today = DataType.CurrentDate;

            DateTime dTime = DateTime.Now;
            DateTime startDay = dTime.AddDays(1 - dTime.Day);   //本月第一天 

            int days = DateTime.DaysInMonth(dTime.Year, dTime.Month);
            Hashtable ht = new Hashtable();

            for (int i = 1; i < days; i++)
            {
                ht.Add(i + 1, startDay.AddDays(i).ToString("yyyy-MM-dd"));
            }
            ht.Add(1, startDay.ToString("yyyy-MM-dd"));

            bool isExitMonth = false;  //本月是否已经有插入数据
            string insertDate = null;
            DataTable dt;
            string sql = null;

            foreach (DictionaryEntry de in ht)
            {
                sql = "SELECT * FROM ND" + int.Parse(flowNo) +
                    "Track  WHERE  RDT LIKE '" + de.Value.ToString() + "%' AND WorkID=" + workid + "  AND NDFrom='" +
                    nodeFrom + "' AND EmpFrom='" + WebUser.No + "' AND ActionType=" + (int)ActionType.WorkCheck;

                if (DBAccess.RunSQLReturnCOUNT(sql) != 0)
                {
                    isExitMonth = true;
                    insertDate = de.Value.ToString();
                    break;
                }
            }

            if (isExitMonth)
            {
                Paras ps = new Paras();
                ps.SQL = "UPDATE  ND" + int.Parse(flowNo) + "Track SET RDT='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' Msg=" + dbStr + "Msg WHERE  RDT LIKE '" + insertDate + "%' AND WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.Msg, msg);
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.NDFrom, nodeFrom);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);

                DBAccess.RunSQL(ps);
            }
            else
            {
                WriteTrack(flowNo, nodeFrom, nodeFromName, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
            }
        }
        /// <summary>
        /// 修改审核信息标识
        /// 比如：在默认的情况下是“审核”，现在要把ActionTypeText 修改成“组长审核。”。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="nodeFrom">节点ID</param>
        /// <param name="label">要修改成的标签</param>
        /// <returns>是否成功</returns>
        public static bool WriteTrackWorkCheckLabel(string flowNo, Int64 workid, int nodeFrom, string label)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT MyPK FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND WorkID=" + workid + " And NDTo='0' ORDER BY RDT DESC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string pk = dt.Rows[0][0].ToString();
            sql = "UPDATE " + table + " SET " + TrackAttr.ActionTypeText + "='" + label + "' WHERE MyPK=" + pk;
            DBAccess.RunSQL(sql);
            return true;
        }

        /// <summary>
        /// 前进,获取等标签
        /// 比如：在默认的情况下是“逻辑删除”，现在要把ActionTypeText 修改成“删除(清场)。”。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="nodeFrom">节点ID</param>
        /// <param name="label">要修改成的标签</param>
        /// <returns>是否成功</returns>
        public static bool WriteTrackLabel(string flowNo, Int64 workid, int nodeFrom, string label)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT MyPK FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND WorkID=" + workid + "  ORDER BY RDT DESC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string pk = dt.Rows[0][0].ToString();
            sql = "UPDATE " + table + " SET " + TrackAttr.ActionTypeText + "='" + label + "' WHERE MyPK=" + pk;
            DBAccess.RunSQL(sql);
            return true;
        }
        /// <summary>
        /// 获取Track 表中的审核的信息
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workId"></param>
        /// <param name="nodeFrom"></param>
        /// <returns></returns>
        public static string GetCheckInfo(string flowNo, Int64 workId, int nodeFrom, string isNullAsVal = null)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT Msg FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.WorkCheck + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + workId + " ORDER BY RDT DESC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                if (isNullAsVal == null)
                    return BP.WF.Glo.DefVal_WF_Node_FWCDefInfo;
                else
                    return isNullAsVal;
            }
            string checkinfo = dt.Rows[0][0].ToString();
            if (DataType.IsNullOrEmpty(checkinfo))
            {
                if (isNullAsVal == null)
                    return BP.WF.Glo.DefVal_WF_Node_FWCDefInfo;
                else
                    return isNullAsVal;
            }

            return checkinfo;
        }

        public static string GetCheckTag(string flowNo, Int64 workId, int nodeFrom, string empFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT Tag FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.WorkCheck + " AND EmpFrom='" + empFrom + "' AND WorkID=" + workId + " ORDER BY RDT DESC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                return "";
            }
            string checkinfo = dt.Rows[0][0].ToString();
            if (DataType.IsNullOrEmpty(checkinfo))
            {
                return "";
            }

            return checkinfo;
        }

        /// <summary>
        /// 获取队列节点Track 表中的审核的信息(队列节点中处理人 共享同一处理意见)
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workId"></param>
        /// <param name="nodeFrom"></param>
        /// <returns></returns>
        public static string GetOrderCheckInfo(string flowNo, Int64 workId, int nodeFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT Msg FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.WorkCheck + " AND WorkID=" + workId + " ORDER BY RDT DESC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                //BP.Sys.FrmWorkCheck fwc = new FrmWorkCheck(nodeFrom);
                //return fwc.FWCDefInfo;
                return null;
            }
            string checkinfo = dt.Rows[0][0].ToString();
            return checkinfo;
        }
        /// <summary>
        /// 删除审核信息,用于退回后.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workId">工作ID</param>
        /// <param name="nodeFrom">节点从</param>
        /// <returns>删除自己的审核信息</returns>
        public static void DeleteCheckInfo(string flowNo, Int64 workId, int nodeFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "DELETE FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.WorkCheck + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + workId;
            DBAccess.RunSQL(sql);
        }
        public static string GetAskForHelpReInfo(string flowNo, Int64 workId, int nodeFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT Msg FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.AskforHelp + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + workId + " ORDER BY RDT DESC ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                return "";
            }

            string checkinfo = dt.Rows[0][0].ToString();
            return checkinfo;
        }

        /// <summary>
        /// 更新Track信息
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workId"></param>
        /// <param name="nodeFrom"></param>
        /// <param name="actionType"></param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        public static void SetTrackInfo(string flowNo, Int64 workId, int nodeFrom, int actionType, string strMsg)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + table + " SET Msg=" + dbstr + "Msg  WHERE ActionType=" + dbstr +
                "ActionType and WorkID=" + dbstr + "WorkID and NDFrom=" + dbstr + "NDFrom";
            ps.Add("Msg", strMsg);
            ps.Add("ActionType", actionType);
            ps.Add("WorkID", workId);
            ps.Add("NDFrom", nodeFrom);
            DBAccess.RunSQL(ps);
        }

        /// <summary>
        /// 设置BillNo信息
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workID"></param>
        /// <param name="newBillNo"></param>
        public static void SetBillNo(string flowNo, Int64 workID, string newBillNo)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET BillNo=" + dbstr + "BillNo  WHERE WorkID=" + dbstr + "WorkID";
            ps.Add("BillNo", newBillNo);
            ps.Add("WorkID", workID);
            DBAccess.RunSQL(ps);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET BillNo=" + dbstr + "BillNo WHERE OID=" + dbstr + "OID";
            ps.Add("BillNo", newBillNo);
            ps.Add("OID", workID);
            DBAccess.RunSQL(ps);
        }

        /// <summary>
        /// 设置父流程信息 
        /// </summary>
        /// <param name="subFlowNo">子流程编号</param>
        /// <param name="subFlowWorkID">子流程workid</param>
        /// <param name="parentWorkID">父流程WorkID</param>
        /// <param name="isCopyParentNo">是否要copy父流程的数据</param>
        public static void SetParentInfo(string subFlowNo, Int64 subFlowWorkID, Int64 parentWorkID,
            string parentEmpNo = null, int parentNodeID = 0, bool isCopyParentNo = false)
        {
            //创建父流程.
            GenerWorkFlow pgwf = new GenerWorkFlow(parentWorkID);

            if (parentNodeID != 0)
                pgwf.FK_Node = parentNodeID;

            if (parentEmpNo == null)
                parentEmpNo = WebUser.No;

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET PFlowNo=" + dbstr + "PFlowNo, PWorkID=" + dbstr + "PWorkID,PNodeID=" + dbstr + "PNodeID,PEmp=" + dbstr + "PEmp WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.PFlowNo, pgwf.FK_Flow);
            ps.Add(GenerWorkFlowAttr.PWorkID, parentWorkID);
            ps.Add(GenerWorkFlowAttr.PNodeID, pgwf.FK_Node);
            ps.Add(GenerWorkFlowAttr.PEmp, parentEmpNo);
            ps.Add(GenerWorkFlowAttr.WorkID, subFlowWorkID);

            DBAccess.RunSQL(ps);

            Flow fl = new Flow(subFlowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET PFlowNo=" + dbstr + "PFlowNo, PWorkID=" + dbstr + "PWorkID,PNodeID=" + dbstr + "PNodeID, PEmp=" + dbstr + "PEmp WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.PFlowNo, pgwf.FK_Flow);
            ps.Add(GERptAttr.PWorkID, pgwf.WorkID);
            ps.Add(GERptAttr.PNodeID, pgwf.FK_Node);
            ps.Add(GERptAttr.PEmp, parentEmpNo);
            ps.Add(GERptAttr.OID, subFlowWorkID);
            DBAccess.RunSQL(ps);

            //把数据copy过来.
            if (isCopyParentNo == true)
                Flow_CopyDataFromSpecWorkID(subFlowWorkID, pgwf.WorkID);
        }

        /// <summary>
        /// 复制数据从指定的WorkID里
        /// </summary>
        /// <param name="workID">当前workID</param>
        /// <param name="fromWorkID"></param>
        public static void Flow_CopyDataFromSpecWorkID(Int64 workID, Int64 fromWorkID)
        {
            #region  当前数据对象.
            Emp emp = new Emp(WebUser.No);
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            Node nd = new Node(gwf.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.Retrieve();
            Flow flow = nd.HisFlow;
            GERpt rpt = flow.HisGERpt;
            rpt.OID = workID;
            rpt.Retrieve();
            #endregion  当前数据对象.

            #region  数据源对象.
            GenerWorkFlow gwfFrom = new GenerWorkFlow(fromWorkID);
            Flow pFlow = new Flow(gwfFrom.FK_Flow);

            string sql = "SELECT * FROM " + pFlow.PTable + " WHERE OID=" + fromWorkID;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@不应该查询不到父流程的数据[" + sql + "], 可能的情况之一,请确认该父流程的调用节点是子线程，但是没有把子线程的FID参数传递进来。");
            #endregion  数据源对象.

            wk.Copy(dt.Rows[0]);
            rpt.Copy(dt.Rows[0]);
            //设置单号为空.
            wk.SetValByKey("BillNo", "");
            rpt.BillNo = "";

            #region 从调用的节点上copy.
            BP.WF.Node fromNd = new BP.WF.Node(gwfFrom.FK_Node);
            Work wkFrom = fromNd.HisWork;
            wkFrom.OID = fromWorkID;
            if (wkFrom.RetrieveFromDBSources() == 0)
                throw new Exception("@父流程的工作ID不正确，没有查询到数据" + fromWorkID);
            //wk.Copy(wkFrom);
            //rpt.Copy(wkFrom);
            #endregion 从调用的节点上copy.



            #region 特殊赋值.
            /*如果不是 执行的从已经完成的流程copy.*/
            wk.SetValByKey(GERptAttr.PFlowNo, gwfFrom.FK_Flow);
            wk.SetValByKey(GERptAttr.PNodeID, gwfFrom.FK_Node);
            wk.SetValByKey(GERptAttr.PWorkID, gwfFrom.WorkID);


            //忘记了增加这句话.
            rpt.SetValByKey(GERptAttr.PEmp, WebUser.No);

            rpt.SetValByKey(GERptAttr.FID, 0);
            rpt.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDateTime);
            rpt.SetValByKey(GERptAttr.FlowEnderRDT, DataType.CurrentDateTime);
            rpt.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);
            rpt.SetValByKey(GERptAttr.FlowStarter, emp.UserID);
            rpt.SetValByKey(GERptAttr.FlowEnder, emp.UserID);
            rpt.SetValByKey(GERptAttr.FlowEndNode, gwf.FK_Node);
            rpt.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
            rpt.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.Name + "@");

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.UserID + "@");

            if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.UserID + "," + emp.Name + "@");


            if (rpt.EnMap.PhysicsTable.Equals(wk.EnMap.PhysicsTable) == false)
                wk.Update(); //更新工作节点数据.
            rpt.Update(); // 更新流程数据表.
            #endregion 特殊赋值.

            #region 复制其他数据..
            //复制明细。
            MapDtls dtls = wk.HisMapDtls;
            if (dtls.Count > 0)
            {
                MapDtls dtlsFrom = wkFrom.HisMapDtls;
                int idx = 0;
                if (dtlsFrom.Count == dtls.Count)
                {
                    foreach (MapDtl dtl in dtls)
                    {
                        if (dtl.IsCopyNDData == false)
                            continue;

                        //new 一个实例.
                        GEDtl dtlData = new GEDtl(dtl.No);

                        //删除以前的数据.
                        try
                        {
                            sql = "DELETE FROM " + dtlData.EnMap.PhysicsTable + " WHERE RefPK=" + wk.OID;
                            DBAccess.RunSQL(sql);
                        }
                        catch (Exception ex)
                        {
                        }

                        MapDtl dtlFrom = dtlsFrom[idx] as MapDtl;

                        GEDtls dtlsFromData = new GEDtls(dtlFrom.No);
                        dtlsFromData.Retrieve(GEDtlAttr.RefPK, fromWorkID);
                        foreach (GEDtl geDtlFromData in dtlsFromData)
                        {
                            //是否启用多附件
                            FrmAttachmentDBs dbs = null;
                            if (dtl.IsEnableAthM == true)
                            {
                                //根据从表的OID 获取附件信息
                                dbs = new FrmAttachmentDBs();
                                dbs.Retrieve(FrmAttachmentDBAttr.RefPKVal, geDtlFromData.OID);
                            }

                            dtlData.Copy(geDtlFromData);
                            dtlData.RefPK = wk.OID.ToString();
                            dtlData.FID = wk.OID;
                            if (flow.No.Equals(gwf.FK_Flow) == false && (flow.StartLimitRole == WF.StartLimitRole.OnlyOneSubFlow))
                            {
                                dtlData.SaveAsOID(geDtlFromData.OID); //为子流程的时候，仅仅允许被调用1次.
                            }
                            else
                            {
                                dtlData.InsertAsNew();
                                if (dbs != null && dbs.Count != 0)
                                {
                                    //复制附件信息
                                    FrmAttachmentDB newDB = new FrmAttachmentDB();
                                    foreach (FrmAttachmentDB db in dbs)
                                    {
                                        newDB.Copy(db);
                                        newDB.RefPKVal = dtlData.OID.ToString();
                                        newDB.FID = dtlData.OID;
                                        newDB.setMyPK(DBAccess.GenerGUID());
                                        newDB.Insert();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //复制附件数据。
            if (wk.HisFrmAttachments.Count > 0)
            {
                if (wkFrom.HisFrmAttachments.Count > 0)
                {
                    int toNodeID = wk.NodeID;

                    //删除数据。
                    DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + toNodeID + "' AND RefPKVal='" + wk.OID + "'");
                    FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + gwfFrom.FK_Node, gwfFrom.WorkID.ToString());

                    foreach (FrmAttachmentDB athDB in athDBs)
                    {
                        FrmAttachmentDB athDB_N = new FrmAttachmentDB();
                        athDB_N.Copy(athDB);
                        athDB_N.setFK_MapData("ND" + toNodeID);
                        athDB_N.RefPKVal = wk.OID.ToString();
                        athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + gwfFrom.FK_Node,
                          "ND" + toNodeID);

                        if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
                        {
                            /*如果是单附件.*/
                            athDB_N.setMyPK(athDB_N.FK_FrmAttachment + "_" + wk.OID);
                            if (athDB_N.IsExits == true)
                                continue; /*说明上一个节点或者子线程已经copy过了, 但是还有子线程向合流点传递数据的可能，所以不能用break.*/
                            athDB_N.Insert();
                        }
                        else
                        {
                            athDB_N.setMyPK(athDB_N.UploadGUID + "_" + athDB_N.FK_MapData + "_" + wk.OID);
                            athDB_N.Insert();
                        }
                    }
                }
            }
            #endregion 复制表单其他数据.

            #region 复制独立表单数据.
            //求出来被copy的节点有多少个独立表单.
            FrmNodes fnsFrom = new FrmNodes(fromNd.NodeID);
            if (fnsFrom.Count != 0)
            {
                //求当前节点表单的绑定的表单.
                FrmNodes fns = new FrmNodes(nd.NodeID);
                if (fns.Count != 0)
                {
                    //开始遍历当前绑定的表单.
                    foreach (FrmNode fn in fns)
                    {
                        foreach (FrmNode fnFrom in fnsFrom)
                        {
                            if (fn.FK_Frm != fnFrom.FK_Frm)
                                continue;

                            BP.Sys.GEEntity geEnFrom = new GEEntity(fnFrom.FK_Frm);
                            geEnFrom.OID = gwfFrom.WorkID;
                            if (geEnFrom.RetrieveFromDBSources() == 0)
                                continue;

                            //执行数据copy , 复制到本身. 
                            geEnFrom.CopyToOID(wk.OID);
                        }
                    }
                }
            }
            #endregion 复制独立表单数据.
        }

        public static GERpt Flow_GenerGERpt(string flowNo, Int64 workID)
        {
            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt", workID);
            return rpt;
        }
        /// <summary>
        /// 产生一个新的工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>返回当前操作员创建的工作</returns>
        public static Work Flow_GenerWork(string flowNo)
        {
            Flow fl = new Flow(flowNo);
            Work wk = fl.NewWork();
            wk.ResetDefaultVal();
            return wk;
        }
        /// <summary>
        /// 把流程从非正常运行状态恢复到正常运行状态
        /// 比如现在的流程的状态是，删除，挂起，现在恢复成正常运行。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">原因</param>
        /// <returns>执行信息</returns>
        public static void Flow_DoComeBackWorkFlow(string flowNo, Int64 workID, string msg)
        {
            WorkFlow wf = new WorkFlow(workID);
            wf.DoComeBackWorkFlow(msg);
        }
        /// <summary>
        /// 恢复已完成的流程数据到指定的节点，如果节点为0就恢复到最后一个完成的节点上去.
        /// 恢复失败抛出异常
        /// </summary>
        /// <param name="flowNo">要恢复的流程编号</param>
        /// <param name="workid">要恢复的workid</param>
        /// <param name="backToNodeID">恢复到的节点编号，如果是0，标示回复到流程最后一个节点上去.</param>
        /// <param name="note">恢复的原因，此原因会记录到日志.</param>
        public static string Flow_DoRebackWorkFlow(string flowNo, Int64 workid,
            int backToNodeID, string note)
        {
            BP.WF.Template.FlowSheet fs = new BP.WF.Template.FlowSheet(flowNo);
            return fs.DoRebackFlowData(workid, backToNodeID, note);
        }
        /// <summary>
        /// 执行删除流程:彻底的删除流程.
        /// 清除的内容如下:
        /// 1, 流程引擎中的数据.
        /// 2, 节点数据,NDxxRpt数据.
        /// 3, 轨迹表数据.
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否要删除它的子流程</param>
        /// <returns>执行信息</returns>
        public static string Flow_DoDeleteFlowByReal(Int64 workID, bool isDelSubFlow = false)
        {
            //if (WebUser.IsAdmin==false)
            //    throw 
            try
            {
                WorkFlow.DeleteFlowByReal(workID, isDelSubFlow);
            }
            catch (Exception ex)
            {
                throw new Exception("err@删除前错误，" + ex.StackTrace);
            }
            return "删除成功";
        }
        public static string Flow_DoDeleteDraft(string flowNo, Int64 workID, bool isDelSubFlow)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            gwf.RetrieveFromDBSources();
            if (gwf.Starter != WebUser.No && WebUser.IsAdmin == false)
                return "err@流程不是您发起的，或者您不是管理员所以您不能删除该草稿。";

            //删除流程。
            gwf.Delete();

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            Paras ps = new Paras();

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "DELETE FROM " + fl.PTable + " WHERE OID=" + dbstr + "OID";
            ps.Add("OID", workID);
            DBAccess.RunSQL(ps);

            //删除开始节点数据.
            Node nd = fl.HisStartNode;
            Work wk = nd.HisWork;
            ps = new Paras();
            ps.SQL = "DELETE FROM " + wk.EnMap.PhysicsTable + " WHERE OID=" + dbstr + "OID";
            ps.Add("OID", workID);
            DBAccess.RunSQL(ps);

            BP.DA.Log.DebugWriteError(WebUser.Name + "删除了FlowNo 为'" + flowNo + "',workID为'" + workID + "'的数据");

            return "草稿删除成功";
        }
        /// <summary>
        /// 删除已经完成的流程
        /// 注意:它不触发事件.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否删除子流程</param>
        /// <param name="note">删除原因</param>
        /// <returns>删除过程信息</returns>
        public static string Flow_DoDeleteWorkFlowAlreadyComplete(string flowNo, Int64 workID, bool isDelSubFlow, string note)
        {
            return WorkFlow.DoDeleteWorkFlowAlreadyComplete(flowNo, workID, isDelSubFlow, note);
        }
        /// <summary>
        /// 删除流程并写入日志
        /// 清除的内容如下:
        /// 1, 流程引擎中的数据.
        /// 2, 节点数据,NDxxRpt数据.
        /// 并作如下处理:
        /// 1, 保留track数据.
        /// 2, 写入流程删除记录表.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="deleteNote">删除原因</param>
        /// <param name="isDelSubFlow">是否要删除它的子流程</param>
        /// <returns>执行信息</returns>
        public static string Flow_DoDeleteFlowByWriteLog(string flowNo, Int64 workID, string deleteNote, bool isDelSubFlow)
        {
            WorkFlow wf = new WorkFlow(workID);
            return wf.DoDeleteWorkFlowByWriteLog(deleteNote, isDelSubFlow);
        }
        /// <summary>
        /// 执行逻辑删除流程:此流程并非真正的删除仅做了流程删除标记
        /// 比如:逻辑删除工单.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">逻辑删除的原因</param>
        /// <param name="isDelSubFlow">逻辑删除的原因</param>
        /// <returns>执行信息,执行不成功抛出异常.</returns>
        public static string Flow_DoDeleteFlowByFlag(Int64 workID, string msg, bool isDelSubFlow)
        {
            WorkFlow wf = new WorkFlow(workID);
            wf.DoDeleteWorkFlowByFlag(msg);
            if (isDelSubFlow)
            {
                //删除子线程
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.FID, workID);
                foreach (GenerWorkFlow item in gwfs)
                {
                    Flow_DoDeleteFlowByFlag(item.WorkID, "删除子流程:" + msg, false);
                }
                //删除子流程
                gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workID);
                foreach (GenerWorkFlow item in gwfs)
                {
                    Flow_DoDeleteFlowByFlag(item.WorkID, "删除子流程:" + msg, false);
                }
            }
            return "删除成功";
        }
        /// <summary>
        /// 撤销删除流程
        /// 说明:如果一个流程处于逻辑删除状态,要回复正常运行状态,就执行此接口.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作流程ID</param>
        /// <param name="msg">撤销删除的原因</param>
        /// <returns>执行消息,如果撤销不成功则抛出异常.</returns>
        public static string Flow_DoUnDeleteFlowByFlag(string flowNo, Int64 workID, string msg)
        {
            WorkFlow wf = new WorkFlow(workID);
            wf.DoUnDeleteWorkFlowByFlag(msg);
            return "撤销删除成功.";
        }
        /// <summary>
        /// 执行-撤销发送
        /// 说明:如果流程转入了下一个节点,就会执行失败,就会抛出异常.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回成功执行信息</returns>
        public static string Flow_DoUnSend(string flowNo, Int64 workID, int unSendToNode = 0, Int64 fid = 0)
        {
            if (unSendToNode == 0)
            {
                //获取用户当前所在的节点
                String currNode = "";
                switch (DBAccess.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.KingBaseR3:
                    case DBType.KingBaseR6:
                        currNode = "SELECT FK_Node FROM (SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC ) WHERE rownum=1";
                        break;
                    case DBType.MySQL:
                    case DBType.PostgreSQL:
                    case DBType.UX:
                        currNode = "SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC LIMIT 1";
                        break;
                    case DBType.MSSQL:
                        currNode = "SELECT TOP 1 FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC";
                        break;
                    default:
                        currNode = "SELECT  FK_Node FROM WF_GenerWorkerlist WHERE FK_Emp='" + WebUser.No + "' Order by RDT DESC";
                        break;
                }
                unSendToNode = DBAccess.RunSQLReturnValInt(currNode, 0);
            }

            WorkUnSend unSend = new WorkUnSend(flowNo, workID, unSendToNode, fid);
            unSend.UnSendToNode = unSendToNode;

            return unSend.DoUnSend();
        }

        /// <summary>
        /// 获得当前节点上一步发送日志记录
        /// </summary>
        /// <param name="WorkID">工作流程ID</param>
        /// <param name="FK_Node">当前节点编号</param>
        /// <returns>上一节点发送记录</returns>
        public static DataTable Flow_GetPreviousNodeTrack(Int64 WorkID, int FK_Node)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(WorkID);
            if (gwf.RetrieveFromDBSources() == 0)
                throw new Exception("err@没有查询到相关业务实例");

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras pas = new Paras();
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    pas.SQL = "SELECT TOP 1 MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Tag,Exer FROM ND" + int.Parse(gwf.FK_Flow) + "Track WHERE WorkID=" + dbstr + "WorkID  AND NDTo=" + dbstr + "NDTo AND (ActionType=1 OR ActionType=" + (int)ActionType.Skip + ") ORDER BY RDT DESC";
                    break;
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    pas.SQL = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Tag,Exer FROM ND" + int.Parse(gwf.FK_Flow) + "Track  WHERE WorkID=" + dbstr + "WorkID  AND NDTo=" + dbstr + "NDTo AND (ActionType=1 OR ActionType=" + (int)ActionType.Skip + ") AND ROWNUM=1 ORDER BY RDT DESC ";
                    break;
                case DBType.MySQL:
                    pas.SQL = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Tag,Exer FROM ND" + int.Parse(gwf.FK_Flow) + "Track  WHERE WorkID=" + dbstr + "WorkID AND NDTo=" + dbstr + "NDTo AND (ActionType=1 OR ActionType=" + (int)ActionType.Skip + ") ORDER BY RDT DESC limit 0,1 ";
                    break;
                case DBType.PostgreSQL:
                case DBType.UX:
                    pas.SQL = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Tag,Exer FROM ND" + int.Parse(gwf.FK_Flow) + "Track  WHERE WorkID=" + dbstr + "WorkID AND NDTo=" + dbstr + "NDTo AND (ActionType=1 OR ActionType=" + (int)ActionType.Skip + ") ORDER BY RDT DESC limit 1 ";
                    break;
                default:
                    break;
            }
            pas.Add("WorkID", WorkID);
            pas.Add("NDTo", FK_Node);
            return DBAccess.RunSQLReturnTable(pas);
        }

        /// <summary>
        /// 执行冻结
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="isFixSubFlows">是否冻结子流程？</param>
        /// <param name="msg">冻结原因.</param>
        /// <returns>冻结的信息.</returns>
        public static string Flow_DoFix(Int64 workid, bool isFixSubFlows, string msg)
        {
            string info = "";
            try
            {
                // 执行冻结.
                WorkFlow wf = new WorkFlow(workid);
                info = wf.DoFix(msg);
            }
            catch (Exception ex)
            {
                info += ex.Message;
            }

            if (isFixSubFlows == false)
                return info;

            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workid);

            foreach (GenerWorkFlow item in gwfs)
            {
                try
                {
                    // 执行冻结.
                    WorkFlow wf = new WorkFlow(item.WorkID);
                    info += wf.DoFix(msg);
                    GenerWorkFlows subgwfs = new GenerWorkFlows();
                    subgwfs.Retrieve(GenerWorkFlowAttr.PWorkID, item.WorkID);
                    foreach (GenerWorkFlow subitem in subgwfs)
                    {
                        try
                        {
                            // 执行冻结.
                            WorkFlow subwf = new WorkFlow(subitem.WorkID);
                            info += subwf.DoFix(msg);
                        }
                        catch (Exception ex)
                        {
                            info += "err@" + ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    info += "err@" + ex.Message;
                }
            }

            return info;
        }
        /// <summary>
        /// 执行解除冻结
        /// 于挂起的区别是,冻结需要有权限的人才可以执行解除冻结，
        /// 挂起是自己的工作可以挂起也可以解除挂起。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">workid</param>
        /// <param name="msg">解除原因</param>
        public static string Flow_DoUnFix(Int64 workid, string msg)
        {
            // 执行冻结.
            WorkFlow wf = new WorkFlow(workid);
            return wf.DoUnFix(msg);
        }
        /// <summary>
        /// 执行流程结束
        /// 说明:正常的流程结束.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">流程结束原因</param>
        /// <param name="stopFlowType">结束类型,写入到WF_GenerWorkFlow， AtPara字段 1=正常结束,0=非正常结束. </param>
        /// <returns>返回成功执行信息</returns>
        public static string Flow_DoFlowOver(Int64 workID, string msg, int stopFlowType = 1)
        {
            WorkFlow wf = new WorkFlow(workID);
            string flowNo = wf.HisGenerWorkFlow.FK_Flow;

            Node nd = new Node(wf.HisGenerWorkFlow.FK_Node);
            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt");
            rpt.OID = workID;
            rpt.RetrieveFromDBSources();
            msg = wf.DoFlowOver(ActionType.FlowOver, msg, nd, rpt, stopFlowType);
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.RetrieveFromDBSources();
            WorkNode wn = new WorkNode(wk, nd);
            msg += WorkNodePlus.SubFlowEvent(wn);//onjs有可能为null的处理

            return msg;
        }
        /// <summary>
        /// 获得到达节点的集合
        /// </summary>
        /// <param name="workID">workID</param>
        /// <returns>要到达的节点对象.</returns>
        public static Directions Node_GetNextStepNodesByWorkID(int workID)
        {
            int nodeID = DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + workID);
            return Node_GetNextStepNodesByNodeID(nodeID);
        }
        /// <summary>
        /// 获得到达节点的集合.
        /// </summary>
        /// <param name="nodeID">当前节点ID</param>
        /// <returns>数据源:Directions</returns>
        /// <exception cref="Exception">如果当前节点是到达节点是自动计算的，就抛出异常.</exception>
        public static Directions Node_GetNextStepNodesByNodeID(int nodeID)
        {
            Node nd = new Node(nodeID);
            if (nd.CondModel == DirCondModel.ByLineCond)
                throw new Exception("err@当前节点转向规则是自动计算，不能获取到达的节点集合.");

            Directions dirs = new Directions();
            dirs.Retrieve(DirectionAttr.Node, nodeID, "Idx");
            return dirs;
        }
        /// <summary>
        /// 获得到指定节点上的工作人员集合
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">指定的WorkID,可以为 0 </param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataTable Node_GetNextStepEmpsByNodeID(int nodeID, Int64 workID = 0)
        {
            if (workID == 0)
                workID = DBAccess.RunSQLReturnValInt("SELECT WorkID FROM WF_GenerWorkFlow WHERE FK_Node=" + nodeID + " AND TodoEmps LIKE '%" + WebUser.No + ",%'");

            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            WorkNode toWN = new WorkNode(workID, nodeID);
            try
            {
                Flow fl = new Flow(gwf.FK_Flow);
                WorkNode currWN = new WorkNode(workID, gwf.FK_Node);

                FindWorker fw = new FindWorker();
                DataTable dt = fw.DoIt(fl, currWN, toWN);
                if (dt.Columns.Count == 1)
                {
                    dt.Columns.Add("Name");
                    foreach (DataRow dr in dt.Rows)
                    {
                        dr["Name"] = DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + dr[0].ToString() + "'");
                    }
                }
                dt.Columns[0].ColumnName = "No";
                dt.Columns[1].ColumnName = "Name";

                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("err@不支持或者没有判断的模式:" + toWN.HisNode.HisDeliveryWay.ToString() + "，技术信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 获得执行下一步骤的节点ID，这个功能是在流程未发送前可以预先知道
        /// 它就要到达那一个节点上去,以方便在当前节点发送前处理业务逻辑.
        /// 1,首先保证当前人员是可以执行当前节点的工作.
        /// 2,其次保证获取下一个节点只有一个.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>下一步骤的所要到达的节点, 如果获取不到就会抛出异常.</returns>
        public static int Node_GetNextStepNode(string fk_flow, Int64 workid)
        {
            ////检查当前人员是否可以执行当前工作.
            //if (BP.WF.Dev2Interface.Flow_CheckIsCanDoCurrentWork( workid, WebUser.No) == false)
            //    throw new Exception("@当前人员不能执行此节点上的工作.");

            //获取当前nodeID.
            int currNodeID = BP.WF.Dev2Interface.Node_GetCurrentNodeID(fk_flow, workid);

            //获取
            Node nd = new Node(currNodeID);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            WorkNode wn = new WorkNode(wk, nd);
            if (wn.rptGe == null)
            {
                wn.rptGe = nd.HisFlow.HisGERpt;
                if (wk.FID != 0)
                {
                    wn.rptGe.OID = wk.FID;
                }
                else
                {
                    wn.rptGe.OID = workid;
                }

                wn.rptGe.RetrieveFromDBSources();
                wk.Row = wn.rptGe.Row;
            }
            return wn.NodeSend_GenerNextStepNode().NodeID;
        }
        /// <summary>
        /// 获取指定的workid 在运行到的节点编号
        /// </summary>
        /// <param name="workID">需要找到的workid</param>
        /// <returns>返回节点编号. 如果没有找到，就会抛出异常.</returns>
        public static int Flow_GetCurrentNode(Int64 workID)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            return DBAccess.RunSQLReturnValInt(ps);
        }
        /// <summary>
        /// 获取指定节点的Work
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>当前工作</returns>
        public static Work Flow_GetCurrentWork(int nodeID, Int64 workID)
        {
            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.Retrieve();
            return wk;
        }
        /// <summary>
        /// 获取当前工作节点的Work
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <returns>当前工作节点的Work</returns>
        public static Work Flow_GetCurrentWork(Int64 workID)
        {
            Node nd = new Node(Flow_GetCurrentNode(workID));
            Work wk = nd.HisWork;
            wk.OID = workID;
            wk.Retrieve();
            wk.ResetDefaultVal();
            return wk;
        }
        /// <summary>
        /// 指定 workid 当前节点由哪些人可以执行.
        /// </summary>
        /// <param name="workID">需要找到的workid</param>
        /// <returns>返回当前处理人员列表,数据结构与WF_GenerWorkerlist一致.</returns>
        public static DataTable Flow_GetWorkerList(Int64 workID)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_GenerWorkerlist WHERE IsEnable=1 AND IsPass=0 AND WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 根据流程标记获得流程编号
        /// </summary>
        /// <param name="flowMark">流程属性的流程标记</param>
        /// <returns>流程编号</returns>
        public static string Flow_GetFlowNoByFlowMark(string flowMark)
        {
            string sql = "SELECT No FROM WF_Flow WHERE FlowMark='" + flowMark + "'";
            return DBAccess.RunSQLReturnStringIsNull(sql, null);
        }
        /// <summary>
        /// 检查是否可以发起流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>是否可以发起当前流程</returns>
        public static bool Flow_IsCanStartThisFlow(string flowNo, string userNo, string pFlowNo = null, int pNodeID = 0, Int64 pworkID = 0)
        {
            if (DataType.IsNullOrEmpty(WebUser.No))
                throw new Exception("err@在判断是否可以发起该流程是出现错误，没有获取到用户的登录信息，请重新登录。");

            #region 判断开始节点是否可以发起.
            Node nd = new Node(int.Parse(flowNo + "01"));
            if (nd.HisDeliveryWay == DeliveryWay.ByGuest || nd.IsGuestNode == true)
            {
                if (BP.Web.WebUser.No.Equals("Guest") == false)
                    throw new Exception("err@当前节点是外部用户处理节点，但是目前您{" + BP.Web.WebUser.No + "}不是外部用户帐号。");
                return true;
            }

            Paras ps = new Paras();
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            int num = 0;

            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByStation:
                case DeliveryWay.ByStationOnly:
                    ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeStation A, Port_DeptEmpStation B WHERE A.FK_Station= B.FK_Station AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp";
                    ps.Add("FK_Node", nd.NodeID);
                    ps.Add("FK_Emp", userNo);
                    num = DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.ByTeamOrgOnly:
                case DeliveryWay.ByTeamDeptOnly:
                    ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeTeam A, Port_TeamEmp B, Port_Emp C WHERE B.FK_Emp=C.No AND A.FK_Team= B.FK_Team AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp AND C.OrgNo=" + dbstr + "OrgNo";
                    ps.Add("FK_Node", nd.NodeID);
                    ps.Add("FK_Emp", userNo);
                    ps.Add("OrgNo", BP.Web.WebUser.OrgNo);
                    num = DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.ByTeamOnly:
                    ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeTeam A, Port_TeamEmp B WHERE A.FK_Group= B.FK_Group AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp";
                    ps.Add("FK_Node", nd.NodeID);
                    ps.Add("FK_Emp", userNo);
                    num = DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.ByDept:

                    ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeDept A, Port_DeptEmp B WHERE A.FK_Dept= B.FK_Dept AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp";
                    ps.Add("FK_Node", nd.NodeID);
                    ps.Add("FK_Emp", userNo);
                    num = DBAccess.RunSQLReturnValInt(ps);

                    if (num == 0)
                    {
                        ps.Clear();
                        ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeDept A, Port_Emp B WHERE A.FK_Dept= B.FK_Dept AND  A.FK_Node=" + dbstr + "FK_Node AND B.No=" + dbstr + "FK_Emp";
                        ps.Add("FK_Node", nd.NodeID);
                        ps.Add("FK_Emp", userNo);
                        num = DBAccess.RunSQLReturnValInt(ps);
                    }

                    break;

                case DeliveryWay.ByBindEmp:
                    ps.SQL = "SELECT COUNT(*) AS Num FROM WF_NodeEmp WHERE FK_Emp=" + dbstr + "FK_Emp AND FK_Node=" + dbstr + "FK_Node";
                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                        ps.Add("FK_Emp", WebUser.OrgNo + "_" + userNo);
                    else
                        ps.Add("FK_Emp", userNo);
                    ps.Add("FK_Node", nd.NodeID);
                    num = DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.ByDeptAndStation:


                    string sql = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeDept A, Port_DeptEmp B, WF_NodeStation C, Port_DeptEmpStation D";
                    sql += " WHERE A.FK_Dept= B.FK_Dept AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp AND  A.FK_Node=C.FK_Node AND C.FK_Station=D.FK_Station AND D.FK_Emp=" + dbstr + "FK_Emp";
                    ps.SQL = sql;
                    ps.Add("FK_Node", nd.NodeID);
                    ps.Add("FK_Emp", userNo);
                    num = DBAccess.RunSQLReturnValInt(ps);
                    break;
                case DeliveryWay.BySelected:
                case DeliveryWay.BySelected_1:
                case DeliveryWay.BySelected_2:
                    num = 1;
                    break;
                case DeliveryWay.BySelectedOrgs: //按照绑定的组织计算.
                    ps.SQL = "SELECT COUNT(*) AS Num FROM WF_FlowOrg WHERE OrgNo=" + dbstr + "OrgNo ";
                    ps.Add("OrgNo", WebUser.OrgNo);
                    num = DBAccess.RunSQLReturnValInt(ps);
                    break;
                default:
                    throw new Exception("@开始节点不允许设置此访问规则：" + nd.HisDeliveryWay);
            }


            if (num == 0)
                return false;

            if (pFlowNo == null)
                return true;
            #endregion 判断开始节点是否可以发起.

            #region 检查流程发起限制规则. 为周大福项目增加判断.
            if (pNodeID == 0)
                return true;

            //当前节点所有配置的子流程.
            SubFlowHands subflows = new SubFlowHands(pNodeID);

            //当前的子流程.
            foreach (SubFlowHand item in subflows)
            {
                if (item.SubFlowNo.Equals(flowNo) == false)
                    continue;

                if (item.StartOnceOnly == true)
                {
                    string sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pworkID + " AND FK_Flow='" + flowNo + "' AND WFState >=2 ";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        // return true; //没有人发起，他可以发起。
                    }
                    else
                    {
                        throw new Exception("该流程只能允许发起一次.");
                    }
                }

                if (item.CompleteReStart == true)
                {
                    string sql = "SELECT Starter, RDT,WFState FROM WF_GenerWorkFlow WHERE PWorkID=" + pworkID + " AND FK_Flow='" + flowNo + "' AND WFState != 3";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count != 0)
                    {
                        if (dt.Rows.Count == 1 && Int32.Parse(dt.Rows[0]["WFState"].ToString()) == 0)
                        {

                        }
                        else
                        {
                            throw new Exception("该流程已经启动还没有运行结束，不能再次启动.");
                        }


                    }

                }


                if (item.IsEnableSpecFlowStart == true)
                {
                    //指定的流程发起之后，才能启动该流程。
                    string[] fls = item.SpecFlowStart.Split(',');
                    foreach (string flStr in fls)
                    {
                        if (DataType.IsNullOrEmpty(flStr) == true)
                            continue;

                        string sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pworkID + " AND FK_Flow='" + flStr + "' AND WFState >=2 ";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            BP.WF.Flow myflow = new Flow(flStr);
                            throw new Exception("流程:[" + myflow.Name + "]没有发起,您不能启动[" + item.SubFlowName + "]。");
                        }
                    }
                }

                if (item.IsEnableSpecFlowOver == true)
                {
                    //指定的流程发起之后，才能启动该流程。
                    string[] fls = item.SpecFlowOver.Split(',');
                    foreach (string flStr in fls)
                    {
                        if (DataType.IsNullOrEmpty(flStr) == true)
                            continue;

                        string sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pworkID + " AND FK_Flow='" + flStr + "' AND WFState =3 ";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        if (dt.Rows.Count == 0)
                        {
                            BP.WF.Flow myflow = new Flow(flStr);
                            throw new Exception("流程:[" + myflow.Name + "]没有完成,您不能启动[" + item.SubFlowName + "]。");
                        }
                    }
                }
            }
            #endregion 检查流程发起限制规则.

            #region 判断流程属性的规则.
            Flow fl = new Flow(flowNo);
            if (fl.StartLimitRole == StartLimitRole.None)
                return true;

            //只有一个子流程,才能发起.
            if (fl.StartLimitRole == StartLimitRole.OnlyOneSubFlow)
            {
                if (pworkID == 0)
                    return true;

                string sql = "SELECT Starter, RDT FROM WF_GenerWorkFlow WHERE PWorkID=" + pworkID + " AND FK_Flow='" + fl.No + "' AND WFState >=2 ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    return true;

                throw new Exception("该流程只能允许发起一个子流程.");
            }
            #endregion 判断流程属性的规则.

            return true;
        }
        /// <summary>
        /// 子线程退回到分流点的处理
        /// </summary>
        /// <param name="workID"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static bool Flow_IsCanToFLTread(Int64 workID, Int64 fid, int nodeId)
        {
            if (workID == 0)
            {
                Node nd = new Node(nodeId);
                if (fid != 0 && (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL))
                    return true;

            }
            else
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                Node nd = new Node(gwf.FK_Node);

                //子线程退回到分流点
                if (gwf.WFState == WFState.ReturnSta && fid != 0 && (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL))
                    return true;
            }

            return false;
        }
        /// <summary>
        /// 获得正在运行中的子流程的数量
        /// </summary>
        /// <param name="workID">父流程的workid</param>
        /// <returns>获得正在运行中的子流程的数量。如果是0，表示所有的流程的子流程都已经结束。</returns>
        public static int Flow_NumOfSubFlowRuning(Int64 pWorkID)
        {
            string sql = "SELECT COUNT(*) AS num FROM WF_GenerWorkFlow WHERE WFState!=" + (int)WFState.Complete + " AND PWorkID=" + pWorkID;
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 获得正在运行中的子流程的数量
        /// </summary>
        /// <param name="pWorkID">父流程的workid</param>
        /// <param name="currWorkID">不包含当前的工作节点ID</param>
        /// <param name="workID">父流程的workid</param>
        /// <returns>获得正在运行中的子流程的数量。如果是0，表示所有的流程的子流程都已经结束。</returns>
        public static int Flow_NumOfSubFlowRuning(Int64 pWorkID, Int64 currWorkID)
        {
            string sql = "SELECT COUNT(*) AS num FROM WF_GenerWorkFlow WHERE WFState!=" + (int)WFState.Complete + " AND WorkID!=" + currWorkID + " AND PWorkID=" + pWorkID;
            return DBAccess.RunSQLReturnValInt(sql);
        }
        public static bool Flow_IsInGenerWork(Int64 workID)
        {

            if (workID == 0)
            {
                return false;
            }

            string sql = "select * from WF_Generworkflow where WorkID='" + workID + "'";
            return DBAccess.RunSQLReturnCOUNT(sql) > 0;
        }
        /// <summary>
        /// 检查指定节点上的所有子流程是否完成？
        /// For: 深圳熊伟.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回该节点上的子流程是否完成？</returns>
        public static bool Flow_CheckAllSubFlowIsOver(int nodeID, Int64 workID)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  PNodeID=" + dbstr + "PNodeID AND PWorkID=" + dbstr + "PWorkID AND WFState!=" + dbstr + "WFState ";
            ps.Add(GenerWorkFlowAttr.PNodeID, nodeID);
            ps.Add(GenerWorkFlowAttr.PWorkID, workID);
            ps.Add(GenerWorkFlowAttr.WFState, (int)WFState.Complete);
            if (DBAccess.RunSQLReturnValInt(ps) == 0)
                return true;
            return false;
        }
        /// <summary>
        /// 检查当前人员是否有权限处理当前的工作
        /// </summary>
        /// <param name="workID"></param>
        /// <param name="userNo"></param>
        /// <param name="auther">指定的授权人</param>
        /// <returns></returns>
        public static bool Flow_IsCanDoCurrentWork(Int64 workID, string userNo)
        {
            if (workID == 0)
                return true;

            //判断是否有待办.
            GenerWorkerList gwl = new GenerWorkerList();
            int inum = gwl.Retrieve(GenerWorkerListAttr.WorkID, workID,
                GenerWorkerListAttr.FK_Emp, userNo,
               GenerWorkerListAttr.IsPass, 0);
            if (inum >= 1)
                return true;

            GenerWorkFlow mygwf = new GenerWorkFlow(workID);

            #region 判断是否是开始节点.
            /* 判断是否是开始节点 . */
            string str = mygwf.FK_Node.ToString();
            if (str.EndsWith("01") == true)
            {
                if (mygwf.Starter.Equals(userNo))
                    return true;

                string mysql = "SELECT FK_Emp, IsPass FROM WF_GenerWorkerlist WHERE WorkID=" + workID + " AND FK_Node=" + mygwf.FK_Node;
                DataTable mydt = DBAccess.RunSQLReturnTable(mysql);
                if (mydt.Rows.Count == 0)
                    return true;

                foreach (DataRow dr in mydt.Rows)
                {
                    string fk_emp = dr[0].ToString();
                    string isPass = dr[1].ToString();
                    if (fk_emp.Equals(userNo) && (isPass.Equals("0") || isPass.Equals("80") || isPass.Equals("90")))
                        return true;
                }
                return false;
            }
            #endregion 判断是否是开始节点.

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT c.RunModel,c.IsGuestNode, a.GuestNo, a.TaskSta, a.WFState, IsPass FROM WF_GenerWorkFlow a, WF_GenerWorkerlist b, WF_Node c WHERE  b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.FK_Emp=" + dbstr + "FK_Emp AND (b.IsEnable=1 OR b.IsPass>=70 OR IsPass=0)   AND a.WorkID=" + dbstr + "WorkID ";
            ps.Add("FK_Emp", userNo);
            ps.Add("WorkID", workID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
            {
                #region 判断是否有授权信息？ 
                Auths auths = new Auths();
                if (DataType.IsNullOrEmpty(BP.Web.WebUser.Auth) == true)
                    auths.Retrieve(AuthAttr.AutherToEmpNo, userNo);
                else
                    auths.Retrieve(AuthAttr.AutherToEmpNo, userNo, AuthAttr.Auther, BP.Web.WebUser.Auth);
                //不存在授权待办
                if (auths.Count == 0)
                    return false;
                foreach (Auth item in auths)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT c.RunModel,c.IsGuestNode, a.GuestNo, a.TaskSta, a.WFState, IsPass FROM WF_GenerWorkFlow a, WF_GenerWorkerlist b, WF_Node c WHERE  b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.FK_Emp=" + dbstr + "FK_Emp AND (b.IsEnable=1 OR b.IsPass>=70 OR IsPass=0)   AND a.WorkID=" + dbstr + "WorkID ";
                    ps.Add("FK_Emp", item.Auther);
                    ps.Add("WorkID", workID);
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        continue;

                    //判断是否是待办.
                    int myisPassTemp = int.Parse(dt.Rows[0]["IsPass"].ToString());
                    //新增加的标记,=90 就是会签主持人执行会签的状态. 翻译.
                    if (myisPassTemp == 90 || myisPassTemp == 80 || myisPassTemp == 0)
                        return true;
                }
                #endregion 判断是否有授权信息？

                return false;
            }

            //判断是否是待办.
            int myisPass = int.Parse(dt.Rows[0]["IsPass"].ToString());

            //新增加的标记,=90 就是会签主持人执行会签的状态. 翻译.
            if (myisPass == 90)
                return true;

            if (myisPass == 80)
                return true;

            if (myisPass != 0)
                return false;

            WFState wfsta = (WFState)int.Parse(dt.Rows[0]["WFState"].ToString());
            if (wfsta == WFState.Complete || wfsta == WFState.Delete)
                return false;

            //判断是否是客户处理节点. 
            int isGuestNode = int.Parse(dt.Rows[0]["IsGuestNode"].ToString());
            if (isGuestNode == 1)
            {
                if (dt.Rows[0]["GuestNo"].ToString().Equals(BP.Web.GuestUser.No) == true)
                    return true;
                else
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 检查当前人员是否有权限处理当前的工作.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="userNo">要判断的操作人员</param>
        /// <returns>返回指定的人员是否有操作当前工作的权限</returns>
        public static bool Flow_IsCanDoCurrentWorkGuest(int nodeID, Int64 workID, string userNo)
        {
            if (workID == 0)
                return true;

            if (WebUser.No.Equals("admin") == true)
                return true;

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            //ps.SQL = "SELECT c.RunModel FROM WF_GenerWorkFlow a , WF_GenerWorkerlist b, WF_Node c WHERE a.FK_Node=" + dbstr + "FK_Node AND b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.FK_Emp=" + dbstr + "FK_Emp AND b.IsEnable=1 AND a.workid=" + dbstr + "WorkID";
            //ps.Add("FK_Node", nodeID);
            //ps.Add("FK_Emp", userNo);
            //ps.Add("WorkID", workID);
            string sql = "SELECT c.RunModel, a.TaskSta FROM WF_GenerWorkFlow a , WF_GenerWorkerlist b, WF_Node c WHERE a.FK_Node='" + nodeID + "'  AND b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.GuestNo='" + userNo + "' AND b.IsEnable=1 AND a.WorkID=" + workID;

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                return false;
            }

            int i = int.Parse(dt.Rows[0][0].ToString());
            TaskSta TaskStai = (TaskSta)int.Parse(dt.Rows[0][1].ToString());
            if (TaskStai == TaskSta.Sharing)
            {
                return false;
            }

            RunModel rm = (RunModel)i;
            switch (rm)
            {
                case RunModel.Ordinary:
                    return true;
                case RunModel.FL:
                    return true;
                case RunModel.HL:
                    return true;
                case RunModel.FHL:
                    return true;
                case RunModel.SubThreadSameWorkID:
                case RunModel.SubThreadUnSameWorkID:
                    return true;
                default:
                    break;
            }

            if (DBAccess.RunSQLReturnValInt(ps) == 0)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 是否可以查看流程数据
        /// 用于判断是否可以查看流程轨迹图.
        /// edit: zhoupeng 2015-03-25
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <returns></returns>
        public static bool Flow_IsCanViewTruck(string flowNo, Int64 workid, string userNo = null)
        {
            if (userNo == null)
                userNo = WebUser.No;
            if (WebUser.No.Equals("admin") == true)
                return true;

            //先从轨迹里判断.
            string dbStr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT count(MyPK) as Num FROM ND" + int.Parse(flowNo) + "Track WHERE (WorkID=" + dbStr + "WorkID OR FID=" + dbStr + "FID) AND (EmpFrom=" + dbStr + "Emp1 OR EmpTo=" + dbStr + "Emp2)";
            ps.Add(BP.WF.TrackAttr.WorkID, workid);
            ps.Add(BP.WF.TrackAttr.FID, workid);
            ps.Add("Emp1", WebUser.No);
            ps.Add("Emp2", WebUser.No);

            if (DBAccess.RunSQLReturnValInt(ps) > 1)
                return true;

            //在查看该流程的发起者，与当前人是否在同一个部门，如果是也返回true.
            ps = new Paras();
            ps.SQL = "SELECT FK_Dept FROM WF_GenerWorkFlow WHERE WorkID=" + dbStr + "WorkID OR WorkID=" + dbStr + "FID";
            ps.Add(BP.WF.TrackAttr.WorkID, workid);
            ps.Add(BP.WF.TrackAttr.FID, workid);

            string fk_dept = DBAccess.RunSQLReturnStringIsNull(ps, null);
            if (fk_dept == null)
            {
                BP.WF.Flow fl = new Flow(flowNo);
                ps.SQL = "SELECT FK_Dept FROM " + fl.PTable + " WHERE OID=" + dbStr + "WorkID OR OID=" + dbStr + "FID";
                fk_dept = DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (fk_dept == null)
                {
                    throw new Exception("@流程引擎数据被删除.");
                }
            }
            if (BP.Web.WebUser.FK_Dept == fk_dept)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">子线程的工作ID</param>
        /// <param name="info">删除信息</param>
        public static string Flow_DeleteSubThread(Int64 workid, string info)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.SetValByKey(GenerWorkFlowAttr.WorkID, workid);
            if (gwf.RetrieveFromDBSources() > 0)
            {
                WorkFlow wf = new WorkFlow(workid);
                string msg = wf.DoDeleteWorkFlowByReal(false);

                BP.WF.Dev2Interface.WriteTrackInfo(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, gwf.FID, 0, info, "删除子线程");
                return msg;
            }
            return null;
        }
        /// <summary>
        /// 执行工作催办
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">催办消息</param>
        /// <param name="isPressSubFlow">是否催办子流程？</param>
        /// <returns>返回执行结果</returns>
        public static string Flow_DoPress(Int64 workID, string msg, bool isPressSubFlow = false)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            if (gwf.WFState == WFState.Complete)
                return "err@流程已完成，您不能催办。";

            if (gwf.FK_Node.ToString().EndsWith("01") == true)
                return "info@当前节点是开始节点，您不能执行催办.";

            /*找到当前待办的工作人员*/
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve("FK_Node", gwf.FK_Node, "WorkID", workID, "IsPass", 0);
            if (wls.Count == 0)
            {
                wls.Retrieve("FID", workID, "IsPass", 0);
                if (wls.Count == 0)
                    return "err@系统错误，没有找到要催办的人。";
            }

            string toEmp = "", toEmpName = "";
            string mailTitle = "催办:" + gwf.Title + ", 发送人:" + WebUser.Name;

            PushMsgs pms = new PushMsgs();
            pms.Retrieve(PushMsgAttr.FK_Node, gwf.FK_Node,
                PushMsgAttr.FK_Event, EventListNode.PressAfter);

            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue;

                toEmp += wl.FK_Emp + ",";
                toEmpName += wl.FK_EmpText + ",";

                // 发消息.
                foreach (PushMsg push in pms)
                    BP.WF.Dev2Interface.Port_SendMsg(wl.FK_Emp, mailTitle, msg, null, BP.WF.SMSMsgType.DoPress, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID, push.SMSPushModel);

                wl.PressTimes = wl.PressTimes + 1;
                wl.Update();
            }

            //写入日志.
            WorkNode wn = new WorkNode(workID, gwf.FK_Node);
            wn.AddToTrack(ActionType.Press, toEmp, toEmpName, gwf.FK_Node, gwf.NodeName, msg);

            //如果催办子流程.
            if (isPressSubFlow == true)
            {
                string subMsg = "";
                GenerWorkFlows gwfs = gwf.HisSubFlowGenerWorkFlows;
                foreach (GenerWorkFlow item in gwfs)
                {
                    subMsg += "@已经启动对子线程:" + item.Title + "的催办,消息如下:";
                    subMsg += Flow_DoPress(item.WorkID, msg, false);
                }
                return "系统已把您的信息通知给:" + toEmpName + "" + subMsg;
            }
            else
            {
                return "系统已把您的信息通知给:" + toEmpName;
            }
        }
        /// <summary>
        /// 重新设置流程标题
        /// 可以在节点的任何位置调用它,产生新的标题。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>是否设置成功</returns>
        public static bool Flow_ReSetFlowTitle(string flowNo, int nodeID, Int64 workid)
        {
            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();
            Flow fl = nd.HisFlow;
            string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
            return Flow_SetFlowTitle(flowNo, workid, title);
        }
        /// <summary>
        /// 设置流程参数
        /// 该参数，用户可以在流程实例中获得到.
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="paras">参数,格式：@GroupMark=xxxx@IsCC=1</param>
        /// <returns>是否设置成功</returns>
        public static bool Flow_SetFlowParas(string flowNo, Int64 workid, string paras)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                throw new Exception("创建流程ID不存在.");
            }

            string[] strs = paras.Split('@');
            foreach (string item in strs)
            {
                if (DataType.IsNullOrEmpty(item))
                {
                    continue;
                }
                //GroupMark=xxxx
                string[] mystr = item.Split('=');
                gwf.SetPara(mystr[0], mystr[1]);
            }
            gwf.Update();
            return true;
        }
        /// <summary>
        /// 设置流程应完成日期.
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="sdt">应完成日期</param>
        public static void Flow_SetSDTOfFlow(Int64 workid, string sdt)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.SDTOfFlow = sdt;
            gwf.Update();
        }
        /// <summary>
        /// 设置流程标题
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="title">标题</param>
        /// <returns>是否设置成功</returns>
        public static bool Flow_SetFlowTitle(string flowNo, Int64 workid, string title)
        {
            //替换标题中出现的英文 ""引号，造成在获取数据时，造成异常
            title = title.Replace('"', '“');
            title = title.Replace('"', '”');

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + dbstr + "Title WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.Title, title);
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            DBAccess.RunSQL(ps);

            if (DataType.IsNullOrEmpty(flowNo) == true)
                flowNo = DBAccess.RunSQLReturnString("SELECT FK_Flow FROM WF_GenerWorkFlow WHERE WorkID=" + workid);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET Title=" + dbstr + "Title WHERE OID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.Title, title);
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            int num = DBAccess.RunSQL(ps);
            if (num == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 调度流程
        /// 说明：
        /// 1，通常是由admin执行的调度。
        /// 2，特殊情况下，需要从一个人的待办调度到另外指定的节点，制定的人员上。
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="toNodeID">调度到节点</param>
        /// <param name="toEmper">调度到人员</param>
        public static string Flow_Schedule(Int64 workid, int toNodeID, string toEmper)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            Node nd = new Node(toNodeID);
            Emp emp = new Emp(toEmper);

            // 找到GenerWorkFlow,并执行更新.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.WFState = WFState.Runing;
            gwf.TaskSta = TaskSta.None;
            gwf.TodoEmps = toEmper + "," + emp.Name + ";";
            gwf.FK_Node = toNodeID;
            gwf.NodeName = nd.Name;
            //gwf.StarterName =emp.Name;
            gwf.Update();

            //让其都设置完成。
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            DBAccess.RunSQL(ps);

            // 更新流程数据信息。
            Flow fl = new Flow(gwf.FK_Flow);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET FlowEnder=" + dbstr + "FlowEnder,FlowEndNode=" + dbstr + "FlowEndNode WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.FlowEnder, toEmper);
            ps.Add(GERptAttr.FlowEndNode, toNodeID);
            ps.Add(GERptAttr.OID, workid);
            DBAccess.RunSQL(ps);

            // 执行更新.
            GenerWorkerLists gwls = new GenerWorkerLists(workid);
            GenerWorkerList gwl = gwls[gwls.Count - 1] as GenerWorkerList; //获得最后一个。
            gwl.WorkID = workid;
            gwl.FK_Node = toNodeID;
            gwl.FK_NodeText = nd.Name;
            gwl.FK_Emp = toEmper;
            gwl.FK_EmpText = emp.Name;
            gwl.IsPass = false;
            gwl.IsEnable = true;
            gwl.IsRead = false;
            gwl.WhoExeIt = nd.WhoExeIt;
            //  gwl.Sender = BP.Web.WebUser.No;
            gwl.HungupTimes = 0;
            gwl.FID = gwf.FID;
            gwl.FK_Dept = emp.FK_Dept;
            gwl.DeptName = emp.FK_DeptText;

            if (gwl.Update() == 0)
            {
                gwl.Insert();
            }

            string sql = "SELECT COUNT(*) FROM WF_EmpWorks where WorkID=" + workid + " and fk_emp='" + toEmper + "'";
            int i = DBAccess.RunSQLReturnValInt(sql);
            if (i == 0)
            {
                throw new Exception("@调度错误");
            }

            return "该流程(" + gwf.Title + ")，已经调度到(" + nd.Name + "),分配给(" + emp.Name + ")";
        }
        /// <summary>
        /// 设置流程运行模式
        /// 如果是自动流程. 负责人:liuxianchen.
        /// 调用地方/WorkOpt/TransferCustom.aspx
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="runType">是否自动运行？ 如果自动运行，就按照流程设置的规则运行。
        /// 非自动运行，就按照用户自己定义的运转顺序计算。</param>
        /// <param name="paras">手工运行的参数格式为: @节点ID1`子流程No`处理模式`接受人1,接受人n`抄送人1NO,抄送人nNO`抄送人1Name,抄送人nName@节点ID2`子流程No`处理模式`接受人1,接受人n`抄送人1NO,抄送人nNO`抄送人1Name,抄送人nName</param>
        public static void Flow_SetFlowTransferCustom(string flowNo, Int64 workid, TransferCustomType runType, string paras)
        {
            //删除以前存储的参数.
            DBAccess.RunSQL("DELETE FROM WF_TransferCustom WHERE WorkID=" + workid);

            //保存参数.
            // 参数格式为  @104`SubFlow002`1`zhangsan,lisi`wangwu,chenba`王五,陈八@......
            string[] strs = paras.Split('@');
            int idx = 0, cidx = 0;
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                {
                    continue;
                }

                if (str.Contains("`") == false)
                {
                    continue;
                }

                // 处理字符串.
                string[] vals = str.Split('`');
                int nodeid = int.Parse(vals[0]);  // 节点ID.
                var subFlow = vals[1]; // 调用的子流程.
                int todomodel = int.Parse(vals[2]); //处理模式.

                TransferCustom tc = new TransferCustom();
                tc.Idx = idx;  //顺序.
                tc.FK_Node = nodeid; // 节点.
                tc.WorkID = workid; //工作ID.
                tc.Worker = vals[3]; //工作人员.
                tc.SubFlowNo = subFlow; //子流程.
                tc.setMyPK(tc.FK_Node + "_" + tc.WorkID + "_" + idx);
                tc.TodolistModel = (TodolistModel)todomodel; //处理模式.
                tc.Save();
                idx++;

                //设置抄送
                string[] ccs = vals[4].Split(',');
                string[] ccNames = vals[5].Split(',');
                SelectAccper sa = new SelectAccper();
                sa.Delete(SelectAccperAttr.FK_Node, nodeid, SelectAccperAttr.WorkID, workid, SelectAccperAttr.AccType, 1);

                cidx = 0;
                for (int i = 0; i < ccs.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(ccs[i]) || ccs[i] == "0")
                    {
                        continue;
                    }

                    sa = new SelectAccper();
                    sa.setMyPK(nodeid + "_" + workid + "_" + ccs[i]);
                    sa.FK_Emp = ccs[i].Trim();
                    sa.EmpName = ccNames[i].Trim();
                    sa.FK_Node = nodeid;

                    sa.WorkID = workid;
                    sa.AccType = 1;
                    sa.Idx = cidx;
                    sa.Insert();
                    cidx++;
                }
            }

            // 设置运行模式.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                //gwf.WFSta = WFSta.Runing;
                gwf.WFState = WFState.Blank;

                gwf.Starter = WebUser.No;
                gwf.StarterName = WebUser.Name;

                gwf.FK_Flow = flowNo;
                BP.WF.Flow fl = new Flow(flowNo);
                gwf.FK_FlowSort = fl.FK_FlowSort;

                gwf.SysType = fl.SysType;
                gwf.FK_Dept = WebUser.FK_Dept;

                gwf.TransferCustomType = runType;
                gwf.Insert();
                return;
            }
            gwf.TransferCustomType = runType;
            gwf.Update();
        }
        /// <summary>
        /// 设置流程运行模式
        /// 启用新的接口原来的接口参数格式太复杂,仍然保留.
        /// 标准格式:@NodeID=节点ID;Worker=操作人员1,操作人员2,操作人员n,TodolistModel=多人处理模式;SubFlowNo=可发起的子流程编号;SDT=应完成时间;
        /// 标准简洁格式:@NodeID=节点ID;Worker=操作人员1,操作人员2,操作人员n;@NodeID=节点ID2;Worker=操作人员1,操作人员2,操作人员n;
        /// 完整格式: @NodeID=101;Worker=zhangsan,lisi;@TodolistModel=1;SubFlowNo=001;SDT=2015-12-12;@NodeID=102;Worker=zhangsan,lisi;@TodolistModel=1;SubFlowNo=001;SDT=2015-12-12;
        /// 简洁格式: @NodeID=101;Worker=zhangsan,lisi;@NodeID=102;Worker=wagnwu,zhaoliu;
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workid"></param>
        /// <param name="runType"></param>
        /// <param name="paras">格式为:@节点编号1;处理人员1,处理人员2,处理人员n(可选);应处理时间(可选)</param>
        public static void Flow_SetFlowTransferCustomV201605(string flowNo, Int64 workid, TransferCustomType runType, string paras)
        {
            #region 更新状态.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                //  gwf.WFSta = WFSta.Runing;
                gwf.WFState = WFState.Blank;

                gwf.Starter = WebUser.No;
                gwf.StarterName = WebUser.Name;

                gwf.FK_Flow = flowNo;
                BP.WF.Flow fl = new Flow(flowNo);
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.SysType = fl.SysType;

                gwf.FK_Dept = WebUser.FK_Dept;

                gwf.TransferCustomType = runType;
                gwf.Insert();
                return;
            }
            gwf.TransferCustomType = runType;
            gwf.Update();
            if (runType == TransferCustomType.ByCCBPMDefine)
            {
                return;  // 如果是按照设置的模式运行，就要更改状态后退出它.
            }
            #endregion

            //删除以前存储的参数.
            DBAccess.RunSQL("DELETE FROM WF_TransferCustom WHERE WorkID=" + workid);

            //保存参数.
            // 参数格式为 格式为:@节点编号1;处理人员1,处理人员2,处理人员n;应处理时间(可选)
            // 例如1: @101;zhangsan,lisi,wangwu;2016-05-12;@102;liming,xiaohong,xiaozhang;2016-05-12
            // 例如2: @101;zhangsan,lisi,wangwu;@102;liming,xiaohong,xiaozhang;2016-05-12

            string[] strs = paras.Split('@');
            int idx = 0, cidx = 0;
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                {
                    continue;
                }

                if (str.Contains(";") == false)
                {
                    continue;
                }

                // 处理字符串.
                string[] vals = str.Split(';');
                int nodeid = int.Parse(vals[0]);  // 节点ID.
                var subFlow = vals[1]; // 调用的子流程.
                int todomodel = int.Parse(vals[2]); //处理模式.

                TransferCustom tc = new TransferCustom();
                tc.Idx = idx;  //顺序.
                tc.FK_Node = nodeid; // 节点.
                tc.WorkID = workid; //工作ID.
                tc.Worker = vals[3]; //工作人员.
                tc.SubFlowNo = subFlow; //子流程.
                tc.setMyPK(tc.FK_Node + "_" + tc.WorkID + "_" + idx);
                tc.TodolistModel = (TodolistModel)todomodel; //处理模式.
                tc.Save();
                idx++;

                //设置抄送
                string[] ccs = vals[4].Split(',');
                string[] ccNames = vals[5].Split(',');
                SelectAccper sa = new SelectAccper();
                sa.Delete(SelectAccperAttr.FK_Node, nodeid, SelectAccperAttr.WorkID, workid, SelectAccperAttr.AccType, 1);

                cidx = 0;
                for (int i = 0; i < ccs.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(ccs[i]) || ccs[i] == "0")
                    {
                        continue;
                    }

                    sa = new SelectAccper();
                    sa.setMyPK(nodeid + "_" + workid + "_" + ccs[i]);
                    sa.FK_Emp = ccs[i].Trim();
                    sa.EmpName = ccNames[i].Trim();
                    sa.FK_Node = nodeid;
                    sa.WorkID = workid;
                    sa.AccType = 1;
                    sa.Idx = cidx;
                    sa.Insert();
                    cidx++;
                }
            }
        }
        /// <summary>
        /// 是否可以删除该流程？
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>是否可以删除该流程</returns>
        public static bool Flow_IsCanDeleteFlowInstance(string flowNo, Int64 workid, string userNo)
        {
            if (WebUser.IsAdmin == true)
                return true;

            //是否是发起人.
            if (1 == 1)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT WorkID FROM WF_GenerWorkFlow WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND Starter=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Starter";
                ps.Add("WorkID", workid);
                ps.Add("Starter", userNo);
                string user = DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (user == null)
                {
                    return false;
                }

                return true;
            }

            return false;
        }
        #region 与流程有关的接口

        /// <summary>
        /// 增加一个评论
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">父工作ID</param>
        /// <param name="msg">消息</param>
        /// <param name="empNo">评论人编号</param>
        /// <param name="empName">评论人名称</param>
        /// <returns>插入ID主键</returns>
        public static string Flow_BBSAdd(string flowNo, Int64 workid, Int64 fid, string msg, string empNo, string empName)
        {
            return Glo.AddToTrack(ActionType.FlowBBS, flowNo, workid, fid, 0, null, empNo, empName, 0, null, empNo, empName, msg, null);
        }
        /// <summary>
        /// 删除一个评论.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="mypk">主键</param>
        /// <returns>返回删除信息.</returns>AddToTrack
        public static string Flow_BBSDelete(string flowNo, string mypk, string username)
        {
            Paras pss = new Paras();
            pss.SQL = "SELECT EMPFROM FROM ND" + int.Parse(flowNo) + "Track WHERE MyPK=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "MyPK ";
            pss.Add("MyPK", mypk);
            string str = DBAccess.RunSQLReturnString(pss);
            if (str.Equals(username) || str == username)
            {
                Paras ps = new Paras();
                ps.SQL = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE MyPK=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "MyPK ";
                ps.Add("MyPK", mypk);
                DBAccess.RunSQL(ps);
                return "删除成功.";
            }
            else
            {
                return "删除失败,仅能删除自己评论!";
            }
        }
        /// <summary>
        /// 取消设置关注
        /// </summary>
        /// <param name="workid">要取消设置的工作ID</param>
        public static void Flow_Focus(Int64 workid)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            int i = gwf.RetrieveFromDBSources();
            if (i == 0)
                throw new Exception("@ 设置关注错误：没有找到 WorkID= " + workid + " 的实例。");

            string isFocus = gwf.GetParaString("F_" + WebUser.No, "0"); //edit by liuxc,2016-10-22,修复关注/取消关注逻辑错误

            if (isFocus == "0")
                gwf.SetPara("F_" + WebUser.No, "1");
            else
                gwf.SetPara("F_" + WebUser.No, "0");

            gwf.DirectUpdate();
        }
        /// <summary>
        /// 调整流程： 
        /// </summary>
        /// <param name="workid">要调整的WorkID</param>
        /// <param name="toNodeID">调整到的节点ID</param>
        /// <param name="toEmpIDs">人员集合：如果为空，则根据当前节点的接受人规则获取人员.</param>
        /// <param name="note">调整原因</param>
        /// <returns></returns>
        public static string Flow_ReSend(Int64 workid, int toNodeID, string toEmpIDs, string note)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.WFState == WFState.Complete)
                return "err@该流程已经运行完成您不能执行调整,可以执行回滚.";

            if (gwf.FID != 0)
                return "err@当前是子线程节点，您不能跳转.";

            BP.WF.Node nd = new Node(toNodeID);
            if (nd.IsSubThread == true)
                return "err@不能跳转到子线程节点上.";

            int curNodeID = gwf.FK_Node;
            string curNodeName = gwf.NodeName;

            #region 处理要调整到的人员
            Emps emps = new Emps();
            string[] strs = toEmpIDs.Split(',');

            string todoEmps = "";
            int num = 0;
            BP.Port.Emp emp = new Emp();
            foreach (string empID in strs)
            {
                if (DataType.IsNullOrEmpty(empID) == true)
                    continue;

                emp.UserID = empID;
                if (emp.RetrieveFromDBSources() == 0)
                    return "err@" + empID + "错误,不存在.";

                todoEmps += emp.UserID + "," + emp.Name + ";";
                num++;
                emps.AddEntity(emp);
            }
            #endregion 处理要调整到的人员

            //设置人员.
            gwf.SetValByKey(GenerWorkFlowAttr.TodoEmps, todoEmps);
            gwf.TodoEmpsNum = num;
            gwf.HuiQianTaskSta = HuiQianTaskSta.None;
            gwf.WFState = WFState.Runing;

            //给当前人员产生待办，删除他.
            GenerWorkerList gwl = new GenerWorkerList();

            //删除当前节点的d待办.
            gwl.Delete(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, gwf.FK_Node);
            //删除要调整到的节点数据.
            gwl.Delete(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, toNodeID);


            //调整流程强制结束.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.FID, workid);
            foreach (GenerWorkFlow item in gwfs)
                BP.WF.Dev2Interface.Flow_DoFlowOver(item.WorkID, "调整强制结束");

            //BP.DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE IsPass!=1 AND FID="+workid)
            string todoEmpsExts = "";
            foreach (Emp item in emps)
            {
                //插入一条信息，让调整的人员显示待办.
                gwl.WorkID = workid;
                gwl.FK_Emp = item.No;
                gwl.FK_EmpText = item.Name;
                gwl.FK_Node = toNodeID;
                gwl.IsPassInt = 0;
                gwl.IsRead = false;
                gwl.WhoExeIt = 0;
                gwl.FK_Dept = item.FK_Dept;
                gwl.DeptName = item.FK_DeptText;
                gwl.FK_NodeText = nd.Name;
                gwl.FK_Flow = nd.FK_Flow;
                gwl.IsEnable = true;
                gwl.Insert();

                todoEmpsExts += item.UserID + "," + item.Name + ";";
            }

            //更新当前节点状态.
            gwf.FK_Node = toNodeID;
            gwf.NodeName = nd.Name;

            //设置当前的处理人.
            gwf.TodoEmpsNum = emps.Count;
            gwf.TodoEmps = todoEmpsExts;

            //发送人.
            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            gwf.SendDT = DataType.CurrentDateTime;
            gwf.Paras_ToNodes = "";
            gwf.Update();

            // 加入track.
            Glo.AddToTrack(ActionType.Adjust, gwf.FK_Flow, gwf.WorkID, gwf.FID, curNodeID, curNodeName, WebUser.No, WebUser.Name,
            nd.NodeID, nd.Name, gwf.TodoEmps, gwf.TodoEmps, note, null);

            //修改rpt表数据.
            string sql = "UPDATE " + nd.HisFlow.PTable + " SET  FlowEnder='" + BP.Web.WebUser.No + "',FlowEnderRDT='" + DataType.CurrentDateTime + "',FlowEndNode=" + toNodeID + " WHERE OID=" + workid;
            DBAccess.RunSQL(sql);

            return "调整成功,调整到节点:" + gwf.NodeName + " , 调整给:" + todoEmpsExts;
        }
        /// <summary>
        /// 取消、确认.
        /// </summary>
        /// <param name="workid">要取消设置的工作ID</param>
        public static void Flow_Confirm(Int64 workid)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            int i = gwf.RetrieveFromDBSources();
            if (i == 0)
            {
                throw new Exception("@ 设置关注错误：没有找到 WorkID= " + workid + " 的实例。");
            }

            string isFocus = gwf.GetParaString("C_" + WebUser.No, "0");

            if (isFocus == "0")
            {
                gwf.SetPara("C_" + WebUser.No, "1");
            }
            else
            {
                gwf.SetPara("C_" + WebUser.No, "0");
            }

            gwf.DirectUpdate();
        }
        /// <summary>
        /// 获得工作进度-用于展示流程的进度图
        /// </summary>
        /// <param name="workID">workID</param>
        /// <returns>返回进度数据</returns>
        public static DataSet DB_JobSchedule(Int64 workID)
        {
            string sql = "";
            DataSet ds = new DataSet();

            /*
             * 流程控制主表, 可以得到流程状态，停留节点，当前的执行人.
             * 该表里有如下字段是重点:
             *  0. WorkID 流程ID.
             *  1. WFState 字段用于标识当前流程的状态..
             *  2. FK_Node 停留节点.
             *  3. NodeName 停留节点名称.
             *  4. TodoEmps 停留的待办人员.
             */
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));


            /*节点信息: 节点信息表,存储每个环节的节点信息数据.
             * NodeID 节点ID.
             * Name 名称.
             * X,Y 节点图形位置，如果使用进度图就不需要了.
            */
            NodeSimples nds = new NodeSimples(gwf.FK_Flow);
            ds.Tables.Add(nds.ToDataTableField("WF_Node"));

            /*
             * 节点的连接线. 
             */
            Directions dirs = new Directions(gwf.FK_Flow);
            ds.Tables.Add(dirs.ToDataTableField("WF_Direction"));

            #region 运动轨迹
            /*
             * 运动轨迹： 构造的一个表，用与存储运动轨迹.
             * 
             */
            DataTable dtHistory = new DataTable();
            dtHistory.TableName = "Track";
            dtHistory.Columns.Add("FK_Node"); //节点ID.
            dtHistory.Columns.Add("NodeName"); //名称.
            dtHistory.Columns.Add("RunModel"); //节点类型.
            dtHistory.Columns.Add("EmpNo");  //人员编号.
            dtHistory.Columns.Add("EmpName"); //名称
            dtHistory.Columns.Add("DeptName"); //部门名称
            dtHistory.Columns.Add("RDT"); //记录日期.
            dtHistory.Columns.Add("SDT"); //应完成日期(可以不用.)
            dtHistory.Columns.Add("IsPass"); //是否通过?

            //执行人.

            //历史执行人. 
            sql = "SELECT C.Name AS DeptName,A.MyPK,A.ActionType,A.ActionTypeText,A.FID,A.WorkID,A.NDFrom,A.NDFromT,A.NDTo,A.NDToT,A.EmpFrom,A.EmpFromT,A.EmpTo,A.EmpToT,A.RDT,A.WorkTimeSpan,A.Msg,A.NodeData,A.Tag,A.Exer FROM ND" + int.Parse(gwf.FK_Flow) + "Track A, Port_Emp B, Port_Dept C  ";
            sql += " WHERE (A.WorkID=" + workID + " OR A.FID=" + workID + ") AND (A.ActionType=1 OR A.ActionType=0  OR A.ActionType=6  OR A.ActionType=7) AND (A.EmpFrom=B.No) AND (B.FK_Dept=C.No) ";
            sql += " ORDER BY A.RDT ";

            DataTable dtTrack = DBAccess.RunSQLReturnTable(sql);
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
            {
                dtTrack.Columns["NDFROM"].ColumnName = "NDFrom";
                dtTrack.Columns["NDFROMT"].ColumnName = "NDFromT";
                dtTrack.Columns["EMPFROM"].ColumnName = "EmpFrom";
                dtTrack.Columns["EMPFROMT"].ColumnName = "EmpFromT";
                dtTrack.Columns["DEPTNAME"].ColumnName = "DeptName";
                dtTrack.Columns["RDT"].ColumnName = "RDT";

            }
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
            {
                dtTrack.Columns["ndfrom"].ColumnName = "NDFrom";
                dtTrack.Columns["ndfromt"].ColumnName = "NDFromT";
                dtTrack.Columns["empfrom"].ColumnName = "EmpFrom";
                dtTrack.Columns["empfromt"].ColumnName = "EmpFromT";
                dtTrack.Columns["deptname"].ColumnName = "DeptName";
                dtTrack.Columns["rdt"].ColumnName = "RDT";

            }

            foreach (DataRow drTrack in dtTrack.Rows)
            {

                DataRow dr = dtHistory.NewRow();
                dr["FK_Node"] = drTrack["NDFrom"];
                //dr["ActionType"] = drTrack["NDFrom"];
                dr["NodeName"] = drTrack["NDFromT"];
                dr["EmpNo"] = drTrack["EmpFrom"];
                dr["EmpName"] = drTrack["EmpFromT"];
                dr["DeptName"] = drTrack["DeptName"]; //部门名称.
                dr["RDT"] = drTrack["RDT"];
                dr["SDT"] = "";
                dr["IsPass"] = 1; // gwl.IsPassInt; //是否通过.
                dtHistory.Rows.Add(dr);
            }

            //如果流程没有完成.
            if (gwf.WFState != WFState.Complete && 1 == 2)
            {
                DataRow dr = dtHistory.NewRow();
                dr["FK_Node"] = gwf.FK_Node;
                //dr["ActionType"] = drTrack["NDFrom"];
                dr["NodeName"] = gwf.NodeName;
                dr["EmpNo"] = WebUser.No;
                dr["EmpName"] = WebUser.Name;
                dr["DeptName"] = WebUser.FK_DeptName; //部门名称.
                dr["RDT"] = DataType.CurrentDate;
                dr["SDT"] = "";
                dr["IsPass"] = 0; // gwl.IsPassInt; //是否通过.
                dtHistory.Rows.Add(dr);
            }

            if (dtHistory.Rows.Count == 0)
            {
                DataRow dr = dtHistory.NewRow();
                dr["FK_Node"] = gwf.FK_Node;
                dr["NodeName"] = gwf.NodeName;
                dr["EmpNo"] = gwf.Starter;
                dr["EmpName"] = gwf.StarterName;
                dr["RDT"] = gwf.RDT;
                dr["SDT"] = gwf.SDTOfNode;
                dtHistory.Rows.Add(dr);
            }

            // 给 dtHistory runModel 赋值.
            foreach (NodeSimple nd in nds)
            {
                int runMode = nd.GetValIntByKey(NodeAttr.RunModel);
                foreach (DataRow dr in dtHistory.Rows)
                {
                    if (int.Parse(dr["FK_Node"].ToString()) == nd.NodeID)
                        dr["RunModel"] = runMode;
                }
            }
            ds.Tables.Add(dtHistory);
            #endregion 运动轨迹

            #region 游离态
            TransferCustoms tranfs = new TransferCustoms(workID);
            ds.Tables.Add(tranfs.ToDataTableField("WF_TransferCustom"));
            #endregion 游离态

            return ds;
        }
        /// <summary>
        /// 获取当前登录人的委托人
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_AuthorEmps()
        {
            if (WebUser.No == null)
            {
                throw new Exception("@ 非法用户，请执行登录后再试。");
            }

            return DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + WebUser.No + "'");
        }
        /// <summary>
        /// 获取委托给当前登录人的流程待办信息
        /// </summary>
        /// <param name="empNo">授权人员编号</param>
        /// <returns></returns>
        public static DataTable DB_AuthorEmpWorks(string empNo)
        {
            //if (WebUser.No == null)
            //    throw new Exception("@ 非法用户，请执行登录后再试。");

            //WF.Port.WFEmp emp = new BP.Port.WFEmp(empNo);
            //if (!DataType.IsNullOrEmpty(emp.Author) && emp.Author == WebUser.No && emp.AuthorIsOK == true)
            //{
            //    string sql = "";
            //    string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + "  OR WFState=" + (int)WFState.AskForReplay + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + " OR WFState=" + (int)WFState.Fix;
            //    switch (emp.HisAuthorWay)
            //    {
            //        case Port.AuthorWay.All:
            //            if (BP.WF.Glo.IsEnableTaskPool == true)
            //            {
            //                sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' AND TaskSta!=1  ORDER BY ADT DESC";
            //            }
            //            else
            //            {
            //                sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' ORDER BY ADT DESC";
            //            }

            //            break;
            //        case Port.AuthorWay.SpecFlows:
            //            if (BP.WF.Glo.IsEnableTaskPool == true)
            //            {
            //                sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' AND  FK_Flow IN " + emp.AuthorFlows + " AND TaskSta!=0    ORDER BY ADT DESC";
            //            }
            //            else
            //            {
            //                sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' AND  FK_Flow IN " + emp.AuthorFlows + "   ORDER BY ADT DESC";
            //            }

            //            break;
            //    }
            //    return DBAccess.RunSQLReturnTable(sql);
            //}
            return null;
        }
        #endregion 与流程有关的接口


        #endregion 与流程有关的接口

        #region get 属性节口
        /// <summary>
        /// 获得流程运行过程中的参数
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>如果没有就返回null,有就返回@参数名0=参数值0@参数名1=参数值1</returns>
        public static string GetFlowParas(int nodeID, Int64 workid)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT Paras FROM WF_GenerWorkerlist WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.FK_Node, nodeID);
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            return DBAccess.RunSQLReturnStringIsNull(ps, null);
        }
        #endregion get 属性节口

        #region 工作有关接口
        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">节点表单:主表数据以Key Value 方式传递(可以为空)</param>
        /// <param name="workDtls">节点表单:从表数据，从表名称与从表单的从表编号要对应(可以为空)</param>
        /// <param name="nextNodeID">发起后要跳转到的节点(可以为空)</param>
        /// <param name="nextWorker">发起后要跳转到的节点并指定的工作人员(可以为空)</param>
        /// <returns>发送到第二个节点的执行信息</returns>
        public static SendReturnObjs Node_StartWork(string flowNo, Hashtable ht, DataSet workDtls,
           int nextNodeID, string nextWorker)
        {
            return Node_StartWork(flowNo, ht, workDtls, nextNodeID, nextWorker, 0, null);
        }
        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="htWork">节点表单:主表数据以Key Value 方式传递(可以为空)</param>
        /// <param name="workDtls">节点表单:从表数据，从表名称与从表单的从表编号要对应(可以为空)</param>
        /// <param name="nextNodeID">发起后要跳转到的节点(可以为空)</param>
        /// <param name="nextWorker">发起后要跳转到的节点并指定的工作人员(可以为空)</param>
        /// <param name="parentWorkID">父流程的workid，如果没有可以为0</param>
        /// <param name="parentFlowNo">父流程的编号，如果没有可以为空</param>
        /// <returns>发送到第二个节点的执行信息</returns>
        public static SendReturnObjs Node_StartWork(string flowNo, Hashtable htWork, DataSet workDtls,
            int nextNodeID, string nextWorker, Int64 parentWorkID, string parentFlowNo)
        {

            Flow fl = new Flow(flowNo);
            Work wk = fl.NewWork();
            Int64 workID = wk.OID;
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                {
                    switch (str)
                    {
                        case GERptAttr.OID:
                        case WorkAttr.MD5:
                        case WorkAttr.Emps:
                        case GERptAttr.FID:
                        case GERptAttr.FK_Dept:
                        case GERptAttr.Rec:
                        case GERptAttr.Title:
                            continue;
                        default:
                            break;
                    }
                    wk.SetValByKey(str, htWork[str]);
                }
            }

            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                        {
                            continue;
                        }

                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }

                            daDtl.RefPK = wk.OID.ToString();
                            daDtl.Insert(); //插入数据.
                        }
                    }
                }
            }

            WorkNode wn = new WorkNode(wk, fl.HisStartNode);

            Node nextNoode = null;
            if (nextNodeID != 0)
            {
                nextNoode = new Node(nextNodeID);
            }

            SendReturnObjs objs = wn.NodeSend(nextNoode, nextWorker);
            if (parentWorkID != 0)
            {
                DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET PWorkID=" + parentWorkID + ",PFlowNo='" + parentFlowNo + "' WHERE WorkID=" + objs.VarWorkID);
            }

            #region 更新发送参数.
            if (htWork != null)
            {
                string paras = "";
                foreach (string key in htWork.Keys)
                {
                    paras += "@" + key + "=" + htWork[key].ToString();
                }

                if (DataType.IsNullOrEmpty(paras) == false && Glo.IsEnableTrackRec == true)
                {
                    string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                    Paras ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET AtPara=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, int.Parse(flowNo + "01"));
                    try
                    {
                        DBAccess.RunSQL(ps);
                    }
                    catch
                    {
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.CheckPhysicsTable();
                        DBAccess.RunSQL(ps);
                    }
                }
            }
            #endregion 更新发送参数.

            return objs;
        }

        public static void CopyDataFromParentFlow(string pFlowNo, Int64 pFID, Int64 pWorkID, Work currEnt)
        {
            //#region copy 首先从父流程的NDxxxRpt copy.
            //Int64 pWorkIDReal = 0;
            //Flow pFlow = new Flow(pFlowNo);
            //string pOID = "";
            //if (DataType.IsNullOrEmpty(PFIDStr) == true || PFIDStr == "0")
            //    pOID = PWorkID.ToString();
            //else
            //    pOID = PFIDStr;

            //string sql = "SELECT * FROM " + pFlow.PTable + " WHERE OID=" + pOID;
            //DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //if (dt.Rows.Count != 1)
            //    throw new Exception("@不应该查询不到父流程的数据[" + sql + "], 可能的情况之一,请确认该父流程的调用节点是子线程，但是没有把子线程的FID参数传递进来。");

            //wk.Copy(dt.Rows[0]);
            //rpt.Copy(dt.Rows[0]);

            ////设置单号为空.
            //wk.SetValByKey("BillNo", "");
            //rpt.BillNo = "";
            //#endregion copy 首先从父流程的NDxxxRpt copy.

            //#region 从调用的节点上copy.
            //BP.WF.Node fromNd = new BP.WF.Node(int.Parse(PNodeIDStr));
            //Work wkFrom = fromNd.HisWork;
            //wkFrom.OID = PWorkID;
            //if (wkFrom.RetrieveFromDBSources() == 0)
            //    throw new Exception("@父流程的工作ID不正确，没有查询到数据" + PWorkID);
            ////wk.Copy(wkFrom);
            ////rpt.Copy(wkFrom);
            //#endregion 从调用的节点上copy.

            //#region 获取web变量.
            //foreach (string k in paras.Keys)
            //{
            //    if (k == "OID")
            //        continue;

            //    wk.SetValByKey(k, paras[k]);
            //    rpt.SetValByKey(k, paras[k]);
            //}
            //#endregion 获取web变量.

            //#region 特殊赋值.
            //wk.OID = newOID;
            //rpt.OID = newOID;

            //// 在执行copy后，有可能这两个字段会被冲掉。
            //if (CopyFormWorkID != null)
            //{
            //    /*如果不是 执行的从已经完成的流程copy.*/

            //    wk.SetValByKey(GERptAttr.PFlowNo, PFlowNo);
            //    wk.SetValByKey(GERptAttr.PNodeID, PNodeID);
            //    wk.SetValByKey(GERptAttr.PWorkID, PWorkID);

            //    rpt.SetValByKey(GERptAttr.PFlowNo, PFlowNo);
            //    rpt.SetValByKey(GERptAttr.PNodeID, PNodeID);
            //    rpt.SetValByKey(GERptAttr.PWorkID, PWorkID);

            //    //忘记了增加这句话.
            //    rpt.SetValByKey(GERptAttr.PEmp, WebUser.No);

            //    //要处理单据编号 BillNo .
            //    if (this.BillNoFormat != "")
            //    {
            //        rpt.SetValByKey(GERptAttr.BillNo, BP.WF.WorkFlowBuessRole.GenerBillNo(this.BillNoFormat, rpt.OID, rpt, this.PTable));

            //        //设置单据编号.
            //        wk.SetValByKey(GERptAttr.BillNo, rpt.BillNo);
            //    }

            //    rpt.SetValByKey(GERptAttr.FID, 0);
            //    rpt.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDateTime);
            //    rpt.SetValByKey(GERptAttr.FlowEnderRDT, DataType.CurrentDateTime);
            //    rpt.SetValByKey(GERptAttr.WFState, (int)WFState.Blank);
            //    rpt.SetValByKey(GERptAttr.FlowStarter, emp.No);
            //    rpt.SetValByKey(GERptAttr.FlowEnder, emp.No);
            //    rpt.SetValByKey(GERptAttr.FlowEndNode, this.StartNodeID);
            //    rpt.SetValByKey(GERptAttr.FK_Dept, emp.FK_Dept);
            //    rpt.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);

            //    if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
            //        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.Name);

            //    if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
            //        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.No);

            //    if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
            //        rpt.SetValByKey(GERptAttr.FlowEmps, "@" + emp.No + "," + emp.Name);

            //}

            //if (rpt.EnMap.PhysicsTable != wk.EnMap.PhysicsTable)
            //    wk.Update(); //更新工作节点数据.
            //rpt.Update(); // 更新流程数据表.
            //#endregion 特殊赋值.

            //#region 复制其他数据..
            ////复制明细。
            //MapDtls dtls = wk.HisMapDtls;
            //if (dtls.Count > 0)
            //{
            //    MapDtls dtlsFrom = wkFrom.HisMapDtls;
            //    int idx = 0;
            //    if (dtlsFrom.Count == dtls.Count)
            //    {
            //        foreach (MapDtl dtl in dtls)
            //        {
            //            if (dtl.IsCopyNDData == false)
            //                continue;

            //            //new 一个实例.
            //            GEDtl dtlData = new GEDtl(dtl.No);

            //            //检查该明细表是否有数据，如果没有数据，就copy过来，如果有，就说明已经copy过了。
            //            //  sql = "SELECT COUNT(OID) FROM "+dtlData.EnMap.PhysicsTable+" WHERE RefPK="+wk.OID;

            //            //删除以前的数据.
            //            sql = "DELETE FROM " + dtlData.EnMap.PhysicsTable + " WHERE RefPK=" + wk.OID;
            //            DBAccess.RunSQL(sql);


            //            MapDtl dtlFrom = dtlsFrom[idx] as MapDtl;

            //            GEDtls dtlsFromData = new GEDtls(dtlFrom.No);
            //            dtlsFromData.Retrieve(GEDtlAttr.RefPK, PWorkID);
            //            foreach (GEDtl geDtlFromData in dtlsFromData)
            //            {
            //                dtlData.Copy(geDtlFromData);
            //                dtlData.RefPK = wk.OID.ToString();
            //                if (this.No == PFlowNo)
            //                {
            //                    dtlData.InsertAsNew();
            //                }
            //                else
            //                {
            //                    if (this.StartLimitRole == WF.StartLimitRole.OnlyOneSubFlow)
            //                        dtlData.SaveAsOID(geDtlFromData.OID); //为子流程的时候，仅仅允许被调用1次.
            //                    else
            //                        dtlData.InsertAsNew();
            //                }
            //            }
            //        }
            //    }
            //}

            ////复制附件数据。
            //if (wk.HisFrmAttachments.Count > 0)
            //{
            //    if (wkFrom.HisFrmAttachments.Count > 0)
            //    {
            //        int toNodeID = wk.NodeID;

            //        //删除数据。
            //        DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + toNodeID + "' AND RefPKVal='" + wk.OID + "'");
            //        FrmAttachmentDBs athDBs = new FrmAttachmentDBs("ND" + PNodeIDStr, PWorkID.ToString());

            //        foreach (FrmAttachmentDB athDB in athDBs)
            //        {
            //            FrmAttachmentDB athDB_N = new FrmAttachmentDB();
            //            athDB_N.Copy(athDB);
            //            athDB_N.setFK_MapData("ND" + toNodeID;
            //            athDB_N.RefPKVal = wk.OID.ToString();
            //            athDB_N.FK_FrmAttachment = athDB_N.FK_FrmAttachment.Replace("ND" + PNodeIDStr,
            //              "ND" + toNodeID);

            //            if (athDB_N.HisAttachmentUploadType == AttachmentUploadType.Single)
            //            {
            //                /*如果是单附件.*/
            //                athDB_N.setMyPK(athDB_N.FK_FrmAttachment + "_" + wk.OID;
            //                if (athDB_N.IsExits == true)
            //                    continue; /*说明上一个节点或者子线程已经copy过了, 但是还有子线程向合流点传递数据的可能，所以不能用break.*/
            //                athDB_N.Insert();
            //            }
            //            else
            //            {
            //                athDB_N.setMyPK(athDB_N.UploadGUID + "_" + athDB_N.FK_MapData + "_" + wk.OID;
            //                athDB_N.Insert();
            //            }
            //        }
            //    }
            //}
            //#endregion 复制表单其他数据.

            //#region 复制独立表单数据.
            ////求出来被copy的节点有多少个独立表单.
            //FrmNodes fnsFrom = new FrmNodes(fromNd.NodeID);
            //if (fnsFrom.Count != 0)
            //{
            //    //求当前节点表单的绑定的表单.
            //    FrmNodes fns = new FrmNodes(nd.NodeID);
            //    if (fns.Count != 0)
            //    {
            //        //开始遍历当前绑定的表单.
            //        foreach (FrmNode fn in fns)
            //        {
            //            foreach (FrmNode fnFrom in fnsFrom)
            //            {
            //                if (fn.FK_Frm != fnFrom.FK_Frm)
            //                    continue;

            //                BP.Sys.GEEntity geEnFrom = new GEEntity(fnFrom.FK_Frm);
            //                geEnFrom.OID = PWorkID;
            //                if (geEnFrom.RetrieveFromDBSources() == 0)
            //                    continue;

            //                //执行数据copy , 复制到本身. 
            //                geEnFrom.CopyToOID(wk.OID);
            //            }
            //        }
            //    }
            //}
            //#endregion 复制独立表单数据.
        }

        /// <summary>
        /// 创建一个空白的 WorkID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">用户编号</param>
        /// <param name="oldWorkID">以前的workID上发起.</param>
        /// <returns>执行结果</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, string userNo, Int64 oldWorkID = 0)
        {
            Int64 workid = Node_CreateBlankWork(flowNo, null, null, userNo);
            if (oldWorkID == 0)
                return workid;

            //复制开始节点表单的数据.
            Node nd = new Node(int.Parse(flowNo + "01"));
            Work wk = nd.HisWork;
            wk.OID = oldWorkID;
            wk.Retrieve();
            wk.OID = workid;
            wk.Update();

            //复制从表数据.

            //复制附件数据.

            return workid;
        }
        /// <summary>
        /// 把流程模板标记，转化为
        /// </summary>
        /// <param name="flowNoOrFlowMark"></param>
        /// <returns></returns>
        public static string Flow_TurnFlowMark2FlowNo(string flowNoOrFlowMark)
        {
            if (DataType.IsNullOrEmpty(flowNoOrFlowMark) == true)
                return flowNoOrFlowMark;

            if (DataType.IsNumStr(flowNoOrFlowMark) == true)
                return flowNoOrFlowMark;

            string sql = "";
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT No FROM WF_Flow WHERE FlowMark='" + flowNoOrFlowMark + "'";
            else
                sql = "SELECT No FROM WF_Flow WHERE FlowMark='" + flowNoOrFlowMark + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
            //return DBAccess.RunSQLReturnStringIsNull(sql, "err@流程标记[" + flowNoOrFlowMark + "]不存在.");
            return DBAccess.RunSQLReturnStringIsNull(sql, flowNoOrFlowMark);
        }
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ht">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="starter">流程的发起人</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <param name="parentWorkID">父流程的WorkID,如果没有父流程就传入为0.</param>
        /// <param name="parentFID">父流程的FID,如果没有父流程就传入为0.</param>
        /// <param name="parentFlowNo">父流程的流程编号,如果没有父流程就传入为null.</param>
        /// <param name="jumpToNode">要跳转到的节点,如果没有则为0.</param>
        /// <param name="jumpToEmp">要跳转到的人员,如果没有则为null.</param>
        /// <param name="todoEmps">待办人员,如果没有则为null.</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, Hashtable ht = null, DataSet workDtls = null,
            string starter = null, string title = null, Int64 parentWorkID = 0,
            Int64 parentFID = 0, string parentFlowNo = null,
            int parentNodeID = 0, string parentEmp = null,
            int jumpToNode = 0, string jumpToEmp = null, string todoEmps = null, string isStartSameLevelFlow = null)
        {

            flowNo = Flow_TurnFlowMark2FlowNo(flowNo);
            parentFlowNo = Flow_TurnFlowMark2FlowNo(parentFlowNo);

            //把一些其他的参数也增加里面去,传递给ccflow.
            Hashtable htPara = new Hashtable();

            if (parentWorkID != 0)
            {
                htPara.Add(StartFlowParaNameList.PWorkID, parentWorkID);
                htPara.Add(StartFlowParaNameList.PFID, parentFID);
                htPara.Add(StartFlowParaNameList.PFlowNo, parentFlowNo);
                htPara.Add(StartFlowParaNameList.PNodeID, parentNodeID);
                htPara.Add(StartFlowParaNameList.PEmp, parentEmp);
            }

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            if (DataType.IsNullOrEmpty(starter))
                starter = WebUser.No;

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);

            // 下一个工作人员。
            Emp empStarter = new Emp(starter);
            if (starter.Equals(WebUser.No))
            {
                empStarter.FK_Dept = WebUser.FK_Dept;
                empStarter.SetValByKey("FK_DeptText", WebUser.FK_DeptName);
            }
            Work wk = fl.NewWork(empStarter, htPara);
            Int64 workID = wk.OID;

            #region 给各个属性-赋值
            if (ht != null)
            {
                foreach (string str in ht.Keys)
                {
                    switch (str)
                    {
                        case GERptAttr.OID:
                        case WorkAttr.MD5:
                        case WorkAttr.Emps:
                        case GERptAttr.FID:
                        case GERptAttr.FK_Dept:
                        case GERptAttr.Rec:
                        case GERptAttr.Title:
                            continue;
                        default:
                            break;
                    }
                    wk.SetValByKey(str, ht[str]);
                }
                wk.Update();
            }
            wk.OID = workID;
            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;

                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }
            #endregion 赋值

            Paras ps = new Paras();
            if (DataType.IsNullOrEmpty(title) == true)
            {
                title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
                if (title.Contains("@") == true)
                {
                    GERpt rptGe = fl.HisGERpt;
                    rptGe.OID = wk.OID;
                    rptGe.Retrieve();
                    title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, rptGe);
                }
            }


            //执行对报表的数据表WFState状态的更新,让它为runing的状态.
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET WFState=0, Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
            //ps.Add(GERptAttr.FK_Dept, empStarter.FK_Dept);
            ps.Add(GERptAttr.Title, title);
            ps.Add(GERptAttr.OID, wk.OID);
            DBAccess.RunSQL(ps);

            // 设置父流程信息.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wk.OID;
            int i = gwf.RetrieveFromDBSources();

            //将流程信息提前写入wf_GenerWorkFlow,避免查询不到
            gwf.FlowName = fl.Name;
            gwf.FK_Flow = flowNo;
            gwf.FK_FlowSort = fl.FK_FlowSort;
            gwf.SysType = fl.SysType;

            gwf.FK_Dept = empStarter.FK_Dept;
            gwf.DeptName = empStarter.FK_DeptText;
            gwf.FK_Node = fl.StartNodeID;
            gwf.NodeName = nd.Name;
            gwf.WFState = WFState.Blank;
            gwf.Title = title;

            gwf.Starter = empStarter.UserID;
            gwf.StarterName = empStarter.Name;
            gwf.RDT = DataType.CurrentDateTime;
            gwf.PWorkID = parentWorkID;
            gwf.PFID = parentFID;
            gwf.PFlowNo = parentFlowNo;
            gwf.PNodeID = parentNodeID;

            if (fl.IsDBTemplate == true)
                gwf.Paras_DBTemplate = true;

            if (i == 0)
                gwf.Insert();
            else
                gwf.Update();

            //更新 domian.
            DBAccess.RunSQL("UPDATE WF_GenerWorkFlow  SET Domain=(SELECT Domain FROM WF_FlowSort WHERE WF_FlowSort.No=WF_GenerWorkFlow.FK_FlowSort) WHERE WorkID=" + wk.OID);

            if (parentWorkID != 0)
                BP.WF.Dev2Interface.SetParentInfo(flowNo, wk.OID, parentWorkID);//设置父流程信息

#warning 增加是防止手动启动子流程或者平级子流程时关闭子流程页面找不到待办 保存到待办。

            if (isStartSameLevelFlow != null)
                BP.WF.Dev2Interface.Node_SaveWork(wk.OID, null);
            // 如果有跳转.
            if (jumpToNode != 0)
                BP.WF.Dev2Interface.Node_SendWork(flowNo, wk.OID, null, null, jumpToNode, jumpToEmp);
            return wk.OID;
        }
        /// <summary>
        /// 增加待办人员
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="todoEmps">要增加的处理人员,多个人员用逗号分开.</param>
        public static void Node_AddTodolist(Int64 workid, string todoEmps)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.WFState == WFState.Complete)
                throw new Exception("流程：" + gwf.Title + "已经完成,您不能增加接受人.");

            #region 增加待办人员.

            GenerWorkerList gwl = new GenerWorkerList();
            gwl.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            string[] empStrs = todoEmps.Split(','); //分开字符串.
            string tempStrs = ""; //临时变量，防止重复插入.
            foreach (string emp in empStrs)
            {
                if (DataType.IsNullOrEmpty(emp) == true)
                    continue;
                if (tempStrs.Contains("," + emp + ",") == true)
                    continue;

                //插入待办.
                gwl = new GenerWorkerList();
                gwl.WorkID = workid;
                gwl.FK_Node = gwf.FK_Node;
                gwl.FK_Emp = emp;
                int i = gwl.RetrieveFromDBSources();
                if (i == 1)
                    continue;

                Emp empEn = new Emp(emp);

                gwl.FK_EmpText = empEn.Name;
                gwl.FK_NodeText = gwf.NodeName;
                gwl.FID = 0;
                gwl.FK_Flow = gwf.FK_Flow;
                gwl.FK_Dept = empEn.FK_Dept;
                gwl.DeptName = empEn.FK_DeptText;

                gwl.SDT = "无";
                gwl.DTOfWarning = DataType.CurrentDateTime;
                gwl.IsEnable = true;
                gwl.IsPass = false;
                gwl.PRI = gwf.PRI;
                gwl.Insert();

                tempStrs += "," + emp + ",";
            }
            #endregion 增加待办人员.

            if (gwf.WFState == WFState.Blank)
            {
                gwf.WFState = WFState.Runing;
                gwf.Update();
            }
        }
        /// <summary>
        /// 创建开始节点工作
        /// 创建后可以创办人形成一个待办.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="htWork">表单参数，可以为null。</param>
        /// <param name="workDtls">明细表参数，可以为null。</param>
        /// <param name="flowStarter">流程的发起人，如果为null就是当前人员。</param>
        /// <param name="title">创建工作时的标题，如果为null，就按设置的规则生成。</param>
        /// <param name="parentWorkID">父流程的WorkID,如果没有父流程就传入为0.</param>
        /// <param name="parentFlowNo">父流程的流程编号,如果没有父流程就传入为null.</param>
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateStartNodeWork(string flowNo, Hashtable htWork = null, DataSet workDtls = null,
            string flowStarter = null, string title = null, Int64 parentWorkID = 0, string parentFlowNo = null, int parentNDFrom = 0)
        {

            if (DataType.IsNullOrEmpty(flowStarter))
            {
                flowStarter = WebUser.No;
            }

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);

            // 下一个工作人员。
            Emp emp = new Emp(flowStarter);
            Work wk = fl.NewWork(flowStarter);

            #region 给各个属性-赋值
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                {
                    switch (str)
                    {
                        case GERptAttr.OID:
                        case WorkAttr.MD5:
                        case GERptAttr.FID:
                        case GERptAttr.FK_Dept:
                        case GERptAttr.Rec:
                        case GERptAttr.Title:
                            continue;
                        default:
                            break;
                    }
                    wk.SetValByKey(str, htWork[str]);
                }
                //将参数保存到业务表
                wk.DirectUpdate();
            }

            if (workDtls != null)
            {
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in wk.HisMapDtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;

                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, wk.OID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = wk.OID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = wk.OID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }
            #endregion 赋值

            #region 为开始工作创建待办
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wk.OID;
            int i = gwf.RetrieveFromDBSources();

            gwf.FlowName = fl.Name;
            gwf.FK_Flow = flowNo;
            gwf.FK_FlowSort = fl.FK_FlowSort;
            gwf.SysType = fl.SysType;

            gwf.FK_Dept = emp.FK_Dept;
            gwf.DeptName = emp.FK_DeptText;
            gwf.FK_Node = fl.StartNodeID;

            gwf.NodeName = nd.Name;

            //默认是空白流程
            //gwf.WFSta = WFSta.Etc;
            gwf.WFState = WFState.Blank;
            //保存到草稿
            if (fl.DraftRole == DraftRole.SaveToDraftList)
            {
                gwf.WFState = WFState.Draft;
            }
            else if (fl.DraftRole == DraftRole.SaveToTodolist)
            {
                //保存到待办
                //  gwf.WFSta = WFSta.Runing;
                gwf.WFState = WFState.Runing;
            }

            if (DataType.IsNullOrEmpty(title))
            {
                gwf.Title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);
            }
            else
            {
                gwf.Title = title;
            }

            gwf.Starter = emp.UserID;
            gwf.StarterName = emp.Name;
            gwf.RDT = DataType.CurrentDateTime;

            if (htWork != null && htWork.ContainsKey("PRI") == true)
            {
                gwf.PRI = int.Parse(htWork["PRI"].ToString());
            }

            if (htWork != null && htWork.ContainsKey("SDTOfNode") == true)
            {
                /*节点应完成时间*/
                gwf.SDTOfNode = htWork["SDTOfNode"].ToString();
            }

            if (htWork != null && htWork.ContainsKey("SDTOfFlow") == true)
            {
                /*流程应完成时间*/
                gwf.SDTOfNode = htWork["SDTOfFlow"].ToString();
            }

            gwf.PWorkID = parentWorkID;
            gwf.PFlowNo = parentFlowNo;
            gwf.PNodeID = parentNDFrom;
            if (i == 0)
            {
                gwf.Insert();
            }
            else
            {
                gwf.Update();
            }

            // 产生工作列表.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.WorkID = wk.OID;
            if (gwl.RetrieveFromDBSources() == 0)
            {
                gwl.FK_Emp = emp.UserID;
                gwl.FK_EmpText = emp.Name;

                gwl.FK_Node = nd.NodeID;
                gwl.FK_NodeText = nd.Name;
                gwl.FID = 0;

                gwl.FK_Flow = fl.No;
                gwl.FK_Dept = emp.FK_Dept;
                gwl.DeptName = emp.FK_DeptText;


                gwl.SDT = "无";
                gwl.DTOfWarning = DataType.CurrentDateTime;
                gwl.IsEnable = true;

                gwl.IsPass = false;
                //     gwl.Sender = WebUser.No;
                gwl.PRI = gwf.PRI;
                gwl.Insert();
            }
            #endregion 为开始工作创建待办

            // 执行对报表的数据表WFState状态的更新 
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,WFSta=" + dbstr + "WFSta,Title=" + dbstr
                + "Title,FK_Dept=" + dbstr + "FK_Dept,PFlowNo=" + dbstr + "PFlowNo,PWorkID=" + dbstr + "PWorkID WHERE OID=" + dbstr + "OID";

            //默认启用草稿.
            if (fl.DraftRole == DraftRole.None)
            {
                ps.Add("WFState", (int)WFState.Blank);
                ps.Add("WFSta", (int)WFSta.Etc);
            }
            else if (fl.DraftRole == DraftRole.SaveToDraftList)
            {
                //保存到草稿
                ps.Add("WFState", (int)WFState.Draft);
                ps.Add("WFSta", (int)WFSta.Etc);
            }
            else if (fl.DraftRole == DraftRole.SaveToTodolist)
            {
                //保存到待办
                ps.Add("WFState", (int)WFState.Runing);
                ps.Add("WFSta", (int)WFSta.Runing);
            }
            ps.Add("Title", gwf.Title);
            ps.Add("FK_Dept", gwf.FK_Dept);

            ps.Add("PFlowNo", gwf.PFlowNo);
            ps.Add("PWorkID", gwf.PWorkID);

            ps.Add("OID", wk.OID);
            DBAccess.RunSQL(ps);

            ////写入日志.
            //WorkNode wn = new WorkNode(wk, nd);
            //wn.AddToTrack(ActionType.CallSubFlow, flowStarter, emp.Name, nd.NodeID, nd.Name, "来自" + WebUser.No + "," + WebUser.Name
            //    + "工作发起.");

            #region 更新发送参数.
            if (htWork != null)
            {
                string paras = "";
                foreach (string key in htWork.Keys)
                {
                    paras += "@" + key + "=" + htWork[key].ToString();
                }

                if (DataType.IsNullOrEmpty(paras) == false && Glo.IsEnableTrackRec == true)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET AtPara=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, wk.OID);
                    ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    DBAccess.RunSQL(ps);
                }
            }
            #endregion 更新发送参数.

            return wk.OID;
        }
        /// <summary>
        /// 执行工作发送
        /// </summary>
        /// <param name="fk_flow">工作编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="ht">节点表单数据</param>
        /// <param name="dsDtl">节点表单从表数据</param>
        /// <returns>返回发送结果</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable ht = null, DataSet dsDtl = null)
        {
            return Node_SendWork(fk_flow, workID, ht, dsDtl, 0, null);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="nodeID">节点编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="toNodeID">发送到的节点编号，如果是0就让ccflow自动计算.</param>
        /// <param name="toEmps">发送到的人员,多个人员用逗号分开比如：zhangsan,lisi. 如果是null则表示让ccflow自动计算.</param>
        /// <returns>返回执行信息</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, int toNodeID, string toEmps)
        {
            return Node_SendWork(fk_flow, workID, null, null, toNodeID, toEmps);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">节点表单数据(Hashtable中的key与节点表单的字段名相同,value 就是字段值)</param>
        /// <returns>执行信息</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID,
            Hashtable htWork, int toNodeID, string nextWorkers)
        {
            return Node_SendWork(fk_flow, workID, htWork, null, toNodeID, nextWorkers, WebUser.No, WebUser.Name, WebUser.FK_Dept, WebUser.FK_DeptName, null, 0, 0);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">节点表单数据(Hashtable中的key与节点表单的字段名相同,value 就是字段值)</param>
        /// <param name="workDtls">节点表单明从表数据(dataset可以包含多个table，每个table的名称与从表名称相同，列名与从表的字段相同, OID,RefPK列需要为空或者null )</param>
        /// <param name="toNodeID">到达的节点，如果是0表示让ccflow自动寻找，否则就按照该参数发送。</param>
        /// <param name="nextWorkers">下一步的接受人，如果多个人员用逗号分开，比如:zhangsan,lisi,
        /// 如果为空，则标识让ccflow按照节点访问规则自动寻找。</param>
        /// <returns>执行信息</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable htWork, DataSet workDtls, int toNodeID, string nextWorkers)
        {
            return Node_SendWork(fk_flow, workID, htWork, workDtls, toNodeID, nextWorkers, WebUser.No, WebUser.Name, WebUser.FK_Dept, WebUser.FK_DeptName, null, 0, 0);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">节点表单数据(Hashtable中的key与节点表单的字段名相同,value 就是字段值)</param>
        /// <param name="workDtls">节点表单明从表数据(dataset可以包含多个table，每个table的名称与从表名称相同，列名与从表的字段相同, OID,RefPK列需要为空或者null )</param>
        /// <param name="toNodeID">到达的节点，如果是0表示让ccflow自动寻找，否则就按照该参数发送。</param>
        /// <param name="nextWorkers">下一步的接受人，如果多个人员用逗号分开，比如:zhangsan,lisi,
        /// 如果为空，则标识让ccflow按照节点访问规则自动寻找。</param>
        /// <param name="execUserNo">执行人编号</param>
        /// <param name="execUserName">执行人名称</param>
        /// <param name="execUserDeptNo">执行人部门名称</param>
        /// <param name="execUserDeptName">执行人部门编号</param>
        /// <returns>发送的结果对象</returns>
        public static SendReturnObjs Node_SendWork(string fk_flow, Int64 workID, Hashtable htWork, DataSet workDtls, int toNodeID,
            string toEmps, string execUserNo, string execUserName, string execUserDeptNo, string execUserDeptName, string title, Int64 fid, Int64 pworkid, bool IsReturnNode = false)
        {
            int currNodeId = Dev2Interface.Node_GetCurrentNodeID(fk_flow, workID);
            Node nd = new Node(currNodeId);
            if (htWork != null)
            {
                BP.WF.Dev2Interface.Node_SaveWork( workID, htWork, workDtls, fid, pworkid);
            }

            if (execUserNo == null)
                execUserNo = WebUser.No;

            // 变量.
            Work sw = nd.HisWork;
            sw.OID = workID;
            sw.RetrieveFromDBSources();

            Node ndOfToNode = null; //到达节点ID
            if (toNodeID != 0)
            {
                ndOfToNode = new Node(toNodeID);
            }

            //补偿性修复.
            if (nd.IsSubThread == false && sw.FID != 0)
            {
                sw.DirectUpdate();
            }

            SendReturnObjs objs;
            //执行流程发送.
            WorkNode wn = new WorkNode(sw, nd);
            wn.Execer = execUserNo;
            wn.ExecerName = execUserName;
            if (execUserNo.Equals("Guest") == true)
            {
                wn.Execer = GuestUser.No;
                wn.ExecerName = GuestUser.Name;
            }

            wn.title = title; // 设置标题，有可能是从外部传递过来的标题.
            wn.SendHTOfTemp = htWork;

            if (ndOfToNode == null)
            {
                objs = wn.NodeSend(null, toEmps, IsReturnNode);
            }
            else
            {
                objs = wn.NodeSend(ndOfToNode, toEmps, IsReturnNode);
            }

            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            if (gwf.ScripNodeID != currNodeId || DataType.IsNullOrEmpty(gwf.GetParaString("HungupCheckMsg")) == false)
            {
                //清空小纸条数据
                gwf.ScripNodeID = 0;
                gwf.ScripMsg = "";
                // 清空挂起的信息
                gwf.SetPara("HungupSta", -1); //同意.
                gwf.SetPara("HungupCheckMsg", "");
                gwf.Update();
            }
            #region 更新发送参数.
            if (htWork != null)
            {
                string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();

                string paras = "";
                foreach (string key in htWork.Keys)
                {
                    if (htWork[key] == null)
                        paras += "@" + key + "=''";
                    else
                        paras += "@" + key + "=" + htWork[key].ToString();

                    switch (key)
                    {
                        case WorkSysFieldAttr.SysSDTOfFlow:
                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkFlow SET SDTOfFlow=" + dbstr + "SDTOfFlow WHERE WorkID=" + dbstr + "WorkID";
                            ps.Add(GenerWorkFlowAttr.SDTOfFlow, htWork[key].ToString());
                            ps.Add(GenerWorkerListAttr.WorkID, workID);
                            DBAccess.RunSQL(ps);

                            break;
                        case WorkSysFieldAttr.SysSDTOfNode:
                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkFlow SET SDTOfNode=" + dbstr + "SDTOfNode WHERE WorkID=" + dbstr + "WorkID";
                            ps.Add(GenerWorkFlowAttr.SDTOfNode, htWork[key].ToString());
                            ps.Add(GenerWorkerListAttr.WorkID, workID);
                            DBAccess.RunSQL(ps);

                            ps = new Paras();
                            ps.SQL = "UPDATE WF_GenerWorkerlist SET SDT=" + dbstr + "SDT WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                            ps.Add(GenerWorkerListAttr.SDT, htWork[key].ToString());
                            ps.Add(GenerWorkerListAttr.WorkID, workID);
                            ps.Add(GenerWorkerListAttr.FK_Node, objs.VarToNodeID);
                            DBAccess.RunSQL(ps);
                            break;
                        default:
                            break;
                    }
                }

                if (DataType.IsNullOrEmpty(paras) == false && Glo.IsEnableTrackRec == true)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET AtPara=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    DBAccess.RunSQL(ps);
                }
            }

            //判断流程是否启动流程时限
            if (nd.IsStartNode
                    && wn.HisGenerWorkFlow.WFState != WFState.ReturnSta)
            {
                DateTime dtOfFlow = DateTime.Now;
                DateTime dtOfFlowWarning = DateTime.Now;
                Part part = new Part();
                part.setMyPK(nd.FK_Flow + "_0_DeadLineRole");
                int count = part.RetrieveFromDBSources();
                if (count != 0 && part.Tag0.Equals("1"))
                {
                    int tag1 = 0;
                    int tag2 = 0;
                    int tag7 = 0;
                    if (DataType.IsNumStr(part.Tag1))
                        tag1 = int.Parse(part.Tag1);
                    if (DataType.IsNumStr(part.Tag2))
                        tag2 = int.Parse(part.Tag2);
                    if (DataType.IsNumStr(part.Tag7))
                        tag7 = int.Parse(part.Tag7);
                    switch (tag7)
                    {
                        case 0: tag7 = 12; break;
                        case 1: tag7 = 24; break;
                        case 2: tag7 = 48; break;
                        case 3: tag7 = 72; break;
                        default: break;
                    }
                    //获取时限时间
                    dtOfFlow = Glo.AddDayHoursSpan(DateTime.Now, tag1,
                        tag2, int.Parse(part.Tag3), (TWay)int.Parse(part.Tag4));
                    //计算警告日期.  时限时间-预警时间
                    dtOfFlowWarning = Glo.AddDayHoursSpan(DateTime.Now, (tag1 * 24 + tag2 - tag7) / 24, (tag1 * 24 + tag2 - tag7) % 24, int.Parse(part.Tag3), (TWay)int.Parse(part.Tag4));
                    string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                    Paras ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkFlow SET SDTOfFlow=" + dbstr + "SDTOfFlow,SDTOfFlowWarning=" + dbstr + "SDTOfFlowWarning WHERE WorkID=" + dbstr + "WorkID";
                    ps.Add(GenerWorkFlowAttr.SDTOfFlow, dtOfFlow.ToString(DataType.SysDateTimeFormat));
                    ps.Add(GenerWorkFlowAttr.SDTOfFlowWarning, dtOfFlowWarning.ToString(DataType.SysDateTimeFormat));
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    DBAccess.RunSQL(ps);

                }
            }

            #endregion 更新发送参数.

            return objs;

        }
        /// <summary>
        /// 单个子线程发送
        /// </summary>
        /// <param name="workid">流程ID</param>
        /// <param name="nodeID">干流程ID</param>
        /// <param name="toNodeID">发送到的子线程ID</param>
        public static string Node_SendSubTread(Int64 workid, int nodeID, int toNodeID)
        {
            //1.子线程的流程实例改成运行状态
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
                return "该退回的子线程已经取消,不能再发送";
            gwf.WFState = WFState.Runing;
            gwf.FK_Node = toNodeID;
            gwf.Update();
            //2.当前子线程的处理人改成待办状态
            string sql = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND  FK_Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Node";
            Paras ps = new Paras();
            ps.SQL = sql;
            ps.Add("WorkID", workid);
            ps.Add("FK_Node", toNodeID);
            DBAccess.RunSQL(ps);

            DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=-2 WHERE WorkID=" + workid + " AND  FK_Node=" + nodeID + " AND IsPass=0 AND FK_Emp='" + WebUser.No + "'");
            //判断当前流程是退回并重新发送的最后一个子线程吗
            sql = "SELECT COUNT(*) FROM WF_GenerWorkFlow WHERE FID=" + gwf.FID + " AND WFState=5";
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
            {
                //修改赶流程上的状态为运行状态
                sql = "UPDATE WF_GenerWorkFlow SET WFState=2 WHERE WorkID=" + gwf.FID;
                DBAccess.RunSQL(sql);
                //修改干流程当前人员的状态为已完成
                sql = "UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + gwf.FID + " AND  FK_Emp='" + WebUser.No + "' AND FK_Node=" + nodeID;
                DBAccess.RunSQL(sql);
                return "url@MyView";
            }
            return "发送成功";
        }
        /// <summary>
        /// 增加在队列工作中增加一个处理人.
        /// 这个处理顺序系统已经自动处理了.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">工作ID</param>
        /// <param name="workid">workid</param>
        /// <param name="fid">fid</param>
        /// <param name="empNo">要增加的处理人编号</param>
        /// <param name="empName">要增加的处理人名称</param>
        public static void Node_InsertOrderEmp(string flowNo, int nodeID, Int64 workid, Int64 fid, string empNo, string empName)
        {
            GenerWorkerList gwl = new GenerWorkerList();
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, workid, GenerWorkerListAttr.FK_Node, nodeID);
            if (i == 0)
            {
                throw new Exception("@没有找到当前工作人员的待办，请检查该流程是否已经运行到该节点上来了。");
            }

            gwl.IsPassInt = 100;
            gwl.IsEnable = true;
            gwl.FK_Emp = empNo;
            gwl.FK_EmpText = empName;

            try
            {
                gwl.Insert();
            }
            catch
            {
                throw new Exception("@该人员已经存在处理队列中，您不能增加.");
            }

            //开始更新他们的顺序, 首先从数据库里获取他们的顺序.     lxl职位由小到大
            string sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + workid + " AND FK_Node=" + nodeID + " AND IsPass >=100 ) ORDER BY IDX desc";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = 100;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                string myEmpNo = dr[0].ToString();
                sql = "UPDATE WF_GenerWorkerlist SET IsPass=" + idx + " WHERE FK_Emp='" + myEmpNo + "' AND WorkID=" + workid + " AND FK_Node=" + nodeID;
                DBAccess.RunSQL(sql);
            }
        }
        /// <summary>
        /// 抄送到部门
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="deptIDs"></param>
        /// <param name="cctype">可选</param>
        /// <returns>返回执行结果.</returns>
        public static string Node_CCToDept(Int64 workid, string deptID, int cctype = 0, bool isWriteTolog = false, bool isSendMsg = false)
        {
            if (DataType.IsNullOrEmpty(deptID) == true)
                return "没有指定人";

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            Node fromNode = new Node(gwf.FK_Node);

            Emp emp = new Emp(); //构造实体类
            BP.Port.Dept dept = new BP.Port.Dept(deptID);


            string sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A WHERE A.FK_Dept='" + deptID + "'";
            sql += " UNION ";
            sql += " SELECT " + BP.Sys.Base.Glo.UserNo + ",Name FROM Port_Emp A, Port_DeptEmp B WHERE A.No=B.FK_Emp AND B.FK_Dept='" + deptID + "'";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            CCList list = new CCList();

            string names = "";
            string empNos = ","; //人员编号s 防止重复.
            foreach (DataRow dr in dt.Rows)
            {
                string empNo = dr[0].ToString();
                string empName = dr[1].ToString();
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                //如果已经包含了.
                if (empNos.Contains("," + empNo + ",") == true)
                    continue;

                empNos += empNo + ",";

                names += empName + "、";

                //list.setMyPK(DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
                if (BP.Difference.SystemConfig.CustomerNo.Equals("GXJSZX") == true)
                    list.setMyPK(gwf.WorkID + "_" + gwf.FK_Node + "_" + empNo + "_" + cctype);
                else
                    list.setMyPK(gwf.WorkID + "_" + gwf.FK_Node + "_" + empNo);
                if (list.IsExits == true)
                    continue; //判断是否存在?

                list.FK_Flow = gwf.FK_Flow;
                list.FlowName = gwf.FlowName;
                list.FK_Node = fromNode.NodeID;
                list.NodeName = gwf.NodeName;
                list.Title = gwf.Title;
                list.Doc = gwf.Title;
                list.CCTo = empNo;
                list.CCToName = empName;

                //增加抄送人部门.
                list.RDT = DataType.CurrentDateTime;
                list.Rec = WebUser.No;
                list.WorkID = gwf.WorkID;
                list.FID = gwf.FID;
                list.PFlowNo = gwf.PFlowNo;
                list.PWorkID = gwf.PWorkID;

                //是否要写入待办.
                if (fromNode.CCWriteTo == CCWriteTo.CCList)
                {
                    list.InEmpWorks = false;    //added by liuxc,2015.7.6
                }
                else
                {
                    list.InEmpWorks = true;    //added by liuxc,2015.7.6
                }

                //写入待办和写入待办与抄送列表,状态不同
                if (fromNode.CCWriteTo == CCWriteTo.All
                    || fromNode.CCWriteTo == CCWriteTo.Todolist)
                {
                    list.HisSta = fromNode.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                }

                if (fromNode.IsEndNode == true)//结束节点只写入抄送列表
                {
                    list.HisSta = CCSta.UnRead;
                    list.InEmpWorks = false;
                }

                try
                {
                    list.Insert();
                }
                catch
                {
                    list.CheckPhysicsTable();
                    list.Update();
                }

                //如果需要发送消息.
                if (isSendMsg == true)
                {
                    //发送消息给他们.
                    BP.WF.Dev2Interface.Port_SendMsg(emp.UserID, gwf.Title,
                        "抄送消息:" + gwf.Title, "CC" + gwf.FK_Node + "_" + gwf.WorkID + "_" + emp.UserID, SMSMsgType.CC, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
                }
            }

            if (isWriteTolog == true)
            {
                //记录日志.
                Glo.AddToTrack(ActionType.CC, gwf.FK_Flow, gwf.WorkID, gwf.FID, gwf.FK_Node, gwf.NodeName,
                   WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, empNos, names, gwf.Title, null);
            }

            return "已经成功的把工作抄送给:" + names;
        }
        /// <summary>
        /// 抄送部门
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="deptIDs">部门IDs</param>
        /// <param name="cctype">抄送类型</param>
        /// <returns>返回执行的结果</returns>
        public static string Node_CCToDepts(Int64 workid, string deptIDs, int cctype = 0)
        {
            string[] deptStrs = deptIDs.Split(',');
            foreach (string deptID in deptStrs)
            {
                Node_CCToDept(workid, deptID, cctype, false, false);
            }
            return "成功执行.";
        }
        /// <summary>
        /// 抄送部门和用户
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="deptIDs">部门IDs</param>
        /// <param name="toEmps">用户账号</param>
        /// <param name="cctype">抄送类型</param>
        /// <returns>返回执行的结果</returns>
        public static string Node_CCToDepts(Int64 workid, string deptIDs, string toEmps, int cctype = 0)
        {
            string[] deptStrs = deptIDs.Split(',');
            foreach (string deptID in deptStrs)
            {
                Node_CCToDept(workid, deptID, cctype, false, false);
            }
            if (DataType.IsNullOrEmpty(toEmps) == true)
            {
                return "成功执行.";
            }
            return Node_CCTo(workid, toEmps, cctype);

        }
        /// <summary>
        /// 写入抄送
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="toEmps">抄送给: zhangsan,lisi,wangwu </param>
        /// <returns>执行结果</returns>
        public static string Node_CCTo(Int64 workid, string toEmps, int cctype = 0)
        {
            if (DataType.IsNullOrEmpty(toEmps) == true)
                return "没有指定人";

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            Node fromNode = new Node(gwf.FK_Node);

            toEmps = toEmps.Replace(";", ",");
            string[] strs = toEmps.Split(',');
            BP.Port.Emp emp = new Emp();
            BP.Port.Dept dept = new BP.Port.Dept();
            CCList list = new CCList();

            string names = "";
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                emp.UserID = str;
                int i = emp.RetrieveFromDBSources();
                if (i == 0)
                    continue;

                //根据人员的部门编号获取所在部门名称
                dept.No = emp.FK_Dept;
                dept.RetrieveFromDBSources();

                names += emp.Name + "、";

                //list.setMyPK(DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
                if (BP.Difference.SystemConfig.CustomerNo.Equals("GXJSZX") == true)
                    list.setMyPK(gwf.WorkID + "_" + gwf.FK_Node + "_" + emp.UserID + "_" + cctype);
                else
                    list.setMyPK(gwf.WorkID + "_" + gwf.FK_Node + "_" + emp.UserID);
                if (list.IsExits == true)
                    continue; //判断是否存在?


                list.FK_Flow = gwf.FK_Flow;
                list.FlowName = gwf.FlowName;
                list.FK_Node = fromNode.NodeID;
                list.NodeName = gwf.NodeName;
                list.Title = gwf.Title;
                list.Doc = gwf.Title;
                list.CCTo = emp.UserID;
                list.CCToName = emp.Name;

                //增加抄送人部门.
                list.RDT = DataType.CurrentDateTime;
                list.Rec = WebUser.No;
                list.WorkID = gwf.WorkID;
                list.FID = gwf.FID;
                list.PFlowNo = gwf.PFlowNo;
                list.PWorkID = gwf.PWorkID;

                //是否要写入待办.
                if (fromNode.CCWriteTo == CCWriteTo.CCList)
                {
                    list.InEmpWorks = false;    //added by liuxc,2015.7.6
                }
                else
                {
                    list.InEmpWorks = true;    //added by liuxc,2015.7.6
                }

                //写入待办和写入待办与抄送列表,状态不同
                if (fromNode.CCWriteTo == CCWriteTo.All
                    || fromNode.CCWriteTo == CCWriteTo.Todolist)
                {
                    list.HisSta = fromNode.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                }

                if (fromNode.IsEndNode == true)//结束节点只写入抄送列表
                {
                    list.HisSta = CCSta.UnRead;
                    list.InEmpWorks = false;
                }

                try
                {
                    list.Insert();
                }
                catch
                {
                    list.CheckPhysicsTable();
                    list.Update();
                }

                //发送消息给他们.
                BP.WF.Dev2Interface.Port_SendMsg(emp.UserID, gwf.Title,
                    "抄送消息:" + gwf.Title, "CC" + gwf.FK_Node + "_" + gwf.WorkID + "_" + emp.UserID, SMSMsgType.CC, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
            }

            //记录日志.
            // Glo.AddToTrack(ActionType.CC, gwf.FK_Flow, gwf.WorkID, gwf.FID, gwf.FK_Node, gwf.NodeName,
            ///    WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, toEmps, names, gwf.Title, null);

            return "已经成功的把工作抄送给:" + names;
        }
        /// <summary>
        /// 把抄送写入待办列表
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="ccToEmpNo">抄送给</param>
        /// <param name="ccToEmpName">抄送给名称</param>
        /// <returns></returns>
        public static string Node_CC_WriteTo_Todolist(int fk_node, Int64 workID, string ccToEmpNo, string ccToEmpName)
        {
            return Node_CC_WriteTo_CClist(fk_node, workID, ccToEmpNo, ccToEmpName, "", "");
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="toEmpNo">抄送人员编号</param>
        /// <param name="toEmpName">抄送人员人员名称</param>
        /// <param name="msgTitle">标题</param>
        /// <param name="msgDoc">内容</param>
        /// <returns>执行信息</returns>
        public static string Node_CC_WriteTo_CClist(int fk_node, Int64 workID,
            string toEmpNo, string toEmpName,
            string msgTitle, string msgDoc)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                Node nd = new Node(fk_node);
                gwf.FK_Node = fk_node;
                gwf.FK_Flow = nd.FK_Flow;
                gwf.FlowName = nd.FlowName;
                gwf.NodeName = nd.Name;
            }

            Node fromNode = new Node(fk_node);

            CCList list = new CCList();
            list.setMyPK(DBAccess.GenerOIDByGUID().ToString()); // workID + "_" + fk_node + "_" + empNo;
            list.FK_Flow = gwf.FK_Flow;
            list.FlowName = gwf.FlowName;
            list.FK_Node = fk_node;
            list.NodeName = gwf.NodeName;
            list.Title = msgTitle;
            list.Doc = msgDoc;
            list.CCTo = toEmpNo;
            list.CCToName = toEmpName;

            //增加抄送人部门.
            Emp emp = new Emp(toEmpNo);
            list.RDT = DataType.CurrentDateTime;
            list.Rec = WebUser.No;
            list.WorkID = gwf.WorkID;
            list.FID = gwf.FID;
            list.PFlowNo = gwf.PFlowNo;
            list.PWorkID = gwf.PWorkID;
            //  list.NDFrom = ndFrom;

            //是否要写入待办.
            if (fromNode.CCWriteTo == CCWriteTo.CCList)
            {
                list.InEmpWorks = false;    //added by liuxc,2015.7.6
            }
            else
            {
                list.InEmpWorks = true;    //added by liuxc,2015.7.6
            }

            //写入待办和写入待办与抄送列表,状态不同
            if (fromNode.CCWriteTo == CCWriteTo.All || fromNode.CCWriteTo == CCWriteTo.Todolist)
            {
                list.HisSta = fromNode.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
            }

            if (fromNode.IsEndNode == true)//结束节点只写入抄送列表
            {
                list.HisSta = CCSta.UnRead;
                list.InEmpWorks = false;
            }

            try
            {
                list.Insert();
            }
            catch
            {
                list.CheckPhysicsTable();
                list.Update();
            }

            //
            BP.WF.Dev2Interface.Port_SendMsg(toEmpNo, msgTitle, msgDoc, "CC" + gwf.FK_Node + "_" + gwf.WorkID, SMSMsgType.CC, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

            //记录日志.
            Glo.AddToTrack(ActionType.CC, gwf.FK_Flow, workID, gwf.FID, gwf.FK_Node, gwf.NodeName,
                WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, toEmpNo, toEmpName, msgTitle, null);

            return "已经成功的把工作抄送给:" + toEmpNo + "," + toEmpName;
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        /// <param name="fk_node">节点</param>
        /// <param name="workID">工作ID</param>
        /// <param name="title">标题</param>
        /// <param name="doc">内容</param>
        /// <param name="toEmps">到人员(zhangsan,张三;lisi,李四;wangwu,王五;)</param>
        /// <param name="toDepts">到部门，格式:001,002,003</param>
        /// <param name="toStations">到角色 格式:001,002,003</param>
        /// <param name="toStations">到权限组 格式:001,002,003</param>
        public static string Node_CC_WriteTo_CClist(int fk_node, Int64 workID, string title, string doc,
            string toEmps = null, string toDepts = null, string toStations = null, string toGroups = null)
        {

            Node nd = new Node(fk_node);

            //计算出来曾经抄送过的人.
            string sql = "SELECT CCTo FROM WF_CCList WHERE FK_Node=" + fk_node + " AND WorkID=" + workID;
            DataTable mydt = DBAccess.RunSQLReturnTable(sql);
            string toAllEmps = ",";
            foreach (DataRow dr in mydt.Rows)
            {
                toAllEmps += dr[0].ToString() + ",";
            }

            //录制本次抄送的人员.
            string ccRec = "";

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                gwf.FK_Node = fk_node;
                gwf.FK_Flow = nd.FK_Flow;
                gwf.FlowName = nd.FlowName;
                gwf.NodeName = nd.Name;
            }

            #region 处理抄送到人员.
            if (toEmps != null)
            {
                string[] empStrs = toEmps.Split(';');
                foreach (string empStr in empStrs)
                {
                    if (DataType.IsNullOrEmpty(empStr) == true || empStr.Contains(",") == false)
                    {
                        continue;
                    }

                    string[] strs = empStr.Split(',');
                    string empNo = strs[0];
                    string empName = strs[1];

                    if (toAllEmps.Contains("," + empNo + ",") == true)
                    {
                        continue;
                    }

                    CCList list = new CCList();
                    list.setMyPK(DBAccess.GenerOIDByGUID().ToString()); // workID + "_" + fk_node + "_" + empNo;
                    list.FK_Flow = gwf.FK_Flow;
                    list.FlowName = gwf.FlowName;
                    list.FK_Node = fk_node;
                    list.NodeName = gwf.NodeName;
                    list.Title = title;
                    list.Doc = doc;
                    list.CCTo = empNo;
                    list.CCToName = empName;
                    list.RDT = DataType.CurrentDateTime;
                    list.Rec = WebUser.No;
                    list.WorkID = gwf.WorkID;
                    list.FID = gwf.FID;
                    list.PFlowNo = gwf.PFlowNo;
                    list.PWorkID = gwf.PWorkID;
                    // list.NDFrom = ndFrom;

                    //是否要写入待办.
                    if (nd.CCWriteTo == CCWriteTo.CCList)
                    {
                        list.InEmpWorks = false;    //added by liuxc,2015.7.6
                    }
                    else
                    {
                        list.InEmpWorks = true;    //added by liuxc,2015.7.6
                    }

                    //写入待办和写入待办与抄送列表,状态不同
                    if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
                    {
                        list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                    }

                    if (nd.IsEndNode == true)//结束节点只写入抄送列表
                    {
                        list.HisSta = CCSta.UnRead;
                        list.InEmpWorks = false;
                    }

                    try
                    {
                        list.Insert();
                    }
                    catch
                    {
                        list.CheckPhysicsTable();
                        list.Update();
                    }

                    ccRec += "" + list.CCToName + ";";
                    //人员编号,加入这个集合.
                    toAllEmps += empNo + ",";
                }
            }
            #endregion 处理抄送到人员.

            #region 处理抄送到部门.
            if (toDepts != null)
            {
                toDepts = toDepts.Replace(";", ",");

                string[] deptStrs = toDepts.Split(',');
                foreach (string deptNo in deptStrs)
                {
                    if (DataType.IsNullOrEmpty(deptNo) == true)
                    {
                        continue;
                    }

                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name,FK_Dept FROM Port_Emp WHERE FK_Dept='" + deptNo + "'";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string empNo = dr[0].ToString();
                        string empName = dr[1].ToString();
                        if (toAllEmps.Contains("," + empNo + ",") == true)
                        {
                            continue;
                        }

                        CCList list = new CCList();
                        list.setMyPK(DBAccess.GenerOIDByGUID().ToString()); // workID + "_" + fk_node + "_" + empNo;
                        list.FK_Flow = gwf.FK_Flow;
                        list.FlowName = gwf.FlowName;
                        list.FK_Node = fk_node;
                        list.NodeName = gwf.NodeName;
                        list.Title = title;
                        list.Doc = doc;
                        list.CCTo = empNo;
                        list.CCToName = empName;
                        list.RDT = DataType.CurrentDateTime;
                        list.Rec = WebUser.No;
                        list.WorkID = gwf.WorkID;
                        list.FID = gwf.FID;
                        list.PFlowNo = gwf.PFlowNo;
                        list.PWorkID = gwf.PWorkID;
                        // list.NDFrom = ndFrom;

                        //是否要写入待办.
                        if (nd.CCWriteTo == CCWriteTo.CCList)
                        {
                            list.InEmpWorks = false;    //added by liuxc,2015.7.6
                        }
                        else
                        {
                            list.InEmpWorks = true;    //added by liuxc,2015.7.6
                        }

                        //写入待办和写入待办与抄送列表,状态不同
                        if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
                        {
                            list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                        }

                        if (nd.IsEndNode == true)//结束节点只写入抄送列表
                        {
                            list.HisSta = CCSta.UnRead;
                            list.InEmpWorks = false;
                        }

                        try
                        {
                            list.Insert();
                        }
                        catch
                        {
                            list.CheckPhysicsTable();
                            list.Update();
                        }

                        //录制本次抄送到的人员.
                        ccRec += "" + list.CCToName + ";";

                        //人员编号,加入这个集合.
                        toAllEmps += empNo + ",";
                    }
                }
            }
            #endregion 处理抄送到部门.

            #region 处理抄送到角色.
            if (toStations != null)
            {
                string empNo = "No";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    empNo = "UserID as No";

                toStations = toStations.Replace(";", ",");
                string[] mystas = toStations.Split(',');
                foreach (string staNo in mystas)
                {
                    if (DataType.IsNullOrEmpty(staNo) == true)
                    {
                        continue;
                    }

                    sql = "SELECT A." + empNo + ", A.Name, a.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B  WHERE a.No=B.FK_Emp AND B.FK_Station='" + staNo + "'";

                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        empNo = dr[0].ToString();
                        string empName = dr[1].ToString();
                        if (toAllEmps.Contains("," + empNo + ",") == true)
                        {
                            continue;
                        }

                        CCList list = new CCList();
                        list.setMyPK(DBAccess.GenerOIDByGUID().ToString()); // workID + "_" + fk_node + "_" + empNo;
                        list.FK_Flow = gwf.FK_Flow;
                        list.FlowName = gwf.FlowName;
                        list.FK_Node = fk_node;
                        list.NodeName = gwf.NodeName;
                        list.Title = title;
                        list.Doc = doc;
                        list.CCTo = empNo;
                        list.CCToName = empName;
                        list.RDT = DataType.CurrentDateTime;
                        list.Rec = WebUser.No;
                        list.WorkID = gwf.WorkID;
                        list.FID = gwf.FID;
                        list.PFlowNo = gwf.PFlowNo;
                        list.PWorkID = gwf.PWorkID;
                        // list.NDFrom = ndFrom;

                        //是否要写入待办.
                        if (nd.CCWriteTo == CCWriteTo.CCList)
                        {
                            list.InEmpWorks = false;    //added by liuxc,2015.7.6
                        }
                        else
                        {
                            list.InEmpWorks = true;    //added by liuxc,2015.7.6
                        }

                        //写入待办和写入待办与抄送列表,状态不同
                        if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
                        {
                            list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                        }

                        if (nd.IsEndNode == true)//结束节点只写入抄送列表
                        {
                            list.HisSta = CCSta.UnRead;
                            list.InEmpWorks = false;
                        }

                        try
                        {
                            list.Insert();
                        }
                        catch
                        {
                            list.CheckPhysicsTable();
                            list.Update();
                        }

                        //录制本次抄送到的人员.
                        ccRec += "" + list.CCToName + ";";

                        //人员编号,加入这个集合.
                        toAllEmps += empNo + ",";
                    }
                }
            }
            #endregion.

            #region 抄送到组.
            if (toGroups != null)
            {
                toGroups = toGroups.Replace(";", ",");
                string[] groups = toGroups.Split(',');

                foreach (string group in groups)
                {
                    if (DataType.IsNullOrEmpty(group) == true)
                    {
                        continue;
                    }

                    string empNo = "No";
                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                        empNo = "UserID as No";

                    //解决分组下的角色人员.
                    sql = "SELECT a." + empNo + ",a.Name, A.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B, GPM_GroupStation C  WHERE A.No=B.FK_Emp AND B.FK_Station=C.FK_Station AND C.FK_Group='" + group + "'";
                    sql += " UNION ";
                    sql += "SELECT A." + empNo + ", A.Name, A.FK_Dept FROM Port_Emp A, Port_TeamEmp B  WHERE A.No=B.FK_Emp AND B.FK_Group='" + group + "'";

                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        empNo = dr[0].ToString();
                        string empName = dr[1].ToString();
                        if (toAllEmps.Contains("," + empNo + ",") == true)
                        {
                            continue;
                        }

                        CCList list = new CCList();
                        list.setMyPK(DBAccess.GenerOIDByGUID().ToString()); // workID + "_" + fk_node + "_" + empNo;
                        list.FK_Flow = gwf.FK_Flow;
                        list.FlowName = gwf.FlowName;
                        list.FK_Node = fk_node;
                        list.NodeName = gwf.NodeName;
                        list.Title = title;
                        list.Doc = doc;
                        list.CCTo = empNo;
                        list.CCToName = empName;
                        list.RDT = DataType.CurrentDateTime;
                        list.Rec = WebUser.No;
                        list.WorkID = gwf.WorkID;
                        list.FID = gwf.FID;
                        list.PFlowNo = gwf.PFlowNo;
                        list.PWorkID = gwf.PWorkID;
                        // list.NDFrom = ndFrom;

                        //是否要写入待办.
                        if (nd.CCWriteTo == CCWriteTo.CCList)
                        {
                            list.InEmpWorks = false;    //added by liuxc,2015.7.6
                        }
                        else
                        {
                            list.InEmpWorks = true;    //added by liuxc,2015.7.6
                        }

                        //写入待办和写入待办与抄送列表,状态不同
                        if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
                        {
                            list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                        }

                        if (nd.IsEndNode == true)//结束节点只写入抄送列表
                        {
                            list.HisSta = CCSta.UnRead;
                            list.InEmpWorks = false;
                        }

                        try
                        {
                            list.Insert();
                        }
                        catch
                        {
                            list.CheckPhysicsTable();
                            list.Update();
                        }

                        //录制本次抄送到的人员.
                        ccRec += "" + list.CCToName + ";";

                        //人员编号,加入这个集合.
                        toAllEmps += empNo + ",";
                    }
                }
            }
            #endregion 抄送到组

            return ccRec;

            ////记录日志.
            //Glo.AddToTrack(ActionType.CC, gwf.FK_Flow, workID, gwf.FID, gwf.FK_Node, gwf.NodeName,
            //    WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, toAllEmps, toAllEmps, title, null);
        }

        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="mypk">删除</param>
        public static void Node_CC_DoDel(string mypk)
        {
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM WF_CCList WHERE MyPK=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add(CCListAttr.MyPK, mypk);
            DBAccess.RunSQL(ps);
        }

        /// <summary>
        /// 设置抄送状态
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="empNo">人员编号</param>
        /// <param name="sta">状态</param>
        public static void Node_CC_SetSta(int nodeID, Int64 workid, string empNo, CCSta sta)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList   SET Sta=" + dbstr + "Sta,CDT=" + dbstr + "CDT WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND CCTo=" + dbstr + "CCTo";
            ps.Add(CCListAttr.Sta, (int)sta);
            ps.Add(CCListAttr.CDT, DataType.CurrentDateTime);
            ps.Add(CCListAttr.WorkID, workid);
            ps.Add(CCListAttr.FK_Node, nodeID);
            ps.Add(CCListAttr.CCTo, empNo);
            DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 执行读取
        /// </summary>
        /// <param name="mypk">主键</param>
        public static void Node_CC_SetRead(string mypk, string bbsSetInfo = null)
        {
            if (DataType.IsNullOrEmpty(mypk))
                return;

            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList SET Sta=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Sta,ReadDT=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ReadDT  WHERE MyPK=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add(CCListAttr.Sta, (int)CCSta.Read);   // @lizhen.
            ps.Add(CCListAttr.ReadDT, DataType.CurrentDateTime); //设置读取日期.
            ps.Add(CCListAttr.MyPK, mypk);
            DBAccess.RunSQL(ps);
        }

        /// <summary>
        /// 设置抄送执行完成
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="checkInfo">审核信息</param>
        public static string Node_CC_SetCheckOver(Int64 workid, string checkInfo = null)
        {
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList SET Sta=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Sta,CDT=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CDT  WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo ";
            ps.Add(CCListAttr.Sta, (int)CCSta.CheckOver);
            ps.Add(CCListAttr.CDT, DataType.CurrentDateTime); //设置完成日期.
            ps.Add(CCListAttr.WorkID, workid);
            ps.Add(CCListAttr.CCTo, WebUser.No);
            int val = DBAccess.RunSQL(ps);
            if (val == 0)
                return "err@执行失败,没有更新到workid=" + workid + ",CCTo=" + WebUser.No + ", 的抄送数据.";

            GenerWorkFlow gwf = new GenerWorkFlow(workid);

            //BP.WF.Dev2Interface.Flow_BBSAdd(gwf.FK_Flow, workid, gwf.FID, checkInfo, BP.Web.WebUser.No, BP.Web.WebUser.Name);
            return "执行成功.";
        }
        /// <summary>
        /// 设置抄送执行完成
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="checkInfo">审核信息</param>
        public static string Node_CC_SetCheckOver(string flowNo, Int64 workid, Int64 fid, string checkInfo = null, string empNo = null, string empName = null)
        {
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList SET Sta=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Sta,CDT=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CDT  WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo ";
            ps.Add(CCListAttr.Sta, (int)CCSta.CheckOver);
            ps.Add(CCListAttr.CDT, DataType.CurrentDateTime); //设置完成日期.
            ps.Add(CCListAttr.WorkID, workid);
            ps.Add(CCListAttr.CCTo, WebUser.No);
            int val = DBAccess.RunSQL(ps);
            if (val == 0)
                return "err@执行失败,没有更新到workid=" + workid + ",CCTo=" + WebUser.No + ", 的抄送数据.";

            BP.WF.Dev2Interface.Flow_BBSAdd(flowNo, workid, fid, checkInfo, empNo, empName);
            return "执行成功.";
        }
        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="workids">多个工作ID使用逗号分割比如:'111,233,444'</param>
        /// <param name="checkInfo">批量审核意见</param>
        public static string Node_CC_SetCheckOverBatch(string workids, string checkInfo = null)
        {
            if (checkInfo == null)
                checkInfo = "已阅";

            string[] ids = workids.Split(',');
            string info = "";
            foreach (string id in ids)
            {
                if (DataType.IsNullOrEmpty(id) == true)
                    continue;

                GenerWorkFlow gwf = new GenerWorkFlow(Int64.Parse(id));

                //表单方案.
                string frmID = null;
                FrmNodes fns = new FrmNodes();
                fns.Retrieve(FrmNodeAttr.FK_Node, gwf.FK_Node);
                foreach (FrmNode fn in fns)
                {
                    if (fn.FK_Frm.Equals("ND" + gwf.FK_Node) == true)
                        continue;

                    frmID = fn.FK_Frm;
                    break;
                }

                if (frmID == null)
                {
                    Node_CC_SetCheckOver(gwf.WorkID, checkInfo);
                    continue;
                }

                //设置阅读track. 这里已经设置了已阅.
                BP.WF.Dev2Interface.Track_WriteBBS(frmID, frmID, gwf.WorkID, checkInfo, gwf.FID, gwf.FK_Flow, gwf.FlowName, gwf.FK_Node, gwf.NodeName);

                //info += Node_CC_SetCheckOver(Int64.Parse(id), checkInfo);
            }
            return "执行成功.";
        }
        /// <summary>
        /// 设置抄送读取
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="empNo">读取人员编号</param>
        public static void Node_CC_SetRead(int nodeID, Int64 workid, string empNo, string bbsCheckInfo = null)
        {
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList SET Sta=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "Sta,ReadDT=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "ReadDT  WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Node AND CCTo=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "CCTo";
            ps.Add(CCListAttr.Sta, (int)CCSta.UnRead);
            ps.Add(CCListAttr.ReadDT, DataType.CurrentDateTime); //设置读取日期.
            ps.Add(CCListAttr.WorkID, workid);
            ps.Add(CCListAttr.FK_Node, nodeID);
            ps.Add(CCListAttr.CCTo, empNo);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsRead=1 WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Node=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Node AND FK_Emp=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            ps.Add(GenerWorkerListAttr.FK_Node, nodeID);
            ps.Add(GenerWorkerListAttr.FK_Emp, empNo);
            DBAccess.RunSQL(ps);

            //   if (bbsCheckInfo!=null)
            //     BP.WF.Dev2Interface.Track_WriteBBS()
        }
        /// <summary>
        /// 执行抄送
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">节点编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="toEmpNo">抄送给人员编号</param>
        /// <param name="toEmpName">抄送给人员名称</param>
        /// <param name="msgTitle">消息标题</param>
        /// <param name="msgDoc">消息内容</param>
        /// <param name="pFlowNo">父流程编号(可以为null)</param>
        /// <param name="pWorkID">父流程WorkID(可以为0)</param>
        /// <param name="FID">FID(可以为0)</param>
        /// <returns></returns>
        public static string Node_CC(string fk_flow, int fk_node, Int64 workID, string toEmpNo, string toEmpName, string msgTitle, string msgDoc, Int64 FID = 0, string pFlowNo = null, Int64 pWorkID = 0)
        {
            Flow fl = new Flow(fk_flow);
            Node nd = new Node(fk_node);

            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            CCList list = new CCList();
            //list.setMyPK(DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
            list.setMyPK(workID + "_" + fk_node + "_" + toEmpNo);
            list.FID = FID;
            list.FK_Flow = fk_flow;
            list.FlowName = fl.Name;
            list.FK_Node = fk_node;
            list.NodeName = nd.Name;
            list.Title = msgTitle;
            list.Doc = msgDoc;
            list.CCTo = toEmpNo;
            list.CCToName = toEmpName;
            list.InEmpWorks = nd.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6
            //写入待办和写入待办与抄送列表,状态不同
            if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
            {
                list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
            }
            if (nd.IsEndNode == true)//结束节点只写入抄送列表
            {
                list.HisSta = CCSta.UnRead;
                list.InEmpWorks = false;
            }
            //增加抄送人部门.
            Emp emp = new Emp(toEmpNo);

            list.RDT = DataType.CurrentDateTime; //抄送日期.
            list.Rec = WebUser.No;
            list.WorkID = workID;
            list.PFlowNo = pFlowNo;
            list.PWorkID = pWorkID;
            list.Domain = gwf.Domain;
            list.OrgNo = gwf.OrgNo; //设置组织编号.

            try
            {
                list.Insert();
            }
            catch
            {
                // list.CheckPhysicsTable();
                list.Update();
            }


            //记录日志.
            Glo.AddToTrack(ActionType.CC, fk_flow, workID, 0, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmpNo, toEmpName, msgTitle, null);

            //发送邮件.
            BP.WF.Dev2Interface.Port_SendMsg(toEmpNo, WebUser.Name + "把工作:" + gwf.Title, "抄送:" + msgTitle, "CC" + nd.NodeID + "_" + workID + "_", BP.WF.SMSMsgType.CC,
                gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

            return "已经成功的把工作抄送给:" + toEmpNo + "," + toEmpName;

        }
        /// <summary>
        /// 抄送到人员s
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="fk_node"></param>
        /// <param name="workID"></param>
        /// <param name="empIDs"></param>
        /// <param name="msgTitle"></param>
        /// <param name="msgDoc"></param>
        /// <param name="FID"></param>
        /// <param name="pFlowNo"></param>
        /// <param name="pWorkID"></param>
        /// <returns>返回执行结果</returns>
        public static string Node_CCToEmps(int nodeID,
            Int64 workID, string empIDs, string msgTitle, string msgDoc, Int64 FID = 0, string pFlowNo = null, Int64 pWorkID = 0)
        {
            if (DataType.IsNullOrEmpty(empIDs) == true)
                return "";

            Node nd = new Node(nodeID);
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT No,Name FROM Port_Emp WHERE No IN (" + BP.Port.Glo.GenerWhereInSQL(empIDs) + ")");
            if (dt.Rows.Count == 0)
                return "";

            string nos = "";
            string names = "";
            CCList list = new CCList();
            foreach (DataRow item in dt.Rows)
            {
                string toEmpNo = item[0].ToString();
                string toEmpName = item[1].ToString();
                names += toEmpName + ",";
                nos += toEmpNo + ",";

                list.setMyPK(workID + "_" + nodeID + "_" + toEmpNo);
                list.FID = FID;
                list.FK_Flow = nd.FK_Flow;
                list.FlowName = nd.FlowName;
                list.FK_Node = nodeID;
                list.NodeName = nd.Name;
                list.Title = msgTitle;
                list.Doc = msgDoc;
                list.CCTo = toEmpNo;
                list.CCToName = toEmpName;
                list.InEmpWorks = nd.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6
                                                                                      //写入待办和写入待办与抄送列表,状态不同
                if (nd.CCWriteTo == CCWriteTo.All || nd.CCWriteTo == CCWriteTo.Todolist)
                {
                    list.HisSta = nd.CCWriteTo == CCWriteTo.All ? CCSta.UnRead : CCSta.Read;
                }
                if (nd.IsEndNode == true)//结束节点只写入抄送列表
                {
                    list.HisSta = CCSta.UnRead;
                    list.InEmpWorks = false;
                }
                list.RDT = DataType.CurrentDateTime; //抄送日期.
                list.Rec = WebUser.No;
                list.WorkID = workID;
                list.PFlowNo = pFlowNo;
                list.PWorkID = pWorkID;
                list.OrgNo = BP.Web.WebUser.OrgNo; //设置组织编号.
                try
                {
                    list.Insert();
                }
                catch
                {
                    // list.CheckPhysicsTable();
                    list.Update();
                }

                //发送邮件.
                BP.WF.Dev2Interface.Port_SendMsg(toEmpNo, msgTitle, "CC" + nd.NodeID + "_" + workID + "_", BP.WF.SMSMsgType.CC, "CC", nd.FK_Flow, nd.NodeID, workID, 0, null);
            }
            //记录日志.
            Glo.AddToTrack(ActionType.CC, nd.FK_Flow, workID, 0, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, nos, names, msgTitle, null);

            return "抄送给:" + names;

        }
        /// <summary>
        /// 删除空白
        /// </summary>
        /// <param name="workID">要删除的ID</param>
        public static void Node_DeleteBlank(Int64 workID)
        {
            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 1)
            {
                //if (gwf.FK_Node != int.Parse(gwf.FK_Flow + "01"))
                //    throw new Exception("@该流程非Blank流程不能删除:" + gwf.Title);

                //if (gwf.WFState != WFState.Blank)
                //    throw new Exception("@非Blank状态不能删除");
                gwf.Delete();
            }

            //删除流程.
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Flow fl = new Flow(gwf.FK_Flow);
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + fl.PTable + " WHERE OID=" + dbstr + "OID ";
            ps.Add(GERptAttr.OID, workID);
            DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="workID">工作ID</param>
        public static void Node_DeleteDraft(Int64 workID)
        {
            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 1)
            {
                //if (gwf.FK_Node != int.Parse(gwf.FK_Flow + "01"))
                //    throw new Exception("@该流程非草稿流程不能删除:" + gwf.Title);
                //if (gwf.WFState != WFState.Draft)
                //    throw new Exception("@非草稿状态不能删除");
                gwf.Delete();
            }

            //删除流程.
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Flow fl = new Flow(gwf.FK_Flow);
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + fl.PTable + " WHERE OID=" + dbstr + "OID ";
            ps.Add(GERptAttr.OID, workID);
            DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 把草稿设置待办
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="workID"></param>
        public static void Node_SetDraft2Todolist(Int64 workID)
        {
            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            if (gwf.WFState == WFState.Draft || gwf.WFState == WFState.Blank)
            {
                if (gwf.FK_Node != int.Parse(gwf.FK_Flow + "01"))
                    throw new Exception("@设置待办错误，只有在开始节点时才能设置待办，现在的节点是:" + gwf.NodeName);

                GenerWorkerList gwl = new GenerWorkerList();
                int i = gwl.Retrieve(GenerWorkerListAttr.FK_Node, gwf.FK_Node,
                    GenerWorkerListAttr.WorkID, gwf.WorkID);
                if (i == 0)
                {
                    gwl.WorkID = gwf.WorkID;
                    gwl.FK_Node = gwf.FK_Node;
                    gwl.FK_NodeText = gwf.NodeName;
                    gwl.FK_Emp = WebUser.No;
                    gwl.FK_EmpText = WebUser.Name;
                    gwl.FK_Flow = gwf.FK_Flow;
                    gwl.IsPassInt = 0;
                    gwl.IsEnable = true;
                    gwl.IsRead = false;
                    gwl.RDT = DataType.CurrentDateTime;
                    gwl.CDT = DataType.CurrentDateTime;
                    gwl.DTOfWarning = DataType.CurrentDateTime;
                    gwl.Insert();
                }
                else
                {
                    //gwl.FK_Emp = WebUser.No;
                    //gwl.FK_EmpText = WebUser.Name;
                    gwl.IsPassInt = 0;
                    gwl.Update();
                }

                gwf.TodoEmps = gwl.FK_Emp + "," + gwl.FK_EmpText + ";";
                gwf.TodoEmpsNum = 1;
                gwf.WFState = WFState.Runing;
                gwf.Update();

                //重置标题
                Flow_ReSetFlowTitle(gwf.FK_Flow, gwf.FK_Node, gwf.WorkID);
                return;
            }
        }
        /// <summary>
        /// 设置当前工作的应该完成日期.
        /// </summary>
        /// <param name="workID">设置的WorkID.</param>
        /// <param name="sdt">应完成日期</param>
        public static void Node_SetSDT(Int64 workID, string sdt)
        {
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET SDT=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SDT WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=0";
            ps.Add("SDT", sdt);
            ps.Add("WorkID", workID);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET SDTOfNode=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "SDTOfNode WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID ";
            ps.Add("SDTOfNode", sdt);
            ps.Add("WorkID", workID);
            DBAccess.RunSQL(ps);

        }
        /// <summary>
        /// 设置当前工作状态为草稿,如果启用了草稿, 请在开始节点的表单保存按钮下增加上它.
        /// 注意:必须是在开始节点时调用.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        public static void Node_SetDraft(Int64 workID)
        {
            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 0)
                throw new Exception("@工作丢失..");

            if (gwf.WFState == WFState.Blank)
            {
                if (gwf.FK_Node != int.Parse(gwf.FK_Flow + "01"))
                {
                    throw new Exception("@设置草稿错误，只有在开始节点时才能设置草稿，现在的节点是:" + gwf.Title);
                }

                gwf.TodoEmps = BP.Web.WebUser.No + "," + WebUser.Name + ";";
                gwf.TodoEmpsNum = 1;
                gwf.WFState = WFState.Draft;
                gwf.Update();

                GenerWorkerList gwl = new GenerWorkerList();
                gwl.WorkID = workID;
                gwl.FK_Node = int.Parse(gwf.FK_Flow + "01");
                gwl.FK_Emp = WebUser.No;
                if (gwl.RetrieveFromDBSources() == 0)
                {
                    gwl.FK_EmpText = WebUser.Name;
                    gwl.IsPassInt = 0;
                    gwl.SDT = DataType.CurrentDateTimess;
                    gwl.DTOfWarning = DataType.CurrentDateTime;
                    gwl.IsEnable = true;
                    gwl.IsRead = true;
                    gwl.IsPass = false;
                    gwl.Insert();
                }
            }

            Flow fl = new Flow(gwf.FK_Flow);
            //string sql = "UPDATE "+fl.PTable+" SET WFStarter=1, FlowStater='"+WebUser.No+"' WHERE OID="+workID;

            string sql = "UPDATE " + fl.PTable + " SET  FlowStarter='" + WebUser.No + "',WFState=1 WHERE OID=" + workID;
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 保存参数，向工作流引擎传入的参数变量.
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="paras">参数</param>
        /// <returns></returns>
        public static bool Flow_SaveParas(Int64 workID, string paras)
        {
            AtPara ap = new AtPara(paras);
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            foreach (string key in ap.HisHT.Keys)
            {
                gwf.SetPara(key, ap.GetValStrByKey(key));
            }
            gwf.Update();
            return true;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">workid</param>
        /// <param name="wk">节点表单参数</param>
        /// <returns></returns>
        public static string Node_SaveWork(Int64 workID, Hashtable wk)
        {
            return Node_SaveWork(workID, wk, null, 0, 0);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">工作数据</param>
        /// <returns>返回执行信息</returns>
        public static string Node_SaveWork(Int64 workID, Hashtable htWork, DataSet dsDtls, Int64 fid, Int64 pworkid)
        {
            if (htWork == null)
                throw new Exception("参数错误，htWork 不能为空, 保存失败。");

            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            try
            {
                Node nd = new Node(gwf.FK_Node);
                if (nd.IsStartNode == false)
                {
                    if (nd.IsEndNode == false && WebUser.IsAdmin == false)
                        if (Dev2Interface.Flow_IsCanDoCurrentWork(workID, WebUser.No) == false)
                            return "没有执行保存.";
                    //这里取消了保存异常.
                    //throw new Exception("err@工作已经发送到下一个环节,您不能执行保存.");
                }

                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    FrmNode frmNode = new FrmNode(nd.NodeID, nd.NodeFrmID);
                    switch (frmNode.WhoIsPK)
                    {
                        case WhoIsPK.FID:
                            workID = fid;
                            break;
                        case WhoIsPK.PWorkID:
                            workID = pworkid;
                            break;
                        case WhoIsPK.P2WorkID:
                            GenerWorkFlow gwfP = new GenerWorkFlow(pworkid);
                            workID = gwfP.PWorkID;
                            break;
                        case WhoIsPK.P3WorkID:
                            string sql = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=(SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + pworkid + ")";
                            workID = DBAccess.RunSQLReturnValInt(sql, 0);
                            break;
                        default:
                            break;
                    }
                }
                Work wk = nd.HisWork;
                if (workID != 0)
                {
                    wk.OID = workID;
                    wk.RetrieveFromDBSources();
                }
                wk.ResetDefaultVal();

                #region 赋值.
                //Attrs attrs = wk.EnMap.Attrs;
                foreach (string str in htWork.Keys)
                {
                    switch (str)
                    {
                        case GERptAttr.OID:
                        case WorkAttr.MD5:
                        case WorkAttr.Emps:
                        case GERptAttr.FID:
                        case GERptAttr.FK_Dept:
                        case GERptAttr.Rec:
                        case GERptAttr.Title:
                            continue;
                        default:
                            break;
                    }

                    if (wk.Row.ContainsKey(str))
                    {
                        wk.SetValByKey(str, htWork[str]);
                    }
                    else
                    {
                        wk.Row.Add(str, htWork[str]);
                    }
                }
                #endregion 赋值.

                wk.Rec = WebUser.No;
                wk.SetValByKey(GERptAttr.FK_Dept, WebUser.FK_Dept);
                ExecEvent.DoFrm(nd.MapData, EventListFrm.SaveBefore, wk);
                wk.Save();
                #region 保存从表
                if (dsDtls != null)
                {
                    //保存从表
                    foreach (DataTable dt in dsDtls.Tables)
                    {
                        foreach (MapDtl dtl in wk.HisMapDtls)
                        {
                            if (dt.TableName != dtl.No)
                                continue;

                            //获取dtls
                            GEDtls daDtls = new GEDtls(dtl.No);
                            daDtls.Delete(GEDtlAttr.RefPK, workID); // 清除现有的数据.

                            // 为从表复制数据.
                            foreach (DataRow dr in dt.Rows)
                            {
                                GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                                daDtl.RefPK = workID.ToString();
                                //明细列.
                                foreach (DataColumn dc in dt.Columns)
                                {
                                    //设置属性.
                                    daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                                }

                                daDtl.ResetDefaultVal();

                                daDtl.RefPK = workID.ToString();
                                daDtl.RDT = DataType.CurrentDateTime;

                                //执行保存.
                                daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                            }
                        }
                    }
                }
                #endregion 保存从表结束

                #region 更新发送参数.
                if (htWork != null)
                {
                    string paras = "";
                    foreach (string key in htWork.Keys)
                    {
                        var val = htWork[key];
                        if (val == null)
                            val = "";
                        paras += "@" + key + "=" + val.ToString();
                    }

                    if (DataType.IsNullOrEmpty(paras) == false && Glo.IsEnableTrackRec == true)
                    {
                        string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
                        Paras ps = new Paras();
                        ps.SQL = "UPDATE WF_GenerWorkerlist SET AtPara=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                        ps.Add(GenerWorkerListAttr.Paras, paras);
                        ps.Add(GenerWorkerListAttr.WorkID, workID);
                        ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                        DBAccess.RunSQL(ps);
                    }
                }
                #endregion 更新发送参数.

                if (nd.SaveModel == SaveModel.NDAndRpt)
                {
                    /* 如果保存模式是节点表与Node与Rpt表. */
                    WorkNode wn = new WorkNode(wk, nd);
                    GERpt rptGe = nd.HisFlow.HisGERpt;
                    rptGe.SetValByKey("OID", workID);
                    wn.rptGe = rptGe;
                    if (rptGe.RetrieveFromDBSources() == 0)
                    {
                        rptGe.SetValByKey("OID", workID);
                        wn.DoCopyWorkToRpt(wk);

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                        {
                            rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No + "," + WebUser.Name + "@");
                        }

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDOnly)
                        {
                            rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No + "@");
                        }

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                        {
                            rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.Name + "@");
                        }

                        rptGe.SetValByKey(GERptAttr.FlowStarter, WebUser.No);
                        rptGe.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDateTime);
                        rptGe.SetValByKey(GERptAttr.WFState, 0);
                        rptGe.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);
                        rptGe.SetValByKey(GERptAttr.FK_Dept, WebUser.FK_Dept);
                        rptGe.Insert();
                    }
                    else
                    {
                        wn.DoCopyWorkToRpt(wk);
                        rptGe.Update();
                    }
                }
                //获取表单树的数据
                BP.WF.WorkNode workNode = new WorkNode(workID, gwf.FK_Node);
                Work treeWork = workNode.CopySheetTree();
                if (treeWork != null)
                {
                    wk.Copy(treeWork);
                    wk.Update();
                }

                #region 处理保存后事件
                bool isHaveSaveAfter = false;
                try
                {
                    //处理表单保存后.
                    string s = ExecEvent.DoFrm(nd.MapData, EventListFrm.SaveAfter, wk);

                    //执行保存前事件.
                    s += ExecEvent.DoNode(EventListNode.NodeFrmSaveAfter, workNode, null);

                    if (s != null)
                    {
                        /*如果不等于null,说明已经执行过数据保存，就让其从数据库里查询一次。*/
                        wk.RetrieveFromDBSources();
                        isHaveSaveAfter = true;
                    }
                }
                catch (Exception ex)
                {
                    return "err@在执行保存后的事件期间出现错误:" + ex.Message;
                }
                #endregion

                Flow fl = new Flow(gwf.FK_Flow);
                //不是开始节点
                string titleRoleNodes = fl.TitleRoleNodes;
                if (nd.IsStartNode == false && DataType.IsNullOrEmpty(titleRoleNodes) == false)
                {
                    if ((titleRoleNodes + ",").Contains(nd.NodeID + ",") == true || titleRoleNodes.Equals("*") == true)
                    {
                        //设置标题.
                        string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);

                        //修改RPT表的标题
                        wk.SetValByKey(GERptAttr.Title, title);
                        wk.Update();
                        GenerWorkFlow mygwf = new GenerWorkFlow(workID);
                        mygwf.Title = title; //标题.
                        mygwf.Update();
                    }
                }
                return "保存成功.";
            }
            catch (Exception ex)
            {
                throw new Exception("err@保存错误:" + ex.Message + ", 技术信息：" + ex.StackTrace);
            }
        }
        /// <summary>
        /// 保存独立表单
        /// </summary>
        /// <param name="fk_mapdata">独立表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htData">独立表单数据Key Value 格式存放.</param>
        /// <returns>返回执行信息</returns>
        public static void Node_SaveFlowSheet(string fk_mapdata, Int64 workID, Hashtable htData)
        {
            Node_SaveFlowSheet(fk_mapdata, workID, htData, null);
        }
        /// <summary>
        /// 保存独立表单
        /// </summary>
        /// <param name="fk_mapdata">独立表单ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htData">独立表单数据Key Value 格式存放.</param>
        /// <param name="workDtls">从表数据</param>
        /// <returns>返回执行信息</returns>
        public static void Node_SaveFlowSheet(string fk_mapdata, Int64 workID, Hashtable htData, DataSet workDtls)
        {
            MapData md = new MapData(fk_mapdata);
            GEEntity en = md.HisGEEn;
            en.SetValByKey("OID", workID);
            int i = en.RetrieveFromDBSources();

            foreach (string key in htData.Keys)
            {
                en.SetValByKey(key, htData[key].ToString());
            }

            en.SetValByKey("OID", workID);

            ExecEvent.DoFrm(md, EventListFrm.SaveBefore, en);

            if (i == 0)
            {
                en.Insert();
            }
            else
            {
                en.Update();
            }

            ExecEvent.DoFrm(md, EventListFrm.SaveAfter, en);


            if (workDtls != null)
            {
                MapDtls dtls = new MapDtls(fk_mapdata);
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in dtls)
                    {
                        if (dt.TableName != dtl.No)
                        {
                            continue;
                        }
                        //获取dtls
                        GEDtls daDtls = new GEDtls(dtl.No);
                        daDtls.Delete(GEDtlAttr.RefPK, workID); // 清除现有的数据.

                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = workID.ToString();

                        // 为从表复制数据.
                        foreach (DataRow dr in dt.Rows)
                        {
                            daDtl.ResetDefaultVal();
                            daDtl.RefPK = workID.ToString();

                            //明细列.
                            foreach (DataColumn dc in dt.Columns)
                            {
                                //设置属性.
                                daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                            }
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                        }
                    }
                }
            }

            ExecEvent.DoFrm(md, EventListFrm.SaveAfter, en);
        }
        /// <summary>
        /// 从任务池里取出来一个子任务
        /// </summary>
        /// <param name="nodeid">节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="empNo">取出来的人员编号</param>
        public static bool Node_TaskPoolTakebackOne(Int64 workid)
        {
            if (Glo.IsEnableTaskPool == false)
            {
                throw new Exception("@配置没有设置成共享任务池的状态。");
            }

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.TaskSta == TaskSta.None)
            {
                throw new Exception("@该任务非共享任务。");
            }

            if (gwf.TaskSta == TaskSta.Takeback)
            {
                throw new Exception("@该任务已经被其他人取走。");
            }

            //更新状态。
            gwf.TaskSta = TaskSta.Takeback;
            gwf.Update();

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            //设置已经被取走的状态。
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=-1 WHERE IsEnable=1 AND WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp!=" + dbstr + "FK_Emp ";
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            ps.Add(GenerWorkerListAttr.FK_Node, gwf.FK_Node);
            ps.Add(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
            int i = DBAccess.RunSQL(ps);

            BP.WF.Dev2Interface.WriteTrackInfo(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, workid, 0, "任务被" + WebUser.Name + "从任务池取走.", "获取");
            if (i > 0)
            {
                Paras ps1 = new Paras();
                //取走后 将WF_GenerWorkFlow 中的 TodoEmps,TodoEmpsNum 修改下  杨玉慧 
                ps1.SQL = "UPDATE WF_GenerWorkFlow SET TodoEmps=" + dbstr + "TodoEmps,TodoEmpsNum=1 WHERE  WorkID=" + dbstr + "WorkID";
                string toDoEmps = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name;
                ps1.Add(GenerWorkFlowAttr.TodoEmps, toDoEmps);
                ps1.Add(GenerWorkerListAttr.WorkID, workid);
                BP.DA.Log.DebugWriteInfo(toDoEmps);
                BP.DA.Log.DebugWriteInfo(ps1.SQL);
                DBAccess.RunSQL(ps1);
            }

            if (i == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DataTable Node_GenerDTOfToNodes(GenerWorkFlow gwf, Node nd)
        {
            //增加转向下拉框数据.
            if (nd.CondModel == DirCondModel.ByDDLSelected || nd.CondModel == DirCondModel.ByButtonSelected)
            {
            }
            else
            {
                return null;
            }
            DataTable dtToNDs = new DataTable("ToNodes");
            dtToNDs.Columns.Add("No", typeof(string));   //节点ID.
            dtToNDs.Columns.Add("Name", typeof(string)); //到达的节点名称.
            dtToNDs.Columns.Add("IsSelectEmps", typeof(string)); //是否弹出选择人的对话框？
            dtToNDs.Columns.Add("IsSelected", typeof(string));  //是否选择？
            dtToNDs.Columns.Add("DeliveryParas", typeof(string));  //自定义URL

            DataRow dr = dtToNDs.NewRow();

            if (nd.IsStartNode == true || (gwf.TodoEmps.Contains(WebUser.No + ",") == true))
            {
                /*如果当前不是主持人,如果不是主持人，就不让他显示下拉框了.*/

                /*如果当前节点，是可以显示下拉框的.*/
                //Nodes nds = nd.HisToNodes;

                BP.WF.Template.NodeSimples nds = nd.HisToNodeSimples;

                #region 增加到达延续子流程节点。 @lizhen.
                if (nd.SubFlowYanXuNum >= 1)
                {
                    SubFlowYanXus ygflows = new SubFlowYanXus(nd.NodeID);
                    foreach (SubFlowYanXu item in ygflows)
                    {
                        string[] yanxuToNDs = item.YanXuToNode.Split(',');
                        foreach (string str in yanxuToNDs)
                        {
                            if (DataType.IsNullOrEmpty(str) == true)
                                continue;

                            int toNodeID = int.Parse(str);

                            Node subNode = new Node(toNodeID);

                            dr = dtToNDs.NewRow(); //创建行。 @lizhen.

                            //延续子流程跳转过了开始节点
                            if (toNodeID == int.Parse(int.Parse(item.SubFlowNo) + "01"))
                            {
                                dr["No"] = toNodeID.ToString();
                                dr["Name"] = "启动:" + item.SubFlowName + " - " + subNode.Name;
                                dr["IsSelectEmps"] = "1";
                                dr["IsSelected"] = "0";
                                dtToNDs.Rows.Add(dr);
                            }
                            else
                            {

                                dr["No"] = toNodeID.ToString();
                                dr["Name"] = "启动:" + item.SubFlowName + " - " + subNode.Name;
                                if (subNode.HisDeliveryWay == DeliveryWay.BySelected)
                                    dr["IsSelectEmps"] = "1";
                                else
                                    dr["IsSelectEmps"] = "0";
                                dr["IsSelected"] = "0";
                                dtToNDs.Rows.Add(dr);
                            }
                        }
                    }
                }
                #endregion 增加到达延续子流程节点。

                #region 到达其他节点.
                //上一次选择的节点.
                int defalutSelectedNodeID = 0;
                if (nds.Count > 1)
                {
                    string mysql = "";
                    // 找出来上次发送选择的节点.
                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                        mysql = "SELECT  top 1 NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + nd.NodeID + " AND ActionType=1 ORDER BY WorkID DESC";
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR3 || BP.Difference.SystemConfig.AppCenterDBType == DBType.KingBaseR6)
                        mysql = "SELECT * FROM ( SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + nd.NodeID + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                        mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + nd.NodeID + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1";
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                        mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + nd.NodeID + " AND ActionType=1 ORDER BY WorkID  DESC limit 1";

                    //获得上一次发送到的节点.
                    defalutSelectedNodeID = DBAccess.RunSQLReturnValInt(mysql, 0);
                }

                #region 为天业集团做一个特殊的判断.
                if (BP.Difference.SystemConfig.CustomerNo == "TianYe" && nd.Name.Contains("董事长") == true)
                {
                    /*如果是董事长节点, 如果是下一个节点默认的是备案. */
                    foreach (Node item in nds)
                    {
                        if (item.Name.Contains("备案") == true && item.Name.Contains("待") == false)
                        {
                            defalutSelectedNodeID = item.NodeID;
                            break;
                        }
                    }
                }
                #endregion 为天业集团做一个特殊的判断.

                #region 是否增加退回的节点
                int returnNode = 0;
                if (gwf.WFState == WFState.ReturnSta && nd.GetParaInt("IsShowReturnNodeInToolbar") == 1)
                {
                    string mysql = "";
                    ReturnWorks returnWorks = new ReturnWorks();
                    QueryObject qo = new QueryObject(returnWorks);
                    qo.AddWhere(ReturnWorkAttr.WorkID, gwf.WorkID);
                    qo.addAnd();
                    qo.AddWhere(ReturnWorkAttr.ReturnToNode, gwf.FK_Node);
                    qo.addAnd();
                    qo.AddWhere(ReturnWorkAttr.ReturnToEmp, WebUser.No);
                    qo.addOrderByDesc(ReturnWorkAttr.RDT);
                    qo.DoQuery();
                    if (returnWorks.Count != 0)
                    {
                        ReturnWork returnWork = returnWorks[0] as ReturnWork;
                        dr = dtToNDs.NewRow();
                        dr["No"] = returnWork.ReturnNode;
                        dr["Name"] = returnWork.ReturnNodeName + "(退回)";
                        dr["IsSelected"] = "1";
                        dr["IsSelectEmps"] = "0";
                        dtToNDs.Rows.Add(dr);
                        returnNode = returnWork.ReturnNode;
                        defalutSelectedNodeID = 0;//设置默认。
                    }
                }
                #endregion 是否增加退回的节点.

                foreach (BP.WF.Template.NodeSimple item in nds)
                {
                    if (item.NodeID == returnNode)
                        continue;

                    dr = dtToNDs.NewRow();
                    dr["No"] = item.NodeID;
                    dr["Name"] = item.Name;

                    //判断到达的节点是不是双向箭头的节点
                    if (item.IsResetAccepter == false && item.HisToNDs.Contains("@" + nd.NodeID) == true && nd.HisToNDs.Contains("@" + item.NodeID) == true)
                    {
                        GenerWorkerLists gwls = new GenerWorkerLists();
                        gwls.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID, GenerWorkerListAttr.FK_Node, item.NodeID, GenerWorkerListAttr.IsPass, 1);
                        if (gwls.Count > 0)
                        {
                            dr["IsSelectEmps"] = "0";
                            //设置默认选择的节点.
                            if (defalutSelectedNodeID == item.NodeID)
                                dr["IsSelected"] = "1";
                            else
                                dr["IsSelected"] = "0";

                            dtToNDs.Rows.Add(dr);
                            continue;
                        }
                    }

                    if (item.HisDeliveryWay == DeliveryWay.BySelected)
                        dr["IsSelectEmps"] = "1";
                    else if (item.HisDeliveryWay == DeliveryWay.BySelfUrl)
                    {
                        dr["IsSelectEmps"] = "2";
                        dr["DeliveryParas"] = item.DeliveryParas;
                    }
                    else if (item.HisDeliveryWay == DeliveryWay.BySelectedEmpsOrgModel)
                        dr["IsSelectEmps"] = "3";
                    else if (item.HisDeliveryWay == DeliveryWay.BySelectEmpByOfficer)
                        dr["IsSelectEmps"] = "5";
                    else
                        dr["IsSelectEmps"] = "0";  //是不是，可以选择接受人.

                    //设置默认选择的节点.
                    if (defalutSelectedNodeID == item.NodeID)
                        dr["IsSelected"] = "1";
                    else
                        dr["IsSelected"] = "0";

                    dtToNDs.Rows.Add(dr);
                }
                #endregion 到达其他节点。
            }

            return dtToNDs;
        }

        /// <summary>
        /// 放入一个任务
        /// </summary>
        /// <param name="nodeid">节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="empNo">人员ID</param>
        public static void Node_TaskPoolPutOne(Int64 workid)
        {
            if (Glo.IsEnableTaskPool == false)
            {
                throw new Exception("@配置没有设置成共享任务池的状态。");
            }

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.TaskSta == TaskSta.None)
            {
                throw new Exception("@该任务非共享任务。");
            }

            if (gwf.TaskSta == TaskSta.Sharing)
            {
                throw new Exception("@该任务已经是共享状态。");
            }

            // 更新 状态。
            gwf.TaskSta = TaskSta.Sharing;
            gwf.Update();

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            //设置已经被取走的状态。
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=1 WHERE IsEnable=-1 AND WorkID=" + dbstr + "WorkID ";
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            int i = DBAccess.RunSQL(ps);
            if (i < 0)//有可能是只有一个人
            {
                throw new Exception("@流程数据错误,不应当更新不到数据。");
            }

            if (i > 0)
            {
                Paras ps1 = new Paras();
                //设置已经被取走的状态。
                ps1.SQL = "SELECT FK_Emp,FK_EmpText FROM WF_GenerWorkerlist  WHERE IsEnable=1 AND WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                ps1.Add(GenerWorkerListAttr.WorkID, workid);
                ps1.Add(GenerWorkerListAttr.FK_Node, gwf.FK_Node);
                ps1.Add(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
                DataTable toDoEmpsTable = DBAccess.RunSQLReturnTable(ps1);
                string toDoEmps = string.Empty;
                string toDoEmpsNum = string.Empty;
                if (toDoEmpsTable == null || toDoEmpsTable.Rows.Count == 0)
                {
                    throw new Exception("@流程数据错误,没有找到需更新的待处理人。");
                }

                toDoEmpsNum = toDoEmpsTable.Rows.Count.ToString();
                foreach (DataRow dr in toDoEmpsTable.Rows)
                {
                    toDoEmps += string.Format("{0},{1}", dr["FK_Emp"].ToString(), dr["FK_EmpText"].ToString()) + ";";
                }
                Paras ps2 = new Paras();
                //将任务放回后 将WF_GenerWorkFlow 中的 TodoEmps,TodoEmpsNum 修改下  杨玉慧 
                ps2.SQL = "UPDATE WF_GenerWorkFlow SET TodoEmps=" + dbstr + "TodoEmps,TodoEmpsNum=" + dbstr + "TodoEmpsNum WHERE  WorkID=" + dbstr + "WorkID";
                ps2.Add(GenerWorkFlowAttr.TodoEmps, toDoEmps);
                ps2.Add(GenerWorkFlowAttr.TodoEmpsNum, toDoEmpsNum);
                ps2.Add(GenerWorkerListAttr.WorkID, workid);
                BP.DA.Log.DebugWriteInfo(toDoEmps);
                BP.DA.Log.DebugWriteInfo(ps2.SQL);
                DBAccess.RunSQL(ps2);
            }

            BP.WF.Dev2Interface.WriteTrackInfo(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, workid, 0, "任务被" + WebUser.Name + "放入了任务池.", "放入");
        }
        /// <summary>
        /// 通用选择器下一点接收人保存到SelectAccper表中
        /// </summary>
        /// <param name="workID"></param>
        /// <param name="toNodeID"></param>
        /// <param name="emps"></param>
        /// <param name="stas"></param>
        /// <param name="depts"></param>
        /// <param name="del_Selected"></param>
        public static string Node_AddNextStepAccepters(Int64 workID, int toNodeID, string emps = null, string depts = null, string stas = null, bool del_Selected = true)
        {
            if (DataType.IsNullOrEmpty(emps) == true && DataType.IsNullOrEmpty(stas) == true && DataType.IsNullOrEmpty(depts) == true)
                return "err@请选择下一个节点的接受人";

            SelectAccper sa = new SelectAccper();
            //删除历史选择
            if (del_Selected == true)
            {
                sa.Delete(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, workID);
            }

            //检查是否是单选？
            BP.WF.Template.Selector st = new Selector(toNodeID);
            if (st.IsSimpleSelector == true)
            {
                sa.Delete(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, workID);
            }
            string sendEmps = ",";
            Emp emp = null;
            //发送到人
            if (emps != null)
            {
                string[] empStrs = emps.Split(',');
                foreach (string empNo in empStrs)
                {
                    if (DataType.IsNullOrEmpty(empNo) == true)
                        continue;


                    emp = new Emp();
                    emp.UserID = empNo;
                    if (emp.RetrieveFromDBSources() == 0)
                        continue;

                    sendEmps += emp.UserID + ",";
                    sa.FK_Emp = emp.UserID;
                    sa.EmpName = emp.Name;
                    sa.DeptName = emp.FK_DeptText;
                    sa.FK_Node = toNodeID;
                    sa.WorkID = workID;
                    sa.ResetPK();
                    if (sa.IsExits == false)
                    {
                        sa.Insert();
                    }
                }
            }
            string sql = "";
            string empno = "";
            string empname = "";
            //发送到部门
            if (depts != null)
            {
                string[] deptStrs = depts.Split(',');
                foreach (string deptNo in deptStrs)
                {
                    if (DataType.IsNullOrEmpty(deptNo) == true)
                        continue;

                    sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name,FK_Dept FROM Port_Emp WHERE FK_Dept='" + deptNo + "'";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        empno = dr[0].ToString();
                        empname = dr[1].ToString();
                        if (sendEmps.Contains("," + empno + ",") == true)
                            continue;
                        emp = new Emp();
                        sendEmps += empno + ",";
                        sa.FK_Emp = empno;
                        sa.EmpName = empname;
                        sa.DeptName = emp.FK_DeptText;
                        sa.FK_Node = toNodeID;
                        sa.WorkID = workID;
                        sa.ResetPK();

                        sa.Save();



                    }

                }
            }
            if (stas != null)
            {
                string empNo = "No";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    empNo = "UserID as No";

                stas = stas.Replace(";", ",");
                string[] mystas = stas.Split(',');
                foreach (string staNo in mystas)
                {
                    if (DataType.IsNullOrEmpty(staNo) == true)
                        continue;
                    sql = "SELECT A." + empNo + ", A.Name, a.FK_Dept FROM Port_Emp A, Port_DeptEmpStation B  WHERE a.No=B.FK_Emp AND B.FK_Station='" + staNo + "'";

                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        empno = dr[0].ToString();
                        empname = dr[1].ToString();
                        if (sendEmps.Contains("," + empno + ",") == true)
                            continue;
                        emp = new Emp();
                        sendEmps += empno + ",";
                        sa.FK_Emp = empno;
                        sa.EmpName = empname;
                        sa.DeptName = emp.FK_DeptText;
                        sa.FK_Node = toNodeID;
                        sa.WorkID = workID;
                        sa.ResetPK();

                        sa.Save();
                    }

                }
            }

            if (DataType.IsNullOrEmpty(empno) == true)
                return "err@请选择下一个节点的接受人";
            else
                return "保存成功";


        }

        #region 多线程信号量
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(100); //限制最大并发数为100
        #endregion
        /// <summary>
        /// 增加下一步骤的接受人(用于当前步骤向下一步骤发送时增加接受人)
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="toNodeID">到达的节点ID</param>
        /// <param name="emps">如果多个就用逗号分开</param>
        /// <param name="Del_Selected">是否删除历史选择</param>
        public static void Node_AddNextStepAccepters(Int64 workID, int toNodeID, string emps, bool del_Selected = true)
        {
            if (DataType.IsNullOrEmpty(emps) == true)
                return;

            SelectAccper sa = new SelectAccper();

            string selectEmps = ",";
            //删除历史选择
            if (del_Selected == true)
            {
                sa.Delete(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, workID);
            }

            //检查是否是单选？
            BP.WF.Template.Selector st = new Selector(toNodeID);
            if (st.IsSimpleSelector == true)
            {
                sa.Delete(SelectAccperAttr.FK_Node, toNodeID, SelectAccperAttr.WorkID, workID);
            }
            else
            {
                SelectAccpers sas = new SelectAccpers();
                sas.Retrieve(SelectAccperAttr.WorkID, workID, SelectAccperAttr.FK_Node, toNodeID);

                foreach (SelectAccper item in sas)
                    selectEmps += item.FK_Emp + ",";
            }


            string[] empStrs = emps.Split(',');
            if (empStrs.Length == 0) return;
            #region 筛选需要执行的任务
            DataTable dt = DBAccess.RunSQLReturnTable("Select e.FK_Dept, e.No as UserID, e.Name, d.Name as FK_DeptText from Port_Emp e left join port_dept d on e.FK_Dept = d.No where e.No IN ('" + String.Join("','", empStrs) + "')");
            List<DataRow> empRowList = new List<DataRow>();
            foreach (DataRow dr in dt.Rows)
            {
                string empNo = dr[1].ToString();

                if (selectEmps.IndexOf("," + empNo + ",") >= 0)
                    continue;
                empRowList.Add(dr);
            }
            int taskCount = empRowList.Count;
            if (taskCount == 0) return;         // 没有任务直接返回
            #endregion

            #region 多线程并行写入
            CountdownEvent cdEvent = new CountdownEvent(taskCount);
            empRowList.ForEach(dr =>
            {
                ThreadPool.QueueUserWorkItem(o =>
                {
                    _semaphore.Wait();
                    try
                    {
                        string empNo = dr[1].ToString();
                        string MyPK = toNodeID + "_" + workID + "_" + empNo;
                        string sql = "INSERT INTO wf_selectaccper (MyPK, FK_Emp, EmpName, DeptName, FK_Node, WorkID, FK_Dept,AccType) VALUES ";
                        sql += "( '" + MyPK + "','" + empNo + "','" + dr[2] + "','" + dr[3] + "'," + toNodeID + "," + workID + ",'" + dr["FK_Dept"] + "',0 )";
                        DBAccess.RunSQL(sql);
                    }
                    catch (Exception ex)
                    {
                        BP.DA.Log.DebugWriteError("插入人员" + dr[1].ToString() + "失败, " + ex.Message);
                    }
                    finally
                    {
                        _semaphore.Release();
                        //affectRows++;
                        cdEvent.Signal();
                    }
                });
            });
            cdEvent.Wait();
            #endregion
        }
        /// <summary>
        /// 增加下一步骤的接受人(用于当前步骤向下一步骤发送时增加接受人)
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="formNodeID">从节点ID</param>
        /// <param name="emp">接收人</param>
        /// <param name="tag">分组维度，可以为空.是为了分流节点向下发送时候，可能有一个工作人员两个或者两个以上的子线程的情况出现。
        /// tag 是个维度，这个维度可能是一个类别，一个批次，一个标记，总之它是一个字符串。详细: http://bbs.ccbpm.cn/showtopic-3065.aspx </param>
        public static void Node_AddNextStepAccepter(Int64 workID, int formNodeID, string emp, string tag)
        {
            SelectAccper sa = new SelectAccper();
            sa.Delete(SelectAccperAttr.FK_Node, formNodeID, SelectAccperAttr.WorkID, workID, SelectAccperAttr.FK_Emp, emp, SelectAccperAttr.Tag, tag);

            Emp empEn = new Emp(emp);
            sa.setMyPK(formNodeID + "_" + workID + "_" + emp + "_" + tag);
            sa.Tag = tag;
            sa.FK_Emp = emp;
            sa.EmpName = empEn.Name;
            sa.FK_Node = formNodeID;

            sa.WorkID = workID;
            sa.Insert();
        }
        /// <summary>
        /// 撤销挂起
        /// </summary>
        /// <param name="workid">要撤销的原因</param>
        /// <returns>执行结果</returns>
        public static string Node_HungupWork_Un(Int64 workid)
        {
            BP.WF.WorkFlow wf = new WorkFlow(workid);
            return wf.DoHungupWork_Un();
        }
        /// <summary>
        /// 节点工作挂起
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="way">挂起方式0，永久挂起, 1.挂起到指定的日期.</param>
        /// <param name="reldata">解除挂起日期(可以为空)</param>
        /// <param name="hungNote">挂起原因</param>
        /// <returns>返回执行信息</returns>
        public static string Node_HungupWork(Int64 workid, int wayInt, string reldata, string hungNote)
        {
            HungupWay way = (HungupWay)wayInt;
            BP.WF.WorkFlow wf = new WorkFlow(workid);
            return wf.DoHungup(way, reldata, hungNote);
        }

        /// <summary>
        /// 同意挂起
        /// </summary>
        /// <param name="workid">工作ID</param>
        public static string Node_HungupWorkAgree(Int64 workid)
        {
            BP.WF.WorkFlow wf = new WorkFlow(workid);
            return wf.HungupWorkAgree();
        }
        /// <summary>
        /// 拒绝挂起
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="msg"></param>
        public static string Node_HungupWorkReject(Int64 workid, string msg)
        {
            BP.WF.WorkFlow wf = new WorkFlow(workid);
            return wf.HungupWorkReject(msg);
        }
        /// <summary>
        /// 获取该节点上的挂起时间
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>返回时间串，如果没有挂起的动作就抛出异常.</returns>
        public static TimeSpan Node_GetHungupTimeSpan(string flowNo, int nodeID, Int64 workid)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;

            string instr = (int)ActionType.Hungup + "," + (int)ActionType.UnHungup;
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + dbstr + "WorkID AND " + TrackAttr.ActionType + " in (" + instr + ")  and  NDFrom=" + dbstr + "NDFrom ";
            ps.Add(TrackAttr.WorkID, workid);
            ps.Add(TrackAttr.NDFrom, nodeID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);

            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            foreach (DataRow item in dt.Rows)
            {
                ActionType at = (ActionType)item[TrackAttr.ActionType];

                //挂起时间.
                if (at == ActionType.Hungup)
                {
                    dtStart = DataType.ParseSysDateTime2DateTime(item[TrackAttr.RDT].ToString());
                }

                //解除挂起时间.
                if (at == ActionType.UnHungup)
                {
                    dtEnd = DataType.ParseSysDateTime2DateTime(item[TrackAttr.RDT].ToString());
                }
            }

            TimeSpan ts = dtEnd - dtStart;
            return ts;
        }
        /// <summary>
        /// 执行加签
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="askfor">加签方式</param>
        /// <param name="askForEmp">请求人员</param>
        /// <param name="askForNote">内容</param>
        /// <returns></returns>
        public static string Node_Askfor(Int64 workid, AskforHelpSta askforSta, string askForEmp, string askForNote)
        {
            //检查人员是否存在.
            Emp emp = new Emp();
            emp.UserID = askForEmp;
            if (emp.RetrieveFromDBSources() == 0)
            {
                throw new Exception("@要加签的人员编号错误:" + askForEmp);
            }

            //获得当前流程注册信息.
            BP.WF.GenerWorkFlow gwf = new GenerWorkFlow(workid);

            //检查当前人员是否开可以执行当前的工作?
            if (Flow_IsCanDoCurrentWork(gwf.WorkID, WebUser.No) == false)
            {
                throw new Exception("@当前的工作已经被别人处理或者您没有处理该工作的权限.");
            }

            //检查被加签的人是否在当前的队列中.
            GenerWorkerLists gwls = new GenerWorkerLists(workid, gwf.FK_Node);
            if (gwls.Contains(GenerWorkerListAttr.FK_Emp, askForEmp, GenerWorkerListAttr.IsEnable, 0) == true)
            {
                throw new Exception("@加签失败，您选择的加签人可以处理当前的工作。");
            }

            gwf.WFState = WFState.Askfor; // 更新流程为加签状态.
            gwf.Update();

            // 设置当前状态为 2 表示加签状态.
            if (gwls.Count == 0)
            {
                /*可能是第一个节点.*/
                GenerWorkerList gwl = new GenerWorkerList();
                gwl.Copy(gwf);
                gwl.WorkID = workid;
                gwl.FK_Emp = askForEmp;
                gwl.FK_Node = gwf.FK_Node;
                gwl.FK_NodeText = gwl.FK_NodeText;
                gwl.FK_Emp = BP.Web.WebUser.No;
                gwl.FK_EmpText = BP.Web.WebUser.Name;
                gwl.FK_Dept = BP.Web.WebUser.FK_Dept;
                gwl.DeptName = WebUser.FK_DeptName;

                gwl.IsPassInt = (int)askforSta;
                gwl.Insert();
                //重新查询.
                gwls = new GenerWorkerLists(workid, gwf.FK_Node);

                //设置流程标题.
                if (gwf.Title.Length == 0)
                {
                    Flow_SetFlowTitle(gwf.FK_Flow, workid, "来自" + WebUser.Name + "的工作加签.");
                }
            }
            // endWarning.


            // 处理状态.
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsEnable == false)
                {
                    continue;
                }

                if (item.FK_Emp == WebUser.No)
                {
                    // GenerWorkerList gwl = gwls[0] as GenerWorkerList;
                    item.IsPassInt = (int)askforSta;
                    item.Update();

                    // 更换主键后，执行insert ,让被加签人有代办工作.
                    item.IsPassInt = 0;
                    item.FK_Emp = emp.UserID;
                    item.FK_EmpText = emp.Name;
                    try
                    {
                        item.Insert();
                    }
                    catch
                    {
                        item.Update();
                    }
                }
                else
                {
                    item.Update();
                }
            }

            //写入日志.
            BP.WF.Dev2Interface.WriteTrack(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, workid, gwf.FID, askForNote, ActionType.AskforHelp, "", null, null, emp.UserID, emp.Name);

            Flow fl = new Flow(gwf.FK_Flow);
            BP.WF.Dev2Interface.Port_SendMsg(askForEmp, gwf.Title, askForNote, "AK" + gwf.FK_Node + "_" + gwf.WorkID, SMSMsgType.AskFor, gwf.FK_Flow, gwf.FK_Node, workid, gwf.FID);
            //更新状态.
            DBAccess.RunSQL("UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Askfor + " WHERE OID=" + workid);

            //设置成工作未读。
            BP.WF.Dev2Interface.Node_SetWorkUnRead(workid, askForEmp);

            string msg = "您的工作已经提交给(" + askForEmp + " " + emp.Name + ")加签了。";

            //加签后事件
            BP.WF.Node hisNode = new BP.WF.Node(gwf.FK_Node);
            Work currWK = hisNode.HisWork;
            currWK.OID = gwf.WorkID;
            currWK.Retrieve();

            WorkNode wn = new WorkNode(currWK, hisNode);

            //执行加签后的事件.
            msg += ExecEvent.DoNode(EventListNode.AskerAfter, wn, null);

            return msg;
        }
        /// <summary>
        /// 答复加签信息
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="replyNote">答复信息</param>
        /// <returns></returns>
        public static string Node_AskforReply(Int64 workid, string replyNote)
        {
            //把回复信息临时的写入 流程注册信息表以便让发送方法获取这个信息写入日志.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.Paras_AskForReply = replyNote;
            gwf.Update();

            Node nd = new Node(gwf.FK_Node);
            string info = "";
            try
            {
                //执行发送, 在发送的方法里面已经做了判断了,并且把 回复的信息写入了日志.
                info = BP.WF.Dev2Interface.Node_SendWork(gwf.FK_Flow, workid).ToMsgOfHtml();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("请选择下一步骤工作") == true || ex.Message.Contains("用户没有选择发送到的节点") == true)
                {
                    if (nd.CondModel == DirCondModel.ByDDLSelected)
                    {
                        /*如果抛出异常，我们就让其转入选择到达的节点里, 在节点里处理选择人员. */
                        return "SelectNodeUrl@./WorkOpt/ToNodes.htm?FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + gwf.FK_Node + "&WorkID=" + gwf.WorkID + "&FID=" + gwf.FID;

                    }
                    return "err@下一个节点的接收人规则是，当前节点选择来选择，在当前节点属性里您没有启动接受人按钮，系统自动帮助您启动了，请关闭窗口重新打开。" + ex.Message;
                }
                return ex.Message;
            }

            Node node = new Node(gwf.FK_Node);
            Work wk = node.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();

            //恢复加签后执行事件.
            WorkNode wn = new WorkNode(wk, node);
            info += ExecEvent.DoNode(EventListNode.AskerReAfter, wn, null);
            return info;
        }
        /// <summary>
        /// 执行工作分配
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="toEmps">要分配的人，多个人用逗号分开.</param>
        /// <param name="msg">分配原因.</param>
        /// <returns>分配信息.</returns>
        public static string Node_Allot(string flowNo, int nodeID, Int64 workID, Int64 fid, string toEmps, string msg)
        {
            //生成实例.
            GenerWorkerLists gwls = new GenerWorkerLists(workID, nodeID);

            //要分配给的人员.
            string[] empStrs = toEmps.Split(',');
            foreach (string empNo in empStrs)
            {
                if (DataType.IsNullOrEmpty(empNo) == true)
                {
                    continue;
                }

                //人员实体.
                Emp empEmp = new Emp(empNo);

                GenerWorkerList gwl = null; //接收人

                //开始找接收人.
                foreach (GenerWorkerList item in gwls)
                {
                    if (item.FK_Emp == empNo)
                    {
                        gwl = item;
                        break;
                    }
                }

                // 没有找到的情况, 就获得一个实例，作为数据样本然后把数据insert.
                if (gwl == null)
                {
                    gwl = gwls[0] as GenerWorkerList;
                    gwl.FK_Emp = empEmp.UserID;
                    gwl.FK_EmpText = empEmp.Name;
                    gwl.IsEnable = true;
                    gwl.IsPassInt = 0;
                    gwl.Insert();
                    continue;
                }

                //如果被禁用了，就启用他.
                if (gwl.IsEnable == false)
                {
                    gwl.IsEnable = true;
                    gwl.Update();
                }
            }
            return "分配成功.";
        }
        /// <summary>
        /// 工作移交
        /// </summary>
        /// <param name="workID"></param>
        /// <param name="toEmps">要移交给的人员,多个人用逗号分开.比如:zhangsan,lisi,wangwu</param>
        /// <param name="msg">移交原因.</param>
        /// <returns>执行的信息.</returns>
        public static string Node_Shift(Int64 workID, string toEmps, string msg)
        {
            if (DataType.IsNullOrEmpty(toEmps) == true)
                return "err@要移交的人员不能为空.";

            ///处理参数格式.
            toEmps = toEmps.Replace(";", ",");
            toEmps = toEmps.Replace("，", ",");

            //如果仅仅移交一个人.
            if (toEmps.IndexOf(",") == -1)
                return BP.WF.ShiftWork.Node_Shift_ToEmp(workID, toEmps, msg);

            //移交给多个人.
            return BP.WF.ShiftWork.Node_Shift_ToEmps(workID, toEmps, msg);
        }
        /// <summary>
        /// 撤销移交
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <returns>返回撤销信息</returns>
        public static string Node_ShiftUn(Int64 workID)
        {
            return BP.WF.ShiftWork.DoUnShift(workID);
        }

        /// <summary>
        /// 执行工作退回(退回指定的点)
        /// </summary>
        /// <param name="workID">退回的工作ID</param>
        /// <param name="returnToNodeID">退回到节点,如果是0,就退回上一个节点.</param>
        /// <param name="returnToEmp">退回到人员,如果是null，就退回给指定节点的处理人。</param>
        /// <param name="msg">退回信息</param>
        /// <param name="isBackToThisNode">退回后是否返回当前节点？</param>
        /// <param name="pageData">页面数据</param>
        /// <param name="isKillEtcThread">如果是退回到分流节点，是否删除其他的子线程？</param>
        /// <returns></returns>
        public static string Node_ReturnWork(Int64 workID, int returnToNodeID,
            string returnToEmp, string msg, bool isBackToThisNode = false, string pageData = null,
            bool isKillEtcThread = true)
        {

            if (DataType.IsNullOrEmpty(msg) == true)
                throw new Exception("退回信息不能为空.");

            //补偿处理退回错误.
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            string sql = "";
            DataTable dt;
            //检查退回的数据是否正确？
            if (returnToNodeID != 0)
            {
                if (DataType.IsNullOrEmpty(returnToEmp) == false)
                    sql = "SELECT WorkID FROM WF_GenerWorkerlist WHERE WorkID=" + workID + " AND FK_Emp='" + returnToEmp + "' AND FK_Node=" + returnToNodeID;
                else
                    sql = "SELECT WorkID,FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + workID + " AND FK_Node=" + returnToNodeID;
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                {
                    // 有可能是父流程.  @lizhen
                    Node pNode = new Node(returnToNodeID);
                    if (pNode.FK_Flow.Equals(gwf.PFlowNo) == false)
                        throw new Exception("err@被退回到的节点数据错误，请联系管理员.");

                    if (gwf.PWorkID == 0)
                        throw new Exception("err@被退回到的节点数据错误，请联系管理员.");

                    Emp toEmp = new Emp(returnToEmp);

                    //需要处理父流程的数据.
                    GenerWorkFlow gwfP = new GenerWorkFlow(gwf.PWorkID);
                    gwfP.WFState = WFState.ReturnSta; //设置退回状态.
                    gwfP.FK_Node = pNode.NodeID;
                    gwfP.NodeName = pNode.Name;
                    gwfP.TodoEmps = returnToEmp + "," + toEmp.Name + ";";
                    gwfP.TodoEmpsNum = 0;
                    gwfP.SendDT = DataType.CurrentDateTime;
                    gwfP.Sender = WebUser.No + "," + WebUser.Name + ";";

                    gwfP.Update();

                    //设置所有的人员信息为已经完成.
                    sql = "UPDATE  WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + gwf.PWorkID;
                    DBAccess.RunSQL(sql);

                    sql = "UPDATE  WF_GenerWorkerlist SET IsPass=0,IsRead=0,RDT='" + DataType.CurrentDateTimess + "' WHERE  FK_Emp='" + returnToEmp + "' AND  WorkID=" + gwf.PWorkID + " AND FK_Node=" + gwf.PNodeID;
                    int i = DBAccess.RunSQL(sql);
                    if (i == 0)
                    {
                        //找到父节点的gwfs
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.FK_Node = pNode.NodeID;
                        gwl.WorkID = gwf.PWorkID;
                        gwl.FK_Emp = returnToEmp;
                        gwl.IsPassInt = 0;
                        gwl.FK_Dept = toEmp.FK_Dept;
                        gwl.FK_EmpText = toEmp.Name;
                        gwl.RDT = DataType.CurrentDateTime;
                        gwl.SDT = DataType.CurrentDateTime;
                        gwl.FK_Flow = pNode.FK_Flow;
                        gwl.IsRead = false;
                        gwl.Sender = WebUser.No;
                        gwl.Save();
                    }


                    //把当前的流程删除掉.
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(workID, false);

                    return "已经退回给父流程“" + gwfP.FlowName + "”节点“" + gwfP.NodeName + "”，退回给“" + toEmp.Name + "”";
                }
                if (DataType.IsNullOrEmpty(returnToEmp) == true && dt.Rows.Count > 0)
                    returnToEmp = dt.Rows[0][1].ToString();

            }

            string info = "";
            WorkReturn wr = new WorkReturn(gwf.FK_Flow, workID, gwf.FID, gwf.FK_Node, returnToNodeID, returnToEmp, isBackToThisNode, msg, pageData);

            Node returnToNode = new Node(wr.ReturnToNodeID);
            if (isKillEtcThread == true && gwf.FID != 0
                && (returnToNode.HisRunModel == RunModel.FL || returnToNode.HisRunModel == RunModel.FHL))
            {
                //子线程退回到分流节点. 要删除其他的子线程.
                info = wr.DoItOfKillEtcThread();
            }
            else
            {
                info = wr.DoIt();
            }

            //检查退回的数据是否正确？
            sql = "SELECT WorkID FROM WF_GenerWorkerlist WHERE WorkID=" + workID + " AND FK_Emp='" + wr.ReturnToEmp + "' AND IsPass=0";
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0 && gwf.FID == 0)
            {
                /*说明数据错误了,回滚回来.*/
                BP.WF.Dev2Interface.Flow_ReSend(gwf.WorkID, gwf.FK_Node,
                    WebUser.No, "退回错误的回滚.");
                throw new Exception("err@退回出现系统错误，请联系管理员或者在执行一次退回.WorkID=" + workID);
            }
            return info;
        }
        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="returnToNodeID">要退回的节点,0 表示上一个节点或者指定的节点.</param>
        /// <param name="msg">退回信息</param>
        /// <param name="isBackToThisNode">是否原路返回</param>
        /// <returns>执行结果</returns>
        public static string Node_ReturnWork(Int64 workID, int returnToNodeID, string msg, bool isBackToThisNode)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            return Node_ReturnWork(workID, returnToNodeID, null, msg, isBackToThisNode);
        }
        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="currentNodeID">当前节点</param>
        /// <param name="returnToNodeID">退回到节点</param>
        /// <param name="msg">退回消息</param>
        /// <param name="isBackToThisNode">是否原路返回</param>
        /// <returns>退回执行的信息，执行不成功就抛出异常。</returns>
        public static string Node_ReturnWork(string fk_flow, Int64 workID, Int64 fid, int currentNodeID, int returnToNodeID, string msg, bool isBackToThisNode)
        {
            return Node_ReturnWork(workID, returnToNodeID, null, msg, isBackToThisNode);
        }
        /// <summary>
        /// 获取当前工作的NodeID
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>指定工作的NodeID.</returns>
        public static int Node_GetCurrentNodeID(string flowNo, Int64 workid)
        {
            int nodeID = DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + workid + " AND FK_Flow='" + flowNo + "'", 0);
            if (nodeID == 0)
                return int.Parse(flowNo + "01");
            return nodeID;
        }
        /// <summary>
        /// 增加子线程
        /// </summary>
        /// <param name="workID">干流程的workid</param>
        /// <param name="empStrs">要增加的子线程的工作人员，多个人员用逗号分开.</param>
        public static string Node_FHL_AddSubThread(Int64 workID, string empStrs, int toNodeID)
        {
            #region 检查参数是否正确.
            empStrs = empStrs.Replace("，", ",");
            empStrs = empStrs.Replace("；", ",");
            empStrs = empStrs.Replace(";", ",");
            string[] strs = empStrs.Split(',');
            string err = "";
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;
                BP.Port.Emp emp = new Emp();
                emp.No = str;
                if (emp.IsExits == false)
                    err += "人员账号：" + str + "不正确。";
            }

            if (DataType.IsNullOrEmpty(err) == false)
                return "err@错误：" + err;
            #endregion 检查参数是否正确.

            #region 求分流节点 Node . 求分流节点下一个子线程节点


            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            Node nd = new Node(gwf.FK_Node);
            if (nd.HisRunModel == RunModel.FL)
            {
                /* 如果节点是分流，则要在分流上,就不需要处理，因为是在分流上增加. */
            }

            //如果运行到合流节点，需要找到分流节点的，节点ID.
            if (nd.HisRunModel == RunModel.HL)
            {
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID);

                foreach (GenerWorkerList item in gwls)
                {
                    Node myNode = new Node(item.FK_Node);
                    if (myNode.HisRunModel == RunModel.FL)
                    {
                        nd = myNode;
                        break;
                    }
                }
            }



            //生成子线程的gwf.
            GenerWorkFlow gwfSub = new GenerWorkFlow();
            gwfSub.Copy(gwf);
            gwfSub.FK_Node = nd.NodeID;
            gwfSub.NodeName = nd.Name;


            //找到分流节点的下一个子线程节点.
            Node ndSubThread = null;
            if (toNodeID != 0)
                ndSubThread = new Node(toNodeID);
            else
            {
                Nodes nds = nd.HisToNodes;
                foreach (Node item in nds)
                {
                    if (item.IsSubThread == true)
                        ndSubThread = item;
                }
                if (ndSubThread == null)
                    return "err@没有找到分流节点下的子线程.";
            }


            #endregion 求分流节点 Node, 求分流节点下一个子线程节点.

            #region 开始追加子线程的处理人.

            // 求出来一个分流节点 gwl;
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID, GenerWorkerListAttr.FK_Node, nd.NodeID);
            gwl.RDT = DataType.CurrentDateTime;
            gwl.SDT = DataType.CurrentDateTime;
            gwl.IsPass = false;
            gwl.FID = gwf.WorkID; // 设置FID.
            gwl.IsRead = false;

            Work wk = nd.HisWork;
            wk.OID = gwf.WorkID;
            wk.Retrieve();

            string msg = "";
            foreach (string empNo in strs)
            {
                if (DataType.IsNullOrEmpty(empNo) == true)
                    continue;

                BP.Port.Emp emp = new Emp(empNo);

                //生成子线程的gwf.
                gwfSub.FID = gwf.WorkID;
                gwfSub.WorkID = DBAccess.GenerOID("WorkID");
                gwfSub.SendDT = DataType.CurrentDateTime;
                gwfSub.RDT = DataType.CurrentDateTime;
                gwfSub.SDTOfFlow = DataType.CurrentDateTime;
                gwfSub.SDTOfNode = DataType.CurrentDateTime;

                gwfSub.FK_Node = ndSubThread.NodeID;
                gwfSub.NodeName = ndSubThread.Name;
                gwfSub.WFState = WFState.Runing;
                gwfSub.FID = gwf.WorkID;
                gwfSub.TodoEmps = emp.No + "," + emp.Name + ";";
                gwfSub.DirectInsert();

                gwl.WorkID = gwfSub.WorkID;

                //生成干流程上的. gwl.
                gwl.FK_Node = nd.NodeID;
                gwl.FK_NodeText = nd.Name;
                gwl.FK_Dept = BP.Web.WebUser.FK_Dept;
                gwl.DeptName = BP.Web.WebUser.FK_DeptName;
                gwl.FK_Emp = BP.Web.WebUser.No;
                gwl.FK_EmpText = BP.Web.WebUser.Name;
                gwl.IsPassInt = -2;
                gwl.RDT = DataType.CurrentDateTime;
                gwl.SDT = DataType.CurrentDateTime;
                gwl.CDT = DataType.CurrentDateTime;
                gwl.Insert();


                //生成子线程的. gwl.
                gwl.FK_Node = ndSubThread.NodeID;
                gwl.FK_NodeText = ndSubThread.Name;
                gwl.FK_Emp = emp.No;
                gwl.FK_EmpText = emp.Name;
                gwl.FK_Dept = emp.FK_Dept;
                gwl.DeptName = emp.FK_DeptText;
                gwl.IsPassInt = 0;
                gwl.RDT = DataType.CurrentDateTime;
                gwl.SDT = DataType.CurrentDateTime;
                gwl.CDT = DataType.CurrentDateTime;
                gwl.Insert();

                //复制工作信息.
                Work wkSub = ndSubThread.HisWork;
                wkSub.Copy(wk);
                wkSub.OID = gwl.WorkID;
                wkSub.FID = gwl.FID;
                wkSub.SaveAsOID(gwl.WorkID);

                msg += "增加成功:" + emp.No + "," + emp.Name;
            }
            #endregion 开始追加子线程的处理人.

            return "执行信息如下：" + msg;
        }

        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <param name="workid">工作ID</param>
        public static void Node_FHL_KillSubFlow(Int64 workid)
        {
            WorkFlow wkf = new WorkFlow(workid);
            wkf.DoDeleteWorkFlowByFlag("删除子线程.");
        }
        /// <summary>
        /// 合流点驳回子线程
        /// </summary>
        /// <param name="NodeSheetfReject">流程编号</param>
        /// <param name="fid">流程ID</param>
        /// <param name="workid">子线程ID</param>
        /// <param name="msg">驳回消息</param>
        public static string Node_FHL_DoReject(int NodeSheetfReject, Int64 fid, Int64 workid, string msg)
        {
            WorkFlow wkf = new WorkFlow(workid);
            return wkf.DoReject(fid, NodeSheetfReject, msg);
        }

        /// <summary>
        /// 跳转审核取回
        /// </summary>
        /// <param name="fromNodeID">从节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="tackToNodeID">取回到的节点ID</param>
        /// <returns></returns>
        public static string Node_Tackback(int fromNodeID, Int64 workid, int tackToNodeID, string doMsg = null)
        {
            if (doMsg == null)
                doMsg = " 执行跳转审核的取回";

            /*
             * 1,首先检查是否有此权限.
             * 2, 执行工作跳转.
             * 3, 执行写入日志.
             */
            Node nd = new Node(tackToNodeID);
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByPreviousNodeFormEmpsField:
                    break;
            }

            WorkNode wn = new WorkNode(workid, fromNodeID);
            string msg = wn.NodeSend(new Node(tackToNodeID), BP.Web.WebUser.No).ToMsgOfHtml();
            wn.AddToTrack(ActionType.Tackback, WebUser.No, WebUser.Name, tackToNodeID, nd.Name, doMsg);
            return msg;
        }
        /// <summary>
        /// 执行抄送已阅
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">流程节点</param>
        /// <param name="workid">工作id</param>
        /// <param name="fid">流程id</param>
        /// <param name="checkNote">填写意见</param>
        public static void Node_DoCCCheckNote(string fk_flow, int fk_node, Int64 workid, Int64 fid, string checkNote)
        {
            NodeWorkCheck fwc = new NodeWorkCheck(fk_node);

            BP.WF.Dev2Interface.Node_WriteWorkCheck(workid, checkNote, fwc.FWCOpLabel, null);

            //设置审核完成.
            BP.WF.Dev2Interface.Node_CC_SetSta(fk_node, workid, BP.Web.WebUser.No, BP.WF.CCSta.CheckOver);
        }
        /// <summary>
        /// 设置是此工作为读取状态
        /// </summary>
        /// <param name="nodeID">节点编号</param>
        /// <param name="workid">工作ID</param>
        public static void Node_SetWorkRead(int nodeID, Int64 workid)
        {
            Node_SetWorkRead(nodeID, workid, BP.Web.WebUser.No);
        }
        /// <summary>
        /// 设置是此工作为读取状态
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">WorkID</param>
        /// <param name="empNo">操作员</param>
        public static void Node_SetWorkRead(int nodeID, Int64 workid, string empNo)
        {
            Node nd = new Node(nodeID);

            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsRead=1 WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add("WorkID", workid);
            ps.Add("FK_Node", nodeID);
            ps.Add("FK_Emp", empNo);
            if (DBAccess.RunSQL(ps) == 0)
            {
                //throw new Exception("设置的工作不存在，或者当前的登陆人员[" + empNo + "]已经改变，请重新登录。");
            }

            // 判断当前节点的已读回执.
            if (nd.ReadReceipts == ReadReceipts.None || nd.IsStartNode == true)
            {
                return;
            }

            bool isSend = false;
            if (nd.ReadReceipts == ReadReceipts.Auto)
            {
                isSend = true;
            }

            if (nd.ReadReceipts == ReadReceipts.BySysField)
            {
                /*获取上一个节点ID */
                Nodes fromNodes = nd.FromNodes;
                int fromNodeID = 0;
                foreach (Node item in fromNodes)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT FK_Node FROM WF_GenerWorkerlist WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                    ps.Add("WorkID", workid);
                    ps.Add("FK_Node", item.NodeID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        continue;

                    fromNodeID = item.NodeID;
                    break;
                }
                if (fromNodeID == 0)
                {
                    throw new Exception("@没有找到它的上一步工作。");
                }

                try
                {
                    ps = new Paras();
                    ps.SQL = "SELECT " + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + " FROM ND" + fromNodeID + "    WHERE OID=" + dbstr + "OID";
                    ps.Add("OID", workid);
                    DataTable dt1 = DBAccess.RunSQLReturnTable(ps);
                    if (dt1.Rows[0][0].ToString() == "1")
                    {
                        isSend = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("@流程设计错误:" + ex.Message + " 在当前节点上个您设置了安上一步的表单字段决定是否回执，但是上一个节点表单中没有约定的字段。");
                }
            }

            if (nd.ReadReceipts == ReadReceipts.BySDKPara)
            {
                /*如果是按开发者参数*/

                /*获取上一个节点ID*/
                Nodes fromNodes = nd.FromNodes;
                int fromNodeID = 0;
                foreach (Node item in fromNodes)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT FK_Node FROM WF_GenerWorkerlist  WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                    ps.Add("WorkID", workid);
                    ps.Add("FK_Node", item.NodeID);
                    DataTable dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        continue;
                    }

                    fromNodeID = item.NodeID;
                    break;
                }
                if (fromNodeID == 0)
                {
                    throw new Exception("@没有找到它的上一步工作。");
                }

                string paras = BP.WF.Dev2Interface.GetFlowParas(fromNodeID, workid);
                if (DataType.IsNullOrEmpty(paras) || paras.Contains("@" + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + "=") == false)
                {
                    throw new Exception("@流程设计错误:在当前节点上个您设置了按开发者参数决定是否回执，但是没有找到该参数。");
                }

                // 开发者参数.
                if (paras.Contains("@" + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + "=1") == true)
                {
                    isSend = true;
                }
            }


            if (isSend == true)
            {
                /*如果是自动的已读回执，就让它发送给当前节点的上一个发送人。*/

                // 获取流程标题.
                ps = new Paras();
                ps.SQL = "SELECT Title FROM WF_GenerWorkFlow WHERE WorkID=" + dbstr + "WorkID ";
                ps.Add("WorkID", workid);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                string title = dt.Rows[0][0].ToString();

                // 获取流程的发送人.
                ps = new Paras();
                ps.SQL = "SELECT " + GenerWorkerListAttr.Sender + " FROM WF_GenerWorkerlist WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node ";
                ps.Add("WorkID", workid);
                ps.Add("FK_Node", nodeID);
                dt = DBAccess.RunSQLReturnTable(ps);
                string sender = dt.Rows[0][0].ToString();

                //发送已读回执。
                BP.WF.Dev2Interface.Port_SendMsg(sender, "已读回执:" + title,
                    "您发送的工作已经被" + WebUser.Name + "在" + DataType.CurrentDateTimeCNOfShort + " 打开.",
                    "RP" + workid + "_" + nodeID, BP.WF.SMSMsgType.Self, nd.FK_Flow, nd.NodeID, workid, 0);
            }

            //执行节点打开后事件.
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();


            WorkNode wn = new WorkNode(wk, nd);

            //执行事件.
            ExecEvent.DoNode(EventListNode.WhenReadWork, wn, null, null);
            ///nd.HisFlow.DoFlowEventEntity(EventListNode.WhenReadWork, nd, wk, null);

        }
        /// <summary>
        /// 设置工作未读取
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="userNo">要设置的人</param>
        public static void Node_SetWorkUnRead(Int64 workid, string userNo)
        {
            string dbstr = BP.Difference.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsRead=0 WHERE WorkID=" + dbstr + "WorkID AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add("WorkID", workid);
            ps.Add("FK_Emp", userNo);
            DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 设置工作未读取
        /// </summary>
        /// <param name="workid">工作ID</param>
        public static void Node_SetWorkUnRead(Int64 workid)
        {
            Node_SetWorkUnRead(workid, BP.Web.WebUser.No);
        }
        #endregion 工作有关接口

        #region 会签相关操作.
        /// <summary>
        /// 获得当前节点会签人的信息
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="huiqianType">会签类型</param>
        /// <param name="dataset">返回结果集</param>
        public static DataSet Node_HuiQian_Init(Int64 workID)
        {
            //要找到主持人.
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            //判断流程实例的节点和当前节点是否相同
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(gwf.WorkID, WebUser.No) == false)
                    throw new Exception("err@当前工作处于{" + gwf.NodeName + "}节点,您({" + WebUser.Name + "})没有处理权限.");
            }

            if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianOver)
            {
                throw new Exception("err@会签工作已经完成，您不能在执行会签。");
            }

            //查询出来集合.
            GenerWorkerLists ens = new GenerWorkerLists(workID, gwf.FK_Node);
            BtnLab btnLab = new BtnLab(gwf.FK_Node);
            if (btnLab.HuiQianRole != HuiQianRole.TeamupGroupLeader || (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader && btnLab.HuiQianLeaderRole != HuiQianLeaderRole.OnlyOne))
            {
                foreach (GenerWorkerList item in ens)
                {

                    if ((gwf.HuiQianZhuChiRen.Contains(item.FK_Emp + ",") == true
                        || (DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true
                            && gwf.GetParaString("AddLeader").Contains(item.FK_Emp + ",") == false
                           && gwf.TodoEmps.Contains(item.FK_Emp + ",") == true))
                         && item.FK_Emp != BP.Web.WebUser.No
                         && item.IsHuiQian == false)
                    {
                        item.FK_EmpText = "<img src='../Img/zhuichiren.png' border=0 />" + item.FK_EmpText;
                        item.FK_EmpText = item.FK_EmpText;
                        if (item.IsPass == true)
                            item.IsPassInt = 1001;
                        else
                            item.IsPassInt = 100;
                        continue;
                    }

                    //标记为自己.
                    if (item.FK_Emp == BP.Web.WebUser.No)
                    {
                        item.FK_EmpText = "" + item.FK_EmpText;
                        if (item.IsPass == true)
                            item.IsPassInt = 9901;
                        else
                            item.IsPassInt = 99;
                    }
                }
            }

            //赋值部门名称。
            DataTable mydt = ens.ToDataTableField("WF_GenerWorkList");
            mydt.Columns.Add("FK_DeptT", typeof(string));
            foreach (DataRow dr in mydt.Rows)
            {
                string fk_emp = dr["FK_Emp"].ToString();
                foreach (GenerWorkerList item in ens)
                {
                    if (item.FK_Emp == fk_emp)
                        dr["FK_DeptT"] = item.DeptName;
                }
            }

            //获取当前人员的流程处理信息
            GenerWorkerList gwlOfMe = new GenerWorkerList();
            gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                        GenerWorkerListAttr.WorkID, workID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            DataSet ds = new DataSet();
            ds.Tables.Add(mydt);
            ds.Tables.Add(gwlOfMe.ToDataTableField("My_GenerWorkList"));

            return ds;
        }
        /// <summary>
        /// 增加会签人
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="huiQianType"> huiQianType=AddLeader增加组长,  </param>
        /// <param name="empStrs"></param>
        /// <returns></returns>
        public static string Node_HuiQian_AddEmps(Int64 workID, string huiQianType, string empStrs)
        {
            if (DataType.IsNullOrEmpty(empStrs) == true)
                return "err@您没有选择人员.";

            empStrs = empStrs.Replace("，", ",");
            empStrs = empStrs.Replace(";", ",");
            empStrs = empStrs.Replace("；", ",");


            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            string addLeader = gwf.GetParaString("AddLeader");
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                //判断是不是第二会签主持人
                if (addLeader.Contains(WebUser.No + ",") == false)
                    return "err@您不是会签主持人，您不能执行该操作。";
            }

            GenerWorkerList gwlOfMe = new GenerWorkerList();
            int num = gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                 GenerWorkerListAttr.WorkID, gwf.WorkID,
                 GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            if (num == 0)
                return "err@没有查询到当前人员的工作列表数据.";

            Node nd = new Node(gwf.FK_Node);
            string err = "";
            string[] myEmpStrs = empStrs.Split(',');
            int addCount = 0;
            foreach (string empStr in myEmpStrs)
            {
                if (DataType.IsNullOrEmpty(empStr) == true)
                    continue;

                Emp emp = new Emp(empStr);

                //查查是否存在队列里？
                num = gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, emp.UserID,
                        GenerWorkerListAttr.WorkID, gwf.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);

                if (num == 1)
                {
                    // err += " 人员[" + emp.UserID + "," + emp.Name + "]已经在队列里.";
                    continue;
                }
                addCount++;
                //增加组长
                if (DataType.IsNullOrEmpty(huiQianType) == false && huiQianType.Equals("AddLeader"))
                {
                    addLeader += emp.UserID + ",";
                }

                //查询出来其他列的数据.
                gwlOfMe.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                    GenerWorkerListAttr.WorkID, gwf.WorkID,
                    GenerWorkerListAttr.FK_Node, gwf.FK_Node);
                gwlOfMe.SetPara("HuiQianType", "");
                gwlOfMe.FK_Emp = emp.UserID;
                gwlOfMe.FK_EmpText = emp.Name;
                gwlOfMe.IsPassInt = -1; //设置不可以用.
                gwlOfMe.FK_Dept = emp.FK_Dept;
                gwlOfMe.DeptName = emp.FK_DeptText; //部门名称.
                gwlOfMe.IsRead = false;
                gwlOfMe.SetPara("HuiQianZhuChiRen", WebUser.No);
                //表明后增加的组长
                if (DataType.IsNullOrEmpty(huiQianType) == false && huiQianType.Equals("AddLeader"))
                    gwlOfMe.SetPara("HuiQianType", huiQianType);

                #region 计算会签时间.
                if (nd.HisCHWay == CHWay.None)
                {
                    gwlOfMe.SDT = "无";
                }
                else
                {
                    //给会签人设置应该完成日期. 考虑到了节假日.                
                    DateTime dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, nd.TimeLimit,
                         nd.TimeLimitHH, nd.TimeLimitMM, nd.TWay);
                    //应完成日期.
                    gwlOfMe.SDT = dtOfShould.ToString(DataType.SysDateTimeFormat + ":ss");
                }

                //求警告日期.
                DateTime dtOfWarning = DateTime.Now;
                if (nd.WarningDay == 0)
                {
                    //  dtOfWarning = "无";
                }
                else
                {
                    //计算警告日期。
                    // 增加小时数. 考虑到了节假日.
                    dtOfWarning = Glo.AddDayHoursSpan(DateTime.Now, (int)nd.WarningDay, 0, 0, nd.TWay);
                }
                gwlOfMe.DTOfWarning = dtOfWarning.ToString(DataType.SysDateTimeFormat);
                #endregion 计算会签时间.

                gwlOfMe.Sender = WebUser.No + "," + WebUser.Name; //发送人为当前人.
                gwlOfMe.IsHuiQian = true;
                gwlOfMe.Insert(); //插入作为待办.

            }
            gwf.SetPara("AddLeader", addLeader);
            gwf.Update();
            if (err.Equals("") == true || (addCount > 0 && err.Equals("") == false))
                return "增加成功.";
            return "err@" + err;
        }
        /// <summary>
        /// 增加会签主持人
        /// </summary>
        /// <param name="workID">流程ID</param>
        /// <returns></returns>
        public static string Node_HuiQian_AddLeader(Int64 workID)
        {
            //生成变量.
            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            //判断流程实例的节点和当前节点是否相同
            if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
            {
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(gwf.WorkID, WebUser.No) == false)
                    return "err@当前工作处于{" + gwf.NodeName + "}节点,您({" + WebUser.Name + "})没有处理权限.";
            }

            if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianOver)
            {
                /*只有一个人的情况下, 并且是会签完毕状态，就执行 */
                return "info@当前工作已经到您的待办理了,会签工作已经完成.";
            }
            string leaders = gwf.GetParaString("AddLeader");

            //获取加签的人
            GenerWorkerLists gwfs = new GenerWorkerLists();
            gwfs.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID,
                GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.IsPass, -1);
            string empsLeader = "";


            foreach (GenerWorkerList item in gwfs)
            {
                if (leaders.Contains(item.FK_Emp + ","))
                {
                    empsLeader += item.FK_Emp + "," + item.FK_EmpText + ";";
                    //发送消息
                    BP.WF.Dev2Interface.Port_SendMsg(item.FK_Emp,
                       "bpm会签邀请", "HuiQian" + gwf.WorkID + "_" + gwf.FK_Node + "_" + item.FK_Emp, BP.Web.WebUser.Name + "邀请您作为工作｛" + gwf.Title + "｝的主持人,请您在{" + item.SDT + "}前完成.", "HuiQian", gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
                }

            }
            if (DataType.IsNullOrEmpty(empsLeader) == true)
                return "没有增加新的主持人";
            leaders = "('" + leaders.Substring(0, leaders.Length - 1).Replace(",", "','") + "')";
            //恢复他的状态.
            string sql = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=-1 AND FK_Emp In" + leaders;
            DBAccess.RunSQL(sql);

            gwf.TodoEmps = gwf.TodoEmps + empsLeader;
            gwf.HuiQianTaskSta = HuiQianTaskSta.HuiQianing;
            Node nd = new Node(gwf.FK_Node);
            if (nd.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne && nd.TodolistModel == TodolistModel.TeamupGroupLeader)
            {

                gwf.HuiQianZhuChiRen = WebUser.No;
                gwf.HuiQianZhuChiRenName = WebUser.Name;
            }
            else
            {
                //多人的组长模式或者协作模式
                if (DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true)
                    gwf.HuiQianZhuChiRen = gwf.TodoEmps;
            }

            gwf.Update();
            return "主持人增加成功";
        }
        /// <summary>
        /// 执行会签
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="toNodeID">到达节点ID,默认为0 可以不传. 如果要发送就传入发送的节点.</param>
        /// <returns>返回执行结果</returns>
        public static string Node_HuiQianDone(Int64 workid, int toNodeID = 0)
        {
            //生成变量.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianOver)
            {
                /*只有一个人的情况下: 并且是会签完毕状态,就执行 */
                return "info@当前工作已经到您的待办理了,会签工作已经完成.";
            }

            if (gwf.HuiQianTaskSta == HuiQianTaskSta.None)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkerlist WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND (IsPass=0 OR IsPass=-1) AND FK_Emp!=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                ps.Add("FK_Node", gwf.FK_Node);
                ps.Add("WorkID", gwf.WorkID);
                ps.Add("FK_Emp", WebUser.No);
                if (DBAccess.RunSQLReturnValInt(ps, 0) == 0)
                    return "close@您没有设置会签人，请在文本框输入会签人，或者选择会签人。";
            }

            //判断当前节点的会签类型.
            Node nd = new Node(gwf.FK_Node);

            //设置当前接单是会签的状态.
            gwf.HuiQianTaskSta = HuiQianTaskSta.HuiQianing; //设置为会签状态.
            if (nd.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne && nd.TodolistModel == TodolistModel.TeamupGroupLeader)
            {

                gwf.HuiQianZhuChiRen = WebUser.No;
                gwf.HuiQianZhuChiRenName = WebUser.Name;
            }
            else
            {
                //多人的组长模式或者协作模式
                if (DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true)
                    gwf.HuiQianZhuChiRen = gwf.TodoEmps;
            }

            //求会签人.
            GenerWorkerLists gwfs = new GenerWorkerLists();
            gwfs.Retrieve(GenerWorkerListAttr.WorkID, gwf.WorkID,
                GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.IsPass, -1);

            string empsOfHuiQian = "会签人:";
            foreach (GenerWorkerList item in gwfs)
            {
                empsOfHuiQian += item.FK_Emp + "," + item.FK_EmpText + ";";

                //发送消息
                BP.WF.Dev2Interface.Port_SendMsg(item.FK_Emp,
                   "bpm会签邀请", "HuiQian" + gwf.WorkID + "_" + gwf.FK_Node + "_" + item.FK_Emp, BP.Web.WebUser.Name + "邀请您对工作｛" + gwf.Title + "｝进行会签,请您在{" + item.SDT + "}前完成.", "HuiQian", gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
            }


            //改变了节点就把会签状态去掉.
            gwf.HuiQianSendToNodeIDStr = "";
            gwf.TodoEmps = gwf.TodoEmps + empsOfHuiQian;
            gwf.Update();

            string sql = "";
            //是否启用会签待办列表, 如果启用了，主持人会签后就转到了HuiQianList.htm里面了.
            if (BP.WF.Glo.IsEnableHuiQianList == true)
            {
                //设置当前操作人员的状态.
                sql = "UPDATE WF_GenerWorkerlist SET IsPass=90 WHERE WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + WebUser.No + "'";
                DBAccess.RunSQL(sql);
            }

            //恢复他的状态.
            sql = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + gwf.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=-1";
            DBAccess.RunSQL(sql);

            //执行会签,写入日志.
            BP.WF.Dev2Interface.WriteTrack(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, gwf.WorkID, gwf.FID, empsOfHuiQian,
                ActionType.HuiQian, "执行会签", null, null);

            string str = "";
            if (nd.TodolistModel == TodolistModel.TeamupGroupLeader)
            {
                /*如果是组长模式.*/
                str = "close@保存成功.\t\n该工作已经移动到会签列表中了,等到所有的人会签完毕后,就可以出现在待办列表里.";
                str += "\t\n如果您要增加或者移除会签人请到会签列表找到该记录,执行操作.";

                //删除自己的意见，以防止其他人员看到.
                BP.WF.Dev2Interface.DeleteCheckInfo(gwf.FK_Flow, gwf.WorkID, gwf.FK_Node);
                return str;
            }

            if (nd.TodolistModel == TodolistModel.Teamup)
            {
                if (toNodeID == 0)
                    return "Send@[" + nd.Name + "]会签成功执行.";

                Node toND = new Node(toNodeID);
                //如果到达的节点是按照接受人来选择,就转向接受人选择器.
                if (toND.HisDeliveryWay == DeliveryWay.BySelected)
                    return "url@Accepter.htm?FK_Node=" + gwf.FK_Node + "&FID=" + gwf.FID + "&WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&ToNode=" + toNodeID;
                else
                    return "Send@执行发送操作";
            }

            return str;
        }
        /// <summary>
        /// 删除会签人员
        /// </summary>
        /// <param name="workID"></param>
        /// <param name="emp">要删除的编号</param>
        /// <returns>返回执行的 </returns>
        public static string Node_HuiQian_Delete(Int64 workID, string emp)
        {
            if (emp.Equals(WebUser.No) == false)
                throw new Exception("err@您不能移除您自己");

            //要找到主持人.
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            string addLeader = gwf.GetParaString("AddLeader");
            if (gwf.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false && addLeader.Contains(BP.Web.WebUser.No + ",") == false)
                throw new Exception("err@您不是主持人，您不能删除。");

            //删除该数据.
            GenerWorkerList gwlOfMe = new GenerWorkerList();
            gwlOfMe.Delete(GenerWorkerListAttr.FK_Emp, emp,
                GenerWorkerListAttr.WorkID, gwf.WorkID,
                GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            //如果已经没有会签待办了,就设置当前人员状态为0.  增加这部分.
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkerlist WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND IsPass=0 ";
            ps.Add("FK_Node", gwf.FK_Node);
            ps.Add("WorkID", gwf.WorkID);
            if (DBAccess.RunSQLReturnValInt(ps) == 0)
            {
                gwf.HuiQianTaskSta = HuiQianTaskSta.None; //设置为 None . 不能设置会签完成,不然其他的就没有办法处理了.
                gwf.Update();
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                ps.Add("FK_Node", gwf.FK_Node);
                ps.Add("WorkID", gwf.WorkID);
                ps.Add("FK_Emp", WebUser.No);
                DBAccess.RunSQL(ps);
            }

            //从待办里移除.
            BP.Port.Emp myemp = new BP.Port.Emp(emp);
            string str = gwf.TodoEmps;
            str = str.Replace(myemp.UserID + "," + myemp.Name + ";", "");
            str = str.Replace(myemp.Name + ";", "");
            addLeader = addLeader.Replace(emp + ",", "");
            gwf.SetPara("AddLeader", addLeader);
            gwf.TodoEmps = str;
            gwf.Update();
            //删除该人员的审核信息
            string sql = "DELETE FROM ND" + int.Parse(gwf.FK_Flow) + "Track WHERE WorkID = " + gwf.WorkID +
                         " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + gwf.FK_Node +
                         " AND NDTo = " + gwf.FK_Node + " AND EmpFrom = '" + emp + "'";
            DBAccess.RunSQL(sql);
            return "执行成功.";
        }
        #endregion 

        #region 写入轨迹.
        /// <summary>
        /// 写入BBS
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="frmName">表单名称</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">消息</param>
        /// <param name="fid">流程ID</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="flowName">流程名称</param>
        /// <param name="nodeID">节点ID</param>
        public static void Track_WriteBBS(string frmID, string frmName, Int64 workID, string msg,
            Int64 fid = 0, string flowNo = "", string flowName = "", int nodeID = 0, string nodeName = "")
        {
            BP.CCBill.Track tk = new BP.CCBill.Track();
            tk.WorkID = workID.ToString();
            tk.FrmID = frmID;
            tk.FrmName = frmName;
            tk.ActionType = "BBS";
            tk.ActionTypeText = "评论";

            tk.Rec = WebUser.No;
            tk.RecName = WebUser.Name;
            tk.DeptNo = WebUser.FK_Dept;
            tk.DeptName = WebUser.FK_DeptName;

            tk.setMyPK(tk.FrmID + "_" + tk.WorkID + "_" + tk.Rec + "_100");
            tk.Msg = msg;
            tk.RDT = DataType.CurrentDateTime;

            //流程信息.
            tk.NodeID = nodeID;
            tk.NodeName = nodeName;
            tk.FlowNo = flowNo;
            tk.FlowName = flowName;
            tk.FID = fid;

            tk.Save();

            //修改抄送状态
            BP.WF.Dev2Interface.Node_CC_SetCheckOver(workID);
        }
        #endregion 写入轨迹.

        #region 流程属性与节点属性变更接口.
        /// <summary>
        /// 更改流程属性
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="attr1">字段1</param>
        /// <param name="v1">值1</param>
        /// <param name="attr2">字段2(可为null)</param>
        /// <param name="v2">值2(可为null)</param>
        /// <returns>执行结果</returns>
        public static string ChangeAttr_Flow(string fk_flow, string attr1, object v1, string attr2, object v2)
        {
            Flow fl = new Flow(fk_flow);
            if (attr1 != null)
            {
                fl.SetValByKey(attr1, v1);
            }

            if (attr2 != null)
            {
                fl.SetValByKey(attr2, v2);
            }

            fl.Update();
            return "修改成功";
        }
        #endregion 流程属性与节点属性变更接口.

        #region ccform 接口

        /// <summary>
        ///  获得指定轨迹的json数据. 
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="mypk">流程主键</param>
        /// <returns>返回当时的表单json字符串</returns>
        public static string CCFrom_GetFrmDBJson(string flowNo, string mypk)
        {
            return DBAccess.GetBigTextFromDB("ND" + int.Parse(flowNo) + "Track", "MyPK", mypk, "FrmDB");
        }
        /// <summary>
        /// SDK签章接口
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="nodeid">签章节点ID</param>
        /// <param name="deptno">部门编号</param>
        /// <param name="stationno">角色编号</param>
        /// <returns>返回非null值时，为签章失败</returns>
        public static string CCForm_Seal(Int64 workid, int nodeid, string deptno, string stationno)
        {
            try
            {
                FrmEleDBs eleDBs = new FrmEleDBs("ND" + nodeid, workid.ToString());

                if (eleDBs.Count > 0)
                {
                    eleDBs.Delete(FrmEleDBAttr.FK_MapData, "ND" + nodeid, FrmEleDBAttr.RefPKVal, workid);
                }

                string sealimg = BP.WF.Glo.CCFlowAppPath + "DataUser/Seal/" + deptno + "_" + stationno + ".jpg";

                if (File.Exists(BP.Difference.SystemConfig.PathOfWebApp + sealimg) == false)
                {
                    return @"签章文件：" + sealimg + "不存在，请联系管理员！";
                }

                FrmEleDB athDB_N = new FrmEleDB();
                athDB_N.setFK_MapData("ND" + nodeid);
                athDB_N.RefPKVal = workid.ToString();
                athDB_N.EleID = workid.ToString();
                athDB_N.GenerPKVal();
                athDB_N.Tag1 = sealimg;
                athDB_N.DirectInsert();

                return null;
            }
            catch (Exception ex)
            {
                return "签章错误：" + ex.Message;
            }
        }
        #endregion ccform 接口

        #region 页面.
        /// <summary>
        /// 附件上传接口
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="flowNo">流程ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="athNo">附件属性No</param>
        /// <param name="frmID">FK_MapData</param>
        /// <param name="filePath">附件路径</param>
        /// <param name="fileName">附件名称</param>
        /// <param name="sort">分类</param>
        /// <returns></returns>
        public static string CCForm_AddAth(int nodeid, string flowNo, Int64 workid, String athNo, string frmID, string filePath, string fileName, string sort = null, Int32 fid = 0, Int32 pworkid = 0)
        {
            string pkVal = workid.ToString();
            // 多附件描述.
            BP.Sys.FrmAttachment athDesc = new BP.Sys.FrmAttachment(athNo);
            MapData mapData = new MapData(frmID);
            string msg = null;

            #region 获取表单方案
            //求主键. 如果该表单挂接到流程上.
            if (nodeid != 0)
            {
                //判断表单方案。
                FrmNode fn = new FrmNode(nodeid, frmID);
                if (fn.FrmSln == FrmSln.Readonly)
                {
                    return "err@不允许上传附件.";
                }

                //是默认的方案的时候.
                if (fn.FrmSln == FrmSln.Default)
                {
                    //判断当前方案设置的whoIsPk ，让附件集成 whoIsPK 的设置。
                    if (fn.WhoIsPK == WhoIsPK.FID)
                    {
                        pkVal = fid.ToString();
                    }

                    if (fn.WhoIsPK == WhoIsPK.PWorkID)
                    {
                        pkVal = pworkid.ToString();
                    }
                }

                //自定义方案.
                if (fn.FrmSln == FrmSln.Self)
                {
                    athDesc = new FrmAttachment(athNo + "_" + nodeid);
                    if (athDesc.HisCtrlWay == AthCtrlWay.FID)
                    {
                        pkVal = fid.ToString();
                    }

                    if (athDesc.HisCtrlWay == AthCtrlWay.PWorkID)
                    {
                        pkVal = pworkid.ToString();
                    }
                }
            }

            #endregion 获取表单方案

            //获取上传文件是否需要加密
            bool fileEncrypt = BP.Difference.SystemConfig.IsEnableAthEncrypt;

            #region 文件上传的iis服务器上 or db数据库里.
            if (athDesc.AthSaveWay == AthSaveWay.IISServer)
            {
                string savePath = athDesc.SaveTo;

                if (savePath.Contains("@") == true || savePath.Contains("*") == true)
                {
                    /*如果有变量*/
                    savePath = savePath.Replace("*", "@");

                    if (savePath.Contains("@") && nodeid != 0)
                    {
                        /*如果包含 @ */
                        BP.WF.Flow flow = new BP.WF.Flow(flowNo);
                        BP.WF.GERpt myen = flow.HisGERpt;
                        myen.OID = workid;
                        myen.RetrieveFromDBSources();
                        savePath = BP.WF.Glo.DealExp(savePath, myen, null);
                    }
                    if (savePath.Contains("@") == true)
                    {
                        throw new Exception("@路径配置错误,变量没有被正确的替换下来." + savePath);
                    }
                }
                else
                {
                    savePath = athDesc.SaveTo + "/" + pkVal;
                }

                //替换关键的字串.
                savePath = savePath.Replace("\\\\", "/");

                try
                {
                    if (System.IO.Directory.Exists(savePath) == false)
                        System.IO.Directory.CreateDirectory(savePath);
                }
                catch (Exception ex)
                {
                    throw new Exception("err@创建路径出现错误，可能是没有权限或者路径配置有问题:" + savePath + "@异常信息:" + ex.Message);
                }

                string guid = DBAccess.GenerGUID();
                string ext = fileName.Substring(fileName.LastIndexOf("."));
                string realSaveTo = savePath + "/" + guid + "." + fileName;
                realSaveTo = realSaveTo.Replace("~", "-");
                realSaveTo = realSaveTo.Replace("'", "-");
                realSaveTo = realSaveTo.Replace("*", "-");
                if (fileEncrypt == true)
                {
                    string strtmp = realSaveTo + ".tmp";
                    if (File.Exists(filePath) == true)
                    {
                        File.Copy(filePath, strtmp);//先明文保存到本地(加个后缀名.tmp)
                    }
                    else
                    {
                        return "err@需要保存的文件不存在";
                    }

                    EncHelper.EncryptDES(strtmp, strtmp.Replace(".tmp", ""));//加密
                    File.Delete(strtmp);//删除临时文件
                }
                else
                {
                    //文件保存的路径
                    if (File.Exists(filePath) == true)
                    {
                        File.Copy(filePath, realSaveTo);
                    }
                    else
                    {
                        return "err@需要保存的文件不存在";
                    }
                }

                FileInfo info = new FileInfo(realSaveTo);
                FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                dbUpload.setMyPK(guid); // athDesc.FK_MapData + oid.ToString();
                dbUpload.NodeID = nodeid;
                dbUpload.Sort = sort;
                dbUpload.setFK_MapData(athDesc.FK_MapData);
                dbUpload.FK_FrmAttachment = athNo;

                dbUpload.FileExts = ext;
                dbUpload.FID = fid;
                if (fileEncrypt == true)
                {
                    dbUpload.SetPara("IsEncrypt", 1);
                }

                #region 处理文件路径，如果是保存到数据库，就存储pk.
                if (athDesc.AthSaveWay == AthSaveWay.IISServer)
                {
                    //文件方式保存
                    dbUpload.FileFullName = realSaveTo;
                }

                if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                {
                    //保存到数据库
                    dbUpload.FileFullName = dbUpload.MyPK;
                }
                #endregion 处理文件路径，如果是保存到数据库，就存储pk.

                dbUpload.FileName = fileName;
                dbUpload.FileSize = (float)info.Length;
                dbUpload.RDT = DataType.CurrentDateTimess;
                dbUpload.Rec = BP.Web.WebUser.No;
                dbUpload.RecName = BP.Web.WebUser.Name;
                dbUpload.FK_Dept = WebUser.FK_Dept;
                dbUpload.FK_DeptName = WebUser.FK_DeptName;
                dbUpload.RefPKVal = pkVal;

                dbUpload.UploadGUID = guid;
                dbUpload.Insert();

                if (athDesc.AthSaveWay == AthSaveWay.DB)
                {
                    //执行文件保存.
                    DBAccess.SaveFileToDB(realSaveTo, dbUpload.EnMap.PhysicsTable, "MyPK", dbUpload.MyPK, "FDB");
                }
            }
            #endregion 文件上传的iis服务器上 or db数据库里.

            #region 保存到数据库 / FTP服务器上.
            if (athDesc.AthSaveWay == AthSaveWay.DB || athDesc.AthSaveWay == AthSaveWay.FTPServer)
            {
                string guid = DBAccess.GenerGUID();

                //把文件临时保存到一个位置.
                string temp = BP.Difference.SystemConfig.PathOfTemp + "" + guid + ".tmp";

                if (fileEncrypt == true)
                {

                    string strtmp = BP.Difference.SystemConfig.PathOfTemp + "" + guid + "_Desc" + ".tmp";
                    if (File.Exists(filePath) == true)
                    {
                        File.Copy(filePath, strtmp);//先明文保存到本地(加个后缀名.tmp)
                    }
                    else
                    {
                        return "err@需要保存的文件不存在";
                    }

                    EncHelper.EncryptDES(strtmp, temp);//加密
                    File.Delete(strtmp);//删除临时文件
                }
                else
                {
                    //文件保存的路径
                    if (File.Exists(filePath) == true)
                    {
                        File.Copy(filePath, temp);
                    }
                    else
                    {
                        return "err@需要保存的文件不存在";
                    }
                }
                string ext = fileName.Substring(fileName.LastIndexOf("."));

                FileInfo info = new FileInfo(temp);
                FrmAttachmentDB dbUpload = new FrmAttachmentDB();
                dbUpload.setMyPK(DBAccess.GenerGUID());
                dbUpload.Sort = sort;
                dbUpload.NodeID = nodeid;
                dbUpload.setFK_MapData(athDesc.FK_MapData);

                dbUpload.FK_FrmAttachment = athDesc.MyPK;
                dbUpload.FileExts = ext;
                dbUpload.FID = fid; //流程id.
                if (fileEncrypt == true)
                {
                    dbUpload.SetPara("IsEncrypt", 1);
                }

                if (athDesc.AthUploadWay == AthUploadWay.Inherit)
                {
                    /*如果是继承，就让他保持本地的PK. */
                    dbUpload.RefPKVal = pkVal.ToString();
                }

                if (athDesc.AthUploadWay == AthUploadWay.Interwork)
                {
                    /*如果是协同，就让他是PWorkID. */
                    Paras ps = new Paras();
                    ps.SQL = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
                    ps.Add("WorkID", pkVal);
                    string pWorkID = DBAccess.RunSQLReturnValInt(ps, 0).ToString();
                    if (pWorkID == null || pWorkID == "0")
                    {
                        pWorkID = pkVal;
                    }

                    dbUpload.RefPKVal = pWorkID;
                }

                dbUpload.setFK_MapData(athDesc.FK_MapData);
                dbUpload.FK_FrmAttachment = athDesc.MyPK;
                dbUpload.FileName = fileName;
                dbUpload.FileSize = (float)info.Length;
                dbUpload.RDT = DataType.CurrentDateTimess;
                dbUpload.Rec = BP.Web.WebUser.No;
                dbUpload.RecName = BP.Web.WebUser.Name;
                dbUpload.FK_Dept = WebUser.FK_Dept;
                dbUpload.FK_DeptName = WebUser.FK_DeptName;
                dbUpload.UploadGUID = guid;

                if (athDesc.AthSaveWay == AthSaveWay.DB)
                {
                    dbUpload.Insert();
                    //把文件保存到指定的字段里.
                    dbUpload.SaveFileToDB("FileDB", temp);
                }

                if (athDesc.AthSaveWay == AthSaveWay.FTPServer)
                {
                    /*保存到fpt服务器上.*/
                    FtpConnection ftpconn = new FtpConnection(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort,
                        SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);

                    string ny = DateTime.Now.ToString("yyyy_MM");

                    //判断目录年月是否存在.
                    if (ftpconn.DirectoryExist(ny) == false)
                    {
                        ftpconn.CreateDirectory(ny);
                    }

                    ftpconn.SetCurrentDirectory(ny);

                    //判断目录是否存在.
                    if (ftpconn.DirectoryExist(athDesc.FK_MapData) == false)
                    {
                        ftpconn.CreateDirectory(athDesc.FK_MapData);
                    }

                    //设置当前目录，为操作的目录。
                    ftpconn.SetCurrentDirectory(athDesc.FK_MapData);

                    //把文件放上去.
                    ftpconn.PutFile(temp, guid + "." + dbUpload.FileExts);
                    ftpconn.Close();

                    //设置路径.
                    dbUpload.FileFullName = ny + "//" + athDesc.FK_MapData + "//" + guid + "." + dbUpload.FileExts;
                    dbUpload.Insert();
                    File.Delete(temp);
                }

            }
            #endregion 保存到数据库.

            return "";
        }

        #endregion 页面.

        #region 与工作处理器相关的接口
        /// <summary>
        /// 获得一个节点要转向的节点
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="ndFrom">节点从</param>
        /// <param name="workid">工作ID</param>
        /// <returns>返回可以到达的节点</returns>
        public static Nodes WorkOpt_GetToNodes(string flowNo, int ndFrom, Int64 workid, Int64 FID)
        {
            Nodes nds = new Nodes();

            Node nd = new Node(ndFrom);
            Nodes toNDs = nd.HisToNodes;

            Flow fl = nd.HisFlow;

            //获得表单的数据，以方便计算方向条件.
            GERpt rpt = fl.HisGERpt;
            if (FID == 0)
                rpt.OID = workid;
            else
                rpt.OID = FID;
            rpt.Retrieve();

            //方向.
            Directions dirs = new Directions(nd.NodeID);

            //同表单的子线程.
            Nodes sameSheetNodes = new Nodes();

            //首先输出普通的节点。            
            foreach (Direction dir in dirs)
            {
                Node mynd = new Node(dir.ToNode);
                if (mynd.IsSubThread == true)
                {
                    sameSheetNodes.AddEntity(mynd);
                    continue; //如果是子线程节点.
                }

                //是否可以处理？
                bool bIsCanDo = true;

                #region 判断方向条件,如果设置了方向条件，判断是否可以通过，不能通过的，就不让其显示.
                Conds conds = new Conds();
                int i = conds.Retrieve(CondAttr.FK_Node, nd.NodeID, CondAttr.ToNodeID,
                    mynd.NodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);
                // 设置方向条件，就判断它。
                if (i > 0)
                    bIsCanDo = conds.GenerResult(rpt);

                //条件不符合则不通过
                if (bIsCanDo == false)
                    continue;
                #endregion

                nds.AddEntity(mynd);
            }

            //同表单子线程.
            foreach (Node mynd in sameSheetNodes)
            {
                if (mynd.IsSubThread == false)
                    continue; //如果是子线程节点.

                if (mynd.HisRunModel == RunModel.SubThreadUnSameWorkID)
                    continue; //如果是异表单的分合流.

                #region 判断方向条件,如果设置了方向条件，判断是否可以通过，不能通过的，就不让其显示.
                Conds conds = new Conds();
                int i = conds.Retrieve(CondAttr.FK_Node, nd.NodeID,
                    CondAttr.ToNodeID, mynd.NodeID, CondAttr.CondType, (int)CondType.Dir,
                    CondAttr.Idx);

                //数量.
                if (conds.Count == 0)
                {
                    nds.AddEntity(mynd);
                    continue;
                }

                //是否可以处理.
                bool bIsCanDo = false;
                foreach (Direction dir in dirs)
                {
                    if (dir.ToNode == mynd.NodeID)
                        bIsCanDo = conds.GenerResult(rpt);
                }
                #endregion

                //如果通过了.
                if (bIsCanDo == true)
                    nds.AddEntity(mynd);
            }

            // 检查是否具有异表单的子线程.
            bool isHave = false;
            foreach (Node mynd in toNDs)
            {
                if (mynd.HisRunModel == RunModel.SubThreadUnSameWorkID)
                {
                    isHave = true;
                }
            }

            if (isHave)
            {
                Node myn1d = new Node();
                myn1d.NodeID = 0;
                myn1d.Name = "可以分发启动的节点";
                nds.AddEntity(myn1d);

                /*增加异表单的子线程*/
                foreach (Node mynd in toNDs)
                {
                    if (mynd.HisRunModel != RunModel.SubThreadUnSameWorkID)
                        continue;

                    #region 判断方向条件,如果设置了方向条件，判断是否可以通过，不能通过的，就不让其显示.
                    Conds conds = new Conds();
                    int i = conds.Retrieve(CondAttr.FK_Node, nd.NodeID,
                        CondAttr.ToNodeID, mynd.NodeID, CondAttr.CondType, (int)CondType.Dir, CondAttr.Idx);
                    // 设置方向条件，就判断它。
                    if (i > 0)
                    {

                        //判断是否可以通过.
                        if (conds.GenerResult(rpt) == false)
                            continue;
                    }
                    #endregion

                    nds.AddEntity(mynd);
                }
            }

            //返回它.
            return nds;
        }
        /// <summary>
        /// 在节点选择转向功能界面，获得当前人员上一次选择的节点，在界面里让其自动选择。
        /// 以改善用户操作体验，就类似于默认记忆上一次的操作功能。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">当前节点编号</param>
        /// <returns>返回上一次当前用户选择的节点,如果没有找到（当前用户第一次发送的情况下找不到）就返回0.</returns>
        public static int WorkOpt_ToNodes_GetLasterSelectNodeID(string flowNo, int nodeID)
        {
            string sql = "";
            switch (BP.Difference.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.Access:
                    sql = "SELECT TOP 1 NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubThreadForward + ")  ORDER BY RDT DESC";
                    break;
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    sql = "SELECT NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE  RowNum=1 AND EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubThreadForward + ")  ORDER BY RDT DESC";
                    break;
                case DBType.MySQL:
                    sql = "SELECT NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubThreadForward + ") limit 0,1";
                    break;
                case DBType.Informix:
                    sql = "SELECT first 1 NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubThreadForward + ")  ORDER BY RDT DESC";
                    break;
                case DBType.PostgreSQL:
                case DBType.UX:
                    sql = "SELECT NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubThreadForward + ") ORDER BY RDT DESC limit 1";
                    break;
                default:
                    throw new Exception("@没有实现该类型的数据库支持.");
            }
            return DBAccess.RunSQLReturnValInt(sql, 0);
        }
        /// <summary>
        /// 发送到节点
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="nodeID"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="toNodes"></param>
        public static SendReturnObjs WorkOpt_SendToNodes(string flowNo, int nodeID, Int64 workid, Int64 fid, string toNodes)
        {
            //把参数更新到数据库里面.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            gwf.RetrieveFromDBSources();
            gwf.Paras_ToNodes = toNodes;
            gwf.Save();

            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            // 以下代码是从 MyFlow.htm Send 方法copy 过来的，需要保持业务逻辑的一致性，所以代码需要保持一致.
            WorkNode firstwn = new WorkNode(wk, nd);
            string msg = "";
            SendReturnObjs objs = firstwn.NodeSend();
            return objs;
        }
        /// <summary>
        /// 获得接收人的数据源
        /// </summary>
        /// <param name="nodeID">指定节点</param>
        /// <param name="WorkID">工作ID</param>
        /// <returns></returns>
        public static DataSet WorkOpt_AccepterDB(int nodeID, Int64 WorkID, Int64 FID = 0)
        {
            DataSet ds = new DataSet();

            Selector en = new Selector(nodeID);
            switch (en.SelectorModel)
            {
                case SelectorModel.Station:
                    DataTable dt = WorkOpt_Accepter_ByStation(nodeID);
                    dt.TableName = "Port_Emp";
                    ds.Tables.Add(dt);
                    break;
                case SelectorModel.SQL:
                    ds = WorkOpt_Accepter_BySQL(nodeID);
                    break;
                case SelectorModel.Dept:
                    ds = WorkOpt_Accepter_ByDept(nodeID);
                    break;
                case SelectorModel.Emp:
                    ds = WorkOpt_Accepter_ByEmp(nodeID);
                    break;
                case SelectorModel.Url:
                default:
                    break;
            }
            return ds;
        }
        /// <summary>
        /// 获取节点绑定角色人员
        /// </summary>
        /// <param name="nodeID">指定的节点</param>
        /// <returns></returns>
        private static DataTable WorkOpt_Accepter_ByStation(int nodeID)
        {
            if (nodeID == 0)
                throw new Exception("@流程设计错误，没有转向的节点。举例说明: 当前是A节点。如果您在A点的属性里启用了[接受人]按钮，那么他的转向节点集合中(就是A可以转到的节点集合比如:A到B，A到C, 那么B,C节点就是转向节点集合)，必须有一个节点是的节点属性的[访问规则]设置为[由上一步发送人员选择]");

            NodeStations stas = new NodeStations(nodeID);
            if (stas.Count == 0)
            {
                BP.WF.Node toNd = new BP.WF.Node(nodeID);
                throw new Exception("@流程设计错误：设计员没有设计节点[" + toNd.Name + "]，接受人的角色范围。");
            }

            string empNo = "No";
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                empNo = "UserID as No";

            // 优先解决本部门的问题。
            string sql = "";

            sql = "SELECT A." + empNo + ",A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
            sql += "SELECT FK_EMP FROM Port_DeptEmpStation WHERE FK_STATION ";
            sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + nodeID + ") ";
            sql += ") AND a.No IN (SELECT " + empNo + " FROM Port_Emp WHERE FK_Dept ='" + WebUser.FK_Dept + "')";
            sql += " ORDER BY B.Idx,B.No,A.Idx,A.No ";


            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
            {
                return dt;
            }

            //组织结构中所有角色人员
            sql = "SELECT A." + empNo + ",A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
            sql += "SELECT FK_Emp FROM  Port_DeptEmpStation WHERE FK_STATION ";
            sql += "IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + nodeID + ") ";
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                sql += " AND Port_DeptEmpStation.OrgNo='" + BP.Web.WebUser.OrgNo + "'";

            sql += ") ORDER BY A.FK_Dept,A.No ";
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 按sql方式
        /// </summary>
        private static DataSet WorkOpt_Accepter_BySQL(int nodeID)
        {
            DataSet ds = new DataSet();
            Selector MySelector = new Selector(nodeID);
            string sqlGroup = MySelector.SelectorP1;
            sqlGroup = sqlGroup.Replace("@WebUser.No", WebUser.No);
            sqlGroup = sqlGroup.Replace("@WebUser.Name", WebUser.Name);
            sqlGroup = sqlGroup.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            string sqlDB = MySelector.SelectorP2;
            sqlDB = sqlDB.Replace("@WebUser.No", WebUser.No);
            sqlDB = sqlDB.Replace("@WebUser.Name", WebUser.Name);
            sqlDB = sqlDB.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

            DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);
            dtGroup.TableName = "Port_Dept";
            ds.Tables.Add(dtGroup);
            DataTable dtDB = DBAccess.RunSQLReturnTable(sqlDB);
            dtDB.TableName = "Port_Emp";
            ds.Tables.Add(dtDB);

            return ds;
        }

        /// <summary>
        /// 获取接收人选择器，按部门绑定
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private static DataSet WorkOpt_Accepter_ByDept(int nodeID)
        {
            string empNo = "No";
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                empNo = "UserID as No";

            DataSet ds = new DataSet();
            string orderByIdx = "Idx,";
            string sqlGroup = "SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node='" + nodeID + "') ORDER BY " + orderByIdx + "No";
            string sqlDB = "SELECT " + empNo + ",Name, FK_Dept FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node='" + nodeID + "') ORDER BY " + orderByIdx + "No";

            DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);
            dtGroup.TableName = "Port_Dept";
            ds.Tables.Add(dtGroup);

            DataTable dtDB = DBAccess.RunSQLReturnTable(sqlDB);
            dtDB.TableName = "Port_Emp";
            ds.Tables.Add(dtDB);
            return ds;
        }
        /// <summary>
        /// 按BindByEmp 方式
        /// </summary>
        private static DataSet WorkOpt_Accepter_ByEmp(int nodeID)
        {
            string orderByIdx = "Idx,";
            string sqlGroup = "SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM Port_Emp WHERE No in(SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node='" + nodeID + "')) ORDER BY " + orderByIdx + "No";


            string sqlDB = "SELECT " + BP.Sys.Base.Glo.UserNo + ",Name,FK_Dept FROM Port_Emp WHERE No in (SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node='" + nodeID + "') ORDER BY " + orderByIdx + "No";

            DataSet ds = new DataSet();
            DataTable dtGroup = DBAccess.RunSQLReturnTable(sqlGroup);
            dtGroup.TableName = "Port_Dept";
            ds.Tables.Add(dtGroup);

            DataTable dtDB = DBAccess.RunSQLReturnTable(sqlDB);
            dtDB.TableName = "Port_Emp";
            ds.Tables.Add(dtDB);
            return ds;
        }

        /// <summary>
        /// 设置指定的节点接受人
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="emps">指定的人员集合zhangsan,lisi,wangwu</param>
        /// <param name="isNextTime">是否下次自动设置</param>
        public static void WorkOpt_SetAccepter(int nodeID, Int64 workid, Int64 fid, string emps, bool isNextTime)
        {
            SelectAccpers ens = new SelectAccpers();
            ens.Delete(SelectAccperAttr.FK_Node, nodeID,
                SelectAccperAttr.WorkID, workid);

            //下次是否记忆选择，清空掉。
            string sql = "UPDATE WF_SelectAccper SET " + SelectAccperAttr.IsRemember + " = 0 WHERE Rec='" + WebUser.No + "' AND IsRemember=1 AND FK_Node=" + nodeID;
            DBAccess.RunSQL(sql);

            //开始执行保存.
            string[] strs = emps.Split(',');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str) == true)
                    continue;

                SelectAccper en = new SelectAccper();
                en.setMyPK(nodeID + "_" + workid + "_" + str);
                en.FK_Emp = str;
                en.FK_Node = nodeID;

                en.WorkID = workid;
                en.Rec = WebUser.No;
                en.IsRemember = isNextTime;
                en.Insert();
            }
        }
        /// <summary>
        /// 发送到节点
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="nodeID"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="toNodeID"></param>
        /// <param name="toEmps"></param>
        /// <param name="isRememberMe"></param>
        public static SendReturnObjs WorkOpt_SendToEmps(string flowNo, int nodeID, Int64 workid, Int64 fid,
            int toNodeID, string toEmps, bool isRememberMe)
        {
            WorkOpt_SetAccepter(toNodeID, workid, fid, toEmps, isRememberMe);

            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            // 以下代码是从 MyFlow.htm Send 方法copy 过来的，需要保持业务逻辑的一致性，所以代码需要保持一致.
            WorkNode firstwn = new WorkNode(wk, nd);
            string msg = "";
            SendReturnObjs objs = firstwn.NodeSend();
            return objs;
        }
        #endregion

        #region 表单：
        /// <summary>
        /// 附件上传.
        /// </summary>
        /// <param name="FileByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string UploadFile(byte[] FileByte, String fileName)
        {
            string path = HttpContextHelper.RequestApplicationPath + "/DataUser/UploadFile";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string filePath = path + "/" + fileName;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            //这里使用绝对路径来索引
            FileStream stream = new FileStream(filePath, FileMode.CreateNew);
            stream.Write(FileByte, 0, FileByte.Length);
            stream.Close();

            return filePath;
        }
        /// <summary>
        /// 把表单生成pdf文件.
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="filePath">要生成的文件路径，全名.</param>
        /// <returns></returns>
        public static string Frm_BuliderPDFMergeForFromTree(Int64 workID, int nodeId, string filePath, string name)
        {
            try
            {
                if (nodeId == 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(workID);
                    nodeId = gwf.FK_Node;
                }
                Node nd = new Node(nodeId);
                BP.WF.MakeForm2Html.MakeCCFormToPDF(nd, workID, nd.FK_Flow, name, filePath);

                return "执行成功.";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion

        #region 调度相关的操作.
        /// <summary>
        /// 更新时间状态, 交付给 huangzhimin.
        /// 作用：按照当前的时间，每天两次更新WF_GenerWorkFlow 的 TodoSta 状态字段。
        /// 该字段： 0=正常(绿牌), 1=预警(黄牌), 2=逾期(红牌), 3=按时完成(绿牌) , 4=逾期完成(红牌).
        /// 该方法作用是，每天，中午时间段，与下午时间段，执行更新这两个状态，仅仅更新两次。
        /// </summary>
        public static void DTS_GenerWorkFlowTodoSta()
        {
            // 中午的更新, 与发送邮件通知.
            bool isPM = false;

            #region 求出是否可以更新状态.
            if (DateTime.Now.Hour >= 9 && DateTime.Now.Hour < 12)
            {
                isPM = true;
                string timeKey = "DTSTodoStaPM" + DateTime.Now.ToString("yyMMdd");
                Paras ps = new Paras();
                ps.SQL = "SELECT Val FROM Sys_GloVar WHERE No='" + timeKey + "'";
                string time = DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (time == null)
                {
                    GloVar var = new GloVar();
                    var.No = timeKey;
                    var.Name = "时效调度 WFTodoSta PM 调度";
                    var.GroupKey = "WF";
                    var.Val = timeKey;  //更新时间点.
                    var.Note = "时效调度PM" + timeKey;
                    var.Insert();
                    time = var.Val;
                }
                else
                {
                    /*如果有数据，就返回，说明已经执行过了。*/
                    return;
                }
            }

            //下午时间段.
            if (DateTime.Now.Hour >= 13 && DateTime.Now.Hour < 18)
            {
                string timeKey = "DTSTodoStaAM" + DateTime.Now.ToString("yyMMdd");
                Paras ps = new Paras();
                ps.SQL = "SELECT Val FROM Sys_GloVar WHERE No='" + timeKey + "'";
                string time = DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (time == null)
                {
                    GloVar var = new GloVar();
                    var.No = timeKey;
                    var.Name = "时效调度 WFTodoSta AM 调度";
                    var.GroupKey = "WF";
                    var.Val = timeKey;  //更新时间点.
                    var.Note = "时效调度AM" + timeKey;
                    var.Insert();
                    time = var.Val;
                }
                else
                {
                    /*如果有数据，就返回，说明已经执行过了。*/
                    return;
                }
            }
            #endregion 求出是否可以更新状态.


            BP.WF.DTS.DTS_GenerWorkFlowTodoSta en = new BP.WF.DTS.DTS_GenerWorkFlowTodoSta();
            en.Do();

        }
        /// <summary>
        /// 预警与逾期的提醒.
        /// </summary>
        private static void DTS_SendMsgToWorker()
        {
            #region 处理预警的消息发送.
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 12)
            {
                string timeKey = "DTSWarningPM" + DateTime.Now.ToString("yyMMdd");
                Paras ps = new Paras();
                ps.SQL = "SELECT Val FROM Sys_GloVar WHERE No='" + timeKey + "'";
                string time = DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (time != null)
                {
                    return;
                }

                BP.WF.DTS.DTS_SendMsgToWarningWorker en = new BP.WF.DTS.DTS_SendMsgToWarningWorker();
                en.Do();

            }
            #endregion
        }
        /// <summary>
        /// 生成工作的 TimeSpan
        /// </summary>
        public static void DTS_GenerWorkFlowTimeSpan()
        {
            if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour < 10 && DateTime.Today.DayOfWeek == DayOfWeek.Monday)
            {
                string timeKey = "DTSTimeSpanPM" + DateTime.Now.ToString("yyMMdd");
                Paras ps = new Paras();
                ps.SQL = "SELECT Val FROM Sys_GloVar WHERE No='" + timeKey + "'";
                string time = DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (time == null)
                {
                    GloVar var = new GloVar();
                    var.No = timeKey;
                    var.Name = "设置时间段" + timeKey + "一周执行一次.";
                    var.GroupKey = "WF";
                    var.Val = timeKey;  //更新时间点.
                    var.Note = "设置时间段PM" + timeKey;
                    var.Insert();
                }
                else
                {
                    return;
                }
            }

            //执行调度.
            BP.WF.DTS.DTS_GenerWorkFlowTimeSpan ts = new BP.WF.DTS.DTS_GenerWorkFlowTimeSpan();
            ts.Do();
        }

        /// <summary>
        /// 根据WorkID获取根节点的WorkID
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        public static Int64 GetRootWorkIDBySQL(Int64 workId, Int64 pworkid)
        {
            if (pworkid == 0)
                return workId;
            GenerWorkFlow gwf = new GenerWorkFlow(pworkid);
            if (gwf.PWorkID == 0)
                return pworkid;
            gwf = new GenerWorkFlow(gwf.PWorkID);
            if (gwf.PWorkID == 0)
                return gwf.WorkID;
            return gwf.PWorkID;


        }

        #endregion

        public static string GetParentChildWorkID(Int64 workid, string workids)
        {
            //加上当前workid
            workids += workid + ",";
            string sql = "";
            //递归获取该流程的父级WorkID;
            GenerWorkFlow gwf = new GenerWorkFlow(workid);

            //获取子级
            sql = "SELECT WORKID FROM WF_GenerWorkFlow Where PWorkID = " + workid;
            string vals = DBAccess.RunSQLReturnStringIsNull(sql, "");
            if (DataType.IsNullOrEmpty(vals) == true)
                return workids;
            else
            {
                string[] strs = vals.Split(',');
                foreach (string str in strs)
                    workids = GetParentChildWorkID(Int64.Parse(str), workids);
            }

            if (gwf.PWorkID == 0)
                return workids;

            //获取他的父级
            sql = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID = " + gwf.PWorkID;
            Int64 pworkid = DBAccess.RunSQLReturnValInt(sql);
            if (pworkid == 0)
                return workids;
            GetParentChildWorkID(pworkid, workids);

            return workids;
        }
        /// <summary>
        /// 求出WhoIsPK的
        /// </summary>
        /// <param name="workid"></param>
        /// <param name="pworkid"></param>
        /// <param name="fid"></param>值
        /// <returns>PKVAl</returns>
        public static string GetAthRefPKVal(Int64 workid, Int64 pworkid, Int64 fid, int fk_node, string fk_mapData, FrmAttachment athDesc)
        {
            Int64 pkval = 0;
            if (fk_node == 0 || fk_node == 9999)
            {
                if (workid != 0)
                    return workid.ToString();
                return "0";

            }

            AthCtrlWay athCtrlWay = athDesc.HisCtrlWay;
            Node nd = new Node(fk_node);
            //表单方案
            FrmNode fn = new FrmNode(fk_node, fk_mapData);
            //树形表单
            if (nd.HisFormType == NodeFormType.SheetTree)
                athCtrlWay = AthCtrlWay.WorkID;
            //单表单
            else if (nd.HisFormType == NodeFormType.RefOneFrmTree)
            {
                switch (fn.WhoIsPK)
                {
                    case WhoIsPK.OID:
                        athCtrlWay = AthCtrlWay.WorkID;
                        break;
                    case WhoIsPK.FID:
                        athCtrlWay = AthCtrlWay.FID;
                        break;
                    case WhoIsPK.PWorkID:
                        athCtrlWay = AthCtrlWay.PWorkID;
                        break;
                    case WhoIsPK.P2WorkID:
                        athCtrlWay = AthCtrlWay.P2WorkID;
                        break;
                    case WhoIsPK.P3WorkID:
                        athCtrlWay = AthCtrlWay.P3WorkID;
                        break;
                    case WhoIsPK.RootFlowWorkID:
                        athCtrlWay = AthCtrlWay.RootFlowWorkID;
                        break;
                    default:
                        athCtrlWay = athDesc.HisCtrlWay;
                        break;
                }

            }

            //根据控制权限获取RefPK的值
            if (athCtrlWay == AthCtrlWay.WorkID || athCtrlWay == AthCtrlWay.PK)
                pkval = workid;

            if (athCtrlWay == AthCtrlWay.FID)
                pkval = fid;


            //如果是父流程的数据. @lizhen
            if (athCtrlWay == AthCtrlWay.PWorkID)
            {
                if (pworkid == 0)
                    pworkid = DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + workid, 0);

                pkval = pworkid;
            }


            if (athCtrlWay == AthCtrlWay.P2WorkID)
            {
                //根据流程的PWorkID获取他的爷爷流程
                pkval = DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + pworkid, 0);
            }
            if (athCtrlWay == AthCtrlWay.P3WorkID)
            {
                string sql = "Select PWorkID From WF_GenerWorkFlow Where WorkID=(Select PWorkID From WF_GenerWorkFlow Where WorkID=" + pworkid + ")";
                //根据流程的PWorkID获取他的P2流程
                pkval = DBAccess.RunSQLReturnValInt(sql, 0);
            }
            if (athCtrlWay == AthCtrlWay.RootFlowWorkID)
                pkval = BP.WF.Dev2Interface.GetRootWorkIDBySQL(workid, pworkid);
            return pkval.ToString();
        }


        public static string GetDtlRefPKVal(Int64 workid, Int64 pworkid, Int64 fid, int fk_node, string fk_mapData, MapDtl mapDtl)
        {
            Int64 pkval = 0;
            if (fk_node == 0 || fk_node == 9999)
                return "0";

            DtlOpenType dtlOpenType = mapDtl.DtlOpenType;
            Node nd = new Node(fk_node);
            //表单方案
            FrmNode fn = new FrmNode(fk_node, fk_mapData);
            //树形表单
            if (nd.HisFormType == NodeFormType.SheetTree)
                dtlOpenType = DtlOpenType.ForWorkID;
            //单表单
            else if (nd.HisFormType == NodeFormType.RefOneFrmTree)
            {
                switch (fn.WhoIsPK)
                {
                    case WhoIsPK.OID:
                        dtlOpenType = DtlOpenType.ForWorkID;
                        break;
                    case WhoIsPK.FID:
                        dtlOpenType = DtlOpenType.ForFID;
                        break;
                    case WhoIsPK.PWorkID:
                        dtlOpenType = DtlOpenType.ForWorkID;
                        break;
                    case WhoIsPK.P2WorkID:
                        dtlOpenType = DtlOpenType.ForP2WorkID;
                        break;
                    case WhoIsPK.P3WorkID:
                        dtlOpenType = DtlOpenType.ForP3WorkID;
                        break;
                    case WhoIsPK.RootFlowWorkID:
                        dtlOpenType = DtlOpenType.RootFlowWorkID;
                        break;
                    default:
                        dtlOpenType = mapDtl.DtlOpenType;
                        break;
                }

            }

            //根据控制权限获取RefPK的值
            if (dtlOpenType == DtlOpenType.ForWorkID)
                pkval = workid;
            if (dtlOpenType == DtlOpenType.ForFID)
            {
                if (nd.IsSubThread == true)
                    pkval = fid;
                else
                    pkval = workid;
            }


            if (dtlOpenType == DtlOpenType.ForP2WorkID)
                if (pworkid != 0)
                    pkval = pworkid;


            if (dtlOpenType == DtlOpenType.ForP2WorkID)
            {
                //根据流程的PWorkID获取他的爷爷流程
                pkval = DBAccess.RunSQLReturnValInt("SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=" + pworkid, 0);
            }
            if (dtlOpenType == DtlOpenType.ForP3WorkID)
            {
                string sql = "SELECT PWorkID FROM WF_GenerWorkFlow WHERE WorkID=(Select PWorkID From WF_GenerWorkFlow Where WorkID=" + pworkid + ")";
                //根据流程的PWorkID获取他的P2流程
                pkval = DBAccess.RunSQLReturnValInt(sql, 0);
            }
            if (dtlOpenType == DtlOpenType.RootFlowWorkID)
            {
                if (fid != 0)
                    pkval = BP.WF.Dev2Interface.GetRootWorkIDBySQL(fid, pworkid);
                else
                    pkval = BP.WF.Dev2Interface.GetRootWorkIDBySQL(workid, pworkid);
            }


            return pkval.ToString();
        }

        /// <summary>
        /// 保存开发者表单的数据
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <param name="fk_mapData"></param>
        /// <returns></returns>
        public static string SaveDevelopForm(string htmlCode, string fk_mapData)
        {
            //保存到DataUser/CCForm/HtmlTemplateFile/文件夹下
            string filePath = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HtmlTemplateFile/";
            if (Directory.Exists(filePath) == false)
                Directory.CreateDirectory(filePath);

            filePath = filePath + fk_mapData + ".htm";
            //写入到html 中
            DataType.WriteFile(filePath, htmlCode);

            //保存类型。
            MapData md = new MapData(fk_mapData);
            if (md.HisFrmType != FrmType.Develop)
            {
                md.HisFrmType = FrmType.Develop;
                md.Update();
            }
            // HtmlTemplateFile 保存到数据库中
            DBAccess.SaveBigTextToDB(htmlCode, "Sys_MapData", "No", fk_mapData, "HtmlTemplateFile");

            //检查数据完整性
            GEEntity en = new GEEntity(fk_mapData);
            en.CheckPhysicsTable();
            return "保存成功";
        }
        public static string GetDeptNoSQLByParentNo(string paretNo, string ptable)
        {
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MySQL:
                    return "SELECT No FROM(SELECT * FROM " + ptable + " WHERE parentNo IS NOT NULL) au,"
                           + " (SELECT @pid:= '" + paretNo + "') pd"
                           + " WHERE FIND_IN_SET(parentNo, @pid) > 0"
                           + " AND @pid := concat(@pid, ',', No)"
                           + " union select No from " + ptable + " where No = '" + paretNo + "'; ";
                case DBType.MSSQL:
                    return "WITH allsub(No,Name,ParentNo) as ("
                            + " SELECT No, Name, ParentNo FROM " + ptable + " where No = '" + paretNo + "'"
                            + " UNION ALL SELECT a.No,a.Name,a.ParentNo FROM " + ptable + " a, allsub b where a.ParentNo = b.No"
                            + " )"
                            + " SELECT No FROM allsub";

                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    return "SELECT D.No FROM " + ptable + " D start with D.No='" + paretNo + "' connect by prior D.No = D.ParentNo";
                case DBType.UX:
                    return "";
                case DBType.DM:
                    return "";
                case DBType.PostgreSQL:
                    return "";
                default:
                    throw new Exception(SystemConfig.AppCenterDBType + "的数据库还没有解析根据父节点获取子级的SQL");

            }
        }
        public static string GetParentNameByCurrNo(string no, string ptable, string orgNo)
        {
            string sql = "";

            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MySQL:
                    sql = "SELECT GROUP_CONCAT(t.Name separator'/') FROM ("
                           + " SELECT @No idlist,"
                           + " (SELECT @No:= GROUP_CONCAT(ParentNo separator ',') FROM " + ptable + " WHERE FIND_IN_SET(No, @No)) sub"
                           + " FROM " + ptable + ",(SELECT @No:='" + no + "') vars"
                           + " WHERE @No is not null ) tl," + ptable + " t";
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql += " WHERE FIND_IN_SET(t.No, tl.idlist)";
                    else
                        sql += " WHERE FIND_IN_SET(t.No, tl.idlist) AND t.OrgNo ='" + orgNo + "'";
                    break;
                case DBType.MSSQL:
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql = "WITH allsub(No,Name,ParentNo) as ("
                        + " SELECT No, Name, ParentNo FROM " + ptable + " WHERE No='" + no + "'"
                        + "  UNION ALL SELECT a.No,a.Name,a.ParentNo FROM " + ptable + " a, allsub b WHERE a.No = b.ParentNo)"
                        + " SELECT name +'/' FROM allsub FOR XML PATH(''); ";
                    else
                        sql = "WITH allsub(No,Name,ParentNo,OrgNo) as ("
                        + " SELECT No, Name, ParentNo,OrgNo FROM " + ptable + " WHERE No='" + no + "'"
                        + "  UNION ALL SELECT a.No,a.Name,a.ParentNo,a.OrgNo FROM " + ptable + " a, allsub b WHERE a.No = b.ParentNo)"
                        + " SELECT name +'/' FROM allsub WHERE OrgNo!='" + orgNo + "'  FOR XML PATH(''); ";
                    break;
                case DBType.Oracle:
                case DBType.KingBaseR3:
                case DBType.KingBaseR6:
                    if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql = "SELECT  replace(wm_concat(D.Name),',','/') FROM " + ptable + " D START WITH D.No='" + no + "' connect by prior D.ParentNo = D.No;";
                    else
                        sql = "SELECT  replace(wm_concat(D.Name),',','/') FROM " + ptable + " D START WITH D.No='" + no + "' connect by prior D.ParentNo = D.No AND D.OrgNo='" + orgNo + "';";
                    break;
                case DBType.UX:
                case DBType.DM:
                case DBType.PostgreSQL:
                default:
                    throw new Exception(SystemConfig.AppCenterDBType + "的数据库还没有解析根据父节点获取子级的SQL");
            }

            return DBAccess.RunSQLReturnStringIsNull(sql, "");
        }
    }

}
