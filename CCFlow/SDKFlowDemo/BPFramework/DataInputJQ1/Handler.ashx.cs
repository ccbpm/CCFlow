using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;

namespace CCFlow.SDKFlowDemo.BPFramework.DataInputJQ
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
                return typeof(BP.Demo.HandlerPage.SDKFlowDemo_BPFramework_DataInputJQ);
            }
        }
    }
}