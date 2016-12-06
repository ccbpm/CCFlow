using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;
using BP.Port;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// xxxxxx 流程事件实体
    /// </summary>
    public class FEETemplete : BP.WF.FlowEventBase
    {
        #region 构造.
        /// <summary>
        /// xxxxxx 流程事件实体
        /// </summary>
        public FEETemplete()
        {
        }
        #endregion 构造.

        #region 重写属性.
        public override string FlowMark
        {
            get { return "Templete"; }
        }
        #endregion 重写属性.

        #region 重写流程运动事件.
        /// <summary>
        /// 删除后
        /// </summary>
        /// <returns></returns>
        public override string AfterFlowDel()
        {
            return null;
        }
        /// <summary>
        /// 删除前
        /// </summary>
        /// <returns></returns>
        public override string BeforeFlowDel()
        {
            return null;
        }
        
        /// <summary>
        /// 结束后
        /// </summary>
        /// <returns></returns>
        public override string FlowOverAfter()
        {
            throw new Exception("@已经调用到了结束后事件了.");
            return null;
        }
        /// <summary>
        /// 结束前
        /// </summary>
        /// <returns></returns>
        public override string FlowOverBefore()
        {
            return null;
        }
        #endregion 重写流程运动事件

        #region 节点表单事件
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
        #endregion


        #region 重写节点运动事件.

        public override string SaveBefore()
        {
            return null;
        }
        public override string SaveAfter()
        {
            return null;
        }
        public override string SendWhen()
        {
            return null;
        }
        public override string SendSuccess()
        {
            return null;
        }
        public override string SendError()
        {
            return null;
        }
        public override string ReturnAfter()
        {
            return null;
        }
        public override string ReturnBefore()
        {
            return null;
        }
        public override string UndoneAfter()
        {
            return null;
        }
        public override string UndoneBefore()
        {
            return null;
        }
        #endregion 重写节点运动事件
    }
}
