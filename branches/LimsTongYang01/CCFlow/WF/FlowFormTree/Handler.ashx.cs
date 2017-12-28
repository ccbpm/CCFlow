using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.FlowFormTree
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : BP.WF.HttpHandler.HttpHandlerBase
    {

        /// 返回子类
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_FlowFormTree);
            }
        }
    }
}