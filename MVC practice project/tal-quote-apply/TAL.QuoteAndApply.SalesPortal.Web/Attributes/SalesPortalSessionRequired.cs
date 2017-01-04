
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Autofac.Integration.WebApi;
using TAL.QuoteAndApply.SalesPortal.Web.User;

namespace TAL.QuoteAndApply.SalesPortal.Web.Attributes
{
    public class MvcSalesPortalSessionRequiredAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public ISalesPortalSessionContext SalesPortalSessionContext { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!SalesPortalSessionContext.HasValue())
            {
                filterContext.Result = new RedirectResult("/Home/Login?timeout=true");
                return;
            }

            SalesPortalSessionContext.ExtendSession();

            base.OnActionExecuting(filterContext);
        }
    }

    public class WebApiSalesPortalSessionRequiredAttribute : System.Web.Http.Filters.ActionFilterAttribute, IAutofacActionFilter
    {

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var requestScope = actionContext.Request.GetDependencyScope();
            var salesPortalSessionContext = requestScope.GetService(typeof(ISalesPortalSessionContext)) as ISalesPortalSessionContext;

            if (salesPortalSessionContext == null || !salesPortalSessionContext.HasValue())
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return;
            }

            salesPortalSessionContext.ExtendSession();

            base.OnActionExecuting(actionContext);
        }

    }
}