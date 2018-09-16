using System;
using System.Collections.Generic;
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
    /// 回滚一个流程
    /// </summary>
    public class RebackWorkFlow : TestBase
    {
        /// <summary>
        /// 回滚一个流程
        /// </summary>
        public RebackWorkFlow()
        {
            this.Title = "回滚一个流程";
            this.DescIt = "流程完成后，由于种种原因需要回滚它。";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行的方法
        /// </summary>
        public override void Do()
        {
            // 执行完成一个流程。
            Int64 workid = this.RunCompeleteOneWork();

            BP.WF.Dev2Interface.Port_Login("admin");

            //恢复到最后一个节点上.
            BP.WF.Dev2Interface.Flow_DoRebackWorkFlow("024", workid, 0, "test");

            #region 检查数据是否完整.
            string sql = "SELECT COUNT(*) AS N FROM WF_EmpWorks where fk_emp='zhanghaicheng' and workid=" + workid;
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该找不到 zhanghaicheng 的待办.");
            #endregion 检查数据是否完整.

            //让他向下发送.
            // BP.WF.Dev2Interface.Port_Login("zhanghaicheng");
            // BP.WF.Dev2Interface.Node_SendWork("024", workid);

            //恢复到第二个节点上去。
            workid = this.RunCompeleteOneWork();
            BP.WF.Dev2Interface.Flow_DoRebackWorkFlow("024", workid, 2402, "test");

            #region 检查数据是否完整.
            sql = "SELECT COUNT(*) AS N FROM WF_EmpWorks where fk_emp='zhoupeng' and workid=" + workid;
            dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                throw new Exception("@不应该找不到 zhoupeng 的待办.");
            #endregion 检查数据是否完整.
        }
        /// <summary>
        /// 运行完一个流程，并返回它的workid.
        /// </summary>
        /// <returns></returns>
        public Int64 RunCompeleteOneWork()
        {
            string fk_flow = "024";
            string startUser = "zhanghaicheng";
            BP.Port.Emp starterEmp = new Port.Emp(startUser);

            Flow fl = new Flow(fk_flow);

            //让zhanghaicheng登录, 在以后，就可以访问WebUser.No, WebUser.Name 。
            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //执行第二步 :  .
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //执行第三步完成:  .
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            return workid;
        }
    }
}
