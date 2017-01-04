using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.ServiceLayer.Data;

namespace TAL.QuoteAndApply.Customer.Web
{
    public class CustomerApplication : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            AreaRegistration.RegisterAllAreas();
            GlobalFilters.Filters.Add(new GlobalViewBagFilter(), 0);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
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

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(
                System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}