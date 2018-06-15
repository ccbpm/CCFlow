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
using BP.WF.Template;

namespace BP.UnitTesting.NodeAttr
{
    /// <summary>
    /// 测试挂起
    /// </summary>
    public class TestHungUp : TestBase
    {
        /// <summary>
        /// 测试挂起
        /// </summary>
        public TestHungUp()
        {
            this.Title = "测试挂起";
            this.DescIt = "流程: 以demo 流程023 为例测试，流程的挂起，解除挂起。";
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
        /// 1, 此流程针对于最简单的分合流程进行， zhanghaicheng发起，zhoushengyu,zhangyifan,两个人处理子线程，
        ///    zhanghaicheng 接受子线程汇总数据.
        /// 2, 测试方法体分成三大部分. 发起，子线程处理，合流点执行，分别对应: Step1(), Step2_1(), Step2_2()，Step3() 方法。
        /// 3，针对发送测试，不涉及到其它的功能.
        /// </summary>
        public override void Do()
        {
            HungUp huEn = new HungUp();
            huEn.CheckPhysicsTable();

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

            //执行挂起。
            BP.WF.Dev2Interface.Node_HungUpWork(fl.No, workID, 0, null, "hungup test");

            #region 检查执行挂起的预期结果.
            GenerWorkFlow gwf = new GenerWorkFlow(this.workID);
            if (gwf.WFState != WFState.HungUp)
                throw new Exception("@应当是挂起的状态，现在是：" + gwf.WFStateText);

            GenerWorkerLists gwls = new GenerWorkerLists(workID, this.fk_flow);
            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.FK_Node == fl.StartNodeID)
                    continue;

                if (gwl.DTOfHungUp.Length < 10)
                    throw new Exception("@挂起日期没有写入");

                if (DataType.IsNullOrEmpty(gwl.DTOfUnHungUp) == false)
                    throw new Exception("@解除挂起日期应当为空,现在是：" + gwl.DTOfUnHungUp);

                if (gwl.HungUpTimes != 1)
                    throw new Exception("@挂起次数应当为１");
            }

            HungUp hu = new HungUp();
            hu.MyPK = "2302_" + this.workID;
            if (hu.RetrieveFromDBSources() == 0)
                throw new Exception("@没有找到　HungUp　数据。");

            #endregion 检查执行挂起的预期结果

            //解除挂起。
            BP.WF.Dev2Interface.Node_UnHungUpWork(fl.No, workID, "un hungup test");

            #region 检查接触执行挂起的预期结果.
            gwf = new GenerWorkFlow(this.workID);
            if (gwf.WFState != WFState.Runing)
                throw new Exception("@应当是挂起的状态，现在是：" + gwf.WFStateText);
            #endregion 检查接触执行挂起的预期结果

            //执行多次挂起于解除挂起.
            BP.WF.Dev2Interface.Node_HungUpWork(fl.No, workID, 0, null, "hungup test");
            BP.WF.Dev2Interface.Node_UnHungUpWork(fl.No, workID, "un hungup test");
        }
    }
}