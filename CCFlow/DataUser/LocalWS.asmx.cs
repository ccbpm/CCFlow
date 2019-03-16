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
        /// <param name="ht">参数，或者表单字段.</param>
        /// <param name="toNodeID">到达的节点ID.如果让系统自动计算就传入0</param>
        /// <param name="toEmps">到达的人员IDs,比如:zhangsan,lisi,wangwu. 如果为Null就标识让系统自动计算.</param>
        /// <returns>发送的结果信息.</returns>
        [WebMethod]
        public string SendWork(string flowNo, Int64 workid, Hashtable ht, int toNodeID, string toEmps)
        {
            BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid, ht, toNodeID, toEmps);
            return objs.ToMsgOfText();
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
            //
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
            Directions dirs = new Directions();
            Nodes nds = dirs.GetHisToNodes(currNodeID, false);
            return nds.ToJson();
        }

    }
}
