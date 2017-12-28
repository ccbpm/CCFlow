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
    /// 测试跳转规则1
    /// </summary>
    public class Skip1_13 : TestBase
    {
        /// <summary>
        /// 测试跳转规则
        /// </summary>
        public Skip1_13()
        {
            this.Title = "测试跳转规则1-13 连续跳转2个节点。";
            this.DescIt = "流程: 以demo 流程 062:测试跳转规则，连续跳转3个节点并到最后结束。";
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
        /// 1， 分别列举4种。
        /// 2， 测试找领导的两种模式
        /// </summary>
        public override void Do()
        {
            this.fk_flow = "062";
            fl = new Flow(this.fk_flow);
            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送, 应该跳转到最后一个步骤上去.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID);

            #region 分析预期
            if (objs.VarAcceptersID != null)
                throw new Exception("@应当是null现在是" + objs.VarAcceptersID);

            //if (objs.VarToNodeID != 5701)
            //    throw new Exception("@应当是开始节点，但是运行到了:" + objs.VarToNodeID + " - " + objs.VarToNodeName);
            #endregion
        }
         
    }
}
