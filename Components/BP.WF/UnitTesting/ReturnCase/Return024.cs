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
    public class Return024 : TestBase
    {
        /// <summary>
        /// 测试退回
        /// </summary>
        public Return024()
        {
            this.Title = "数据覆盖模式的退回";
            this.DescIt = "发送的退回，与原路返回方式的退回.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 001 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            this.Test1();
        }
        public void Test1()
        {
            string fk_flow = "024";
            string startUser = "zhanghaicheng";
            BP.Port.Emp starterEmp = new Port.Emp(startUser);

            Flow fl = new Flow(fk_flow);

            //让zhanghaicheng登录, 在以后，就可以访问WebUser.No, WebUser.Name 。
            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //下一个工作者.
            string nextUser = objs.VarAcceptersID;

            // 下一个发送的节点ID
            int nextNodeID = objs.VarToNodeID;

            // 让 nextUser = zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login(nextUser);

            //获取第二个节点上的退回集合.
            DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(objs.VarToNodeID, workid, 0);

            #region 检查获取第二步退回的节点数据是否符合预期.
            if (dt.Rows.Count != 1)
                throw new Exception("@在第二个节点是获取退回节点集合时，不符合数据预期,应该只能获取一个退回节点，现在是:" + dt.Rows.Count);

            int nodeID = int.Parse(dt.Rows[0]["No"].ToString());
            if (nodeID != 2401)
                throw new Exception("@在第二个节点是获取退回节点集合时，被退回的点应该是2401");

            string RecNo = dt.Rows[0]["Rec"].ToString();
            if (RecNo != startUser)
                throw new Exception("@在第二个节点是获取退回节点集合时，被退回人应该是" + startUser + ",现在是" + RecNo);
            #endregion 检查获取第二步退回的节点数据是否符合预期.

            //在第二个节点执行退回.
            BP.WF.Dev2Interface.Node_ReturnWork(fk_flow, workid, 0,
                objs.VarToNodeID, 2401, "退回测试", false);

            #region 检查退回后的数据完整性.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.ReturnSta)
                throw new Exception("@执行退回，流程状态应该是退回,现在是:" + gwf.WFState.ToString());

            if (gwf.FK_Node != 2401)
                throw new Exception("@执行退回，当前节点应该是2401, 现在是" + gwf.FK_Node.ToString());

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
                        if (val != "2401")
                            throw new Exception("@应当是 2401, 现在是:" + val);
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
        }
    }
}
