using System;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;
using System.Text;
using System.IO;

namespace BP.Demo
{
    /// <summary>
    /// 使用url的方式作为api调用的接口.
    /// 1,为了解决webservice的限制.
    /// 2,面webservces部署.
    /// </summary>
    public class HandlerInterfaceAPI
    {
        #region 三大菜单 api.
        /// <summary>
        /// 发起流程列表
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable StartFlows(string userNo)
        {
            string str= ExecUrl("StartFlows", userNo);
            return BP.Tools.Json.ToDataTable(str);
        }
        /// <summary>
        /// 待办列表
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable Todolist(string userNo)
        {
            string str =ExecUrl("Todolist", userNo);
            return BP.Tools.Json.ToDataTable(str);
        }
        /// <summary>
        /// 运行中的列表
        /// </summary>
        /// <param name="userNo"></param>
        /// <returns></returns>
        public static DataTable Runing(string userNo)
        {
            string str= ExecUrl("Runing", userNo);
            return BP.Tools.Json.ToDataTable(str);
        }
        #endregion 三大菜单 api.

        #region 工作发送处理.
        /// <summary>
        /// 创建WorkID
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="flowNo">流程编号</param>
        /// <returns></returns>
        public static Int64 CreateBlankWork(string userNo, string flowNo)
        {
            string str= ExecUrl("CreateBlankWork", userNo, flowNo);
            return Int64.Parse(str);
        }
        /// <summary>
        /// 发送工作
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="toNodeID">到达节点id(可以为0)</param>
        /// <param name="toEmpNos">到达的人员(多个可以用逗号分开比如zhangsan,lisi, 可以为null)</param>
        /// <returns>执行的结果</returns>
        public static Int64 SendWork(string userNo, string flowNo, Int64 workid, int toNodeID, string toEmpNos)
        {
            string paras = "&WorkID=" + workid + "&ToNodeID=" + toNodeID;
            if (toEmpNos != null)
                paras += "&ToEmps=" + toEmpNos;

            string str = ExecUrl("SendWork", userNo, flowNo, paras);
            return Int64.Parse(str);
        }
        /// <summary>
        /// 获得当前节点信息.
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作ID</param>
        /// <param name="fk_node">当前节点ID</param>
        /// <returns></returns>
        public static DataSet GenerWorkNode(string userNo, string flowNo, Int64 workid, int fk_node)
        {
            string paras = "&WorkID=" + workid + "&FK_Node=" + fk_node;
            string str = ExecUrl("GenerWorkNode", userNo, flowNo, paras);

            return BP.Tools.Json.ToDataSet(str);
        }
        #endregion 工作处理.

        #region 工作退回.
        /// <summary>
        /// 获得可以退回的节点
        /// </summary>
        /// <param name="userNo">当前用户编号</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作id</param>
        /// <returns>返回No,Name两个列的数据，这是用于可以退回节点的集合.</returns>
        public static DataTable GenerWillReturnNodes(string userNo, string flowNo, Int64 workid)
        {
            string paras = "&WorkID="+workid;
            string str = ExecUrl("GenerWillReturnNodes", userNo,flowNo,paras);

            return BP.Tools.Json.ToDataTable(str);
        }
        /// <summary>
        /// 执行退回
        /// </summary>
        /// <param name="userNo">当前用户编号</param>
        /// <param name="flowNo">流程编号</param>
        /// <param name="workid">工作id</param>
        /// <param name="currentNodeID">当前节点ID</param>
        /// <param name="returnToNodeID">要退回的节点ID</param>
        /// <param name="msg">退回消息</param>
        /// <param name="isBackToThisNode">是否要原路返回？</param>
        /// <returns>退回信息</returns>
        public static string ReturnWork(string userNo, string flowNo, Int64 workid, int currentNodeID, int returnToNodeID, string msg, bool isBackToThisNode = false)
        {
            string paras = "&WorkID=" + workid + "&CurrentNodeID=" + currentNodeID + "&ToNodeID=" + returnToNodeID + "&Msg=" + msg;
            if (isBackToThisNode == false)
                paras += "&IsBackToThisNode=0";
            else
                paras += "&IsBackToThisNode=1";

            return ExecUrl("ReturnWork", userNo, flowNo, paras);
        }
        #endregion 工作退回.

        #region 辅助方法.
        public static string ExecUrl(string doType, string userNo, string flowNo = null, string paras = null)
        {
            string url = "http://localhost:18272/SDKFlowDemo/WindowsFormsApplicationDemo.ashx";
            url += "?DoType=" + doType;
            url += "&UserNo=" + userNo;

            if (flowNo != null)
                url += "&FK_Flow=" + flowNo;
            if (paras != null)
                url += paras;

            string str = ReadURLContext(url, 5000, Encoding.UTF8);

            if (str.IndexOf("err@") == 0)
                throw new Exception("系统错误:" + str);

            return str;
        }

        public static string ReadURLContext(string url, int timeOut, Encoding encode)
        {
            HttpWebRequest webRequest = null;
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "get";
                webRequest.Timeout = timeOut;
                string str = webRequest.Address.AbsoluteUri;
                str = str.Substring(0, str.LastIndexOf("/"));
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            //	因为它返回的实例类型是WebRequest而不是HttpWebRequest,因此记得要进行强制类型转换
            //  接下来建立一个HttpWebResponse以便接收服务器发送的信息，它是调用HttpWebRequest.GetResponse来获取的：
            HttpWebResponse webResponse;
            try
            {
                webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                    return "err@"+ex.Message;
            }

            //如果webResponse.StatusCode的值为HttpStatusCode.OK，表示成功，那你就可以接着读取接收到的内容了：
            // 获取接收到的流
            Stream stream = webResponse.GetResponseStream();
            System.IO.StreamReader streamReader = new StreamReader(stream, encode);
            string content = streamReader.ReadToEnd();
            webResponse.Close();
            return content;
        }
        #endregion 辅助方法.

    }
}
