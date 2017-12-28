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

namespace BP.UnitTesting.NodeAttr
{
    /// <summary>
    /// 测试 按URL 做为转向条件
    /// </summary>
    public class TurnByURL : TestBase
    {
        /// <summary>
        /// 测试 按URL 做为转向条件
        /// </summary>
        public TurnByURL()
        {
            this.Title = "测试 按URL 做为转向条件";
            this.DescIt = "以测试用例的-075流程做测试用例.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Do()
        {
            string fk_flow = "075";
            Flow fl = new Flow(fk_flow);

            #region   zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoushengyu");
            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //加入 按URL ,表单里没有TurnTo字段.
            Hashtable ht = new Hashtable();
            ht.Add("ToNodeID", "7502");
            SendReturnObjs objs = null;

            //执行发送，并获取发送对象, 应该出来异常才对，因为传递这个参数会导致sql出错误。
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);

            if (objs.VarToNodeID != 7502)
                throw new Exception("@应该转向7502 ");

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);

            //测试流程完成条件.
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            ht = new Hashtable();
            ht.Add("ToNodeID", "");
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            if (objs.IsStopFlow == false)
                throw new Exception("@流程应该结束，但是未结束。");
            #endregion
        }
    }
}
