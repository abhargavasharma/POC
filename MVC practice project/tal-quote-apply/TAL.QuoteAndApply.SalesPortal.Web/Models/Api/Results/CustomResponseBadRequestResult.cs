using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Results
{
    public class CustomResponseBadRequestResult<T> : IHttpActionResult
    {
        public T Model { get; set; }

        public CustomResponseBadRequestResult(T modelState)
        {
            Model = modelState;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ExecuteResult());
        }

        public HttpResponseMessage ExecuteResult()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(Model.ToJson(), Encoding.UTF8, "application/json"),
            };
            return responseMessage;
        }
    }
}