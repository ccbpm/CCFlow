using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.RptSearch
{
    /// <summary>
    /// JQFileUpload 的摘要说明
    /// </summary>
    public class Handler : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 返回子类 
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_RptSearch);
            }
        }
    }
}