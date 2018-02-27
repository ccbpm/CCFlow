using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_CCForm : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public CCMobile_CCForm(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        public string HandlerMapExt()
        {
            WF_CCForm en = new WF_CCForm(this.context);
            return en.HandlerMapExt();
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.AttachmentUpload_Down();
        }
        /// <summary>
        /// 表单初始化.
        /// </summary>
        /// <returns></returns>
        public string Frm_Init()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.Frm_Init();
        }

        public string Dtl_Init()
        {
            WF_CCForm ccform = new WF_CCForm(this.context);
            return ccform.Dtl_Init();
        }
    }
}
