using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;

namespace BP.UnitTesting
{
    /// <summary>
    /// 子流程的回滚
    /// </summary>
    public class SubFlowReback : TestBase
    {
        /// <summary>
        /// 子流程的回滚
        /// </summary>
        public SubFlowReback()
        {
            this.Title = "子流程的回滚";
            this.DescIt = "以023,024流程来测试子流程的回滚";
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
        public Int64 workid = 0;
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
        /// 子流程的回滚:
        /// 1, 把023做为父流程，024做为子流程。
        /// 2, 在父流程吊起子流程后，子流程完成，让其回滚，检查数据是否符合预期。
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "005";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);

            // 让 zhanghaicheng  登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            // 创建.
            this.workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.fk_flow, null, null, WebUser.No, "parent flow");

            //发送到下一步骤,还是让zhoupeng 处理.
            BP.WF.Dev2Interface.Node_SendWork(this.fk_flow, this.workid, null, null, 0, "zhoupeng");

            //创建一个子流程,让 zhoushengyu 做为发起人.
            Int64 subWorkID = BP.WF.Dev2Interface.Node_CreateStartNodeWork("024", null, null, "zhoushengyu", "sub flow", this.workid, this.fk_flow,0);

            // 让子流程执行到结束.
            LetSubFlowRunOver(subWorkID);

            // 让 zhoupeng  登录,开始监控子流程.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //执行回滚到最后一个节点上.
            BP.WF.Dev2Interface.Flow_DoRebackWorkFlow("024", subWorkID, 2401, "test reback");

            // 在让子流程执行到结束.
            LetSubFlowRunOver(subWorkID);


            //重复一次，检查是否有问题？ 在执行回滚到最后一个节点上.
            BP.WF.Dev2Interface.Flow_DoRebackWorkFlow("024", subWorkID, 2401, "test reback");

            // 在让子流程执行到结束.
            LetSubFlowRunOver(subWorkID);

        }
        /// <summary>
        /// 让子流程运行到结束.
        /// </summary>
        public void LetSubFlowRunOver(Int64 subWorkID)
        {
            #region 子流程第一个节点的操作人员.
            // 让 zhoushengyu 登录.
            BP.WF.Dev2Interface.Port_Login("zhoushengyu");

            //发送这个子流程,到 zhangyifan， 发送到第二个节点上去.
            BP.WF.Dev2Interface.Node_SendWork("024", subWorkID, null, null, 0, "zhangyifan");
            #endregion 子流程第一个节点的操作人员.


            #region 子流程第二个节点的操作人员.
            // 让 zhangyifan 登录.
            BP.WF.Dev2Interface.Port_Login("zhangyifan");

            //发送这个子流程,到 zhoutianjiao.
            BP.WF.Dev2Interface.Node_SendWork("024", subWorkID, null, null, 0, "zhoutianjiao");
            #endregion 子流程第二个节点的操作人员.


            #region 子流程第三个节点的操作人员,现在子流程完成.
            // 让 zhoutianjiao 登录.
            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");

            //发送这个子流程,到 zhoutianjiao.
            BP.WF.Dev2Interface.Node_SendWork("024", subWorkID, null, null, 0, "zhoutianjiao");
            #endregion 子流程第三个节点的操作人员,现在子流程完成.
        }
    }
}
