using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.En;
using BP.WF.Template;
using BP.WF.Data;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 此接口为程序员二次开发使用,在阅读代码前请注意如下事项.
    /// 1, CCFlow的对外的接口都是以静态方法来实现的.
    /// 2, 以 DB_ 开头的是需要返回结果集合的接口.
    /// 3, 以 Flow_ 是流程接口.
    /// 4, 以 Node_ 是节点接口.
    /// 5, 以 Port_ 是组织架构接口.
    /// 6, 以 DTS_ 是调度. data tranr system.
    /// 7, 以 UI_ 是流程的功能窗口
    /// 8, 以 WorkOpt_ 用工作处理器相关的接口。
    /// </summary>
    public class Dev2Interface
    {
        #region 写入消息表.
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
        public static bool WriteToSMS(string sendToUserNo, string sendDT, string title, string doc, string msgFlag)
        {
            SMS.SendMsg(sendToUserNo, title, doc, msgFlag, "Info", "");
            return true;
        }
        #endregion

        #region 等待要去处理的消息数量.
        /// <summary>
        /// 待办工作数量
        /// </summary>
        public static int Todolist_EmpWorks
        {
            get
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + "  OR WFState=" + (int)WFState.AskForReplay + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + " OR WFState=" + (int)WFState.Fix;
                string sql;

                if (WebUser.IsAuthorize == false)
                {
                    /*不是授权状态*/
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1 ";
                    else
                        ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";

                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    //throw new Exception(ps.SQL);

                    //  BP.DA.Log.DebugWriteInfo(ps.SQL);
                    return BP.DA.DBAccess.RunSQLReturnValInt(ps);
                }

                /*如果是授权状态, 获取当前委托人的信息. */
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                switch (emp.HisAuthorWay)
                {
                    case Port.AuthorWay.All:
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1  ";
                        else
                            ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        break;
                    case Port.AuthorWay.SpecFlows:
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + " AND TaskSta!=0   ";
                        else
                            ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows;

                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        break;
                    case Port.AuthorWay.None:
                        /*不是授权状态 */
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1 ";
                        else
                            ps.SQL = "SELECT count(WorkID) as Num FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";

                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        return BP.DA.DBAccess.RunSQLReturnValInt(ps);
                    default:
                        throw new Exception("no such way...");
                }
                return BP.DA.DBAccess.RunSQLReturnValInt(ps);
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
                ps.SQL = "SELECT count(MyPK) as Num FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND Sta=0";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return DBAccess.RunSQLReturnValInt(ps, 0);
            }
        }
        /// <summary>
        /// 获取抄送人员的
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string GetNode_CCList(WorkNode wn)
        {
            string ccers = null;

            if (wn.HisNode.HisCCRole == CCRole.AutoCC
                   || wn.HisNode.HisCCRole == CCRole.HandAndAuto)
            {
                try
                {
                    /*如果是自动抄送*/
                    CC cc = wn.HisNode.HisCC;

                    DataTable table = cc.GenerCCers(wn.rptGe);
                    if (table.Rows.Count > 0)
                    {
                        string ccMsg = "@消息自动抄送给";

                        foreach (DataRow dr in table.Rows)
                        {
                            ccers += dr[0] + ",";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("@处理操送时出现错误:" + ex.Message);
                }
            }

            #region  执行抄送 BySysCCEmps
            if (wn.HisNode.HisCCRole == CCRole.BySysCCEmps)
            {
                CC cc = wn.HisNode.HisCC;

                //取出抄送人列表
                string temps = wn.rptGe.GetValStrByKey("SysCCEmps");
                if (!string.IsNullOrEmpty(temps))
                {
                    string[] cclist = temps.Split('|');
                    Hashtable ht = new Hashtable();
                    foreach (string item in cclist)
                    {
                        string[] tmp = item.Split(',');
                        ccers += tmp[0] + ",";
                    }
                }
            }
            #endregion

            return ccers;
        }
        /// <summary>
        /// 返回挂起流程数量
        /// </summary>
        public static int Todolist_HungUpNum
        {
            get
            {
                string sql = "select  COUNT(WorkID) AS Num from WF_GenerWorkFlow where WFState=4 and  WorkID in (SELECT distinct WorkID FROM WF_HungUp WHERE Rec='" + BP.Web.WebUser.No + "')";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
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
                if (WebUser.IsAuthorize)
                {
                    /*如果是授权状态.*/
                    WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                    sql = "SELECT count( distinct a.WorkID ) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND (B.IsPass=1 OR B.IsPass<0) AND A.FK_Flow IN " + emp.AuthorFlows;
                    return BP.DA.DBAccess.RunSQLReturnValInt(sql);
                }
                else
                {
                    Paras ps = new Paras();
                    ps.SQL = "SELECT count( distinct a.WorkID ) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND B.IsEnable=1 AND (B.IsPass=1 OR B.IsPass<0) ";
                    ps.Add("FK_Emp", WebUser.No);
                    return BP.DA.DBAccess.RunSQLReturnValInt(ps);
                }
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
                if (Glo.IsDeleteGenerWorkFlow == false)
                {
                    /* 如果不是删除流程注册表. */
                    Paras ps = new Paras();
                    string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                    ps.SQL = "SELECT count(WorkID) Num FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' AND WFState=" + (int)WFState.Complete;
                    return BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
                }
                else
                {
                    Paras ps = new Paras();
                    string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                    ps.SQL = "SELECT count(*) Num FROM V_FlowData WHERE FlowEmps LIKE '%@" + WebUser.No + "%' AND FID=0 AND WFState=" + (int)WFState.Complete;
                    return BP.DA.DBAccess.RunSQLReturnValInt(ps, 0);
                }
            }
        }
        /// <summary>
        /// 共享任务个数
        /// </summary>
        public static int Todolist_Sharing
        {
            get
            {
                //if (BP.WF.Glo.IsEnableTaskPool == false)
                //    return 0
                //    throw new Exception("@你必须在Web.config中启用IsEnableTaskPool才可以执行此操作。");

                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Sharing;
                string sql;
                string realSql = null;
                if (WebUser.IsAuthorize == false)
                {
                    /*不是授权状态*/
                    ps.SQL = "SELECT COUNT(WorkID) FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    return BP.DA.DBAccess.RunSQLReturnValInt(ps);
                }

                /*如果是授权状态, 获取当前委托人的信息. */
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                switch (emp.HisAuthorWay)
                {
                    case Port.AuthorWay.All:
                        ps.SQL = "SELECT COUNT(WorkID) FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta=0";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        break;
                    case Port.AuthorWay.SpecFlows:
                        ps.SQL = "SELECT COUNT(WorkID) FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + " ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        break;
                    case Port.AuthorWay.None:
                        //   WebUser.IsAuthorize = false;
                        /*不是授权状态*/
                        ps.SQL = "SELECT COUNT(WorkID) FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        return BP.DA.DBAccess.RunSQLReturnValInt(ps);
                    //throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                    default:
                        throw new Exception("no such way...");
                }
                return BP.DA.DBAccess.RunSQLReturnValInt(ps);
            }
        }
        #endregion 等待要去处理的消息数量.

        #region 自动执行
        /// <summary>
        /// 处理延期的任务.根据节点属性的设置
        /// </summary>
        /// <returns>返回处理的消息</returns>
        public static string DTS_DealDeferredWork()
        {
            //string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <='" + DataType.CurrentData + "' ORDER BY FK_Emp";
            //改成小于号SDT <'" + DataType.CurrentData
            string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <'" + DataType.CurrentData + "' ORDER BY FK_Emp";
            //string sql = "SELECT * FROM WF_EmpWorks WHERE FK_Node IN (SELECT NodeID FROM WF_Node WHERE OutTimeDeal >0 ) AND SDT <='2013-12-30' ORDER BY FK_Emp";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            string msg = "";
            string dealWorkIDs = "";
            foreach (DataRow dr in dt.Rows)
            {
                string FK_Emp = dr["FK_Emp"].ToString();
                string fk_flow = dr["FK_Flow"].ToString();
                int fk_node = int.Parse(dr["FK_Node"].ToString());
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                Int64 fid = Int64.Parse(dr["FID"].ToString());

                // 方式两个人同时处理一件工作, 一个人处理后，另外一个人还可以处理的情况.
                if (dealWorkIDs.Contains("," + workid + ","))
                    continue;
                dealWorkIDs += "," + workid + ",";

                if (WebUser.No != FK_Emp)
                {
                    Emp emp = new Emp(FK_Emp);
                    BP.Web.WebUser.SignInOfGener(emp);
                }

                BP.WF.Template.NodeSheet nd = new BP.WF.Template.NodeSheet();
                nd.NodeID = fk_node;
                nd.Retrieve();

                // 首先判断是否有启动的表达式, 它是是否自动执行的总阀门。
                if (string.IsNullOrEmpty(nd.DoOutTimeCond) == false)
                {
                    Node nodeN = new Node(nd.NodeID);
                    Work wk = nodeN.HisWork;
                    wk.OID = workid;
                    wk.Retrieve();
                    string exp = nd.DoOutTimeCond.Clone() as string;
                    if (Glo.ExeExp(exp, wk) == false)
                        continue; // 不能通过条件的设置.
                }

                switch (nd.HisOutTimeDeal)
                {
                    case OutTimeDeal.None:
                        break;
                    case OutTimeDeal.AutoTurntoNextStep: //自动转到下一步骤.
                        if (string.IsNullOrEmpty(nd.DoOutTime))
                        {
                            /*如果是空的,没有特定的点允许，就让其它向下执行。*/
                            msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                        }
                        else
                        {
                            int nextNode = Dev2Interface.Node_GetNextStepNode(fk_flow, workid);
                            if (nd.DoOutTime.Contains(nextNode.ToString())) /*如果包含了当前点的ID,就让它执行下去.*/
                                msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfText();
                        }
                        break;
                    case OutTimeDeal.AutoJumpToSpecNode: //自动的跳转下一个节点.
                        if (string.IsNullOrEmpty(nd.DoOutTime))
                            throw new Exception("@设置错误,没有设置要跳转的下一步节点.");
                        int nextNodeID = int.Parse(nd.DoOutTime);
                        msg += BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, null, null, nextNodeID, null).ToMsgOfText();
                        break;
                    case OutTimeDeal.AutoShiftToSpecUser: //移交给指定的人员.
                        msg += BP.WF.Dev2Interface.Node_Shift(fk_flow, fk_node, workid, fid, nd.DoOutTime, "来自ccflow的自动消息:(" + BP.Web.WebUser.Name + ")工作未按时处理(" + nd.Name + "),现在移交给您。");
                        break;
                    case OutTimeDeal.SendMsgToSpecUser: //向指定的人员发消息.
                        BP.WF.Dev2Interface.Port_SendMsg(nd.DoOutTime,
                            "来自ccflow的自动消息:(" + BP.Web.WebUser.Name + ")工作未按时处理(" + nd.Name + ")",
                            "感谢您选择ccflow.", "SpecEmp" + workid);
                        break;
                    case OutTimeDeal.DeleteFlow: //删除流程.
                        msg += BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fk_flow, workid, true);
                        break;
                    case OutTimeDeal.RunSQL:
                        msg += BP.DA.DBAccess.RunSQL(nd.DoOutTime);
                        break;
                    default:
                        throw new Exception("@错误没有判断的超时处理方式." + nd.HisOutTimeDeal);
                }
            }
            Emp emp1 = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp1);
            return msg;
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
                BP.DA.Log.DefaultLogWriteLineError("没有为流程(" + fl.Name + ")的开始节点设置发起数据,请参考说明书解决.");
                return;
            }

            // 获取从表数据.
            DataSet ds = new DataSet();
            string[] dtlSQLs = me.Tag1.Split('*');
            foreach (string sql in dtlSQLs)
            {
                if (string.IsNullOrEmpty(sql))
                    continue;

                string[] tempStrs = sql.Split('=');
                string dtlName = tempStrs[0];
                DataTable dtlTable = BP.DA.DBAccess.RunSQLReturnTable(sql.Replace(dtlName + "=", ""));
                dtlTable.TableName = dtlName;
                ds.Tables.Add(dtlTable);
            }
            #endregion 读取数据.

            #region 检查数据源是否正确.
            string errMsg = "";
            // 获取主表数据.
            DataTable dtMain = BP.DA.DBAccess.RunSQLReturnTable(me.Tag);
            if (dtMain.Columns.Contains("Starter") == false)
                errMsg += "@配值的主表中没有Starter列.";

            if (dtMain.Columns.Contains("MainPK") == false)
                errMsg += "@配值的主表中没有MainPK列.";

            if (errMsg.Length > 2)
            {
                BP.DA.Log.DefaultLogWriteLineError("流程(" + fl.Name + ")的开始节点设置发起数据,不完整." + errMsg);
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
                    emp.No = starter;
                    if (emp.RetrieveFromDBSources() == 0)
                    {
                        BP.DA.Log.DefaultLogWriteLineInfo("@数据驱动方式发起流程(" + fl.Name + ")设置的发起人员:" + emp.No + "不存在。");
                        continue;
                    }

                    BP.Web.WebUser.SignInOfGener(emp);
                }

                #region  给值.
                Work wk = fl.NewWork();
                foreach (DataColumn dc in dtMain.Columns)
                    wk.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());

                if (ds.Tables.Count != 0)
                {
                    string refPK = dr["MainPK"].ToString();
                    MapDtls dtls = wk.HisNode.MapData.MapDtls; // new MapDtls(nodeTable);
                    foreach (MapDtl dtl in dtls)
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            if (dt.TableName != dtl.No)
                                continue;

                            //删除原来的数据。
                            GEDtl dtlEn = dtl.HisGEDtl;
                            dtlEn.Delete(GEDtlAttr.RefPK, wk.OID.ToString());

                            // 执行数据插入。
                            foreach (DataRow drDtl in dt.Rows)
                            {
                                if (drDtl["RefMainPK"].ToString() != refPK)
                                    continue;

                                dtlEn = dtl.HisGEDtl;

                                foreach (DataColumn dc in dt.Columns)
                                    dtlEn.SetValByKey(dc.ColumnName, drDtl[dc.ColumnName].ToString());

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
                    //BP.DA.Log.DefaultLogWriteLineInfo(msg);
                }
                catch (Exception ex)
                {
                    BP.DA.Log.DefaultLogWriteLineWarning(ex.Message);
                }
            }
            #endregion 处理流程发起.

        }
        #endregion

        #region 数据集合接口(如果您想获取一个结果集合的接口，都是以DB_开头的.)
        /// <summary>
        /// 获取能发起流程的人员
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns></returns>
        public static string GetFlowStarters(string fk_flow)
        {
            BP.WF.Node nd = new Node(int.Parse(fk_flow + "01"));
            string sql = "";
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByBindEmp: /*按人员*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + nd.NodeID + ")";
                    break;
                case DeliveryWay.ByDept: /*按部门*/
                    sql = "SELECT * FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nd.NodeID + ")";
                    break;
                case DeliveryWay.ByStation: /*按岗位*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ( SELECT FK_Station from WF_nodeStation where FK_Node=" + nd.NodeID + ")) ";
                    break;
                default:
                    throw new Exception("@开始节点的人员访问规则错误,不允许在开始节点设置此访问类型:" + nd.HisDeliveryWay);
                    break;
            }
            return sql;
        }
        public static string GetFlowStarters(string fk_flow, string fk_dept)
        {
            BP.WF.Node nd = new Node(int.Parse(fk_flow + "01"));
            string sql = "";
            switch (nd.HisDeliveryWay)
            {
                case DeliveryWay.ByBindEmp: /*按人员*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + nd.NodeID + ") and fk_dept='" + fk_dept + "'";
                    break;
                case DeliveryWay.ByDept: /*按部门*/
                    sql = "SELECT * FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + nd.NodeID + ") and fk_dept='" + fk_dept + "' ";
                    break;
                case DeliveryWay.ByStation: /*按岗位*/
                    sql = "SELECT * FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ( SELECT FK_Station from WF_nodeStation where FK_Node=" + nd.NodeID + ")) and fk_dept='" + fk_dept + "' ";
                    break;
                default:
                    throw new Exception("@开始节点的人员访问规则错误,不允许在开始节点设置此访问类型:" + nd.HisDeliveryWay);
                    break;
            }
            return sql;
        }

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
        /// 获取流程事例的运行轨迹数据.
        /// 说明：使用这些数据可以生成流程的操作日志.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <returns>从临时表与轨迹表获取流程轨迹数据.</returns>
        public static DataSet DB_GenerTrack(string fk_flow, Int64 workid, Int64 fid)
        {
            //定义变量，把数据都放入这个track里面.
            DataSet ds = new DataSet();

            #region 获取track数据.
            string sqlOfWhere2 = "";
            string sqlOfWhere1 = "";
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (fid == 0)
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "WorkID11 OR WorkID=" + dbStr + "WorkID12 )  ";
                ps.Add("WorkID11", workid);
                ps.Add("WorkID12", workid);
            }
            else
            {
                sqlOfWhere1 = " WHERE (FID=" + dbStr + "FID11 OR WorkID=" + dbStr + "FID12 ) ";
                ps.Add("FID11", fid);
                ps.Add("FID12", fid);
            }

            string sql = "";
            sql = "SELECT MyPK,ActionType,ActionTypeText,FID,WorkID,NDFrom,NDFromT,NDTo,NDToT,EmpFrom,EmpFromT,EmpTo,EmpToT,RDT,WorkTimeSpan,Msg,NodeData,Exer,Tag FROM ND" + int.Parse(fk_flow) + "Track " + sqlOfWhere1 + " ORDER BY RDT asc";
            ps.SQL = sql;
            DataTable dt = null;
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

            //把track加入里面去.
            dt.TableName = "Track";
            ds.Tables.Add(dt);
            #endregion 获取track数据.

            //把抄送信息加入.
            CCLists ens = new CCLists(fk_flow, workid, fid);
            ds.Tables.Add(ens.ToDataTableField("WF_CCList"));

            //把未来节点选择人信息写入里面.
            Int64 wfid = 0;
            if (fid != 0)
                wfid = fid;
            SelectAccpers accepts = new SelectAccpers(wfid);
            ds.Tables.Add(ens.ToDataTableField("WF_SelectAccper"));


            //把节点信息写入里面.
            sql = "SELECT * FROM WF_Node WHERE FK_Flow='" + fk_flow + "'";
            DataTable dtNode = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtNode.TableName = "WF_Node";
            ds.Tables.Add(dtNode);

            //把方向写入里面.
            sql = "SELECT * FROM WF_Direction WHERE FK_Flow='" + fk_flow + "'";
            DataTable dtDir = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dtDir.TableName = "WF_Direction";
            ds.Tables.Add(dtDir);

            return ds;
        }

        /// <summary>
        /// 获取一个流程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="userNo">操作员编号</param>
        /// <returns></returns>
        public static DataTable DB_GenerNDxxxRpt(string fk_flow, string userNo)
        {
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM ND" + int.Parse(fk_flow) + "Rpt WHERE FlowStarter=" + dbstr + "FlowStarter  ORDER BY RDT";
            ps.Add(GERptAttr.FlowStarter, userNo);
            return DBAccess.RunSQLReturnTable(ps);

        }
        #endregion 获取流程事例的轨迹图

        #region 获取能够发送或者抄送人员的列表.
        /// <summary>
        /// 获取当前节点可抄送人员列表
        /// </summary>
        /// <param name="fk_node"></param>
        /// <param name="workid"></param>
        /// <returns></returns>
        public static DataTable DB_CanCCEmps(int fk_node, Int64 workid)
        {
            DataTable table = new DataTable();
            string ccers = null;

            WorkNode node = new WorkNode(workid, fk_node);

            if (node.HisNode.HisCCRole == CCRole.AutoCC
                   || node.HisNode.HisCCRole == CCRole.HandAndAuto)
            {
                try
                {
                    /*如果是自动抄送*/
                    CC cc = node.HisNode.HisCC;

                    table = cc.GenerCCers(node.rptGe);


                }
                catch (Exception ex)
                {
                    throw new Exception("@处理操送时出现错误:" + ex.Message);
                }
            }

            #region  执行抄送 BySysCCEmps
            if (node.HisNode.HisCCRole == CCRole.BySysCCEmps)
            {
                CC cc = node.HisNode.HisCC;

                //取出抄送人列表
                string temps = node.rptGe.GetValStrByKey("SysCCEmps");
                if (!string.IsNullOrEmpty(temps))
                {
                    string[] cclist = temps.Split('|');

                    if (!table.Columns.Contains("No"))
                        table.Columns.Add("No");
                    if (!table.Columns.Contains("Name"))
                        table.Columns.Add("Name");
                    foreach (string item in cclist)
                    {
                        string[] tmp = item.Split(',');

                        DataRow row = table.NewRow();

                        row["No"] = tmp[0];
                        row["Name"] = tmp[1];
                        table.Rows.Add(row);
                    }
                }
            }
            #endregion



            return table;
        }
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
        /// <param name="FK_Emp">人员编号,如果是null,则返回所有的.</param>
        /// <returns>返回该人员的所有抄送列表,结构同表WF_CCList.</returns>
        public static DataTable DB_CCList(string FK_Emp)
        {
            Paras ps = new Paras();
            if (FK_Emp == null)
            {
                ps.SQL = "SELECT * FROM WF_CCList WHERE 1=1";
            }
            else
            {
                ps.SQL = "SELECT * FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
                ps.Add("FK_Emp", FK_Emp);
            }
            return DBAccess.RunSQLReturnTable(ps);
        }
        public static DataTable DB_CCList(string FK_Emp, CCSta sta)
        {
            Paras ps = new Paras();
            if (FK_Emp == null)
            {
                ps.SQL = "SELECT * FROM WF_CCList WHERE Sta=" + SystemConfig.AppCenterDBVarStr + "Sta";
                ps.Add("Sta", (int)sta);
            }
            else
            {
                ps.SQL = "SELECT * FROM WF_CCList WHERE CCTo=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND Sta=" + SystemConfig.AppCenterDBVarStr + "Sta";
                ps.Add("FK_Emp", FK_Emp);
                ps.Add("Sta", (int)sta);
            }
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(未读)
        /// </summary>
        /// <param name="FK_Emp">人员编号,如果是null,则返回所有的.</param>
        /// <returns>返回该人员的未读的抄送列表</returns>
        public static DataTable DB_CCList_UnRead(string FK_Emp)
        {
            return DB_CCList(FK_Emp, CCSta.UnRead);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已读)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已读的抄送列表</returns>
        public static DataTable DB_CCList_Read(string FK_Emp)
        {
            return DB_CCList(FK_Emp, CCSta.Read);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已删除)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已删除的抄送列表</returns>
        public static DataTable DB_CCList_Delete(string FK_Emp)
        {
            return DB_CCList(FK_Emp, CCSta.Del);
        }
        /// <summary>
        /// 获取指定人员的抄送列表(已回复)
        /// </summary>
        /// <param name="FK_Emp">人员编号</param>
        /// <returns>返回该人员的已删除的抄送列表</returns>
        public static DataTable DB_CCList_CheckOver(string FK_Emp)
        {
            return DB_CCList(FK_Emp, CCSta.CheckOver);
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
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                string sql = "";
                // 采用新算法.
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                    sql = "SELECT FK_Flow FROM V_FlowStarter WHERE FK_Emp='" + userNo + "'";
                else
                    sql = "SELECT FK_Flow FROM V_FlowStarterBPM WHERE FK_Emp='" + userNo + "'";

                Flows fls = new Flows();
                BP.En.QueryObject qo = new BP.En.QueryObject(fls);
                qo.AddWhereInSQL("No", sql);
                qo.addAnd();
                qo.AddWhere(FlowAttr.IsCanStart, true);
                if (WebUser.IsAuthorize)
                {
                    /*如果是授权状态*/
                    qo.addAnd();
                    WF.Port.WFEmp wfEmp = new Port.WFEmp(userNo);
                    qo.AddWhereIn("No", wfEmp.AuthorFlows);
                }
                qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
                qo.DoQuery();
                return fls;
            }

            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                string sql = "";
                // 按岗位计算.
                sql += "SELECT A.FK_Flow FROM WF_Node A,WF_NodeStation B WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 ) AND  A.NodeID=B.FK_Node AND B.FK_Station IN (" + BP.Web.WebUser.HisStationsStr + ")";
                sql += " UNION  "; //按指定的人员计算.
                sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeEmp WHERE FK_Emp='" + userNo + "' ) ";
                sql += " UNION  "; // 按部门计算.
                sql += "SELECT A.FK_Flow FROM WF_Node A,WF_NodeDept B WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 ) AND  A.NodeID=B.FK_Node AND B.FK_Dept IN (" + BP.Web.WebUser.HisDeptsStr + ") ";

                //// 采用新算法.
                //if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                //    sql = "SELECT FK_Flow FROM V_FlowStarter WHERE FK_Emp='" + userNo + "'";
                //else
                //    sql = "SELECT FK_Flow FROM V_FlowStarterBPM WHERE FK_Emp='" + userNo + "'";

                Flows fls = new Flows();
                BP.En.QueryObject qo = new BP.En.QueryObject(fls);
                qo.AddWhereInSQL("No", sql);
                qo.addAnd();
                qo.AddWhere(FlowAttr.IsCanStart, true);
                if (WebUser.IsAuthorize)
                {
                    /*如果是授权状态*/
                    qo.addAnd();
                    WF.Port.WFEmp wfEmp = new Port.WFEmp(userNo);
                    qo.AddWhereIn("No", wfEmp.AuthorFlows);
                }
                qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
                qo.DoQuery();
                return fls;
            }
            throw new Exception("@未判断的类型。");
        }
        /// <summary>
        /// 获取指定人员能够发起流程的集合
        /// 说明:利用此接口可以生成用户的发起的流程列表.
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <returns>Datatable类型的数据集合,数据结构与表WF_Flow大致相同.
        /// 如何使用该方法形成发起工作列表,请参考:\WF\UC\Start.ascx</returns>
        public static DataTable DB_GenerCanStartFlowsOfDataTable(string userNo)
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                string sql = "";
                //// 按岗位计算.
                //sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeStation WHERE FK_Station IN (SELECT FK_Station FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Emp='" + userNo + "')) ";
                //sql += " UNION  "; //按指定的人员计算.
                //sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeEmp WHERE FK_Emp='" + userNo + "' ) ";
                //sql += " UNION  "; // 按部门计算.
                //sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeDept WHERE FK_Dept IN(SELECT FK_Dept FROM Port_Emp WHERE No='" + userNo + "' UNION SELECT FK_DEPT FROM " + Glo.EmpDept + " WHERE FK_Emp='" + userNo + "') ) ";

                // 采用新算法.
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                    sql = "SELECT FK_Flow FROM V_FlowStarter WHERE FK_Emp='" + userNo + "'";
                else
                    sql = "SELECT FK_Flow FROM V_FlowStarterBPM WHERE FK_Emp='" + userNo + "'";

                Flows fls = new Flows();
                BP.En.QueryObject qo = new BP.En.QueryObject(fls);
                qo.AddWhereInSQL("No", sql);
                qo.addAnd();
                qo.AddWhere(FlowAttr.IsCanStart, true);
                if (WebUser.IsAuthorize)
                {
                    /*如果是授权状态*/
                    qo.addAnd();
                    WF.Port.WFEmp wfEmp = new Port.WFEmp(userNo);
                    qo.AddWhereIn("No", wfEmp.AuthorFlows);
                }
                qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
                return qo.DoQueryToTable();
            }

            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.WebServices)
            {
                string sql = "";
                // 按岗位计算.
                sql += "SELECT A.FK_Flow FROM WF_Node A,WF_NodeStation B WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 ) AND  A.NodeID=B.FK_Node AND B.FK_Station IN (" + BP.Web.WebUser.HisStationsStr + ")";
                sql += " UNION  "; //按指定的人员计算.
                sql += "SELECT FK_Flow FROM WF_Node WHERE NodePosType=0 AND ( WhoExeIt=0 OR WhoExeIt=2 ) AND NodeID IN ( SELECT FK_Node FROM WF_NodeEmp WHERE FK_Emp='" + userNo + "' ) ";
                sql += " UNION  "; // 按部门计算.
                sql += "SELECT A.FK_Flow FROM WF_Node A,WF_NodeDept B WHERE A.NodePosType=0 AND ( A.WhoExeIt=0 OR A.WhoExeIt=2 ) AND  A.NodeID=B.FK_Node AND B.FK_Dept IN (" + BP.Web.WebUser.HisDeptsStr + ") ";

                //// 采用新算法.
                //if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                //    sql = "SELECT FK_Flow FROM V_FlowStarter WHERE FK_Emp='" + userNo + "'";
                //else
                //    sql = "SELECT FK_Flow FROM V_FlowStarterBPM WHERE FK_Emp='" + userNo + "'";

                Flows fls = new Flows();
                BP.En.QueryObject qo = new BP.En.QueryObject(fls);
                qo.AddWhereInSQL("No", sql);
                qo.addAnd();
                qo.AddWhere(FlowAttr.IsCanStart, true);
                if (WebUser.IsAuthorize)
                {
                    /*如果是授权状态*/
                    qo.addAnd();
                    WF.Port.WFEmp wfEmp = new Port.WFEmp(userNo);
                    qo.AddWhereIn("No", wfEmp.AuthorFlows);
                }
                qo.addOrderBy("FK_FlowSort", FlowAttr.Idx);
                return qo.DoQueryToTable();
            }

            throw new Exception("@未判断的类型。");

        }
        public static DataTable DB_GenerCanStartFlowsTree(string userNo)
        {
            //发起.
            DataTable table = DB_GenerCanStartFlowsOfDataTable(userNo);
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
                if (string.IsNullOrEmpty(row["ParentNo"].ToString()))
                {
                    row["ParentNo"] = row["FK_FlowSort"];
                }
                if (string.IsNullOrEmpty(row["ICON"].ToString()))
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
        /// <returns>与表WF_GenerWorkerList结构类同的datatable.</returns>
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
        /// <returns>与表WF_GenerWorkerList结构类同的datatable.</returns>
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
        public static DataTable DB_GenerDraftDataTable()
        {
            return DB_GenerDraftDataTable(null);
        }
        public static DataTable DB_GenerDraftDataTable(string flowNo)
        {
            /*获取数据.*/
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            BP.DA.Paras ps = new BP.DA.Paras();
            if (flowNo == null)
            {
                ps.SQL = "SELECT WorkID,Title,FK_Flow,FlowName,RDT,FlowNote FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter ORDER BY RDT";
                ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
            }
            else
            {
                ps.SQL = "SELECT WorkID,Title,FK_Flow,FlowName,RDT,FlowNote FROM WF_GenerWorkFlow A WHERE WFState=1 AND Starter=" + dbStr + "Starter AND FK_Flow=" + dbStr + "FK_Flow ORDER BY RDT";
                ps.Add(GenerWorkFlowAttr.FK_Flow, flowNo);
                ps.Add(GenerWorkFlowAttr.Starter, BP.Web.WebUser.No);
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        #endregion 流程草稿

        #region 我关注的流程
        /// <summary>
        /// 获得我关注的流程列表
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">操作员编号</param>
        /// <returns>返回当前关注的流程列表.</returns>
        public static DataTable DB_Focus(string flowNo = null, string userNo = null)
        {
            if (flowNo == "")
                flowNo = null;
            if (userNo == null)
                userNo = BP.Web.WebUser.No;

            //执行sql.
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE AtPara LIKE  '%F_" + userNo + "=1%'";
            if (flowNo != null)
            {
                ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE AtPara LIKE  '%F_" + userNo + "=1%' AND FK_Flow=" + SystemConfig.AppCenterDBVarStr + "FK_Flow";
                ps.Add("FK_Flow", flowNo);
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        #endregion 我关注的流程

        #region 获取当前操作员的共享工作
        /// <summary>
        /// 获取当前人员待处理的工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>共享工作列表</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable(string userNo, string fk_flow)
        {
            //执行 todolist 调度.
            DTS_GenerWorkFlowTodoSta();

            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string sql;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                if (string.IsNullOrEmpty(fk_flow))
                {
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND TaskSta=0 AND WFState!=" + (int)WFState.Batch + " ORDER BY FK_Flow,ADT DESC ";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp  AND WFState!=" + (int)WFState.Batch + " ORDER BY FK_Flow,ADT DESC ";

                    ps.Add("FK_Emp", userNo);
                }
                else
                {
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND TaskSta=0 AND FK_Flow=" + dbstr + "FK_Flow  AND WFState!=" + (int)WFState.Batch + " ORDER BY  ADT DESC ";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND FK_Flow=" + dbstr + "FK_Flow  AND WFState!=" + (int)WFState.Batch + " ORDER BY  ADT DESC ";

                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", userNo);
                }
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp  AND TaskSta=0 ORDER BY FK_Flow,ADT DESC ";
                        else
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";

                        ps.Add("FK_Emp", userNo);
                    }
                    else
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp AND FK_Flow" + dbstr + "FK_Flow AND TaskSta=0 ORDER BY FK_Flow,ADT DESC ";
                        else
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp AND FK_Flow" + dbstr + "FK_Flow ORDER BY FK_Flow,ADT DESC ";

                        ps.Add("FK_Emp", userNo);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.SpecFlows:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + " AND TaskSta=0 ORDER BY FK_Flow,ADT DESC ";
                        else
                            sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";

                        ps.Add("FK_Emp", userNo);
                    }
                    else
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            sql = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp  AND FK_Flow" + dbstr + "FK_Flow AND FK_Flow IN " + emp.AuthorFlows + " AND TaskSta=0  ORDER BY FK_Flow,ADT DESC ";
                        else
                            sql = "SELECT * FROM WF_EmpWorks WHERE  FK_Emp=" + dbstr + "FK_Emp  AND FK_Flow" + dbstr + "FK_Flow AND FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";

                        ps.Add("FK_Emp", userNo);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 根据状态获取当前操作员的共享工作
        /// </summary>
        /// <param name="wfState">流程状态</param>
        /// <param name="fk_flow">流程编号</param>
        /// <returns>表结构与视图WF_EmpWorks一致</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable(string userNo, WFState wfState, string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string sql;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                if (string.IsNullOrEmpty(fk_flow))
                {
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta=0   ORDER BY FK_Flow,ADT DESC ";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";


                    ps.Add("WFState", (int)wfState);
                    ps.Add("FK_Emp", userNo);
                }
                else
                {
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND FK_Flow=" + dbstr + "FK_Flow AND TaskSta=0  ORDER BY  ADT DESC ";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND FK_Flow=" + dbstr + "FK_Flow ORDER BY  ADT DESC ";

                    ps.Add("WFState", (int)wfState);
                    ps.Add("FK_Flow", fk_flow);
                    ps.Add("FK_Emp", userNo);
                }
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  AND TaskSta=0  ORDER BY FK_Flow,ADT DESC ";
                        else
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  ORDER BY FK_Flow,ADT DESC ";

                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    }
                    else
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND FK_Flow" + dbstr + "FK_Flow AND TaskSta=0  ORDER BY FK_Flow,ADT DESC ";
                        else
                            ps.SQL = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND FK_Flow" + dbstr + "FK_Flow ORDER BY FK_Flow,ADT DESC ";


                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.SpecFlows:
                    if (string.IsNullOrEmpty(fk_flow))
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            sql = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + " AND TaskSta=0   ORDER BY FK_Flow,ADT DESC ";
                        else
                            sql = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";


                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                    }
                    else
                    {
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            sql = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  AND FK_Flow" + dbstr + "FK_Flow AND FK_Flow IN " + emp.AuthorFlows + " AND TaskSta=0   ORDER BY FK_Flow,ADT DESC ";
                        else
                            sql = "SELECT * FROM WF_EmpWorks WHERE WFState=" + dbstr + "WFState AND FK_Emp=" + dbstr + "FK_Emp  AND FK_Flow" + dbstr + "FK_Flow AND FK_Flow IN " + emp.AuthorFlows + "  ORDER BY FK_Flow,ADT DESC ";

                        ps.Add("WFState", (int)wfState);
                        ps.Add("FK_Emp", BP.Web.WebUser.No);
                        ps.Add("FK_Flow", fk_flow);
                    }
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取当前操作人员的待办信息
        /// 数据内容请参考图:WF_EmpWorks
        /// </summary>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerEmpWorksOfDataTable()
        {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + "  OR WFState=" + (int)WFState.AskForReplay + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + " OR WFState=" + (int)WFState.Fix;
            string sql;

            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                if (BP.WF.Glo.IsEnableTaskPool == true)
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1  ORDER BY ADT DESC";
                else
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY ADT DESC";

                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1  ORDER BY ADT DESC";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY ADT DESC";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.SpecFlows:
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + " AND TaskSta!=0    ORDER BY ADT DESC";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "   ORDER BY ADT DESC";

                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.None:
                    /*不是授权状态*/
                    if (BP.WF.Glo.IsEnableTaskPool == true)
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta!=1  ORDER BY ADT DESC";
                    else
                        ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY ADT DESC";

                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    return BP.DA.DBAccess.RunSQLReturnTable(ps);

                    WebUser.Auth = null; //对方已经取消了授权.
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获得已完成数据统计列表
        /// </summary>
        /// <param name="userNo">操作员编号</param>
        /// <returns>具有FlowNo,FlowName,Num三个列的指定人员的待办列表</returns>
        public static DataTable DB_FlowCompleteGroup(string userNo)
        {
            if (Glo.IsDeleteGenerWorkFlow == false)
            {
                /* 如果不是删除流程注册表. */
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT FK_Flow as No,FlowName,COUNT(*) Num FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + userNo + "@%' AND FID=0 AND WFState=" + (int)WFState.Complete + " GROUP BY FK_Flow,FlowName";
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
            else
            {
                throw new Exception("未实现..");
                //Paras ps = new Paras();
                //string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                //ps.SQL = "SELECT * FROM V_FlowData WHERE FlowEmps LIKE '%@" + userNo + "%' AND FID=0 AND WFState=" + (int)WFState.Complete;
                //return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
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
            if (Glo.IsDeleteGenerWorkFlow == false)
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
            else
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT *,FlowEndNode FK_Node FROM V_FlowData WHERE FlowEmps LIKE '%@" + userNo + "%' AND   FID=0 AND WFState=" + (int)WFState.Complete;

                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
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
            if (Glo.IsDeleteGenerWorkFlow == false)
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
            else
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT *,FlowEndNode FK_Node FROM V_FlowData WHERE FK_Flow='" + strFlow + "' AND FlowEmps LIKE '%@" + userNo + "%' AND FID=0 AND WFState=" + (int)WFState.Complete;
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
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
            if (Glo.IsDeleteGenerWorkFlow == false)
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
            else
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT *,FlowEndNode FK_Node FROM V_FlowData WHERE FK_Flow='" + strFlow + "' AND FID=0 AND WFState=" + (int)WFState.Complete;
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
        }
        /// <summary>
        /// 获取已经完成流程
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_TongJi_FlowComplete()
        {
            if (Glo.IsDeleteGenerWorkFlow == false)
            {
                /* 如果不是删除流程注册表. */
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT T.FK_Flow, T.FlowName, COUNT(T.WorkID) as Num FROM WF_GenerWorkFlow T WHERE T.Emps LIKE '%@" + WebUser.No + "@%' AND T.FID=0 AND T.WFSta=" + (int)WFSta.Complete + " GROUP BY T.FK_Flow,T.FlowName";
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
            else
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT T.FK_Flow, T.FlowName, COUNT(T.WorkID) as Num FROM V_FlowData T WHERE T.FlowEmps LIKE '%@" + WebUser.No + "@%'  AND T.FID=0 AND T.WFSta=" + (int)WFSta.Complete + "   GROUP BY T.FK_Flow,T.FlowName";
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
        }
        /// <summary>
        /// 获取已经完成流程
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_FlowComplete()
        {
            if (Glo.IsDeleteGenerWorkFlow == false)
            {
                /* 如果不是删除流程注册表. */
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM WF_GenerWorkFlow T WHERE T.Emps LIKE '%@" + WebUser.No + "@%' AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY  RDT DESC";
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
            else
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                ps.SQL = "SELECT 'RUNNING' AS Type, T.* FROM V_FlowData T WHERE T.FlowEmps LIKE '%@" + WebUser.No + "@%' AND T.FID=0 AND T.WFState=" + (int)WFState.Complete + " ORDER BY RDT DESC";
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
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
            if (Glo.IsDeleteGenerWorkFlow == false)
            {
                /* 如果不是删除流程注册表. */
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                if (string.IsNullOrEmpty(fk_flow))
                {
                    if (string.IsNullOrEmpty(title))
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow!='010' order by RDT desc";
                    else
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' and Title Like '%" + title + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow!='010' order by RDT desc";
                }
                else
                {
                    if (string.IsNullOrEmpty(title))
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow='" + fk_flow + "' order by RDT desc";
                    else
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM WF_GenerWorkFlow WHERE Emps LIKE '%@" + WebUser.No + "@%' and Title Like '%" + title + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow='" + fk_flow + "' order by RDT desc";
                }
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
            else
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                if (string.IsNullOrEmpty(fk_flow))
                {
                    if (string.IsNullOrEmpty(title))
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM V_FlowData WHERE FlowEmps LIKE '%@" + WebUser.No + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow!='010' order by RDT desc";
                    else
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM V_FlowData WHERE FlowEmps LIKE '%@" + WebUser.No + "%' and Title Like '%" + title + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow!='010' order by RDT desc";
                }
                else
                {
                    if (string.IsNullOrEmpty(title))
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM V_FlowData WHERE FlowEmps LIKE '%@" + WebUser.No + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow='" + fk_flow + "' order by RDT desc";
                    else
                        ps.SQL = "SELECT 'RUNNING' AS Type,* FROM V_FlowData WHERE FlowEmps LIKE '%@" + WebUser.No + "%' and Title Like '%" + title + "%' AND FID=0 AND WFState=" + (int)WFState.Complete + " and FK_Flow='" + fk_flow + "' order by RDT desc";
                }
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }
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
        //获取我发起的流程
        public static DataTable DB_MyFlow(string fk_flow, string title)
        {
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            if (string.IsNullOrEmpty(fk_flow))
            {
                if (string.IsNullOrEmpty(title))
                    ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE Starter='" + WebUser.No + "' AND FK_Flow!='010' ";
                else
                    ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE Starter='" + WebUser.No + "' AND FK_Flow!='010' and Title Like '%" + title + "%' ";
            }
            else
            {
                if (string.IsNullOrEmpty(title))
                    ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE Starter='" + WebUser.No + "' AND FK_Flow='" + fk_flow + "' ";
                else
                    ps.SQL = "SELECT * FROM WF_GenerWorkFlow WHERE Starter='" + WebUser.No + "' AND FK_Flow='" + fk_flow + "' and Title Like '%" + title + "%'";
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }

        /// <summary>
        /// 获得任务池的工作列表
        /// </summary>
        /// <returns>任务池的工作列表</returns>
        public static DataTable DB_TaskPool()
        {
            if (BP.WF.Glo.IsEnableTaskPool == false)
                throw new Exception("@你必须在Web.config中启用IsEnableTaskPool才可以执行此操作。");

            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Sharing;
            string sql;
            string realSql = null;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta=0";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.SpecFlows:
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + " ";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获得我从任务池里申请下来的工作列表
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_TaskPoolOfMyApply()
        {
            if (BP.WF.Glo.IsEnableTaskPool == false)
                throw new Exception("@你必须在Web.config中启用IsEnableTaskPool才可以执行此操作。");

            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string wfSql = "  (WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + ") AND TaskSta=" + (int)TaskSta.Takeback;
            string sql;
            string realSql;
            if (WebUser.IsAuthorize == false)
            {
                /*不是授权状态*/
                // ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY FK_Flow,ADT DESC ";
                //ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp ORDER BY ADT DESC ";
                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp";

                // ps.SQL = "select v1.*,v2.name,v3.name as ParentName from (" + realSql + ") as v1 left join JXW_Inc v2 on v1.WorkID=v2.OID left join Jxw_Inc V3 on v1.PWorkID = v3.OID ORDER BY v1.ADT DESC";

                ps.Add("FK_Emp", BP.Web.WebUser.No);
                return BP.DA.DBAccess.RunSQLReturnTable(ps);
            }

            /*如果是授权状态, 获取当前委托人的信息. */
            WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
            switch (emp.HisAuthorWay)
            {
                case Port.AuthorWay.All:
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND TaskSta=0";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.SpecFlows:
                    ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp AND  FK_Flow IN " + emp.AuthorFlows + "";
                    ps.Add("FK_Emp", BP.Web.WebUser.No);
                    break;
                case Port.AuthorWay.None:
                    throw new Exception("对方(" + WebUser.No + ")已经取消了授权.");
                default:
                    throw new Exception("no such way...");
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获得所有的流程挂起工作列表
        /// </summary>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerHungUpList()
        {
            return DB_GenerHungUpList(null);
        }
        /// <summary>
        /// 获得指定流程挂起工作列表
        /// </summary>
        /// <param name="fk_flow">流程编号,如果编号为空则返回所有的流程挂起工作列表.</param>
        /// <returns>返回从视图WF_EmpWorks查询出来的数据.</returns>
        public static DataTable DB_GenerHungUpList(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.HungUp;
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND A.FK_Flow IN " + emp.AuthorFlows;
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.FK_Flow='" + fk_flow + "' AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND  B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1   ";
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 ";
            }
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
            return gwfs.ToDataTableField();
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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.Delete;
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(WebUser.No);
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND A.FK_Flow IN " + emp.AuthorFlows;
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND  B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1   ";
                else
                    sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 ";
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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            Flow fl = new Flow(fk_flow);
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string sql = "SELECT OID,Title,RDT,FID FROM " + fl.PTable + " WHERE WFState=" + (int)sta + " AND Rec=" + dbstr + "Rec";
            BP.DA.Paras ps = new BP.DA.Paras();
            ps.SQL = sql;
            ps.Add("Rec", BP.Web.WebUser.No);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion

        #region 获取当前可以退回的节点。
        /// <summary>
        /// 获取当前节点可以退回的节点
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <returns>No节点编号,Name节点名称,Rec记录人,RecName记录人名称</returns>
        public static DataTable DB_GenerWillReturnNodes(int fk_node, Int64 workid, Int64 fid)
        {
            DataTable dt = new DataTable("obt");
            dt.Columns.Add("No"); // 节点ID
            dt.Columns.Add("Name"); // 节点名称.
            dt.Columns.Add("Rec"); // 被退回节点上的操作员编号.
            dt.Columns.Add("RecName"); // 被退回节点上的操作员名称.

            Node nd = new Node(fk_node);
            if (nd.HisRunModel == RunModel.SubThread)
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
                            wk.OID = fid;
                            if (wk.RetrieveFromDBSources() == 0)
                                continue;
                            break;
                        case RunModel.SubThread:
                            wk = ndFrom.HisWork;
                            wk.OID = workid;
                            if (wk.RetrieveFromDBSources() == 0)
                                continue;
                            break;
                        case RunModel.Ordinary:
                        default:
                            throw new Exception("流程设计异常，子线程的上一个节点不能是普通节点。");
                            break;
                    }

                    if (ndFrom.NodeID == fk_node)
                        continue;

                    DataRow dr = dt.NewRow();
                    dr["No"] = ndFrom.NodeID.ToString();
                    dr["Name"] = ndFrom.Name;

                    dr["Rec"] = wk.Rec;
                    dr["RecName"] = wk.RecText;

                    dt.Rows.Add(dr);
                }
                if (dt.Rows.Count == 0)
                    throw new Exception("没有获取到应该退回的节点列表.");
                return dt;
            }

            string sql = "";
            if (nd.IsHL || nd.IsFLHL)
            {
                /*如果当前点是分流，或者是分合流，就不按退回规则计算了。*/
                sql = "SELECT FK_Node AS No,FK_NodeText as Name, FK_Emp as Rec, FK_EmpText as RecName FROM WF_GenerWorkerlist WHERE FID=" + fid + " AND WorkID=" + workid + " AND FK_Node!=" + fk_node + " AND IsPass=1 ORDER BY RDT  ";
                return DBAccess.RunSQLReturnTable(sql);
            }

            WorkNode wn = new WorkNode(workid, fk_node);
            WorkNodes wns = new WorkNodes();
            switch (nd.HisReturnRole)
            {
                case ReturnRole.CanNotReturn:
                    return dt;
                case ReturnRole.ReturnAnyNodes:
                    if (nd.TodolistModel == TodolistModel.Order)
                        sql = "SELECT FK_Node as No,FK_NodeText as Name,FK_Emp as Rec,FK_EmpText as RecName FROM WF_GenerWorkerlist WHERE  (WorkID=" + workid + " AND IsEnable=1 AND IsPass=1 AND FK_Node!=" + fk_node + ") OR (FK_Node=" + fk_node + " AND IsPass <0)  ORDER BY RDT";
                    else
                        sql = "SELECT FK_Node as No,FK_NodeText as Name,FK_Emp as Rec,FK_EmpText as RecName FROM WF_GenerWorkerlist WHERE  WorkID=" + workid + " AND IsEnable=1 AND IsPass=1 AND FK_Node!=" + fk_node + " ORDER BY RDT";
                    return DBAccess.RunSQLReturnTable(sql);
                    break;
                case ReturnRole.ReturnPreviousNode:
                    WorkNode mywnP = wn.GetPreviousWorkNode();
                    //  turnTo = mywnP.HisWork.Rec + mywnP.HisWork.RecText;
                    DataRow dr1 = dt.NewRow();
                    dr1["No"] = mywnP.HisNode.NodeID.ToString();
                    dr1["Name"] = mywnP.HisNode.Name;

                    dr1["Rec"] = mywnP.HisWork.Rec;
                    dr1["RecName"] = mywnP.HisWork.RecText;
                    dt.Rows.Add(dr1);
                    break;
                case ReturnRole.ReturnSpecifiedNodes: //退回指定的节点。
                    if (wns.Count == 0)
                        wns.GenerByWorkID(wn.HisNode.HisFlow, workid);

                    NodeReturns rnds = new NodeReturns();
                    rnds.Retrieve(NodeReturnAttr.FK_Node, fk_node);
                    if (rnds.Count == 0)
                        throw new Exception("@流程设计错误，您设置该节点可以退回指定的节点，但是指定的节点集合为空，请在节点属性设置它的制订节点。");
                    foreach (WorkNode mywn in wns)
                    {
                        if (mywn.HisNode.NodeID == fk_node)
                            continue;

                        if (rnds.Contains(NodeReturnAttr.ReturnTo,
                            mywn.HisNode.NodeID) == false)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["No"] = mywn.HisNode.NodeID.ToString();
                        dr["Name"] = mywn.HisNode.Name;
                        dr["Rec"] = mywn.HisWork.Rec;
                        dr["RecName"] = mywn.HisWork.RecText;
                        dt.Rows.Add(dr);
                    }
                    break;
                case ReturnRole.ByReturnLine: //按照流程图画的退回线执行退回.
                    Directions dirs = new Directions();
                    dirs.Retrieve(DirectionAttr.Node, fk_node, DirectionAttr.DirType, 1);
                    if (dirs.Count == 0)
                        throw new Exception("@流程设计错误:当前节点没有画向后退回的退回线,更多的信息请参考退回规则.");
                    foreach (Direction dir in dirs)
                    {
                        Node toNode = new Node(dir.ToNode);
                        sql = "SELECT FK_Emp,FK_EmpText FROM WF_GenerWorkerlist WHERE FK_Node=" + toNode.NodeID + " AND WorkID=" + workid + " AND IsEnable=1 AND IsPass=1";
                        DataTable dt1 = DBAccess.RunSQLReturnTable(sql);
                        if (dt1.Rows.Count == 0)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["No"] = toNode.NodeID.ToString();
                        dr["Name"] = toNode.Name;
                        dr["Rec"] = dt1.Rows[0][0];
                        dr["RecName"] = dt1.Rows[0][1];
                        dt.Rows.Add(dr);
                    }
                    break;
                default:
                    throw new Exception("@没有判断的退回类型。");
            }

            if (dt.Rows.Count == 0)
                throw new Exception("@没有计算出来要退回的节点，请管理员确认节点退回规则是否合理？");
            return dt;
        }
        #endregion 获取当前可以退回的节点

        #region 获取当前操作员的在途工作

        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// 该接口为在途菜单提供数据,在在途工作中，可以执行撤销发送。
        /// </summary>
        /// <param name="userNo">操作员</param>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="isMyStarter">是否仅仅查询我发起的在途流程</param>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning(string userNo, string fk_flow, bool isMyStarter=false)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            string dbStr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(userNo);
                if (string.IsNullOrEmpty(fk_flow))
                {
                    if (isMyStarter ==true )
                    {
                        ps.SQL = "SELECT DISTINCT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND A.Starter=" + dbStr + "Starter  AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND A.FK_Flow IN " + emp.AuthorFlows;
                        ps.Add("Starter", userNo);
                        ps.Add("FK_Emp", userNo);
                    }
                    else
                    {
                        ps.SQL = "SELECT DISTINCT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND A.FK_Flow IN " + emp.AuthorFlows;
                        ps.Add("FK_Emp", userNo);
                    }
                }
                else
                {
                    if (isMyStarter == true)
                    {
                        ps.SQL = "SELECT DISTINCT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE  A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND  A.Starter=" + dbStr + "Starter AND A.FK_Flow IN " + emp.AuthorFlows;
                        ps.Add("FK_Flow", fk_flow);
                        ps.Add("FK_Emp", userNo);
                        ps.Add("Starter", userNo);
                    }
                    else
                    {
                        ps.SQL = "SELECT DISTINCT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND A.FK_Flow IN " + emp.AuthorFlows;
                        ps.Add("FK_Flow", fk_flow);
                        ps.Add("FK_Emp", userNo);
                    }
                 
                }
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                {
                    if (isMyStarter == true)
                    {
                        ps.SQL = "SELECT DISTINCT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND  A.Starter=" + dbStr + "Starter ";
                        ps.Add("FK_Emp", userNo);
                        ps.Add("Starter", userNo);
                    }
                    else
                    {
                        ps.SQL = "SELECT DISTINCT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) ";
                        ps.Add("FK_Emp", userNo);
                    }
                }
                else
                {
                    if (isMyStarter == true)
                    {
                        ps.SQL = "SELECT DISTINCT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 ) AND  A.Starter=" + dbStr + "Starter  ";
                        ps.Add("FK_Flow", fk_flow);
                        ps.Add("FK_Emp", userNo);
                        ps.Add("Starter", userNo);
                    }
                    else
                    {
                        ps.SQL = "SELECT DISTINCT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow=" + dbStr + "FK_Flow  AND A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 ) ";
                        ps.Add("FK_Flow", fk_flow);
                        ps.Add("FK_Emp", userNo);
                    }
                }
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 在途统计:用于流程查询
        /// </summary>
        /// <returns>返回 FK_Flow,FlowName,Num 三个列.</returns>
        public static DataTable DB_TongJi_Runing()
        {
            string dbStr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(BP.Web.WebUser.No);
                ps.SQL = "SELECT a.FK_Flow,a.FlowName, Count(a.WorkID) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND A.FK_Flow IN " + emp.AuthorFlows + " GROUP BY A.FK_Flow, A.FlowName";
                ps.Add("FK_Emp", WebUser.No);
            }
            else
            {
                ps.SQL = "SELECT a.FK_Flow,a.FlowName, Count(a.WorkID) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0)  GROUP BY A.FK_Flow, A.FlowName";
                ps.Add("FK_Emp", WebUser.No);
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 统计流程状态
        /// </summary>
        /// <returns>返回：流程类别编号，名称，流程编号，流程名称，TodoSta0代办中,TodoSta1预警中,TodoSta2预期中,TodoSta3已办结. </returns>
        public static DataTable DB_TongJi_TodoSta()
        {
            string dbStr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(BP.Web.WebUser.No);
                ps.SQL = "SELECT a.FK_Flow,a.FlowName, Count(a.WorkID) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) AND A.FK_Flow IN " + emp.AuthorFlows + " GROUP BY A.FK_Flow, A.FlowName";
                ps.Add("FK_Emp", WebUser.No);
            }
            else
            {
                ps.SQL = "SELECT a.FK_Flow,a.FlowName, Count(a.WorkID) as Num FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp=" + dbStr + "FK_Emp AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0)  GROUP BY A.FK_Flow, A.FlowName";
                ps.Add("FK_Emp", WebUser.No);
            }
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        public static DataTable DB_GenerRuning2(string userNo, string fk_flow, string title)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.Runing;
            if (string.IsNullOrEmpty(fk_flow))
            {
                if (string.IsNullOrEmpty(title))
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) and A.FK_Flow!='010'";
                else
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND  (B.IsPass=1 or B.IsPass < 0) and A.FK_Flow!='010' and A.Title Like '%" + title + "%'";
            }
            else
            {
                if (string.IsNullOrEmpty(title))
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 )";
                else
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 ) and A.Title Like '%" + title + "%' ";
            }

            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 在途工作
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_GenerRuningV2()
        {
            string userNo = WebUser.No;
            string fk_flow = null;
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            string sql;
            int state = (int)WFState.Runing;
            if (WebUser.IsAuthorize)
            {
                WF.Port.WFEmp emp = new Port.WFEmp(userNo);
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
                else
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND B.IsEnable=1 AND B.IsPass=1 AND A.FK_Flow IN " + emp.AuthorFlows;
            }
            else
            {
                if (string.IsNullOrEmpty(fk_flow))
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND B.IsPass=1 ";
                else
                    sql = "SELECT a.* FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + fk_flow + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + userNo + "' AND B.IsEnable=1 AND B.IsPass=1 ";
            }
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 获取内部系统消息
        /// </summary>
        /// <param name="myPK"></param>
        /// <returns></returns>
        public static DataTable DB_GenerPopAlert(string type)
        {
            string sql = "";
            if (type == "unRead")
            {
                sql = "SELECT LEFT(CONVERT(VARCHAR(20),RDT,120),10) AS SortRDT,Datepart(WEEKDAY, CONVERT(DATETIME,RDT)  + @@DateFirst - 1) AS WeekRDT,"
                    + "* FROM Sys_SMS WHERE SendTo ='" + WebUser.No + "' AND (IsRead = 0 OR IsRead IS NULL)  ORDER BY RDT DESC";
            }
            else
            {
                sql = "SELECT LEFT(CONVERT(VARCHAR(20),RDT,120),10) AS SortRDT,Datepart(WEEKDAY, CONVERT(DATETIME,RDT)  + @@DateFirst - 1) AS WeekRDT,"
                    + "* FROM Sys_SMS WHERE SendTo ='" + WebUser.No + "'  ORDER BY RDT DESC";
            }
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// 获取外部系统消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="No"></param>
        /// <returns></returns>
        public static DataTable DB_GenerPopAlert(string type, string No)
        {
            string sql = "";
            if (type == "unRead")
            {
                sql = "SELECT LEFT(CONVERT(VARCHAR(20),RDT,120),10) AS SortRDT,Datepart(WEEKDAY, CONVERT(DATETIME,RDT)  + @@DateFirst - 1) AS WeekRDT,"
                    + "* FROM Sys_SMS WHERE SendTo ='" + No + "' AND (IsRead = 0 OR IsRead IS NULL)  ORDER BY RDT DESC";
            }
            else
            {
                sql = "SELECT LEFT(CONVERT(VARCHAR(20),RDT,120),10) AS SortRDT,Datepart(WEEKDAY, CONVERT(DATETIME,RDT)  + @@DateFirst - 1) AS WeekRDT,"
                    + "* FROM Sys_SMS WHERE SendTo ='" + No + "'  ORDER BY RDT DESC";
            }
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 更新消息状态
        /// </summary>
        /// <param name="myPK"></param>
        public static DataTable DB_GenerUpdateMsgSta(string myPK)
        {
            string sql = "";
            sql = " UPDATE Sys_SMS SET IsRead=1 WHERE MyPK='" + myPK + "'";
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// 获取未完成的流程(也称为在途流程:我参与的但是此流程未完成)
        /// </summary>
        /// <returns>返回从数据视图WF_GenerWorkflow查询出来的数据.</returns>
        public static DataTable DB_GenerRuning()
        {
            DataTable dt = DB_GenerRuning(BP.Web.WebUser.No, null);
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

            if (fl.HisDataStoreModel == DataStoreModel.ByCCFlow)
                fromTable = nd.PTable;
            else
                fromTable = fl.PTable;
            string sql = "SELECT a.*, b.Starter,b.Title as STitle,b.ADT,b.WorkID FROM " + fromTable
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            // string sql = "SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return dt;
        }

        #endregion 获取当前的批处理工作
        #endregion

        #region 登陆接口
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        /// <param name="SID">安全ID,请参考流程设计器操作手册</param>
        public static void Port_Login(string userNo, string sid)
        {
            if (WebUser.No == userNo)
                return; 

            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT SID FROM Port_Emp WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No";
                ps.Add("No", userNo);
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    throw new Exception("用户不存在或者SID错误。");

                if (dt.Rows[0]["SID"].ToString() != sid)
                    throw new Exception("用户不存在或者SID错误。");
            }

            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp, true);
            WebUser.IsWap = false;
            return;
        }
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        /// <param name="isRememberMe">是否记住密码</param>
        public static string Port_Login(string userNo, bool isRememberMe = true)
        {
            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp, isRememberMe);
            WebUser.IsWap = false;
            return Port_GetSID(userNo);
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
            ps.SQL = "SELECT MyPK, EmailTitle  FROM sys_sms where SendTo=" + SystemConfig.AppCenterDBVarStr + "SendTo AND IsAlert=0";
            ps.Add("SendTo", userNo);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += "@" + dr[0] + "=" + dr[1].ToString();
            }
            ps = new Paras();
            ps.SQL = "UPDATE  sys_sms SET IsAlert=1 WHERE  SendTo=" + SystemConfig.AppCenterDBVarStr + "SendTo AND IsAlert=0";
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
        /// 获取SID
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns>SID</returns>
        public static string Port_GetSIDName(string userNo)
        {
            string info = "";
            Paras ps = new Paras();
            ps.SQL = "SELECT SID, Name FROM Port_Emp WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No";
            ps.Add("No", userNo);
            DataTable table = BP.DA.DBAccess.RunSQLReturnTable(ps);
            info = BP.Tools.FormatToJson.ToJson(table);
            return info;
        }
        /// <summary>
        /// 获取SID
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns>SID</returns>
        public static string Port_GetSID(string userNo)
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT SID FROM Port_Emp WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No";
                ps.Add("No", userNo);
                string sid = BP.DA.DBAccess.RunSQLReturnString(ps);
                if (string.IsNullOrEmpty(sid) == true)
                {
                    try
                    {
                        sid = BP.DA.DBAccess.GenerGUID();
                        ps.SQL = Glo.UpdataSID;
                        ps.Add("SID", sid);
                        ps.Add("No", userNo);
                        BP.DA.DBAccess.RunSQL(ps);
                    }
                    catch
                    {
                        // throw new Exception("@可");
                        /*这里有可能是更新失败，是因为用户连接的视图. */
                    }
                }
                return sid;
            }

            throw new Exception("@没有判断的数据源模式...");
        }
        /// <summary>
        /// 验证用户的合法性
        /// </summary>
        /// <param name="UserNo">用户编号</param>
        /// <param name="SID">密钥</param>
        /// <returns>是否匹配</returns>
        public static bool Port_CheckUserLogin(string UserNo, string SID)
        {

#warning 为了调试，暂时都返回true.
            return true;

            if (string.IsNullOrEmpty(UserNo))
                return false;
            if (string.IsNullOrEmpty(SID))
                return false;

            //随后改成动态sid
            if (UserNo == "admin" && SID.ToUpper() == "33273D4A-35C8-4AE8-BDD9-0085EA648CCB")
                return true;

            BP.Port.Emp emp = new BP.Port.Emp(UserNo);
            bool bHave = emp.RetrieveByAttr(BP.Port.EmpAttr.No, UserNo);
            //用户编号不存在
            if (bHave == false)
                return false;

            //用户匹配正确
            if (emp.SID.Equals(SID))
                return true;
            return false;
        }
        /// <summary>
        /// 设置SID
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID信息</param>
        /// <returns>SID</returns>
        public static bool Port_SetSID(string userNo, string sid)
        {
            Paras ps = new Paras();
            ps.SQL = "UPDATE Port_Emp SET SID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "SID WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "No";
            ps.Add("SID", sid);
            ps.Add("No", userNo);
            if (BP.DA.DBAccess.RunSQL(ps) == 1)
                return true;
            else
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
            string flowNo, Int64 nodeID, Int64 workID, Int64 fid)
        {
            if (workID != 0)
            {
                string url = Glo.HostURL + "WF/Do.aspx?SID=" + userNo + "_" + workID + "_" + nodeID;
                url = url.Replace("//", "/");
                url = url.Replace("//", "/");
                msgDoc += " <hr>打开工作: " + url;
            }

            string para = "@FK_Flow=" + flowNo + "@WorkID=" + workID + "@FK_Node=" + nodeID + "@Sender=" + BP.Web.WebUser.No;
            BP.WF.SMS.SendMsg(userNo, title, msgDoc, msgFlag, msgType, para);
        }
      
      
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailAddress">邮件地址</param>
        /// <param name="emilTitle">标题</param>
        /// <param name="emailBody">内容</param>
        /// <param name="msgType">消息类型(CC抄送,Todolist待办,Return退回,Etc其他消息...)</param>
        /// <param name="msgGroupFlag">分组标记</param>
        /// <param name="sender">发送人</param>
        /// <param name="msgPK">消息唯一标记，防止发送重复.</param>
        public static void Port_SendEmail(string mailAddress, string emilTitle, string emailBody,
            string msgType, string msgGroupFlag=null, string sender = null, string msgPK = null, string sendToEmpNo=null)
        {
            if (string.IsNullOrEmpty(mailAddress))
                return;

            SMS sms = new SMS();
            if (string.IsNullOrEmpty(msgPK) == false)
            {
                /*如果有唯一标志,就判断是否有该数据，没有该数据就允许插入.*/
                if (sms.IsExit(SMSAttr.MyPK, msgPK) == true)
                    return;
                sms.MyPK = msgPK;
            }
            else
            {
                sms.MyPK = DBAccess.GenerGUID();
            }

            sms.HisEmailSta = MsgSta.UnRun;
            if (sender == null)
                sms.Sender = WebUser.No;
            else
                sms.Sender = sender;

            sms.SendToEmpNo = sendToEmpNo;

            //邮件地址.
            sms.Email = mailAddress;

            //邮件标题.
            sms.Title = emilTitle;
            sms.DocOfEmail = emailBody;

            //手机状态禁用.
            sms.HisMobileSta = MsgSta.Disable;

            // 其他属性.
            sms.RDT = BP.DA.DataType.CurrentDataTime;

            sms.MsgFlag = msgGroupFlag; // 消息标志.
            sms.MsgType = msgType;   // 消息类型(CC抄送,Todolist待办,Return退回,Etc其他消息...).
            sms.Insert();
        }
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="tel">电话</param>
        /// <param name="smsDoc">短信内容</param>
        /// <param name="msgType">消息类型</param>
        /// <param name="msgGroupFloag">消息分组</param>
        /// <param name="sender">发送人</param>
        /// <param name="msgPK">唯一标志,防止发送重复.</param>
        /// <param name="sendEmpNo">发送给人员.</param>
        /// <param name="atParas">参数.</param>
        public static void Port_SendSMS(string tel, string smsDoc, string msgType, string msgGroupFlag, 
            string sender = null, string msgPK = null, string sendToEmpNo = null, string atParas=null)
        {
            if (string.IsNullOrEmpty(tel))
                return;

            SMS sms = new SMS();
            if (string.IsNullOrEmpty(msgPK) == false)
            {
                /*如果有唯一标志,就判断是否有该数据，没有该数据就允许插入.*/
                if (sms.IsExit(SMSAttr.MyPK, msgPK) == true)
                    return;
                sms.MyPK = msgPK;
            }
            else
            {
                sms.MyPK = DBAccess.GenerGUID();
            }

            sms.HisEmailSta = MsgSta.Disable;
            sms.HisMobileSta = MsgSta.UnRun;

            if (sender == null)
                sms.Sender = WebUser.No;
            else
                sms.Sender = sender;

            //发送给人员ID , 有可能这个人员空的.
            sms.SendToEmpNo = sendToEmpNo;

            sms.Mobile = tel;
            sms.MobileInfo = smsDoc;

            // 其他属性.
            sms.RDT = BP.DA.DataType.CurrentDataTime;

            sms.MsgType = msgType; // 消息类型.

            sms.MsgFlag = msgGroupFlag; // 消息分组标志,用于批量删除.

            // 先保留本机一份.
            sms.Insert();
        }
        /// <summary>
        /// 转化流程Code到流程编号
        /// </summary>
        /// <param name="FlowMark">流程编号</param>
        /// <returns>返回编码</returns>
        public static string TurnFlowMarkToFlowNo(string flowMark)
        {
            if (string.IsNullOrEmpty(flowMark))
                return "";

            // 如果是编号，就不用转化.
            if (DataType.IsNumStr(flowMark))
                return flowMark;

            string s = DBAccess.RunSQLReturnStringIsNull("SELECT No FROM WF_Flow WHERE FlowMark='" + flowMark + "'", null);
            if (s == null)
                throw new Exception("@FlowMark错误:" + flowMark + ",没有找到它的流程编号.");
            return s;
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
            ps.SQL = "SELECT MsgType , Count(*) as Num FROM Sys_SMS WHERE SendTo=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "SendTo AND RDT >" + BP.Sys.SystemConfig.AppCenterDBVarStr + "RDT Group By MsgType";
            ps.Add(BP.WF.SMSAttr.SendTo, userNo);
            ps.Add(BP.WF.SMSAttr.RDT, dateLastTime);
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
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
            ps.SQL = "SELECT MsgType , Count(*) as Num FROM Sys_SMS WHERE SendTo=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "SendTo  Group By MsgType";
            ps.Add(BP.WF.SMSAttr.SendTo, userNo);
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            return dt;
        }

        //public static bool Port_auto(string userNo, string sid)
        //{
        //}

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
        public static void WriteTrack(string flowNo, int nodeFrom, Int64 workid, Int64 fid, string msg, ActionType at, string tag,
            string cFlowInfo, string optionMsg=null, string empNoTo=null, string empNameTo=null)
        {
            if (at == ActionType.CallChildenFlow)
                if (string.IsNullOrEmpty(cFlowInfo) == true)
                    throw new Exception("@必须输入信息cFlowInfo信息,在 CallChildenFlow 模式下.");

            if (string.IsNullOrEmpty(optionMsg))
                optionMsg = Track.GetActionTypeT(at);

            Track t = new Track();
            t.WorkID = workid;
            t.FID = fid;
            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;
            t.ActionTypeText = optionMsg;

            Node nd = new Node(nodeFrom);
            t.NDFrom = nodeFrom;
            t.NDFromT = nd.Name;

            t.EmpFrom = WebUser.No;
            t.EmpFromT = WebUser.Name;
            t.FK_Flow = flowNo;

            t.NDTo = nodeFrom;
            t.NDToT = nd.Name;

            if (empNoTo == null)
                t.EmpTo = WebUser.No;
            else
                t.EmpTo = empNoTo;

            if (empNameTo == null)
                t.EmpToT = WebUser.Name;
            else
                t.EmpToT = empNameTo;

            t.Msg = msg;

            if (tag != null)
                t.Tag = tag;

            try
            {
                t.Insert();
            }
            catch
            {
                t.CheckPhysicsTable();
                t.Insert();
                t.DirectInsert();
            }

            #region 特殊判断.
            if (at == ActionType.CallChildenFlow)
            {
                /* 如果是吊起子流程，就要向它父流程信息里写数据，让父流程可以看到能够发起那些流程数据。*/
                AtPara ap = new AtPara(tag);
                BP.WF.GenerWorkFlow gwf = new GenerWorkFlow(ap.GetValInt64ByKey(GenerWorkFlowAttr.PWorkID));
                t.WorkID = gwf.WorkID;

                nd = new Node(gwf.FK_Node);
                t.NDFrom = gwf.FK_Node;
                t.NDFromT = nd.Name;

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
        public static void WriteTrackInfo(string flowNo, int nodeFrom, Int64 workid, Int64 fid, string msg, string optionMsg)
        {
            WriteTrack(flowNo, nodeFrom, workid, fid, msg, ActionType.Info, null, null, optionMsg);
        }
        /// <summary>
        /// 写入工作审核日志:
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="currNodeID">当前节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="FID">FID</param>
        /// <param name="msg">审核信息</param>
        /// <param name="optionName">操作名称(比如:科长审核、部门经理审批),如果为空就是"审核".</param>
        public static void WriteTrackWorkCheck(string flowNo, int currNodeID, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            WorkNode wn = new WorkNode(workid, currNodeID);
            //待办抢办模式，一个节点只能有一条记录
            Paras ps = new Paras();
            if (wn.HisNode.TodolistModel == TodolistModel.QiangBan)
            {
                //先删除
                ps.SQL = "DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE  WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.NDFrom, currNodeID);
                DBAccess.RunSQL(ps);
                //写入日志
                WriteTrack(flowNo, currNodeID, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
            }
            else
            {
                ps.SQL = "UPDATE  ND" + int.Parse(flowNo) + "Track SET Msg=" + dbStr + "Msg WHERE  WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND ActionType=" + (int)ActionType.WorkCheck;
                ps.Add(TrackAttr.Msg, msg);
                ps.Add(TrackAttr.WorkID, workid);
                ps.Add(TrackAttr.NDFrom, currNodeID);
                ps.Add(TrackAttr.EmpFrom, WebUser.No);

                if (DBAccess.RunSQL(ps) == 0)
                {
                    //如果没有更新到，就写入.
                    WriteTrack(flowNo, currNodeID, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
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
        public static void WriteTrackDailyLog(string flowNo, int nodeFrom, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string today = BP.DA.DataType.CurrentData;

            Paras ps = new Paras();
            ps.SQL = "UPDATE  ND" + int.Parse(flowNo) + "Track SET Msg=" + dbStr + "Msg WHERE  RDT LIKE '" + today + "%' AND WorkID=" + dbStr + "WorkID  AND NDFrom=" + dbStr + "NDFrom AND EmpFrom=" + dbStr + "EmpFrom AND ActionType=" + (int)ActionType.WorkCheck;
            ps.Add(TrackAttr.Msg, msg);
            ps.Add(TrackAttr.WorkID, workid);
            ps.Add(TrackAttr.NDFrom, nodeFrom);
            ps.Add(TrackAttr.EmpFrom, WebUser.No);
            if (DBAccess.RunSQL(ps) == 0)
            {
                //如果没有更新到，就写入.
                WriteTrack(flowNo, nodeFrom, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
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
        public static void WriteTrackWeekLog(string flowNo, int nodeFrom, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;

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
                WriteTrack(flowNo, nodeFrom, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
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
        public static void WriteTrackMonthLog(string flowNo, int nodeFrom, Int64 workid, Int64 fid, string msg, string optionName)
        {
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            string today = BP.DA.DataType.CurrentData;

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
                WriteTrack(flowNo, nodeFrom, workid, fid, msg, ActionType.WorkCheck, null, null, optionName);
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
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return false;

            string pk = dt.Rows[0][0].ToString();
            sql = "UPDATE " + table + " SET " + TrackAttr.ActionTypeText + "='" + label + "' WHERE MyPK=" + pk;
            BP.DA.DBAccess.RunSQL(sql);
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
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return false;

            string pk = dt.Rows[0][0].ToString();
            sql = "UPDATE " + table + " SET " + TrackAttr.ActionTypeText + "='" + label + "' WHERE MyPK=" + pk;
            BP.DA.DBAccess.RunSQL(sql);
            return true;
        }
        /// <summary>
        /// 获取Track 表中的审核的信息
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workId"></param>
        /// <param name="nodeFrom"></param>
        /// <returns></returns>
        public static string GetCheckInfo(string flowNo, Int64 workId, int nodeFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT Msg FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.WorkCheck + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + workId + " ORDER BY RDT DESC ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
        /// <returns></returns>
        public static void DeleteCheckInfo(string flowNo, Int64 workId, int nodeFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "DELETE FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.WorkCheck + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + workId;
            BP.DA.DBAccess.RunSQL(sql);
        }
        public static string GetAskForHelpReInfo(string flowNo, Int64 workId, int nodeFrom)
        {
            string table = "ND" + int.Parse(flowNo) + "Track";
            string sql = "SELECT Msg FROM " + table + " WHERE NDFrom=" + nodeFrom + " AND ActionType=" + (int)ActionType.AskforHelp + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + workId + " ORDER BY RDT DESC ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return "";
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

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + table + " SET Msg=" + dbstr + "Msg  WHERE ActionType=" + dbstr +
                "ActionType and WorkID=" + dbstr + "WorkID and NDFrom=" + dbstr + "NDFrom";
            ps.Add("Msg", strMsg);
            ps.Add("ActionType", actionType);
            ps.Add("WorkID", workId);
            ps.Add("NDFrom", nodeFrom);
            BP.DA.DBAccess.RunSQL(ps);
        }

        /// <summary>
        /// 设置BillNo信息
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="workID"></param>
        /// <param name="newBillNo"></param>
        public static void SetBillNo(string flowNo, Int64 workID, string newBillNo)
        {
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET BillNo=" + dbstr + "BillNo  WHERE WorkID=" + dbstr + "WorkID";
            ps.Add("BillNo", newBillNo);
            ps.Add("WorkID", workID);
            BP.DA.DBAccess.RunSQL(ps);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET BillNo=" + dbstr + "BillNo WHERE OID=" + dbstr + "OID";
            ps.Add("BillNo", newBillNo);
            ps.Add("OID", workID);
            BP.DA.DBAccess.RunSQL(ps);
        }

        /// <summary>
        /// 设置父流程信息 
        /// </summary>
        /// <param name="subFlowNo">子流程编号</param>
        /// <param name="subFlowWorkID">子流程workid</param>
        /// <param name="parentFlowNo">父流程编号</param>
        /// <param name="parentWorkID">父流程WorkID</param>
        /// <param name="parentNodeID">调用子流程的节点ID</param>
        /// <param name="parentEmpNo">调用人</param>
        public static void SetParentInfo(string subFlowNo, Int64 subFlowWorkID, string parentFlowNo, Int64 parentWorkID,
            int parentNodeID, string parentEmpNo)
        {
            if (parentWorkID == 0)
                throw new Exception("@设置的父流程的流程编号为0，这是非法的。");

            if (string.IsNullOrEmpty(parentEmpNo))
                parentEmpNo = WebUser.No;

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET PFlowNo=" + dbstr + "PFlowNo, PWorkID=" + dbstr + "PWorkID,PNodeID=" + dbstr + "PNodeID,PEmp=" + dbstr + "PEmp WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.PFlowNo, parentFlowNo);
            ps.Add(GenerWorkFlowAttr.PWorkID, parentWorkID);
            ps.Add(GenerWorkFlowAttr.PNodeID, parentNodeID);
            ps.Add(GenerWorkFlowAttr.PEmp, parentEmpNo);
            ps.Add(GenerWorkFlowAttr.WorkID, subFlowWorkID);

            BP.DA.DBAccess.RunSQL(ps);

            Flow fl = new Flow(subFlowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET PFlowNo=" + dbstr + "PFlowNo, PWorkID=" + dbstr + "PWorkID,PNodeID=" + dbstr + "PNodeID, PEmp=" + dbstr + "PEmp WHERE OID=" + dbstr + "OID";
            ps.Add(NDXRptBaseAttr.PFlowNo, parentFlowNo);
            ps.Add(NDXRptBaseAttr.PWorkID, parentWorkID);
            ps.Add(NDXRptBaseAttr.PNodeID, parentNodeID);
            ps.Add(NDXRptBaseAttr.PEmp, parentEmpNo);

            ps.Add(NDXRptBaseAttr.OID, subFlowWorkID);
            BP.DA.DBAccess.RunSQL(ps);
        }

        public static GERpt Flow_GenerGERpt(string flowNo, Int64 workID)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt", workID);
            return rpt;
        }
        /// <summary>
        /// 产生一个新的工作ID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>返回当前操作员创建的工作ID</returns>
        public static Int64 Flow_GenerWorkID(string flowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            Flow fl = new Flow(flowNo);
            return fl.NewWork().OID;
        }
        /// <summary>
        /// 产生一个新的工作
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>返回当前操作员创建的工作</returns>
        public static Work Flow_GenerWork(string flowNo)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

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
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
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
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否要删除它的子流程</param>
        /// <returns>执行信息</returns>
        public static string Flow_DoDeleteFlowByReal(string flowNo, Int64 workID, bool isDelSubFlow = false)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);
            try
            {
                WorkFlow.DeleteFlowByReal(flowNo, workID, isDelSubFlow);
                // WorkFlow wf = new WorkFlow(flowNo, workID);
                //wf.DoDeleteWorkFlowByReal(isDelSubFlow);
            }
            catch (Exception ex)
            {
                throw new Exception("@删除前错误，" + ex.StackTrace);
            }
            return "删除成功";
        }
        public static string Flow_DoDeleteDraft(string flowNo, Int64 workID, bool isDelSubFlow)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "DELETE WF_GenerWorkFlow WHERE WorkID=" + dbstr + "WorkID";
            ps.Add("WorkID", workID);
            BP.DA.DBAccess.RunSQL(ps);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "DELETE " + fl.PTable + " WHERE OID=" + dbstr + "OID";
            ps.Add("OID", workID);
            BP.DA.DBAccess.RunSQL(ps);

            //删除开始节点数据.
            Node nd = fl.HisStartNode;
            Work wk = nd.HisWork;
            ps = new Paras();
            ps.SQL = "DELETE " + wk.EnMap.PhysicsTable + " WHERE OID=" + dbstr + "OID";
            ps.Add("OID", workID);
            BP.DA.DBAccess.RunSQL(ps);

            BP.DA.Log.DefaultLogWriteLineInfo(WebUser.Name + "删除了FlowNo 为'" + flowNo + "',workID为'" + workID + "'的数据");

            return "删除成功";
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
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);
            WorkFlow wf = new WorkFlow(flowNo, workID);
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
        public static string Flow_DoDeleteFlowByFlag(string flowNo, Int64 workID, string msg, bool isDelSubFlow)
        {
            //转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            wf.DoDeleteWorkFlowByFlag(msg);
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workID);
                foreach (GenerWorkFlow item in gwfs)
                {
                    Flow_DoDeleteFlowByFlag(item.FK_Flow, item.WorkID, "删除子流程:" + msg, false);
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
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
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
        public static string Flow_DoUnSend(string flowNo, Int64 workID)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            WorkUnSend unSend = new WorkUnSend(flowNo, workID);
            return unSend.DoUnSend();
        }
        /// <summary>
        /// 执行冻结
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">workid</param>
        /// <param name="msg">冻结原因</param>
        public static string Flow_DoFix(string flowNo, Int64 workid, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            // 执行冻结.
            WorkFlow wf = new WorkFlow(flowNo, workid);
            return wf.DoFix(msg);
        }
        /// <summary>
        /// 执行解除冻结
        /// 于挂起的区别是,冻结需要有权限的人才可以执行解除冻结，
        /// 挂起是自己的工作可以挂起也可以解除挂起。
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">workid</param>
        /// <param name="msg">解除原因</param>
        public static string Flow_DoUnFix(string flowNo, Int64 workid, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            // 执行冻结.
            WorkFlow wf = new WorkFlow(flowNo, workid);
            return wf.DoUnFix(msg);
        }
        /// <summary>
        /// 执行流程结束
        /// 说明:正常的流程结束.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">流程结束原因</param>
        /// <returns>返回成功执行信息</returns>
        public static string Flow_DoFlowOver(string flowNo, Int64 workID, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            WorkFlow wf = new WorkFlow(flowNo, workID);
            return wf.DoFlowOver(ActionType.FlowOver, msg, null, null);
        }
        /// <summary>
        /// 执行流程结束:强制的流程结束.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="flowNo">当前节点编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">工作ID</param>
        /// <param name="msg">强制流程结束的原因</param>
        /// <returns>执行强制结束流程</returns>
        public static string Flow_DoFlowOverByCoercion(string flowNo, int nodeid, Int64 workID, Int64 fid, string msg)
        {
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);
            WorkFlow wf = new WorkFlow(flowNo, workID);

            Node currND = new Node(nodeid);

            Flow fl = new Flow(flowNo);
            GERpt rpt = fl.HisGERpt;
            rpt.OID = workID;
            rpt.RetrieveFromDBSources();

            string s = wf.DoFlowOver(ActionType.FlowOverByCoercion, msg, currND, rpt);
            if (string.IsNullOrEmpty(s))
                s = "流程已经成功结束.";
            return s;
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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

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
            ps.SQL = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            return BP.DA.DBAccess.RunSQLReturnValInt(ps);
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
        /// <returns>返回当前处理人员列表,数据结构与WF_GenerWorkerList一致.</returns>
        public static DataTable Flow_GetWorkerList(Int64 workID)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM WF_GenerWorkerList WHERE IsEnable=1 AND IsPass=0 AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
            ps.Add("WorkID", workID);
            return BP.DA.DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 检查是否可以发起流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>是否可以发起当前流程</returns>
        public static bool Flow_IsCanStartThisFlow(string flowNo, string userNo)
        {
            Node nd = new Node(int.Parse(flowNo + "01"));
            if (nd.IsGuestNode == true)
            {
                if (BP.Web.WebUser.No != "Guest")
                    throw new Exception("@当前节点是来宾处理节点，但是目前您{"+BP.Web.WebUser.No+"}不是来宾帐号。");
                return true;
            }

            Paras ps = new Paras();
            string dbstr = SystemConfig.AppCenterDBVarStr;
            int num = 0;
            if (SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                        ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeStation A, " + BP.WF.Glo.EmpStation + " B WHERE A.FK_Station= B.FK_Station AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp";
                        ps.Add("FK_Node", nd.NodeID);
                        ps.Add("FK_Emp", userNo);
                        num = DBAccess.RunSQLReturnValInt(ps);
                        break;
                    case DeliveryWay.ByDept:
                        ps.SQL = "SELECT COUNT(A.FK_Node) as Num FROM WF_NodeDept A, " + BP.WF.Glo.EmpDept + " B WHERE A.FK_Dept= B.FK_Dept AND  A.FK_Node=" + dbstr + "FK_Node AND B.FK_Emp=" + dbstr + "FK_Emp";
                        ps.Add("FK_Node", nd.NodeID);
                        ps.Add("FK_Emp", userNo);
                        num = DBAccess.RunSQLReturnValInt(ps);
                        break;
                    case DeliveryWay.ByBindEmp:
                        ps.SQL = "SELECT COUNT(*) AS Num FROM WF_NodeEmp WHERE FK_Emp=" + dbstr + "FK_Emp AND FK_Node=" + dbstr + "FK_Node";
                        ps.Add("FK_Emp", userNo);
                        ps.Add("FK_Node", nd.NodeID);
                        num = DBAccess.RunSQLReturnValInt(ps);
                        break;
                    default:
                        throw new Exception("@开始节点不允许设置此访问规则：" + nd.HisDeliveryWay);
                }
            }
            else
            {
                switch (nd.HisDeliveryWay)
                {
                    case DeliveryWay.ByStation:
                        var obj = BP.DA.DataType.GetPortalInterfaceSoapClientInstance();
                        DataTable mydt = obj.GetEmpHisStations(BP.Web.WebUser.No);
                        string mystas = BP.DA.DBAccess.GenerWhereInPKsString(mydt);
                        ps.SQL = "SELECT COUNT(FK_Node) AS Num FROM WF_NodeStation WHERE FK_Node=" + dbstr + "FK_Node AND FK_Station IN(" + mystas + ")";
                        ps.Add("FK_Node", nd.NodeID);
                        num = DBAccess.RunSQLReturnValInt(ps);
                        break;
                    case DeliveryWay.ByDept:
                        var objMy = BP.DA.DataType.GetPortalInterfaceSoapClientInstance();
                        DataTable mydtDept = objMy.GetEmpHisDepts(BP.Web.WebUser.No);
                        string dtps = BP.DA.DBAccess.GenerWhereInPKsString(mydtDept);

                        ps.SQL = "SELECT COUNT(FK_Node) as Num FROM WF_NodeDept WHERE FK_Dept IN (" + dtps + ") B.FK_Dept AND  A.FK_Node=" + dbstr + "FK_Node";
                        ps.Add("FK_Node", nd.NodeID);
                        num = DBAccess.RunSQLReturnValInt(ps);
                        break;
                    case DeliveryWay.ByBindEmp:
                        ps.SQL = "SELECT COUNT(*) AS Num FROM WF_NodeEmp WHERE FK_Emp=" + dbstr + "FK_Emp AND FK_Node=" + dbstr + "FK_Node";
                        ps.Add("FK_Emp", userNo);
                        ps.Add("FK_Node", nd.NodeID);
                        num = DBAccess.RunSQLReturnValInt(ps);
                        break;
                    default:
                        throw new Exception("@开始节点不允许设置此访问规则：" + nd.HisDeliveryWay);
                }
            }
            if (num == 0)
                return false;
            return true;
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
                return false;

            string sql = "select * from WF_Generworkflow where WorkID='" + workID + "'";


            return DBAccess.RunSQLReturnCOUNT(sql) > 0;
        }
        /// <summary>
        /// 是否可以处理当前工作
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="userNo">用户编号</param>
        /// <returns>是否可以处理当前工作</returns>
        public static bool Flow_IsCanDoCurrentWork(string fk_flow, Int64 workID, string userNo)
        {
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workID);
                return Flow_IsCanDoCurrentWork(fk_flow, gwf.FK_Node, workID, userNo);
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 检查是否可以处理当前的工作？
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="userNo">操作员编号</param>
        /// <returns>是否可以处理当前的工作</returns>
        public static bool Flow_IsCanDoCurrentWork(int nodeID, Int64 workID, string userNo)
        {
            Node nd = new Node(nodeID);
            return Flow_IsCanDoCurrentWork(nd.FK_Flow, nodeID, workID, userNo);
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
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT COUNT(WorkID) FROM WF_GenerWorkFlow WHERE  PNodeID=" + dbstr + "PNodeID AND PWorkID=" + dbstr + "PWorkID AND WFState!=" + dbstr + "WFState ";
            ps.Add(GenerWorkFlowAttr.PNodeID, nodeID);
            ps.Add(GenerWorkFlowAttr.PWorkID, workID);
            ps.Add(GenerWorkFlowAttr.WFState, (int)WFState.Complete);

            if (BP.DA.DBAccess.RunSQLReturnValInt(ps) == 0)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 检查当前人员是否有权限处理当前的工作.
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="userNo">要判断的操作人员</param>
        /// <returns>返回指定的人员是否有操作当前工作的权限</returns>
        public static bool Flow_IsCanDoCurrentWork(string fk_flow, int nodeID, Int64 workID, string userNo)
        {
            if (workID == 0)
                return true;

            if (userNo == "admin")
                return true;


            #region 判断是否是开始节点.
            /* 判断是否是开始节点 . */
            string str = nodeID.ToString();
            int len = str.Length - 2;
            if (str.Substring(len, 2) == "01")
                return true;
            #endregion 判断是否是开始节点.

            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT c.RunModel,c.IsGuestNode, a.GuestNo, a.TaskSta, a.WFState, IsPass FROM WF_GenerWorkFlow a, WF_GenerWorkerlist b, WF_Node c WHERE a.FK_Node=" + dbstr + "FK_Node  AND b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.FK_Emp=" + dbstr + "FK_Emp AND b.IsEnable=1 AND a.WorkID=" + dbstr + "WorkID ";
            ps.Add("FK_Node",nodeID);
            ps.Add("FK_Emp", userNo);
            ps.Add("WorkID",workID);
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
                return false;


            //判断是否是待办.
            int isPass = int.Parse(dt.Rows[0]["IsPass"].ToString());
            if (isPass != 0)
                return false;

            WFState wfsta = (WFState)int.Parse(dt.Rows[0]["WFState"].ToString());
            if (wfsta == WFState.Complete)
                return false;
            if (wfsta == WFState.Delete)
                return false;

            //判断是否是客户处理节点. 
            int isGuestNode = int.Parse(dt.Rows[0]["IsGuestNode"].ToString());
            if (isGuestNode == 1)
            {
                if (dt.Rows[0]["GuestNo"].ToString() == BP.Web.GuestUser.No)
                    return true;
                else
                    return false;
            }


            int i = int.Parse(dt.Rows[0][0].ToString());
            //TaskSta TaskStai = (TaskSta)int.Parse(dt.Rows[0][1].ToString());
            //if (TaskStai == TaskSta.Sharing)
            //    return false; /*如果是共享状态，没有申请下来，就不能审批.*/

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
                case RunModel.SubThread:
                    return true;
                default:
                    break;
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

            if (userNo == "admin")
                return true;

            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            //ps.SQL = "SELECT c.RunModel FROM WF_GenerWorkFlow a , WF_GenerWorkerlist b, WF_Node c WHERE a.FK_Node=" + dbstr + "FK_Node AND b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.FK_Emp=" + dbstr + "FK_Emp AND b.IsEnable=1 AND a.workid=" + dbstr + "WorkID";
            //ps.Add("FK_Node", nodeID);
            //ps.Add("FK_Emp", userNo);
            //ps.Add("WorkID", workID);
            string sql = "SELECT c.RunModel, a.TaskSta FROM WF_GenerWorkFlow a , WF_GenerWorkerlist b, WF_Node c WHERE a.FK_Node='" + nodeID + "'  AND b.FK_Node=c.NodeID AND a.WorkID=b.WorkID AND a.FK_Node=b.FK_Node  AND b.GuestNo='" + userNo + "' AND b.IsEnable=1 AND a.workid='" + workID + "'";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return false;

            int i = int.Parse(dt.Rows[0][0].ToString());
            TaskSta TaskStai = (TaskSta)int.Parse(dt.Rows[0][1].ToString());
            if (TaskStai == TaskSta.Sharing)
                return false;

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
                case RunModel.SubThread:
                    return true;
                default:
                    break;
            }

            if (DBAccess.RunSQLReturnValInt(ps) == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 是否可以查看流程数据
        /// 用于判断是否可以查看流程轨迹图.
        /// edit: stone 2015-03-25
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <returns></returns>
        public static bool Flow_IsCanViewTruck(string flowNo, Int64 workid, Int64 fid)
        {
            if (WebUser.No == "admin")
                return true;

            //先从轨迹里判断.
            string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "SELECT count(MyPK) as Num FROM ND" + int.Parse(flowNo) + "Track WHERE (WorkID=" + dbStr + "WorkID OR FID=" + dbStr + "FID) AND (EmpFrom=" + dbStr + "Emp1 OR EmpTo=" + dbStr + "Emp2)";
            ps.Add(BP.WF.TrackAttr.WorkID, workid);
            ps.Add(BP.WF.TrackAttr.FID, workid);
            ps.Add("Emp1", WebUser.No);
            ps.Add("Emp2", WebUser.No);

            if (BP.DA.DBAccess.RunSQLReturnValInt(ps) > 1)
                return true;

            //在查看该流程的发起者，与当前人是否在同一个部门，如果是也返回true.
            ps = new Paras();
            ps.SQL = "SELECT FK_Dept FROM WF_GenerWorkFlow WHERE WorkID=" + dbStr + "WorkID OR WorkID=" + dbStr + "FID";
            ps.Add(BP.WF.TrackAttr.WorkID, workid);
            ps.Add(BP.WF.TrackAttr.FID, workid);

            string fk_dept = BP.DA.DBAccess.RunSQLReturnStringIsNull(ps, null);
            if (fk_dept == null)
            {
                BP.WF.Flow fl = new Flow(flowNo);
                ps.SQL = "SELECT FK_Dept FROM " + fl.PTable + " WHERE OID=" + dbStr + "WorkID OR OID=" + dbStr + "FID";
                fk_dept = BP.DA.DBAccess.RunSQLReturnStringIsNull(ps, null);
                if (fk_dept == null)
                    throw new Exception("@流程引擎数据被删除.");
            }
            if (BP.Web.WebUser.FK_Dept == fk_dept)
                return true;

            return false;
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">子线程的工作ID</param>
        /// <param name="info">删除信息</param>
        public static void Flow_DeleteSubThread(string flowNo, Int64 workid, string info)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workid);

            WorkFlow wf = new WorkFlow(flowNo, workid);
            wf.DoDeleteWorkFlowByReal(false);
            BP.WF.Dev2Interface.WriteTrackInfo(flowNo, gwf.FK_Node, gwf.FID, 0, info, "删除子线程");
        }
        /// <summary>
        /// 执行工作催办
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="msg">催办消息</param>
        /// <param name="isPressSubFlow">是否催办子流程？</param>
        /// <returns>返回执行结果</returns>
        public static string Flow_DoPress(Int64 workID, string msg, bool isPressSubFlow)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);

            /*找到当前待办的工作人员*/
            GenerWorkerLists wls = new GenerWorkerLists(workID, gwf.FK_Node);
            string toEmp = "", toEmpName = "";
            string mailTitle = "催办:" + gwf.Title + ", 发送人:" + WebUser.Name;
            //如果子线程找不到流转日志并且父流程编号不为空，在父流程进行查找接收人
            if (wls.Count == 0 && gwf.FID != 0)
                wls = new GenerWorkerLists(gwf.FID, gwf.FK_Node);

            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue;

                toEmp += wl.FK_Emp + ",";
                toEmpName += wl.FK_EmpText + ",";

                // 发消息.
                BP.WF.Dev2Interface.Port_SendMsg(wl.FK_Emp, mailTitle, msg, null, BP.WF.SMSMsgType.DoPress, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

                wl.PressTimes = wl.PressTimes + 1;
                wl.Update();

                //wl.Update(GenerWorkerListAttr.PressTimes, wl.PressTimes + 1);
            }

            //写入日志.
            WorkNode wn = new WorkNode(workID, gwf.FK_Node);
            wn.AddToTrack(ActionType.Press, toEmp, toEmpName, gwf.FK_Node, gwf.NodeName, msg);

            //如果催办子流程.
            if (isPressSubFlow)
            {
                string subMsg = "";
                GenerWorkFlows gwfs = gwf.HisSubFlowGenerWorkFlows;
                foreach (GenerWorkFlow item in gwfs)
                {
                    subMsg += "@已经启动对子线程:" + item.Title + "的催办,消息如下:";
                    subMsg += Flow_DoPress(item.WorkID, msg, false);
                }
                return "系统已经把您的信息通知给:" + toEmpName + "" + subMsg;
            }
            else
            {
                return "系统已经把您的信息通知给:" + toEmpName;
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
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();
            Flow fl = nd.HisFlow;
            string title = BP.WF.WorkNode.GenerTitle(fl, wk);
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
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
                throw new Exception("创建流程ID不存在.");

            string[] strs = paras.Split('@');
            foreach (string item in strs)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                //GroupMark=xxxx
                string[] mystr = item.Split('=');
                gwf.SetPara(mystr[0], mystr[1]);
            }
            gwf.Update();
            return true;
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
            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);


            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET Title=" + dbstr + "Title WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.Title, title);
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            DBAccess.RunSQL(ps);

            Flow fl = new Flow(flowNo);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET Title=" + dbstr + "Title WHERE OID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.Title, title);
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            int num = DBAccess.RunSQL(ps);

            if (fl.HisDataStoreModel == DataStoreModel.ByCCFlow)
            {
                ps = new Paras();
                ps.SQL = "UPDATE ND" + int.Parse(flowNo + "01") + " SET Title=" + dbstr + "Title WHERE OID=" + dbstr + "WorkID";
                ps.Add(GenerWorkFlowAttr.Title, title);
                ps.Add(GenerWorkFlowAttr.WorkID, workid);
                DBAccess.RunSQL(ps);
            }

            if (num == 0)
                return false;
            else
                return true;
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
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;

            Node nd = new Node(toNodeID);
            Emp emp = new Emp(toEmper);

            // 找到GenerWorkFlow,并执行更新.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.WFState = WFState.Runing;
            gwf.TaskSta = TaskSta.None;
            gwf.TodoEmps = toEmper;
            gwf.FK_Node = toNodeID;
            gwf.NodeName = nd.Name;
            //gwf.StarterName =emp.Name;
            gwf.Update();

            //让其都设置完成。
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=1 WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WorkID, workid);
            BP.DA.DBAccess.RunSQL(ps);

            // 更新流程数据信息。
            Flow fl = new Flow(gwf.FK_Flow);
            ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET FlowEnder=" + dbstr + "FlowEnder,FlowEndNode=" + dbstr + "FlowEndNode WHERE OID=" + dbstr + "OID";
            ps.Add(NDXRptBaseAttr.FlowEnder, toEmper);
            ps.Add(NDXRptBaseAttr.FlowEndNode, toNodeID);
            ps.Add(NDXRptBaseAttr.OID, workid);
            BP.DA.DBAccess.RunSQL(ps);

            // 执行更新.
            GenerWorkerLists gwls = new GenerWorkerLists(workid);
            GenerWorkerList gwl = gwls[gwls.Count - 1] as GenerWorkerList; //获得最后一个。
            gwl.RDT = DataType.CurrentDataTime;
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
            gwl.HungUpTimes = 0;
            gwl.FID = gwf.FID;
            gwl.FK_Dept = emp.FK_Dept;

            if (gwl.Update() == 0)
                gwl.Insert();

            string sql = "SELECT COUNT(*) FROM WF_EmpWorks where WorkID=" + workid + " and fk_emp='" + toEmper + "'";
            int i = BP.DA.DBAccess.RunSQLReturnValInt(sql);
            if (i == 0)
                throw new Exception("@调度错误");

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
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_TransferCustom WHERE WorkID=" + workid);

            //保存参数.
            // 参数格式为  @104`SubFlow002`1`zhangsan,lisi`wangwu,chenba`王五,陈八@......
            string[] strs = paras.Split('@');
            int idx = 0, cidx = 0;
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                if (str.Contains("`") == false)
                    continue;

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
                tc.MyPK = tc.FK_Node + "_" + tc.WorkID + "_" + idx;
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
                        continue;

                    sa = new SelectAccper();
                    sa.MyPK = nodeid + "_" + workid + "_" + ccs[i];
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
                gwf.WFSta = WFSta.Runing;
                gwf.WFState = WFState.Blank;

                gwf.Starter = WebUser.No;
                gwf.StarterName = WebUser.Name;

                gwf.FK_Flow = flowNo;
                BP.WF.Flow fl = new Flow(flowNo);
                gwf.FK_FlowSort = fl.FK_FlowSort;
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
                gwf.WFSta = WFSta.Runing;
                gwf.WFState = WFState.Blank;

                gwf.Starter = WebUser.No;
                gwf.StarterName = WebUser.Name;

                gwf.FK_Flow = flowNo;
                BP.WF.Flow fl = new Flow(flowNo);
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.FK_Dept = WebUser.FK_Dept;

                gwf.TransferCustomType = runType;
                gwf.Insert();
                return;
            }
            gwf.TransferCustomType = runType;
            gwf.Update();
            if (runType == TransferCustomType.ByCCBPMDefine)
                return;  // 如果是按照设置的模式运行，就要更改状态后退出它.
            #endregion 

            //删除以前存储的参数.
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_TransferCustom WHERE WorkID=" + workid);

            //保存参数.
            // 参数格式为 格式为:@节点编号1;处理人员1,处理人员2,处理人员n;应处理时间(可选)
            // 例如1: @101;zhangsan,lisi,wangwu;2016-05-12;@102;liming,xiaohong,xiaozhang;2016-05-12
            // 例如2: @101;zhangsan,lisi,wangwu;@102;liming,xiaohong,xiaozhang;2016-05-12

            string[] strs = paras.Split('@');
            int idx = 0, cidx = 0;
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                if (str.Contains(";") == false)
                    continue;

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
                tc.MyPK = tc.FK_Node + "_" + tc.WorkID + "_" + idx;
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
                        continue;

                    sa = new SelectAccper();
                    sa.MyPK = nodeid + "_" + workid + "_" + ccs[i];
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

        #region 与流程有关的接口
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
            string paras = gwf.GetValStrByKey("AtPara");

            if (paras.Contains("F_" + WebUser.No + "=0") == false)
                gwf.SetPara("F_" + WebUser.No, "1");
            else
                gwf.SetPara("F_" + WebUser.No, "0");

            gwf.DirectUpdate();
        }

        /// <summary>
        /// 设置委托
        /// </summary>
        /// <param name="Author">接收委托人账号</param>
        /// <param name="AuthorWay">委托方式：0不授权， 1完全授权，2，指定流程范围授权. </param>
        /// <param name="AuthorFlows">委托流程编号，格式：001,002,003</param>
        /// <param name="AuthorDate">委托开始时间，默认当前时间</param>
        /// <param name="AuthorToDate">委托结束时间</param>
        /// <returns>设置结果：成功true,失败 false</returns>
        public static bool Flow_AuthorSave(string Author, int AuthorWay, string AuthorFlows = null, string AuthorDate = null, string AuthorToDate = null)
        {
            if (WebUser.No == null)
                throw new Exception("@ 非法用户，请执行登录后再试。");

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(WebUser.No);
            emp.Author = Author;
            emp.AuthorWay = AuthorWay;
            emp.AuthorDate = BP.DA.DataType.CurrentData;

            if (!string.IsNullOrEmpty(AuthorFlows))
                emp.AuthorFlows = AuthorFlows;
            if (!string.IsNullOrEmpty(AuthorDate))
                emp.AuthorFlows = AuthorDate;
            if (!string.IsNullOrEmpty(AuthorToDate))
                emp.AuthorToDate = AuthorToDate;
            int i = emp.Save();

            return i >= 0 ? true : false;
        }
        /// <summary>
        /// 取消委托当前登录人的委托信息
        /// </summary>
        /// <returns></returns>
        public static bool Flow_AuthorCancel()
        {
            if (WebUser.No == null)
                throw new Exception("@ 非法用户，请执行登录后再试。");

            BP.WF.Port.WFEmp myau = new BP.WF.Port.WFEmp(WebUser.No);
            BP.DA.Log.DefaultLogWriteLineInfo("取消授权:" + WebUser.No + "取消了对(" + myau.Author + ")的授权。");
            myau.Author = "";
            myau.AuthorWay = 0;
            myau.AuthorDate = "";
            myau.AuthorToDate = "";
            int i = myau.Update();
            return i >= 0 ? true : false;
        }
        /// <summary>
        /// 获取当前登录人的委托人
        /// </summary>
        /// <returns></returns>
        public static DataTable DB_AuthorEmps()
        {
            if (WebUser.No == null)
                throw new Exception("@ 非法用户，请执行登录后再试。");
            return BP.DA.DBAccess.RunSQLReturnTable("SELECT * FROM WF_EMP WHERE AUTHOR='" + WebUser.No + "'");
        }
        /// <summary>
        /// 获取委托给当前登录人的流程待办信息
        /// </summary>
        /// <param name="empNo">授权人员编号</param>
        /// <returns></returns>
        public static DataTable DB_AuthorEmpWorks(string empNo)
        {
            if (WebUser.No == null)
                throw new Exception("@ 非法用户，请执行登录后再试。");

            WF.Port.WFEmp emp = new Port.WFEmp(empNo);
            if (!string.IsNullOrEmpty(emp.Author) && emp.Author == WebUser.No && emp.AuthorIsOK == true)
            {
                string sql = "";
                string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + "  OR WFState=" + (int)WFState.AskForReplay + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + " OR WFState=" + (int)WFState.Fix;
                switch (emp.HisAuthorWay)
                {
                    case Port.AuthorWay.All:
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' AND TaskSta!=1  ORDER BY ADT DESC";
                        else
                            sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' ORDER BY ADT DESC";
                        break;
                    case Port.AuthorWay.SpecFlows:
                        if (BP.WF.Glo.IsEnableTaskPool == true)
                            sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' AND  FK_Flow IN " + emp.AuthorFlows + " AND TaskSta!=0    ORDER BY ADT DESC";
                        else
                            sql = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp='" + emp.No + "' AND  FK_Flow IN " + emp.AuthorFlows + "   ORDER BY ADT DESC";
                        break;
                }
                return BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
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
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
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
            // 给全局变量赋值.
            BP.WF.Glo.SendHTOfTemp = htWork;

            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            // 父流程编号.
            parentFlowNo = TurnFlowMarkToFlowNo(parentFlowNo);
            Flow fl = new Flow(flowNo);
            Work wk = fl.NewWork();
            Int64 workID = wk.OID;
            if (htWork != null)
            {
                foreach (string str in htWork.Keys)
                {
                    switch (str)
                    {
                        case StartWorkAttr.OID:
                        case StartWorkAttr.CDT:
                        case StartWorkAttr.MD5:
                        case StartWorkAttr.Emps:
                        case StartWorkAttr.FID:
                        case StartWorkAttr.FK_Dept:
                        case StartWorkAttr.PRI:
                        case StartWorkAttr.Rec:
                        case StartWorkAttr.Title:
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

            WorkNode wn = new WorkNode(wk, fl.HisStartNode);

            Node nextNoode = null;
            if (nextNodeID != 0)
                nextNoode = new Node(nextNodeID);

            SendReturnObjs objs = wn.NodeSend(nextNoode, nextWorker);
            if (parentWorkID != 0)
                DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET PWorkID=" + parentWorkID + ",PFlowNo='" + parentFlowNo + "' WHERE WorkID=" + objs.VarWorkID);

            #region 更新发送参数.
            if (htWork != null)
            {
                string paras = "";
                foreach (string key in htWork.Keys)
                    paras += "@" + key + "=" + htWork[key].ToString();

                if (string.IsNullOrEmpty(paras) == false)
                {
                    string dbstr = SystemConfig.AppCenterDBVarStr;
                    Paras ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist set AtPara=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
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
        /// <returns>为开始节点创建工作后产生的WorkID.</returns>
        public static Int64 Node_CreateBlankWork(string flowNo, Hashtable ht = null, DataSet workDtls = null,
            string starter = null, string title = null, Int64 parentWorkID = 0, Int64 parentFID = 0, string parentFlowNo = null, int parentNodeID = 0, string parentEmp = null,
            int jumpToNode = 0, string jumpToEmp = null)
        {
            //把一些其他的参数也增加里面去,传递给ccflow.
            Hashtable htPara = new Hashtable();
            if (parentWorkID != 0)
                htPara.Add(StartFlowParaNameList.PWorkID, parentWorkID);
            if (parentFID != 0)
                htPara.Add(StartFlowParaNameList.PFID, parentFID);

            if (parentFlowNo != null)
                htPara.Add(StartFlowParaNameList.PFlowNo, parentFlowNo);
            if (parentNodeID != 0)
                htPara.Add(StartFlowParaNameList.PNodeID, parentNodeID);
            if (parentEmp != null)
                htPara.Add(StartFlowParaNameList.PEmp, parentEmp);

            // 给全局变量赋值.
            BP.WF.Glo.SendHTOfTemp = ht;

            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            //转化成编号
            parentFlowNo = TurnFlowMarkToFlowNo(parentFlowNo);

            if (parentFlowNo == null)
                parentFlowNo = "";

            string dbstr = SystemConfig.AppCenterDBVarStr;

            if (string.IsNullOrEmpty(starter))
                starter = WebUser.No;

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);

            // 下一个工作人员。
            Emp empStarter = new Emp(starter);
            Work wk = fl.NewWork(empStarter, htPara);
            Int64 workID = wk.OID;

            #region 给各个属性-赋值
            if (ht != null)
            {
                foreach (string str in ht.Keys)
                {
                    switch (str)
                    {
                        case StartWorkAttr.OID:
                        case StartWorkAttr.CDT:
                        case StartWorkAttr.MD5:
                        case StartWorkAttr.Emps:
                        case StartWorkAttr.FID:
                        case StartWorkAttr.FK_Dept:
                        case StartWorkAttr.PRI:
                        case StartWorkAttr.Rec:
                        case StartWorkAttr.Title:
                            continue;
                        default:
                            break;
                    }
                    wk.SetValByKey(str, ht[str]);
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
            // 执行对报表的数据表WFState状态的更新,让它为runing的状态.
            if (string.IsNullOrEmpty(title) == false)
            {
                if (fl.TitleRole != "@OutPara")
                {
                    fl.TitleRole = "@OutPara";
                    fl.Update();
                }

                ps = new Paras();
                ps.SQL = "UPDATE " + fl.PTable + " SET PFlowNo=" + dbstr + "PFlowNo,PWorkID=" + dbstr + "PWorkID,WFState=" + dbstr + "WFState,Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
                ps.Add(GERptAttr.PFlowNo, parentFlowNo);
                ps.Add(GERptAttr.PWorkID, parentWorkID);
                //   ps.Add(GERptAttr.PFID, parentFID); PFID=" + dbstr + "PFID,

                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                ps.Add(GERptAttr.Title, title);
                ps.Add(GERptAttr.OID, wk.OID);
                DBAccess.RunSQL(ps);
            }
            else
            {
                ps = new Paras();
                ps.SQL = "UPDATE " + fl.PTable + " SET PFlowNo=" + dbstr + "PFlowNo,PWorkID=" + dbstr + "PWorkID,WFState=" + dbstr + "WFState,FK_Dept=" + dbstr + "FK_Dept,Title=" + dbstr + "Title WHERE OID=" + dbstr + "OID";
                ps.Add(GERptAttr.PFlowNo, parentFlowNo);
                ps.Add(GERptAttr.PWorkID, parentWorkID);
                //  ps.Add(GERptAttr.PFID, parentFID);

                ps.Add(GERptAttr.WFState, (int)WFState.Blank);
                ps.Add(GERptAttr.FK_Dept, empStarter.FK_Dept);
                ps.Add(GERptAttr.Title, WorkNode.GenerTitle(fl, wk));
                ps.Add(GERptAttr.OID, wk.OID);
                DBAccess.RunSQL(ps);
            }

            // 设置父流程信息.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wk.OID;
            int i = gwf.RetrieveFromDBSources();

            //将流程信息提前写入wf_GenerWorkFlow,避免查询不到
            gwf.FlowName = fl.Name;
            gwf.FK_Flow = flowNo;
            gwf.FK_FlowSort = fl.FK_FlowSort;
            gwf.FK_Dept = WebUser.FK_Dept;
            gwf.DeptName = WebUser.FK_DeptName;
            gwf.FK_Node = fl.StartNodeID;
            gwf.NodeName = nd.Name;
            gwf.WFState = WFState.Runing;
            if (string.IsNullOrEmpty(title))
                gwf.Title = BP.WF.WorkNode.GenerTitle(fl, wk);
            else
                gwf.Title = title;
            gwf.Starter = WebUser.No;
            gwf.StarterName = WebUser.Name;
            gwf.RDT = DataType.CurrentDataTime;
            gwf.PWorkID = parentWorkID;
            gwf.PFID = parentFID;
            gwf.PFlowNo = parentFlowNo;
            gwf.PNodeID = parentNodeID;
            if (i == 0)
                gwf.Insert();
            else
                gwf.Update();

            //插入待办.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.WorkID = wk.OID;
            gwl.FK_Node = nd.NodeID;
            gwl.FK_Emp = WebUser.No;
            i = gwl.RetrieveFromDBSources();

            gwl.FK_EmpText = WebUser.Name;
            gwl.FK_NodeText = nd.Name;
            gwl.FID = 0;
            gwl.FK_Flow = fl.No;
            gwl.FK_Dept = WebUser.FK_Dept;
            gwl.SDT = DataType.CurrentDataTime;
            gwl.DTOfWarning = DataType.CurrentDataTime;
            gwl.RDT = DataType.CurrentDataTime;
            gwl.IsEnable = true;
            gwl.IsPass = false;
            gwl.PRI = gwf.PRI;
            if (i == 0)
                gwl.Insert();
            else
                gwl.Update();

            if (parentWorkID != 0)
            {
                //设置父流程信息
                BP.WF.Dev2Interface.SetParentInfo(flowNo, wk.OID, parentFlowNo, parentWorkID, parentNodeID, parentEmp);
            }

            // 如果有跳转.
            if (jumpToNode != 0)
                BP.WF.Dev2Interface.Node_SendWork(flowNo, wk.OID, null, null, jumpToNode, jumpToEmp);

            return wk.OID;
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
            // 给全局变量赋值.
            BP.WF.Glo.SendHTOfTemp = htWork;

            // 转化成编号.
            flowNo = TurnFlowMarkToFlowNo(flowNo);

            //转化成编号
            parentFlowNo = TurnFlowMarkToFlowNo(parentFlowNo);

            if (string.IsNullOrEmpty(flowStarter))
                flowStarter = WebUser.No;

            Flow fl = new Flow(flowNo);
            Node nd = new Node(fl.StartNodeID);

            #region 处理流程标题.
            if (string.IsNullOrEmpty(title) == false && fl.TitleRole != "@OutPara")
            {
                /*如果标题不为空*/
                fl.TitleRole = "@OutPara"; //特殊标记不为空.
                fl.Update();
            }
            if (string.IsNullOrEmpty(title) == true && fl.TitleRole == "@OutPara")
            {
                /*如果标题为空 */
                fl.TitleRole = ""; //特殊标记不为空.
                fl.Update();
            }
            #endregion 处理流程标题.

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
                        case StartWorkAttr.OID:
                        case StartWorkAttr.CDT:
                        case StartWorkAttr.MD5:
                        case StartWorkAttr.Emps:
                        case StartWorkAttr.FID:
                        case StartWorkAttr.FK_Dept:
                        case StartWorkAttr.PRI:
                        case StartWorkAttr.Rec:
                        case StartWorkAttr.Title:
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

            gwf.FK_Dept = emp.FK_Dept;
            gwf.DeptName = emp.FK_DeptText;
            gwf.FK_Node = fl.StartNodeID;

            gwf.NodeName = nd.Name;
            gwf.WFSta = WFSta.Runing;
            gwf.WFState = WFState.Runing;

            if (string.IsNullOrEmpty(title))
                gwf.Title = BP.WF.WorkNode.GenerTitle(fl, wk);
            else
                gwf.Title = title;

            gwf.Starter = emp.No;
            gwf.StarterName = emp.Name;
            gwf.RDT = DataType.CurrentDataTime;

            if (htWork != null && htWork.ContainsKey("PRI") == true)
                gwf.PRI = int.Parse(htWork["PRI"].ToString());

            if (htWork != null && htWork.ContainsKey("SDTOfNode") == true)
                /*节点应完成时间*/
                gwf.SDTOfNode = htWork["SDTOfNode"].ToString();

            if (htWork != null && htWork.ContainsKey("SDTOfFlow") == true)
                /*流程应完成时间*/
                gwf.SDTOfNode = htWork["SDTOfFlow"].ToString();

            gwf.PWorkID = parentWorkID;
            gwf.PFlowNo = parentFlowNo;
            gwf.PNodeID = parentNDFrom;
            if (i == 0)
                gwf.Insert();
            else
                gwf.Update();

            // 产生工作列表.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.WorkID = wk.OID;
            if (gwl.RetrieveFromDBSources() == 0)
            {
                gwl.FK_Emp = emp.No;
                gwl.FK_EmpText = emp.Name;

                gwl.FK_Node = nd.NodeID;
                gwl.FK_NodeText = nd.Name;
                gwl.FID = 0;

                gwl.FK_Flow = fl.No;
                gwl.FK_Dept = emp.FK_Dept;

                gwl.SDT = DataType.CurrentDataTime;
                gwl.DTOfWarning = DataType.CurrentDataTime;
                gwl.RDT = DataType.CurrentDataTime;
                gwl.IsEnable = true;

                gwl.IsPass = false;
                //     gwl.Sender = WebUser.No;
                gwl.PRI = gwf.PRI;
                gwl.Insert();
            }
            #endregion 为开始工作创建待办

            // 执行对报表的数据表WFState状态的更新,让它为runing的状态. 
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE " + fl.PTable + " SET WFState=" + dbstr + "WFState,WFSta=" + dbstr + "WFSta,Title=" + dbstr 
                + "Title,FK_Dept=" + dbstr + "FK_Dept,PFlowNo=" + dbstr + "PFlowNo,PWorkID=" + dbstr + "PWorkID WHERE OID=" + dbstr + "OID";
            ps.Add("WFState", (int)WFState.Runing);
            ps.Add("WFSta", (int)WFSta.Runing);
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
                    paras += "@" + key + "=" + htWork[key].ToString();

                if (string.IsNullOrEmpty(paras) == false)
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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            return Node_SendWork(fk_flow, workID, htWork, null, toNodeID, nextWorkers, WebUser.No, WebUser.Name, WebUser.FK_Dept, WebUser.FK_DeptName, null);
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
            return Node_SendWork(fk_flow, workID, htWork, workDtls, toNodeID, nextWorkers, WebUser.No, WebUser.Name, WebUser.FK_Dept, WebUser.FK_DeptName, null);
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
            string nextWorkers, string execUserNo, string execUserName, string execUserDeptNo, string execUserDeptName, string title)
        {
            //给临时的发送变量赋值，解决带有参数的转向。
            Glo.SendHTOfTemp = htWork;

            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            int currNodeId = Dev2Interface.Node_GetCurrentNodeID(fk_flow, workID);
            BP.WF.Dev2Interface.Node_SaveWork(fk_flow, currNodeId, workID, htWork, workDtls);

            // 变量.
            Node nd = new Node(currNodeId);
            Work sw = nd.HisWork;
            sw.OID = workID;
            sw.RetrieveFromDBSources();

            //补偿性修复.
            if (nd.HisRunModel != RunModel.SubThread)
            {
                if (sw.FID != 0)
                    sw.DirectUpdate();
            }

            SendReturnObjs objs;
            //执行流程发送.
            WorkNode wn = new WorkNode(sw, nd);
            wn.Execer = execUserNo;
            wn.ExecerName = execUserName;
            wn.Execer = execUserNo;
            wn.title = title; // 设置标题，有可能是从外部传递过来的标题.
            wn.SendHTOfTemp = htWork;

            if (toNodeID == 0 || toNodeID == null)
                objs = wn.NodeSend(null, nextWorkers);
            else
                objs = wn.NodeSend(new Node(toNodeID), nextWorkers);

            #region 更新发送参数.
            if (htWork != null)
            {
                string dbstr = SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();

                string paras = "";
                foreach (string key in htWork.Keys)
                {
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

                if (string.IsNullOrEmpty(paras) == false)
                {
                    ps = new Paras();
                    ps.SQL = "UPDATE WF_GenerWorkerlist SET AtPara=" + dbstr + "Paras WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node";
                    ps.Add(GenerWorkerListAttr.Paras, paras);
                    ps.Add(GenerWorkerListAttr.WorkID, workID);
                    ps.Add(GenerWorkerListAttr.FK_Node, nd.NodeID);
                    DBAccess.RunSQL(ps);
                }
            }
            #endregion 更新发送参数.

            return objs;
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
                throw new Exception("@没有找到当前工作人员的待办，请检查该流程是否已经运行到该节点上来了。");

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
            string sql = "SELECT No,Name FROM Port_Emp WHERE No IN (SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + workid + " AND FK_Node=" + nodeID + " AND IsPass >=100 ) ORDER BY IDX desc";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            int idx = 100;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                string myEmpNo = dr[0].ToString();
                sql = "UPDATE WF_GenerWorkerList SET IsPass=" + idx + " WHERE FK_Emp='" + myEmpNo + "' AND WorkID=" + workid + " AND FK_Node=" + nodeID;
                BP.DA.DBAccess.RunSQL(sql);
            }
        }
        /// <summary>
        /// 把抄送写入待办列表
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="ccToEmpNo">抄送给</param>
        /// <param name="ccToEmpName">抄送给名称</param>
        /// <returns></returns>
        public static string Node_CC_WriteTo_Todolist(int ndFrom, int ndTo, Int64 workID, string ccToEmpNo, string ccToEmpName)
        {
            return Node_CC_WriteTo_CClist(ndFrom, ndTo, workID, ccToEmpNo, ccToEmpName, "", "");
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
        public static string Node_CC_WriteTo_CClist(int ndFrom, int ndTo, Int64 workID, string toEmpNo, string toEmpName,
            string msgTitle, string msgDoc)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                Node nd = new Node(ndTo);
                gwf.FK_Node = ndTo;
                gwf.FK_Flow = nd.FK_Flow;
                gwf.FlowName = nd.FlowName;
                gwf.NodeName = nd.Name;
            }
            Node fromNode = new Node(ndFrom);
            CCList list = new CCList();
            list.MyPK = DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
            list.FK_Flow = gwf.FK_Flow;
            list.FlowName = gwf.FlowName;
            list.FK_Node = ndTo;
            list.NodeName = gwf.NodeName;
            list.Title = msgTitle;
            list.Doc = msgDoc;
            list.CCTo = toEmpNo;
            list.CCToName = toEmpName;

            //增加抄送人部门.
            Emp emp = new Emp(toEmpNo);
            list.CCToDept = emp.FK_Dept;
            list.RDT = DataType.CurrentDataTime;
            list.Rec = WebUser.No;
            list.WorkID = gwf.WorkID;
            list.FID = gwf.FID;
            list.PFlowNo = gwf.PFlowNo;
            list.PWorkID = gwf.PWorkID;
            list.NDFrom = ndFrom;
            list.InEmpWorks = fromNode.CCWriteTo == CCWriteTo.CCList ? false : true;    //added by liuxc,2015.7.6
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
            BP.WF.Dev2Interface.Port_SendMsg(toEmpNo, msgTitle, msgDoc, "CC" + gwf.FK_Node + "_" + gwf.WorkID, SMSMsgType.CC, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);
            //记录日志.
            Glo.AddToTrack(ActionType.CC, gwf.FK_Flow, workID, gwf.FID, gwf.FK_Node, gwf.NodeName,
                WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, toEmpNo, toEmpName, msgTitle, null);

            return "已经成功的把工作抄送给:" + toEmpNo + "," + toEmpName;
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="mypk">删除</param>
        public static void Node_CC_DoDel(string mypk)
        {
            Paras ps = new Paras();
            ps.SQL = "DELETE WF_CCList WHERE MyPK=" + SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add(CCListAttr.MyPK, mypk);
            BP.DA.DBAccess.RunSQL(ps);
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
            string dbstr = SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList   SET Sta=" + dbstr + "Sta,CDT=" + dbstr + "CDT WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND CCTo=" + dbstr + "CCTo";
            ps.Add(CCListAttr.Sta, (int)sta);
            ps.Add(CCListAttr.CDT, DataType.CurrentDataTime);
            ps.Add(CCListAttr.WorkID, workid);
            ps.Add(CCListAttr.FK_Node, nodeID);
            ps.Add(CCListAttr.CCTo, empNo);
            BP.DA.DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 执行读取
        /// </summary>
        /// <param name="mypk"></param>
        public static void Node_CC_SetRead(string mypk)
        {
            if (string.IsNullOrEmpty(mypk))
                return;

            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList SET Sta=" + SystemConfig.AppCenterDBVarStr + "Sta  WHERE MyPK=" + SystemConfig.AppCenterDBVarStr + "MyPK";
            ps.Add(CCListAttr.Sta, (int)CCSta.Read);
            ps.Add(CCListAttr.MyPK, mypk);
            BP.DA.DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 设置抄送读取
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="empNo">读取人员编号</param>
        public static void Node_CC_SetRead(int nodeID, Int64 workid, string empNo)
        {
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_CCList SET Sta=" + SystemConfig.AppCenterDBVarStr + "Sta  WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND CCTo=" + SystemConfig.AppCenterDBVarStr + "CCTo";
            ps.Add(CCListAttr.Sta, (int)CCSta.Read);
            ps.Add(CCListAttr.WorkID, workid);
            ps.Add(CCListAttr.FK_Node, nodeID);
            ps.Add(CCListAttr.CCTo, empNo);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsRead=1 WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp";
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            ps.Add(GenerWorkerListAttr.FK_Node, nodeID);
            ps.Add(GenerWorkerListAttr.FK_Emp, empNo);

            DBAccess.RunSQL(ps);
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
        /// <returns></returns>
        public static string Node_CC(string fk_flow, int fk_node, Int64 workID, string toEmpNo, string toEmpName, string msgTitle, string msgDoc, string pFlowNo, Int64 pWorkID)
        {
            Flow fl = new Flow(fk_flow);
            Node nd = new Node(fk_node);

            CCList list = new CCList();
            list.MyPK = DBAccess.GenerOIDByGUID().ToString(); // workID + "_" + fk_node + "_" + empNo;
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
            list.CCToDept = emp.FK_Dept;

            list.RDT = DataType.CurrentDataTime;
            list.Rec = WebUser.No;
            list.WorkID = workID;
            list.FID = 0;
            list.PFlowNo = pFlowNo;
            list.PWorkID = pWorkID;

            try
            {
                list.Insert();
            }
            catch
            {
                list.CheckPhysicsTable();
                list.Update();
            }

            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            //记录日志.
            Glo.AddToTrack(ActionType.CC, fk_flow, workID, 0, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmpNo, toEmpName, msgTitle, null);

            //发送邮件.
            BP.WF.Dev2Interface.Port_SendMsg(toEmpNo, WebUser.Name + "把工作:" + gwf.Title, "抄送:" + msgTitle, "CC" + nd.NodeID + "_" + workID + "_", BP.WF.SMSMsgType.CC,
                gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

            return "已经成功的把工作抄送给:" + toEmpNo + "," + toEmpName;

        }
        /// <summary>
        /// 删除草稿
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        public static void Node_DeleteDraft(string fk_flow, Int64 workID)
        {
            //转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 1)
            {
                if (gwf.FK_Node != int.Parse(fk_flow + "01"))
                    throw new Exception("@该流程非草稿流程不能删除:" + gwf.Title);

                if (gwf.WFState != WFState.Draft)
                    throw new Exception("@非草稿状态不能删除");

                gwf.Delete();
            }

            //删除流程.
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Flow fl = new Flow(fk_flow);
            Paras ps = new Paras();
            ps.SQL = "DELETE FROM " + fl.PTable + " WHERE OID=" + dbstr + "OID ";
            ps.Add(GERptAttr.OID, workID);
            DBAccess.RunSQL(ps);


            //删除开始节点数据.
            try
            {
                ps = new Paras();
                ps.SQL = "DELETE FROM ND" + int.Parse(fk_flow + "01") + " WHERE OID=" + dbstr + "OID ";
                ps.Add(GERptAttr.OID, workID);
                DBAccess.RunSQL(ps);
            }
            catch
            {
            }

        }
        /// <summary>
        /// 把草稿设置待办
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="workID"></param>
        public static void Node_SetDraft2Todolist(string fk_flow, Int64 workID)
        {

        }
        /// <summary>
        /// 设置当前工作状态为草稿,如果启用了草稿, 请在开始节点的表单保存按钮下增加上它.
        /// 注意:必须是在开始节点时调用.
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        public static void Node_SetDraft(string fk_flow, Int64 workID)
        {
            //转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            Flow fl = new Flow(fk_flow);

            //保存数据
            Node_SaveWork(fk_flow, fl.StartNodeID, workID);

            //设置引擎表.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 1)
            {
                if (gwf.FK_Node != int.Parse(fk_flow + "01"))
                    throw new Exception("@设置草稿错误，只有在开始节点时才能设置草稿，现在的节点是:" + gwf.Title);

                if (gwf.WFState != WFState.Blank)
                    return;
                //规则设置为写入待办，将状态置为运行中，其他设置为草稿
                WFState wfState = fl.DraftRole == DraftRole.SaveToTodolist ? WFState.Runing : WFState.Draft;
                //设置成草稿.
                gwf.Update(GenerWorkFlowAttr.WFState, (int)wfState);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回保存的信息</returns>
        public static string Node_SaveWork(string fk_flow, int fk_node, Int64 workID)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            return Node_SaveWork(fk_flow, fk_node, workID, new Hashtable(), null);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">workid</param>
        /// <param name="wk">节点表单参数</param>
        /// <returns></returns>
        public static string Node_SaveWork(string fk_flow, int fk_node, Int64 workID, Hashtable wk)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            return Node_SaveWork(fk_flow, fk_node, workID, wk, null);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workID">工作ID</param>
        /// <param name="htWork">工作数据</param>
        /// <returns>返回执行信息</returns>
        public static string Node_SaveWork(string fk_flow, int fk_node, Int64 workID, Hashtable htWork, DataSet dsDtls)
        {
            if (htWork == null)
                return "参数错误，保存失败。";

            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            try
            {
                Node nd = new Node(fk_node);
                Work wk = nd.HisWork;
                if (workID != 0)
                {
                    wk.OID = workID;
                    wk.RetrieveFromDBSources();
                }
                wk.ResetDefaultVal();

                if (htWork != null)
                {
                    foreach (string str in htWork.Keys)
                    {
                        switch (str)
                        {
                            case StartWorkAttr.OID:
                            case StartWorkAttr.CDT:
                            case StartWorkAttr.MD5:
                            case StartWorkAttr.Emps:
                            case StartWorkAttr.FID:
                            case StartWorkAttr.FK_Dept:
                            case StartWorkAttr.PRI:
                            case StartWorkAttr.Rec:
                            case StartWorkAttr.Title:
                                continue;
                            default:
                                break;
                        }

                        if (wk.Row.ContainsKey(str))
                            wk.SetValByKey(str, htWork[str]);
                        else
                            wk.Row.Add(str, htWork[str]);
                    }
                }

                wk.Rec = WebUser.No;
                wk.RecText = WebUser.Name;
                wk.SetValByKey(StartWorkAttr.FK_Dept, WebUser.FK_Dept);
                wk.BeforeSave();
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
                                daDtl.RDT = DataType.CurrentDataTime;

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
                        paras += "@" + key + "=" + htWork[key].ToString();

                    if (string.IsNullOrEmpty(paras) == false)
                    {
                        string dbstr = SystemConfig.AppCenterDBVarStr;
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
                        wn.DoCopyRptWork(wk);

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDUserName)
                            rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No + "," + WebUser.Name);

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserIDOnly)
                            rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No);

                        if (Glo.UserInfoShowModel == UserInfoShowModel.UserNameOnly)
                            rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.Name);

                        rptGe.SetValByKey(GERptAttr.FlowStarter, WebUser.No);
                        rptGe.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDataTime);
                        rptGe.SetValByKey(GERptAttr.WFState, 0);
                        rptGe.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);
                        rptGe.SetValByKey(GERptAttr.FK_Dept, WebUser.FK_Dept);
                        rptGe.Insert();
                    }
                    else
                    {
                        wn.DoCopyRptWork(wk);
                        rptGe.Update();
                    }
                }
                //获取表单树的数据
                BP.WF.WorkNode workNode = new WorkNode(workID, fk_node);
                Work treeWork = workNode.CopySheetTree();
                if (treeWork != null)
                    wk.Copy(treeWork);

                #region 为开始工作创建待办.
                if (nd.IsStartNode == true)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow();
                    Flow fl = new Flow(fk_flow);
                    if (fl.DraftRole == DraftRole.None)
                        return "保存成功";

                    //规则设置为写入待办，将状态置为运行中，其他设置为草稿
                    WFState wfState = fl.DraftRole == DraftRole.SaveToTodolist ? WFState.Runing : WFState.Draft;

                    gwf.WorkID = workID;
                    int i = gwf.RetrieveFromDBSources();
                    if (i == 0)
                    {
                        gwf.FlowName = fl.Name;
                        gwf.FK_Flow = fk_flow;
                        gwf.FK_FlowSort = fl.FK_FlowSort;

                        gwf.FK_Node = fk_node;
                        gwf.NodeName = nd.Name;
                        gwf.WFState = wfState;

                        gwf.FK_Dept = WebUser.FK_Dept;
                        gwf.DeptName = WebUser.FK_DeptName;
                        gwf.Title = BP.WF.WorkNode.GenerTitle(fl, wk);
                        gwf.Starter = WebUser.No;
                        gwf.StarterName = WebUser.Name;
                        gwf.RDT = DataType.CurrentDataTime;
                        gwf.Insert();
                        // 产生工作列表.
                        GenerWorkerList gwl = new GenerWorkerList();
                        gwl.WorkID = workID;
                        gwl.FK_Emp = WebUser.No;
                        gwl.FK_EmpText = WebUser.Name;

                        gwl.FK_Node = fk_node;
                        gwl.FK_NodeText = nd.Name;
                        gwl.FID = 0;

                        gwl.FK_Flow = fk_flow;
                        gwl.FK_Dept = WebUser.FK_Dept;
                        gwl.SDT = DataType.CurrentDataTime;
                        gwl.DTOfWarning = DataType.CurrentDataTime;
                        gwl.RDT = DataType.CurrentDataTime;
                        gwl.IsEnable = true;

                        gwl.IsPass = false;
                        //  gwl.Sender = WebUser.No;
                        gwl.PRI = gwf.PRI;
                        gwl.Insert();
                    }
                    else
                    {
                        gwf.WFState = wfState;
                        gwf.DirectUpdate();
                    }
                }
                #endregion 为开始工作创建待办

                return "保存成功.";
            }
            catch (Exception ex)
            {
                return "保存失败:" + ex.Message;
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
                en.SetValByKey(key, htData[key].ToString());

            en.SetValByKey("OID", workID);

            FrmEvents fes = md.FrmEvents;
            fes.DoEventNode(FrmEventList.SaveBefore, en);
            if (i == 0)
                en.Insert();
            else
                en.Update();

            if (workDtls != null)
            {
                MapDtls dtls = new MapDtls(fk_mapdata);
                //保存从表
                foreach (DataTable dt in workDtls.Tables)
                {
                    foreach (MapDtl dtl in dtls)
                    {
                        if (dt.TableName != dtl.No)
                            continue;
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
            fes.DoEventNode(FrmEventList.SaveAfter, en);
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
                throw new Exception("@配置没有设置成共享任务池的状态。");

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.TaskSta == TaskSta.None)
                throw new Exception("@该任务非共享任务。");

            if (gwf.TaskSta == TaskSta.Takeback)
                throw new Exception("@该任务已经被其他人取走。");

            //更新状态。
            gwf.TaskSta = TaskSta.Takeback;
            gwf.Update();

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            //设置已经被取走的状态。
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=-1 WHERE IsEnable=1 AND WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp!=" + dbstr + "FK_Emp ";
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            ps.Add(GenerWorkerListAttr.FK_Node, gwf.FK_Node);
            ps.Add(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
            int i = DBAccess.RunSQL(ps);

            BP.WF.Dev2Interface.WriteTrackInfo(gwf.FK_Flow, gwf.FK_Node, workid, 0, "任务被" + WebUser.Name + "从任务池取走.", "获取");
            if (i == 1)
                return true;
            else
                return false;
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
                throw new Exception("@配置没有设置成共享任务池的状态。");

            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.TaskSta == TaskSta.None)
                throw new Exception("@该任务非共享任务。");

            if (gwf.TaskSta == TaskSta.Sharing)
                throw new Exception("@该任务已经是共享状态。");

            // 更新 状态。
            gwf.TaskSta = TaskSta.Sharing;
            gwf.Update();

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            //设置已经被取走的状态。
            ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=1 WHERE IsEnable=-1 AND WorkID=" + dbstr + "WorkID ";
            ps.Add(GenerWorkerListAttr.WorkID, workid);
            int i = DBAccess.RunSQL(ps);
            if (i == 0)
                throw new Exception("@流程数据错误,不应当更新不到数据。");

            BP.WF.Dev2Interface.WriteTrackInfo(gwf.FK_Flow, gwf.FK_Node, workid, 0, "任务被" + WebUser.Name + "放入了任务池.", "放入");
        }
        /// <summary>
        /// 增加下一步骤的接受人(用于当前步骤向下一步骤发送时增加接受人)
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="formNodeID">节点ID</param>
        /// <param name="emps">如果多个就用逗号分开</param>
        /// <param name="Del_Selected">是否删除历史选择</param>
        public static void Node_AddNextStepAccepters(Int64 workID, int formNodeID, string emps, bool Del_Selected = true)
        {
            SelectAccper sa = new SelectAccper();
            //删除历史选择
            if (Del_Selected == true)
            {
                sa.Delete(SelectAccperAttr.FK_Node, formNodeID, SelectAccperAttr.WorkID, workID);
            }
            emps = emps.Replace(" ", "");
            emps = emps.Replace(";", ",");
            emps = emps.Replace("@", ",");
            string[] strs = emps.Split(',');

            Emp empEn = new Emp();
            foreach (string emp in strs)
            {
                if (string.IsNullOrEmpty(emp))
                    continue;
                sa.MyPK = formNodeID + "_" + workID + "_" + emp;

                empEn.No = emp;
                int i = empEn.RetrieveFromDBSources();
                if (i == 0)
                    continue;

                sa.FK_Emp = emp;
                sa.EmpName = empEn.Name;

                sa.FK_Node = formNodeID;
                sa.WorkID = workID;
                sa.Insert();
            }
        }
        /// <summary>
        /// 增加下一步骤的接受人(用于当前步骤向下一步骤发送时增加接受人)
        /// </summary>
        /// <param name="workID">工作ID</param>
        /// <param name="formNodeID">从节点ID</param>
        /// <param name="emp">接收人</param>
        /// <param name="tag">分组维度，可以为空.是为了分流节点向下发送时候，可能有一个工作人员两个或者两个以上的子线程的情况出现。
        /// tag 是个维度，这个维度可能是一个类别，一个批次，一个标记，总之它是一个字符串。详细: http://bbs.ccflow.org/showtopic-3065.aspx </param>
        public static void Node_AddNextStepAccepter(Int64 workID, int formNodeID, string emp, string tag)
        {
            SelectAccper sa = new SelectAccper();
            sa.Delete(SelectAccperAttr.FK_Node, formNodeID, SelectAccperAttr.WorkID, workID, SelectAccperAttr.FK_Emp, emp, SelectAccperAttr.Tag, tag);

            Emp empEn = new Emp();
            sa.MyPK = formNodeID + "_" + workID + "_" + emp + "_" + tag;
            empEn.No = emp;
            sa.Tag = tag;
            sa.FK_Emp = emp;
            sa.EmpName = empEn.Name;
            sa.FK_Node = formNodeID;
            sa.WorkID = workID;
            sa.Insert();
        }
        /// <summary>
        /// 节点工作挂起
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="way">挂起方式</param>
        /// <param name="reldata">解除挂起日期(可以为空)</param>
        /// <param name="hungNote">挂起原因</param>
        /// <returns>返回执行信息</returns>
        public static string Node_HungUpWork(string fk_flow, Int64 workid, int wayInt, string reldata, string hungNote)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            HungUpWay way = (HungUpWay)wayInt;
            BP.WF.WorkFlow wf = new WorkFlow(fk_flow, workid);
            return wf.DoHungUp(way, reldata, hungNote);
        }
        /// <summary>
        /// 节点工作取消挂起
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="msg">取消挂起原因</param>
        /// <returns>执行信息</returns>
        public static void Node_UnHungUpWork(string fk_flow, Int64 workid, string msg)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            BP.WF.WorkFlow wf = new WorkFlow(fk_flow, workid);
            wf.DoUnHungUp();
        }
        /// <summary>
        /// 获取该节点上的挂起时间
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <returns>返回时间串，如果没有挂起的动作就抛出异常.</returns>
        public static TimeSpan Node_GetHungUpTimeSpan(string flowNo, int nodeID, Int64 workid)
        {
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;

            string instr = (int)ActionType.HungUp + "," + (int)ActionType.UnHungUp;
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
                if (at == ActionType.HungUp)
                    dtStart = DataType.ParseSysDateTime2DateTime(item[TrackAttr.RDT].ToString());

                //解除挂起时间.
                if (at == ActionType.UnHungUp)
                    dtEnd = DataType.ParseSysDateTime2DateTime(item[TrackAttr.RDT].ToString());
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
            emp.No = askForEmp;
            if (emp.RetrieveFromDBSources() == 0)
                throw new Exception("@要加签的人员编号错误:" + askForEmp);

            //获得当前流程注册信息.
            BP.WF.GenerWorkFlow gwf = new GenerWorkFlow(workid);

            //检查当前人员是否开可以执行当前的工作?
            if (Flow_IsCanDoCurrentWork(gwf.FK_Node, gwf.WorkID, WebUser.No) == false)
                throw new Exception("@当前的工作已经被别人处理或者您没有处理该工作的权限.");

            //检查被加签的人是否在当前的队列中.
            GenerWorkerLists gwls = new GenerWorkerLists(workid, gwf.FK_Node);
            if (gwls.Contains(GenerWorkerListAttr.FK_Emp, askForEmp, GenerWorkerListAttr.IsEnable, 0) == true)
                throw new Exception("@加签失败，您选择的加签人可以处理当前的工作。");


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
                gwl.IsPassInt = (int)askforSta;
                gwl.Insert();
                //重新查询.
                gwls = new GenerWorkerLists(workid, gwf.FK_Node);

                //设置流程标题.
                if (gwf.Title.Length == 0)
                    Flow_SetFlowTitle(gwf.FK_Flow, workid, "来自" + WebUser.Name + "的工作加签.");
            }
            // endWarning.


            // 处理状态.
            foreach (GenerWorkerList item in gwls)
            {
                if (item.IsEnable == false)
                    continue;

                if (item.FK_Emp == WebUser.No)
                {
                    // GenerWorkerList gwl = gwls[0] as GenerWorkerList;
                    item.IsPassInt = (int)askforSta;
                    item.Update();

                    // 更换主键后，执行insert ,让被加签人有代办工作.
                    item.IsPassInt = 0;
                    item.FK_Emp = emp.No;
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


            BP.WF.Dev2Interface.WriteTrack(gwf.FK_Flow, gwf.FK_Node, workid, gwf.FID, askForNote, ActionType.AskforHelp, "", null,null,emp.No,emp.Name);

            Flow fl = new Flow(gwf.FK_Flow);
            BP.WF.Dev2Interface.Port_SendMsg(askForEmp, gwf.Title, askForNote, "AK" + gwf.FK_Node + "_" + gwf.WorkID, SMSMsgType.AskFor, gwf.FK_Flow, gwf.FK_Node, workid, gwf.FID);
            //更新状态.
            DBAccess.RunSQL("UPDATE " + fl.PTable + " SET WFState=" + (int)WFState.Askfor + " WHERE OID=" + workid);

            //设置成工作未读。
            BP.WF.Dev2Interface.Node_SetWorkUnRead(gwf.FK_Node, workid);

            string msg = "您的工作已经提交给(" + askForEmp + " " + emp.Name + ")加签了。";

            //加签后事件
            BP.WF.Node hisNode = new BP.WF.Node(gwf.FK_Node);
            Work currWK = hisNode.HisWork;
            currWK.OID = gwf.WorkID;
            currWK.Retrieve();

            //执行加签后的事件.
            msg += fl.DoFlowEventEntity(EventListOfNode.AskerAfter, hisNode, currWK, null);

            return msg;
        }
        /// <summary>
        /// 答复加签信息
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">FID</param>
        /// <param name="replyNote">答复信息</param>
        /// <returns></returns>
        public static string Node_AskforReply(string fk_flow, int fk_node, Int64 workid, Int64 fid, string replyNote)
        {
            // 把回复信息临时的写入 流程注册信息表以便让发送方法获取这个信息写入日志.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.Paras_AskForReply = replyNote;
            gwf.Update();

            //执行发送, 在发送的方法里面已经做了判断了,并且把 回复的信息写入了日志.
            string info = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid).ToMsgOfHtml();

            Node node = new Node(fk_node);
            Work wk = node.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();

            //恢复加签后执行事件
            info += node.HisFlow.DoFlowEventEntity(EventListOfNode.AskerReAfter, node, wk, null);
            return info;
        }
        /// <summary>
        /// 工作移交
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="toEmp">移交到人员(只给移交给一个人)</param>
        /// <param name="msg">移交消息</param>
        public static string Node_Shift(string flowNo, int nodeID, Int64 workID, Int64 fid, string toEmp, string msg)
        {
            // 人员.
            Emp emp = new Emp(toEmp);
            Node nd = new Node(nodeID);

            if (nd.TodolistModel == TodolistModel.Order || nd.TodolistModel == TodolistModel.Teamup)
            {
                /*如果是队列模式，或者是协作模式. */
                try
                {
                    string sql = "UPDATE WF_GenerWorkerlist SET FK_Emp='" + emp.No + "', FK_EmpText='" + emp.Name + "' WHERE FK_Emp='" + WebUser.No + "' AND FK_Node=" + nodeID + " AND WorkID=" + workID;
                    BP.DA.DBAccess.RunSQL(sql);
                }
                catch
                {
                    return "@移交失败，您所移交的人员(" + emp.No + " " + emp.Name + ")已经在代办列表里.";
                }

                //记录日志.
                Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, fid, nd.NodeID, nd.Name,
                    WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmp, emp.Name, msg, null);

                string info = "@工作移交成功。@您已经成功的把工作移交给：" + emp.No + " , " + emp.Name;

                //移交后事件
                info += "@" + nd.HisFlow.DoFlowEventEntity(EventListOfNode.ShitAfter, nd, nd.HisWork, null);

                info += "@<a href='" + Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?DoType=UnShift&FK_Flow=" + nd.FK_Flow + "&WorkID=" + workID + "&FK_Node=" + nodeID + "&FID=" + fid + "' ><img src='Img/Action/UnSend.png' border=0 />撤消工作移交</a>.";
                return info;

            }

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                /*说明开始节点数据表单移交.*/
                gwf.WorkID = workID;
                gwf.Title = "由" + WebUser.No + " ; " + WebUser.Name + ", 在(" + DataType.CurrentDataCNOfShort + ")移交来的工作";
                gwf.FK_Dept = WebUser.FK_Dept;
                gwf.FK_Flow = flowNo;

                Flow fl = new Flow(flowNo);
                gwf.FK_FlowSort = fl.FK_FlowSort;
                gwf.FK_Node = nodeID;
                gwf.Starter = emp.No;
                gwf.StarterName = emp.Name;
                gwf.WFState = WFState.Shift;
                gwf.TodoEmps = toEmp;
                gwf.TodoEmpsNum = 1;
                gwf.RDT = DataType.CurrentDataTime;
                gwf.NodeName = "";
                gwf.FlowName = fl.Name;
                gwf.Emps = toEmp;
                gwf.DeptName = WebUser.FK_DeptName;
                gwf.Insert();

                GenerWorkerList gwl = new GenerWorkerList();
                gwl.WorkID = workID;
                gwl.FK_Dept = WebUser.FK_Dept;

                //gwl.FK_DeptT = WebUser.FK_DeptName;
                gwl.FK_Node = nodeID;
                gwl.FK_NodeText = nd.Name;

                gwl.FK_Emp = toEmp;
                gwl.FK_EmpText = emp.Name;

                gwl.FK_Flow = flowNo;

                gwl.IsPass = false;
                gwl.IsPassInt = 0;
                gwl.IsRead = false;
                gwl.PressTimes = 0;
                gwl.RDT = gwf.RDT;
                gwl.SDT = gwf.RDT;
                //gwl.Sender = WebUser.No;
                gwl.Insert();
            }
            else
            {
                if (gwf.WFSta == WFSta.Complete)
                    throw new Exception("@流程已经完成，您不能执行移交了。");

                // 删除当前非配的工作。
                // 已经非配或者自动分配的任务。
                //设置所有的工作人员为不可处理.
                string dbStr = SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=0  WHERE WorkID=" + dbStr + "WorkID AND FK_Node=" + dbStr + "FK_Node";
                ps.Add(GenerWorkerListAttr.WorkID, workID);
                ps.Add(GenerWorkerListAttr.FK_Node, nodeID);
                DBAccess.RunSQL(ps);


                //设置被移交人的FK_Emp 为当前处理人，（有可能被移交人不在工作列表里，就返回0.）
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerlist SET IsEnable=1  WHERE WorkID=" + dbStr + "WorkID AND FK_Node=" + dbStr + "FK_Node AND FK_Emp=" + dbStr + "FK_Emp";
                ps.Add(GenerWorkerListAttr.WorkID, workID);
                ps.Add(GenerWorkerListAttr.FK_Node, nodeID);
                ps.Add(GenerWorkerListAttr.FK_Emp, toEmp);
                int i = DBAccess.RunSQL(ps);

                GenerWorkerLists wls = null;
                GenerWorkerList wl = null;
                if (i == 0)
                {
                    /*说明: 用其它的岗位上的人来处理的，就给他增加共享工作。*/
                    wls = new GenerWorkerLists(workID, nodeID);
                    if (wls.Count == 0)
                    {
                        if (nd.IsStartNode == false)
                            throw new Exception("@流程引擎 GenerWorkerLists 数据丢失, workID=" + workID + ",nodeID=" + nodeID);

                        wl = new GenerWorkerList();
                        wl.FK_Dept = BP.Web.WebUser.FK_Dept;
                        //   mygwfl.FK_DeptT = BP.Web.WebUser.FK_DeptName;
                        wl.WorkID = workID;
                        wl.FID = 0;
                        wl.FK_Emp = BP.Web.WebUser.No;
                        wl.FK_EmpText = BP.Web.WebUser.Name;
                        wl.FK_Node = nodeID;
                        wl.FK_NodeText = nd.Name;
                        wl.Sender = BP.Web.WebUser.No;
                        wl.RDT = BP.DA.DataType.CurrentDataTime;
                        wl.IsPass = false;
                        wl.IsRead = false;
                        wl.IsEnable = true;
                        wl.Insert();
                        wls.AddEntity(wl);
                    }
                    else
                        wl = wls[0] as GenerWorkerList;

                    wl.FK_Emp = toEmp.ToString();
                    wl.FK_EmpText = emp.Name;
                    wl.IsEnable = true;
                    wl.IsRead = false;
                    wl.Insert();

                    // 清除工作者，为转发消息所用.
                    wls.Clear();
                    wls.AddEntity(wl);
                }

                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Shift + ", WFSta=" + (int)WFSta.Runing + "   WHERE WorkID=" + dbStr + "WorkID ";
                ps.Add(GenerWorkerListAttr.WorkID, workID);
                DBAccess.RunSQL(ps);
            }

            ShiftWork sf = new ShiftWork();
            sf.WorkID = workID;
            sf.FK_Node = nodeID;
            sf.ToEmp = toEmp;
            sf.ToEmpName = emp.Name;
            sf.Note = msg;
            sf.FK_Emp = WebUser.No;
            sf.FK_EmpName = WebUser.Name;
            sf.Insert();
            //记录日志.
            Glo.AddToTrack(ActionType.Shift, nd.FK_Flow, workID, gwf.FID, nd.NodeID, nd.Name,
                WebUser.No, WebUser.Name, nd.NodeID, nd.Name, toEmp, emp.Name, msg, null);

            //发送邮件.
            BP.WF.Dev2Interface.Port_SendMsg(emp.No, WebUser.Name + "向您移交了工作" + gwf.Title, "移交信息:" + msg, "SF" + workID + "_" + sf.FK_Node, BP.WF.SMSMsgType.Shift, gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID);

            string inf1o = "@工作移交成功。@您已经成功的把工作移交给：" + emp.No + " , " + emp.Name;

            //移交后事件
            inf1o += "@" + nd.HisFlow.DoFlowEventEntity(EventListOfNode.ShitAfter, nd, nd.HisWork, null);

            inf1o += "@<a href='" + Glo.CCFlowAppPath + "WF/MyFlowInfo.aspx?DoType=UnShift&FK_Flow=" + nd.FK_Flow + "&WorkID=" + workID + "&FK_Node=" + nodeID + "&FID=" + fid + "' ><img src='Img/Action/UnSend.png' border=0 />撤消工作移交</a>.";
            return inf1o;
        }
        /// <summary>
        /// 撤销移交
        /// </summary>
        /// <param name="flowNo">撤销编号</param>
        /// <param name="workID">工作ID</param>
        /// <returns>返回撤销信息</returns>
        public static string Node_ShiftUn(string flowNo, Int64 workID)
        {
            WorkFlow mwf = new WorkFlow(flowNo, workID);
            return mwf.DoUnShift();
        }
        /// <summary>
        /// 执行工作退回(退回指定的点)
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="currentNodeID">当前节点ID</param>
        /// <param name="returnToNodeID">退回到的工作ID</param>
        /// <param name="returnToEmp">退回到人员</param>
        /// <param name="msg">退回原因</param>
        /// <param name="isBackToThisNode">退回后是否要原路返回？</param>
        /// <returns>执行结果，此结果要提示给用户。</returns>
        public static string Node_ReturnWork(string fk_flow, Int64 workID, Int64 fid, int currentNodeID, int returnToNodeID,
            string returnToEmp, string msg = "无", bool isBackToThisNode = false)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            WorkReturn wr = new WorkReturn(fk_flow, workID, fid, currentNodeID, returnToNodeID, returnToEmp, isBackToThisNode, msg);
            return wr.DoIt();
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
            return Node_ReturnWork(fk_flow, workID, fid, currentNodeID, returnToNodeID, null, msg, isBackToThisNode);
        }
        /// <summary>
        /// 获取当前工作的NodeID
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <returns>指定工作的NodeID.</returns>
        public static int Node_GetCurrentNodeID(string fk_flow, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            int nodeID = BP.DA.DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + workid + " AND FK_Flow='" + fk_flow + "'", 0);
            if (nodeID == 0)
                return int.Parse(fk_flow + "01");
            return nodeID;
        }

        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fid">流程ID</param>
        /// <param name="workid">工作ID</param>
        public static void Node_FHL_KillSubFlow(string fk_flow, Int64 fid, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            WorkFlow wkf = new WorkFlow(fk_flow, workid);
            wkf.DoDeleteWorkFlowByReal(true);
        }
        /// <summary>
        /// 合流点驳回子线程
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fid">流程ID</param>
        /// <param name="workid">子线程ID</param>
        /// <param name="msg">驳回消息</param>
        public static string Node_FHL_DoReject(string fk_flow, int NodeSheetfReject, Int64 fid, Int64 workid, string msg)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            WorkFlow wkf = new WorkFlow(fk_flow, workid);
            return wkf.DoReject(fid, NodeSheetfReject, msg);
        }

        /// <summary>
        /// 跳转审核取回
        /// </summary>
        /// <param name="fromNodeID">从节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="tackToNodeID">取回到的节点ID</param>
        /// <returns></returns>
        public static string Node_Tackback(int fromNodeID, Int64 workid, int tackToNodeID)
        {
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
            wn.AddToTrack(ActionType.Tackback, WebUser.No, WebUser.Name, tackToNodeID, nd.Name, "执行跳转审核的取回.");
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
            FrmWorkCheck fwc = new FrmWorkCheck(fk_node);

            BP.WF.Dev2Interface.WriteTrackWorkCheck(fk_flow, fk_node, workid,
                fid, checkNote, fwc.FWCOpLabel);

            //设置审核完成.
            BP.WF.Dev2Interface.Node_CC_SetSta(fk_node, workid, BP.Web.WebUser.No, BP.WF.Template.CCSta.CheckOver);

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
            if (nd.IsStartNode)
                return;

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsRead=1 WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add("WorkID", workid);
            ps.Add("FK_Node", nodeID);
            ps.Add("FK_Emp", empNo);
            if (DBAccess.RunSQL(ps) == 0)
                throw new Exception("@设置的工作不存在，或者当前的登陆人员已经改变。");

            // 判断当前节点的已读回执.
            if (nd.ReadReceipts == ReadReceipts.None)
                return;

            bool isSend = false;
            if (nd.ReadReceipts == ReadReceipts.Auto)
                isSend = true;

            if (nd.ReadReceipts == ReadReceipts.BySysField)
            {
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
                        continue;
                    fromNodeID = item.NodeID;
                    break;
                }
                if (fromNodeID == 0)
                    throw new Exception("@没有找到它的上一步工作。");

                try
                {
                    ps = new Paras();
                    ps.SQL = "SELECT " + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + " FROM ND" + fromNodeID + "    WHERE OID=" + dbstr + "OID";
                    ps.Add("OID", workid);
                    DataTable dt1 = DBAccess.RunSQLReturnTable(ps);
                    if (dt1.Rows[0][0].ToString() == "1")
                        isSend = true;
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
                        continue;

                    fromNodeID = item.NodeID;
                    break;
                }
                if (fromNodeID == 0)
                    throw new Exception("@没有找到它的上一步工作。");

                string paras = BP.WF.Dev2Interface.GetFlowParas(fromNodeID, workid);
                if (string.IsNullOrEmpty(paras) || paras.Contains("@" + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + "=") == false)
                    throw new Exception("@流程设计错误:在当前节点上个您设置了按开发者参数决定是否回执，但是没有找到该参数。");

                // 开发者参数.
                if (paras.Contains("@" + BP.WF.WorkSysFieldAttr.SysIsReadReceipts + "=1") == true)
                    isSend = true;
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
                    "您发送的工作已经被" + WebUser.Name + "在" + DataType.CurrentDataTimeCNOfShort + " 打开.",
                    "RP" + workid + "_" + nodeID, BP.WF.SMSMsgType.Self, nd.FK_Flow, nd.NodeID, workid, 0);
            }

            //执行节点打开后事件.
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.RetrieveFromDBSources();
            nd.HisFlow.DoFlowEventEntity(EventListOfNode.WhenReadWork, nd, wk, null);

        }
        /// <summary>
        /// 设置工作未读取
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="userNo">要设置的人</param>
        public static void Node_SetWorkUnRead(int nodeID, Int64 workid, string userNo)
        {
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsRead=0 WHERE WorkID=" + dbstr + "WorkID AND FK_Node=" + dbstr + "FK_Node AND FK_Emp=" + dbstr + "FK_Emp";
            ps.Add("WorkID", workid);
            ps.Add("FK_Node", nodeID);
            ps.Add("FK_Emp", userNo);
            DBAccess.RunSQL(ps);
        }
        /// <summary>
        /// 设置工作未读取
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <param name="workid">工作ID</param>
        public static void Node_SetWorkUnRead(int nodeID, Int64 workid)
        {
            Node_SetWorkUnRead(nodeID, workid, BP.Web.WebUser.No);
        }
        #endregion 工作有关接口

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
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            Flow fl = new Flow(fk_flow);
            if (attr1 != null)
                fl.SetValByKey(attr1, v1);
            if (attr2 != null)
                fl.SetValByKey(attr2, v2);
            fl.Update();
            return "修改成功";
        }
        #endregion 流程属性与节点属性变更接口.

        #region UI 接口
        /// <summary>
        /// 获取按钮状态
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">流程ID</param>
        /// <returns>返回按钮状态</returns>
        public static ButtonState UI_GetButtonState(string fk_flow, int fk_node, Int64 workid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            ButtonState bs = new ButtonState(fk_flow, fk_node, workid);
            return bs;
        }
        /// <summary>
        /// 打开退回窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Return(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            string url = Glo.CCFlowAppPath + "WF/WorkOpt/ReturnWork.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid;
            System.Web.HttpContext.Current.Response.Redirect(url, true);
            return;
        }
        /// <summary>
        /// 打开抄送窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_CC(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WorkOpt/CC.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                800, 600);
        }
        /// <summary>
        /// 打开加签窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_AskForHelp(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            string tKey = DateTime.Now.ToString("MMddhhmmss");
            string urlr3 = Glo.CCFlowAppPath + "WF/WorkOpt/Askfor.aspx?FK_Node=" + fk_node + "&FID=" + fid + "&WorkID=" + workid + "&FK_Flow=" + fk_flow + "&s=" + tKey;
            PubClass.WinOpen(urlr3, 800, 600);
        }
        /// <summary>
        /// 打开挂起窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_HungUp(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WorkOpt/HungUp.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开催办窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Hurry(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/Hurry.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开跳转窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_JumpWay(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/JumpWaySmallSingle.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开流程轨迹窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="nodeID">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_FlowChartTruck(string fk_flow, int nodeID, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fk_flow + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 下一步工作的接受人
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_Accepter(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);

            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WorkOpt/Accepter.aspx?FK_Flow=" + fk_flow + "&FK_Node=" + fk_node + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 打开流程图窗口
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="fk_node">当前节点编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_FlowChart(string fk_flow)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?FK_Flow=" + fk_flow,
                500, 400);
        }
        /// <summary>
        /// 打开OneWork
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fid">流程ID</param>
        public static void UI_Window_OneWork(string fk_flow, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            PubClass.WinOpen(Glo.CCFlowAppPath + "WF/WorkOpt/OneWork/Track.aspx?FK_Flow=" + fk_flow + "&WorkID=" + workid + "&FID=" + fid,
                500, 400);
        }
        /// <summary>
        /// 查看子线程信息
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="fk_node"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        public static void UI_Window_ThreadInfo(string fk_flow, int fk_node, Int64 workid, Int64 fid)
        {
            // 转化成编号.
            fk_flow = TurnFlowMarkToFlowNo(fk_flow);
            string key = DateTime.Now.ToString("yyyyMMddhhmmss");
            string url = Glo.CCFlowAppPath + "WF/ThreadDtl.aspx?FK_Node=" + fk_node + "&FID=" + fid + "&WorkID=" + workid + "&FK_Flow=" + fk_flow + "&s=" + key;
            PubClass.WinOpen(url, 500, 400);
        }
        #endregion UI 接口

        #region ccform 接口
        /// <summary>
        /// SDK签章接口
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="nodeid">签章节点ID</param>
        /// <param name="deptno">部门编号</param>
        /// <param name="stationno">岗位编号</param>
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

                if (File.Exists(BP.Sys.Glo.Request.MapPath(sealimg)) == false)
                {
                    return @"签章文件：" + sealimg + "不存在，请联系管理员！";
                }

                FrmEleDB athDB_N = new FrmEleDB();
                athDB_N.FK_MapData = "ND" + nodeid;
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
            GERpt rpt = fl.HisGERpt;
            rpt.OID = FID == 0 ? workid : FID;
            rpt.Retrieve();

            //首先输出普通的节点 
            foreach (Node mynd in toNDs)
            {
                if (mynd.HisRunModel == RunModel.SubThread)
                    continue; //如果是子线程节点.

                #region 判断方向条件,如果设置了方向条件，判断是否可以通过，不能通过的，就不让其显示.
                Cond cond = new Cond();
                int i = cond.Retrieve(CondAttr.FK_Node, nd.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                // 设置方向条件，就判断它。
                if (i > 0)
                {
                    cond.WorkID = workid;
                    cond.en = rpt;
                    if (cond.IsPassed == false)
                        continue;
                }
                #endregion

                nds.AddEntity(mynd);
            }

            //同表单子线程.
            foreach (Node mynd in toNDs)
            {
                if (mynd.HisRunModel != RunModel.SubThread)
                    continue; //如果是子线程节点.

                if (mynd.HisSubThreadType == SubThreadType.UnSameSheet)
                    continue; //如果是异表单的分合流.

                #region 判断方向条件,如果设置了方向条件，判断是否可以通过，不能通过的，就不让其显示.
                Cond cond = new Cond();
                int i = cond.Retrieve(CondAttr.FK_Node, nd.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                // 设置方向条件，就判断它。
                if (i > 0)
                {
                    cond.WorkID = workid;
                    cond.en = rpt;
                    if (cond.IsPassed == false)
                        continue;
                }
                #endregion


                nds.AddEntity(mynd);
            }

            // 检查是否具有异表单的子线程.
            bool isHave = false;
            foreach (Node mynd in toNDs)
            {
                if (mynd.HisSubThreadType == SubThreadType.UnSameSheet)
                    isHave = true;
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
                    if (mynd.HisSubThreadType != SubThreadType.UnSameSheet)
                        continue;

                    #region 判断方向条件,如果设置了方向条件，判断是否可以通过，不能通过的，就不让其显示.
                    Cond cond = new Cond();
                    int i = cond.Retrieve(CondAttr.FK_Node, nd.NodeID, CondAttr.ToNodeID, mynd.NodeID);
                    // 设置方向条件，就判断它。
                    if (i > 0)
                    {
                        cond.WorkID = workid;
                        cond.en = rpt;
                        if (cond.IsPassed == false)
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
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.Access:
                    sql = "SELECT TOP 1 NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubFlowForward + ")  ORDER BY RDT DESC";
                    break;
                case DBType.Oracle:
                    sql = "SELECT NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE  RowNum=1 AND EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubFlowForward + ")  ORDER BY RDT DESC";
                    break;
                case DBType.MySQL:
                    sql = "SELECT NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE  limit 0,1  AND EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubFlowForward + ")  ORDER BY RDT DESC";
                    break;
                case DBType.Informix:
                    sql = "SELECT first 1 NDTo FROM ND" + int.Parse(flowNo) + "Track WHERE EmpFrom='" + BP.Web.WebUser.No + "' AND NDFrom=" + nodeID + " AND (ActionType=" + (int)ActionType.Forward + " OR ActionType=" + (int)ActionType.ForwardFL + " OR ActionType=" + (int)ActionType.SubFlowForward + ")  ORDER BY RDT DESC";
                    break;
                default:
                    throw new Exception("@没有实现该类型的数据库支持.");
            }
            return BP.DA.DBAccess.RunSQLReturnValInt(sql, 0);
        }
        /// <summary>
        /// 发送到节点
        /// </summary>
        /// <param name="flowNo"></param>
        /// <param name="node"></param>
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

            // 以下代码是从 MyFlow.aspx Send 方法copy 过来的，需要保持业务逻辑的一致性，所以代码需要保持一致.
            WorkNode firstwn = new WorkNode(wk, nd);
            string msg = "";
            SendReturnObjs objs = firstwn.NodeSend();
            return objs;
        }
        /// <summary>
        /// 获得接收人的数据源
        /// </summary>
        /// <param name="FK_Flow">流程编号</param>
        /// <param name="ToNode">到达节点ID</param>
        /// <param name="WorkID">工作ID</param>
        /// <param name="FID">流程ID</param>
        /// <returns></returns>
        public static DataSet WorkOpt_AccepterDB(string FK_Flow, int ToNode, Int64 WorkID, Int64 FID)
        {
            DataSet ds = new DataSet();
            Selector MySelector = new Selector(ToNode);
            switch (MySelector.SelectorModel)
            {
                case SelectorModel.Station:
                    DataTable dt = WorkOpt_Accepter_ByStation(ToNode);
                    dt.TableName = "Port_Emp";
                    ds.Tables.Add(dt);
                    //部门表
                    // string sql = "SELECT * FROM Port_Dept ";
                    // DataTable dt1 = DBAccess.RunSQLReturnTable(sql);
                    // dt1.TableName = "Port_Dept";
                    // ds.Tables.Add(dt1);
                    break;
                case SelectorModel.SQL:
                    ds = WorkOpt_Accepter_BySQL(ToNode);
                    break;
                case SelectorModel.Dept:
                    ds = WorkOpt_Accepter_ByDept(ToNode);
                    break;
                case SelectorModel.Emp:
                    ds = WorkOpt_Accepter_ByEmp(ToNode);
                    break;
                case SelectorModel.Url:
                default:
                    break;
            }
            return ds;
        }
        /// <summary>
        /// 获取节点绑定岗位人员
        /// </summary>
        /// <param name="ToNode"></param>
        /// <returns></returns>
        public static DataTable WorkOpt_Accepter_ByStation(int ToNode)
        {
            if (ToNode == 0)
                throw new Exception("@流程设计错误，没有转向的节点。举例说明: 当前是A节点。如果您在A点的属性里启用了[接受人]按钮，那么他的转向节点集合中(就是A可以转到的节点集合比如:A到B，A到C, 那么B,C节点就是转向节点集合)，必须有一个节点是的节点属性的[访问规则]设置为[由上一步发送人员选择]");

            NodeStations stas = new NodeStations(ToNode);
            if (stas.Count == 0)
            {
                BP.WF.Node toNd = new BP.WF.Node(ToNode);
                throw new Exception("@流程设计错误：设计员没有设计节点[" + toNd.Name + "]，接受人的岗位范围。");
            }
            // 优先解决本部门的问题。
            string sql = "";
            if (BP.WF.Glo.OSModel == OSModel.OneMore)
            {
                sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
                sql += "SELECT FK_EMP FROM Port_DeptEmpStation WHERE FK_STATION ";
                sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + ToNode + ") ";
                sql += ") AND a.No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept ='" + WebUser.FK_Dept + "')";
                sql += " ORDER BY FK_DEPT ";
            }
            else
            {
                sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
                sql += "SELECT FK_EMP FROM " + BP.WF.Glo.EmpStation + " WHERE FK_STATION ";
                sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + ToNode + ") ";
                sql += ") AND a.No IN (SELECT FK_Emp FROM Port_EmpDept WHERE FK_Dept ='" + WebUser.FK_Dept + "')";
                sql += " ORDER BY FK_DEPT ";
            }
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                return dt;

            //组织结构中所有岗位人员
            sql = "SELECT A.No,A.Name, A.FK_Dept, B.Name as DeptName FROM Port_Emp A,Port_Dept B WHERE A.FK_Dept=B.No AND a.NO IN ( ";
            sql += "SELECT FK_EMP FROM " + BP.WF.Glo.EmpStation + " WHERE FK_STATION ";
            sql += "IN (SELECT FK_STATION FROM WF_NodeStation WHERE FK_Node=" + ToNode + ") ";
            sql += ") ORDER BY FK_DEPT ";
            return BP.DA.DBAccess.RunSQLReturnTable(sql);
        }

        /// <summary>
        /// 按sql方式
        /// </summary>
        public static DataSet WorkOpt_Accepter_BySQL(int ToNode)
        {
            DataSet ds = new DataSet();
            Selector MySelector = new Selector(ToNode);
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
        /// <param name="ToNode"></param>
        /// <returns></returns>
        public static DataSet WorkOpt_Accepter_ByDept(int ToNode)
        {
            DataSet ds = new DataSet();
            string sqlGroup = "SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node='" + ToNode + "')";
            string sqlDB = "SELECT No,Name, FK_Dept FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node='" + ToNode + "')";

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
        public static DataSet WorkOpt_Accepter_ByEmp(int ToNode)
        {
            string sqlGroup = "SELECT No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM Port_Emp WHERE No in(SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node='" + ToNode + "'))";
            string sqlDB = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE No in (SELECT FK_EMP FROM WF_NodeEmp WHERE FK_Node='" + ToNode + "')";

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
        public static void WorkOpt_SetAccepter(int toNode, Int64 workid, Int64 fid, string emps, bool isNextTime)
        {
            SelectAccpers ens = new SelectAccpers();
            ens.Delete(SelectAccperAttr.FK_Node, toNode,
                SelectAccperAttr.WorkID, workid);

            //下次是否记忆选择，清空掉。
            string sql = "UPDATE WF_SelectAccper SET " + SelectAccperAttr.IsRemember + " = 0 WHERE Rec='" + WebUser.No + "' AND IsRemember=1 AND FK_Node=" + toNode;
            BP.DA.DBAccess.RunSQL(sql);

            //开始执行保存.
            string[] strs = emps.Split(',');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                SelectAccper en = new SelectAccper();
                en.MyPK = toNode + "_" + workid + "_" + str;
                en.FK_Emp = str;
                en.FK_Node = toNode;
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
        /// <param name="node"></param>
        /// <param name="workid"></param>
        /// <param name="fid"></param>
        /// <param name="toNodes"></param>
        public static SendReturnObjs WorkOpt_SendToEmps(string flowNo, int nodeID, Int64 workid, Int64 fid,
            int toNodeID, string toEmps, bool isRememberMe)
        {
            WorkOpt_SetAccepter(toNodeID, workid, fid, toEmps, isRememberMe);

            Node nd = new Node(nodeID);
            Work wk = nd.HisWork;
            wk.OID = workid;
            wk.Retrieve();

            // 以下代码是从 MyFlow.aspx Send 方法copy 过来的，需要保持业务逻辑的一致性，所以代码需要保持一致.
            WorkNode firstwn = new WorkNode(wk, nd);
            string msg = "";
            SendReturnObjs objs = firstwn.NodeSend();
            return objs;
        }
        #endregion

        #region 附件上传
        public static string SaveImageAsFile(byte[] img, string pkval, string fk_Frm_Ele)
        {
            FrmEle fe = new FrmEle(fk_Frm_Ele);
            System.Drawing.Image newImage;
            using (MemoryStream ms = new MemoryStream(img, 0, img.Length))
            {
                ms.Write(img, 0, img.Length);
                newImage = Image.FromStream(ms, true);
                Bitmap bitmap = new Bitmap(newImage, new Size(fe.WOfInt, fe.HOfInt));

                if (System.IO.Directory.Exists(fe.HandSigantureSavePath + "\\" + fe.FK_MapData + "\\") == false)
                    System.IO.Directory.CreateDirectory(fe.HandSigantureSavePath + "\\" + fe.FK_MapData + "\\");

                string saveTo = fe.HandSigantureSavePath + "\\" + fe.FK_MapData + "\\" + pkval + ".jpg";
                bitmap.Save(saveTo, ImageFormat.Jpeg);

                string pathFile = BP.Sys.Glo.Request.ApplicationPath + fe.HandSiganture_UrlPath + fe.FK_MapData + "/" + pkval + ".jpg";
                FrmEleDB ele = new FrmEleDB();
                ele.FK_MapData = fe.FK_MapData;
                ele.EleID = fe.EleID;
                ele.RefPKVal = pkval;
                ele.Tag1 = pathFile.Replace("\\\\", "\\");
                ele.Tag1 = pathFile.Replace("////", "//");

                ele.Tag2 = saveTo.Replace("\\\\", "\\");
                ele.Tag2 = saveTo.Replace("////", "//");

                ele.GenerPKVal();
                ele.Save();

                return pathFile;
            }
        }

        /// <summary>
        /// 上传文件.
        /// </summary>
        /// <param name="FileByte"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string UploadFile(byte[] FileByte, String fileName)
        {
            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\DataUser\\UploadFile";
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            string filePath = path + "\\" + fileName;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            //这里使用绝对路径来索引
            FileStream stream = new FileStream(filePath, FileMode.CreateNew);
            stream.Write(FileByte, 0, FileByte.Length);
            stream.Close();

            return filePath;
        }

        #endregion

        #region 调度相关的操作.
        /// <summary>
        /// 更新时间状态, 交付给 huangzhimin.
        /// 作用：按照当前的时间，每天两次更新WF_GenerWorkFlow 的 TodoSta 状态字段。
        /// 该字段： 0=正常, 1=预警, 2=逾期, 3=按时完成 , 4=逾期完成.
        /// 该方法作用是，每天，中午时间段，与下午时间段，执行更新这两个状态，仅仅更新两次。
        /// </summary>
        public static void DTS_GenerWorkFlowTodoSta()
        {
            // 中午的更新, 与发送邮件通知.

            #region 求出是否可以更新状态.
            if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 12 )
            {
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

            if (DateTime.Now.Hour >= 13 && DateTime.Now.Hour < 24)
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

            string  timeDT = DateTime.Now.ToString("yyyy-MM-dd");

            //更新预警状态.
            string sql = "UPDATE WF_GenerWorkFlow  SET TodoSta=1 ";
            sql += " WHERE WorkID IN (SELECT WorkID FROM WF_GenerWorkerlist A WHERE a.DTOfWarning >'" + timeDT + "' AND a.SDT <'" + timeDT + "' AND A.IsPass=0 ) ";
            sql += " AND WF_GenerWorkFlow.WFState!=3 ";
            sql += " AND WF_GenerWorkFlow.TodoSta=0 ";
            int i= BP.DA.DBAccess.RunSQL(sql);

            //更新逾期期状态.
            sql = "UPDATE WF_GenerWorkFlow  SET TodoSta=2 ";
            sql += " WHERE WorkID IN (SELECT WorkID FROM WF_GenerWorkerlist A WHERE a.DTOfWarning >'" + timeDT + "' AND a.SDT <'" + timeDT + "' AND A.IsPass=0 ) ";
            sql += " AND WF_GenerWorkFlow.WFState!=3 ";
            sql += " AND WF_GenerWorkFlow.TodoSta=1 ";
            BP.DA.DBAccess.RunSQL(sql);
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
                if (time == null)
                {
                    /*没有数据就说明没有执行过.*/

                    GloVar var = new GloVar();
                    var.No = timeKey;
                    var.Name = "预警信息发送 DTSWarningPM 调度";
                    var.GroupKey = "WF";
                    var.Val = timeKey;  //更新时间点.
                    var.Note = "预警信息发送PM" + timeKey;
                    var.Insert();


                    /*查找一天预警1次的消息记录，并执行推送。*/
                    string sql = "SELECT A.WorkID, A.Title, A.FlowName, A.TodoSta, B.FK_Emp, b.FK_EmpText, C.WAlertWay  FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B, WF_Node C  ";
                   sql += " WHERE A.WorkID=B.WorkID AND A.FK_Node=C.NodeID AND a.TodoSta=1 AND ( C.WAlertRole=1 OR C.WAlertRole=2 ) ";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        CHAlertWay way = (CHAlertWay)int.Parse(dr["WAlertWay"].ToString()); //提醒方式.
                        Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                        string title = dr["Title"].ToString();
                        string flowName = dr["FlowName"].ToString();
                        string empNo = dr["FK_Emp"].ToString();
                        string empName = dr["FK_EmpText"].ToString();

                        BP.WF.Port.WFEmp emp = new Port.WFEmp(empNo);

                        if (way == CHAlertWay.ByEmail)
                        {
                            string titleMail = "";
                            string docMail = "";
                          //  BP.WF.Dev2Interface.Port_SendEmail(emp.Email, titleMail, "");
                        }

                        if (way == CHAlertWay.BySMS)
                        {
                            string titleMail = "";
                            string docMail = "";
                             //BP.WF.Dev2Interface.Port_SendMsg(emp.Email, titleMail, "");
                        }
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 生成工作的 TimeSpan
        /// </summary>
        public static void DTS_GenerWorkFlowTimeSpan()
        {
            //只能在周1执行.
            DateTime dtNow = DateTime.Now;

            //设置为开始的日期为周1.
            DateTime dtBegin = DateTime.Now;

            dtBegin = dtBegin.AddDays(-7);
            for (int i = 0; i < 8; i++)
            {
                if (dtBegin.DayOfWeek == DayOfWeek.Monday)
                    break;
                dtBegin = dtBegin.AddDays(-1);
            }

            //结束日期为当前.
            DateTime dtEnd = dtBegin.AddDays(7);

            //设置为上周.
            string sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.NextWeek + " WHERE RDT >= '" + dtBegin.ToString(DataType.SysDataFormat) + " 00:00' AND RDT <= '" + dtEnd.ToString(DataType.SysDataFormat) + " 00:00' AND TSpan=0";
            BP.DA.DBAccess.RunSQL(sql);

            dtBegin = dtBegin.AddDays(-7);
            dtEnd = dtEnd.AddDays(-7);

            //把上周的，设置为两个周以前.
            sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.TowWeekAgo + " WHERE RDT >= '" + dtBegin.ToString(DataType.SysDataFormat) + " 00:00' AND RDT <= '" + dtEnd.ToString(DataType.SysDataFormat) + " 00:00' AND (TSpan=1 OR TSpan=0) ";
            BP.DA.DBAccess.RunSQL(sql);

            //把上周的，设置为更早.
            sql = "UPDATE WF_GenerWorkFlow SET TSpan=" + (int)TSpan.More + " WHERE RDT <= '" + dtBegin.ToString(DataType.SysDataFormat) + " 00:00' AND (TSpan=2 OR TSpan=0)";
            BP.DA.DBAccess.RunSQL(sql);
        }
        #endregion
    }

}
