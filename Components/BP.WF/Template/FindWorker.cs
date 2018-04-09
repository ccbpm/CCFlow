using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.GPM;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Port;
using BP.Sys;


namespace BP.WF.Template
{
    /// <summary>
    /// 找人规则
    /// </summary>
    public class FindWorker
    {
        #region 变量
        public WorkNode town = null;
        public WorkNode currWn = null;
        public Flow fl = null;
        string dbStr = BP.Sys.SystemConfig.AppCenterDBVarStr;
        public Paras ps = null;
        string JumpToEmp = null;
        int JumpToNode = 0;
        Int64 WorkID = 0;
        #endregion 变量

        /// <summary>
        /// 找人
        /// </summary>
        /// <param name="fl"></param>
        /// <param name="currWn"></param>
        /// <param name="toWn"></param>
        public FindWorker()
        {
        }
        public DataTable FindByWorkFlowModel()
        {
            this.town = town;

            DataTable dt = new DataTable();
            dt.Columns.Add("No", typeof(string));
            string sql;
            string FK_Emp;

            // 如果执行了两次发送，那前一次的轨迹就需要被删除,这里是为了避免错误。
            ps = new Paras();
            ps.Add("WorkID", this.WorkID);
            ps.Add("FK_Node", town.HisNode.NodeID);
            ps.SQL = "DELETE FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node =" + dbStr + "FK_Node";
            DBAccess.RunSQL(ps);

            // 如果指定特定的人员处理。
            if (DataType.IsNullOrEmpty(JumpToEmp) == false)
            {
                string[] emps = JumpToEmp.Split(',');
                foreach (string emp in emps)
                {
                    if (DataType.IsNullOrEmpty(emp))
                        continue;
                    DataRow dr = dt.NewRow();
                    dr[0] = emp;
                    dt.Rows.Add(dr);
                }
                return dt;
            }

            // 按上一节点发送人处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeEmp)
            {
                DataRow dr = dt.NewRow();
                dr[0] = BP.Web.WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }

            //首先判断是否配置了获取下一步接受人员的sql.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySQL
                || town.HisNode.HisDeliveryWay == DeliveryWay.BySQLAsSubThreadEmpsAndData)
            {
                if (town.HisNode.DeliveryParas.Length < 4)
                    throw new Exception("@您设置的当前节点按照SQL，决定下一步的接受人员，但是你没有设置SQL.");

                sql = town.HisNode.DeliveryParas;
                sql = sql.Clone().ToString();

                //特殊的变量.
                sql = sql.Replace("@FK_Node", this.town.HisNode.NodeID.ToString());
                sql = sql.Replace("@NodeID", this.town.HisNode.NodeID.ToString());

                sql = BP.WF.Glo.DealExp(sql, this.currWn.rptGe, null);
                if (sql.Contains("@"))
                {
                    if (BP.WF.Glo.SendHTOfTemp != null)
                    {
                        foreach (string key in BP.WF.Glo.SendHTOfTemp.Keys)
                        {
                            sql = sql.Replace("@" + key, BP.WF.Glo.SendHTOfTemp[key].ToString());
                        }
                    }
                }

                if (sql.Contains("@GuestUser.No"))
                    sql = sql.Replace("@GuestUser.No", GuestUser.No);

                if (sql.Contains("@GuestUser.Name"))
                    sql = sql.Replace("@GuestUser.Name", GuestUser.Name);

                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@没有找到可接受的工作人员。@技术信息：执行的SQL没有发现人员:" + sql);
                return dt;
            }

