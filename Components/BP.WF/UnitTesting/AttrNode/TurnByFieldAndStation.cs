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
    public class TurnByField : TestBase
    {
        /// <summary>
        /// 测试岗位方向条件
        /// </summary>
        public TurnByField()
        {
            this.Title = "测试岗位方向条件与表单字段的方向条件";
            this.DescIt = "以 002请假流程(按岗位控制走向)为测试对象.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 002 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            string fk_flow = "002";
            Flow fl = new Flow(fk_flow);

            #region   zhoutianjiao 登录. 基层路线
            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarToNodeID != 299)
                throw new Exception("@按照岗位做方向条件错误，基层人员没有发送到[部门经理审批]。");
            #endregion

            #region 测试表单字段的方向条件.
            if (objs.VarAcceptersID != "qifenglin")
                throw new Exception("@没有让他的部门经理审批不是所期望的值.");

            //按照他的部门经理登录.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            //建立表单参数.
            Hashtable ht = new Hashtable();
            ht.Add("QingJiaTianShu", 8);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            if (objs.VarToNodeID != 204)
                throw new Exception("@按表单字段控制方向错误，小于10天的应该让人办资源部门审批。");

            // 撤销发送.
            BP.WF.Dev2Interface.Flow_DoUnSend(fl.No, workid);

            // 按照请假15天发送.
            ht = new Hashtable();
            ht.Add("QingJiaTianShu", 15);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            if (objs.VarToNodeID != 206)
                throw new Exception("@按表单字段控制方向错误，大于等于10天的应该让[总经理审批]");

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion

            #region 让中层 qifenglin登录.
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarToNodeID != 202)
                throw new Exception("@按照岗位做方向条件错误，中层人员没有发送到[总经理审批]节点。");
            // 删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion 让中层 qifenglin登录.

            #region 让高层 zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarToNodeID != 203)
                throw new Exception("@按照岗位做方向条件错误，高层人员没有发送到[人力资源审批]节点。");
            //delete it.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion
        }
    }
}
