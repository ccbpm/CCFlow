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
    public class HeLiuNodeReturn : TestBase
    {
        /// <summary>
        /// 测试合流节点退回
        /// </summary>
        public HeLiuNodeReturn()
        {
            this.Title = "测试合流节点向子线程退回";
            this.DescIt = "以demo中的 FlowNo=005 月销售总结(同表单分合流), 为测试案例.";
            this.EditState = EditState.Passed;
        }

        #region 变量
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
        /// 说明 ：以demo中的 FlowNo=005 月销售总结(同表单分合流), 为测试案例。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            //BP.WF.ClearDB cd = new ClearDB();
            //cd.Do();

            // 给全局变量赋值.
            fk_flow = "005";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);

            // 让发起人登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作, 发起开始节点.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行发送，并获取发送对象.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID);

            #region 发送子线程 zhangyifan
            // 合流点发送后，子线程点执行发送.
            BP.WF.Dev2Interface.Port_Login("zhangyifan");

            //获得子线程ID.
            Int64 threadWorkID1 = this.GetThreadID(workID);

            // 执行发送, 让他发送到合流节点上去.
            BP.WF.Dev2Interface.Node_SendWork(fk_flow, threadWorkID1);
            #endregion 发送子线程 zhangyifan

            #region 发送子线程 zhoushengyu
            // 合流点发送后，子线程点执行发送.
            BP.WF.Dev2Interface.Port_Login("zhoushengyu");

            //获得子线程ID.
            Int64 threadWorkID2 = this.GetThreadID(workID);

            // 执行发送, 让他发送到合流节点上去.
            BP.WF.Dev2Interface.Node_SendWork(fk_flow, threadWorkID2);
            #endregion 发送子线程 zhoushengyu

            // 让发起人登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            // 执行退回子线程1.
            BP.WF.Dev2Interface.Node_ReturnWork(fl.No, threadWorkID1, workID, 599, 502, "test msg1", false);

            #region 检查子线程数据是否正确？
            gwf = new GenerWorkFlow(threadWorkID1);
            if (gwf.FK_Node != 502)
                throw new Exception("@子线程的退回节点与预期不符合，现在是:"+gwf.FK_Node);

            if (gwf.FID != workID)
                throw new Exception("@子线程的退回节点后 WF_GenerWorkFlow 上的 FID 丢失，现在是:" + gwf.FID);

            gwl = new GenerWorkerList(threadWorkID1, 502, "zhoushengyu");
            if (gwl.IsPass == true)
                throw new Exception("@子线程不应该是已通过的状态.");
            #endregion 检查子线程数据是否正确？

            // 执行退回子线程2.
            BP.WF.Dev2Interface.Node_ReturnWork(fl.No, threadWorkID2, workID, 599, 502, "test msg2", false);

            #region 检查子线程数据是否正确？
            gwf = new GenerWorkFlow(threadWorkID2);
            if (gwf.FK_Node != 502)
                throw new Exception("@子线程的退回节点与预期不符合，现在是:" + gwf.FK_Node);

            if (gwf.FID != workID)
                throw new Exception("@子线程的退回节点后 WF_GenerWorkFlow 上的 FID 丢失，现在是:" + gwf.FID);

            gwl = new GenerWorkerList(threadWorkID1, 502, "zhangyifan");
            if (gwl.IsPass == true)
                throw new Exception("@子线程不应该是已通过的状态.");
            #endregion 检查子线程数据是否正确？
        }

        private Int64 GetThreadID(Int64 workID)
        {
            //获取它的待办, 从而获取子线程id.
            var dt = Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, fk_flow);
            foreach (DataRow dr in dt.Rows)
            {
                Int64 fid = Int64.Parse(dr["FID"].ToString());
                if (fid != workID)
                    continue;

                return Int64.Parse(dr["WorkID"].ToString());
            }
            throw new Exception("@没有找到.");
        }
    }
}
