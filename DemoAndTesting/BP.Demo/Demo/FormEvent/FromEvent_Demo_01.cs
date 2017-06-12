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
    public class FromEvent_Demo_01 : BP.Sys.FormEventBase
    {
        /// <summary>
        /// 标记该表单事件实体是要注册到那个类上
        /// </summary>
        public override string FormMark
        {
            get { return "Demo_01"; }
        }

        public override string SaveBefore()
        {
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
