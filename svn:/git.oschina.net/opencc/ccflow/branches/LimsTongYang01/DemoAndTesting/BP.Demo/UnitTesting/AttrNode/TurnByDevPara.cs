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
    /// 测试开发者参数做为转向条件
    /// </summary>
    public class TurnByDevPara : TestBase
    {
        /// <summary>
        /// 测试开发者参数做为转向条件
        /// </summary>
        public TurnByDevPara()
        {
            this.Title = "测试开发者参数做为转向条件";
            this.DescIt = "以测试用例的-029流程做为开发者参数.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Do()
        {
            string fk_flow = "029";
            Flow fl = new Flow(fk_flow);
            fl.DoCheck();

            #region   zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            //加入开发者参数,表单里没有TurnTo字段.
            Hashtable ht = new Hashtable();
            ht.Add("Turn", "A");

            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            if (objs.VarToNodeID != 2999)
                throw new Exception("@应该转向A。");

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion

            #region   zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            //创建空白工作, 发起开始节点.
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            //加入开发者参数,表单里没有TurnTo字段.
            ht = new Hashtable();
            ht.Add("Turn", "B");

            //执行发送，并获取发送对象,.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            if (objs.VarToNodeID != 2902)
                throw new Exception("@应该转向B。");

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion
        }
    }
}
