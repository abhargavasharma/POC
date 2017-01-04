using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Autofac.Integration.WebApi;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.Attributes
{
    public class MvcQuoteSessionRequiredAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public IQuoteSessionContext QuoteSessionContext { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!QuoteSessionContext.HasValue())
            {
                filterContext.Result = new RedirectResult("/");
                return;
            }

            QuoteSessionContext.ExtendSession();

            base.OnActionExecuting(filterContext);
        }
    }

    public class WebApiQuoteSessionRequiredAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute, IAutofacAuthorizationFilter
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var requestScope = actionContext.Request.GetDependencyScope();
            var quoteSessionContext = requestScope.GetService(typeof(IQuoteSessionContext)) as IQuoteSessionContext;


            if (quoteSessionContext == null || !quoteSessionContext.HasValue())
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            quoteSessionContext.ExtendSession();

            base.OnAuthorization(actionContext);
        }
    }

}