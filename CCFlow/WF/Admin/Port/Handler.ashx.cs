using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCFlow.WF.Admin.Port
{
    /// <summary>
    /// 集成基类，业务逻辑都在子类
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
                return typeof(BP.WF.HttpHandler.WF_Admin_Port);
            }
        }
    }
}