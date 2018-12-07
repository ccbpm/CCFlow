using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using BP.WF;
using BP.Sys;
using BP.En;
using BP.DA;
using BP.WF.Template;

namespace CCFlow.WF.Admin.Sln
{
    public class Handler : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 获取 “Handler业务处理类”的Type
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_Admin_Sln);
            }
        }
    }

}