using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.ServiceLayer.Data;

namespace TAL.QuoteAndApply.SalesPortal.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            IocConfig.RegisterForWeb(GlobalConfiguration.Configuration);
            ClassMapRegistration.SafeRegisterAllClassMapsFromAssemblies("TAL.QuoteAndApply");

            log4net.Config.XmlConfigurator.Configure();

        }
        
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            var operations = DependencyResolver.Current.GetService<IEndRequestOperationCollection>();
            operations.ExecuteTasks();
        }
    }
}
