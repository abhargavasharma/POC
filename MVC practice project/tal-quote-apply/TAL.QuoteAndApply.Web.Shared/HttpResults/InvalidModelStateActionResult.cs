using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Web.Shared.Extensions;

namespace TAL.QuoteAndApply.Web.Shared.HttpResults
{
    public class InvalidModelStateActionResult : IHttpActionResult
    {
        public ModelStateDictionary ModelState { get; set; }

        public InvalidModelStateActionResult(ModelStateDictionary modelState)
        {
            ModelState = modelState;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ExecuteResult());
        }

        public HttpResponseMessage ExecuteResult()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(ModelState.ToErrorDictionary().ToJson(), Encoding.UTF8, "application/json"),
            };
            return responseMessage;
        }
    }
}