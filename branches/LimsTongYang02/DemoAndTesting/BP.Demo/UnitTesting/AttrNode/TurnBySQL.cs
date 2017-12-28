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
    /// 测试 按SQL 做为转向条件
    /// </summary>
    public class TurnBySQL : TestBase
    {
        /// <summary>
        /// 测试 按SQL 做为转向条件
        /// </summary>
        public TurnBySQL()
        {
            this.Title = "测试 按SQL 做为转向条件";
            this.DescIt = "以测试用例的-030流程做测试用例.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Do()
        {
            string fk_flow = "030";
            Flow fl = new Flow(fk_flow);

            #region   zhoupeng 登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //加入 按SQL ,表单里没有TurnTo字段.
            Hashtable ht = new Hashtable();
            ht.Add("MyPara", "qqqq");
            SendReturnObjs objs = null;
            try
            {
                //执行发送，并获取发送对象, 应该出来异常才对，因为传递这个参数会导致sql出错误。
                objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineInfo("已经检测到SQL的变量已经被正确的替换了,导致此语句执行失败:" + ex.Message);
            }
            ht.Clear();
            ht.Add("MyPara", "1");
            //执行发送，并获取发送对象, 应该出来异常才对，因为传递这个参数会导致sql出错误。
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid, ht);
            if (objs.VarToNodeID != 3002)
                throw new Exception("@应该转向B。");

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, objs.VarWorkID, false);
            #endregion
        }
    }
}
