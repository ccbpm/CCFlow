using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using BP.WF.Data;
using BP.Sys;

namespace CCFlow.WF
{
    /// <summary>
    /// MyFlow 的摘要说明
    /// </summary>
    public class MyFlow : BP.WF.HttpHandler.HttpHandlerBase
    {
        public override Type CtrlType
        {
            get
            {
                
                return typeof(BP.WF.HttpHandler.WF_MyFlow);
            }
        }
    }
}