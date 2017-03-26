using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Text;
using System.Configuration;
using System.Web.SessionState;
using BP.DA;
using BP.Web;
using BP.Sys;
using BP.En;
using BP.WF.Template;
using System.Collections.Generic;
using BP.WF;
using LitJson;

namespace CCFlow.WF.Admin.CCFormDesigner.common
{
    /// <summary>
    /// FormDesignerController 的摘要说明
    /// by dgq FormDesiner service
    /// </summary>
    public class FormDesignerController :  BP.WF.HttpHandler.HttpHandlerBase
    {
        public override Type CtrlType
        {
            get
            {
                return typeof(BP.WF.HttpHandler.WF_Admin_CCFormDesignerCCFromHandler);
            }
        }
    }
}