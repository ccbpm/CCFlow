using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程模版的操作
    /// </summary>
    public class TemplateGlo
    {
        /// <summary>
        /// 创建一个流程模版
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        /// <param name="flowName">名称</param>
        /// <param name="dsm">存储方式</param>
        /// <param name="ptable">物理量</param>
        /// <param name="flowMark">标记</param>
        /// <param name="flowVer">版本</param>
        /// <returns>创建的流程编号</returns>
        public static string NewFlow(string flowSort, string flowName, BP.WF.Template.DataStoreModel dsm, string ptable, string flowMark, string flowVer)
        {
            //执行保存.
            BP.WF.Flow fl = new BP.WF.Flow();

            string flowNo = fl.DoNewFlow(flowSort, flowName, dsm, ptable, flowMark);
            fl.No = flowNo;
            fl.Retrieve();

           FlowExt flowExt = new FlowExt(flowNo);
           flowExt.DesignerNo = BP.Web.WebUser.No;
           flowExt.DesignerName = BP.Web.WebUser.Name;
           flowExt.DesignTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
           flowExt.DirectSave();


            //如果为CCFlow模式则不进行写入Json串
            if (flowVer == "0")
                return flowNo;

            //创建连线
            Direction drToNode = new Direction();
            drToNode.FK_Flow = flowNo;
            drToNode.Node = int.Parse(int.Parse(flowNo) + "01");
            drToNode.ToNode = int.Parse(int.Parse(flowNo) + "02");
            drToNode.Insert();

            //执行一次流程检查, 为了节省效率，把检查去掉了.
            fl.DoCheck();

            return flowNo;
        }
        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <returns>新的节点ID</returns>
        public static int NewNode(string flowNo, int x, int y,string icon=null)
        {
            BP.WF.Flow fl = new WF.Flow(flowNo);
            BP.WF.Node nd = fl.DoNewNode(x, y, icon);
            return nd.NodeID;
        }
        /// <summary>
        /// 删除节点.
        /// </summary>
        /// <param name="nodeid"></param>
        public static void DeleteNode(int nodeid)
        {
            BP.WF.Node nd = new WF.Node(nodeid);
            nd.Delete();
        }
    }
}
