using System;
using System.Threading;
using System.Collections;

using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.MES
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
            //相关的变量,

            // 当前的节点, 其他的变量请从 this.HisNode .
            int nodeID = this.HisNode.NodeID;    // int类型的ID.
            string nodeName = this.HisNode.Name; // 当前节点名称.
            switch (nodeID)
            {
                case 102:  //判断是否全部合规,不合格就抛出异常..
                    string sql = "SELECT COUNT(*) FROM ND101Dtl1 WHERE RefPK=" + this.WorkID + " AND  WorkSta!= 1 AND  WorkSta!=4  ";
                  
                   DBAccess.RunSQL("UPDATE ND201Dtl1 SET XTSta=1    WHERE REFPK=(SELECT PWORKID FROM ND1Rpt WHERE OID=" + this.WorkID +")  AND XiangTiMingCheng = (SELECT XiangTiMingCheng FROM ND1Rpt WHERE OID = "+ this.WorkID + ")");

                    var num = DBAccess.RunSQLReturnValInt(sql);
                    if (num != 0)
                        throw new Exception("err@该项目有【" + num + "】没有检查合格，或者没有完成，您不能发送。");
                    break;
                case 103:

                    // 0=未处理. 1=已完成. 2=重做. 3=检查合格. 4=完成待检查. 5=复检合格 
                    /* 1. 把所有 WorkSta=4的 修改未5  === 让完成待检查的修改未，复检合格.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=4 AND RefPK=" + this.WorkID);

                    /* 2. 把所有 WorkSta=2 的 修改未5 === 让重做的，修改未复检合格 */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=2 AND RefPK=" + this.WorkID);

                    /* 3. 把所有 WorkSta=1 的 修改未3  ====  让已完成的，设置未检查合格.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=1 AND RefPK=" + this.WorkID);


                    //判断是否全部合规,不合格就抛出异常..
                    string sql3 = "SELECT COUNT(*) FROM ND101Dtl1 WHERE RefPK=" + this.WorkID + " AND WorkSta!= 3 ";
                    var num3 = DBAccess.RunSQLReturnValInt(sql3);
                    if (num3 != 0)
                        throw new Exception("err@该项目有【" + num3 + "】没有检查合格，或者没有完成，您不能发送。");
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
                Int64 workid = this.WorkID; // 工作id.w
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

                //如果检查节点，向下发送.
                if (this.HisNode.NodeID == 103)
                {
                    // 0=未处理. 1=已完成. 2=重做. 3=检查合格. 4=完成待检查. 5=复检合格 
                    /* 1. 把所有 WorkSta=4的 修改未5  === 让完成待检查的修改未，复检合格.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=4 AND RefPK="+this.WorkID);

                    /* 2. 把所有 WorkSta=2 的 修改未5 === 让重做的，修改未复检合格 */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=2 AND RefPK=" + this.WorkID);

                    /* 3. 把所有 WorkSta=1 的 修改未3  ====  让已完成的，设置未检查合格.  */
                    DBAccess.RunSQL("UPDATE ND101Dtl1 SET WorkSta=3 WHERE WorkSta=1 AND RefPK=" + this.WorkID);
               

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

        /// <summary>
        /// 流程结束之后
        /// </summary>
        /// <returns></returns>
        public override string FlowOverAfter()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            //检测每个该订单的每个子流程，是否完成，如果完成，就让订单流程，运动到打包封装节点。
            string sql = "SELECT COUNT(*) from WF_GenerWorkFlow WHERE PWorkID=" + gwf.PWorkID + " AND WFState!=3 ";
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (num > 0)
               
           return ""; //说明该订单未完成的箱体.

           // 求出来：生产装配的监督人员.
            sql = "SELECT FK_Emp from WF_GenerWorkerList WHERE WorkID=" + gwf.PWorkID + " AND FK_Node=202 AND IsPass=0 ";
            string worker = DBAccess.RunSQLReturnStringIsNull(sql, null);
            if (worker == null)
                throw new Exception("err@不应该查询不到，人员。");

            string currEmpNo = BP.Web.WebUser.No;

            BP.WF.Dev2Interface.Port_Login(worker);

            //让订单流程，运动到封装节点.
            BP.WF.Dev2Interface.Node_SendWork("002", gwf.PWorkID);

            //切换过来
            BP.WF.Dev2Interface.Port_Login(currEmpNo);


            return base.FlowOverAfter();
        }

    }
}
