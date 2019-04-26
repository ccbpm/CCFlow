using System;
using BP.En;
using BP.Web;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Sys;
using BP.WF.Template;
using BP.WF.Data;

namespace BP.WF
{
    /// <summary>
    /// WF 的摘要说明。
    /// 工作流
    /// 这里包含了两个方面
    /// 工作的信息．
    /// 流程的信息．
    /// </summary>
    public class WorkFlow
    {
        #region 当前工作统计信息
        /// <summary>
        /// 正常范围的运行的个数。
        /// </summary>
        public static int NumOfRuning(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=0";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 进入警告期限的个数
        /// </summary>
        public static int NumOfAlert(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=1";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        /// <summary>
        /// 逾期
        /// </summary>
        public static int NumOfTimeout(string FK_Emp)
        {
            string sql = "SELECT COUNT(*) FROM V_WF_CURRWROKS WHERE FK_Emp='" + FK_Emp + "' AND WorkTimeState=2";
            return DBAccess.RunSQLReturnValInt(sql);
        }
        #endregion

        #region  权限管理
        /// <summary>
        /// 是不是能够作当前的工作。
        /// </summary>
        /// <param name="empId">工作人员ID</param>
        /// <returns>是不是能够作当前的工作</returns>
        public bool IsCanDoCurrentWork(string empId)
        {
            WorkNode wn = this.GetCurrentWorkNode();
            return BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(wn.WorkID, empId);
            #region 使用dev2InterFace 中的算法
            //return true;
            // 找到当前的工作节点

            // 判断是不是开始工作节点..
            if (wn.HisNode.IsStartNode)
            {
                // 从物理上判断是不是有这个权限。
                // return WorkFlow.IsCanDoWorkCheckByEmpStation(wn.HisNode.NodeID, empId);
                return true;
            }

            // 判断他的工作生成的工作者.
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID, wn.HisNode.NodeID);
            if (gwls.Count == 0)
            {
                //return true;
                //throw new Exception("@工作流程定义错误,没有找到能够执行此项工作的人员.相关信息:工作ID="+this.WorkID+",节点ID="+wn.HisNode.NodeID );
                throw new Exception("@工作流程定义错误,没有找到能够执行此项工作的人员.相关信息:WorkID=" + this.WorkID + ",NodeID=" + wn.HisNode.NodeID);
            }

            foreach (GenerWorkerList en in gwls)
            {
                if (en.FK_Emp == empId)
                    return true;
            }
            return false;
            #endregion
        }
        #endregion

        #region 流程公共方法
        /// <summary>
        /// 执行驳回
        /// 应用场景:子流程向分合点驳回时
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="fk_node">被驳回的节点</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string DoReject(Int64 fid, int fk_node, string msg)
        {
            GenerWorkerList wl = new GenerWorkerList();
            int i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, fk_node);

            //if (i == 0)
            //    throw new Exception("系统错误，没有找到应该找到的数据。");

            i = wl.Delete();
            //if (i == 0)
            //    throw new Exception("系统错误，没有删除应该删除的数据。");

            wl = new GenerWorkerList();
            i = wl.Retrieve(GenerWorkerListAttr.FID, fid,
                GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.IsPass, 3);

            //if (i == 0)
            //    throw new Exception("系统错误，想找到退回的原始起点没有找到。");

            Node nd = new Node(fk_node);
            // 更新当前流程管理表的设置当前的节点。
            DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET FK_Node=" + fk_node + ", NodeName='" + nd.Name + "' WHERE WorkID=" + this.WorkID);

            wl.IsPass = false;
            wl.Update();

            return "工作已经驳回到(" + wl.FK_Emp + " , " + wl.FK_EmpText + ")";
            // wl.HisNode
        }
        /// <summary>
        /// 逻辑删除流程
        /// </summary>
        /// <param name="msg">逻辑删除流程原因，可以为空。</param>
        public void DoDeleteWorkFlowByFlag(string msg)
        {
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

                BP.WF.Node nd = new Node(gwf.FK_Node);
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.RetrieveFromDBSources();

                //调用结束前事件.
                this.HisFlow.DoFlowEventEntity(EventListOfNode.FlowOverBefore, nd, wk, null);

                //设置产生的工作流程为.
                gwf.WFState = BP.WF.WFState.Delete;
                gwf.Update();

                //记录日志 感谢 itdos and 888 , 提出了这个bug.
                WorkNode wn = new WorkNode(WorkID, gwf.FK_Node);
                wn.AddToTrack(ActionType.DeleteFlowByFlag, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                        msg);

                //更新-流程数据表的状态. 
                string sql = "UPDATE  " + this.HisFlow.PTable + " SET WFState=" + (int)WFState.Delete + " WHERE OID=" + this.WorkID;
                DBAccess.RunSQL(sql);

                //删除他的工作者，不让其有待办.
                sql = "DELETE FROM WF_GenerWorkerList WHERE WorkID=" + this.WorkID;
                DBAccess.RunSQL(sql);

                //调用结束后事件.
                this.HisFlow.DoFlowEventEntity(EventListOfNode.FlowOverAfter, nd, wk, null);

            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLine(LogType.Error, "@逻辑删除出现错误:" + ex.Message);
                throw new Exception("@逻辑删除出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 恢复逻辑删除流程
        /// </summary>
        /// <param name="msg">回复原因,可以为空.</param>
        public void DoUnDeleteWorkFlowByFlag(string msg)
        {
            try
            {
                DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=" + (int)WFState.Runing + " WHERE  WorkID=" + this.WorkID);

                //设置产生的工作流程为.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                gwf.WFState = BP.WF.WFState.Runing;
                gwf.Update();

                WorkNode wn = new WorkNode(WorkID, gwf.FK_Node);
                wn.AddToTrack(ActionType.UnDeleteFlowByFlag, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name,
                        msg);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLine(LogType.Error, "@逻辑删除出现错误:" + ex.Message);
                throw new Exception("@逻辑删除出现错误:" + ex.Message);
            }
        }
        /// <summary>
        /// 删除已经完成的流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workID">工作ID</param>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <param name="note">删除原因</param>
        /// <returns>删除信息</returns>
        public static string DoDeleteWorkFlowAlreadyComplete(string flowNo, Int64 workID, bool isDelSubFlow, string note)
        {
            Log.DebugWriteInfo("开始删除流程:流程编号:" + flowNo + "-WorkID:" + workID + "-" + ". 是否要删除子流程:" + isDelSubFlow + ";删除原因:" + note);

            Flow fl = new Flow(flowNo);

            #region 记录流程删除日志
            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt");
            rpt.SetValByKey(GERptAttr.OID, workID);
            rpt.Retrieve();
            WorkFlowDeleteLog log = new WorkFlowDeleteLog();
            log.OID = workID;
            try
            {
                log.Copy(rpt);
                log.DeleteDT = DataType.CurrentDataTime;
                log.OperDept = WebUser.FK_Dept;
                log.OperDeptName = WebUser.FK_DeptName;
                log.Oper = WebUser.No;
                log.DeleteNote = note;
                log.OID = workID;
                log.FK_Flow = flowNo;
                log.FK_FlowSort = fl.FK_FlowSort;
                log.InsertAsOID(log.OID);
            }
            catch (Exception ex)
            {
                log.CheckPhysicsTable();
                log.Delete();
                return ex.StackTrace;
            }
            #endregion 记录流程删除日志

            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workID);
            DBAccess.RunSQL("DELETE FROM " + fl.PTable + " WHERE OID=" + workID);
            DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE  WorkID=" + workID); // 删除质量考核数据。

            string info = "";

            #region 正常的删除信息.
            string msg = "";
            try
            {
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workID);

                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + workID);
                // 删除退回.
                DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + workID);
                // 删除移交.
                // DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + workID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + workID + " OR FID=" + workID + " ) AND FK_Flow='" + flowNo + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + workID + " OR FID=" + workID + " ) AND FK_Flow='" + flowNo + "'");

