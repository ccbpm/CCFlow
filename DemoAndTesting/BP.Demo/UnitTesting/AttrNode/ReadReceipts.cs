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
    /// 已读回执
    /// </summary>
    public class ReadReceipts : TestBase
    {
        /// <summary>
        /// 已读回执
        /// </summary>
        public ReadReceipts()
        {
            this.Title = "已读回执";
            this.DescIt = "036 已读回执-请假流程 ";
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
        /// 测试案例 
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "036";
            userNo = "zhangyifan";
            fl = new Flow(fk_flow);

            //执行登陆.
            BP.WF.Dev2Interface.Port_Login(userNo);

            // 创建工作。
            this.workID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.fk_flow, null, null, null, null);
            // 发送到下一步骤(部门经理审批).
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.fk_flow, this.workID, null,null, 0, null);

            // 获取下一步骤的接收人员，让下一步人员登陆.
            string nextWorker2 = objs.VarAcceptersID;
            BP.WF.Dev2Interface.Port_Login(nextWorker2);

            // 执行部门经理读取工作的api.
            BP.WF.Dev2Interface.Node_SetWorkRead(objs.VarToNodeID, this.workID);

            #region 检查发起人是否接受到了回执消息.
            if (BP.WF.Glo.IsEnableSysMessage == true)
            {
                string sql = "SELECT * FROM Sys_SMS WHERE MsgFlag='RP" + this.workID + "_3602' AND " + SMSAttr.SendTo + "='" + userNo + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@人员(" + userNo + ")没有接受到回执消息。");
            }
            #endregion 检查发起人是否接受到了回执消息.

            // 让第2步的人员发送， 并获取发送对象.
            Hashtable ht = new Hashtable();
            ht.Add(BP.WF.WorkSysFieldAttr.SysIsReadReceipts, "1"); //传入表单参数.
            objs = BP.WF.Dev2Interface.Node_SendWork(this.fk_flow, this.workID,
                ht, null, 0, null);

            string nextWorker3 = objs.VarAcceptersID; //获得第三步骤地接受者。
            BP.WF.Dev2Interface.Port_Login(nextWorker3); // 让第三步骤地工作人员登陆。
            BP.WF.Dev2Interface.Node_SetWorkRead(objs.VarToNodeID, this.workID); //执行读取回执。

            #region 检查部门经理是否接受了已读回执.
            if (BP.WF.Glo.IsEnableSysMessage == true)
            {
                string sql = "SELECT * FROM Sys_SMS WHERE MsgFlag='RP" + this.workID + "_3699' AND " + SMSAttr.SendTo + "='" + nextWorker2 + "'";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("@人员(" + nextWorker2 + ")没有接受到回执消息。");
            }
            #endregion 检查部门经理是否接受了已读回执.
        }
    }
}
