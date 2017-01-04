using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api.Results
{
    public class InvalidBeneficiariesActionResult : IHttpActionResult
    {
        private readonly ICollection<RiskBeneficiaryValidationModel> _validationResult;

        public InvalidBeneficiariesActionResult(ICollection<RiskBeneficiaryValidationModel> validationResult)
        {
            _validationResult = validationResult;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ExecuteResult());
        }

        public HttpResponseMessage ExecuteResult()
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(_validationResult, Formatting.Indented, jsonSerializerSettings), Encoding.UTF8, "application/json"),
            };
            return responseMessage;
        }
    }
}