            #region 按绑定部门计算,该部门一人处理标识该工作结束(子线程)..
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySetDeptAsSubthread)
            {
                if (this.town.HisNode.HisRunModel != RunModel.SubThread)
                    throw new Exception("@您设置的节点接收人方式为：按绑定部门计算,该部门一人处理标识该工作结束(子线程)，但是当前节点非子线程节点。");

                sql = "SELECT No, Name,FK_Dept AS GroupMark FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + town.HisNode.NodeID + ")";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@没有找到可接受的工作人员,接受人方式为, ‘按绑定部门计算,该部门一人处理标识该工作结束(子线程)’ @技术信息：执行的SQL没有发现人员:" + sql);
                return dt;
            }
            #endregion 按绑定部门计算,该部门一人处理标识该工作结束(子线程)..

            #region 按照明细表,作为子线程的接收人.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDtlAsSubThreadEmps)
            {
                if (this.town.HisNode.HisRunModel != RunModel.SubThread)
                    throw new Exception("@您设置的节点接收人方式为：以分流点表单的明细表数据源确定子线程的接收人，但是当前节点非子线程节点。");

                BP.Sys.MapDtls dtls = new BP.Sys.MapDtls(this.currWn.HisNode.NodeFrmID);
                string msg = null;
                foreach (BP.Sys.MapDtl dtl in dtls)
                {
                    try
                    {
                        string empFild = town.HisNode.DeliveryParas;
                        if (DataType.IsNullOrEmpty(empFild))
                            empFild = " UserNo ";

                        ps = new Paras();
                        ps.SQL = "SELECT " + empFild + ", * FROM " + dtl.PTable + " WHERE RefPK=" + dbStr + "OID ORDER BY OID";
                        ps.Add("OID", this.WorkID);
                        dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker == false)
                            throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）在指定的节点中没有数据，无法找到子线程的工作人员。");
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        msg += ex.Message;
                        //if (dtls.Count == 1)
                        //    throw new Exception("@估计是流程设计错误,没有在分流节点的明细表中设置");
                    }
                }
                throw new Exception("@没有找到分流节点的明细表作为子线程的发起的数据源，流程设计错误，请确认分流节点表单中的明细表是否有UserNo约定的系统字段。" + msg);
            }
            #endregion 按照明细表,作为子线程的接收人.

            #region 按节点绑定的人员处理.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByBindEmp)
            {
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = "SELECT FK_Emp FROM WF_NodeEmp WHERE FK_Node=" + dbStr + "FK_Node ORDER BY FK_Emp";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")没有绑定工作人员 . ");
                return dt;
            }
            #endregion 按节点绑定的人员处理.

            #region 按照选择的人员处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelected
                || town.HisNode.HisDeliveryWay == DeliveryWay.ByFEE)
            {
                ps = new Paras();
                ps.Add("FK_Node", this.town.HisNode.NodeID);
                ps.Add("WorkID", this.currWn.HisWork.OID);
                ps.SQL = "SELECT FK_Emp FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND AccType=0 ORDER BY IDX";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    /*从上次发送设置的地方查询. */
                    SelectAccpers sas = new SelectAccpers();
                    int i = sas.QueryAccepterPriSetting(this.town.HisNode.NodeID);
                    if (i == 0)
                    {
                        if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelected)
                            throw new Exception("@请选择下一步骤工作(" + town.HisNode.Name + ")接受人员。");
                        else
                            throw new Exception("@流程设计错误，请重写FEE，然后为节点(" + town.HisNode.Name + ")设置接受人员，详细请参考cc流程设计手册。");
                    }

                    //插入里面.
                    foreach (SelectAccper item in sas)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = item.FK_Emp;
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                return dt;
            }
            #endregion 按照选择的人员处理。

            #region 按照指定节点的处理人计算。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySpecNodeEmp
                || town.HisNode.HisDeliveryWay == DeliveryWay.ByStarter)
            {
                /* 按指定节点的人员计算 */
                string strs = town.HisNode.DeliveryParas;
                if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStarter)
                {
                    /*找开始节点的处理人员. */
                    //strs = int.Parse(this.fl.No) + "01";
                    //ps = new Paras();
                    //ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                    //ps.Add("FK_Node", int.Parse(strs));

                    //if (currWn.HisNode.HisRunModel == RunModel.SubThread)
                    //    ps.Add("OID", currWn.HisWork.FID);
                    //else
                    //    ps.Add("OID", this.WorkID);


                    dt = DBAccess.RunSQLReturnTable("SELECT Starter No, StarterName Name FROM WF_GenerWorkFlow WHERE WorkID=" + this.currWn.HisWork.FID + " OR WorkID=" + this.WorkID);
                    if (dt.Rows.Count == 1)
                        return dt;
                    else
                    {
                        /* 有可能当前节点就是第一个节点，那个时间还没有初始化数据，就返回当前人. */
                        if (this.currWn.HisNode.IsStartNode)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = BP.Web.WebUser.No;
                            dt.Rows.Add(dr);
                            return dt;
                        }

                        if (dt.Rows.Count == 0)
                            throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）无法找到开始节点的工作人员。");
                        else
                            return dt;
                    }
                }

                // 首先从本流程里去找。
                strs = strs.Replace(";", ",");
                string[] nds = strs.Split(',');
                foreach (string nd in nds)
                {
                    if (DataType.IsNullOrEmpty(nd))
                        continue;

                    if (DataType.IsNumStr(nd) == false)
                        throw new Exception("流程设计错误:您设置的节点(" + town.HisNode.Name + ")的接收方式为按指定的节点岗位投递，但是您没有在访问规则设置中设置节点编号。");

                    ps = new Paras();
                    ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsEnable=1 ";
                    ps.Add("FK_Node", int.Parse(nd));
                    if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                        ps.Add("OID", this.currWn.HisWork.FID);
                    else
                        ps.Add("OID", this.WorkID);

                    DataTable dt_ND = DBAccess.RunSQLReturnTable(ps);
                    //添加到结果表
                    if (dt_ND.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt_ND.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = row[0].ToString();
                            dt.Rows.Add(dr);
                        }
                        //此节点已找到数据则不向下找，继续下个节点
                        continue;
                    }

                    //就要到轨迹表里查,因为有可能是跳过的节点.
                    ps = new Paras();
                    ps.SQL = "SELECT " + TrackAttr.EmpFrom + " FROM ND" + int.Parse(fl.No) + "Track WHERE (ActionType=" + dbStr + "ActionType1 OR ActionType=" + dbStr + "ActionType2 OR ActionType=" + dbStr + "ActionType3 OR ActionType=" + dbStr + "ActionType4 OR ActionType=" + dbStr + "ActionType5) AND NDFrom=" + dbStr + "NDFrom AND WorkID=" + dbStr + "WorkID";
                    ps.Add("ActionType1", (int)ActionType.Skip);
                    ps.Add("ActionType2", (int)ActionType.Forward);
                    ps.Add("ActionType3", (int)ActionType.ForwardFL);
                    ps.Add("ActionType4", (int)ActionType.ForwardHL);
                    ps.Add("ActionType5", (int)ActionType.Start);

                    ps.Add("NDFrom", int.Parse(nd));
                    ps.Add("WorkID", this.WorkID);
                    dt_ND = DBAccess.RunSQLReturnTable(ps);
                    if (dt_ND.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt_ND.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = row[0].ToString();
                            dt.Rows.Add(dr);
                        }
                    }
                }

                //本流程里没有有可能该节点是配置的父流程节点,也就是说子流程的一个节点与父流程指定的节点的工作人员一致.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.PWorkID != 0)
                {
                    foreach (string pnodeiD in nds)
                    {
                        if (DataType.IsNullOrEmpty(pnodeiD))
                            continue;

                        Node nd = new Node(int.Parse(pnodeiD));
                        if (nd.FK_Flow != gwf.PFlowNo)
                            continue; // 如果不是父流程的节点，就不执行.

                        ps = new Paras();
                        ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node AND IsPass=1 AND IsEnable=1 ";
                        ps.Add("FK_Node", nd.NodeID);
                        if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                            ps.Add("OID", gwf.PFID);
                        else
                            ps.Add("OID", gwf.PWorkID);

                        DataTable dt_PWork = DBAccess.RunSQLReturnTable(ps);
                        if (dt_PWork.Rows.Count != 0)
                        {
                            foreach (DataRow row in dt_PWork.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = row[0].ToString();
                                dt.Rows.Add(dr);
                            }
                            //此节点已找到数据则不向下找，继续下个节点
                            continue;
                        }

                        //就要到轨迹表里查,因为有可能是跳过的节点.
                        ps = new Paras();
                        ps.SQL = "SELECT " + TrackAttr.EmpFrom + " FROM ND" + int.Parse(fl.No) + "Track WHERE (ActionType=" + dbStr + "ActionType1 OR ActionType=" + dbStr + "ActionType2 OR ActionType=" + dbStr + "ActionType3 OR ActionType=" + dbStr + "ActionType4 OR ActionType=" + dbStr + "ActionType5) AND NDFrom=" + dbStr + "NDFrom AND WorkID=" + dbStr + "WorkID";
                        ps.Add("ActionType1", (int)ActionType.Start);
                        ps.Add("ActionType2", (int)ActionType.Forward);
                        ps.Add("ActionType3", (int)ActionType.ForwardFL);
                        ps.Add("ActionType4", (int)ActionType.ForwardHL);
                        ps.Add("ActionType5", (int)ActionType.Skip);

                        ps.Add("NDFrom", nd.NodeID);

                        if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                            ps.Add("OID", gwf.PFID);
                        else
                            ps.Add("OID", gwf.PWorkID);

                        dt_PWork = DBAccess.RunSQLReturnTable(ps);
                        if (dt_PWork.Rows.Count != 0)
                        {
                            foreach (DataRow row in dt_PWork.Rows)
                            {
                                DataRow dr = dt.NewRow();
                                dr[0] = row[0].ToString();
                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }
                //返回指定节点的处理人
                if (dt.Rows.Count != 0)
                    return dt;

                throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）在指定的节点(" + strs + ")中没有数据，无法找到工作的人员。 @技术信息如下: 投递方式:BySpecNodeEmp sql=" + ps.SQLNoPara);
            }
            #endregion 按照节点绑定的人员处理。

            #region 按照上一个节点表单指定字段的人员处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByPreviousNodeFormEmpsField)
            {
                // 检查接受人员规则,是否符合设计要求.
                string specEmpFields = town.HisNode.DeliveryParas;
                if (DataType.IsNullOrEmpty(specEmpFields))
                    specEmpFields = "SysSendEmps";

                if (this.currWn.rptGe.EnMap.Attrs.Contains(specEmpFields) == false)
                    throw new Exception("@您设置的接受人规则是按照表单指定的字段，决定下一步的接受人员，该字段{" + specEmpFields + "}已经删除或者丢失。");

                //获取接受人并格式化接受人, 
                string emps = this.currWn.rptGe.GetValStringByKey(specEmpFields);
                emps = emps.Replace(" ", ""); //去掉空格.

                if (emps.Contains(",") && emps.Contains(";"))
                {
                    /*如果包含,; 例如 zhangsan,张三;lisi,李四;*/
                    string[] myemps1 = emps.Split(';');
                    foreach (string str in myemps1)
                    {
                        if (DataType.IsNullOrEmpty(str))
                            continue;

                        string[] ss = str.Split(',');
                        DataRow dr = dt.NewRow();
                        dr[0] = ss[0];
                        dt.Rows.Add(dr);
                    }
                    if (dt.Rows.Count == 0)
                        throw new Exception("@输入的接受人员信息错误;[" + emps + "]。");
                    else
                        return dt;
                }

                emps = emps.Replace(";", ",");
                emps = emps.Replace("；", ",");
                emps = emps.Replace("，", ",");
                emps = emps.Replace("、", ",");
                emps = emps.Replace("@", ",");

                if (DataType.IsNullOrEmpty(emps))
                    throw new Exception("@没有在字段[" + this.currWn.HisWork.EnMap.Attrs.GetAttrByKey(specEmpFields).Desc + "]中指定接受人，工作无法向下发送。");

                // 把它加入接受人员列表中.
                string[] myemps = emps.Split(',');
                foreach (string s in myemps)
                {
                    if (DataType.IsNullOrEmpty(s))
                        continue;

                    //if (BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(NO) AS NUM FROM Port_Emp WHERE NO='" + s + "' or name='"+s+"'", 0) == 0)
                    //    continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = s;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            #endregion 按照上一个节点表单指定字段的人员处理。

            string prjNo = "";
            FlowAppType flowAppType = this.currWn.HisNode.HisFlow.HisFlowAppType;
            sql = "";
            if (this.currWn.HisNode.HisFlow.HisFlowAppType == FlowAppType.PRJ)
            {
                prjNo = "";
                try
                {
                    prjNo = this.currWn.HisWork.GetValStrByKey("PrjNo");
                }
                catch (Exception ex)
                {
                    throw new Exception("@当前流程是工程类流程，但是在节点表单中没有PrjNo字段(注意区分大小写)，请确认。@异常信息:" + ex.Message);
                }
            }

            #region 按部门与岗位的交集计算.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
            {
                //added by liuxc,2015.6.29.
                //区别集成与BPM模式
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneOne)
                {
                    sql = " SELECT a.No,a.Name FROM Port_Emp A, WF_NodeDept B, WF_NodeStation C, Port_EmpStation D ";
                    sql += " WHERE A.FK_Dept=B.FK_Dept AND A.No=D.FK_Emp AND C.FK_Station=D.FK_Station AND B.FK_Node=C.FK_Node ";
                    sql += " AND B.FK_Node=" + dbStr + "FK_Node";

                    ps = new Paras();
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                }
                else
                {
                    sql = "SELECT pdes.FK_Emp AS No"
                          + " FROM   Port_DeptEmpStation pdes"
                          + " INNER JOIN WF_NodeDept wnd ON wnd.FK_Dept = pdes.FK_Dept"
                          + " AND wnd.FK_Node = " + town.HisNode.NodeID
                          + " INNER JOIN WF_NodeStation wns ON  wns.FK_Station = pdes.FK_Station"
                          + " AND wns.FK_Node =" + town.HisNode.NodeID
                          + " ORDER BY pdes.FK_Emp";

                    dt = DBAccess.RunSQLReturnTable(sql);
                }

                if (dt.Rows.Count > 0)
                    return dt;
                else
                {
                    if (this.town.HisNode.HisWhenNoWorker == false)
                        throw new Exception("@节点访问规则(" + town.HisNode.HisDeliveryWay.ToString() + ")错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 按照岗位与部门的交集确定接受人的范围错误，没有找到人员:SQL=" + sql);
                    else
                        return dt;
                }
            }
            #endregion 按部门与岗位的交集计算.

            #region 判断节点部门里面是否设置了部门，如果设置了就按照它的部门处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDept)
            {
                ps = new Paras();
                ps.Add("FK_Node", this.town.HisNode.NodeID);
                ps.Add("WorkID", this.currWn.HisWork.OID);
                ps.SQL = "SELECT FK_Emp FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND WorkID=" + dbStr + "WorkID AND AccType=0 ORDER BY IDX";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;

                if (flowAppType == FlowAppType.Normal)
                {
                    ps = new Paras();
                    if (BP.WF.Glo.OSModel == OSModel.OneOne)
                    {
                        ps.SQL = "SELECT  A.No, A.Name  FROM Port_Emp A, WF_NodeDept B WHERE A.FK_Dept=B.FK_Dept AND B.FK_Node=" + dbStr + "FK_Node";
                    }
                    else if (BP.WF.Glo.OSModel == OSModel.OneMore)
                    {
                        ps.SQL = "SELECT  A.No, A.Name  FROM Port_Emp A, WF_NodeDept B, Port_DeptEmp C  WHERE  A.No = C.FK_Emp AND C.FK_Dept=B.FK_Dept AND B.FK_Node=" + dbStr + "FK_Node";
                    }

                    ps.Add("FK_Node", town.HisNode.NodeID);
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count > 0 && town.HisNode.HisWhenNoWorker == false)
                        return dt;
                    else
                        throw new Exception("@按部门确定接受人的范围,没有找到人员.");
                }

                if (flowAppType == FlowAppType.PRJ)
                {
                    sql = " SELECT A.No,A.Name FROM Port_Emp A, WF_NodeDept B, Prj_EmpPrjStation C, WF_NodeStation D ";
                    sql += "  WHERE A.FK_Dept=B.FK_Dept AND A.No=C.FK_Emp AND C.FK_Station=D.FK_Station AND B.FK_Node=D.FK_Node ";
                    sql += "  AND C.FK_Prj=" + dbStr + "FK_Prj  AND D.FK_Node=" + dbStr + "FK_Node";

                    ps = new Paras();
                    ps.Add("FK_Prj", prjNo);
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.SQL = sql;

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /* 如果项目组里没有工作人员就提交到公共部门里去找。*/
                        sql = "SELECT NO FROM Port_Emp WHERE NO IN ";

                        if (BP.WF.Glo.OSModel == OSModel.OneOne)
                            sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE FK_Dept IN ";
                        else
                            sql += "(SELECT FK_Emp FROM Port_DeptEmp WHERE FK_Dept IN ";

                        sql += "( SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + dbStr + "FK_Node1)";
                        sql += ")";
                        sql += "AND NO IN ";
                        sql += "(";
                        sql += "SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN ";
                        sql += "( SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node2)";
                        sql += ")";
                        sql += " ORDER BY No";

                        ps = new Paras();
                        ps.Add("FK_Node1", town.HisNode.NodeID);
                        ps.Add("FK_Node2", town.HisNode.NodeID);
                        ps.SQL = sql;
                    }
                    else
                    {
                        return dt;
                    }

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count > 0)
                        return dt;
                }
            }
            #endregion 判断节点部门里面是否设置了部门，如果设置了，就按照它的部门处理。

            #region 仅按岗位计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStationOnly)
            {
                sql = "SELECT A.FK_Emp FROM " + BP.WF.Glo.EmpStation + " A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;
                else
                {
                    if (this.town.HisNode.HisWhenNoWorker == false)
                        throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 仅按岗位计算，没有找到人员:SQL=" + ps.SQLNoPara);
                    else
                        return dt;  //可能处理跳转,在没有处理人的情况下.
                }
            }
            #endregion

            #region 按配置的人员路由表计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByFromEmpToEmp)
            {
                string[] fromto = town.HisNode.DeliveryParas.Split('@');

                string defUser = "";

                foreach (string str in fromto)
                {
                    string[] kv=str.Split(',');

                    if (kv[0].Equals("Defalut") == true)
                    {
                        defUser = kv[1];
                        continue;
                    }

                    if (kv[0]==WebUser.No)
                    {
                        string empTo=kv[1];
                        //BP.Port.Emp emp = new BP.Port.Emp(empTo);
                        DataRow dr = dt.NewRow();
                        dr[0] = empTo;
                      //  dr[1] = emp.Name;
                        dt.Rows.Add(dr);
                        return dt;
                    }
                }

                if (DataType.IsNullOrEmpty(defUser) == false)
                {
                    string empTo = defUser;
                    DataRow dr = dt.NewRow();
                    dr[0] = empTo;
                    dt.Rows.Add(dr);
                    return dt;
                }

                throw new Exception("@接收人规则是按照人员路由表设置的，但是系统管理员没有为您配置路由,当前节点;"+town.HisNode.Name);
            }
            #endregion


            #region 按岗位计算(以部门集合为纬度).
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStationAndEmpDept)
            {
                /* 考虑当前操作人员的部门, 如果本部门没有这个岗位就不向上寻找. */
                ps = new Paras();
                sql = "SELECT No,Name FROM Port_Emp WHERE No=" + dbStr + "FK_Emp ";
                ps.Add("FK_Emp", WebUser.No);
                dt = DBAccess.RunSQLReturnTable(ps);

                if (dt.Rows.Count > 0)
                    return dt;
                else
                {
                    if (this.town.HisNode.HisWhenNoWorker == false)
                        throw new Exception("@节点访问规则(" + town.HisNode.HisDeliveryWay.ToString() + ")错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 按岗位计算(以部门集合为纬度)。技术信息,执行的SQL=" + ps.SQLNoPara);
                    else
                        return dt; //可能处理跳转,在没有处理人的情况下.
                }
            }
            #endregion

            string empNo = WebUser.No;
            string empDept = WebUser.FK_Dept;

            #region 按指定的节点的人员岗位，做为下一步骤的流程接受人。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySpecNodeEmpStation)
            {
                /* 按指定的节点的人员岗位 */
                string para = town.HisNode.DeliveryParas;
                para = para.Replace("@", "");
                para = para.Replace("@", "");

                if (DataType.IsNullOrEmpty(para) == false)
                {
                    string[] strs = para.Split(',');

                    foreach (string str in strs)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;

                        ps = new Paras();
                        ps.SQL = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node ";
                        ps.Add("OID", this.WorkID);
                        ps.Add("FK_Node", int.Parse(str));

                        dt = DBAccess.RunSQLReturnTable(ps);
                        if (dt.Rows.Count != 1)
                            continue;

                        empNo = dt.Rows[0][0].ToString();
                        empDept = dt.Rows[0][1].ToString();
                    }

                    //  throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）在指定的节点中没有数据，无法找到工作的人员，指定的节点是:"+para);
                }
                else
                {
                    if (this.currWn.rptGe.Row.ContainsKey(para) == false)
                        throw new Exception("@在找人接收人的时候错误@字段{" + para + "}不包含在rpt里，流程设计错误。");

                    empNo = this.currWn.rptGe.GetValStrByKey(para);
                    if (DataType.IsNullOrEmpty(empNo))
                        throw new Exception("@字段{" + para + "}不能为空，没有取出来处理人员。");

                    BP.Port.Emp em = new BP.Port.Emp(empNo);
                    empDept = em.FK_Dept;
                }
            }
            #endregion 按指定的节点人员，做为下一步骤的流程接受人。

            #region 最后判断 - 按照岗位来执行。
            if (this.currWn.HisNode.IsStartNode == false)
            {
                ps = new Paras();
                if (flowAppType == FlowAppType.Normal || flowAppType == FlowAppType.DocFlow)
                {
                    // 如果当前的节点不是开始节点， 从轨迹里面查询。
                    sql = "SELECT DISTINCT FK_Emp  FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN "
                       + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + town.HisNode.NodeID + ") "
                       + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";

                    sql += " ORDER BY FK_Emp ";

                    ps.SQL = sql;
                    ps.Add("WorkID", this.WorkID);
                }

                if (flowAppType == FlowAppType.PRJ)
                {
                    // 如果当前的节点不是开始节点， 从轨迹里面查询。
                    sql = "SELECT DISTINCT FK_Emp  FROM Prj_EmpPrjStation WHERE FK_Station IN "
                       + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) AND FK_Prj=" + dbStr + "FK_Prj "
                       + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";
                    sql += " ORDER BY FK_Emp ";

                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.Add("FK_Prj", prjNo);
                    ps.Add("WorkID", this.WorkID);

                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /* 如果项目组里没有工作人员就提交到公共部门里去找。*/
                        sql = "SELECT DISTINCT FK_Emp  FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN "
                         + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) "
                         + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";
                        sql += " ORDER BY FK_Emp ";

                        ps = new Paras();
                        ps.SQL = sql;
                        ps.Add("FK_Node", town.HisNode.NodeID);
                        ps.Add("WorkID", this.WorkID);
                    }
                    else
                    {
                        return dt;
                    }
                }

                dt = DBAccess.RunSQLReturnTable(ps);
                // 如果能够找到.
                if (dt.Rows.Count >= 1)
                {
                    if (dt.Rows.Count == 1)
                    {
                        /*如果人员只有一个的情况，说明他可能要 */
                    }
                    return dt;
                }
            }

            /* 如果执行节点 与 接受节点岗位集合一致 */
            if (this.currWn.HisNode.GroupStaNDs == town.HisNode.GroupStaNDs)
            {
                /* 说明，就把当前人员做为下一个节点处理人。*/
                DataRow dr = dt.NewRow();
                dr[0] = WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }

            /* 如果执行节点 与 接受节点岗位集合不一致 */
            if (this.currWn.HisNode.GroupStaNDs != town.HisNode.GroupStaNDs)
            {
                /* 没有查询到的情况下, 先按照本部门计算。*/
                if (flowAppType == FlowAppType.Normal)
                {
                    if (BP.Sys.SystemConfig.OSDBSrc == Sys.OSDBSrc.Database)
                    {
                        if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                            sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B         WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept";
                        else
                            sql = "SELECT FK_Emp as No FROM Port_EmpStation A, WF_NodeStation B, Port_Emp C WHERE A.FK_Station=B.FK_Station AND A.FK_Emp=C.No  AND B.FK_Node=" + dbStr + "FK_Node AND C.FK_Dept=" + dbStr + "FK_Dept";
                        ps = new Paras();
                        ps.SQL = sql;
                        ps.Add("FK_Node", town.HisNode.NodeID);
                        ps.Add("FK_Dept", empDept);
                    }

                    if (BP.Sys.SystemConfig.OSDBSrc == Sys.OSDBSrc.WebServices)
                    {
                        DataTable dtStas = BP.DA.DBAccess.RunSQLReturnTable("SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + town.HisNode.NodeID);
                        string stas = DBAccess.GenerWhereInPKsString(dtStas);
                        var ws = DataType.GetPortalInterfaceSoapClientInstance();
                        return ws.GenerEmpsBySpecDeptAndStats(empDept, stas);
                    }
                }

                if (flowAppType == FlowAppType.PRJ)
                {
                    sql = "SELECT  FK_Emp  FROM Prj_EmpPrjStation WHERE FK_Prj=" + dbStr + "FK_Prj1 AND FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node)"
                    + " AND  FK_Prj=" + dbStr + "FK_Prj2 ";
                    sql += " ORDER BY FK_Emp ";

                    ps = new Paras();
                    ps.SQL = sql;
                    ps.Add("FK_Prj1", prjNo);
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.Add("FK_Prj2", prjNo);
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count == 0)
                    {
                        /* 如果项目组里没有工作人员就提交到公共部门里去找。 */

                        if (BP.WF.Glo.OSModel == OSModel.OneOne)
                        {
                            sql = "SELECT No FROM Port_Emp WHERE NO IN "
                          + "(SELECT  FK_Emp  FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node))"
                          + " AND  NO IN "
                          + "(SELECT No as FK_Emp FROM Port_Emp WHERE FK_Dept =" + dbStr + "FK_Dept)";
                            sql += " ORDER BY No ";
                        }
                        else
                        {
                            sql = "SELECT No FROM Port_Emp WHERE NO IN "
                        + "(SELECT  FK_Emp  FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node))"
                        + " AND  NO IN "
                        + "(SELECT FK_Emp FROM Port_DeptEmp WHERE FK_Dept =" + dbStr + "FK_Dept)";
                            sql += " ORDER BY No ";
                        }

                        ps = new Paras();
                        ps.SQL = sql;
                        ps.Add("FK_Node", town.HisNode.NodeID);
                        ps.Add("FK_Dept", empDept);
                        //  dt = DBAccess.RunSQLReturnTable(ps);
                    }
                    else
                    {
                        return dt;
                    }
                }

                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                {
                    NodeStations nextStations = town.HisNode.NodeStations;
                    if (nextStations.Count == 0)
                        throw new Exception("@节点没有岗位:" + town.HisNode.NodeID + "  " + town.HisNode.Name);
                }
                else
                {
                    bool isInit = false;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0].ToString() == BP.Web.WebUser.No)
                        {
                            /* 如果岗位分组不一样，并且结果集合里还有当前的人员，就说明了出现了当前操作员，拥有本节点上的岗位也拥有下一个节点的工作岗位
                             导致：节点的分组不同，传递到同一个人身上。 */
                            isInit = true;
                        }
                    }

