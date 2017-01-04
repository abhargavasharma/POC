using System.Web.Mvc;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Web.Shared.Loggers
{
    public class MvcExceptionLoggingAttribute : HandleErrorAttribute
    {
        public ILoggingService LoggingService { get; set; }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                LoggingService.Error(filterContext.Exception);
            }

            base.OnException(filterContext);
        }
    }
}