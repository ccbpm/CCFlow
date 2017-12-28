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

namespace BP.UnitTesting.NodeAttr
{
    /// <summary>
    /// 发送参数
    /// </summary>
    public class SendPara : TestBase
    {
        /// <summary>
        /// 发送参数
        /// </summary>
        public SendPara()
        {
            this.Title = "发送参数";
            this.DescIt = "流程: 023 执行发送,产看参数是否符合要求.";
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
        /// 1, .
        /// 2, .
        /// 3，.
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建一个工作。
            this.workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);

            Hashtable ht=new Hashtable();
            ht.Add(BP.WF.WorkSysFieldAttr.SysSDTOfNode,"2020-12-01 08:00"); //下一个节点完成时间。
            ht.Add(BP.WF.WorkSysFieldAttr.SysSDTOfFlow,"2020-12-31 08:00"); //整体流程需要完成的时间。。
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, this.workID, ht, null, 0, null);

            #region 检查发送结果是否符合预期.
            //sql = "SELECT "+GenerWorkFlowAttr.SDTOfFlow+","+GenerWorkFlowAttr.SDTOfNode+" FROM WF_GenerWorkFlow WHERE WorkID="+this.workID;
            GenerWorkFlow gwf = new GenerWorkFlow(this.workID);
            if (gwf.SDTOfFlow != "2020-12-31 08:00")
                throw new Exception("@没有写入流程应完成时间，现在的应完成时间是:"+gwf.SDTOfFlow);

            if (gwf.SDTOfNode != "2020-12-01 08:00")
                throw new Exception("@没有写入节点应该完成时间，现在的应完成时间是:" + gwf.SDTOfNode);

            GenerWorkerLists gwls = new GenerWorkerLists(this.workID, 2302);
            foreach (GenerWorkerList gwl in gwls)
            {
                if (gwl.SDT != "2020-12-01 08:00")
                    throw new Exception("@没有写入节点应该应该完成时间.");
            }
            #endregion 检查发送结果是否符合预期.

        }
    }
}
