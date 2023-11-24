using System;
using System.Data;
using BP.DA;
using BP.WF;

namespace BP.Demo
{
    /// <summary>
    /// 报销流程001
    /// 0. 此类库必须放入到 BP.*.dll 才能被解析发射出来.
    /// 1. 一定被发现后才能起作用.
    /// </summary>
    public class F001 : BP.WF.FlowEventBase
    {
        #region 属性.
        /// <summary>
        /// 重写流程标记,点流程属性.
        /// </summary>
        public override string FlowMark
        {
            get { return ",001,002,003,004,006,005,"; }
        }
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F001()
        {
        }
        public override string FrmLoadAfter()
        {

            return base.FrmLoadAfter();
        }
        #endregion 属性.

        #region 发送事件.
        /// <summary>
        /// 重写发送前事件
        /// </summary>
        /// <returns></returns>
        public override string SendWhen()
        {
            //if (this.HisNode.NodeID == 107)
            //{
            //    //判断是否有bomcode 或者物料数据，如果没有，就不让其下发.
            //    string sql = "SELECT COUNT(*) AS SNU FROM ND101Dtl1 WHERE RefPK="+this.OID;
            //    if (DBAccess.RunSQLReturnValInt(sql) == 0)
            //        throw new Exception("@电气柜，没有导入或者设置BOM信息。");
            //}
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
                if (1 == 1)
                { 
                }

                //// 组织必要的变量.
                //Int64 workid = this.WorkID; // 工作id.w
                //string flowNo = this.HisNode.FK_Flow; // 流程编号.
                //int currNodeID = this.SendReturnObjs.VarCurrNodeID; //当前节点id
                //int toNodeID = this.SendReturnObjs.VarToNodeID;     //到达节点id.
                //string toNodeName = this.SendReturnObjs.VarToNodeName; // 到达节点名称。
                //string acceptersID = this.SendReturnObjs.VarAcceptersID; // 接受人员id, 多个人员会用 逗号分看 ,比如 zhangsan,lisi。
                //string acceptersName = this.SendReturnObjs.VarAcceptersName; // 接受人员名称，多个人员会用逗号分开比如:张三,李四.

                //DBAccess.RunSQL("UPDATE ND1Rpt SET TodoEmps='" + this.HisGenerWorkFlow.TodoEmps + "' WHERE OID=" + this.WorkID);

                //// 实时更新项目的数量.
                ////ND2RptExt ext = new ND2RptExt(this.HisGenerWorkFlow.PWorkID);
                ////ext.ResetNum();
                //DBAccess.RunSQL("UPDATE nd1rpt set  CCRQ=QiXianXianDing WHERE CCRQ='' OR  CCRQ IS NULL ");

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

            ////设置完成状态.
            //ND1Rpt rpt = new ND1Rpt(this.WorkID);
            //if (rpt.QiXianXianDingDT >= DateTime.Now)
            //{
            //    rpt.SetValByKey("SXSta", 3);
            //}
            //else
            //{
            //    rpt.SetValByKey("SXSta", 4);
            //}
            //rpt.Update();

            //GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            ////检测每个该订单的每个子流程，是否完成，如果完成，就让订单流程，运动到打包封装节点。
            //string sql = "SELECT COUNT(*) FROM WF_GenerWorkFlow WHERE PWorkID=" + gwf.PWorkID + " AND WFState!=3 ";
            //int num = DBAccess.RunSQLReturnValInt(sql);
            //if (num > 0)
            //    return ""; //说明该订单未完成的电气柜.

            //// 求出来：生产装配的监督人员.
            //sql = "SELECT FK_Emp FROM WF_GenerWorkerList WHERE WorkID=" + gwf.PWorkID + " AND FK_Node=202 AND IsPass=0 ";
            //string worker = DBAccess.RunSQLReturnStringIsNull(sql, null);
            //if (worker == null)
            //    throw new Exception("err@不应该查询不到，人员。");

            //string currEmpNo = BP.Web.WebUser.No;

            //BP.WF.Dev2Interface.Port_Login(worker);

            ////让订单流程，运动到封装节点.
            //BP.WF.Dev2Interface.Node_SendWork("002", gwf.PWorkID);

            ////切换过来
            //BP.WF.Dev2Interface.Port_Login(currEmpNo);


            return base.FlowOverAfter();
        }

    }
}
