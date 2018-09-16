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
using BP.UnitTesting;


namespace BP.UnitTesting.AttrFlow
{
    /// <summary>
    /// 生成WorkID
    /// </summary>
    public  class GeneWorkID : TestBase
    {
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
        /// 生成WorkID
        /// </summary>
        public GeneWorkID()
        {
            this.Title = "检查生成WorkID是否重复";
            this.DescIt = "有几次出现生成WorkID重复的现象.";
            this.EditState = EditState.Passed; 
        }
        /// <summary>
        /// 过程说明：
        /// 1，以流程 023最简单的3节点(轨迹模式)， 为测试用例。
        /// 2，仅仅测试发送功能，与检查发送后的数据是否完整.
        /// 3, 此测试有三个节点发起点、中间点、结束点，对应三个测试方法。
        /// </summary>
        public override void Do()
        {
            #region 定义变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
            #endregion 定义变量.

            //让 userNo 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            // 创建空白工作, 在标题为空的情况下.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            #region 检查数据是否符合预期.
            #endregion 检查数据是否符合预期

            //在执行一次创建.
            Int64 workID2 = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            if (workID != workID2)
                throw new Exception("应该两次生成的WorkID相同， 但是现在不同.");

             //执行发送.
             BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID);

             //在执行一次创建.
             workID2 = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

               if (workID == workID2)
                   throw new Exception("应该两次生成的WorkID不相同， 但是现在相同.");
             
            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, workID, false);
        }
    }
}
