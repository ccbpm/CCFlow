using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.En;
using BP.WF;
using BP.DA;
using BP.Web;
using BP.Port;
using BP.WF.Port;

namespace CCFlow.WF
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 获取 “Handler业务处理类”的Type
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF);
            }
        }
    }
}