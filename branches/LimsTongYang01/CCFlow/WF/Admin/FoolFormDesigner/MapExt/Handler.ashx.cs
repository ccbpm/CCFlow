using System;
using System.Web;
using System.Data;
using BP.Web;
using BP.Sys;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    /// <summary>
    /// MapExt 的摘要说明
    /// </summary>
    public class MapExtHandler : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 控件类型
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_Admin_FoolFormDesigner_MapExt);
            }
        }
    }
}