                //删除所有节点上的数据.
                Nodes nds = fl.HisNodes;
                foreach (Node nd in nds)
                {
                    try
                    {
                        DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + workID + " OR FID=" + workID);
                    }
                    catch (Exception ex)
                    {
                        msg += "@ delete data error " + ex.Message;
                    }
                }
                if (msg != "")
                {
                    Log.DebugWriteInfo(msg);
                }
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程 Err " + ex.TargetSite;
                Log.DefaultLogWriteLine(LogType.Error, err);
                throw new Exception(err);
            }
            info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 删除该流程下面的子流程.
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, workID);
                foreach (GenerWorkFlow item in gwfs)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
            }
            #endregion 删除该流程下面的子流程.

            BP.DA.Log.DefaultLogWriteLineInfo("@[" + fl.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + workID + "]。");
            return "已经完成的流程被您删除成功.";
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns>返回删除结果.</returns>
        private string DoDeleteSubThread()
        {
            WorkNode wn = this.GetCurrentWorkNode();
            Emp empOfWorker = wn.HisWork.RecOfEmp;

            #region 正常的删除信息.
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除质量考核信息.
                DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE WorkID=" + this.WorkID); // 删除质量考核数据。

                // 删除抄送信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);

                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + this.WorkID);
                // 删除退回.
                DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + this.WorkID);
                // 删除移交.
                // DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");

                if (msg != "")
                    Log.DebugWriteInfo(msg);
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisStartWork.OID + "," + this.HisStartWork.Title + "] Err " + ex.Message;
                Log.DefaultLogWriteLine(LogType.Error, err);
                throw new Exception(err);
            }
            string info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (1 == 2)
            {
                /* 目前还没有必要，因为在分流点,才有计算完成率的需求. */
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */
                sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + this.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束", null, null);
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /*说明全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0  WHERE IsPass=3  AND WorkID=" + this.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.FK_Node);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            ///* 解决删除最后一个子流程时要把干流程也要删除。*/
                            //sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" +this.HisGenerWorkFlow +" AND FID=" + this.FID;
                            //int num = DBAccess.RunSQLReturnValInt(sql);
                            //if (num == 0)
                            //{
                            //    /*说明没有子进程，就要把这个流程执行完成。*/
                            //    WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            //    info += "@所有的子线程已经结束。";
                            //    info += "@结束主流程信息。";
                            //    info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束");
                            //}
                            break;
                    }
                }
            }
            #endregion

            #region 写入删除日志.
            wn.AddToTrack(ActionType.DeleteSubThread, empOfWorker.No, empOfWorker.Name,
             wn.HisNode.NodeID,
             wn.HisNode.Name, "子线程被:" + BP.Web.WebUser.Name + "删除.");
            #endregion 写入删除日志.

            return "子线程被删除成功.";
        }
        /// <summary>
        /// 删除已经完成的流程
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="isDelSubFlow">是否删除子流程</param>
        /// <returns>删除错误会抛出异常</returns>
        public static void DeleteFlowByReal(string flowNo, Int64 workid, bool isDelSubFlow)
        {
            BP.WF.Flow fl = new Flow(flowNo);
            //检查流程是否完成，如果没有完成就调用workflow流程删除.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() != 0)
            {
                if (gwf.WFState != WFState.Complete)
                {
                    WorkFlow wf = new WorkFlow(flowNo, workid);
                    wf.DoDeleteWorkFlowByReal(isDelSubFlow);
                    return;
                }
            }

            #region 删除独立表单的数据.
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, flowNo);
            string strs = "";
            foreach (FrmNode nd in fns)
            {
                if (strs.Contains("@" + nd.FK_Frm) == true)
                    continue;

                strs += "@" + nd.FK_Frm + "@";
                try
                {
                    MapData md = new MapData(nd.FK_Frm);
                    DBAccess.RunSQL("DELETE FROM " + md.PTable + " WHERE OID=" + workid);
                }
                catch
                {

                }
            }
            #endregion 删除独立表单的数据.

            //删除流程数据.
            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(flowNo) + "Track WHERE WorkID=" + workid);
            DBAccess.RunSQL("DELETE FROM " + fl.PTable + " WHERE OID=" + workid);
            DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE  WorkID=" + workid); // 删除质量考核数据。

            #region 正常的删除信息.
            BP.DA.Log.DefaultLogWriteLineInfo("@[" + fl.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + workid + "]。");
            string msg = "";

            // 删除单据信息.
            DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + workid);
            // 删除单据信息.
            DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + workid);
            // 删除退回.
            DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + workid);

            //删除它的工作.
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + workid + " OR FID=" + workid + " ) AND FK_Flow='" + flowNo + "'");
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + workid + " OR FID=" + workid + " ) AND FK_Flow='" + flowNo + "'");

            //删除所有节点上的数据.
            Nodes nds = new Nodes(flowNo); // this.HisFlow.HisNodes;
            foreach (Node nd in nds)
            {
                try
                {
                    if (DBAccess.IsExitsObject("ND" + nd.NodeID) == false)
                        continue;

                    DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + workid + " OR FID=" + workid);
                }
                catch (Exception ex)
                {
                    msg += "@ delete data error " + ex.Message;
                }
            }

            if (msg != "")
            {
                Log.DebugWriteInfo(msg);
            }
            #endregion 正常的删除信息.
        }
        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns>删除的消息</returns>
        public string DoDeleteSubThread2015()
        {
            if (this.FID == 0)
                throw new Exception("@该流程非子线程流程实例，不能执行该方法。");

            #region 正常的删除信息.
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除质量考核信息.
                DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE WorkID=" + this.WorkID); // 删除质量考核数据。

                // 删除抄送信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);

                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + this.WorkID);
                // 删除退回.
                DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + this.WorkID);
                // 删除移交.
                // DBAccess.RunSQL("DELETE FROM WF_ForwardWork WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE WorkID=" + this.WorkID);

                if (msg != "")
                    Log.DebugWriteInfo(msg);
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisStartWork.OID + "," + this.HisStartWork.Title + "] Err " + ex.Message;
                Log.DefaultLogWriteLine(LogType.Error, err);
                throw new Exception(err);
            }
            string info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (1 == 2)
            {
                /*
                 * 开发说明：
                 * 1，当前是删除子线程操作,当前的节点就是子线程节点.
                 * 2, 删除子线程的动作，1，合流点。2，分流点。
                 * 3，这里要解决合流节点的完成率的问题.
                 */

#warning 应该删除一个子线程后，就需要计算完成率的问题。但是目前应用到该场景极少,因为。能够看到河流点信息，说明已经到达了完成率了。

                /* 目前还没有必要，因为在分流点,才有计算完成率的需求. */
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */

                sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + this.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + this.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束", null, null);
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /* 说明: 全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0  WHERE IsPass=3  AND WorkID=" + this.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.FK_Node);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            ///* 解决删除最后一个子流程时要把干流程也要删除。*/
                            //sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" +this.HisGenerWorkFlow +" AND FID=" + this.FID;
                            //int num = DBAccess.RunSQLReturnValInt(sql);
                            //if (num == 0)
                            //{
                            //    /*说明没有子进程，就要把这个流程执行完成。*/
                            //    WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            //    info += "@所有的子线程已经结束。";
                            //    info += "@结束主流程信息。";
                            //    info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束");
                            //}
                            break;
                    }
                }
            }
            #endregion



            //检查是否是最后一个子线程被删除了？如果是，就需要当分流节点产生待办.
            GenerWorkFlow gwfMain = new GenerWorkFlow(this.FID);

            /*说明仅仅停留在分流节点,还没有到合流节点上去.
             * 删除子线程的时候，判断是否是最后一个子线程,如果是，就要把他设置为待办状态。
             * 1.首先要找到.
             * 2.xxxx.
             */
            //  string sql = "SELECT COUNT(*) FROM WF_GenerWorkerList WHERE FK_Node=";
            string mysql = "SELECT COUNT(*)  as Num FROM WF_GenerWorkerList WHERE IsPass=0 AND FID=" + this.FID;
            int num = BP.DA.DBAccess.RunSQLReturnValInt(mysql);
            if (num == 0)
            {
                /* 说明当前主流程上是分流节点，但是已经没有子线程的待办了。
                 * 就是说，删除子流程的时候，删除到最后已经没有活动或者已经完成的子线程了.
                 * */

                GenerWorkerList gwl = new GenerWorkerList();
                int i = gwl.Retrieve(GenerWorkerListAttr.FK_Node, gwfMain.FK_Node, GenerWorkerListAttr.WorkID, gwfMain.WorkID,
                    GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
                if (i == 0)
                {
                    Node ndMain = new Node(gwfMain.FK_Node);
                    if (ndMain.IsHL == true)
                    {
                        /* 有可能是当前节点已经到了合流节点上去了, 要判断合流节点是否有代办？如果没有代办，就撤销到分流节点上去.
                         * 
                         * 就要检查他是否有代办.
                         */
                        mysql = "SELECT COUNT(*)  as Num FROM WF_GenerWorkerList WHERE IsPass=0 AND FK_Node=" + gwfMain.FK_Node;
                        num = BP.DA.DBAccess.RunSQLReturnValInt(mysql);
                        if (num == 0)
                        {
                            /*如果没有待办，就说明，当前节点已经运行到合流节点，但是不符合合流节点的完成率，导致合流节点上的人员看不到待办. 
                             * 这种情况，就需要让当前分流节点产生待办.
                             */

                            mysql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE FID=0 AND WorkID=" + gwfMain.WorkID + " ORDER BY RDT DESC ";
                            int fenLiuNodeID = BP.DA.DBAccess.RunSQLReturnValInt(mysql);

                            Node nd = new Node(fenLiuNodeID);
                            if (nd.IsFL == false)
                                throw new Exception("@程序错误，没有找到最近的一个分流节点.");

                            GenerWorkerLists gwls = new GenerWorkerLists();
                            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, fenLiuNodeID);
                            foreach (GenerWorkerList item in gwls)
                            {
                                item.IsRead = false;
                                item.IsPassInt = 0;
                                item.SDT = BP.DA.DataType.CurrentDataTime;
                                item.Update();
                            }
                        }
                    }
                }
                else
                {
                    gwl.IsRead = false;
                    gwl.IsPassInt = 0;
                    gwl.SDT = BP.DA.DataType.CurrentDataTime;
                    gwl.Update();
                    return "子线程被删除成功,这是最后一个删除的子线程已经为您在{" + gwfMain.NodeName + "}产生了待办,<a href='/WF/MyFlow.htm?WorkID=" + gwfMain.WorkID + "&FK_Flow=" + gwfMain.FK_Flow + "'>点击处理工作</a>.";

                }
            }
            return "子线程被删除成功.";
        }

        /// <summary>
        /// 彻底的删除流程
        /// </summary>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <returns>删除的消息</returns>
        public string DoDeleteWorkFlowByReal(bool isDelSubFlow)
        {
            if (this.FID != 0)
                return DoDeleteSubThread2015();

            string info = "";
            WorkNode wn = null;
            try
            {
                wn = this.GetCurrentWorkNode();
            }
            catch (Exception ex)
            {
            }
            // 处理删除前事件。
            if (wn != null)
                wn.HisFlow.DoFlowEventEntity(EventListOfNode.BeforeFlowDel, wn.HisNode, wn.HisWork, null);

            #region 删除独立表单的数据.
            FrmNodes fns = new FrmNodes();
            fns.Retrieve(FrmNodeAttr.FK_Flow, this.HisFlow.No);
            string strs = "";
            foreach (FrmNode nd in fns)
            {
                if (strs.Contains("@" + nd.FK_Frm) == true)
                    continue;

                strs += "@" + nd.FK_Frm + "@";
                try
                {
                    MapData md = new MapData(nd.FK_Frm);
                    DBAccess.RunSQL("DELETE FROM " + md.PTable + " WHERE OID=" + this.WorkID);
                }
                catch
                {
                }
            }
            #endregion 删除独立表单的数据.

            //删除流程数据.
            DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.HisFlow.No) + "Track WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM " + this.HisFlow.PTable + " WHERE OID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE  WorkID=" + this.WorkID); // 删除质量考核数据。

            #region 正常的删除信息.
            BP.DA.Log.DefaultLogWriteLineInfo("@[" + this.HisFlow.Name + "]流程被[" + BP.Web.WebUser.No + BP.Web.WebUser.Name + "]删除，WorkID[" + this.WorkID + "]。");
            string msg = "";
            try
            {
                Int64 workId = this.WorkID;
                string flowNo = this.HisFlow.No;
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程的 ID 与流程编号 出现错误。" + ex.Message);
            }

            try
            {
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_CCList WHERE WorkID=" + this.WorkID);
                // 删除单据信息.
                DBAccess.RunSQL("DELETE FROM WF_Bill WHERE WorkID=" + this.WorkID);
                // 删除退回.
                DBAccess.RunSQL("DELETE FROM WF_ReturnWork WHERE WorkID=" + this.WorkID);

                //删除它的工作.
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + this.WorkID + " OR FID=" + this.WorkID + " ) AND FK_Flow='" + this.HisFlow.No + "'");

                //删除所有节点上的数据.
                Nodes nds = this.HisFlow.HisNodes;
                foreach (Node nd in nds)
                {
                    try
                    {
                        if (DBAccess.IsExitsObject("ND" + nd.NodeID) == false)
                            continue;

                        DBAccess.RunSQL("DELETE FROM ND" + nd.NodeID + " WHERE OID=" + this.WorkID + " OR FID=" + this.WorkID);
                    }
                    catch (Exception ex)
                    {
                        msg += "@ delete data error " + ex.Message;
                    }
                }
                if (msg != "")
                {
                    Log.DebugWriteInfo(msg);
                }
            }
            catch (Exception ex)
            {
                string err = "@删除工作流程[" + this.HisStartWork.OID + "," + this.HisStartWork.Title + "] Err " + ex.Message;
                Log.DefaultLogWriteLine(LogType.Error, err);
                throw new Exception(err);
            }
            info = "@删除流程删除成功";
            #endregion 正常的删除信息.

            #region 处理分流程删除的问题完成率的问题。
            if (this.FID != 0)
            {
                string sql = "";
                /* 
                 * 取出来获取停留点,没有获取到说明没有任何子线程到达合流点的位置.
                 */
                sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + wn.HisWork.FID + " AND IsPass=3";
                int fk_node = DBAccess.RunSQLReturnValInt(sql, 0);
                if (fk_node != 0)
                {
                    /* 说明它是待命的状态 */
                    Node nextNode = new Node(fk_node);
                    if (nextNode.PassRate > 0)
                    {
                        /* 找到等待处理节点的上一个点 */
                        Nodes priNodes = nextNode.FromNodes;
                        if (priNodes.Count != 1)
                            throw new Exception("@没有实现子流程不同线程的需求。");

                        Node priNode = (Node)priNodes[0];

                        #region 处理完成率
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + wn.HisWork.FID + " AND IsPass=1";
                        decimal ok = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + priNode.NodeID + " AND FID=" + wn.HisWork.FID;
                        decimal all = (decimal)DBAccess.RunSQLReturnValInt(sql);
                        if (all == 0)
                        {
                            /*说明:所有的子线程都被杀掉了, 就应该整个流程结束。*/
                            WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                            info += "@所有的子线程已经结束。";
                            info += "@结束主流程信息。";
                            info += "@" + wf.DoFlowOver(ActionType.FlowOver, "合流点流程结束", null, null);
                        }

                        decimal passRate = ok / all * 100;
                        if (nextNode.PassRate <= passRate)
                        {
                            /*说明全部的人员都完成了，就让合流点显示它。*/
                            DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0  WHERE IsPass=3  AND WorkID=" + wn.HisWork.FID + " AND FK_Node=" + fk_node);
                        }
                        #endregion 处理完成率
                    }
                } /* 结束有待命的状态判断。*/

                if (fk_node == 0)
                {
                    /* 说明:没有找到等待启动工作的合流节点. */
                    GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
                    Node fND = new Node(gwf.FK_Node);
                    switch (fND.HisNodeWorkType)
                    {
                        case NodeWorkType.WorkHL: /*主流程运行到合流点上了*/
                            break;
                        default:
                            /* 解决删除最后一个子流程时要把干流程也要删除。*/
                            sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerList WHERE FK_Node=" + wn.HisNode.NodeID + " AND FID=" + wn.HisWork.FID;
                            int num = DBAccess.RunSQLReturnValInt(sql);
                            if (num == 0)
                            {
                                /*说明没有子进程，就要把这个流程执行完成。*/
                                WorkFlow wf = new WorkFlow(this.HisFlow, this.FID);
                                info += "@所有的子线程已经结束。";
                                info += "@结束主流程信息。";
                                info += "@" + wf.DoFlowOver(ActionType.FlowOver, "主流程结束", null, null);
                            }
                            break;
                    }
                }
            }
            #endregion

            #region 删除该流程下面的子流程.
            if (isDelSubFlow)
            {
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);

                foreach (GenerWorkFlow item in gwfs)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
            }
            #endregion 删除该流程下面的子流程.


            // 处理删除hou事件。
            if (wn != null)
                wn.HisFlow.DoFlowEventEntity(EventListOfNode.AfterFlowDel, wn.HisNode, wn.HisWork, null);
            return info;
        }

        /// <summary>
        /// 删除工作流程记录日志，并保留运动轨迹.
        /// </summary>
        /// <param name="isDelSubFlow">是否要删除子流程</param>
        /// <returns></returns>
        public string DoDeleteWorkFlowByWriteLog(string info, bool isDelSubFlow)
        {
            GERpt rpt = new GERpt("ND" + int.Parse(this.HisFlow.No) + "Rpt", this.WorkID);
            WorkFlowDeleteLog log = new WorkFlowDeleteLog();
            log.OID = this.WorkID;
            try
            {
                log.Copy(rpt);
                log.DeleteDT = DataType.CurrentDataTime;
                log.OperDept = WebUser.FK_Dept;
                log.OperDeptName = WebUser.FK_DeptName;
                log.Oper = WebUser.No;
                log.DeleteNote = info;
                log.OID = this.WorkID;
                log.FK_Flow = this.HisFlow.No;
                log.InsertAsOID(log.OID);
                return DoDeleteWorkFlowByReal(isDelSubFlow);
            }
            catch (Exception ex)
            {
                log.CheckPhysicsTable();
                log.Delete();
                return ex.StackTrace;
            }
        }

        #region 流程的强制终止\删除 或者恢复使用流程,
        /// <summary>
        /// 恢复流程.
        /// </summary>
        /// <param name="msg">回复流程的原因</param>
        public void DoComeBackWorkFlow(string msg)
        {
            try
            {
                //设置产生的工作流程为
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                gwf.WFState = WFState.Runing;
                gwf.DirectUpdate();

                // 增加消息 
                WorkNode wn = this.GetCurrentWorkNode();
                GenerWorkerLists wls = new GenerWorkerLists(wn.HisWork.OID, wn.HisNode.NodeID);
                if (wls.Count == 0)
                    throw new Exception("@恢复流程出现错误,产生的工作者列表");
                BP.WF.MsgsManager.AddMsgs(wls, "恢复的流程", wn.HisNode.Name, "回复的流程");
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLine(LogType.Error, "@恢复流程出现错误." + ex.Message);
                throw new Exception("@恢复流程出现错误." + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// 得到当前的进行中的工作。
        /// </summary>
        /// <returns></returns>		 
        public WorkNode GetCurrentWorkNode()
        {
            int currNodeID = 0;
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                this.DoFlowOver(ActionType.FlowOver, "非正常结束，没有找到当前的流程记录。", null, null);
                throw new Exception("@" + string.Format("工作流程{0}已经完成。", this.HisStartWork.Title));
            }

            Node nd = new Node(gwf.FK_Node);
            Work work = nd.HisWork;
            work.OID = this.WorkID;
            work.NodeID = nd.NodeID;
            work.SetValByKey("FK_Dept", BP.Web.WebUser.FK_Dept);
            if (work.RetrieveFromDBSources() == 0)
            {
                Log.DefaultLogWriteLineError("@WorkID=" + this.WorkID + ",FK_Node=" + gwf.FK_Node + ".不应该出现查询不出来工作."); // 没有找到当前的工作节点的数据，流程出现未知的异常。
                work.Rec = BP.Web.WebUser.No;
                try
                {
                    work.Insert();
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineError("@没有找到当前的工作节点的数据，流程出现未知的异常" + ex.Message + ",不应该出现"); // 没有找到当前的工作节点的数据
                }
            }
            work.FID = gwf.FID;

            WorkNode wn = new WorkNode(work, nd);
            return wn;
        }
        /// <summary>
        /// 结束分流的节点
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string DoFlowOverFeiLiu(GenerWorkFlow gwf)
        {
            // 查询出来有少没有完成的流程。
            int i = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) FROM WF_GenerWorkFlow WHERE FID=" + gwf.FID + " AND WFState!=1");
            switch (i)
            {
                case 0:
                    throw new Exception("@不应该的错误。");
                case 1:
                    BP.DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow  WHERE FID=" + gwf.FID + " OR WorkID=" + gwf.FID);
                    BP.DA.DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE FID=" + gwf.FID + " OR WorkID=" + gwf.FID);

                    StartWork wk = this.HisFlow.HisStartNode.HisWork as StartWork;
                    wk.OID = gwf.FID;
                    wk.Update();

                    return "@当前的工作已经完成，该流程上所有的工作都已经完成。";
                default:
                    BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkFlow SET WFState=1 WHERE WorkID=" + this.WorkID);
                    BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=1 WHERE WorkID=" + this.WorkID);
                    return "@当前的工作已经完成。";
            }
        }
        /// <summary>
        /// 处理子流程完成.
        /// </summary>
        /// <returns></returns>
        public string DoFlowSubOver()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Node nd = new Node(gwf.FK_Node);

            DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow   WHERE WorkID=" + this.WorkID);
            DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID);

            string sql = "SELECT count(*) FROM WF_GenerWorkFlow WHERE  FID=" + this.FID;
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (DBAccess.RunSQLReturnValInt(sql) == 0)
            {
                /*说明这是最后一个*/
                WorkFlow wf = new WorkFlow(gwf.FK_Flow, this.FID);
                wf.DoFlowOver(ActionType.FlowOver, "子流程结束", null, null);
                return "@当前子流程已完成，主流程已完成。";
            }
            else
            {
                return "@当前子流程已完成，主流程还有(" + num + ")个子流程未完成。";
            }
        }
        /// <summary>
        /// 让父亲流程自动发送到下一步骤上去.
        /// </summary>
        public string LetParentFlowAutoSendNextSetp()
        {
            if (this.HisGenerWorkFlow.PWorkID == 0)
                return "";

            if (this.HisFlow.SubFlowOver == SubFlowOver.None)
            {
                /*让父流程显示待办.*/
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE IsPass=80 AND WorkID="+this.HisGenerWorkFlow.PWorkID);
                return "";
                //return "父流程已经显示待办.";
            }

            //如果是结束子流程.
            if (this.HisFlow.SubFlowOver == SubFlowOver.OverParentFlow)
            {
                BP.WF.Dev2Interface.Flow_DoFlowOver(this.HisGenerWorkFlow.PFlowNo, this.HisGenerWorkFlow.PWorkID, "子流程完成自动结束父流程.");
                return "父流程自动结束.";
            }

            // 检查是否是最后的一个.
            int num = BP.WF.Dev2Interface.Flow_NumOfSubFlowRuning(this.HisGenerWorkFlow.PWorkID, this.HisGenerWorkFlow.WorkID);
            if (num != 0)
                return "";

            //检查父流程是否存在?
            GenerWorkFlow pGWF = new GenerWorkFlow();
            pGWF.WorkID = this.HisGenerWorkFlow.PWorkID;
            if (pGWF.RetrieveFromDBSources() == 0)
                return ""; // 父流程被删除了也不能执行。

            if (pGWF.WFState == WFState.Complete)
                return ""; //父流程已经完成也不能执行.

            //检查父流程的当前停留的节点是否还是发起子流程的节点？
            if (this.HisGenerWorkFlow.PNodeID != pGWF.FK_Node)
                return "";

            //找到调用该流程的人，这里判断不严禁，如果有多个人处理该节点，就只能找到当前人处理了。
            //  string pEmp = DBAccess.RunSQLReturnStringIsNull("SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + this.HisGenerWorkFlow.PWorkID + " AND FK_Node=" + this.HisGenerWorkFlow.FK_Node + " AND IsPass=0", null);
            //NDXRptBaseAttr

            // 因为前面已经对他进行个直接更新所以这里需要进行查询之后在执行更新.
            this.HisGenerWorkFlow.RetrieveFromDBSources();

            //当前登录用户.
            string currUserNo = BP.Web.WebUser.No;
            try
            {
                //取得调起子流程的人员.
                string pEmp = this.HisGenerWorkFlow.PEmp;
                if (DataType.IsNullOrEmpty(pEmp) == true)
                    throw new Exception("@没有找到调起子流程的工作人员.");

                Emp emp = new Emp();
                emp.No = pEmp;
                if (emp.RetrieveFromDBSources() == 0)
                    throw new Exception("@吊起子流程上的人员编号(" + pEmp + ")已不存在,无法启动父流程.");

                //改变当前节点的状态，不然父流程如果做了让所有的子流程发送完成后才能运行的设置后，不能不能让其发送了.
                this.HisGenerWorkFlow.WFState = WFState.Complete;
                this.HisGenerWorkFlow.DirectUpdate();


                //让父流程的userNo登录.
                BP.WF.Dev2Interface.Port_Login(emp.No);

                //@袁丽娜.
                if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork( pGWF.WorkID, WebUser.No) == false)
                {
                    /*没有权限的情况下，就移交给当前人员，让其在发送. */
                    BP.WF.Dev2Interface.Node_Shift( pGWF.WorkID, WebUser.No, "工作自动移交，让其运行到下一步。");
                }

               // GERpt rpt = new GERpt("ND" + int.Parse(this.HisGenerWorkFlow.PFlowNo) + "Rpt", this.HisGenerWorkFlow.PWorkID);

                // 让当前人员向下发送，但是这种发送一定不要检查发送权限，否则的话就出错误，不能发送下去.
                SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.HisGenerWorkFlow.PFlowNo, pGWF.WorkID, null, null, 0, null,
                    emp.No, emp.Name, emp.FK_Dept, emp.FK_DeptText, null);

                this.HisGenerWorkFlow.WFState = WFState.Complete;
                this.HisGenerWorkFlow.DirectUpdate();

                //切换到当前流程节点.
                BP.WF.Dev2Interface.Port_Login(currUserNo);

                return "@成功让父流程运行到下一个节点." + objs.ToMsgOfHtml();
            }
            catch (Exception ex)
            {
                this.HisGenerWorkFlow.WFState = WFState.Complete;
                this.HisGenerWorkFlow.DirectUpdate();

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
        /// 执行流程完成
        /// </summary>
        /// <param name="at"></param>
        /// <param name="stopMsg"></param>
        /// <returns></returns>
        public string DoFlowOver(ActionType at, string stopMsg, Node currNode, GERpt rpt)
        {
            if (null == currNode)
                return "err@当前节点为空..";

            if (DataType.IsNullOrEmpty(stopMsg))
                stopMsg += "流程结束";

            //调用结束前事件.
            string mymsg = this.HisFlow.DoFlowEventEntity(EventListOfNode.FlowOverBefore, currNode, rpt, null);
            if (mymsg != null)
                stopMsg += "@" + mymsg;

            string exp = currNode.FocusField;
            if (DataType.IsNullOrEmpty(exp) == false && exp.Length > 1)
            {
                if (rpt != null)
                    stopMsg += Glo.DealExp(exp, rpt, null);
            }

            if (this.IsMainFlow == false)
            {
                /* 处理子流程完成*/
                stopMsg += this.DoFlowSubOver();
            }

            #region 处理明细表的汇总.
            Node currND = new Node(this.HisGenerWorkFlow.FK_Node);

            //处理明细数据的copy问题。 首先检查：当前节点（最后节点）是否有明细表。
            MapDtls dtls = currND.MapData.MapDtls; // new MapDtls("ND" + nd.NodeID);
            int i = 0;
            foreach (MapDtl dtl in dtls)
            {
                i++;
                // 查询出该明细表中的数据。
                GEDtls dtlDatas = new GEDtls(dtl.No);
                dtlDatas.Retrieve(GEDtlAttr.RefPK, this.WorkID);

                GEDtl geDtl = null;
                try
                {
                    // 创建一个Rpt对象。
                    geDtl = new GEDtl("ND" + int.Parse(this.HisFlow.No) + "RptDtl" + i.ToString());
                    geDtl.ResetDefaultVal();
                }
                catch
                {
#warning 此处需要修复。
                    continue;
                }
            }
            this._IsComplete = 1;
            #endregion 处理明细表的汇总.

            #region 处理后续的业务.

            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            if (1 == 2)
            {
                // 是否删除流程注册表的数据？
                ps = new Paras();
                ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2 ";
                ps.Add("WorkID1", this.WorkID);
                ps.Add("WorkID2", this.WorkID);
                DBAccess.RunSQL(ps);
            }


            // 删除子线程产生的 流程注册信息.
            if (this.FID == 0)
            {
                ps = new Paras();
                ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbstr + "WorkID";
                ps.Add("WorkID", this.WorkID);
                DBAccess.RunSQL(ps);
            }

            // 清除工作者.
            ps = new Paras();
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbstr + "WorkID1 OR FID=" + dbstr + "WorkID2 ";
            ps.Add("WorkID1", this.WorkID);
            ps.Add("WorkID2", this.WorkID);
            DBAccess.RunSQL(ps);

            //把当前的人员字符串加入到参与人里面去,以方便查询.
            string emps = WebUser.No + "@";

            // 设置流程完成状态.
            ps = new Paras();
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
                ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET  FlowEmps= FlowEmps ||'" + emps + "', WFState=:WFState,WFSta=:WFSta WHERE OID=" + dbstr + "OID";
            else if (SystemConfig.AppCenterDBType == DBType.MySQL)
                ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET FlowEmps= CONCAT(FlowEmps ,'" + emps + "'), WFState=@WFState,WFSta=@WFSta WHERE OID=" + dbstr + "OID";
            else
                ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET FlowEmps= FlowEmps + '" + emps + "', WFState=" + dbstr + "WFState,WFSta=" + dbstr + "WFSta WHERE OID=" + dbstr + "OID";

            ps.Add("WFState", (int)WFState.Complete);
            ps.Add("WFSta", (int)WFSta.Complete);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);

            //加入轨迹.
            WorkNode wn = new WorkNode(WorkID, this.HisGenerWorkFlow.FK_Node);
            wn.AddToTrack(at, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, stopMsg);

            //执行流程结束.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.Emps += emps;
            gwf.WFState = WFState.Complete;
            gwf.Update();


            //调用结束后事件.
            stopMsg += this.HisFlow.DoFlowEventEntity(EventListOfNode.FlowOverAfter, currNode, rpt, null);
            #endregion 处理后续的业务.

            //执行最后一个子流程发送后的检查，不管是否成功，都要结束该流程。
            stopMsg += this.LetParentFlowAutoSendNextSetp();

            //string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;

            #region 处理审核问题,更新审核组件插入的审核意见中的 到节点，到人员。
            ps = new Paras();
            ps.SQL = "UPDATE ND" + int.Parse(currNode.FK_Flow) + "Track SET NDTo=" + dbstr + "NDTo,NDToT=" + dbstr + "NDToT,EmpTo=" + dbstr + "EmpTo,EmpToT=" + dbstr + "EmpToT WHERE NDFrom=" + dbstr + "NDFrom AND EmpFrom=" + dbstr + "EmpFrom AND WorkID=" + dbstr + "WorkID AND ActionType=" + (int)ActionType.WorkCheck;
            ps.Add(TrackAttr.NDTo, currNode.NodeID);
            ps.Add(TrackAttr.NDToT, "");
            ps.Add(TrackAttr.EmpTo, "");
            ps.Add(TrackAttr.EmpToT, "");

            ps.Add(TrackAttr.NDFrom, currNode.NodeID);
            ps.Add(TrackAttr.EmpFrom, WebUser.No);
            ps.Add(TrackAttr.WorkID, this.WorkID);
            BP.DA.DBAccess.RunSQL(ps);
            #endregion 处理审核问题.

            
     
            return stopMsg;
        }
        public string GenerFHStartWorkInfo()
        {
            string msg = "";
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT Title,RDT,Rec,OID FROM ND" + this.StartNodeID + " WHERE FID=" + this.FID);
            switch (dt.Rows.Count)
            {
                case 0:
                    Node nd = new Node(this.StartNodeID);
                    throw new Exception("@没有找到他们开始节点的数据，流程异常。FID=" + this.FID + "，节点：" + nd.Name + "节点ID：" + nd.NodeID);
                case 1:
                    msg = string.Format("@发起人： {0}  日期：{1} 发起的流程 标题：{2} ，已经成功完成。",
                        dt.Rows[0]["Rec"].ToString(), dt.Rows[0]["RDT"].ToString(), dt.Rows[0]["Title"].ToString());
                    break;
                default:
                    msg = "@下列(" + dt.Rows.Count + ")位人员发起的流程已经完成。";
                    foreach (DataRow dr in dt.Rows)
                    {
                        msg += "<br>发起人：" + dr["Rec"] + " 发起日期：" + dr["RDT"] + " 标题：" + dr["Title"] + "<a href='./../../WF/WFRpt.htm?WorkID=" + dr["OID"] + "&FK_Flow=" + this.HisFlow.No + "' target=_blank>详细...</a>";
                    }
                    break;
            }
            return msg;
        }
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.HisFlow.No + "01");
            }
        }
        /// <summary>
        ///  抄送到
        /// </summary>
        /// <param name="dt"></param>
        public string CCTo(DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return "";

            string emps = "";
            string empsExt = "";

            string ip = "127.0.0.1";
            System.Net.IPAddress[] addressList = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList;
            if (addressList.Length > 1)
                ip = addressList[1].ToString();
            else
                ip = addressList[0].ToString();


            foreach (DataRow dr in dt.Rows)
            {
                string no = dr[0].ToString();
                string name = dr[1].ToString();

                emps += BP.WF.Glo.DealUserInfoShowModel(no, name);
            }

            Paras pss = new Paras();
            pss.Add("Sender", BP.Web.WebUser.No);
            pss.Add("Receivers", emps);
            pss.Add("Title", "工作流抄送：工作名称:" + this.HisFlow.Name + "，最后处理人：" + BP.Web.WebUser.Name);
            pss.Add("Context", "工作轨迹 http://" + ip + "/WF/WFRpt.htm?WorkID=" + this.WorkID + "&FID=0");

            try
            {
                DBAccess.RunSP("CCstaff", pss);
                return "@" + empsExt;
            }
            catch (Exception ex)
            {
                return "@抄送出现错误，没有把该流程的信息抄送到(" + empsExt + ")请联系管理员检查系统异常" + ex.Message;
            }
        }
        /// <summary>
        /// 执行冻结
        /// </summary>
        /// <param name="msg">冻结原因</param>
        public string DoFix(string fixMsg)
        {
            if (this.HisGenerWorkFlow.WFState == WFState.Fix)
                throw new Exception("@当前已经是冻结的状态您不能执行再冻结.");

            if (DataType.IsNullOrEmpty(fixMsg))
                fixMsg = "无";


            ///* 获取它的工作者，向他们发送消息。*/
            //GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);

            //string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.FK_Node;
            //string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            //string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "冻结" + fixMsg;
            //string emps = "";
            //foreach (GenerWorkerList wl in wls)
            //{
            //    if (wl.IsEnable == false)
            //        continue; //不发送给禁用的人。

            //    emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";

            //    //写入消息。
            //    BP.WF.Dev2Interface.Port_SendMsg(wl.FK_Emp, title, mailDoc, "Fix" + wl.WorkID, BP.Sys.SMSMsgType.Etc, wl.FK_Flow, wl.FK_Node, wl.WorkID, wl.FID);
            //}

            /* 执行 WF_GenerWorkFlow 冻结. */
            int sta = (int)WFState.Fix;
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, sta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, sta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.FK_Node);

            //wn.AddToTrack(ActionType.Info, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, fixMsg,);

            return "已经成功执行冻结";
        }
        /// <summary>
        /// 执行解除冻结
        /// </summary>
        /// <param name="msg">冻结原因</param>
        public string DoUnFix(string unFixMsg)
        {
            if (this.HisGenerWorkFlow.WFState != WFState.Fix)
                throw new Exception("@当前非冻结的状态您不能执行解除冻结.");

            if (DataType.IsNullOrEmpty(unFixMsg))
                unFixMsg = "无";


            ///* 获取它的工作者，向他们发送消息。*/
            //GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);

            //string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.FK_Node;
            //string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            //string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "冻结" + unFixMsg;
            //string emps = "";
            //foreach (GenerWorkerList wl in wls)
            //{
            //    if (wl.IsEnable == false)
            //        continue; //不发送给禁用的人。

            //    emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";

            //    //写入消息。
            //    BP.WF.Dev2Interface.Port_SendMsg(wl.FK_Emp, title, mailDoc, "Fix" + wl.WorkID, BP.Sys.SMSMsgType.Self, wl.FK_Flow, wl.FK_Node, wl.WorkID, wl.FID);
            //}

            /* 执行 WF_GenerWorkFlow 冻结. */
            int sta = (int)WFState.Runing;
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, sta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, sta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.FK_Node);
            //wn.AddToTrack(ActionType.Info, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, unFixMsg);

            return "已经成功执行解除冻结:";
        }
        #endregion

        #region 基本属性
        /// <summary>
        /// 他的节点
        /// </summary>
        private Nodes _HisNodes = null;
        /// <summary>
        /// 节点s
        /// </summary>
        public Nodes HisNodes
        {
            get
            {
                if (this._HisNodes == null)
                    this._HisNodes = this.HisFlow.HisNodes;
                return this._HisNodes;
            }
        }
        /// <summary>
        /// 工作节点s(普通的工作节点)
        /// </summary>
        private WorkNodes _HisWorkNodesOfWorkID = null;
        /// <summary>
        /// 工作节点s
        /// </summary>
        public WorkNodes HisWorkNodesOfWorkID
        {
            get
            {
                if (this._HisWorkNodesOfWorkID == null)
                {
                    this._HisWorkNodesOfWorkID = new WorkNodes();
                    this._HisWorkNodesOfWorkID.GenerByWorkID(this.HisFlow, this.WorkID);
                }
                return this._HisWorkNodesOfWorkID;
            }
        }
        /// <summary>
        /// 工作节点s
        /// </summary>
        private WorkNodes _HisWorkNodesOfFID = null;
        /// <summary>
        /// 工作节点s
        /// </summary>
        public WorkNodes HisWorkNodesOfFID
        {
            get
            {
                if (this._HisWorkNodesOfFID == null)
                {
                    this._HisWorkNodesOfFID = new WorkNodes();
                    this._HisWorkNodesOfFID.GenerByFID(this.HisFlow, this.FID);
                }
                return this._HisWorkNodesOfFID;
            }
        }
        /// <summary>
        /// 工作流程
        /// </summary>
        private Flow _HisFlow = null;
        /// <summary>
        /// 工作流程
        /// </summary>
        public Flow HisFlow
        {
            get
            {
                return this._HisFlow;
            }
        }
        private GenerWorkFlow _HisGenerWorkFlow = null;
        public GenerWorkFlow HisGenerWorkFlow
        {
            get
            {
                if (_HisGenerWorkFlow == null)
                    _HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);
                return _HisGenerWorkFlow;
            }
            set
            {
                _HisGenerWorkFlow = value;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 _WorkID = 0;
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this._WorkID;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 _FID = 0;
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this._FID;
            }
            set
            {
                this._FID = value;
            }
        }
        /// <summary>
        /// 是否是干流
        /// </summary>
        public bool IsMainFlow
        {
            get
            {
                if (this.FID != 0 && this.FID != this.WorkID)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        #region 构造方法
        public WorkFlow(string fk_flow, Int64 wkid)
        {
            this.HisGenerWorkFlow = new GenerWorkFlow();
            this.HisGenerWorkFlow.RetrieveByAttr(GenerWorkerListAttr.WorkID, wkid);
            this._FID = this.HisGenerWorkFlow.FID;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            Flow flow = new Flow(fk_flow);
            this._HisFlow = flow;
            this._WorkID = wkid;

        }

        public WorkFlow(Flow flow, Int64 wkid)
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = wkid;
            gwf.RetrieveFromDBSources();

            this._FID = gwf.FID;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        /// <summary>
        /// 建立一个工作流事例
        /// </summary>
        /// <param name="flow">流程No</param>
        /// <param name="wkid">工作ID</param>
        public WorkFlow(Flow flow, Int64 wkid, Int64 fid)
        {
            this._FID = fid;
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        public WorkFlow(string FK_flow, Int64 wkid, Int64 fid)
        {
            this._FID = fid;

            Flow flow = new Flow(FK_flow);
            if (wkid == 0)
                throw new Exception("@没有指定工作ID, 不能创建工作流程.");
            //Flow flow= new Flow(FlowNo);
            this._HisFlow = flow;
            this._WorkID = wkid;
        }
        #endregion

        #region 公共属性

        /// <summary>
        /// 开始工作
        /// </summary>
        private StartWork _HisStartWork = null;
        /// <summary>
        /// 他开始的工作.
        /// </summary>
        public StartWork HisStartWork
        {
            get
            {
                if (_HisStartWork == null)
                {
                    StartWork en = (StartWork)this.HisFlow.HisStartNode.HisWork;
                    en.OID = this.WorkID;
                    en.FID = this.FID;
                    if (en.RetrieveFromDBSources() == 0)
                        en.RetrieveFID();
                    _HisStartWork = en;
                }
                return _HisStartWork;
            }
        }
        /// <summary>
        /// 开始工作节点
        /// </summary>
        private WorkNode _HisStartWorkNode = null;
        /// <summary>
        /// 他开始的工作.
        /// </summary>
        public WorkNode HisStartWorkNode
        {
            get
            {
                if (_HisStartWorkNode == null)
                {
                    Node nd = this.HisFlow.HisStartNode;
                    StartWork en = (StartWork)nd.HisWork;
                    en.OID = this.WorkID;
                    en.Retrieve();

                    WorkNode wn = new WorkNode(en, nd);
                    _HisStartWorkNode = wn;

                }
                return _HisStartWorkNode;
            }
        }
        #endregion

        #region 运算属性
        public int _IsComplete = -1;
        /// <summary>
        /// 是不是完成
        /// </summary>
        public bool IsComplete
        {
            get
            {

                //  bool s = !DBAccess.IsExits("select workid from WF_GenerWorkFlow WHERE WorkID=" + this.WorkID + " AND FK_Flow='" + this.HisFlow.No + "'");

                GenerWorkFlow generWorkFlow = new GenerWorkFlow(this.WorkID);
                if (generWorkFlow.WFState == WFState.Complete)
                    return true;
                else
                    return false;

            }
        }
        /// <summary>
        /// 是不是完成
        /// </summary>
        public string IsCompleteStr
        {
            get
            {
                if (this.IsComplete)
                    return "已";
                else
                    return "未";
            }
        }
        #endregion

        #region 静态方法

        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="empId">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpStation(int nodeId, string empId)
        {
            bool isCan = false;
            // 判断岗位对应关系是不是能够执行.
            string sql = "SELECT a.FK_Node FROM WF_NodeStation a,  " + BP.WF.Glo.EmpStation + " b WHERE (a.FK_Station=b.FK_Station) AND (a.FK_Node=" + nodeId + " AND b.FK_Emp='" + empId + "' )";
            isCan = DBAccess.IsExits(sql);
            if (isCan)
                return true;
            // 判断他的主要工作岗位能不能执行它.
            sql = "select FK_Node from WF_NodeStation WHERE FK_Node=" + nodeId + " AND ( FK_Station in (select FK_Station from " + BP.WF.Glo.EmpStation + " WHERE FK_Emp='" + empId + "') ) ";
            return DBAccess.IsExits(sql);
        }
        /// <summary>
        /// 是否这个工作人员能执行这个工作
        /// </summary>
        /// <param name="nodeId">节点</param>
        /// <param name="dutyNo">工作人员</param>
        /// <returns>能不能执行</returns> 
        public static bool IsCanDoWorkCheckByEmpDuty(int nodeId, string dutyNo)
        {
            string sql = "SELECT a.FK_Node FROM WF_NodeDuty  a,  Port_EmpDuty b WHERE (a.FK_Duty=b.FK_Duty) AND (a.FK_Node=" + nodeId + " AND b.FK_Duty=" + dutyNo + ")";
            if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 0)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 在物理上能构作这项工作的人员。
        /// </summary>
        /// <param name="nodeId">节点ID</param>		 
        /// <returns></returns>
        public static DataTable CanDoWorkEmps(int nodeId)
        {
            string sql = "select a.FK_Node, b.EmpID from WF_NodeStation  a,  " + BP.WF.Glo.EmpStation + " b WHERE (a.FK_Station=b.FK_Station) AND (a.FK_Node=" + nodeId + " )";
            return DBAccess.RunSQLReturnTable(sql);
        }
        /// <summary>
        /// GetEmpsBy
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public Emps GetEmpsBy(DataTable dt)
        {
            // 形成能够处理这件事情的用户几何。
            Emps emps = new Emps();
            foreach (DataRow dr in dt.Rows)
            {
                emps.AddEntity(new Emp(dr["EmpID"].ToString()));
            }
            return emps;
        }

        #endregion

        #region 流程方法

        private string _AppType = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string AppType
        {
            get
            {
                if (_AppType == null)
                {
                    if (BP.Sys.SystemConfig.IsBSsystem == false)
                    {
                        _AppType = "WF";
                    }
                    else
                    {
                        
                          
                                _AppType = "WF";
                         
                    }
                }
                return _AppType;
            }
        }
        private string _VirPath = null;
        /// <summary>
        /// 虚拟目录的路径
        /// </summary>
        public string VirPath
        {
            get
            {
                if (_VirPath == null)
                {
                    if (BP.Sys.SystemConfig.IsBSsystem)
                        _VirPath = BP.Sys.Glo.Request.ApplicationPath;
                    else
                        _VirPath = "";
                }
                return _VirPath;
            }
        }
        /// <summary>
        /// 执行挂起
        /// </summary>
        /// <param name="way">挂起方式</param>
        /// <param name="relData">释放日期</param>
        /// <param name="hungNote">挂起原因</param>
        /// <returns></returns>
        public string DoHungUp(HungUpWay way, string relData, string hungNote)
        {
            if (this.HisGenerWorkFlow.WFState == WFState.HungUp)
                throw new Exception("@当前已经是挂起的状态您不能执行在挂起.");

            if (DataType.IsNullOrEmpty(hungNote))
                hungNote = "无";

            if (way == HungUpWay.SpecDataRel)
                if (relData.Length < 10)
                    throw new Exception("@解除挂起的日期不正确(" + relData + ")");
            if (relData == null)
                relData = "";

            HungUp hu = new HungUp();
            hu.FK_Node = this.HisGenerWorkFlow.FK_Node;
            hu.WorkID = this.WorkID;
            hu.MyPK = hu.FK_Node + "_" + hu.WorkID;
            hu.HungUpWay = way; //挂起方式.
            hu.DTOfHungUp = DataType.CurrentDataTime; // 挂起时间
            hu.Rec = BP.Web.WebUser.No;  //挂起人
            hu.DTOfUnHungUp = relData; // 解除挂起时间。
            hu.Note = hungNote;
            hu.Insert();

            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.FK_Node;
            string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "挂起" + hungNote;
            string emps = "";
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue; //不发送给禁用的人。

                //BP.WF.Port.WFEmp emp = new Port.WFEmp(wl.FK_Emp);
                emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";

                //写入消息。
                BP.WF.Dev2Interface.Port_SendMsg(wl.FK_Emp, title, mailDoc, "HungUp" + wl.WorkID, BP.WF.SMSMsgType.HungUp, wl.FK_Flow, wl.FK_Node, wl.WorkID, wl.FID);
            }

            /* 执行 WF_GenerWorkFlow 挂起. */
            int hungSta = (int)WFState.HungUp;
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, hungSta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, hungSta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新工作者的挂起时间。
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET DTOfHungUp=" + dbstr + "DTOfHungUp,DTOfUnHungUp=" + dbstr + "DTOfUnHungUp, HungUpTimes=HungUpTimes+1 WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.DTOfHungUp, DataType.CurrentDataTime);
            ps.Add(GenerWorkerListAttr.DTOfUnHungUp, relData);

            ps.Add(GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.FK_Node);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.FK_Node);
            wn.AddToTrack(ActionType.HungUp, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, hungNote);
            return "已经成功执行挂起,并且已经通知给:" + emps;
        }
        /// <summary>
        /// 取消挂起
        /// </summary>
        /// <returns></returns>
        public string DoUnHungUp()
        {
            if (this.HisGenerWorkFlow.WFState != WFState.HungUp)
                throw new Exception("@非挂起状态,您不能解除挂起.");

            /* 执行解除挂起. */
            int sta = (int)WFState.Runing;
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbstr + "WFState WHERE WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, sta);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新流程报表的状态。 
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisFlow.PTable + " SET WFState=" + dbstr + "WFState WHERE OID=" + dbstr + "OID";
            ps.Add(GERptAttr.WFState, sta);
            ps.Add(GERptAttr.OID, this.WorkID);
            DBAccess.RunSQL(ps);

            // 更新工作者的挂起时间。
            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerlist SET  DTOfUnHungUp=" + dbstr + "DTOfUnHungUp WHERE FK_Node=" + dbstr + "FK_Node AND WorkID=" + dbstr + "WorkID";
            ps.Add(GenerWorkerListAttr.DTOfUnHungUp, DataType.CurrentDataTime);
            ps.Add(GenerWorkerListAttr.FK_Node, this.HisGenerWorkFlow.FK_Node);
            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);
            DBAccess.RunSQL(ps);

            //更新 HungUp
            HungUp hu = new HungUp();
            hu.FK_Node = this.HisGenerWorkFlow.FK_Node;
            hu.WorkID = this.HisGenerWorkFlow.WorkID;
            hu.MyPK = hu.FK_Node + "_" + hu.WorkID;
            if (hu.RetrieveFromDBSources() == 0)
                throw new Exception("@系统错误，没有找到挂起点");

            hu.DTOfUnHungUp = DataType.CurrentDataTime; // 挂起时间
            hu.Update();

            //更新他的主键。
            ps = new Paras();
            ps.SQL = "UPDATE WF_HungUp SET MyPK=" + SystemConfig.AppCenterDBVarStr + "MyPK WHERE MyPK=" + dbstr + "MyPK1";
            ps.Add("MyPK", BP.DA.DBAccess.GenerGUID());
            ps.Add("MyPK1", hu.MyPK);
            DBAccess.RunSQL(ps);


            /* 获取它的工作者，向他们发送消息。*/
            GenerWorkerLists wls = new GenerWorkerLists(this.WorkID, this.HisFlow.No);
            string url = Glo.ServerIP + "/" + this.VirPath + this.AppType + "/WorkOpt/OneWork/OneWork.htm?CurrTab=Track&FK_Flow=" + this.HisFlow.No + "&WorkID=" + this.WorkID + "&FID=" + this.HisGenerWorkFlow.FID + "&FK_Node=" + this.HisGenerWorkFlow.FK_Node;
            string mailDoc = "详细信息:<A href='" + url + "'>打开流程轨迹</A>.";
            string title = "工作:" + this.HisGenerWorkFlow.Title + " 被" + WebUser.Name + "解除挂起.";
            string emps = "";
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.IsEnable == false)
                    continue; //不发送给禁用的人。

                emps += wl.FK_Emp + "," + wl.FK_EmpText + ";";

                //写入消息。
                BP.WF.Dev2Interface.Port_SendMsg(wl.FK_Emp, title, mailDoc,
                    "HungUp" + wl.FK_Node + this.WorkID, BP.WF.SMSMsgType.Self, HisGenerWorkFlow.FK_Flow, HisGenerWorkFlow.FK_Node, this.WorkID, this.FID);

                //写入消息。
                //Glo.SendMsg(wl.FK_Emp, title, mailDoc);
            }


            // 记录日志..
            WorkNode wn = new WorkNode(this.WorkID, this.HisGenerWorkFlow.FK_Node);
            wn.AddToTrack(ActionType.UnHungUp, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "解除挂起,已经通知给:" + emps);
            return null;
        }
        /// <summary>
        /// 撤消移交
        /// </summary>
        /// <returns></returns>
        public string DoUnShift()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);
            if (wls.Count == 0)
                return "移交失败没有当前的工作。";

            Node nd = new Node(gwf.FK_Node);
            Work wk1 = nd.HisWork;
            wk1.OID = this.WorkID;
            wk1.Retrieve();

            // 记录日志.
            WorkNode wn = new WorkNode(wk1, nd);
            wn.AddToTrack(ActionType.UnShift, WebUser.No, WebUser.Name, nd.NodeID, nd.Name, "撤消移交");

            if (wls.Count == 1)
            {
                GenerWorkerList wl = (GenerWorkerList)wls[0];
                wl.FK_Emp = WebUser.No;
                wl.FK_EmpText = WebUser.Name;
                wl.IsEnable = true;
                wl.IsPass = false;
                wl.Update();
                return "@撤消移交成功，<a href='" + Glo.CCFlowAppPath + "WF/MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&FK_Node=" + wl.FK_Node + "&FID=" + wl.FID + "&WorkID=" + this.WorkID + "'><img src='" + Glo.CCFlowAppPath + "WF/Img/Btn/Do.gif' border=0/>执行工作</A>";
            }

            GenerWorkerList mywl = null;
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.FK_Emp == WebUser.No)
                {
                    wl.FK_Emp = WebUser.No;
                    wl.FK_EmpText = WebUser.Name;
                    wl.IsEnable = true;
                    wl.IsPass = false;
                    wl.Update();
                    mywl = wl;
                }
                else
                {
                    wl.Delete();
                }
            }
            if (mywl != null)
                return "@撤消移交成功，<a href='" + Glo.CCFlowAppPath + "WF/MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&FK_Node=" + mywl.FK_Node + "&FID=" + mywl.FID + "&WorkID=" + this.WorkID + "'><img src='" + Glo.CCFlowAppPath + "WF/Img/Btn/Do.gif' border=0/>执行工作</A>";

            GenerWorkerList wk = (GenerWorkerList)wls[0];
            GenerWorkerList wkNew = new GenerWorkerList();
            wkNew.Copy(wk);
            wkNew.FK_Emp = WebUser.No;
            wkNew.FK_EmpText = WebUser.Name;
            wkNew.IsEnable = true;
            wkNew.IsPass = false;
            wkNew.Insert();

            //删除撤销信息.
            BP.DA.DBAccess.RunSQL("DELETE FROM WF_ShiftWork WHERE WorkID=" + this.WorkID + " AND FK_Node=" + wk.FK_Node);

            return "@撤消移交成功，<a href='" + Glo.CCFlowAppPath + "WF/MyFlow.htm?FK_Flow=" + this.HisFlow.No + "&FK_Node=" + wk.FK_Node + "&FID=" + wk.FID + "&WorkID=" + this.WorkID + "'><img src='" + Glo.CCFlowAppPath + "WF/Img/Btn/Do.gif' border=0/>执行工作</A>";
        }
        #endregion
    }
    /// <summary>
    /// 工作流程集合.
    /// </summary>
    public class WorkFlows : CollectionBase
    {
        #region 构造
        /// <summary>
        /// 工作流程
        /// </summary>
        /// <param name="flow">流程编号</param>
        public WorkFlows(Flow flow)
        {
            StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            ens.RetrieveAll(10000);
            foreach (StartWork sw in ens)
            {
                this.Add(new WorkFlow(flow, sw.OID, sw.FID));
            }
        }
        /// <summary>
        /// 工作流程集合
        /// </summary>
        public WorkFlows()
        {
        }
        /// <summary>
        /// 工作流程集合
        /// </summary>
        /// <param name="flow">流程</param>
        /// <param name="flowState">工作ID</param> 
        public WorkFlows(Flow flow, int flowState)
        {
            //StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            //QueryObject qo = new QueryObject(ens);
            //qo.AddWhere(StartWorkAttr.WFState, flowState);
            //qo.DoQuery();
            //foreach (StartWork sw in ens)
            //{
            //    this.Add(new WorkFlow(flow, sw.OID, sw.FID));
            //}
        }

        #endregion

        #region 查询方法
        /// <summary>
        /// GetNotCompleteNode
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>StartWorks</returns>
        public static StartWorks GetNotCompleteWork(string flowNo)
        {
            return null;

            //Flow flow = new Flow(flowNo);
            //StartWorks ens = (StartWorks)flow.HisStartNode.HisWorks;
            //QueryObject qo = new QueryObject(ens);
            //qo.AddWhere(StartWorkAttr.WFState, "!=", 1);
            //qo.DoQuery();
            //return ens;

            /*
            foreach(StartWork sw in ens)
            {
                ens.AddEntity( new WorkFlow( flow, sw.OID) ) ; 
            }
            */
        }
        #endregion

        #region 方法
        /// <summary>
        /// 增加一个工作流程
        /// </summary>
        /// <param name="wn">工作流程</param>
        public void Add(WorkFlow wn)
        {
            this.InnerList.Add(wn);
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public WorkFlow this[int index]
        {
            get
            {
                return (WorkFlow)this.InnerList[index];
            }
        }
        #endregion

        #region 关于调度的自动方法
        /// <summary>
        /// 清除死节点。
        /// 死节点的产生，就是用户非法的操作，或者系统出现存储故障，造成的流程中的当前工作节点没有工作人员，从而不能正常的运行下去。
        /// 清除死节点，就是把他们放到死节点工作集合里面。
        /// </summary>
        /// <returns></returns>
        public static string ClearBadWorkNode()
        {
            string infoMsg = "清除死节点的信息：";
            string errMsg = "清除死节点的错误信息：";
            return infoMsg + errMsg;
        }
        #endregion
    }
}
