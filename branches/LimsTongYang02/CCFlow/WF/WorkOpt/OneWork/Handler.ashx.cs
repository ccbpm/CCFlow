using System;
using System.Web;
using BP.DA;
using BP.En;
using BP.Web;
using System.Data;
using BP.WF;
using BP.WF.Template;
using BP.WF.XML;

namespace CCFlow.WF.WorkOpt.OneWork
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
                return typeof(BP.WF.HttpHandler.WF_WorkOpt_OneWork);
            }
        }
    }
}