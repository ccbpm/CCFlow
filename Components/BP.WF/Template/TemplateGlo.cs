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
            //修改类型为CCBPMN
            //fl.DType = string.IsNullOrEmpty(flowVer) ? 1 : Int32.Parse(flowVer);

            fl.DType = CCBPM_DType.CCBPM;

            string flowNo = fl.DoNewFlow(flowSort, flowName, dsm, ptable, flowMark);
            fl.No = flowNo;

            //如果为CCFlow模式则不进行写入Json串
            if (flowVer == "0") return flowNo;

            //确定模板
            string tempFile = BP.Sys.SystemConfig.PathOfWebApp + "\\WF\\Data\\Templete\\NewFlow.json";
            if (flowVer == "1")//CCBPM_DType.CCBPM
                tempFile = BP.Sys.SystemConfig.PathOfWebApp + "\\WF\\Data\\Templete\\ccbpm.json";
            //将流程模版保存到数据库里.
            fl.SaveFileToDB("FlowJson", tempFile);

            //替换流程模板中的默认节点编号
            Flow fl_BPMN = new Flow(flowNo);
            string flowJson = fl_BPMN.FlowJson;
            //开始节点
            flowJson = flowJson.Replace("@UserTask01@", int.Parse(flowNo) + "01");
            //第二节点
            flowJson = flowJson.Replace("@UserTask02@", int.Parse(flowNo) + "02");
            fl_BPMN.FlowJson = flowJson;
            //创建连线
            Direction drToNode = new Direction();
            drToNode.FK_Flow = flowNo;
            drToNode.Node = int.Parse(int.Parse(flowNo) + "01");
            drToNode.ToNode = int.Parse(int.Parse(flowNo) + "02");
            drToNode.Insert();

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
        /// <summary>
        /// 获得流程JSON数据.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns>json格式的数据</returns>
        public static string GenerFlowJsonFromDB(string flowNo)
        {
            Flow fl = new Flow(flowNo);
            return fl.FlowJson;
        }
        public static string GenerFlowJsonFromFile(string flowNo)
        {
            string tempFile = BP.Sys.SystemConfig.PathOfDataUser + "FlowDesc\\" + flowNo + ".json";
            if (System.IO.File.Exists(tempFile) == false)
            {
                string json = GenerFlowJsonFromDB(flowNo);
            }


            Flow fl = new Flow(flowNo);
            return fl.FlowJson;
        }
        /// <summary>
        /// 保存流程数据.
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="json">json格式的数据.</param>
        public static string SaveGraphData(string flowNo, string json)
        {
            // 保存到临时文件.
            string tempFile = BP.Sys.SystemConfig.PathOfTemp + "\\" + flowNo + ".json";
            BP.DA.DataType.WriteFile(tempFile, json);

            #region 第1步骤:进行必要的安全检查.
            #endregion 进行必要的安全检查.

            #region 第2步骤:进行相关的到ccbpm存储.
            #endregion 进行相关的到ccbpm存储.

            //执行保存.
            Flow fl = new Flow(flowNo);
            fl.SaveFileToDB("FlowJson", tempFile);
            return null;
        }


    }
}
