using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.Admin.CCFormDesigner.DialogCtr
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 返回处理类型.
        /// </summary>
        public override Type CtrlType
        {
            get { 
                return typeof(BP.WF.HttpHandler.WF_Admin_CCFormDesigner_DialogCtr);
            }
        }
    }
}