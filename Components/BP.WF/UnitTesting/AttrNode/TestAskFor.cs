using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.NodeAttr
{
    /// <summary>
    /// 测试加签
    /// </summary>
    public class TestAskFor : TestBase
    {
        /// <summary>
        /// 测试加签
        /// </summary>
        public TestAskFor()
        {
            this.Title = "测试加签";
            this.DescIt = "流程: 以demo 流程023 为例测试，节点的加签，加签的发送。";
            this.EditState = EditState.Passed;
        }

        #region 全局变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        #endregion 变量

        /// <summary>
        /// 测试案例说明:
        /// 1， 以最简单的三节点流程 023 说明。
        /// 2， 测试加签的两种模式
        /// </summary>
        public override void Do()
        {
            //模式0，加签后，由加签人发送到下一步.
            this.Mode0();

            //模式1，加签后，由被加签人发送到加签人.
            this.Mode1();
        }

        public void Mode0()
        {
            this.fk_flow = "023";
            fl = new Flow("023");
            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            //让他登陆。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            //执行加签，并且直接向下发送。
            BP.WF.Dev2Interface.Node_Askfor(workID, AskforHelpSta.AfterDealSend, "liping", "askforhelp test");

            #region 检查执行挂起的预期结果.
            GenerWorkFlow gwf = new GenerWorkFlow(this.workID);
            if (gwf.WFState != WFState.Askfor)
                throw new Exception("@应当是加签的状态，现在是：" + gwf.WFStateText);

            if (gwf.FK_Node != objs.VarToNodeID)
                throw new Exception("@流程的待办节点应当是(" + objs.VarToNodeID + ")，现在是：" + gwf.FK_Node);

            // 获取当前工作列表.
            GenerWorkerLists gwls = new GenerWorkerLists(objs.VarWorkID, objs.VarToNodeID);
            if (gwls.Count != 2)
                throw new Exception("@应当有两个人员的列表，加签人与被加签人，现在是:" + gwls.Count + "个");

            string sql = "SELECT * FROM WF_GenerWorkerList WHERE FK_Emp='liping' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到被加签人的工作.");

            // 检查被加签人的状态.
            string var = dt.Rows[0][GenerWorkerListAttr.IsPass].ToString();
            if (var != "0")
                throw new Exception("@被加签人的isPass状态应该是0,显示到待办。，现在是:" + var);

            // 检查被加签人的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='liping' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到被加签人的待办工作.");
            #endregion 检查执行挂起的预期结果


            #region 检查加签人
            sql = "SELECT * FROM WF_GenerWorkerList WHERE FK_Emp='" + objs.VarAcceptersID + "' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到加签人的工作.");
            var = dt.Rows[0][GenerWorkerListAttr.IsPass].ToString();
            if (var != "5")
                throw new Exception("@被加签人的isPass状态应该是0,显示到待办。，现在是:" + var);
            #endregion 检查加签人

            //让加签人登录.
            BP.WF.Dev2Interface.Port_Login("liping");

            // 让加签人执行登录.
            SendReturnObjs objsAskFor = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            #region 检查加签人执行的结果.
            gwf = new GenerWorkFlow(this.workID);
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是的运行状态，现在是：" + gwf.WFStateText);

            if (gwf.FK_Node == objs.VarCurrNodeID)
                throw new Exception("@应当运行到下一个节点，但是现在是仍然停留在当前节点上：" + gwf.FK_Node);

            // 检查加签订人的工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='" + objs.VarAcceptersID + "' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 0)
                throw new Exception("@不应该找到，加签人的待办工作." + objs.VarAcceptersID);
            #endregion 检查加签人执行的结果.
        }

        /// <summary>
        /// 执行加签后，让被加前人，发送给加签人.
        /// </summary>
        public void Mode1()
        {
            this.fk_flow = "023";
            fl = new Flow("023");
            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            //让他登陆。
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            //执行加签，并且直接向下发送。
            BP.WF.Dev2Interface.Node_Askfor(workID, AskforHelpSta.AfterDealSendByWorker, "liping", "askforhelp test");

            #region 检查执行挂起的预期结果.
            GenerWorkFlow gwf = new GenerWorkFlow(this.workID);
            if (gwf.WFState != WFState.Askfor)
                throw new Exception("@应当是加签的状态，现在是：" + gwf.WFStateText);

            if (gwf.FK_Node != objs.VarToNodeID)
                throw new Exception("@流程的待办节点应当是(" + objs.VarToNodeID + ")，现在是：" + gwf.FK_Node);

            // 获取当前工作列表.
            GenerWorkerLists gwls = new GenerWorkerLists(objs.VarWorkID, objs.VarToNodeID);
            if (gwls.Count != 2)
                throw new Exception("@应当有两个人员的列表，加签人与被加签人，现在是:" + gwls.Count + "个");

            string sql = "SELECT * FROM WF_GenerWorkerList WHERE FK_Emp='liping' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到被加签人的工作.");

            // 检查被加签人的状态.
            string var = dt.Rows[0][GenerWorkerListAttr.IsPass].ToString();
            if (var != "0")
                throw new Exception("@被加签人的isPass状态应该是0,显示到待办。，现在是:" + var);

            // 检查被加签人的待办工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='liping' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到被加签人的待办工作.");
            #endregion 检查执行挂起的预期结果


            #region 检查加签人
            sql = "SELECT * FROM WF_GenerWorkerList WHERE FK_Emp='" + objs.VarAcceptersID + "' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count != 1)
                throw new Exception("@没有找到加签人的工作.");
            var = dt.Rows[0][GenerWorkerListAttr.IsPass].ToString();
            if (var != "6")
                throw new Exception("@加签人的isPass状态应该是6，现在是:" + var);
            #endregion 检查加签人

            //让加签人登录.
            BP.WF.Dev2Interface.Port_Login("liping");

            // 让加签人执行登录.
            SendReturnObjs objsAskFor = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            #region 检查加签人执行的结果.
            gwf = new GenerWorkFlow(this.workID);
            if (gwf.WFState != WFState.Askfor)
                throw new Exception("@应当是的加签状态，现在是:" + gwf.WFStateText);

            if (gwf.FK_Node != objsAskFor.VarToNodeID)
                throw new Exception("@不应当运行到下一个节点，现在运行到了:" + gwf.FK_Node);

            // 检查加签订人的工作.
            sql = "SELECT * FROM WF_EmpWorks WHERE FK_Emp='" + objs.VarAcceptersID + "' AND WorkID=" + workID + " AND FK_Node=" + gwf.FK_Node;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该找不到加签人的待办工作:" + objs.VarAcceptersID);
            #endregion 检查加签人执行的结果.
        }
    }
}
