using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;
using BP.Port;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// 报销流程 - 开始节点.
    /// </summary>
    public class ND101 : BP.WF.FlowEventBase
    {
        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public ND101()
        {
        }
        #endregion 属性.

        #region 重写属性.
        public override string FlowMark
        {
            get { return "QingJia"; }
        }
        #endregion 重写属性.

        #region 重写节点表单事件.
        /// <summary>
        /// 表单载入前
        /// </summary>
        public override string FrmLoadAfter()
        {
            return null;
        }
        /// <summary>
        /// 表单载入后
        /// </summary>
        public override string FrmLoadBefore()
        {
            return null;
        }
        /// <summary>
        /// 表单保存后
        /// </summary>
        public override string SaveAfter()
        {
            return null;
        }
        /// <summary>
        /// 表单保存前
        /// </summary>
        public override string SaveBefore()
        {
            if (this.HisNode.NodeID == 101)
            {
                //更新合计小写.
                string sql = "UPDATE ND101 SET HeJi=(SELECT SUM(XiaoJi) FROM ND101Dtl1 WHERE RefPK=" + this.OID + ") WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                //把合计转化成大写.
                float hj = BP.DA.DBAccess.RunSQLReturnValFloat("SELECT HeJi FROM ND101 WHERE OID=" + this.OID, 0);

                sql = "UPDATE ND101 SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "' WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                return null;
            }
            return null;
        }
        #endregion 重写节点表单事件

        #region 重写节点运动事件.
        /// <summary>
        /// 发送前:用于检查业务逻辑是否可以执行发送，不能执行发送就抛出异常.
        /// </summary>
        public override string SendWhen()
        {
            if (this.HisNode.NodeID == 101)
            {
                //更新合计小写.
                string sql = "UPDATE ND101 SET HeJi=(SELECT SUM(XiaoJi) FROM ND101Dtl1 WHERE RefPK=" + this.OID + ") WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                //把合计转化成大写.
                float hj = BP.DA.DBAccess.RunSQLReturnValFloat("SELECT HeJi FROM ND101 WHERE OID=" + this.OID, 0);
                if (hj == 0)
                    throw new Exception("@你需要输入费用明细项目.");

                sql = "UPDATE ND101 SET DaXie='" + BP.DA.DataType.ParseFloatToCash(hj) + "' WHERE OID=" + this.OID;
                BP.DA.DBAccess.RunSQL(sql);
                return "合计已经在发送前事件完成.";
            }

            return null;
        }
        /// <summary>
        /// 发送成功后
        /// </summary>
        public override string SendSuccess()
        {
            return null;
        }
        /// <summary>
        /// 发送失败后
        /// </summary>
        public override string SendError()
        {
            return null;
        }
        /// <summary>
        /// 退回前
        /// </summary>
        public override string ReturnBefore()
        {
            return null;
        }
        /// <summary>
        /// 退回后
        /// </summary>
        public override string ReturnAfter()
        {
            return null;
        }
        #endregion 重写事件，完成业务逻辑.
    }
}
