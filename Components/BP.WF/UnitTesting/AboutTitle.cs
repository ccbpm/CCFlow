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
    /// 关于标题
    /// </summary>
    public class AboutTitle : TestBase
    {
        /// <summary>
        /// 关于标题
        /// </summary>
        public AboutTitle()
        {
            this.Title = "关于标题";
            this.DescIt = "以测试用例的-001流程做测试用例.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行
        /// </summary>
        public override void Do()
        {
            string fk_flow = "001";
            Flow fl = new Flow(fk_flow);

            BP.WF.Dev2Interface.Port_Login("zhoutianjiao");

            //创建空白工作, 发起开始节点.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_StartWork(fk_flow, null, null, 0, null);

            Int64 workid = objs.VarWorkID;

            //用第二个人员登录.
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);

            //设置标题
            BP.WF.Dev2Interface.Flow_SetFlowTitle(fk_flow,workid,"test");

            #region 检查标题是否正确.
            string title = DBAccess.RunSQLReturnString("SELECT Title FROM WF_GenerWorkFlow  WHERE WorkID=" + workid);
            if (title != "test")
                throw new Exception("@Flow_SetFlowTitle 方法失败. WF_GenerWorkFlow没有变化正确。");
            title = DBAccess.RunSQLReturnString("SELECT Title FROM ND"+int.Parse(fl.No)+"Rpt  WHERE OID=" + workid);
            if (title != "test")
                throw new Exception("@Flow_SetFlowTitle 方法失败.Rpt table 没有变化正确。");
            #endregion 检查标题是否正确.


            //重新设置标题
            BP.WF.Dev2Interface.Flow_ReSetFlowTitle(fk_flow, objs.VarToNodeID, workid);

            #region 检查标题是否正确.
              title = DBAccess.RunSQLReturnString("SELECT Title FROM WF_GenerWorkFlow  WHERE WorkID=" + workid);
            if (title == "test")
                throw new Exception("@Flow_SetFlowTitle 方法失败. WF_GenerWorkFlow没有变化正确。");
            title = DBAccess.RunSQLReturnString("SELECT Title FROM ND" + int.Parse(fl.No) + "Rpt  WHERE OID=" + workid);
            if (title == "test")
                throw new Exception("@Flow_SetFlowTitle 方法失败.Rpt table 没有变化正确。");
            #endregion 检查标题是否正确.

        }
    }
}
