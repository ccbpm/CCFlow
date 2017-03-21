using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using BP.Sys;
using System.IO;
using BP.Web;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using BP.DA;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using CCFlow.WF.CCForm;


namespace CCFlow.WF.CCForm
{
    /// <summary>
    /// JQFileUpload 的摘要说明
    /// </summary>
    public class CCFormHeader : BP.WF.HttpHandler.HttpHandlerBase
    {
        /// <summary>
        /// 返回子类
        /// </summary>
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_CCForm);
            }
        }
    }
}