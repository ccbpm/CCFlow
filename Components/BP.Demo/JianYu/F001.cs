using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.JianYu
{
    /// <summary>
    /// 报销流程001
    /// 此类库必须放入到 BP.*.dll 才能被解析发射出来。
    /// </summary>
    public class F001 : BP.WF.FlowEventBase
    {
        #region 属性.
        /// <summary>
        /// 重写流程标记
        /// </summary>
        public override string FlowMark
        {
            get { return ",001,"; }
        }
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F001()
        {
        }
        #endregion 属性.

        #region 发送事件.
        /// <summary>
        /// 重写发送前事件
        /// </summary>
        /// <returns></returns>
        public override string SendWhen()
        {

            //  string deptNo = WebUser.FK_Dept.Substring(0, 4);
            //  string sql = "SELECT No, Name FROM Port_Emp A, Port_DeptEmpStation B WHERE A.NO=B.FK_Emp AND B.FK_Station='98' AND A.FK_Dept LIKE '" + deptNo + "%'";
            //  DataTable dt = DBAccess.RunSQLReturnTable(sql);

            //   this.JumpToEmps = "";
            //  this.JumpToNodeID = 102;

            //相关的变量,

            // 当前的节点, 其他的变量请从 this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int类型的ID.
            string nodeName = this.HisNode.Name; // 当前节点名称.
            switch (nodeID)
            {
                case 103:  //判断是否全部合规,不合格就抛出异常..
                    //string sql = "SELECT COUNT(*) FORM ND101Dtl1 WHERE RefPK=" + this.WorkID + " AND WorkSta!= 3 ";
                    //var num = DBAccess.RunSQLReturnValInt(sql);
                    //if (num != 0)
                    //    throw new Exception("err@该项目有【" + num + "】没有检查合格，或者没有完成，您不能发送。");
                    break;
                default:
                    break;
            }
            return null;
        }
        #endregion 发送事件.

        /// <summary>
        /// 发送成功事件，发送成功时，把流程的待办写入其他系统里.
        /// </summary>
        /// <returns>返回执行结果，如果返回null就不提示。</returns>
        public override string SendSuccess()
        {
            try
            {
                // 组织必要的变量.
                Int64 workid = this.WorkID; // 工作id.
                string flowNo = this.HisNode.FK_Flow; // 流程编号.
                int currNodeID = this.SendReturnObjs.VarCurrNodeID; //当前节点id
                int toNodeID = this.SendReturnObjs.VarToNodeID;     //到达节点id.
                string toNodeName = this.SendReturnObjs.VarToNodeName; // 到达节点名称。
                string acceptersID = this.SendReturnObjs.VarAcceptersID; // 接受人员id, 多个人员会用 逗号分看 ,比如 zhangsan,lisi。
                string acceptersName = this.SendReturnObjs.VarAcceptersName; // 接受人员名称，多个人员会用逗号分开比如:张三,李四.

                //执行向其他系统写入待办.
                /*
                 * 在这里需要编写你的业务逻辑，根据上面组织的变量.
                 */

                if (this.HisNode.NodeID == 102)
                {
                    /* 根据不同的节点，执行不同的业务逻辑 */
                }

                //返回.
                return base.SendSuccess();
            }
            catch (Exception ex)
            {
                return base.SendSuccess();

                // throw new Exception("向其他系统写入待办失败，详细信息："+ex.Message);
            }
        }

    }
}
