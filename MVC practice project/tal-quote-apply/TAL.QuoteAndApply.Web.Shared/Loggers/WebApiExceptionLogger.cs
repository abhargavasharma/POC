using System;
using System.Web.Http.ExceptionHandling;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Web.Shared.Loggers
{
    public class WebApiExceptionLogger : ExceptionLogger
    {
        private readonly ILoggingService _log;

        public WebApiExceptionLogger(ILoggingService log)
        {
            _log = log;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            // When the framework calls an exception logger or an exception handler, it will always provide an Exception and a Request.
            // http://aspnetwebstack.codeplex.com/wikipage?title=Global%20Error%20Handling&referringTitle=Specs
            if (context.Exception == null)
            {
                throw new ArgumentException("context.Exception is null", "context");
            }
            if (context.Request == null)
            {
                throw new ArgumentException("context.Request is null", "context");
            }

            _log.Error(context.Exception);
        }
    }
}