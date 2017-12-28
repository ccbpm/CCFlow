using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using Newtonsoft.Json.Converters;

namespace CCFlow.WF.Admin.FoolFormDesigner.DtlSetting
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
                return typeof(BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_DtlSetting);
            }
        }
    }

}