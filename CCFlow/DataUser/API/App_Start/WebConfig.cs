
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace CCFlow.DataUser.API
{
    public class SessionRouteHandler : HttpControllerHandler, IRequiresSessionState
    {
        public SessionRouteHandler(RouteData routeData) : base(routeData)
        {
        }
    }

    public class SessionControllerRouteHandler : HttpControllerRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new SessionRouteHandler(requestContext.RouteData);
        }
    }
    public class WebConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //清除XML返回格式            
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));
            RouteTable.Routes.MapHttpRoute(
              name: "Default",
              routeTemplate: "WF/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }).RouteHandler = new SessionControllerRouteHandler();

        }
    }
}

