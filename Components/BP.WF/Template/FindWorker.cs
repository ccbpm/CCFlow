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
using System.Collections;

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
        string dbStr =  BP.Difference.SystemConfig.AppCenterDBVarStr;
        public Paras ps = null;
        string JumpToEmp = null;
        int JumpToNode = 0;
        Int64 WorkID = 0;
        #endregion 变量

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
                string[] myEmpStrs = JumpToEmp.Split(',');
                foreach (string emp in myEmpStrs)
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
                || town.HisNode.HisDeliveryWay == DeliveryWay.BySQLTemplate
                || town.HisNode.HisDeliveryWay == DeliveryWay.BySQLAsSubThreadEmpsAndData)
            {

                if (town.HisNode.HisDeliveryWay == DeliveryWay.BySQLTemplate)
                {
                    SQLTemplate st = new SQLTemplate(town.HisNode.DeliveryParas);
                    sql = st.Docs;
                }
                else
                {
                    if (town.HisNode.DeliveryParas.Length < 4)
                        throw new Exception("@您设置的当前节点按照SQL，决定下一步的接受人员，但是你没有设置SQL.");
                    sql = town.HisNode.DeliveryParas;
                    sql = sql.Clone().ToString();
                }


                //特殊的变量.
                sql = sql.Replace("@FK_Node", this.town.HisNode.NodeID.ToString());
                sql = sql.Replace("@NodeID", this.town.HisNode.NodeID.ToString());

                sql = sql.Replace("@WorkID", this.currWn.HisWork.OID.ToString());
                sql = sql.Replace("@FID", this.currWn.HisWork.FID.ToString());


                if (this.town.HisNode.FormType == NodeFormType.RefOneFrmTree)
                {
                    GEEntity en = new GEEntity(this.town.HisNode.NodeFrmID, this.currWn.HisWork.OID);
                    sql = BP.WF.Glo.DealExp(sql, en, null);
                }
                else
                    sql = BP.WF.Glo.DealExp(sql, this.currWn.rptGe, null);


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

                sql = "SELECT " + BP.Sys.Base.Glo.UserNo + ", Name,FK_Dept AS GroupMark FROM Port_Emp WHERE FK_Dept IN (SELECT FK_Dept FROM WF_NodeDept WHERE FK_Node=" + town.HisNode.NodeID + ")";
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

                this.currWn.HisNode.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.
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
                        if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                            ps.SQL = "SELECT " + empFild + ", A.* FROM " + dtl.PTable + " A WHERE RefPK=" + dbStr + "OID ORDER BY OID";
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

            string empNo = WebUser.No;
            string empDept = WebUser.FK_Dept;

            #region 找指定节点的人员直属领导 .
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByEmpLeader)
            {
                //查找指定节点的人员， 如果没有节点，就是当前的节点.
                string para = town.HisNode.DeliveryParas;
                if (DataType.IsNullOrEmpty(para) == true)
                    para = this.currWn.HisNode.NodeID.ToString();

                //throw new Exception("err@配置错误，当前节点是找指定节点的直属领导，但是您没有设置指定的节点ID.");

                string[] strs = para.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    ps = new Paras();
                    ps.SQL = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node ";
                    ps.Add("OID", this.WorkID);
                    ps.Add("FK_Node", int.Parse(str));
                    dt = DBAccess.RunSQLReturnTable(ps);
                    if (dt.Rows.Count != 1)
                        continue;

                    empNo = dt.Rows[0][0].ToString();

                    //查找人员的直属leader
                    sql = "";
                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                        sql = "SELECT Leader,FK_Dept FROM Port_Emp WHERE No='" + empNo + "'";
                    else
                        sql = "SELECT Leader,FK_Dept FROM Port_Emp WHERE No='" + BP.Web.WebUser.OrgNo + "_" + empNo + "'";

                    DataTable dtEmp = DBAccess.RunSQLReturnTable(sql);

                    //查找他的leader, 如果没有就找部门领导.
                    string leader = dtEmp.Rows[0][0] as string;
                    string deptNo = dtEmp.Rows[0][1] as string;
                    if (leader == null)
                    {
                        sql = "SELECT Leader FROM Port_Dept WHERE No='" + deptNo + "'";
                        leader = DBAccess.RunSQLReturnStringIsNull(sql, null);
                        if (leader == null)
                            throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")设置的按照直属领导计算，没有维护(" + WebUser.No + "," + BP.Web.WebUser.Name + ")的直属领导 . ");
                    }
                    dt = DBAccess.RunSQLReturnTable(sql);
                    return dt;
                }
            }
            #endregion .按照部门负责人计算

            #region 按照部门负责人计算. 
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDeptLeader)
                return ByDeptLeader();

            #endregion .按照部门负责人计算

            #region 按照部门分管领导计算.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDeptShipLeader)
                return ByDeptShipLeader();

            #endregion .按照部门负责人计算

            #region 按照选择的人员处理。
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelected || town.HisNode.HisDeliveryWay == DeliveryWay.BySelectedForPrj
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
                        if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelected
                            || town.HisNode.HisDeliveryWay == DeliveryWay.BySelectedForPrj)
                        {
                            Node toNode = this.town.HisNode;
                            Selector select = new Selector(toNode.NodeID);
                            if (select.SelectorModel == SelectorModel.GenerUserSelecter)
                                throw new Exception("url@./WorkOpt/AccepterOfGener.htm?FK_Flow=" + toNode.FK_Flow + "&FK_Node=" + this.currWn.HisNode.NodeID + "&ToNode=" + toNode.NodeID + "&WorkID=" + this.WorkID);
                            else
                                throw new Exception("url@./WorkOpt/Accepter.htm?FK_Flow=" + toNode.FK_Flow + "&FK_Node=" + this.currWn.HisNode.NodeID + "&ToNode=" + toNode.NodeID + "&WorkID=" + this.WorkID);
                        }
                        else
                        {
                            throw new Exception("@流程设计错误，请重写FEE，然后为节点(" + town.HisNode.Name + ")设置接受人员，详细请参考cc流程设计手册。");
                        }
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
                    Int64 myworkid = this.currWn.WorkID;
                    if (this.currWn.HisWork.FID != 0)
                        myworkid = this.currWn.HisWork.FID;
                    dt = DBAccess.RunSQLReturnTable("SELECT Starter as No, StarterName as Name FROM WF_GenerWorkFlow WHERE WorkID=" + myworkid);
                    if (dt.Rows.Count == 1)
                        return dt;

                    /* 有可能当前节点就是第一个节点，那个时间还没有初始化数据，就返回当前人. */
                    if (this.currWn.HisNode.IsStartNode)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = BP.Web.WebUser.No;
                        dt.Rows.Add(dr);
                        return dt;
                    }

                    if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker == false)
                        throw new Exception("@流程设计错误，到达的节点（" + town.HisNode.Name + "）无法找到开始节点的工作人员。");
                    else
                        return dt;

                }

                // 首先从本流程里去找。
                strs = strs.Replace(";", ",");
                string[] ndStrs = strs.Split(',');
                foreach (string nd in ndStrs)
                {
                    if (DataType.IsNullOrEmpty(nd))
                        continue;

                    if (DataType.IsNumStr(nd) == false)
                        throw new Exception("流程设计错误:您设置的节点(" + town.HisNode.Name + ")的接收方式为按指定的节点岗位投递，但是您没有在访问规则设置中设置节点编号。");

                    ps = new Paras();
                    string workSQL = "";
                    //获取指定节点的信息
                    Node specNode = new Node(nd);
                    //指定节点是子线程
                    if (specNode.HisRunModel == RunModel.SubThread)
                    {
                        if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                            workSQL = "FID=" + this.currWn.HisWork.FID;
                        else
                            workSQL = "FID=" + this.WorkID;
                    }
                    else
                    {
                        if (this.currWn.HisNode.HisRunModel == RunModel.SubThread)
                            workSQL = "WorkID=" + this.currWn.HisWork.FID;
                        else
                            workSQL = "WorkID=" + this.WorkID;

                    }

                    ps.SQL = "SELECT DISTINCT(FK_Emp) FROM WF_GenerWorkerList WHERE " + workSQL + " AND FK_Node=" + dbStr + "FK_Node AND IsEnable=1 ";
                    ps.Add("FK_Node", int.Parse(nd));

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
                    ps.SQL = "SELECT DISTINCT(" + TrackAttr.EmpFrom + ") FROM ND" + int.Parse(fl.No) + "Track WHERE"
                        + " (ActionType=" + dbStr + "ActionType1 OR ActionType=" + dbStr + "ActionType2 OR ActionType=" + dbStr + "ActionType3"
                        + "  OR ActionType=" + dbStr + "ActionType4 OR ActionType=" + dbStr + "ActionType5 OR ActionType=" + dbStr + "ActionType6)"
                        + "   AND NDFrom=" + dbStr + "NDFrom AND " + workSQL;
                    ps.Add("ActionType1", (int)ActionType.Skip);
                    ps.Add("ActionType2", (int)ActionType.Forward);
                    ps.Add("ActionType3", (int)ActionType.ForwardFL);
                    ps.Add("ActionType4", (int)ActionType.ForwardHL);
                    ps.Add("ActionType5", (int)ActionType.SubThreadForward);
                    ps.Add("ActionType6", (int)ActionType.Start);
                    ps.Add("NDFrom", int.Parse(nd));

                    dt_ND = DBAccess.RunSQLReturnTable(ps);
                    if (dt_ND.Rows.Count != 0)
                    {
                        foreach (DataRow row in dt_ND.Rows)
                        {
                            DataRow dr = dt.NewRow();
                            dr[0] = row[0].ToString();
                            dt.Rows.Add(dr);
                        }
                        continue;
                    }

                    //从Selector中查找
                    ps = new Paras();
                    ps.SQL = "SELECT DISTINCT(FK_Emp) FROM WF_SelectAccper WHERE FK_Node=" + dbStr + "FK_Node AND " + workSQL;
                    ps.Add("FK_Node", int.Parse(nd));


                    dt_ND = DBAccess.RunSQLReturnTable(ps);
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


                }

                //本流程里没有有可能该节点是配置的父流程节点,也就是说子流程的一个节点与父流程指定的节点的工作人员一致.
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.PWorkID != 0)
                {
                    foreach (string pnodeiD in ndStrs)
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
                            ps.Add("WorkID", gwf.PFID);
                        else
                            ps.Add("WorkID", gwf.PWorkID);

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

                //判断该字段是否启用了pop返回值？
                sql = "SELECT  Tag1 AS VAL FROM Sys_FrmEleDB WHERE RefPKVal=" + this.WorkID + " AND EleID='" + specEmpFields + "'";
                string emps = "";
                DataTable dtVals = DBAccess.RunSQLReturnTable(sql);

                //获取接受人并格式化接受人, 
                if (dtVals.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtVals.Rows)
                        emps += dr[0].ToString() + ",";
                }
                else
                {
                    emps = this.currWn.rptGe.GetValStringByKey(specEmpFields);
                }


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
                    if (dt.Rows.Count == 0 && town.HisNode.HisWhenNoWorker == false)
                        throw new Exception("@输入的接受人员信息错误;[" + emps + "]。");
                    else
                        return dt;
                }

                emps = emps.Replace(";", ",");
                emps = emps.Replace("；", ",");
                emps = emps.Replace("，", ",");
                emps = emps.Replace("、", ",");
                emps = emps.Replace("@", ",");

                if (DataType.IsNullOrEmpty(emps) && town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@没有在字段[" + this.currWn.HisWork.EnMap.Attrs.GetAttrByKey(specEmpFields).Desc + "]中指定接受人，工作无法向下发送。");

                // 把它加入接受人员列表中.
                string[] myemps = emps.Split(',');
                foreach (string s in myemps)
                {
                    if (DataType.IsNullOrEmpty(s))
                        continue;

                    //if (DBAccess.RunSQLReturnValInt("SELECT COUNT(NO) AS NUM FROM Port_Emp WHERE NO='" + s + "' or name='"+s+"'", 0) == 0)
                    //    continue;

                    DataRow dr = dt.NewRow();
                    dr[0] = s;
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            #endregion 按照上一个节点表单指定字段的人员处理。

            #region 为省立医院增加，按照指定的部门范围内的岗位计算..
            if (town.HisNode.HisDeliveryWay == DeliveryWay.FindSpecDeptEmpsInStationlist)
            {
                //sql = "SELECT pdes.FK_Emp AS No"
                //      + " FROM   Port_DeptEmpStation pdes"
                //      + " INNER JOIN WF_NodeDept wnd ON wnd.FK_Dept = pdes.FK_Dept"
                //      + " AND wnd.FK_Node = " + town.HisNode.NodeID
                //      + " INNER JOIN WF_NodeStation wns ON  wns.FK_Station = pdes.FK_Station"
                //      + " AND wns.FK_Node =" + town.HisNode.NodeID
                //      + " ORDER BY pdes.FK_Emp";

                sql = "SELECT A.FK_Emp FROM Port_DeptEmpStation A WHERE A.FK_DEPT ='" + BP.Web.WebUser.FK_Dept + "' AND A.FK_Station in(";
                sql += "select FK_Station from WF_NodeStation where FK_node=" + town.HisNode.NodeID + ")";

                dt = DBAccess.RunSQLReturnTable(sql);

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

            #region 按部门与岗位的交集计算.
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByDeptAndStation)
            {
                //added by liuxc,2015.6.29.

                sql = "SELECT pdes.FK_Emp AS No"
                     + " FROM   Port_DeptEmpStation pdes"
                     + " INNER JOIN WF_NodeDept wnd ON wnd.FK_Dept = pdes.FK_Dept"
                     + " AND wnd.FK_Node = " + town.HisNode.NodeID
                     + " INNER JOIN WF_NodeStation wns ON  wns.FK_Station = pdes.FK_Station"
                     + " AND wns.FK_Node =" + town.HisNode.NodeID
                     + " ORDER BY pdes.FK_Emp";


                dt = DBAccess.RunSQLReturnTable(sql);

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
            }
            #endregion 判断节点部门里面是否设置了部门，如果设置了，就按照它的部门处理。

            #region 用户组 计算 
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByTeamOnly)
            {
                ps = new Paras();
                sql = "SELECT A.FK_Emp FROM Port_TeamEmp A, WF_NodeTeam B WHERE A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;

                if (this.town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 仅按用户组计算，没有找到人员:SQL=" + ps.SQLNoPara);
                else
                    return dt;  //可能处理跳转,在没有处理人的情况下.
            }
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByTeamOrgOnly)
            {
                sql = "SELECT DISTINCT A.FK_Emp FROM Port_TeamEmp A, WF_NodeTeam B, Port_Emp C WHERE A.FK_Emp=C." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node AND C.OrgNo=" + dbStr + "OrgNo  ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("OrgNo", BP.Web.WebUser.OrgNo);

                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;

                if (this.town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 仅按用户组计算，没有找到人员:SQL=" + ps.SQLNoPara);

                return dt;  //可能处理跳转,在没有处理人的情况下.
            }

            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByTeamDeptOnly)
            {
                sql = "SELECT DISTINCT A.FK_Emp FROM Port_TeamEmp A, WF_NodeTeam B, Port_Emp C WHERE A.FK_Emp=C." + BP.Sys.Base.Glo.UserNoWhitOutAS + " AND A.FK_Team=B.FK_Team AND B.FK_Node=" + dbStr + "FK_Node AND C.FK_Dept=" + dbStr + "FK_Dept  ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", BP.Web.WebUser.FK_Dept);

                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count > 0)
                    return dt;

                if (this.town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@节点访问规则错误 ByTeamDeptOnly :节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 仅按用户组计算，没有找到人员:SQL=" + ps.SQLNoPara);

                return dt;  //可能处理跳转,在没有处理人的情况下.
            }
            #endregion

            #region 仅按岗位计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStationOnly)
            {
                ps = new Paras();
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    //2020-4-25 按照岗位倒序排序 修改原因队列模式时，下级岗位处理后发给上级岗位， 岗位越高数值越小
                    sql = "SELECT A.FK_Emp FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND A.OrgNo=" + dbStr + "OrgNo AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Station desc";
                    ps.Add("OrgNo", BP.Web.WebUser.OrgNo);
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                }
                else
                {
                    //2020-4-25 按照岗位倒序排序 修改原因队列模式时，下级岗位处理后发给上级岗位， 岗位越高数值越小
                    sql = "SELECT A.FK_Emp FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Station desc";
                    ps.Add("FK_Node", town.HisNode.NodeID);
                    ps.SQL = sql;
                    dt = DBAccess.RunSQLReturnTable(ps);
                }
                if (dt.Rows.Count > 0)
                    return dt;

                if (this.town.HisNode.HisWhenNoWorker == false)
                {
                    //   throw new Exception("@节点访问规则错误:节点(" + town.HisNode.NodeID + "," + town.HisNode.Name + "), 仅按岗位计算，没有找到人员:SQL=" + ps.SQLNoPara);
                    throw new Exception("@节点访问规则错误:流程[" + town.HisNode.FlowName + "]节点[" + town.HisNode.NodeID + "," + town.HisNode.Name + "], 仅按岗位计算，没有找到人员。");
                }

                return dt;  //可能处理跳转,在没有处理人的情况下.
            }
            #endregion

            #region 按配置的人员路由表计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByFromEmpToEmp)
            {
                string[] fromto = town.HisNode.DeliveryParas.Split('@');

                string defUser = "";

                foreach (string str in fromto)
                {
                    string[] kv = str.Split(',');

                    if (kv[0].Equals("Defalut") == true)
                    {
                        defUser = kv[1];
                        continue;
                    }

                    if (kv[0] == WebUser.No)
                    {
                        string empTo = kv[1];
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

                throw new Exception("@接收人规则是按照人员路由表设置的，但是系统管理员没有为您配置路由,当前节点;" + town.HisNode.Name);
            }
            #endregion

            #region 按岗位计算(以部门集合为纬度).
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByStationAndEmpDept)
            {
                /* 考虑当前操作人员的部门, 如果本部门没有这个岗位就不向上寻找. */

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    ps = new Paras();
                    ps.SQL = "SELECT UserID as No,Name FROM Port_Emp WHERE UserID=" + dbStr + "FK_Emp AND OrgNo=" + dbStr + "OrgNo ";
                    ps.Add("FK_Emp", WebUser.No);
                    ps.Add("OrgNo", WebUser.OrgNo);

                    dt = DBAccess.RunSQLReturnTable(ps);
                }
                else
                {
                    ps = new Paras();
                    ps.SQL = "SELECT No,Name FROM Port_Emp WHERE No=" + dbStr + "FK_Emp ";
                    ps.Add("FK_Emp", WebUser.No);
                    dt = DBAccess.RunSQLReturnTable(ps);

                }

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

            #region 按照自定义的URL来计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelfUrl)
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
                        Node toNode = this.town.HisNode;
                        GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                        if (DataType.IsNullOrEmpty(toNode.DeliveryParas) == true)
                            throw new Exception("节点" + toNode.NodeID + "_" + toNode.Name + "设置的接收人规则是自定义的URL,现在未获取到设置的信息");
                        else
                            throw new Exception("BySelfUrl@" + toNode.DeliveryParas + "?FK_Flow=" + toNode.FK_Flow + "&FK_Node=" + this.currWn.HisNode.NodeID + "&ToNode=" + toNode.NodeID + "&WorkID=" + this.WorkID + "&PWorkID=" + gwf.PWorkID + "&FID=" + gwf.FID);
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
            #endregion 按照自定义的URL来计算

            #region 按照设置的WebAPI接口获取的数据计算
            if (town.HisNode.HisDeliveryWay == DeliveryWay.ByAPIUrl)
            {
                //返回值
                string postData = "";
                //用户输入的webAPI地址
                string apiUrl = town.HisNode.DeliveryParas;
                if (apiUrl.Contains("@WebApiHost"))//可以替换配置文件中配置的webapi地址
                    apiUrl = apiUrl.Replace("@WebApiHost", BP.Difference.SystemConfig.AppSettings["WebApiHost"]);
                //如果有参数
                if (apiUrl.Contains("?"))
                {
                    //api接口地址
                    string apiHost = apiUrl.Split('?')[0];
                    //api参数
                    string apiParams = apiUrl.Split('?')[1];
                    //参数替换
                    apiParams = BP.WF.Glo.DealExp(apiParams, town.HisWork);
                    //执行POST
                    postData = BP.WF.Glo.HttpPostConnect(apiHost, apiParams);

                    if (postData == "[]" || postData == "" || postData == null)
                        throw new Exception("节点" + town.HisNode.NodeID + "_" + town.HisNode.Name + "设置的WebAPI接口返回的数据出错，请检查接口返回值。");

                    dt = BP.Tools.Json.ToDataTable(postData);
                    return dt;
                }
                else
                {//如果没有参数，执行GET
                    postData = BP.WF.Glo.HttpGet(apiUrl);
                    if (postData == "[]" || postData == "" || postData == null)
                        throw new Exception("节点" + town.HisNode.NodeID + "_" + town.HisNode.Name + "设置的WebAPI接口返回的数据出错，请检查接口返回值。");

                    dt = BP.Tools.Json.ToDataTable(postData);
                    return dt;
                }
            }
            #endregion 按照设置的WebAPI接口获取的数据计算

            #region 按照组织模式人员选择器
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySelectedEmpsOrgModel)
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
                        Node toNode = this.town.HisNode;
                        throw new Exception("url@./WorkOpt/AccepterOfOrg.htm?FK_Flow=" + toNode.FK_Flow + "&FK_Node=" + this.currWn.HisNode.NodeID + "&ToNode=" + toNode.NodeID + "&WorkID=" + this.WorkID);
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
            #endregion 按照自定义的URL来计算

            #region 发送人的上级部门的负责人: 2022.2.20 benjing. by zhoupeng  
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptLeader)
            {
                string deptNo = BP.Web.WebUser.DeptParentNo;
                sql = "SELECT A.No,A.Name FROM Port_Emp A, Port_Dept B WHERE A.No=B.Leader AND B.No='" + deptNo + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                string leaderNo = null;
                if (dt.Rows.Count == 1)
                {
                    leaderNo = dt.Rows[0][0] as string;
                    //如果领导是当前操作员，就让其找上一级的部门领导。
                    if (leaderNo != null && WebUser.No.Equals(leaderNo) == true)
                        leaderNo = null;
                }

                if (dt.Rows.Count == 0 || BP.DA.DataType.IsNullOrEmpty(leaderNo) == true)
                {
                    //如果没有找到,就到父节点去找.
                    BP.Port.Dept pDept = new BP.Port.Dept(deptNo);
                    sql = "SELECT A.No,A.Name FROM Port_Emp A, Port_Dept B WHERE A.No=B.Leader AND B.No='" + pDept.No + "'";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    return dt;
                    // throw new Exception("err@按照 [发送人的上级部门的负责人] 计算接收人的时候出现错误，您没有维护部门[" + pDept.Name + "]的部门负责人.");
                }
                return dt;
            }
            #endregion 发送人的上级部门的负责人 2022.2.20 benjing.

            #region 发送人上级部门指定的岗位 2022.2.20 beijing. by zhoupeng  
            if (town.HisNode.HisDeliveryWay == DeliveryWay.BySenderParentDeptStations)
            {
                string deptNo = BP.Web.WebUser.DeptParentNo;
                sql = "SELECT A.FK_Emp FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + town.HisNode.NodeID + " AND A.FK_Dept='" + deptNo + "'";
                dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                {
                    BP.Port.Dept pDept = new BP.Port.Dept(deptNo);
                    throw new Exception("err@按照 [发送人上级部门指定的岗位] 计算接收人的时候出现错误，没有找到人，请检查节点绑定的岗位以及该部门【" + pDept.Name + "】下的人员设置的岗位.");
                }
                return dt;
            }
            #endregion 发送人的上级部门的负责人 2022.2.20 beijing.  

            #region 最后判断 - 按照岗位来执行。
            //从历史数据中获取接收人 2022-03-25这块代码注释，需要查询本部门的岗位
            //if (this.currWn.HisNode.IsStartNode == false)
            //{
            //    ps = new Paras();

            //    如果当前的节点不是开始节点， 从轨迹里面查询。
            //    sql = "SELECT DISTINCT FK_Emp  FROM Port_DeptEmpStation WHERE FK_Station IN "
            //       + "(SELECT FK_Station FROM WF_NodeStation WHERE FK_Node=" + town.HisNode.NodeID + ") "
            //       + "AND FK_Emp IN (SELECT FK_Emp FROM WF_GenerWorkerlist WHERE WorkID=" + dbStr + "WorkID AND FK_Node IN (" + DataType.PraseAtToInSql(town.HisNode.GroupStaNDs, true) + ") )";

            //    sql += " ORDER BY FK_Emp ";

            //    ps.SQL = sql;
            //    ps.Add("WorkID", this.WorkID);
            //    dt = DBAccess.RunSQLReturnTable(ps);
            //    如果能够找到.
            //    if (dt.Rows.Count >= 1)
            //        return dt;
            //}



            /* 如果执行节点 与 接受节点岗位集合一致 */
            string currGroupStaNDs = this.currWn.HisNode.GroupStaNDs;
            string toNodeTeamStaNDs = town.HisNode.GroupStaNDs;

            if (DataType.IsNullOrEmpty(currGroupStaNDs) == false && currGroupStaNDs.Equals(toNodeTeamStaNDs) == true)
            {
                /* 说明，就把当前人员做为下一个节点处理人。*/
                DataRow dr = dt.NewRow();
                if (dt.Columns.Count == 0)
                    dt.Columns.Add("No");

                dr[0] = WebUser.No;
                dt.Rows.Add(dr);
                return dt;
            }

            //获取当前人员信息的
            Hashtable ht = GetEmpDeptBySFModel();
            empDept = ht["DeptNo"].ToString();
            empNo = ht["EmpNo"].ToString();

            /* 如果执行节点 与 接受节点岗位集合不一致 */
            if ((DataType.IsNullOrEmpty(toNodeTeamStaNDs) == true && DataType.IsNullOrEmpty(currGroupStaNDs) == true)
                || currGroupStaNDs.Equals(toNodeTeamStaNDs) == false)
            {
                /* 没有查询到的情况下, 先按照本部门计算。*/


                sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B         WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept";
                ps = new Paras();
                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", empDept);

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

            //第1步:直线父级寻找.
            while (true)
            {
                BP.Port.Dept myDept = new BP.Port.Dept(nowDeptID);
                nowDeptID = myDept.ParentNo;
                if (nowDeptID == "-1" || nowDeptID.ToString() == "0")
                {
                    break; /*一直找到了最高级仍然没有发现，就跳出来循环从当前操作员人部门向下找。*/
                    throw new Exception("@按岗位计算没有找到(" + town.HisNode.Name + ")接受人.");
                }

                //检查指定的父部门下面是否有该人员.
                DataTable mydtTemp = this.Func_GenerWorkerList_SpecDept(nowDeptID, empNo);
                if (mydtTemp.Rows.Count != 0)
                    return mydtTemp;

                continue;
            }

            //第2步：父级的子级.
            nowDeptID = empDept.Clone() as string;
            while (true)
            {
                BP.Port.Dept myDept = new BP.Port.Dept(nowDeptID);
                nowDeptID = myDept.ParentNo;
                if (nowDeptID == "-1" || nowDeptID.ToString() == "0")
                {
                    break; /*一直找到了最高级仍然没有发现，就跳出来循环从当前操作员人部门向下找。*/
                    throw new Exception("@按岗位计算没有找到(" + town.HisNode.Name + ")接受人.");
                }

                //该部门下的所有子部门是否有人员.
                DataTable mydtTemp = Func_GenerWorkerList_SpecDept_SameLevel(nowDeptID, empNo);
                if (mydtTemp.Rows.Count != 0)
                    return mydtTemp;
                continue;
            }

            /*如果向上找没有找到，就考虑从本级部门上向下找。只找一级下级的平级 */
            nowDeptID = empDept.Clone() as string;

            //递归出来子部门下有该岗位的人员
            DataTable mydt = Func_GenerWorkerList_SpecDept_SameLevel(nowDeptID, empNo);

            if ((mydt == null || mydt.Rows.Count == 0) && this.town.HisNode.HisWhenNoWorker == false)
            {
                //如果递归没有找到人,就全局搜索岗位.
                sql = "SELECT A.FK_Emp FROM  Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node ORDER BY A.FK_Emp";
                ps = new Paras();
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.SQL = sql;
                dt = DBAccess.RunSQLReturnTable(ps);

                if (dt.Rows.Count > 0)
                    return dt;
                if (this.town.HisNode.HisWhenNoWorker == false)
                    throw new Exception("@按岗位智能计算没有找到(" + town.HisNode.Name + ")接受人 @当前工作人员:" + WebUser.No + ",名称:" + WebUser.Name + " , 部门编号:" + WebUser.FK_Dept + " 部门名称：" + WebUser.FK_DeptName);

                if (dt.Rows.Count == 0)
                {
                    mydt = new DataTable();
                    mydt.Columns.Add(new DataColumn("No", typeof(string)));
                    mydt.Columns.Add(new DataColumn("Name", typeof(string)));
                }
            }

            return mydt;
            #endregion  按照岗位来执行。
        }

        private Hashtable GetEmpDeptBySFModel()
        {
            Node nd = town.HisNode;
            Hashtable ht = new Hashtable();
            //身份模式.
            var sfModel = nd.GetParaInt("ShenFenModel");

            //身份参数.
            var sfVal = nd.GetParaString("ShenFenVal");

            //按照当前节点的身份计算
            if (sfModel == 0)
                sfVal = currWn.HisNode.NodeID.ToString();

            //按照指定节点的身份计算.
            if (sfModel == 0 || sfModel == 1)
            {
                if (DataType.IsNullOrEmpty(sfVal))
                    sfVal = currWn.HisNode.NodeID.ToString();
                Paras ps = new Paras();
                ps.SQL = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node Order By RDT DESC";
                ps.Add("OID", this.WorkID);
                ps.Add("FK_Node", int.Parse(sfVal));

                DataTable dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    throw new Exception("err@不符合常理，没有找到数据");
                ht.Add("EmpNo", dt.Rows[0][0].ToString());
                ht.Add("DeptNo", dt.Rows[0][1].ToString());
            }

            //按照 字段的值的人员编号作为身份计算.
            if (sfModel == 2)
            {
                //获得字段的值.
                string empNo = "";
                if (currWn.HisNode.HisFormType == NodeFormType.RefOneFrmTree)
                    empNo = currWn.HisWork.GetValStrByKey(sfVal);
                else
                    empNo = currWn.rptGe.GetValStrByKey(sfVal);
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.UserID = empNo;
                if (emp.RetrieveFromDBSources() == 0)
                    throw new Exception("err@根据字段值:" + sfVal + "在Port_Emp中没有找到人员信息");
                ht.Add("EmpNo", emp.No);
                ht.Add("DeptNo", emp.FK_Dept);
            }
            return ht;
        }
        /// <summary>
        /// 找部门的领导
        /// </summary>
        /// <returns></returns>
        private DataTable ByDeptLeader()
        {

            Node nd = town.HisNode;

            //身份模式.
            var sfModel = nd.GetParaInt("ShenFenModel");

            //身份参数.
            var sfVal = nd.GetParaString("ShenFenVal");

            //按照当前节点的身份计算.
            if (sfModel == 0)
                return ByDeptLeader_Nodes(currWn.HisNode.NodeID.ToString());

            //按照指定节点的身份计算.
            if (sfModel == 1)
                return ByDeptLeader_Nodes(sfVal);

            //按照 字段的值的人员编号作为身份计算.
            if (sfModel == 2)
            {
                //获得字段的值.
                string empNo = "";
                if (currWn.HisNode.HisFormType == NodeFormType.RefOneFrmTree)
                    empNo = currWn.HisWork.GetValStrByKey(sfVal);
                else
                    empNo = currWn.rptGe.GetValStrByKey(sfVal);
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.UserID = empNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    throw new Exception("err@根据字段值:" + sfVal + "在Port_Emp中没有找到人员信息");
                }
                return ByDeptLeader_Fields(emp.No, emp.FK_Dept);
            }

            throw new Exception("err@没有判断的身份模式.");
        }
        /// <summary>
        /// 找部门的分管领导
        /// </summary>
        /// <returns></returns>
        private DataTable ByDeptShipLeader()
        {

            Node nd = town.HisNode;

            //身份模式.
            var sfModel = nd.GetParaInt("ShenFenModel");

            //身份参数.
            var sfVal = nd.GetParaString("ShenFenVal");

            //按照当前节点的身份计算
            if (sfModel == 0)
                return ByDeptShipLeader_Nodes(currWn.HisNode.NodeID.ToString());

            //按照指定节点的身份计算.
            if (sfModel == 1)
                return ByDeptShipLeader_Nodes(sfVal);

            //按照 字段的值的人员编号作为身份计算.
            if (sfModel == 2)
            {
                //获得字段的值.
                string empNo = "";
                if (currWn.HisNode.HisFormType == NodeFormType.RefOneFrmTree)
                    empNo = currWn.HisWork.GetValStrByKey(sfVal);
                else
                    empNo = currWn.rptGe.GetValStrByKey(sfVal);
                BP.Port.Emp emp = new BP.Port.Emp();
                emp.UserID = empNo;
                if (emp.RetrieveFromDBSources() == 0)
                {
                    throw new Exception("err@根据字段值:" + sfVal + "在Port_Emp中没有找到人员信息");
                }
                return ByDeptShipLeader_Fields(emp.No, emp.FK_Dept);
            }

            throw new Exception("err@没有判断的身份模式.");
        }
        private DataTable ByDeptLeader_Nodes(string nodes)
        {
            DataTable dt = null;
            //查找指定节点的人员， 如果没有节点，就是当前的节点.
            if (DataType.IsNullOrEmpty(nodes) == true)
                nodes = this.currWn.HisNode.NodeID.ToString();

            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node Order By RDT DESC";
            ps.Add("OID", this.WorkID);
            ps.Add("FK_Node", int.Parse(nodes));

            dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
                throw new Exception("err@不符合常理，没有找到数据");
            string empNo = dt.Rows[0][0].ToString();
            string deptNo = dt.Rows[0][1].ToString();
            return ByDeptLeader_Fields(empNo, deptNo);
        }
        private DataTable ByDeptShipLeader_Nodes(string nodes)
        {
            DataTable dt = null;
            //查找指定节点的人员， 如果没有节点，就是当前的节点.
            if (DataType.IsNullOrEmpty(nodes) == true)
                nodes = this.currWn.HisNode.NodeID.ToString();

            Paras ps = new Paras();
            ps.SQL = "SELECT FK_Emp,FK_Dept FROM WF_GenerWorkerList WHERE WorkID=" + dbStr + "OID AND FK_Node=" + dbStr + "FK_Node Order By RDT DESC";
            ps.Add("OID", this.WorkID);
            ps.Add("FK_Node", int.Parse(nodes));

            dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count == 0)
                throw new Exception("err@不符合常理，没有找到数据");
            string empNo = dt.Rows[0][0].ToString();
            string deptNo = dt.Rows[0][1].ToString();
            return ByDeptShipLeader_Fields(empNo, deptNo);
        }
        private DataTable ByDeptLeader_Fields(string empNo, string empDept)
        {
            string sql = "SELECT Leader FROM Port_Dept WHERE No='" + empDept + "'";
            string myEmpNo = DBAccess.RunSQLReturnStringIsNull(sql, null);

            if (DataType.IsNullOrEmpty(myEmpNo) == true)
            {
                //如果部门的负责人为空，则查找Port_Emp中的Learder信息
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    sql = "SELECT Leader FROM Port_Emp WHERE UserID='" + empNo + "' AND OrgNo='" + WebUser.OrgNo + "'";
                else
                    sql = "SELECT Leader FROM Port_Emp WHERE No='" + empNo + "'";

                myEmpNo = DBAccess.RunSQLReturnStringIsNull(sql, null);
                if (DataType.IsNullOrEmpty(myEmpNo) == true)
                {
                    Dept mydept = new Dept(empDept);
                    throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")设置的按照部门负责人计算，当前您的部门(" + mydept.No + "," + mydept.Name + ")没有维护负责人 . ");
                }
            }

            //如果有这个人,并且是当前人员，说明他本身就是经理或者部门负责人.
            if (myEmpNo.Equals(empNo) == true)
            {
                sql = "SELECT Leader FROM Port_Dept WHERE No=(SELECT PARENTNO FROM PORT_DEPT WHERE NO='" + empDept + "')";
                myEmpNo = DBAccess.RunSQLReturnStringIsNull(sql, null);
                if (DataType.IsNullOrEmpty(myEmpNo) == true)
                {
                    Dept mydept = new Dept(empDept);
                    throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")设置的按照部门负责人计算，当前您的部门(" + mydept.Name + ")上级没有维护负责人 . ");
                }
            }
            return DBAccess.RunSQLReturnTable(sql);
        }
        private DataTable ByDeptShipLeader_Fields(string empNo, string empDept)
        {
            BP.Port.Dept mydept = new BP.Port.Dept(empDept);
            Paras ps = new Paras();
            ps.Add("No", empDept);
            ps.SQL = "SELECT ShipLeader FROM Port_Dept WHERE No='" + empDept + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            if (dt.Rows.Count != 0 && dt.Rows[0][0] != null && DataType.IsNullOrEmpty(dt.Rows[0][0].ToString()) == true)
            {
                //如果部门的负责人为空，则查找Port_Emp中的Learder信息
                ps.Clear();
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    ps.SQL = "SELECT ShipLeader FROM Port_Emp WHERE UserID='" + empNo + "' AND OrgNo='" + WebUser.OrgNo + "'";
                else
                    ps.SQL = "SELECT ShipLeader FROM Port_Emp WHERE No='" + empNo + "'";

                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count != 0 && dt.Rows[0][0] != null && DataType.IsNullOrEmpty(dt.Rows[0][0].ToString()) == true)
                    throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")设置的按照部门负责人计算，当前您的部门(" + mydept.No + "," + mydept.Name + ")没有维护负责人 . ");
            }

            //如果有这个人,并且是当前人员，说明他本身就是经理或者部门负责人.
            if (dt.Rows[0][0].ToString().Equals(empNo) == true)
            {
                ps.SQL = "SELECT ShipLeader FROM Port_Dept WHERE No=(SELECT PARENTNO FROM PORT_DEPT WHERE NO='" + empDept + "')";
                dt = DBAccess.RunSQLReturnTable(ps);
                if (dt.Rows.Count == 0)
                    throw new Exception("@流程设计错误:下一个节点(" + town.HisNode.Name + ")设置的按照部门负责人计算，当前您的部门(" + mydept.Name + ")上级没有维护负责人 . ");
            }
            return dt;
        }
        /// <summary>
        /// 获得指定部门下是否有该岗位的人员.
        /// </summary>
        /// <param name="deptNo">部门编号</param>
        /// <param name="empNo">人员编号</param>
        /// <returns></returns>
        public DataTable Func_GenerWorkerList_SpecDept(string deptNo, string empNo)
        {
            string sql;

            Paras ps = new Paras();
            if (this.town.HisNode.IsExpSender == true)
            {
                /* 不允许包含当前处理人. */
                sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept AND A.FK_Emp!=" + dbStr + "FK_Emp";

                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", deptNo);
                ps.Add("FK_Emp", empNo);
            }
            else
            {
                sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B WHERE A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND A.FK_Dept=" + dbStr + "FK_Dept";

                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", deptNo);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            return dt;
        }
        /// <summary>
        /// 获得本部门的人员
        /// </summary>
        /// <param name="deptNo"></param>
        /// <param name="emp1"></param>
        /// <returns></returns>
        public DataTable Func_GenerWorkerList_SpecDept_SameLevel(string deptNo, string empNo)
        {
            string sql;

            Paras ps = new Paras();
            if (this.town.HisNode.IsExpSender == true)
            {
                /* 不允许包含当前处理人. */
                sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B, Port_Dept C WHERE A.FK_Dept=C.No AND A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND C.ParentNo=" + dbStr + "FK_Dept AND A.FK_Emp!=" + dbStr + "FK_Emp";

                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", deptNo);
                ps.Add("FK_Emp", empNo);
            }
            else
            {
                sql = "SELECT FK_Emp as No FROM Port_DeptEmpStation A, WF_NodeStation B, Port_Dept C  WHERE A.FK_Dept=C.No AND A.FK_Station=B.FK_Station AND B.FK_Node=" + dbStr + "FK_Node AND C.ParentNo=" + dbStr + "FK_Dept";
                ps.SQL = sql;
                ps.Add("FK_Node", town.HisNode.NodeID);
                ps.Add("FK_Dept", deptNo);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            return dt;
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
                                    string sql = "";
                                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                                        sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station='" + station + "'";
                                    else
                                    {
                                        sql = "SELECT FK_Emp FROM Port_DeptEmpStation WHERE FK_Station='" + station + "' AND OrgNo='" + BP.Web.WebUser.OrgNo + "'";
                                    }

                                    DataTable dt_Emps = DBAccess.RunSQLReturnTable(sql);
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
                                string[] myEmpStrs = specContent.Split(',');
                                foreach (string emp in myEmpStrs)
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
                if (this.town.HisNode.IsExpSender == true && re_dt.Rows.Count >= 2)
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
