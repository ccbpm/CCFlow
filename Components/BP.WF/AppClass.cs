using System;
using System.Collections.Generic;
using System.Data;

namespace BP.WF
{
    /// <summary>
    /// 第三方应用开发的应用,非标准应用.
    /// </summary>
    public class AppClass
    {
        #region 进度条.
        /// <summary>
        /// 进度条 - for 为中科曙光.
        /// 1. 处理进度条.
        /// </summary>
        /// <param name="workid"></param>
        /// <returns></returns>
        public static string JobSchedule(Int64 workid)
        {
            DataSet ds = BP.WF.Dev2Interface.DB_JobSchedule(workid);

            DataTable gwf = ds.Tables["WF_GenerWorkFlow"]; //工作记录.
            DataTable nodes = ds.Tables["WF_Node"]; //节点.
            DataTable dirs = ds.Tables["WF_Direction"]; //连接线.
            DataTable tracks = ds.Tables["Track"]; //历史记录.

            //状态,
            int wfState = int.Parse(gwf.Rows[0]["WFState"].ToString());
            int currNode = int.Parse(gwf.Rows[0]["FK_Node"].ToString());
            if (wfState == 3)
                return BP.Tools.Json.ToJson(tracks); //如果流程结束了，所有的数据都在tracks里面.


            //把以后的为未完成的节点放入到track里面.
            for (int i = 0; i < 100; i++)
            {

                #region 判断当前节点的类型.
                // 如果是 =0 就说明有分支，有分支就判断当前节点是否是分河流。
                RunModel model = 0;
                foreach (DataRow dr in nodes.Rows)
                {
                    if (int.Parse(dr["NodeID"].ToString()) == currNode)
                    {
                        model = (RunModel)int.Parse(dr["RunModel"].ToString());
                    }
                }

                // 分合流.
                if (model == RunModel.FL || model == RunModel.FHL)
                {

                    Node nd = new Node(currNode);
                    Nodes tonds = nd.HisToNodes;

                    foreach (Node tond in tonds)
                    {
                        DataRow mydr = tracks.NewRow();
                        mydr["NodeName"] = tond.Name;
                        mydr["FK_Node"] = tond.NodeID; // nd["NodeID"].ToString();
                        mydr["RunModel"] = (int)tond.HisRunModel;
                        tracks.Rows.Add(mydr);

                        //设置当前节点.
                      //  currNode = tond.HisToNodes[0].GetValIntByKey("NodeID");
                        currNode = tond.NodeID;
                    }
                }
                #endregion 判断当前节点的类型.


                int nextNode = GetNextNodeID(currNode, dirs, nodes);
                if (nextNode == 0)
                    break;

                foreach (DataRow nd in nodes.Rows)
                {
                    if (int.Parse(nd["NodeID"].ToString()) == nextNode)
                    {
                        DataRow mydr = tracks.NewRow();
                        mydr["NodeName"] = nd["Name"].ToString();
                        mydr["FK_Node"] = nd["NodeID"].ToString();
                        mydr["RunModel"] = nd["RunModel"].ToString();
                        tracks.Rows.Add(mydr);
                        break;
                    }
                }
                currNode = nextNode;
            }
            return BP.Tools.Json.ToJson(tracks);
        }
        //根据当前节点获得下一个节点.
        private static int GetNextNodeID(int nodeID, DataTable dirs, DataTable nds)
        {
            int toNodeID = 0;
            foreach (DataRow dir in dirs.Rows)
            {
                if (int.Parse(dir["Node"].ToString()) == nodeID)
                {
                    toNodeID = int.Parse(dir["ToNode"].ToString());
                    break;
                }
            }
            var toNodeID2 = 0;
            foreach (DataRow dir in dirs.Rows)
            {
                if (int.Parse(dir["Node"].ToString()) == nodeID)
                {
                    toNodeID2 = int.Parse(dir["ToNode"].ToString());
                }
            }

            //两次去的不一致，就有分支，有分支就reutrn 0 .
            if (toNodeID2 == toNodeID)
                return toNodeID;

         

            return 0;
        }
        #endregion 进度条.

        #region sdk表单装载的时候返回的数据.
        /// <summary>
        /// sdk表单加载的时候，要返回的数据.
        /// 1. 系统会处理一些业务,设置当前工作已经读取等等.
        /// 2. 会判断权限，当前人员是否可以打开当前的工作.
        /// 3. 增加了一些审核组件的数据信息.
        /// 4. WF_Node的 FWCSta 是审核组件的状态  0=禁用,1=启用,2=只读.
        /// </summary>
        /// <param name="workid">工作ID</param>
        /// <returns>初始化的sdk表单页面信息</returns>
        public static string SDK_Page_Init(Int64 workid)
        {
            try
            {
                GenerWorkFlow gwf = new GenerWorkFlow(workid);

                //加载接口.
                DataSet ds = new DataSet();
                ds = BP.WF.CCFlowAPI.GenerWorkNode(gwf.FK_Flow, gwf.FK_Node, gwf.WorkID,
                    gwf.FID, BP.Web.WebUser.No);

                //要保留的tables.
               // string tables = ",WF_GenerWorkFlow,WF_Node,AlertMsg,Track,";
                 
                //移除不要的数据.
                ds.Tables.Remove("Sys_MapData");
                ds.Tables.Remove("Sys_MapDtl");
                ds.Tables.Remove("Sys_Enum");
                ds.Tables.Remove("Sys_MapExt");
                ds.Tables.Remove("Sys_FrmLine");
                ds.Tables.Remove("Sys_FrmLink");
                ds.Tables.Remove("Sys_FrmBtn");
                ds.Tables.Remove("Sys_FrmLab");
                ds.Tables.Remove("Sys_FrmImg");
                ds.Tables.Remove("Sys_FrmRB");
                ds.Tables.Remove("Sys_FrmEle");
                ds.Tables.Remove("Sys_MapFrame");
                ds.Tables.Remove("Sys_FrmAttachment");
                ds.Tables.Remove("Sys_FrmImgAth");
                ds.Tables.Remove("Sys_FrmImgAthDB");
                ds.Tables.Remove("Sys_MapAttr");
                ds.Tables.Remove("Sys_GroupField");
                ds.Tables.Remove("WF_FrmNodeComponent");
                ds.Tables.Remove("MainTable");
                ds.Tables.Remove("UIBindKey");

                if (ds.Tables.Contains("BP.Port.Depts")==true)
                    ds.Tables.Remove("BP.Port.Depts");
                 

                //获得审核信息.

                //历史执行人. 
                string sql = "SELECT C.Name AS DeptName, A.* FROM ND" + int.Parse(gwf.FK_Flow) + "Track A, Port_Emp B, Port_Dept C WHERE A.WorkID=" + workid + " AND (A.ActionType="+(int)ActionType.WorkCheck+") AND (A.EmpFrom=B.No) AND (B.FK_Dept=C.No) ORDER BY A.RDT DESC";
                DataTable dtTrack = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dtTrack.TableName = "Track";
                ds.Tables.Add(dtTrack);

                return BP.Tools.Json.ToJson(ds);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        #endregion sdk表单装载的时候返回的数据.
    }

}
