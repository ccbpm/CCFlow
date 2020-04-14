﻿using System;
using System.Collections.Generic;
using BP.DA;
using System.Data;
using BP.En;
using BP.WF.Template;
using BP.Web;
using BP.Port;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 计算未来处理人
    /// </summary>
    public class FullSA
    {
        /// <summary>
        /// 工作Node.
        /// </summary>
        public WorkNode HisCurrWorkNode = null;
        /// <summary>
        /// 自动计算未来处理人（该方法在发送成功后执行.）
        /// </summary>
        /// <param name="CurrWorkNode">当前的节点</param>
        /// <param name="nd"></param>
        /// <param name="toND"></param>
        public FullSA(WorkNode currWorkNode)
        {
            //如果当前不需要计算未来处理人.
            if (currWorkNode.HisFlow.IsFullSA == false
                && currWorkNode.IsSkip == false)
                return;

            //如果到达最后一个节点，就不处理了。
            if (currWorkNode.HisNode.IsEndNode)
                return;

            //初始化一些变量.
            this.HisCurrWorkNode = currWorkNode;
            Node currND = currWorkNode.HisNode;
            Int64 workid = currWorkNode.HisWork.OID;

            //查询出来所有的节点.
            Nodes nds =currWorkNode.HisFlow.HisNodes;

            // 开始节点需要特殊处理》
            /* 如果启用了要计算未来的处理人 */
            SelectAccper sa = new SelectAccper();

            //首先要清除以前的计算，重新计算。
            sa.Delete(SelectAccperAttr.WorkID, workid);

            //求出已经路过的节点.
            DataTable dt = DBAccess.RunSQLReturnTable("SELECT FK_Node FROM WF_GenerWorkerList WHERE WorkID=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "WorkID", "WorkID", workid);
            string passedNodeIDs = "";
            foreach (DataRow item in dt.Rows)
            {
                passedNodeIDs += item[0].ToString() + ",";
            }

            //遍历当前的节点。
            foreach (Node item in nds)
            {
                if (item.IsStartNode == true)
                    continue;

                //如果已经包含了，就说明该节点已经经过了，就不处理了。
                if (passedNodeIDs.Contains(item.NodeID + ",") == true)
                    continue;

                //如果按照岗位计算（默认的第一个规则.）
                if (item.HisDeliveryWay == DeliveryWay.ByStation)
                {
                    // string sql = "SELECT No, Name FROM Port_Emp WHERE No IN (SELECT A.FK_Emp FROM " + BP.WF.Glo.EmpStation + " A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + item.NodeID + ")";

                    string sql = "SELECT a.No, a.Name FROM Port_Emp A, Port_DeptEmpStation B, WF_NodeStation C "; // WHERE No IN (SELECT A.FK_Emp FROM " + BP.WF.Glo.EmpStation + " A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + item.NodeID + ")";
                    sql += " WHERE A.No=B.FK_Emp AND B.FK_Station=C.FK_Station AND C.FK_Node="+item.NodeID;

                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count ==0)
                        continue;
                    foreach(DataRow dr in dt.Rows){
                        string no = dr[0].ToString();
                        string name = dr[1].ToString();
                        sa = new SelectAccper();
                        sa.FK_Emp = no;
                        sa.EmpName = name;
                        sa.FK_Node = item.NodeID;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);

                        sa.Insert();
                    }
                    continue;
                }

                //按照绑定的部门计算
                if (item.HisDeliveryWay == DeliveryWay.ByDept)
                {
                    string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                    Paras ps = new Paras();
                    ps.Add("FK_Node", item.NodeID);
                    ps.Add("WorkID", currWorkNode.HisWork.OID);
                    ps.SQL = "SELECT FK_Emp FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND AccType=0 ORDER BY IDX";
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        if (item.HisFlow.HisFlowAppType == FlowAppType.Normal)
                        {
                            ps = new Paras();
                            ps.SQL = "SELECT  A.No, A.Name  FROM Port_Emp A, WF_NodeDept B, Port_DeptEmp C  WHERE  A.No = C.FK_Emp AND C.FK_Dept=B.FK_Dept AND B.FK_Node=" + dbStr + "FK_Node";
                             

                            ps.Add("FK_Node", item.NodeID);
                            dt = DBAccess.RunSQLReturnTable(ps);
                            if (dt.Rows.Count == 0)
                                continue;
                            foreach (DataRow dr in dt.Rows)
                            {
                                string no = dr[0].ToString();
                                string name = dr[1].ToString();
                                sa = new SelectAccper();
                                sa.FK_Emp = no;
                                sa.EmpName = name;
                                sa.FK_Node = item.NodeID;

                                sa.WorkID = workid;
                                sa.Info = "无";
                                sa.AccType = 0;
                                sa.ResetPK();
                                if (sa.IsExits)
                                    continue;

                                //计算接受任务时间与应该完成任务时间.
                                InitDT(sa, item);

                                sa.Insert();
                            }
                        }
                        continue;
                    }
                    continue;
                }

                #region 仅按组织计算  @lizhen
                if (item.HisDeliveryWay == DeliveryWay.ByGroupOnly)
                {
                    string sql = "SELECT c.No,c.Name FROM GPM_GroupEmp A, WF_NodeGroup B, Port_Emp C WHERE A.FK_Emp=C.No AND A.FK_Group=B.FK_Group AND B.FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node ORDER BY A.FK_Emp";
                    Paras ps = new Paras();
                    ps.Add("FK_Node", item.NodeID);
                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        throw new Exception("err@节点绑定的仅按照 用户组 计算，没有找到人员:" + item.Name + " SQL=" + ps.SQLNoPara);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string no = dr[0].ToString();
                        string name = dr[1].ToString();
                        sa = new SelectAccper();
                        sa.FK_Emp = no;
                        sa.EmpName = name;
                        sa.FK_Node = item.NodeID;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);
                        sa.Insert();
                    }
                }
                #endregion

                #region 仅按组织智能计算  @lizhen
                if (item.HisDeliveryWay == DeliveryWay.ByGroup)
                {
                    string sql = "SELECT c.No,c.Name FROM GPM_GroupEmp A, WF_NodeGroup B, Port_Emp C WHERE A.FK_Emp=C.No AND A.FK_Group=B.FK_Group AND B.FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND C.OrgNo=" + SystemConfig.AppCenterDBVarStr + "OrgNo ORDER BY A.FK_Emp";
                    Paras ps = new Paras();
                    ps.Add("FK_Node", item.NodeID);
                    ps.Add("OrgNo", BP.Web.WebUser.OrgNo);

                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                        throw new Exception("err@节点绑定的仅按照 用户组 智能计算，没有找到人员:" + item.Name + " SQL=" + ps.SQLNoPara);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string no = dr[0].ToString();
                        string name = dr[1].ToString();
                        sa = new SelectAccper();
                        sa.FK_Emp = no;
                        sa.EmpName = name;
                        sa.FK_Node = item.NodeID;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);
                        sa.Insert();
                    }
                }
                #endregion

                #region 2019-09-25 byzhoupeng, 仅按岗位计算 
                if (item.HisDeliveryWay == DeliveryWay.ByStationOnly)
                {
                   string sql = "SELECT c.No,c.Name FROM Port_DeptEmpStation A, WF_NodeStation B, Port_Emp C WHERE A.FK_Emp=C.No AND A.FK_Station=B.FK_Station AND B.FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node ORDER BY A.FK_Emp";
                    Paras ps = new Paras();
                    ps.Add("FK_Node", item.NodeID);
                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)                    
                        throw new Exception("err@节点绑定的仅按照岗位计算，没有找到人员:"+item.Name +" SQL="+ps.SQLNoPara);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string no = dr[0].ToString();                         
                        string name = dr[1].ToString();
                        sa = new SelectAccper();
                        sa.FK_Emp = no;
                        sa.EmpName = name;
                        sa.FK_Node = item.NodeID;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);
                        sa.Insert();
                    }

                }
                #endregion

                //处理与指定节点相同的人员.
                if (item.HisDeliveryWay == DeliveryWay.BySpecNodeEmp
                   && item.DeliveryParas == currND.NodeID.ToString())
                {

                    sa.FK_Emp = WebUser.No;
                    sa.FK_Node = item.NodeID;

                    sa.WorkID = workid;
                    sa.Info = "无";
                    sa.AccType = 0;
                    sa.EmpName = WebUser.Name;

                    sa.ResetPK();
                    if (sa.IsExits)
                        continue;

                    //计算接受任务时间与应该完成任务时间.
                    InitDT(sa, item);

                    sa.Insert();
                    continue;
                }

                //处理绑定的节点人员..
                if (item.HisDeliveryWay == DeliveryWay.ByBindEmp)
                {
                    NodeEmps nes = new NodeEmps();
                    nes.Retrieve(NodeEmpAttr.FK_Node, item.NodeID);
                    foreach (NodeEmp ne in nes)
                    {
                        sa.FK_Emp = ne.FK_Emp;
                        sa.FK_Node = item.NodeID;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.EmpName = ne.FK_EmpT;

                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);

                        sa.Insert();
                    }
                }

                //按照节点的 岗位与部门的交集计算.
                #region 按部门与岗位的交集计算.
                if (item.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
                {
                    string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                    string sql = string.Empty;

                    //added by liuxc,2015.6.30.
                    //区别集成与BPM模式
                    
                        sql = "SELECT pdes.FK_Emp AS No"
                              + " FROM   Port_DeptEmpStation pdes"
                              + "        INNER JOIN WF_NodeDept wnd"
                              + "             ON  wnd.FK_Dept = pdes.FK_Dept"
                              + "             AND wnd.FK_Node = " + item.NodeID
                              + "        INNER JOIN WF_NodeStation wns"
                              + "             ON  wns.FK_Station = pdes.FK_Station"
                              + "             AND wnd.FK_Node =" + item.NodeID
                              + " ORDER BY"
                              + "        pdes.FK_Emp";

                        dt = DBAccess.RunSQLReturnTable(sql);
                     

                    foreach (DataRow dr in dt.Rows)
                    {
                        Emp emp = new Emp(dr[0].ToString());
                        sa.FK_Emp = emp.No;
                        sa.FK_Node = item.NodeID;
                        sa.DeptName = emp.FK_DeptText;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.EmpName = emp.Name;

                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);

                        sa.Insert();
                    }
                }
                #endregion 按部门与岗位的交集计算.
            }

            //预制当前节点到达节点的数据。
            Nodes toNDs = currND.HisToNodes;
            foreach (Node item in toNDs)
            {
                if (item.HisDeliveryWay == DeliveryWay.ByStation
                    || item.HisDeliveryWay == DeliveryWay.FindSpecDeptEmpsInStationlist)
                {
                    /*如果按照岗位访问*/
                    #region 最后判断 - 按照岗位来执行。
                    string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                    string sql = "";
                    Paras ps = new Paras();
                    /* 如果执行节点 与 接受节点岗位集合不一致 */
                    /* 没有查询到的情况下, 先按照本部门计算。*/

                    switch (BP.Sys.SystemConfig.AppCenterDBType)
                    {
                        case DBType.MySQL: 
                        case DBType.MSSQL:
                            sql = "select x.No from Port_Emp x inner join (select FK_Emp from " + BP.WF.Glo.EmpStation + " a inner join WF_NodeStation b ";
                            sql += " on a.FK_Station=b.FK_Station where FK_Node=" + dbStr + "FK_Node) as y on x.No=y.FK_Emp inner join Port_DeptEmp z on";
                            sql += " x.No=z.FK_Emp where z.FK_Dept =" + dbStr + "FK_Dept order by x.No";
                            break;
                        default:
                            sql = "SELECT No FROM Port_Emp WHERE NO IN "
                        + "(SELECT  FK_Emp  FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node) )"
                        + " AND  NO IN "
                        + "(SELECT  FK_Emp  FROM Port_DeptEmp WHERE FK_Dept =" + dbStr + "FK_Dept)";
                            sql += " ORDER BY No ";
                            break;
                    }

                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Node", item.NodeID);
                    ps.Add("FK_Dept", WebUser.FK_Dept);

                    dt = DBAccess.RunSQLReturnTable(ps);
                    foreach (DataRow dr in dt.Rows)
                    {
                        Emp emp = new Emp(dr[0].ToString());
                        sa.FK_Emp = emp.No;
                        sa.FK_Node = item.NodeID;
                        sa.DeptName = emp.FK_DeptText;

                        sa.WorkID = workid;
                        sa.Info = "无";
                        sa.AccType = 0;
                        sa.EmpName = emp.Name;

                        sa.ResetPK();
                        if (sa.IsExits)
                            continue;

                        //计算接受任务时间与应该完成任务时间.
                        InitDT(sa, item);

                        sa.Insert();
                    }
                    #endregion  按照岗位来执行。
                }
            }
        }
        /// <summary>
        /// 计算两个时间点.
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="nd"></param>
        private void InitDT(SelectAccper sa, Node nd)
        {
            //计算上一个时间的发送点.
            if (this.LastTimeDot == null)
            {
                Paras ps = new Paras();
                ps.SQL = "SELECT SDT FROM WF_GenerWorkerlist WHERE WorkID=" + ps.DBStr + "WorkID AND FK_Node=" + ps.DBStr + "FK_Node";
                ps.Add("WorkID", this.HisCurrWorkNode.WorkID);
                ps.Add("FK_Node", nd.NodeID);
                DataTable dt = DBAccess.RunSQLReturnTable(ps);

                foreach (DataRow dr in dt.Rows)
                {
                    this.LastTimeDot = dr[0].ToString();
                    break;
                }
            }

            //上一个节点的发送时间点或者 到期的时间点，就是当前节点的接受任务的时间。
            sa.PlanADT = this.LastTimeDot;

            //计算当前节点的应该完成日期。
            DateTime dtOfShould = Glo.AddDayHoursSpan(this.LastTimeDot, nd.TimeLimit, nd.TimeLimitHH,
                nd.TimeLimitMM, nd.TWay);
            sa.PlanSDT = dtOfShould.ToString(DataType.SysDatatimeFormatCN);

            //给最后的时间点复制.
            this.LastTimeDot = sa.PlanSDT;
        }
        /// <summary>
        /// 当前节点应该完成的日期.
        /// </summary>
        private string LastTimeDot = null;
    }
}
