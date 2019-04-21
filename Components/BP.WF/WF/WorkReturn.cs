using System;
using BP.En;
using BP.DA;
using System.Collections;
using System.Data;
using BP.Port;
using BP.Web;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template;

namespace BP.WF
{
    /// <summary>
    /// 处理工作退回
    /// </summary>
    public class WorkReturn
    {
        #region 变量
        /// <summary>
        /// 从节点
        /// </summary>
        private Node HisNode = null;
        /// <summary>
        /// 退回到节点
        /// </summary>
        private Node ReturnToNode = null;
        /// <summary>
        /// 工作ID
        /// </summary>
        private Int64 WorkID = 0;
        /// <summary>
        /// 流程ID
        /// </summary>
        private Int64 FID = 0;
        /// <summary>
        /// 是否按原路返回?
        /// </summary>
        private bool IsBackTrack = false;
        /// <summary>
        /// 退回原因
        /// </summary>
        private string Msg = "退回原因未填写.";
        /// <summary>
        /// 当前节点
        /// </summary>
        private Work HisWork = null;
        /// <summary>
        /// 退回到节点
        /// </summary>
        private Work ReurnToWork = null;
        private string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
        private Paras ps;
        public string ReturnToEmp = null;
        #endregion

        /// <summary>
        /// 工作退回
        /// </summary>
        /// <param name="fk_flow">流程编号</param>
        /// <param name="workID">WorkID</param>
        /// <param name="fid">流程ID</param>
        /// <param name="currNodeID">从节点</param>
        /// <param name="returnToNodeID">退回到节点, 0表示上一个节点，或者指定的一个节点.</param>
        /// <param name="reutrnToEmp">退回到人</param>
        /// <param name="isBackTrack">是否需要原路返回？</param>
        /// <param name="returnInfo">退回原因</param>
        public WorkReturn(string fk_flow, Int64 workID, Int64 fid, int currNodeID, int returnToNodeID, string reutrnToEmp, bool isBackTrack, string returnInfo)
        {
            this.HisNode = new Node(currNodeID);


            //如果退回的节点为0,就求出可以退回的唯一节点. @shilianyu 同步到jflow上。
            if (returnToNodeID == 0)
            {
                DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(currNodeID, workID, fid);
                if (dt.Rows.Count==0)
                    throw new Exception("err@当前节点不允许退回，系统根据退回规则没有找到可以退回的到的节点。");

                if (dt.Rows.Count!=1)
                    throw new Exception("err@当前节点可以退回的节点有["+dt.Rows.Count+"]个，您需要指定要退回的节点才能退回。");

                returnToNodeID = int.Parse( dt.Rows[0][0].ToString());
            }

            this.ReturnToNode = new Node(returnToNodeID);
            this.WorkID = workID;
            this.FID = fid;
            this.IsBackTrack = isBackTrack;
            this.Msg = returnInfo;
            this.ReturnToEmp = reutrnToEmp;

            //当前工作.
            this.HisWork = this.HisNode.HisWork;

            this.HisWork.OID = workID;
            this.HisWork.RetrieveFromDBSources();

            //退回工作
            this.ReurnToWork = this.ReturnToNode.HisWork;
            this.ReurnToWork.OID = workID;
            if (this.ReurnToWork.RetrieveFromDBSources() == 0)
            {
                this.ReurnToWork.OID = fid;
                this.ReurnToWork.RetrieveFromDBSources();
            }
        }
        /// <summary>
        /// 删除两个节点之间的业务数据与流程引擎控制数据.
        /// </summary>
        private void DeleteSpanNodesGenerWorkerListData()
        {
            if (this.IsBackTrack == true)
                return;

            Paras ps = new Paras();
            string dbStr = SystemConfig.AppCenterDBVarStr;

            // 删除FH, 不管是否有这笔数据.
            ps.Clear();

            /*如果不是退回并原路返回，就需要清除 两个节点之间的数据, 包括WF_GenerWorkerList的数据.*/
            if (this.ReturnToNode.IsStartNode == true)
            {
                // 删除其子线程流程.
                ps.Clear();
                ps.SQL = "DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbStr + "FID ";
                ps.Add("FID", this.WorkID);
                DBAccess.RunSQL(ps);

                /*如果退回到了开始的节点，就删除出开始节点以外的数据，不要删除节点表单数据，这样会导致流程轨迹打不开.*/
                ps.Clear();
                ps.SQL = "DELETE FROM WF_GenerWorkerList WHERE FK_Node!=" + dbStr + "FK_Node AND (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2)";
                ps.Add(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);
                ps.Add("WorkID1", this.WorkID);
                ps.Add("WorkID2", this.WorkID);
                DBAccess.RunSQL(ps);
                return;
            }

            /*找到发送到退回的时间点，把从这个时间点以来的数据都要删除掉.*/
            ps.Clear();
            ps.SQL = "SELECT RDT,ActionType,NDFrom FROM ND" + int.Parse(this.HisNode.FK_Flow) + "Track WHERE  NDFrom=" + dbStr + "NDFrom AND WorkID=" + dbStr + "WorkID AND ActionType=" + (int)ActionType.Forward + " ORDER BY RDT desc ";
            ps.Add("NDFrom", this.ReturnToNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count >= 1)
            {
                string rdt = dt.Rows[0][0].ToString();

                ps.Clear();
                ps.SQL = "SELECT ActionType,NDFrom FROM ND" + int.Parse(this.HisNode.FK_Flow) + "Track WHERE   RDT >=" + dbStr + "RDT AND WorkID=" + dbStr + "WorkID ORDER BY RDT ";
                ps.Add("RDT", rdt);
                ps.Add("WorkID", this.WorkID);
                dt = BP.DA.DBAccess.RunSQLReturnTable(ps);

                foreach (DataRow dr in dt.Rows)
                {
                    ActionType at = (ActionType)int.Parse(dr["ActionType"].ToString());
                    int nodeid = int.Parse(dr["NDFrom"].ToString());
                    if (nodeid == this.ReturnToNode.NodeID)
                        continue;

                    //删除中间的节点.
                    ps.Clear();
                    ps.SQL = "DELETE FROM WF_GenerWorkerList WHERE FK_Node=" + dbStr + "FK_Node AND (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2) ";
                    ps.Add("FK_Node", nodeid);
                    ps.Add("WorkID1", this.WorkID);
                    ps.Add("WorkID2", this.WorkID);
                    DBAccess.RunSQL(ps);

                    //删除审核意见
                    ps.Clear();
                    ps.SQL = "DELETE FROM ND" + int.Parse(this.ReturnToNode.FK_Flow) + "Track WHERE NDFrom=" + dbStr + "NDFrom AND  (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2) AND ActionType=22";
                    ps.Add("NDFrom", nodeid);
                    ps.Add("WorkID1", this.WorkID);
                    ps.Add("WorkID2", this.WorkID);
                    DBAccess.RunSQL(ps);
                }
            }


            //删除当前节点的数据.
            ps.Clear();
            ps.SQL = "DELETE FROM WF_GenerWorkerList WHERE FK_Node=" + dbStr + "FK_Node AND (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2) ";
            ps.Add("FK_Node", this.HisNode.NodeID);
            ps.Add("WorkID1", this.WorkID);
            ps.Add("WorkID2", this.WorkID);
            DBAccess.RunSQL(ps);

            //  string sql = "SELECT * FROM ND" + int.Parse(this.HisNode.FK_Flow) + "Track WHERE  NDTo='"+this.ReturnToNode.NodeID+" AND WorkID="+this.WorkID;
            //  ActionType
        }
        /// <summary>
        /// 队列节点上一个人退回另外一个人.
        /// </summary>
        /// <returns></returns>
        public string DoOrderReturn()
        {
            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;
            
            //如果事件返回的信息不是null，就终止执行。
            string msg = this.HisNode.HisFlow.DoFlowEventEntity(EventListOfNode.ReturnBefore, this.HisNode, this.HisWork, atPara);
            if (msg != null)
                return msg; 


            //执行退回的考核.
            Glo.InitCH(this.HisNode.HisFlow, this.HisNode, this.WorkID, this.FID, this.HisNode.Name + ":退回考核.");


            if (this.HisNode.FocusField != "")
            {
                try
                {
                    string focusField = "";
                    string[] focusFields = this.HisNode.FocusField.Split('@');
                    if (focusFields.Length >= 2)
                        focusField = focusFields[1];
                    else
                        focusField = focusFields[0];



                    // 把数据更新它。
                    this.HisWork.Update(focusField, "");
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineError("退回时更新焦点字段错误:" + ex.Message);
                }
            }

            //退回到人.
            Emp returnToEmp = new Emp(this.ReturnToEmp);

            // 退回状态。
            Paras ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkFlow SET WFState=" + dbStr + "WFState,FK_Node=" + dbStr + "FK_Node,NodeName=" + dbStr + "NodeName,TodoEmps=" + dbStr + "TodoEmps, TodoEmpsNum=0 WHERE  WorkID=" + dbStr + "WorkID";
            ps.Add(GenerWorkFlowAttr.WFState, (int)WFState.ReturnSta);
            ps.Add(GenerWorkFlowAttr.FK_Node, this.ReturnToNode.NodeID);
            ps.Add(GenerWorkFlowAttr.NodeName, this.ReturnToNode.Name);

            ps.Add(GenerWorkFlowAttr.TodoEmps, returnToEmp.No + "," + returnToEmp.Name + ";");

            ps.Add(GenerWorkFlowAttr.WorkID, this.WorkID);

            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0,IsRead=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp ";
            ps.Add("FK_Node", this.ReturnToNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            ps.Add("FK_Emp", this.ReturnToEmp);
            DBAccess.RunSQL(ps);

            ps = new Paras();
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=1000,IsRead=0 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND FK_Emp=" + dbStr + "FK_Emp ";
            ps.Add("FK_Node", this.HisNode.NodeID);
            ps.Add("WorkID", this.WorkID);
            ps.Add("FK_Emp", WebUser.No);
            DBAccess.RunSQL(ps);

            //更新流程报表数据.
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisNode.HisFlow.PTable + " SET  WFState=" + dbStr + "WFState, FlowEnder=" + dbStr + "FlowEnder, FlowEndNode=" + dbStr + "FlowEndNode WHERE OID=" + dbStr + "OID";
            ps.Add("WFState", (int)WFState.ReturnSta);
            ps.Add("FlowEnder", WebUser.No);
            ps.Add("FlowEndNode", ReturnToNode.NodeID);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);

            ////从工作人员列表里找到被退回人的接受人.
            //GenerWorkerList gwl = new GenerWorkerList();
            //gwl.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);

            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = this.ReturnToEmp; //退回给。
            rw.BeiZhu = Msg;

            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.Insert();

            // 加入track.
            this.AddToTrack(ActionType.Return, returnToEmp.No, returnToEmp.Name,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);

            try
            {
                // 记录退回日志.
                ReorderLog(this.HisNode, this.ReturnToNode, rw);
            }
            catch (Exception ex)
            {
                Log.DebugWriteWarning(ex.Message);
            }

            // 以退回到的节点向前数据用递归删除它。
            if (IsBackTrack == false)
            {
                /*如果退回不需要原路返回，就删除中间点的数据。*/
#warning 没有考虑两种流程数据存储模式。
                //DeleteToNodesData(this.ReturnToNode.HisToNodes);
            }

            // 向他发送消息。
            if (Glo.IsEnableSysMessage == true)
            {
                //   WF.Port.WFEmp wfemp = new Port.WFEmp(wnOfBackTo.HisWork.Rec);
                string title = string.Format("工作退回：流程:{0}.工作:{1},退回人:{2},需您处理",
                    this.HisNode.FlowName, this.ReturnToNode.Name, WebUser.Name);

                BP.WF.Dev2Interface.Port_SendMsg(returnToEmp.No, title, Msg, "RE" + this.HisNode.NodeID + this.WorkID, BP.WF.SMSMsgType.ReturnAfter, ReturnToNode.FK_Flow, ReturnToNode.NodeID, this.WorkID, this.FID);
            }

            //退回后事件
            string text = this.HisNode.HisFlow.DoFlowEventEntity(EventListOfNode.ReturnAfter, this.HisNode, this.HisWork,
                atPara);
            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";
            // 返回退回信息.
            if (this.ReturnToNode.IsGuestNode)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                return "工作已经被您退回到(" + this.ReturnToNode.Name + "),退回给(" + gwf.GuestNo + "," + gwf.GuestName + ").\n\r" + text;
            }
            else
            {
                return "工作已经被您退回到(" + this.ReturnToNode.Name + "),退回给(" + returnToEmp.No + "," + returnToEmp.Name + ").\n\r" + text;
            }
        }
        /// <summary>
        /// 要退回到父流程上去
        /// </summary>
        /// <returns></returns>
        private string ReturnToParentFlow()
        {
            //当前 gwf.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            //设置子流程信息.
            GenerWorkFlow gwfP = new GenerWorkFlow(gwf.PWorkID);
            gwfP.WFState = WFState.ReturnSta;
            gwfP.FK_Node = this.ReturnToNode.NodeID;
            gwfP.NodeName = this.ReturnToNode.Name;

            //int toNodeID = this.ReturnToNode;

            //启用待办.
            GenerWorkerList gwl = new GenerWorkerList();
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID, GenerWorkerListAttr.WorkID, gwfP.WorkID);

