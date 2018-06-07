using System;
using System.IO;
using System.Data;
using System.Web.UI.WebControls;
using BP.Web;
//using BP.Rpt ; 
using BP.DA;
using BP.En;
using System.Reflection;
using System.Text.RegularExpressions;


namespace BP.Web
{
    /// <summary>
    /// PortalPage 的摘要说明。
    /// </summary>
    public class WebPageAdmin : WebPageSession
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
           
            if (WebUser.No != "admin")
                throw new Exception("非法的管理员用户。");
        }
    }
}

