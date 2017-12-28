using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Services;

namespace CCFlow.SDKFlowDemo
{
    /// <summary>
    /// WindowsFormsApplicationDemo 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class WindowsFormsApplicationDemo : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 创建workid.
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public Int64 CreateBlankWork(string flowNo, string userNo)
        {
            if (BP.Web.WebUser.No != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo);

            return BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo);
        }
        /// <summary>
        /// 工作发送
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="flowNo"></param>
        /// <param name="workid"></param>
        /// <param name="toNodeID"></param>
        /// <param name="toEmps"></param>
        /// <param name="ht"></param>
        /// <returns></returns>
        public string Node_SendWork(string userNo, string flowNo, Int64 workid, int toNodeID, string toEmps, Hashtable ht)
        {
            if (BP.Web.WebUser.No != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo);

            return BP.WF.Dev2Interface.Node_SendWork(flowNo,workid,ht,toNodeID,toEmps).ToMsgOfText();
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="userNo">用户</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="currentNodeID">当前节点ID</param>
        /// <param name="returnToNodeID">退回到</param>
        /// <param name="msg">退回消息</param>
        /// <param name="isBackToThisNode">是否原路返回</param>
        /// <returns>执行结果</returns>
        public string Node_ReturnWork(string userNo, string flowNo, Int64 workid, int currentNodeID, int returnToNodeID, 
            string msg, bool isBackToThisNode = false)
        {
            if (BP.Web.WebUser.No != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo);

            return BP.WF.Dev2Interface.Node_ReturnWork(flowNo, workid, 0, currentNodeID, returnToNodeID, msg, isBackToThisNode);
        }
        /// <summary>
        /// 获得发起列表.
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public DataTable Start(string userNo)
        {
            return BP.WF.Dev2Interface.DB_StarFlows(userNo);
        }
        /// <summary>
        /// 获得待办
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public DataTable Todolist(string userNo)
        {
            return BP.WF.Dev2Interface.DB_Todolist(userNo);
        }

        /// <summary>
        /// 在途
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public DataTable Runing(string userNo)
        {
            return BP.WF.Dev2Interface.DB_GenerRuning(userNo);
        } 
    }
}
