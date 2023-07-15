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
            //AreaRegistration.RegisterAllAreas();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            CCFlow.DataUser.API.WebConfig.Register(GlobalConfiguration.Configuration);
            //SwaggerConfig
            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
           
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //System.ServiceModel.Activation.ServiceRoute serviceRoute = new System.ServiceModel.Activation.ServiceRoute("CCBPMServices", new System.ServiceModel.Activation.WebServiceHostFactory(), typeof(Services));
            //System.Web.Routing.RouteTable.Routes.Add(serviceRoute);
            //RouteTable.Routes.Add(new ServiceRoute("Services", new WebServiceHostFactory(), typeof(CCFlow.DataUser.Services)));
            // BP.Sys.GroupFields
            //  BP.WF.Dev2Interface.Node_SetDraft("1111", 111111);
            // BP.GPM.Home.WinDocModel.ChatPie
            //  BP.DA.DBAccess.RunSQL();
            //BP.GPM.Menu2020.Menu en = new BP.GPM.Menu2020.Menu("332");
            //en.Name = "sss";
            //en.Update();
            //   BP.WF.Template.FrmWorkCheck fwc = new BP.WF.Template.FrmWorkCheck();
            //
            //DataTable dt = BP.WF.Dev2Interface.DB_Start();
            //DataTable dt = BP.WF.Dev2Interface.DB_Start();
            //DataTable dt = BP.WF.Dev2Interface.DB_Start();
            //DataTable dt = BP.WF.Dev2Interface.DB_Start();

            //  BP.Cloud.Emp EMP = new BP.Cloud.Emp();
            //   ReadEntityBodyMod
            //   BP.Cloud.Dev2Interface.Port_Login()
            //   BP.WF.Dev2Interface.CCForm_AddAth( )
            // BP.DA.DBAccess.RunSQLReturnTable()
            // BP.GPM.Menu2020.Module module = new BP.GPM.Menu2020.Module();
            // BP.CCBill.Template.GroupMethod gm = new BP.CCBill.Template.GroupMethod();

            // BP.Sys.MapData md = new BP.Sys.MapData();
            // md.Delete
            //   BP.WF.Template.SysFormTree
            // BP.WF.Node nd = new BP.WF.Node();
            // nd.HisDeliveryWay
            //  BP.WF.Template.SysFormTree
            //     BP.WF.Dev2Interface.DB_StarFlows(); 

            //登录方法.
            // BP.WF.Dev2Interface.Port_Login("zhangsan");


            //退出方法.
            //  BP.WF.Dev2Interface.Port_SigOut(); //  ("zhangsan");


            // BP.WF.Template.NodeSheet
            // BP.WF.Template.NodeEmp
            // BP.WF.DeliveryWay.ByBindEmp
            // BP.Sys.GEEntity en = new BP.Sys.GEEntity("Frm_ABC", 1001);
            // BP.En.GENoName

            //  BP.WF.Dev2Interface.Port_Login("zhangsan");
            // BP.Cloud.OrgExts
            //BP.Sys.EnCfg
            // BP.Sys.FrmUI.MapAttrDocWord
            // 在应用程序启动时运行的代码
            // BP.WF.Glo.UpdataCCFlowVer();
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
