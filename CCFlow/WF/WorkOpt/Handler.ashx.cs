using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace CCFlow.WF.WorkOpt
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
                

                return typeof(BP.WF.HttpHandler.WF_WorkOpt);
            }
        }
    }
}