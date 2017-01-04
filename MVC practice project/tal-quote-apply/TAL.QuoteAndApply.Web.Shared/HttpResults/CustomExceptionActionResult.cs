using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TAL.QuoteAndApply.Web.Shared.HttpResults
{
    public class CustomExceptionActionResult : IHttpActionResult
    {
        private readonly string _errorMessage;
        private readonly string _section;
        private readonly HttpStatusCode _statusCode;

        public CustomExceptionActionResult(HttpStatusCode statusCode, string errorMessage, string section)
        {
            _statusCode = statusCode;
            _errorMessage = errorMessage;
            _section = section;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ExecuteResult());
        }

        public HttpResponseMessage ExecuteResult()
        {
            var responseMessage = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_errorMessage),
                ReasonPhrase = _section
            };
            return responseMessage;
        }
    }
}