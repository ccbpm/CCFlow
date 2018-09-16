using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.ReturnCase
{
    public class Test001Return : TestBase
    {
        /// <summary>
        /// 测试退回
        /// </summary>
        public Test001Return()
        {
            this.Title = "财务报销流程的退回功能";
            this.DescIt = "发送的退回，与原路返回方式的退回.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 001 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            string fk_flow = "001";
            string startUser = "zhoutianjiao";
            BP.Port.Emp starterEmp = new Port.Emp(startUser);


            Flow fl = new Flow(fk_flow);

            //让周天娇登录, 在以后，就可以访问WebUser.No, WebUser.Name 。
            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //下一个工作者.
            string nextUser = objs.VarAcceptersID;
            // 下一个发送的节点ID
            int nextNodeID = objs.VarToNodeID;

            // 让 nextUser = qifenglin 登录.
            BP.WF.Dev2Interface.Port_Login(nextUser);

            //获取第二个节点上的退回集合.
            DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(objs.VarToNodeID, workid, 0);

            #region 检查获取第二步退回的节点数据是否符合预期.
            if (dt.Rows.Count != 1)
                throw new Exception("@在第二个节点是获取退回节点集合时，不符合数据预期,应该只能获取一个退回节点，现在是:" + dt.Rows.Count);

            int nodeID = int.Parse(dt.Rows[0]["No"].ToString());
            if (nodeID != 101)
                throw new Exception("@在第二个节点是获取退回节点集合时，被退回的点应该是101");

            string RecNo = dt.Rows[0]["Rec"].ToString();
            if (RecNo != startUser)
                throw new Exception("@在第二个节点是获取退回节点集合时，被退回人应该是" + startUser + ",现在是" + RecNo);
            #endregion 检查获取第二步退回的节点数据是否符合预期.

            //在第二个节点执行退回.
            BP.WF.Dev2Interface.Node_ReturnWork(fk_flow, workid, 0, objs.VarToNodeID, 101, "退回测试", false);

            #region 检查退回后的数据完整性.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.ReturnSta)
                throw new Exception("@执行退回，流程状态应该是退回,现在是:" + gwf.WFState.ToString());

            if (gwf.FK_Node != 101)
                throw new Exception("@执行退回，当前节点应该是101, 现在是" + gwf.FK_Node.ToString());

            sql = "SELECT WFState from nd1rpt where oid=" + workid;
            int wfstate = DBAccess.RunSQLReturnValInt(sql, -1);
            if (wfstate != (int)WFState.ReturnSta)
                throw new Exception("@在第二个节点退回后rpt数据不正确，流程状态应该是退回,现在是:" + wfstate);

            //检查流程报表是否符合需求。
            sql = "SELECT * FROM " + fl.PTable + " WHERE oid=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@流程报表数据被删除了.");
            foreach (DataColumn dc in dt.Columns)
            {
                string val = dt.Rows[0][dc.ColumnName].ToString();
                switch (dc.ColumnName)
                {
                    case GERptAttr.Title:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@退回后流程标题丢失了");
                        break;
                    case GERptAttr.FID:
                        if (val != "0")
                            throw new Exception("@应当是0");
                        break;
                    case GERptAttr.FK_Dept:
                        if (val != starterEmp.FK_Dept)
                            throw new Exception("@发起人的部门发生了变化，应当是(" + starterEmp.FK_Dept + "),现在是:" + val);
                        break;
                    case GERptAttr.FK_NY:
                        if (val != DataType.CurrentYearMonth)
                            throw new Exception("@应当是" + DataType.CurrentYearMonth + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowDaySpan:
                        if (val != "0")
                            throw new Exception("@应当是 0 , 现在是:" + val);
                        break;
                    //case GERptAttr.FlowEmps:
                    //    if (val.Contains("zhanghaicheng") == false || val.Contains("zhoupeng") == false)
                    //        throw new Exception("@应当包含的人员，现在不存在, 现在是:" + val);
                    //    break;
                    //case GERptAttr.FlowEnder:
                    //    if (val != "zhanghaicheng")
                    //        throw new Exception("@应当是 zhanghaicheng , 现在是:" + val);
                    //    break;
                    case GERptAttr.FlowEnderRDT:
                        if (val.Contains(DataType.CurrentData) == false)
                            throw new Exception("@应当是 当前日期, 现在是:" + val);
                        break;
                    case GERptAttr.FlowEndNode:
                        if (val != "101")
                            throw new Exception("@应当是 101, 现在是:" + val);
                        break;
                    case GERptAttr.FlowStarter:
                        if (val != startUser)
                            throw new Exception("@应当是 " + startUser + ", 现在是:" + val);
                        break;
                    case GERptAttr.FlowStartRDT:
                        if (DataType.IsNullOrEmpty(val))
                            throw new Exception("@应当不能为空,现在是:" + val);
                        break;
                    case GERptAttr.WFState:
                        if (int.Parse(val) != (int)WFState.ReturnSta)
                            throw new Exception("@应当是  WFState.Complete 现在是" + val);
                        break;
                    default:
                        break;
                }
            }
            #endregion 检查退回后的数据完整性.

            //让发起人登录，并发送到部门经理审批.
            BP.WF.Dev2Interface.Port_Login(startUser);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //让第二步骤的qifengin登录并处理它，发送到总经理审批。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            Hashtable ht = new Hashtable();
            ht.Add("HeJiFeiYong", 999999); //金额大于1w 就让它发送到总经理审批节点上去.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);

            // 让zhoupeng登录, 并执行退回.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            //获取第三个节点上的退回集合.
            dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(objs.VarToNodeID, workid, 0);

            #region 检查获取第二步退回的节点数据是否符合预期.
            if (dt.Rows.Count != 2)
                throw new Exception("@在第三个节点是获取退回节点集合时，不符合数据预期,应该 获取2个退回节点，现在是:" + dt.Rows.Count);

            bool isHave101 = false;
            bool isHave102 = false;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr[0].ToString() == "101")
                    isHave101 = true;

                if (dr[0].ToString() == "102")
                    isHave102 = true;
            }

            if (isHave101==false || isHave102==false)
                 throw new Exception("@获得将要可以退回的节点集合错误,缺少101，或者102节点.");

            //if (dt.Rows[0][0].ToString() != "101")
            //    throw new Exception("@应该是101,现在是:" + dt.Rows[0][0].ToString());
            //if (dt.Rows[1][0].ToString() != "102")
            //    throw new Exception("@应该是102,现在是:" + dt.Rows[0][0].ToString());

            #endregion 检查获取第二步退回的节点数据是否符合预期.

            //在第3个节点执行退回.
            BP.WF.Dev2Interface.Node_ReturnWork(fk_flow, workid, 0, objs.VarToNodeID, 101, "总经理-退回测试", false);

            #region 检查总经理-退回后的数据完整性.
            gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.ReturnSta)
                throw new Exception("@执行退回，流程状态应该是退回,现在是:" + gwf.WFState.ToString());

            if (gwf.FK_Node != 101)
                throw new Exception("@执行退回，当前节点应该是101, 现在是" + gwf.FK_Node.ToString());
            #endregion 检查退回后的数据完整性.

            // 让发起人登录, 并执行发送.
            BP.WF.Dev2Interface.Port_Login(startUser);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            // 让部门经理登录登录, 并执行发送.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            ht = new Hashtable();
            ht.Add("HeJiFeiYong", 999999);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht,null,103,"zhoupeng");

            //让总经理登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //执行退回并原路返回.
            BP.WF.Dev2Interface.Node_ReturnWork(fk_flow, workid, 0, objs.VarToNodeID, 101, "总经理-退回并原路返回-测试", true);

            //让发起人登录, 此人在发起时，应该直接发送给第三个节点的退回人,就是zhoupeng才正确.
            BP.WF.Dev2Interface.Port_Login(startUser);
            SendReturnObjs objsNew = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            #region 开始检查数据是否完整.
            if (objsNew.VarAcceptersID != "zhoupeng")
                throw new Exception("@退回并原路返回错误，应该发送给 zhoupeng，但是目前发送到了:" + objsNew.VarAcceptersID);

            if (objsNew.VarToNodeID != 103)
                throw new Exception("@退回并原路返回错误，应该发送给103，但是目前发送到了:" + objsNew.VarToNodeID);
            #endregion
        }
    }
}
