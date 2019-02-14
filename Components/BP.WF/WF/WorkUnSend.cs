using System;
using System.Collections;
using System.Data;
using BP.En;
using BP.Web;
using BP.DA;
using BP.Port;
using BP.Sys;
using BP.WF.XML;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 撤销发送
    /// </summary>
    public class WorkUnSend
    {
        #region 属性.
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
                        if (BP.Web.WebUser.IsWap)
                            _AppType = "WF/WAP";
                        else
                        {
                            bool b = BP.Sys.Glo.Request.RawUrl.ToLower().Contains("oneflow");
                            if (b)
                                _AppType = "WF/OneFlow";
                            else
                                _AppType = "WF";
                        }
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
                        _VirPath = Glo.CCFlowAppPath;//BP.Sys.Glo.Request.ApplicationPath;
                    else
                        _VirPath = "";
                }
                return _VirPath;
            }
        }
        public string FlowNo = null;
        private Flow _HisFlow = null;
        public Flow HisFlow
        {
            get
            {
                if (_HisFlow == null)
                    this._HisFlow = new Flow(this.FlowNo);
                return this._HisFlow;
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID = 0;
        /// <summary>
        /// FID
        /// </summary>
        public Int64 FID = 0;
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

        /// <summary>
        /// 撤销发送
        /// </summary>
        public WorkUnSend(string flowNo, Int64 workID, int unSendToNode = 0,Int64 fid = 0)
        {
            this.FlowNo = flowNo;
            this.WorkID = workID;
            this.FID = fid;
            this.UnSendToNode = UnSendToNode; //撤销到节点.
        }
        public int UnSendToNode = 0;
        /// <summary>
        /// 得到当前的进行中的工作。
        /// </summary>
        /// <returns></returns>		 
        public WorkNode GetCurrentWorkNode()
        {
            int currNodeID = 0;
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
            {
                // this.DoFlowOver(ActionType.FlowOver, "非正常结束，没有找到当前的流程记录。");
                throw new Exception("@" + string.Format("工作流程{0}已经完成。", this.WorkID));
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
        /// 执行子线程的撤销.
        /// </summary>
        /// <returns></returns>
        private string DoThreadUnSend()
        {
            //定义当前的节点.
            WorkNode wn = this.GetCurrentWorkNode();

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            Node nd = new Node(gwf.FK_Node);

            #region 求的撤销的节点.
            int cancelToNodeID = 0;

            if (nd.HisCancelRole == CancelRole.SpecNodes)
            {
                /*1.指定的节点可以撤销，首先判断是否设置指定的节点.*/

                //
                NodeCancels ncs = new NodeCancels();
                ncs.Retrieve(NodeCancelAttr.FK_Node, wn.HisNode.NodeID);
                if (ncs.Count == 0)
                    throw new Exception("@流程设计错误, 您设置了当前节点(" + wn.HisNode.Name + ")可以让指定的节点人员撤销，但是您没有设置指定的节点.");

                //获取Track表
                 string truckTable = "ND" + int.Parse(wn.HisNode.FK_Flow) + "Track";

                //获取到当前节点走过的节点 与 设定可撤销节点的交集
                 string sql = "SELECT DISTINCT(FK_Node) FROM  WF_GenerWorkerlist WHERE ";
                sql +=" FK_Node IN(SELECT CancelTO FROM WF_NodeCancel WHERE FK_Node="+wn.HisNode.NodeID+") AND FK_Emp='"+WebUser.No+"'";

                string nds = DBAccess.RunSQLReturnString(sql);
                if(DataType.IsNullOrEmpty(nds))
                    throw new Exception("@您不能执行撤消发送，两种原因：1，你不具备撤销该节点的功能；2.流程设计错误，你指定的可以撤销的节点不在流程运转中走过的节点.");

                //获取可以删除到的节点
                cancelToNodeID = int.Parse(nds.Split(',')[0]);

            }

            if (nd.HisCancelRole == CancelRole.OnlyNextStep)
            {
                /*如果仅仅允许撤销上一步骤.*/
                WorkNode wnPri = wn.GetPreviousWorkNode();

                GenerWorkerList wl = new GenerWorkerList();
                int num = wl.Retrieve(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No,
                    GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID);
                if (num == 0)
                    throw new Exception("@您不能执行撤消发送，因为当前工作不是您发送的或下一步工作已处理。");
                cancelToNodeID = wnPri.HisNode.NodeID;
                  
            }

            if (cancelToNodeID == 0)
                throw new Exception("@没有求出要撤销到的节点.");
            #endregion 求的撤销的节点.

            /********** 开始执行撤销. **********************/
            Node cancelToNode = new Node(cancelToNodeID);

            switch(cancelToNode.HisNodeWorkType)
            {
                case NodeWorkType.StartWorkFL:
                case NodeWorkType.WorkFHL:
                case NodeWorkType.WorkFL:

                    // 调用撤消发送前事件。
                   nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, nd, wn.HisWork, null);

                    BP.WF.Dev2Interface.Node_FHL_KillSubFlow(cancelToNode.FK_Flow, this.FID, this.WorkID); //杀掉子线程.

                    // 调用撤消发送前事件。
                    nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, nd, wn.HisWork, null);
                      return "KillSubThared@子线程撤销成功.";
                default:
                    break;

            }
          //  if (cancelToNode.HisNodeWorkType == NodeWorkType.StartWorkFL)


            WorkNode wnOfCancelTo = new WorkNode(this.WorkID, cancelToNodeID);

            // 调用撤消发送前事件。
            string msg = nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, nd, wn.HisWork, null);

            #region 删除当前节点数据。

            // 删除产生的工作列表。
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            // 删除工作信息,如果是按照ccflow格式存储的。
            if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                wn.HisWork.Delete();

            // 删除附件信息。
            DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + gwf.FK_Node + "' AND RefPKVal='" + this.WorkID + "'");
            #endregion 删除当前节点数据。

            // 更新.
            gwf.FK_Node = cancelToNode.NodeID;
            gwf.NodeName = cancelToNode.Name;
            //如果不启动自动记忆，删除tonodes,用于 选择节点发送。撤消后，可重新选择节点发送
            if (cancelToNode.IsRememberMe == false)
                gwf.Paras_ToNodes = "";

            if (cancelToNode.IsEnableTaskPool && Glo.IsEnableTaskPool)
                gwf.TaskSta = TaskSta.Takeback;
            else
                gwf.TaskSta = TaskSta.None;

            gwf.TodoEmps = WebUser.No + "," + WebUser.Name + ";";
            gwf.Update();

            if (cancelToNode.IsEnableTaskPool && Glo.IsEnableTaskPool)
            {
                //设置全部的人员不可用。
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0,  IsEnable=-1 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);

                //设置当前人员可用。
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0,  IsEnable=1  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + WebUser.No + "'");
            }
            else
            {
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);
            }

            //更新当前节点，到rpt里面。
            BP.DA.DBAccess.RunSQL("UPDATE " + this.HisFlow.PTable + " SET FlowEndNode=" + gwf.FK_Node + " WHERE OID=" + this.WorkID);

            // 记录日志..
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, cancelToNode.NodeID, cancelToNode.Name, "无");

            // 删除数据.
            if (wn.HisNode.IsStartNode)
            {
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node=" + nd.NodeID);
            }

            if (wn.HisNode.IsEval)
            {
                /*如果是质量考核节点，并且撤销了。*/
                DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + this.WorkID);
            }

            #region 恢复工作轨迹，解决工作抢办。
            if (cancelToNode.IsStartNode == false && cancelToNode.IsEnableTaskPool == false)
            {
                WorkNode ppPri = wnOfCancelTo.GetPreviousWorkNode();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Retrieve(GenerWorkerListAttr.FK_Node, wnOfCancelTo.HisNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);
                // BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + backtoNodeID + " AND WorkID=" + this.WorkID);
                RememberMe rm = new RememberMe();
                rm.Retrieve(RememberMeAttr.FK_Node, wnOfCancelTo.HisNode.NodeID, RememberMeAttr.FK_Emp, ppPri.HisWork.Rec);

                string[] empStrs = rm.Objs.Split('@');
                foreach (string s in empStrs)
                {
                    if (s == "" || s == null)
                        continue;

                    if (s == wl.FK_Emp)
                        continue;
                    GenerWorkerList wlN = new GenerWorkerList();
                    wlN.Copy(wl);
                    wlN.FK_Emp = s;

                    WF.Port.WFEmp myEmp = new Port.WFEmp(s);
                    wlN.FK_EmpText = myEmp.Name;

                    wlN.Insert();
                }
            }
            #endregion 恢复工作轨迹，解决工作抢办。

            #region 如果是开始节点, 检查此流程是否有子线程，如果有则删除它们。
            if (nd.IsStartNode)
            {
                /*要检查一个是否有 子流程，如果有，则删除它们。*/
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
                if (gwfs.Count > 0)
                {
                    foreach (GenerWorkFlow item in gwfs)
                    {
                        /*删除每个子线程.*/
                        BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
                    }
                }
            }
            #endregion

            #region 计算完成率。
            bool isSetEnable = false; //是否关闭合流节点待办.
            if (nd.PassRate == 100)
            {
                isSetEnable = true;
            }
            else
            {
                string mysql = "SELECT COUNT(DISTINCT WorkID) FROM WF_GenerWorkerlist WHERE FID=" + this.FID + " AND IsPass=1 AND FK_Node IN (SELECT FK_Node FROM WF_Direction WHERE ToNode=" + gwf.FK_Node + ")";
                decimal numOfPassed = DBAccess.RunSQLReturnValDecimal(mysql, 0, 1);

                mysql = "SELECT COUNT(DISTINCT WorkID) FROM WF_GenerWorkFlow WHERE FID=" + this.FID;
                decimal numOfAll = DBAccess.RunSQLReturnValDecimal(mysql, 0, 1);

                decimal rate = numOfPassed / numOfAll * 100;
                if (nd.PassRate > rate)
                    isSetEnable = true;
            }

            //是否关闭合流节点待办.
            if (isSetEnable == true)
                DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=2 WHERE WorkID="+this.FID+" AND  FK_Node="+gwf.FK_Node);
            #endregion

            //调用撤消发送后事件。
            msg += nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, nd, wn.HisWork, null);

            if (wnOfCancelTo.HisNode.IsStartNode)
                return "@撤消执行成功. " + msg;
            else
                return "@撤消执行成功. " + msg;

            return "工作已经被您撤销到:" + cancelToNode.Name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DoUnSend()
        {
            string str = DoUnSendIt();

            int fk_node = DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID, 0);

            //删除自己审核的信息.
            string sql = "DELETE FROM ND" + int.Parse(FlowNo) + "Track WHERE WorkID = " + this.WorkID +
                              " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + fk_node +
                              " AND EmpFrom = '" + WebUser.No + "'";
            DBAccess.RunSQL(sql);

            return str;
        }
        /// <summary>
        /// 执行撤消
        /// </summary>
        private string DoUnSendIt()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (gwf.WFState == WFState.Complete)
                return "err@该流程已经完成，您不能撤销。";


            // 如果停留的节点是分合流。
            Node nd = new Node(gwf.FK_Node);

            /*该节点不允许退回.*/
            if (nd.HisCancelRole == CancelRole.None)
                throw new Exception("当前节点，不允许撤销。");


            if (nd.IsStartNode && nd.HisNodeWorkType != NodeWorkType.StartWorkFL)
                throw new Exception("当前节点是开始节点，所以您不能撤销。");

            //如果撤销到的节点和当前流程运行到的节点相同，则是分流、或者分河流
            if (this.UnSendToNode == nd.NodeID)
            {
                //如果当前节点是分流、分合流节点则可以撤销
                if ( nd.HisNodeWorkType == NodeWorkType.StartWorkFL
                    || nd.HisNodeWorkType == NodeWorkType.WorkFL
                    || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
                {
                    //获取当前节点的子线程
                    string truckTable = "ND" + int.Parse(nd.FK_Flow) + "Track";
                    string threadSQL = "SELECT FK_Node,WorkID FROM WF_GenerWorkFlow  WHERE FID=" + this.WorkID + " AND FK_Node"
                            + " IN(SELECT DISTINCT(NDTo) FROM " + truckTable + "  WHERE ActionType=" + (int)ActionType.ForwardFL + " AND WorkID=" + this.WorkID + " AND NDFrom='" + nd.NodeID + "'"
                            + "  ) ";
                    DataTable dt = DBAccess.RunSQLReturnTable(threadSQL);
                    if (dt == null || dt.Rows.Count == 0)
                        throw new Exception("err@流程运行错误：当不存在子线程时,改过程应该处于待办状态");


                    foreach (DataRow dr in dt.Rows)
                    {
                        Node threadnd = new Node(dr["FK_Node"].ToString());
                        // 调用撤消发送前事件。
                        nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, nd, nd.HisWork, null);

                        BP.WF.Dev2Interface.Node_FHL_KillSubFlow(threadnd.FK_Flow, this.WorkID, long.Parse(dr["WorkID"].ToString())); //杀掉子线程.

                        // 调用撤消发送前事件。
                        nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, nd, nd.HisWork, null);
                    }

                    return "撤销成功";

                }

            }

            

            //如果启用了对方已读，就不能撤销.
            if (nd.CancelDisWhenRead == true)
            {
                //撤销到的节点是干流程节点/子线程撤销到子线程
                int i = DBAccess.RunSQLReturnValInt("SELECT SUM(IsRead) AS Num FROM WF_GenerWorkerList WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node ,0);
                if (i >= 1)
                    return "err@当前待办已经有[" + i + "]个工作人员打开了该工作,您不能执行撤销.";
                else
                {
                    //干流节点撤销到子线程
                    i = DBAccess.RunSQLReturnValInt("SELECT SUM(IsRead) AS Num FROM WF_GenerWorkerList WHERE WorkID=" + this.FID + " AND FK_Node=" + gwf.FK_Node, 0);
                    if(i>=1)
                        return "err@当前待办已经有[" + i + "]个工作人员打开了该工作,您不能执行撤销.";
                }
            }


            #region 如果是越轨流程状态 @fanleiwei.
            string sql = "SELECT COUNT(*) AS Num FROM WF_GenerWorkerlist WHERE WorkID="+this.WorkID+" AND IsPass=80";
            if (DBAccess.RunSQLReturnValInt(sql, 0) != 0)
            {
                //求出来越轨子流程workid并把它删除掉.
                GenerWorkFlow gwfSubFlow = new GenerWorkFlow();
                int i = gwfSubFlow.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
                if (i == 1)
                    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(gwfSubFlow.FK_Flow, gwfSubFlow.WorkID,true);

                //执行回复当前节点待办..
                sql = "UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE IsPass=80 AND FK_Node="+gwf.FK_Node+" AND WorkID="+this.WorkID;
                DBAccess.RunSQL(sql);

                return "撤销延续流程执行成功，撤销到["+gwf.NodeName+"],撤销给["+gwf.TodoEmps+"]";
            }
            #endregion 如果是越轨流程状态 .

            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(gwf.FK_Flow, gwf.FK_Node, this.WorkID, WebUser.No) == true)
                return "info@您有处理当前工作的权限,可能您已经执行了撤销,请使用退回或者发送功能.";


            #region 判断是否是会签状态,是否是会签人做的撤销. 主持人是不能撤销的.
            if (gwf.HuiQianTaskSta != HuiQianTaskSta.None)
            {
                string IsEnableUnSendWhenHuiQian  = SystemConfig.AppSettings["IsEnableUnSendWhenHuiQian"];
                if (DataType.IsNullOrEmpty(IsEnableUnSendWhenHuiQian) == false && IsEnableUnSendWhenHuiQian.Equals("0"))
                    return "info@当前节点是会签状态，您不能执行撤销.";

                GenerWorkerList gwl = new GenerWorkerList();
                int numOfmyGwl = gwl.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                      GenerWorkerListAttr.WorkID, this.WorkID,
                      GenerWorkerListAttr.FK_Node, gwf.FK_Node);

                //如果没有找到当前会签人.
                if (numOfmyGwl == 0)
                    return "err@当前节点[" + gwf.NodeName + "]是会签状态,[" + gwf.TodoEmps + "]在执行会签,您不能执行撤销.";

                if (gwl.IsHuiQian == true)
                {

                }

                //如果是会签人，就让其显示待办.
                gwl.IsPassInt = 0;
                gwl.IsEnable = true;
                gwl.Update();

                // 在待办人员列表里加入他. 要判断当前人员是否是主持人,如果是主持人的话，主持人是否在发送的时候，
                // 就选择方向与接受人.
                if (gwf.HuiQianZhuChiRen == WebUser.No)
                {
                    gwf.TodoEmps = WebUser.No + "," + BP.Web.WebUser.Name + ";" + gwf.TodoEmps;
                }
                else
                {
                    gwf.TodoEmps = gwf.TodoEmps + BP.Web.WebUser.Name + ";";
                }

                gwf.Update();

                return "会签人撤销成功.";
            }
            #endregion 判断是否是会签状态,是否是会签人做的撤销.

            if (gwf.FID != 0)
            {
                //执行子线程的撤销.
                return DoThreadUnSend();
            }

            //定义当前的节点.
            WorkNode wn = this.GetCurrentWorkNode();

            #region 求的撤销的节点.
            int cancelToNodeID = 0;
            if (nd.HisCancelRole == CancelRole.SpecNodes)
            {
                /*指定的节点可以撤销,首先判断当前人员是否有权限.*/
                NodeCancels ncs = new NodeCancels();
                ncs.Retrieve(NodeCancelAttr.FK_Node, wn.HisNode.NodeID);
                if (ncs.Count == 0)
                    throw new Exception("@流程设计错误, 您设置了当前节点(" + wn.HisNode.Name + ")可以让指定的节点人员撤销，但是您没有设置指定的节点.");

                /* 查询出来. */
                sql = "SELECT FK_Node FROM WF_GenerWorkerList WHERE FK_Emp='" + WebUser.No + "' AND IsPass=1 AND IsEnable=1 AND WorkID=" + wn.HisWork.OID + " ORDER BY RDT DESC ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("err@撤销流程错误,您没有权限执行撤销发送.");

                // 找到将要撤销到的NodeID.
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (NodeCancel nc in ncs)
                    {
                        if (nc.CancelTo == int.Parse(dr[0].ToString()))
                        {
                            cancelToNodeID = nc.CancelTo;
                            break;
                        }
                    }

                    if (cancelToNodeID != 0)
                        break;
                }

                if (cancelToNodeID == 0)
                    throw new Exception("@撤销流程错误,您没有权限执行撤销发送,当前节点不可以执行撤销.");
            }

            if (nd.HisCancelRole == CancelRole.OnlyNextStep)
            {
                /*如果仅仅允许撤销上一步骤.*/
                WorkNode wnPri = wn.GetPreviousWorkNode();

                GenerWorkerList wl = new GenerWorkerList();
                int num = wl.Retrieve(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No,
                    GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID);
                if (num == 0)
                    throw new Exception("err@您不能执行撤消发送，因为当前工作不是您发送的或下一步工作已处理。");
                cancelToNodeID = wnPri.HisNode.NodeID;
            }

            if (cancelToNodeID == 0)
                throw new Exception("err@没有求出要撤销到的节点.");
            #endregion 求的撤销的节点.

            if (this.UnSendToNode != 0 && gwf.FK_Node != this.UnSendToNode)
            {
                /* 要撤销的节点是分流节点，并且当前节点不在分流节点而是在合流节点的情况， for:华夏银行.
                 * 1, 分流节点发送给n个人.
                 * 2, 其中一个人发送到合流节点，另外一个人退回给分流节点。
                 * 3，现在分流节点的人接收到一个待办，并且需要撤销整个分流节点的发送.
                 * 4, UnSendToNode 这个时间没有值，并且当前干流节点的停留的节点与要撤销到的节点不一致。
                 */
                return DoUnSendInFeiLiuHeiliu(gwf);
            }

            #region 判断当前节点的模式.
            switch (nd.HisNodeWorkType)
            {
                case NodeWorkType.WorkFHL:
                    return this.DoUnSendFeiLiu(gwf);
                case NodeWorkType.WorkFL:
                case NodeWorkType.StartWorkFL:
                    break;
                case NodeWorkType.WorkHL:
                    if (this.IsMainFlow)
                    {
                        /* 首先找到与他最近的一个分流点，并且判断当前的操作员是不是分流点上的工作人员。*/
                        return this.DoUnSendHeiLiu_Main(gwf);
                    }
                    else
                    {
                        return this.DoUnSendSubFlow(gwf); //是子流程时.
                    }
                    break;
                case NodeWorkType.SubThreadWork:
                    break;
                default:
                    break;
            }
            #endregion 判断当前节点的模式.

            /********** 开始执行撤销. **********************/
            Node cancelToNode = new Node(cancelToNodeID);

            #region 如果撤销到的节点是普通的节点，并且当前的节点是分流(分流)节点，并且分流(分流)节点已经发送下去了,就不允许撤销了.
            if (cancelToNode.HisRunModel == RunModel.Ordinary
                 && nd.HisRunModel == RunModel.HL
                 && nd.HisRunModel == RunModel.FHL
                 && nd.HisRunModel == RunModel.FL)
            {
                /* 检查一下是否还有没有完成的子线程，如果有就抛出不允许撤销的异常。 */
                  sql = "SELECT COUNT(*) as NUM FROM WF_GenerWorkerList WHERE FID="+this.WorkID+" AND IsPass=0";
                  if (BP.DA.DBAccess.RunSQLReturnValInt(sql) != 0)
                      return "err@不允许撤销，因为有未完成的子线程.";

                //  return this.DoUnSendHeiLiu_Main(gwf);
            }
            #endregion 如果撤销到的节点是普通的节点，并且当前的节点是分流节点，并且分流节点已经发送下去了.

           
            #region 如果当前是协作组长模式,就要考虑当前是否是会签节点，如果是会签节点，就要处理。
            if (cancelToNode.TodolistModel == TodolistModel.TeamupGroupLeader
                || cancelToNode.TodolistModel == TodolistModel.Teamup)
            {
                sql = "SELECT ActionType FROM ND" + int.Parse(this.FlowNo) + "Track WHERE NDFrom=" + cancelToNodeID + " AND EmpFrom='" + WebUser.No + "' AND WorkID=" + this.WorkID;
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    int ac = int.Parse(dr[0].ToString());
                    ActionType at = (ActionType)ac;
                    if (at == ActionType.TeampUp)
                    {
                        /*如果是写作人员，就不允许他撤销 */
                        throw new Exception("@您是节点[" + cancelToNode.Name + "]的会签人，您不能执行撤销。");
                    }
                }
            }
            #endregion 如果当前是协作组长模式

            WorkNode wnOfCancelTo = new WorkNode(this.WorkID, cancelToNodeID);

            // 调用撤消发送前事件。
            string msg = nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, nd, wn.HisWork, null);

            #region 删除当前节点数据。
            // 删除产生的工作列表。
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            // 删除工作信息,如果是按照ccflow格式存储的。
            if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                wn.HisWork.Delete();

            // 删除附件信息。
            DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE FK_MapData='ND" + gwf.FK_Node + "' AND RefPKVal='" + this.WorkID + "'");
            #endregion 删除当前节点数据。

            // 更新.
            gwf.FK_Node = cancelToNode.NodeID;
            gwf.NodeName = cancelToNode.Name;
            //恢复上一步发送人
            DataTable dtPrevTrack = Dev2Interface.Flow_GetPreviousNodeTrack(this.WorkID,cancelToNode.NodeID);
            if(dtPrevTrack != null && dtPrevTrack.Rows.Count > 0)
            {
                gwf.Sender = dtPrevTrack.Rows[0]["EmpFrom"].ToString();
            }

            if (cancelToNode.IsEnableTaskPool && Glo.IsEnableTaskPool)
                gwf.TaskSta = TaskSta.Takeback;
            else
                gwf.TaskSta = TaskSta.None;

            gwf.TodoEmps = WebUser.No + "," + WebUser.Name + ";";
            gwf.Update();

            if (cancelToNode.IsEnableTaskPool && Glo.IsEnableTaskPool)
            {
                //设置全部的人员不可用。
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0,  IsEnable=-1 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);

                //设置当前人员可用。
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0,  IsEnable=1  WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + WebUser.No + "'");
            }
            else
            {
                BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND FK_Emp='" + WebUser.No + "'");
            }

            //更新当前节点，到rpt里面。
            BP.DA.DBAccess.RunSQL("UPDATE " + this.HisFlow.PTable + " SET FlowEndNode=" + gwf.FK_Node + " WHERE OID=" + this.WorkID);

            // 记录日志..
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, cancelToNode.NodeID, cancelToNode.Name, "无");

            //删除审核组件设置“协作模式下操作员显示顺序”为“按照接受人员列表先后顺序(官职大小)”，而生成的待审核轨迹信息
            FrmWorkCheck fwc = new FrmWorkCheck(nd.NodeID);
            if (fwc.FWCSta == FrmWorkCheckSta.Enable && fwc.FWCOrderModel == FWCOrderModel.SqlAccepter)
            {
                BP.DA.DBAccess.RunSQL("DELETE FROM ND" + int.Parse(nd.FK_Flow) + "Track WHERE WorkID = " + this.WorkID +
                                      " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + nd.NodeID +
                                      " AND NDTo = " + nd.NodeID + " AND (Msg = '' OR Msg IS NULL)");
            }

            // 删除数据.
            if (wn.HisNode.IsStartNode)
            {
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + this.WorkID + " AND FK_Node=" + nd.NodeID);
            }

            //首先删除当前节点的，审核意见.
            string delTrackSQl = "DELETE FROM ND" + int.Parse(nd.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND NDFrom=" + nd.NodeID + " AND ActionType =22 ";
            DBAccess.RunSQL(delTrackSQl);

            if (wn.HisNode.IsEval)
            {
                /*如果是质量考核节点，并且撤销了。*/
                DBAccess.RunSQL("DELETE FROM WF_CHEval WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + this.WorkID);
            }

            #region 恢复工作轨迹，解决工作抢办。
            if (cancelToNode.IsStartNode == false && cancelToNode.IsEnableTaskPool == false)
            {
                WorkNode ppPri = wnOfCancelTo.GetPreviousWorkNode();
                GenerWorkerList wl = new GenerWorkerList();
                wl.Retrieve(GenerWorkerListAttr.FK_Node, wnOfCancelTo.HisNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);
                // BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + backtoNodeID + " AND WorkID=" + this.WorkID);
                RememberMe rm = new RememberMe();
                rm.Retrieve(RememberMeAttr.FK_Node, wnOfCancelTo.HisNode.NodeID, RememberMeAttr.FK_Emp, ppPri.HisWork.Rec);

                string[] empStrs = rm.Objs.Split('@');
                foreach (string s in empStrs)
                {
                    if (s == "" || s == null)
                        continue;

                    if (s == wl.FK_Emp)
                        continue;
                    GenerWorkerList wlN = new GenerWorkerList();
                    wlN.Copy(wl);
                    wlN.FK_Emp = s;

                    WF.Port.WFEmp myEmp = new Port.WFEmp(s);
                    wlN.FK_EmpText = myEmp.Name;

                    wlN.Insert();
                }
            }
            #endregion 恢复工作轨迹，解决工作抢办。

            #region 如果是开始节点, 检查此流程是否有子流程，如果有则删除它们。
            if (nd.IsStartNode)
            {
                /*要检查一个是否有 子流程，如果有，则删除它们。*/
                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID);
                if (gwfs.Count > 0)
                {
                    foreach (GenerWorkFlow item in gwfs)
                    {
                        /*删除每个子线程.*/
                        BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(item.FK_Flow, item.WorkID, true);
                    }
                }
            }
            #endregion

            //调用撤消发送后事件。
            msg += nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, nd, wn.HisWork, null);

            if (wnOfCancelTo.HisNode.IsStartNode)
            {
                if (BP.Web.WebUser.IsWap)
                {
                    if (wnOfCancelTo.HisNode.HisFormType != NodeFormType.SDKForm)
                        return "@撤消发送执行成功." + msg;
                    else
                        return "@撤销成功." + msg;
                }
                else
                {
                    switch (wnOfCancelTo.HisNode.HisFormType)
                    {
                        case NodeFormType.FoolForm:
                        case NodeFormType.FreeForm:
                            return "@撤消执行成功." + msg;
                            break;
                        default:
                            return "@撤销成功." + msg;
                            break;
                    }
                }
            }
            else
            {
                // 更新是否显示。
                //  DBAccess.RunSQL("UPDATE WF_ForwardWork SET IsRead=1 WHERE WORKID=" + this.WorkID + " AND FK_Node=" + cancelToNode.NodeID);
                switch (wnOfCancelTo.HisNode.HisFormType)
                {
                    case NodeFormType.FoolForm:
                    case NodeFormType.FreeForm:
                        return "@撤消执行成功. " + msg;
                        break;
                    default:
                        return "撤销成功:" + msg;
                        break;
                }
            }
            return "工作已经被您撤销到:" + cancelToNode.Name;
        }
        /// <summary>
        /// 撤消分流点
        /// 1, 把分流节点的人员设置成待办。
        /// 2，删除所有该分流点发起的子线程。
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        private string DoUnSendFeiLiu(GenerWorkFlow gwf)
        {
            //首先要检查，当前的处理人是否是分流节点的处理人？如果是，就要把，未走完的所有子线程都删除掉。
            GenerWorkerList gwl = new GenerWorkerList();
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node, GenerWorkerListAttr.FK_Emp, WebUser.No);
            if (i == 0)
                throw new Exception("@您不能执行撤消发送，因为当前工作不是您发送的。");

            //处理事件.
            Node nd = new Node(gwf.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = gwf.WorkID;
            wk.RetrieveFromDBSources();

            string msg = nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, nd, wk, null);

            // 记录日志..
            WorkNode wn = new WorkNode(wk, nd);
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, "");

            //删除上一个节点的数据。
            foreach (Node ndNext in nd.HisToNodes)
            {
                i = DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + this.WorkID + " AND FK_Node=" + ndNext.NodeID);
                if (i == 0)
                    continue;

                if (ndNext.HisRunModel == RunModel.SubThread)
                {
                    /*如果到达的节点是子线程,就查询出来发起的子线程。*/
                    GenerWorkFlows gwfs = new GenerWorkFlows();
                    gwfs.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);
                    foreach (GenerWorkFlow en in gwfs)
                        BP.WF.Dev2Interface.Flow_DeleteSubThread(gwf.FK_Flow, en.WorkID, "合流节点撤销发送前，删除子线程.");
                    continue;
                }

                // 删除工作记录。
                Works wks = ndNext.HisWorks;
                if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                    wks.Delete(GenerWorkerListAttr.FID, this.WorkID);
            }

            //设置当前节点。
            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node + " AND IsPass=1");

            // 设置当前节点的状态.
            Node cNode = new Node(gwf.FK_Node);
            Work cWork = cNode.HisWork;
            cWork.OID = this.WorkID;
            msg += nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, nd, wk, null);

            return "@撤消执行成功." + msg;
        }
        /// <summary>
        /// 分合流的撤销发送.
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        private string DoUnSendInFeiLiuHeiliu(GenerWorkFlow gwf)
        {
            //首先要检查，当前的处理人是否是分流节点的处理人？如果是，就要把，未走完的所有子线程都删除掉。
            GenerWorkerList gwl = new GenerWorkerList();

            //删除合流节点的处理人.
            gwl.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node);

            //查询已经走得分流节点待办.
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.UnSendToNode, GenerWorkerListAttr.FK_Emp, WebUser.No);
            if (i == 0)
                throw new Exception("@您不能执行撤消发送，因为当前分流工作不是您发送的。");

            // 更新分流节点，让其出现待办.
            gwl.IsPassInt = 0;
            gwl.IsRead = false;
            gwl.SDT = BP.DA.DataType.CurrentDataTime;  //这里计算时间有问题.
            gwl.Update();

            // 把设置当前流程运行到分流流程上.
            gwf.FK_Node = this.UnSendToNode;
            Node nd = new Node(this.UnSendToNode);
            gwf.NodeName = nd.Name;
            gwf.Sender = BP.Web.WebUser.No;
            gwf.SendDT = BP.DA.DataType.CurrentDataTime;
            gwf.Update();


            Work wk = nd.HisWork;
            wk.OID = gwf.WorkID;
            wk.RetrieveFromDBSources();

            string msg = nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, nd, wk, null);

            // 记录日志..
            WorkNode wn = new WorkNode(wk, nd);
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, "");
 

            //删除上一个节点的数据。
            foreach (Node ndNext in nd.HisToNodes)
            {
                i = DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + this.WorkID + " AND FK_Node=" + ndNext.NodeID);
                if (i == 0)
                    continue;

                if (ndNext.HisRunModel == RunModel.SubThread)
                {
                    /*如果到达的节点是子线程,就查询出来发起的子线程。*/
                    GenerWorkFlows gwfs = new GenerWorkFlows();
                    gwfs.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);
                    foreach (GenerWorkFlow en in gwfs)
                        BP.WF.Dev2Interface.Flow_DeleteSubThread(gwf.FK_Flow, en.WorkID, "合流节点撤销发送前，删除子线程.");
                    continue;
                }

                // 删除工作记录。
                Works wks = ndNext.HisWorks;
                if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                    wks.Delete(GenerWorkerListAttr.FID, this.WorkID);
            }


            // 设置当前节点的状态.
            Node cNode = new Node(gwf.FK_Node);
            Work cWork = cNode.HisWork;
            cWork.OID = this.WorkID;
            msg += nd.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, nd, wk, null);
            if (cNode.IsStartNode)
            {

                return "@撤消执行成功." + msg;

            }
            else
            {

                return "@撤消执行成功." + msg;

            }
        }
        /// <summary>
        /// 执行撤销发送
        /// </summary>
        /// <param name="gwf"></param>
        /// <returns></returns>
        public string DoUnSendHeiLiu_Main(GenerWorkFlow gwf)
        {
            Node currNode = new Node(gwf.FK_Node);
            Node priFLNode = currNode.HisPriFLNode;
            GenerWorkerList wl = new GenerWorkerList();

            //判断改操作人员是否是分流节点上的人员.
            int i = wl.Retrieve(GenerWorkerListAttr.FK_Node, priFLNode.NodeID, GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
            if (i == 0)
                return "@不是您把工作发送到当前节点上，所以您不能撤消。";

            WorkNode wn = this.GetCurrentWorkNode();
            WorkNode wnPri = new WorkNode(this.WorkID, priFLNode.NodeID);

            // 记录日志..
            wnPri.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, wnPri.HisNode.NodeID, wnPri.HisNode.Name, "无");

            //删除当前节点的流程
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node.ToString());

            if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                wn.HisWork.Delete();

            //更改流程信息
            gwf.FK_Node = wnPri.HisNode.NodeID;
            gwf.NodeName = wnPri.HisNode.Name;
            gwf.Update();

            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);

            //删除子线程的功能
            foreach (Node ndNext in wnPri.HisNode.HisToNodes)
            {
                i = DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + this.WorkID + " AND FK_Node=" + ndNext.NodeID);
                if (i == 0)
                    continue;

                if (ndNext.HisRunModel == RunModel.SubThread)
                {
                    /*如果到达的节点是子线程,就查询出来发起的子线程。*/
                    GenerWorkFlows gwfs = new GenerWorkFlows();
                    gwfs.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);
                    foreach (GenerWorkFlow en in gwfs)
                        BP.WF.Dev2Interface.Flow_DeleteSubThread(gwf.FK_Flow, en.WorkID, "合流节点撤销发送前，删除子线程.");
                    continue;
                }

                // 删除工作记录。
                Works wks = ndNext.HisWorks;
                if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                    wks.Delete(GenerWorkerListAttr.FID, this.WorkID);
            }
 
            ShiftWorks fws = new ShiftWorks();
            fws.Delete(ShiftWorkAttr.FK_Node, wn.HisNode.NodeID.ToString(),
                ShiftWorkAttr.WorkID, this.WorkID.ToString());

            #region 恢复工作轨迹，解决工作抢办。
            if (wnPri.HisNode.IsStartNode == false)
            {
                WorkNode ppPri = wnPri.GetPreviousWorkNode();
                wl = new GenerWorkerList();
                wl.Retrieve(GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);
                RememberMe rm = new RememberMe();
                rm.Retrieve(RememberMeAttr.FK_Node, wnPri.HisNode.NodeID, RememberMeAttr.FK_Emp, ppPri.HisWork.Rec);

                string[] empStrs = rm.Objs.Split('@');
                foreach (string s in empStrs)
                {
                    if (s == "" || s == null)
                        continue;

                    if (s == wl.FK_Emp)
                        continue;
                    GenerWorkerList wlN = new GenerWorkerList();
                    wlN.Copy(wl);
                    wlN.FK_Emp = s;

                    WF.Port.WFEmp myEmp = new Port.WFEmp(s);
                    wlN.FK_EmpText = myEmp.Name;

                    wlN.Insert();
                }
            }
            #endregion 恢复工作轨迹，解决工作抢办。

            // 删除以前的节点数据.
            wnPri.DeleteToNodesData(priFLNode.HisToNodes);
            if (wnPri.HisNode.IsStartNode)
            {
                if (BP.Web.WebUser.IsWap)
                {
                    if (wnPri.HisNode.HisFormType != NodeFormType.SDKForm)
                        return "@撤消执行成功.";
                    else
                        return "@撤销成功.";
                }
                else
                {
                    if (wnPri.HisNode.HisFormType != NodeFormType.SDKForm)
                        return "@撤消执行成功.";
                    else
                        return "@撤销成功.";
                }
            }
            else
            {
                if (BP.Web.WebUser.IsWap == false)
                {
                    return "@撤消执行成功.";
                }
                else
                {
                    return "@撤消执行成功.";
                }
            }
        }
        public string DoUnSendSubFlow(GenerWorkFlow gwf)
        {
            WorkNode wn = this.GetCurrentWorkNode();
            WorkNode wnPri = wn.GetPreviousWorkNode();

            GenerWorkerList wl = new GenerWorkerList();
            int num = wl.Retrieve(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No,
                GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID);
            if (num == 0)
                return "@您不能执行撤消发送，因为当前工作不是您发送的。";

            // 处理事件。
            string msg = wn.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneBefore, wn.HisNode, wn.HisWork, null);

            // 删除工作者。
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Delete(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, gwf.FK_Node.ToString());

            if (this.HisFlow.HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                wn.HisWork.Delete();

            gwf.FK_Node = wnPri.HisNode.NodeID;
            gwf.NodeName = wnPri.HisNode.Name;
            gwf.Update();

            BP.DA.DBAccess.RunSQL("UPDATE WF_GenerWorkerlist SET IsPass=0 WHERE WorkID=" + this.WorkID + " AND FK_Node=" + gwf.FK_Node);
            ShiftWorks fws = new ShiftWorks();
            fws.Delete(ShiftWorkAttr.FK_Node, wn.HisNode.NodeID.ToString(), ShiftWorkAttr.WorkID, this.WorkID.ToString());

            #region 判断撤消的百分比条件的临界点条件
            if (wn.HisNode.PassRate != 0)
            {
                decimal all = (decimal)BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) NUM FROM WF_GenerWorkerList WHERE FID=" + this.FID + " AND FK_Node=" + wnPri.HisNode.NodeID);
                decimal ok = (decimal)BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) NUM FROM WF_GenerWorkerList WHERE FID=" + this.FID + " AND IsPass=1 AND FK_Node=" + wnPri.HisNode.NodeID);
                decimal rate = ok / all * 100;
                if (wn.HisNode.PassRate <= rate)
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + this.FID);
                else
                    DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET IsPass=3 WHERE FK_Node=" + wn.HisNode.NodeID + " AND WorkID=" + this.FID);
            }
            #endregion

            // 处理事件。
            msg += wn.HisFlow.DoFlowEventEntity(EventListOfNode.UndoneAfter, wn.HisNode, wn.HisWork, null);

            // 记录日志..
            wn.AddToTrack(ActionType.UnSend, WebUser.No, WebUser.Name, wn.HisNode.NodeID, wn.HisNode.Name, "无");



            return "@撤消执行成功." + msg;


        }
    }
}
