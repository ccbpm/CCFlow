using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.Admin.FoolFormDesigner.Rpt
{
    /// <summary>
    /// Handler1 的摘要说明
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
                return typeof(BP.WF.HttpHandler.WF_Admin_RptDfine);
            }
        }
    }
}