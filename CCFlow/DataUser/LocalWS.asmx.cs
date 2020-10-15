﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.Services;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.DA;

namespace CCFlow.DataUser
{
    /// <summary>
    /// LocalWS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    [System.Web.Script.Services.ScriptService]
    public class LocalWS : System.Web.Services.WebService
    {
        /// <summary>
        /// 获得工作进度-用于展示流程的进度图
        /// </summary>
        /// <param name="workID">workID</param>
        /// <returns>返回进度数据</returns>
        public string DB_JobSchedule(Int64 workID)
        {
            DataSet ds = BP.WF.Dev2Interface.DB_JobSchedule(workID);
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
            if (sysNo == null)
                sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='" + userNo + "'";
            else
                sql = "SELECT * FROM WF_EmpWorks WHERE Domain='" + sysNo + "' AND FK_Emp='" + userNo + "'";

            DataTable dt =DBAccess.RunSQLReturnTable(sql);
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
        public string DB_StarFlows(string userNo, string domain = null)
        {
            DataTable dt = BP.WF.Dev2Interface.DB_StarFlows(userNo);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 我发起的流程实例
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sysNo">子系统编号</param>
        /// <returns>我发起的流程列表.</returns>
        public string DB_MyStartFlowInstance(string userNo, string domain = null, int pageSize = 0, int pageIdx = 0)
        {
            string sql = "";
            if (domain == null)
                sql = "SELECT * FROM WF_GenerWorkFlow WHERE Starter='" + userNo + "'";
            else
                sql = "SELECT * FROM WF_GenerWorkFlow WHERE Domain='" + domain + "' AND Starter='" + userNo + "'";

            DataTable dt =DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 运行一个sql，返回一个json.
        /// </summary>
        /// <param name="sqlOfSelect">要运行的SQL,查询</param>
        /// <param name="password">密码,双方约定的密码</param>
        /// <returns>json</returns>
        [WebMethod]
        public string DB_RunSQLReturnJSON(string sqlOfSelect, string password)
        {
            // if ( password.Equals("xxxxxx") == false)
            //  return "err@密码错误";
            // DataTable dt =DBAccess.RunSQLReturnTable(sqlOfSelect);
            // return BP.Tools.Json.ToJson(dt);
            throw new Exception("err@请实现该方法,密码部分是双方约定的,不对外公开的.");
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
           AtPara ap = new AtPara(atParas);

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
            if (nd.CondModel != DirCondModel.SendButtonSileSelect)
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
        [WebMethod]
        public string Node_ReturnWork(Int64 workid, int returnToNodeID, string returnMsg)
        {
            GenerWorkFlow gwf=new GenerWorkFlow(workid);
            return BP.WF.Dev2Interface.Node_ReturnWork(gwf.FK_Flow, gwf.WorkID, gwf.FID, gwf.FK_Node, returnToNodeID, returnMsg);
        }
        /// <summary>
        /// 写入审核信息
        /// </summary>
        /// <param name="workid">workID</param>
        /// <param name="msg">审核信息</param>
        [WebMethod]
        public void Node_WriteWorkCheck(Int64 workid, string msg)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
              BP.WF.Dev2Interface.WriteTrackWorkCheck(gwf.FK_Flow, gwf.FK_Node, gwf.WorkID, gwf.FID, msg,"审核",null);
        }
        /// <summary>
        /// 是否可以查看该工作
        /// </summary>
        /// <param name="flowNo">流程No</param>
        /// <param name="workid">工作ID</param>
        /// <param name="userNo">人员ID</param>
        /// <returns>true,false</returns>
        [WebMethod]
        public bool Flow_IsCanView(string flowNo, Int64 workid, string userNo)
        {
            return BP.WF.Dev2Interface.Flow_IsCanViewTruck(flowNo, workid,userNo);
        }
        /// <summary>
        /// 是否可以处理当前工作.
        /// </summary>
        /// <param name="workid">当前工作ID</param>
        /// <param name="workid">处理人员ID</param>
        /// <returns>true,false</returns>
        [WebMethod]
        public bool Flow_IsCanDoCurrentWork(Int64 workid, string userNo)
        {
            return BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(workid, userNo);
        }
        /// <summary>
        /// 获得当前节点信息.
        /// </summary>
        /// <param name="currNodeID">节点ID.</param>
        /// <returns>当前节点信息</returns>
        [WebMethod]
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
        [WebMethod]
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
        [WebMethod]
        public string CurrGenerWorkFlowInfo(Int64 workID)
        {
            GenerWorkFlow gwf = new GenerWorkFlow(workID);
            return gwf.ToJson();
        }
        [WebMethod]
        //微信小程序授权接口
        public string WeChatLogin(string userID, string userName, string userHeadURL)
        {
            DataTable dt=new DataTable();
            //查询当前授权用户是否存在人员表中
            string sql = "SELECT * FROM Port_Emp WHERE openid='" + userID+"'";
            dt= DBAccess.RunSQLReturnTable(sql);
            //如果存在此微信用户的openid
            if (dt.Rows.Count > 0)
            {
                //将微信登录名转换为拼音
                string userNo = BP.DA.DataType.ParseStringToPinyin(userName);
                //调用登录接口
                BP.WF.Dev2Interface.Port_Login(userNo);
                return BP.Tools.Json.ToJson(dt);
            }
            else
            {
                //将新授权的微信用户插入到Port_Emp表里
                BP.WF.Port.Emp emp = new BP.WF.Port.Emp();
                //将微信登录名转换为拼音
                string userNo= BP.DA.DataType.ParseStringToPinyin(userName);
                emp.No = userNo;
                emp.Name = userName;
                emp.Pass = "123";
                emp.openID = userID;
                emp.avatarUrl = userHeadURL;
                emp.Insert();

                //调用登录接口
                BP.WF.Dev2Interface.Port_Login(userName);

                dt = DBAccess.RunSQLReturnTable(sql);
            }
            return BP.Tools.Json.ToJson(dt);
        }

    }
}
