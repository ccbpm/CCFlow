using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.ReturnCase
{
    public class ReturnByDesigner : TestBase
    {
        /// <summary>
        /// 测试按轨迹退回
        /// </summary>
        public ReturnByDesigner()
        {
            this.Title = "测试按轨迹退回";
            this.DescIt = "对应测试用例 031流程-按轨迹退回";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Do()
        {
            string fk_flow = "031";
            string startUser = "zhangyifan";

            Flow fl = new Flow(fk_flow);

            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            // 让 下一个工作者登录.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            //让第二个节点执行发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@此节点的执行人员应该是zhoupeng.");

            // 让 第三个工作者登录.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);


            //获取第三个节点上的退回集合.
            DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes(objs.VarToNodeID, workid, 0);

            #region 检查获取第二步退回的节点数据是否符合预期.
            if (dt.Rows.Count != 1)
                throw new Exception("@在第3个节点是获取退回节点集合时，不符合数据预期,应该只能获取一个退回节点，现在是:" + dt.Rows.Count);

            int nodeID = int.Parse(dt.Rows[0]["No"].ToString());
            if (nodeID != 3101)
                throw new Exception("@在第3个节点是获取退回节点集合时，被退回的点应该是3101");
            string RecNo = dt.Rows[0]["Rec"].ToString();
            if (RecNo != startUser)
                throw new Exception("@在第3个节点是获取退回节点集合时，被退回人应该是" + startUser + ",现在是" + RecNo);
            #endregion 检查获取第二步退回的节点数据是否符合预期.

            //执行退回,当前节点编号.
            BP.WF.Dev2Interface.Node_ReturnWork(fk_flow, workid, 0, objs.VarToNodeID,3101, "按轨迹退回测试", false);

            #region 检查退回后的数据完整性.
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            if (gwf.WFState != WFState.ReturnSta)
                throw new Exception("@执行退回，流程状态应该是退回,现在是:" + gwf.WFState.ToString());

            if (gwf.FK_Node != 3101)
                throw new Exception("@执行退回，当前节点应该是101, 现在是" + gwf.FK_Node.ToString());

            sql = "SELECT WFState from nd31rpt where oid=" + workid;
            int wfstate = DBAccess.RunSQLReturnValInt(sql, -1);
            if (wfstate != (int)WFState.ReturnSta)
                throw new Exception("@在第3个节点退回后rpt数据不正确，流程状态应该是退回,现在是:" + wfstate);

            sql = "SELECT FlowEndNode from nd31rpt where oid=" + workid;
            int FlowEndNode = DBAccess.RunSQLReturnValInt(sql, -1);
            if (FlowEndNode != 3101)
                throw new Exception("@在第3个节点退回后rpt数据不正确，最后的节点应该是101,现在是:" + FlowEndNode);
            #endregion 检查退回后的数据完整性.


            //删除此测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);

        }
    }
}
