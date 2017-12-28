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
    public class F104:BP.Sys.EventBase
    {
        #region 属性.
        #endregion 属性.

        #region 构造.
        /// <summary>
        /// 报销流程事件
        /// </summary>
        public F104()
        {
            this.Title = "请假流程";
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
                case 10401: //填写报销申请单节点.
                case 10402: //填写报销申请单节点.
                case 10403: //填写报销申请单节点.
                    switch (this.EventType)
                    {
                        case EventListOfNode.FrmLoadBefore: //表单保存后事件.
                            break;
                        case EventListOfNode.SaveBefore: //表单保存前事件.
                            SendWhen10401();
                            break;
                        case EventListOfNode.SendWhen: //发送前.
                            SendWhen10401();
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
        /// 执行发送前事件
        /// </summary>
        public void SendWhen10401()
        {
            DateTime dtFrom = this.HisEn.GetValDateTime("QingJiaShiJianCong");
            DateTime dtTo = this.HisEn.GetValDateTime("Dao");

            if (dtFrom > dtTo)
                throw new Exception("请假时间到不能小于请假时间从.");

            //求出请假天数.
            TimeSpan ts = dtTo - dtFrom;
            float span = ts.Days;
            this.HisEn.SetValByKey("QingJiaTianShu", span);

            //设置到数据源中,让其直接更新到数据源，如果YuE字段是可以编辑的这个步骤就没有必要.
            string sql = "UPDATE ND10401 SET QingJiaTianShu=" + span + " WHERE OID=" + this.OID;
            BP.DA.DBAccess.RunSQL(sql);
        }
    }
}
