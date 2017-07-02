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
    /// 表单事件demo.
    /// </summary>
    public class CCFrm_ZhuBiaoShiJianCeShi : BP.Sys.FormEventBase
    {
        /// <summary>
        /// 标记该表单事件实体是要注册到那个类上
        /// </summary>
        public override string FormMark
        {
            get { return "CCFrm_ZhuBiaoShiJianCeShi"; }
        }

        public override string SaveBefore()
        {
            //取单价.
            decimal dj = this.HisEn.GetValMoneyByKey("DJ");
            //取数.
            decimal sl = this.HisEn.GetValMoneyByKey("SL");
            //计算合计.
            this.HisEn.SetValByKey("JE", dj * sl);

            return base.SaveBefore();
        }

        public override string SaveAfter()
        {
            return base.SaveAfter();
        }

        public override string AthUploadeBefore()
        {
            return base.AthUploadeBefore();
        }

        public override string AthUploadeAfter()
        {
            return base.AthUploadeAfter();
        }
    }
}
