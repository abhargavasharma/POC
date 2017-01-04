using System;
using System.Collections.Specialized;
using System.Monads;
using System.Web;
using System.Web.Mvc;
using TAL.QuoteAndApply.Web.Shared.Configuration;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class ApplicationStepValidationFilter : ActionFilterAttribute
    {
        public const string AppIdParamName = "dg.id";
        public const string IgnoreWorkFlowParamName = "ignore.workflow";
        public const string ClearSessionParamName = "new";

        private readonly IRedirectionActionProvider RedirectionActionProvider;
        private readonly IQuoteSessionContext CookieApplicationContext;
        private readonly IWebConfiguration Configuration;

        public string RedirectOnNoSession { get; set; }

        public ApplicationStepValidationFilter()
        {
            RedirectionActionProvider = DependencyResolver.Current.GetService<IRedirectionActionProvider>();
            CookieApplicationContext = DependencyResolver.Current.GetService<IQuoteSessionContext>();
            Configuration = DependencyResolver.Current.GetService<IWebConfiguration>();
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ClearSession(filterContext.HttpContext))
            {
                CookieApplicationContext.Clear();
                RemoveParamAndRedirect(filterContext, ClearSessionParamName);
                return;
            }

            if (Configuration.AllowOverrideParameter && SetApplicationId(filterContext.HttpContext))
            {
                RemoveParamAndRedirect(filterContext, AppIdParamName);
                return;
            }

            if (!CookieApplicationContext.HasValue())
            {
                filterContext.Result = new RedirectResult(RedirectOnNoSession ?? "/");
                return;
            }

            CookieApplicationContext.ExtendSession();

            if (IgnoreRedirection(filterContext.HttpContext))
            {
                return;
            }

            var url = filterContext.With(ctx => ctx.HttpContext).With(ctx => ctx.Request).With(req => req.Url);
            if (url == null)
                return;

            var redirectAction = RedirectionActionProvider.Get(url);
            if (redirectAction.ShouldIRedirect)
                filterContext.Result = new RedirectResult(redirectAction.RedirectUrl);
        }



        private NameValueCollection QueryStringParams(HttpContextBase context)
        {
            return HttpUtility.ParseQueryString(
                context.With(ctx => ctx.Request)
                    .With(req => req.Url)
                    .With(url => url.Query)
                    .CheckNullWithDefault(""));
        }

        private void RemoveParamAndRedirect(ActionExecutingContext filterContext, string key)
        {
            var url = filterContext.With(ctx => ctx.HttpContext).With(ctx => ctx.Request).With(req => req.Url);
            var urlBuilder = new UriBuilder(url)
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = -1
                };
            url = urlBuilder.Uri;
            
            var queryParams = HttpUtility.ParseQueryString(url.Query);
            queryParams.Remove(key);
            var pagePathWithoutQueryString = url.GetLeftPart(UriPartial.Path);

            var newUrl = queryParams.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, queryParams)
                : pagePathWithoutQueryString;

            filterContext.Result = new RedirectResult(newUrl);
        }

        public bool SetApplicationId(HttpContextBase context)
        {
            var keyValuePairs = QueryStringParams(context);
            var value = keyValuePairs[AppIdParamName];
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            CookieApplicationContext.Set(value);
            return true;
        }

        public bool IgnoreRedirection(HttpContextBase context)
        {
            if (!Configuration.AllowOverrideParameter)
            {
                return false;
            }

            var keyValuePairs = QueryStringParams(context);
            return keyValuePairs[IgnoreWorkFlowParamName] != null;
        }

        public bool ClearSession(HttpContextBase context)
        {
            var keyValuePairs = QueryStringParams(context);
            return keyValuePairs[ClearSessionParamName] != null &&
                   keyValuePairs[ClearSessionParamName].Equals("yes", StringComparison.OrdinalIgnoreCase);
        }
    }
}