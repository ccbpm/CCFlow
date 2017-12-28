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

namespace BP.Demo.FormEvent
{
    /// <summary>
    /// 从表的表单事件 demo.
    /// </summary>
    public class Demo_TastCaseEventFrmDtl2 : BP.Sys.FormEventBaseDtl
    {
        /// <summary>
        /// 标记(多个从表的ID，可以用逗号分开比如: "ABCDtl1,ABCDtl1" )
        /// </summary>
        public override string FormDtlMark
        {
            get { return "Demo_TastCaseEventFrmDtl2"; }
        }

        public override string BeforeFormDel()
        {
            return base.BeforeFormDel();
        }
        /// <summary>
        /// 保存前的动作.
        /// </summary>
        /// <returns></returns>
        public override string RowSaveBefore()
        {
            decimal danjia = this.HisEnDtl.GetValDecimalByKey("DanJia");
            decimal shuliang = this.HisEnDtl.GetValDecimalByKey("ShuLiang");


            this.HisEnDtl.SetValByKey("XiaoJi", danjia * shuliang);

            return base.RowSaveBefore();
        }
    }
}
