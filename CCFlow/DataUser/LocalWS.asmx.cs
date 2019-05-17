using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Services;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;

namespace CCFlow.DataUser
{
    /// <summary>
    /// LocalWS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class LocalWS : System.Web.Services.WebService
    {
        /// <summary>
        /// 获得工作进度-用于展示流程的进度图
        /// </summary>
        /// <param name="workID">workID</param>
        /// <returns>返回进度数据</returns>
        public string DB_JobSchedule(Int64 workID)
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
            GenerWorkFlow gwf=new GenerWorkFlow(workID);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));


            /*节点信息: 节点信息表,存储每个环节的节点信息数据.
             * NodeID 节点ID.
             * Name 名称.
             * X,Y 节点图形位置，如果使用进度图就不需要了.
            */
            Nodes nds = new Nodes(gwf.FK_Flow);
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
            dtHistory.Columns.Add("EmpNo");  //人员编号.
            dtHistory.Columns.Add("EmpName"); //名称
            dtHistory.Columns.Add("RDT"); //记录日期.
            dtHistory.Columns.Add("SDT"); //应完成日期(可以不用.)

            //执行人.
            if (gwf.WFState == WFState.Complete)
            {
                //历史执行人. 
                sql = "SELECT * FROM ND" + int.Parse(gwf.FK_Flow) + "Track WHERE WorkID=" + workID + " AND (ActionType=1 OR ActionType=0)  ORDER BY RDT DESC";
                DataTable dtTrack = BP.DA.DBAccess.RunSQLReturnTable(sql);

                foreach (DataRow drTrack in dtTrack.Rows)
                {
                    DataRow dr = dtHistory.NewRow();
                    dr["FK_Node"] = drTrack["NDFrom"];
                   // dr["ActionType"] = drTrack["NDFrom"];
                    dr["NodeName"] = drTrack["NDFromT"];
                    dr["EmpNo"] = drTrack["EmpFrom"];
                    dr["EmpName"] = drTrack["EmpFromT"];
                    dr["RDT"] = drTrack["RDT"];
                    dr["SDT"] = drTrack[""];
                    dtHistory.Rows.Add(dr);
                }
            }
            else
            {
                GenerWorkerLists gwls = new GenerWorkerLists(workID);
                foreach (GenerWorkerList gwl in gwls)
                {
                    DataRow dr = dtHistory.NewRow();
                    dr["FK_Node"] = gwl.FK_Node;
                    dr["NodeName"] = gwl.FK_NodeText;
                    dr["EmpNo"] = gwl.FK_Emp;
                    dr["EmpName"] = gwl.FK_EmpText;
                    dr["RDT"] = gwl.RDT;
                    dr["SDT"] = gwl.SDT;
                    dtHistory.Rows.Add(dr);
                }
            }
            ds.Tables.Add(dtHistory);
            #endregion 运动轨迹

            return BP.Tools.Json.ToJson(ds);
        }
       
        /// <summary>
        /// 获得待办
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sysNo">系统编号,为空时返回平台所有数据。</param>
        /// <returns>返回待办</returns>
        public string DB_Todolist(string userNo, string sysNo = null)
        {
            string sql = "";
            if (userNo == null)
                sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='" + userNo + "'";
            else
                sql = "SELECT * FROM WF_EmpWorks WHERE Domain='" + sysNo + "' AND FK_Emp='" + userNo + "'";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得在途
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sysNo">系统编号，为空时返回平台所有数据。</param>
        /// <returns></returns>
        public string DB_Runing(string userNo, string sysNo = null)
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerRuning(userNo, null, false, sysNo);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得我可以发起的流程.
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sysNo">系统编号，为空时返回平台所有数据。</param>
        /// <returns>返回我可以发起的流程列表.</returns>
        public string DB_StarFlows(string userNo, string sysNo = null)
        {
            DataTable dt= BP.WF.Dev2Interface.DB_StarFlows(userNo);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 我发起的流程实例
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sysNo">子系统编号</param>
        /// <returns>我发起的流程列表.</returns>
        public string DB_MyStartFlowInstance(string userNo, string sysNo = null, int pageSize=0, int pageIdx=0)
        {
            string sql = "";
            if (userNo == null)
                sql = "SELECT * FROM WF_GenerWorkFlow WHERE Starter='" + userNo + "'";
            else
                sql = "SELECT * FROM WF_GenerWorkFlow WHERE Domain='" + sysNo + "' AND Starter='" + userNo + "'";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">工作人员编号</param>
        /// <returns>一个长整型的工作流程实例.</returns>
        [WebMethod]
        public Int64 CreateWorkID(string flowNo, string userNo)
        {
            return BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo, userNo);
        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="flowNo">流的程模版ID</param>
        /// <param name="workid">工作ID</param>
        /// <param name="atParas">参数: @Field1=Val1@Field2=Val2</param>
        /// <param name="toNodeID">到达的节点ID.如果让系统自动计算就传入0</param>
        /// <param name="toEmps">到达的人员IDs,比如:zhangsan,lisi,wangwu. 如果为Null就标识让系统自动计算.</param>
        /// <returns>发送的结果信息.</returns>
        [WebMethod]
        public string SendWork(string flowNo, Int64 workid, string atParas, int toNodeID, string toEmps)
        {
            BP.DA.AtPara ap = new BP.DA.AtPara(atParas);

            BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ap.HisHT, toNodeID, toEmps);

            string msg = objs.ToMsgOfText();

            Hashtable myht = new Hashtable();
            myht.Add("Message", msg);
            myht.Add("IsStopFlow", objs.IsStopFlow);
            myht.Add("VarAcceptersID", objs.VarAcceptersID);
            myht.Add("VarAcceptersName", objs.VarAcceptersName);
            myht.Add("VarToNodeID", objs.VarToNodeID);
            myht.Add("VarToNodeName", objs.VarToNodeName);

            return BP.Tools.Json.ToJson(myht);
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="paras">用于控制流程运转的参数，比如方向条件. 格式为:@JinE=1000@QingJaiTianShu=100</param>
        [WebMethod]
        public void SaveParas(Int64 workid, string paras)
        {
            BP.WF.Dev2Interface.Flow_SaveParas(workid, paras);
        }
        /// <summary>
        /// 获得下一个节点信息
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">流程实例</param>
        /// <param name="paras">方向条件所需要的参数，可以为空。</param>
        /// <returns>下一个节点的JSON.</returns>
        [WebMethod]
        public string GenerNextStepNode(string flowNo, Int64 workid, string paras = null)
        {
            if (paras != null)
                BP.WF.Dev2Interface.Flow_SaveParas(workid, paras);

            int nodeID = BP.WF.Dev2Interface.Node_GetNextStepNode(flowNo, workid);
            BP.WF.Node nd = new BP.WF.Node(nodeID);

            //如果字段 DeliveryWay = 4 就表示到达的接点是由当前节点发送人选择接收人.
            //自定义参数的字段是 SelfParas, DeliveryWay 
            // CondModel = 方向条件计算规则.
            return nd.ToJson();
        }
        /// <summary>
        /// 获得下一步节点的接收人
        /// </summary>
        /// <param name="toNodeID">节点ID</param>
        /// <param name="workid">工作事例ID</param>
        /// <returns>返回两个结果集一个是分组的Depts(No,Name)，另外一个是人员的Emps(No, Name, FK_Dept),接受后，用于构造人员选择器.</returns>
        [WebMethod]
        public string GenerNextStepNodeEmps(string flowNo, int toNodeID, int workid)
        {
            Selector select = new Selector(toNodeID);
            Node nd = new Node(toNodeID);

            GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt", workid);
            DataSet ds = select.GenerDataSet(toNodeID, rpt);
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 将要达到的节点
        /// </summary>
        /// <param name="currNodeID">当前节点ID</param>
        /// <returns>返回节点集合的json.</returns>
        [WebMethod]
        public string WillToNodes(int currNodeID)
        {
            Node nd = new Node(currNodeID);
            if (nd.CondModel != CondModel.SendButtonSileSelect)
                return "err@";

            Directions dirs = new Directions();
            Nodes nds = dirs.GetHisToNodes(currNodeID, false);
            return nds.ToJson();
        }
        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <param name="returnToNodeID">退回到nodeID</param>
        /// <param name="returnMsg">退回信息</param>
        /// <returns></returns>
        public string Node_ReturnWork(Int64 workid, int returnToNodeID, string returnMsg)
        {
            GenerWorkFlow gwf=new GenerWorkFlow(workid);
            return BP.WF.Dev2Interface.Node_ReturnWork(gwf.FK_Flow, gwf.WorkID, gwf.FID, gwf.FK_Node, returnToNodeID, returnMsg);
        }
        /// <summary>
        /// 获得当前节点信息.
        /// </summary>
        /// <param name="currNodeID">节点ID.</param>
        /// <returns>当前节点信息</returns>
        public string CurrNodeInfo(int currNodeID)
        {
            Node nd = new Node(currNodeID);
            return nd.ToJson();
        }
        /// <summary>
        /// 获得当前流程信息.
        /// </summary>
        /// <param name="flowNo">流程ID.</param>
        /// <returns>当前节点信息</returns>
        public string CurrFlowInfo(string flowNo)
        {
            Flow fl = new Flow(flowNo);
            return fl.ToJson();
        }
        /// <summary>
        /// 获得当前流程信息.
        /// </summary>
        /// <param name="flowNo">流程ID.</param>
        /// <returns>当前节点信息</returns>
        public string CurrGenerWorkFlowInfo(Int64 workID)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            return gwf.ToJson();
        }


    }
}
