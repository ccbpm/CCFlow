using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web.SessionState;
using BP.DA;
using BP.Web;
using BP.BPMN;
using BP.Sys;
using BP.En;
using BP.WF.Template;
using LitJson;
using System.Collections.Generic;

namespace CCFlow.WF.Admin.CCBPMDesigner.common
{
    /// <summary>
    /// controller 的摘要说明
    /// </summary>
    public class controller : IHttpHandler, IRequiresSessionState
    {
        #region 全局变量IRequiresSessionState
        /// <summary>
        /// 流程编号
        /// </summary>
        private string FK_Flow
        {
            get
            {
                return getUTF8ToString("FK_Flow");
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        private int FK_Node
        {
            get
            {
                string fk_node = getUTF8ToString("FK_Node");
                if (!string.IsNullOrEmpty(fk_node)) return int.Parse(fk_node);
                return 0;
            }
        }
        /// <summary>
        /// http请求
        /// </summary>
        public HttpContext _Context
        {
            get;
            set;
        }

        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(_Context.Request[param], System.Text.Encoding.UTF8);
        }
        #endregion

        public void ProcessRequest(HttpContext context)
        {
            _Context = context;

            if (_Context == null) return;

            string action = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!string.IsNullOrEmpty(context.Request["action"]))
                action = context.Request["action"].ToString();

            switch (action)
            {
                case "load"://获取流程图表数据
                    s_responsetext = Flow_LoadFlowJsonData();
                    break;
                case "save"://保存流程图
                    s_responsetext = Flow_Save();
                    break;
                case "saveAs"://另存为流程
                    s_responsetext = Flow_SaveAs();
                    break;
                case "genernodeid"://创建节点获取节点编号
                    s_responsetext = Node_Create_GenerNodeID();
                    break;
                case "deletenode"://删除流程节点
                    s_responsetext = Node_DeleteNodeOfNodeID();
                    break;
                case "editnodename"://修改节点名称
                    s_responsetext = Node_EditNodeName();
                    break;
                case "ccbpm_flow_elements"://流程所有元素集合
                    s_responsetext = Flow_AllElements_ResponseJson();
                    break;
                case "changenoderunmodel"://修改节点运行模式
                    s_responsetext = Node_ChangeRunModel();
                    break;
                case "ccbpm_flow_resetversion"://重置流程版本为1.0
                    s_responsetext = Flow_ResetFlowVersion();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            context.Response.Charset = "UTF-8";
            context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            context.Response.ContentType = "text/html";
            context.Response.Expires = 0;
            context.Response.Write(s_responsetext);
            context.Response.End();
        }

        #region 流程相关 Flow
        /// <summary>
        /// 加载流程图数据 
        /// </summary>
        /// <returns></returns>
        private string Flow_LoadFlowJsonData()
        {
            string diagramId = getUTF8ToString("diagramId");
            Flow fl = new Flow(diagramId);
            return fl.FlowJson;
        }
        /// <summary>
        /// 保存流程图信息
        /// </summary>
        /// <returns></returns>
        private string Flow_Save()
        {
            //流程格式.
            string diagram = getUTF8ToString("diagram");
            //流程图.
            string png = getUTF8ToString("png");
            // 流程编号.
            string flowNo = getUTF8ToString("diagramId");
            //节点到节点关系
            string direction = getUTF8ToString("direction");
            //直接保存流程图信息
            Flow fl = new Flow(flowNo);
            //修改版本
            fl.DType = fl.DType == CCBPM_DType.BPMN ? CCBPM_DType.BPMN : CCBPM_DType.CCBPM;
            fl.Update();
            //直接保存了.
            fl.FlowJson = diagram; 

            //节点方向
            string[] dir_Nodes = direction.Split('@');
            Direction drToNode = new Direction();
            drToNode.Delete(DirectionAttr.FK_Flow, flowNo);
            foreach (string item in dir_Nodes)
            {
                if (string.IsNullOrEmpty(item)) continue;
                string[] nodes = item.Split(':');
                if (nodes.Length == 2)
                {
                    drToNode = new Direction();
                    drToNode.FK_Flow = flowNo;
                    drToNode.Node = int.Parse(nodes[0]);
                    drToNode.ToNode = int.Parse(nodes[1]);
                    drToNode.Insert();
                }
            }

            #region //保存节点坐标及标签
            try
            {
                //清空标签
                LabNote labelNode = new LabNote();
                labelNode.Delete();

                JsonData flowJsonData = JsonMapper.ToObject(diagram);
                if (flowJsonData.IsObject == true)
                {
                    JsonData flow_Nodes = flowJsonData["s"]["figures"];
                    for (int iNode = 0, jNode = flow_Nodes.Count; iNode < jNode; iNode++)
                    {
                        JsonData figure = flow_Nodes[iNode];
                        //不存在不进行处理，继续循环
                        if (figure == null || figure["CCBPM_Shape"] == null)
                            continue;
                        if (figure["CCBPM_Shape"].ToString() == "Node")
                        {
                            //节点坐标处理
                            BP.WF.Node node = new BP.WF.Node();
                            node.RetrieveByAttr(NodeAttr.NodeID, figure["CCBPM_OID"]);
                            if (!string.IsNullOrEmpty(node.Name) && figure["rotationCoords"].Count > 0)
                            {
                                JsonData rotationCoord = figure["rotationCoords"][0];
                                node.X = int.Parse(rotationCoord["x"].ToString());
                                node.Y = int.Parse(rotationCoord["y"].ToString());
                                node.DirectUpdate();
                            }
                        }
                        else if (figure["CCBPM_Shape"].ToString() == "Text")
                        {
                            //流程标签处理
                            JsonData primitives = figure["primitives"][0];
                            JsonData vector = primitives["vector"][0];
                            labelNode = new LabNote();
                            labelNode.FK_Flow = flowNo;
                            labelNode.Name = primitives["str"].ToString();
                            labelNode.X = int.Parse(vector["x"].ToString());
                            labelNode.Y = int.Parse(vector["y"].ToString());
                            labelNode.Insert();
                        }
                    }
                    return "true";
                }
            }
            catch (Exception ex)
            {
            }
            #endregion

            return "true";
        }
        /// <summary>
        /// 另存为流程图
        /// </summary>
        /// <returns></returns>
        private string Flow_SaveAs()
        {
            return "";
        }

        /// <summary>
        /// 重置流程版本为1.0
        /// </summary>
        /// <returns></returns>
        private string Flow_ResetFlowVersion()
        {
            DBAccess.RunSQL("UPDATE WF_FLOW SET DTYPE=0,FLOWJSON='' WHERE NO='" + this.FK_Flow + "'");
            return "true";
        }

        /// <summary>
        /// 获取流程所有元素
        /// </summary>
        /// <returns>json data</returns>
        private string Flow_AllElements_ResponseJson()
        {
            try
            {
                BP.WF.Flow flow = new BP.WF.Flow();
                flow.No = this.FK_Flow;
                flow.RetrieveFromDBSources();
                if (flow.DType == 0)
                {
                    //获取所有节点
                    string sqls = "SELECT NODEID,NAME,X,Y,RUNMODEL FROM WF_NODE WHERE FK_FLOW='" + this.FK_Flow + "';" + Environment.NewLine
                                + "SELECT NODE,TONODE FROM WF_DIRECTION WHERE FK_FLOW='" + this.FK_Flow + "';" + Environment.NewLine
                                + "SELECT MYPK,NAME,X,Y FROM WF_LABNOTE WHERE FK_FLOW='" + this.FK_Flow + "';";
                    DataSet ds = DBAccess.RunSQLReturnDataSet(sqls);
                    ds.Tables[0].TableName = "Nodes";
                    ds.Tables[1].TableName = "Direction";
                    ds.Tables[2].TableName = "LabNote";

                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, msg = "", data = Newtonsoft.Json.JsonConvert.SerializeObject(ds) });
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = "", data = new { } });
            }
            catch (Exception ex)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message, data = new { } });
            }
        }
        #endregion end Flow

        #region 节点相关 Nodes
        /// <summary>
        /// 创建流程节点并返回编号
        /// </summary>
        /// <returns></returns>
        private string Node_Create_GenerNodeID()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string figureName = getUTF8ToString("FigureName");
                string x = getUTF8ToString("x");
                string y = getUTF8ToString("y");
                int iX = 0;
                int iY = 0;
                if (!string.IsNullOrEmpty(x)) iX = int.Parse(x);
                if (!string.IsNullOrEmpty(y)) iY = int.Parse(y);

                int nodeId = BP.BPMN.Glo.NewNode(FK_Flow, iX, iY);
                BP.WF.Node node = new BP.WF.Node(nodeId);
                node.HisRunModel = Node_GetRunModelByFigureName(figureName);
                node.Update();
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = true, msg = "", data = new { NodeID = nodeId, text = node.Name } });
            }
            catch (Exception ex)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { success = false, msg = ex.Message, data = new { } });
            }
        }
        /// <summary>
        /// gen
        /// </summary>
        /// <param name="figureName"></param>
        /// <returns></returns>
        private BP.WF.RunModel Node_GetRunModelByFigureName(string figureName)
        {
            BP.WF.RunModel runModel = BP.WF.RunModel.Ordinary;
            switch (figureName)
            {
                case "NodeOrdinary":
                    runModel = BP.WF.RunModel.Ordinary;
                    break;
                case "NodeFL":
                    runModel = BP.WF.RunModel.FL;
                    break;
                case "NodeHL":
                    runModel = BP.WF.RunModel.HL;
                    break;
                case "NodeFHL":
                    runModel = BP.WF.RunModel.FHL;
                    break;
                case "NodeSubThread":
                    runModel = BP.WF.RunModel.SubThread;
                    break;
                default:
                    runModel = BP.WF.RunModel.Ordinary;
                    break;
            }
            return runModel;
        }
        /// <summary>
        /// 根据节点编号删除流程节点
        /// </summary>
        /// <returns>执行结果</returns>
        private string Node_DeleteNodeOfNodeID()
        {
            try
            {
                int delResult = 0;
                string FK_Node = getUTF8ToString("FK_Node");
                if (string.IsNullOrEmpty(FK_Node)) return "true";

                BP.WF.Node node = new BP.WF.Node(int.Parse(FK_Node));
                if (node.IsExits == false)
                    return "true";

                if (node.IsStartNode == true)
                {
                    return "开始节点不允许被删除。";
                }
                delResult = node.Delete();

                if (delResult > 0) return "true";

                return "Delete Error.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 修改节点名称
        /// </summary>
        /// <returns></returns>
        private string Node_EditNodeName()
        {
            string FK_Node = getUTF8ToString("NodeID");

            string NodeName = getUTF8ToString("NodeName");


            BP.WF.Node node = new BP.WF.Node();
            node.NodeID = int.Parse(FK_Node);
            int iResult = node.RetrieveFromDBSources();
            if (iResult > 0)
            {
                node.Name = NodeName;
                node.Update();
                return "true";
            }
            return "false";
        }

        /// <summary>
        /// 修改节点运行模式
        /// </summary>
        /// <returns></returns>
        private string Node_ChangeRunModel()
        {
            string runModel = getUTF8ToString("RunModel");
            BP.WF.Node node = new BP.WF.Node(this.FK_Node);
            //节点运行模式
            switch (runModel)
            {
                case "NodeOrdinary":
                    node.HisRunModel = BP.WF.RunModel.Ordinary;
                    break;
                case "NodeFL":
                    node.HisRunModel = BP.WF.RunModel.FL;
                    break;
                case "NodeHL":
                    node.HisRunModel = BP.WF.RunModel.HL;
                    break;
                case "NodeFHL":
                    node.HisRunModel = BP.WF.RunModel.FHL;
                    break;
                case "NodeSubThread":
                    node.HisRunModel = BP.WF.RunModel.SubThread;
                    break;
            }
            return node.Update().ToString();
        }
        #endregion end Node

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}