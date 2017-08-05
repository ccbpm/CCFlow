using System;
using System.Threading;
using System.Collections;
using BP.Web.Controls;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF;

namespace BP.Demo.FlowEvent
{
    /// <summary>
    /// 报销流程001
    /// </summary>
    public class F025 : BP.Sys.EventBase
    {
        #region 属性.
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F025()
        {
            this.Title = "报销流程";
        }
        #endregion 属性.

        /// <summary>
        /// 执行事件
        /// 1，如果遇到错误就抛出异常信息，前台界面就会提示错误并不向下执行。
        /// 2，执行成功，把执行的结果赋给SucessInfo变量，如果不需要提示就赋值为空或者为null。
        /// 3，所有的参数都可以从  this.SysPara.GetValByKey 中获取。
        /// </summary>
        public override void Do()
        {
            switch (this.FK_Node)
            {
                case 2501: //填写报销申请单节点.
                    switch (this.EventType)
                    {
                        case EventListOfNode.FrmLoadBefore: //表单保存后事件.
                            this.ND2501_FrmLoadBefore();
                            break;
                        case EventListOfNode.SendWhen: //发送前.
                            this.ND2501_SendWhen();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 装载前事件
        /// 方法命名规则为:ND+节点名_事件名.
        /// </summary>
        public void ND2501_FrmLoadBefore()
        {
            // 求余额. 
            int dtlOID = this.GetValInt("ItemNo");
            int Totalmoney = this.GetValInt("Totalmoney");

            string sql = "select sum(BXJE) as Num from ND25Rpt WHERE ItemNo='" + dtlOID + "' AND WFState!=0";
            decimal bxjeSum = BP.DA.DBAccess.RunSQLReturnValDecimal(sql, 0, 1);

            decimal ye = Totalmoney - bxjeSum;

            /*求出余额后，需要做两个操作.*/

            //设置到实体中，让其可以显示到表单上.
            this.HisEn.SetValByKey("YuE", ye);

            //设置到数据源中,让其直接更新到数据源，如果YuE字段是可以编辑的这个步骤就没有必要.
            sql = "UPDATE ND2501 SET YuE=" + ye + " WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 发送前
        /// 方法命名规则为:ND+节点名_事件名.
        /// </summary>
        public void ND2501_SendWhen()
        {
            throw new Exception("@您报销金额已经超过了预算。");
        }
    }
}
