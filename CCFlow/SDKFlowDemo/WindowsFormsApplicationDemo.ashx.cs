using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;

namespace CCFlow.SDKFlowDemo
{
    /// <summary>
    /// WindowsFormsApplicationDemo1 的摘要说明
    /// </summary>
    public class WindowsFormsApplicationDemo1 : IHttpHandler
    {
        #region 变量.
        public HttpContext context = null;
        public string DoType
        {
            get
            {
                return context.Request.QueryString["DoType"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return context.Request.QueryString["FK_Flow"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse( context.Request.QueryString["WorkID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(context.Request.QueryString["FK_Node"]);
            }
        }
        public string ToEmps
        {
            get
            {
                return context.Request.QueryString["ToEmps"];
            }
        }
        public int ToNodeID
        {
            get
            {
                return int.Parse(context.Request.QueryString["ToNodeID"]);
            }
        }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public int CurrentNodeID
        {
            get
            {
                return int.Parse(context.Request.QueryString["CurrentNodeID"]);
            }
        }

        public string Msg
        {
            get
            {
                return context.Request.QueryString["Msg"];
            }
        }
        public string UserNo
        {
            get
            {
                return context.Request.QueryString["UserNo"];
            }
        }
        #endregion 变量.

        public void ProcessRequest(HttpContext con)
        {
            context = con;

            if (BP.Web.WebUser.No != UserNo)
                BP.WF.Dev2Interface.Port_Login(UserNo);

            try
            {
                string info = "";
                switch (this.DoType)
                {
                    case "StartFlows": //获得发起列表. 形成菜单内容.
                        info = StartFlows(this.UserNo);
                        break;
                    case "Todolist": //代办列表. 形成菜单内容.
                        info = Todolist(this.UserNo);
                        break;
                    case "Runing": //在途列表. 形成菜单内容.
                        info = Runing(this.UserNo);
                        break;
                    case "CreateBlankWork": //创建工作ID.
                        info = CreateBlankWork(this.FK_Flow, this.UserNo).ToString();
                        break;
                    case "SendWork": //执行发送ID.
                        info = SendWork(this.FK_Flow, this.WorkID, this.ToNodeID, this.ToEmps, null).ToString();
                        break;
                    case "GenerWorkNode": //获得WorkNode. 用于初始化工作处理器信息.
                        info = GenerWorkNode();
                        break;
                    case "GenerWillReturnNodes": //获得可以退回的节点.
                        info = GenerWillReturnNodes();
                        break;
                    case "ReturnWork": //执行退回.
                        info = ReturnWork(this.UserNo, this.WorkID, this.CurrentNodeID,
                            this.ToNodeID, this.Msg, false).ToString();
                        break;
                    default:
                        info = "err@" + this.DoType;
                        break;
                }

                context.Response.ContentType = "text/plain";
                context.Response.Write(info);

               // BP.Tools.Json.ToDataTable(
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write("err@" + ex.Message);
            }
        }

        #region 工作处理.
        /// <summary>
        /// 创建workid.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="userNo">人员编号</param>
        /// <returns>该流程的一个空白的workid.</returns>
        public Int64 CreateBlankWork(string flowNo, string userNo)
        {
            if (BP.Web.WebUser.No != userNo)
                BP.WF.Dev2Interface.Port_Login(userNo);

            return BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo);
        }
        /// <summary>
        /// 工作发送
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="toNodeID">到达的节点,可以为0</param>
        /// <param name="toEmps">到达的人员，可以为null</param>
        /// <param name="ht">相关参数，可以为null</param>
        /// <returns>发送结果</returns>
        public string SendWork(string flowNo, Int64 workid, int toNodeID, string toEmps, Hashtable ht)
        {
            return BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ht, toNodeID, toEmps).ToMsgOfText();
        }
        /// <summary>
        /// 获得一个工作节点.
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            BP.WF.HttpHandler.WF_MyFlow myflow = new BP.WF.HttpHandler.WF_MyFlow(this.context);
            return myflow.GenerWorkNode();
        }
        #endregion 工作处理.

        #region 退回窗口。
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
        public string ReturnWork(string flowNo, Int64 workid, int currentNodeID, int returnToNodeID,
            string msg, bool isBackToThisNode = false)
        {
            return BP.WF.Dev2Interface.Node_ReturnWork(flowNo, workid, 0, currentNodeID, returnToNodeID, msg, isBackToThisNode);
        }
        /// <summary>
        /// 获得可以退回的节点
        /// </summary>
        /// <returns></returns>
        public string GenerWillReturnNodes()
        {
            DataTable dt= BP.WF.Dev2Interface.DB_GenerWillReturnNodes(this.FK_Node,this.WorkID,0);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 退回窗口。

        #region 三大菜单.
        /// <summary>
        /// 获得发起列表.
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public string StartFlows(string userNo)
        {
            DataTable dt= BP.WF.Dev2Interface.DB_StarFlows(userNo);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得待办
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public string Todolist(string userNo)
        {
            DataTable dt = BP.WF.Dev2Interface.DB_Todolist(userNo);
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 在途
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public string Runing(string userNo)
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerRuning(userNo);
            return BP.Tools.Json.ToJson(dt);
        }
        #endregion 三大菜单.

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}