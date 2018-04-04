using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF.Template
{
    /// <summary>
    /// 
    /// </summary>
    public class TemplateGlo
    {
        /// <summary>
        /// 创建一个流程.
        /// </summary>
        /// <param name="flowSort">流程类别</param>
        /// <returns>string</returns>
        public static string NewFlow(string flowSort, string flowName, BP.WF.Template.DataStoreModel dsm, string ptable, string flowMark, string flowVer)
        {
            //执行保存.
            BP.WF.Flow fl = new BP.WF.Flow();

            fl.DType = CCBPM_DType.CCBPM;

            string flowNo = fl.DoNewFlow(flowSort, flowName, dsm, ptable, flowMark);
            fl.No = flowNo;
            fl.Retrieve();

            //如果为CCFlow模式则不进行写入Json串
            if (flowVer == "0")
                return flowNo;

            //确定模板
            string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "\\WF\\Data\\Templete\\NewFlow.json";

            //将流程模版保存到数据库里.
            fl.SaveFileToDB("FlowJson", tempFile);


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
        /// <param name="flowNo"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int NewNode(string flowNo, int x, int y)
        {
            BP.WF.Flow fl = new WF.Flow(flowNo);
            BP.WF.Node nd = fl.DoNewNode(x, y);
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
