using System;
using System.Web;
using System.Web.SessionState;
using System.Web.Http;

namespace ccflowSite
{
    public class Global : System.Web.HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        { 
            CCFlow.DataUser.API.WebConfig.Register(GlobalConfiguration.Configuration);
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);

        }

        void Application_End(object sender, EventArgs e)
        {
            //  在应用程序关闭时运行的代码

        }

        void Application_Error(object sender, EventArgs e)
        {
            // 在出现未处理的错误时运行的代码
        }

        void Session_Start(object sender, EventArgs e)
        {
            // 在新会话启动时运行的代码。因为在此运行系统刷新时webuser.no会改变
            // 将此方法删除移动到RegisterAdminer_Submit()里面去
            //BP.WF.Dev2Interface.Port_Login("Guest");
        }

        void Session_End(object sender, EventArgs e)
        {
            // 在会话结束时运行的代码。 
            // 注意: 只有在 Web.config 文件中的 sessionstate 模式设置为.
            // InProc 时，才会引发 Session_End 事件。如果会话模式设置为 StateServer 
            // 或 SQLServer，则不会引发该事件。
        }

    }
}