            string toEmps = "";
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPassInt = 0;
                item.Update();
                gwl = item;

                toEmps += item.FK_Emp + "," + item.FK_EmpText + ";";
            }

            gwfP.TodoEmps = toEmps;
            gwfP.Update();

            #region 写入退回提示.
            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID = gwfP.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = gwfP.NodeName;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = gwl.FK_Emp; //退回给。
            rw.BeiZhu = Msg;

            // 去掉了 else .
            rw.IsBackTracking = this.IsBackTrack;
            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.Insert();


            // 加入track.
            this.AddToTrack(ActionType.Return, gwl.FK_Emp, gwl.FK_EmpText,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            #endregion

            //删除当前的流程.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(gwf.FK_Flow, this.WorkID, true);

            //设置当前为未读的状态.
            BP.WF.Dev2Interface.Node_SetWorkUnRead(gwfP.WorkID);

            //返回退回信息.
            return "成功的退回到[" + gwfP.FlowName + " - " + this.ReturnToNode.Name + "],退回给[" + toEmps + "].";
        }
        /// <summary>
        /// 执行退回.
        /// </summary>
        /// <returns>返回退回信息</returns>
        public string DoIt()
        {

            // 增加要退回到父流程上去. by zhoupeng.
            if (this.ReturnToNode.FK_Flow.Equals(this.HisNode.FK_Flow) == false)
            {
                /*子流程要退回到父流程的情况.*/
                return ReturnToParentFlow();
            }


            if (this.HisNode.NodeID == this.ReturnToNode.NodeID)
            {
                if (this.HisNode.TodolistModel == TodolistModel.Order)
                {
                    /*一个队列的模式，一个人退回给另外一个人 */
                    return DoOrderReturn();
                }
            }

            if (this.ReturnToNode.TodolistModel == TodolistModel.Order)
            {
                /* 当退回到的节点是 队列模式或者是协作模式时. */
                return DoOrderReturn();
            }

            /* 删除退回选择的信息, forzhuhai: 退回后，删除发送人上次选择的信息.
             * 
             * 场景:
             * 1, a b c d 节点 d节点退回给c 如果d的接收人是c来选择的, 他退回后要把d的选择信息删除掉.
             * 2, a b c d 节点 d节点退回给a 如果 b c d 的任何一个接受人的范围是有上一步发送人来选择的，就要删除选择人的信息.
             * 
             * */

            //是否需要删除中间点. 
            bool isNeedDeleteSpanNodes = true;
            string sql = "";
            foreach (Node nd in this.ReturnToNode.HisToNodes)
            {
                if (nd.NodeID == this.HisNode.NodeID)
                {
                    sql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID;
                    BP.DA.DBAccess.RunSQL(sql);
                    isNeedDeleteSpanNodes = false;
                }
            }

            //如果有中间步骤.
            if (isNeedDeleteSpanNodes)
            {
                //获得可以退回的节点，这个节点是有顺序的.
                DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.HisNode.NodeID, this.WorkID, this.FID);
                bool isDelBegin = false;
                foreach (DataRow dr in dt.Rows)
                {
                    int nodeID = int.Parse(dr["No"].ToString());

                    if (nodeID == this.ReturnToNode.NodeID)
                        isDelBegin = true; /*如果等于当前的节点，就开始把他们删除掉.*/

                    if (isDelBegin)
                    {
                        sql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + nodeID + " AND WorkID=" + this.WorkID;
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                }

                // 删除当前节点信息.
                sql = "DELETE FROM WF_SelectAccper WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID;
                BP.DA.DBAccess.RunSQL(sql);
            }


            //删除.
            Template.FrmWorkCheck fwc = new Template.FrmWorkCheck(this.HisNode.NodeID);
            if (fwc.FWCIsShowReturnMsg == false)
                BP.WF.Dev2Interface.DeleteCheckInfo(this.HisNode.FK_Flow, this.WorkID, this.HisNode.NodeID);

            //删除审核组件设置“协作模式下操作员显示顺序”为“按照接受人员列表先后顺序(官职大小)”，而生成的待审核轨迹信息
            if (fwc.FWCSta == FrmWorkCheckSta.Enable && fwc.FWCOrderModel == FWCOrderModel.SqlAccepter)
            {
                BP.DA.DBAccess.RunSQL("DELETE FROM ND" + int.Parse(this.HisNode.FK_Flow) + "Track WHERE WorkID = " + this.WorkID +
                                      " AND ActionType = " + (int)ActionType.WorkCheck + " AND NDFrom = " + this.HisNode.NodeID +
                                      " AND NDTo = " + this.HisNode.NodeID + " AND (Msg = '' OR Msg IS NULL)");
            }

            switch (this.HisNode.HisRunModel)
            {
                case RunModel.Ordinary: /* 1： 普通节点向下发送的*/
                    switch (ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary:   /*1-1 普通节to普通节点 */
                            return ExeReturn1_1(); //
                            break;
                        case RunModel.FL:  /* 1-2 普通节to分流点   */
                            return ExeReturn1_1(); //
                            break;
                        case RunModel.HL:  /*1-3 普通节to合流点   */
                            return ExeReturn1_1(); //
                            break;
                        case RunModel.FHL: /*1-4 普通节点to分合流点 */
                            return ExeReturn1_1();
                            break;
                        case RunModel.SubThread: /*1-5 普通节to子线程点 */
                        default:
                            throw new Exception("@退回错误:非法的设计模式或退回模式.普通节to子线程点");
                            break;
                    }
                    break;
                case RunModel.FL: /* 2: 分流节点向下发送的*/
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary:    /*2.1 分流点to普通节点 */
                            return ExeReturn1_1(); //
                        case RunModel.FL:  /*2.2 分流点to分流点  */
                        case RunModel.HL:  /*2.3 分流点to合流点,分合流点   */
                        case RunModel.FHL:
                            return ExeReturn1_1(); //
                        case RunModel.SubThread: /* 2.4 分流点to子线程点   */
                            return ExeReturn2_4(); //
                        // throw new Exception("@退回错误:非法的设计模式或退回模式.分流点to子线程点,请反馈给管理员.");
                        default:
                            throw new Exception("@没有判断的节点类型(" + ReturnToNode.Name + ")");
                            break;
                    }
                    break;
                case RunModel.HL:  /* 3: 合流节点向下退回 */
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*3.1 普通工作节点 */
                            return ExeReturn1_1(); //
                        case RunModel.FL: /*3.2 合流点向分流点退回 */
                            return ExeReturn3_2(); //
                        case RunModel.HL: /*3.3 合流点 */
                        case RunModel.FHL:
                            throw new Exception("@尚未完成.");
                        case RunModel.SubThread:/*3.4 合流点向子线程退回 */
                            return ExeReturn3_4();
                        default:
                            throw new Exception("@退回错误:非法的设计模式或退回模式.普通节to子线程点");
                    }
                    break;
                case RunModel.FHL:  /* 4: 分流节点向下发送的 */
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*4.1 普通工作节点 */
                            return ExeReturn1_1();
                        case RunModel.FL: /*4.2 分流点 */
                        case RunModel.HL: /*4.3 合流点 */
                        case RunModel.FHL:
                            throw new Exception("@尚未完成.");
                        case RunModel.SubThread:/*4.5 子线程*/
                            return ExeReturn3_4();
                        default:
                            throw new Exception("@没有判断的节点类型(" + this.ReturnToNode.Name + ")");
                    }
                    break;
                case RunModel.SubThread:  /* 5: 子线程节点向下发送的 */
                    switch (this.ReturnToNode.HisRunModel)
                    {
                        case RunModel.Ordinary: /*5.1 普通工作节点 */
                            throw new Exception("@非法的退回模式,,请反馈给管理员.");
                        case RunModel.FL: /*5.2 分流点 */
                            /*子线程退回给分流点.*/
                            return ExeReturn5_2();
                        case RunModel.HL: /*5.3 合流点 */
                            throw new Exception("@非法的退回模式,请反馈给管理员.");
                        case RunModel.FHL: /*5.4 分合流点 */
                            return ExeReturn5_2();
                        //throw new Exception("@目前不支持此场景下的退回,请反馈给管理员.");
                        case RunModel.SubThread: /*5.5 子线程*/
                            return ExeReturn1_1();
                        default:
                            throw new Exception("@没有判断的节点类型(" + ReturnToNode.Name + ")");
                    }
                    break;
                default:
                    throw new Exception("@没有判断的退回类型:" + this.HisNode.HisRunModel);
            }
            throw new Exception("@系统出现未判断的异常.");
        }
        /// <summary>
        /// 分流点退回给子线程
        /// </summary>
        /// <returns></returns>
        private string ExeReturn2_4()
        {
            //更新运动到节点,但是仍然是退回状态.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.FK_Node = this.ReturnToNode.NodeID;
            //增加参与的人员
            string emps = gwf.Emps;
            if (emps.Contains("@" + WebUser.No) == false)
            {
                if (DataType.IsNullOrEmpty(emps) == true)
                    emps = "@" + WebUser.No + "@";
                else
                    emps += WebUser.No + "@";
            }
            gwf.Emps = emps;
            gwf.Update();

            //更新退回到的人员信息可见.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPassInt = 0;
                item.Update();
                this.ReturnToEmp = item.FK_Emp + "," + item.FK_EmpText;
            }

            // 去掉合流节点工作人员的待办.
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPassInt = 0;
                item.IsRead = false;
                item.Update();
            }

            //把分流节点的待办去掉. 
            gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FID, this.FID, GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No);
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPassInt = -2;
                item.Update();
            }
            return "成功的把信息退回到：" + this.ReturnToNode.Name + " , 退回给:(" + this.ReturnToEmp + ")";
        }
        /// <summary>
        /// 子线程退回给分流点
        /// </summary>
        /// <returns></returns>
        private string ExeReturn5_2()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.FK_Node = this.ReturnToNode.NodeID;
            string info = "@工作已经成功的退回到（" + ReturnToNode.Name + "）退回给：";

            //查询退回到的工作人员列表.
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);

            string toEmp = "";
            string toEmpName = "";
            if (gwls.Count == 1)
            {
                /*有可能多次退回的情况，表示曾经退回过n次。*/
                foreach (GenerWorkerList item in gwls)
                {
                    item.IsPass = false; // 显示待办, 这个是合流节点的工作人员.
                    item.IsRead = false; //
                    item.Update();
                    info += item.FK_Emp + "," + item.FK_EmpText;
                    toEmp = item.FK_Emp;
                    toEmpName = item.FK_EmpText;
                    info += "(" + item.FK_Emp + "," + item.FK_EmpText + ")";
                }
            }
            else
            {
                // 找到合流点的发送人.
                Nodes nds = this.HisNode.FromNodes;
                gwls = new GenerWorkerLists();
                GenerWorkerList gwl = new GenerWorkerList();
                foreach (Node nd in nds)
                {
                    gwls.Retrieve(GenerWorkerListAttr.WorkID, this.FID,
                        GenerWorkerListAttr.FK_Node, nd.NodeID,
                        GenerWorkerListAttr.IsPass, 1);
                    if (gwls.Count == 0)
                        continue;

                    if (gwls.Count != 1)
                        throw new Exception("@应该只有一个记录，现在有多个，可能错误。");

                    //求出分流节点的发送人.
                    gwl = (GenerWorkerList)gwls[0];
                    toEmp = gwl.FK_Emp;
                    toEmpName = gwl.FK_EmpText;
                    info += "(" + toEmp + "," + toEmpName + ")";
                }

                if (DataType.IsNullOrEmpty(toEmp) == true)
                    throw new Exception("@在退回时出现错误，没有找到分流节点的发送人。");

                // 插入一条数据, 行程一个工作人员记录,这个记录就是子线程的延长点. 给合流点上的接受人设置待办.
                gwl.WorkID = this.WorkID;
                gwl.FID = this.FID;
                gwl.IsPass = false;
                if (gwl.IsExits == false)
                    gwl.Insert();
                else
                    gwl.Update();
            }

            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = toEmp; //退回给。

            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.BeiZhu = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.Insert();

            // 加入track.
            this.AddToTrack(ActionType.Return, toEmp, toEmpName,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);

            BP.WF.Dev2Interface.Port_SendMsg(toEmp, gwf.Title, Msg, "RE" + this.HisNode.NodeID + this.WorkID, BP.WF.SMSMsgType.ReturnAfter, ReturnToNode.FK_Flow, ReturnToNode.NodeID, this.WorkID, this.FID);

            gwf.WFState = WFState.ReturnSta;
            gwf.FK_Node = this.ReturnToNode.NodeID;
            gwf.NodeName = this.ReturnToNode.Name;
            gwf.Starter = toEmp;
            gwf.StarterName = toEmpName;
            gwf.Sender = WebUser.No;
            //增加参与的人员
            string emps = gwf.Emps;
            if (emps.Contains("@" + WebUser.No) == false)
            {
                if(DataType.IsNullOrEmpty(emps) == true)
                    emps = "@"+WebUser.No + "@";
                else
                    emps += WebUser.No + "@";
            }
            gwf.Emps = emps;
            gwf.Update();

            //找到当前的工作数据.
            GenerWorkerList currWorker = new GenerWorkerList();
            int i = currWorker.Retrieve(GenerWorkerListAttr.FK_Emp, WebUser.No,
                 GenerWorkerListAttr.WorkID, this.WorkID,
                 GenerWorkerListAttr.FK_Node, this.HisNode.NodeID);

            if (i != 1)
                throw new Exception("@当前的工作人员列表数据丢失了,流程引擎错误.");

            //设置当前的工作数据为退回状态,让其不能看到待办, 这个是约定的值.
            currWorker.IsPassInt = (int)WFState.ReturnSta;
            currWorker.Update();

            // 返回退回信息.
            return info;
        }
        /// <summary>
        /// 合流点向子线程退回
        /// </summary>
        private string ExeReturn3_4()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.FK_Node = this.ReturnToNode.NodeID;

            string info = "@工作已经成功的退回到（" + ReturnToNode.Name + "）退回给：";
            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);

            string toEmp = "";
            string toEmpName = "";
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPass = false;
                item.IsRead = false;
                item.Update();
                info += item.FK_Emp + "," + item.FK_EmpText;
                toEmp = item.FK_Emp;
                toEmpName = item.FK_EmpText;
            }

            //删除已经发向合流点的汇总数据.
            MapDtls dtls = new MapDtls("ND" + this.HisNode.NodeID);
            foreach (MapDtl dtl in dtls)
            {
                /*如果是合流数据*/
                if (dtl.IsHLDtl)
                    BP.DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE OID=" + this.WorkID);
            }

            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = toEmp; //退回给。

            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.BeiZhu = Msg;
            rw.IsBackTracking = this.IsBackTrack;
            rw.Insert();

            // 加入track.
            this.AddToTrack(ActionType.Return, toEmp, toEmpName,
                this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            BP.WF.Dev2Interface.Port_SendMsg(toEmp, gwf.Title, Msg, "RE" + this.HisNode.NodeID + this.WorkID, BP.WF.SMSMsgType.ReturnAfter, ReturnToNode.FK_Flow, ReturnToNode.NodeID, this.WorkID, this.FID);

            gwf.WFState = WFState.ReturnSta;
            gwf.Sender = WebUser.No;
            //增加参与的人员
            string emps = gwf.Emps;
            if (emps.Contains("@" + WebUser.No) == false)
            {
                if (DataType.IsNullOrEmpty(emps) == true)
                    emps = "@" + WebUser.No + "@";
                else
                    emps += WebUser.No + "@";
            }
            gwf.Emps = emps;

            gwf.Update();

            // 返回退回信息.
            return info;
        }
        /// <summary>
        /// 合流点向分流点退回
        /// </summary>
        private string ExeReturn3_2()
        {
            //删除分流点与合流点之间的子线程数据。
            //if (this.ReturnToNode.IsStartNode == false)
            //    throw new Exception("@没有处理的模式。");

            //求出来退回到的 时间点。
            GenerWorkerList toWL = new GenerWorkerList();
            toWL.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID,
                GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID);


            //如果是仅仅退回，就删除子线程数据。
            if (this.IsBackTrack == false)
            {
                //删除子线程节点数据。
                GenerWorkerLists gwls = new GenerWorkerLists();
                gwls.Retrieve(GenerWorkFlowAttr.FID, this.WorkID);

                foreach (GenerWorkerList item in gwls)
                {
                    if (item.RDT.CompareTo(toWL.RDT) == -1)
                        continue;

                    /* 删除 子线程数据 */
                    if (DBAccess.IsExitsObject("ND" + item.FK_Node) == true)
                        DBAccess.RunSQL("DELETE FROM ND" + item.FK_Node + " WHERE OID=" + item.WorkID);

                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + this.WorkID + " AND FK_Node=" + item.FK_Node);
                }

                //删除流程控制数据。
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + this.WorkID);
            }

            return ExeReturn1_1();
        }
        /// <summary>
        /// 普通节点到普通节点的退回
        /// </summary>
        /// <returns></returns>
        private string ExeReturn1_1()
        {
            //为软通小杨处理rpt变量不能替换的问题.
            GERpt rpt = this.HisNode.HisFlow.HisGERpt;
            rpt.OID = this.WorkID;
            if (rpt.RetrieveFromDBSources() == 0)
            {
                rpt.OID = this.FID;
                rpt.RetrieveFromDBSources();
            }
            rpt.Row.Add("ReturnMsg", Msg);

            Flow fl = this.HisNode.HisFlow;

            //退回前事件
            string atPara = "@ToNode=" + this.ReturnToNode.NodeID;

            //如果事件返回的信息不是null，就终止执行。
            string msg = fl.DoFlowEventEntity(EventListOfNode.ReturnBefore, this.HisNode, rpt,
                atPara);
            if (msg != null)
                return msg; 

            if (this.HisNode.FocusField != "")
            {
                try
                {
                    string focusField = "";
                    string[] focusFields = this.HisNode.FocusField.Split('@');
                    if (focusFields.Length >= 2)
                        focusField = focusFields[1];
                    else
                        focusField = focusFields[0];

                    // 把数据更新它。
                    this.HisWork.Update(focusField, "");
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineError("退回时更新焦点字段错误:" + ex.Message);
                }
            }


            // 计算出来 退回到节点的应完成时间. 
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            DateTime dtOfShould;
            if (fl.HisTimelineRole == Template.TimelineRole.ByFlow)
            {
                /*如果整体流程是按流程设置计算 */
                dtOfShould = DataType.ParseSysDateTime2DateTime(gwf.SDTOfFlow);
            }
            else
            {
                //增加天数. 考虑到了节假日.             
                dtOfShould = Glo.AddDayHoursSpan(DateTime.Now, this.ReturnToNode.TimeLimit,
                    this.ReturnToNode.TimeLimitHH, this.ReturnToNode.TimeLimitMM, this.ReturnToNode.TWay);
            }
            // 应完成日期.
            string sdt = dtOfShould.ToString(DataType.SysDataTimeFormat);

            // 改变当前待办工作节点
            gwf.WFState = WFState.ReturnSta;
            gwf.FK_Node = this.ReturnToNode.NodeID;
            gwf.NodeName = this.ReturnToNode.Name;
            gwf.SDTOfNode = sdt;

            gwf.Sender = WebUser.No + "," + WebUser.Name;
            gwf.HuiQianTaskSta = HuiQianTaskSta.None;
            gwf.HuiQianZhuChiRen = "";
            gwf.HuiQianZhuChiRenName = "";

            //如果已经计算出来要退回到的人了,就删除其他相关的人员.
            if (DataType.IsNullOrEmpty(this.ReturnToEmp) == false)
            {
                string sql = "DELETE FROM WF_GenerWorkerList WHERE FK_Node=" + this.ReturnToNode.NodeID + " AND WorkID=" + this.WorkID + " AND FK_Emp!='" + this.ReturnToEmp + "'";
                DBAccess.RunSQL(sql);
            }


            GenerWorkerLists gwls = new GenerWorkerLists();
            gwls.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);

            //更新当前待办人员.
            string toDoEmps = "";
            foreach (GenerWorkerList item in gwls)
            {
                toDoEmps += item.FK_Emp + "," + item.FK_EmpText + ";";
            }

            //增加参与的人员
            string emps = gwf.Emps;
            if (emps.Contains("@" + WebUser.No) == false)
            {
                if (DataType.IsNullOrEmpty(emps) == true)
                    emps = "@" + WebUser.No + "@";
                else
                    emps += WebUser.No + "@";
            }

            gwf.TodoEmps = toDoEmps;
            gwf.TodoEmpsNum = gwls.Count;
            gwf.Emps = emps;
            gwf.Update();

            
            //更新待办状态.
            foreach (GenerWorkerList item in gwls)
            {
                item.IsPassInt = 0;
                item.IsRead = false;
                item.SDT = sdt;
                item.RDT = DataType.CurrentDataTime;
                item.Sender = WebUser.Name + "," + WebUser.No;
                item.Update();
            }

            //更新流程报表数据.
            ps = new Paras();
            ps.SQL = "UPDATE " + this.HisNode.HisFlow.PTable + " SET  WFState=" + dbStr + "WFState, FlowEnder=" + dbStr + "FlowEnder, FlowEndNode=" + dbStr + "FlowEndNode WHERE OID=" + dbStr + "OID";
            ps.Add("WFState", (int)WFState.ReturnSta);
            ps.Add("FlowEnder", WebUser.No);
            ps.Add("FlowEndNode", ReturnToNode.NodeID);
            ps.Add("OID", this.WorkID);
            DBAccess.RunSQL(ps);

            //从工作人员列表里找到被退回人的接受人.
            GenerWorkerList gwl = new GenerWorkerList();
            gwl.Retrieve(GenerWorkerListAttr.FK_Node, this.ReturnToNode.NodeID, GenerWorkerListAttr.WorkID, this.WorkID);

            // 记录退回轨迹。
            ReturnWork rw = new ReturnWork();
            rw.WorkID = this.WorkID;
            rw.ReturnToNode = this.ReturnToNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;

            rw.ReturnNode = this.HisNode.NodeID; // 当前退回节点.
            rw.ReturnToEmp = gwl.FK_Emp; //退回给。
            rw.BeiZhu = Msg;
            //杨玉慧 
            Emp emp = new Emp(rw.ReturnToEmp);

            if (this.HisNode.TodolistModel == TodolistModel.Order
                || this.HisNode.TodolistModel == TodolistModel.Sharing
                || this.HisNode.TodolistModel == TodolistModel.TeamupGroupLeader
                || this.HisNode.TodolistModel == TodolistModel.Teamup)
            {

                // 为软通小杨屏蔽， 共享，顺序，协作模式的退回并原路返回的 问题. 
                //rw.IsBackTracking = true; /*如果是共享，顺序，协作模式，都必须是退回并原路返回.*/

                // 需要更新当前人待办的状态, 把1000作为特殊标记，让其发送时可以找到他.
                string sql = "UPDATE WF_GenerWorkerlist SET IsPass=1000 WHERE FK_Node=" + this.HisNode.NodeID + " AND WorkID=" + this.WorkID + " AND FK_Emp='" + WebUser.No + "'";
                if (BP.DA.DBAccess.RunSQL(sql) == 0 && 1 == 2)
                    throw new Exception("@退回错误,没有找到要更新的目标数据.技术信息:" + sql);

                //杨玉慧 将流程的  任务池状态设置为  NONE
                sql = "UPDATE WF_GenerWorkFlow SET TaskSta=0 WHERE  WorkID=" + this.WorkID;
                if (BP.DA.DBAccess.RunSQL(sql) == 0 && 1 == 2)
                    throw new Exception("@退回错误，没有找到要更新的目标数据.技术信息:" + sql);
            }

            // 去掉了 else .
            rw.IsBackTracking = this.IsBackTrack;

            //调用删除GenerWorkerList数据，不然会导致两个节点之间有垃圾数据，特别遇到中间有分合流时候。
            this.DeleteSpanNodesGenerWorkerListData();


            rw.MyPK = DBAccess.GenerOIDByGUID().ToString();
            rw.Insert();

            // 为电建增加一个退回并原路返回的日志类型.
            if (IsBackTrack == true)
            {
                // 加入track.
                this.AddToTrack(ActionType.ReturnAndBackWay, gwl.FK_Emp, gwl.FK_EmpText,
                    this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            }
            else
            {
                // 加入track.
                this.AddToTrack(ActionType.Return, gwl.FK_Emp, gwl.FK_EmpText,
                    this.ReturnToNode.NodeID, this.ReturnToNode.Name, Msg);
            }

            try
            {
                // 记录退回日志. this.HisNode, this.ReturnToNode
                ReorderLog(this.ReturnToNode, this.HisNode, rw);
            }
            catch (Exception ex)
            {
                Log.DebugWriteWarning(ex.Message);
            }


            //把退回原因加入特殊变量里. 为软通小杨处理rpt变量不能替换的问题.
            // string text = fl.DoFlowEventEntity(EventListOfNode.ReturnAfter, this.HisNode, rpt,atPara, null, gwl.FK_Emp);

            // 把消息
            atPara += "@SendToEmpIDs=" + gwl.FK_Emp;

            string text = fl.DoFlowEventEntity(EventListOfNode.ReturnAfter, this.HisNode, rpt, atPara);
            if (text == null)
                text = "";

            if (text != null && text.Length > 1000)
                text = "退回事件:无返回信息.";

            // 返回退回信息.
            if (this.ReturnToNode.IsGuestNode)
            {
                return "工作已经被您退回到(" + this.ReturnToNode.Name + "),退回给(" + gwf.GuestNo + "," + gwf.GuestName + ").\n\r" + text;
            }
            else
            {
                return "工作已经被您退回到(" + this.ReturnToNode.Name + "),退回给(" + gwl.FK_Emp + "," + gwl.FK_EmpText + ").\n\r" + text;
            }
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="at">类型</param>
        /// <param name="toEmp">到人员</param>
        /// <param name="toEmpName">到人员名称</param>
        /// <param name="toNDid">到节点</param>
        /// <param name="toNDName">到节点名称</param>
        /// <param name="msg">消息</param>
        public void AddToTrack(ActionType at, string toEmp, string toEmpName, int toNDid, string toNDName, string msg)
        {
            Track t = new Track();
            t.WorkID = this.WorkID;
            t.FK_Flow = this.HisNode.FK_Flow;
            t.FID = this.FID;
            t.RDT = DataType.CurrentDataTimess;
            t.HisActionType = at;

            t.NDFrom = this.HisNode.NodeID;
            t.NDFromT = this.HisNode.Name;

            t.EmpFrom = WebUser.No;
            t.EmpFromT = WebUser.Name;
            t.FK_Flow = this.HisNode.FK_Flow;

            if (toNDid == 0)
            {
                toNDid = this.HisNode.NodeID;
                toNDName = this.HisNode.Name;
            }


            t.NDTo = toNDid;
            t.NDToT = toNDName;

            t.EmpTo = toEmp;
            t.EmpToT = toEmpName;
            t.Msg = msg;
            t.Insert();
        }
        private string infoLog = "";
        private void ReorderLog(Node fromND, Node toND, ReturnWork rw)
        {
            string filePath = BP.Sys.SystemConfig.PathOfDataUser + "\\ReturnLog\\" + this.HisNode.FK_Flow + "\\";
            if (System.IO.Directory.Exists(filePath) == false)
                System.IO.Directory.CreateDirectory(filePath);

            string file = filePath + "\\" + rw.MyPK;
            infoLog = "\r\n退回人:" + WebUser.No + "," + WebUser.Name + " \r\n退回节点:" + fromND.Name + " \r\n退回到:" + toND.Name;
            infoLog += "\r\n退回时间:" + DataType.CurrentDataTime;
            infoLog += "\r\n原因:" + rw.BeiZhu;

            ReorderLog(fromND, toND);
            DataType.WriteFile(file + ".txt", infoLog);
            DataType.WriteFile(file + ".htm", infoLog.Replace("\r\n", "<br>"));

            // this.HisWork.Delete();
        }
        private void ReorderLog(Node fromND, Node toND)
        {
            /*开始遍历到达的节点集合*/
            foreach (Node nd in fromND.HisToNodes)
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                if (wk.RetrieveFromDBSources() == 0)
                {
                    wk.FID = this.WorkID;
                    if (wk.Retrieve(WorkAttr.FID, this.WorkID) == 0)
                        continue;
                }

                if (nd.IsFL)
                {
                    /* 如果是分流 */
                    GenerWorkerLists wls = new GenerWorkerLists();
                    QueryObject qo = new QueryObject(wls);
                    qo.AddWhere(GenerWorkerListAttr.FID, this.WorkID);
                    qo.addAnd();

                    string[] ndsStrs = nd.HisToNDs.Split('@');
                    string inStr = "";
                    foreach (string s in ndsStrs)
                    {
                        if (s == "" || s == null)
                            continue;
                        inStr += "'" + s + "',";
                    }
                    inStr = inStr.Substring(0, inStr.Length - 1);
                    if (inStr.Contains(",") == true)
                        qo.AddWhere(GenerWorkerListAttr.FK_Node, int.Parse(inStr));
                    else
                        qo.AddWhereIn(GenerWorkerListAttr.FK_Node, "(" + inStr + ")");

                    qo.DoQuery();
                    foreach (GenerWorkerList wl in wls)
                    {
                        Node subNd = new Node(wl.FK_Node);
                        Work subWK = subNd.GetWork(wl.WorkID);

                        infoLog += "\r\n*****************************************************************************************";
                        infoLog += "\r\n节点ID:" + subNd.NodeID + "  工作名称:" + subWK.EnDesc;
                        infoLog += "\r\n处理人:" + subWK.Rec + " , " + wk.RecOfEmp.Name;
                        infoLog += "\r\n接收时间:" + subWK.RDT + " 处理时间:" + subWK.CDT;
                        infoLog += "\r\n ------------------------------------------------- ";

                        foreach (Attr attr in wk.EnMap.Attrs)
                        {
                            if (attr.UIVisible == false)
                                continue;
                            infoLog += "\r\n " + attr.Desc + ":" + subWK.GetValStrByKey(attr.Key);
                        }

                        //递归调用。 //递归调用。  先把此处注释掉   会造成死循环 杨玉慧
                        //ReorderLog(subNd, toND);
                    }
                }
                else
                {
                    infoLog += "\r\n*****************************************************************************************";
                    infoLog += "\r\n节点ID:" + wk.NodeID + "  工作名称:" + wk.EnDesc;
                    infoLog += "\r\n处理人:" + wk.Rec + " , " + wk.RecOfEmp.Name;
                    infoLog += "\r\n接收时间:" + wk.RDT + " 处理时间:" + wk.CDT;
                    infoLog += "\r\n ------------------------------------------------- ";

                    foreach (Attr attr in wk.EnMap.Attrs)
                    {
                        if (attr.UIVisible == false)
                            continue;
                        infoLog += "\r\n" + attr.Desc + " : " + wk.GetValStrByKey(attr.Key);
                    }
                }

                /* 如果到了当前的节点 */
                if (nd.NodeID == toND.NodeID)
                    break;

                //递归调用。  先把此处注释掉   会造成死循环 杨玉慧
                //ReorderLog(nd, toND);
            }
        }
        /// <summary>
        /// 递归删除两个节点之间的数据
        /// </summary>
        /// <param name="nds">到达的节点集合</param>
        public void DeleteToNodesData(Nodes nds)
        {
            /*开始遍历到达的节点集合*/
            foreach (Node nd in nds)
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                if (wk.Delete() == 0)
                {
                    wk.FID = this.WorkID;
                    if (wk.Delete(WorkAttr.FID, this.WorkID) == 0)
                        continue;
                }

                #region 删除当前节点数据，删除附件信息。
                // 删除明细表信息。
                MapDtls dtls = new MapDtls("ND" + nd.NodeID);
                foreach (MapDtl dtl in dtls)
                {
                    ps = new Paras();
                    ps.SQL = "DELETE FROM " + dtl.PTable + " WHERE RefPK=" + dbStr + "WorkID";
                    ps.Add("WorkID", this.WorkID.ToString());
                    BP.DA.DBAccess.RunSQL(ps);
                }

                // 删除表单附件信息。
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmAttachmentDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                // 删除签名信息。
                BP.DA.DBAccess.RunSQL("DELETE FROM Sys_FrmEleDB WHERE RefPKVal=" + dbStr + "WorkID AND FK_MapData=" + dbStr + "FK_MapData ",
                    "WorkID", this.WorkID.ToString(), "FK_MapData", "ND" + nd.NodeID);
                #endregion 删除当前节点数据。


                /*说明:已经删除该节点数据。*/
                DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE (WorkID=" + dbStr + "WorkID1 OR FID=" + dbStr + "WorkID2 ) AND FK_Node=" + dbStr + "FK_Node",
                    "WorkID1", this.WorkID, "WorkID2", this.WorkID, "FK_Node", nd.NodeID);

                if (nd.IsFL)
                {
                    /* 如果是分流 */
                    GenerWorkerLists wls = new GenerWorkerLists();
                    QueryObject qo = new QueryObject(wls);
                    qo.AddWhere(GenerWorkerListAttr.FID, this.WorkID);
                    qo.addAnd();

                    string[] ndsStrs = nd.HisToNDs.Split('@');
                    string inStr = "";
                    foreach (string s in ndsStrs)
                    {
                        if (s == "" || s == null)
                            continue;
                        inStr += "'" + s + "',";
                    }
                    inStr = inStr.Substring(0, inStr.Length - 1);
                    if (inStr.Contains(",") == true)
                        qo.AddWhere(GenerWorkerListAttr.FK_Node, int.Parse(inStr));
                    else
                        qo.AddWhereIn(GenerWorkerListAttr.FK_Node, "(" + inStr + ")");

                    qo.DoQuery();
                    foreach (GenerWorkerList wl in wls)
                    {
                        Node subNd = new Node(wl.FK_Node);
                        Work subWK = subNd.GetWork(wl.WorkID);
                        subWK.Delete();

                        //删除分流下步骤的节点信息.
                        DeleteToNodesData(subNd.HisToNodes);
                    }

                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkFlow WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                    DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + dbStr + "WorkID",
                        "WorkID", this.WorkID);
                }
                DeleteToNodesData(nd.HisToNodes);
            }
        }
        private WorkNode DoReturnSubFlow(int backtoNodeID, string msg, bool isHiden)
        {
            Node nd = new Node(backtoNodeID);
            ps = new Paras();
            ps.SQL = "DELETE  FROM WF_GenerWorkerList WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID  AND FID=" + dbStr + "FID";
            ps.Add("FK_Node", backtoNodeID);
            ps.Add("WorkID", this.HisWork.OID);
            ps.Add("FID", this.HisWork.FID);
            BP.DA.DBAccess.RunSQL(ps);

            // 找出分合流点处理的人员.
            ps = new Paras();
            ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "FID";
            ps.Add("FID", this.HisWork.FID);
            ps.Add("FK_Node", backtoNodeID);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count != 1)
                throw new Exception("@ system error , this values must be =1");

            string FK_Emp = dt.Rows[0][0].ToString();
            // 获取当前工作的信息.
            GenerWorkerList wl = new GenerWorkerList(this.HisWork.FID, this.HisNode.NodeID, FK_Emp);
            Emp emp = new Emp(FK_Emp);

            // 改变部分属性让它适应新的数据,并显示一条新的待办工作让用户看到。
            wl.IsPass = false;
            wl.WorkID = this.HisWork.OID;
            wl.FID = this.HisWork.FID;
            wl.FK_Emp = FK_Emp;
            wl.FK_EmpText = emp.Name;

            wl.FK_Node = backtoNodeID;
            wl.FK_NodeText = nd.Name;
            // wl.WarningHour = nd.WarningHour;
            wl.FK_Dept = emp.FK_Dept;

            DateTime dtNew = DateTime.Now;
            // dtNew = dtNew.AddDays(nd.WarningHour);

            wl.SDT = dtNew.ToString(DataType.SysDataTimeFormat); // DataType.CurrentDataTime;
            wl.FK_Flow = this.HisNode.FK_Flow;
            wl.Insert();

            GenerWorkFlow gwf = new GenerWorkFlow(this.HisWork.OID);
            gwf.FK_Node = backtoNodeID;
            gwf.NodeName = nd.Name;
            gwf.DirectUpdate();

            ps = new Paras();
            ps.Add("FK_Node", backtoNodeID);
            ps.Add("WorkID", this.HisWork.OID);
            ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=3 WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID";
            BP.DA.DBAccess.RunSQL(ps);

            /* 如果是隐性退回。*/
            BP.WF.ReturnWork rw = new ReturnWork();
            rw.WorkID = wl.WorkID;
            rw.ReturnToNode = wl.FK_Node;
            rw.ReturnNode = this.HisNode.NodeID;
            rw.ReturnNodeName = this.HisNode.Name;
            rw.ReturnToEmp = FK_Emp;
            rw.BeiZhu = msg;
            try
            {
                rw.MyPK = rw.ReturnToNode + "_" + rw.WorkID + "_" + DateTime.Now.ToString("yyyyMMddhhmmss");
                rw.Insert();
            }
            catch
            {
                rw.MyPK = rw.ReturnToNode + "_" + rw.WorkID + "_" + BP.DA.DBAccess.GenerOID();
                rw.Insert();
            }

            // 加入track.
            this.AddToTrack(ActionType.Return, FK_Emp, emp.Name, backtoNodeID, nd.Name, msg);

            WorkNode wn = new WorkNode(this.HisWork.FID, backtoNodeID);
            if (Glo.IsEnableSysMessage)
            {
                //  WF.Port.WFEmp wfemp = new Port.WFEmp(wn.HisWork.Rec);
                string title = string.Format("工作退回：流程:{0}.工作:{1},退回人:{2},需您处理",
                      wn.HisNode.FlowName, wn.HisNode.Name, WebUser.Name);

                BP.WF.Dev2Interface.Port_SendMsg(wn.HisWork.Rec, title, msg,
                    "RESub" + backtoNodeID + "_" + this.WorkID, BP.WF.SMSMsgType.SendSuccess, nd.FK_Flow, nd.NodeID, this.WorkID, this.FID);
            }
            return wn;
        }
    }
}
