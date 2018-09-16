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
    /// 按部门的方向条件转向
    /// </summary>
    public class TurnByDept : TestBase
    {
        /// <summary>
        /// 测试岗位方向条件
        /// </summary>
        public TurnByDept()
        {
            this.Title = "测试岗位、部门、表单字段的方向条件";
            this.DescIt = "以测试用例的-028部门方向条件做为测试用例.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 028 流程编写的单元测试代码。
        /// 涉及到了: 多种方式的方向转向功能是否可用。
        /// </summary>
        public override void Do()
        {
            string fk_flow = "028";
            Flow fl = new Flow(fk_flow);

            #region   yangyilei 登录. 基层路线
            BP.WF.Dev2Interface.Port_Login("yangyilei");
            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarToNodeID != 2802)
                throw new Exception("@财务部门人员发起没有转入到财务部节点上去。");

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion yangyilei

            #region  qifenglin登录.
            BP.WF.Dev2Interface.Port_Login("qifenglin");
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workid);
            if (objs.VarToNodeID != 2899)
                throw new Exception("@研发部人员发起没有转入研发部节点上去。");
            // 删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion  qifenglin登录.

            #region  liyan登录.
            BP.WF.Dev2Interface.Port_Login("liyan");
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workid);
            if (objs.VarToNodeID != 2803)
                throw new Exception("@没有转入人力资源部上去。");
            // 删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion liyan登录.
        }
    }
}