#warning edit by peng, 用来确定不同岗位集合的传递包含同一个人的处理方式。

                    //  if (isInit == false || isInit == true)
                    return dt;
                }
            }

            /*这里去掉了向下级别寻找的算法. */


            /* 没有查询到的情况下, 按照最大匹配数 提高一个级别计算，递归算法未完成。
             * 因为:以上已经做的岗位的判断，就没有必要在判断其它类型的节点处理了。
             * */
            string nowDeptID = empDept.Clone() as string;
            while (true)
            {
                BP.Port.Dept myDept = new BP.Port.Dept(nowDeptID);
                nowDeptID = myDept.ParentNo;
                if (nowDeptID == "-1" || nowDeptID.ToString() == "0")
                {
                    break; /*一直找到了最高级仍然没有发现，就跳出来循环从当前操作员人部门向下找。*/
                    throw new Exception("@按岗位计算没有找到(" + town.HisNode.Name + ")接受人.");
                }

                //检查指定的部门下面是否有该人员.
                DataTable mydtTemp = this.Func_GenerWorkerList_DiGui(nowDeptID, empNo);
                if (mydtTemp == null)
                {
                    /*如果父亲级没有，就找父级的平级. */
                    BP.Port.Depts myDepts = new BP.Port.Depts();
                    myDepts.Retrieve(BP.Port.DeptAttr.ParentNo, myDept.ParentNo);
                    foreach (BP.Port.Dept item in myDepts)
                    {
                        if (item.No == nowDeptID)
                            continue;
                        mydtTemp = this.Func_GenerWorkerList_DiGui(item.No, empNo);
                        if (mydtTemp == null)
                            continue;
                        else
                            return mydtTemp;
                    }

                    continue; /*如果平级也没有，就continue.*/
                }
                else
                    return mydtTemp;
            }

            /*如果向上找没有找到，就考虑从本级部门上向下找。 */
            nowDeptID = empDept.Clone() as string;
            BP.Port.Depts subDepts = new BP.Port.Depts(nowDeptID);

            //递归出来子部门下有该岗位的人员
            DataTable mydt = Func_GenerWorkerList_DiGui_ByDepts(subDepts, empNo);
            if (mydt == null && this.town.HisNode.HisWhenNoWorker == false)
                throw new Exception("@按岗位智能计算没有找到(" + town.HisNode.Name + ")接受人 @当前工作人员:" + WebUser.No + ",名称:" + WebUser.Name + " , 部门编号:" + WebUser.FK_Dept + " 部门名称：" + WebUser.FK_DeptName);

            //add by zhoupeng  考虑到自动跳转，在没有接受人的情况下.
            if (mydt == null)
            {
                mydt = new DataTable();
                mydt.Columns.Add(new DataColumn("No", typeof(string)));
                mydt.Columns.Add(new DataColumn("Name", typeof(string)));
            }

            return mydt;
            #endregion  按照岗位来执行。
        }
        /// <summary>
        /// 递归出来子部门下有该岗位的人员
        /// </summary>
        /// <param name="subDepts"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public DataTable Func_GenerWorkerList_DiGui_ByDepts(BP.Port.Depts subDepts, string empNo)
        {
            foreach (BP.Port.Dept item in subDepts)
            {
                DataTable dt = Func_GenerWorkerList_DiGui(item.No, empNo);
                if (dt != null)
                    return dt;

                dt = Func_GenerWorkerList_DiGui_ByDepts(item.HisSubDepts, empNo);
                if (dt != null)
                    return dt;
            }
            return null;
        }
        /// <summary>
        /// 根据部门获取下一步的操作员
        /// </summary>
        /// <param name="deptNo"></param>
        /// <param name="emp1"></param>
        /// <returns></returns>
        public DataTable Func_GenerWorkerList_DiGui(string deptNo, string empNo)
        {
            string sql;

            Paras ps = new Paras();
            if (this.town.HisNode.IsExpSender == true)
            {
                /* 不允许包含当前处理人. */
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                    sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept AND A.FK_Emp!=" + dbStr + "FK_Emp";
                else
                    sql = "SELECT FK_Emp as No FROM Port_EmpStation A, WF_NodeStation B, Port_Emp C WHERE A.FK_Station=B.FK_Station AND A.FK_Emp=C.No AND B.FK_Node=" + dbStr + "FK_Node AND C.FK_Dept=" + dbStr + "FK_Dept AND A.FK_Emp!=" + dbStr + "FK_Emp";

                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", deptNo);
                ps.Add("FK_Emp", empNo);
            }
            else
            {
                if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                    sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept";
                else
                    sql = "SELECT FK_Emp as No FROM Port_EmpStation A, WF_NodeStation B, Port_Emp C WHERE A.FK_Station=B.FK_Station AND A.FK_Emp=C.No AND B.FK_Node=" + dbStr + "FK_Node AND C.FK_Dept=" + dbStr + "FK_Dept ";

                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", deptNo);
            }


            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
            {
                NodeStations nextStations = town.HisNode.NodeStations;
                if (nextStations.Count == 0)
                    throw new Exception("@节点没有岗位:" + town.HisNode.NodeID + "  " + town.HisNode.Name);

                sql = "SELECT No FROM Port_Emp WHERE No IN ";
                sql += "(SELECT  FK_Emp  FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station IN (SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + dbStr + "FK_Node ) )";
                sql += " AND No IN ";

                if (deptNo == "1")
                {
                    sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE No!=" + dbStr + "FK_Emp ) ";
                }
                else
                {
                    BP.Port.Dept deptP = new BP.Port.Dept(deptNo);
                    sql += "(SELECT No as FK_Emp FROM Port_Emp WHERE No!=" + dbStr + "FK_Emp AND FK_Dept = '" + deptP.ParentNo + "')";
                }

                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Emp", empNo);
                dt = DBAccess.RunSQLReturnTable(ps);

                if (dt.Rows.Count == 0)
                    return null;
                return dt;
            }
            else
            {
                return dt;
            }
            return null;
        }
        /// <summary>
        /// 执行找人
        /// </summary>
        /// <returns></returns>
        public DataTable DoIt(Flow fl, WorkNode currWn, WorkNode toWn)
        {
            // 给变量赋值.
            this.fl = fl;
            this.currWn = currWn;
            this.town = toWn;
            this.WorkID = currWn.WorkID;

            if (this.town.HisNode.IsGuestNode)
            {
                /*到达的节点是客户参与的节点. add by zhoupeng 2016.5.11*/
                DataTable mydt = new DataTable();
                mydt.Columns.Add("No", typeof(string));
                mydt.Columns.Add("Name", typeof(string));

                DataRow dr = mydt.NewRow();
                dr["No"] = "Guest";
                dr["Name"] = "外部用户";
                mydt.Rows.Add(dr);
                return mydt;
            }


            //如果到达的节点是按照workflow的模式。
            if (toWn.HisNode.HisDeliveryWay != DeliveryWay.ByCCFlowBPM)
            {
                DataTable re_dt = this.FindByWorkFlowModel();
                if (re_dt.Rows.Count == 1)
                    return re_dt; //如果只有一个人，就直接返回，就不处理了。

                #region 根据配置追加接收人 by dgq 2015.5.18

                string paras = this.town.HisNode.DeliveryParas;
                if (paras.Contains("@Spec"))
                {
                    //如果返回null ,则创建表
                    if (re_dt == null)
                    {
                        re_dt = new DataTable();
                        re_dt.Columns.Add("No", typeof(string));
                    }

                    //获取配置规则
                    string[] reWays = this.town.HisNode.DeliveryParas.Split('@');
                    foreach (string reWay in reWays)
                    {
                        if (DataType.IsNullOrEmpty(reWay))
                            continue;
                        string[] specItems = reWay.Split('=');
                        //配置规则错误
                        if (specItems.Length != 2)
                            continue;
                        //规则名称，SpecStations、SpecEmps
                        string specName = specItems[0];
                        //规则内容
                        string specContent = specItems[1];
                        switch (specName)
                        {
                            case "SpecStations"://按岗位
                                string[] stations = specContent.Split(',');
                                foreach (string station in stations)
                                {
                                    if (DataType.IsNullOrEmpty(station))
                                        continue;

                                    //获取岗位下的人员
                                    DataTable dt_Emps = DBAccess.RunSQLReturnTable("SELECT FK_Emp FROM " + BP.WF.Glo.EmpStation + " WHERE FK_Station='" + station + "'");
                                    foreach (DataRow empRow in dt_Emps.Rows)
                                    {
                                        //排除为空编号
                                        if (empRow[0] == null || DataType.IsNullOrEmpty(empRow[0].ToString()))
                                            continue;

                                        DataRow dr = re_dt.NewRow();
                                        dr[0] = empRow[0];
                                        re_dt.Rows.Add(dr);
                                    }
                                }
                                break;
                            case "SpecEmps"://按人员编号
                                string[] emps = specContent.Split(',');
                                foreach (string emp in emps)
                                {
                                    //排除为空编号
                                    if (DataType.IsNullOrEmpty(emp))
                                        continue;

                                    DataRow dr = re_dt.NewRow();
                                    dr[0] = emp;
                                    re_dt.Rows.Add(dr);
                                }
                                break;
                        }
                    }
                }
                #endregion

                //本节点接收人不允许包含上一步发送人 。
                if (this.town.HisNode.IsExpSender == true && re_dt.Columns.Count <= 2)
                {
                    /*
                     * 排除了接受人分组的情况, 因为如果有了分组，就破坏了分组的结构了.
                     * 
                     */
                    //复制表结构
                    DataTable dt = re_dt.Clone();
                    foreach (DataRow row in re_dt.Rows)
                    {
                        //排除当前登录人
                        if (row[0].ToString() == WebUser.No)
                            continue;

                        DataRow dr = dt.NewRow();
                        dr[0] = row[0];
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                return re_dt;
            }

            // 规则集合.
            FindWorkerRoles ens = new FindWorkerRoles(town.HisNode.NodeID);
            foreach (FindWorkerRole en in ens)
            {
                en.fl = this.fl;
                en.town = toWn;
                en.currWn = currWn;
                en.HisNode = currWn.HisNode;
                en.WorkID = this.WorkID;

                DataTable dt = en.GenerWorkerOfDataTable();
                if (dt == null || dt.Rows.Count == 0)
                    continue;

                //本节点接收人不允许包含上一步发送人
                if (this.town.HisNode.IsExpSender == true)
                {
                    DataTable re_dt = dt.Clone();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row[0].ToString() == WebUser.No)
                            continue;
                        DataRow dr = re_dt.NewRow();
                        dr[0] = row[0];
                        re_dt.Rows.Add(dr);
                    }
                    return re_dt;
                }
                return dt;
            }

            //没有找到人的情况，就返回空.
            return null;
        }


    }
}
