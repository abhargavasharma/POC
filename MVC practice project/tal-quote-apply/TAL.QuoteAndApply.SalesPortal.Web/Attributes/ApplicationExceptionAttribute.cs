using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Web.Http.Filters;
using TAL.QuoteAndApply.Infrastructure.Http.Exceptions;
using TAL.QuoteAndApply.Infrastructure.Logging;


namespace TAL.QuoteAndApply.SalesPortal.Web.Attributes
{
    public class ApplicationExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly string _errorMessage;
        private readonly string _section;
        private readonly HttpStatusCode _statusCode;        

        public ApplicationExceptionAttribute(HttpStatusCode statusCode, string errorMessage = "Server error", string section = "")
        {
            _statusCode = statusCode;
            _errorMessage = errorMessage;
            _section = section;
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            //only handle ApplicationException only
            if (context.Exception is ApplicationException || context.Exception is ApiException)
            {
                context.Response = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_errorMessage),
                    ReasonPhrase = _section
                };

                var log = DependencyResolver.Current.GetService<ILoggingService>();
                log.Error(context.Exception);
            }
        }
    }
}