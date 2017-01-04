using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Web.Shared.HttpResults
{
    public class RedirectActionResult : IHttpActionResult
    {
        public string Url { get; set; }
        
        public RedirectActionResult(string url)
        {
            Url = url;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ExecuteResult());
        }

        public HttpResponseMessage ExecuteResult()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(new RedirectToResponse() { RedirectTo = Url}.ToJson(), Encoding.UTF8, "application/json"),
            };
            return responseMessage;
        }
    }

    public class RedirectToResponse
    {
        public string RedirectTo { get; set; }
    }
}