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
    /// 测试按部门找人员
    /// </summary>
    public class FindByDept : TestBase
    {
        /// <summary>
        /// 测试按部门找人员
        /// </summary>
        public FindByDept()
        {
            this.Title = "测试按部门找人员";
            this.DescIt = "流程: 以demo 流程 064:找人规则(找领导) 为例测试。";
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
        /// 2， 测试按部门找人员的两种模式
        /// </summary>
        public override void Do()
        {
            this.fk_flow = "064";
            fl = new Flow(this.fk_flow);
            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送, 按照职务找
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, null, 6499, null);

            #region 分析预期
            if (objs.VarAcceptersID != "zhanghaicheng")
                throw new Exception("@按照职务找错误, 应当是zhanghaicheng现在是" + objs.VarAcceptersID);
            #endregion

            //执行撤销发送,按照岗位找.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workID);
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, null, 6402, null);

            #region 分析预期
            if (objs.VarAcceptersID != "zhanghaicheng")
                throw new Exception("@按照职务找错误, 应当是zhanghaicheng现在是" + objs.VarAcceptersID);
            #endregion

            //执行撤销发送,找部门所有人员.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workID);
            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, null, 6403, null);

            #region 分析预期
            if (objs.VarAcceptersID != "zhoupeng")
                throw new Exception("@按照职务找错误, 应当是zhoupeng现在是" + objs.VarAcceptersID);
            #endregion

        }
    }
}
