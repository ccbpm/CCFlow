using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.En;
using BP.Sys;

namespace CCFlow.WF.Comm
{
    /// <summary>
    /// 实例说明：
    /// 1.Handler.ashx.cs只需继承BP.WF.HttpHandler.HttpHandlerBase类，实现CtrlType属性，返回此“Handler业务处理类”的Type；
    /// 2.“Handler业务处理类”必须继承自BP.WF.HttpHandler.WebControlBase类，必须声明含有1个HttpContext类型参数的构造函数；
    /// 3.“Handler业务处理类”中编写JS端要调用的业务逻辑方法，JS调用此方法时，传递DoType=该业务逻辑方法名即可；
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
                return typeof(BP.WF.HttpHandler.WF_Comm);
            }
        }
    }
}