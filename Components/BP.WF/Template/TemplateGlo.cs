using System;
using System.Collections.Generic;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using System.Data;
using BP.WF;
using BP.WF.Template;

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
        /// <returns>创建的流程编号</returns>
        public static string NewFlow(string flowSort, string flowName, BP.WF.Template.DataStoreModel dsm,
            string ptable, string flowMark)
        {
            //定义一个变量.
            Flow flow = new Flow();
            try
            {
                //检查参数的完整性.
                if (DataType.IsNullOrEmpty(ptable) == false && ptable.Length >= 1)
                {
                    string c = ptable.Substring(0, 1);
                    if (DataType.IsNumStr(c) == true)
                        throw new Exception("@非法的流程数据表(" + ptable + "),它会导致ccflow不能创建该表.");
                }

                flow.HisDataStoreModel = dsm;
                flow.PTable = ptable;
                flow.FK_FlowSort = flowSort;
                flow.FlowMark = flowMark;

                if (DataType.IsNullOrEmpty(flowMark) == false)
                {
                    if (flow.IsExit(FlowAttr.FlowMark, flowMark))
                        throw new Exception("@该流程标示:" + flowMark + "已经存在于系统中.");
                }

                /*给初始值*/
                //this.Paras = "@StartNodeX=10@StartNodeY=15@EndNodeX=40@EndNodeY=10";
                flow.Paras = "@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350";

                flow.No = flow.GenerNewNoByKey(FlowAttr.No);
                flow.Name = flowName;
                if (string.IsNullOrWhiteSpace(flow.Name))
                    flow.Name = "新建流程" + flow.No; //新建流程.

                if (flow.IsExits == true)
                    throw new Exception("err@系统出现自动生成的流程编号重复.");

                if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                    flow.OrgNo = WebUser.OrgNo; //隶属组织 
                flow.Insert();

                BP.WF.Node nd = new BP.WF.Node();
                nd.NodeID = int.Parse(flow.No + "01");
                nd.Name = "Start Node";//  "开始节点"; 
                nd.Step = 1;
                nd.FK_Flow = flow.No;
                nd.FlowName = flow.Name;
                nd.HisNodePosType = NodePosType.Start;
                nd.HisNodeWorkType = NodeWorkType.StartWork;
                nd.X = 200;
                nd.Y = 150;
                nd.NodePosType = NodePosType.Start;
                nd.ICON = "前台";

                //增加了两个默认值值 . 2016.11.15. 目的是让创建的节点，就可以使用.
                nd.CondModel = DirCondModel.SendButtonSileSelect; //默认的发送方向.
                nd.HisDeliveryWay = DeliveryWay.BySelected; //上一步发送人来选择.
                nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.

                //如果是集团模式.   
                if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
                {
                    if (DataType.IsNullOrEmpty(WebUser.OrgNo) == true)
                        throw new Exception("err@登录信息丢失了组织信息,请重新登录.");

                    nd.HisDeliveryWay = DeliveryWay.BySelectedOrgs;

                    //把本组织加入进去.
                    FlowOrg fo = new FlowOrg();
                    fo.Delete(FlowOrgAttr.FlowNo, nd.FK_Flow);
                    fo.FlowNo = nd.FK_Flow;
                    fo.OrgNo = WebUser.OrgNo;
                    fo.Insert();
                }

                nd.Insert();
                nd.CreateMap();

                //为开始节点增加一个删除按钮. @李国文.
                string sql = "UPDATE WF_Node SET DelEnable=1 WHERE NodeID=" + nd.NodeID;
                BP.DA.DBAccess.RunSQL(sql);

                //nd.HisWork.CheckPhysicsTable();  去掉，检查的时候会执行.
                flow.CreatePushMsg(nd);

                //通用的人员选择器.
                BP.WF.Template.Selector select = new Template.Selector(nd.NodeID);
                select.SelectorModel = SelectorModel.GenerUserSelecter;
                select.Update();

                nd = new BP.WF.Node();

                //为创建节点设置默认值 
                string fileNewNode = SystemConfig.PathOfDataUser + "\\XML\\DefaultNewNodeAttr.xml";
                if (System.IO.File.Exists(fileNewNode) == true)
                {
                    DataSet myds = new DataSet();
                    myds.ReadXml(fileNewNode);
                    DataTable dt = myds.Tables[0];
                    foreach (DataColumn dc in dt.Columns)
                    {
                        nd.SetValByKey(dc.ColumnName, dt.Rows[0][dc.ColumnName]);
                    }
                }
                else
                {
                    nd.HisNodePosType = NodePosType.Mid;
                    nd.HisNodeWorkType = NodeWorkType.Work;
                    nd.X = 200;
                    nd.Y = 250;
                    nd.ICON = "审核";
                    nd.NodePosType = NodePosType.End;

                    //增加了两个默认值值 . 2016.11.15. 目的是让创建的节点，就可以使用.
                    nd.CondModel = DirCondModel.SendButtonSileSelect; //默认的发送方向.
                    nd.HisDeliveryWay = DeliveryWay.BySelected; //上一步发送人来选择.
                    nd.FormType = NodeFormType.FoolForm; //设置为傻瓜表单.
                }

                nd.NodeID = int.Parse(flow.No + "02");
                nd.Name = "Node 2"; // "结束节点";
                nd.Step = 2;
                nd.FK_Flow = flow.No;
                nd.FlowName = flow.Name;

                nd.X = 200;
                nd.Y = 250;

                nd.Insert();
                nd.CreateMap();
                //nd.HisWork.CheckPhysicsTable(); //去掉，检查的时候会执行.
                flow.CreatePushMsg(nd);

                //通用的人员选择器.
                select = new Template.Selector(nd.NodeID);
                select.SelectorModel = SelectorModel.GenerUserSelecter;
                select.Update();

                BP.Sys.MapData md = new BP.Sys.MapData();
                md.No = "ND" + int.Parse(flow.No) + "Rpt";
                md.Name = flow.Name;
                md.Save();

                // 装载模版.
                string file = BP.Sys.SystemConfig.PathOfDataUser + "XML\\TempleteSheetOfStartNode.xml";
                if (System.IO.File.Exists(file) == false)
                    throw new Exception("@开始节点表单模版丢失" + file);

                /*如果存在开始节点表单模版*/
                DataSet ds = new DataSet();
                ds.ReadXml(file);

                string nodeID = "ND" + int.Parse(flow.No + "01");
                BP.Sys.MapData.ImpMapData(nodeID, ds);

                //创建track.
                Track.CreateOrRepairTrackTable(flow.No);


            }
            catch (Exception ex)
            {
                ///删除垃圾数据.
                flow.DoDelete();
                //提示错误.
                throw new Exception("err@创建流程错误:" + ex.Message);
            }
             

            FlowExt flowExt = new FlowExt(flow.No);
            flowExt.DesignerNo = BP.Web.WebUser.No;
            flowExt.DesignerName = BP.Web.WebUser.Name;
            flowExt.DesignTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            flowExt.DirectSave();

            //创建连线
            Direction drToNode = new Direction();
            drToNode.FK_Flow = flow.No;
            drToNode.Node = int.Parse(int.Parse(flow.No) + "01");
            drToNode.ToNode = int.Parse(int.Parse(flow.No) + "02");
            drToNode.Insert();

            //增加方向.
            Node mynd = new Node(drToNode.Node);
            mynd.HisToNDs = drToNode.ToNode.ToString();
            mynd.Update();


            //设置流程的默认值.
            foreach (string key in SystemConfig.AppSettings.AllKeys)
            {
                if (key.Contains("NewFlowDefVal") == false)
                    continue;

                string val = SystemConfig.AppSettings[key];

                //设置值.
                flow.SetValByKey(key.Replace("NewFlowDefVal_",""), val);
            }


            //执行一次流程检查, 为了节省效率，把检查去掉了.
            flow.DoCheck();
            
            return flow.No;
        }
        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <returns>新的节点ID</returns>
        public static Node NewNode(string flowNo, int x, int y, string icon = null)
        {
            BP.WF.Flow fl = new WF.Flow(flowNo);
            BP.WF.Node nd = fl.DoNewNode(x, y, icon);
            return nd;
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
